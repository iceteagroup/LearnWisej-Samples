using System;

namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a work order (shared vocabulary of every EnterpriseOps module).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    /// <summary>A customer organisation the field-service company works for. Every row is tenant-scoped.</summary>
    public class Tenant
    {
        public string Id;      // "contoso", "fabrikam", "northwind"
        public string Name;
    }

    /// <summary>
    /// The aggregate root of the domain. It never crosses the wire: the screens see projections
    /// (<c>WorkQueueRow</c>, <c>HistoryEntryView</c>) and the components see their own DTOs
    /// (<c>TimelineItem</c>, <c>ChartSegment</c>).
    /// </summary>
    public class WorkOrder
    {
        public int Id;
        public string TenantId;
        public string Title;
        public string Customer;
        public string Site;
        public WorkOrderStatus Status;
        public Priority Priority;
        public string AssignedTo;
        public DateTime CreatedUtc;
        public DateTime? DueUtc;
        public int Version;            // optimistic concurrency token
    }
}
