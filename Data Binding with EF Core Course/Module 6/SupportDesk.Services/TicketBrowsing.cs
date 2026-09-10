namespace SupportDesk.Services;

/// <summary>
/// One row of the ticket grid — the seven-and-a-bit columns the operator actually reads, and nothing else.
/// </summary>
/// <remarks>
/// <para>
/// This is a <b>projection</b>, not an entity. <c>SearchTicketsAsync</c> builds it inside the LINQ
/// <c>Select</c>, so the database returns exactly these columns (the names come from joins it does itself)
/// instead of the <c>Ticket</c> entity with its <c>Description</c>, <c>RowVersion</c>, <c>Comments</c>
/// collection and three related objects. Fewer bytes on the wire, nothing in the change tracker, and a list
/// that stays valid long after the context that produced it is gone — which is exactly what a session-lived
/// <c>BindingSource</c> needs.
/// </para>
/// <para>
/// A record with a positional constructor binds read-only to a <c>DataGridView</c> without any extra work:
/// the grid only ever reads properties, and each column names one of them through
/// <c>DataPropertyName</c>.
/// </para>
/// </remarks>
public sealed record TicketListItem(
    int Id,
    string Number,
    string Title,
    string CustomerName,
    string? AgentName,
    string CategoryName,
    string Status,
    string Priority,
    DateTime? DueDate,
    DateTime UpdatedAt);

/// <summary>
/// What the operator asked for: the filter values read from the page's controls plus the page to fetch.
/// Every property is optional except the paging pair — an unset filter simply never becomes a
/// <c>Where</c> clause, so the empty criteria return the whole table one page at a time.
/// </summary>
public sealed record TicketSearchCriteria
{
    /// <summary>Free text: a ticket number prefix, or part of a title or customer name.</summary>
    public string? Text { get; init; }

    /// <summary>Exact status ("Open", "In Progress", …). Null = every status.</summary>
    public string? Status { get; init; }

    /// <summary>Customer key from <c>customerComboBox.SelectedValue</c>. Null = every customer.</summary>
    public int? CustomerId { get; init; }

    /// <summary>Earliest due date (inclusive). Null = no lower bound; tickets without a due date are excluded when either bound is set.</summary>
    public DateTime? DueFrom { get; init; }

    /// <summary>Latest due date (inclusive).</summary>
    public DateTime? DueTo { get; init; }

    /// <summary>Zero-based page. 0 = the first page.</summary>
    public int PageIndex { get; init; }

    /// <summary>Rows per page. The browser page uses 50.</summary>
    public int PageSize { get; init; } = 50;

    /// <summary>A one-line description of the filters that are actually set (for the trace).</summary>
    public string Describe()
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(Text)) parts.Add($"text '{Text.Trim()}'");
        if (!string.IsNullOrWhiteSpace(Status)) parts.Add($"status '{Status}'");
        if (CustomerId is not null) parts.Add($"customer #{CustomerId}");
        if (DueFrom is not null) parts.Add($"due ≥ {DueFrom:yyyy-MM-dd}");
        if (DueTo is not null) parts.Add($"due ≤ {DueTo:yyyy-MM-dd}");
        var filters = parts.Count == 0 ? "no filters" : string.Join(", ", parts);
        return $"{filters} · page {PageIndex + 1}, {PageSize} rows";
    }
}

/// <summary>One page of results plus the total the same filters match in the database.</summary>
/// <param name="Items">The materialised rows — a real list, never a query.</param>
/// <param name="TotalCount">How many rows match the filters before <c>Skip</c>/<c>Take</c> cut the page.</param>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount)
{
    /// <summary>Number of pages for <paramref name="pageSize"/>; always at least 1 so "page 1 of 1" reads correctly on an empty result.</summary>
    public int PageCount(int pageSize) => pageSize <= 0 ? 1 : Math.Max(1, (TotalCount + pageSize - 1) / pageSize);
}

/// <summary>
/// What Module 6's naive branch (<see cref="TicketQueryService.SearchTicketsNaiveAsync"/>) produced: the
/// same shape of rows the optimised branch returns, plus the two numbers that show the anti-pattern —
/// <see cref="TotalMatching"/> (every row the database returned, not one page of it: the naive branch never
/// asked the database to page) and <see cref="TrackedEntities"/> (how many entities the context is still
/// holding when the method returns — always 0 for the optimised, <see cref="PagedResult{T}"/>-returning
/// branch, since it runs <c>AsNoTracking</c>).
/// </summary>
public sealed record NaiveSearchResult(IReadOnlyList<TicketListItem> Items, int TotalMatching, int TrackedEntities);

/// <summary>A lookup row for a ComboBox: <see cref="Id"/> is the ValueMember, <see cref="Name"/> the DisplayMember.</summary>
public sealed record LookupItem(int Id, string Name);

/// <summary>
/// A string-keyed lookup row (statuses, priorities). Verified in the browser: a ComboBox bound with
/// SelectedValue needs a ValueMember; bound to a plain List&lt;string&gt; it shows the first row instead of the
/// model value and never pushes the model value into the control. Name is the display text, Value the key.
/// </summary>
public sealed record NamedValue(string Value, string Name);

/// <summary>
/// The ticket statuses the Support Desk uses. One fixed list shared by the seeder, the query service's
/// <c>GetStatusesAsync</c> and (from Module 5) the validator, so the lookup can never drift away from the
/// data: a <c>SELECT DISTINCT "Status"</c> would only ever show the statuses that happen to exist today.
/// </summary>
public static class TicketStatuses
{
    public const string Open = "Open";
    public const string InProgress = "In Progress";
    public const string Waiting = "Waiting";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";

    /// <summary>The five statuses, in workflow order.</summary>
    public static readonly IReadOnlyList<string> All = new[] { Open, InProgress, Waiting, Resolved, Closed };
}

/// <summary>
/// The ticket priorities the Support Desk uses. One fixed list — the same shape as
/// <see cref="TicketStatuses"/> — shared by the seeder and (from Module 4) <c>cboPriority</c> in the
/// ticket editor, so the ComboBox can never offer a value the database does not expect.
/// </summary>
public static class TicketPriorities
{
    public const string Low = "Low";
    public const string Normal = "Normal";
    public const string High = "High";

    /// <summary>The three priorities, lowest first.</summary>
    public static readonly IReadOnlyList<string> All = new[] { Low, Normal, High };
}
