using System;
using AccessOps;
using AccessOps.Infrastructure;
using AccessOps.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisej.Core;

// AccessOps host. Wisej.NET 4 is an ordinary ASP.NET Core application: everything that is not
// the Wisej.NET UI (an endpoint, a header, a limit) is configured here, in this one file.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// Logging: one provider for console and file, with the redaction policy applied at the sink and the
// command's correlation scope on every line (Infrastructure/RedactingLogger.cs).
var logFolder = System.IO.Path.GetFullPath(System.IO.Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["AccessOps:DataRoot"] ?? "../AccessOpsData", "logs"));
builder.Logging.ClearProviders();
builder.Logging.AddProvider(new RedactingLoggerProvider(logFolder));

// Secrets - the connection string, the OIDC client secret, the share-link signing key - come from
// configuration layered as appsettings.json < user secrets (Development) < environment variables.
// None is in a tracked file; each is checked below and the app refuses to start without it.

// The data lives outside the web root: the web root is this project folder, the database is in
// the module's AccessOpsData folder beside it.
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));

// Process-wide services. None of them holds a user: identity is read from the session of the
// calling thread through IAccessOpsSession, every time.
builder.Services.AddSingleton<IAccessOpsSession, WisejSession>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<LoginThrottle>();
builder.Services.AddSingleton<AuditService>();
builder.Services.AddSingleton<LoginAuditService>();
builder.Services.AddSingleton<PermissionEditorService>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<PermissionService>();
builder.Services.AddSingleton<IAuthorize, Authorize>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<SignOutService>();
builder.Services.AddSingleton<EmployeeDirectoryService>();
builder.Services.AddSingleton<DocumentRepository>();
builder.Services.AddSingleton<DocumentService>();

// Documents: the store outside the web root, the upload gate, delivery through the session, share links.
// The host's request limit is the same ceiling as Upload.MaxFileSize (plus room for the multipart
// envelope), so a hand-built post cannot go around the control.
var maxUpload = builder.Configuration.GetValue<long>("Storage:MaxFileSizeBytes", 20 * 1024 * 1024);
builder.WebHost.ConfigureKestrel(k => k.Limits.MaxRequestBodySize = maxUpload + 64 * 1024);
builder.Services.AddSingleton(sp => DocumentStore.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));
builder.Services.AddSingleton(new UploadGate(maxUpload));
builder.Services.AddSingleton<IFileDelivery, WisejFileDelivery>();
builder.Services.AddSingleton<ShareLinkService>();
builder.Services.AddSingleton<ApprovalRepository>();
builder.Services.AddSingleton<ApprovalService>();
builder.Services.AddSingleton<AdminService>();
builder.Services.AddSingleton<PermissionRefreshService>();

// External sign-in: the ASP.NET Core OpenID Connect handler (Infrastructure/OpenIdConnectSetup.cs)
// and the one seam the pages use (IExternalSignIn).
builder.AddAccessOpsOpenIdConnect();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ExternalIdentityMapper>();
builder.Services.AddSingleton<IExternalSignIn, WisejExternalSignIn>();
builder.Services.AddSingleton<HrProfileClient>();

// Module 7: the host's part - forwarded headers, HTTPS + HSTS, the security headers, cookie policy,
// request limits and the share-link rate limit (Infrastructure/Hardening.cs).
var hardening = builder.AddAccessOpsHardening();
builder.Services.AddSingleton(new LoginRateLimiter(hardening.LoginBurst, TimeSpan.FromSeconds(hardening.LoginRefillSeconds), () => DateTime.UtcNow));

var app = builder.Build();

// First in the pipeline, before any endpoint and before app.UseWisej(), so every response carries
// the headers - the Wisej.NET pages, the OIDC callback and the share-link URL alike.
app.UseAccessOpsHardening();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated(app.Services.GetRequiredService<PasswordService>());
app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);
var store = app.Services.GetRequiredService<DocumentStore>();
if (store.IsUnderWebRoot(app.Environment.WebRootPath ?? app.Environment.ContentRootPath))
    throw new InvalidOperationException("The document store must be outside the web root.");
app.Services.GetRequiredService<DocumentRepository>().EnsureSeedFiles(store);
app.Services.GetRequiredService<ShareLinkService>();   // fails now if the signing key is missing
app.Logger.LogInformation("Audit chain check at start-up: {Result}",
    app.Services.GetRequiredService<AuditService>().VerifyChain() is long broken ? "BROKEN at entry " + broken : "intact");
app.Logger.LogInformation("Hardening: HTTPS {Https}, CSP {Csp}, share-link bucket {Burst}+{Rate}/min, login bucket {LoginBurst} per {Refill}s",
    hardening.RequireHttps ? "required (redirect + HSTS)" : "NOT required (development profile)",
    hardening.CspReportOnly ? "report-only" : "enforced", hardening.ShareLinkBurst, hardening.ShareLinkPerMinute,
    hardening.LoginBurst, hardening.LoginRefillSeconds);

// ASP.NET Core endpoints beside the Wisej.NET app: /account/signin, /signin-oidc (the handler),
// /account/signout. Mapped before app.UseWisej(); they have no Wisej.NET session.
app.MapAccessOpsAccountEndpoints();

// The signed, expiring share link (Infrastructure/ShareLinkEndpoint.cs).
app.MapShareLinks().RequireRateLimiting(Hardening.ShareLinkPolicy);

// Where browsers report Content-Security-Policy violations (Infrastructure/Hardening.cs).
app.MapCspReports();

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
