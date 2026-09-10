using System.Collections.Generic;

namespace TicketOps.Domain
{
    public enum TransitionKind
    {
        /// <summary>Same status, or a move the lifecycle table allows.</summary>
        Legal,
        /// <summary>Not in the table but allowed for a privileged role (reopening a closed order).</summary>
        Privileged,
        /// <summary>No path in the lifecycle — a business error even if every field is valid.</summary>
        Illegal
    }

    /// <summary>
    /// The Work Order status machine as an explicit table (applied-concepts guide, section 3).
    /// Modelling the legal moves once makes the missing edges obvious: there is no path from New to
    /// Completed, and reopening a Closed order is not a transition — it is a privileged action gated by role.
    /// Plain C#: the validator, the service rules and the test cases all read this one table.
    /// </summary>
    public static class WorkOrderTransitions
    {
        private static readonly Dictionary<WorkOrderStatus, WorkOrderStatus[]> Allowed = new Dictionary<WorkOrderStatus, WorkOrderStatus[]>
        {
            [WorkOrderStatus.New] = new[] { WorkOrderStatus.Assigned, WorkOrderStatus.Cancelled },
            [WorkOrderStatus.Assigned] = new[] { WorkOrderStatus.InProgress, WorkOrderStatus.Cancelled },
            [WorkOrderStatus.InProgress] = new[] { WorkOrderStatus.OnHold, WorkOrderStatus.Completed },
            [WorkOrderStatus.OnHold] = new[] { WorkOrderStatus.InProgress, WorkOrderStatus.Cancelled },
            [WorkOrderStatus.Completed] = new[] { WorkOrderStatus.Closed },
            [WorkOrderStatus.Closed] = new WorkOrderStatus[0],      // reopen is Supervisor-only (Privileged)
            [WorkOrderStatus.Cancelled] = new WorkOrderStatus[0]
        };

        /// <summary>The one privileged move: a Supervisor may reopen a Closed order back to Assigned.</summary>
        public const WorkOrderStatus ReopenTarget = WorkOrderStatus.Assigned;

        public static TransitionKind Classify(WorkOrderStatus from, WorkOrderStatus to)
        {
            if (from == to)
                return TransitionKind.Legal;

            if (from == WorkOrderStatus.Closed && to == ReopenTarget)
                return TransitionKind.Privileged;

            return Allowed.TryGetValue(from, out var targets) && System.Array.IndexOf(targets, to) >= 0
                ? TransitionKind.Legal
                : TransitionKind.Illegal;
        }

        public static bool IsLegalTransition(WorkOrderStatus from, WorkOrderStatus to)
            => Classify(from, to) == TransitionKind.Legal;

        public static IReadOnlyList<WorkOrderStatus> AllowedFrom(WorkOrderStatus from)
            => Allowed.TryGetValue(from, out var targets) ? targets : new WorkOrderStatus[0];
    }
}
