using System.Collections.Generic;

namespace IntegrationLab.Data
{
    /// <summary>
    /// Wire shape of one work order. Plain data: strings, numbers, nothing the
    /// browser could execute and nothing that leaks server internals.
    /// Serialized with camelCase names ({"id":"WO-1042","asset":…}).
    /// </summary>
    public sealed class WorkOrderRow
    {
        public string Id { get; set; }
        public string Asset { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Assignee { get; set; }
        public string DueDate { get; set; }
        public double Hours { get; set; }
    }

    /// <summary>
    /// One page of rows plus the paging facts the grid needs to draw its footer.
    /// This is the payload of BOTH endpoints: the postback handler writes it as JSON,
    /// the WebMethod returns it and lets the framework marshal it.
    /// </summary>
    public sealed class PageResult
    {
        public List<WorkOrderRow> Rows { get; set; } = new List<WorkOrderRow>();

        /// <summary>Total number of rows in the dataset (not in the page).</summary>
        public int Total { get; set; }

        /// <summary>1-based page number that was served.</summary>
        public int Page { get; set; }

        /// <summary>Page size that was served (after bounding).</summary>
        public int Size { get; set; }

        /// <summary>Sort column that was applied (canonical whitelist name).</summary>
        public string Sort { get; set; }

        public bool Desc { get; set; }
    }
}
