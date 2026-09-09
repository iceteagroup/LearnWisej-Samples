using System;
using System.Collections.Generic;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Services
{
    /// <summary>What the UI asks for. The page size is the field the failure path abuses (5,000).</summary>
    public sealed class WorkQueueQuery
    {
        public string TenantId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>The projection the UI receives — never the WorkOrder entity.</summary>
    public sealed class WorkQueueRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Site { get; set; }
        public WorkOrderStatus Status { get; set; }
        public Priority Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? DueUtc { get; set; }
    }

    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Rows { get; set; } = Array.Empty<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public sealed class CommandResult
    {
        public bool Succeeded { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
        public string CorrelationId { get; init; }

        public static CommandResult Ok(string correlationId) => new CommandResult { Succeeded = true, CorrelationId = correlationId };
        public static CommandResult Fail(string correlationId, params string[] errors) => new CommandResult { Succeeded = false, CorrelationId = correlationId, Errors = errors };
    }

    /// <summary>
    /// What SearchWorkOrdersAsync returns to the page: the rows, how long it took, the correlation id and
    /// the budget verdict the Diagnostics layer reached — the UI only displays, it never decides.
    /// </summary>
    public sealed class SearchResult
    {
        public PagedResult<WorkQueueRow> Page { get; init; }
        public long ElapsedMs { get; init; }
        public string CorrelationId { get; init; }
        public BudgetRow Budget { get; init; }
    }
}
