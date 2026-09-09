using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The application service behind the WorkOrderHistoryPage. Everything the two components show
    /// comes from here as typed results: the timeline entries of one work order, the status breakdown
    /// of the tenant, and the work orders behind one chart segment. Tenant isolation is checked here
    /// (through <see cref="AccessPolicy"/>) — the components never decide who may see what.
    /// </summary>
    public class WorkOrderHistoryService
    {
        private readonly FakeWorkOrderStore _store;
        private readonly AccessPolicy _policy;
        private readonly IActivityTrace _trace;

        public WorkOrderHistoryService(FakeWorkOrderStore store, AccessPolicy policy, IActivityTrace trace)
        {
            _store = store;
            _policy = policy;
            _trace = trace;
        }

        /// <summary>The status history of one work order, oldest first.</summary>
        public async Task<CommandResult<WorkOrderHistoryView>> GetHistoryAsync(SessionContext context, int workOrderId)
        {
            string correlationId = context.NextCorrelation();
            _trace.Write($"Service: GetHistoryAsync(workOrder {workOrderId}) [{correlationId}]");

            await Task.Delay(60);                       // a repository round trip would sit here

            var workOrder = _store.Find(workOrderId);
            if (!_policy.CanView(context, workOrder, out string reason))
                return CommandResult<WorkOrderHistoryView>.Fail(correlationId, reason);

            var entries = _store.HistoryOf(workOrderId)
                .Select(h => new HistoryEntryView
                {
                    AtUtc = h.AtUtc,
                    Status = h.Status,
                    StatusLabel = StatusGroup.StatusLabel(h.Status),
                    Note = h.Note,
                })
                .ToArray();

            _trace.Write($"Data: {entries.Length} history entries for work order {workOrderId} (actor column not projected)");

            return CommandResult<WorkOrderHistoryView>.Ok(correlationId, new WorkOrderHistoryView
            {
                WorkOrderId = workOrder.Id,
                Title = workOrder.Title,
                Status = workOrder.Status,
                Entries = entries,
            });
        }

        /// <summary>How the tenant's work orders split across the four reporting groups (the chart's data).</summary>
        public async Task<CommandResult<IReadOnlyList<StatusCountView>>> GetStatusBreakdownAsync(SessionContext context)
        {
            string correlationId = context.NextCorrelation();
            _trace.Write($"Service: GetStatusBreakdownAsync(tenant '{context.TenantId}') [{correlationId}]");

            await Task.Delay(60);

            var counts = _store.ForTenant(context.TenantId)
                .GroupBy(w => StatusGroup.KeyFor(w.Status))
                .ToDictionary(g => g.Key, g => g.Count());

            var result = StatusGroup.All
                .Select(key => new StatusCountView
                {
                    Key = key,
                    Label = StatusGroup.LabelFor(key),
                    Count = counts.TryGetValue(key, out int n) ? n : 0,
                })
                .ToArray();

            _trace.Write($"Data: {string.Join(" · ", result.Select(r => $"{r.Label} {r.Count}"))} (tenant '{context.TenantId}' only)");
            return CommandResult<IReadOnlyList<StatusCountView>>.Ok(correlationId, result);
        }

        /// <summary>The work orders behind one chart segment, as grid rows. The key is validated: the browser chose it.</summary>
        public async Task<CommandResult<PagedResult<WorkQueueRow>>> GetWorkOrdersByGroupAsync(SessionContext context, string groupKey, int page = 1, int pageSize = 50)
        {
            string correlationId = context.NextCorrelation();
            _trace.Write($"Service: GetWorkOrdersByGroupAsync(group '{groupKey}', page {page}) [{correlationId}]");

            if (!StatusGroup.IsKnown(groupKey))
            {
                _trace.Write($"Service: rejected — '{groupKey}' is not a reporting group (keys come from the browser; never trusted)");
                return CommandResult<PagedResult<WorkQueueRow>>.Fail(correlationId, $"unknown status group '{groupKey}'");
            }

            await Task.Delay(60);

            var matching = _store.ForTenant(context.TenantId)
                .Where(w => StatusGroup.KeyFor(w.Status) == groupKey)
                .OrderBy(w => w.DueUtc)
                .ToList();

            var rows = matching
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new WorkQueueRow
                {
                    Id = w.Id,
                    Title = w.Title,
                    Customer = w.Customer,
                    Status = StatusGroup.StatusLabel(w.Status),
                    Priority = w.Priority.ToString(),
                    AssignedTo = string.IsNullOrEmpty(w.AssignedTo) ? "—" : w.AssignedTo,
                    Due = w.DueUtc?.ToString("yyyy-MM-dd") ?? "",
                })
                .ToArray();

            _trace.Write($"Data: {matching.Count} work orders in '{groupKey}' for tenant '{context.TenantId}' → {rows.Length} rows projected (WorkQueueRow, no entity)");
            return CommandResult<PagedResult<WorkQueueRow>>.Ok(correlationId, new PagedResult<WorkQueueRow>(rows, matching.Count, page, pageSize));
        }

        /// <summary>The work orders the picker offers (tenant's first few, plus one foreign one to show the denied path).</summary>
        public IReadOnlyList<WorkOrder> PickerWorkOrders(SessionContext context)
        {
            var own = _store.ForTenant(context.TenantId).Take(8).ToList();
            var foreign = _store.ForTenant("contoso").Skip(6).FirstOrDefault();   // 2107
            if (foreign != null)
                own.Add(foreign);
            return own;
        }
    }
}
