using System;
using System.Collections.Generic;
using System.Linq;
using OperationsConsole.Models;
using OperationsConsole.Orders;

namespace OperationsConsole.Services
{
    /// <summary>
    /// Thrown when the orders back end does not answer. The screen catches it, says something friendly and keeps the
    /// grid usable.
    /// </summary>
    public class OrderServiceException : Exception
    {
        public OrderServiceException(string message) : base(message) { }
    }

    /// <summary>
    /// The in-memory orders back end. It is the only place with business rules: filtering, paging, the due-date rules
    /// and the failure switch. No cell handler decides anything.
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
            // human-typed names really do contain & and <, which is why the status badge encodes its text
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

        /// <summary>When true every call fails, so the failure path can be shown.</summary>
        public bool SimulateFailure { get; set; }

        // ------------------------------------------------------------------------------------------------------------
        // Reads
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// The filtered working set for the bound path, capped at <paramref name="maxRows"/>.
        /// <see cref="OrderPage.TotalCount"/> is the real number of matches.
        /// </summary>
        public OrderPage GetOrders(OrderFilter filter, int maxRows = 250)
        {
            Fail();

            if (maxRows < 1)
                maxRows = 1;

            var filtered = Filtered(filter).ToList();
            var rows = filtered.Take(maxRows).ToList();

            return new OrderPage(0, rows, filtered.Count);
        }

        /// <summary>How many rows the filter matches — the grid's <c>RowCount</c> in virtual mode.</summary>
        public int Count(OrderFilter filter)
        {
            Fail();
            return Filtered(filter).Count();
        }

        /// <summary>One page of the filtered result — the only read the virtual path makes.</summary>
        public OrderPage GetPage(OrderFilter filter, int first, int count)
        {
            Fail();

            if (first < 0)
                first = 0;
            if (count < 1)
                count = 1;

            var filtered = Filtered(filter).ToList();
            var rows = filtered.Skip(first).Take(count).ToList();
            return new OrderPage(first, rows, filtered.Count);
        }

        /// <summary>One order by its stable ID — what the command column calls.</summary>
        public OrderRow GetOrder(string number)
        {
            Fail();
            return _orders.FirstOrDefault(o => o.Number == number);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Writes
        // ------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Reschedules an order. A due date in the past, a cancelled order, or a date more than a year out is rejected;
        /// a rejection is a normal answer and the caller puts the stored value back.
        /// </summary>
        public OrderUpdateResult UpdateDueDate(string number, DateTime dueDate)
        {
            Fail();

            var order = _orders.FirstOrDefault(o => o.Number == number);
            if (order == null)
                return OrderUpdateResult.Rejected(null, "That order is no longer available. Refresh the list and try again.");

            var date = dueDate.Date;

            if (order.Status == "Cancelled")
                return OrderUpdateResult.Rejected(order, "Order " + number + " is cancelled, so its due date cannot be changed.");

            if (date < DateTime.Today)
                return OrderUpdateResult.Rejected(order, "A due date in the past cannot be saved — pick " + DateTime.Today.ToString("d") + " or later.");

            if (date > DateTime.Today.AddYears(1))
                return OrderUpdateResult.Rejected(order, "A due date more than a year away is usually a typing slip — pick a date before " + DateTime.Today.AddYears(1).ToString("d") + ".");

            order.DueDate = date;
            return OrderUpdateResult.Ok(order, "Order " + number + " is now due " + date.ToString("d") + ".");
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

        private void Fail()
        {
            if (SimulateFailure)
                throw new OrderServiceException("The orders service did not answer.");
        }

        /// <summary>A few thousand deterministic orders (fixed seed).</summary>
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
