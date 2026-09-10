using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 3 deliverable, tested without the UI: <see cref="TicketQueryService.SearchTicketsAsync"/>
/// against SQLite in memory with the real development seed (312 tickets). What is proved here is what the
/// instructor acceptance criteria ask for — the total and the page, the filters, the ordering, paging that
/// happens in the database, and <b>exactly two statements per search</b>.
/// </summary>
public sealed class TicketSearchTests : IClassFixture<SeededSupportDesk>
{
    private const int PageSize = 50;

    private readonly SeededSupportDesk _db;

    public TicketSearchTests(SeededSupportDesk db) => _db = db;

    private static TicketSearchCriteria Page(int index = 0) => new() { PageIndex = index, PageSize = PageSize };

    #region The total, the page and the ordering

    [Fact]
    public async Task Empty_criteria_return_one_page_and_the_total_of_every_ticket()
    {
        var result = await _db.Tickets.SearchTicketsAsync(Page());

        Assert.Equal(312, result.TotalCount);                 // what the status label calls "of 312"
        Assert.Equal(PageSize, result.Items.Count);           // …and what it calls "Showing 50"
        Assert.Equal(7, result.PageCount(PageSize));          // 312 / 50, rounded up
    }

    [Fact]
    public async Task Results_are_ordered_by_UpdatedAt_descending()
    {
        var result = await _db.Tickets.SearchTicketsAsync(Page());

        var updated = result.Items.Select(t => t.UpdatedAt).ToList();
        Assert.Equal(updated.OrderByDescending(u => u).ToList(), updated);
        Assert.Equal(updated.Distinct().Count(), updated.Count);   // no ties, so paging is stable
    }

    [Fact]
    public async Task Page_two_skips_the_first_fifty_rows_and_keeps_the_same_total()
    {
        var first = await _db.Tickets.SearchTicketsAsync(Page(0));
        var second = await _db.Tickets.SearchTicketsAsync(Page(1));

        Assert.Equal(first.TotalCount, second.TotalCount);
        Assert.Equal(PageSize, second.Items.Count);
        Assert.Empty(first.Items.Select(t => t.Id).Intersect(second.Items.Select(t => t.Id)));
        Assert.True(second.Items[0].UpdatedAt < first.Items[^1].UpdatedAt);
    }

    [Fact]
    public async Task The_last_page_returns_the_remainder_only()
    {
        var last = await _db.Tickets.SearchTicketsAsync(Page(6));       // page 7 of 7

        Assert.Equal(312, last.TotalCount);
        Assert.Equal(312 - 6 * PageSize, last.Items.Count);             // 12 rows
    }

    [Fact]
    public async Task A_page_past_the_end_returns_no_rows_but_still_reports_the_total()
    {
        var beyond = await _db.Tickets.SearchTicketsAsync(Page(20));

        Assert.Equal(312, beyond.TotalCount);
        Assert.Empty(beyond.Items);
    }

    #endregion

    #region Two statements per search — the work happens in the database

    [Fact]
    public async Task A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _db.Tickets.SearchTicketsAsync(Page(1));

        Assert.Equal(2, scope.Commands);
        Assert.Equal(1, scope.ContextsCreated);
        Assert.Equal(1, scope.ContextsDisposed);

        var sql = entries.Where(e => e.Kind == TraceKind.Command).Select(e => e.Text).ToList();
        Assert.Contains("SELECT COUNT(*)", sql[0]);
        Assert.DoesNotContain("LIMIT", sql[0]);                         // the total is counted before the page is cut

