using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory repository seeded with the TicketOps work orders. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list — the same reason nothing here is static.
    /// Writes go through <see cref="IWorkOrderTransaction"/>: staged, then applied all at once on commit.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();
        private readonly List<string> _audit = new List<string>();
        private int _nextId;
        private int _nextTx;

        public IReadOnlyList<string> AuditTrail => _audit;

        public InMemoryWorkOrderRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var o in SeedData.WorkOrders())
                _orders[o.Id] = o;
            _nextId = _orders.Keys.Max() + 1;
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(o => o.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            _orders.TryGetValue(id, out var order);
            return Task.FromResult(order == null ? null : Clone(order));
        }

        public IWorkOrderTransaction BeginTransaction()
        {
            return new Transaction(this, ++_nextTx);
        }

        /// <summary>Applies a staged transaction. This is the only method that changes the visible rows.</summary>
        private WorkOrder Apply(Transaction tx)
        {
            WorkOrder last = null;
            foreach (var staged in tx.Orders)
            {
                var copy = Clone(staged);
                copy.RowVersion = _orders.TryGetValue(copy.Id, out var existing) ? existing.RowVersion + 1 : 1;
                _orders[copy.Id] = copy;
                last = Clone(copy);
            }
            foreach (var entry in tx.AuditEntries)
                _audit.Add(entry);

            return last;
        }

        private void Rollback(Transaction tx)
        {
            _log.Warn(LogLayer.Data, "InMemoryWorkOrderRepository.Rollback",
                $"tx#{tx.Number} rolled back — {tx.Orders.Count} staged write(s) discarded");
        }

        private static WorkOrder Clone(WorkOrder o) => new WorkOrder
        {
            Id = o.Id,
            Title = o.Title,
            AssigneeId = o.AssigneeId,
            Priority = o.Priority,
            DueDate = o.DueDate,
            EstimatedCost = o.EstimatedCost,
            EstimatedHours = o.EstimatedHours,
            Status = o.Status,
            RowVersion = o.RowVersion
        };

        /// <summary>Stages writes; the repository applies them all at once on commit, or none on dispose.</summary>
        private sealed class Transaction : IWorkOrderTransaction
        {
            private readonly InMemoryWorkOrderRepository _repo;
            private bool _finished;   // committed or rolled back

            public int Number { get; }
            public List<WorkOrder> Orders { get; } = new List<WorkOrder>();
            public List<string> AuditEntries { get; } = new List<string>();

            public Transaction(InMemoryWorkOrderRepository repo, int number)
            {
                _repo = repo;
                Number = number;
            }

            public int Upsert(WorkOrder order)
            {
                if (order == null) throw new ArgumentNullException(nameof(order));
                if (_finished) throw new InvalidOperationException("Transaction already finished.");

                var staged = Clone(order);
                if (staged.Id == 0)
                    staged.Id = _repo._nextId++;
                Orders.Add(staged);
                return staged.Id;
            }

            public void Audit(int workOrderId, string action, string actor)
            {
                if (_finished) throw new InvalidOperationException("Transaction already finished.");
                AuditEntries.Add(string.Format(CultureInfo.InvariantCulture, "{0:HH:mm:ss} #{1} {2} by {3}", DateTime.Now, workOrderId, action, actor));
            }

            public Task<WorkOrder> CommitAsync()
            {
                if (_finished) throw new InvalidOperationException("Transaction already finished.");
                var applied = _repo.Apply(this);             // if this throws, _finished stays false → Dispose rolls back
                _finished = true;
                return Task.FromResult(applied);
            }

            public void Dispose()
            {
                if (_finished)
                    return;                                  // committed (or already rolled back): nothing to undo
                _finished = true;
                _repo.Rollback(this);
            }
        }
    }

    /// <summary>The work orders the walkthrough video shows in the TicketOps Console (#2002 is the one it edits).</summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            var today = DateTime.Today;
            yield return new WorkOrder { Id = 2001, Title = "Inspect rooftop HVAC unit", AssigneeId = "M. Okafor", Priority = WorkOrderPriority.Medium, DueDate = today.AddDays(14), EstimatedCost = 600m, EstimatedHours = 3, Status = WorkOrderStatus.New };
            yield return new WorkOrder { Id = 2002, Title = "Repair loading dock pump", AssigneeId = "T. Nguyen", Priority = WorkOrderPriority.High, DueDate = today.AddDays(10), EstimatedCost = 2150m, EstimatedHours = 6, Status = WorkOrderStatus.Assigned };
            yield return new WorkOrder { Id = 2003, Title = "Replace warehouse LED panels", AssigneeId = "S. Patel", Priority = WorkOrderPriority.Low, DueDate = today.AddDays(21), EstimatedCost = 1800m, EstimatedHours = 12, Status = WorkOrderStatus.InProgress };
            yield return new WorkOrder { Id = 2004, Title = "Calibrate freezer thermostats", AssigneeId = "T. Nguyen", Priority = WorkOrderPriority.Medium, DueDate = today.AddDays(5), EstimatedCost = 350m, EstimatedHours = 2, Status = WorkOrderStatus.OnHold };
            yield return new WorkOrder { Id = 2005, Title = "Service backup generator", AssigneeId = "M. Okafor", Priority = WorkOrderPriority.High, DueDate = today.AddDays(-3), EstimatedCost = 2400m, EstimatedHours = 8, Status = WorkOrderStatus.Completed };
            yield return new WorkOrder { Id = 2006, Title = "Replace lobby badge reader", AssigneeId = "S. Patel", Priority = WorkOrderPriority.Low, DueDate = today.AddDays(-12), EstimatedCost = 900m, EstimatedHours = 4, Status = WorkOrderStatus.Closed, RowVersion = 3 };
        }
    }
}
