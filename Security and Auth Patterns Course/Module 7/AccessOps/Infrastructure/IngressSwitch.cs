using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// The lab's stand-in for the ingress rule that publishes the partner route (an IIS site
    /// binding, an ARR / load-balancer rule). It is the first middleware of the partner pipeline and
    /// reads <c>Ingress:PartnerRoute</c> from <c>ingress.json</c> in the shared data folder, which the
    /// host reloads on change: writing "Off" there - by hand or from the release review's rollback -
    /// makes every partner request (pages, the WebSocket, the OpenID Connect callback, the cookie
    /// endpoints) answer <c>503</c> with one plain sentence, while the intranet route keeps running.
    /// Nothing about grants or sessions changes: rolling the route back leaves the store alone.
    /// </summary>
    public sealed class IngressSwitch
    {
        /// <summary>The file, in the shared data folder.</summary>
        public const string FileName = "ingress.json";

        /// <summary>The sentence a switched-off route answers with.</summary>
        public const string OffSentence = "The AccessOps partner route is turned off. Staff use the intranet route.";

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IngressSwitch> _logger;

        /// <summary>Creates the middleware.</summary>
        public IngressSwitch(RequestDelegate next, IConfiguration configuration, ILogger<IngressSwitch> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>True unless <c>Ingress:PartnerRoute</c> is "Off".</summary>
        public static bool IsPartnerRouteOn(IConfiguration configuration)
            => !string.Equals(configuration["Ingress:PartnerRoute"], "Off", StringComparison.OrdinalIgnoreCase);

        /// <summary>The middleware body.</summary>
        public async Task InvokeAsync(HttpContext context)
        {
            if (IsPartnerRouteOn(_configuration))
            {
                await _next(context);
                return;
            }

            _logger.LogWarning("ingress partner route off · {Method} {Path} -> 503", context.Request.Method, context.Request.Path.Value);
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            context.Response.Headers.CacheControl = "no-store";
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(OffSentence);
        }

        /// <summary>
        /// Writes the switch file (<paramref name="on"/> = "On" or "Off"). Atomic replace, so the
        /// watching host never reads half a file.
        /// </summary>
        public static void Write(string dataRoot, bool on)
        {
            var path = Path.Combine(dataRoot, FileName);
            var temp = path + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(new { Ingress = new { PartnerRoute = on ? "On" : "Off" } }));
            File.Move(temp, path, overwrite: true);
        }
    }
}
