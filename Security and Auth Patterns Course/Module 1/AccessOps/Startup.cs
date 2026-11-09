using System;
using AccessOps;
using AccessOps.Documents;
using AccessOps.Identity;
using AccessOps.Infrastructure;
using AccessOps.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wisej.Core;

// AccessOps host. Wisej.NET 4 is an ordinary ASP.NET Core application: the host (Kestrel here,
// IIS in production) authenticates - or not - the HTTP request; that is the "host request
// principal". Module 1 adds no host authentication, so it is always anonymous on this run.

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// The data lives outside the web root: the database is in the module's AccessOpsData folder.
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<AuditLog>();
builder.Services.AddSingleton<GrantStore>();
builder.Services.AddSingleton<AccountStore>();
builder.Services.AddSingleton<CountingDocumentRepository>();
builder.Services.AddSingleton<IDocumentRepository>(sp => sp.GetRequiredService<CountingDocumentRepository>());
builder.Services.AddSingleton<DocumentService>();
#if DEBUG
// Training mock: Debug builds only. A Release build registers no verifier and refuses sign-in.
builder.Services.AddSingleton<IAccountVerifier, TestVerifier>();
builder.Services.AddHttpContextAccessor();   // the diagnostics view reads the host principal
#endif

var app = builder.Build();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated();
Subject.UseOpaqueKey(database.LoadOrCreateOpaqueKey());
app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
