using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Workflow
{
    public enum CompensationKind { NotificationOutstanding, AuditGap, RevertedPersist }

    public enum CompensationStatus { Open, Resolved }

    /// <summary>
    /// One compensating action: which step failed after which steps had succeeded, what was done about it,
    /// and what still has to happen. This is the "manual-review queued" the video's banner names.
    /// </summary>
    public class CompensationEntry
    {
        public int Id;
        public CompensationKind Kind;
        public CompensationStatus Status = CompensationStatus.Open;
        public int? EscalationId;
        public int WorkOrderId;
        public string FailedStep;
        public string Cause;
        public string Action;
        public string CorrelationId;
        public DateTime CreatedUtc = DateTime.UtcNow;
        public DateTime? ResolvedUtc;

        public string Subject => EscalationId.HasValue ? $"ESC-{EscalationId}" : $"WO-{WorkOrderId}";

        public override string ToString()
        {
            string state = Status == CompensationStatus.Open ? "OPEN" : "resolved";
            return $"#{Id} {state,-8} {Kind,-23} {Subject,-9} {FailedStep} ✕ {Cause} → {Action}";
        }
    }

    /// <summary>
    /// Records compensations instead of pretending the workflow was atomic. The manual-review queue is
    /// simply the open entries; RetryNotificationAsync in the workflow resolves them.
    /// </summary>
    public class CompensationLog
    {
        private readonly ActivityTrace _trace;
        private readonly List<CompensationEntry> _entries = new List<CompensationEntry>();
        private int _nextId = 1;

        public event Action Changed;

        public CompensationLog(ActivityTrace trace)
        {
            _trace = trace;
        }

        public IReadOnlyList<CompensationEntry> Entries => _entries;

        public IReadOnlyList<CompensationEntry> ManualReviewQueue =>
            _entries.Where(e => e.Status == CompensationStatus.Open).ToList();

        public CompensationEntry Record(CompensationKind kind, int workOrderId, int? escalationId,
            string failedStep, string cause, string action, string correlationId)
        {
            var entry = new CompensationEntry
            {
                Id = _nextId++,
                Kind = kind,
                WorkOrderId = workOrderId,
                EscalationId = escalationId,
                FailedStep = failedStep,
                Cause = cause,
                Action = action,
                CorrelationId = correlationId,
            };
            _entries.Add(entry);
            _trace.Write($"Service: CompensationLog #{entry.Id} {kind} — {failedStep} failed ({cause}) → {action}");
            Changed?.Invoke();
            return entry;
        }

        public void Resolve(CompensationEntry entry, string how)
        {
            entry.Status = CompensationStatus.Resolved;
            entry.ResolvedUtc = DateTime.UtcNow;
            entry.Action = how;
            _trace.Write($"Service: CompensationLog #{entry.Id} resolved — {how}");
            Changed?.Invoke();
        }
    }
}
