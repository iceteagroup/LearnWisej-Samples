using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OrderDesk.Services
{
    /// <summary>One measured grid load: what was fetched, what was bound, how long the server took.</summary>
    public sealed class LoadMetrics
    {
        /// <summary>"naive 20k", "naive 200k" or "optimized".</summary>
        public string Mode { get; set; }
        /// <summary>The OrderQuery (optimized) or "GetAll()" (naive).</summary>
        public string Query { get; set; }
        /// <summary>Rows the store handed back to the page (GetAll = the whole table; Search = one block).</summary>
        public int RowsFetched { get; set; }
        /// <summary>Rows the grid now holds on the server (bound rows, or RowCount in VirtualMode).</summary>
        public int RowsInGrid { get; set; }
        /// <summary>Rows that would cross the wire for the first paint (the payload basis).</summary>
        public int RowsShipped { get; set; }
        public double ServerMs { get; set; }
        public long MemoryDeltaBytes { get; set; }

        public long EstimatedPayloadBytes => PayloadEstimate.For(RowsShipped);

        public override string ToString()
            => Mode + ": " + ServerMs.ToString("0", CultureInfo.InvariantCulture) + " ms · " + RowsFetched.ToString("N0") + " fetched · "
             + RowsInGrid.ToString("N0") + " in grid · ~" + PayloadEstimate.Human(EstimatedPayloadBytes);
    }

    /// <summary>
    /// A payload ESTIMATE, calibrated to the numbers the walkthrough video shows (200,000 rows ≈ 96 MB,
    /// 50 virtual rows ≈ 38 KB): rows × 5 columns × ~96 bytes of JSON per cell + ~14 KB of grid definition.
    /// It is the cost of shipping every row the way the desktop grid painted every row. Wisej.NET already
    /// streams row blocks on scroll, so the real per-request payload is smaller — measure it in DevTools →
    /// Network. The server-side numbers next to it (ms, rows, memory) are measured, not estimated.
    /// </summary>
    public static class PayloadEstimate
    {
        public const int Columns = 5;
        public const int BytesPerCell = 96;
        public const int GridDefinitionBytes = 14 * 1024;

        public static string Formula => "rows × 5 cols × 96 B + 14 KB grid definition";

        public static long For(int rows) => GridDefinitionBytes + (long)rows * Columns * BytesPerCell;

        public static string Human(long bytes)
        {
            double b = Math.Abs(bytes);
            string sign = bytes < 0 ? "-" : "";
            // decimal units, like the browser's Network panel (and the video: 200,000 rows ≈ 96 MB, 50 rows ≈ 38 KB)
            if (b >= 1_000_000) return sign + (b / 1_000_000).ToString("0.0", CultureInfo.InvariantCulture) + " MB";
            if (b >= 1000) return sign + (b / 1000).ToString("0", CultureInfo.InvariantCulture) + " KB";
            return sign + b.ToString("0", CultureInfo.InvariantCulture) + " B";
        }

        public static string Signed(long bytes) => (bytes >= 0 ? "+" : "") + Human(bytes);
    }

    /// <summary>The runs collected by the Measure button; renders the performance-notes table.</summary>
    public sealed class PerformanceLog
    {
        private readonly List<LoadMetrics> _runs = new List<LoadMetrics>();

        public IReadOnlyList<LoadMetrics> Runs => _runs;
        public void Add(LoadMetrics m) => _runs.Add(m);
        public void Clear() => _runs.Clear();
        public IEnumerable<string> Modes => _runs.Select(r => r.Mode).Distinct();

        public string Line(string mode)
        {
            var rows = _runs.Where(r => r.Mode == mode).ToList();
            if (rows.Count == 0) return null;
            var last = rows[rows.Count - 1];
            return mode.PadRight(10)
                 + " min " + rows.Min(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture)
                 + " · avg " + rows.Average(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture)
                 + " · max " + rows.Max(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture) + " ms"
                 + " (" + rows.Count + " runs) · " + last.RowsFetched.ToString("N0") + " fetched · "
                 + last.RowsInGrid.ToString("N0") + " in grid · ~" + PayloadEstimate.Human(last.EstimatedPayloadBytes);
        }

        public IEnumerable<string> Lines() => Modes.Select(Line).Where(l => l != null);

        /// <summary>The same table as Markdown, for App_Data/performance-notes.md.</summary>
        public string Markdown(string title)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# " + title);
            sb.AppendLine();
            sb.AppendLine("Written by the Measure button of Module 5 on " + DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) + ".");
            sb.AppendLine("Server-side milliseconds are measured with Stopwatch; the payload column is an estimate (" + PayloadEstimate.Formula + ").");
            sb.AppendLine();
            sb.AppendLine("| Mode | Runs | Min ms | Avg ms | Max ms | Rows fetched from store | Rows in grid | Est. payload |");
            sb.AppendLine("|---|---|---|---|---|---|---|---|");
            foreach (var mode in Modes)
            {
                var rows = _runs.Where(r => r.Mode == mode).ToList();
                var last = rows[rows.Count - 1];
                sb.AppendLine("| " + mode + " | " + rows.Count
                    + " | " + rows.Min(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture)
                    + " | " + rows.Average(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture)
                    + " | " + rows.Max(r => r.ServerMs).ToString("0", CultureInfo.InvariantCulture)
                    + " | " + last.RowsFetched.ToString("N0") + " | " + last.RowsInGrid.ToString("N0")
                    + " | ~" + PayloadEstimate.Human(last.EstimatedPayloadBytes) + " |");
            }
            sb.AppendLine();
            sb.AppendLine("Every run:");
            sb.AppendLine();
            foreach (var r in _runs) sb.AppendLine("- " + r);
            return sb.ToString();
        }
    }
}
