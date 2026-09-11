using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The server-side application service. It is deliberately the ONLY code that changes a work order —
    /// the online screen calls it, and the reconnect sync replays every queued command through it. That is
    /// what makes every permission check, concurrency check and audit entry apply to offline work exactly as
    /// it applies to online work.
    ///
    /// The methods are synchronous in-memory operations exposed behind Task-returning wrappers: the page
    /// awaits them, the sync workflow (running inside Application.StartTask) calls the sync cores directly.
    /// </summary>
    public class WorkOrderService
    {
        private readonly FakeWorkOrderRepository _repository;
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly IActivityTrace _trace;

        public WorkOrderService(FakeWorkOrderRepository repository, PermissionService permissions, AuditLog audit, IActivityTrace trace)
        {
            _repository = repository;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>The technician's assignments — the only rows the device is allowed to cache.</summary>
        public Task<List<WorkOrder>> GetAssignedAsync(CommandContext ctx)
        {
            var rows = _repository.FindAssigned(ctx.TenantId, ctx.User);
            _trace.Log(TraceLayer.Data, $"repository.FindAssigned({ctx.TenantId}, {ctx.User}) → {rows.Count} rows (of {_repository.Count} in the store)");
            return Task.FromResult(rows);
        }

        public Task<CommandResult> CompleteAsync(CompleteWorkOrderCommand command, CommandContext ctx, DateTimeOffset? deviceTimestamp = null)
            => Task.FromResult(Complete(command, ctx, deviceTimestamp));

        /// <summary>
        /// Complete a work order. Decisions, in order: permission (current, on the server) → row exists →
        /// version matches (else Conflict, with the server's version attached) → apply → audit.
        /// </summary>
        public CommandResult Complete(CompleteWorkOrderCommand command, CommandContext ctx, DateTimeOffset? deviceTimestamp = null)
        {
            string code = "WO-" + command.WorkOrderId;

            // 1. Security — always the server's CURRENT answer, never the device's cached set.
            if (!_permissions.IsGranted(ctx.User, Permissions.CompleteWorkOrder))
            {
                _trace.Log(TraceLayer.Security, $"{ctx.User} lacks {Permissions.CompleteWorkOrder} → denied ({ctx.CorrelationId})");
                _audit.Append(ctx.TenantId, ctx.User, "Complete", code, "Rejected", "permission revoked before sync", ctx.CorrelationId, deviceTimestamp);
                return CommandResult.Denied(ctx.CorrelationId, $"{ctx.User} is no longer allowed to complete work orders (permission {Permissions.CompleteWorkOrder} revoked).");
            }

            var row = _repository.Find(ctx.TenantId, command.WorkOrderId);
            if (row == null)
            {
                _trace.Log(TraceLayer.Data, $"{code} not found for tenant {ctx.TenantId}");
                return CommandResult.Failed(ctx.CorrelationId, $"{code} does not exist for tenant {ctx.TenantId}.");
            }

            // 2. Concurrency — the command says which version the technician saw.
            if (row.Version != command.ExpectedVersion && !command.OverrideServerChange)
            {
                var server = WorkOrderSnapshot.From(row);
                _trace.Log(TraceLayer.Service, $"{code} version mismatch: command based on v{command.ExpectedVersion}, server is v{row.Version} ({server}) → Conflict");
                _audit.Append(ctx.TenantId, ctx.User, "Complete", code, "Conflict",
                    $"local: Completed \"{command.Notes}\" · server: {server}", ctx.CorrelationId, deviceTimestamp);
                return CommandResult.Conflict(ctx.CorrelationId, server, $"{code} changed on the server while you were offline.");
            }

            // 3. Override — product rule: applying a local completion over a server change needs workorder.override.
            if (command.OverrideServerChange)
            {
                if (!_permissions.IsGranted(ctx.User, Permissions.OverrideConflict))
                {
                    _trace.Log(TraceLayer.Security, $"{ctx.User} ({_permissions.RoleOf(ctx.User)}) lacks {Permissions.OverrideConflict} → override denied");
                    _audit.Append(ctx.TenantId, ctx.User, "Override", code, "Rejected", "workorder.override required (Manager/Admin)", ctx.CorrelationId, deviceTimestamp);
                    return CommandResult.Denied(ctx.CorrelationId, $"Applying your completion over the dispatcher's change needs {Permissions.OverrideConflict} (Manager). Ask the dispatcher, or keep the server version and attach your notes.");
                }
                _trace.Log(TraceLayer.Security, $"{ctx.User} holds {Permissions.OverrideConflict} → override allowed");
            }

            // 4. Apply, under the store's own version check.
            int newVersion = _repository.Update(ctx.TenantId, row.Id, row.Version, r =>
            {
                r.Status = WorkOrderStatus.Completed;
                r.Notes = AppendNote(r.Notes, $"{ctx.User} {command.CompletedAt.ToLocalTime():HH:mm}: {command.Notes}");
                r.LastChangedBy = ctx.User;
            });
            if (newVersion < 0)
            {
                var server = WorkOrderSnapshot.From(_repository.Find(ctx.TenantId, row.Id));
                _trace.Log(TraceLayer.Data, $"{code} moved on between read and write → Conflict");
                return CommandResult.Conflict(ctx.CorrelationId, server, $"{code} changed while the command was being applied.");
            }

            _trace.Log(TraceLayer.Data, $"repository.Update {code} v{row.Version} → v{newVersion} · Completed");
            _audit.Append(ctx.TenantId, ctx.User, command.OverrideServerChange ? "Override" : "Complete", code, "Applied",
                $"\"{command.Notes}\"", ctx.CorrelationId, deviceTimestamp);
            return CommandResult.Ok(ctx.CorrelationId, newVersion, $"{code} completed (v{newVersion}).");
        }

        /// <summary>
        /// Conflict resolution "Keep server — attach my notes": the server row stays as it is (e.g. Cancelled),
        /// the technician's notes are appended so no field information is lost. Any technician may do this.
        /// </summary>
        public Task<CommandResult> AttachNotesAsync(int workOrderId, string notes, CommandContext ctx, DateTimeOffset? deviceTimestamp = null)
            => Task.FromResult(AttachNotes(workOrderId, notes, ctx, deviceTimestamp));

        public CommandResult AttachNotes(int workOrderId, string notes, CommandContext ctx, DateTimeOffset? deviceTimestamp = null)
        {
            string code = "WO-" + workOrderId;
            var row = _repository.Find(ctx.TenantId, workOrderId);
            if (row == null)
                return CommandResult.Failed(ctx.CorrelationId, $"{code} does not exist.");

            int newVersion = _repository.Update(ctx.TenantId, row.Id, row.Version, r =>
            {
                r.Notes = AppendNote(r.Notes, $"{ctx.User} (offline {deviceTimestamp?.ToLocalTime():HH:mm}, not applied): {notes}");
                r.LastChangedBy = ctx.User;
            });
            _trace.Log(TraceLayer.Data, $"repository.Update {code} v{row.Version} → v{newVersion} · notes attached, status stays {row.Status}");
            _audit.Append(ctx.TenantId, ctx.User, "AttachNotes", code, "Resolved", $"kept server ({row.Status}), notes preserved: \"{notes}\"", ctx.CorrelationId, deviceTimestamp);
            return CommandResult.Ok(ctx.CorrelationId, newVersion, $"{code}: server version kept, your notes are attached.");
        }

        /// <summary>The dispatcher's action (the "server changed while offline" half of the conflict).</summary>
        public Task<CommandResult> CancelAsync(int workOrderId, string reason, CommandContext ctx)
        {
            string code = "WO-" + workOrderId;
            if (!_permissions.IsGranted(ctx.User, Permissions.CancelWorkOrder))
                return Task.FromResult(CommandResult.Denied(ctx.CorrelationId, $"{ctx.User} may not cancel work orders."));

            var row = _repository.Find(ctx.TenantId, workOrderId);
            if (row == null)
                return Task.FromResult(CommandResult.Failed(ctx.CorrelationId, $"{code} does not exist."));
            if (row.Status == WorkOrderStatus.Cancelled)
                return Task.FromResult(CommandResult.Failed(ctx.CorrelationId, $"{code} is already cancelled."));

            int newVersion = _repository.Update(ctx.TenantId, row.Id, row.Version, r =>
            {
                r.Status = WorkOrderStatus.Cancelled;
                r.Notes = AppendNote(r.Notes, reason);
                r.LastChangedBy = ctx.User;
            });
            _trace.Log(TraceLayer.Data, $"repository.Update {code} v{row.Version} → v{newVersion} · Cancelled by {ctx.User}");
            _audit.Append(ctx.TenantId, ctx.User, "Cancel", code, "Applied", reason, ctx.CorrelationId);
            return Task.FromResult(CommandResult.Ok(ctx.CorrelationId, newVersion, $"{code} cancelled on the server (v{newVersion})."));
        }

        /// <summary>Server-side validation of device output: a scanned barcode is untrusted input.</summary>
        public Task<CommandResult> ValidateScannedAssetAsync(int workOrderId, string barcode, CommandContext ctx)
        {
            string code = "WO-" + workOrderId;
            if (string.IsNullOrWhiteSpace(barcode) || !barcode.StartsWith("ASSET-", StringComparison.Ordinal))
            {
                _trace.Log(TraceLayer.Service, $"scanned value \"{barcode}\" rejected — not an asset tag");
                return Task.FromResult(CommandResult.Failed(ctx.CorrelationId, "The scanned code is not an asset tag."));
            }

            var row = _repository.Find(ctx.TenantId, workOrderId);
            bool matches = row != null && barcode.IndexOf(row.Site.Replace(" ", "").Substring(0, Math.Min(4, row.Site.Replace(" ", "").Length)), StringComparison.OrdinalIgnoreCase) >= 0;
            _trace.Log(TraceLayer.Service, $"scanned {barcode} for {code} @ {row?.Site} → {(matches ? "asset belongs to the work-order site ✓" : "asset does not match the site — technician warned")}");
            return Task.FromResult(matches
                ? CommandResult.Ok(ctx.CorrelationId, row.Version, $"{barcode} matches {code} ({row.Site}).")
                : CommandResult.Failed(ctx.CorrelationId, $"{barcode} does not belong to {code}'s site."));
        }

        private static string AppendNote(string existing, string note)
            => string.IsNullOrEmpty(existing) ? note : existing + " | " + note;
    }
}
