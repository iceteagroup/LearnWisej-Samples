using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// What the diagnostics page shows a Supervisor at 2 a.m.: runtime facts about this node plus the live
    /// health report. Plain data — the values come from IRuntimeInfo (an adapter over Wisej.NET) so the type
    /// itself has no UI dependency and can be rendered, logged or serialized anywhere.
    /// Nothing here is a secret: no connection string, key, token or another user's data.
    /// </summary>
    public sealed class DiagnosticsSnapshot
    {
        public string ServerName { get; set; }
        public int ServerPort { get; set; }
        public bool RuntimeMode { get; set; }
        public string ProductVersion { get; set; }
        public string Framework { get; set; }
        public int SessionCount { get; set; }
        public string SessionId { get; set; }
        public bool IsWebSocket { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime ServerTimeUtc { get; set; }
        public string TimeZoneId { get; set; }

        public string OperatorName { get; set; }
        public OperatorRole OperatorRole { get; set; }

        public HealthReport Health { get; set; }
        public DateTime TakenAt { get; set; }

        /// <summary>The first 8 characters are enough to correlate a trace line with a session; the full id stays in the log.</summary>
        public string ShortSessionId
            => string.IsNullOrEmpty(SessionId) ? "—" : SessionId.Length <= 8 ? SessionId : SessionId.Substring(0, 8);
    }
}
