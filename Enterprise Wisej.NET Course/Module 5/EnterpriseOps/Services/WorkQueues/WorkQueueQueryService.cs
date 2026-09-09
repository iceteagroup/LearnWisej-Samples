using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EnterpriseOps.Data;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>
    /// The paged query service (deliverable 1). Translates a <see cref="WorkQueueQuery"/> into a query over the
    /// store — filter, sort, skip/take — and projects only the requested page into <see cref="WorkQueueRow"/>.
    /// Against a real database the same shape becomes <c>Where(...).OrderBy(...).Skip(...).Take(...).Select(...)</c>
    /// and the database does the filtering; here LINQ-to-objects over the in-memory table stands in for it.
    ///
    /// Answer to review question 1: the first screen loads exactly <c>PageSize</c> rows (50), never the table.
    /// </summary>
    public sealed class WorkQueueQueryService : IWorkQueueQueryService
    {
        public const int MaxPageSize = 200;

        private readonly WorkOrderStore _store;
        private readonly PermissionService _permissions;
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;

        /// <summary>Server-side time of the last search (the pager shows the UI-measured time next to it).</summary>
        public long LastElapsedMs { get; private set; }

        /// <summary>Approximate JSON size of the last page — what actually travels to the browser.</summary>
        public int LastPayloadBytes { get; private set; }

        public WorkQueueQueryService(WorkOrderStore store, PermissionService permissions, SessionContext session)
        {
            _store = store;
            _permissions = permissions;
            _session = session;
            _trace = session.Trace;
        }

        public async Task<PagedResult<WorkQueueRow>> SearchAsync(WorkQueueQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            // Security: a query is always scoped to the caller's tenant, whatever the UI sent.
            if (!string.Equals(query.TenantId, _session.TenantId, StringComparison.Ordinal))
            {
                _trace.Write($"Security: query for tenant '{query.TenantId}' rejected — session tenant is '{_session.TenantId}'");
                throw new UnauthorizedAccessException("The query targets another tenant.");
            }

            query = Normalize(query);
            _trace.Write($"Service: SearchAsync page {query.Page} size {query.PageSize} · status {query.Status ?? "any"} · " +
                         $"assigned {query.AssignedTo ?? "any"} · search \"{query.SearchText ?? ""}\" · sort {query.SortBy} {(query.Descending ? "desc" : "asc")}");

            var sw = Stopwatch.StartNew();

            // A real repository would run this on the database; the shape is the same.
            IEnumerable<WorkOrder> rows = _store.Snapshot().Where(o => o.TenantId == query.TenantId);
            rows = ApplyFilters(rows, query);
            var filtered = rows.ToList();
            int total = filtered.Count;

            var page = ApplySort(filtered, query)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(Project)
                .ToList();

            sw.Stop();
            LastElapsedMs = sw.ElapsedMilliseconds;
            LastPayloadBytes = JsonSerializer.SerializeToUtf8Bytes(page).Length;

            _trace.Write($"Data: {page.Count} of {total:N0} matching rows in {LastElapsedMs} ms — " +
                         $"skip {(query.Page - 1) * query.PageSize:N0}, take {query.PageSize}; page payload ≈ {LastPayloadBytes / 1024.0:0.0} KB");

            // Simulates the round trip to a database that is not on this machine (keeps the number honest).
            await Task.Delay(15).ConfigureAwait(false);

            return new PagedResult<WorkQueueRow>(page, total, query.Page, query.PageSize);
        }

        /// <summary>Clamps page and page size so a bad request cannot ask for the table.</summary>
        private static WorkQueueQuery Normalize(WorkQueueQuery q)
        {
            int pageSize = Math.Clamp(q.PageSize <= 0 ? GridState.DefaultPageSize : q.PageSize, 1, MaxPageSize);
            int page = Math.Max(1, q.Page);
            string sortBy = string.IsNullOrWhiteSpace(q.SortBy) ? "Priority" : q.SortBy;
            string status = string.IsNullOrWhiteSpace(q.Status) ? null : q.Status.Trim();
            string assigned = string.IsNullOrWhiteSpace(q.AssignedTo) ? null : q.AssignedTo.Trim();
            string search = string.IsNullOrWhiteSpace(q.SearchText) ? null : q.SearchText.Trim();
            return q with { Page = page, PageSize = pageSize, SortBy = sortBy, Status = status, AssignedTo = assigned, SearchText = search };
        }

        private static IEnumerable<WorkOrder> ApplyFilters(IEnumerable<WorkOrder> rows, WorkQueueQuery q)
        {
            if (q.Status != null)
            {
                if (q.Status.Equals("Open", StringComparison.OrdinalIgnoreCase))
                    rows = rows.Where(o => o.IsOpen);
                else if (Enum.TryParse<WorkOrderStatus>(q.Status, true, out var status))
                    rows = rows.Where(o => o.Status == status);
            }

            if (q.AssignedTo != null)
                rows = rows.Where(o => string.Equals(o.AssignedTo, q.AssignedTo, StringComparison.OrdinalIgnoreCase));

            if (q.SearchText != null)
            {
                string text = q.SearchText;
                rows = rows.Where(o =>
                    o.Number.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    o.Title.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    o.Customer.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    o.Site.Contains(text, StringComparison.OrdinalIgnoreCase));
            }

            return rows;
        }

        private static IEnumerable<WorkOrder> ApplySort(IEnumerable<WorkOrder> rows, WorkQueueQuery q)
        {
            IOrderedEnumerable<WorkOrder> ordered;
            switch (q.SortBy)
            {
                case "Number":     ordered = Order(rows, o => o.Id, q.Descending); break;
                case "Title":      ordered = Order(rows, o => o.Title, q.Descending); break;
                case "Status":     ordered = Order(rows, o => o.Status, q.Descending); break;
                case "AssignedTo": ordered = Order(rows, o => o.AssignedTo, q.Descending); break;
                case "DueAt":      ordered = Order(rows, o => o.DueUtc ?? DateTime.MaxValue, q.Descending); break;
                case "AgeDays":    ordered = Order(rows, o => o.CreatedUtc, !q.Descending); break;
                default:           ordered = Order(rows, o => o.Priority, q.Descending); break;
            }
            // A deterministic tie-breaker: the same query must always give the same page.
            return ordered.ThenBy(o => o.Id);
        }

        private static IOrderedEnumerable<WorkOrder> Order<TKey>(IEnumerable<WorkOrder> rows, Func<WorkOrder, TKey> key, bool descending) =>
            descending ? rows.OrderByDescending(key) : rows.OrderBy(key);

        /// <summary>
        /// Entity → projection. Display values and permission flags are computed here, once, on the server;
        /// the grid never computes anything per row.
        /// </summary>
        public WorkQueueRow Project(WorkOrder o)
        {
            var now = DateTime.UtcNow;
            var due = o.DueUtc ?? o.CreatedUtc.AddDays(30);
            return new WorkQueueRow(
                Id: o.Id,
                Number: o.Number,
                Title: o.Title,
                StatusText: StatusLabel(o.Status),
                PriorityText: o.Priority.ToString(),
                AssignedTo: string.IsNullOrEmpty(o.AssignedTo) ? "—" : o.AssignedTo,
                DueAt: new DateTimeOffset(due, TimeSpan.Zero),
                CanApprove: _permissions.CanApprove(_session.Role, o),
                CanReassign: _permissions.CanReassign(_session.Role, o),
                DueText: due.ToString("MMM d", CultureInfo.InvariantCulture),
                AgeDays: Math.Max(0, (int)(now - o.CreatedUtc).TotalDays),
                IsOverdue: o.IsOpen && due < now,
                Version: o.Version);
        }

        private static string StatusLabel(WorkOrderStatus status)
        {
            switch (status)
            {
                case WorkOrderStatus.InProgress: return "In progress";
                case WorkOrderStatus.OnHold: return "On hold";
                default: return status.ToString();
            }
        }
    }
}
