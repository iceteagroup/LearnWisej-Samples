using System;
using System.Collections.Generic;
using System.Linq;
using OperationsConsole.Models;
using OperationsConsole.Orders;
using OperationsConsole.Shell;

namespace OperationsConsole.Services
{
    /// <summary>
    /// Thrown when the orders back end does not answer. The screen catches it, says something friendly and keeps the
    /// grid usable; the type name and the message go to the Event log only.
    /// </summary>
    public class OrderServiceException : Exception
    {
        public OrderServiceException(string message) : base(message) { }
    }

    /// <summary>
    /// The in-memory orders back end for Module 5 (DataGridView Mastery).
    /// <para>
    /// It stands in for the query layer a real Operations Console would have, and it is deliberately the <b>only</b>
    /// place with business rules: filtering, paging, the due-date rules and the failure switch. No cell handler
    /// decides anything; they call these methods and report what comes back.
    /// </para>
    /// <para>
    /// Every call that would be a query in production is counted and written to the Event log
    /// (<see cref="FetchCount"/>). That counter is the point of the lab's last step: scroll the virtual grid and watch
    /// the log show <b>one fetch per page</b>, not one per cell and not one per row.
    /// </para>
    /// </summary>
    public class OrderService
    {
        /// <summary>The status values the grid and the filter strip know about.</summary>
        public static readonly string[] Statuses =
            { "Open", "Confirmed", "Packed", "Shipped", "On hold", "Cancelled" };

        private static readonly string[] Companies =
        {
            "Northwind Traders", "Contoso Manufacturing", "Fabrikam Logistics", "Adventure Works", "Tailspin Toys",
            "Wide World Importers", "Litware Systems", "Proseware Medical", "Woodgrove Supply", "Alpine Ski House",
            "Blue Yonder Airlines", "Trey Research", "Lucerne Publishing", "Fourth Coffee", "Graphic Design Institute",
            "Humongous Insurance", "Margie's Travel", "Nod Publishers", "Coho Vineyard", "Relecloud Hosting",
            // human-typed names really do contain & and <, which is exactly why the status badge encodes its text
            "Novak & Sons", "Bergström <Nordic> AB", "O'Rourke & Daughters"
        };

        private static readonly string[] Regions =
            { "Milan", "Turin", "Munich", "Lyon", "Rotterdam", "Dublin", "Porto", "Malmö", "Graz", "Bilbao" };

        private readonly List<OrderRow> _orders;

        public OrderService() : this(4000) { }

