using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Web;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

// 1. The connection string comes from configuration (appsettings.json, environment), never from source.
var connectionString = builder.Configuration.GetConnectionString("SupportDesk")
    ?? throw new InvalidOperationException("Missing connection string 'SupportDesk'.");
connectionString = SupportDeskPaths.ResolveSqliteDataSource(connectionString, builder.Environment.ContentRootPath);

// 2. EF Core is registered in Microsoft DI: AddDbContextFactory + the SQLite provider (inside SupportDesk.Data).
builder.Services.AddSupportDeskData(connectionString, builder.Environment.IsDevelopment());

// 3. Application services. They are stateless and hold only the factory, so Transient is the right lifetime.
//    Verified: Wisej.NET resolves [Inject] properties through the ROOT Microsoft provider, so a Scoped
//    registration fails in Development ("Cannot resolve scoped service ... from root provider").
builder.Services.AddTransient<TicketQueryService>();
builder.Services.AddTransient<TicketCommandService>();
builder.Services.AddTransient<SchemaInfoService>();
builder.Services.AddTransient<DevelopmentSeeder>();
builder.Services.AddTransient<ModelDemoService>();
builder.Services.AddTransient<SharedContextAntiPattern>();
builder.Services.AddTransient<BoundIQueryableAntiPattern>();
// Module 5: UI-free, no reference to Wisej.Web — see TicketValidator's remarks.
builder.Services.AddTransient<TicketValidator>();
// Module 7: UI-free, no reference to Wisej.Web — see ConflictResolution's remarks. TransactionFailureSwitch
// and StartupDiagnostics are registered as singletons inside AddSupportDeskData, next to DevelopmentOutageSwitch.
builder.Services.AddTransient<ConflictResolution>();

var app = builder.Build();

// 4. The bridge: Wisej.NET resolves [Inject] properties on Pages/Forms through Microsoft's provider from here on.
//    After this line, register application services with Microsoft DI only — never mix the two containers.
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);

// 5. Module 7: record what this instance decided, for the server console and the page's "Environment &
//    diagnostics" panel — read once here, never guessed by the UI. EnableSensitiveDataLogging (inside
//    AddSupportDeskData, step 2 above) is already gated by the same isDevelopment flag; this only reports
//    the decision, it does not make it twice.
var diagnostics = app.Services.GetRequiredService<StartupDiagnostics>();
diagnostics.EnvironmentName = app.Environment.EnvironmentName;
diagnostics.SensitiveDataLoggingOn = app.Environment.IsDevelopment();
Console.Error.WriteLine($"[SupportDesk] environment: {diagnostics.EnvironmentName} · sensitive-data logging: {(diagnostics.SensitiveDataLoggingOn ? "ON (Development)" : "OFF")}");

// 6. Development convenience: migrate the local SQLite file to the current model and seed it before the
//    first session starts (MigrateAsync + DevelopmentSeeder). Production applies the reviewed script
//    (artifacts/sql/supportdesk_migrations.sql) as a release step instead — this call, and therefore
//    Migrate(), never runs outside Development. See docs/DeploymentNotes.md.
if (app.Environment.IsDevelopment())
{
    await SupportDeskDevelopmentDatabase.EnsureReadyAsync(app.Services);
    diagnostics.MigrationsAppliedAtStartup = true;

    await using var db = await app.Services.GetRequiredService<IDbContextFactory<SupportDeskContext>>().CreateDbContextAsync();
    diagnostics.AppliedMigrations = (await db.Database.GetAppliedMigrationsAsync()).ToList();
}
else
{
    diagnostics.MigrationsAppliedAtStartup = false;
    Console.Error.WriteLine("[SupportDesk] migrations: NOT applied at startup outside Development — apply artifacts/sql/supportdesk_migrations.sql as a reviewed release step, never from application startup.");
}

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    a => a.UseFileServer());

app.Run();
