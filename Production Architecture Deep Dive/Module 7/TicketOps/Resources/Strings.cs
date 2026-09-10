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

        // Module 7 · background import
        public const string ImportAlreadyRunning = "An import is already running. Wait for it to finish or cancel it.";
        public const string ImportCancelled = "Import cancelled. The rows already imported were kept; resume to continue.";
        public const string ImportInterrupted = "The import stopped because the data store is unavailable. Recover it, then resume.";
        public const string ImportFailed = "The import failed unexpectedly. Check the log for details.";
    }
}
