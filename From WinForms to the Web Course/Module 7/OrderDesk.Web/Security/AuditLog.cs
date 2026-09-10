using System;
using System.Collections.Generic;
using System.Globalization;
using OrderDesk.Diagnostics;
using Wisej.Web;

namespace OrderDesk.Security
{
    /// <summary>One audited action: when, who, from which session, what, and whether it was allowed.</summary>
    public sealed class AuditEntry
    {
        public DateTime WhenUtc { get; set; }
        public string User { get; set; }
        public string Session { get; set; }
        public string Action { get; set; }
        public string Detail { get; set; }
        public bool Allowed { get; set; }

        public override string ToString() =>
            $"{WhenUtc.ToLocalTime().ToString("HH:mm:ss", CultureInfo.InvariantCulture)}  {(Allowed ? "✓" : "✕")}  {User,-6} {Session}  {Action}  {Detail}";
    }

    /// <summary>
    /// Process-wide, append-only, thread-safe audit log: who did what, when, from which session — recorded
    /// on the server, never trusted from the browser. Every session's dashboard reads the same list, which
    /// is exactly why it is a static: it is shared data behind a lock, like a database table, not per-user
    /// state. In production the same Record call writes to a table or a log sink.
    /// </summary>
    public static class AuditLog
    {
        private static readonly object Gate = new object();
        private static readonly List<AuditEntry> Entries = new List<AuditEntry>();
        private static long _version;

        /// <summary>Increments on every Record; the dashboard timer refreshes only when it changed.</summary>
        public static long Version { get { lock (Gate) return _version; } }

        public static int Count { get { lock (Gate) return Entries.Count; } }

        public static void Record(string action, string detail, bool allowed, string userOverride = null)
        {
            var entry = new AuditEntry
            {
                WhenUtc = DateTime.UtcNow,
                User = userOverride ?? AuthService.Current?.UserName ?? "-",
                Session = ShortSession(),
                Action = action,
                Detail = detail,
                Allowed = allowed
            };
            lock (Gate)
            {
                Entries.Add(entry);
                _version++;
            }
            AppLog.Write(allowed ? "AUDIT" : "AUDIT-DENIED", $"{entry.User} [{entry.Session}] {action}: {detail}");
        }

        /// <summary>The newest entries first, at most <paramref name="max"/>.</summary>
        public static IList<AuditEntry> Snapshot(int max)
        {
            lock (Gate)
            {
                var list = new List<AuditEntry>(Math.Min(max, Entries.Count));
                for (int i = Entries.Count - 1; i >= 0 && list.Count < max; i--)
                    list.Add(Entries[i]);
                return list;
            }
        }

        private static string ShortSession()
        {
            try
            {
                var id = Application.SessionId ?? "";
                return id.Length > 6 ? id.Substring(0, 6) : (id.Length == 0 ? "-" : id);
            }
            catch (Exception)
            {
                return "-";
            }
        }
    }
}
