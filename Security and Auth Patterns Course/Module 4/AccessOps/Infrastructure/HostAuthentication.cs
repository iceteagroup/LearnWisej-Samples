using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Everything with HTTP headers, configured in Startup.cs before <c>app.UseWisej()</c>: the
    /// ASP.NET Core cookie scheme for the host authentication ticket (Secure, HttpOnly,
    /// SameSite=Strict), antiforgery for every state-changing POST that trusts that cookie, the
    /// WebSocket origin guard, and the endpoints <c>/auth/antiforgery</c>, <c>/auth/cookie</c>,
    /// <c>/auth/signout</c>, <c>/auth/status</c> and <c>/auth/continue</c>.
    /// <para>
    /// This ticket is not Wisej's: the Wisej.NET session ID lives in the tab's sessionStorage, and
    /// Wisej 4.1.7's own HttpOnly request-validation cookie is left alone.
    /// </para>
    /// </summary>
    public static class HostAuthentication
    {
        /// <summary>The host authentication cookie.</summary>
        public const string CookieName = "AccessOps.Auth";

        /// <summary>The antiforgery cookie.</summary>
        public const string AntiforgeryCookieName = "AccessOps.Antiforgery";

        /// <summary>The request header that carries the antiforgery token.</summary>
        public const string AntiforgeryHeader = "RequestVerificationToken";

        /// <summary>
        /// Registers the cookie scheme, the grant check on host requests and antiforgery.
        /// <paramref name="requireHttps"/> is true everywhere except a Development run on
        /// <c>http://localhost</c> (<c>AccessOps:LocalHttpVerification</c>, see README): then the
        /// antiforgery cookie follows the request scheme, because ASP.NET Core refuses to issue a
        /// Secure-only antiforgery cookie on a plain HTTP request. The host ticket stays Secure.
        /// </summary>
        public static IServiceCollection AddAccessOpsHostAuthentication(this IServiceCollection services, bool requireHttps = true)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
                {
                    o.Cookie.Name = CookieName;
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    o.Cookie.SameSite = SameSiteMode.Strict;
                    o.Cookie.Path = "/";
                    o.ExpireTimeSpan = GrantStore.DefaultLifetime;
                    o.SlidingExpiration = false;
                    // the same grant check every protected Wisej.NET operation makes, for host requests
                    o.Events.OnValidatePrincipal = ValidateGrantAsync;
                    // endpoints answer 401/403; there is no login page to redirect to here
                    o.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
                    o.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
                });

            services.AddAuthorization();
            services.AddAntiforgery(o =>
            {
                o.HeaderName = AntiforgeryHeader;
                o.Cookie.Name = AntiforgeryCookieName;
                o.Cookie.HttpOnly = true;
                o.Cookie.SecurePolicy = requireHttps ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
                o.Cookie.SameSite = SameSiteMode.Strict;
            });
            return services;
        }

        /// <summary>The WebSocket origin guard (before <c>app.UseWisej()</c>).</summary>
        public static IApplicationBuilder UseAccessOpsOriginGuard(this IApplicationBuilder app)
            => app.UseMiddleware<WebSocketOriginGuard>();

        /// <summary>Maps the host-cookie endpoints. Call after <c>UseAuthentication()</c>, before <c>UseWisej()</c>.</summary>
        public static IEndpointRouteBuilder MapAccessOpsAuthEndpoints(this IEndpointRouteBuilder app)
        {
            // A request token for the page's own script. A foreign page cannot read this response
            // (no CORS), and the cookie half is SameSite=Strict.
            app.MapGet("/auth/antiforgery", (HttpContext ctx, IAntiforgery antiforgery) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(ctx);
                ctx.Response.Headers.CacheControl = "no-store";
                return Results.Json(new { token = tokens.RequestToken });
            });

            // Redeems the single-use handle the Wisej.NET page load created and issues the ticket.
            app.MapPost("/auth/cookie", async (HttpContext ctx, IAntiforgery antiforgery, CookieHandoffStore handoffs, GrantStore grants, ILoggerFactory logs) =>
            {
                var log = logs.CreateLogger("AccessOps.HostCookie");
                if (!await IsValidPostAsync(ctx, antiforgery, log))
                    return Results.BadRequest();

                var handle = await ReadStringAsync(ctx, "handle");
                if (!handoffs.TryRedeem(handle, out var signIn) || !grants.IsActive(signIn.GrantId))
                {
                    log.LogWarning("POST /auth/cookie refused: handle not redeemable");
                    return Results.BadRequest();
                }

                await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, HostPrincipal(signIn));
                log.LogInformation("POST /auth/cookie · grant={Grant} · Set-Cookie (value redacted): {SetCookie}",
                    Grant.Shorten(signIn.GrantId), RedactSetCookie(ctx.Response.Headers.SetCookie.ToString()));
                return Results.NoContent();
            });

            // Sign-out for the host ticket: revoke the grant it names, then expire the cookie.
            app.MapPost("/auth/signout", async (HttpContext ctx, IAntiforgery antiforgery, GrantStore grants, ILoggerFactory logs) =>
            {
                var log = logs.CreateLogger("AccessOps.HostCookie");
                if (!await IsValidPostAsync(ctx, antiforgery, log))
                    return Results.BadRequest();

                var grantId = ctx.User.FindFirst(ApplicationPrincipal.GrantClaim)?.Value;
                var revoked = grantId != null && grants.Revoke(grantId);
                await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                log.LogInformation("POST /auth/signout · grant={Grant} revokedNow={Revoked} · Set-Cookie (value redacted): {SetCookie}",
                    Grant.Shorten(grantId), revoked, RedactSetCookie(ctx.Response.Headers.SetCookie.ToString()));
                return Results.NoContent();
            });

            // What the host cookie says, after OnValidatePrincipal rechecked its grant.
            app.MapGet("/auth/status", (HttpContext ctx) =>
            {
                ctx.Response.Headers.CacheControl = "no-store";
                if (ctx.User.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();
                return Results.Json(new
                {
                    signedIn = true,
                    grant = Grant.Shorten(ctx.User.FindFirst(ApplicationPrincipal.GrantClaim)?.Value),
                    tenant = ctx.User.FindFirst(ApplicationPrincipal.TenantClaim)?.Value
                });
            });

            // A continuation link: only a local path or an allowlisted entry; anything else -> "/".
            app.MapGet("/auth/continue", (string returnUrl, IConfiguration configuration) =>
                Results.Redirect(ReturnUrlPolicy.Resolve(returnUrl,
                    configuration.GetSection("AccessOps:ReturnUrlAllowlist").Get<string[]>())));

            return app;
        }

        /// <summary>The host ticket's principal: account, tenant and grant - nothing else.</summary>
        public static ClaimsPrincipal HostPrincipal(VerifiedSignIn signIn)
            => new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, signIn.AccountId),
                new Claim(ApplicationPrincipal.TenantClaim, signIn.TenantId),
                new Claim(ApplicationPrincipal.GrantClaim, signIn.GrantId)
            }, CookieAuthenticationDefaults.AuthenticationScheme));

        /// <summary>Replaces each cookie value with "(redacted)" and keeps the attributes.</summary>
        public static string RedactSetCookie(string setCookie)
            => Regex.Replace(setCookie ?? "", @"(^|,\s*)([^=;,\s]+)=[^;]*", "$1$2=(redacted)");

        private static async Task ValidateGrantAsync(CookieValidatePrincipalContext ctx)
        {
            var grantId = ctx.Principal?.FindFirst(ApplicationPrincipal.GrantClaim)?.Value;
            var grants = ctx.HttpContext.RequestServices.GetRequiredService<GrantStore>();
            if (!grants.IsActive(grantId))
            {
                ctx.RejectPrincipal();
                await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        private static async Task<bool> IsValidPostAsync(HttpContext ctx, IAntiforgery antiforgery, ILogger log)
        {
            try
            {
                await antiforgery.ValidateRequestAsync(ctx);
                return true;
            }
            catch (AntiforgeryValidationException)
            {
                // no token value, no cookie value, no exception text in the log
                log.LogWarning("POST {Path} refused: antiforgery validation failed", ctx.Request.Path.Value);
                return false;
            }
        }

        private static async Task<string> ReadStringAsync(HttpContext ctx, string property)
        {
            try
            {
                using var json = await JsonDocument.ParseAsync(ctx.Request.Body);
                return json.RootElement.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
                    ? value.GetString() : null;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
