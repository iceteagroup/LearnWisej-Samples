using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
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
    /// </summary>
    /// <remarks>
    /// Module 6 replaced the body of <see cref="GetPage"/>. It used to load full entities for the page and
    /// then ask for each row's customer name separately — 201 statements for 200 rows, with two long text
    /// columns nobody shows fetched on every one of them. It is now <b>one</b> statement: no tracking, the
    /// customer name joined, only the displayed columns selected, ordered and paged in the database.
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
        /// One page of rows, in one statement.
        /// </summary>
        /// <remarks>
        /// This method is synchronous on purpose. Its caller is <c>CellValueNeeded</c>, a synchronous
        /// event: making this <c>async</c> would mean blocking on it there, and blocking on an
        /// asynchronous call is the anti-pattern this module exists to remove. The async version is
        /// <see cref="SearchAsync"/>, used by the export, which runs on a background task and can await.
        /// </remarks>
        public TicketPage GetPage(TicketFilter filter, int firstIndex, int count)
        {
            using var db = _dbFactory.CreateDbContext();

            var page = Query(db, filter)
                .Skip(firstIndex)
                .Take(count)
                .ToList();

            return new TicketPage { FirstIndex = firstIndex, Rows = ToRows(page), QueryCount = 1 };
        }

        /// <summary>The same single statement, awaited — for callers that are allowed to await.</summary>
        public async Task<IReadOnlyList<TicketGridRow>> SearchAsync(TicketFilter filter, CancellationToken token)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(token);

            var page = await Query(db, filter)
                .Take(filter.PageSize)
                .ToListAsync(token);

            return ToRows(page);
        }

        /// <summary>
        /// The one query both callers share: read-only, joined, narrowed, ordered and paged by the
        /// database.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><c>AsNoTracking</c> — the grid never writes, so the change tracker would only cost memory
        /// and time.</item>
        /// <item><c>t.Customer.Name</c> — joined by the database inside the same statement, not fetched
        /// once per row.</item>
        /// <item>the <c>Select</c> lists the displayed columns only: <c>Description</c> and <c>Notes</c>
        /// are never read, never transferred and never materialised.</item>
        /// <item>ordering by <c>UpdatedAt</c> then <c>Id</c> keeps the paging stable, which is what stops
        /// a virtual grid from showing values from the wrong row.</item>
        /// <item>the filter and sort columns are indexed — see <c>PerfLabContext.OnModelCreating</c>.
        /// Without the index the same statement is a table scan, and the Database trace shows it.</item>
        /// </list>
        /// </remarks>
        private static IQueryable<TicketFields> Query(PerfLabContext db, TicketFilter filter)
            => db.Tickets
                .AsNoTracking()
                .Where(t => t.Status == filter.Status)
                .OrderByDescending(t => t.UpdatedAt)
                .ThenBy(t => t.Id)
                .Select(t => new TicketFields
                {
                    Id = t.Id,
                    Number = t.Number,
                    Customer = t.Customer.Name,
                    Status = t.Status,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                });

        /// <summary>
        /// Formats the page once, in memory, after the rows have arrived. Formatting is a .NET concern —
        /// pushing <c>ToString</c> into the SQL would either fail to translate or produce a string the
        /// database formatted in its own culture.
        /// </summary>
        private static List<TicketGridRow> ToRows(List<TicketFields> fields)
        {
            var now = DateTime.Now;
            var rows = new List<TicketGridRow>(fields.Count);

            foreach (var f in fields)
            {
                Diagnostics.CallCounter.Count("TicketQueryService.BuildRow");

                rows.Add(new TicketGridRow
                {
                    Id = f.Id,
                    Number = f.Number,
                    Customer = f.Customer,
                    Status = TicketFormatter.FormatStatus(f.Status),
                    Priority = f.Priority,
                    AgeText = TicketFormatter.FormatAge(now - f.CreatedAt),
                    UpdatedText = TicketFormatter.FormatMoment(f.UpdatedAt)
                });
            }

            return rows;
        }

        /// <summary>The columns the statement selects: the displayed ones, plus what the display strings need.</summary>
        private sealed class TicketFields
        {
            public int Id { get; set; }

            public string Number { get; set; }

            public string Customer { get; set; }

            public string Status { get; set; }

            public string Priority { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime UpdatedAt { get; set; }
        }
    }
}
