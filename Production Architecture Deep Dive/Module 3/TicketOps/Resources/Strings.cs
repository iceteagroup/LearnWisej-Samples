namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, stack traces or table names.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string WorkspaceTitle = "TicketOps — Ticket Workspace";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string Saved = "Saved.";

        /// <summary>{0} = the profile name the framework reported.</summary>
        public const string UnknownProfile = "Profile \"{0}\" is not defined in ClientProfiles.json — the desktop layout was applied.";

        /// <summary>{0} = previewed profile, {1} = the browser's real profile.</summary>
        public const string PreviewPinned = "Previewing the {0} layout — the browser is really on the {1} profile. Choose \"Live\" to follow the browser again.";

        public const string SearchPlaceholderTickets = "Search tickets…";
        public const string SearchPlaceholderActivity = "Filter activity…";
        public const string SelectTicketFirst = "Select a ticket first.";
    }
}
