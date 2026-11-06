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
// the module's AccessOpsData folder beside it. Nothing under ./ is ever a data file.
builder.Services.AddSingleton(sp => AccessOpsDatabase.FromConfiguration(builder.Configuration, builder.Environment.ContentRootPath));
builder.Services.AddSingleton<EmployeeDirectoryService>();

var app = builder.Build();

AppHost.Services = app.Services;

var database = app.Services.GetRequiredService<AccessOpsDatabase>();
database.EnsureCreated();
app.Logger.LogInformation("AccessOps data folder: {DataRoot}", database.DataRoot);

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve the start page, but never a .json file: Default.json and appsettings.json sit in the
// web root and are configuration, not downloads.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
    a => a.UseFileServer());

app.Run();
