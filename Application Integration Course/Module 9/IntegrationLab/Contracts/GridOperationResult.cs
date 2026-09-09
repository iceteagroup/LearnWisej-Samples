using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// The result of a load operation: the page of rows plus the total count so the
    /// vendor can draw its pager. Serialized as {"rows":[...],"total":150,"skip":0,"take":20}.
    /// </summary>
    public sealed class GridOperationResult<T>
    {
        public GridOperationResult(IReadOnlyList<T> rows, int total, int skip, int take)
        {
            this.Rows = rows;
            this.Total = total;
            this.Skip = skip;
            this.Take = take;
        }

        public IReadOnlyList<T> Rows { get; }
        public int Total { get; }
        public int Skip { get; }
        public int Take { get; }
    }
}
