using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace AccessOps.Tests
{
    /// <summary>
    /// The host side of Startup.cs in TestHost: the same origin guard, cookie scheme, antiforgery
    /// and /auth endpoints, with a terminal handler standing in for <c>app.UseWisej()</c>.
    /// </summary>
    public class HostEndpointTests
    {
        private readonly ITestOutputHelper _output;

        public HostEndpointTests(ITestOutputHelper output) => _output = output;

        private static async Task<(WebApplication App, HttpClient Client)> StartAsync(TestWorld w)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Development" });
            builder.WebHost.UseTestServer();
            builder.Logging.ClearProviders();
            builder.Logging.AddProvider(w.Logs);
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["AccessOps:AllowedOrigins:0"] = "https://localhost"
            });
            builder.Services.AddSingleton(w.Grants);
            builder.Services.AddSingleton(w.Handoffs);
            builder.Services.AddAccessOpsHostAuthentication();

            var app = builder.Build();
            app.UseAccessOpsOriginGuard();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapAccessOpsAuthEndpoints();
            // stands in for app.UseWisej(). A terminal app.Run() would run before the endpoint
            // middleware WebApplication appends at the end, so it is a fallback endpoint instead.
            app.MapFallback("{**path}", ctx => ctx.Response.WriteAsync("wisej"));
            await app.StartAsync();

            var client = app.GetTestClient();
            client.BaseAddress = new Uri("https://localhost/");
            return (app, client);
        }

        /// <summary>A minimal cookie jar: name -> value from Set-Cookie headers.</summary>
        private sealed class Jar
        {
            public readonly Dictionary<string, string> Cookies = new Dictionary<string, string>();

            public void Take(HttpResponseMessage response)
            {
                if (!response.Headers.TryGetValues("Set-Cookie", out var values))
                    return;
                foreach (var v in values)
                {
                    var pair = v.Split(';')[0];
                    var i = pair.IndexOf('=');
                    var name = pair[..i];
                    var value = pair[(i + 1)..];
                    if (v.Contains("expires=Thu, 01 Jan 1970", StringComparison.OrdinalIgnoreCase))
                        this.Cookies.Remove(name);
                    else
                        this.Cookies[name] = value;
                }
            }

            public HttpRequestMessage Request(HttpMethod method, string path, string token = null, object json = null)
            {
                var request = new HttpRequestMessage(method, path);
                if (this.Cookies.Count > 0)
                    request.Headers.Add("Cookie", string.Join("; ", this.Cookies.Select(c => c.Key + "=" + c.Value)));
                if (token != null)
                    request.Headers.Add(HostAuthentication.AntiforgeryHeader, token);
                if (json != null)
                    request.Content = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");
                return request;
            }
        }

        private static async Task<string> TokenAsync(HttpClient client, Jar jar)
        {
            var response = await client.SendAsync(jar.Request(HttpMethod.Get, "auth/antiforgery"));
            jar.Take(response);
            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return doc.RootElement.GetProperty("token").GetString();
        }

        [Fact]
        public async Task Cookie_hand_off_issues_a_Secure_HttpOnly_SameSite_Strict_ticket_and_sign_out_revokes_it()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;
            var jar = new Jar();

            w.SignInAna(out var signIn);
            var handle = w.Handoffs.Create(signIn);

            var token = await TokenAsync(client, jar);
            var issued = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/cookie", token, new { handle }));
            Assert.Equal(HttpStatusCode.NoContent, issued.StatusCode);
            var setCookie = issued.Headers.GetValues("Set-Cookie").Single(v => v.StartsWith(HostAuthentication.CookieName + "=", StringComparison.Ordinal));
            var redacted = HostAuthentication.RedactSetCookie(setCookie);
            _output.WriteLine("Set-Cookie (value redacted): " + redacted);
            Assert.Contains("secure", setCookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("httponly", setCookie, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("samesite=strict", setCookie, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain(setCookie.Split(';')[0].Split('=', 2)[1], redacted, StringComparison.Ordinal);
            jar.Take(issued);

            // the handle is single-use
            var again = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/cookie", await TokenAsync(client, jar), new { handle }));
            Assert.Equal(HttpStatusCode.BadRequest, again.StatusCode);

            var status = await client.SendAsync(jar.Request(HttpMethod.Get, "auth/status"));
            Assert.Equal(HttpStatusCode.OK, status.StatusCode);

            // sign-out: a fresh token (the user changed), then the grant is revoked and the cookie expired
            var signedOut = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/signout", await TokenAsync(client, jar), new { }));
            Assert.Equal(HttpStatusCode.NoContent, signedOut.StatusCode);
            Assert.False(w.Grants.IsActive(signIn.GrantId));
            _output.WriteLine("Set-Cookie on sign-out (value redacted): " + HostAuthentication.RedactSetCookie(
                string.Join(", ", signedOut.Headers.GetValues("Set-Cookie"))));
            jar.Take(signedOut);

            var after = await client.SendAsync(jar.Request(HttpMethod.Get, "auth/status"));
            Assert.Equal(HttpStatusCode.Unauthorized, after.StatusCode);
        }

        [Fact]
        public async Task The_host_cookie_stops_working_when_its_grant_is_revoked_elsewhere()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;
            var jar = new Jar();
            w.SignInAna(out var signIn);

            var issued = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/cookie", await TokenAsync(client, jar), new { handle = w.Handoffs.Create(signIn) }));
            jar.Take(issued);
            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(jar.Request(HttpMethod.Get, "auth/status"))).StatusCode);

            w.Grants.Revoke(signIn.GrantId);   // sign-out in another tab
            var status = await client.SendAsync(jar.Request(HttpMethod.Get, "auth/status"));
            Assert.Equal(HttpStatusCode.Unauthorized, status.StatusCode);   // OnValidatePrincipal rejected it
        }

        [Fact]
        public async Task F11_a_forged_post_without_or_with_a_wrong_token_changes_nothing()
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;
            var jar = new Jar();
            w.SignInAna(out var signIn);
            var issued = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/cookie", await TokenAsync(client, jar), new { handle = w.Handoffs.Create(signIn) }));
            jar.Take(issued);

            // what another site can make the browser send: the cookies, but no token it could read
            var noToken = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/signout", null, new { }));
            var wrongToken = await client.SendAsync(jar.Request(HttpMethod.Post, "auth/signout", "forged", new { }));
            Assert.Equal(HttpStatusCode.BadRequest, noToken.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, wrongToken.StatusCode);
            Assert.True(w.Grants.IsActive(signIn.GrantId));          // no state change
            Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(jar.Request(HttpMethod.Get, "auth/status"))).StatusCode);
        }

        [Theory]
        [InlineData("https://evil.example", HttpStatusCode.Forbidden)]
        [InlineData(null, HttpStatusCode.Forbidden)]
        [InlineData("https://localhost", HttpStatusCode.OK)]
        public async Task F11_a_WebSocket_upgrade_from_a_foreign_origin_never_reaches_Wisej(string origin, HttpStatusCode expected)
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            var request = new HttpRequestMessage(HttpMethod.Get, "ws.wx");
            request.Headers.Add("Upgrade", "websocket");
            request.Headers.Add("Connection", "Upgrade");
            request.Headers.Add("Sec-WebSocket-Version", "13");
            request.Headers.Add("Sec-WebSocket-Key", Convert.ToBase64String(new byte[16]));
            if (origin != null)
                request.Headers.Add("Origin", origin);

            var response = await client.SendAsync(request);
            Assert.Equal(expected, response.StatusCode);   // 200 = the stand-in for UseWisej() answered
        }

        [Theory]
        [InlineData("//evil.example", "/")]
        [InlineData("/\\evil.example", "/")]
        [InlineData("https://evil.example/", "/")]
        [InlineData("/Default.html?record=17", "/Default.html?record=17")]
        public async Task F11_an_external_continuation_url_goes_home(string returnUrl, string location)
        {
            using var w = new TestWorld();
            var (app, client) = await StartAsync(w);
            await using var _ = app;

            var response = await client.GetAsync("auth/continue?returnUrl=" + Uri.EscapeDataString(returnUrl));
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal(location, response.Headers.Location.OriginalString);
        }
    }
}
