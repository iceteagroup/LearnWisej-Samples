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
    /// <summary>The queue the screen shows, plus the count for the status bar.</summary>
    public sealed class WorkQueueResult : CommandResult
    {
        private WorkQueueResult(bool succeeded, bool denied, string summary, string correlationId,
            IReadOnlyList<string> errors, IReadOnlyList<WorkQueueRow> rows)
            : base(succeeded, denied, summary, correlationId, errors)
        {
            Rows = rows ?? new List<WorkQueueRow>();
        }

        public IReadOnlyList<WorkQueueRow> Rows { get; }

        public static WorkQueueResult Loaded(IReadOnlyList<WorkQueueRow> rows, string correlationId)
            => new WorkQueueResult(true, false, $"{rows.Count} work orders", correlationId, null, rows);

        public static new WorkQueueResult Refused(string reason, string correlationId)
            => new WorkQueueResult(false, true, "denied", correlationId, new List<string> { reason }, null);
    }

    /// <summary>
    /// The work order service. Every public method starts with a <c>Demand</c> and ends with the record it
    /// changed — that is the whole contract, and it is the answer to "where is authorization enforced?".
    ///
    /// Note what the methods do **not** take: no "is the button visible" flag, no role name from the screen, no
    /// tenant from a combo box. They take a <see cref="CommandContext"/> built from verified claims, and an id.
    /// A tampered client can send any id it likes; it cannot send a permission.
    /// </summary>
    public sealed class WorkOrderService
    {
        private readonly IWorkOrderRepository _repository;
        private readonly IPermissionService _permissions;
        private readonly IAuditLog _audit;
        private readonly ActivityTrace _trace;

        public WorkOrderService(IWorkOrderRepository repository, IPermissionService permissions, IAuditLog audit, ActivityTrace trace)
        {
            _repository = repository;
            _permissions = permissions;
            _audit = audit;
            _trace = trace;
        }

        /// <summary>Reading is a permission too. The rows are the caller's tenant only — the guard is not optional.</summary>
        public async Task<WorkQueueResult> LoadQueueAsync(CommandContext context)
        {
            _trace?.Service("WorkOrderService.LoadQueueAsync — Demand(ViewWorkOrders) before a single row is read");

            try
            {
                _permissions.Demand(context, Permission.ViewWorkOrders);
            }
            catch (UnauthorizedAccessException ex)
            {
                return WorkQueueResult.Refused(ex.Message, context.CorrelationId);
            }

            await Task.Delay(80);   // stands in for the query

            var rows = _repository.ForTenant(context.TenantId)
                .OrderByDescending(w => w.Priority)
                .ThenBy(w => w.Id)
                .Take(12)
                .Select(Project)
                .ToList();

            return WorkQueueResult.Loaded(rows, context.CorrelationId);
        }

        /// <summary>
        /// Approve a work order. The tenant of the **record** is passed to Demand, so the tenant guard runs before
        /// the role store is consulted: a manager of fabrikam gets nothing on a contoso row.
        /// </summary>
        public async Task<CommandResult> ApproveAsync(CommandContext context, int workOrderId)
        {
            WorkOrder order = _repository.Find(workOrderId);
            if (order == null)
                return CommandResult.Failed($"WO-{workOrderId:0000} no longer exists.", context.CorrelationId);

            _trace?.Service($"WorkOrderService.ApproveAsync(WO-{order.Id:0000}) — Demand(ApproveWorkOrders, resourceTenant '{order.TenantId}')");

            try
            {
                _permissions.Demand(context, Permission.ApproveWorkOrders, order.TenantId);
            }
            catch (CrossTenantAccessException)
            {
                return CommandResult.Refused($"WO-{order.Id:0000} belongs to another tenant.", context.CorrelationId);
            }
            catch (UnauthorizedAccessException ex)
            {
                return CommandResult.Refused(ex.Message, context.CorrelationId);
            }

            await Task.Delay(120);

            order.Status = WorkOrderStatus.Completed;
            order.ApprovedBy = context.UserId;
            _repository.Save(order);

            // The audit entry for the *command* is written next to the change, in the same operation. The Demand
            // above already recorded the permission decision; this row records what the command did with it.
            _audit.Write(context, "ApproveWorkOrder", AuditResult.Ok, $"WO-{order.Id:0000}",
                $"status Escalated → Completed, version {order.Version}");

            return CommandResult.Ok($"WO-{order.Id:0000} approved", context.CorrelationId);
        }

        public int CountForTenant(string tenantId) => _repository.ForTenant(tenantId).Count;

        private static WorkQueueRow Project(WorkOrder w) => new WorkQueueRow
        {
            Id = w.Id,
            Reference = $"WO-{w.Id:0000}",
            Title = w.Title,
            Status = w.Status.ToString(),
            Priority = w.Priority.ToString(),
            AssignedTo = w.AssignedTo,
            TenantId = w.TenantId,
            ApprovedBy = string.IsNullOrEmpty(w.ApprovedBy) ? "—" : w.ApprovedBy,
        };
    }
}
