using System;
using System.Globalization;
using TicketOps.Domain;

namespace TicketOps.Validation
{
    /// <summary>
    /// The save command: what the editor collected, read ONCE into an immutable object. The form's job
    /// ends at building it; validation, rules, persistence and auditing all operate on the command, never
    /// on live controls — which is what lets a CSV import or a background job reuse the same pipeline.
    ///
    /// Notice what is NOT here: the actor's role. A command is client input; anyone can build one. The
    /// role is read from the server-side SessionContext inside the service.
    /// </summary>
    public sealed class SaveWorkOrderCommand
    {
        /// <summary>null for a new work order.</summary>
        public int? Id { get; init; }
        public string Title { get; init; }
        public string AssigneeId { get; init; }
        public WorkOrderPriority Priority { get; init; }
        public DateTime DueDate { get; init; }
        public decimal EstimatedCost { get; init; }
        public double EstimatedHours { get; init; }
        /// <summary>The status the editor showed when the user started (a hint — the service re-reads the stored one).</summary>
        public WorkOrderStatus FromStatus { get; init; }
        public WorkOrderStatus ToStatus { get; init; }

        public override string ToString()
            => string.Format(CultureInfo.InvariantCulture,
                "{{id:{0}, title:\"{1}\", assignee:\"{2}\", due:{3:yyyy-MM-dd}, cost:{4}, hours:{5}, {6}→{7}}}",
                Id.HasValue ? Id.Value.ToString(CultureInfo.InvariantCulture) : "new",
                Title, AssigneeId, DueDate, EstimatedCost, EstimatedHours, FromStatus, ToStatus);
    }
}
