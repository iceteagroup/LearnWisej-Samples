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

        /// <summary>The video's AlertBox text: calm, actionable, and it promises the edits are still on screen.</summary>
        public const string SaveFailed = "The work order could not be saved. Your changes are still here — try again or contact support.";
        public const string SaveFailedStatus = "Save failed — your changes are still here";
    }
}
