# DbUpdateException: a friendly message for a duplicate ticket number

**Deliverable:** `DbUpdateException` handled with a friendly message for a duplicate ticket number.

## Why this cannot be caught by `TicketValidator`

Every rule in [`ValidationLayers.md`](ValidationLayers.md) is a fact about the model in isolation, or a fact
about two of its own fields (the closed/future-due-date rule). Uniqueness of `Ticket.Number` is a fact about
*every other row in the table at the instant of the write* — no amount of client-side checking can promise
that, because another save could land between the check and the write. The unique index
`IX_Tickets_Number` (Module 2) is the only thing that can actually guarantee it, and it does so by refusing
the `INSERT` and making EF Core throw `DbUpdateException` from `SaveChangesAsync`.

## Reproducing it on demand: the lab prop

`TicketCommandService.SaveAsync` gained a `forceDuplicateNumber` parameter:

```csharp
public async Task<SaveTicketResult> SaveAsync(TicketEditModel model, TimeSpan latency, bool forceDuplicateNumber, CancellationToken token = default)
{
    …
    if (model.Id == 0)
    {
        string number = forceDuplicateNumber
            ? await db.Tickets.OrderBy(t => t.Id).Select(t => t.Number).FirstAsync(token)   // lab prop: reuse one
            : await NextNumberAsync(db, token);                                             // ordinary path
        ticket = new Ticket { Number = number };
        …
```

`TicketEditorForm` has a matching development-only lab control, `chkDuplicateNumber` ("Lab: reuse an existing
ticket number (forces a duplicate save)"), visible only for a **new** ticket (it means nothing on an edit —
`forceDuplicateNumber` is only read when `model.Id == 0`). `SaveAsync` passes it straight through:

```csharp
var result = await Commands.SaveAsync(model, _saveLatency, this.chkDuplicateNumber.Checked && model.Id == 0);
```

The **"Add: duplicate number (DbUpdateException)"** button on the browser page (`btnLabDuplicateNumber_Click`)
opens the editor with `EditorLabScenario.DuplicateNumber`: a valid Title, a valid Customer and Category (so
`TicketValidator` passes and Save actually reaches the database), and `chkDuplicateNumber` pre-ticked.

## The catch, below the concurrency catch

```csharp
catch (DbUpdateConcurrencyException ex) { Fail(…); }     // must come first — it derives from DbUpdateException
catch (DbUpdateException ex)               { await FailDbUpdateAsync(ex); }
```

`FailDbUpdateAsync`:

1. **Logs the full exception server-side** — `Console.Error.WriteLine($"[SupportDesk] server log: DbUpdateException saving ticket — {ex}")`,
   the whole exception (including `ex.InnerException`, the `SqliteException`), never trimmed.
2. **Shows one plain sentence** — `FriendlyDatabaseErrors.TicketSaveRejected` ("The ticket could not be saved
   because the database rejected the change. Please verify required fields and lookup values."), through both
   `labelBanner` (the form's own banner) and `AlertBox.Show(…, MessageBoxIcon.Error, alignment: TopRight, autoCloseDelay: 4000)`.
   No SQL, no constraint name, no connection string — `FriendlyDatabaseErrors` is a small, UI-free static class
   in `SupportDesk.Services` precisely so a test can assert that directly (see [`NegativeTests.md`](NegativeTests.md)).
3. **Reloads the lookup lists** — `ReloadLookupsAsync()` re-fetches `Commands.GetLookupsAsync()` and refills
   `cboCustomer`/`cboAgent`/`cboCategory`, because a stale foreign key (a customer or category deleted while
   the dialog was open) is another plausible cause of the same exception, not just a duplicate number.

The dialog **stays open** — nothing about a caught `DbUpdateException` closes it, the same rule Module 4
established for every caught failure — so the operator's typed values are not lost; unticking the lab checkbox
and pressing Save again succeeds.

## The trace

Because `SaveAsync` wraps the whole operation in `QueryTrace.Begin(OnQueryTrace)`, the failed `INSERT` is
reported by the same `QueryTraceInterceptor.CommandFailed` hook every other SQL statement in this course goes
through — captured on this machine:

```
→ SQL failed   SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'. — INSERT INTO "Tickets" …
```

followed by the editor's own line:

```
• editor       caught DbUpdateException → friendly message shown, full exception logged server-side
```

## Evidence

- `dotnet build` — 0 warnings, 0 errors.
- `SupportDesk.Tests/DuplicateNumberTests.cs`:
  - `SaveAsync_with_forceDuplicateNumber_throws_DbUpdateException_wrapping_the_UNIQUE_constraint` — asserts
    the thrown exception is `DbUpdateException`, its `InnerException` is `Microsoft.Data.Sqlite.SqliteException`,
    and that inner exception's `Message` contains `UNIQUE constraint failed: Tickets.Number`. Console probe of
    the actual text on this machine: `SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'.`
  - `SaveAsync_without_forceDuplicateNumber_never_collides_with_an_existing_number` — the ordinary path still
    saves normally when the lab prop is off.
  - `The_friendly_save_message_contains_no_SQL_constraint_name_or_connection_detail` — asserts
    `FriendlyDatabaseErrors.TicketSaveRejected` contains none of `SQLite`, `SQL`, `constraint`, `UNIQUE`,
    `Data Source` or `Tickets.Number` (case-insensitive).
