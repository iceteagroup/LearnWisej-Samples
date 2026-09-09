using System;

namespace EnterpriseOps.Domain
{
    /// <summary>Lifecycle of a work order. "Open" in the work queue filter means anything but Completed / Cancelled.</summary>
    public enum WorkOrderStatus { New, Assigned, InProgress, OnHold, Escalated, Completed, Cancelled }

    public enum Priority { Low, Normal, High, Critical }

    public sealed class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// The domain entity. It knows nothing about the grid: no display text, no "age in days", no
    /// permission flags. Those belong to the search projection (<c>WorkQueueRow</c>) — see
    /// docs/SearchProjectionModel.md for why the two change for different reasons.
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

        /// <summary>Optimistic concurrency token: every write bumps it; a batch row with a stale version fails.</summary>
        public int Version { get; set; }

        /// <summary>Certification the assigned technician must hold (null = none required).</summary>
        public string RequiredCertification { get; set; }

        /// <summary>Id of an approval in flight. While it is set the order is locked for reassignment.</summary>
        public string OpenApprovalId { get; set; }

        public string Number => "WO-" + Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

        public bool IsOpen => Status != WorkOrderStatus.Completed && Status != WorkOrderStatus.Cancelled;
    }
}
