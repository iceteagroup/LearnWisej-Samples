using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Data
{
    /// <summary>
    /// Per-session fake repository for escalations — the "persist" step's target. The rows survive
    /// a notification failure on purpose: a valid escalation is kept and compensated, never rolled back.
    /// </summary>
    public class InMemoryEscalationStore
    {
        private readonly ActivityTrace _trace;
        private readonly List<Escalation> _rows = new List<Escalation>();
        private int _nextId = 1041;

        public InMemoryEscalationStore(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<Escalation> All => _rows;

        public Escalation Find(int id) => _rows.FirstOrDefault(e => e.Id == id);

        public bool ExistsForWorkOrder(int workOrderId) => _rows.Any(e => e.WorkOrderId == workOrderId);

        public async Task<Escalation> AddAsync(Escalation escalation)
        {
            await Task.Delay(150);                              // a database round trip
            escalation.Id = _nextId++;
            _rows.Add(escalation);
            _trace.Write($"Data: persisted {escalation.Number} for WO-{escalation.WorkOrderId} (approver {escalation.ApproverId}, due {escalation.DueAt:MMM d HH:mm})");
            return escalation;
        }

        public void SetNotificationStatus(int id, NotificationStatus status)
        {
            var row = Find(id);
            if (row == null) return;
            row.NotificationStatus = status;
            _trace.Write($"Data: {row.Number} notification status → {status}");
        }

        /// <summary>Only the anti-pattern demo needs this: it "cleans up" by deleting, which loses the audit trail.</summary>
        public void Remove(int id)
        {
            int removed = _rows.RemoveAll(e => e.Id == id);
            if (removed > 0)
                _trace.Write($"Data: ESC-{id} DELETED");
        }
    }
}
