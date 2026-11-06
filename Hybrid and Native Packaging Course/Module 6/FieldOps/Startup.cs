using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using FieldOps.Model;
using FieldOps.Services;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// Serve static content (Default.html, the PWA shell files) but never the .json configuration
// files. sw.js and the manifest go out with no-cache (Module 2).
var files = new FileServerOptions();
files.StaticFileOptions.OnPrepareResponse = ctx =>
{
    var name = ctx.File.Name;
    if (name == "sw.js" || name.EndsWith(".webmanifest", StringComparison.OrdinalIgnoreCase))
        ctx.Context.Response.Headers["Cache-Control"] = "no-cache";
};

app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer(files));

// The dispatch stand-in: the office system that assigns work. POST here and the order is added
// and published on OrderFeed, which every signed-in session of that technician hears through
// Wisej.NET server push. Loopback only - it is a lab stand-in, not an API.
//   Invoke-RestMethod -Method Post "http://localhost:6303/dispatch/assign?tech=dana"
app.MapPost("/dispatch/assign", (Microsoft.AspNetCore.Http.HttpContext http, string tech, string customer, string task, string address, string priority) =>
{
    if (http.Connection.RemoteIpAddress == null || !IPAddress.IsLoopback(http.Connection.RemoteIpAddress))
        return Results.StatusCode(403);
    try
    {
        var order = WorkOrderService.Current.Assign(
            tech ?? "dana",
            customer ?? "Elmtree Pumping Station",
            address ?? "Elmtree Lane, Pump House 1",
            task ?? "Inlet valve leak",
            Enum.TryParse<WorkOrderPriority>(priority, true, out var p) ? p : WorkOrderPriority.High);
        return Results.Json(new { order.Number, order.Customer, order.Fault, order.AssignedTechnician });
    }
    catch (Exception ex)
    {
        OpsLog.Error("dispatch", ex);
        return Results.BadRequest(ex.Message);
    }
});

// HybridConfig.LogUrl target (Module 6): the Hybrid client POSTs its log lines here.
app.MapPost("/client-log", async (Microsoft.AspNetCore.Http.HttpContext http) =>
{
    using var reader = new System.IO.StreamReader(http.Request.Body);
    var body = await reader.ReadToEndAsync();
    OpsLog.Info("client-log: " + (body.Length > 500 ? body.Substring(0, 500) + "…" : body));
    return Results.Ok();
});

app.Run();
