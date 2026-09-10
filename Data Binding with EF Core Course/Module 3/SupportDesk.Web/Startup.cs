using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SupportDesk.Data;
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
builder.Services.AddTransient<SchemaInfoService>();
builder.Services.AddTransient<DevelopmentSeeder>();
builder.Services.AddTransient<ModelDemoService>();
builder.Services.AddTransient<SharedContextAntiPattern>();
builder.Services.AddTransient<BoundIQueryableAntiPattern>();

var app = builder.Build();

// 4. The bridge: Wisej.NET resolves [Inject] properties on Pages/Forms through Microsoft's provider from here on.
//    After this line, register application services with Microsoft DI only — never mix the two containers.
Wisej.Web.Application.Services.AddService<IServiceProvider>(app.Services);

// 5. Development convenience: migrate the local SQLite file to the current model and seed it before the
//    first session starts (MigrateAsync + DevelopmentSeeder). Production applies the reviewed script instead.
if (app.Environment.IsDevelopment())
    await SupportDeskDevelopmentDatabase.EnsureReadyAsync(app.Services);

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    a => a.UseFileServer());

app.Run();
