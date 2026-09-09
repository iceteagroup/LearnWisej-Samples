using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The work order entity as the store keeps it. <see cref="Version"/> is the optimistic concurrency
    /// token: read when the record is loaded for editing, compared when the record is saved, incremented
    /// by every successful save. Entities never reach the UI — services hand out projections and edit models.
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

        /// <summary>Optimistic concurrency token (a SQL rowversion / EF Core [Timestamp] column in Module 4).</summary>
        public int Version { get; set; }

        public string ModifiedBy { get; set; }
        public DateTime ModifiedUtc { get; set; }

        /// <summary>The store hands out copies, never its own instances, so a session cannot mutate shared state.</summary>
        public WorkOrder Clone() => (WorkOrder)MemberwiseClone();
    }
}
