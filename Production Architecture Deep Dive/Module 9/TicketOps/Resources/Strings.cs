namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, stack traces, table names —
    /// or, in this module, the browser's exception text.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string Saved = "Saved.";

        // Module 9 · interop outcomes
        public const string LinkCopied = "Link copied to clipboard.";
        public const string CopyFailed = "Copy failed — select the link text below and copy it manually (Ctrl+C).";
        public const string SelectWorkOrder = "Select a work order to copy its link.";
    }
}
