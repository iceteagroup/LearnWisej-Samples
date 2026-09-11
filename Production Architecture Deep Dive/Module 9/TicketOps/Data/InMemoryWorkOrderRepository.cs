using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory repository seeded with the work orders the Module 9 video shows. One instance per session
    /// (created in AppComposition), so two browser tabs never share a list — the same reason nothing here
    /// is static.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();

        public InMemoryWorkOrderRepository()
        {
            foreach (var w in SeedData.WorkOrders())
                _orders[w.Id] = w;
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(w => w.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order == null ? null : Clone(order));
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

    /// <summary>The work orders the walkthrough video shows in "TicketOps — Work Orders", plus one confidential row.</summary>
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
