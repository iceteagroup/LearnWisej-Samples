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
`_loading` guard, `SetBusy(true)`, one awaited service call, a friendly `AlertBox` in `catch`, the UI
restored in `finally`. The button exists so the idempotence can be *watched*: on a database the host
already seeded, `statusLabel` reads *nothing to seed: 60 tickets already exist* and the counts line does
not move. Behind it the seeder sent one `SELECT EXISTS`, one `COUNT` and no `INSERT` at all
(`DevelopmentSeederTests.Seed_is_idempotent_a_second_call_adds_nothing` checks exactly that).

Deleting `SupportDesk.Web/App_Data/supportdesk.db` and restarting is the way back to a clean state; it
also re-runs `MigrateAsync` from nothing. Nothing in the application bulk-deletes tickets.

## Evidence

**First seed** (the first host start on an empty file, or `btnSeed` on an empty database): the seed is
one `SELECT EXISTS` followed by 90 `INSERT`s in one `SaveChangesAsync`, and the change tracker releases 90
tracked entities when the context is disposed. That is everything the seed staged, and the reason the
context is not allowed to survive the operation. The page's `statusLabel` reads *seeded 5 customers, 3
agents, 6 categories, 60 tickets, 16 comments in … ms*.

**Second run**, the whole point of the deliverable: two statements, no `INSERT`, and *nothing to seed: 60
tickets already exist*.

Server console at start:

```
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 354 ms
[SupportDesk] Development seed: nothing to seed: 60 tickets already exist
```

Tests (`SupportDesk.Tests`):

- `DevelopmentSeederTests.Seed_inserts_five_customers_three_agents_six_categories_and_at_least_fifty_tickets`
  — the counts in `SeedResult` and the counts actually in the database agree, and there are at least 50
  tickets.
- `DevelopmentSeederTests.Seed_is_idempotent_a_second_call_adds_nothing` — the second call runs one
  context, reports `nothing to seed`, and the test's command trace (`QueryTrace`) contains **no** command
  starting with `INSERT`.
- `DevelopmentSeederTests.Seed_data_is_realistic_and_mixed` — unique ticket numbers, all four statuses,
  all three priorities, due dates present and absent, 20–40 % unassigned, every title within 180
  characters, and a 16-byte `RowVersion` on every row.
- `MigrationTests.MigrateAsync_applies_InitialCreate_once_and_the_seeder_fills_the_schema` — the seeder
  fills a schema created by the **migration**, not by `EnsureCreated`.
