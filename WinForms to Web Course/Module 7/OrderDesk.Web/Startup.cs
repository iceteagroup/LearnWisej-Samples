using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using Wisej.Core;

// The ASP.NET Core host. In WinForms the executable owned the process; here Kestrel does and
// Wisej.NET is middleware inside it (app.UseWisej()).
//
// Module 7 (deployment): the process also answers a plain HTTP probe at /health so a load balancer,
// an orchestrator or the reviewer's second browser tab can ask "is this node alive?" without a
// Wisej.NET session. Wisej.NET only handles its own *.wx paths, so a minimal-API endpoint coexists.
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Add Wisej.NET.
app.UseWisej();

// Deployment: liveness/readiness probe — JSON {status, sessions, orders, version, ...}.
// No session, no controls, nothing that needs a Wisej request context (see Services/HealthReport.cs).
app.MapGet("/health", () => Results.Json(OrderDesk.Services.HealthReport.Build()));

// Serve static content (Default.html, wwwroot/*) but never the .json configuration files
// (Default.json and ClientProfiles.json stay private).
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
