using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationLab.Data
{
    /// <summary>
    /// Read-only, in-memory work order dataset (120 rows, deterministic).
    /// Stands in for the application service both endpoints call. Being a plain
    /// class it can be used from a postback handler (raw HTTP) and from a
    /// WebMethod (RPC) alike: the endpoint decides the transport, the service
    /// only knows about pages.
    /// </summary>
    public sealed class WorkOrderService
    {
        private const int RowCount = 120;
        private const int FirstNumber = 1042;     // the first row is WO-1042, as in the walkthrough
        private const int Seed = 8042;

        private static readonly string[] Assets =
        {
            "Boiler 3", "Chiller 1", "Pump P-12", "Conveyor C-4", "Compressor K-2",
            "Press 7", "HVAC Unit 5", "Turbine T-1", "Mixer M-9", "Furnace F-2",
            "Crane CR-3", "Packaging Line 2"
        };

        private static readonly string[] Priorities = { "Low", "Normal", "Normal", "High", "High", "Urgent" };

        private static readonly string[] Assignees =
        {
            "A. Ortega", "B. Lindqvist", "C. Nakamura", "D. Okafor", "E. Rossi",
            "F. Dubois", "G. Petrov", "H. Yamada"
        };

        private readonly List<WorkOrder> _orders;

        /// <summary>One shared instance per session is enough: the data never changes.</summary>
        public static WorkOrderService Instance { get; } = new WorkOrderService();

        public WorkOrderService()
        {
            var random = new Random(Seed);
            var baseDate = new DateTime(2026, 9, 9);
            _orders = new List<WorkOrder>(RowCount);

            for (int i = 0; i < RowCount; i++)
            {
                var status = (WorkOrderStatus)random.Next(0, 3);
                _orders.Add(new WorkOrder(
                    number: FirstNumber + i,
                    asset: Assets[random.Next(Assets.Length)],
                    status: status,
                    priority: Priorities[random.Next(Priorities.Length)],
                    assignee: Assignees[random.Next(Assignees.Length)],
                    dueDate: baseDate.AddDays(random.Next(-20, 45)),
                    hours: Math.Round(0.5 + random.Next(0, 80) * 0.5, 1)));
            }
        }

        public int Count => _orders.Count;

        /// <summary>
        /// Returns one page of rows. Arguments are trusted here: they have already
        /// been bounded and whitelisted by <see cref="PageRequest"/>.
        /// </summary>
        public PageResult LoadPage(PageRequest request)
            => LoadPage(request.Page, request.Size, request.Sort, request.Desc);

        public PageResult LoadPage(int page, int size, string sortField, bool desc)
        {
            IEnumerable<WorkOrder> ordered = Sort(sortField, desc);

            return new PageResult
            {
                Rows = ordered.Skip((page - 1) * size).Take(size).Select(o => o.ToRow()).ToList(),
                Total = _orders.Count,
                Page = page,
                Size = size,
                Sort = sortField,
                Desc = desc
            };
        }

        private IEnumerable<WorkOrder> Sort(string field, bool desc)
        {
            // The switch IS the whitelist on the data side: every case is a known column,
            // nothing here is built from the string the browser sent.
            IOrderedEnumerable<WorkOrder> ordered = field switch
            {
                "asset" => _orders.OrderBy(o => o.Asset),
                "status" => _orders.OrderBy(o => o.Status),
                "priority" => _orders.OrderBy(o => PriorityRank(o.Priority)),
                "assignee" => _orders.OrderBy(o => o.Assignee),
                "dueDate" => _orders.OrderBy(o => o.DueDate),
                "hours" => _orders.OrderBy(o => o.Hours),
                _ => _orders.OrderBy(o => o.Number),
            };

            ordered = ordered.ThenBy(o => o.Number);
            return desc ? ordered.Reverse() : ordered;
        }

        private static int PriorityRank(string priority) => priority switch
        {
            "Low" => 0,
            "Normal" => 1,
            "High" => 2,
            "Urgent" => 3,
            _ => 9,
        };
    }
}
