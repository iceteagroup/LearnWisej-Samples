using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;
using SupportDesk.Services;
using SupportDesk.Tests.Support;

namespace SupportDesk.Tests;

public sealed class TicketQueryServiceTests : IDisposable
{
    private readonly SqliteTestFactory _factory = new();

    [Fact]
    public async Task CountTicketsAsync_returns_zero_on_an_empty_database()
    {
        var service = new TicketQueryService(_factory);

        Assert.Equal(0, await service.CountTicketsAsync());
    }

    [Fact]
    public async Task CountTicketsAsync_counts_the_rows_that_exist()
    {
        await using (var db = _factory.CreateDbContext())
        {
            db.Tickets.AddRange(
                new Ticket { Number = "T-1001", Title = "Printer offline" },
                new Ticket { Number = "T-1002", Title = "VPN drops" },
                new Ticket { Number = "T-1003", Title = "Password reset" });
            await db.SaveChangesAsync();
        }

        var service = new TicketQueryService(_factory);

        Assert.Equal(3, await service.CountTicketsAsync());
    }

    [Fact]
    public async Task Each_operation_creates_and_disposes_its_own_context()
    {
        var entries = new List<TraceEntry>();
        using var scope = QueryTrace.Begin(entries.Add);
        var service = new TicketQueryService(_factory);

        await service.CountTicketsAsync();
        await service.CountTicketsAsync();

        Assert.Equal(2, scope.ContextsCreated);
        Assert.Equal(2, scope.ContextsDisposed);
        Assert.Equal(2, scope.Commands);
        Assert.All(entries.Where(e => e.Kind == TraceKind.Command), e => Assert.Contains("COUNT(*)", e.Text));
    }

    public void Dispose() => _factory.Dispose();
}
