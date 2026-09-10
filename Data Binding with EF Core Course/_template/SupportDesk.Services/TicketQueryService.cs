using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;

namespace SupportDesk.Services;

/// <summary>
/// Read-side service for tickets. It holds only the factory — no context, no entities, no UI
/// state — so it is safe to resolve from any session and any thread.
/// </summary>
public sealed class TicketQueryService
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public TicketQueryService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

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
}
