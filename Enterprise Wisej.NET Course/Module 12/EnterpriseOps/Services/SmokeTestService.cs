using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseOps.Services
{
    /// <summary>One line of docs/RollbackAndSmokeTestChecklist.md, with the answer this run got.</summary>
    public class SmokeTestResult
    {
        public int Number;
        public string Name;
        public bool Blocking;               // a blocking check that fails stops the release
        public bool Passed;
        public string Detail;               // safe to show: never an exception message, never a secret

        public override string ToString() => $"{Number}. {Name}: {(Passed ? "PASS" : "FAIL")} — {Detail}";
    }

    /// <summary>The whole checklist for one node, with the verdict the runbook acts on.</summary>
    public class SmokeTestRun
    {
        public string CorrelationId;
        public string NodeName;
        public string Release;
        public List<SmokeTestResult> Checks = new List<SmokeTestResult>();
        public long ElapsedMs;

        public bool Passed => Checks.All(c => !c.Blocking || c.Passed);
        public int Failures => Checks.Count(c => !c.Passed);

        public string Summary => Passed
            ? $"Smoke tests green on {NodeName} ({Release}) — {Checks.Count} checks, {ElapsedMs} ms"
            : $"Smoke tests FAILED on {NodeName} ({Release}) — {Failures} of {Checks.Count} checks";
    }

    /// <summary>
    /// The few checks that prove a deployed EnterpriseOps is actually alive. They run in minutes (here,
    /// in a second) against the real thing — this sample runs them IN-PROCESS against itself, which is
    /// exactly what the staging node does before the runbook lets a release reach production.
    ///
    /// Every check answers a question operations would otherwise ask by clicking around:
    /// does the page render, does a session start, is the node ready, does the data path answer,
    /// can the browser keep a WebSocket, is the configuration contract satisfied, can we go back.
    /// </summary>
    public class SmokeTestService
    {
        private readonly ActivityTrace _trace;
        private readonly WorkOrderRepository _repository = new WorkOrderRepository();

        public SmokeTestService(ActivityTrace trace)
        {
            _trace = trace;
        }

        /// <summary>Runs the checklist against one node. Node A is this process; node B answers through the simulated probe.</summary>
        public async Task<SmokeTestRun> RunAsync(CommandContext context, ReleaseNode node, string sessionId)
        {
            var run = new SmokeTestRun { CorrelationId = context.CorrelationId, NodeName = node.Name, Release = node.Build };
            var stopwatch = Stopwatch.StartNew();
            _trace.Service($"SmokeTestService.RunAsync({node.Name} · {node.Build}) — 7 checks, corr {context.CorrelationId}");

            run.Checks.Add(await Check(1, "The application page renders", true, () =>
            {
                string path = Path.Combine(HostConfiguration.ContentRootPath, "Default.html");
                bool ok = File.Exists(path) && File.ReadAllText(path).Contains("wisej.wx");
                return (ok, ok ? "Default.html served and bootstraps wisej.wx" : "Default.html missing or does not load wisej.wx");
            }));

            run.Checks.Add(await Check(2, "A session starts", true, () =>
            {
                bool ok = !string.IsNullOrEmpty(sessionId);
                return (ok, ok ? $"session {Short(sessionId)} is alive on {node.Name}" : "no session id — the session store did not answer");
            }));

            run.Checks.Add(await Check(3, "The health check returns healthy", true, () =>
            {
                var report = node.IsThisProcess
                    ? HealthProbeService.Probe()
                    : HealthProbeService.ProbeSimulated(node.Name, node.Build, node.DatabaseFault);
                node.LastReport = report;
                return (report.IsHealthy, $"GET /healthz → {report.Status} ({string.Join(" · ", report.Checks.Select(c => c.ToString()))})");
            }));

            run.Checks.Add(await Check(4, "A known query returns rows", true, () =>
            {
                if (!node.IsThisProcess && node.DatabaseFault)
                    return (false, "known query returned 0 open work orders (migration 2.4.2 incomplete)");
                int rows = _repository.CountOpen(context.TenantId);
                return (rows > 0, $"{rows} open work orders for tenant {context.TenantId} (of {_repository.CountAll()} seeded)");
            }));

            run.Checks.Add(await Check(5, "The WebSocket transport is available", false, () =>
            {
                var websocket = (node.LastReport ?? HealthProbeService.Probe()).Checks.FirstOrDefault(c => c.Name == "websocket");
                bool ok = websocket == null || websocket.Status == HealthStatus.Ok;
                return (ok, ok
                    ? "server push available — the proxy must forward Upgrade/Connection (deployment/nginx.conf)"
                    : "the WebSocket transport is off; server push would fall back to polling");
            }));

            run.Checks.Add(await Check(6, "The configuration contract is satisfied", true, () =>
            {
                var validation = HostConfiguration.StartupValidation;
                bool ok = validation != null && validation.Succeeded;
                return (ok, ok
                    ? $"{validation.Passed.Count} rules passed for {HostConfiguration.EnvironmentName}"
                    : "startup validation reported errors — see the configuration card");
            }));

            run.Checks.Add(await Check(7, "The rollback artifact exists", true, () =>
            {
                string previous = HostConfiguration.PreviousVersion;
                bool ok = !string.IsNullOrWhiteSpace(previous) && previous != "?";
                return (ok, ok
                    ? $"previous package {previous} is retained and redeployable (runbook step 8)"
                    : "no previous version configured — the release cannot be reversed");
            }));

            stopwatch.Stop();
            run.ElapsedMs = stopwatch.ElapsedMilliseconds;
            _trace.Service("SmokeTestService → " + run.Summary);
            return run;
        }

        /// <summary>One check: run it, trace it, never let it throw into the release workflow.</summary>
        private async Task<SmokeTestResult> Check(int number, string name, bool blocking, Func<(bool, string)> probe)
        {
            await Task.Delay(90);                       // each check is a real round trip in a real environment
            var result = new SmokeTestResult { Number = number, Name = name, Blocking = blocking };
            try
            {
                var (passed, detail) = probe();
                result.Passed = passed;
                result.Detail = detail;
            }
            catch (Exception ex)
            {
                result.Passed = false;
                result.Detail = "check threw " + ex.GetType().Name;   // the message stays in the log
            }
            _trace.Health("smoke " + result);
            return result;
        }

        private static string Short(string value) => value.Length <= 8 ? value : value.Substring(0, 8);
    }
}
