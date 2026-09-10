using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OrderDesk.Diagnostics;
using System;
using Wisej.Core;

// The ASP.NET Core host. WinForms had no equivalent: the executable owned the process.
// Here the server host owns the process and Wisej.NET is middleware inside it.
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Module 7: the health endpoint a load balancer / container orchestrator polls. It is a plain
// ASP.NET Core endpoint — no Wisej session is created for it, so HealthCheck.Json() only reads
// process-level facts (uptime, version, storage root writable, log folder). Registered before
// UseWisej so the intent is visible first; WebApplication matches endpoints at the start of the
// pipeline and executes them at the end, after the Wisej and file-server middleware have passed
// a request they do not own.
app.MapGet("/health", () => Results.Content(OrderDesk.Diagnostics.HealthCheck.Json(), "application/json"));

// Wisej.NET middleware (Wisej-4 4.1.0 exposes UseWisej(); there is no separate AddWisej() service call).
app.UseWisej();

// Static files from the project folder (Default.html, wwwroot/...), never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
