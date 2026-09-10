# The model and the delete behaviours

Deliverable 1 of the Module 2 lab: `Customer`, `Agent`, `Category`, `Ticket` and `TicketComment` with
relationships and a deliberate `OnDelete` rule on each one.

## The entity graph

```
Customer 1 ──< Ticket >── 1 Category          (both required: CustomerId, CategoryId are int)
Agent    0..1 ──< Ticket                      (optional: AgentId is int?)
Ticket   1 ──< TicketComment                  (the detail collection)
```

| Entity | Role | Fields |
|---|---|---|
| `Customer` | lookup | `Id`, `Name`, `Email?`, `Tickets` |
| `Agent` | lookup | `Id`, `DisplayName`, `Email?`, `Tickets` |
| `Category` | lookup | `Id`, `Name`, `Tickets` |
| `Ticket` | the aggregate the screens work on | `Id`, `Number`, `Title`, `Description?`, `Status`, `Priority`, `IsUrgent`, `DueDate?`, `CreatedAt`, `UpdatedAt`, `CustomerId`/`Customer`, `AgentId?`/`Agent`, `CategoryId`/`Category`, `Comments`, `RowVersion` |
| `TicketComment` | detail row | `Id`, `TicketId`/`Ticket`, `Body`, `Author`, `CreatedAt` |

## Why this shape binds well

- **A predictable integer key on every entity.** A grid row, a `BindingSource.Current` and a ComboBox
  `ValueMember` all need one stable identity, and `int` is what the editor will carry between the grid,
  the dialog and the save.
- **Small lookups with one display field.** `Customer.Name`, `Agent.DisplayName`, `Category.Name` are
  `DisplayMember` material: `cbo.DisplayMember = "Name"; cbo.ValueMember = "Id";` and nothing else has to
  be computed on the client.
- **`AgentId` is `int?`.** A ticket exists before anyone owns it, so the agent ComboBox can offer
  "Unassigned" as a real state instead of a sentinel row with `Id = -1`.
- **Navigation properties in both directions.** `Ticket.Comments` is the master-detail collection Module 4
  loads under the editor; `Customer.Tickets` is what the `Restrict` rule below is about, and it is also
  what lets a query say `db.Customers.Where(c => c.Tickets.Any())` without a join written by hand.
- **`CreatedAt` / `UpdatedAt` on the ticket.** `UpdatedAt` is the "recently changed" sort of the browser
  and is stamped by the context on every update (see [RowVersionAndIndexes.md](RowVersionAndIndexes.md)).
- **No mapping attributes on the classes.** Every schema rule is in `SupportDeskContext.OnModelCreating`,
  so the entity stays a plain class and never doubles as the UI validation contract
  (see [SchemaVsUiValidation.md](SchemaVsUiValidation.md)).

## The Fluent rules (`SupportDesk.Data/SupportDeskContext.cs`)

Lengths and required flags, all in `OnModelCreating`:

| Entity | Property | Rule |
|---|---|---|
| `Customer` | `Name` | `HasMaxLength(160).IsRequired()` |
| `Customer` | `Email` | `HasMaxLength(240)` (nullable) |
| `Agent` | `DisplayName` | `HasMaxLength(160).IsRequired()` |
| `Agent` | `Email` | `HasMaxLength(240)` (nullable) |
| `Category` | `Name` | `HasMaxLength(160).IsRequired()` |
| `Ticket` | `Number` | `HasMaxLength(20).IsRequired()` + a **unique** index |
| `Ticket` | `Title` | `HasMaxLength(180).IsRequired()` + a CHECK constraint |
| `Ticket` | `Description` | `HasMaxLength(4000)` (nullable) |
| `Ticket` | `Status`, `Priority` | `HasMaxLength(30).IsRequired()` |
| `TicketComment` | `Body` | `HasMaxLength(4000).IsRequired()` |
| `TicketComment` | `Author` | `HasMaxLength(120).IsRequired()` |

`180` is not a literal in three places: `SupportDeskContext.TitleMaxLength` is the single constant the
`HasMaxLength`, the CHECK constraint and Module 5's validator will all read.

## One `OnDelete` decision per relationship

```csharp
b.HasOne(x => x.Customer).WithMany(c => c.Tickets)
    .HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);   // a customer with tickets cannot go
b.HasOne(x => x.Agent).WithMany(a => a.Tickets)
    .HasForeignKey(x => x.AgentId).OnDelete(DeleteBehavior.SetNull);       // a departing agent unassigns their tickets
b.HasOne(x => x.Category).WithMany(c => c.Tickets)
    .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);   // a category in use cannot go
```

