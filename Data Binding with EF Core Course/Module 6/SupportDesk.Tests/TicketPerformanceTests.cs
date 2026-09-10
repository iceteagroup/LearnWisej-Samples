using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 6 deliverable, proved without the UI: the naive branch really does issue one statement per
/// distinct related entity it has not already loaded (not the illustrative "1 + 3 × rows" — see the remarks
/// on <see cref="TicketQueryService.SearchTicketsNaiveAsync"/> for why), the optimised branch still sends
/// exactly two statements and tracks nothing, and the two branches return the same rows in the same order
/// for the same criteria — the whole point of calling this a performance fix and not a behaviour change.
/// </summary>
public sealed class TicketPerformanceTests : IClassFixture<SeededSupportDesk>
{
    private readonly SeededSupportDesk _db;

    public TicketPerformanceTests(SeededSupportDesk db) => _db = db;

    #region The naive branch — tracked, no paging, and exactly one statement per distinct lookup value

    [Fact]
    public async Task The_naive_branch_loads_every_matching_ticket_tracked_and_caps_the_shaped_rows()
    {
        var result = await _db.Tickets.SearchTicketsNaiveAsync(new TicketSearchCriteria());

        Assert.Equal(312, result.TotalMatching);                          // every matching row — no SQL paging
        Assert.Equal(TicketQueryService.NaivePageCap, result.Items.Count); // only the cap is shaped into rows
        Assert.True(result.TrackedEntities >= 312);                       // at least every matching Ticket is tracked
    }

    [Fact]
    public async Task The_naive_branch_statement_count_is_one_plus_the_distinct_lookups_the_capped_rows_touch()
    {
        // What RunSearchNaiveAsync actually does: one statement loads every matching Ticket tracked, then
        // one Reference(...).LoadAsync() per row for Customer/Agent/Category — but EF Core's automatic
        // fix-up among already-tracked entities means only the FIRST occurrence of each distinct
        // Customer/Agent/Category among the capped rows costs a statement; repeats are free. This test
        // computes that expected number independently (from the same 50 rows, by key) instead of hard-coding
        // it, so it stays true if the seed ever changes shape.
        await using var db = _db.Factory.CreateDbContext();
        var top50 = await db.Tickets.AsNoTracking()
            .OrderByDescending(t => t.UpdatedAt)
            .Take(TicketQueryService.NaivePageCap)
            .Select(t => new { t.CustomerId, t.AgentId, t.CategoryId })
            .ToListAsync();

        var distinctLookups =
            top50.Select(t => t.CustomerId).Distinct().Count() +
            top50.Select(t => t.AgentId).Where(id => id != null).Distinct().Count() +
            top50.Select(t => t.CategoryId).Distinct().Count();

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var result = await _db.Tickets.SearchTicketsNaiveAsync(new TicketSearchCriteria());

        Assert.Equal(1 + distinctLookups, scope.Commands);
        Assert.Equal(1, scope.ContextsCreated);
        Assert.Equal(1, scope.ContextsDisposed);
        Assert.Equal(312 + distinctLookups, result.TrackedEntities);
    }

    [Fact]
    public async Task A_reference_load_for_a_null_foreign_key_sends_nothing()
    {
        // The fact the statement-count test above depends on: EF Core resolves a null-FK reference from the
        // tracked graph without a round trip. Verified directly, in isolation, against one unassigned ticket.
        await using var db = _db.Factory.CreateDbContext();
        var unassigned = await db.Tickets.AsNoTracking().FirstAsync(t => t.AgentId == null);
        await using var trackedDb = _db.Factory.CreateDbContext();
        var tracked = await trackedDb.Tickets.SingleAsync(t => t.Id == unassigned.Id);

        using var scope = QueryTrace.Begin(_ => { });
        await trackedDb.Entry(tracked).Reference(x => x.Agent).LoadAsync();

        Assert.Equal(0, scope.Commands);
    }

    #endregion

    #region Both branches agree on the rows — only how they were loaded differs

    [Fact]
    public async Task Naive_and_optimised_return_the_same_rows_in_the_same_order_for_the_same_criteria()
    {
        var criteria = new TicketSearchCriteria { PageIndex = 0, PageSize = 50 };

        var naive = await _db.Tickets.SearchTicketsNaiveAsync(criteria);
        var optimised = await _db.Tickets.SearchTicketsAsync(criteria);

        Assert.Equal(optimised.Items.Select(t => t.Number), naive.Items.Select(t => t.Number));
        Assert.Equal(optimised.Items.Select(t => t.CustomerName), naive.Items.Select(t => t.CustomerName));
        Assert.Equal(optimised.Items.Select(t => t.AgentName), naive.Items.Select(t => t.AgentName));
        Assert.Equal(optimised.Items.Select(t => t.CategoryName), naive.Items.Select(t => t.CategoryName));
    }

    [Fact]
    public async Task The_optimised_branch_still_sends_exactly_two_statements()
    {
        // Tracks-nothing is already proved directly in TicketSearchTests
        // (The_search_tracks_nothing_and_never_returns_an_entity); this test is the head-to-head statement
        // count next to the naive branch's, run with the same criteria.
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        await _db.Tickets.SearchTicketsAsync(new TicketSearchCriteria { PageIndex = 0, PageSize = 50 });

        Assert.Equal(2, scope.Commands);
    }

    [Fact]
    public async Task The_naive_branch_filtered_to_one_customer_still_returns_only_that_customer()
    {
        await using var db = _db.Factory.CreateDbContext();
        var customer = await db.Customers.OrderBy(c => c.Id).FirstAsync();

        using var scope = QueryTrace.Begin(_ => { });
        var result = await _db.Tickets.SearchTicketsNaiveAsync(new TicketSearchCriteria { CustomerId = customer.Id });

        Assert.True(result.TotalMatching > 0);
        Assert.All(result.Items, t => Assert.Equal(customer.Name, t.CustomerName));   // the filter reached the naive branch's Where too — ApplyFilters is shared
        Assert.True(scope.Commands <= 1 + 1 + 3 + 6);   // one ticket load + at most 1 distinct customer + at most 3 agents + at most 6 categories
    }

    #endregion
}
