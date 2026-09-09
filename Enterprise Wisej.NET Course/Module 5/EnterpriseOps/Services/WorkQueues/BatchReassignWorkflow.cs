using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>
    /// The batch reassignment workflow (deliverable 4). Validates the command once, then processes every row
    /// independently: permission → exists → open → not locked → version → certification → apply → audit. A row
    /// that fails never stops the others, and every row ends in the report with a reason. See
    /// docs/BatchReassignmentWorkflow.md and the SVG next to it.
    /// </summary>
    public sealed class BatchReassignWorkflow
    {
        /// <summary>Per-row pause so progress is visible in the UI (a real batch would be as slow as its writes).</summary>
        public static readonly TimeSpan RowDelay = TimeSpan.FromMilliseconds(450);

        private readonly WorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly AuditTrail _audit;
        private readonly ActivityTrace _trace;

        public BatchReassignWorkflow(WorkOrderStore store, PermissionService permissions, AuditTrail audit, ActivityTrace trace)
        {
            _store = store;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        public async Task<BatchResult> RunAsync(ReassignBatchCommand command, Action<BatchProgress> onProgress, CancellationToken cancellation)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            // Command-level validation: these reject the whole batch before any row is touched.
            if (command.Items == null || command.Items.Count == 0)
                throw new ArgumentException("Select at least one work order to reassign.");

            var target = _store.FindTechnician(command.TargetTechnician);
            if (target == null)
                throw new ArgumentException($"'{command.TargetTechnician}' is not a technician.");

            var ctx = command.Context;
            _trace.Write($"Job: {(command.IsRetry ? "retry" : "batch")} {ctx.CorrelationId} — reassign {command.Items.Count} to {target.UserName} " +
                         $"by {ctx.UserName} ({ctx.Role}) @ {ctx.TenantId}");

            var results = new List<BatchRowResult>(command.Items.Count);
            var sw = Stopwatch.StartNew();
            int done = 0;

            foreach (var item in command.Items)
            {
                cancellation.ThrowIfCancellationRequested();

                // Each row is its own unit of work: validate, apply, audit — then move on whatever happened.
                BatchRowResult result = ProcessRow(item, target, ctx);
                results.Add(result);
                _audit.Record(ctx.CorrelationId, ctx.TenantId, ctx.UserName, "reassign→" + target.UserName, item.WorkOrderId,
                              result.Outcome + ": " + result.Code);
                _trace.Write($"Job: {result.Glyph} {item.Number} → {result.Message} [{result.Code}]");

                done++;
                onProgress?.Invoke(new BatchProgress(done, command.Items.Count, item.Number, result.Outcome));

                await Task.Delay(RowDelay, cancellation).ConfigureAwait(false);
            }

            sw.Stop();
            var batch = new BatchResult(ctx.CorrelationId, target.UserName, results, sw.ElapsedMilliseconds);
            _trace.Write($"Job: done {ctx.CorrelationId} — {batch.Summary} in {batch.ElapsedMs} ms; {command.Items.Count} audit entries written");
            return batch;
        }

        private BatchRowResult ProcessRow(BatchItem item, Technician target, CommandContext ctx)
        {
            // 1. Security — decided again here, never trusted from the row the browser sent back.
            var order = _store.Find(item.WorkOrderId);
            if (order == null || order.TenantId != ctx.TenantId)
                return Fail(item, BatchFailureCodes.NotFound, "not found in this tenant — skipped", BatchRowOutcome.Skipped);

            if (!_permissions.CanReassign(ctx.Role, order))
            {
                if (!order.IsOpen)
                    return Fail(item, BatchFailureCodes.Closed, $"already {order.Status} — closed orders cannot be reassigned");
                _trace.Write($"Security: {ctx.UserName} ({ctx.Role}) may not reassign {item.Number}");
                return Fail(item, BatchFailureCodes.PermissionDenied, $"{ctx.Role} may not reassign work orders");
            }

            // 2. Business rules the projection could not know at load time.
            if (order.OpenApprovalId != null)
                return Fail(item, BatchFailureCodes.ApprovalLock, $"locked by an open approval ({order.OpenApprovalId}) — not changed");

            if (order.Version != item.ExpectedVersion)
                return Fail(item, BatchFailureCodes.StaleVersion, $"changed by another user (v{item.ExpectedVersion} → v{order.Version}) — refresh and retry");

            if (!target.Holds(order.RequiredCertification))
                return Fail(item, BatchFailureCodes.Certification, $"{target.UserName} lacks the {order.RequiredCertification} certification");

            if (string.Equals(order.AssignedTo, target.UserName, StringComparison.OrdinalIgnoreCase))
                return Fail(item, BatchFailureCodes.NoChange, $"already assigned to {target.UserName} — skipped", BatchRowOutcome.Skipped);

            // 3. Apply — the store checks the version again under its lock (the row-level "transaction").
            if (!_store.TryReassign(order.Id, item.ExpectedVersion, target.UserName, out var updated))
                return Fail(item, BatchFailureCodes.StaleVersion, "changed while the batch was running — refresh and retry");

            _trace.Write($"Data: commit {item.Number} AssignedTo={target.UserName} Version {item.ExpectedVersion} → {updated.Version}");
            return new BatchRowResult(item.WorkOrderId, item.Number, BatchRowOutcome.Succeeded,
                $"reassigned to {target.UserName} (v{item.ExpectedVersion} → v{updated.Version})", "ok");
        }

        private static BatchRowResult Fail(BatchItem item, string code, string message, BatchRowOutcome outcome = BatchRowOutcome.Failed) =>
            new BatchRowResult(item.WorkOrderId, item.Number, outcome, message, code);
    }
}
