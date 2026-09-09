using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;

namespace EnterpriseOps.Diagnostics
{
    /// <summary>
    /// The diagnostics/health card of the capstone (Modules 11–12 compacted): six probes, each answering
    /// a question the architecture review will ask. Runs as a progress path — one probe at a time with a
    /// small delay so the screen can show it filling in — and honours cancellation.
    /// </summary>
    public sealed class DiagnosticsService
    {
        private readonly ActivityTrace _trace;
        private readonly PermissionService _permissions;
        private readonly InMemoryWorkOrderStore _store;
        private readonly AuditLog _audit;
        private readonly DocumentationIndexService _docs;
        private readonly CapstonePackageService _package;
        private readonly GeneratedCodeReviewService _review;

        public DiagnosticsService(ActivityTrace trace, PermissionService permissions, InMemoryWorkOrderStore store, AuditLog audit,
                                  DocumentationIndexService docs, CapstonePackageService package, GeneratedCodeReviewService review)
        {
            _trace = trace;
            _permissions = permissions;
            _store = store;
            _audit = audit;
            _docs = docs;
            _package = package;
            _review = review;
        }

        /// <summary>The probe list, so the screen can render "pending" rows before the run starts.</summary>
        public List<HealthProbe> CreateProbes() => new List<HealthProbe>
        {
            new HealthProbe { Name = "Session context" },
            new HealthProbe { Name = "Work-order store" },
            new HealthProbe { Name = "Audit log" },
            new HealthProbe { Name = "Documentation index" },
            new HealthProbe { Name = "Capstone package" },
            new HealthProbe { Name = "Review engine" },
        };

        /// <summary>
        /// Runs every probe. <paramref name="onProgress"/> is called after each one on the calling context's
        /// thread (the screen pushes the change with Application.Update). Permission is checked here — the
        /// handler only asked.
        /// </summary>
        public async Task<CommandResult> RunAsync(List<HealthProbe> probes, CommandContext ctx, Func<HealthProbe, Task> onProgress, CancellationToken cancellation)
        {
            string denied = _permissions.Check(ctx.User, Permission.RunDiagnostics);
            if (denied != null)
            {
                _trace.Security($"RunDiagnostics denied — {denied}");
                return CommandResult.Fail(ctx.CorrelationId, denied);
            }
            _trace.Security($"RunDiagnostics granted to {ctx.User}");

            foreach (var probe in probes)
            {
                cancellation.ThrowIfCancellationRequested();
                probe.State = ProbeState.Running;
                var watch = Stopwatch.StartNew();
                await Task.Delay(350, cancellation);          // simulated probe latency (a real probe would hit the DB / disk)
                Evaluate(probe, ctx);
                probe.DurationMs = (int)watch.ElapsedMilliseconds;
                _trace.Job($"probe {probe.Name}: {probe.State} — {probe.Detail} ({probe.DurationMs} ms)");
                if (onProgress != null)
                    await onProgress(probe);
            }

            int degraded = probes.Count(p => p.State != ProbeState.Healthy);
            _trace.Service($"health check complete: {probes.Count - degraded}/{probes.Count} healthy");
            return degraded == 0
                ? CommandResult.Ok(ctx.CorrelationId)
                : CommandResult.Fail(ctx.CorrelationId, $"{degraded} probe(s) degraded");
        }

        private void Evaluate(HealthProbe probe, CommandContext ctx)
        {
            switch (probe.Name)
            {
                case "Session context":
                    probe.State = ctx.User != null && !string.IsNullOrEmpty(ctx.TenantId) ? ProbeState.Healthy : ProbeState.Failed;
                    probe.Detail = $"tenant {ctx.TenantId}, user {ctx.User}, per-session (no statics)";
                    break;
                case "Work-order store":
                    int tenantRows = _store.Query(ctx.TenantId).Count();
                    probe.State = tenantRows > 0 ? ProbeState.Healthy : ProbeState.Degraded;
                    probe.Detail = $"{_store.Count} rows total, {tenantRows} visible to {ctx.TenantId}";
                    break;
                case "Audit log":
                    int denied = _audit.Entries.Count(e => !e.Allowed);
                    probe.State = ProbeState.Healthy;
                    probe.Detail = $"{_audit.Entries.Count} entries this session, {denied} denied";
                    break;
                case "Documentation index":
                    var index = _docs.Load();
                    int missing = index.Count(d => !d.Exists);
                    probe.State = index.Count == 0 ? ProbeState.Failed : missing == 0 ? ProbeState.Healthy : ProbeState.Degraded;
                    probe.Detail = index.Count == 0 ? "docs/index.json not found" : $"{index.Count} documents indexed, {missing} missing on disk";
                    break;
                case "Capstone package":
                    var checks = _package.Verify(ctx, quiet: true);
                    int failed = checks.Count(c => c.Required && !c.Passed);
                    probe.State = failed == 0 ? ProbeState.Healthy : ProbeState.Degraded;
                    probe.Detail = $"{checks.Count(c => c.Passed)}/{checks.Count} checks pass, {failed} required missing";
                    break;
                case "Review engine":
                    probe.State = ProbeState.Healthy;
                    probe.Detail = $"{_review.Rules.Count} checklist rules, {DocumentedApiCatalog.MemberCount} documented members in the API catalog";
                    break;
                default:
                    probe.State = ProbeState.Failed;
                    probe.Detail = "unknown probe";
                    break;
            }
        }
    }
}
