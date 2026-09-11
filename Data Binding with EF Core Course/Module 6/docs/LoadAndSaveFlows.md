# Deliverable 3 · LoadEditorAsync for new and existing tickets and SaveAsync with EndEdit, mapping and SaveChangesAsync

## LoadEditorAsync

`TicketEditorForm.LoadEditorAsync` (called from the form's `Load` event) does the same three things for
both cases, in the same order:

1. `Commands.GetLookupsAsync()` — customers, agents (plus the "— unassigned —" sentinel row), categories,
   `TicketStatuses.All`, `TicketPriorities.All`. Assigned to every ComboBox's `DataSource` **before** the
   model exists (`FillLookups`, then `cboStatus`/`cboPriority` as `NamedValue` rows).
2. Either `Commands.LoadEditModelAsync(existingId)` (a no-tracking read, mapped into a `TicketEditModel`,
   plus the ticket number) or `TicketEditModel.NewTicket()` (defaults: `Status = Open`, `Priority = Normal`,
   everything else unset).
3. `this.editBindingSource.DataSource = model;`, then `BindLookupControls()` and the manual `dtpDueDate`
   copy (see `docs/ControlDataBindings.md`). Finally the validator decides whether Save starts enabled
   (see `docs/CrossFieldRuleAndSaveGating.md`).

```csharp
TicketEditModel model;
if (_ticketId is int existingId)
{
    var data = await Commands.LoadEditModelAsync(existingId);
    model = data.Model;
    _ticketNumber = data.Number;
    this.Text = $"Edit Ticket — {_ticketNumber}";
    this.btnDelete.Visible = true;
}
else
{
    model = TicketEditModel.NewTicket();
    _ticketNumber = null;
    this.Text = "Add Ticket";
    this.btnDelete.Visible = false;
}
this.editBindingSource.DataSource = model;
BindLookupControls();
```

The context `LoadEditModelAsync` opens is disposed before the method returns — the form never holds one.

## SaveAsync

The pipeline, in order, exactly as the lesson names it:

```csharp
if (_saving)
    return;                                                 // guard: a second click is dropped

try
{
    _saving = true; this.btnSave.Enabled = false; this.btnDelete.Enabled = false;

    this.errorProvider.Clear();
    this.editBindingSource.EndEdit();                       // 1. commit pending control edits
    var model = (TicketEditModel)this.editBindingSource.Current;
    model.DueDate = this.dtpDueDate.Checked ? this.dtpDueDate.Value.Date : (DateTime?)null;

    var messages = RunValidation(paint: true);              // 2. TicketValidator (Module 5)
    if (messages.Count > 0)
        return;                                             // stopped — no context created

    await Commands.SaveAsync(model);                        // 3. fresh context, map, SaveChangesAsync
    this.DialogResult = DialogResult.OK;
    Close();
}
catch (TicketNotFoundException)         { /* warning AlertBox, still closes OK — see docs/DeleteConfirmationAndReload.md */ }
catch (DbUpdateConcurrencyException ex) { /* Modules 5–6: an error AlertBox; Module 7: the conflict dialog */ }
catch (DbUpdateException ex)            { ShowError(FriendlyDatabaseErrors.TicketSaveRejected, ex); await ReloadLookupsAsync(); }
catch (Exception ex)                    { ShowError("The ticket could not be saved. Please try again in a moment.", ex); }
finally { _saving = false; RunValidation(paint: false); this.btnDelete.Enabled = true; Application.Update(this); }
```

`ShowError` writes the full exception to the server console and shows the sentence in a top-right error
`AlertBox`; the dialog stays open, so nothing the operator typed is lost.

`TicketCommandService.SaveAsync` is the "fresh context, load or create, map, SaveChangesAsync" half.
`SaveAsync(model)` calls `SaveAsync(model, forceDuplicateNumber: false)`; the flag exists for
`DuplicateNumberTests` (see `docs/DuplicateNumberHandling.md`). Module 7 adds a third overload,
`SaveAsync(model, forceDuplicateNumber, overwriteOriginalRowVersion)`, and restores the model's
`RowVersion` as the original value before mapping (see `docs/ConcurrencyResolution.md`). The ordinary path:

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

## The double-click guard

`_saving` is a form field, checked first in both `SaveAsync` and `DeleteAsync` — the same shape as
`TicketBrowserPage._loading`. Save and Delete are disabled while it is set; `finally` clears it and lets
`RunValidation` decide whether Save comes back enabled. A second click on `btnSave` while a save is still
running finds `_saving` already `true` and returns without starting a second save.

## Evidence

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
Save always starts with `EndEdit`); whether the Save button visibly greys out while a save runs and a second
real click is dropped; whether `errorProvider.SetError` renders the red indicator next to the field that
failed.
