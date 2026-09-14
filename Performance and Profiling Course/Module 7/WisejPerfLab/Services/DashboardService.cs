using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Data.Entities;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// The dashboard data. Since Module 3 this service owns the counting <b>and</b> the formatting, and
    /// returns a <see cref="DashboardSnapshot"/> of display-ready values.
    /// </summary>
    public sealed class DashboardService
    {
        /// <summary>A ticket is overdue when it is still open after this many hours.</summary>
        private const int OverdueAfterHours = 48;

        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public DashboardService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        /// <summary>
        /// The three KPI values and the chart series, counted by the database and formatted once.
        /// Four statements, none of them per row, and no entity leaves this method.
        /// </summary>
        public DashboardSnapshot GetSnapshot()
        {
            using var db = _dbFactory.CreateDbContext();

            var now = DateTime.Now;
            var overdueBefore = now.AddHours(-OverdueAfterHours);

            // 1 + 2. Counting is what a database is for. These materialise nothing.
            var open = db.Tickets.Count(t => t.Status == "Open");
            var overdue = db.Tickets.Count(t => t.Status != "Closed" && t.CreatedAt < overdueBefore);

            // 3. The average ticket age in hours — over every ticket, exactly as the old in-memory loop
            //    computed it, so the number on screen does not change when the implementation does.
            //    EF Core cannot translate an average over a date difference, so this is the one
            //    deliberately provider-specific statement in the sample: julianday() is SQLite, and on
            //    SQL Server it would be AVG(DATEDIFF(...)). It is still an aggregate — either way the
            //    rows stay in the database.
            var averageAgeHours = db.Database
                .SqlQueryRaw<double?>(
                    "SELECT AVG((julianday('now') - julianday(CreatedAt)) * 24.0) AS Value FROM Tickets")
                .AsEnumerable()
                .FirstOrDefault() ?? 0d;

            // 4. The chart: one grouped query for fourteen numbers, instead of 50,000 rows and a loop.
            var since = DateTime.Today.AddDays(-13);
            var perDay = db.Tickets
                .Where(t => t.UpdatedAt >= since)
                .GroupBy(t => t.UpdatedAt.Date)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToList()
                .ToDictionary(x => x.Day, x => x.Count);

            var chartRows = new List<ChartRow>(14);
            for (var offset = 13; offset >= 0; offset--)
            {
                var day = DateTime.Today.AddDays(-offset);
                chartRows.Add(new ChartRow
                {
                    Label = TicketFormatter.FormatDay(day),
                    Count = perDay.TryGetValue(day, out var count) ? count : 0
                });
            }

            // The formatting happens here, once, in the culture the session runs in — not in the page,
            // and not per row.
            return new DashboardSnapshot
            {
                OpenTicketsText = TicketFormatter.FormatCount(open),
                OverdueTicketsText = TicketFormatter.FormatCount(overdue),
                AverageAgeText = TicketFormatter.FormatAge(TimeSpan.FromHours(averageAgeHours)),
                ChartRows = chartRows,
                GeneratedAt = now,
                QueryCount = 4
            };
        }

        /// <summary>
        /// Every ticket, as tracked entities with every column — what the refresh used to do.
        /// </summary>
        /// <remarks>
        /// Kept and unused, so the shape of the old scenario stays readable next to the new one. Nothing
        /// in the UI calls it any more; to measure the old refresh again, run the Module 2 folder.
        /// </remarks>
        public IReadOnlyList<Ticket> LoadAllTickets()
        {
            using var db = _dbFactory.CreateDbContext();

            return db.Tickets
                .OrderByDescending(t => t.UpdatedAt)
                .ToList();
        }
    }
}
