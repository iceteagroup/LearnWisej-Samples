using System;

namespace EnterpriseOps.Domain
{
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    public class Tenant
    {
        public string Id;
        public string Name;
    }

    /// <summary>
    /// The shared EnterpriseOps entity (cookbook vocabulary). The wizard never receives it:
    /// the work-order screen shows a WorkQueueRow projection and the workflow reads it from the store.
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
        public int Version;                     // optimistic concurrency token

        public string Number => $"WO-{Id}";
    }
}
