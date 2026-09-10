using System;

namespace TicketOps.Services
{
    /// <summary>
    /// The runtime facts the diagnostics page shows, behind an interface so the services take no Wisej.NET type.
    /// Infrastructure/WisejRuntimeInfo implements it over Wisej.Web.Application (ServerName, ServerPort,
    /// RuntimeMode, ProductVersion, SessionCount, IsWebSocket, SessionId); a test can fake it.
    /// </summary>
    public interface IRuntimeInfo
    {
        string ServerName { get; }
        int ServerPort { get; }

        /// <summary>true when the application runs in release mode (not design, debug or test).</summary>
        bool RuntimeMode { get; }
        string ProductVersion { get; }
        string Framework { get; }

        /// <summary>Live sessions on this node — the number a restart or a redeploy would evict.</summary>
        int SessionCount { get; }
        string SessionId { get; }

        /// <summary>false means the browser fell back to long-polling (the proxy did not forward the WebSocket upgrade).</summary>
        bool IsWebSocket { get; }
        TimeSpan Uptime { get; }
        DateTime ServerTimeUtc { get; }
        string TimeZoneId { get; }
    }
}
