using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Domain
{
    /// <summary>What a single health check reported. Mirrors one entry of HealthCheck.json "checks".</summary>
    public enum HealthStatus { Ok, Fail, Skipped }

    public class HealthCheckResult
    {
        public string Name;
        public bool Critical;
        public HealthStatus Status;
        public string Detail;          // safe to expose: never an exception message or a connection string

        public override string ToString() => $"{Name}: {(Status == HealthStatus.Ok ? "OK" : Status == HealthStatus.Fail ? "FAIL" : "skipped")}";
    }

    /// <summary>
    /// The JSON body of GET /healthz — the same shape for the real node (this process) and for the
    /// simulated peer node on the release dashboard. Only fields HealthCheck.json marks safeToExpose.
    /// </summary>
    public class HealthReport
    {
        public string Status => IsHealthy ? "Healthy" : "Unhealthy";
        public string Node;
        public string Release;
        public string Environment;
        public DateTime TimestampUtc;
        public List<HealthCheckResult> Checks = new List<HealthCheckResult>();

        /// <summary>Readiness rule: every critical check must be OK; non-critical failures only degrade.</summary>
        public bool IsHealthy => Checks.All(c => !c.Critical || c.Status == HealthStatus.Ok);

        public bool IsDegraded => IsHealthy && Checks.Any(c => c.Status == HealthStatus.Fail);

        /// <summary>The one-line form the dashboard shows under the node grid.</summary>
        public string ToSummaryLine()
        {
            string checks = string.Join(" · ", Checks.Select(c => c.ToString()));
            return $"{Node} {{ \"status\": \"{Status}\", \"release\": \"{Release}\", \"checks\": [ {checks} ] }}";
        }
    }

    /// <summary>What the load balancer is doing with a node right now.</summary>
    public enum BalancerState { InRotation, Draining, RoutedAway, Deploying }

    /// <summary>
    /// One application instance behind the balancer. app-node-A is this process (its health comes
    /// from the real probe); app-node-B is a simulated peer so one machine can show a two-node release.
    /// </summary>
    public class ReleaseNode
    {
        public string Name;
        public string Build;                 // the release version currently running on the node
        public bool IsThisProcess;           // true → HealthProbeService.Probe(); false → simulated
        public BalancerState Balancer = BalancerState.InRotation;
        public int PinnedSessions;           // sessions the balancer has affinity-pinned to this node
        public bool DatabaseFault;           // the simulated failure: 2.4.2's migration broke the DB check
        public HealthReport LastReport;

        public bool IsHealthy => LastReport == null || LastReport.IsHealthy;

        public string HealthText => Balancer == BalancerState.Deploying ? "Deploying…" : (IsHealthy ? "Healthy" : "Unhealthy");

        public string BalancerText
        {
            get
            {
                switch (Balancer)
                {
                    case BalancerState.RoutedAway: return "ROUTED AWAY — no new sessions";
                    case BalancerState.Draining: return $"draining sessions ({PinnedSessions} pinned)";
                    case BalancerState.Deploying: return "out of rotation — deploying";
                    default: return $"receiving traffic · {PinnedSessions} sessions pinned";
                }
            }
        }
    }

    /// <summary>Projection bound to dgvNodes — never the entity itself.</summary>
    public class NodeRow
    {
        public string Node { get; set; }
        public string Build { get; set; }
        public string Health { get; set; }
        public string LoadBalancer { get; set; }
    }
}
