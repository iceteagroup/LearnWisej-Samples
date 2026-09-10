using System;
using System.Collections.Generic;
using TicketOps.Security;
using TicketOps.Services;

namespace TicketOps.Infrastructure
{
    /// <summary>
    /// In-memory, append-only implementation of <see cref="IAuditService"/>. Every entry is also written to
    /// the session <see cref="ILog"/> as an "[AUDIT]" line, so the activity trace shows the paper trail next
    /// to the boundary crossings that produced it. Production forwards the same entries to a durable,
    /// tamper-evident sink (database table, SIEM); the writers do not change.
    ///
    /// Thread-safe, because Module 7-style background work may audit from a worker thread.
    /// </summary>
    public sealed class AuditLog : IAuditService
    {
        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();
        private readonly ILog _log;

        public AuditLog(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IReadOnlyList<AuditEntry> Entries
        {
            get { lock (_gate) return _entries.ToArray(); }
        }

        public event EventHandler<AuditEntry> EntryAdded;

        public void Success(IUserContext actor, string action, string target, string detail = null)
            => Write(actor, action, target, AuditOutcome.Success, detail);

        public void Denied(IUserContext actor, string action, string target, string reason)
            => Write(actor, action, target, AuditOutcome.Denied, reason);

        public void Failed(IUserContext actor, string action, string target, string reason)
            => Write(actor, action, target, AuditOutcome.Failed, reason);

        private void Write(IUserContext actor, string action, string target, AuditOutcome outcome, string detail)
        {
            var entry = new AuditEntry
            {
                TimeUtc = DateTime.UtcNow,
                Actor = actor?.UserName ?? "(anonymous)",
                ActorRoles = actor == null ? "-" : string.Join(", ", actor.Roles),
                Action = action,
                Target = target,
                Outcome = outcome,
                Detail = detail
            };

            lock (_gate) _entries.Add(entry);      // append-only: there is no Remove and no Clear

            string line = Format(entry);
            switch (outcome)
            {
                case AuditOutcome.Success:
                    _log.Info(LogLayer.Infrastructure, "AuditLog", line);
                    break;
                case AuditOutcome.Denied:
                    _log.Warn(LogLayer.Infrastructure, "AuditLog", line);
                    break;
                default:
                    _log.Error(LogLayer.Infrastructure, "AuditLog", null, line);
                    break;
            }

            EntryAdded?.Invoke(this, entry);
        }

        /// <summary>"[AUDIT] ⛔ DENIED DeleteTicket #2002 — l.romero (Technician): needs Supervisor or Admin".</summary>
        public static string Format(AuditEntry e)
        {
            string mark = e.Outcome switch
            {
                AuditOutcome.Success => "✓",
                AuditOutcome.Denied => "⛔ DENIED",
                _ => "✖ FAILED"
            };
            string who = e.ActorRoles == "-" ? e.Actor : $"{e.Actor} ({e.ActorRoles})";
            string detail = string.IsNullOrEmpty(e.Detail) ? string.Empty : $": {e.Detail}";
            return $"[AUDIT] {mark} {e.Action} {e.Target} — {who}{detail}";
        }
    }
}
