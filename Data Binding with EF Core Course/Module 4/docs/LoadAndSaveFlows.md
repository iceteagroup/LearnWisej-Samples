# Deliverable 3 · LoadEditorAsync for new and existing tickets and SaveAsync with EndEdit, mapping and SaveChangesAsync

## LoadEditorAsync

`TicketEditorForm.LoadEditorAsync` (called from the form's `Load` event) does the same three things for
both cases, in the same order:

1. `Commands.GetLookupsAsync()` — customers, agents (plus the sentinel), categories, `TicketStatuses.All`,
   `TicketPriorities.All`. Assigned to every ComboBox's `DataSource` **before** the model exists.
2. Either `Commands.LoadEditModelAsync(existingId)` (a no-tracking read, mapped into a `TicketEditModel`,
   plus the ticket number) or `TicketEditModel.NewTicket()` (defaults: `Status = Open`, `Priority = Normal`,
   everything else unset).
3. `this.editBindingSource.DataSource = model;` then the manual `dtpDueDate` copy (see
   `docs/ControlDataBindings.md`).

```csharp
TicketEditModel model;
if (_ticketId is int existingId)
{
    var data = await Commands.LoadEditModelAsync(existingId);
    model = data.Model;
    _ticketNumber = data.Number;
    this.Text = $"Edit ticket {_ticketNumber}";
    this.lblNumber.Text = _ticketNumber;
    this.btnDelete.Visible = true;
}
else
{
    model = TicketEditModel.NewTicket();
    this.Text = "Add ticket";
    this.lblNumber.Text = "assigned on save";
    this.btnDelete.Visible = false;
}
this.editBindingSource.DataSource = model;
```

The context `LoadEditModelAsync` opens is disposed before the method returns — the form never holds one.

## SaveAsync

The pipeline, in order, exactly as the lesson names it:

```csharp
if (_saving) { /* guard: log and return */ }
using var scope = QueryTrace.Begin(OnQueryTrace);
try
{
    _saving = true; this.btnSave.Enabled = false; this.btnDelete.Enabled = false;

    this.editBindingSource.EndEdit();                     // 1. commit pending control edits
    var model = (TicketEditModel)this.editBindingSource.Current;
    model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

    this.errorProvider.Clear();                            // 2. the Module 4 validation placeholder
    if (string.IsNullOrWhiteSpace(model.Title))
    {
        this.errorProvider.SetError(this.txtTitle, "Title is required.");
        return;                                             // stopped — no context created
    }

    var result = await Commands.SaveAsync(model, _saveLatency);   // 3. fresh context, map, SaveChangesAsync
    this.DialogResult = DialogResult.OK;
    Close();
}
catch (TicketNotFoundException) { /* friendly message, still closes OK — see docs/DeleteConfirmationAndReload.md */ }
catch (DatabaseUnavailableException ex) { Fail("...not reachable...", ex); }
catch (DbUpdateConcurrencyException ex) { Fail("Someone else changed this ticket...", ex); }
catch (DbUpdateException ex) { Fail("...database rejected the change.", ex); }
catch (Exception ex) { Fail("...try again in a moment.", ex); }
finally { _saving = false; buttons re-enabled; Application.Update(this); }
```

`TicketCommandService.SaveAsync` is the "fresh context, load or create, map, SaveChangesAsync" half:

```csharp
Ticket ticket;
if (model.Id == 0)
{
    var number = await NextNumberAsync(db, token);
    ticket = new Ticket { Number = number };
    db.Tickets.Add(ticket);
}
else
{
    ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == model.Id, token)
        ?? throw new TicketNotFoundException(model.Id);
}
ticket.Title = model.Title.Trim();
ticket.Description = ...; ticket.Status = model.Status; ticket.Priority = model.Priority;
ticket.IsUrgent = model.IsUrgent; ticket.DueDate = model.DueDate;
ticket.CustomerId = model.CustomerId ?? throw ...; ticket.AgentId = model.AgentId;
ticket.CategoryId = model.CategoryId ?? throw ...;
await db.SaveChangesAsync(token);
```

`UpdatedAt` and `RowVersion` are **not** set here — `SupportDeskContext.SaveChanges(Async)`'s
`StampTickets` override does that for every `Added`/`Modified` `Ticket` entry (a Module 2 mechanism,
unchanged since). `Number` and `CreatedAt` are never touched on an update because `TicketEditModel` never
carried them.

## The double-click guard, proved with the slow-save toggle

`_saving` is a form field, checked first in both `SaveAsync` and `DeleteAsync` — the same shape as
`TicketBrowserPage._loading`. `TicketBrowserPage`'s **Slow save (2.5 s)** toggle arms a `TimeSpan` that
`TicketCommandService.SaveAsync` sleeps on **after the context is open and the ticket is mapped, before
`SaveChangesAsync`** — inside the unit of work, exactly like `SearchTicketsSlowlyAsync`'s latency in Module
3:

```csharp
if (latency > TimeSpan.Zero)
{
    QueryTrace.Note($"simulated latency of {latency.TotalSeconds:0.#} s inside the unit of work — the context is already open and Save is guarded");
    await Task.Delay(latency, token);
}
await db.SaveChangesAsync(token);
```

A second click on `btnSave` while that delay is running finds `_saving` already `true` and logs
`• editor guard: save already running — this click is ignored` instead of starting a second save.

## Evidence

- `dotnet build` / `dotnet test` — 0 warnings, 0 errors, 52 passed.
- The real SQL, captured from `TicketCommandService` running against SQLite in memory (a console check —
  the same code path the form calls, not written from memory):
  - Update: `UPDATE "Tickets" SET "RowVersion" = @p0, "Status" = @p1, "Title" = @p2, "UpdatedAt" = @p3 WHERE "Id" = @p4 AND "RowVersion" = @p5 RETURNING 1;`
    preceded by the tracked read `SELECT ... FROM "Tickets" AS "t" WHERE "t"."Id" = @model_Id LIMIT 2` —
    **exactly two statements**, proved by `SaveAsync_update_sends_exactly_two_statements_the_tracked_read_and_the_UPDATE`.
  - Create: `SELECT "t"."Number" FROM "Tickets" AS "t"` (the next-number scan) then
    `INSERT INTO "Tickets" (...) VALUES (...) RETURNING "Id";` — **exactly two statements**, proved by
    `SaveAsync_create_sends_exactly_two_statements_the_number_scan_and_the_INSERT`.
- `SaveAsync_creates_a_new_ticket_with_a_generated_number_and_maps_the_approved_fields` — the new row's
  `Number` is `SD-1313` (the seed's highest is `SD-1312`), every mapped field matches the model, and
  `UpdatedAt`/`RowVersion` are stamped by the context, not by `SaveAsync`.
- `SaveAsync_updates_an_existing_ticket_and_leaves_Number_and_CreatedAt_unchanged` — `Number` and
  `CreatedAt` survive an update untouched; the changed fields do land.
- `SaveAsync_continues_the_next_number_after_the_highest_existing_number` — `SD-1007` and `SD-1500` in the
  database (a gap) produce `SD-1501`, not `SD-1008`: the scan really looks at the highest number, not the
  row count.

**Not verified here — for the browser reviewer.** Whether `editBindingSource.EndEdit()` really commits a
`txtTitle` edit that has not lost focus yet (the mechanism the cookbook's lesson calls out as the reason
Save always starts with `EndEdit`); whether the Save button visibly greys out during the armed 2.5 s delay
and a second real click is dropped with the guard trace line appearing in the page's trace list; whether
`errorProvider.SetError` renders the red indicator next to `txtTitle` when Title is blank.
