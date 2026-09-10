using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// The Module 5 deliverable's database layer: reproduces the duplicate-ticket-number failure through
/// <see cref="TicketCommandService"/> — the same lab prop <c>TicketEditorForm</c>'s
/// <c>chkDuplicateNumber</c> checkbox arms — and proves two things the README and the lesson both promise:
/// the exception really is <c>DbUpdateException</c> wrapping a <c>SqliteException</c> whose message names
/// the unique index, and the sentence the operator would actually see
/// (<see cref="FriendlyDatabaseErrors.TicketSaveRejected"/>) never repeats any of that back to them.
/// </summary>
public sealed class DuplicateNumberTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();
    private readonly TicketCommandService _commands;

    public DuplicateNumberTests()
    {
        _commands = new TicketCommandService(_factory);
    }

    [Fact]
    public async Task SaveAsync_with_forceDuplicateNumber_throws_DbUpdateException_wrapping_the_UNIQUE_constraint()
    {
        await new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();
        int customerId, categoryId;
        await using (var db = _factory.CreateDbContext())
        {
            customerId = (await db.Customers.OrderBy(c => c.Id).FirstAsync()).Id;
            categoryId = (await db.Categories.OrderBy(c => c.Id).FirstAsync()).Id;
        }

        var model = TicketEditModel.NewTicket();
        model.Title = "Duplicate-number lab scenario";
        model.CustomerId = customerId;
        model.CategoryId = categoryId;

        // TicketValidator has already passed this model — every DataAnnotation and the cross-field rule are
        // satisfied. Only the database can catch this one: forceDuplicateNumber: true reuses an existing
        // ticket's Number instead of the next one, the same lab prop chkDuplicateNumber arms.
        var ex = await Assert.ThrowsAsync<DbUpdateException>(
            () => _commands.SaveAsync(model, TimeSpan.Zero, forceDuplicateNumber: true));

        var inner = Assert.IsType<SqliteException>(ex.InnerException);
        Assert.Contains("UNIQUE constraint failed: Tickets.Number", inner.Message);
    }

    [Fact]
    public async Task SaveAsync_without_forceDuplicateNumber_never_collides_with_an_existing_number()
    {
        await new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();
        int customerId, categoryId;
        await using (var db = _factory.CreateDbContext())
        {
            customerId = (await db.Customers.OrderBy(c => c.Id).FirstAsync()).Id;
            categoryId = (await db.Categories.OrderBy(c => c.Id).FirstAsync()).Id;
        }

        var model = TicketEditModel.NewTicket();
        model.Title = "Ordinary save — no lab prop armed";
        model.CustomerId = customerId;
        model.CategoryId = categoryId;

        var result = await _commands.SaveAsync(model, TimeSpan.Zero, forceDuplicateNumber: false);

        Assert.False(string.IsNullOrWhiteSpace(result.Number));
    }

    [Fact]
    public void The_friendly_save_message_contains_no_SQL_constraint_name_or_connection_detail()
    {
        var text = FriendlyDatabaseErrors.TicketSaveRejected;

        Assert.DoesNotContain("SQLite", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SQL", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("constraint", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("UNIQUE", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Data Source", text, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Tickets.Number", text, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose() => _factory.Dispose();
}