```csharp
b.HasOne(x => x.Ticket).WithMany(t => t.Comments)
    .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);      // comments die with their ticket
```

| Relationship | Behaviour | Why |
|---|---|---|
| `Ticket.CustomerId → Customers` | **Restrict** | Deleting a customer must never quietly delete their support history. The delete is refused and the operator has to decide what to do with the tickets first. |
| `Ticket.CategoryId → Categories` | **Restrict** | A category is a classification other rows point at. Removing one that is in use would either destroy tickets or leave them unclassified; both are worse than a refusal. |
| `Ticket.AgentId → Agents` | **SetNull** | People leave. Their tickets must stay in the queue and simply become unassigned — the same state a brand-new ticket is in, which is exactly why `AgentId` is nullable. |
| `TicketComment.TicketId → Tickets` | **Cascade** | A comment has no life of its own. It cannot be reassigned to another ticket and it means nothing on its own, so it goes with the parent. |

The rule of thumb the course uses: **Cascade only for rows that are part of the parent** (a true detail
collection). Anything a user would look for on its own gets `Restrict` or `SetNull`.

## How SQLite renders them

`DeleteBehavior` is not a runtime check by EF Core — it becomes an `ON DELETE` clause in the foreign key,
and the database enforces it. `Microsoft.Data.Sqlite` turns `PRAGMA foreign_keys` on by default, so the
rules are real here and not just on SQL Server. From the generated `CREATE TABLE` (the
`Migrations/…_InitialCreate.cs` `Up` and the script in `artifacts/sql/supportdesk_migrations.sql`):

```sql
CREATE TABLE "Tickets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Tickets" PRIMARY KEY AUTOINCREMENT,
    ...
    "RowVersion" BLOB NOT NULL,
    CONSTRAINT "CK_Tickets_Title_Length" CHECK (length("Title") <= 180),
    CONSTRAINT "FK_Tickets_Agents_AgentId"       FOREIGN KEY ("AgentId")    REFERENCES "Agents"     ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Tickets_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Tickets_Customers_CustomerId"  FOREIGN KEY ("CustomerId") REFERENCES "Customers"  ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TicketComments" (
    ...
    CONSTRAINT "FK_TicketComments_Tickets_TicketId" FOREIGN KEY ("TicketId") REFERENCES "Tickets" ("Id") ON DELETE CASCADE
);
```

Two consequences the page demonstrates:

- **The database does the work, not the change tracker.** `ModelDemoService` deletes an agent and a ticket
  *without loading* their dependants, so EF Core has nothing to fix up in memory. One `DELETE` goes out and
  the counts change anyway — that is `ON DELETE SET NULL` and `ON DELETE CASCADE` running inside SQLite.
- **A refusal arrives as an exception, not as a return value.** `Restrict` makes SQLite raise
  `SQLITE_CONSTRAINT_FOREIGNKEY` (error code 19, *FOREIGN KEY constraint failed*), which EF Core wraps in
  `DbUpdateException`. The page catches it and shows *This customer still has tickets and cannot be
  deleted.* — the provider message never reaches the user.

`Restrict` versus `NoAction`: `Restrict` is the one that says "refuse" in both EF Core's fix-up and the
generated SQL. `NoAction` leaves the enforcement entirely to the database's default and, on a provider
with deferred constraints, can behave differently. The Support Desk asks for a refusal, so it says so.

## The grid projection, previewed

Module 3 binds the grid to a **flat projection**, not to `Ticket`:

```csharp
public sealed record TicketListItem(
    int Id, string Number, string Title, string Customer,
    string Status, DateTime? DueDate, string Agent, DateTime UpdatedAt);
```

It does not exist in this module yet — the model has to be right first. The reason it will exist:
`Ticket` carries three navigations and a comment collection, and a grid that binds to the entity either
drags that whole graph across the wire or triggers one query per row. A record with exactly the eight
columns the grid shows is produced by one `Select(...)` inside the service, arrives detached, and keeps
the read-only grid independent from the tracked entity the editor works on. The `IsUrgent` flag and
`Priority` are on the entity for the same reason — a `CellFormatting` handler colours the row from them
once they join the projection.

## Evidence

The page's **Model & migration card** reads this back from the model after every operation (never
hard-coded), through `SchemaInfoService.DescribeAsync()`:

