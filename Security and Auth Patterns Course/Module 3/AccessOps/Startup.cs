using System;
using AccessOps;
using AccessOps.Infrastructure;
using AccessOps.Services;
using Microsoft.AspNetCore.Builder;
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

// The data lives outside the web root: the web root is this project folder, the database is in
// the module's AccessOpsData folder beside it.
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));

// Process-wide services. None of them holds a user: identity is read from the session of the
// calling thread through IAccessOpsSession, every time.
builder.Services.AddSingleton<IAccessOpsSession, WisejSession>();
builder.Services.AddSingleton<PasswordService>();
builder.Services.AddSingleton<LoginThrottle>();
builder.Services.AddSingleton<LoginAuditService>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<RememberedDeviceService>();
builder.Services.AddSingleton<PermissionService>();
builder.Services.AddSingleton<IAuthorize, Authorize>();
builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<SignOutService>();
builder.Services.AddSingleton<EmployeeDirectoryService>();
builder.Services.AddSingleton<DocumentRepository>();
builder.Services.AddSingleton<DocumentService>();
builder.Services.AddSingleton<ApprovalRepository>();
builder.Services.AddSingleton<ApprovalService>();
builder.Services.AddSingleton<AdminService>();
builder.Services.AddSingleton<PermissionRefreshService>();

var app = builder.Build();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated(app.Services.GetRequiredService<PasswordService>());
app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
