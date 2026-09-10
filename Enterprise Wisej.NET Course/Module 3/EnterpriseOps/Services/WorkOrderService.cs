using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The tenant-aware application service. Every method takes a <see cref="CommandContext"/> and starts by
    /// asking the <see cref="ITenantGuard"/> whether this context may touch this record — before any business
    /// logic, before any read of the row's fields. The service never reads the session, never reads a control,
    /// and never learns the tenant from its arguments: it learns it from the context the handler passed in.
    ///
    /// The service instance itself is per session (see <see cref="ServiceRegistry"/>) but holds no user state;
    /// the only thing it keeps is a reference to the shared store and the session's trace.
    /// </summary>
    public sealed class WorkOrderService
    {
        private const int SimulatedLatencyMs = 60;

        private readonly IWorkOrderStore _store;
        private readonly ITenantGuard _guard;
        private readonly AuditTrail _audit;
        private readonly ActivityTrace _trace;

        public WorkOrderService(IWorkOrderStore store, ITenantGuard guard, AuditTrail audit, ActivityTrace trace)
        {
            _store = store;
            _guard = guard;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>
        /// The work queue for the tenant on the context. The tenant is not a parameter and not a grid filter —
        /// it comes from the context, so there is no call shape that could ask for another tenant's rows.
        /// </summary>
        public async Task<PagedResult<WorkQueueRow>> QueryAsync(CommandContext context, WorkQueueQuery query)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            query = query ?? new WorkQueueQuery();

            await Task.Delay(SimulatedLatencyMs);

            IReadOnlyList<WorkOrder> matched = _store.QueryByTenant(context.TenantId, query.Search, query.Status);
            _trace.Data($"WorkOrderStore.QueryByTenant(tenant={context.TenantId}) → {matched.Count} of {_store.Count} rows (correlation {context.CorrelationId})");

            List<WorkQueueRow> page = matched
                .Skip((Math.Max(query.Page, 1) - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(ToRow)
                .ToList();

            _trace.Service($"WorkOrderService.QueryAsync → page {query.Page} · {page.Count} rows of {matched.Count} for '{context.TenantId}'");
            return new PagedResult<WorkQueueRow>(page, matched.Count, query.Page, query.PageSize);
        }

        /// <summary>
        /// Opens one work order for editing. The row is fetched by id across every tenant and then handed to
        /// the guard: a record belonging to another customer is rejected loudly with the correlation id, not
        /// silently returned as "not found". The screen gets an edit model carrying the version it loaded.
        /// </summary>
        public async Task<WorkOrderEditModel> OpenAsync(CommandContext context, int workOrderId)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            await Task.Delay(SimulatedLatencyMs / 3);

            WorkOrder row = _store.FindById(workOrderId);
            if (row == null)
            {
                _trace.Service($"WorkOrderService.OpenAsync({workOrderId}) → not found (correlation {context.CorrelationId})");
                return null;
            }

            // Tenant check first — before the title, the status or anything else is looked at.
            _guard.DemandTenant(context, row.TenantId);

            var model = WorkOrderEditModel.From(row);
            _audit.Record(context, "work-order.open", $"#{model.Id} at {model.Token.ToDisplayText()}");
            _trace.Service($"WorkOrderService.OpenAsync({workOrderId}) → \"{model.Title}\" at {model.Token.ToDisplayText()} · this tab now owns that token");
            return model;
        }

        /// <summary>
        /// The save, with the concurrency check in the command handler where it belongs — not in the form, so
        /// a batch job or an import obeys the same rule. Order of business:
        ///
        ///  1. the guard checks the tenant the client claimed (never trusted, always checked);
        ///  2. the row is read and the guard checks the row's real tenant;
        ///  3. validation;
        ///  4. the store compares the expected version with the current one inside its lock and only then writes.
        ///
        /// A version mismatch comes back as a <see cref="ConflictInfo"/> — data the screen can explain — and
        /// the attempt is written to the audit trail with the correlation id either way.
        /// </summary>
        public async Task<SaveWorkOrderResult> SaveAsync(CommandContext context, SaveWorkOrderCommand command)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (command == null) throw new ArgumentNullException(nameof(command));

            // 1. The tenant the client sent is checked against the tenant the session owns.
            _guard.DemandTenant(context, command.TenantId);

            await Task.Delay(SimulatedLatencyMs);

            WorkOrder row = _store.FindById(command.WorkOrderId);
            if (row == null)
                return SaveWorkOrderResult.Rejected(context.CorrelationId, $"Work order #{command.WorkOrderId} no longer exists.");

            // 2. And against the tenant the record actually belongs to.
            _guard.DemandTenant(context, row.TenantId);

            // 3. Validation — an expected failure, reported as data.
            if (string.IsNullOrWhiteSpace(command.Title))
            {
                _trace.Service($"WorkOrderService.SaveAsync → rejected: title is required (correlation {context.CorrelationId})");
                _audit.Record(context, "work-order.save.rejected", $"#{command.WorkOrderId} validation: title is required");
                return SaveWorkOrderResult.Rejected(context.CorrelationId, "A work order needs a title.");
            }

            // 4. The optimistic concurrency check.
            _trace.Service($"WorkOrderService.SaveAsync(#{command.WorkOrderId}) expecting {command.ExpectedVersion.ToDisplayText()} (correlation {context.CorrelationId})");
            StoreSaveResult outcome = _store.TrySave(command.WorkOrderId, command.Title.Trim(), command.Status,
                command.ExpectedVersion.Version, context.UserId);

            if (!outcome.Saved)
            {
                var conflict = new ConflictInfo
                {
                    WorkOrderId = command.WorkOrderId,
                    Yours = WorkOrderEditModel.From(row).WithEdits(command.Title.Trim(), command.Status),
                    Current = WorkOrderEditModel.From(outcome.Current),
                    Expected = command.ExpectedVersion,
                    Found = ConcurrencyToken.From(outcome.Current),
                    CorrelationId = context.CorrelationId,
                };

                _trace.Data($"WorkOrderStore.TrySave → REJECTED · expected {conflict.Expected.ToDisplayText()}, found {conflict.Found.ToDisplayText()} · nothing written");
                _trace.Service($"WorkOrderService.SaveAsync → stale version, returning a ConflictInfo (not an exception): the record changed under this tab");
                _audit.Record(context, "work-order.save.conflict",
                    $"#{command.WorkOrderId} expected {conflict.Expected.ToDisplayText()}, found {conflict.Found.ToDisplayText()} (saved by {conflict.SavedByOther})");
                return SaveWorkOrderResult.Stale(context.CorrelationId, conflict);
            }

            var saved = WorkOrderEditModel.From(outcome.Current);
            _trace.Data($"WorkOrderStore.TrySave → committed · {command.ExpectedVersion.ToDisplayText()} → {saved.Token.ToDisplayText()}");
            _audit.Record(context, "work-order.save",
                $"#{saved.Id} {command.ExpectedVersion.ToDisplayText()} → {saved.Token.ToDisplayText()} \"{saved.Title}\" [{saved.Status}]");
            _trace.Service($"WorkOrderService.SaveAsync → saved · this tab's token is now {saved.Token.ToDisplayText()}");
            return SaveWorkOrderResult.Applied(context.CorrelationId, saved);
        }

        private static WorkQueueRow ToRow(WorkOrder order) => new WorkQueueRow
        {
            Id = order.Id,
            Title = order.Title,
            Status = order.Status.ToString(),
            Priority = order.Priority.ToString(),
            AssignedTo = order.AssignedTo ?? "(unassigned)",
            Version = "v" + order.Version,
            DueUtc = order.DueUtc,
        };
    }
}
