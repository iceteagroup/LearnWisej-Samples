using System;

namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a field-service work order (shared vocabulary of all 14 modules).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    /// <summary>A customer organisation. Every query and command is scoped to one tenant.</summary>
    public sealed class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString() => Name;
    }

    /// <summary>
    /// The aggregate the Command Center manages. <see cref="Version"/> is the optimistic-concurrency
    /// token: a command carries the version it saw, and the store rejects the write when it moved on.
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public WorkOrderStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? DueUtc { get; set; }
        public DateTime? CompletedUtc { get; set; }
        public int Version { get; set; }

        /// <summary>Approval is a domain rule, not a UI rule: only work that is in progress or escalated can be signed off.</summary>
        public bool CanBeApproved => Status == WorkOrderStatus.InProgress || Status == WorkOrderStatus.Escalated;

        public WorkOrder Clone() => (WorkOrder)MemberwiseClone();
    }
}
