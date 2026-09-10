using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TicketOps.Services;
using Wisej.Web;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// The one adapter that reads runtime facts from Wisej.NET (Application.ServerName, ServerPort, RuntimeMode,
    /// ProductVersion, SessionCount, SessionId, IsWebSocket) and hands them to the services as plain values.
    /// Keeping it here means DiagnosticsService and HealthCheckService compile without Wisej.NET and can be
    /// exercised with a fake IRuntimeInfo. Nothing it exposes is a secret.
    /// </summary>
    public sealed class WisejRuntimeInfo : IRuntimeInfo
    {
        // Process start is a fact about the whole node, identical for every session — the one kind of value a
        // static may hold. Per-user state never goes in a static.
        private static readonly DateTime ProcessStart = Process.GetCurrentProcess().StartTime;

        public string ServerName => string.IsNullOrEmpty(Application.ServerName) ? Environment.MachineName : Application.ServerName;
        public int ServerPort => Application.ServerPort;
        public bool RuntimeMode => Application.RuntimeMode;
        public string ProductVersion => string.IsNullOrEmpty(Application.ProductVersion) ? typeof(WisejRuntimeInfo).Assembly.GetName().Version.ToString() : Application.ProductVersion;
        public string Framework => RuntimeInformation.FrameworkDescription;
        public int SessionCount => Application.SessionCount;
        public string SessionId => Application.SessionId;
        public bool IsWebSocket => Application.IsWebSocket;
        public TimeSpan Uptime => DateTime.Now - ProcessStart;
        public DateTime ServerTimeUtc => DateTime.UtcNow;
        public string TimeZoneId => TimeZoneInfo.Local.Id;
    }
}
