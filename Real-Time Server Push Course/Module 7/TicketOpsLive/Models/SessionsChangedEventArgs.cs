using System;

namespace TicketOpsLive
{
    public enum SessionChange { Joined, Left }

    /// <summary>
    /// Raised by <see cref="SessionRegistry"/> when a session registers or unregisters. Carries values only
    /// (ids and counts): the subscriber restores its own session context before touching any control.
    /// </summary>
    public sealed class SessionsChangedEventArgs : EventArgs
    {
        public SessionsChangedEventArgs(string sessionId, SessionChange change, int count)
        {
            SessionId = sessionId;
            Change = change;
            Count = count;
        }

        /// <summary>The session that joined or left.</summary>
        public string SessionId { get; }

        public SessionChange Change { get; }

        /// <summary>Number of live sessions after the change.</summary>
        public int Count { get; }
    }
}
