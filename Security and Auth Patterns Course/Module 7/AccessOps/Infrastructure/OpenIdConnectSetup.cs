using System;
using System.Threading.Tasks;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.AspNetCore.Antiforgery;
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
    /// Partner federation with what Wisej.NET 4.1.7 offers: ASP.NET Core's cookie and OpenID
    /// Connect handlers (authorization code flow, PKCE, state, nonce, ID-token validation against
    /// the provider's published keys), registered in Startup.cs before <c>app.UseWisej()</c>. The
    /// handler never signs anybody in to AccessOps: once the ID token is validated it parks issuer +
    /// subject in <see cref="ExternalHandoffStore"/>, gives the browser a one-time handle in the
    /// HttpOnly <c>AccessOps.Handoff</c> cookie and sends it back to the Wisej.NET start page.
    /// </summary>
    public static class OpenIdConnectSetup
    {
        /// <summary>Starts the challenge.</summary>
        public const string SignInPath = "/account/signin";

        /// <summary>Ends the provider session (POST, antiforgery).</summary>
        public const string SignOutPath = "/account/signout";

        /// <summary>The one sentence the sign-in view shows for every failed or refused external sign-in.</summary>
        public const string FailedSentence = "Sign-in did not complete. Try again or contact your administrator.";

        /// <summary>
        /// <c>AddAuthentication</c> with the cookie scheme (Module 4's host ticket), then
        /// <c>AddOpenIdConnect</c>: code flow, PKCE, no tokens in cookies, protocol claim names,
        /// HTTPS metadata unless configuration says otherwise, and the provider's response mode.
        /// The client secret is read from configuration (<c>Oidc__ClientSecret</c> from user
        /// secrets or the environment) - never from a tracked file; startup fails without it.
        /// </summary>
        public static void AddAccessOpsOpenIdConnect(this WebApplicationBuilder builder, bool requireHttps = true)
        {
            var oidc = builder.Configuration.GetSection("Oidc");
            var clientSecret = oidc["ClientSecret"];
            if (string.IsNullOrEmpty(clientSecret))
                throw new InvalidOperationException("Oidc:ClientSecret is not configured. Supply it with 'dotnet user-secrets set Oidc:ClientSecret <value>' (Development) or the Oidc__ClientSecret environment variable.");


            builder.Services.AddAccessOpsHostAuthentication(requireHttps);   // cookie scheme + antiforgery
            builder.Services.AddAuthentication()
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
                {
                    o.Authority = oidc["Authority"];                       // pinned: discovery and keys come only from here
                    o.ClientId = oidc["ClientId"];
                    o.ClientSecret = clientSecret;                         // Oidc__ClientSecret at runtime
                    o.CallbackPath = oidc["CallbackPath"] ?? "/signin-oidc";
                    o.SignedOutCallbackPath = "/signout-callback-oidc";
                    o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    // On in production. The lab's test provider runs on http://localhost, so the
                    // module's appsettings.json turns it off (Oidc:RequireHttpsMetadata = false).
                    o.RequireHttpsMetadata = oidc.GetValue("RequireHttpsMetadata", true);
                    o.ResponseType = OpenIdConnectResponseType.Code;
                    o.ResponseMode = OpenIdConnectResponseMode.Query;     // what the provider supports; the handler's default is form_post
                    o.UsePkce = true;
                    o.SaveTokens = false;                                  // tokens stay on the server
                    o.MapInboundClaims = false;                            // keep iss and sub as the protocol names them
                    o.GetClaimsFromUserInfoEndpoint = false;
                    o.Scope.Clear();
                    foreach (var scope in oidc.GetSection("Scopes").Get<string[]>() ?? new[] { "openid", "profile", "email" })
                        o.Scope.Add(scope);

                    // The correlation and nonce cookies ride the provider's cross-site callback (a
                    // top-level GET): they must be Lax, not Strict.
                    o.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    o.NonceCookie.SameSite = SameSiteMode.Lax;

                    o.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = HandOffToWisej,
                        OnRemoteFailure = ctx => BackToSignIn(ctx.HttpContext, Classify(ctx.Failure), ctx.HandleResponse),
                        OnAccessDenied = ctx => BackToSignIn(ctx.HttpContext, "access_denied", ctx.HandleResponse)
                    };
                });
        }

        /// <summary>
        /// <c>app.UseAuthentication()</c> and the account endpoints, before <c>app.UseWisej()</c>.
        /// The handler itself answers <c>/signin-oidc</c> and <c>/signout-callback-oidc</c>.
        /// </summary>
        public static void MapAccessOpsAccountEndpoints(this WebApplication app)
        {
            app.UseAuthentication();

            // "Sign in with your work account": the tab leaves for the provider from here.
            app.MapGet(SignInPath, () =>
                Results.Challenge(new AuthenticationProperties { RedirectUri = "/Default.html" },
                    new[] { OpenIdConnectDefaults.AuthenticationScheme }));

            // The provider half of sign-out. The Wisej.NET session is already gone and the grant
            // revoked; the page posts here with an antiforgery token, the handler sends the browser
            // to the provider's end_session_endpoint and back through /signout-callback-oidc.
            app.MapPost(SignOutPath, async (HttpContext ctx, IAntiforgery antiforgery, ILoggerFactory logs) =>
            {
                try
                {
                    await antiforgery.ValidateRequestAsync(ctx);
                }
                catch (AntiforgeryValidationException)
                {
                    logs.CreateLogger("AccessOps.ExternalSignIn").LogWarning("POST {Path} refused: antiforgery validation failed", SignOutPath);
                    return Results.BadRequest();
                }

                logs.CreateLogger("AccessOps.ExternalSignIn").LogInformation("POST {Path} · ending the provider session (end_session_endpoint)", SignOutPath);
                return Results.SignOut(new AuthenticationProperties { RedirectUri = "/Default.html?ended=signout" },
                    new[] { CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme });
            });
        }

        private static Task HandOffToWisej(TokenValidatedContext ctx)
        {
            // iss and sub of the validated ID token (signature, issuer, audience, lifetime and nonce
            // were checked by the handler before this event)
            var identity = new ExternalIdentity(ctx.Principal.FindFirst("iss")?.Value, ctx.Principal.FindFirst("sub")?.Value);
            var handle = ctx.HttpContext.RequestServices.GetRequiredService<ExternalHandoffStore>().Park(identity);

            ctx.Response.Cookies.Append(ExternalHandoffStore.CookieName, handle, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,     // read by the start page's own same-origin request
                Path = "/",
                MaxAge = ExternalHandoffStore.Lifetime
            });

            Logger(ctx.HttpContext).LogInformation("ID token validated (signature, iss, aud, exp, nonce) · {Subject} · parked for the Wisej.NET page load",
                identity.RedactedSubject);

            ctx.Response.Redirect("/Default.html");
            ctx.HandleResponse();   // no cookie sign-in here: the Wisej.NET session is the session
            return Task.CompletedTask;
        }

        // Every failure lands on the sign-in view with one sentence; the log gets an allowlisted
        // reason - never the callback query, a code, a state value or the exception text.
        private static Task BackToSignIn(HttpContext http, string reason, Action handleResponse)
        {
            var grants = http.RequestServices.GetRequiredService<GrantStore>();
            http.RequestServices.GetRequiredService<LoginAudit>().Write("x-callback", "a-callback", LoginOutcome.ExternalRefused, AuditService.NewCorrelationId(), reason);
            Logger(http).LogWarning("external sign-in failed at the callback · reason={Reason} · grants total={Grants}", reason, grants.CountAll());
            http.Response.Redirect("/Default.html?signin=failed");
            handleResponse();
            return Task.CompletedTask;
        }

        /// <summary>Turns a handler failure into one of a fixed set of reasons.</summary>
        public static string Classify(Exception failure)
        {
            var message = failure?.Message ?? "";
            if (message.Contains("Correlation failed", StringComparison.OrdinalIgnoreCase))
                return "correlation";
            if (message.Contains("unprotect the message.State", StringComparison.OrdinalIgnoreCase))
                return "state";
            if (message.Contains("access_denied", StringComparison.OrdinalIgnoreCase))
                return "access_denied";
            if (failure is OpenIdConnectProtocolException)
                return "provider_error";
            return "other";
        }

        private static ILogger Logger(HttpContext http)
            => http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AccessOps.ExternalSignIn");
    }
}