```
on delete   TicketComments.TicketId → Tickets  ON DELETE CASCADE
            Tickets.AgentId → Agents           ON DELETE SET NULL
            Tickets.CategoryId → Categories    ON DELETE RESTRICT
            Tickets.CustomerId → Customers     ON DELETE RESTRICT
checks      Tickets: CK_Tickets_Title_Length = length("Title") <= 180
```

**Delete a customer with tickets (refused)** — `buttonDeleteCustomer`:

```
• buttonDeleteCustomer_Click ModelDemoService.DeleteCustomerWithTicketsAsync() — Remove a customer that has tickets (DeleteBehavior.Restrict)
◦ context      #6 created (SupportDeskContext from the factory)
→ SQL          SELECT "c"."Id", "c"."Email", "c"."Name", ( SELECT COUNT(*) FROM "Tickets" AS "t0" WHERE "c"."Id" = "t0"."CustomerId") AS "TicketCount" FROM "Customers" AS "c" WHERE EXISTS ( SELECT 1 FROM "Tickets" AS "t" WHERE "c"."Id" = "t"."CustomerId") ORDER BY "c"."Id" LIMIT 1   (0.4 ms)
• service      customer #1 'Halden Logistics' has 60 tickets — DELETE goes to the database, ON DELETE RESTRICT decides
→ SQL failed   SqliteException: SQLite Error 19: 'FOREIGN KEY constraint failed'. — DELETE FROM "Customers" WHERE "Id" = @p0 RETURNING 1;
◦ context      #6 disposed (1 tracked entity released)
• caught       DbUpdateException → SqliteException: SQLite Error 19: 'FOREIGN KEY constraint failed'. → friendly message shown, full exception logged server-side
```

**Unassign an agent (SetNull)** — `buttonDeleteAgent`:

```
◦ context      #7 created (SupportDeskContext from the factory)
→ SQL          SELECT "a"."Id", "a"."DisplayName", … AS "TicketCount" FROM "Agents" AS "a" WHERE EXISTS (…) ORDER BY "a"."Id" LIMIT 1   (0.3 ms)
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t" WHERE "t"."AgentId" IS NULL   (0.1 ms)
• service      agent #1 'Priya Natarajan' owns 83 tickets · 96 tickets unassigned before — DELETE goes to the database, ON DELETE SET NULL decides
→ SQL          DELETE FROM "Agents" WHERE "Id" = @p0 RETURNING 1;   (0.2 ms)
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t" WHERE "t"."AgentId" IS NULL   (0.1 ms)
◦ context      #7 disposed (0 tracked entities released)
← result       agent 'Priya Natarajan' deleted · unassigned tickets 96 → 179 (+83, set to NULL by the database) · 4 statement(s) · … ms in the database · 1 context created, 1 disposed
```

Only one `DELETE` was sent — the eighty-three `AgentId` values were cleared by SQLite.

**Delete a ticket with comments (Cascade)** — `buttonDeleteTicket`:

```
• service      ticket SD-1001 'Printer offline on floor 2' has 2 comments · 99 comments in total — DELETE goes to the database (WHERE Id AND RowVersion), ON DELETE CASCADE decides
→ SQL          DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;   (0.2 ms)
→ SQL          SELECT COUNT(*) FROM "TicketComments" AS "t"   (0.1 ms)
← result       ticket SD-1001 deleted · comments 99 → 97 (−2, cascaded by the database) · 4 statement(s) · … ms in the database · 1 context created, 1 disposed
```

Tests (`SupportDesk.Tests`):

- `ModelRulesTests.Foreign_keys_are_enforced_on_the_test_connection` — `PRAGMA foreign_keys` is `1`.
- `ModelRulesTests.Restrict_deleting_a_customer_with_tickets_throws_DbUpdateException_and_changes_nothing`
  — inner `SqliteException`, `SqliteErrorCode == 19`, message contains *FOREIGN KEY constraint failed*,
  and the five customers are still there afterwards.
- `ModelRulesTests.SetNull_deleting_an_agent_unassigns_their_tickets` — the deleted agent's ticket count
  moves into the unassigned count; two agents remain.
- `ModelRulesTests.Cascade_deleting_a_ticket_removes_its_comments` — the ticket is gone and so are exactly
  its comments.
- `ModelRulesTests.The_model_carries_the_planned_indexes_delete_behaviours_and_check_constraint` — the four
  `DeleteBehavior` values as read back from the model metadata.
- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — reads
  `sqlite_master` and asserts `ON DELETE SET NULL`, `ON DELETE RESTRICT` in `Tickets` and `ON DELETE
  CASCADE` in `TicketComments`.
