using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WisejPerfLab.Data;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>
    /// The CSV export. It is asynchronous, it writes a real file, and it does both in the way that shows
    /// up in a File I/O trace as thousands of tiny writes.
    /// </summary>
    /// <remarks>
    /// Module 6 profiles this method together with its caller: the handler blocks on
    /// <c>BuildAsync(...).Result</c> while this loop opens, writes and closes the file once per line.
    /// The fix is a background workflow that streams into one open writer and reports throttled progress.
    /// </remarks>
    public sealed class ExportService
    {
        private readonly IDbContextFactory<PerfLabContext> _dbFactory;

        public ExportService(IDbContextFactory<PerfLabContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public static string ExportFolder => Path.Combine(AppContext.BaseDirectory, "exports");

        /// <summary>Builds the CSV file and returns its path.</summary>
        public async Task<string> BuildAsync(TicketFilter filter, Action<int, int> progress = null)
        {
            List<Data.Entities.Ticket> tickets;
            await using (var db = await _dbFactory.CreateDbContextAsync())
            {
                tickets = await db.Tickets
                    .Where(t => t.Status == filter.Status)
                    .OrderByDescending(t => t.UpdatedAt)
                    .Take(filter.PageSize)
                    .ToListAsync();
            }

            Directory.CreateDirectory(ExportFolder);
            var path = Path.Combine(ExportFolder, $"tickets-{DateTime.Now:yyyyMMdd-HHmmss}.csv");

            // One open, one write and one close per line. Correct output, and a file handle churn that
            // the File I/O tool renders as a solid wall of writes.
            File.AppendAllText(path, "Number,Customer,Status,Priority,Updated,AgeHours" + Environment.NewLine);

            var now = DateTime.Now;
            for (var i = 0; i < tickets.Count; i++)
            {
                var ticket = tickets[i];
                var line = string.Join(",",
                    ticket.Number,
                    ticket.CustomerId.ToString(CultureInfo.InvariantCulture),
                    ticket.Status,
                    ticket.Priority,
                    ticket.UpdatedAt.ToString("s", CultureInfo.InvariantCulture),
                    (now - ticket.CreatedAt).TotalHours.ToString("F1", CultureInfo.InvariantCulture));

                File.AppendAllText(path, line + Environment.NewLine);

                // A progress report per row: every one of them is an update the browser has to apply.
                progress?.Invoke(i + 1, tickets.Count);
            }

            return path;
        }
    }
}
