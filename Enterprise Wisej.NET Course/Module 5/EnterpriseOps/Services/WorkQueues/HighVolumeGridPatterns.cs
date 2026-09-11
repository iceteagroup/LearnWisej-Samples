// HighVolumeGridPatterns.cs — the work-queue contracts: query, projection, paged result.
// Kept in one file, as in the video, so the query, the projection and the paged result can be read together.
// This file opts into nullable annotations (the project default is off) so the `string?` fields read as shown.
#nullable enable

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>
    /// Everything the server needs to answer one grid request: tenant, search text, filters, sort — and
    /// <b>Page + PageSize</b>. The UI fills it in; the query service does the work. Saved views serialize
    /// exactly this object, which is why the search is repeatable after a refresh.
    /// </summary>
    public sealed record WorkQueueQuery(
        string TenantId,
        string? SearchText,
        string? Status,         // "Open" (anything not Completed/Cancelled), a WorkOrderStatus name, or null = any
        string? AssignedTo,     // a technician user name, or null = any
        string SortBy,          // Number | Title | Status | Priority | AssignedTo | DueAt | AgeDays
        bool Descending,
        int Page,               // 1-based
        int PageSize);

    /// <summary>
    /// The search projection: a row shaped for the grid — display text, computed fields and the permission
    /// flags — not the domain entity. The video's record plus <c>DueText</c>, <c>AgeDays</c>, <c>IsOverdue</c>
    /// and <c>Version</c> (the batch sends the version back so a stale row fails instead of overwriting).
    /// </summary>
    public sealed record WorkQueueRow(
        int Id,
        string Number,
        string Title,
        string StatusText,
        string PriorityText,
        string AssignedTo,
        DateTimeOffset DueAt,
        bool CanApprove,
        bool CanReassign,
        string DueText,
        int AgeDays,
        bool IsOverdue,
        int Version);

    /// <summary>One page of rows plus the total, so the pager can say "Page 1 of 5,213 · 50 of 260,634 matching".</summary>
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int TotalCount,
        int Page,
        int PageSize)
    {
        public int PageCount => TotalCount == 0 ? 1 : (TotalCount + PageSize - 1) / PageSize;
    }

    public interface IWorkQueueQueryService
    {
        Task<PagedResult<WorkQueueRow>> SearchAsync(WorkQueueQuery query);
    }
}
