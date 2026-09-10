using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

/// <summary>
/// What the schema rules configured in SupportDeskContext.OnModelCreating really do on SQLite:
/// Restrict / SetNull / Cascade, the RowVersion token, the unique ticket number and the title CHECK.
/// The in-memory database is created with EnsureCreated (same model as the migration); foreign keys are
/// enforced because Microsoft.Data.Sqlite turns PRAGMA foreign_keys on by default.
/// </summary>
public sealed class ModelRulesTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();

    private async Task SeedAsync() => await new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();

    [Fact]
    public async Task Foreign_keys_are_enforced_on_the_test_connection()
    {
        await using var db = _factory.CreateDbContext();
        await using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "PRAGMA foreign_keys;";
        await db.Database.OpenConnectionAsync();

        Assert.Equal(1L, (long)(await command.ExecuteScalarAsync())!);
    }

    [Fact]
    public async Task Restrict_deleting_a_customer_with_tickets_throws_DbUpdateException_and_changes_nothing()
    {
        await SeedAsync();
        var service = new ModelDemoService(_factory);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => service.DeleteCustomerWithTicketsAsync());

        var sqlite = Assert.IsType<SqliteException>(ex.InnerException);
        Assert.Equal(19, sqlite.SqliteErrorCode);                                   // SQLITE_CONSTRAINT
        Assert.Contains("FOREIGN KEY constraint failed", sqlite.Message);

        await using var db = _factory.CreateDbContext();
        Assert.Equal(5, await db.Customers.CountAsync());
    }

    [Fact]
    public async Task SetNull_deleting_an_agent_unassigns_their_tickets()
    {
        await SeedAsync();
        int agentId, owned;
        await using (var db = _factory.CreateDbContext())
        {
            var agent = await db.Agents.OrderBy(a => a.Id).FirstAsync();
            agentId = agent.Id;
            owned = await db.Tickets.CountAsync(t => t.AgentId == agentId);
            Assert.True(owned > 0);
        }

        var result = await new ModelDemoService(_factory).UnassignAgentByDeletingAsync();

        Assert.Equal(owned, result.After - result.Before);
        await using (var db = _factory.CreateDbContext())
        {
            Assert.Equal(2, await db.Agents.CountAsync());
            Assert.Equal(0, await db.Tickets.CountAsync(t => t.AgentId == agentId));
            Assert.Equal(result.After, await db.Tickets.CountAsync(t => t.AgentId == null));
        }
    }

    [Fact]
    public async Task Cascade_deleting_a_ticket_removes_its_comments()
    {
        await SeedAsync();
        int ticketId, comments;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.Where(t => t.Comments.Any()).OrderBy(t => t.Id).FirstAsync();
            ticketId = ticket.Id;
            comments = await db.TicketComments.CountAsync(c => c.TicketId == ticketId);
            Assert.True(comments > 0);
        }

        var result = await new ModelDemoService(_factory).DeleteTicketWithCommentsAsync();

        Assert.Equal(comments, result.Before - result.After);
        await using (var db = _factory.CreateDbContext())
        {
            Assert.Null(await db.Tickets.FindAsync(ticketId));
            Assert.Equal(0, await db.TicketComments.CountAsync(c => c.TicketId == ticketId));
        }
    }

    [Fact]
    public async Task RowVersion_is_stamped_on_insert_and_changes_on_update_together_with_UpdatedAt()
    {
        await SeedAsync();
        byte[] first;
        DateTime updatedBefore;
        int id;
        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.OrderBy(t => t.Id).FirstAsync();
            id = ticket.Id;
            first = ticket.RowVersion;
            updatedBefore = ticket.UpdatedAt;
            Assert.Equal(16, first.Length);

            ticket.Title = "Printer offline on floor 2 — escalated";
            await db.SaveChangesAsync();
        }

        await using (var db = _factory.CreateDbContext())
        {
            var ticket = await db.Tickets.FindAsync(id);
            Assert.NotNull(ticket);
            Assert.Equal(16, ticket!.RowVersion.Length);
            Assert.NotEqual(first, ticket.RowVersion);
            Assert.True(ticket.UpdatedAt > updatedBefore);
        }
    }

    [Fact]
    public async Task Stale_RowVersion_original_value_makes_SaveChangesAsync_throw_DbUpdateConcurrencyException()
    {
        await SeedAsync();
        await using var db = _factory.CreateDbContext();
        var ticket = await db.Tickets.OrderBy(t => t.Id).FirstAsync();

        ticket.Title = "Edited by an agent who loaded the row a minute ago";
        var staleBytes = Guid.NewGuid().ToByteArray();                                 // a token the row never had
        db.Entry(ticket).Property(x => x.RowVersion).OriginalValue = staleBytes;

        var ex = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db.SaveChangesAsync());
        Assert.Single(ex.Entries);
        Assert.Same(ticket, ex.Entries[0].Entity);
    }

    [Fact]
    public async Task Two_contexts_the_second_save_of_the_same_ticket_throws()
    {
        await SeedAsync();
        await using var first = _factory.CreateDbContext();
        await using var second = _factory.CreateDbContext();
        var a = await first.Tickets.OrderBy(t => t.Id).FirstAsync();
        var b = await second.Tickets.OrderBy(t => t.Id).FirstAsync();

        // The edited property must differ from what the seed wrote, or EF Core detects no change, sends no
        // UPDATE and stamps no new token — and the test would then pass for the wrong reason.
        a.Title = "First writer wins";
        await first.SaveChangesAsync();                                                 // wins: token matches

        b.Title = "Second writer loses";
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());   // loses: stale token

        await using var check = _factory.CreateDbContext();
        Assert.Equal("First writer wins", (await check.Tickets.FindAsync(a.Id))!.Title);
    }

    [Fact]
    public async Task Duplicate_ticket_number_violates_the_unique_index()
    {
        await SeedAsync();
        await using var db = _factory.CreateDbContext();
        var customer = await db.Customers.FirstAsync();
        var category = await db.Categories.FirstAsync();
        var existing = await db.Tickets.Select(t => t.Number).OrderBy(n => n).FirstAsync();

        db.Tickets.Add(TestData.Ticket(customer, category, existing, "Duplicate number"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        var sqlite = Assert.IsType<SqliteException>(ex.InnerException);
        Assert.Contains("UNIQUE constraint failed: Tickets.Number", sqlite.Message);
    }

    [Fact]
    public async Task A_200_character_title_is_refused_by_the_check_constraint()
    {
        await SeedAsync();
        var service = new ModelDemoService(_factory);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => service.SaveOverlongTitleAsync());

        var sqlite = Assert.IsType<SqliteException>(ex.InnerException);
        Assert.Contains("CHECK constraint failed: CK_Tickets_Title_Length", sqlite.Message);
        Assert.Equal(200, ModelDemoService.OverlongTitle(200).Length);
    }

    [Fact]
    public async Task The_model_carries_the_planned_indexes_delete_behaviours_and_check_constraint()
    {
        var info = await new SchemaInfoService(_factory).DescribeAsync();

        var ticketIndexes = info.Indexes.Where(i => i.Table == "Tickets").ToList();
        Assert.Contains(ticketIndexes, i => i.Columns.SequenceEqual(new[] { "Status", "DueDate" }));
        Assert.Contains(ticketIndexes, i => i.Columns.SequenceEqual(new[] { "CustomerId" }));
        Assert.Contains(ticketIndexes, i => i.Columns.SequenceEqual(new[] { "UpdatedAt" }));
        Assert.Contains(ticketIndexes, i => i.Columns.SequenceEqual(new[] { "Number" }) && i.IsUnique);
        Assert.Contains(info.Indexes, i => i.Table == "Customers" && i.Columns.SequenceEqual(new[] { "Name" }));

        Assert.Equal(DeleteBehavior.Restrict, info.Relationships.Single(r => r.ForeignKeyColumn == "CustomerId").DeleteBehavior);
        Assert.Equal(DeleteBehavior.SetNull, info.Relationships.Single(r => r.ForeignKeyColumn == "AgentId").DeleteBehavior);
        Assert.Equal(DeleteBehavior.Restrict, info.Relationships.Single(r => r.ForeignKeyColumn == "CategoryId").DeleteBehavior);
        Assert.Equal(DeleteBehavior.Cascade, info.Relationships.Single(r => r.ForeignKeyColumn == "TicketId").DeleteBehavior);

        Assert.Contains(info.CheckConstraints, c => c.Name == "CK_Tickets_Title_Length");
        Assert.Empty(info.AppliedMigrations);                                           // EnsureCreated: no history table
    }

    public void Dispose() => _factory.Dispose();
}
