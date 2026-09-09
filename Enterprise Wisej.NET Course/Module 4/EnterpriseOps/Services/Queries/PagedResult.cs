using System.Collections.Generic;

namespace EnterpriseOps.Services.Queries
{
    /// <summary>One page of projections plus the total — the grid binds one page at a time.</summary>
    public sealed class PagedResult<T>
    {
        public List<T> Rows { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
