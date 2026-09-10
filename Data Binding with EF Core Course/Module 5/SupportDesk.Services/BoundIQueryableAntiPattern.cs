using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>
/// Deliberately wrong code, kept out of the real services and named for what it is: what happens when a
/// handler assigns the <b>query</b> to the UI instead of the <b>list</b>
/// (<c>ticketBindingSource.DataSource = db.Tickets.Where(...)</c> — or, worse, <c>= db.Tickets</c>).
/// </summary>
/// <remarks>
/// <para>
/// The assignment itself looks harmless: an <see cref="IQueryable{T}"/> is only a recipe, so nothing is
/// sent and the handler returns happily. The damage arrives later. <c>await using</c> disposes the context
/// the moment the handler returns, and the grid asks its data source to enumerate afterwards, while it is
/// painting rows — at which point the recipe tries to open a connection on a context that no longer
/// exists. EF Core answers with <see cref="ObjectDisposedException"/> ("Cannot access a disposed context
/// instance"), thrown from inside the UI layer, in a stack that contains none of the data code.
/// </para>
/// <para>
/// This method reproduces that sequence for real — compose, dispose, then enumerate — instead of
/// describing it. Even if the context happened to still be alive, the grid would re-run the whole query on
/// every scroll, sort and repaint, with no <c>Skip</c>/<c>Take</c> and every row tracked. The fix is one
/// line: <c>ToListAsync</c> before the assignment.
/// </para>
/// </remarks>
public sealed class BoundIQueryableAntiPattern
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public BoundIQueryableAntiPattern(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Builds the query, disposes the context the way the handler's <c>await using</c> would, then
    /// enumerates it the way a bound grid would. Throws <see cref="ObjectDisposedException"/> (or
    /// <see cref="InvalidOperationException"/> on providers that report it that way); the caller catches
    /// and explains. If it ever returns, the sentence it returns says the demo failed to fail.
    /// </summary>
    public async Task<string> BindTheQueryAndLetTheGridEnumerateAsync(CancellationToken token = default)
    {
        IQueryable<TicketListItem> boundQuery;

        var db = await _dbFactory.CreateDbContextAsync(token);
        try
        {
            // What the wrong handler writes: no ToListAsync anywhere.
            boundQuery = db.Tickets
                .OrderByDescending(t => t.UpdatedAt)
                .Select(t => new TicketListItem(
                    t.Id, t.Number, t.Title, t.Customer.Name,
                    t.Agent == null ? null : t.Agent.DisplayName,
                    t.Category.Name, t.Status, t.Priority, t.DueDate, t.UpdatedAt));

            QueryTrace.Note("ticketBindingSource.DataSource = <IQueryable> — the assignment sends nothing: no COUNT, no SELECT, no rows. The handler looks like it worked");
        }
        finally
        {
            // The handler returns; `await using` disposes the unit of work, exactly as it should.
            await db.DisposeAsync();
        }

        QueryTrace.Note("the handler has returned and the context is disposed — now the grid enumerates its data source to paint the first rows (this is where the real query would run)");

        // The grid's enumeration, reproduced.
        var rows = boundQuery.ToList();

        return $"the disposed context still answered with {rows.Count} rows — the anti-pattern did not fail this time";
    }
}
