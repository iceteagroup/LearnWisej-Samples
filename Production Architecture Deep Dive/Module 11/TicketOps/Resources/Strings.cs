namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, stack traces, table names,
    /// host names — or which half of a credential was wrong.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string AccessDenied = "You are not allowed to perform this action.";
        public const string SignInFailed = "The user name or password is incorrect.";
        public const string SignInUnavailable = "Sign-in is temporarily unavailable. Please try again in a moment.";
        public const string SignedOut = "You have been signed out.";
    }
}
