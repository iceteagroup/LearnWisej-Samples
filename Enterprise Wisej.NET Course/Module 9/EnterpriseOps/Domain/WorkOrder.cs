using System;

namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a field-service work order (shared vocabulary of the EnterpriseOps course).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    public class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static readonly Tenant[] All =
        {
            new Tenant { Id = "contoso",   Name = "Contoso Field Services" },
            new Tenant { Id = "fabrikam",  Name = "Fabrikam Maintenance" },
            new Tenant { Id = "northwind", Name = "Northwind Utilities" },
        };
    }

    /// <summary>
    /// The domain entity. It never crosses the wire: the palette only ever sees a
    /// WorkQueueRow projection (Services) and a string entity id (Interop).
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

        /// <summary>Optimistic concurrency token: bumped by every state change.</summary>
        public int Version { get; set; }

        public bool IsTerminal => Status == WorkOrderStatus.Completed || Status == WorkOrderStatus.Cancelled;
    }
}
