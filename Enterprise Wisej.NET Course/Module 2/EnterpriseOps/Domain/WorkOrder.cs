using System;

namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a work order — the same vocabulary every EnterpriseOps module uses.</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    /// <summary>
    /// Priority as the Advanced baseline names it. The Intermediate TicketOps Console called the middle
    /// value "Medium"; the migration inventory records the rename (Medium → Normal) as a mapped resource.
    /// </summary>
    public enum Priority { Low, Normal, High, Critical }

    public class Tenant
    {
        public string Id { get; set; }        // "contoso", "fabrikam", "northwind"
        public string Name { get; set; }
    }

    /// <summary>
    /// The entity. Services never hand it to the UI directly — screens receive projections
    /// (<see cref="Services.WorkQueueRow"/>) and typed results.
    /// </summary>
    public class WorkOrder
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

        /// <summary>Optimistic concurrency token: a stale command carries an older Version and is rejected.</summary>
        public int Version { get; set; }
    }
}
