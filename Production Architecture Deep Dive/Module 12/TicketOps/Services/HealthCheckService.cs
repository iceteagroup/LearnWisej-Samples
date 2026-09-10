using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// The health check the load balancer would call. It keeps the two halves apart on purpose:
    /// HealthCheck.json declares the build (version, build, environment) and the dependencies the node needs;
    /// the probes say whether each dependency answers right now. A dependency failure becomes a status in the
    /// report — the node keeps answering the probe, which is exactly how the balancer learns to pull it out.
    ///
    /// No Wisej.NET type here: the repository, the runtime facts and the manifest all arrive through interfaces.
    /// </summary>
    public sealed class HealthCheckService : IHealthCheckService
    {
        private sealed class Override
        {
            public DependencyStatus Status;
            public string Detail;
        }

        private readonly IHealthCheckSource _source;
        private readonly ITicketRepository _repository;
        private readonly IRuntimeInfo _runtime;
        private readonly ILog _log;
        private readonly Dictionary<string, Override> _overrides = new Dictionary<string, Override>(StringComparer.OrdinalIgnoreCase);

        public HealthCheckService(IHealthCheckSource source, ITicketRepository repository, IRuntimeInfo runtime, ILog log)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<HealthReport> CheckAsync()
        {
            _log.Info(LogLayer.Service, "HealthCheckService.CheckAsync", "→ IHealthCheckSource.Load() (HealthCheck.json: what the build declares)");
            var report = _source.Load();

            foreach (var check in report.Checks)
                await ProbeAsync(check);

            report.Refresh();

            string summary = report.ToString();
            if (report.Status == HealthStatus.Healthy)
                _log.Info(LogLayer.Service, "HealthCheckService.CheckAsync", summary);
            else
                _log.Warn(LogLayer.Service, "HealthCheckService.CheckAsync", summary + (report.IsServing() ? " (still serving)" : " (out of rotation)"));

            return report;
        }

        public void OverrideDependency(string name, DependencyStatus status, string detail)
        {
            _overrides[name] = new Override { Status = status, Detail = detail };
            _log.Warn(LogLayer.Service, "HealthCheckService.OverrideDependency", $"{name} forced to {status} — {detail} (lab switch, this session only)");
        }

        public void ClearOverride(string name)
        {
            if (_overrides.Remove(name))
                _log.Info(LogLayer.Service, "HealthCheckService.ClearOverride", $"{name} probes live again");
        }

        public bool IsOverridden(string name) => _overrides.ContainsKey(name);

        private async Task ProbeAsync(DependencyCheck check)
        {
            if (_overrides.TryGetValue(check.Name, out var forced))
            {
                check.Status = forced.Status;
                check.Detail = forced.Detail;
                _log.Warn(LogLayer.Service, "HealthCheckService.Probe", $"{check.Name} → {check.Status} (simulated: {check.Detail})");
                return;
            }

            switch ((check.Name ?? "").ToLowerInvariant())
            {
                case "database":
                    await ProbeDatabaseAsync(check);
                    break;

                case "storage":
                    ProbeStorage(check);
                    break;

                case "websocket":
                    check.Status = _runtime.IsWebSocket ? DependencyStatus.OK : DependencyStatus.Degraded;
                    check.Detail = _runtime.IsWebSocket ? "WebSocket connection active" : "long-polling fallback in use";
                    break;

                default:
                    // Declared but not probed here: report what the manifest says and say so.
                    check.Detail = "declared in HealthCheck.json (no live probe)";
                    break;
            }

            if (check.Status == DependencyStatus.OK)
                _log.Info(LogLayer.Service, "HealthCheckService.Probe", $"{check.Name} → OK ({check.Detail})");
            else
                _log.Warn(LogLayer.Service, "HealthCheckService.Probe", $"{check.Name} → {check.Status} ({check.Detail})");
        }

        /// <summary>Readiness: can this node reach its ticket store? The repository throws like a real driver; the probe turns that into Unhealthy.</summary>
        private async Task ProbeDatabaseAsync(DependencyCheck check)
        {
            try
            {
                var rows = await _repository.GetAllAsync();
                check.Status = DependencyStatus.OK;
                check.Detail = $"{rows.Count} rows reachable";
            }
            catch (Exception ex)
            {
                // The driver's message (host, table) is already in the log from the data layer; the report gets a safe sentence.
                _log.Warn(LogLayer.Service, "HealthCheckService.Probe", $"database probe caught {ex.GetType().Name} → reported as Unhealthy, not thrown");
                check.Status = DependencyStatus.Unhealthy;
                check.Detail = "ticket store unreachable";
            }
        }

        /// <summary>Liveness of the disk the app writes to: a tiny write/delete in the temp folder — cheap, real, no secrets.</summary>
        private void ProbeStorage(DependencyCheck check)
        {
            try
            {
                string path = Path.Combine(Path.GetTempPath(), "ticketops-health-" + Guid.NewGuid().ToString("N") + ".tmp");
                File.WriteAllText(path, "ok");
                File.Delete(path);
                check.Status = DependencyStatus.OK;
                check.Detail = "temp folder writable";
            }
            catch (Exception ex)
            {
                _log.Error(LogLayer.Service, "HealthCheckService.Probe", ex, "storage probe failed");
                check.Status = DependencyStatus.Unhealthy;
                check.Detail = "temp folder not writable";
            }
        }
    }
}
