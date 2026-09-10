# The development seed

Deliverable 5 of the Module 2 lab: five customers, three agents, the categories and at least fifty
tickets in the local database, inserted by a development-only service that can be run twice without
doing anything the second time. **Module 3 raised the ticket count to 312** so the ticket browser has
seven pages of fifty to move through — the rest of this note is unchanged.

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
| `Tickets` | **312** | `SD-1001` … `SD-1312`, real support requests ("Printer offline on floor 2", "VPN drops every hour on the Copenhagen link", …); sixty request titles reused across five rounds with a site qualifier ("Printer offline on floor 2 (Leeds branch)") |
| `TicketComments` | **99** | one or two per ticket, on every fifth ticket |

312 rather than the fifty the lab asks for, so Module 3's paging has seven pages to show and the status
line reads *Showing 50 of 312 tickets · page 1 of 7 · page size 50* — the line the walkthrough shows.

## Deterministic on purpose

`SampleData` uses one `Random` with a **fixed seed**, so the 312 tickets are identical on every machine
and after every reseed, and every screenshot in the course matches.

```csharp
var random = new Random(RandomSeed);                              // 20260910 — fixed

// i * 37 minutes apart with at most 29 minutes of jitter: unique, ordered, and irregular.
var updatedAt  = newest.AddMinutes(-(i * 37 + random.Next(0, 30)));
var createdAt  = updatedAt.AddDays(-random.Next(0, 40)).AddHours(-random.Next(1, 9));
var status     = WeightedStatuses[random.Next(WeightedStatuses.Length)];
var priority   = Priorities[random.Next(Priorities.Length)];
var unassigned = random.Next(10) < 3;                             // ~30 % without an agent
var hasDueDate = random.Next(3) != 0;                             // two thirds carry a due date
var dueOffset  = random.Next(-12, 30);                            // some overdue, most in the next weeks
```

`UpdatedAt` is **strictly decreasing with the index**, so `ORDER BY "UpdatedAt" DESC` has no ties: page 2
of the browser always shows the next fifty rows, and a test can assert exactly which ones. Ties in the
sort key are the classic reason a paged grid shows the same row twice.

The distributions are chosen so the later modules have something to work with:

- all five statuses appear — measured on the seeded database: `Open` 93, `In Progress` 69, `Resolved` 65,
  `Closed` 53, `Waiting` 32 — and all three priorities, so the status filter of Module 3 has real buckets;
- 96 of the 312 tickets (31 %) have no agent, so the grid's `AgentName` really is null on some rows and
  the projection's `LEFT JOIN` is exercised;
- 214 of the 312 tickets carry a due date and 61 of those are already overdue, so the grid has to format a
  `DateTime?`, the due-date range filter has something on both sides of today, and the `(Status, DueDate)`
  index has nulls to sort;
- `UpdatedAt` and `CreatedAt` are independent, so the "recently changed" sort the browser uses is not the
  same as the created order;
- `Description` is set on every fourth ticket only, so the editor has to cope with an empty memo.

The tokens are not in `SampleData`: `RowVersion` is stamped by `SupportDeskContext.SaveChanges` on the
way in (see [RowVersionAndIndexes.md](RowVersionAndIndexes.md)), which is why every seeded ticket comes
out of the database with 16 bytes in that column.

## Why it is a service and not `HasData`

Fixed lookup rows *could* travel with the migration through `HasData` — categories are a reasonable
candidate. Three hundred sample tickets are not: `HasData` rows become `InsertData` operations inside the
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
• service      AddRange: 5 customers, 3 agents, 6 categories, 312 tickets (99 comments) → one SaveChangesAsync
→ SQL          INSERT INTO "Agents" ("DisplayName", "Email") VALUES (@p0, @p1) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Categories" ("Name") VALUES (@p0) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Customers" ("Email", "Name") VALUES (@p0, @p1) RETURNING "Id";   (0.1 ms)
→ SQL          INSERT INTO "Tickets" ("AgentId", "CategoryId", "CreatedAt", "CustomerId", "Description", "DueDate", "IsUrgent", "Number", "Priority", "RowVersion", "Status", "Title", "UpdatedAt") VALUES (…) RETURNING "Id";   (0.1 ms)
   …                                                                   (425 INSERTs in total, one SaveChangesAsync)
→ SQL          INSERT INTO "TicketComments" ("Author", "Body", "CreatedAt", "TicketId") VALUES (@p0, @p1, @p2, @p3) RETURNING "Id";   (0.1 ms)
◦ context      #2 disposed (425 tracked entities released)
← result       seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in 445 ms · 426 statement(s) · … ms in the database · 1 context created, 1 disposed
◦ card         Model & migration card refreshed · SchemaInfoService.DescribeAsync(): 9 statements (1.2 ms), 1 context created, 1 disposed
```

`425 tracked entities released` on dispose is the change tracker letting go of everything the seed staged
— and the reason the context is not allowed to survive the operation.

**Second click** — the whole point of the deliverable:

```
• btnSeed_Click DevelopmentSeeder.SeedDevelopmentDataAsync() — one context: AnyAsync, then AddRange + SaveChangesAsync or nothing
◦ context      #3 created (SupportDeskContext from the factory)
→ SQL          SELECT EXISTS ( SELECT 1 FROM "Tickets" AS "t")   (0.2 ms)
→ SQL          SELECT COUNT(*) FROM "Tickets" AS "t"   (0.1 ms)
• service      tickets already exist (312) — returning without AddRange
◦ context      #3 disposed (0 tracked entities released)
← result       nothing to seed: 312 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange · 2 statement(s) · … ms in the database · 1 context created, 1 disposed
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
• service      AddRange: 5 customers, 3 agents, 6 categories, 312 tickets (99 comments) → one SaveChangesAsync
   …
◦ context      #12 disposed (425 tracked entities released)
← result       seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in 81 ms · 431 statement(s) · … ms in the database · 2 context created, 2 disposed
```

Server console at start:

```
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in 445 ms
[SupportDesk] Development seed: nothing to seed: 312 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange
```

Tests (`SupportDesk.Tests`):

- `DevelopmentSeederTests.Seed_inserts_five_customers_three_agents_six_categories_and_312_tickets`
  — the counts in `SeedResult` and the counts actually in the database agree, and there are exactly 312
  tickets.
- `DevelopmentSeederTests.Seed_is_idempotent_a_second_call_adds_nothing` — the second call runs one
  context, reports `nothing to seed`, and the trace contains **no** command starting with `INSERT`.
- `DevelopmentSeederTests.Seed_data_is_realistic_and_mixed` — unique ticket numbers, all five statuses,
  all three priorities, due dates present and absent, 20–40 % unassigned, every title within 180
  characters, and a 16-byte `RowVersion` on every row.
- `TicketSearchTests.Results_are_ordered_by_UpdatedAt_descending` — the fifty `UpdatedAt` values on a page
  are distinct, which is what makes paging over this seed stable.
- `DevelopmentSeederTests.Reset_empties_every_table_and_seeds_again` — deletes a ticket (cascading its
  comments), resets, and gets the original ticket and comment counts back.
- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — the seeder
  fills a schema created by the **migration**, not by `EnsureCreated`.
