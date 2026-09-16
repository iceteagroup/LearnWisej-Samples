using System;
using Microsoft.AspNetCore.Builder;
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
builder.Services.AddTransient<TicketSearchService>();
builder.Services.AddTransient<CustomerTreeService>();
builder.Services.AddTransient<ExportService>();

var app = builder.Build();

// 5. The bridge: from here on Wisej.NET resolves [Inject] properties on Pages and Forms through
//    Microsoft DI. PerfLabServices keeps the same provider reachable from the tab UserControls.
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);
PerfLabServices.Provider = app.Services;

// 6. Create and seed the lab dataset before the first session: no scenario should ever measure the
//    first-run setup.
await PerfLabDatabase.EnsureReadyAsync(app.Services);

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    a => a.UseFileServer());

app.Run();
