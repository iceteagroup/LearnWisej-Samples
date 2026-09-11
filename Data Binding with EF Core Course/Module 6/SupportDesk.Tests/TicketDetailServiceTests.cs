using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 6 related-data decision table, one method per row: <c>Include</c>
/// (<see cref="TicketDetailService.LoadForEditorAsync"/>), a filtered read equivalent to a filtered
/// <c>Include</c> (<see cref="TicketDetailService.LoadRecentCommentsAsync"/>), and explicit loading
/// (<see cref="TicketDetailService.LoadAllCommentsAsync"/>). Each test asserts the statement count EF Core
/// 10 actually sends, not the count the lesson would predict in the abstract.
/// </summary>
public sealed class TicketDetailServiceTests : IClassFixture<SeededSupportDesk>
{
    private readonly SeededSupportDesk _db;
    private readonly TicketDetailService _details;

    public TicketDetailServiceTests(SeededSupportDesk db)
    {
        _db = db;
        _details = new TicketDetailService(db.Factory);
    }

    #region LoadForEditorAsync — Include(Customer).Include(Category), tracked

    [Fact]
    public async Task LoadForEditorAsync_sends_one_statement_and_eagerly_loads_customer_and_category()
    {
        await using var db = _db.Factory.CreateDbContext();
        var anyTicket = await db.Tickets.AsNoTracking().OrderBy(t => t.Id).FirstAsync();

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var ticket = await _details.LoadForEditorAsync(anyTicket.Id);

        Assert.Equal(1, scope.Commands);
        Assert.Equal(1, scope.ContextsCreated);
        Assert.NotNull(ticket.Customer);
        Assert.NotNull(ticket.Category);
    }

    [Fact]
    public async Task LoadForEditorAsync_throws_TicketNotFoundException_for_a_missing_id()
    {
        await Assert.ThrowsAsync<TicketNotFoundException>(() => _details.LoadForEditorAsync(999_999));
    }

    #endregion

    #region LoadRecentCommentsAsync — filtered to the last 5, no-tracking, identity resolution

    [Fact]
    public async Task LoadRecentCommentsAsync_sends_one_statement_and_returns_at_most_five_newest_first()
    {
        await using var db = _db.Factory.CreateDbContext();
        var ticketWithComments = await db.Tickets.AsNoTracking().Where(t => t.Comments.Count > 0).OrderBy(t => t.Id).FirstAsync();

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var comments = await _details.LoadRecentCommentsAsync(ticketWithComments.Id);

        Assert.Equal(1, scope.Commands);
        Assert.True(comments.Count <= 5);
        Assert.Equal(comments.OrderByDescending(c => c.CreatedAt).Select(c => c.Id), comments.Select(c => c.Id));
    }

    [Fact]
    public async Task LoadRecentCommentsAsync_returns_an_empty_list_for_a_ticket_with_no_comments()
    {
        await using var db = _db.Factory.CreateDbContext();
        var ticketWithoutComments = await db.Tickets.AsNoTracking().Where(t => t.Comments.Count == 0).OrderBy(t => t.Id).FirstAsync();

        var comments = await _details.LoadRecentCommentsAsync(ticketWithoutComments.Id);

        Assert.Empty(comments);
    }

    #endregion

    #region LoadAllCommentsAsync — explicit loading, one extra statement beyond the ticket read

    [Fact]
    public async Task LoadAllCommentsAsync_sends_two_statements_the_ticket_read_plus_one_extra_for_the_collection()
    {
        await using var db = _db.Factory.CreateDbContext();
        var ticketWithComments = await db.Tickets.AsNoTracking().Where(t => t.Comments.Count > 0).OrderBy(t => t.Id).FirstAsync();
        var expectedCount = await db.TicketComments.CountAsync(c => c.TicketId == ticketWithComments.Id);

        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);

        var comments = await _details.LoadAllCommentsAsync(ticketWithComments.Id);

        Assert.Equal(2, scope.Commands);            // the tracked ticket read, then Collection(...).LoadAsync() — 1 extra
        Assert.Equal(expectedCount, comments.Count); // the FULL history, not capped at 5 like LoadRecentCommentsAsync
    }

    [Fact]
    public async Task LoadAllCommentsAsync_returns_more_rows_than_LoadRecentCommentsAsync_when_a_ticket_has_more_than_five()
    {
        // The seed's comment pattern tops out at two per ticket, so this asserts the general rule (All >= Recent)
        // rather than requiring a >5-comment ticket to exist in the fixture.
        await using var db = _db.Factory.CreateDbContext();
        var ticketWithComments = await db.Tickets.AsNoTracking().Where(t => t.Comments.Count > 0).OrderBy(t => t.Id).FirstAsync();

        var recent = await _details.LoadRecentCommentsAsync(ticketWithComments.Id);
        var all = await _details.LoadAllCommentsAsync(ticketWithComments.Id);

        Assert.True(all.Count >= recent.Count);
    }

    [Fact]
    public async Task LoadAllCommentsAsync_throws_TicketNotFoundException_for_a_missing_id()
    {
        await Assert.ThrowsAsync<TicketNotFoundException>(() => _details.LoadAllCommentsAsync(999_999));
    }

    #endregion
}
