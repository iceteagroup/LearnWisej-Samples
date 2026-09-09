using System;

namespace EnterpriseOps.Domain
{
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    /// <summary>A customer of the multi-tenant field-service company ("contoso", "fabrikam", "northwind").</summary>
    public class Tenant
    {
        public string Id;
        public string Name;
    }

    /// <summary>
    /// The system-of-record entity. It lives on the server (Data/FakeWorkOrderRepository); the device only ever
    /// holds a <see cref="Hybrid.CachedWorkOrder"/> copy of the technician's own assignments.
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

        /// <summary>Optimistic concurrency token: every server-side change increments it. An offline command
        /// carries the version it was based on, and a mismatch on replay is a sync conflict.</summary>
        public int Version;

        public string Notes;
        public string LastChangedBy;
        public DateTime LastChangedUtc;

        public string Code => "WO-" + Id;

        public WorkOrder Clone() => (WorkOrder)MemberwiseClone();
    }
}
