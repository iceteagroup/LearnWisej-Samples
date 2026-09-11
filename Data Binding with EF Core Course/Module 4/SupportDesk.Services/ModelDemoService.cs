using Microsoft.EntityFrameworkCore;
using SupportDesk.Data;
using SupportDesk.Data.Diagnostics;

namespace SupportDesk.Services;

/// <summary>Outcome of one model demo: which row was touched and the counts before and after.</summary>
public sealed record ModelDemoResult(string Subject, int Before, int After, string Summary);

/// <summary>
/// Lab props for Module 2: each method runs one deliberate mutation through one short-lived context so
/// the page can show what the schema rules do — the CHECK constraint refusing a 200-character title, the
/// Restrict rule refusing a customer delete, SetNull unassigning tickets, Cascade removing comments.
/// The two "refused" methods are expected to throw <see cref="DbUpdateException"/>; the caller shows a
/// friendly message. Nothing here belongs in a real command service.
/// </summary>
public sealed class ModelDemoService
{
    private readonly IDbContextFactory<SupportDeskContext> _dbFactory;

    public ModelDemoService(IDbContextFactory<SupportDeskContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    /// <summary>
    /// Saves a ticket whose title is 200 characters long. HasMaxLength(180) alone is not enforced by SQLite,
    /// so the CHECK constraint CK_Tickets_Title_Length does the refusing: SQLITE_CONSTRAINT_CHECK inside a
    /// <see cref="DbUpdateException"/>. The context is disposed with the failed insert still tracked — and
    /// then gone, because the context is gone.
    /// </summary>
    public async Task SaveOverlongTitleAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var customer = await db.Customers.OrderBy(c => c.Id).FirstOrDefaultAsync(token)
            ?? throw new InvalidOperationException("No customers yet — seed the development data first.");
        var category = await db.Categories.OrderBy(c => c.Id).FirstOrDefaultAsync(token)
            ?? throw new InvalidOperationException("No categories yet — seed the development data first.");

        var title = OverlongTitle(200);
        QueryTrace.Note($"INSERT a ticket with a {title.Length}-character title (limit {SupportDeskContext.TitleMaxLength}) — SQLite ignores HasMaxLength, the CHECK constraint decides");

        db.Tickets.Add(new Ticket
        {
            Number = $"SD-{DateTime.UtcNow:HHmmssfff}",
            Title = title,
            Status = "Open",
            Priority = "Normal",
            CustomerId = customer.Id,
            CategoryId = category.Id
        });
        await db.SaveChangesAsync(token);
    }

    /// <summary>
    /// Removes the first customer that still has tickets. DeleteBehavior.Restrict became
    /// <c>ON DELETE RESTRICT</c> in the schema, so the DELETE fails with SQLITE_CONSTRAINT_FOREIGNKEY
    /// inside a <see cref="DbUpdateException"/>. Nothing is changed.
    /// </summary>
    public async Task DeleteCustomerWithTicketsAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var target = await db.Customers
            .Where(c => c.Tickets.Any())
            .OrderBy(c => c.Id)
            .Select(c => new { Customer = c, TicketCount = c.Tickets.Count })
            .FirstOrDefaultAsync(token)
            ?? throw new InvalidOperationException("No customer has tickets — seed the development data first.");

        QueryTrace.Note($"customer #{target.Customer.Id} '{target.Customer.Name}' has {target.TicketCount} tickets — DELETE goes to the database, ON DELETE RESTRICT decides");

        db.Customers.Remove(target.Customer);
        await db.SaveChangesAsync(token);
    }

    /// <summary>
    /// Deletes the first agent that has tickets. Their tickets are NOT loaded into the context, so the
    /// change tracker cannot fix anything up: the database's <c>ON DELETE SET NULL</c> clears AgentId.
    /// Returns the number of unassigned tickets before and after.
    /// </summary>
    public async Task<ModelDemoResult> UnassignAgentByDeletingAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var target = await db.Agents
            .Where(a => a.Tickets.Any())
            .OrderBy(a => a.Id)
            .Select(a => new { Agent = a, TicketCount = a.Tickets.Count })
            .FirstOrDefaultAsync(token);

        var unassignedBefore = await db.Tickets.CountAsync(t => t.AgentId == null, token);
        if (target is null)
        {
            QueryTrace.Note("no agent has tickets left — nothing to delete");
            return new ModelDemoResult("—", unassignedBefore, unassignedBefore, "no agent has tickets left");
        }

        QueryTrace.Note($"agent #{target.Agent.Id} '{target.Agent.DisplayName}' owns {target.TicketCount} tickets · {unassignedBefore} tickets unassigned before — DELETE goes to the database, ON DELETE SET NULL decides");

        db.Agents.Remove(target.Agent);
        await db.SaveChangesAsync(token);

        var unassignedAfter = await db.Tickets.CountAsync(t => t.AgentId == null, token);
        return new ModelDemoResult(target.Agent.DisplayName, unassignedBefore, unassignedAfter,
            $"agent '{target.Agent.DisplayName}' deleted · unassigned tickets {unassignedBefore} → {unassignedAfter} (+{unassignedAfter - unassignedBefore}, set to NULL by the database)");
    }

    /// <summary>
    /// Deletes the first ticket that has comments. The comments are not loaded; the database's
    /// <c>ON DELETE CASCADE</c> removes them. Returns the comment count before and after.
    /// </summary>
    public async Task<ModelDemoResult> DeleteTicketWithCommentsAsync(CancellationToken token = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(token);

        var target = await db.Tickets
            .Where(t => t.Comments.Any())
            .OrderBy(t => t.Id)
            .Select(t => new { Ticket = t, CommentCount = t.Comments.Count })
            .FirstOrDefaultAsync(token);

        var commentsBefore = await db.TicketComments.CountAsync(token);
        if (target is null)
        {
            QueryTrace.Note("no ticket has comments left — nothing to delete");
            return new ModelDemoResult("—", commentsBefore, commentsBefore, "no ticket has comments left");
        }

        QueryTrace.Note($"ticket {target.Ticket.Number} '{target.Ticket.Title}' has {target.CommentCount} comments · {commentsBefore} comments in total — DELETE goes to the database (WHERE Id AND RowVersion), ON DELETE CASCADE decides");

        db.Tickets.Remove(target.Ticket);
        await db.SaveChangesAsync(token);

        var commentsAfter = await db.TicketComments.CountAsync(token);
        return new ModelDemoResult(target.Ticket.Number, commentsBefore, commentsAfter,
            $"ticket {target.Ticket.Number} deleted · comments {commentsBefore} → {commentsAfter} (−{commentsBefore - commentsAfter}, cascaded by the database)");
    }

    /// <summary>A realistic sentence stretched to exactly <paramref name="length"/> characters.</summary>
    public static string OverlongTitle(int length)
    {
        const string sentence = "Printer on floor 2 is offline again after the firmware update and the whole accounts team cannot print invoices. ";
        var text = string.Concat(Enumerable.Repeat(sentence, length / sentence.Length + 1));
        return text[..length];
    }
}
