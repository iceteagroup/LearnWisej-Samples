# RowVersion and the planned indexes

Deliverables 2 and 3 of the Module 2 lab: the concurrency token on `Ticket`, and one index per filter or
sort the ticket browser will issue.

## The concurrency token

Two agents open ticket SD-1042 in the queue. Both edit it, both press Save a second apart. Without a
token the second save overwrites the first silently and nobody ever learns that work was lost. With a
token the second save updates zero rows and fails loudly, and the UI can offer reload, overwrite or merge
(Module 7 builds that dialog).

```csharp
b.Property(x => x.RowVersion).IsRowVersion();
b.Property(x => x.RowVersion).Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Save);
b.Property(x => x.RowVersion).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
```

`IsRowVersion()` does two things: it marks the property as a **concurrency token**, and it sets
`ValueGeneratedOnAddOrUpdate`. The first half is what matters — EF Core adds the property's *original*
value to the `WHERE` clause of every `UPDATE` and `DELETE` of that entity, and if the statement reports
zero affected rows, `SaveChanges` throws `DbUpdateConcurrencyException`.

## Why SQLite needs the two extra lines and the stamping override

On SQL Server, `IsRowVersion()` maps to the `rowversion` column type: the **server** bumps an 8-byte value
on every write, EF Core reads it back after the save, and nothing has to be written by hand.

SQLite has no `rowversion` type. The migration produces `"RowVersion" BLOB NOT NULL` and nothing ever
fills it — the column would stay at its default and every row would carry the same token, which is the
same as having no token at all. Two changes make it real:

1. The save behaviours are switched to `Save`. A `ValueGeneratedOnAddOrUpdate` property is normally
   *excluded* from `INSERT` and `UPDATE` and read back from the database instead; with `Save` the value
   the application sets is actually sent.
2. The context writes the value itself, in the `SaveChanges` overrides:

```csharp
private void StampTickets()
{
    var now = DateTime.UtcNow;
    foreach (var entry in ChangeTracker.Entries<Ticket>())
    {
        switch (entry.State)
        {
            case EntityState.Added:
                entry.Property(t => t.RowVersion).CurrentValue = NewToken();
                break;
            case EntityState.Modified:
                entry.Property(t => t.RowVersion).CurrentValue = NewToken();
                entry.Property(t => t.UpdatedAt).CurrentValue = now;
                break;
        }
    }
}

private static byte[] NewToken() => Guid.NewGuid().ToByteArray();
```

Only `CurrentValue` is touched. The **original** value — the one that was read from the database when the
entity was loaded — is left alone, which is precisely what EF Core puts in the `WHERE` clause. The same
override stamps `UpdatedAt` on every modified ticket, so the "recently changed" sort never depends on a
handler remembering to set it.

The result on the wire, captured from the trace:

```sql
UPDATE "Tickets" SET "RowVersion" = @p0, "UpdatedAt" = @p1
 WHERE "Id" = @p2 AND "RowVersion" = @p3
RETURNING 1;
```

`@p0` is the new token, `@p3` is the token the row had when it was loaded. A `DELETE` carries the same
condition: `DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;`.

| | SQL Server | SQLite (this sample) |
|---|---|---|
| Column type | `rowversion` (8 bytes) | `BLOB` (16 bytes here — a `Guid`) |
| Who sets the value | the database engine, on every write | `SupportDeskContext.SaveChanges` / `SaveChangesAsync` |
| Save behaviours | left as `IsRowVersion()` sets them | forced to `Save` on both sides |
| In the `UPDATE` | `WHERE Id = @p AND RowVersion = @p` | identical |
| On a stale token | 0 rows → `DbUpdateConcurrencyException` | identical |

The important part is that **the failure mode is the same**. Everything Modules 4 and 7 build on top —
`db.Entry(ticket).Property(t => t.RowVersion).OriginalValue = model.RowVersion`, the conflict dialog,
reload versus overwrite — behaves the same way on either provider. An alternative configuration with the
same result is `.IsConcurrencyToken().ValueGeneratedNever()` plus the same override; `IsRowVersion()` was
chosen because it is what a SQL Server model would say, and the difference then lives in one commented
block instead of in the mapping.

`SaveChanges` (sync) is overridden alongside `SaveChangesAsync` on purpose: a stamping rule that only
applies on one of the two is a bug waiting for the first synchronous call site.

## The planned indexes

Every index answers a query the ticket browser is going to issue. None of them is speculative.

