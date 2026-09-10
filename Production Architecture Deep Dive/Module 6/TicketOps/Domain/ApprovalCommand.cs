using System;

namespace TicketOps.Domain
{
    /// <summary>
    /// The confirmed intent as plain data: "what should the service do now?". It is neither the dialog
    /// result (owned by the UI, describes what the user decided) nor the work order (the thing being
    /// changed). The service builds it from a confirmed <c>ApprovalDialogResult</c> plus the facts the
    /// dialog must never supply — who is deciding — and executes it as one transaction. Because it is
    /// plain data it doubles as the audit record and can be replayed by a test or a bulk job without a UI.
    /// </summary>
    public sealed class ApprovalCommand
    {
        public int WorkOrderId { get; init; }
        public ApprovalAction Action { get; init; }
        public string Comments { get; init; }
        public string DecidedBy { get; init; }
        public DateTime DecidedAtUtc { get; init; }

        public override string ToString()
            => $"{{workOrder:{WorkOrderId}, action:{Action}, comments:\"{Comments}\", by:{DecidedBy}}}";
    }

    /// <summary>One line of the approval audit trail — written in the same transaction as the status change.</summary>
    public sealed class ApprovalRecord
    {
        public int WorkOrderId { get; init; }
        public string WorkOrderNumber { get; init; }
        public ApprovalAction Action { get; init; }
        public string Comments { get; init; }
        public string DecidedBy { get; init; }
        public DateTime DecidedAtUtc { get; init; }

        public override string ToString() => $"{WorkOrderNumber} {Action} by {DecidedBy}";
    }
}
