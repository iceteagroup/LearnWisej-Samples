using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Xunit;

namespace AccessOps.Tests
{
    /// <summary>Module 5: issuer + subject mapping, the one-time hand-off and the handler configuration.</summary>
    public class ExternalSignInTests
    {
        [Fact]
        public void Issuer_and_subject_map_to_the_local_account_with_tenant_and_role_from_AccessOps_data()
        {
            using var w = new TestWorld();
            var result = w.Mapper.Map(TestWorld.Dana);
            Assert.True(result.Succeeded);
            Assert.Equal(AccessOpsDatabase.DanaAccountId, result.Account.Id);
            Assert.Equal("A", result.Account.Tenant);
            Assert.Equal("Reader", result.Account.Role);
        }

        [Theory]
        [InlineData("http://localhost:6416", "c77d-21b4-kai-berg", "unapproved_tenant")]      // F08
        [InlineData("http://localhost:6416", "0000-nobody-unmapped", "unknown_login")]        // no external_logins row
        [InlineData("https://evil.example", "4f21-9ac0-dana-ellis", "unknown_login")]         // same subject, other issuer
        public void The_mapper_refuses_before_any_grant(string issuer, string subject, string reason)
        {
            using var w = new TestWorld();
            var result = w.Mapper.Map(new ExternalIdentity(issuer, subject));
            Assert.False(result.Succeeded);
            Assert.Equal(reason, result.Reason);
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public void F07_a_hand_off_handle_redeems_once_and_expires()
        {
            using var w = new TestWorld();
            var handle = w.ExternalHandoffs.Park(TestWorld.Dana);
            Assert.True(w.ExternalHandoffs.TryRedeem(handle, out var identity));
            Assert.Equal(TestWorld.Dana, identity);
            Assert.False(w.ExternalHandoffs.TryRedeem(handle, out _));     // replayed hand-off

            var late = w.ExternalHandoffs.Park(TestWorld.Dana);
            w.Clock.Now += ExternalHandoffStore.Lifetime;
            Assert.False(w.ExternalHandoffs.TryRedeem(late, out _));
        }

        [Fact]
        public void One_sign_in_creates_exactly_one_grant()
        {
            using var w = new TestWorld();
            w.SignInPartner(out var signIn);
            Assert.Equal(1, w.Grants.CountAll());
            Assert.Equal(1, w.Grants.CountActive(AccessOpsDatabase.DanaAccountId));
            Assert.Equal("oidc", signIn.Method);
        }

        [Theory]
        [InlineData("Correlation failed.", "correlation")]
        [InlineData("Unable to unprotect the message.State.", "state")]
        [InlineData("Message contains error: 'access_denied', error_description: 'error_description is null'", "access_denied")]
        [InlineData("Something else", "other")]
        public void Handler_failures_become_allowlisted_reasons(string message, string reason)
            => Assert.Equal(reason, OpenIdConnectSetup.Classify(new Exception(message)));

        /// <summary>The real handler configuration, with a fixed provider metadata document instead of discovery.</summary>
        private static async Task<(WebApplication App, HttpClient Client)> StartAsync(TestWorld w)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(w.Logs);
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Oidc:Authority"] = "https://idp.test",
                ["Oidc:ClientId"] = "accessops",
                ["Oidc:ClientSecret"] = "unit-test-only",
                ["Oidc:CallbackPath"] = "/signin-oidc",
                ["Oidc:Scopes:0"] = "openid",
                ["Oidc:Scopes:1"] = "profile",
                ["Oidc:Scopes:2"] = "email"
            });
            builder.Services.AddSingleton(w.Grants);
            builder.Services.AddSingleton(w.LoginAudit);
            builder.Services.AddSingleton(w.Handoffs);
            builder.Services.AddSingleton<IClock>(w.Clock);
            builder.AddAccessOpsOpenIdConnect();
            builder.Services.PostConfigure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, o =>
            {
                o.Configuration = new OpenIdConnectConfiguration
                {
                    Issuer = "https://idp.test",
                    AuthorizationEndpoint = "https://idp.test/connect/authorize",
                    TokenEndpoint = "https://idp.test/connect/token",
                    EndSessionEndpoint = "https://idp.test/connect/endsession"
                };
                // the handler asks its configuration manager, not Options.Configuration
                o.ConfigurationManager = new Microsoft.IdentityModel.Protocols.StaticConfigurationManager<OpenIdConnectConfiguration>(o.Configuration);
            });

            var app = builder.Build();
            app.MapAccessOpsAccountEndpoints();
            app.MapFallback("{**path}", ctx => ctx.Response.WriteAsync("wisej"));
            await app.StartAsync();
            var client = app.GetTestClient();
            client.BaseAddress = new Uri("https://localhost/");
            return (app, client);
        }

        [Fact]
        public async Task The_challenge_asks_for_the_code_flow_with_PKCE_and_minimal_scopes()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            var response = await client.GetAsync("account/signin");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(response.Headers.Location.Query);
            Assert.Equal("https://idp.test/connect/authorize", response.Headers.Location.GetLeftPart(UriPartial.Path));
            Assert.Equal("code", query["response_type"]);
            // ResponseMode = Query: the handler then sends no response_mode at all (query is the
            // code flow's default); with the default form_post it would send response_mode=form_post
            Assert.False(query.ContainsKey("response_mode"));
            Assert.Equal("S256", query["code_challenge_method"]);
            Assert.False(string.IsNullOrEmpty(query["code_challenge"]));
            Assert.False(string.IsNullOrEmpty(query["state"]));
            Assert.False(string.IsNullOrEmpty(query["nonce"]));
            Assert.Equal("openid profile email", query["scope"]);
            Assert.Equal("https://localhost/signin-oidc", query["redirect_uri"]);

            var cookies = response.Headers.GetValues("Set-Cookie").ToList();
            Assert.Contains(cookies, c => c.StartsWith(".AspNetCore.Correlation.", StringComparison.Ordinal) && c.Contains("samesite=lax", StringComparison.OrdinalIgnoreCase) && c.Contains("httponly"));
            Assert.Contains(cookies, c => c.StartsWith(".AspNetCore.OpenIdConnect.Nonce.", StringComparison.Ordinal) && c.Contains("samesite=lax", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task F06_a_forged_or_foreign_state_lands_on_the_sign_in_view_with_no_grant()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            var response = await client.GetAsync("signin-oidc?code=forged-code&state=forged-state");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Default.html?signin=failed", response.Headers.Location.OriginalString);
            Assert.Equal(0, w.Grants.CountAll());
            Assert.Contains(w.LoginAudit.Recent(1), r => r.Outcome == "ExternalRefused");

            // the log has the allowlisted reason and nothing from the callback query
            Assert.Contains(w.Logs.Lines, l => l.Contains("reason=state", StringComparison.Ordinal));
            Assert.DoesNotContain(w.Logs.Lines, l => l.Contains("forged-code", StringComparison.Ordinal) || l.Contains("forged-state", StringComparison.Ordinal));
        }

        [Fact]
        public async Task A_provider_denial_lands_on_the_sign_in_view_with_no_grant()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            // a real state from this browser's own challenge, then the provider answers access_denied
            var challenge = await client.GetAsync("account/signin");
            var state = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(challenge.Headers.Location.Query)["state"].ToString();
            var cookies = string.Join("; ", challenge.Headers.GetValues("Set-Cookie").Select(c => c.Split(';')[0]));

            var request = new HttpRequestMessage(HttpMethod.Get, "signin-oidc?error=access_denied&state=" + Uri.EscapeDataString(state));
            request.Headers.Add("Cookie", cookies);
            var response = await client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Default.html?signin=failed", response.Headers.Location.OriginalString);
            Assert.Contains(w.Logs.Lines, l => l.Contains("reason=access_denied", StringComparison.Ordinal));
            Assert.Equal(0, w.Grants.CountAll());
        }

        [Fact]
        public async Task The_provider_sign_out_post_needs_an_antiforgery_token()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            var forged = await client.PostAsync("account/signout", new FormUrlEncodedContent(new Dictionary<string, string>()));
            Assert.Equal(HttpStatusCode.BadRequest, forged.StatusCode);
        }
    }
}
