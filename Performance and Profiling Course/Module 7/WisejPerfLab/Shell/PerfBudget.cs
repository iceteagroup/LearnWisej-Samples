using System.Collections.Generic;

namespace WisejPerfLab.Shell
{
    /// <summary>
    /// The acceptance thresholds from <c>docs/Budget.md</c>, in code so the status line can say whether a
    /// run passed instead of only how long it took. A budget that lives in a document nobody opens is a
    /// number nobody checks.
    /// </summary>
    /// <remarks>
    /// These are this sample's numbers, not universal ones. The course lesson quotes a budget of 800 ms
    /// for the dashboard refresh and 900 ms for the ticket search, measured against SQL Server on the
    /// author's machine, where every statement costs a network hop. This sample runs an in-process SQLite
    /// database on a developer laptop, where the same work is roughly twice as fast, so the thresholds are
    /// tightened to keep the same relationship between the budget and what the naive code costs.
    /// <b>Writing your own from your own baseline is the Module 1 deliverable</b> — see
    /// <c>docs/Budget.md</c>, which shows the arithmetic for both.
    /// </remarks>
    public static class PerfBudget
    {
        /// <summary>Scenario/UserAction, the threshold in milliseconds, and the tool that proves or disproves it.</summary>
        private static readonly Dictionary<string, (int Ms, string Tool)> Thresholds =
            new Dictionary<string, (int, string)>
            {
                ["Dashboard/Refresh"] = (250, "CPU Usage"),
                ["Tickets/Search"] = (300, "Database + .NET Object Allocation"),
                ["Tickets/Redraw"] = (150, ".NET Object Allocation"),
                ["Tickets/Export"] = (400, "File I/O + .NET Async"),
                ["Customers/LoadTree"] = (150, ".NET Object Allocation + browser network panel"),
                ["Customers/ExpandNode"] = (100, ".NET Object Allocation + browser network panel"),
                // A load balancer asks this every few seconds, from every instance. It has to be free.
                ["Capacity/HealthCheck"] = (20, "the health endpoint itself")
            };

        /// <summary>The memory half of the budget: what one idle session may retain.</summary>
        public const int RetainedMbPerIdleSession = 25;

        public static int? ThresholdMs(string scenario, string userAction)
            => Thresholds.TryGetValue(scenario + "/" + userAction, out var entry) ? entry.Ms : (int?)null;

        public static string Tool(string scenario, string userAction)
            => Thresholds.TryGetValue(scenario + "/" + userAction, out var entry) ? entry.Tool : "—";

        /// <summary>"1,840 ms — over the 800 ms budget" / "612 ms — within the 800 ms budget".</summary>
        public static string Describe(string scenario, string userAction, long elapsedMs)
        {
            var threshold = ThresholdMs(scenario, userAction);
            if (threshold == null)
                return $"{elapsedMs:N0} ms";

            return elapsedMs > threshold.Value
                ? $"{elapsedMs:N0} ms — over the {threshold.Value:N0} ms budget"
                : $"{elapsedMs:N0} ms — within the {threshold.Value:N0} ms budget";
        }

        public static ShellState StateFor(string scenario, string userAction, long elapsedMs)
        {
            var threshold = ThresholdMs(scenario, userAction);
            if (threshold == null)
                return ShellState.Ok;

            return elapsedMs > threshold.Value ? ShellState.Warn : ShellState.Ok;
        }
    }
}
