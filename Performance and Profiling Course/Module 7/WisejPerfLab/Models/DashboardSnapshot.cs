using System;
using System.Collections.Generic;

namespace WisejPerfLab.Models
{
    /// <summary>One bar of the dashboard chart: a day and how many tickets were updated on it.</summary>
    public sealed class ChartRow
    {
        public string Label { get; set; }

        public int Count { get; set; }
    }

    /// <summary>
    /// Everything the dashboard shows, already formatted. The page assigns these values; it does not
    /// compute, format or count anything.
    /// </summary>
    /// <remarks>
    /// This is the Module 3 fix in one type. The old refresh loaded 50,000 entities so it could count
    /// three things in memory and build five strings per row; the new one asks the database for the
    /// three numbers, formats them once, and hands over this object. The rows never exist.
    /// <para>
    /// The trade is <b>staleness</b>: a snapshot is true at <see cref="GeneratedAt"/> and not after.
    /// That is stated on the screen and in <c>docs/BeforeAfter.md</c> — a fix that quietly changes when
    /// the data is true is not a free fix.
    /// </para>
    /// </remarks>
    public sealed class DashboardSnapshot
    {
        public string OpenTicketsText { get; set; }

        public string OverdueTicketsText { get; set; }

        public string AverageAgeText { get; set; }

        public IReadOnlyList<ChartRow> ChartRows { get; set; }

        public DateTime GeneratedAt { get; set; }

        /// <summary>How many SQL statements it took to build this snapshot. Four, and none of them per row.</summary>
        public int QueryCount { get; set; }
    }
}
