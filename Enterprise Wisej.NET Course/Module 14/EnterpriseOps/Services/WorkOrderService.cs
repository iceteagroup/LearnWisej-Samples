using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The work-queue service: one tenant-scoped read and one command. The command is the capstone's
    /// reference path — <b>permission checked in the service</b>, optimistic version enforced by the store,
    /// <b>one audit line written whether it was allowed or not</b>, and a typed result for the screen.
    ///
    /// Nothing here touches a control, so the whole path can be reviewed (and tested) without the Designer.
    /// </summary>
    public sealed class WorkOrderService
    {
        private readonly InMemoryWorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly AuditLog _audit;
        private readonly ActivityTrace _trace;

        public WorkOrderService(InMemoryWorkOrderStore store, PermissionService permissions, AuditLog audit, ActivityTrace trace)
        {
            _store = store;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>One page of the caller's tenant. The tenant filter is the store's job — a query cannot forget it.</summary>
        public async Task<PagedResult<WorkQueueRow>> GetQueueAsync(WorkQueueQuery query, CommandContext ctx)
        {
            string refusal = _permissions.Check(ctx.User, Permission.ViewWorkQueue);
            if (refusal != null)
            {
                _trace.Security($"ViewWorkQueue denied — {refusal}");
                return new PagedResult<WorkQueueRow> { Page = query.Page, PageSize = query.PageSize };
            }

            var watch = Stopwatch.StartNew();
            await Task.Delay(60).ConfigureAwait(true);
            IEnumerable<WorkOrder> rows = _store.Query(ctx.TenantId);

            switch (query.StatusFilter)
            {
                case "Escalated":
                    rows = rows.Where(w => w.Status == WorkOrderStatus.Escalated);
                    break;
                case "All":
                    break;
                default:
                    rows = rows.Where(DashboardService.IsOpen);
                    break;
            }

            List<WorkOrder> ordered = rows
                .OrderByDescending(w => w.Priority)
                .ThenBy(w => w.DueUtc ?? DateTime.MaxValue)
                .ToList();

            var page = new PagedResult<WorkQueueRow>
            {
                Total = ordered.Count,
                Page = query.Page,
                PageSize = query.PageSize,
                Rows = ordered.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(WorkQueueRow.From).ToList(),
                ElapsedMs = (int)watch.ElapsedMilliseconds,
            };

            _trace.Service($"work queue [{query.StatusFilter}] page {page.Page} → {page.Rows.Count}/{page.Total} rows in {page.ElapsedMs} ms");
            return page;
        }

        /// <summary>
        /// The approve command. Order matters and it is the order the architecture defense claims:
        /// authorize → validate → write with the expected version → audit → return a typed result.
        /// A denial is audited too; an unaudited denial is an invisible attack.
        /// </summary>
        public async Task<ApproveResult> ApproveAsync(ApproveWorkOrderCommand command, CommandContext ctx)
        {
            _trace.Service($"ApproveWorkOrderCommand #{command.WorkOrderId} v{command.ExpectedVersion} corr={ctx.CorrelationId}");

            string refusal = _permissions.Check(ctx.User, Permission.ApproveWorkOrder);
            if (refusal != null)
            {
                var deniedEntry = _audit.Write(ctx.TenantId, ctx.UserName, "ApproveWorkOrder", $"#{command.WorkOrderId}", false, refusal, ctx.CorrelationId);
                _trace.Security($"ApproveWorkOrder denied — {refusal}");
                _trace.Data($"audit ← {deniedEntry}");
                return ApproveResult.Refused(ctx.CorrelationId, command.WorkOrderId, deniedEntry.ToString(), refusal);
            }
            _trace.Security($"ApproveWorkOrder granted to {ctx.User}");

            WorkOrder current = _store.Find(command.WorkOrderId);
            if (current == null)
            {
                _trace.Service($"refused — work order #{command.WorkOrderId} no longer exists (nothing written)");
                return ApproveResult.Refused(ctx.CorrelationId, command.WorkOrderId, null, $"work order #{command.WorkOrderId} no longer exists");
            }
            if (current.TenantId != ctx.TenantId)
            {
                var crossEntry = _audit.Write(ctx.TenantId, ctx.UserName, "ApproveWorkOrder", $"#{command.WorkOrderId}", false, "cross-tenant request", ctx.CorrelationId);
                _trace.Security($"cross-tenant approve blocked: #{command.WorkOrderId} belongs to {current.TenantId}");
                return ApproveResult.Refused(ctx.CorrelationId, command.WorkOrderId, crossEntry.ToString(), "That work order belongs to another tenant.");
            }
            if (!current.CanBeApproved)
            {
                _trace.Service($"refused as data — #{command.WorkOrderId} is {current.Status}; only InProgress or Escalated can be approved (nothing written, no exception)");
                return ApproveResult.Refused(ctx.CorrelationId, command.WorkOrderId, null,
                    $"#{command.WorkOrderId} is {current.Status} — only work that is InProgress or Escalated can be approved.");
            }

            await Task.Delay(80).ConfigureAwait(true);

            WorkOrderStatus originalStatus = current.Status;
            current.Status = WorkOrderStatus.Completed;
            current.CompletedUtc = DateTime.UtcNow;

            WorkOrder saved;
            try
            {
                saved = _store.Update(current, command.ExpectedVersion);
            }
            catch (ConcurrencyException ex)
            {
                var staleEntry = _audit.Write(ctx.TenantId, ctx.UserName, "ApproveWorkOrder", $"#{command.WorkOrderId}", false, ex.Message, ctx.CorrelationId);
                _trace.Data($"update rejected — {ex.Message}");
                _trace.Data($"audit ← {staleEntry}");
                return ApproveResult.Refused(ctx.CorrelationId, command.WorkOrderId, staleEntry.ToString(),
                    $"#{command.WorkOrderId} changed while you were looking at it (you saw v{ex.ExpectedVersion}, it is now v{ex.CurrentVersion}). Reload the queue and try again.");
            }

            var entry = _audit.Write(ctx.TenantId, ctx.UserName, "ApproveWorkOrder", $"#{saved.Id}", true,
                $"status {originalStatus}→{saved.Status}, v{command.ExpectedVersion}→v{saved.Version}{(string.IsNullOrEmpty(command.Note) ? "" : ", note: " + command.Note)}",
                ctx.CorrelationId);
            _trace.Data($"update work_order #{saved.Id} → v{saved.Version}");
            _trace.Data($"audit ← {entry}");

            return ApproveResult.Approved(ctx.CorrelationId, saved.Id, saved.Version, entry.ToString(), saved.CompletedUtc.Value);
        }

        /// <summary>
        /// The "somebody else saved it first" failure path: bump the row's version behind the screen's back,
        /// so the next approve carries a stale token. In production this is a second user, not a button.
        /// </summary>
        public void SimulateConcurrentEdit(int workOrderId)
        {
            _store.BumpVersionBehindTheScenes(workOrderId);
            _trace.Data($"another session saved work order #{workOrderId} — its version moved on");
        }
    }
}
