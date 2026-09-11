using System;
using System.Collections.Generic;
using System.Linq;

namespace AdaptiveOps.Models
{
    /// <summary>
    /// Thrown by <see cref="TicketRepository.Save"/> when a ticket does not pass server-side validation.
    /// Nothing is written.
    /// </summary>
    public sealed class TicketValidationException : Exception
    {
        public TicketValidationException(string message) : base(message) { }
    }

    /// <summary>
    /// In-memory ticket store, one instance per session (it is a field of MainPage).
    /// The server is the source of truth: the grid and the editor are always re-filled from here,
    /// and validation happens here, never in the browser.
    ///
    /// Module 3 adds the two data rules the visual layer follows: the SLA on-time share that drives
    /// the progress bar (the single CssStyle) and the "stale" threshold that drives the custom theme
    /// state on the Overdue card. The threshold lives here, in data; how stale *looks* lives in the theme.
    /// </summary>
    public sealed class TicketRepository
    {
        /// <summary>The signed-in operator; "Assigned to me" counts tickets owned by this name.</summary>
        public const string CurrentUser = "Dana";

        /// <summary>An open ticket this many days past its due date makes the Overdue metric stale.</summary>
        public const int StaleAfterDays = 2;

        private readonly List<Ticket> _tickets = new List<Ticket>();

        public TicketRepository()
        {
            _tickets.AddRange(Seed());
        }

        /// <summary>Copies of every ticket, ordered by id.</summary>
        public IReadOnlyList<Ticket> GetAll()
        {
            return _tickets.OrderBy(t => t.Id, StringComparer.Ordinal).Select(t => t.Clone()).ToList();
        }

        /// <summary>A copy of one ticket, or null.</summary>
        public Ticket Get(string id)
        {
            var t = _tickets.FirstOrDefault(x => x.Id == id);
            return t?.Clone();
        }

        /// <summary>
        /// Validates and stores a ticket. Throws <see cref="TicketValidationException"/> when the
        /// title is empty or too long, the owner is empty, or the id is unknown.
        /// </summary>
        public Ticket Save(Ticket ticket)
        {
            if (ticket == null)
                throw new ArgumentNullException(nameof(ticket));

            string title = (ticket.Title ?? string.Empty).Trim();
            if (title.Length == 0)
                throw new TicketValidationException("Title is required.");
            if (title.Length > 80)
                throw new TicketValidationException("Title must be 80 characters or fewer.");

            string owner = (ticket.Owner ?? string.Empty).Trim();
            if (owner.Length == 0)
                throw new TicketValidationException("Owner is required.");

            if (ticket.DueDate == default)
                throw new TicketValidationException("Due date is required.");

            var stored = _tickets.FirstOrDefault(x => x.Id == ticket.Id);
            if (stored == null)
                throw new TicketValidationException($"Unknown ticket '{ticket.Id}'.");

            stored.Title = title;
            stored.Priority = ticket.Priority;
            stored.Status = ticket.Status;
            stored.Owner = owner;
            stored.DueDate = ticket.DueDate.Date;
            stored.Notes = (ticket.Notes ?? string.Empty).Trim();
            return stored.Clone();
        }

        #region Metrics

        public int CountOpen() => _tickets.Count(t => t.IsOpen);

        public int CountOverdue(DateTime today) => _tickets.Count(t => t.IsOverdue(today));

        public int CountAssignedToMe() => _tickets.Count(t => t.IsOpen && t.Owner == CurrentUser);

        /// <summary>Closed or resolved tickets whose due date falls in the last seven days.</summary>
        public int CountClosedThisWeek(DateTime today)
        {
            var from = today.Date.AddDays(-7);
            return _tickets.Count(t => !t.IsOpen && t.DueDate.Date >= from && t.DueDate.Date <= today.Date);
        }

        /// <summary>
        /// Open tickets that are at least <see cref="StaleAfterDays"/> days past due. When this is
        /// greater than zero the Overdue metric card gets the "stale" theme state; when it drops to
        /// zero (move the due dates forward in the editor and save) the state is removed again.
        /// </summary>
        public int CountStaleOverdue(DateTime today)
        {
            var limit = today.Date.AddDays(-StaleAfterDays);
            return _tickets.Count(t => t.IsOpen && t.DueDate.Date <= limit);
        }

        /// <summary>
        /// Share of open tickets that are still within their due date, 0..100. This is the one value
        /// the UI renders through CssStyle: it is computed from live data and cannot be a CSS class.
        /// </summary>
        public int SlaOnTimePercent(DateTime today)
        {
            int open = CountOpen();
            if (open == 0)
                return 100;

            int onTime = open - CountOverdue(today);
            return (int)Math.Round(100.0 * onTime / open, MidpointRounding.AwayFromZero);
        }

        #endregion

        private static IEnumerable<Ticket> Seed()
        {
            // Due dates are relative to today so the Overdue / Closed-this-week / stale metrics stay meaningful.
            var today = DateTime.Today;
            return new[]
            {
                T("T-1042", "Printer offline, floor 3", TicketPriority.High, TicketStatus.Open, "Dana", today, "Kyocera on floor 3 shows 'network unreachable'. Switch port checked."),
                T("T-1041", "VPN drops every hour", TicketPriority.Medium, TicketStatus.Assigned, "Priya", today.AddDays(1), "Reproduced on two laptops; DHCP lease renewal suspected."),
                T("T-1040", "New laptop for finance", TicketPriority.Low, TicketStatus.Waiting, "Sam", today.AddDays(5), "Waiting for purchase order approval."),
                T("T-1039", "Badge reader rebooted", TicketPriority.Medium, TicketStatus.Resolved, "Lee", today.AddDays(-1), "Firmware updated, monitoring for a week."),
                T("T-1038", "Shared mailbox permissions", TicketPriority.Low, TicketStatus.Open, "Dana", today.AddDays(-2), "Two users cannot send as support@."),
                T("T-1037", "Warehouse Wi-Fi dead zone", TicketPriority.High, TicketStatus.Assigned, "Priya", today.AddDays(-3), "Aisle 12-14; survey scheduled."),
                T("T-1036", "Dashboard export truncates rows", TicketPriority.Critical, TicketStatus.Open, "Dana", today.AddDays(2), "CSV stops at 1000 rows. Paging bug in the report service."),
                T("T-1035", "Monitor flicker, room 2B", TicketPriority.Low, TicketStatus.Closed, "Lee", today.AddDays(-4), "Cable replaced."),
                T("T-1034", "Password reset for contractor", TicketPriority.Medium, TicketStatus.Closed, "Sam", today.AddDays(-6), "Done; contractor account expires next month."),
                T("T-1033", "Backup job skipped Sunday", TicketPriority.Critical, TicketStatus.Assigned, "Dana", today.AddDays(-1), "Job window overlapped with patching."),
                T("T-1032", "Conference phone echo", TicketPriority.Low, TicketStatus.Waiting, "Lee", today.AddDays(7), "Vendor ticket open."),
                T("T-1031", "Label printer driver", TicketPriority.Medium, TicketStatus.Closed, "Priya", today.AddDays(-9), "Driver package deployed through Intune."),
            };
        }

        private static Ticket T(string id, string title, TicketPriority priority, TicketStatus status, string owner, DateTime due, string notes)
        {
            return new Ticket { Id = id, Title = title, Priority = priority, Status = status, Owner = owner, DueDate = due.Date, Notes = notes };
        }
    }
}
