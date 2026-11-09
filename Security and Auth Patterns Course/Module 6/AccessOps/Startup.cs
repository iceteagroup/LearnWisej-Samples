using System;
using AccessOps;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wisej.Core;

// AccessOps host, partner route: passkeys (Module 6) beside OpenID Connect federation (Module 5). Wisej.NET 4 is an ordinary ASP.NET Core application: everything
// with HTTP headers - the host cookie, antiforgery, the WebSocket origin check, continuation
// URLs - is configured here, before app.UseWisej(). The Wisej.NET session itself carries the
// application principal (Application.User) and never a cookie of ours.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// Data outside the web root; process-wide services only (none of them holds a user).
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton(new PasswordHasher());
builder.Services.AddSingleton<AuditLog>();
builder.Services.AddSingleton<LoginAudit>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<GrantStore>();
builder.Services.AddSingleton<LoginThrottle>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<SignOutService>();
builder.Services.AddSingleton<CookieHandoffStore>();
builder.Services.AddSingleton<CountingDocumentRepository>();
builder.Services.AddSingleton<IDocumentRepository>(sp => sp.GetRequiredService<CountingDocumentRepository>());
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddSingleton<ExportService>();

// The host authentication cookie (Secure, HttpOnly, SameSite=Strict) and antiforgery.
// AccessOps:LocalHttpVerification is a Development-only switch for checking the sample on
// http://localhost in a browser that does not trust the HTTPS development certificate.
var localHttp = builder.Configuration.GetValue("AccessOps:LocalHttpVerification", false);
if (localHttp && !builder.Environment.IsDevelopment())
    throw new InvalidOperationException("AccessOps:LocalHttpVerification is allowed in the Development environment only.");

// Partner federation: the cookie scheme, then the OpenID Connect handler (code + PKCE), configured
// from the Oidc section; the client secret comes from user secrets or Oidc__ClientSecret.
builder.AddAccessOpsOpenIdConnect(requireHttps: !localHttp);
builder.Services.AddSingleton<ExternalIdentityMapper>();
builder.Services.AddSingleton<IExternalSignIn, WisejExternalSignIn>();

// Passkeys (Wisej-4-WebAuthn 4.1.7): the relying party from the WebAuthn section, the one-time
// challenges, the credential store and the server-side checks.
builder.Services.AddSingleton(RelyingPartySettings.FromConfiguration(builder.Configuration));
builder.Services.AddSingleton<PasskeyChallengeStore>();
builder.Services.AddSingleton<PasskeyCredentialStore>();
builder.Services.AddSingleton<PasskeyService>();

// Behind a TLS-terminating proxy the scheme, host and client address come from forwarded headers -
// honoured only from the proxies named in Hosting:KnownProxies (loopback by default).
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    foreach (var proxy in builder.Configuration.GetSection("Hosting:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
        o.KnownProxies.Add(System.Net.IPAddress.Parse(proxy));
});

var app = builder.Build();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated(app.Services.GetRequiredService<PasswordHasher>());
Subject.UseOpaqueKey(database.LoadOrCreateOpaqueKey());
app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);

app.UseForwardedHeaders();

// Refuse a WebSocket upgrade from a foreign Origin before Wisej.NET sees it.
app.UseAccessOpsOriginGuard();

// UseAuthentication + /account/signin (challenge) + /account/signout; the handler answers
// /signin-oidc and /signout-callback-oidc.
app.MapAccessOpsAccountEndpoints();
app.UseAuthorization();

// /auth/antiforgery, /auth/cookie, /auth/signout, /auth/status, /auth/continue
app.MapAccessOpsAuthEndpoints();

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
