using System;

namespace EnterpriseOps.Services.Queries
{
    /// <summary>The audit projection the Audit log dialog binds to.</summary>
    public sealed class AuditLogRow
    {
        public DateTime TimestampUtc { get; set; }
        public string Action { get; set; }
        public string Outcome { get; set; }
        public string ErrorCode { get; set; }
        public int? WorkOrderId { get; set; }
        public string UserId { get; set; }
        public string CorrelationId { get; set; }
        public string Detail { get; set; }
    }
}
