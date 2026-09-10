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
    /// Raised by the data layer when the store refuses a write. Its message is deliberately internal
    /// (host names, table names): it belongs in the log, and the screen must never show it.
    /// </summary>
    public sealed class DataOutageException : Exception
    {
        public DataOutageException(string message) : base(message) { }
    }

    /// <summary>
    /// In-memory repository seeded with the TicketOps work orders. One instance per session (created in
    /// AppComposition), so two browser tabs never share a list — the same reason nothing here is static.
    /// <see cref="SimulateWriteOutage"/> makes every COMMIT fail the way a primary that is read-only during
    /// a failover would: reads still answer, so the lab can prove afterwards that nothing was written.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly ILog _log;
        private readonly Dictionary<int, WorkOrder> _orders = new Dictionary<int, WorkOrder>();
        private readonly List<string> _audit = new List<string>();
        private int _nextId;
        private int _nextTx;

        public bool SimulateWriteOutage { get; set; }

        public IReadOnlyList<string> AuditTrail => _audit;

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
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.GetAllAsync", $"SELECT * FROM WorkOrders — {_orders.Count} rows");
            IReadOnlyList<WorkOrder> rows = _orders.Values.OrderBy(o => o.Id).Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            _orders.TryGetValue(id, out var order);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.FindAsync",
                order == null ? $"#{id} not found" : $"#{id} found — {order.Status} · v{order.RowVersion}");
            return Task.FromResult(order == null ? null : Clone(order));
        }

        public IWorkOrderTransaction BeginTransaction()
        {
            var tx = new Transaction(this, ++_nextTx);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.BeginTransaction", $"tx#{tx.Number} open — writes are staged, not visible");
            return tx;
        }

        /// <summary>Applies a staged transaction. This is the only method that changes the visible rows.</summary>
        private WorkOrder Apply(Transaction tx)
        {
            if (SimulateWriteOutage)
            {
                // What a real driver would say — and exactly what must not reach the user.
                string statement = $"COMMIT tx#{tx.Number} ({tx.Statements})";
                _log.Error(LogLayer.Data, "InMemoryWorkOrderRepository", null,
                    $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders is read-only during failover)");
                throw new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
            }

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

            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.Commit",
                $"tx#{tx.Number} committed — {tx.Orders.Count} row(s), {tx.AuditEntries.Count} audit entr{(tx.AuditEntries.Count == 1 ? "y" : "ies")}"
                + (last != null ? $" · #{last.Id} now v{last.RowVersion}" : ""));
            return last;
        }

        private void Rollback(Transaction tx)
        {
            _log.Warn(LogLayer.Data, "InMemoryWorkOrderRepository.Rollback",
                $"tx#{tx.Number} rolled back — 0 rows changed ({tx.Orders.Count} staged write(s) discarded)");
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
            public string Statements => string.Join(", ", Orders.Select(o => $"UPDATE WorkOrders #{o.Id}").Concat(AuditEntries.Select(a => "INSERT AuditLog")));

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
                _repo._log.Info(LogLayer.Data, "Transaction.Upsert",
                    $"tx#{Number} stage {(order.Id == 0 ? "INSERT" : "UPDATE")} WorkOrders #{staged.Id} (pending — readers still see the old row)");
                return staged.Id;
            }

            public void Audit(int workOrderId, string action, string actor)
            {
                if (_finished) throw new InvalidOperationException("Transaction already finished.");
                string entry = string.Format(CultureInfo.InvariantCulture, "{0:HH:mm:ss} #{1} {2} by {3}", DateTime.Now, workOrderId, action, actor);
                AuditEntries.Add(entry);
                _repo._log.Info(LogLayer.Data, "Transaction.Audit", $"tx#{Number} stage INSERT AuditLog \"{entry}\"");
            }

            public Task<WorkOrder> CommitAsync()
            {
                if (_finished) throw new InvalidOperationException("Transaction already finished.");
                var applied = _repo.Apply(this);             // throws on outage: _finished stays false → Dispose rolls back
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
