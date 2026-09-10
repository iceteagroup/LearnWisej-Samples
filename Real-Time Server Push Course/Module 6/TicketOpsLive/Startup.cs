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

// Serve static content (Default.html) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
