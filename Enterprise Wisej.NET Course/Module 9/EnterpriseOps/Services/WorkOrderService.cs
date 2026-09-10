using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>The projection the UI and the palette see. The entity itself never crosses a boundary.</summary>
    public sealed class WorkQueueRow
    {
        public string EntityId { get; set; }      // wire form: WO-1040
        public string Title { get; set; }
        public string Customer { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public int Version { get; set; }

        public override string ToString()
            => $"{EntityId}  {Status,-10} {Priority,-8} {Title}";
    }

    /// <summary>
    /// The business rules of the module's commands. Everything here is reachable from a button
    /// AND from the palette; both go through the same methods, so there is no "JavaScript path"
    /// with weaker rules. State transitions, the version bump and the tenant scope live here —
    /// which is exactly what would be bypassed if a script were allowed to set Status directly.
    /// </summary>
    public sealed class WorkOrderService
    {
        private readonly IWorkOrderRepository _repository;
        private readonly ActivityTrace _trace;
        private int _importBatches;

        public WorkOrderService(IWorkOrderRepository repository, ActivityTrace trace)
        {
            _repository = repository;
            _trace = trace;
        }

        public IReadOnlyList<WorkQueueRow> Queue(string tenantId, int take = 12)
        {
            IReadOnlyList<WorkOrder> rows = _repository.Query(tenantId);
            _trace.Data($"IWorkOrderRepository.Query(tenant={tenantId}) → {rows.Count} rows");
            return rows.Take(take).Select(Project).ToList();
        }

        /// <summary>Resolves a wire entity id inside the SESSION tenant. A foreign id resolves to null.</summary>
        public WorkOrder Resolve(string tenantId, int key)
        {
            WorkOrder order = _repository.Find(tenantId, key);
            _trace.Data($"IWorkOrderRepository.Find(tenant={tenantId}, id={key}) → {(order == null ? "not found" : order.Status.ToString())}");
            return order;
        }

        /// <summary>Approve: New/Assigned/OnHold → InProgress. Anything else is INVALID_STATE.</summary>
        public CommandResult Approve(CommandContext context, WorkOrder order)
        {
            if (order.Status != WorkOrderStatus.New && order.Status != WorkOrderStatus.Assigned && order.Status != WorkOrderStatus.OnHold)
            {
                _trace.Service($"WorkOrderService.Approve rejected: {order.Status} is not approvable");
                return CommandResult.Fail(context.CorrelationId, ResultCodes.InvalidState,
                    $"Work order is {order.Status}; only New, Assigned or OnHold can be approved.");
            }

            order.Status = WorkOrderStatus.InProgress;
            if (string.IsNullOrEmpty(order.AssignedTo)) order.AssignedTo = "ben.tech";
            _repository.Save(order);
            _trace.Service($"WorkOrderService.Approve → InProgress, version {order.Version}");
            return CommandResult.Ok(context.CorrelationId, $"Approved — now InProgress (v{order.Version}).");
        }

        /// <summary>Reassign: any non-terminal work order moves to the next technician in the rota.</summary>
        public CommandResult Reassign(CommandContext context, WorkOrder order)
        {
            if (order.IsTerminal)
            {
                _trace.Service($"WorkOrderService.Reassign rejected: {order.Status} is terminal");
                return CommandResult.Fail(context.CorrelationId, ResultCodes.InvalidState,
                    $"Work order is {order.Status}; a closed work order cannot be reassigned.");
            }

            string[] rota = { "ben.tech", "dana.tech", "eli.tech", "faye.tech" };
            int index = Math.Max(0, Array.IndexOf(rota, order.AssignedTo));
            order.AssignedTo = rota[(index + 1) % rota.Length];
            order.Status = WorkOrderStatus.Assigned;
            _repository.Save(order);
            _trace.Service($"WorkOrderService.Reassign → {order.AssignedTo}, version {order.Version}");
            return CommandResult.Ok(context.CorrelationId, $"Reassigned to {order.AssignedTo} (v{order.Version}).");
        }

        /// <summary>Escalate: the one state change a Technician is allowed to make.</summary>
        public CommandResult Escalate(CommandContext context, WorkOrder order)
        {
            if (order.IsTerminal || order.Status == WorkOrderStatus.Escalated)
            {
                _trace.Service($"WorkOrderService.Escalate rejected: already {order.Status}");
                return CommandResult.Fail(context.CorrelationId, ResultCodes.InvalidState,
                    $"Work order is {order.Status}; it cannot be escalated again.");
            }

            order.Status = WorkOrderStatus.Escalated;
            _repository.Save(order);
            _trace.Service($"WorkOrderService.Escalate → Escalated, version {order.Version}");
            return CommandResult.Ok(context.CorrelationId, $"Escalated (v{order.Version}).");
        }

        /// <summary>Navigation-style command: no entity, no state change, still permission-checked.</summary>
        public CommandResult OpenWorkQueue(CommandContext context)
        {
            IReadOnlyList<WorkQueueRow> rows = Queue(context.TenantId);
            _trace.Service($"WorkOrderService.OpenWorkQueue → {rows.Count} rows for {context.TenantId}");
            return CommandResult.Ok(context.CorrelationId, $"Work queue open — {rows.Count} rows for {context.TenantId}.");
        }

        public CommandResult StartImport(CommandContext context)
        {
            _importBatches++;
            _trace.Service($"WorkOrderService.StartImport → batch #{_importBatches} created (staged, not committed)");
            return CommandResult.Ok(context.CorrelationId, $"Import batch #{_importBatches} staged for {context.TenantId}.");
        }

        public CommandResult OpenDiagnostics(CommandContext context)
        {
            _trace.Service("WorkOrderService.OpenDiagnostics → diagnostics snapshot requested");
            return CommandResult.Ok(context.CorrelationId, "Diagnostics snapshot opened.");
        }

        /// <summary>
        /// Recovery path: put an entity back the way the audit log says it was. Used to undo the
        /// state change the anti-pattern let a forged payload make.
        /// </summary>
        public CommandResult RestoreSnapshot(CommandContext context, WorkOrder before)
        {
            WorkOrder current = _repository.Find(before.TenantId, before.Id);
            if (current == null)
                return CommandResult.Fail(context.CorrelationId, ResultCodes.InvalidTarget, "The work order is no longer in the store.");

            current.Status = before.Status;
            current.AssignedTo = before.AssignedTo;
            current.Priority = before.Priority;
            _repository.Save(current);
            _trace.Service($"WorkOrderService.RestoreSnapshot → {Interop.InteropContract.ToEntityId(current.Id)} back to {current.Status} (v{current.Version})");
            return CommandResult.Ok(context.CorrelationId, $"Reverted to {current.Status} (v{current.Version}).");
        }

        public static WorkQueueRow Project(WorkOrder order) => new WorkQueueRow
        {
            EntityId = Interop.InteropContract.ToEntityId(order.Id),
            Title = order.Title,
            Customer = order.Customer,
            Status = order.Status.ToString(),
            Priority = order.Priority.ToString(),
            AssignedTo = string.IsNullOrEmpty(order.AssignedTo) ? "—" : order.AssignedTo,
            Version = order.Version,
        };
    }
}
