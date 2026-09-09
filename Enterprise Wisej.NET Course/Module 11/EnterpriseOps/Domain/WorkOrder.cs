using System;

namespace EnterpriseOps.Domain
{
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    /// <summary>A customer of the field-service company. Every work order belongs to exactly one tenant.</summary>
    public class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static readonly Tenant Contoso = new Tenant { Id = "contoso", Name = "Contoso Facilities" };
        public static readonly Tenant Fabrikam = new Tenant { Id = "fabrikam", Name = "Fabrikam Energy" };
        public static readonly Tenant Northwind = new Tenant { Id = "northwind", Name = "Northwind Logistics" };

        public static readonly Tenant[] All = { Contoso, Fabrikam, Northwind };
    }

    /// <summary>
    /// The domain entity. It never reaches the UI: services project it into WorkQueueRow.
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

        /// <summary>Optimistic concurrency token (Module 4); carried here so the entity shape is the shared one.</summary>
        public int Version { get; set; }
    }
}
