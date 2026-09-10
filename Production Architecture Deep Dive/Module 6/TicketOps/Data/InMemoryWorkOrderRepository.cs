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
    /// In-memory work-order store seeded with the TicketOps demo queue. One instance per session
    /// (created in AppComposition), so two browser tabs never share a queue — the same reason nothing
    /// here is static.
    ///
    /// The interesting part is <see cref="BeginTransaction"/>: a transaction buffers its writes and
    /// its commit applies them to a working copy of the store, which replaces the live store only when
    /// every statement succeeded. <see cref="SimulateOutage"/> makes the second statement of a commit
    /// (the audit INSERT) fail like a real driver would, after the status UPDATE already "ran" — so the
    /// lab can show that nothing was partially applied.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly object _gate = new object();
        private readonly ILog _log;
        private Dictionary<int, WorkOrder> _workOrders = new Dictionary<int, WorkOrder>();
        private List<ApprovalRecord> _audit = new List<ApprovalRecord>();
        private List<string> _notifications = new List<string>();
        private int _transactionCount;

        /// <summary>Lab switch for the error path: the next commit fails on its audit INSERT and rolls back.</summary>
        public bool SimulateOutage { get; set; }

        public int AuditCount { get { lock (_gate) return _audit.Count; } }
        public int NotificationCount { get { lock (_gate) return _notifications.Count; } }

        public InMemoryWorkOrderRepository(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            foreach (var wo in SeedData.WorkOrders())
                _workOrders[wo.Id] = wo;
            _audit.AddRange(SeedData.AuditTrail());
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository", $"seeded {_workOrders.Count} work orders, {_audit.Count} audit rows (in-memory, per session)");
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            EnsureAvailable("SELECT * FROM WorkOrders");
            IReadOnlyList<WorkOrder> rows;
            lock (_gate)
                rows = _workOrders.Values.Select(Clone).ToList();
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.GetAllAsync", $"{rows.Count} rows");
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            EnsureAvailable($"SELECT * FROM WorkOrders WHERE Id={id}");
            WorkOrder wo;
            lock (_gate)
                _workOrders.TryGetValue(id, out wo);
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.FindAsync", wo == null ? $"#{id} not found" : $"#{id} found ({wo.Number} · {wo.Status})");
            return Task.FromResult(wo == null ? null : Clone(wo));
        }

        public Task<IReadOnlyList<ApprovalRecord>> GetAuditTrailAsync()
        {
            IReadOnlyList<ApprovalRecord> rows;
            lock (_gate)
                rows = _audit.ToList();
            return Task.FromResult(rows);
        }

        public IWorkOrderTransaction BeginTransaction()
        {
            int id;
            lock (_gate)
                id = ++_transactionCount;
            _log.Info(LogLayer.Data, "InMemoryWorkOrderRepository.BeginTransaction", $"tx#{id} begin — writes are buffered until commit");
            return new InMemoryTransaction(this, id);
        }

        /// <summary>
        /// The commit: statements run against a working copy; the copy becomes the live store only after the
        /// last statement succeeded. When the outage switch is on the audit INSERT throws — the status
        /// UPDATE that ran before it is discarded with the copy, so the caller observes zero writes.
        /// </summary>
        private void Commit(InMemoryTransaction tx)
        {
            lock (_gate)
            {
                var workOrders = _workOrders.ToDictionary(p => p.Key, p => Clone(p.Value));
                var audit = _audit.ToList();
                var notifications = _notifications.ToList();
                int applied = 0;
                int total = tx.Updates.Count + tx.Audit.Count + tx.Notifications.Count;
                string source = "tx#" + tx.Id;

                try
                {
                    foreach (var wo in tx.Updates)
                    {
                        workOrders[wo.Id] = Clone(wo);
                        applied++;
                        _log.Info(LogLayer.Data, source, $"UPDATE WorkOrders SET Status='{wo.Status}' WHERE Id={wo.Id} → {applied}/{total} on the working copy");
                    }

                    foreach (var record in tx.Audit)
                    {
                        if (SimulateOutage)
                            throw Outage($"INSERT INTO ApprovalAudit ({record.WorkOrderNumber}) in tx#{tx.Id}");
                        audit.Add(record);
                        applied++;
                        _log.Info(LogLayer.Data, source, $"INSERT INTO ApprovalAudit ({record}) → {applied}/{total} on the working copy");
                    }

                    foreach (var note in tx.Notifications)
                    {
                        notifications.Add(note);
                        applied++;
                        _log.Info(LogLayer.Data, source, $"INSERT INTO Outbox ({note}) → {applied}/{total} on the working copy");
                    }
                }
                catch
                {
                    // The working copy is simply dropped: the live store never saw a single statement.
                    _log.Warn(LogLayer.Data, source, $"rolled back — {applied} of {total} statements discarded, live store unchanged");
                    throw;
                }

                _workOrders = workOrders;
                _audit = audit;
                _notifications = notifications;
                _log.Info(LogLayer.Data, source, $"commit — {applied} of {total} statements applied atomically");
            }
        }

        private void EnsureAvailable(string statement)
        {
            if (SimulateOutage)
                throw Outage(statement);
        }

        private DataOutageException Outage(string statement)
        {
            // What a real driver would say — and exactly what must not reach the user.
            _log.Error(LogLayer.Data, "InMemoryWorkOrderRepository", null,
                $"outage: {statement} failed — timeout connecting to sql01:1433 (TicketOps.dbo.WorkOrders)");
            return new DataOutageException("Timeout connecting to sql01:1433 while executing: " + statement);
        }

        private static WorkOrder Clone(WorkOrder w) => new WorkOrder
        {
            Id = w.Id,
            Number = w.Number,
            Title = w.Title,
            Requester = w.Requester,
            Amount = w.Amount,
            Status = w.Status,
            RequestedAt = w.RequestedAt,
            DecidedBy = w.DecidedBy,
            DecidedAtUtc = w.DecidedAtUtc,
            DecisionComments = w.DecisionComments
        };

        /// <summary>Buffers the writes of one approval; the repository applies them together in Commit.</summary>
        private sealed class InMemoryTransaction : IWorkOrderTransaction
        {
            private readonly InMemoryWorkOrderRepository _owner;
            private bool _committed;

            public int Id { get; }
            public List<WorkOrder> Updates { get; } = new List<WorkOrder>();
            public List<ApprovalRecord> Audit { get; } = new List<ApprovalRecord>();
            public List<string> Notifications { get; } = new List<string>();

            public InMemoryTransaction(InMemoryWorkOrderRepository owner, int id)
            {
                _owner = owner;
                Id = id;
            }

            public void Update(WorkOrder workOrder)
            {
                if (workOrder == null) throw new ArgumentNullException(nameof(workOrder));
                Updates.Add(Clone(workOrder));
            }

            public void RecordAudit(ApprovalRecord record)
            {
                if (record == null) throw new ArgumentNullException(nameof(record));
                Audit.Add(record);
            }

            public void QueueNotification(string recipient, string message)
            {
                Notifications.Add($"to {recipient}: {message}");
            }

            public Task CommitAsync()
            {
                if (_committed)
                    throw new InvalidOperationException($"tx#{Id} was already committed.");
                _owner.Commit(this);
                _committed = true;
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                if (!_committed && (Updates.Count + Audit.Count + Notifications.Count) > 0)
                    _owner._log.Info(LogLayer.Data, "tx#" + Id, "disposed without commit — buffered writes dropped (rollback)");
            }
        }
    }

    /// <summary>The work orders the walkthrough video shows in the TicketOps Console (2002 is the one it approves).</summary>
    public static class SeedData
    {
        public static IEnumerable<WorkOrder> WorkOrders()
        {
            yield return new WorkOrder { Id = 2001, Number = "WO-2001", Title = "Replace warehouse lighting", Requester = "J. Okafor", Amount = 2400m, Status = WorkOrderStatus.Approved, RequestedAt = DateTime.Now.AddDays(-4), DecidedBy = "approver@ticketops", DecidedAtUtc = DateTime.UtcNow.AddDays(-3), DecisionComments = "Within the Q2 facilities budget." };
            yield return new WorkOrder { Id = 2002, Number = "WO-2002", Title = "Repair loading dock pump", Requester = "T. Nguyen", Amount = 1850m, RequestedAt = DateTime.Now.AddDays(-2) };
            yield return new WorkOrder { Id = 2003, Number = "WO-2003", Title = "Forklift battery replacement", Requester = "R. Alvarez", Amount = 3200m, RequestedAt = DateTime.Now.AddDays(-1) };
            yield return new WorkOrder { Id = 2004, Number = "WO-2004", Title = "Cold-room compressor service", Requester = "M. Chen", Amount = 940m, RequestedAt = DateTime.Now.AddHours(-20) };
            yield return new WorkOrder { Id = 2005, Number = "WO-2005", Title = "Paint line 3 safety markings", Requester = "S. Patel", Amount = 410m, RequestedAt = DateTime.Now.AddHours(-6) };
            yield return new WorkOrder { Id = 2006, Number = "WO-2006", Title = "Conveyor belt tensioner", Requester = "L. Fischer", Amount = 1275m, Status = WorkOrderStatus.Rejected, RequestedAt = DateTime.Now.AddDays(-5), DecidedBy = "approver@ticketops", DecidedAtUtc = DateTime.UtcNow.AddDays(-4), DecisionComments = "Duplicate of WO-1988 — already scheduled." };
        }

        public static IEnumerable<ApprovalRecord> AuditTrail()
        {
            yield return new ApprovalRecord { WorkOrderId = 2001, WorkOrderNumber = "WO-2001", Action = ApprovalAction.Approve, Comments = "Within the Q2 facilities budget.", DecidedBy = "approver@ticketops", DecidedAtUtc = DateTime.UtcNow.AddDays(-3) };
            yield return new ApprovalRecord { WorkOrderId = 2006, WorkOrderNumber = "WO-2006", Action = ApprovalAction.Reject, Comments = "Duplicate of WO-1988 — already scheduled.", DecidedBy = "approver@ticketops", DecidedAtUtc = DateTime.UtcNow.AddDays(-4) };
        }
    }
}
