namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, host names, stack traces or table names.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string DirectoryUnavailable = "The user directory is not available right now. Try again in a moment.";
        public const string SelectTicketHint = "Select a ticket — the selection is stored in this session only.";
        public const string TicketSelected = "Ticket {0} selected for this session.";
    }
}
