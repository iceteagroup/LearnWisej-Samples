using System;
using System.Collections.Generic;

namespace OrderDesk.Domain
{
    /// <summary>The columns an order list can be ordered by — the same set the grid shows.</summary>
    public enum OrderSort
    {
        Id,
        Customer,
        Total,
        Status,
        Date
    }

    /// <summary>
    /// Module 5 addition to the Domain: the filter + sort + page the server applies BEFORE anything
    /// is sent to the browser. LegacyOrderDesk had only <see cref="OrderFilter"/> (status + text) and
    /// bound the whole result to the grid; this class carries the rest of the "virtual row thinking":
    /// the user sees a page-sized viewport, so the query says which page. No UI dependency.
    /// </summary>
    public sealed class OrderQuery
    {
        public OrderStatus? Status { get; set; }
        public string Text { get; set; }
        public int? CustomerId { get; set; }
        public OrderSort SortBy { get; set; } = OrderSort.Date;
        public bool Descending { get; set; } = true;
        public int Skip { get; set; }
        public int Take { get; set; } = 100;

        /// <summary>The same filter and sort, a different window.</summary>
        public OrderQuery WithPage(int skip, int take)
        {
            var copy = Clone();
            copy.Skip = Math.Max(0, skip);
            copy.Take = Math.Max(0, take);
            return copy;
        }

        public OrderQuery Clone() => new OrderQuery
        {
            Status = Status, Text = Text, CustomerId = CustomerId, SortBy = SortBy, Descending = Descending, Skip = Skip, Take = Take
        };

        /// <summary>
        /// Identifies the filter + sort (not the page): two queries with the same key share the same
        /// ordered result set, so the store can memoize it between block fetches.
        /// </summary>
        public string Key =>
            $"{(Status.HasValue ? Status.Value.ToString() : "*")}|{(Text ?? "").Trim().ToLowerInvariant()}|{(CustomerId.HasValue ? CustomerId.Value.ToString() : "*")}|{SortBy}|{(Descending ? "desc" : "asc")}";

        /// <summary>Human-readable form for the trace: "status=Open · text='north' · sort=Date desc".</summary>
        public string Describe()
        {
            var parts = new List<string>
            {
                "status=" + (Status.HasValue ? Status.Value.ToString() : "all")
            };
            if (!string.IsNullOrWhiteSpace(Text)) parts.Add($"text='{Text.Trim()}'");
            if (CustomerId.HasValue) parts.Add("customer=" + CustomerId.Value);
            parts.Add($"sort={SortBy} {(Descending ? "desc" : "asc")}");
            return string.Join(" · ", parts);
        }
    }

    /// <summary>One page of a server-side query plus the numbers the grid footer and the trace need.</summary>
    public sealed class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> items, int total, TimeSpan elapsed, bool fromCache)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            Total = total;
            Elapsed = elapsed;
            FromCache = fromCache;
        }

        /// <summary>The rows of this page only — never the whole table.</summary>
        public IReadOnlyList<T> Items { get; }

        /// <summary>How many rows match the filter (what the grid reports as RowCount).</summary>
        public int Total { get; }

        /// <summary>Server time spent producing this page.</summary>
        public TimeSpan Elapsed { get; }

        /// <summary>True when the ordered result set was already memoized for this filter + sort.</summary>
        public bool FromCache { get; }
    }

    /// <summary>The Σ row: count and total of everything the filter matches, computed on the server.</summary>
    public sealed class OrderSummary
    {
        public OrderSummary(int count, decimal total, TimeSpan elapsed)
        {
            Count = count;
            Total = total;
            Elapsed = elapsed;
        }

        public int Count { get; }
        public decimal Total { get; }
        public TimeSpan Elapsed { get; }
    }
}
