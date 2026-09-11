using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    public class WorkflowResult
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public MigrationStep FailedStep { get; set; }
        public HarnessResult Harness { get; set; }
        public int CompletedSteps { get; set; }
        public string CorrelationId { get; set; }
    }

    /// <summary>
    /// The incremental path: seven verifiable steps, each with a check and a fallback point. The workflow
    /// refuses to run past a failed step, refuses to "fix forward" on a broken build, and applies a rollback
    /// only for a user who may — every refusal is a typed result the screen shows, never an exception.
    /// </summary>
    public class MigrationWorkflow
    {
        private readonly MigrationInventoryStore _store;
        private readonly MigrationAssessmentService _assessment;
        private readonly RegressionHarnessService _harness;
        private readonly ThemeService _theme;
        private readonly PermissionService _permissions;
        private readonly WorkOrderService _workOrders;
        private readonly ActivityTrace _trace;
        private List<MigrationStep> _steps;

        public MigrationWorkflow(MigrationInventoryStore store, MigrationAssessmentService assessment, RegressionHarnessService harness,
            ThemeService theme, PermissionService permissions, WorkOrderService workOrders, ActivityTrace trace)
        {
            _store = store;
            _assessment = assessment;
            _harness = harness;
            _theme = theme;
            _permissions = permissions;
            _workOrders = workOrders;
            _trace = trace;
            _steps = store.Steps();
        }

        public IReadOnlyList<MigrationStep> Steps => _steps;

        public MigrationStep FailedStep => _steps.FirstOrDefault(s => s.State == StepState.Failed);

        public bool IsComplete => _steps.All(s => s.State == StepState.Passed);

        public HarnessResult LastHarness { get; private set; }

        /// <summary>Runs from the first step that has not passed. Stops at the first failed check and names the fallback point.</summary>
        public async Task<WorkflowResult> RunAsync(CommandContext ctx, Action<MigrationStep> stepProgress, Action<RegressionFlow> flowProgress, CancellationToken cancellation)
        {
            if (!_assessment.IsBuilt)
            {
                _trace.Service("MigrationWorkflow.RunAsync refused → the dossier is not built; the path runs against the dossier rows");
                return Fail(ctx, "Build the dossier first — the migration path proves dossier rows, and there are none yet.");
            }

            var failed = FailedStep;
            if (failed != null)
            {
                _trace.Service($"MigrationWorkflow.RunAsync refused → step {failed.Number} {failed.Name} is Failed; roll back to \"{failed.FallbackPoint}\" before re-running");
                return Fail(ctx, $"Step {failed.Number} ({failed.Name}) failed — roll back to its fallback point \"{failed.FallbackPoint}\" before re-running. Never fix forward on a broken build.", failed);
            }

            if (IsComplete)
                return new WorkflowResult { Succeeded = true, Message = "Migration path already complete — 7/7 steps passed.", CompletedSteps = 7, CorrelationId = ctx.CorrelationId };

            _trace.Service($"MigrationWorkflow.RunAsync for {ctx} → resuming at step {_steps.First(s => s.State != StepState.Passed).Number}");

            foreach (var step in _steps.Where(s => s.State != StepState.Passed))
            {
                cancellation.ThrowIfCancellationRequested();

                step.State = StepState.Running;
                step.Detail = "running…";
                stepProgress?.Invoke(step);
                _trace.Job($"step {step.Number}/7 {step.Name} — running · check: {step.Check} · fallback point: \"{step.FallbackPoint}\"");
                await Task.Delay(450, cancellation);

                var (ok, detail, harness) = await ExecuteStepAsync(step, ctx, flowProgress, cancellation);
                step.Detail = detail;

                if (ok)
                {
                    step.State = StepState.Passed;
                    _assessment.MarkRows(step.Number, DossierRowState.Proven);
                    _trace.Job($"step {step.Number}/7 {step.Name} → PASSED — {detail}");
                    stepProgress?.Invoke(step);
                    continue;
                }

                step.State = StepState.Failed;
                _assessment.MarkRows(step.Number, DossierRowState.Failed);
                _trace.Job($"step {step.Number}/7 {step.Name} → FAILED — path halted at fallback point \"{step.FallbackPoint}\" (steps {step.Number + 1}–7 not started)");
                stepProgress?.Invoke(step);

                string next = step.Number < 7 ? $" · do not proceed to step {step.Number + 1}" : "";
                return new WorkflowResult
                {
                    Succeeded = false,
                    Message = $"Regression harness: {detail}{next}",
                    FailedStep = step,
                    Harness = harness,
                    CompletedSteps = _steps.Count(s => s.State == StepState.Passed),
                    CorrelationId = ctx.CorrelationId
                };
            }

            _trace.Service("MigrationWorkflow → migration path complete 7/7 — every dossier row proven");
            return new WorkflowResult
            {
                Succeeded = true,
                Message = "Migration path complete — 7/7 steps passed, 7/7 dossier rows proven.",
                Harness = LastHarness,
                CompletedSteps = 7,
                CorrelationId = ctx.CorrelationId
            };
        }

        private async Task<(bool ok, string detail, HarnessResult harness)> ExecuteStepAsync(MigrationStep step, CommandContext ctx, Action<RegressionFlow> flowProgress, CancellationToken cancellation)
        {
            switch (step.Number)
            {
                case 1:
                    return (true, "dotnet build net10.0-windows;net10.0 → 0 errors, 1 expected warning (CS7022)", null);

                case 2:
                    return (true, $"session {ctx.CorrelationId} started, Default.json startup resolved, MainPage = MigrationDossierPage", null);

                case 3:
                {
                    var result = await _harness.RunAsync(ctx, f => f.Id <= 5, flowProgress, cancellation);
                    LastHarness = result;
                    return (result.Succeeded, $"flows 1–5 {result.Verdict}", result);
                }

                case 4:
                {
                    // The full harness: a theme change can touch every screen, so every flow runs — and the
                    // visual diff (flows 6–8) is where an unmapped theme is caught.
                    var result = await _harness.RunAsync(ctx, null, flowProgress, cancellation);
                    LastHarness = result;
                    return (result.Succeeded, result.Verdict, result);
                }

                case 5:
                {
                    var result = await _harness.RunAsync(ctx, f => f.Id >= 9, flowProgress, cancellation);
                    LastHarness = result;
                    return (result.Succeeded, $"flows 9–10 {result.Verdict}", result);
                }

                case 6:
                    return (true, "staging deploy: /healthz answered 200 in 41 ms; previous package kept for rollback (simulated)", null);

                case 7:
                {
                    var watch = Stopwatch.StartNew();
                    var page = _workOrders.Query(new WorkQueueQuery { Bucket = QueueBucket.Open }, ctx);
                    watch.Stop();
                    const int budgetMs = 250;
                    bool ok = watch.ElapsedMilliseconds <= budgetMs;
                    return (ok, $"work queue query ({page.Total} rows) in {watch.ElapsedMilliseconds} ms — budget {budgetMs} ms", null);
                }

                default:
                    return (false, "unknown step", null);
            }
        }

        /// <summary>Rolls the failed step back to its fallback point. Needs Manager or Admin — the build changes for everyone.</summary>
        public async Task<CommandResult> RollbackAsync(CommandContext ctx)
        {
            var step = FailedStep;
            if (step == null)
            {
                _trace.Service("MigrationWorkflow.RollbackAsync → nothing to roll back: no step is in the Failed state");
                return CommandResult.Fail(ctx, "Nothing to roll back — no step is in the Failed state. Run the migration path first.");
            }

            var permission = _permissions.Check(ctx, Permission.RollbackMigrationStep);
            if (!permission.Allowed)
                return CommandResult.Fail(ctx, permission.Reason);

            _trace.Service($"step {step.Number} {step.Name} → fallback point \"{step.FallbackPoint}\": restoring the pre-step state");
            await Task.Delay(400);

            switch (step.Number)
            {
                case 4:
                    _theme.RestoreFolderCopy();
                    break;
                default:
                    _trace.Data($"fallback \"{step.FallbackPoint}\" restored (simulated)");
                    break;
            }

            step.State = StepState.RolledBack;
            step.Detail = $"rolled back to \"{step.FallbackPoint}\" — fix, then re-run";
            _assessment.MarkRows(step.Number, DossierRowState.RolledBack);
            _trace.Service($"step {step.Number} state Failed → RolledBack; steps {step.Number + 1}–7 untouched; dossier rows for step {step.Number} reopened");

            return CommandResult.Ok(ctx, $"Rolled back to fallback point \"{step.FallbackPoint}\" — map the theme mixin, then re-run the harness.");
        }

        /// <summary>Resource mapping for the "Themes / resources" row. Refused while step 4 is Failed: fix on the fallback, never on the broken build.</summary>
        public CommandResult MapThemeMixin(CommandContext ctx)
        {
            var permission = _permissions.Check(ctx, Permission.MapThemeMixin);
            if (!permission.Allowed)
                return CommandResult.Fail(ctx, permission.Reason);

            var failed = FailedStep;
            if (failed != null && failed.Number == 4)
            {
                _trace.Service("MigrationWorkflow.MapThemeMixin refused → step 4 is Failed; roll back to \"theme folder copy\" first");
                return CommandResult.Fail(ctx, "Step 4 is in the Failed state — roll back to \"theme folder copy\" first. Fix on the fallback point, never on the broken build.");
            }

            if (_theme.Current.IsMapped)
                return CommandResult.Ok(ctx, "The theme mixin is already mapped — 0 differences from the 3.5 baseline.");

            _trace.Service("resource mapping: Themes/Blue-2019/*.json (3.x) → Themes/Blue-2019.mixin.json (4.x mixin over Bootstrap-4)");
            _theme.ApplyMappedMixin();

            var diff = _theme.Diff();
            _trace.Service($"theme diff vs 3.5 baseline: {diff.Count} difference(s){(diff.Count == 0 ? " → ready to re-run the harness from step 4" : " → " + string.Join(" · ", diff))}");

            return diff.Count == 0
                ? CommandResult.Ok(ctx, "Theme mixin mapped — 3/3 tokens match the 3.5 baseline. Re-run the migration path from step 4.")
                : CommandResult.Fail(ctx, diff.ToArray());
        }

        private static WorkflowResult Fail(CommandContext ctx, string message, MigrationStep failedStep = null)
        {
            return new WorkflowResult { Succeeded = false, Message = message, FailedStep = failedStep, CorrelationId = ctx.CorrelationId };
        }
    }
}
