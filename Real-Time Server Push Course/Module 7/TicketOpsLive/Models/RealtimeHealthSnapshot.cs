using System;

namespace TicketOpsLive.Models
{
    /// <summary>
    /// What an operator needs to know about the real-time path of ONE session, taken as a consistent snapshot
    /// (the walkthrough's <c>RealtimeHealthSnapshot</c> / <c>RenderHealth</c> pair).
    ///
    /// The values are deliberately cheap to compute: a health panel that costs more than the work it watches is
    /// its own problem. <see cref="UpdatesPerMinute"/> is measured, not estimated — the page keeps the timestamp
    /// of every push it made in the last minute.
    /// </summary>
    public sealed class RealtimeHealthSnapshot
    {
        /// <summary>True when this session has a live WebSocket, i.e. pushes leave the server immediately.</summary>
        public bool WebSocketExpected { get; set; }

        /// <summary>True while this session has asked the browser to poll (no WebSocket and work in flight).</summary>
        public bool PollingFallbackEnabled { get; set; }

        /// <summary>Hub subscribers + registered sessions: what a leak would make grow without bound.</summary>
        public int ActiveSubscriptions { get; set; }

        /// <summary>Pushes this session made in the last 60 seconds — the cadence, measured.</summary>
        public int UpdatesPerMinute { get; set; }

        /// <summary>When the server last pushed anything to this session (UTC), or null if it never did.</summary>
        public DateTime? LastServerEventUtc { get; set; }

        /// <summary>Loops that are running right now in this session (heartbeat, import, simulator, feed).</summary>
        public int RunningLoops { get; set; }

        /// <summary>Human-readable list of those loops, for the label and the trace.</summary>
        public string RunningLoopNames { get; set; } = "none";

        /// <summary>Seconds since the last push, or -1 when nothing was ever pushed.</summary>
        public double SecondsSinceLastEvent =>
            LastServerEventUtc.HasValue ? Math.Round((DateTime.UtcNow - LastServerEventUtc.Value).TotalSeconds, 1) : -1;
    }
}
