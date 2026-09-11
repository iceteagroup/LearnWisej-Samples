using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseOps.Services
{
    /// <summary>One runbook step changing state — the page paints one chip per report.</summary>
    public class ReleaseProgress
    {
        public int StepNumber;
        public string Key;
        public RunbookStepState State;
        public string Message;
    }

    /// <summary>Typed result of a release action. The UI shows this; it never sees a node, a probe or an exception.</summary>
    public class ReleaseResult
    {
        public bool Succeeded;
        public List<string> Errors = new List<string>();
        public string CorrelationId;
        public string Message;
        public bool RollbackArmed;                 // step 8 is now the next thing a human must do
        public SmokeTestRun SmokeTests;
        public string FailedStepKey;
    }

    /// <summary>
    /// The release runbook, executed. Eight steps, in order, every one of them traced — and the last one
    /// is rollback, because a release procedure that cannot be reversed is not finished.
    ///
    /// The service owns every decision (who may release, when a step failed, whether rollback is armed,
    /// what the balancer should do with a node). The Release dashboard only starts it and paints what
    /// comes back, which is why the handlers stay three lines long.
    /// </summary>
    public class ReleaseService
    {
        private readonly ActivityTrace _trace;
        private readonly LoadBalancerSimulator _balancer;
        private readonly SmokeTestService _smokeTests;
        private readonly SessionContext _session;

        // The 2.4.2 package carries a database migration that does not complete on app-node-B the first time
        // it is deployed — the failure runbook step 8 exists for. Once rolled back, the team ships the fix and
        // the next deploy of the package passes.
        private bool _migrationFixed;

        public ReleaseService(ActivityTrace trace, LoadBalancerSimulator balancer, SmokeTestService smokeTests, SessionContext session)
        {
            _trace = trace;
            _balancer = balancer;
            _smokeTests = smokeTests;
            _session = session;

            Steps = RunbookStep.CreateDefault();
            Candidate = new ReleasePackage
            {
                Version = HostConfiguration.ReleaseVersion,
                Artifact = $"enterpriseops-{HostConfiguration.ReleaseVersion}.zip (image tag enterpriseops:{HostConfiguration.ReleaseVersion})",
                ReleaseNotes = "Work-order archive migration, release dashboard, forwarded-headers support.",
                IntroducesDbMigration = true,
            };
            Previous = new ReleasePackage
            {
                Version = HostConfiguration.PreviousVersion,
                Artifact = $"enterpriseops-{HostConfiguration.PreviousVersion}.zip (image tag enterpriseops:{HostConfiguration.PreviousVersion})",
                ReleaseNotes = "The package currently proven in production.",
                IntroducesDbMigration = false,
            };
        }

        public List<RunbookStep> Steps { get; }

        public ReleasePackage Candidate { get; }

        public ReleasePackage Previous { get; }

        /// <summary>True once a deploy failed its smoke tests or its health check: step 8 is waiting for a human.</summary>
        public bool RollbackArmed { get; private set; }

        /// <summary>
        /// Runbook steps 1–8 against one node. The peer node (app-node-B) is the one that gets the new
        /// package first, exactly like a real rolling deployment: node A keeps serving its pinned sessions
        /// throughout, which is the whole point of sticky sessions.
        /// </summary>
        public async Task<ReleaseResult> DeployAsync(CommandContext context, Action<ReleaseProgress> progress)
        {
            var result = new ReleaseResult { CorrelationId = context.CorrelationId, Succeeded = true };

            _trace.Security(ReleaseAuthorization.Explain(_session, "deploy"));
            if (!ReleaseAuthorization.CanDeploy(_session))
            {
                result.Succeeded = false;
                result.Errors.Add($"{_session.User} ({_session.Role}) may not deploy — Manager or Admin only");
                result.Message = "Deploy refused by policy";
                return result;
            }

            ResetRunbook(context, progress);
            var node = _balancer.NodeB;

            // 1 — the artifact is named and versioned before anything moves.
            await Step(1, progress, () =>
            {
                _trace.Runbook($"artifact {Candidate.Artifact} · notes: {Candidate.ReleaseNotes}");
                return (true, $"release {Candidate.Version} (previous {Previous.Version})");
            });

            // 2 — the configuration contract for the environment this node will run in.
            bool configOk = true;
            await Step(2, progress, () =>
            {
                var validation = HostConfiguration.StartupValidation;
                configOk = validation != null && validation.Succeeded;
                _trace.Host($"environment={HostConfiguration.EnvironmentName} · secrets from environment variables, never from the package");
                return (configOk, configOk
                    ? $"{validation.Passed.Count} configuration rules passed for {HostConfiguration.EnvironmentName}"
                    : "configuration contract broken — the node would refuse to boot");
            });
            if (!configOk)
                return Fail(result, 2, "config + secrets", "The release stopped before touching a node: the configuration contract is broken.", progress);

            // 3 — the node leaves the pool, drains its pinned sessions and takes the new package.
            await Step(3, progress, () =>
            {
                node.Balancer = BalancerState.Deploying;
                node.Build = Candidate.Version;
                node.DatabaseFault = Candidate.IntroducesDbMigration && !_migrationFixed;
                _trace.Balancer($"{node.Name} out of rotation — draining {node.PinnedSessions} pinned session(s); {_balancer.NodeA.Name} keeps serving");
                return (true, $"{node.Name} staged on {Candidate.Version}");
            });

            // 4 — the smoke tests, against the staged node.
            var smoke = await _smokeTests.RunAsync(context, node, _session.SessionId);
            result.SmokeTests = smoke;
            Report(progress, 4, smoke.Passed ? RunbookStepState.Done : RunbookStepState.Failed, smoke.Summary);
            if (!smoke.Passed)
            {
                _balancer.RefreshHealth();
                return Fail(result, 4, "smoke tests", smoke.Summary, progress);
            }

            // 5 — readiness, as the balancer will ask for it.
            bool healthy = true;
            await Step(5, progress, () =>
            {
                _balancer.RefreshHealth();
                healthy = node.LastReport.IsHealthy;
                return (healthy, node.LastReport.ToSummaryLine());
            });
            if (!healthy)
                return Fail(result, 5, "health + diagnostics", $"{node.Name} failed readiness on {Candidate.Version} — the balancer routed traffic away before a user noticed.", progress);

            // 6 — back into rotation, now on the new package.
            await Step(6, progress, () =>
            {
                node.Balancer = BalancerState.InRotation;
                _trace.Balancer($"{node.Name} passed readiness — back in rotation on {node.Build}");
                return (true, $"{node.Name} serving {node.Build}");
            });

            // 7 — the release is not finished when the deploy finishes.
            await Step(7, progress, () =>
            {
                _balancer.RefreshHealth();
                return (true, $"watching logs and /healthz every {HealthProbeService.Contract.Probe.IntervalSeconds}s");
            });

            // 8 — not needed this time, and that is a result, not an omission.
            Report(progress, 8, RunbookStepState.Pending, $"rollback not needed — {Previous.Version} stays retained and redeployable");
            RollbackArmed = false;

            result.Message = $"Release {Candidate.Version} live on both nodes — smoke tests green, /healthz healthy";
            _trace.Runbook("runbook complete: " + result.Message);
            return result;
        }

        /// <summary>
        /// Runbook step 8. Redeploys the named previous artifact on the failed node, re-runs the smoke
        /// tests and puts it back in rotation. Not a panic — a rehearsed step with a named package.
        /// </summary>
        public async Task<ReleaseResult> RollbackAsync(CommandContext context, Action<ReleaseProgress> progress)
        {
            var result = new ReleaseResult { CorrelationId = context.CorrelationId, Succeeded = true };

            _trace.Security(ReleaseAuthorization.Explain(_session, "roll back"));
            if (!ReleaseAuthorization.CanRollback(_session))
            {
                result.Succeeded = false;
                result.Errors.Add($"{_session.User} ({_session.Role}) may not roll back — Manager or Admin only");
                result.Message = "Rollback refused by policy";
                return result;
            }

            var node = _balancer.NodeB;
            Report(progress, 8, RunbookStepState.Running, $"redeploying {Previous.Artifact} on {node.Name}");
            _trace.Runbook($"step 8 — rollback to {Previous.Version}; {_balancer.NodeA.Name}'s pinned sessions are untouched");

            await Task.Delay(220);
            node.Balancer = BalancerState.Deploying;
            node.Build = Previous.Version;
            node.DatabaseFault = false;              // 2.4.1 predates the migration that broke the database check
            _migrationFixed = true;                  // 2.4.2 is held for a fix; the next deploy carries it
            _trace.Service($"{node.Name} rolled back to {Previous.Version} — the 2.4.2 migration is no longer on this node");

            var smoke = await _smokeTests.RunAsync(context, node, _session.SessionId);
            result.SmokeTests = smoke;
            if (!smoke.Passed)
            {
                node.Balancer = BalancerState.RoutedAway;
                Report(progress, 8, RunbookStepState.Failed, smoke.Summary);
                result.Succeeded = false;
                result.Errors.Add(smoke.Summary);
                result.Message = "Rollback did not restore the node — escalate, keep it out of rotation";
                return result;
            }

            node.Balancer = BalancerState.InRotation;
            _balancer.RefreshHealth();
            RollbackArmed = false;
            SetState(8, RunbookStepState.RolledBack);
            Report(progress, 8, RunbookStepState.RolledBack, $"{node.Name} healthy on {Previous.Version} — back in rotation");

            result.Message = $"Rollback complete — {node.Name} healthy on {Previous.Version}; release {Candidate.Version} held for a fix. Users never saw a broken node.";
            _trace.Runbook("incident logged · " + result.Message);
            return result;
        }

        #region Runbook plumbing (state + one log line per step)

        private void ResetRunbook(CommandContext context, Action<ReleaseProgress> progress)
        {
            RollbackArmed = false;
            foreach (var step in Steps)
            {
                step.State = RunbookStepState.Pending;
                progress?.Invoke(new ReleaseProgress { StepNumber = step.Number, Key = step.Key, State = step.State, Message = null });
            }
            _trace.Runbook($"ReleaseRunbook.md — 8 steps, release {Candidate.Version}, corr {context.CorrelationId}");
        }

        /// <summary>Run one step: mark it running, do the work, mark the outcome. Every step reports, even the boring ones.</summary>
        private async Task Step(int number, Action<ReleaseProgress> progress, Func<(bool, string)> work)
        {
            Report(progress, number, RunbookStepState.Running, null);
            await Task.Delay(180);
            var (ok, message) = work();
            Report(progress, number, ok ? RunbookStepState.Done : RunbookStepState.Failed, message);
        }

        private void Report(Action<ReleaseProgress> progress, int number, RunbookStepState state, string message)
        {
            SetState(number, state);
            var step = Steps.First(s => s.Number == number);
            if (message != null)
                _trace.Runbook($"step {number} {step.Key}: {state.ToString().ToUpperInvariant()} — {message}");
            progress?.Invoke(new ReleaseProgress { StepNumber = number, Key = step.Key, State = state, Message = message });
        }

        private void SetState(int number, RunbookStepState state) => Steps.First(s => s.Number == number).State = state;

        /// <summary>A failed step arms rollback and says so in one sentence a human can act on.</summary>
        private ReleaseResult Fail(ReleaseResult result, int number, string key, string message, Action<ReleaseProgress> progress)
        {
            result.Succeeded = false;
            result.FailedStepKey = key;
            result.Errors.Add(message);
            result.Message = message;

            if (number >= 3)
            {
                // The node was already out of the pool while it deployed; now it stays out. The balancer
                // would have reached the same conclusion on its own after unhealthyThreshold probes —
                // this just records it at the moment the release gate failed.
                var node = _balancer.NodeB;
                if (!node.IsHealthy)
                {
                    node.Balancer = BalancerState.RoutedAway;
                    _trace.Balancer($"{node.Name} failed the release gate — routed away, no NEW sessions; {_balancer.NodeA.Name} keeps every pinned session");
                }

                RollbackArmed = true;
                result.RollbackArmed = true;
                Report(progress, 8, RunbookStepState.Running, $"ARMED — press Rollback to redeploy {Previous.Version} on {node.Name}");
            }
            _trace.Runbook($"release halted at step {number} ({key}) — corr {result.CorrelationId}");
            return result;
        }

        #endregion
    }
}
