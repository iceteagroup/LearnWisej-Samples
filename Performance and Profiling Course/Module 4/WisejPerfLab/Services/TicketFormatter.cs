using System;
using System.Collections.Generic;
using System.Globalization;
using WisejPerfLab.Data.Entities;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// Turns one ticket entity into the strings a screen shows. Straightforward, correct, and called
    /// once per row on every redraw — which is how a function that costs 30 microseconds becomes the
    /// top of the hot path at 50,000 calls.
    /// </summary>
    /// <remarks>
    /// Nothing here is a mistake in itself. The mistake is where it is called from: Module 3 profiles
    /// <c>FormatRow</c>, finds the loop that drives the call count, and moves the work to a snapshot that
    /// is built once per refresh instead of once per row per redraw.
    /// </remarks>
    public static class TicketFormatter
    {
        public static TicketDisplayRow FormatRow(Ticket ticket, string customerName, DateTime now)
        {
            // Module 2: the call count an Instrumentation trace reports and a sampling trace cannot.
            Diagnostics.CallCounter.Count("TicketFormatter.FormatRow");

            return new TicketDisplayRow
            {
                Id = ticket.Id,
                Number = ticket.Number,
                Customer = customerName,
                Status = FormatStatus(ticket.Status),
                Priority = FormatPriority(ticket.Priority),
                AgeText = FormatAge(now - ticket.CreatedAt),
                UpdatedText = ticket.UpdatedAt.ToString("g", CultureInfo.CurrentCulture)
            };
        }

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
