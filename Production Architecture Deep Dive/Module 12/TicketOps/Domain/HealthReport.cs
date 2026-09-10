using System;
using System.Collections.Generic;
using System.Linq;

namespace TicketOps.Domain
{
    /// <summary>What one dependency probe reports. "OK" is the word the HealthCheck.json manifest uses.</summary>
    public enum DependencyStatus
    {
        OK,
        Degraded,
        Unhealthy
    }

    /// <summary>The node's overall answer to the load balancer: Healthy/Degraded keep serving (200), Unhealthy leaves the rotation (503).</summary>
    public enum HealthStatus
    {
        Healthy,
        Degraded,
        Unhealthy
    }

    /// <summary>
    /// One dependency the node needs. <see cref="Probe"/> says what is checked, in words; <see cref="Detail"/> is a short,
    /// safe explanation of the current status. Neither may carry a host name, a connection string or a secret:
    /// the whole report is public-facing infrastructure (the probe reads it, so may anyone on the network).
    /// </summary>
    public sealed class DependencyCheck
    {
        public string Name { get; set; }
        public DependencyStatus Status { get; set; } = DependencyStatus.OK;
        public string Probe { get; set; }
        public string Detail { get; set; }

        public override string ToString() => $"{Name}:{Status}";
    }

    /// <summary>
    /// The health report: what HealthCheck.json declares about the build (app, version, build, environment, the
    /// dependencies it needs) merged with what the live probes found. Plain C#: it compiles without Wisej.NET,
    /// so the aggregation rule can be unit-tested and reused by a real /health endpoint.
    /// </summary>
    public sealed class HealthReport
    {
        public string App { get; set; }
        public string Version { get; set; }
        public string Build { get; set; }
        public string Environment { get; set; }
        public HealthStatus Status { get; set; } = HealthStatus.Healthy;
        public List<DependencyCheck> Checks { get; set; } = new List<DependencyCheck>();
        public DateTime CheckedAtUtc { get; set; }

        /// <summary>200 while the node may receive traffic, 503 when the balancer must pull it out.</summary>
        public int HttpStatusCode => Status == HealthStatus.Unhealthy ? 503 : 200;

        /// <summary>
        /// The rule, in one place: any Unhealthy dependency takes the node out of rotation; any Degraded one is
        /// reported but the node keeps serving; otherwise the node is Healthy.
        /// </summary>
        public static HealthStatus Aggregate(IEnumerable<DependencyCheck> checks)
        {
            var list = checks?.ToList() ?? new List<DependencyCheck>();
            if (list.Any(c => c.Status == DependencyStatus.Unhealthy))
                return HealthStatus.Unhealthy;
            if (list.Any(c => c.Status == DependencyStatus.Degraded))
                return HealthStatus.Degraded;
            return HealthStatus.Healthy;
        }

        /// <summary>Re-computes <see cref="Status"/> from the checks and stamps the time.</summary>
        public void Refresh()
        {
            Status = Aggregate(Checks);
            CheckedAtUtc = DateTime.UtcNow;
        }

        public bool IsServing() => Status != HealthStatus.Unhealthy;

        public DependencyCheck Find(string name)
            => Checks.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));

        public override string ToString()
            => $"{Status} · {string.Join(", ", Checks.Select(c => c.ToString()))} → HTTP {HttpStatusCode}";
    }
}
