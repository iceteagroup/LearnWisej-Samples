using System;
using System.Globalization;
using TicketOps.Domain;

namespace TicketOps.Validation
{
    /// <summary>
    /// The central validator: takes a command, returns a result, touches no control and no database.
    /// Because it is pure it runs in three places with identical answers — the editor's pre-check when
    /// the user presses Save (UX), the service's re-check before the write (the real gate), and the
    /// test cases in docs/TestCases.md. Rules that need stored state or the caller's role are NOT here:
    /// see Domain/WorkOrderRules.
    /// </summary>
    public sealed class WorkOrderValidator
    {
        public const int MaxTitleLength = 120;
        public const decimal MinCost = 0m;
        public const decimal MaxCost = 10000m;
        public const double MinHours = 0;
        public const double MaxHours = 999;

        /// <summary>Injected so tests can pin "today"; production passes nothing.</summary>
        private readonly Func<DateTime> _today;

        public WorkOrderValidator() : this(null) { }

        public WorkOrderValidator(Func<DateTime> today)
        {
            _today = today ?? (() => DateTime.Today);
        }

        public ValidationResult Validate(SaveWorkOrderCommand cmd)
        {
            if (cmd == null) throw new ArgumentNullException(nameof(cmd));

            var result = new ValidationResult();
            var today = _today().Date;

            // Required fields — fixed in place, so they are field errors.
            if (string.IsNullOrWhiteSpace(cmd.Title))
                result.AddFieldError(nameof(cmd.Title), "Title is required.");
            else if (cmd.Title.Trim().Length > MaxTitleLength)
                result.AddFieldError(nameof(cmd.Title), $"Use at most {MaxTitleLength} characters.");

            if (string.IsNullOrWhiteSpace(cmd.AssigneeId))
                result.AddFieldError(nameof(cmd.AssigneeId), "Assign the order to a technician.");

            // Range — a data invariant, re-checked on the server no matter what the spin box allowed.
            if (cmd.EstimatedCost < MinCost || cmd.EstimatedCost > MaxCost)
                result.AddFieldError(nameof(cmd.EstimatedCost),
                    string.Format(CultureInfo.InvariantCulture, "Cost must be between ${0:N0} and ${1:N0}.", MinCost, MaxCost));

            if (cmd.EstimatedHours < MinHours || cmd.EstimatedHours > MaxHours)
                result.AddFieldError(nameof(cmd.EstimatedHours),
                    string.Format(CultureInfo.InvariantCulture, "Hours must be between {0:0} and {1:0}.", MinHours, MaxHours));

            // Cross-field — the due date only makes sense together with the target status.
            bool stillOpen = cmd.ToStatus == WorkOrderStatus.New || cmd.ToStatus == WorkOrderStatus.Assigned
                          || cmd.ToStatus == WorkOrderStatus.InProgress || cmd.ToStatus == WorkOrderStatus.OnHold;
            if (stillOpen && cmd.DueDate.Date < today)
                result.AddFieldError(nameof(cmd.DueDate), "Due date can't be in the past.");
            if (cmd.ToStatus == WorkOrderStatus.Closed && cmd.DueDate.Date > today)
                result.AddFieldError(nameof(cmd.DueDate), "A closed order cannot have a due date in the future.");

            // State machine — belongs to no single field, so it is a summary error. The validator only
            // knows the status the editor started from; the service repeats this against the stored one.
            if (WorkOrderTransitions.Classify(cmd.FromStatus, cmd.ToStatus) == TransitionKind.Illegal)
                result.AddSummaryError($"Cannot move a {cmd.FromStatus} order to {cmd.ToStatus}.");

            return result;
        }
    }
}
