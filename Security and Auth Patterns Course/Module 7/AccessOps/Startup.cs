using System;
using System.IO;
using System.Net;
using AccessOps;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wisej.Core;

// AccessOps, Module 7: ONE build, started once per entry point - like the course's two IIS
// applications. AccessOps:Route = Intranet (Windows integrated authentication: IIS in production,
// Kestrel Negotiate in the lab; scoped reads) or Partner (public HTTPS: own sign-in, OpenID Connect,
// passkeys). Both routes end in the same application principal and the same RecordPolicy, over the
// same durable store. Everything below the route switch is identical on every node of a route:
// forwarded headers from the known proxies only, one shared Data Protection key ring and
// application name, no downloadable .json, the grant read from the store on every call.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var route = Enum.TryParse<AccessOpsRoute>(builder.Configuration["AccessOps:Route"], ignoreCase: true, out var r)
    ? r
    : throw new InvalidOperationException("AccessOps:Route must be 'Intranet' or 'Partner' (one process per entry point; see README.md).");

// The shared data folder (database, audit key, Data Protection keys, the ingress switch file).
var database = AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile(Path.Combine(database.DataRoot, IngressSwitch.FileName), optional: true, reloadOnChange: true);

// ---- identical on every node, both routes ------------------------------------------------------

// Shared cookie-protection keys: every node that must read another node's correlation, nonce or
// authentication cookie uses the same key ring and the same application name.
var keyPath = builder.Configuration["DataProtection:KeyPath"] ?? "keys";
if (!Path.IsPathRooted(keyPath))
    keyPath = Path.Combine(database.DataRoot, keyPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
    .SetApplicationName(builder.Configuration["DataProtection:ApplicationName"] ?? "AccessOps");

// Behind a TLS-terminating proxy the scheme, host and client address come from forwarded headers -
// honoured only from the proxies and networks named in configuration (Hosting:KnownProxies,
// Hosting:KnownNetworks). The defaults (loopback) are cleared first so nothing is trusted implicitly.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    o.ForwardLimit = 1;
    o.KnownProxies.Clear();
    o.KnownIPNetworks.Clear();
    foreach (var proxy in builder.Configuration.GetSection("Hosting:KnownProxies").Get<string[]>() ?? Array.Empty<string>())
        o.KnownProxies.Add(IPAddress.Parse(proxy));
    foreach (var network in builder.Configuration.GetSection("Hosting:KnownNetworks").Get<string[]>() ?? Array.Empty<string>())
        o.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(network));
    foreach (var host in builder.Configuration.GetSection("Hosting:AllowedHosts").Get<string[]>() ?? Array.Empty<string>())
        o.AllowedHosts.Add(host);
});

// Data and process-wide services (none of them holds a user). Both routes register the same
// policy and stores; the intranet process also needs the partner services because its release
// review runs the partner cases against the shared store.
builder.Services.AddSingleton(database);
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton(new PasswordHasher());
builder.Services.AddSingleton<AuditService>();
builder.Services.AddSingleton<LoginAudit>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<GrantStore>();
builder.Services.AddSingleton<LoginThrottle>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<SignOutService>();
builder.Services.AddSingleton<CookieHandoffStore>();
builder.Services.AddSingleton<ExternalHandoffStore>();
builder.Services.AddSingleton<CountingDocumentRepository>();
builder.Services.AddSingleton<IDocumentRepository>(sp => sp.GetRequiredService<CountingDocumentRepository>());
builder.Services.AddSingleton<RecordPolicy>();
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddSingleton<ExportService>();
builder.Services.AddSingleton<ExternalIdentityMapper>();
builder.Services.AddSingleton<WisejExternalSignIn>();
builder.Services.AddSingleton<IExternalSignIn>(sp => sp.GetRequiredService<WisejExternalSignIn>());
builder.Services.AddSingleton(RelyingPartySettings.FromConfiguration(builder.Configuration));
builder.Services.AddSingleton<PasskeyChallengeStore>();
builder.Services.AddSingleton<PasskeyCredentialStore>();
builder.Services.AddSingleton<PasskeyService>();
builder.Services.AddSingleton<WindowsAccountMap>();
builder.Services.AddSingleton<ConfidentialFileStore>();
builder.Services.AddSingleton<IdentityTraceLog>();
builder.Services.AddHttpContextAccessor();   // the scoped read takes the request's Windows token from here

