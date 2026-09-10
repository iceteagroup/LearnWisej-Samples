# The development seed

Deliverable 5 of the Module 2 lab: five customers, three agents, the categories and at least fifty
tickets in the local database, inserted by a development-only service that can be run twice without
doing anything the second time.

## The shape of the method

`SupportDesk.Services/DevelopmentSeeder.cs`, `SeedDevelopmentDataAsync`:

```csharp
await using var db = await _dbFactory.CreateDbContextAsync(token);

if (await db.Tickets.AnyAsync(token))
{
    var existing = await db.Tickets.CountAsync(token);
    return new SeedResult(false, 0, 0, 0, 0, 0, existing, watch.Elapsed.TotalMilliseconds);
}

db.Customers.AddRange(customers);
db.Agents.AddRange(agents);
db.Categories.AddRange(categories);
db.Tickets.AddRange(tickets);          // comments travel with their tickets through the navigation
await db.SaveChangesAsync(token);
```

Five things, in this order, and nothing else:

1. **One context, from the factory.** `await using` — created when the method starts, disposed before it
   returns. The same rule as every other operation in the course; the seeder is not special.
2. **`AnyAsync` first.** `SELECT EXISTS (SELECT 1 FROM "Tickets")` is the cheapest possible question and
   the whole of the idempotence: if a ticket exists, the method returns before `AddRange` is reached.
3. **`AddRange`, not a loop of `Add` + `SaveChanges`.** Everything is staged in the change tracker and
   sent in one `SaveChangesAsync`, inside one transaction. A partial seed is not a state anyone has to
   reason about.
4. **The graph goes in through the navigations.** `Ticket.Customer`, `.Category`, `.Agent` and
   `.Comments` are object references, so the customers, categories, agents and comments are inserted with
   their tickets and the foreign keys are filled in by EF Core after the parents get their identity
   values. No `Id` is set by hand anywhere.
5. **It returns what it did.** `SeedResult` carries `Seeded`, the five counts, `ExistingTickets` and the
   elapsed time, and `Describe()` turns that into the sentence the page shows.

## What it inserts

| Table | Rows | What they are |
|---|---|---|
| `Customers` | **5** | Halden Logistics, Brightwater Clinics, Northgate Retail Group, Verity Insurance, Copperfield Engineering |
| `Agents` | **3** | Priya Natarajan, Tomasz Wierzbicki, Elena Marchetti |
| `Categories` | **6** | Hardware, Network, Accounts & Access, Email & Collaboration, Business Applications, Printing |
| `Tickets` | **60** | `SD-1001` … `SD-1060`, real support requests ("Printer offline on floor 2", "VPN drops every hour on the Copenhagen link", …) |
| `TicketComments` | **16** | one or two per ticket, on every fifth ticket |

Sixty rather than the fifty the lab asks for, so Module 3's paging has more than two pages to show.

## Deterministic on purpose

`SampleData` contains no random numbers and no `DateTime.Now` inside a row: every value is derived from
the row's index, so two machines that seed produce the same database and every screenshot in the course
matches.

```csharp
var status   = Statuses[(i * 7 + i / 4) % Statuses.Length];       // mixed, deterministic
var priority = Priorities[(i * 5 + i / 3) % Priorities.Length];
var createdAt = today.AddDays(-(i % 45) - 1).AddHours(8 + i % 9).AddMinutes((i * 13) % 60);
var unassigned = i % 10 is 3 or 6 or 9;                           // 30 % without an agent
var hasDueDate = i % 3 != 1;                                      // two thirds carry a due date
```

The distributions are chosen so the later modules have something to work with:

- all four statuses (`Open`, `In Progress`, `Waiting`, `Closed`) and all three priorities appear, so the
  status filter of Module 3 has real buckets;
- about 30 % of tickets have no agent, so "Unassigned" in the agent ComboBox is a state that exists;
- a third of tickets have no due date, so the grid has to format a `DateTime?` and the `(Status, DueDate)`
  index has nulls to sort;
- `UpdatedAt` differs from `CreatedAt` on everything except open tickets, so the "recently changed" sort
  is not the same as the created order;
