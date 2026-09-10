using System;

namespace TicketOps.Domain
{
    public enum WorkOrderStatus
    {
        Pending,
        Approved,
        Rejected
    }

    /// <summary>
    /// The decision an approver can take on a work order. An enum, not a string, so the dialog result,
    /// the command and the domain rule all compare the same value and a typo cannot become a third action.
    /// </summary>
    public enum ApprovalAction
    {
        Approve,
        Reject
    }

    /// <summary>
    /// A work order waiting for an approver. Plain C# with no UI dependency: it would compile in a class
    /// library that never references Wisej.NET, which is the test of a good Domain type.
    ///
    /// The domain rule of this module is <see cref="CanDecide"/>: only a pending work order can be approved
    /// or rejected. The dialog does not know that rule; the service asks the work order, so the rule holds
    /// whether the command came from the dialog, from a bulk job or from an API endpoint.
    /// </summary>
    public sealed class WorkOrder
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Requester { get; set; }
        public decimal Amount { get; set; }
        public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Pending;
        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public string DecidedBy { get; set; }
        public DateTime? DecidedAtUtc { get; set; }
        public string DecisionComments { get; set; }

        public bool IsPending => Status == WorkOrderStatus.Pending;

        /// <summary>The status rule: a work order is decided once. Reusable by any caller, testable without a browser.</summary>
        public bool CanDecide(out string reason)
        {
            if (Status != WorkOrderStatus.Pending)
            {
                reason = $"Only a pending work order can be approved or rejected ({Number} is already {Status.ToString().ToLowerInvariant()}).";
                return false;
            }

            reason = null;
            return true;
        }

        /// <summary>
        /// Applies a confirmed decision. Called by the service inside its transaction — never by the dialog,
        /// which only gathers intent. Throws if the rule says no, because reaching this point without asking
        /// <see cref="CanDecide"/> first is a programming error, not a user mistake.
        /// </summary>
        public void Decide(ApprovalAction action, string comments, string decidedBy, DateTime decidedAtUtc)
        {
            if (!CanDecide(out string reason))
                throw new InvalidOperationException(reason);

            Status = action == ApprovalAction.Approve ? WorkOrderStatus.Approved : WorkOrderStatus.Rejected;
            DecisionComments = (comments ?? string.Empty).Trim();
            DecidedBy = decidedBy;
            DecidedAtUtc = decidedAtUtc;
        }

        public override string ToString() => $"{Number} · {Status}";
    }
}
