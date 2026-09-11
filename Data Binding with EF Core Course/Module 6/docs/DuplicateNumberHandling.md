# DbUpdateException: a friendly message for a duplicate ticket number

**Deliverable:** `DbUpdateException` handled with a friendly message for a duplicate ticket number.

## Why this cannot be caught by `TicketValidator`

Every rule in [`ValidationLayers.md`](ValidationLayers.md) is a fact about the model in isolation, or a fact
about two of its own fields (the closed/future-due-date rule). Uniqueness of `Ticket.Number` is a fact about
*every other row in the table at the instant of the write* — no amount of client-side checking can promise
that, because another save could land between the check and the write. The unique index
`IX_Tickets_Number` (Module 2) is the only thing that can actually guarantee it, and it does so by refusing
the `INSERT` and making EF Core throw `DbUpdateException` from `SaveChangesAsync`.

## Reproducing it: the `forceDuplicateNumber` parameter

The editor always saves through `SaveAsync(model)`, which numbers a new ticket with `NextNumberAsync` and
never collides in normal use. To reproduce the collision on demand, `TicketCommandService` has an overload
with a `forceDuplicateNumber` parameter, used by `DuplicateNumberTests`:

```csharp
public async Task<SaveTicketResult> SaveAsync(TicketEditModel model, bool forceDuplicateNumber, CancellationToken token = default)
{
    …
    if (model.Id == 0)
    {
        string number;
        if (forceDuplicateNumber)
            number = await db.Tickets.OrderBy(t => t.Id).Select(t => t.Number).FirstAsync(token);   // reuse one
        else
            number = await NextNumberAsync(db, token);                                             // ordinary path
        ticket = new Ticket { Number = number };
        …
```

`forceDuplicateNumber` is only read when `model.Id == 0`; an edit never changes `Number`. The screen has no
control for it: the same `DbUpdateException` also covers a stale foreign key (a customer or category
deleted while the dialog was open), and the handling below is identical for both causes.

## The catch, below the concurrency catch

```csharp
catch (DbUpdateConcurrencyException ex) { … }      // must come first — it derives from DbUpdateException
catch (DbUpdateException ex)
{
    // A duplicate ticket number or a stale foreign key: the database's final word.
    ShowError(FriendlyDatabaseErrors.TicketSaveRejected, ex);
    await ReloadLookupsAsync();
}
```

1. **Logs the full exception server-side** — `ShowError` writes `Console.Error.WriteLine("[SupportDesk] " + ex)`,
   the whole exception (including `ex.InnerException`, the `SqliteException`), never trimmed.
2. **Shows one plain sentence** — `FriendlyDatabaseErrors.TicketSaveRejected` ("The ticket could not be saved
   because the database rejected the change. Please verify required fields and lookup values.") in an error
   `AlertBox` (`MessageBoxIcon.Error`, top-right, `autoCloseDelay: 4000`). No SQL, no constraint name, no
   connection string — `FriendlyDatabaseErrors` is a small, UI-free static class in `SupportDesk.Services`
   precisely so a test can assert that directly (see [`NegativeTests.md`](NegativeTests.md)).
3. **Reloads the lookup lists** — `ReloadLookupsAsync()` re-fetches `Commands.GetLookupsAsync()` and refills
   `cboCustomer`/`cboAgent`/`cboCategory`, because a stale foreign key is another plausible cause of the same
   exception, not just a duplicate number.

The dialog **stays open** — nothing about a caught `DbUpdateException` closes it, the same rule Module 4
established for every caught failure — so the operator's typed values are not lost and Save can be pressed
again.

## Evidence

- `SupportDesk.Tests/DuplicateNumberTests.cs`:
  - `SaveAsync_with_forceDuplicateNumber_throws_DbUpdateException_wrapping_the_UNIQUE_constraint` — asserts
    the thrown exception is `DbUpdateException`, its `InnerException` is `Microsoft.Data.Sqlite.SqliteException`,
    and that inner exception's `Message` contains `UNIQUE constraint failed: Tickets.Number`. Console probe of
    the actual text on this machine: `SQLite Error 19: 'UNIQUE constraint failed: Tickets.Number'.`
  - `SaveAsync_without_forceDuplicateNumber_never_collides_with_an_existing_number` — the ordinary path still
    saves normally.
  - `The_friendly_save_message_contains_no_SQL_constraint_name_or_connection_detail` — asserts
    `FriendlyDatabaseErrors.TicketSaveRejected` contains none of `SQLite`, `SQL`, `constraint`, `UNIQUE`,
    `Data Source` or `Tickets.Number` (case-insensitive).
