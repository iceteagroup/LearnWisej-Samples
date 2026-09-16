using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Services;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// 1. The dataset lives in a local SQLite file next to the project, resolved to an absolute path so the
//    app opens the same file however it was started.
var connectionString = builder.Configuration.GetConnectionString("PerfLab")
    ?? throw new InvalidOperationException("Missing connection string 'PerfLab'.");
connectionString = PerfLabPaths.ResolveSqliteDataSource(connectionString, builder.Environment.ContentRootPath);

// 2. The lab switch behind "Break the database": one instance, read by the connection interceptor.
builder.Services.AddSingleton<OutageSwitch>();

builder.Services.AddDbContextFactory<PerfLabContext>((services, options) =>
{
    options.UseSqlite(connectionString);
    options.AddInterceptors(new OutageInterceptor(services.GetRequiredService<OutageSwitch>()));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        // Parameter values in logs: local development only.
        options.EnableSensitiveDataLogging();
    }
});

// 3. The probe is registered next to the logging the host already configured: it holds no per-session
//    state, so a singleton is the right lifetime, and its records reach the console through ILogger.
builder.Services.AddSingleton<ScenarioProbe>();

// 4. Application services. Stateless, so Transient — and Wisej.NET resolves [Inject] through the ROOT
//    provider, where a Scoped registration would throw "Cannot resolve scoped service ... from root provider".
builder.Services.AddTransient<DashboardService>();
builder.Services.AddTransient<TicketQueryService>();
builder.Services.AddTransient<CustomerTreeService>();
builder.Services.AddTransient<ExportService>();   // Module 6: it takes TicketQueryService, not the DbContext factory

var app = builder.Build();

// 5. The bridge: from here on Wisej.NET resolves [Inject] properties on Pages and Forms through
//    Microsoft DI. PerfLabServices keeps the same provider reachable from the tab UserControls.
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);
PerfLabServices.Provider = app.Services;

// 6. Create and seed the lab dataset before the first session: no scenario should ever measure the
//    first-run setup.
await PerfLabDatabase.EnsureReadyAsync(app.Services);

// 7. The health gate. Wisej.NET reads HealthCheck.json from the application root and serves
//    healthcheck.wx; this adds the custom availability function that records why the instance last
//    said no. The thresholds come from docs/Capacity.md, which comes from the measurements.
WisejPerfLab.Health.HealthPolicy.Apply();
Console.Error.WriteLine("[WisejPerfLab] health: " + WisejPerfLab.Health.HealthPolicy.DescribeConfiguration());

// 8. The health URL a load balancer polls.
//    Wisej.NET's own healthcheck.wx handler belongs to the classic System.Web pipeline; running under
//    Kestrel with dotnet run it answered 200 whatever the configured limits were (verified on
//    Wisej-4 4.1.0), so this host serves the same URL itself — from the same HealthCheck.json values
//    and the same HealthPolicy.IsAvailable the framework hook uses. On IIS the framework handler applies
//    and this middleware is harmless: it answers first, with the same decision.
app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/healthcheck.wx", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    if (WisejPerfLab.Health.HealthPolicy.IsAvailable())
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync(WisejPerfLab.Health.HealthPolicy.LastAnswer);
        return;
    }

    // 503 plus Retry-After: the load balancer sends new users to another instance and comes back later.
    // Existing sessions are untouched — they keep talking to this instance until they are done.
    context.Response.StatusCode = Wisej.Core.HealthCheck.ReturnCode;
    context.Response.Headers["Retry-After"] = Wisej.Core.HealthCheck.RetryAfter.ToString();
    await context.Response.WriteAsync(WisejPerfLab.Health.HealthPolicy.LastAnswer);
});

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    a => a.UseFileServer());

app.Run();
