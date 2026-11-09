using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AccessOps.Infrastructure
{
    /// <summary>
    /// Refuses a WebSocket upgrade whose <c>Origin</c> is not an AccessOps origin. The handshake
    /// carries the browser's cookies and is not covered by CORS, so a page on another site could
    /// otherwise open a socket to AccessOps. Registered ahead of <c>app.UseWisej()</c>; the
    /// ingress repeats the rule. A missing Origin is refused too (browsers always send one).
    /// </summary>
    public sealed class WebSocketOriginGuard
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _allowed;
        private readonly ILogger<WebSocketOriginGuard> _logger;

        /// <summary>Reads the exact allowed origins from <c>AccessOps:AllowedOrigins</c>.</summary>
        public WebSocketOriginGuard(RequestDelegate next, IConfiguration configuration, ILogger<WebSocketOriginGuard> logger)
        {
            _next = next;
            _logger = logger;
            _allowed = new HashSet<string>(
                (configuration.GetSection("AccessOps:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                    .Select(o => o.TrimEnd('/')),
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>The middleware body.</summary>
        public Task InvokeAsync(HttpContext context)
        {
            if (IsUpgrade(context.Request))
            {
                var origin = context.Request.Headers.Origin.ToString();
                if (!_allowed.Contains(origin))
                {
                    // the origin is attacker-controlled text: log only whether it was present
                    _logger.LogWarning("WebSocket upgrade refused: origin not allowed (present={Present}) path={Path}",
                        origin.Length > 0, context.Request.Path.Value);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            }
            return _next(context);
        }

        /// <summary>
        /// A WebSocket handshake, read from the headers. <c>HttpContext.WebSockets.IsWebSocketRequest</c>
        /// is false here: it needs the WebSockets middleware, which <c>UseWisej()</c> adds later in the
        /// pipeline. HTTP/1.1 sends <c>Upgrade: websocket</c>; HTTP/2 (RFC 8441) uses an extended
        /// CONNECT with <c>:protocol = websocket</c>, which ASP.NET Core exposes the same way.
        /// </summary>
        public static bool IsUpgrade(HttpRequest request)
        {
            if (string.Equals(request.Headers.Upgrade.ToString(), "websocket", StringComparison.OrdinalIgnoreCase))
                return true;
            return HttpMethods.IsConnect(request.Method)
                && string.Equals(request.Headers[":protocol"].ToString(), "websocket", StringComparison.OrdinalIgnoreCase);
        }
    }
}
