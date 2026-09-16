using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Data.Entities;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>One page of grid rows, with the statement count it took to fetch.</summary>
    public sealed class TicketPage
    {
        public int FirstIndex { get; set; }

        public IReadOnlyList<TicketGridRow> Rows { get; set; }

        public int QueryCount { get; set; }
    }

    /// <summary>
    /// The ticket queries behind the grid: how many rows match, and one page of rows at a time.
    /// Since Module 5 nothing but <see cref="TicketGridRow"/> leaves this service.
    /// </summary>
    /// <remarks>
    /// The grid is in virtual mode, so the screen asks for a count and then for the rows it is actually
    /// showing. What is still wrong here is inside <see cref="GetPage"/>: it loads full entities for the
    /// page and then asks for each row's customer name separately. That is 201 statements per page
    /// instead of 5,001 for the whole result — an improvement produced by fetching less, not by fixing
    /// the query. Module 6 replaces the body of this method with one projection query.
    /// </remarks>
    public sealed class TicketQueryService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public TicketQueryService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>How many rows match the filter. In virtual mode this is what <c>RowCount</c> is set from.</summary>
        public int Count(TicketFilter filter)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Tickets.Count(t => t.Status == filter.Status);
        }

        /// <summary>
        /// One page of rows, ordered exactly as the grid orders them, so row index N is always the same
        /// ticket. A virtual grid that returns rows in a different order on the next fetch shows the user
        /// values from the wrong row.
        /// </summary>
        public TicketPage GetPage(TicketFilter filter, int firstIndex, int count)
        {
            using var db = _dbFactory.CreateDbContext();

            var tickets = db.Tickets
                .Where(t => t.Status == filter.Status)
                .OrderByDescending(t => t.UpdatedAt)
                .ThenBy(t => t.Id)
                .Skip(firstIndex)
                .Take(count)
                .ToList();

            var queries = 1;
            var now = DateTime.Now;
            var rows = new List<TicketGridRow>(tickets.Count);

            foreach (var ticket in tickets)
            {
                // Still one statement per row. Fewer rows, so fewer statements — and the same mistake.
                var customer = db.Customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
                queries++;

                rows.Add(BuildRow(ticket, customer?.Name ?? "(unknown)", now));
            }

            return new TicketPage { FirstIndex = firstIndex, Rows = rows, QueryCount = queries };
        }

        /// <summary>Builds one display row. The only place ticket text is formatted for the grid.</summary>
        private static TicketGridRow BuildRow(Ticket ticket, string customerName, DateTime now)
        {
            Diagnostics.CallCounter.Count("TicketQueryService.BuildRow");

            return new TicketGridRow
            {
                Id = ticket.Id,
                Number = ticket.Number,
                Customer = customerName,
                Status = TicketFormatter.FormatStatus(ticket.Status),
                Priority = ticket.Priority,
                AgeText = TicketFormatter.FormatAge(now - ticket.CreatedAt),
                UpdatedText = TicketFormatter.FormatMoment(ticket.UpdatedAt)
            };
        }
    }
}
