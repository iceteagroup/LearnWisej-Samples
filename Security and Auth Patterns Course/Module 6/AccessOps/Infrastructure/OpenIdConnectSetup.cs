using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccessOps.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// External sign-in with what the released stack offers: ASP.NET Core authentication - the
    /// OpenID Connect handler (authorization code flow, PKCE, state, nonce, ID-token validation
    /// against the provider's keys) - configured here and mapped in Startup.cs before
    /// app.UseWisej(). The handler never signs anybody in to AccessOps: when the ID token has been
    /// validated it parks the identity in <see cref="ExternalHandoffStore"/>, hands the browser a
    /// one-time handle in an HttpOnly cookie, and sends it back to the Wisej.NET start page, where
    /// the session maps it (see WisejExternalSignIn).
    /// </summary>
    public static class OpenIdConnectSetup
    {
        public const string SignInPath = "/account/signin";
        public const string SignOutPath = "/account/signout";

        public static void AddAccessOpsOpenIdConnect(this WebApplicationBuilder builder)
        {
            var oidc = builder.Configuration.GetSection("Oidc");
            // checked at start-up: a fresh clone does not start until the environment supplies the secret
            var clientSecret = oidc["ClientSecret"]
                ?? throw new InvalidOperationException("Oidc:ClientSecret is not configured. Supply it with 'dotnet user-secrets' (Development) or the Oidc__ClientSecret environment variable.");
            builder.Services.AddSingleton<ExternalHandoffStore>();

            builder.Services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                // the handler needs a sign-in scheme; AccessOps never signs in to it (the callback
                // is handled before that) - the Wisej.NET session holds the identity
                .AddCookie(o =>
                {
                    o.Cookie.Name = "AccessOps.External";
                    o.Cookie.HttpOnly = true;
                    o.Cookie.SameSite = SameSiteMode.Lax;
                    o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                })
                .AddOpenIdConnect(o =>
                {
                    o.Authority = oidc["Authority"];
                    o.ClientId = oidc["ClientId"];
                    // never in source: supplied by configuration from the environment / user secrets
                    o.ClientSecret = clientSecret;
                    o.RequireHttpsMetadata = oidc.GetValue("RequireHttpsMetadata", true);
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.ResponseMode = OpenIdConnectResponseMode.Query;   // the handler's default is form_post
                    o.UsePkce = true;
                    o.SaveTokens = false;                        // tokens stay on the server, not in a cookie
                    o.GetClaimsFromUserInfoEndpoint = false;
                    o.MapInboundClaims = false;                  // keep "sub", "email", "name" as the provider sent them
                    o.Scope.Clear();
                    foreach (var scope in oidc.GetSection("Scopes").Get<string[]>() ?? new[] { "openid", "profile", "email" })
                        o.Scope.Add(scope);

                    // On plain http (local lab) a SameSite=None cookie without Secure is dropped by the
                    // browser; the callback is a top-level GET, so Lax is enough for both.
                    o.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    o.NonceCookie.SameSite = SameSiteMode.Lax;

                    o.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = HandOffToWisej,
                        OnRemoteFailure = ctx =>
                        {
                            Logger(ctx.HttpContext).LogWarning("External sign-in failed: {Reason}", ctx.Failure?.Message);
                            ctx.Response.Redirect("/Default.html?ended=failed");
                            ctx.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        public static void MapAccessOpsAccountEndpoints(this WebApplication app)
        {
            app.UseAuthentication();

            // "Sign in with your work account": the browser leaves for the provider from here
            app.MapGet(SignInPath, () =>
                Results.Challenge(new AuthenticationProperties { RedirectUri = "/Default.html" },
                    new[] { OpenIdConnectDefaults.AuthenticationScheme }));

            // the second half of sign-out: the Wisej session is already gone; end the provider's session too
            // (reason only picks which fixed sentence the sign-in page shows afterwards)
            app.MapGet(SignOutPath, (string reason) =>
                Results.SignOut(new AuthenticationProperties { RedirectUri = reason switch { "idle" => "/Default.html?ended=idle", "expired" => "/Default.html?ended=expired", _ => "/Default.html?ended=signout" } },
                    new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme }));
        }

        private static Task HandOffToWisej(TokenValidatedContext ctx)
        {
            var p = ctx.Principal;
            string Claim(string type) => p.FindFirst(type)?.Value;

            var identity = new ExternalIdentity(
                ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Oidc:ProviderName"] ?? "identity provider",
                Claim("iss"), Claim("sub"), Claim("email"), Claim("name"), Claim("given_name"), Claim("family_name"));

            var response = ctx.TokenEndpointResponse;
            var tokens = new ExternalTokenSet
            {
                AccessToken = response?.AccessToken,
                RefreshToken = response?.RefreshToken,
                AccessTokenExpiresUtc = DateTime.UtcNow.AddSeconds(int.TryParse(response?.ExpiresIn, out var s) ? s : 300)
            };

            var handle = ctx.HttpContext.RequestServices.GetRequiredService<ExternalHandoffStore>().Park(identity, tokens);
            ctx.Response.Cookies.Append(ExternalHandoffStore.CookieName, handle, new CookieOptions
            {
                HttpOnly = true,
                Secure = ctx.Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                MaxAge = ExternalHandoffStore.Lifetime
            });

            Logger(ctx.HttpContext).LogInformation(
                "ID token validated (signature, issuer, audience, expiry, nonce) for subject {Subject}; handed to the Wisej.NET session",
                identity.Subject);

            ctx.Response.Redirect("/Default.html");
            ctx.HandleResponse();   // no AccessOps cookie session: the Wisej.NET session is the session
            return Task.CompletedTask;
        }

        private static ILogger Logger(HttpContext http)
            => http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AccessOps.ExternalSignIn");
    }
}
