using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>
/// Read-side service for tickets. It holds only the factory — no context, no entities, no UI
/// state — so it is safe to resolve from any session and any thread.
/// </summary>
/// <remarks>
/// Module 3 adds the ticket browser's query: <see cref="SearchTicketsAsync"/> composes the filters, the
/// order and the page on an <see cref="IQueryable{T}"/> and executes it exactly twice — one
/// <c>COUNT</c> for the total and one paged <c>SELECT</c> for the rows. Everything the grid shows is
/// projected into <see cref="TicketListItem"/>; no <c>Ticket</c> entity ever leaves this class.
/// </remarks>
public sealed class TicketQueryService
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public TicketQueryService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    #region Module 1 · the first query

    /// <summary>One context, one COUNT, disposed before the method returns.</summary>
    public async Task<int> CountTicketsAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        return await db.Tickets.CountAsync(token);
    }

    #endregion

    #region Module 3 · the ticket browser query

    /// <summary>
    /// The ticket browser's search: filters, order and page all composed on the query and executed by the
    /// database in <b>exactly two statements</b> — a <c>COUNT</c> over the filtered set and one projected,
    /// paged <c>SELECT</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nothing runs while the query is being built. <c>db.Tickets.AsNoTracking()</c> is an
    /// <see cref="IQueryable{T}"/> — a recipe — and every <c>Where</c> below only extends that recipe. The
    /// two <c>await</c>s are the only lines that send SQL, which is why the filters, the ordering and the
    /// page reach the database instead of being applied to a list in server memory.
    /// </para>
    /// <para>
    /// <c>AsNoTracking</c> keeps the change tracker empty: the rows are read-only list items, nothing is
    /// snapshotted, and a stray <c>SaveChangesAsync</c> elsewhere could not write them back. The editor in
    /// Module 4 does the opposite on purpose — it loads one <i>tracked</i> ticket through its own context.
    /// </para>
    /// </remarks>
    public async Task<PagedResult<TicketListItem>> SearchTicketsAsync(TicketSearchCriteria criteria, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        return await RunSearchAsync(db, criteria, token);
    }

    /// <summary>The composed query: exactly two statements, a COUNT and one paged SELECT.</summary>
    private static async Task<PagedResult<TicketListItem>> RunSearchAsync(SupportDeskContext db, TicketSearchCriteria criteria, CancellationToken token)
    {
        var pageSize = Math.Max(1, criteria.PageSize);
        var pageIndex = Math.Max(0, criteria.PageIndex);

        // 1. The recipe. Nothing has been sent yet.
        var query = db.Tickets.AsNoTracking();

        // 2. One Where per filter that is actually set — an unset filter never becomes SQL.
        if (!string.IsNullOrWhiteSpace(criteria.Text))
        {
            var text = criteria.Text.Trim();
            // StartsWith on Number translates to LIKE 'SD-10%', which the unique index IX_Tickets_Number can
            // seek — the index-friendly half. Contains becomes instr(...) > 0 on SQLite (LIKE '%…%' on SQL
            // Server) over Title and the joined customer name and always scans; that is the price of a
            // free-text box, and the reason the number is matched by prefix instead of by Contains.
            query = query.Where(t => t.Number.StartsWith(text) || t.Title.Contains(text) || t.Customer.Name.Contains(text));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Status))
        {
            var status = criteria.Status;
            query = query.Where(t => t.Status == status);           // IX_Tickets_Status_DueDate
        }

        if (criteria.CustomerId is int customerId)
        {
            query = query.Where(t => t.CustomerId == customerId);   // IX_Tickets_CustomerId
        }

        if (criteria.DueFrom is DateTime dueFrom)
        {
            query = query.Where(t => t.DueDate != null && t.DueDate >= dueFrom);
        }

        if (criteria.DueTo is DateTime dueTo)
        {
            query = query.Where(t => t.DueDate != null && t.DueDate <= dueTo);
        }

        QueryTrace.Note($"composed IQueryable<Ticket>: {criteria.Describe()} — nothing sent yet; the two awaits below are the only statements");

        // 3. Statement one: how many rows match, before the page is cut.
        var total = await query.CountAsync(token);

        // 4. Statement two: order, page and project — the database returns at most one page of ten columns.
        var items = await query
            .OrderByDescending(t => t.UpdatedAt)                     // IX_Tickets_UpdatedAt
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(t => new TicketListItem(
                t.Id,
                t.Number,
                t.Title,
                t.Customer.Name,                                    // joined by the database
                t.Agent == null ? null : t.Agent.DisplayName,       // LEFT JOIN: a ticket may be unassigned
                t.Category.Name,
                t.Status,
                t.Priority,
                t.DueDate,
                t.UpdatedAt))
            .ToListAsync(token);

        QueryTrace.Note($"materialised {items.Count} TicketListItem rows of {total} matching · 0 entities tracked (AsNoTracking) — the list outlives this context");

        return new PagedResult<TicketListItem>(items, total);
    }

    #endregion

    #region Module 3 · the lookups (loaded before the first search)

    /// <summary>
    /// The status lookup. It is the fixed <see cref="TicketStatuses.All"/> list the seeder writes, not a
    /// <c>SELECT DISTINCT "Status"</c>: a distinct query costs a statement and would silently drop a status
    /// nobody happens to be using today — "Resolved" would vanish from the filter the moment the last
    /// resolved ticket was closed. No database round-trip at all; the task is already complete.
    /// </summary>
    public Task<IReadOnlyList<string>> GetStatusesAsync(CancellationToken token = default)
    {
        QueryTrace.Note($"status lookup: the fixed TicketStatuses.All list ({TicketStatuses.All.Count} values) — no statement is sent");
        return Task.FromResult(TicketStatuses.All);
    }

    /// <summary>
    /// The customer lookup: <c>Id</c> + <c>Name</c> only, no tracking, ordered by name — one statement, and
    /// the smallest projection that can fill a ComboBox. It is loaded once in the page's Load handler,
    /// before the first search, because a <c>SelectedValue</c> with no matching item resolves to nothing.
    /// </summary>
    public async Task<IReadOnlyList<LookupItem>> GetCustomersAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var customers = await db.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)                                   // IX_Customers_Name
            .Select(c => new LookupItem(c.Id, c.Name))
            .ToListAsync(token);

        QueryTrace.Note($"customer lookup: {customers.Count} rows projected to LookupItem (Id + Name), nothing tracked");
        return customers;
    }

    #endregion
}
