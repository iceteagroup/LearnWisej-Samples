using System;
using AccessOps;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisej.Core;

// AccessOps intranet host (Module 2). Wisej.NET 4 is an ordinary ASP.NET Core application: the
// host authenticates the HTTP request before any Wisej.NET code runs. In production that host is
// IIS with Windows Authentication on the "Lab Intranet" application (iis/windows-auth.location.xml);
// on a developer machine it is Kestrel with the Negotiate handler (NTLM for a local account,
// Kerberos only on a domain). Either way the Windows user reaches Wisej.NET as Application.User.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// "Negotiate" (default): every request is challenged, like an IIS application with Anonymous off.
// "Off": no host authentication at all, like an IIS application where Windows Authentication was
// never enabled - Program.Main then shows the HostFaultPage (the host layer failed, not the app).
var windowsAuthentication = builder.Configuration["AccessOps:WindowsAuthentication"] ?? "Negotiate";
var challengeEnabled = !string.Equals(windowsAuthentication, "Off", StringComparison.OrdinalIgnoreCase);

if (challengeEnabled)
{
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
    builder.Services.AddAuthorization(options =>
    {
        // Every endpoint, the Wisej.NET handlers and the static files included, needs a signed-in
        // Windows user: the 401 happens before Program.Main can run.
        options.FallbackPolicy = options.DefaultPolicy;
    });
}

// The data lives outside the web root: the database is in the module's AccessOpsData folder.
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<AuditLog>();
builder.Services.AddSingleton<GrantStore>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<WindowsAccountMap>();
builder.Services.AddSingleton<CountingDocumentRepository>();
builder.Services.AddSingleton<IDocumentRepository>(sp => sp.GetRequiredService<CountingDocumentRepository>());
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddHttpContextAccessor();   // the diagnostics view reads the host request principal

var app = builder.Build();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated();
Subject.UseOpaqueKey(database.LoadOrCreateOpaqueKey());

// Fixture: the Windows account running this lab is mapped to an AccessOps account (reader1 by
// default, visitor1 to show the application deny) so the lab works on any machine. See README.md.
var mappedAs = builder.Configuration["AccessOps:CurrentWindowsUser"] ?? "reader1";
app.Services.GetRequiredService<WindowsAccountMap>().MapCurrentWindowsUser(mappedAs);

app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);
app.Logger.LogInformation("Windows authentication: {Mode}; the Windows account running this lab is mapped to {Account}",
    challengeEnabled ? "Negotiate (every request challenged)" : "Off (anonymous host: HostFaultPage)", mappedAs);

// The redacted challenge trace: scheme and status only, never a token or an Authorization value.
app.UseMiddleware<ChallengeTrace>();

if (challengeEnabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
