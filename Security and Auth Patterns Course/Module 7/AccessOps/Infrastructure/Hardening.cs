using System;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using AccessOps.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>The "Hardening" section of appsettings.json. Production values are the defaults.</summary>
    public sealed class HardeningOptions
    {
        /// <summary>Redirect HTTP to HTTPS and send HSTS. Off only in appsettings.Development.json (the lab runs on http://localhost).</summary>
        public bool RequireHttps { get; set; } = true;

        /// <summary>The port the redirect sends the browser to.</summary>
        public int HttpsPort { get; set; } = 443;

        public int HstsMaxAgeDays { get; set; } = 365;

        /// <summary>Only once every subdomain serves TLS.</summary>
        public bool HstsIncludeSubDomains { get; set; }

        /// <summary>"ReportOnly" until every screen and the share link load clean, then "Enforce".</summary>
        public string CspMode { get; set; } = "Enforce";

        /// <summary>Share link: a token bucket per client address.</summary>
        public int ShareLinkBurst { get; set; } = 10;
        public int ShareLinkPerMinute { get; set; } = 5;

        /// <summary>Password sign-in: a token bucket per user name and client address, taken before the hash.</summary>
        public int LoginBurst { get; set; } = 5;
        public int LoginRefillSeconds { get; set; } = 12;

        public bool CspReportOnly => string.Equals(this.CspMode, "ReportOnly", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Module 7: everything the host owns - TLS and HSTS, the response headers, cookie flags, request
    /// limits and the share-link rate limit - in one place, registered in Startup.cs before any
    /// endpoint and before app.UseWisej(), so every response carries it: the Wisej.NET pages, the
    /// start page, the OIDC callback and the share-link URL.
    /// </summary>
    public static class Hardening
    {
        public const string ShareLinkPolicy = "share-link";
        public const string CspReportPolicy = "csp-report";

        /// <summary>
        /// The Content-Security-Policy the Wisej.NET client can run under: it evaluates script and
        /// writes inline styles, and Default.html carries two small inline scripts, so
        /// 'unsafe-inline' 'unsafe-eval' stay in script-src. frame-ancestors 'none' refuses framing
        /// (no click-jacked Approve button); connect-src 'self' covers the same-origin WebSocket.
        /// </summary>
        public const string ContentSecurityPolicy =
            "default-src 'self'; " +
            "script-src 'unsafe-inline' 'unsafe-eval' 'self'; " +
            "style-src 'unsafe-inline' 'self'; " +
            "img-src 'self' data:; " +
            "font-src 'self' data:; " +
            "connect-src 'self'; " +
            "object-src 'none'; " +
            "base-uri 'self'; " +
            "form-action 'self'; " +
            "frame-ancestors 'none'; " +
            "report-uri " + CspReportPath;

        /// <summary>Where browsers post the violations - read them before switching report-only to enforce.</summary>
        public const string CspReportPath = "/csp-report";

        public static HardeningOptions AddAccessOpsHardening(this WebApplicationBuilder builder)
        {
            var options = builder.Configuration.GetSection("Hardening").Get<HardeningOptions>() ?? new HardeningOptions();
            builder.Services.AddSingleton(options);

            // Request limits at the host. The body limit itself is set beside Upload.MaxFileSize in Startup.
            builder.WebHost.ConfigureKestrel(k =>
            {
                k.AddServerHeader = false;                                   // no "Server: Kestrel"
                k.Limits.MaxRequestHeadersTotalSize = 32 * 1024;
                k.Limits.MaxRequestLineSize = 8 * 1024;
                k.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
                k.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            });

            // The client address and scheme behind a reverse proxy: trusted only from the proxies named
            // in Hosting:KnownProxies (loopback by default). Without it, behind a TLS-terminating proxy
            // the app believes it is on HTTP and its cookies go out without Secure.
            builder.Services.Configure<ForwardedHeadersOptions>(o =>
            {
                o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                foreach (var proxy in builder.Configuration.GetSection("Hosting:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
                    o.KnownProxies.Add(System.Net.IPAddress.Parse(proxy));
            });

            if (options.RequireHttps)
            {
                builder.Services.AddHttpsRedirection(o =>
                {
                    o.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                    o.HttpsPort = options.HttpsPort;
                });
                builder.Services.AddHsts(o =>
                {
                    o.MaxAge = TimeSpan.FromDays(options.HstsMaxAgeDays);
                    o.IncludeSubDomains = options.HstsIncludeSubDomains;
                    o.Preload = false;
                });
            }

            // Every cookie ASP.NET Core writes (the OIDC correlation and nonce cookies, the hand-off
            // cookie): HttpOnly always, Secure always once the site is HTTPS-only, never SameSite=None.
            builder.Services.Configure<CookiePolicyOptions>(o =>
            {
                o.HttpOnly = HttpOnlyPolicy.Always;
                o.Secure = options.RequireHttps ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                o.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            // The share link is a real URL, so middleware can limit it: a token bucket per client
            // address, 429 when it is empty, and every refusal audited under its own correlation ID.
            builder.Services.AddRateLimiter(r =>
            {
                r.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                r.AddPolicy(ShareLinkPolicy, http => RateLimitPartition.GetTokenBucketLimiter(
                    http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = options.ShareLinkBurst,
                        TokensPerPeriod = options.ShareLinkPerMinute,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));
                // a page load can report a handful of violations at once; a flood from one address cannot fill the log
                r.AddPolicy(CspReportPolicy, http => RateLimitPartition.GetFixedWindowLimiter(
                    http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    _ => new FixedWindowRateLimiterOptions { PermitLimit = 60, Window = TimeSpan.FromMinutes(1), QueueLimit = 0 }));
                r.OnRejected = (context, _) =>
                {
                    var http = context.HttpContext;
                    if (http.Request.Path.StartsWithSegments(CspReportPath))
                        return ValueTask.CompletedTask;     // dropped reports are not an audit event
                    var address = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var command = CommandContext.Anonymous("share-link", address, http.Request.Headers.UserAgent.ToString());
                    var log = http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AccessOps.RateLimit");
                    using (log.BeginScope(command.Scope()))
                        log.LogWarning("Share link rate limit reached for {Address} on {Path}", address, http.Request.Path.Value);
                    var target = http.Request.RouteValues.TryGetValue("id", out var id) ? id?.ToString() : "-";
                    http.RequestServices.GetRequiredService<AuditService>().Record(command, "Document.ShareOpen", "Document", target, "RateLimited");
                    return ValueTask.CompletedTask;
                };
            });

            return options;
        }

        public static void UseAccessOpsHardening(this WebApplication app)
        {
            var options = app.Services.GetRequiredService<HardeningOptions>();

            app.UseForwardedHeaders();                   // the proxy terminated TLS, not us

            if (options.RequireHttps)
            {
                app.UseHsts();                           // Strict-Transport-Security (never sent for localhost)
                app.UseHttpsRedirection();
            }

            app.Use(async (context, next) =>
            {
                var h = context.Response.Headers;
                h["X-Content-Type-Options"] = "nosniff";
                h["Referrer-Policy"] = "strict-origin-when-cross-origin";
                h["X-Frame-Options"] = "DENY";           // frame-ancestors for browsers that predate CSP 2
                h["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=()";
                h[options.CspReportOnly ? "Content-Security-Policy-Report-Only" : "Content-Security-Policy"] = ContentSecurityPolicy;
                await next();
            });

            app.UseCookiePolicy();
            app.UseRateLimiter();
        }

        /// <summary>
        /// The CSP report endpoint: every violation the browser reports (in report-only and in enforce
        /// mode) becomes one warning in the log, so "load every screen clean" is something you can read.
        /// Small body (8 KB, answered 413 above that), rate limited per address, no session.
        /// </summary>
        public static void MapCspReports(this WebApplication app)
        {
            app.MapPost(CspReportPath, async (HttpContext ctx, ILoggerFactory logs) =>
            {
                const int limit = 8 * 1024;
                if (ctx.Request.ContentLength is null or > limit)
                    return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);

                var buffer = new char[limit];
                using var reader = new System.IO.StreamReader(ctx.Request.Body);
                var read = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
                logs.CreateLogger("AccessOps.Csp").LogWarning("CSP violation reported: {Report}", new string(buffer, 0, read));
                return Results.NoContent();
            }).RequireRateLimiting(CspReportPolicy);
        }
    }
}
