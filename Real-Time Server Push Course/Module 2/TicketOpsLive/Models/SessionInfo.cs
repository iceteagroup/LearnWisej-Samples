using System;

namespace TicketOpsLive
{
    /// <summary>
    /// What the global registry keeps about a session: an id and a start time. Never a page, a control
    /// or an application context — a global service that stored those would keep dead sessions alive.
    /// </summary>
    public sealed class SessionInfo
    {
        public SessionInfo(string sessionId, DateTime startedAt)
        {
            SessionId = sessionId;
            StartedAt = startedAt;
        }

        /// <summary>Application.SessionId of the session (a GUID string — one per browser tab; Application.ClientId is shared by every tab of one browser).</summary>
        public string SessionId { get; }

        /// <summary>Server time at which MainPage registered the session.</summary>
        public DateTime StartedAt { get; }

        public override string ToString() => $"{SessionId} since {StartedAt:HH:mm:ss}";
    }
}
