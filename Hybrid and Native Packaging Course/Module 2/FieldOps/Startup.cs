using Microsoft.AspNetCore.Builder;
using System;
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
// files. sw.js and the manifest go out with no-cache, so the browser revalidates them on every
// check and notices a new worker or a changed manifest the moment it is deployed.
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

app.Run();
