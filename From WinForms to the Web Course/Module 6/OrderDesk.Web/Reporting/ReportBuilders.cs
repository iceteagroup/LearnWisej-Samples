using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using OrderDesk.Domain;

namespace OrderDesk.Reporting
{
    /// <summary>
    /// The long-running reports the queue executes. Each builder returns a function the worker
    /// calls off-session: it reports progress through the callback, honours the cancellation token
    /// between units of work and returns the finished PDF bytes. The pauses stand in for a real
    /// rendering engine (a 1,204-order batch takes about 36 s, a monthly statement about 15 s) so the
    /// progress column has something to show.
    /// </summary>
    public static class ReportBuilders
    {
        /// <summary>Simulated cost of rendering one invoice page.</summary>
        public const int MillisecondsPerInvoice = 30;

        /// <summary>
        /// One invoice page per order: <paramref name="count"/> pages built from the given orders
        /// (repeated when there are fewer orders than pages), exactly what the desktop loop of
        /// PrintDocument calls did — but on the server, cancellable, with progress.
        /// </summary>
        public static Func<Action<int>, CancellationToken, byte[]> InvoiceBatch(IList<Order> orders, int count, OrderService service)
        {
            if (orders == null || orders.Count == 0) throw new ArgumentException("The batch needs at least one order.", nameof(orders));
            return (progress, token) =>
            {
                var pages = new List<IList<string>>(count);
                for (int i = 0; i < count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var order = orders[i % orders.Count];
                    var lines = InvoiceDocument.Build(order, service);           // the reused content rule, once per page
                    lines.Add("");
                    lines.Add($"Batch page {i + 1} of {count}");
                    pages.Add(lines);
                    Thread.Sleep(MillisecondsPerInvoice);
                    progress((i + 1) * 100 / count);
                }
                return InvoicePdfWriter.WritePages(pages, $"Invoice batch x {count}");
            };
        }

        /// <summary>A statement per customer (orders, subtotals, grand total); ~15 s of simulated work.</summary>
        public static Func<Action<int>, CancellationToken, byte[]> MonthlyStatement(IList<Order> orders, OrderService service)
        {
            return (progress, token) => Statement(orders, service, "MONTHLY STATEMENT", DateTime.Today.ToString("MMMM yyyy", CultureInfo.InvariantCulture), steps: 100, millisecondsPerStep: 150, progress, token);
        }

        /// <summary>A short summary by status; ~3 s — the job that is Done first so View / Download can be tried.</summary>
        public static Func<Action<int>, CancellationToken, byte[]> QuarterSummary(IList<Order> orders, OrderService service)
        {
            return (progress, token) => Statement(orders, service, "Q2 SUMMARY", "Quarter 2", steps: 30, millisecondsPerStep: 100, progress, token);
        }

        private static byte[] Statement(IList<Order> orders, OrderService service, string title, string period, int steps, int millisecondsPerStep, Action<int> progress, CancellationToken token)
        {
            var ci = CultureInfo.InvariantCulture;
            for (int step = 1; step <= steps; step++)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(millisecondsPerStep);                                  // the "reporting engine" at work
                progress(step * 100 / steps);
            }

            var lines = new List<string>
            {
                title,
                $"Period: {period}    Generated: {DateTime.Now.ToString("yyyy-MM-dd HH:mm", ci)}    Orders: {orders.Count}",
                "",
                $"{"Customer",-28}{"Orders",8}{"Open",12}{"Invoiced",12}{"Total",12}",
                new string('-', 72),
            };
            foreach (var group in orders.GroupBy(o => o.CustomerName).OrderBy(g => g.Key))
            {
                decimal total = group.Sum(o => service.CalculateOrderTotal(o));     // the business rule, not a stored column
                decimal open = group.Where(o => o.Status == OrderStatus.Open).Sum(o => o.Total);
                decimal invoiced = group.Where(o => o.Status == OrderStatus.Invoiced).Sum(o => o.Total);
                lines.Add($"{Trunc(group.Key, 27),-28}{group.Count(),8}{open.ToString("N2", ci),12}{invoiced.ToString("N2", ci),12}{total.ToString("N2", ci),12}");
            }
            lines.Add(new string('-', 72));
            lines.Add($"{"By status",-28}");
            foreach (var group in orders.GroupBy(o => o.Status).OrderBy(g => g.Key))
                lines.Add($"  {group.Key,-26}{group.Count(),8}{group.Sum(o => o.Total).ToString("N2", ci),36}");
            lines.Add("");
            lines.Add($"{"GRAND TOTAL",60}{orders.Sum(o => o.Total).ToString("N2", ci),12}");
            return InvoicePdfWriter.Write(lines, title);
        }

        private static string Trunc(string s, int max) => string.IsNullOrEmpty(s) || s.Length <= max ? s ?? "" : s.Substring(0, max - 1) + "…";
    }
}
