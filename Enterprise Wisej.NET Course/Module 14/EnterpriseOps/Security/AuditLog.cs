using System;
using System.Collections.Generic;

namespace EnterpriseOps.Security
{
    /// <summary>One audit line: who did what to which record, under which correlation id, and whether it was allowed.</summary>
    public sealed class AuditEntry
    {
        public DateTime AtUtc { get; set; }
        public string TenantId { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Target { get; set; }
        public bool Allowed { get; set; }
        public string Detail { get; set; }
        public string CorrelationId { get; set; }

        public override string ToString()
            => $"{AtUtc:HH:mm:ss} {TenantId}/{User} {Action} {Target} → {(Allowed ? "ok" : "DENIED")} [{CorrelationId}] {Detail}";
    }

    /// <summary>
    /// Append-only audit trail. In production this is a table written in the same transaction as the
    /// command (Module 10); here it is a per-session list so the trace can show the line being written.
    /// </summary>
    public sealed class AuditLog
    {
        private readonly List<AuditEntry> _entries = new List<AuditEntry>();

        public IReadOnlyList<AuditEntry> Entries => _entries;

        public AuditEntry Write(string tenantId, string user, string action, string target, bool allowed, string detail, string correlationId)
        {
            var entry = new AuditEntry
            {
                AtUtc = DateTime.UtcNow,
                TenantId = tenantId,
                User = user,
                Action = action,
                Target = target,
                Allowed = allowed,
                Detail = detail,
                CorrelationId = correlationId,
            };
            _entries.Add(entry);
            return entry;
        }
    }
}
