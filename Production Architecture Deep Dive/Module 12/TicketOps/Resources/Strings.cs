namespace TicketOps.Resources
{
    /// <summary>
    /// User-facing text lives here, never inline in handlers. The messages are safe to show:
    /// they explain what happened without leaking connection strings, stack traces, host or table names.
    /// </summary>
    public static class Strings
    {
        public const string AppTitle = "TicketOps Console";
        public const string ActionFailed = "The action could not be completed. Check the log for details.";
        public const string Saved = "Saved.";

        // Module 12 · deployment & diagnostics
        public const string DiagnosticsAccessDenied = "Access denied. The diagnostics page requires the Supervisor role.";
        public const string HealthHealthy = "All dependencies OK — this node is in the load-balancer rotation (HTTP 200).";
        public const string HealthDegraded = "A dependency is degraded. The node keeps serving (HTTP 200) — see the health checks.";
        public const string HealthUnhealthy = "A required dependency is unavailable. The node answers HTTP 503 and leaves the rotation until it recovers.";
    }
}
