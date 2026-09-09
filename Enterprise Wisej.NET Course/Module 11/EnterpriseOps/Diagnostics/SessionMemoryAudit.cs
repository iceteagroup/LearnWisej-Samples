using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>Something a session retains: registered by whoever owns it, measured by the audit.</summary>
    public sealed class RetainedHolder
    {
        public string Name { get; init; }
        public string Reason { get; init; }
        public bool Bounded { get; init; }
        public Func<long> Bytes { get; init; }
        public Func<string> Lifetime { get; init; }
    }

    public enum AuditVerdict { Ok, Review, OverBudget }

    public sealed class RetainedItem
    {
        public string Name { get; init; }
        public string Reason { get; init; }
        public string Lifetime { get; init; }
        public long Bytes { get; init; }
        public AuditVerdict Verdict { get; init; }
        public string Advice { get; init; }

        public string VerdictText => Verdict switch
        {
            AuditVerdict.Ok => "ok",
            AuditVerdict.Review => "REVIEW",
            _ => "OVER BUDGET",
        };
    }

    public sealed class TimerState
    {
        public string Name { get; init; }
        public bool Running { get; init; }
        public string StoppedWhere { get; init; }
    }

    public sealed class AuditReport
    {
        public IReadOnlyList<RetainedItem> Items { get; init; }
        public IReadOnlyList<TimerState> Timers { get; init; }
        public long TotalRetainedBytes { get; init; }
        public long ManagedHeapBytes { get; init; }
        public string CorrelationId { get; init; }

        public int Flagged => Items.Count(i => i.Verdict != AuditVerdict.Ok);
        public bool Passed => Flagged == 0 && TotalRetainedBytes <= SessionMemoryAudit.PerSessionBudgetBytes;

        public string Summary => Passed
            ? $"{Items.Count} holders, ≈{SessionMemoryAudit.Mb(TotalRetainedBytes)} retained (budget {SessionMemoryAudit.Mb(SessionMemoryAudit.PerSessionBudgetBytes)})"
            : $"{Flagged} flagged: " + string.Join(", ", Items.Where(i => i.Verdict != AuditVerdict.Ok).Select(i => $"{i.Name} ≈{SessionMemoryAudit.Mb(i.Bytes)}"));
    }

    /// <summary>
    /// The session-memory review, as code. Every collection a session keeps registers itself with a reason
    /// and a lifetime; every timer registers a probe. Run() answers the three review questions:
    /// which fields hold collections, which timers/subscriptions run, what is retained for the whole session —
    /// and challenges every item over the per-item budget.
    /// </summary>
    public sealed class SessionMemoryAudit
    {
        public const long PerItemBudgetBytes = 2L * 1024 * 1024;
        public const long PerSessionBudgetBytes = 8L * 1024 * 1024;

        private readonly StructuredLog _log;
        private readonly List<RetainedHolder> _holders = new List<RetainedHolder>();
        private readonly List<Func<TimerState>> _timers = new List<Func<TimerState>>();

        public SessionMemoryAudit(StructuredLog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void Register(RetainedHolder holder)
        {
            if (holder == null) throw new ArgumentNullException(nameof(holder));
            if (holder.Bytes == null || holder.Lifetime == null) throw new ArgumentException("A holder needs Bytes and Lifetime probes.", nameof(holder));
            _holders.Add(holder);
        }

        public void RegisterTimer(Func<TimerState> probe)
        {
            _timers.Add(probe ?? throw new ArgumentNullException(nameof(probe)));
        }

        public AuditReport Run(string correlationId, bool writeLog = true)
        {
            var items = new List<RetainedItem>();

            foreach (RetainedHolder holder in _holders)
            {
                long bytes = holder.Bytes();
                AuditVerdict verdict;
                string advice;

                if (bytes > PerItemBudgetBytes)
                {
                    verdict = AuditVerdict.OverBudget;
                    advice = holder.Bounded
                        ? "bounded but large — lower the cap or page it"
                        : "unbounded and large — release it after use, or do not retain it at all";
                }
                else if (!holder.Bounded)
                {
                    verdict = AuditVerdict.Review;
                    advice = "unbounded — it will grow with usage; bound it or clear it on close";
                }
                else
                {
                    verdict = AuditVerdict.Ok;
                    advice = "";
                }

                items.Add(new RetainedItem
                {
                    Name = holder.Name,
                    Reason = holder.Reason,
                    Lifetime = holder.Lifetime(),
                    Bytes = bytes,
                    Verdict = verdict,
                    Advice = advice,
                });
            }

            var report = new AuditReport
            {
                Items = items,
                Timers = _timers.Select(t => t()).ToList(),
                TotalRetainedBytes = items.Sum(i => i.Bytes),
                ManagedHeapBytes = GC.GetTotalMemory(forceFullCollection: false),
                CorrelationId = correlationId,
            };

            if (writeLog)
            {
                _log.Write(report.Passed ? LogLevel.Information : LogLevel.Warning, "SessionMemoryAudit", correlationId, new
                {
                    holders = items.Count,
                    retainedBytes = report.TotalRetainedBytes,
                    managedHeapBytes = report.ManagedHeapBytes,
                    flagged = string.Join(",", items.Where(i => i.Verdict != AuditVerdict.Ok).Select(i => i.Name)),
                    timersRunning = string.Join(",", report.Timers.Where(t => t.Running).Select(t => t.Name)),
                    verdict = report.Passed ? "passed" : "flagged",
                });
            }

            return report;
        }

        public static string Mb(long bytes) => $"{bytes / 1048576.0:N1} MB";
    }
}
