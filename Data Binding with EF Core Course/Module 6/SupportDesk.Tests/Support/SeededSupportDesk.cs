using SupportDesk.Services;

namespace SupportDesk.Tests.Support;

/// <summary>
/// One seeded in-memory database shared by the read-only ticket-browser tests: the development seeder is
/// deterministic, so all of them see exactly the same 312 tickets. Any test that changes data must build
/// its own <see cref="SqliteTestFactory"/> instead.
/// </summary>
public sealed class SeededSupportDesk : IAsyncLifetime
{
    public SqliteTestFactory Factory { get; } = new();

    public TicketQueryService Tickets { get; private set; } = null!;

    public SeedResult Seed { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Seed = await new DevelopmentSeeder(Factory).SeedDevelopmentDataAsync();
        Tickets = new TicketQueryService(Factory);
    }

    public Task DisposeAsync()
    {
        Factory.Dispose();
        return Task.CompletedTask;
    }
}
