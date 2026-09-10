# Deliverable 4 · Delete with confirmation, a fresh entity load and graceful handling of already-deleted records

## The business rule

A ticket whose `Status` is `Closed` cannot be deleted — it is kept for the record. This is the rule Module
4 picks (the lab guide asks for "a business rule", without naming one); it is documented here, in
`TicketCommandService.cs`'s `ClosedCannotBeDeletedMessage` constant, and tested. A production system
usually has several such rules (open comments, an active SLA, an audit hold) — this is the one that gives
`DeleteAsync` a real refusal path to demonstrate next to "already deleted".

## TicketEditorForm.DeleteAsync

```csharp
var confirm = await MessageBox.ShowAsync(
    $"Delete ticket {_ticketNumber}? This cannot be undone.",
    "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
if (confirm != DialogResult.Yes) { /* trace: not confirmed, nothing sent */ return; }

using var scope = QueryTrace.Begin(OnQueryTrace);
var result = await Commands.DeleteAsync(id);
switch (result.Outcome)
{
    case DeleteOutcome.Deleted:
        this.DialogResult = DialogResult.OK; Close();
        break;
    case DeleteOutcome.NotFound:
        AlertBox.Show(result.Reason, MessageBoxIcon.Warning, ...);
        this.DialogResult = DialogResult.OK; Close();       // still OK: the grid is stale either way
        break;
    case DeleteOutcome.Refused:
        ShowBanner(result.Reason);                          // dialog stays open
        break;
}
```

## TicketCommandService.DeleteAsync

Never trusts the grid row the operator selected — always a fresh context, loaded by key:

```csharp
await using var db = await _dbFactory.CreateDbContextAsync(token);
var ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == id, token);

if (ticket is null)
    return new DeleteTicketResult(DeleteOutcome.NotFound, null, "This ticket was already deleted by someone else.");

if (ticket.Status == TicketStatuses.Closed)
    return new DeleteTicketResult(DeleteOutcome.Refused, ticket.Number, ClosedCannotBeDeletedMessage);

db.Tickets.Remove(ticket);
await db.SaveChangesAsync(token);
return new DeleteTicketResult(DeleteOutcome.Deleted, ticket.Number, null);
```

Three outcomes, no unhandled exception for the two expected ones (`NotFound`, `Refused`) — only a genuine
database failure (`DatabaseUnavailableException`, `DbUpdateException`) reaches `TicketEditorForm`'s catch
blocks.

## Proving "already deleted" without two browser tabs

`TicketBrowserPage`'s **"Simulate: another operator deletes it"** button calls
`Commands.DeleteAsync(selected.Id)` **directly, bypassing the editor** — exactly as if a second session had
done it — and deliberately does **not** refresh the grid. The stale row stays visible; opening Edit (or
Delete inside the editor) on it afterwards is what shows the already-deleted handling in a single browser
session, without needing a second tab.

## Evidence

- `dotnet build` / `dotnet test` — 0 warnings, 0 errors, 52 passed.
- The real SQL for a successful delete: `SELECT "t"."Id", ... FROM "Tickets" AS "t" WHERE "t"."Id" = @id LIMIT 2`
  then `DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;` — captured from
  `TicketCommandService` running against SQLite in memory, not written from memory.
- `DeleteAsync_removes_an_existing_non_closed_ticket` — the row is gone afterward.
- `DeleteAsync_returns_NotFound_and_sends_only_one_statement_when_the_ticket_is_already_gone` — a second
  `DeleteAsync` on the same id returns `DeleteOutcome.NotFound` with a friendly `Reason`, and the trace
  shows **exactly one statement** (the `SELECT`) — no `DELETE` is ever sent for a row that is not there.
- `DeleteAsync_refuses_a_Closed_ticket_and_sends_no_DELETE` — a `Closed` ticket comes back
  `DeleteOutcome.Refused`, the reason mentions "Closed", the row still exists afterward, and again exactly
  one statement is sent.
- `SaveAsync_throws_TicketNotFoundException_when_the_ticket_was_deleted_since_it_was_loaded` — loading the
  edit model, then deleting the row through the service (as the page's Simulate button would), then saving
  the stale model throws `TicketNotFoundException` carrying the ticket's id — the exception
  `TicketEditorForm.SaveAsync` catches to show "This ticket was already deleted by someone else" and still
  close with `DialogResult.OK`.

**Not verified here — for the browser reviewer.** Whether `MessageBox.ShowAsync` with
`MessageBoxButtons.YesNo` really blocks `DeleteAsync` from running until the operator answers, and that a
`No` (or dismissing the dialog) truly sends nothing; whether the friendly banner and the top-right
`AlertBox` both render for the `Refused` and `NotFound` cases; whether the grid really shows the stale
"deleted by Simulate" row until the next search, and disappears once Edit → Delete (or Edit → Save) on it
closes with `DialogResult.OK`.
