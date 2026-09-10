using System;
using System.Collections.Concurrent;

namespace OrderDesk.Services
{
    /// <summary>
    /// Process-wide list of live sessions (id → start time). Registered in Program.Main, removed on
    /// Application.ApplicationExit. Read by the /health endpoint, which runs outside any Wisej.NET
    /// session and therefore cannot call Application.SessionCount itself.
    /// A static, but not per-user state: the collection is concurrent and holds no user data.
    /// </summary>
    public static class SessionRegistry
    {
        private static readonly ConcurrentDictionary<string, DateTime> Sessions = new ConcurrentDictionary<string, DateTime>();
        private static readonly DateTime ProcessStart = DateTime.UtcNow;

        public static int Count => Sessions.Count;
        public static TimeSpan Uptime => DateTime.UtcNow - ProcessStart;

        public static void Register(string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
                Sessions[sessionId] = DateTime.UtcNow;
        }

        public static void Unregister(string sessionId)
        {
            if (!string.IsNullOrEmpty(sessionId))
                Sessions.TryRemove(sessionId, out _);
        }
    }
}
