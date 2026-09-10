using System.Globalization;
using TicketOps.Validation;

namespace TicketOps.Domain
{
    /// <summary>Who is acting. Comes from the server-side <c>SessionContext</c>, never from the command.</summary>
    public enum UserRole
    {
        Technician,
        Supervisor
    }

    /// <summary>
    /// The business and authorization rules that need state the browser has no authority over: the
    /// status the store actually holds and the role the server knows the caller has. They run in the
    /// service AFTER the pure validator and BEFORE anything is written. Pure C#, no UI, no repository:
    /// the service loads the stored record and hands it in, so the same rules run for an import job.
    /// </summary>
    public static class WorkOrderRules
    {
        /// <summary>A cost above this needs a Supervisor (role-based restriction).</summary>
        public const decimal ApprovalThreshold = 2500m;

        public static ValidationResult Check(WorkOrder stored, SaveWorkOrderCommand command, UserRole role)
        {
            var result = new ValidationResult();
            if (command == null)
                return result;

            var from = stored != null ? stored.Status : command.FromStatus;

            // 1. A closed order is frozen for everyone but a Supervisor, who may only reopen it.
            if (from == WorkOrderStatus.Closed)
            {
                if (role != UserRole.Supervisor)
                    result.AddSummaryError("Closed work orders cannot be edited. Ask a Supervisor to reopen it.");
                else if (command.ToStatus == WorkOrderStatus.Closed)
                    result.AddSummaryError("This order is closed. Reopen it (status → Assigned) before editing.");
            }

            // 2. The transition is re-checked against the STORED status, not the one the editor remembered.
            switch (WorkOrderTransitions.Classify(from, command.ToStatus))
            {
                case TransitionKind.Illegal:
                    result.AddSummaryError($"Cannot move a {from} order to {command.ToStatus}.");
                    break;
                case TransitionKind.Privileged when role != UserRole.Supervisor:
                    result.AddSummaryError("Only a Supervisor may reopen a closed work order.");
                    break;
            }

            // 3. Role-based restriction on cost. The UI may hide the field; this line is the enforcement.
            if (command.EstimatedCost > ApprovalThreshold && role != UserRole.Supervisor)
                result.AddSummaryError(string.Format(CultureInfo.InvariantCulture,
                    "Only a Supervisor may set a cost above ${0:N0}.", ApprovalThreshold));

            return result;
        }
    }
}
