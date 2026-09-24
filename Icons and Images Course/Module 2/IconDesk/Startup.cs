using Microsoft.AspNetCore.Builder;

using System;
using System.IO;
using System.Threading.Tasks;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    WebRootPath = "./"
});

var app = builder.Build();

// Add Wisej.NET (HTTP + WebSocket endpoints).
app.UseWisej();

// A deliberately slow endpoint for the Module 2 lab. It stands in for a far-away host so the
// asynchronous load can be demonstrated on a machine with no internet access, and so the delay
// is the same every time the lab is run. Everything else about the request is ordinary: it is a
// URL that returns image bytes.
app.MapGet("/slow/hero.jpg", async (Microsoft.AspNetCore.Http.HttpContext context) =>
{
    await Task.Delay(TimeSpan.FromSeconds(2.5));

    var path = Path.Combine(AppContext.BaseDirectory, "Images", "hero.jpg");
    var bytes = await File.ReadAllBytesAsync(path);

    context.Response.ContentType = "image/jpeg";
    await context.Response.Body.WriteAsync(bytes);
});

// Serve static content - including everything under Images/, which is what an image source
// with a relative URL resolves against - but never the .json configuration files.
app.UseWhen(
    context => !context.Request.Path.Value.EndsWith(".json", StringComparison.InvariantCulture),
    app => app.UseFileServer());

app.Run();