| Index | Columns | Serves |
|---|---|---|
| `IX_Tickets_Status_DueDate` | `Status`, `DueDate` | the queue: `WHERE Status = @s ORDER BY DueDate` — one composite index covers the filter and the sort in that order |
| `IX_Tickets_CustomerId` | `CustomerId` | the customer filter of the browser (and the foreign key lookup) |
| `IX_Tickets_UpdatedAt` | `UpdatedAt` | the "recently changed" sort |
| `IX_Tickets_Number` | `Number` **unique** | ticket numbers identify a ticket to a human; the unique index is what refuses a duplicate (Module 5's duplicate path) |
| `IX_Customers_Name` | `Name` | the customer ComboBox's ordering and the customer name filter |
| `IX_Tickets_AgentId` | `AgentId` | created by EF Core for the foreign key |
| `IX_Tickets_CategoryId` | `CategoryId` | created by EF Core for the foreign key |
| `IX_TicketComments_TicketId` | `TicketId` | created by EF Core for the foreign key; also the detail load under a ticket |

Column order in the composite index is a decision, not a formality: `(Status, DueDate)` supports
`WHERE Status = …` on its own and `WHERE Status = … ORDER BY DueDate` together, while `(DueDate, Status)`
would help neither of the browser's two queries. Module 6 comes back to this with measurements.

The unique index is the only one that changes behaviour rather than speed: a second ticket with the same
`Number` fails the `INSERT` with `SQLITE_CONSTRAINT_UNIQUE` — *UNIQUE constraint failed: Tickets.Number* —
inside a `DbUpdateException`, exactly the way the CHECK constraint refuses an overlong title.

In the migration they are plain `CreateIndex` calls, and in the SQL script:

```sql
CREATE INDEX        "IX_Customers_Name"        ON "Customers" ("Name");
CREATE INDEX        "IX_Tickets_CustomerId"    ON "Tickets" ("CustomerId");
CREATE UNIQUE INDEX "IX_Tickets_Number"        ON "Tickets" ("Number");
CREATE INDEX        "IX_Tickets_Status_DueDate" ON "Tickets" ("Status", "DueDate");
CREATE INDEX        "IX_Tickets_UpdatedAt"     ON "Tickets" ("UpdatedAt");
```

## Evidence

The **Model & migration card** lists them straight from the model metadata after every operation:

```
indexes     Customers: IX_Customers_Name (Name)
            TicketComments: IX_TicketComments_TicketId (TicketId)
            Tickets: IX_Tickets_AgentId (AgentId) · IX_Tickets_CategoryId (CategoryId) · IX_Tickets_CustomerId (CustomerId) · IX_Tickets_Number (Number) UNIQUE · IX_Tickets_UpdatedAt (UpdatedAt) · IX_Tickets_Status_DueDate (Status, DueDate)
```

The token is visible in the trace of every write path. **Delete a ticket with comments** sends the
condition:

```
→ SQL          DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;   (0.2 ms)
```

and a stale token — what the second agent's save produces — comes back as:

```
◦ context      #9 created (SupportDeskContext from the factory)
→ SQL          SELECT "t"."Id", … FROM "Tickets" AS "t" ORDER BY "t"."Id" LIMIT 1   (0.3 ms)
→ SQL          UPDATE "Tickets" SET "RowVersion" = @p0, "UpdatedAt" = @p1 WHERE "Id" = @p2 AND "RowVersion" = @p3 RETURNING 1;   (0.2 ms)
◦ context      #9 disposed (1 tracked entity released)
• caught       DbUpdateConcurrencyException: The database operation was expected to affect 1 row(s), but actually affected 0 row(s); data may have been modified or deleted since entities were loaded. → friendly message shown, full exception logged server-side
```

with the banner *Someone else changed this row in the meantime. Nothing was saved — reload and try
again.* — `RunAsync` catches `DbUpdateConcurrencyException` before the general `DbUpdateException`, so
this path already has its own message a whole module before the conflict dialog exists.

Tests (`SupportDesk.Tests`):

- `ModelRulesTests.RowVersion_is_stamped_on_insert_and_changes_on_update_together_with_UpdatedAt` — the
  token is 16 bytes after the insert, a different 16 bytes after the update, and `UpdatedAt` moved.
- `ModelRulesTests.Stale_RowVersion_original_value_makes_SaveChangesAsync_throw_DbUpdateConcurrencyException`
  — sets `OriginalValue` to a token the row never had; one entry on the exception, and it is the ticket.
- `ModelRulesTests.Two_contexts_the_second_save_of_the_same_ticket_throws` — the realistic version: two
  contexts read the same ticket, the first save wins, the second throws, and the database keeps the
  winner's value.
- `ModelRulesTests.Duplicate_ticket_number_violates_the_unique_index` — *UNIQUE constraint failed:
  Tickets.Number* inside `DbUpdateException`.
- `ModelRulesTests.The_model_carries_the_planned_indexes_delete_behaviours_and_check_constraint` — asserts
  `(Status, DueDate)`, `(CustomerId)`, `(UpdatedAt)`, unique `(Number)` and `Customers (Name)`.
- `DevelopmentSeederTests.Seed_data_is_realistic_and_mixed` — every seeded ticket carries a 16-byte token,
  so the stamping ran on the `INSERT` path too.
