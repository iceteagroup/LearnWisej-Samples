using System.Collections.Generic;

namespace IntegrationLab.Contracts
{
    /// <summary>
    /// The vendor-free description of one "load" operation. Every vendor library, whatever
    /// it calls these things (Kendo: skip/take/sort/filter, DevExtreme: skip/take/sort/filter
    /// with a different filter shape), is translated into this request before the server
    /// touches any data. Designing it first keeps the server in control of what a filter
    /// may contain and how large a page may be.
    /// </summary>
    public sealed class GridOperationRequest
    {
        /// <summary>The largest page the server is willing to serve. Anything above it is a 400.</summary>
        public const int MaxTake = 100;

        /// <summary>Rows to skip (remote paging).</summary>
        public int Skip { get; set; }

        /// <summary>Rows to return (remote paging). 1..MaxTake.</summary>
        public int Take { get; set; } = 20;

        /// <summary>Sort descriptors in priority order (remote sorting).</summary>
        public List<SortDescriptor> Sort { get; set; } = new List<SortDescriptor>();

        /// <summary>Filter descriptors, all of which must match (remote filtering).</summary>
        public List<FilterDescriptor> Filter { get; set; } = new List<FilterDescriptor>();

        /// <summary>Compact one-line form used by the trace: {skip:0,take:20,sort:"status asc"}.</summary>
        public string ToTraceString()
        {
            var parts = new List<string> { "skip:" + this.Skip, "take:" + this.Take };
            if (this.Sort.Count > 0)
            {
                var s = new List<string>();
                foreach (var d in this.Sort) s.Add(d.Field + (d.Desc ? " desc" : " asc"));
                parts.Add("sort:\"" + string.Join(",", s) + "\"");
            }
            if (this.Filter.Count > 0)
            {
                var f = new List<string>();
                foreach (var d in this.Filter) f.Add(d.Field + " " + d.Op + " " + d.Value);
                parts.Add("filter:\"" + string.Join(" and ", f) + "\"");
            }
            return "{" + string.Join(",", parts) + "}";
        }
    }

    /// <summary>One sort term: field name and direction.</summary>
    public sealed class SortDescriptor
    {
        public SortDescriptor() { }
        public SortDescriptor(string field, bool desc) { this.Field = field; this.Desc = desc; }

        public string Field { get; set; }
        public bool Desc { get; set; }
    }

    /// <summary>One filter term. Allowed operators are decided by the store, not by the vendor.</summary>
    public sealed class FilterDescriptor
    {
        public FilterDescriptor() { }
        public FilterDescriptor(string field, string op, string value) { this.Field = field; this.Op = op; this.Value = value; }

        public string Field { get; set; }

        /// <summary>eq, neq, contains, gt, gte, lt, lte</summary>
        public string Op { get; set; }

        public string Value { get; set; }
    }
}
