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
    /// The numbers on the Command Center cards. Every rule that turns rows into a KPI — what counts as
    /// "open", when something is "overdue" — lives here, so the definition can be reviewed without opening
    /// the Designer (instructor acceptance criterion 3) and unit-tested without a browser (checklist Q6).
    /// </summary>
    public sealed class DashboardService
    {
        private readonly InMemoryWorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly ActivityTrace _trace;

        public DashboardService(InMemoryWorkOrderStore store, PermissionService permissions, ActivityTrace trace)
        {
            _store = store;
            _permissions = permissions;
            _trace = trace;
        }

        /// <summary>Statuses that count as "open work" — one definition, used by the KPI and by the queue filter.</summary>
        public static readonly WorkOrderStatus[] OpenStatuses =
        {
            WorkOrderStatus.New, WorkOrderStatus.Assigned, WorkOrderStatus.InProgress,
            WorkOrderStatus.OnHold, WorkOrderStatus.Escalated,
        };

        public static bool IsOpen(WorkOrder w) => Array.IndexOf(OpenStatuses, w.Status) >= 0;

        /// <summary>
        /// Computes the dashboard KPIs for the caller's tenant. Returns null (with the reason in
        /// <paramref name="denied"/>) when the caller may not look — the screen shows the reason, the
        /// service made the decision.
        /// </summary>
        public async Task<DashboardKpis> GetKpisAsync(CommandContext ctx, Action<string> denied)
        {
            string refusal = _permissions.Check(ctx.User, Permission.ViewWorkQueue);
            if (refusal != null)
            {
                _trace.Security($"ViewWorkQueue denied — {refusal}");
                denied?.Invoke(refusal);
                return null;
            }

            await Task.Delay(60).ConfigureAwait(true);        // stands in for the query round trip
            List<WorkOrder> rows = _store.Query(ctx.TenantId).ToList();
            _trace.Data($"select work_order where tenant_id = '{ctx.TenantId}' → {rows.Count} rows");

            DateTime today = DateTime.UtcNow.Date;
            var kpis = new DashboardKpis
            {
                TenantId = ctx.TenantId,
                Total = rows.Count,
                Open = rows.Count(IsOpen),
                Escalated = rows.Count(w => w.Status == WorkOrderStatus.Escalated),
                DueToday = rows.Count(w => IsOpen(w) && w.DueUtc.HasValue && w.DueUtc.Value.Date == today),
                Overdue = rows.Count(w => IsOpen(w) && w.DueUtc.HasValue && w.DueUtc.Value.Date < today),
                CompletedLast7Days = rows.Count(w => w.CompletedUtc.HasValue && w.CompletedUtc.Value >= today.AddDays(-7)),
                ComputedUtc = DateTime.UtcNow,
            };

            _trace.Service($"KPIs for {ctx.TenantId}: open {kpis.Open}, escalated {kpis.Escalated}, due today {kpis.DueToday}, overdue {kpis.Overdue}");
            return kpis;
        }
    }
}
