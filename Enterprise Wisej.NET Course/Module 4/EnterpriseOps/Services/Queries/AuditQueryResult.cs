using System.Collections.Generic;

namespace EnterpriseOps.Services.Queries
{
    /// <summary>
    /// What the audit query returns. Reading the audit log is a permission, so the decision is made in
    /// the query service and reported here — the screen renders the answer, it does not make it.
    /// </summary>
    public sealed class AuditQueryResult
    {
        public bool Allowed { get; private set; }

        /// <summary>Why the caller may not read the log — safe to show as-is. Null when allowed.</summary>
        public string DeniedReason { get; private set; }

        public List<AuditLogRow> Rows { get; private set; }

        public static AuditQueryResult Ok(List<AuditLogRow> rows)
            => new AuditQueryResult { Allowed = true, Rows = rows };

        public static AuditQueryResult Denied(string reason)
            => new AuditQueryResult { Allowed = false, DeniedReason = reason, Rows = new List<AuditLogRow>() };
    }
}
