using Microsoft.AspNetCore.Builder;
using System;
using Wisej.Core;

// The ASP.NET Core host. In WinForms the executable owned the process; here Kestrel does and
// Wisej.NET is middleware inside it (app.UseWisej()).
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Add Wisej.NET.
app.UseWisej();

// Serve static content (Default.html, wwwroot/*) but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
