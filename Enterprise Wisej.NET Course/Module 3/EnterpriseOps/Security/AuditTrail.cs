using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Security
{
    public sealed class AuditEntry
    {
        public DateTimeOffset At { get; init; }
        public string CorrelationId { get; init; }
        public string TenantId { get; init; }
        public string UserId { get; init; }
        public string Action { get; init; }
        public string Detail { get; init; }

        public override string ToString() => $"{At:HH:mm:ss.fff} {CorrelationId} {UserId}@{TenantId} {Action} — {Detail}";
    }

    /// <summary>
    /// Append-only audit trail shared by every session — the in-memory stand-in for an audit table. It is
    /// application-scoped on purpose (an audit that lived in one session would vanish with it), so it is
    /// documented as shared state, every write goes through one lock, and every read is tenant-keyed.
    /// </summary>
    public sealed class AuditTrail
    {
        [SharedState(StateScope.Application,
            holds: "append-only audit entries for every tenant, each tagged with tenant + user + correlation id",
            synchronization: "single lock around append and snapshot; reads are filtered by tenant")]
        public static readonly AuditTrail Shared = new AuditTrail();

        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public void Record(CommandContext context, string action, string detail)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var entry = new AuditEntry
            {
                At = DateTimeOffset.UtcNow,
                CorrelationId = context.CorrelationId,
                TenantId = context.TenantId,
                UserId = context.UserId,
                Action = action,
                Detail = detail,
            };

            lock (_gate)
                _entries.Add(entry);
        }

        /// <summary>Entries for one tenant only — the trail is shared, the view of it is not.</summary>
        public IReadOnlyList<AuditEntry> Snapshot(string tenantId)
        {
            lock (_gate)
                return _entries.Where(e => StringComparer.Ordinal.Equals(e.TenantId, tenantId)).ToList();
        }

        public int Count
        {
            get { lock (_gate) return _entries.Count; }
        }
    }
}