- `Description` is set on every fourth ticket only, so the editor has to cope with an empty memo.

The tokens are not in `SampleData`: `RowVersion` is stamped by `SupportDeskContext.SaveChanges` on the
way in (see [RowVersionAndIndexes.md](RowVersionAndIndexes.md)), which is why every seeded ticket comes
out of the database with 16 bytes in that column.

## Why it is a service and not `HasData`

Fixed lookup rows *could* travel with the migration through `HasData` — categories are a reasonable
candidate. Sixty sample tickets are not: `HasData` rows become `InsertData` operations inside the
migration, they are part of the schema history, and every change to them scaffolds another migration.
That is right for reference data a production database must contain and wrong for demo content.

So the split the course uses is: **anything production needs → the migration; anything only a developer
needs → a development-only service.** The seeder lives in `SupportDesk.Services`, is registered
`Transient` like every other service, and is only ever called from two places, both of which are
Development-gated.

## Where it is called from

**At host start** — `SupportDesk.Web/Startup.cs`, step 5:

```csharp
if (app.Environment.IsDevelopment())
    await SupportDeskDevelopmentDatabase.EnsureReadyAsync(app.Services);
```

`EnsureReadyAsync` runs `MigrateAsync` and then the seeder, and writes both results to the server
console. It runs once, before the first session exists, so no page is waiting on it.

**From the page** — `btnSeed_Click`, through the same `RunAsync` pattern as every other handler: the
`_loading` guard, `SetBusy(true)`, one awaited service call, a friendly message in `catch`, the UI
restored in `finally`. The button exists so the idempotence can be *watched*: click it a second time and
the trace shows one `SELECT EXISTS`, one `COUNT`, and no `INSERT` at all, with the state chip turning
amber (`● nothing to seed`).

## Reset & reseed

`ResetDevelopmentDataAsync` is a lab prop, not a pattern to copy. It empties every table in foreign-key
order and then seeds again:

```csharp
await db.TicketComments.ExecuteDeleteAsync(token);
await db.Tickets.ExecuteDeleteAsync(token);
await db.Customers.ExecuteDeleteAsync(token);
await db.Agents.ExecuteDeleteAsync(token);
await db.Categories.ExecuteDeleteAsync(token);
```

Children first, because the `Restrict` rules on `Tickets` would refuse a customer or category delete
while tickets exist — the reset has to respect exactly the rules
[ModelAndDeleteBehaviours.md](ModelAndDeleteBehaviours.md) describes. `ExecuteDeleteAsync` sends one
`DELETE FROM "…"` per table without loading or tracking anything, and the whole reset is **two contexts**
(one for the deletes, then `SeedDevelopmentDataAsync` creates its own) — which the trace shows.

It exists because the delete demos are destructive: after unassigning an agent and cascading a ticket's
comments away, this button puts the database back so the next learner sees the same numbers. Nothing in
production ever bulk-deletes tickets, and there is no equivalent button anywhere outside Development.

Deleting `SupportDesk.Web/App_Data/supportdesk.db` and restarting is the other way back to a clean state
— that one also re-runs `MigrateAsync` from nothing.

## Evidence

**First click of `btnSeed`** (or the first host start):

```
• btnSeed_Click DevelopmentSeeder.SeedDevelopmentDataAsync() — one context: AnyAsync, then AddRange + SaveChangesAsync or nothing
◦ context      #2 created (SupportDeskContext from the factory)
→ SQL          SELECT EXISTS ( SELECT 1 FROM "Tickets" AS "t")   (0.3 ms)
• service      AddRange: 5 customers, 3 agents, 6 categories, 60 tickets (16 comments) → one SaveChangesAsync
→ SQL          INSERT INTO "Agents" ("DisplayName", "Email") VALUES (@p0, @p1) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Categories" ("Name") VALUES (@p0) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Customers" ("Email", "Name") VALUES (@p0, @p1) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Tickets" ("AgentId", "CategoryId", "CreatedAt", "CustomerId", "Description", "DueDate", "IsUrgent", "Number", "Priority", "RowVersion", "Status", "Title", "UpdatedAt") VALUES (…) RETURNING "Id";   (0.1 ms)
   …                                                                   (90 INSERTs in total, one SaveChangesAsync)
→ SQL          INSERT INTO "TicketComments" ("Author", "Body", "CreatedAt", "TicketId") VALUES (@p0, @p1, @p2, @p3) RETURNING "Id";   (0.1 ms)
◦ context      #2 disposed (90 tracked entities released)
← result       seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 354 ms · 91 statement(s) · … ms in the database · 1 context created, 1 disposed
◦ card         Model & migration card refreshed · SchemaInfoService.DescribeAsync(): 9 statements (1.2 ms), 1 context created, 1 disposed
```

