using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WisejPerfLab.Models;

namespace WisejPerfLab.Services
{
    /// <summary>How far the export has got. Reported at most once per ten percent.</summary>
    public sealed class ExportProgress
    {
        public int Written { get; set; }

        public int Total { get; set; }

        public int Percent => Total == 0 ? 0 : Written * 100 / Total;
    }

    /// <summary>
    /// The CSV export.
    /// </summary>
    /// <remarks>
    /// Module 6 rewrote both halves of this. The rows now come from the single projection query
    /// (<see cref="TicketQueryService.SearchAsync"/>) instead of full entities with their long text
    /// columns, and the file is written through <b>one open <see cref="StreamWriter"/></b> instead of
    /// opening, appending and closing once per line. Progress is reported every ten percent rather than
    /// per row: each report is an update the browser has to apply, and five thousand of them would cost
    /// more than the export.
    /// </remarks>
    public sealed class ExportService
    {
        private readonly TicketQueryService _tickets;

        public ExportService(TicketQueryService tickets)
        {
            _tickets = tickets;
        }

        public static string ExportFolder => Path.Combine(AppContext.BaseDirectory, "exports");

        /// <summary>
        /// Builds the CSV and returns its path. Fully asynchronous: the caller runs it on a background
        /// task and awaits it, so no request thread is ever held.
        /// </summary>
        public async Task<string> BuildAsync(TicketFilter filter, Action<ExportProgress> progress, CancellationToken token)
        {
            var rows = await _tickets.SearchAsync(filter, token);

            Directory.CreateDirectory(ExportFolder);
            var path = Path.Combine(ExportFolder, $"tickets-{DateTime.Now:yyyyMMdd-HHmmss}.csv");

            var reportEvery = Math.Max(1, rows.Count / 10);

            // One open, one close, and a buffered write per line. The file is streamed, so the export of
            // a million rows costs the same memory as the export of a thousand.
            await using (var writer = new StreamWriter(path, append: false, Encoding.UTF8))
            {
                await writer.WriteLineAsync("Number,Customer,Status,Priority,Updated,Age");

                for (var i = 0; i < rows.Count; i++)
                {
                    token.ThrowIfCancellationRequested();

                    var row = rows[i];
                    await writer.WriteLineAsync(string.Join(",",
                        row.Number,
                        Quote(row.Customer),
                        row.Status,
                        row.Priority,
                        Quote(row.UpdatedText),
                        Quote(row.AgeText)));

                    // Throttled: ten updates for the whole export, not one per row.
                    if ((i + 1) % reportEvery == 0 || i == rows.Count - 1)
                        progress?.Invoke(new ExportProgress { Written = i + 1, Total = rows.Count });
                }
            }

            return path;
        }

        private static string Quote(string value)
            => value != null && value.IndexOf(',') >= 0
                ? "\"" + value.Replace("\"", "\"\"") + "\""
                : value ?? string.Empty;
    }
}
