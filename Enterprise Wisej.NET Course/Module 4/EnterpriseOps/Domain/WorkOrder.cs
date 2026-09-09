using System;

namespace EnterpriseOps.Domain
{
    /// <summary>
    /// The work-order aggregate — an EF Core entity in this module. It never crosses the data boundary:
    /// screens see <see cref="EnterpriseOps.Services.Queries.WorkQueueRow"/> projections and
    /// <see cref="EnterpriseOps.Services.Commands.CommandResult"/>, never this class.
    /// </summary>
    public class WorkOrder
    {
        public int Id { get; set; }
        public string TenantId { get; set; }

        /// <summary>Business number, unique per tenant ("WO-2002"). The unique index is what produces the
        /// UNIQUE-constraint failure path (WO_NUMBER_IN_USE).</summary>
        public string Number { get; set; }

        public string Title { get; set; }
        public string Customer { get; set; }
        public string Site { get; set; }
        public WorkOrderStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? DueUtc { get; set; }

        /// <summary>Optimistic-concurrency token: every committed change bumps it (v8 → v9).
        /// Configured as a concurrency token in <c>EnterpriseOpsDbContext</c>.</summary>
        public int Version { get; set; }

        public string ApprovedBy { get; set; }
        public DateTime? ApprovedUtc { get; set; }
        public string ApprovalComment { get; set; }
    }
}
