namespace EnterpriseOps.Resources
{
    /// <summary>
    /// User-facing strings in one place (a .resx with cultures replaces this in a later module). The rule they
    /// enforce: what the user reads never contains an exception type, a host name or a stack trace.
    /// </summary>
    public static class UiText
    {
        public const string ScreenTitle = "EnterpriseOps — Command Center";
        public const string Ready = "ready";
        public const string Refreshing = "Refreshing…";
        public const string Reviewing = "Running the review gate…";

        /// <summary>The lab placeholder's message — generic on purpose; details are in the log with the correlation id.</summary>
        public const string ActionFailed = "The action could not be completed. Check the log for details.";

        public const string TraceFooter = "UI → · Security: · Integration: · Data: · Service: · Diagnostics: · Architecture: · UI ←";
    }
}
