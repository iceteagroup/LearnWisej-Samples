using System;

namespace EnterpriseOps.Domain
{
    /// <summary>The shared EnterpriseOps vocabulary (same names in every module of the course).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    public sealed class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// A work order for a multi-tenant field-service company. Imports create or update these rows;
    /// <see cref="ExternalRef"/> is the idempotency key an import row carries (same ref twice → one record).
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public string ExternalRef { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public string AssetCode { get; set; }
        public WorkOrderStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? DueUtc { get; set; }
        public int Version { get; set; }   // optimistic concurrency token (Module 4/5)
    }
}
