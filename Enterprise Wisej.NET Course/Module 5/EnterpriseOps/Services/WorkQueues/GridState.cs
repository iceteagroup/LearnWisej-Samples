using System;
using System.Collections.Generic;

namespace EnterpriseOps.Services.WorkQueues
{
    /// <summary>
    /// The server-side grid state: the current query (filters, sort, page), the saved view it came from and the
    /// rows the user has selected — possibly across pages. It is owned by the <c>SessionContext</c>, not by the
    /// page, so a refresh restores the same view instead of starting over.
    /// </summary>
    public sealed class GridState
    {
        public const int DefaultPageSize = 50;

        public WorkQueueQuery Query { get; set; }

        /// <summary>Name of the saved view the query was loaded from; null once the user edits a filter.</summary>
        public string SavedViewName { get; set; }

        /// <summary>Selected rows keyed by work-order id. Snapshots, so a batch can carry Number + Version.</summary>
        public Dictionary<int, WorkQueueRow> Selected { get; } = new Dictionary<int, WorkQueueRow>();

        public int LastTotalCount { get; set; }
        public int LastPageCount { get; set; } = 1;
        public DateTime LastLoadedUtc { get; set; }

        public static GridState Default(string tenantId) => new GridState
        {
            Query = new WorkQueueQuery(tenantId, null, "Open", null, "Priority", true, 1, DefaultPageSize),
            SavedViewName = "My critical queue",
        };
    }
}
