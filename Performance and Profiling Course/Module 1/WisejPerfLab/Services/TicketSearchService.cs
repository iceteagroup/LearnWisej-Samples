using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Data.Entities;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>The result of one ticket search, with the statement count it took to produce.</summary>
    public sealed class TicketSearchResult
    {
        public IReadOnlyList<Ticket> Tickets { get; set; }

        /// <summary>Customer name per customer id, filled one query at a time.</summary>
        public IReadOnlyDictionary<int, string> CustomerNames { get; set; }

        /// <summary>How many SQL statements the search issued. The Database profiler tool counts the same ones.</summary>
        public int QueryCount { get; set; }
    }

    /// <summary>
    /// The ticket search as it is first written: the matching tickets as full tracked entities, then the
    /// customer name fetched per row because the screen needs it and the entity does not carry it.
    /// </summary>
    /// <remarks>
    /// Three separate costs live in this one method, and the course takes them apart one module at a time:
    /// the entities carry two long text columns no list shows (Module 5), the customer lookup is one
    /// statement per row (Module 6), and the rows are re-projected on every redraw by the page (Module 4).
    /// </remarks>
    public sealed class TicketSearchService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public TicketSearchService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public TicketSearchResult Search(TicketFilter filter)
        {
            using var db = _dbFactory.CreateDbContext();

            var tickets = db.Tickets
                .Where(t => t.Status == filter.Status)
                .OrderByDescending(t => t.UpdatedAt)
                .Take(filter.PageSize)
                .ToList();

            // One statement per displayed row. It is invisible in the code and obvious in the Database
            // trace: the screen needs a customer name, the entity carries a customer id, and the gap is
            // closed one row at a time.
            var names = new Dictionary<int, string>();
            var queries = 1;
            foreach (var ticket in tickets)
            {
                var customer = db.Customers.FirstOrDefault(c => c.Id == ticket.CustomerId);
                queries++;
                names[ticket.CustomerId] = customer?.Name ?? "(unknown)";
            }

            return new TicketSearchResult
            {
                Tickets = tickets,
                CustomerNames = names,
                QueryCount = queries
            };
        }

        /// <summary>The total number of tickets in the current status, for the status line and the budget.</summary>
        public int Count(TicketFilter filter)
        {
            using var db = _dbFactory.CreateDbContext();
            return db.Tickets.Count(t => t.Status == filter.Status);
        }
    }
}
