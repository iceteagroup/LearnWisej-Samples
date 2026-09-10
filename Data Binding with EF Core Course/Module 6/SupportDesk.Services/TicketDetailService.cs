using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>One comment, shaped for the editor's read-only comments list — never a tracked <see cref="TicketComment"/>.</summary>
public sealed record CommentSummary(int Id, string Author, string Body, DateTime CreatedAt);

/// <summary>
/// Module 6's related-data decision table, in code: three ways to bring a ticket's related data into the
/// editor, each chosen for what the screen is about to do with it — never lazy loading, which does not
/// exist anywhere in this solution (<c>UseLazyLoadingProxies</c> is never added; see the remarks on
/// <see cref="TicketQueryService.SearchTicketsNaiveAsync"/> for what turning it on would have hidden).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="LoadForEditorAsync"/> uses <c>Include</c>: the editor needs one controlled aggregate — the
/// ticket plus its <see cref="Ticket.Customer"/> and <see cref="Ticket.Category"/> — and nothing about that
/// shape changes size as the database grows, so eager loading with the root is the right, and cheapest,
/// choice: one statement, one round trip.
/// </para>
/// <para>
/// <see cref="LoadRecentCommentsAsync"/> needs only a small, ordered subset of a collection that can be
/// arbitrarily large over a ticket's lifetime — loading every comment just to show five would be its own
/// small anti-pattern. A filtered read (<c>Where</c> + <c>OrderByDescending</c> + <c>Take(5)</c> against
/// <see cref="SupportDeskContext.TicketComments"/> directly, equivalent to a filtered
/// <c>Include(t =&gt; t.Comments.OrderByDescending(c =&gt; c.CreatedAt).Take(5))</c> on the ticket) is the
/// right shape; see its remarks for why it also uses <c>AsNoTrackingWithIdentityResolution</c> instead of
/// plain <c>AsNoTracking</c>.
/// </para>
/// <para>
/// <see cref="LoadAllCommentsAsync"/> is <b>explicit loading, on demand</b>: the operator has to click
/// "Show full history" before this method ever runs. That is exactly the case explicit loading is for — a
/// detail the screen does not need until the user asks for it, loaded inside the same tracked read that
/// found the ticket (<c>db.Entry(ticket).Collection(t =&gt; t.Comments).LoadAsync()</c>), one extra
/// statement beyond the ticket read itself.
/// </para>
/// <para>
/// <b>Projection is deliberately the wrong answer for the editor.</b> The ticket browser (Module 3) projects
/// straight to <see cref="TicketListItem"/> because the grid is read-only and never writes anything back.
/// The editor is not read-only — <see cref="TicketCommandService.SaveAsync(TicketEditModel, TimeSpan, bool, CancellationToken)"/>
/// writes a tracked <see cref="Ticket"/> back through the very same kind of context — so the ticket itself
/// has to be loadable as a real, trackable entity, which a flat DTO can never be. What Module 6 changes is
/// only which <i>related</i> data comes along for the ride, and how much of it, never the ticket's own
/// shape.
/// </para>
/// </remarks>
public sealed class TicketDetailService
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public TicketDetailService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// <c>Include</c>: the ticket plus its <see cref="Ticket.Customer"/> and <see cref="Ticket.Category"/>,
    /// tracked, in one statement — a controlled aggregate, not a whole graph (Agent and Comments are left
    /// unloaded; nothing here needs them).
    /// </summary>
    public async Task<Ticket> LoadForEditorAsync(int id, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var ticket = await db.Tickets
            .Include(t => t.Customer)
            .Include(t => t.Category)
            .SingleOrDefaultAsync(t => t.Id == id, token)
            ?? throw new TicketNotFoundException(id);

        QueryTrace.Note($"TicketDetailService.LoadForEditorAsync(#{id}): Include(Customer).Include(Category), tracked — 1 statement, a controlled aggregate (Agent and Comments stay unloaded)");
        return ticket;
    }

    /// <summary>
    /// The last five comments, newest first — a filtered read, not the whole collection. Equivalent to a
    /// filtered <c>Include</c> on <see cref="Ticket.Comments"/>, written directly against
    /// <see cref="SupportDeskContext.TicketComments"/> so the query also demonstrates
    /// <c>AsNoTrackingWithIdentityResolution</c> on its <c>Include(c =&gt; c.Ticket)</c> back-reference: up
    /// to five rows here all carry a reference to the <b>same</b> ticket, and plain <c>AsNoTracking</c> would
    /// materialise a separate clone of it for every one of them. Identity resolution reuses the one instance
    /// already materialised for the first row instead — verified with a console probe: five comment rows,
    /// one shared <c>Ticket</c> instance, not five. The saving is modest at five rows for one ticket; it is
    /// the right choice specifically because the same call shape (join many rows back to far fewer parents)
    /// is exactly what a wider "recent activity across the queue" query would do at much larger scale, where
    /// the difference between five separate clones and one shared instance becomes real memory. A plain
    /// projection (no <c>Include</c> at all) would not need this — <c>AsNoTracking</c> alone is right for a
    /// query that returns no entities.
    /// </summary>
    public async Task<IReadOnlyList<CommentSummary>> LoadRecentCommentsAsync(int id, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var comments = await db.TicketComments
            .AsNoTrackingWithIdentityResolution()
            .Include(c => c.Ticket)
            .Where(c => c.TicketId == id)
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .ToListAsync(token);

        QueryTrace.Note($"TicketDetailService.LoadRecentCommentsAsync(#{id}): filtered to the last 5, no-tracking with identity resolution, Include(Ticket) — 1 statement, {comments.Count} row(s)");
        return comments.Select(c => new CommentSummary(c.Id, c.Author, c.Body, c.CreatedAt)).ToList();
    }

    /// <summary>
    /// Explicit loading, on demand: only runs when the operator clicks "Show full history" in the editor's
    /// comments panel. A tracked read of the ticket by key, then <c>db.Entry(ticket).Collection(t =&gt;
    /// t.Comments).LoadAsync()</c> <b>inside the same context</b> — one extra statement beyond the ticket
    /// read itself, not a second context. Chosen over a separate stand-alone query on purpose: the two
    /// reads belong to the same "show me this ticket's full history" operation, and the tracked ticket read
    /// is nearly free (its own key is already known), so there is no reason to pay for a second context
    /// only to avoid one extra statement on the one already open.
    /// </summary>
    public async Task<IReadOnlyList<CommentSummary>> LoadAllCommentsAsync(int id, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == id, token)
            ?? throw new TicketNotFoundException(id);

        await db.Entry(ticket).Collection(t => t.Comments).LoadAsync(token);

        QueryTrace.Note($"TicketDetailService.LoadAllCommentsAsync(#{id}): tracked ticket read + explicit Collection(Comments).LoadAsync, same context — 2 statements (1 extra beyond the ticket read), {ticket.Comments.Count} row(s)");
        return ticket.Comments.OrderByDescending(c => c.CreatedAt).Select(c => new CommentSummary(c.Id, c.Author, c.Body, c.CreatedAt)).ToList();
    }
}
