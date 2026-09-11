using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory repository seeded with the six work orders the walkthrough video shows. One instance
    /// per session (created in AppComposition), so two browser tabs never share a list — the same reason
    /// nothing here is static. It stores and returns <b>copies</b>: the bound objects on the screen are
    /// the service's, and a save is an explicit hand-over, not a shared reference.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();
        private int _nextId;

        public InMemoryWorkOrderRepository()
        {
            foreach (var o in SeedData.WorkOrders())
                _orders[o.Id] = o;
            _nextId = _orders.Keys.Max() + 1;
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(o => o.Id).Select(o => o.Clone()).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order?.Clone());
        }

        public Task<WorkOrder> UpsertAsync(WorkOrder order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            if (order.Id == 0)
                order.Id = _nextId++;

            _orders[order.Id] = order.Clone();
            return Task.FromResult(order.Clone());
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
