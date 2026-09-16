using System;

namespace WisejPerfLab.Health
{
    /// <summary>
    /// The capacity model, in code: the measurements from Modules 3 to 6, the arithmetic they feed, and
    /// the thresholds that come out of it. Every number here has a source, and the source is a
    /// measurement taken in this application — not a guess, and not a default from a blog post.
    /// </summary>
    /// <remarks>
    /// The full derivation is <c>docs/Capacity.md</c>. This type exists so the screen can show the same
    /// numbers, and so that changing a measurement changes the thresholds rather than leaving the
    /// document and the configuration to drift apart.
    /// </remarks>
    public static class CapacityModel
    {
        // ---- measured inputs -------------------------------------------------------------------

        /// <summary>Module 4: managed heap retained by one idle session that has done a search.</summary>
        public const double RetainedMbPerIdleSession = 6.0;

        /// <summary>Module 4: the budget threshold, which leaves room for the screens a session may open.</summary>
        public const double BudgetedMbPerSession = 25.0;

        /// <summary>Module 3: the dashboard refresh, median of three, warm, Release.</summary>
        public const int DashboardRefreshMs = 113;

        /// <summary>Module 6: the ticket search click, median of three.</summary>
        public const int TicketSearchMs = 66;

        /// <summary>Module 6: the CSV export of 5,000 rows, on a background task.</summary>
        public const int ExportMs = 171;

        /// <summary>The instance this model is written for.</summary>
        public const double InstanceMemoryGb = 8.0;

        /// <summary>What is left for the process after the operating system and everything else.</summary>
        public const double UsableMemoryGb = 5.0;

        /// <summary>The headroom kept for spikes, garbage collection and the operating system.</summary>
        public const double HeadroomFraction = 0.30;

        // ---- derived ---------------------------------------------------------------------------

        /// <summary>Usable memory divided by the budgeted cost of a session, minus headroom.</summary>
        public static int MaxSessionsFromMemory
            => (int)(UsableMemoryGb * 1024 / BudgetedMbPerSession * (1 - HeadroomFraction));

        /// <summary>
        /// How many scenarios one core can serve per second at the measured cost, as a sanity check on
        /// the session number: 150 sessions each refreshing once a minute is 2.5 refreshes per second.
        /// </summary>
        public static double RefreshesPerSecondPerCore => 1000.0 / DashboardRefreshMs;

        public static string Describe()
            => $"retained {RetainedMbPerIdleSession:N1} MB/session measured, {BudgetedMbPerSession:N0} MB budgeted   " +
               $"usable {UsableMemoryGb:N0} GB   headroom {HeadroomFraction:P0}   " +
               $"→ maxSessions {MaxSessionsFromMemory}";
    }
}
