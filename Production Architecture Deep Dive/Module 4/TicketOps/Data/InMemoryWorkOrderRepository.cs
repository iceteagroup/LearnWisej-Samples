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
    /// In-memory repository seeded with the six work orders the walkthrough video shows. One instance
    /// per session (created in AppComposition), so two browser tabs never share a list — the same reason
    /// nothing here is static. It stores and returns <b>copies</b>: the bound objects on the screen are
    /// the service's, and a save is an explicit hand-over, not a shared reference.
    /// <see cref="SimulateOutage"/> lets the lab show the error path without a real database.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();
        private int _nextId;

        public bool SimulateOutage { get; set; }

        public InMemoryWorkOrderRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var o in SeedData.WorkOrders())
                _orders[o.Id] = o;
            _nextId = _orders.Keys.Max() + 1;
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository", $"seeded {_orders.Count} work orders (in-memory, per session)");
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM WorkOrders");
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(o => o.Id).Select(o => o.Clone()).ToList();
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.GetAllAsync", $"{rows.Count} rows");
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM WorkOrders WHERE Id={id}");
            _orders.TryGetValue(id, out var order);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.FindAsync", order == null ? $"#{id} not found" : $"#{id} found");
            return Task.FromResult(order?.Clone());
        }

        public Task<WorkOrder> UpsertAsync(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            EnsureAvailable(order.Id == 0 ? "INSERT INTO WorkOrders" : $"UPDATE WorkOrders WHERE Id={order.Id}");

            if (order.Id == 0)
                order.Id = _nextId++;

            _orders[order.Id] = order.Clone();
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.UpsertAsync", $"#{order.Id} written ({_orders.Count} rows)");
            return Task.FromResult(order.Clone());
        }

        private void EnsureAvailable(string statement)
        {
            if (!SimulateOutage)
                return;

            // What a real driver would say — and exactly what must not reach the user.
            _log.Error(LogLayer.Data, "InMemoryWorkOrderRepository", null,
                $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders)");
            throw new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
        }
    }

    /// <summary>The work orders the walkthrough video shows in the TicketOps Console (due dates relative to today).</summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            var today = DateTime.Today;
            yield return Saved(new WorkOrder { Id = 2001, Title = "Replace HVAC filter — Building A", Status = WorkOrderStatus.Open, Priority = WorkOrderPriority.Medium, AssignedTo = "R. Alvarez", DueDate = today.AddDays(4), Cost = 240m });
            yield return Saved(new WorkOrder { Id = 2002, Title = "Repair loading dock pump", Status = WorkOrderStatus.Open, Priority = WorkOrderPriority.High, AssignedTo = "T. Nguyen", DueDate = today, Cost = 1850m });
            yield return Saved(new WorkOrder { Id = 2003, Title = "Quarterly elevator inspection", Status = WorkOrderStatus.Scheduled, Priority = WorkOrderPriority.Medium, AssignedTo = "S. Patel", DueDate = today.AddDays(8), Cost = 600m });
            yield return Saved(new WorkOrder { Id = 2004, Title = "Replace lobby lighting", Status = WorkOrderStatus.Open, Priority = WorkOrderPriority.Low, AssignedTo = "J. Kim", DueDate = today.AddDays(11), Cost = 320m });
            yield return Saved(new WorkOrder { Id = 2005, Title = "Calibrate pressure sensors", Status = WorkOrderStatus.InProgress, Priority = WorkOrderPriority.High, AssignedTo = "T. Nguyen", DueDate = today.AddDays(-1), Cost = 980m });
            yield return Saved(new WorkOrder { Id = 2006, Title = "Paint stairwell B", Status = WorkOrderStatus.Scheduled, Priority = WorkOrderPriority.Low, AssignedTo = "M. Chen", DueDate = today.AddDays(18), Cost = 450m });
        }

        private static WorkOrder Saved(WorkOrder o)
        {
            o.AcceptChanges();
            return o;
        }
    }
}
