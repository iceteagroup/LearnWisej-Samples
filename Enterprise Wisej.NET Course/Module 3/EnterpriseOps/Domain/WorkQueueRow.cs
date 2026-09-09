using System;
using System.Collections.Generic;

namespace EnterpriseOps.Domain
{
    /// <summary>Grid projection of a work order — what the work queue shows, never the entity itself.</summary>
    public sealed class WorkQueueRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public string Version { get; set; }
        public DateTime? DueUtc { get; set; }
    }

    /// <summary>What the work queue asks for. The tenant is NOT part of the query: it comes from the command context.</summary>
    public sealed class WorkQueueQuery
    {
        public string Search { get; set; } = "";
        public WorkOrderStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>One page of rows plus the total the filter matched.</summary>
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Rows { get; }
        public int Total { get; }
        public int Page { get; }
        public int PageSize { get; }

        public PagedResult(IReadOnlyList<T> rows, int total, int page, int pageSize)
        {
            Rows = rows;
            Total = total;
            Page = page;
            PageSize = pageSize;
        }
    }
}
