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
        public const string SaveFailedEditsKept = "Your changes could not be saved and are still on screen. Check the log for details.";
        public const string Saved = "Saved.";
        public const string UnsavedChanges = "● Unsaved changes";
        public const string NoSelection = "Select a work order to edit it here.";
        public const string NoMatches = "No work orders match the search. Clear the search or pick another status.";
        public const string ReloadBlockedByUnsaved = "Save or discard the unsaved changes before reloading.";
    }
}