        public OrderService(int orderCount)
        {
            _orders = Generate(orderCount);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Diagnostics — the failure switch and the service log counter
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>When true the next call fails, so the failure path is reproducible from the command row.</summary>
        public bool SimulateFailure { get; set; }

        /// <summary>How many "queries" this service has answered since <see cref="ResetFetchCount"/>.</summary>
        public int FetchCount { get; private set; }

        /// <summary>How many orders exist in total, before any filter.</summary>
        public int TotalOrders => _orders.Count;

        /// <summary>Zeroes the counter so the next scroll can be read on its own.</summary>
        public void ResetFetchCount()
        {
            FetchCount = 0;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Reads
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The filtered working set for the <b>bound</b> path, capped at <paramref name="maxRows"/>.
        /// <para>
        /// The cap is the point: binding a list means one row object per row, so the bound path is for a set a person
        /// can actually work with. The returned <see cref="OrderPage.TotalCount"/> is the real number of matches, so
        /// the screen can say "the first 250 of 1 342" instead of quietly hiding rows — and the user can either narrow
        /// the filter or switch to the virtual path.
        /// </para>
        /// </summary>
        public OrderPage GetOrders(OrderFilter filter, int maxRows = 250)
        {
            Fail();
            FetchCount++;

            if (maxRows < 1)
                maxRows = 1;

            var filtered = Filtered(filter).ToList();
            var rows = filtered.Take(maxRows).ToList();

            ConsoleLog.Add("OrderService.GetOrders(" + Describe(filter) + ", max " + maxRows + ") → " + rows.Count +
                           " of " + filtered.Count + " matching rows  [fetch #" + FetchCount + "]");
            return new OrderPage(0, rows, filtered.Count);
        }

        /// <summary>
        /// How many rows the filter matches — the number the grid's <c>RowCount</c> is set from in virtual mode.
        /// Counting is cheap; materialising the rows is not.
        /// </summary>
        public int Count(OrderFilter filter)
        {
            Fail();
            FetchCount++;
            var count = Filtered(filter).Count();
            ConsoleLog.Add("OrderService.Count(" + Describe(filter) + ") → " + count + " rows  [fetch #" + FetchCount + "]");
            return count;
        }

        /// <summary>
        /// One page of the filtered result — the only read the virtual path makes. <paramref name="first"/> is an
        /// index into the filtered result, not into the whole table.
        /// </summary>
        public OrderPage GetPage(OrderFilter filter, int first, int count)
        {
            Fail();
            FetchCount++;

            if (first < 0)
                first = 0;
            if (count < 1)
                count = 1;

            var filtered = Filtered(filter).ToList();
            var rows = filtered.Skip(first).Take(count).ToList();
            var page = new OrderPage(first, rows, filtered.Count);

            ConsoleLog.Add("OrderService.GetPage(first=" + first + ", count=" + count + ") → " + rows.Count +
                           " rows of " + filtered.Count + "  [fetch #" + FetchCount + "]");
            return page;
        }

        /// <summary>One order by its stable ID — what the command column calls. Never "by whatever the cell shows".</summary>
        public OrderRow GetOrder(string number)
        {
            Fail();
            FetchCount++;
            var order = _orders.FirstOrDefault(o => o.Number == number);
            ConsoleLog.Add("OrderService.GetOrder(\"" + number + "\") → " + (order == null ? "not found" : order.Status + ", due " + order.DueDate.ToString("yyyy-MM-dd")) +
                           "  [fetch #" + FetchCount + "]");
            return order;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Writes — the business rules of this screen
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reschedules an order.
        /// <list type="bullet">
        /// <item>a due date in the past is rejected — you cannot promise a date that has already gone;</item>
        /// <item>a cancelled order cannot be rescheduled;</item>
        /// <item>more than a year out is rejected as a typo (2062 instead of 2026).</item>
        /// </list>
        /// A rejection is a normal answer: <see cref="OrderUpdateResult.Accepted"/> is false and the caller puts the
        /// stored value back into the cell.
        /// </summary>
        public OrderUpdateResult UpdateDueDate(string number, DateTime dueDate)
        {
            Fail();
            FetchCount++;

            var order = _orders.FirstOrDefault(o => o.Number == number);
            if (order == null)
            {
                ConsoleLog.Add("OrderService.UpdateDueDate(\"" + number + "\") → rejected: unknown order  [fetch #" + FetchCount + "]");
                return OrderUpdateResult.Rejected(null, "That order is no longer available. Refresh the list and try again.");
            }

            var date = dueDate.Date;

            if (order.Status == "Cancelled")
            {
                ConsoleLog.Add("OrderService.UpdateDueDate(\"" + number + "\", " + date.ToString("yyyy-MM-dd") + ") → rejected: order is cancelled  [fetch #" + FetchCount + "]");
                return OrderUpdateResult.Rejected(order, "Order " + number + " is cancelled, so its due date cannot be changed.");
            }

            if (date < DateTime.Today)
            {
                ConsoleLog.Add("OrderService.UpdateDueDate(\"" + number + "\", " + date.ToString("yyyy-MM-dd") + ") → rejected: due date is in the past  [fetch #" + FetchCount + "]");
                return OrderUpdateResult.Rejected(order, "A due date in the past cannot be saved — pick " + DateTime.Today.ToString("d") + " or later.");
            }

            if (date > DateTime.Today.AddYears(1))
            {
                ConsoleLog.Add("OrderService.UpdateDueDate(\"" + number + "\", " + date.ToString("yyyy-MM-dd") + ") → rejected: more than a year out  [fetch #" + FetchCount + "]");
                return OrderUpdateResult.Rejected(order, "A due date more than a year away is usually a typing slip — pick a date before " + DateTime.Today.AddYears(1).ToString("d") + ".");
            }

            order.DueDate = date;
            ConsoleLog.Add("OrderService.UpdateDueDate(\"" + number + "\", " + date.ToString("yyyy-MM-dd") + ") → accepted  [fetch #" + FetchCount + "]");
            return OrderUpdateResult.Ok(order, "Order " + number + " is now due " + date.ToString("d") + ".");
        }

        /// <summary>
        /// Stores a status exactly as it is given — used by the "Inject unsafe status" demo to prove that the badge in
        /// <c>CellFormatting</c> encodes whatever the model holds instead of executing it.
        /// </summary>
        public OrderRow SetStatus(string number, string status)
        {
            Fail();
            FetchCount++;

            var order = _orders.FirstOrDefault(o => o.Number == number);
            if (order != null)
                order.Status = status;

            ConsoleLog.Add("OrderService.SetStatus(\"" + number + "\", <raw text>) → stored unchanged  [fetch #" + FetchCount + "]");
            return order;
        }

        // ------------------------------------------------------------------------------------------------------------
        // Internals
        // ------------------------------------------------------------------------------------------------------------

        private IEnumerable<OrderRow> Filtered(OrderFilter filter)
        {
            IEnumerable<OrderRow> rows = _orders;

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status) && filter.Status != OrderFilter.AllStatuses)
                    rows = rows.Where(o => o.Status == filter.Status);

                var search = (filter.Search ?? "").Trim();
                if (search.Length > 0)
                    rows = rows.Where(o =>
                        o.Number.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        o.Customer.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            return rows;
        }

        private static string Describe(OrderFilter filter) => filter == null ? "no filter" : filter.Describe();

        private void Fail()
        {
            if (!SimulateFailure)
                return;

            throw new OrderServiceException("simulated back-end failure (the 'Simulate service failure' switch is on)");
        }

        /// <summary>
        /// A few thousand deterministic orders. Deterministic on purpose: the same seed gives the same list every run,
        /// so a log line the reviewer reads once still means the same thing on the next run.
        /// </summary>
        private static List<OrderRow> Generate(int count)
        {
            var random = new Random(20260910);
            var today = DateTime.Today;
            var list = new List<OrderRow>(count);

            for (var i = 0; i < count; i++)
            {
                var company = Companies[random.Next(Companies.Length)];
                var region = Regions[random.Next(Regions.Length)];
                var status = Statuses[random.Next(Statuses.Length)];

                list.Add(new OrderRow
                {
                    Number = "SO-" + (100001 + i).ToString("D6"),
                    Customer = company + " — " + region,
                    DueDate = today.AddDays(random.Next(-45, 120)),
                    Total = Math.Round((decimal)(random.NextDouble() * 47800 + 120), 2),
                    Status = status
                });
            }

            return list;
        }
    }
}
