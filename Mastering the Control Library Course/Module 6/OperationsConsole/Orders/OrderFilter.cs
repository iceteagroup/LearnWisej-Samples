using System;

namespace OperationsConsole.Orders
{
    /// <summary>
    /// What the composed filter strip asks the service for: free text and one status.
    /// <para>
    /// The filter is the screen's first line of defence against large data — the lab's rule is "filter before
    /// displaying". Both the bound path and the virtual path take the same filter object, so switching between them
    /// never changes what the user is looking at, only how the rows reach the grid.
    /// </para>
    /// </summary>
    public class OrderFilter
    {
        /// <summary>The "no status filter" entry of the status ComboBox.</summary>
        public const string AllStatuses = "All statuses";

        /// <summary>Free text matched against the order number and the customer name (case-insensitive).</summary>
        public string Search { get; set; } = "";

        /// <summary>One status, or <see cref="AllStatuses"/>.</summary>
        public string Status { get; set; } = AllStatuses;

        /// <summary>True when nothing is filtered out.</summary>
        public bool IsEmpty =>
            string.IsNullOrWhiteSpace(Search) &&
            (string.IsNullOrEmpty(Status) || Status == AllStatuses);

        /// <summary>A copy — the cache keeps the filter it was primed with, so a half-typed search box cannot
        /// silently change what the pages already in memory mean.</summary>
        public OrderFilter Clone() => new OrderFilter { Search = Search, Status = Status };

        /// <summary>True when the two filters ask for the same result (used by the cache to decide whether its pages
        /// are still valid).</summary>
        public bool SameAs(OrderFilter other) =>
            other != null &&
            string.Equals(Search ?? "", other.Search ?? "", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(Status ?? "", other.Status ?? "", StringComparison.Ordinal);

        /// <summary>A short human description for the Event log and the status strip.</summary>
        public string Describe()
        {
            if (IsEmpty)
                return "no filter";

            var text = string.IsNullOrWhiteSpace(Search) ? "" : "search \"" + Search.Trim() + "\"";
            var status = (string.IsNullOrEmpty(Status) || Status == AllStatuses) ? "" : "status " + Status;

            if (text.Length > 0 && status.Length > 0)
                return text + " + " + status;

            return text.Length > 0 ? text : status;
        }

        /// <inheritdoc/>
        public override string ToString() => Describe();
    }
}