// ---- the route ----------------------------------------------------------------------------------

var localHttp = false;
if (route == AccessOpsRoute.Intranet)
{
    // Every request is challenged, like an IIS application with Anonymous off and Windows
    // Authentication on (iis/windows-auth.location.xml): the 401 happens before Program.Main.
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
    builder.Services.AddAuthorization(o => o.FallbackPolicy = o.DefaultPolicy);
}
else
{
    // AccessOps:LocalHttpVerification is a Development-only switch for checking the partner route
    // on http://localhost in a browser that does not trust the HTTPS development certificate.
    localHttp = builder.Configuration.GetValue("AccessOps:LocalHttpVerification", false);
    if (localHttp && !builder.Environment.IsDevelopment())
        throw new InvalidOperationException("AccessOps:LocalHttpVerification is allowed in the Development environment only.");

    // the host cookie + antiforgery, then the OpenID Connect handler (code + PKCE); the client
    // secret comes from user secrets or Oidc__ClientSecret, never from a tracked file
    builder.AddAccessOpsOpenIdConnect(requireHttps: !localHttp);
}

// LAB FAULT INJECTION (Debug + Development only): a hand-off handle that can be redeemed twice, so
// the release review's stop condition "a replay accepted as a fresh sign-in" can be seen to fire.
#if DEBUG
HandoffTable.ReplayableForLab = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue("AccessOps:FaultInjection:ReplayableHandoff", false);
#endif

var app = builder.Build();

AppHost.Services = app.Services;
AppHost.Route = route;
AppHost.IsDevelopment = app.Environment.IsDevelopment();

database.EnsureCreated(app.Services.GetRequiredService<PasswordHasher>());
Subject.UseOpaqueKey(database.LoadOrCreateOpaqueKey());
app.Logger.LogInformation("AccessOps route: {Route} · data folder: {DataRoot} · key ring: {KeyPath}", route, database.DataRoot, keyPath);
#if DEBUG
if (HandoffTable.ReplayableForLab)
    app.Logger.LogWarning("LAB FAULT INJECTION ON: hand-off handles can be redeemed twice (AccessOps:FaultInjection:ReplayableHandoff)");
#endif

app.UseForwardedHeaders();

if (route == AccessOpsRoute.Intranet)
{
    // Fixture: the Windows account running this lab is mapped to an intranet account (reader1 by
    // default, visitor1 for the host-authenticated deny) so the lab works on any machine.
    var mappedAs = app.Configuration["AccessOps:CurrentWindowsUser"] ?? "reader1";
    if (OperatingSystem.IsWindows())
        app.Services.GetRequiredService<WindowsAccountMap>().MapCurrentWindowsUser(mappedAs);
    app.Logger.LogInformation("Intranet route: Negotiate on every request; this Windows account is mapped to {Account}; approved root {Root}",
        mappedAs, app.Services.GetRequiredService<ConfidentialFileStore>().ApprovedRoot);

    // the redacted challenge trace: scheme and status only, never a token or an Authorization value
    app.UseMiddleware<ChallengeTrace>();
    app.UseAuthentication();
    app.UseAuthorization();
}
else
{
    // The ingress switch first: with the partner route off nothing behind it runs.
    app.UseMiddleware<IngressSwitch>();

    // Refuse a WebSocket upgrade from a foreign Origin before Wisej.NET sees it.
    app.UseAccessOpsOriginGuard();

    // UseAuthentication + /account/signin (challenge) + /account/signout; the handler answers
    // /signin-oidc and /signout-callback-oidc.
    app.MapAccessOpsAccountEndpoints();
    app.UseAuthorization();

    // /auth/antiforgery, /auth/cookie, /auth/signout, /auth/status, /auth/continue
    app.MapAccessOpsAuthEndpoints();
}

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
