using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>The three lookups <c>TicketEditorForm</c> needs before it can bind anything, plus the two fixed lists.</summary>
public sealed record EditorLookups(
    IReadOnlyList<LookupItem> Customers,
    IReadOnlyList<LookupItem> Agents,
    IReadOnlyList<LookupItem> Categories,
    IReadOnlyList<string> Statuses,
    IReadOnlyList<string> Priorities);

/// <summary>An existing ticket's edit model plus the ticket number, which the model itself never carries.</summary>
public sealed record TicketEditData(TicketEditModel Model, string Number);

/// <summary>What <c>SaveAsync</c> wrote: the id and number of the saved ticket.</summary>
public sealed record SaveTicketResult(int Id, string Number);

public enum DeleteOutcome
{
    Deleted,
    /// <summary>The row was already gone — another session deleted it first.</summary>
    NotFound,
    /// <summary>The row exists but the business rule below refuses to remove it.</summary>
    Refused
}

public sealed record DeleteTicketResult(DeleteOutcome Outcome, string? Number, string? Reason);

/// <summary>
/// Thrown by <see cref="TicketCommandService.SaveAsync(TicketEditModel, CancellationToken)"/> when the ticket being edited
/// no longer exists: the editor was open on a row another session removed in the meantime.
/// </summary>
public sealed class TicketNotFoundException : Exception
{
    public TicketNotFoundException(int ticketId)
        : base($"Ticket #{ticketId} no longer exists — it was deleted after the editor loaded it.")
    {
        TicketId = ticketId;
    }

    public int TicketId { get; }
}

/// <summary>
/// Write-side service for tickets: everything <c>TicketEditorForm</c> needs to load, save and delete one
/// ticket. Like <see cref="TicketQueryService"/> it holds only the factory — no context, no entities, no UI
/// state — so a fresh <c>DbContext</c> is created for every operation and disposed before the method
/// returns. Module 4's habit in code: load for <i>display</i> with <c>AsNoTracking</c>
/// (<see cref="LoadEditModelAsync"/>), load for a <i>write</i> as a tracked entity inside the very method
/// that is about to change it (<see cref="SaveAsync(TicketEditModel, CancellationToken)"/>,
/// <see cref="DeleteAsync"/>) — the two never share a context, and the tracked read is never held longer
/// than the save it belongs to.
/// </summary>
public sealed class TicketCommandService
{
    /// <summary>The sentinel <see cref="LookupItem.Id"/> the "— unassigned —" row in <c>cboAgent</c> carries. It becomes a null <see cref="TicketEditModel.AgentId"/>.</summary>
    public const int UnassignedAgentId = 0;

    /// <summary>
    /// Tickets whose <see cref="TicketStatuses.Closed"/> status is set cannot be deleted — they are kept
    /// for the record. An arbitrary but sensible rule for a teaching sample: production systems usually
    /// have several such rules (open comments, an active SLA, an audit hold); this is the one Module 4
    /// picks so <see cref="DeleteAsync"/> has a real refusal path to demonstrate, next to the "already
    /// deleted" path.
    /// </summary>
    private const string ClosedCannotBeDeletedMessage = "This ticket is Closed and is kept for the record. Reopen it (change its status and save) before it can be deleted.";

    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public TicketCommandService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    #region Load — for display, never for a write

