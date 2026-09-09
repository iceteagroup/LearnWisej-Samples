using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Diagnostics
{
    public enum HealthStatus { Healthy, Degraded, Unhealthy }

    public sealed class HealthCheckResult
    {
        public string Name { get; init; }
        public HealthStatus Status { get; init; }
        public string Detail { get; init; }
    }

    public sealed class HealthReport
    {
        public HealthStatus Status { get; init; }
        public IReadOnlyList<HealthCheckResult> Checks { get; init; }
        public DateTime CheckedUtc { get; init; }
        public string CorrelationId { get; init; }

        public IEnumerable<HealthCheckResult> NotHealthy => Checks.Where(c => c.Status != HealthStatus.Healthy);
    }

    /// <summary>
    /// The in-app health check: the same questions a /healthz probe answers, computed from the session's own
    /// services. Built-in checks: structured log, performance budgets, session memory. Dependencies the page
    /// owns (the work-order store, the background job) register a probe.
    /// </summary>
    public sealed class HealthCheck
    {
        private readonly PerformanceBudget _budget;
        private readonly SessionMemoryAudit _memoryAudit;
        private readonly StructuredLog _log;
        private readonly List<(string Name, Func<(HealthStatus Status, string Detail)> Probe)> _probes =
            new List<(string, Func<(HealthStatus, string)>)>();

        public HealthCheck(PerformanceBudget budget, SessionMemoryAudit memoryAudit, StructuredLog log)
        {
            _budget = budget ?? throw new ArgumentNullException(nameof(budget));
            _memoryAudit = memoryAudit ?? throw new ArgumentNullException(nameof(memoryAudit));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public void AddProbe(string name, Func<(HealthStatus Status, string Detail)> probe)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A probe needs a name.", nameof(name));
            _probes.Add((name, probe ?? throw new ArgumentNullException(nameof(probe))));
        }

        /// <summary>Runs every check. Writes a structured entry only when asked (the live tick runs every second).</summary>
        public HealthReport Run(string correlationId, bool writeLog = false)
        {
            var checks = new List<HealthCheckResult>();

            foreach ((string name, Func<(HealthStatus Status, string Detail)> probe) in _probes)
            {
                try
                {
                    (HealthStatus status, string detail) = probe();
                    checks.Add(new HealthCheckResult { Name = name, Status = status, Detail = detail });
                }
                catch (Exception ex)
                {
                    checks.Add(new HealthCheckResult { Name = name, Status = HealthStatus.Unhealthy, Detail = "probe threw " + ex.GetType().Name });
                }
            }

            BudgetRow[] over = _budget.OverBudget.ToArray();
            checks.Add(new HealthCheckResult
            {
                Name = "performance budgets",
                Status = over.Length == 0 ? HealthStatus.Healthy : HealthStatus.Degraded,
                Detail = over.Length == 0
                    ? $"{_budget.Rows.Count(r => r.Status == BudgetStatus.Ok)} of {_budget.Rows.Count} rows measured within budget"
                    : "over budget: " + string.Join(", ", over.Select(r => $"{r.Operation} {r.MeasuredText} (≤ {r.BudgetMs} ms, correlation {r.CorrelationId})")),
            });

            AuditReport memory = _memoryAudit.Run(correlationId, writeLog: false);
            checks.Add(new HealthCheckResult
            {
                Name = "session memory",
                Status = memory.Passed ? HealthStatus.Healthy : HealthStatus.Degraded,
                Detail = memory.Summary,
            });

            int errors = _log.ErrorCount;
            checks.Add(new HealthCheckResult
            {
                Name = "structured log",
                Status = errors == 0 ? HealthStatus.Healthy : HealthStatus.Degraded,
                Detail = $"{_log.Entries.Count}/{StructuredLog.Capacity} entries buffered · {errors} error(s) this session",
            });

            HealthStatus overall = checks.Count == 0 ? HealthStatus.Healthy : checks.Max(c => c.Status);
            var report = new HealthReport { Status = overall, Checks = checks, CheckedUtc = DateTime.UtcNow, CorrelationId = correlationId };

            if (writeLog)
            {
                _log.Write(overall == HealthStatus.Healthy ? LogLevel.Information : LogLevel.Warning, "HealthCheck", correlationId, new
                {
                    status = overall.ToString().ToLowerInvariant(),
                    checks = checks.Count,
                    notHealthy = string.Join(",", report.NotHealthy.Select(c => c.Name)),
                });
            }

            return report;
        }
    }
}
