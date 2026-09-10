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

    /// <summary>
    /// Lab prop: the same count with an artificial delay, so the loading guard in the page can be
    /// watched (a busy server, a slow network). The delay sits inside the unit of work on purpose:
    /// the context stays alive for the whole operation and is still disposed at the end.
    /// </summary>
    public async Task<int> CountTicketsSlowlyAsync(TimeSpan latency, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        await Task.Delay(latency, token);
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

    /// <summary>
    /// Lab prop: the same search with an artificial delay <b>inside</b> the unit of work, so the loading
    /// guard, the disabled buttons and the status text can be watched. The context is created before the
    /// delay and disposed after the query, exactly like the real path — the operation is slow, the lifetime
    /// rule is not bent.
    /// </summary>
    public async Task<PagedResult<TicketListItem>> SearchTicketsSlowlyAsync(TicketSearchCriteria criteria, TimeSpan latency, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        QueryTrace.Note($"simulated latency of {latency.TotalSeconds:0.#} s inside the unit of work — the context is already open and the page is guarded");
        await Task.Delay(latency, token);
        return await RunSearchAsync(db, criteria, token);
    }

    /// <summary>The composed query. Shared by the normal and the slow search, so both send the same two statements.</summary>
    private static async Task<PagedResult<TicketListItem>> RunSearchAsync(SupportDeskContext db, TicketSearchCriteria criteria, CancellationToken token)
    {
        var pageSize = Math.Max(1, criteria.PageSize);
        var pageIndex = Math.Max(0, criteria.PageIndex);

        // 1. The recipe. Nothing has been sent yet.
        var query = ApplyFilters(db.Tickets.AsNoTracking(), criteria);

        QueryTrace.Note($"composed IQueryable<Ticket>: {criteria.Describe()} — nothing sent yet; the two awaits below are the only statements");

        // 2. Statement one: how many rows match, before the page is cut.
        var total = await query.CountAsync(token);

        // 3. Statement two: order, page and project — the database returns at most one page of ten columns.
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

    /// <summary>
    /// One <c>Where</c> per filter that is actually set — an unset filter never becomes SQL. Shared by the
    /// optimised branch (<see cref="RunSearchAsync"/>) and Module 6's naive branch
    /// (<see cref="RunSearchNaiveAsync"/>) so the two branches are guaranteed to test the same criteria the
    /// same way: only the loading and shaping strategy below differs, never the filters.
    /// </summary>
    private static IQueryable<Ticket> ApplyFilters(IQueryable<Ticket> query, TicketSearchCriteria criteria)
    {
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

        return query;
    }

    #endregion

    #region Module 6 · the deliberately naive branch — kept as a named anti-pattern to measure against

    /// <summary>
    /// How many of the naive branch's matching tickets are shaped into rows — the same page size the
    /// optimised branch pages at in SQL, so the two branches are comparable. The naive branch's own SQL has
    /// <b>no</b> <c>Skip</c>/<c>Take</c> at all — capping happens only after everything has already been
    /// pulled into memory, which is the anti-pattern the lab measures, not a paging strategy of its own.
    /// </summary>
    public const int NaivePageCap = 50;

    /// <summary>
    /// The Module 6 lab's starting point, kept in the solution as a named anti-pattern rather than deleted
    /// once the fix lands: every matching <see cref="Ticket"/> is loaded <b>tracked</b>, with no SQL-level
    /// paging and no projection, and then shaped into <see cref="TicketListItem"/> rows in memory exactly
    /// the way a <c>CellFormatting</c> handler that reads navigation properties would — one explicit
    /// per-row load of <see cref="Ticket.Customer"/>, <see cref="Ticket.Agent"/> and
    /// <see cref="Ticket.Category"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>There is no lazy loading anywhere in this solution</b> — <c>UseLazyLoadingProxies</c> is never
    /// added in <c>AddSupportDeskData</c> — so a hidden per-row query cannot happen here by accident. This
    /// method writes out, on purpose and in the open, the exact queries lazy loading would otherwise have
    /// issued silently the moment a formatting handler first read <c>ticket.Customer.Name</c>:
    /// <c>await db.Entry(t).Reference(x =&gt; x.Customer).LoadAsync()</c> and its Agent/Category
    /// equivalents. That is the whole point of keeping this branch instead of deleting it once
    /// <see cref="TicketQueryService.SearchTicketsAsync"/> is fixed: "nothing in the application warned
    /// you" (the lab guide's own review question) is true of lazy loading turned on, and it would be just
    /// as true of this anti-pattern if it were quietly removed instead of measured and left in place.
    /// </para>
    /// <para>
    /// <b>Why the measured statement count on this seed is not the lesson's illustrative "1 + 3 × rows".</b>
    /// EF Core's change tracker performs automatic reference fix-up among entities already tracked by the
    /// same <see cref="SupportDeskContext"/>: once one ticket's <c>Customer</c> has been loaded, every later
    /// ticket that shares the same <c>CustomerId</c> gets its <c>Customer</c> navigation wired up from the
    /// tracked graph, with <c>IsLoaded</c> already <see langword="true"/> —
    /// <c>ReferenceEntry.LoadAsync</c> returns immediately and never reaches the database. Verified with a
    /// console probe against this seed: a <c>Reference().LoadAsync()</c> call for a foreign key that is
    /// <see langword="null"/> issues <b>zero</b> commands, and a foreign key value already represented by a
    /// tracked entity issues zero <i>additional</i> commands. The Support Desk seed has only 5 customers, 3
    /// agents and 6 categories behind 312 tickets, so only the first occurrence of each costs a statement —
    /// measured at <b>15</b> statements for the default (no filter) 50-row page, not 151. The anti-pattern
    /// is exactly as real on this seed as on a large one (untracked memory held for the life of the
    /// context, no SQL-level paging, no projection, and the count still depends on which rows the filters
    /// happen to match), but its <i>statement count</i> undersells the classic N+1 story here, because the
    /// lookup cardinality is small. See <c>docs/BeforeAfterMeasurements.md</c> for the full measured
    /// comparison, including what the count becomes when every per-row load is forced to skip the
    /// tracked-graph shortcut (independent queries against <c>Customers</c>/<c>Agents</c>/<c>Categories</c>
    /// instead of <c>Entry(...).Reference(...)</c> — measured separately at 131 statements for the same 50
    /// rows: the number a support desk with thousands of distinct customers would actually see).
    /// </para>
    /// </remarks>
    public async Task<NaiveSearchResult> SearchTicketsNaiveAsync(TicketSearchCriteria criteria, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        return await RunSearchNaiveAsync(db, criteria, token);
    }

    private static async Task<NaiveSearchResult> RunSearchNaiveAsync(SupportDeskContext db, TicketSearchCriteria criteria, CancellationToken token)
    {
        // Tracked — no AsNoTracking — and no Skip/Take: the whole matching set is about to be pulled into
        // memory, exactly the mistake the lab wants visible in the trace.
        var query = ApplyFilters(db.Tickets, criteria);

        QueryTrace.Note($"naive branch: {criteria.Describe()} — tracked Tickets.Where(...), no Skip/Take, no Select: the whole matching set is about to be loaded");

        // Statement 1: every matching ticket, tracked, not paged in SQL at all.
        var all = await query.OrderByDescending(t => t.UpdatedAt).ToListAsync(token);

        // The lab's deterministic cap: only the first NaivePageCap rows are shaped into the grid — matching
        // what one screen would actually show — so the per-row cost below does not depend on how many rows
        // the filters happened to match. The SQL above still has no Skip/Take; this Take runs in memory,
        // after the database has already done the (unbounded) work.
        var page = all.Take(NaivePageCap).ToList();
        var items = new List<TicketListItem>(page.Count);

        foreach (var t in page)
        {
            // The three lines a CellFormatting handler's first navigation read would have triggered under
            // lazy loading — written out loud here instead of hidden there. See the class remarks for why
            // the resulting statement count is not literally 1 + 3 × rows on this seed.
            await db.Entry(t).Reference(x => x.Customer).LoadAsync(token);
            await db.Entry(t).Reference(x => x.Agent).LoadAsync(token);
            await db.Entry(t).Reference(x => x.Category).LoadAsync(token);

            items.Add(new TicketListItem(
                t.Id, t.Number, t.Title, t.Customer.Name,
                t.Agent?.DisplayName, t.Category.Name,
                t.Status, t.Priority, t.DueDate, t.UpdatedAt));
        }

        var tracked = db.ChangeTracker.Entries().Count();
        QueryTrace.Note($"naive branch: {all.Count} tracked Ticket(s) matched (no SQL paging), {page.Count} shaped in memory (capped) · {tracked} entities held tracked by this context — none of them will ever be saved");

        // TotalMatching reflects what the naive branch actually asked the database for: every matching row,
        // not one page of it — unlike the optimised branch's separate CountAsync, this number is simply the
        // size of the list already sitting in memory, which is the whole anti-pattern in one field.
        return new NaiveSearchResult(items, all.Count, tracked);
    }

    #endregion

    #region Module 6 · a background report job — its own context, progress pushed by the caller

    /// <summary>
    /// A report-style background job: how many tickets sit in each of the five statuses, counted one status
    /// at a time with a pause between steps so progress is actually visible to a human. Runs inside
    /// <b>one</b> context, created here — not captured from whatever click started it — and disposed when
    /// every step is done: the "a DbContext lives for one operation" rule applied to an operation that
    /// happens to take a few seconds instead of a few milliseconds. The caller
    /// (<c>TicketBrowserPage.btnLongJob_Click</c>, running inside <c>Application.StartTask</c>) supplies
    /// <paramref name="onStep"/> to push progress into a label through <c>Application.Update</c>; this
    /// method has no reference to <c>Wisej.Web</c> and knows nothing about how its progress is shown.
    /// </summary>
    /// <param name="onStep">Called once per status with (status, count so far this status, step number).</param>
    /// <param name="stepDelay">The pause between steps. Zero in a test — nothing here needs to be slow to be correct.</param>
    public async Task CountTicketsPerStatusAsync(Action<string, int, int> onStep, TimeSpan stepDelay, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(onStep);

        await using var db = await _dbFactory.CreateDbContextAsync(token);
        QueryTrace.Note($"CountTicketsPerStatusAsync: one context created here for the whole {TicketStatuses.All.Count}-step job, not one per step");

        for (var i = 0; i < TicketStatuses.All.Count; i++)
        {
            var status = TicketStatuses.All[i];
            var count = await db.Tickets.CountAsync(t => t.Status == status, token);
            onStep(status, count, i + 1);

            if (stepDelay > TimeSpan.Zero)
                await Task.Delay(stepDelay, token);
        }

        QueryTrace.Note($"CountTicketsPerStatusAsync: {TicketStatuses.All.Count} statements in one context, about to be disposed");
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
