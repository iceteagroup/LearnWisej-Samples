using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>
    /// The application service behind the work queue. Every operation runs inside an OperationTimer that
    /// writes one structured log entry when it completes — operation, tenant, user, page, pageSize, elapsed,
    /// budget verdict and the correlation id as separate fields — so the log can later be filtered to
    /// "every SearchWorkOrders over 400 ms in tenant fabrikam".
    /// </summary>
    public sealed class WorkOrderService
    {
        public const string SearchOperation = "SearchWorkOrders";

        private readonly InMemoryWorkOrderStore _store;
        private readonly StructuredLog _log;
        private readonly PerformanceBudget _budget;
        private readonly Action<string> _trace;

        public WorkOrderService(InMemoryWorkOrderStore store, StructuredLog log, PerformanceBudget budget, Action<string> trace)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _budget = budget ?? throw new ArgumentNullException(nameof(budget));
            _trace = trace ?? throw new ArgumentNullException(nameof(trace));
        }

        public async Task<SearchResult> SearchWorkOrdersAsync(WorkQueueQuery query, CommandContext ctx, CancellationToken ct)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            _trace($"Service: {SearchOperation} tenant={ctx.TenantId} user={ctx.UserName} page={query.Page} pageSize={query.PageSize:N0} · correlation {ctx.CorrelationId}");

            PagedResult<WorkOrder> page = null;
            BudgetRow verdict = null;
            bool failed = false;

            // Timing as a using block: when the block ends — normally or by exception — the timer logs itself
            // with the operation name and the correlation id. RecordTiming adds the query fields.
            using (new OperationTimer(SearchOperation, ctx.CorrelationId,
                       (operation, elapsed, correlationId) => verdict = RecordTiming(operation, elapsed, correlationId, query, ctx, failed)))
            {
                try
                {
                    page = await _store.SearchAsync(query, ctx, ct);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // The service does not swallow the failure: the handler owns the user-facing message.
                    failed = true;
                    _trace($"Service: {SearchOperation} threw {ex.GetType().Name} · correlation {ctx.CorrelationId} → rethrown to the handler, which logs it");
                    throw;
                }
            }

            // Projection: the UI never sees the entity.
            List<WorkQueueRow> rows = page.Rows.Select(ToRow).ToList();

            return new SearchResult
            {
                Page = new PagedResult<WorkQueueRow> { Rows = rows, Total = page.Total, Page = page.Page, PageSize = page.PageSize },
                ElapsedMs = verdict?.MeasuredMs ?? 0,
                CorrelationId = ctx.CorrelationId,
                Budget = verdict,
            };
        }

        /// <summary>
        /// The OperationTimer callback. The Diagnostics layer decides OK / OVER against the budget table; the
        /// structured log gets the fields; the trace shows the decision. A failed call is logged as such and
        /// does not touch the budget row (a 3 ms exception is not a fast query).
        /// </summary>
        private BudgetRow RecordTiming(string operation, TimeSpan elapsed, string correlationId, WorkQueueQuery query, CommandContext ctx, bool failed)
        {
            long ms = (long)Math.Round(elapsed.TotalMilliseconds);

            if (failed)
            {
                _log.Write(LogLevel.Warning, operation, correlationId, new
                {
                    elapsedMs = ms,
                    tenant = ctx.TenantId,
                    user = ctx.UserName,
                    page = query.Page,
                    pageSize = query.PageSize,
                    outcome = "failed",
                });
                _trace($"Diagnostics: OperationTimer {operation} {ms:N0} ms · correlation {correlationId} · outcome failed (budget row untouched)");
                return _budget.Find(PerformanceBudget.QuerySearchWorkOrders);
            }

            BudgetRow row = _budget.Record(PerformanceBudget.QuerySearchWorkOrders, ms, correlationId);

            _log.Write(row.IsOver ? LogLevel.Warning : LogLevel.Information, operation, correlationId, new
            {
                elapsedMs = ms,
                tenant = ctx.TenantId,
                user = ctx.UserName,
                page = query.Page,
                pageSize = query.PageSize,
                budgetMs = row.BudgetMs,
                budget = row.IsOver ? "over" : "ok",
            });

            _trace($"Diagnostics: OperationTimer {operation} {ms:N0} ms · correlation {correlationId} · budget ≤ {row.BudgetMs} ms → {row.StatusText}");
            return row;
        }

        private static WorkQueueRow ToRow(WorkOrder w) => new WorkQueueRow
        {
            Id = w.Id,
            Title = w.Title,
            Site = w.Site,
            Status = w.Status,
            Priority = w.Priority,
            AssignedTo = w.AssignedTo,
            DueUtc = w.DueUtc,
        };
    }
}
