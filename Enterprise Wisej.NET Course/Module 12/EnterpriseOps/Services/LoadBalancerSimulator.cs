using EnterpriseOps.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services
{
    /// <summary>What the balancer did with one request, and what the user saw because of it.</summary>
    public class RoutingDecision
    {
        public string SessionId;
        public string NodeName;
        public bool SessionPreserved;
        public string Message;
    }

    /// <summary>
    /// A two-node load balancer, simulated in this process so one machine can show a session-aware release.
    ///
    /// app-node-A IS this process: its health comes from the real GET /healthz probe.
    /// app-node-B is a peer: the same checks run, but its database check honours the fault the release
    /// failure path injects (the 2.4.2 migration).
    ///
    /// The rule the whole module exists for: a Wisej.NET session lives in ONE node's memory, so the
    /// balancer must pin it (sticky sessions / affinity). Turn affinity off and the second request lands
    /// on the other node, which has never heard of the session — the user gets a brand new one.
    /// </summary>
    public class LoadBalancerSimulator
    {
        private readonly ActivityTrace _trace;

        // Where the round-robin pointer happens to be when affinity is switched off. It starts on the peer
        // so the first click shows the case operations must design for; with N nodes, only 1 request in N
        // survives by luck, and "by luck" is not a hosting strategy.
        private int _roundRobin = 1;

        public LoadBalancerSimulator(ActivityTrace trace, string thisNodeName, string releaseVersion)
        {
            _trace = trace;
            Nodes = new List<ReleaseNode>
            {
                new ReleaseNode { Name = thisNodeName, Build = releaseVersion, IsThisProcess = true,  PinnedSessions = 1 },
                new ReleaseNode { Name = "app-node-B", Build = releaseVersion, IsThisProcess = false, PinnedSessions = 0 },
            };
        }

        public List<ReleaseNode> Nodes { get; }

        /// <summary>Sticky sessions. On = the balancer pins each session to a node (the production setting).</summary>
        public bool AffinityEnabled { get; private set; } = true;

        public ReleaseNode NodeA => Nodes[0];

        public ReleaseNode NodeB => Nodes[1];

        /// <summary>Re-runs the health checks on every node and lets the balancer act on the answers.</summary>
        public void RefreshHealth()
        {
            foreach (var node in Nodes)
            {
                string previous = node.LastReport?.Status;
                node.LastReport = node.IsThisProcess
                    ? HealthProbeService.Probe()
                    : HealthProbeService.ProbeSimulated(node.Name, node.Build, node.DatabaseFault);

                // The real node knows exactly how many sessions it is holding; that number is what the
                // balancer would lose if it cut the node instead of draining it.
                if (node.IsThisProcess)
                    node.PinnedSessions = HealthProbeService.SessionsStarted;

                // One line per *change*: a probe that keeps saying "Healthy" every ten seconds is not news,
                // and a trace nobody reads is a trace nobody reads.
                if (previous != node.LastReport.Status)
                    _trace.Health($"{node.Name} → {node.LastReport.Status} · {string.Join(" · ", node.LastReport.Checks.Select(c => c.ToString()))}");

                // A node that is mid-deploy is already out of the pool; the release service decides what
                // happens to it next (back in rotation at step 6, or routed away when a step fails).
                if (node.Balancer == BalancerState.Deploying)
                    continue;

                if (!node.LastReport.IsHealthy && node.Balancer != BalancerState.RoutedAway)
                {
                    node.Balancer = BalancerState.RoutedAway;
                    _trace.Balancer($"{node.Name} failed readiness ({FirstFailure(node)}) — routed away, no NEW sessions; {node.PinnedSessions} pinned session(s) keep answering until they drain");
                }
                else if (node.LastReport.IsHealthy && node.Balancer == BalancerState.RoutedAway)
                {
                    node.Balancer = BalancerState.InRotation;
                    _trace.Balancer($"{node.Name} passed readiness {Contract.HealthyThreshold}× — back in rotation");
                }
            }
        }

        private static HealthProbeSettings Contract => HealthProbeService.Contract.Probe;

        private static string FirstFailure(ReleaseNode node)
            => node.LastReport.Checks.FirstOrDefault(c => c.Critical && c.Status == HealthStatus.Fail)?.Detail ?? "critical check failed";

        /// <summary>Turn sticky sessions on or off — the switch operations get wrong once, and only once.</summary>
        public void SetAffinity(bool enabled)
        {
            AffinityEnabled = enabled;
            _trace.Balancer(enabled
                ? "affinity ON (cookie EnterpriseOpsNode) — every request of a session goes to the node holding it"
                : "affinity OFF — requests are spread round-robin, which a session-aware application cannot survive");
        }

        /// <summary>
        /// Route the next request of an existing session. With affinity the pinned node answers; without
        /// it the request lands wherever round-robin points and the session is gone.
        /// </summary>
        public RoutingDecision Route(string sessionId, string pinnedNodeName)
        {
            var healthy = Nodes.Where(n => n.Balancer == BalancerState.InRotation).ToList();
            if (healthy.Count == 0)
                return new RoutingDecision { SessionId = sessionId, NodeName = "(none)", SessionPreserved = false, Message = "no node in rotation — the balancer answers 503" };

            if (AffinityEnabled)
            {
                var pinned = healthy.FirstOrDefault(n => n.Name == pinnedNodeName) ?? healthy[0];
                _trace.Balancer($"request for session {sessionId} → {pinned.Name} (affinity cookie) — session found in memory");
                return new RoutingDecision
                {
                    SessionId = sessionId,
                    NodeName = pinned.Name,
                    SessionPreserved = true,
                    Message = $"{pinned.Name} answered from the session it already holds",
                };
            }

            var target = healthy[_roundRobin++ % healthy.Count];
            bool preserved = target.Name == pinnedNodeName;
            _trace.Balancer($"request for session {sessionId} → {target.Name} (round-robin, no affinity)");
            if (!preserved)
                _trace.Service($"{target.Name} has no session {sessionId} in memory → a NEW session starts; the user's grid, wizard step and unsaved edits are gone");

            return new RoutingDecision
            {
                SessionId = sessionId,
                NodeName = target.Name,
                SessionPreserved = preserved,
                Message = preserved
                    ? $"{target.Name} happened to be the right node this time — luck, not a design"
                    : $"{target.Name} never heard of session {sessionId} — the user is signed out mid-workflow",
            };
        }

        /// <summary>The projection dgvNodes binds to — never the entity.</summary>
        public List<NodeRow> ToRows() => Nodes.Select(n => new NodeRow
        {
            Node = n.Name,
            Build = n.Build,
            Health = n.HealthText,
            LoadBalancer = n.BalancerText,
        }).ToList();
    }
}
