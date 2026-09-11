namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, stack traces or table names.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string Saved = "Saved.";

        // Module 6 · approval workflow
        public const string SelectWorkOrder = "Select a work order first.";
        public const string CommentsRequiredToReject = "Comments are required when rejecting.";
        public const string DecisionNotConfirmed = "The decision was not confirmed — nothing was applied.";
        public const string WorkOrderMissing = "The work order no longer exists. Refresh the list.";
        public const string DialogOpen = "Approval dialog open — work order {0} untouched.";
        public const string DialogCancelled = "Approval canceled — work order {0} unchanged.";
        public const string DecisionApplied = "Work order {0} {1}.";
    }
}
