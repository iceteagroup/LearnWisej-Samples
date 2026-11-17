using Microsoft.AspNetCore.Builder;
using System;
using Wisej.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
	Args = args,
	WebRootPath = "./"
});

var app = builder.Build();

app.UseWisej();

// Serves Default.html and the Assets folder, but never the Wisej configuration files.
app.UseWhen(
	context => !context.Request.Path.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase),
	branch => branch.UseFileServer());

app.Run();