        // SQLite renders paging as LIMIT/OFFSET, not the OFFSET … FETCH NEXT of SQL Server. Both are
        // parameterised, so one plan serves every page.
        Assert.Contains("LIMIT @", sql[1]);
        Assert.Contains("OFFSET @", sql[1]);
        Assert.Contains("ORDER BY \"t\".\"UpdatedAt\" DESC", sql[1]);
    }

    [Fact]
    public async Task Every_filter_that_is_set_becomes_SQL_and_the_others_do_not_exist()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria
        {
            PageSize = PageSize,
            Text = "printer",
            Status = TicketStatuses.Open,
            CustomerId = 1,
            DueFrom = DateTime.UtcNow.Date,
            DueTo = DateTime.UtcNow.Date.AddDays(14)
        });

        var count = entries.First(e => e.Kind == TraceKind.Command).Text;
        Assert.Equal(2, scope.Commands);
        Assert.Contains("\"t\".\"Status\" = @", count);
        Assert.Contains("\"t\".\"CustomerId\" = @", count);
        Assert.Contains("\"t\".\"DueDate\" >= @", count);
        Assert.Contains("\"t\".\"DueDate\" <= @", count);
        Assert.Contains("LIKE @", count);                               // Number.StartsWith → LIKE 'x%'
        Assert.Contains("instr(", count);                               // Contains → instr(...) on SQLite
    }

    [Fact]
    public async Task An_unset_filter_produces_no_WHERE_clause_at_all()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _db.Tickets.SearchTicketsAsync(Page());

        var count = entries.First(e => e.Kind == TraceKind.Command).Text;
        Assert.Equal(2, scope.Commands);
        Assert.DoesNotContain("WHERE", count);
    }

    [Fact]
    public async Task The_search_tracks_nothing_and_never_returns_an_entity()
    {
        await using var db = _db.Factory.CreateDbContext();
        var before = db.ChangeTracker.Entries().Count();

        var result = await _db.Tickets.SearchTicketsAsync(Page());

        Assert.Equal(before, db.ChangeTracker.Entries().Count());        // the search used its own context
        Assert.All(result.Items, item => Assert.IsType<TicketListItem>(item));
    }

    #endregion

    #region The filters

    [Fact]
    public async Task Status_filter_returns_only_that_status_and_a_smaller_total()
    {
        var all = await _db.Tickets.SearchTicketsAsync(Page());
        var open = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, Status = TicketStatuses.Open });

        Assert.All(open.Items, t => Assert.Equal(TicketStatuses.Open, t.Status));
        Assert.InRange(open.TotalCount, 1, all.TotalCount - 1);

        await using var db = _db.Factory.CreateDbContext();
        Assert.Equal(await db.Tickets.CountAsync(t => t.Status == TicketStatuses.Open), open.TotalCount);
    }

    [Fact]
    public async Task Customer_filter_uses_the_key_not_the_display_text()
    {
        await using var db = _db.Factory.CreateDbContext();
        var customer = await db.Customers.OrderBy(c => c.Id).FirstAsync();
        var expected = await db.Tickets.CountAsync(t => t.CustomerId == customer.Id);

        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, CustomerId = customer.Id });

        Assert.Equal(expected, result.TotalCount);
        Assert.All(result.Items, t => Assert.Equal(customer.Name, t.CustomerName));
    }

    [Fact]
    public async Task Text_matches_a_ticket_number_by_prefix()
    {
        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, Text = "SD-10" });

        Assert.Equal(99, result.TotalCount);                            // SD-1001 … SD-1099
        Assert.All(result.Items, t => Assert.StartsWith("SD-10", t.Number));
    }

    [Fact]
    public async Task Text_matches_part_of_a_title()
    {
        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, Text = "printer" });

        Assert.True(result.TotalCount > 0);
        Assert.All(result.Items, t => Assert.True(
            t.Title.Contains("printer", StringComparison.Ordinal) || t.Number.StartsWith("printer", StringComparison.Ordinal) || t.CustomerName.Contains("printer", StringComparison.Ordinal),
            $"'{t.Title}' does not contain the search text"));
    }

    [Fact]
    public async Task Text_matches_part_of_a_customer_name()
    {
        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, Text = "Brightwater" });

        Assert.True(result.TotalCount > 0);
        Assert.All(result.Items, t => Assert.Equal("Brightwater Clinics", t.CustomerName));
    }

    [Fact]
    public async Task Due_range_filter_keeps_the_bounds_and_drops_tickets_without_a_due_date()
    {
        var from = DateTime.UtcNow.Date.AddDays(1);
        var to = DateTime.UtcNow.Date.AddDays(10);

        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, DueFrom = from, DueTo = to });

        Assert.True(result.TotalCount > 0);
        Assert.All(result.Items, t =>
        {
            Assert.NotNull(t.DueDate);
            Assert.InRange(t.DueDate!.Value, from, to);
        });

        await using var db = _db.Factory.CreateDbContext();
        Assert.Equal(await db.Tickets.CountAsync(t => t.DueDate != null && t.DueDate >= from && t.DueDate <= to), result.TotalCount);
    }

    [Fact]
    public async Task Filters_that_match_nothing_return_an_empty_page_and_a_zero_total()
    {
        var result = await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageSize = PageSize, Text = "there is no such ticket" });

        Assert.Equal(0, result.TotalCount);
        Assert.Empty(result.Items);
        Assert.Equal(1, result.PageCount(PageSize));                    // "page 1 of 1", never "of 0"
    }

    #endregion

    #region The projection

    [Fact]
    public async Task The_projection_carries_the_names_the_database_joined()
    {
        await using var db = _db.Factory.CreateDbContext();
        var expected = await db.Tickets
            .AsNoTracking()
            .OrderByDescending(t => t.UpdatedAt)
            .Select(t => new { t.Number, Customer = t.Customer.Name, Category = t.Category.Name, Agent = t.Agent!.DisplayName })
            .FirstAsync();

        var first = (await _db.Tickets.SearchTicketsAsync(Page())).Items[0];

        Assert.Equal(expected.Number, first.Number);
        Assert.Equal(expected.Customer, first.CustomerName);
        Assert.Equal(expected.Category, first.CategoryName);
        Assert.Equal(expected.Agent, first.AgentName);
    }

    [Fact]
    public async Task An_unassigned_ticket_comes_back_with_a_null_agent_name()
    {
        // The seed leaves about 30 % of the tickets unassigned; the LEFT JOIN must not drop them.
        var pages = new List<TicketListItem>();
        for (var page = 0; page < 7; page++)
            pages.AddRange((await _db.Tickets.SearchTicketsAsync(Page(page))).Items);

        Assert.Equal(312, pages.Count);
        Assert.Contains(pages, t => t.AgentName is null);
        Assert.Contains(pages, t => t.AgentName is not null);
        Assert.Contains(pages, t => t.DueDate is null);
        Assert.Equal(312, pages.Select(t => t.Id).Distinct().Count());   // every ticket exactly once
    }

    #endregion

    #region The lookups

    [Fact]
    public async Task GetStatusesAsync_returns_the_five_statuses_the_seeder_uses_and_sends_nothing()
    {
        using var scope = QueryTrace.Begin(_ => { });

        var statuses = await _db.Tickets.GetStatusesAsync();

        Assert.Equal(new[] { "Open", "In Progress", "Waiting", "Resolved", "Closed" }, statuses);
        Assert.Equal(0, scope.Commands);

        await using var db = _db.Factory.CreateDbContext();
        var used = await db.Tickets.Select(t => t.Status).Distinct().ToListAsync();
        Assert.All(used, status => Assert.Contains(status, statuses));   // the lookup covers the data
    }

    [Fact]
    public async Task GetCustomersAsync_returns_keys_and_names_in_one_statement()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var customers = await _db.Tickets.GetCustomersAsync();

        Assert.Equal(5, customers.Count);
        Assert.Equal(1, scope.Commands);
        Assert.Equal(1, scope.ContextsCreated);
        Assert.Equal(1, scope.ContextsDisposed);
        Assert.Equal(customers.OrderBy(c => c.Name, StringComparer.Ordinal).ToList(), customers);
        Assert.All(customers, c => Assert.True(c.Id > 0));

        var sql = entries.Single(e => e.Kind == TraceKind.Command).Text;
        Assert.Equal("SELECT \"c\".\"Id\", \"c\".\"Name\" FROM \"Customers\" AS \"c\" ORDER BY \"c\".\"Name\"", sql);
    }

    #endregion

    #region The anti-pattern

    [Fact]
    public async Task Binding_the_query_instead_of_the_list_fails_when_the_grid_enumerates()
    {
        var antiPattern = new BoundIQueryableAntiPattern(_db.Factory);

        var ex = await Assert.ThrowsAnyAsync<Exception>(() => antiPattern.BindTheQueryAndLetTheGridEnumerateAsync());

        Assert.True(ex is ObjectDisposedException or InvalidOperationException, $"unexpected {ex.GetType().Name}");
        Assert.Contains("disposed context", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region The slow search (the progress path)

    [Fact]
    public async Task The_slow_search_returns_the_same_page_and_still_sends_two_statements()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var slow = await _db.Tickets.SearchTicketsSlowlyAsync(Page(), TimeSpan.FromMilliseconds(20));
        var fast = await _db.Tickets.SearchTicketsAsync(Page());

        Assert.Equal(fast.TotalCount, slow.TotalCount);
        Assert.Equal(fast.Items.Select(t => t.Id), slow.Items.Select(t => t.Id));
        Assert.Equal(4, scope.Commands);                                 // two searches, two statements each
        Assert.Equal(2, scope.ContextsCreated);
        Assert.Equal(2, scope.ContextsDisposed);                         // the latency did not extend a lifetime
    }

    #endregion
}
