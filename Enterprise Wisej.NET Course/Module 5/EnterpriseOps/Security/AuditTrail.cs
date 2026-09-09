using System;
using System.Collections.Generic;
using System.Globalization;

namespace EnterpriseOps.Security
{
    public sealed class AuditEntry
    {
        public DateTime AtUtc { get; set; }
        public string CorrelationId { get; set; }
        public string TenantId { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public int WorkOrderId { get; set; }
        public string Outcome { get; set; }

        public override string ToString() =>
            string.Format(CultureInfo.InvariantCulture, "{0:HH:mm:ss} {1} {2}@{3} {4} WO-{5} → {6}",
                AtUtc, CorrelationId, User, TenantId, Action, WorkOrderId, Outcome);
    }

    /// <summary>
    /// Append-only audit log — the "audited" in the walkthrough's status line. Like the work-order store it
    /// stands in for a table, so it is process-wide; every batch row writes exactly one entry, success or not.
    /// </summary>
    public sealed class AuditTrail
    {
        public static AuditTrail Instance { get; } = new AuditTrail();

        private readonly object _gate = new object();
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public int Count
        {
            get { lock (_gate) return _entries.Count; }
        }

        public AuditEntry Record(string correlationId, string tenantId, string user, string action, int workOrderId, string outcome)
        {
            var entry = new AuditEntry
            {
                AtUtc = DateTime.UtcNow,
                CorrelationId = correlationId,
                TenantId = tenantId,
                User = user,
                Action = action,
                WorkOrderId = workOrderId,
                Outcome = outcome,
            };
            lock (_gate)
                _entries.Add(entry);
            return entry;
        }

        public IReadOnlyList<AuditEntry> ForCorrelation(string correlationId)
        {
            lock (_gate)
                return _entries.FindAll(e => e.CorrelationId == correlationId);
        }
    }
}
