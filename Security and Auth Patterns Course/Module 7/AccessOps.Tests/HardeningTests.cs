using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AccessOps.Infrastructure;
using AccessOps.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>
    /// Module 7: the host's controls, run in memory. The lab app listens on http://localhost, where
    /// HSTS is never sent and no certificate is installed; these tests run the same middleware with
    /// RequireHttps on, against a real host name, so the HTTPS-only behaviour is checked too.
    /// </summary>
    public sealed class HardeningTests
    {
        private const string Host = "accessops.contoso.example";
        private const string SvcPassword = "Demo-Service-2026";   // seeded demo credential (Data/seed-credentials.json)

        private static async Task<(WebApplication App, HttpClient Client)> StartAsync(TestHost data, Dictionary<string, string> settings)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Production" });
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Configuration.AddInMemoryCollection(settings);
            builder.AddAccessOpsHardening();
            builder.Services.AddSingleton(data.Audit);

            var app = builder.Build();
            app.UseAccessOpsHardening();
            app.MapGet("/", () => "AccessOps");
            app.MapGet("/cookie", (HttpContext ctx) =>
            {
                ctx.Response.Cookies.Append("AccessOps.Probe", "1");   // no flags requested
                return "ok";
            });
            app.MapGet("/share/{id:int}", (int id) => Results.Ok()).RequireRateLimiting(Hardening.ShareLinkPolicy);
            await app.StartAsync();
            return (app, app.GetTestClient());
        }

        private static Dictionary<string, string> Production() => new Dictionary<string, string>
        {
            ["Hardening:RequireHttps"] = "true",
            ["Hardening:HttpsPort"] = "443",
            ["Hardening:CspMode"] = "Enforce",
            ["Hardening:ShareLinkBurst"] = "3",
            ["Hardening:ShareLinkPerMinute"] = "1"
        };

        [Fact]
        public async Task Plain_http_is_redirected_to_https()
        {
            using var data = new TestHost();
            var (app, client) = await StartAsync(data, Production());
            await using var _ = app;

            var response = await client.GetAsync($"http://{Host}/Default.html?x=1");

            Assert.Equal((HttpStatusCode)308, response.StatusCode);
            Assert.Equal($"https://{Host}/Default.html?x=1", response.Headers.Location?.ToString());
        }

        [Fact]
        public async Task Https_responses_carry_hsts_and_the_security_headers()
        {
            using var data = new TestHost();
            var (app, client) = await StartAsync(data, Production());
            await using var _ = app;

            var response = await client.GetAsync($"https://{Host}/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("max-age=31536000", response.Headers.GetValues("Strict-Transport-Security").Single());
            Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
            Assert.Equal("strict-origin-when-cross-origin", response.Headers.GetValues("Referrer-Policy").Single());
            var csp = response.Headers.GetValues("Content-Security-Policy").Single();
            Assert.Contains("script-src 'unsafe-inline' 'unsafe-eval' 'self'", csp);
            Assert.Contains("frame-ancestors 'none'", csp);
            Assert.False(response.Headers.Contains("Content-Security-Policy-Report-Only"));
        }

        [Fact]
        public async Task Report_only_mode_sends_the_same_policy_under_the_report_only_header()
        {
            using var data = new TestHost();
            var settings = Production();
            settings["Hardening:CspMode"] = "ReportOnly";
            var (app, client) = await StartAsync(data, settings);
            await using var _ = app;

            var response = await client.GetAsync($"https://{Host}/");

            Assert.Equal(Hardening.ContentSecurityPolicy, response.Headers.GetValues("Content-Security-Policy-Report-Only").Single());
            Assert.False(response.Headers.Contains("Content-Security-Policy"));
        }

        [Fact]
        public async Task Cookies_written_through_aspnetcore_get_secure_httponly_and_samesite()
        {
            using var data = new TestHost();
            var (app, client) = await StartAsync(data, Production());
            await using var _ = app;

            var response = await client.GetAsync($"https://{Host}/cookie");

            var cookie = response.Headers.GetValues("Set-Cookie").Single().ToLowerInvariant();
            Assert.Contains("secure", cookie);
            Assert.Contains("httponly", cookie);
            Assert.Contains("samesite=lax", cookie);
        }

        [Fact]
        public async Task The_share_link_answers_429_when_the_bucket_is_empty_and_the_refusal_is_audited()
        {
            using var data = new TestHost();
            var (app, client) = await StartAsync(data, Production());
            await using var _ = app;

            var codes = new List<HttpStatusCode>();
            for (var i = 0; i < 5; i++)
                codes.Add((await client.GetAsync($"https://{Host}/share/4471")).StatusCode);

            Assert.Equal(new[] { HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.OK, HttpStatusCode.TooManyRequests, HttpStatusCode.TooManyRequests }, codes);
            var row = data.Audit.Latest(1).Single();
            Assert.Equal("Document.ShareOpen", row.Action);
            Assert.Equal("RateLimited", row.Outcome);
            Assert.Equal("Document:4471", row.Target);
        }

        [Fact]
        public async Task The_429_carries_the_security_headers_too()
        {
            using var data = new TestHost();
            var (app, client) = await StartAsync(data, Production());
            await using var _ = app;

            HttpResponseMessage last = null;
            for (var i = 0; i < 4; i++)
                last = await client.GetAsync($"https://{Host}/share/4471");

            Assert.Equal(HttpStatusCode.TooManyRequests, last.StatusCode);
            Assert.True(last.Headers.Contains("Content-Security-Policy"));
            Assert.True(last.Headers.Contains("X-Content-Type-Options"));
        }

        [Fact]
        public void Twenty_passwords_in_ten_seconds_are_refused_after_the_bucket_empties_before_the_hash()
        {
            using var host = new TestHost();
            var now = new DateTime(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);
            var login = new LoginService(host.Employees, new PasswordService(), new LoginThrottle(),
                new LoginAuditService(host.Audit, host.Session), host.Session, host.Permissions,
                new LoginRateLimiter(5, TimeSpan.FromSeconds(12), () => now));

            var outcomes = new List<LoginOutcome>();
            for (var i = 0; i < 20; i++)
            {
                outcomes.Add(login.SignIn("svc.reports", "wrong-" + i).Outcome);
                now = now.AddMilliseconds(500);
            }

            Assert.All(outcomes.Take(5), o => Assert.Equal(LoginOutcome.BadCredential, o));
            Assert.All(outcomes.Skip(5), o => Assert.Contains(o, new[] { LoginOutcome.RateLimited, LoginOutcome.LockedOut }));
            Assert.Contains(LoginOutcome.RateLimited, outcomes);
            Assert.Contains(host.Audit.Latest(30), r => r.Action == "Session.SignIn" && r.Outcome == "RateLimited");

            // even the right password is refused while the bucket is empty: the hash is never reached
            Assert.Equal(LoginOutcome.RateLimited, login.SignIn("svc.reports", SvcPassword).Outcome);
        }

        [Fact]
        public void A_provider_sign_out_needs_a_ticket_the_server_issued_and_only_once()
        {
            var now = new DateTime(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);
            var tickets = new SignOutTickets(() => now);

            Assert.False(tickets.Redeem(null));                 // the bare GET from another site
            Assert.False(tickets.Redeem("made-up-ticket-value-1234"));

            var ticket = tickets.Issue();
            Assert.True(tickets.Redeem(ticket));
            Assert.False(tickets.Redeem(ticket));               // one-time

            var late = tickets.Issue();
            now = now.Add(SignOutTickets.Lifetime).AddSeconds(1);
            Assert.False(tickets.Redeem(late));                 // expired
        }

        [Fact]
        public void The_login_bucket_is_per_name_and_address_and_refills()
        {
            var now = new DateTime(2026, 11, 6, 9, 0, 0, DateTimeKind.Utc);
            var limiter = new LoginRateLimiter(2, TimeSpan.FromSeconds(10), () => now);

            Assert.True(limiter.TryTake("svc.reports", "10.0.0.1"));
            Assert.True(limiter.TryTake("svc.reports", "10.0.0.1"));
            Assert.False(limiter.TryTake("svc.reports", "10.0.0.1"));
            Assert.True(limiter.TryTake("svc.reports", "10.0.0.2"));    // another address
            Assert.True(limiter.TryTake("other.account", "10.0.0.1"));  // another name

            now = now.AddSeconds(10);
            Assert.True(limiter.TryTake("svc.reports", "10.0.0.1"));
            Assert.False(limiter.TryTake("svc.reports", "10.0.0.1"));
        }
    }
}
