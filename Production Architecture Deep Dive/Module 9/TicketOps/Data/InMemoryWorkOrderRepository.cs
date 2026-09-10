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
    /// In-memory repository seeded with the work orders the Module 9 video shows. One instance per session
    /// (created in AppComposition), so two browser tabs never share a list — the same reason nothing here
    /// is static. <see cref="SimulateOutage"/> lets the lab show the error path without a real database.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();

        public bool SimulateOutage { get; set; }

        public InMemoryWorkOrderRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var w in SeedData.WorkOrders())
                _orders[w.Id] = w;
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository", $"seeded {_orders.Count} work orders (in-memory, per session)");
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM WorkOrders");
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.GetAllAsync", $"{_orders.Count} rows");
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(w => w.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM WorkOrders WHERE Id={id}");
            _orders.TryGetValue(id, out var order);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.FindAsync", order == null ? $"#{id} not found" : $"#{id} found");
            return Task.FromResult(order == null ? null : Clone(order));
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

        private static WorkOrder Clone(WorkOrder w) => new WorkOrder
        {
            Id = w.Id,
            Title = w.Title,
            Priority = w.Priority,
            AssignedTo = w.AssignedTo,
            Confidential = w.Confidential
        };
    }

    /// <summary>The work orders the walkthrough video shows in "TicketOps — Work Orders", plus one confidential row for the domain-rule path.</summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            yield return new WorkOrder { Id = 2001, Title = "Replace HVAC filter — Building A", Priority = WorkOrderPriority.Medium, AssignedTo = "R. Alvarez" };
            yield return new WorkOrder { Id = 2002, Title = "Repair loading dock pump", Priority = WorkOrderPriority.High, AssignedTo = "T. Nguyen" };
            yield return new WorkOrder { Id = 2003, Title = "Quarterly elevator inspection", Priority = WorkOrderPriority.Medium, AssignedTo = "S. Patel" };
            yield return new WorkOrder { Id = 2004, Title = "Replace lobby lighting", Priority = WorkOrderPriority.Low, AssignedTo = "J. Kim" };
            yield return new WorkOrder { Id = 2005, Title = "Calibrate pressure sensors", Priority = WorkOrderPriority.High, AssignedTo = "T. Nguyen" };
            yield return new WorkOrder { Id = 2006, Title = "Badge-reader logs for HR investigation", Priority = WorkOrderPriority.High, AssignedTo = "M. Okafor", Confidential = true };
        }
    }
}
