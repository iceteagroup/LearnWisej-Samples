using System;

namespace EnterpriseOps.Domain
{
    /// <summary>The shared EnterpriseOps vocabulary (same names in all 14 modules).</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    public class Tenant
    {
        public string Id;
        public string Name;
    }

    /// <summary>
    /// A field-service work order. Module 12 only needs it for the smoke test's "known query returns
    /// rows" check and the health probe's database check — the release must prove the data path works.
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
        public int Version;     // optimistic concurrency token
    }
}
