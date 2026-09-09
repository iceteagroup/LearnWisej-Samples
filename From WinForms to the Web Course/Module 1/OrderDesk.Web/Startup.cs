using Microsoft.AspNetCore.Builder;
using System;
using Wisej.Core;

// The ASP.NET Core host. WinForms had no equivalent: the executable owned the process.
// Here the server host owns the process and Wisej.NET is middleware inside it.
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Wisej.NET middleware (Wisej-4 4.1.0 exposes UseWisej(); there is no separate AddWisej() service call).
app.UseWisej();

// Static files from the project folder (Default.html, wwwroot/...), never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