    /// <summary>
    /// One no-tracking read, mapped into a <see cref="TicketEditModel"/> plus the ticket number (which the
    /// model itself does not carry — see its remarks). The context is disposed before this method returns;
    /// nothing tracked outlives the read, exactly like <see cref="TicketQueryService.SearchTicketsAsync"/>.
    /// </summary>
    public async Task<TicketEditData> LoadEditModelAsync(int id, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        var ticket = await db.Tickets.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, token)
            ?? throw new TicketNotFoundException(id);   // deleted between the grid search and this click: a normal multi-user event, not a crash
        QueryTrace.Note($"LoadEditModelAsync(#{id} / {ticket.Number}): 1 no-tracking read mapped into TicketEditModel");
        return new TicketEditData(ToModel(ticket), ticket.Number);
    }

    /// <summary>
    /// The three lookups the editor's ComboBoxes need — <c>Id</c> + display name only, no tracking — in one
    /// context, three statements. Loaded and assigned to each ComboBox's <c>DataSource</c> <b>before</b> the
    /// edit model becomes <c>editBindingSource.DataSource</c>, the same rule <c>TicketBrowserPage</c>
    /// follows for its filter ComboBoxes: a bound <c>SelectedValue</c> with nothing to select resolves to
    /// nothing.
    /// </summary>
    public async Task<EditorLookups> GetLookupsAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var customers = await db.Customers.AsNoTracking().OrderBy(c => c.Name)
            .Select(c => new LookupItem(c.Id, c.Name)).ToListAsync(token);
        var agents = await db.Agents.AsNoTracking().OrderBy(a => a.DisplayName)
            .Select(a => new LookupItem(a.Id, a.DisplayName)).ToListAsync(token);
        var categories = await db.Categories.AsNoTracking().OrderBy(c => c.Name)
            .Select(c => new LookupItem(c.Id, c.Name)).ToListAsync(token);

        QueryTrace.Note($"editor lookups: {customers.Count} customers, {agents.Count} agents, {categories.Count} categories · 3 statements, 0 tracked");
        return new EditorLookups(customers, agents, categories, TicketStatuses.All, TicketPriorities.All);
    }

    #endregion

    #region Save — a fresh, tracked context for exactly this write

    /// <summary>The ordinary path.</summary>
    public Task<SaveTicketResult> SaveAsync(TicketEditModel model, CancellationToken token = default)
        => SaveAsync(model, forceDuplicateNumber: false, token);


    /// <summary>
    /// Creates or updates one ticket from the approved fields of <paramref name="model"/> and disposes the
    /// context before returning. <paramref name="model"/>.<c>Id</c> == 0 means "new": a fresh
    /// <see cref="Ticket"/> is added with a generated <see cref="Ticket.Number"/>; otherwise the tracked
    /// entity is loaded by key <b>in this context</b> — never the no-tracking copy the editor loaded to
    /// display it — and its fields are overwritten. <c>UpdatedAt</c> and <c>RowVersion</c> are stamped by
    /// <c>SupportDeskContext.SaveChanges(Async)</c>, not here.
    /// </summary>
    /// <param name="forceDuplicateNumber">
    /// Test hook: for a new ticket (<paramref name="model"/>.<c>Id</c> == 0),
    /// reuse an existing ticket's <see cref="Ticket.Number"/> instead of calling <see cref="NextNumberAsync"/>,
    /// so the unique index <c>IX_Tickets_Number</c> refuses the <c>INSERT</c> and <c>SaveChangesAsync</c>
    /// throws <see cref="Microsoft.EntityFrameworkCore.DbUpdateException"/> with an inner <c>SqliteException</c>
    /// "UNIQUE constraint failed: Tickets.Number" — the one failure this module's validator cannot catch in
    /// advance, because uniqueness can only be guaranteed by the database. False in the ordinary path.
    /// </param>
    /// <exception cref="TicketNotFoundException">
    /// <paramref name="model"/>.<c>Id</c> names a ticket that no longer exists — another session removed it after the editor loaded it.
    /// </exception>
    public async Task<SaveTicketResult> SaveAsync(TicketEditModel model, bool forceDuplicateNumber, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        Ticket ticket;
        if (model.Id == 0)
        {
            string number;
            if (forceDuplicateNumber)
            {
                number = await db.Tickets.OrderBy(t => t.Id).Select(t => t.Number).FirstAsync(token);
                QueryTrace.Note($"SaveAsync: reusing existing number {number} to reproduce the UNIQUE constraint on Tickets.Number");
            }
            else
            {
                number = await NextNumberAsync(db, token);
            }

            ticket = new Ticket { Number = number };
            db.Tickets.Add(ticket);
            QueryTrace.Note($"SaveAsync: new ticket {number} — Add, fields mapped from the edit model");
        }
        else
        {
            ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == model.Id, token)
                ?? throw new TicketNotFoundException(model.Id);
            QueryTrace.Note($"SaveAsync: existing ticket {ticket.Number} (#{ticket.Id}) — tracked read, fields mapped from the edit model");
        }

        ticket.Title = model.Title.Trim();
        ticket.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description!.Trim();
        ticket.Status = model.Status;
        ticket.Priority = model.Priority;
        ticket.IsUrgent = model.IsUrgent;
        ticket.DueDate = model.DueDate;
        ticket.CustomerId = model.CustomerId ?? throw new InvalidOperationException("A customer is required to save a ticket.");
        ticket.AgentId = model.AgentId;
        ticket.CategoryId = model.CategoryId ?? throw new InvalidOperationException("A category is required to save a ticket.");
        // Number and CreatedAt are never touched here — TicketEditModel never carried them in the first
        // place. UpdatedAt and RowVersion are stamped by SupportDeskContext.SaveChanges(Async) below (see
        // StampTickets) — this method does not set either one by hand.

        await db.SaveChangesAsync(token);
        return new SaveTicketResult(ticket.Id, ticket.Number);
    }

    #endregion

    #region Delete — confirm first, then a fresh load by key

    /// <summary>
    /// Loads the ticket by key in a fresh context — never trusting the grid row the operator selected — and
    /// either removes it or explains why it did not. Three outcomes, never an unhandled exception for the
    /// two expected ones:
    /// <list type="bullet">
    /// <item><see cref="DeleteOutcome.Deleted"/> — removed and saved.</item>
    /// <item><see cref="DeleteOutcome.NotFound"/> — the row is already gone (another session deleted it). The caller should still treat this as "close and refresh": the
    /// grid is stale and a search will fix it.</item>
    /// <item><see cref="DeleteOutcome.Refused"/> — the row exists but <see cref="ClosedCannotBeDeletedMessage"/> applies.</item>
    /// </list>
    /// </summary>
    public async Task<DeleteTicketResult> DeleteAsync(int id, CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);
        var ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == id, token);

        if (ticket is null)
        {
            QueryTrace.Note($"DeleteAsync(#{id}): not found — already deleted by someone else");
            return new DeleteTicketResult(DeleteOutcome.NotFound, null, "This ticket was already deleted by someone else.");
        }

        if (ticket.Status == TicketStatuses.Closed)
        {
            QueryTrace.Note($"DeleteAsync(#{id} / {ticket.Number}): refused — Status is Closed, DELETE never reaches the database");
            return new DeleteTicketResult(DeleteOutcome.Refused, ticket.Number, ClosedCannotBeDeletedMessage);
        }

        db.Tickets.Remove(ticket);
        await db.SaveChangesAsync(token);
        QueryTrace.Note($"DeleteAsync(#{id} / {ticket.Number}): removed, SaveChangesAsync");
        return new DeleteTicketResult(DeleteOutcome.Deleted, ticket.Number, null);
    }

    #endregion

    private static TicketEditModel ToModel(Ticket ticket) => new()
    {
        Id = ticket.Id,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status,
        Priority = ticket.Priority,
        CustomerId = ticket.CustomerId,
        AgentId = ticket.AgentId,
        CategoryId = ticket.CategoryId,
        DueDate = ticket.DueDate,
        IsUrgent = ticket.IsUrgent
    };

    /// <summary>
    /// <c>SD-NNNN</c>, continuing after the highest existing number <b>in this context</b>. A production
    /// system would use a database sequence (or a unique index plus retry on the constraint violation) so
    /// two simultaneous inserts cannot compute the same next number — scanning every number in application
    /// code has exactly that race, and is a teaching shortcut, not a pattern to keep. The unique index on
    /// <c>Ticket.Number</c> (Module 2) at least turns a collision into a loud <c>DbUpdateException</c>
    /// instead of a silent duplicate.
    /// </summary>
    private static async Task<string> NextNumberAsync(SupportDeskContext db, CancellationToken token)
    {
        var numbers = await db.Tickets.Select(t => t.Number).ToListAsync(token);
        var max = numbers.Count == 0
            ? 1000
            : numbers.Select(n => n.Length > 3 && int.TryParse(n.AsSpan(3), out var v) ? v : 1000).Max();
        return $"SD-{max + 1}";
    }
}
