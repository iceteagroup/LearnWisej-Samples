using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// One row per command outcome. Committed outcomes are written inside the command's transaction;
    /// rejected outcomes are written in their own transaction after the rollback (an audit row that
    /// rolls back with the failure it records is no audit at all).
    /// </summary>
    public class AuditEntry
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public int? WorkOrderId { get; set; }
        public string Action { get; set; }        // Create · Update · Approve
        public string Outcome { get; set; }       // Committed · Rejected
        public string ErrorCode { get; set; }     // null when committed
        public string UserId { get; set; }
        public string CorrelationId { get; set; }
        public string Detail { get; set; }        // never a stack trace, never SQL
        public DateTime TimestampUtc { get; set; }
    }
}
