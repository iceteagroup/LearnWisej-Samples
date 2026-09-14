using System;
using System.Collections.Generic;
using System.Globalization;
namespace WisejPerfLab.Services
{
    /// <summary>
    /// The display strings the screens show: counts, ages, statuses, days and timestamps.
    /// </summary>
    /// <remarks>
    /// <c>FormatRow</c> — the function at the top of the Module 2 hot path, called 50,000 times per
    /// refresh — no longer exists. Module 3 took the dashboard off it and Module 5 took the grid off it,
    /// and the last caller went with them. Deleting a function you spent a module measuring is the
    /// normal ending: the fix is usually not making the hot function faster, it is not calling it.
    /// </remarks>
    public static class TicketFormatter
    {
        /// <summary>A timestamp as the grid shows it.</summary>
        public static string FormatMoment(DateTime moment)
            => moment.ToString("g", CultureInfo.CurrentCulture);

        /// <summary>
        /// The status and priority labels never change and are the same for every user, so they are
        /// built once for the process instead of being formatted per row. This is the only kind of thing
        /// that belongs in an application-scope cache: immutable, small, and shared by definition.
        /// Anything that differs per user belongs in the session, and anything that changes belongs
        /// nowhere but the database.
        /// </summary>
        private static readonly Dictionary<string, string> StatusLabels = new Dictionary<string, string>
        {
            ["Open"] = "Open",
            ["Waiting"] = "Waiting on customer",
            ["Escalated"] = "Escalated",
            ["Closed"] = "Closed"
        };

        /// <summary>The status as the operator reads it. A lookup, not a format call.</summary>
        public static string FormatStatus(string status)
            => status != null && StatusLabels.TryGetValue(status, out var label) ? label : status;

        public static string FormatPriority(string priority) => priority;

        /// <summary>A chart axis label: "14 Sep".</summary>
        public static string FormatDay(DateTime day)
            => day.ToString("dd MMM", CultureInfo.CurrentCulture);

        /// <summary>Culture-aware duration text: the "27.4 h" on the dashboard and the Age column in the grid.</summary>
        public static string FormatAge(TimeSpan age)
        {
            if (age.TotalDays >= 1)
                return string.Format(CultureInfo.CurrentCulture, "{0:N1} d", age.TotalDays);

            if (age.TotalHours >= 1)
                return string.Format(CultureInfo.CurrentCulture, "{0:N1} h", age.TotalHours);

            return string.Format(CultureInfo.CurrentCulture, "{0:N0} min", age.TotalMinutes);
        }

        public static string FormatCount(int value)
            => value.ToString("N0", CultureInfo.CurrentCulture);
    }
}