`90 tracked entities released` on dispose is the change tracker letting go of everything the seed staged
— and the reason the context is not allowed to survive the operation.

**Second click** — the whole point of the deliverable:

```
• btnSeed_Click DevelopmentSeeder.SeedDevelopmentDataAsync() — one context: AnyAsync, then AddRange + SaveChangesAsync or nothing
◦ context      #3 created (SupportDeskContext from the factory)
→ SQL          SELECT EXISTS ( SELECT 1 FROM "Tickets" AS "t")   (0.2 ms)
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t"   (0.1 ms)
• service      tickets already exist (60) — returning without AddRange
◦ context      #3 disposed (0 tracked entities released)
← result       nothing to seed: 60 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange · 2 statement(s) · … ms in the database · 1 context created, 1 disposed
```

Two statements, no `INSERT`, `● nothing to seed` in amber.

**Reset & reseed** — two contexts, five `DELETE`s, then the seed:

```
• buttonReset_Click DevelopmentSeeder.ResetDevelopmentDataAsync() — ExecuteDelete on every table (children first), then the seeder (lab prop)
◦ context      #11 created (SupportDeskContext from the factory)
• service      ExecuteDeleteAsync on TicketComments, Tickets, Customers, Agents, Categories (children first)
→ SQL          DELETE FROM "TicketComments" AS "t"   (0.2 ms)
→ SQL          DELETE FROM "Tickets" AS "t"   (0.3 ms)
→ SQL          DELETE FROM "Customers" AS "c"   (0.1 ms)
→ SQL          DELETE FROM "Agents" AS "a"   (0.1 ms)
→ SQL          DELETE FROM "Categories" AS "c"   (0.1 ms)
◦ context      #11 disposed (0 tracked entities released)
◦ context      #12 created (SupportDeskContext from the factory)
→ SQL          SELECT EXISTS ( SELECT 1 FROM "Tickets" AS "t")   (0.1 ms)
• service      AddRange: 5 customers, 3 agents, 6 categories, 60 tickets (16 comments) → one SaveChangesAsync
   …
◦ context      #12 disposed (90 tracked entities released)
← result       seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 12 ms · 96 statement(s) · … ms in the database · 2 context created, 2 disposed
```

Server console at start:

```
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 354 ms
[SupportDesk] Development seed: nothing to seed: 60 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange
```

Tests (`SupportDesk.Tests`):

- `DevelopmentSeederTests.Seed_inserts_five_customers_three_agents_six_categories_and_at_least_fifty_tickets`
  — the counts in `SeedResult` and the counts actually in the database agree, and there are at least 50
  tickets.
- `DevelopmentSeederTests.Seed_is_idempotent_a_second_call_adds_nothing` — the second call runs one
  context, reports `nothing to seed`, and the trace contains **no** command starting with `INSERT`.
- `DevelopmentSeederTests.Seed_data_is_realistic_and_mixed` — unique ticket numbers, all four statuses,
  all three priorities, due dates present and absent, 20–40 % unassigned, every title within 180
  characters, and a 16-byte `RowVersion` on every row.
- `DevelopmentSeederTests.Reset_empties_every_table_and_seeds_again` — deletes a ticket (cascading its
  comments), resets, and gets the original ticket and comment counts back.
- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — the seeder
  fills a schema created by the **migration**, not by `EnsureCreated`.
