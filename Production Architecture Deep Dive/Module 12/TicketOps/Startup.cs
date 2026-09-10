using Microsoft.AspNetCore.Builder;
using System;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html, wwwroot/*) but never the .json configuration files.
//
// Module 12 exception: HealthCheck.json is the load-balancer probe target (GET /HealthCheck.json). It carries
// no secret — app name, version, build, environment and the names of the dependencies the node needs — so it is
// the one .json the file server may answer. Default.json (startup type, theme, debug flag) stays protected, as
// does every other .json in the folder. A probe that gets 200 knows the process is up and which build answers;
// readiness (can the node reach its store?) is what IHealthCheckService adds inside the app — see
// docs/LoadBalancingNotes.md for how a real /health endpoint would combine the two.
app.UseWhen(
    context => !IsProtectedJson(context.Request.Path.Value),
    app => app.UseFileServer());

app.Run();

static bool IsProtectedJson(string path)
{
    if (path == null || !path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        return false;

    return !string.Equals(path, "/HealthCheck.json", StringComparison.OrdinalIgnoreCase);
}
