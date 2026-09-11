using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Data
{
    /// <summary>
    /// In-memory work-order store seeded with the TicketOps demo queue. One instance per session
    /// (created in AppComposition), so two browser tabs never share a queue — the same reason nothing
    /// here is static.
    ///
    /// <see cref="BeginTransaction"/>: a transaction buffers its writes and its commit applies them to a
    /// working copy of the store, which replaces the live store only when every statement succeeded.
    /// </summary>
    public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
    {
        private readonly object _gate = new object();
        private Dictionary<int, WorkOrder> _workOrders = new Dictionary<int, WorkOrder>();
        private List<ApprovalRecord> _audit = new List<ApprovalRecord>();
        private List<string> _notifications = new List<string>();

        public int AuditCount { get { lock (_gate) return _audit.Count; } }
        public int NotificationCount { get { lock (_gate) return _notifications.Count; } }

        public InMemoryWorkOrderRepository()
        {
            foreach (var wo in SeedData.WorkOrders())
                _workOrders[wo.Id] = wo;
            _audit.AddRange(SeedData.AuditTrail());
        }

        public Task<IReadOnlyList<WorkOrder>> GetAllAsync()
        {
            IReadOnlyList<WorkOrder> rows;
            lock (_gate)
                rows = _workOrders.Values.Select(Clone).ToList();
            return Task.FromResult(rows);
        }

        public Task<WorkOrder> FindAsync(int id)
        {
            WorkOrder wo;
            lock (_gate)
                _workOrders.TryGetValue(id, out wo);
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
            return new InMemoryTransaction(this);
        }

        /// <summary>
        /// The commit: statements run against a working copy; the copy becomes the live store only after the
        /// last statement succeeded. If a statement throws, the copy is dropped and the caller observes zero writes.
        /// </summary>
        private void Commit(InMemoryTransaction tx)
        {
            lock (_gate)
            {
                var workOrders = _workOrders.ToDictionary(p => p.Key, p => Clone(p.Value));
                var audit = _audit.ToList();
                var notifications = _notifications.ToList();

                foreach (var wo in tx.Updates)
                    workOrders[wo.Id] = Clone(wo);
                audit.AddRange(tx.Audit);
                notifications.AddRange(tx.Notifications);

                _workOrders = workOrders;
                _audit = audit;
                _notifications = notifications;
            }
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

            public List<WorkOrder> Updates { get; } = new List<WorkOrder>();
            public List<ApprovalRecord> Audit { get; } = new List<ApprovalRecord>();
            public List<string> Notifications { get; } = new List<string>();

            public InMemoryTransaction(InMemoryWorkOrderRepository owner)
            {
                _owner = owner;
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
                    throw new InvalidOperationException("The transaction was already committed.");
                _owner.Commit(this);
                _committed = true;
                return Task.CompletedTask;
            }

            public void Dispose()
            {
                // Not committed: the buffered writes are simply dropped (rollback).
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
