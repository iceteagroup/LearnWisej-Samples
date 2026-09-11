using Microsoft.EntityFrameworkCore;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

public sealed class DevelopmentSeederTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();

    [Fact]
    public async Task Seed_inserts_five_customers_three_agents_six_categories_and_312_tickets()
    {
        var seeder = new DevelopmentSeeder(_factory);

        var result = await seeder.SeedDevelopmentDataAsync();

        Assert.True(result.Seeded);
        Assert.Equal(5, result.Customers);
        Assert.Equal(3, result.Agents);
        Assert.Equal(6, result.Categories);
        Assert.Equal(312, result.Tickets);            // seven pages of fifty, plus twelve
        Assert.True(result.Comments > 0);

        await using var db = _factory.CreateDbContext();
        Assert.Equal(5, await db.Customers.CountAsync());
        Assert.Equal(3, await db.Agents.CountAsync());
        Assert.Equal(6, await db.Categories.CountAsync());
        Assert.Equal(result.Tickets, await db.Tickets.CountAsync());
        Assert.Equal(result.Comments, await db.TicketComments.CountAsync());
    }

    [Fact]
    public async Task Seed_data_is_realistic_and_mixed()
    {
        await new DevelopmentSeeder(_factory).SeedDevelopmentDataAsync();

        await using var db = _factory.CreateDbContext();
        var tickets = await db.Tickets.AsNoTracking().ToListAsync();

        Assert.Equal(tickets.Count, tickets.Select(t => t.Number).Distinct().Count());       // unique numbers
        Assert.Equal(new[] { "Closed", "In Progress", "Open", "Resolved", "Waiting" }, tickets.Select(t => t.Status).Distinct().OrderBy(s => s));
        Assert.Equal(new[] { "High", "Low", "Normal" }, tickets.Select(t => t.Priority).Distinct().OrderBy(p => p));
        Assert.Contains(tickets, t => t.DueDate is not null);
        Assert.Contains(tickets, t => t.DueDate is null);

        var unassigned = tickets.Count(t => t.AgentId is null);
        Assert.InRange(unassigned / (double)tickets.Count, 0.2, 0.4);                        // ~30 % unassigned
        Assert.All(tickets, t => Assert.True(t.Title.Length is > 0 and <= 180));
        Assert.All(tickets, t => Assert.Equal(16, t.RowVersion.Length));                     // the token was stamped on insert
    }

    [Fact]
    public async Task Seed_is_idempotent_a_second_call_adds_nothing()
    {
        var seeder = new DevelopmentSeeder(_factory);
        var first = await seeder.SeedDevelopmentDataAsync();

        var entries = new List<TraceEntry>();
        SeedResult second;
        using (var scope = QueryTrace.Begin(entries.Add))
        {
            second = await seeder.SeedDevelopmentDataAsync();
            Assert.Equal(1, scope.ContextsCreated);
            Assert.Equal(1, scope.ContextsDisposed);
        }

        Assert.False(second.Seeded);
        Assert.Equal(0, second.Tickets);
        Assert.Equal(first.Tickets, second.ExistingTickets);
        Assert.Contains("nothing to seed", second.Describe());
        Assert.DoesNotContain(entries, e => e.Kind == TraceKind.Command && e.Text.StartsWith("INSERT", StringComparison.Ordinal));

        await using var db = _factory.CreateDbContext();
        Assert.Equal(first.Tickets, await db.Tickets.CountAsync());
    }

    public void Dispose() => _factory.Dispose();
}
