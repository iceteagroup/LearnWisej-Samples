using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Integrations;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>
    /// The workflow service behind CommandCenterDashboard — "EnterpriseOps.Services.Workflow" in the walkthrough.
    ///
    /// Everything the screen must NOT decide lives here, in this order:
    ///   1. Security   — DashboardPolicy: may this user see the Command Center? (deny = stop, nothing else runs)
    ///   2. Integration— OperationsFeed: pull the changes that happened in the field since the last refresh
    ///   3. Data       — IWorkOrderRepository: apply the changes, then query the tenant's incidents
    ///   4. Service    — compute the KPIs and project entities into IncidentRow for the grid
    ///
    /// A new senior developer finds this class in under a minute: Services/Workflow/&lt;Screen&gt;Workflow.cs.
    /// </summary>
    public sealed class DashboardWorkflow
    {
        /// <summary>An open work order whose due time is closer than this counts as "SLA at risk".</summary>
        public static readonly TimeSpan SlaWarningWindow = TimeSpan.FromHours(4);

        private readonly IWorkOrderRepository _repository;
        private readonly OperationsFeed _feed;
        private readonly ReleaseCalendar _releases;
        private readonly DashboardPolicy _policy;
        private readonly ActivityTrace _trace;

        public DashboardWorkflow(IWorkOrderRepository repository, OperationsFeed feed, ReleaseCalendar releases,
            DashboardPolicy policy, ActivityTrace trace)
        {
            _repository = repository;
            _feed = feed;
            _releases = releases;
            _policy = policy;
            _trace = trace;
        }

        /// <summary>First load of the screen: authorize, then read. No feed pull — the screen shows what the store holds.</summary>
        public Task<DashboardResult> LoadAsync(CommandContext ctx) => RunAsync(ctx, pullFeed: false);

        /// <summary>The Refresh button: authorize, pull field changes, apply them, read, compute.</summary>
        public Task<DashboardResult> RefreshAsync(CommandContext ctx) => RunAsync(ctx, pullFeed: true);

        private async Task<DashboardResult> RunAsync(CommandContext ctx, bool pullFeed)
        {
            var clock = Stopwatch.StartNew();
            _trace.Service($"DashboardWorkflow.{(pullFeed ? "RefreshAsync" : "LoadAsync")}({ctx})");

            // 1. Security — decided here, before any integration or data access.
            PolicyDecision decision = _policy.CanViewCommandCenter(ctx);
            if (!decision.Allowed)
            {
                _trace.Service("stopped: policy denied — no feed pull, no query (" + clock.ElapsedMilliseconds + " ms)");
                return DashboardResult.Denied(ctx.CorrelationId, decision.Reason, clock.ElapsedMilliseconds);
            }

            // 2. Integration — what changed in the field since the last refresh. May throw when the feed is down:
            //    that is an unexpected failure, so it propagates to the handler's catch (logged, generic message).
            var changed = new List<int>();
            string summary;
            if (pullFeed)
            {
                IReadOnlyList<FeedChange> changes = await _feed.PullChangesAsync(ctx.TenantId);

                // 3a. Data — apply each change through the entity (Version + UpdatedUtc move together).
                foreach (FeedChange change in changes)
                {
                    WorkOrder order = await _repository.FindAsync(ctx.TenantId, change.WorkOrderId);
                    if (order == null)
                    {
                        _trace.Data($"change for #{change.WorkOrderId} ignored — not a {ctx.TenantId} work order");
                        continue;
                    }

                    order.ChangeStatus(change.NewStatus, DateTime.UtcNow);
                    await _repository.SaveAsync(order);
                    changed.Add(order.Id);
                    _trace.Data($"WorkOrder #{order.Id} → {order.Status} (v{order.Version}) — {change.Note}");
                }

                summary = changes.Count == 0
                    ? "Refreshed — no changes"
                    : "Refreshed — " + string.Join(", ", changes.Select(c => $"INC-{c.WorkOrderId} {DisplayState(c.NewStatus)}"));
            }
            else
            {
                summary = null; // filled in below once we know the row count
            }

            // 3b. Data — the tenant's incidents: everything open plus what was resolved today.
            DateTime todayUtc = DateTime.UtcNow.Date;
            IReadOnlyList<WorkOrder> orders = await _repository.QueryAsync(ctx.TenantId,
                o => o.IsOpen || (o.Status == WorkOrderStatus.Completed && o.UpdatedUtc >= todayUtc));

            // 4. Service — the numbers and the rows the screen shows.
            DateTime nowUtc = DateTime.UtcNow;
            var kpis = new DashboardKpis
            {
                OpenIncidents = orders.Count(o => o.IsOpen),
                SlaAtRisk = orders.Count(o => o.IsOpen && o.DueUtc.HasValue && o.DueUtc.Value <= nowUtc + SlaWarningWindow),
                DeploymentsToday = _releases.CountDeploymentsToday(ctx.TenantId),
            };

            List<IncidentRow> rows = orders
                .OrderBy(o => o.Id)
                .Select(o => new IncidentRow
                {
                    Id = $"INC-{o.Id}",
                    Title = o.Title,
                    Priority = DisplayPriority(o.Priority),
                    State = DisplayState(o.Status),
                    ChangedByThisRefresh = changed.Contains(o.Id),
                })
                .ToList();

            summary = summary ?? $"Loaded {rows.Count} incidents";
            clock.Stop();
            _trace.Service($"KPIs OpenIncidents={kpis.OpenIncidents} SlaAtRisk={kpis.SlaAtRisk} DeploymentsToday={kpis.DeploymentsToday}; {rows.Count} rows projected → DashboardResult ({clock.ElapsedMilliseconds} ms)");

            return DashboardResult.Loaded(ctx.CorrelationId, kpis, rows, summary, clock.ElapsedMilliseconds);
        }

        /// <summary>Display words for the grid — the walkthrough's "open / triage / watch / mitigated".</summary>
        public static string DisplayState(WorkOrderStatus status)
        {
            switch (status)
            {
                case WorkOrderStatus.New: return "open";
                case WorkOrderStatus.Assigned: return "triage";
                case WorkOrderStatus.InProgress: return "in progress";
                case WorkOrderStatus.OnHold: return "watch";
                case WorkOrderStatus.Escalated: return "escalated";
                case WorkOrderStatus.Completed: return "mitigated";
                default: return "closed";
            }
        }

        public static string DisplayPriority(Priority priority) =>
            priority == Priority.Normal ? "Medium" : priority.ToString();
    }
}
