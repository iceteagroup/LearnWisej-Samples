using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// Raised by the data layer when the store is unreachable. Its message is deliberately internal
    /// (host names, table names): it belongs in the log, and the screen must not show it.
    /// </summary>
    public sealed class DataOutageException : Exception
    {
        public DataOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// In-memory repository seeded with the work orders the walkthrough video shows. One instance per session
    /// (created in AppComposition), so two browser tabs never share a list. <see cref="SimulateOutage"/> lets
    /// the lab show the error path without a real database. Dates and amounts are stored raw: the culture
    /// decides how they look, and that decision is made in the screen, never here.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();

        public bool SimulateOutage { get; set; }

        public InMemoryWorkOrderRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var o in SeedData.WorkOrders())
                _orders[o.Id] = o;
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository", $"seeded {_orders.Count} work orders (in-memory, per session)");
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM WorkOrders");
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.GetAllAsync", $"{_orders.Count} rows");
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(o => o.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM WorkOrders WHERE Id={id}");
            _orders.TryGetValue(id, out var order);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.FindAsync", order == null ? $"#{id} not found" : $"#{id} found");
            return Task.FromResult(order == null ? null : Clone(order));
        }

        public Task<WorkOrder> UpsertAsync(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            EnsureAvailable($"UPDATE WorkOrders WHERE Id={order.Id}");

            _orders[order.Id] = Clone(order);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.UpsertAsync", $"#{order.Id} written · status {order.Status}");
            return Task.FromResult(Clone(order));
        }

        private void EnsureAvailable(string statement)
        {
            if (!SimulateOutage)
                return;

            // What a real driver would say — and exactly what must not reach the user (in any language).
            _log.Error(LogLayer.Data, "InMemoryWorkOrderRepository", null,
                $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders)");
            throw new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
        }

        private static WorkOrder Clone(WorkOrder o) => new WorkOrder
        {
            Id = o.Id,
            Title = o.Title,
            Status = o.Status,
            CreatedOn = o.CreatedOn,
            DueOn = o.DueOn,
            LaborCost = o.LaborCost,
            Hours = o.Hours
        };
    }

    /// <summary>
    /// The work orders of the walkthrough video (2002 "Repair loading dock pump", due 6/14/2026, $1,850.00 …).
    /// Fixed dates on purpose: the point of the module is that 6/14/2026 and 14.06.2026 are the SAME value.
    /// Titles are operator-entered data, not UI text, so they are not translated (see docs/LocalizationNotes.md).
    /// </summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            yield return new WorkOrder { Id = 2001, Title = "Replace conveyor belt motor", Status = WorkOrderStatus.InProgress, CreatedOn = new DateTime(2026, 5, 28, 8, 15, 0), DueOn = new DateTime(2026, 6, 10), LaborCost = 2400.00m, Hours = 6.5 };
            yield return new WorkOrder { Id = 2002, Title = "Repair loading dock pump", Status = WorkOrderStatus.Open, CreatedOn = new DateTime(2026, 6, 1, 9, 30, 0), DueOn = new DateTime(2026, 6, 14), LaborCost = 1850.00m, Hours = 0 };
            yield return new WorkOrder { Id = 2003, Title = "Quarterly elevator inspection", Status = WorkOrderStatus.Open, CreatedOn = new DateTime(2026, 6, 2, 14, 5, 0), DueOn = new DateTime(2026, 6, 22), LaborCost = 600.00m, Hours = 0 };
            yield return new WorkOrder { Id = 2004, Title = "Calibrate cold-room sensors", Status = WorkOrderStatus.Blocked, CreatedOn = new DateTime(2026, 6, 3, 10, 0, 0), DueOn = new DateTime(2026, 6, 12), LaborCost = 320.50m, Hours = 1.5 };
            yield return new WorkOrder { Id = 2005, Title = "Rewire warehouse lighting", Status = WorkOrderStatus.InProgress, CreatedOn = new DateTime(2026, 6, 4, 7, 45, 0), DueOn = new DateTime(2026, 6, 30), LaborCost = 5120.00m, Hours = 12.25 };
            yield return new WorkOrder { Id = 2006, Title = "Service forklift #3", Status = WorkOrderStatus.Done, CreatedOn = new DateTime(2026, 5, 20, 11, 20, 0), DueOn = new DateTime(2026, 6, 5), LaborCost = 780.00m, Hours = 4 };
            yield return new WorkOrder { Id = 2007, Title = "Patch roof leak, bay 4", Status = WorkOrderStatus.Done, CreatedOn = new DateTime(2026, 5, 22, 16, 0, 0), DueOn = new DateTime(2026, 6, 1), LaborCost = 1275.00m, Hours = 9 };
            yield return new WorkOrder { Id = 2008, Title = "Annual sprinkler test", Status = WorkOrderStatus.Open, CreatedOn = new DateTime(2026, 6, 5, 8, 0, 0), DueOn = new DateTime(2026, 7, 3), LaborCost = 450.00m, Hours = 0 };
            yield return new WorkOrder { Id = 2009, Title = "Replace HVAC filters", Status = WorkOrderStatus.InProgress, CreatedOn = new DateTime(2026, 6, 6, 13, 10, 0), DueOn = new DateTime(2026, 6, 18), LaborCost = 210.00m, Hours = 2 };
            yield return new WorkOrder { Id = 2010, Title = "Paint safety lines", Status = WorkOrderStatus.Open, CreatedOn = new DateTime(2026, 6, 8, 9, 0, 0), DueOn = new DateTime(2026, 7, 15), LaborCost = 990.00m, Hours = 0 };
        }
    }
}
