# Schema constraints are not UI validation

The fourth objective of Module 2: keep the layer that tells the *database* what it must guarantee apart
from the layer that tells the *user* what to fix. The page proves the split with one button — a
200-character title — and the result is a red banner, not an "Enter a shorter title" hint, because the
hint does not exist yet. That is the point.

## The two layers

| | Schema constraints (this module) | UI / domain validation (Module 5) |
|---|---|---|
| Written in | `SupportDeskContext.OnModelCreating` | the edit model, `TicketValidator`, `ErrorProvider` |
| Enforced by | the database, on `SaveChanges` | the editor, before anything is sent |
| Timing | after a round-trip | as the user types or leaves the field |
| Failure looks like | `DbUpdateException` + a provider message | a red marker next to the field and one sentence |
| Can express | nullability, length, uniqueness, foreign keys, the concurrency token | "closed tickets cannot have a future due date", "pick a category", "this number is already in use, try SD-1061" |
| Audience | the developer and the operator | the user |

Both are needed. The database constraint is the *last* line of defence — it survives a bug in the
editor, a second application talking to the same schema, and a script somebody runs by hand. The UI
validation is the *first* one, and it is the only one a user should ever meet.

## The 200-character title, end to end

**1 · The mapping says 180.**

```csharp
public const int TitleMaxLength = 180;
...
b.Property(x => x.Title).HasMaxLength(TitleMaxLength).IsRequired();
```

On SQL Server that produces `nvarchar(180) NOT NULL` and the engine refuses anything longer.

**2 · On SQLite, `HasMaxLength` is metadata only.** SQLite's `TEXT` has no declared length; the migration
faithfully records `maxLength: 180` in the C# operation, but the emitted DDL is just `"Title" TEXT NOT
NULL`. EF Core does not add a client-side length check either — `HasMaxLength` is a *mapping* facet, and
`SaveChanges` would happily send 200 characters. Without one more rule this sample would teach the
opposite of what it means to.

**3 · So the model adds a CHECK constraint.**

```csharp
b.ToTable(t => t.HasCheckConstraint("CK_Tickets_Title_Length", $"length(\"Title\") <= {TitleMaxLength}"));
```

which lands in the `CREATE TABLE`:

```sql
CONSTRAINT "CK_Tickets_Title_Length" CHECK (length("Title") <= 180)
```

The constant is shared, so the mapping, the constraint and Module 5's validator cannot drift apart.
`IsRequired()` needs no such help: `NOT NULL` is a real SQLite constraint.

**4 · The insert fails at the database.** `ModelDemoService.SaveOverlongTitleAsync` builds a realistic
200-character sentence, adds the ticket and saves. SQLite raises `SQLITE_CONSTRAINT_CHECK` (error code
19) and EF Core wraps it:

```
DbUpdateException
  └─ SqliteException: SQLite Error 19: 'CHECK constraint failed: CK_Tickets_Title_Length'.
```

**5 · The handler translates it.** `RunAsync` catches `DbUpdateException` and shows the message the
handler passed in — never the provider text:

```csharp
catch (DbUpdateException ex)
{
    Fail(dbUpdateMessage, ex);      // "The ticket could not be saved because the database rejected the change."
}
```

`Fail` sets `statusLabel` to *Not completed — nothing was changed*, shows the red banner, turns the state
chip red, and writes the **real** exception into the trace with its inner type and first sentence — the
developer's copy of the failure, next to the SQL that produced it. In a production application that line
is an `ILogger.LogError` and the banner is all the user gets.

Note the ordering of the catch blocks: `DbUpdateConcurrencyException` is caught **before**
`DbUpdateException` (it derives from it), so the stale-token path gets its own sentence instead of the
generic one.

**6 · Nothing was changed.** The failed `INSERT` leaves the ticket in the change tracker as `Added`, the
`await using` disposes the context anyway, and the entity goes with it. The row count on the **Model &
migration card** is identical before and after — which is why the card refreshes in `finally`, on the
failure path too.

## What Module 5 adds on top

The same 200-character title, once the editor exists:

- the `TextBox` has a `MaxLength` so the 181st character cannot be typed;
- `TicketValidator` checks the edit model on `btnSave` and `errorProvider.SetError(txtTitle, "Title must
  be 180 characters or fewer.")` marks the field;
- the save is never attempted, so there is no round-trip and no exception;
- and the CHECK constraint is still there, silently, for the case none of the above ran.

The rules that have **no** schema equivalent arrive at the same time: "a closed ticket cannot have a
future due date", "an urgent ticket needs an agent". No column constraint can express those, which is
the clearest argument that the two layers are different things rather than one thing written twice.

The duplicate ticket number is the same story with a different constraint: the unique index refuses it
today with *UNIQUE constraint failed: Tickets.Number*, and Module 5 turns that into "SD-1042 already
exists" before the save is sent.

## Evidence

**Save a 200-char title (fails)** — `buttonOverlongTitle`:

```
• buttonOverlongTitle_Click ModelDemoService.SaveOverlongTitleAsync() — a 200-character title against HasMaxLength(180) + CK_Tickets_Title_Length
◦ context      #5 created (SupportDeskContext from the factory)
→ SQL          SELECT "c"."Id", "c"."Email", "c"."Name" FROM "Customers" AS "c" ORDER BY "c"."Id" LIMIT 1   (0.3 ms)
→ SQL          SELECT "c"."Id", "c"."Name" FROM "Categories" AS "c" ORDER BY "c"."Id" LIMIT 1   (0.1 ms)
• service      INSERT a ticket with a 200-character title (limit 180) — SQLite ignores HasMaxLength, the CHECK constraint decides
→ SQL failed   SqliteException: SQLite Error 19: 'CHECK constraint failed: CK_Tickets_Title_Length'. — INSERT INTO "Tickets" ("AgentId", "CategoryId", "CreatedAt", "CustomerId", "Description", "DueDate", "IsUrgent", "Number…
◦ context      #5 disposed (3 tracked entities released)
• caught       DbUpdateException → SqliteException: SQLite Error 19: 'CHECK constraint failed: CK_Tickets_Title_Length'. → friendly message shown, full exception logged server-side
```

On screen: the red banner *The ticket could not be saved because the database rejected the change.*,
`statusLabel` → *Not completed — nothing was changed*, the state chip red (`● fault`), and the ticket
count on the card unchanged. The provider message appears exactly once, in the trace, where a developer
is meant to read it.

The card shows the constraint itself, read from the design-time model rather than hard-coded:

```
checks      Tickets: CK_Tickets_Title_Length = length("Title") <= 180
```

(`SchemaInfoService` asks `IDesignTimeModel` for this — the read-optimised runtime model drops check
constraints, so the runtime `db.Model` would report none.)

Tests (`SupportDesk.Tests`):

- `ModelRulesTests.A_200_character_title_is_refused_by_the_check_constraint` — `DbUpdateException` with
  an inner `SqliteException` containing *CHECK constraint failed: CK_Tickets_Title_Length*, and the
  generated title really is 200 characters.
- `ModelRulesTests.Duplicate_ticket_number_violates_the_unique_index` — the other schema-level refusal,
  *UNIQUE constraint failed: Tickets.Number*.
- `ModelRulesTests.The_model_carries_the_planned_indexes_delete_behaviours_and_check_constraint` —
  `CK_Tickets_Title_Length` is in the model metadata.
- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — the
  constraint text is read back out of `sqlite_master` on a database built by the migration.
- `DevelopmentSeederTests.Seed_data_is_realistic_and_mixed` — every seeded title is within the limit, so
  the constraint never fires on the happy path.
