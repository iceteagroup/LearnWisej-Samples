using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderDesk.Services
{
    /// <summary>One audit entry: who (user + session) did what, when — recorded on the server.</summary>
    public sealed class AuditEntry
    {
        public DateTime Time { get; set; }
        public string User { get; set; }
        public string SessionId { get; set; }
        public string Action { get; set; }

        /// <summary>The first 8 characters of the session id, as every module prints it.</summary>
        public string ShortSession => string.IsNullOrEmpty(SessionId) ? "--------" : (SessionId.Length > 8 ? SessionId.Substring(0, 8) : SessionId);

        public override string ToString() => Time.ToString("HH:mm:ss") + "  " + (User ?? "-").PadRight(6) + " · " + ShortSession + " · " + Action;
    }

    /// <summary>
    /// The process-wide audit log behind the dashboard's Activity feed. Every session writes to the same
    /// list (this IS shared state, on purpose: the feed is multi-user) and every open page subscribes to
    /// <see cref="Added"/>. The subscriber runs on the writer's thread, so a page belonging to another
    /// session pushes the entry into its own browser with Application.StartTask + Application.Update(page, …).
    /// Bounded to the last 500 entries; the store is locked, never per-user.
    /// </summary>
    public static class AuditLog
    {
        private const int Capacity = 500;
        private static readonly object Gate = new object();
        private static readonly List<AuditEntry> Entries = new List<AuditEntry>();

        /// <summary>Raised after an entry is stored. Handlers run synchronously on the writer's thread.</summary>
        public static event Action<AuditEntry> Added;

        public static AuditEntry Add(string user, string sessionId, string action)
        {
            var entry = new AuditEntry { Time = DateTime.Now, User = user ?? "-", SessionId = sessionId ?? "", Action = action ?? "" };
            lock (Gate)
            {
                Entries.Add(entry);
                if (Entries.Count > Capacity) Entries.RemoveRange(0, Entries.Count - Capacity);
            }

            var handlers = Added;
            if (handlers != null)
            {
                foreach (Action<AuditEntry> h in handlers.GetInvocationList())
                {
                    try { h(entry); }
                    catch { /* a broken subscriber (disposed page, closed socket) must not break the writer */ }
                }
            }
            return entry;
        }

        public static List<AuditEntry> Recent(int count = 50)
        {
            lock (Gate) return Entries.Skip(Math.Max(0, Entries.Count - count)).ToList();
        }

        public static int Count { get { lock (Gate) return Entries.Count; } }
    }
}
