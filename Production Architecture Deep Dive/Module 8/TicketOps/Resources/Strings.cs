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
        public const string SelectTicket = "Select a ticket first.";
        public const string ServiceUnavailable = "Exporting is not available in this profile. Ask an administrator to check the service registration.";
        public const string TestsPassed = "All presenter tests passed — no browser was needed for the decisions.";
        public const string TestsFailed = "Some presenter tests failed. Check the trace for details.";
    }
}
