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
}
