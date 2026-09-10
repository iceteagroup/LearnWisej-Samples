using EnterpriseOps.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using System;
using Wisej.Core;

// ---------------------------------------------------------------------------------------------
// EnterpriseOps host — Module 12 (Cloud, Containers, Load Balancing & Release Engineering).
//
// Everything that is environment-specific happens HERE, once, at process start:
//   1. builder.Configuration layers appsettings.json → appsettings.{ASPNETCORE_ENVIRONMENT}.json
//      → environment variables (the ASP.NET Core model; secrets arrive as environment variables
//      injected by the platform or a vault, never from a committed file).
//   2. StartupValidation checks the contract and refuses to boot when a required setting is
//      missing (fail fast: a node that cannot work must never join the load balancer).
//   3. HealthCheck.json configures the probe; /healthz (readiness) and /healthz/live (liveness)
//      are mapped next to Wisej.NET so a balancer or orchestrator can ask "are you ready?".
//   4. Forwarded headers are honoured so the app sees the real client scheme/address behind
//      NGINX / Apache / a cloud ingress (see deployment/nginx.conf).
// ---------------------------------------------------------------------------------------------

// The lab default is Development (every value lives in the files). A platform sets
// ASPNETCORE_ENVIRONMENT explicitly — Staging/Production then demand their secrets from the
// environment and the host refuses to boot without them (verified: with the variable set to
// Production and no secrets, the process exits with the two configuration errors below).
var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrWhiteSpace(environmentName))
    environmentName = "Development";

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./",
    EnvironmentName = environmentName
});

// (1) The configuration contract. builder.Configuration already contains appsettings.json,
//     appsettings.{Environment}.json and environment variables; we only capture it for the app.
HostConfiguration.Capture(builder.Configuration, builder.Environment.EnvironmentName, builder.Environment.ContentRootPath);

// (2) Fail fast on a broken contract. Under Development every value is in the files; under
//     Staging/Production the secrets must come from the environment (vault / platform).
var validation = StartupValidation.Validate(builder.Configuration, builder.Environment.EnvironmentName);
HostConfiguration.RecordStartupValidation(validation);
Console.WriteLine($"[startup] environment={builder.Environment.EnvironmentName} node={HostConfiguration.NodeName} release={HostConfiguration.ReleaseVersion}");
if (!validation.Succeeded)
{
    foreach (var error in validation.Errors)
        Console.Error.WriteLine("[startup] configuration error: " + error);
    throw new InvalidOperationException(
        $"EnterpriseOps refuses to start in '{builder.Environment.EnvironmentName}': " +
        string.Join(" | ", validation.Errors));
}

// (3) HealthCheck.json → the probe contract (which checks, intervals, thresholds).
HealthProbeService.LoadHealthCheckFile(builder.Environment.ContentRootPath);

var app = builder.Build();

// (4) Behind a reverse proxy: trust X-Forwarded-For / X-Forwarded-Proto so redirects, cookies
//     and the audit log see https:// and the real client address. KnownProxies/KnownIPNetworks are
//     cleared on purpose for the lab (the docker-compose network is not known in advance); in
//     production name the proxy or the ingress subnet instead — see deployment/nginx.proxy.notes.md.
var forwarded = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwarded.KnownIPNetworks.Clear();
forwarded.KnownProxies.Clear();
app.UseForwardedHeaders(forwarded);

// Add Wisej.NET (owns *.wx — everything else passes through).
app.UseWisej();

// Health probes. Readiness = every check in HealthCheck.json; liveness = the process answers.
// Wisej.NET only handles its own *.wx paths, so a plain minimal-API endpoint coexists with it.
app.MapGet("/healthz", () =>
{
    var report = HealthProbeService.Probe();
    return Results.Json(report, statusCode: report.IsHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable);
});
app.MapGet("/healthz/live", () => Results.Json(HealthProbeService.Liveness()));

// Serve static content (Default.html) but never the .json configuration files
// (Default.json, appsettings*.json, HealthCheck.json stay private).
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
