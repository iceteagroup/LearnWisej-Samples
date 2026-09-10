using System;

namespace EnterpriseOps.Domain
{
    /// <summary>The course-wide work order. TenantId is part of the record, not of the query — the tenant guard compares it.</summary>
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
        public int Version { get; set; }

        /// <summary>Set when an approval decision has been recorded, so the screen can show who approved.</summary>
        public string ApprovedBy { get; set; }

        public override string ToString() => $"WO-{Id:0000} {Title} ({TenantId})";
    }
}
