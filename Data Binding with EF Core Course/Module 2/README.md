# SupportDesk · Data Binding with EF Core · Module 2

Local lab build for **Module 2 · Modeling, DbContext Configuration, and Migrations**. It follows the
walkthrough video: the Support Desk model arrives — `Customer`, `Agent`, `Category`, `Ticket` and
`TicketComment` — configured entirely with the Fluent API (required strings and lengths, a `RowVersion`
concurrency token, the indexes the ticket browser will need, one `OnDelete` rule per relationship and a
CHECK constraint on the title), scaffolded into the `InitialCreate` migration through the design-time
factory, applied with `MigrateAsync` at start in Development, and filled by a development-only seeder.
Module 1's count now returns sixty. Nothing is bound to a grid yet — that starts in Module 3.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local
SQLite file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 2/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5402
```

Then open <http://localhost:5402>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web
project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the
first session: `MigrateAsync` applies `InitialCreate` (creating the file and `__EFMigrationsHistory` on
first start), then `DevelopmentSeeder` inserts five customers, three agents, six categories, sixty
tickets and sixteen comments. Both results are printed on the server console:

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 354 ms
```

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal` / `-shm` companions) to reset the
module completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — eighteen tests against SQLite in memory (the three Module 1
lifetime tests, the seeder, the migration start-up path, and the schema rules: Restrict / SetNull /
Cascade, the RowVersion token, the unique ticket number and the title CHECK).

Migration commands, from the `Module 2` folder (`--framework` is required because the web project
multi-targets):

```bash
dotnet ef migrations list   --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web \
    --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql
```

## What to click

Bottom bar **row 1** is the Module 2 paths, **row 2** ("Module 1 · lifetimes") keeps every Module 1 path
working. `Clear trace` is anchored right on row 1.

| Action | Path | What you should see |
|---|---|---|
| **Count tickets** (`countButton`) | success | `statusLabel` → *60 tickets in the Support Desk database*; the trace shows `• countButton_Click TicketQueryService.CountTicketsAsync()` → `◦ context #n created` → `→ SQL SELECT COUNT(*) FROM "Tickets" AS "t"` → `◦ context #n disposed (0 tracked entities released)` → `← result 60 tickets · 1 statement(s) · … · 1 context created, 1 disposed` |
| **Seed development data** (`btnSeed`) | success · idempotence | already seeded at start, so the first click reports *nothing to seed: 60 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange*, the chip turns amber `● nothing to seed`, and the trace shows two statements (`SELECT EXISTS …`, `SELECT COUNT(*) …`) and **no** `INSERT`. After a **Reset & reseed** or a deleted database file the same button reports *seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in … ms* with 91 statements in one context |
| **Reset & reseed** (`buttonReset`) | lab prop · recovery | five `DELETE FROM "…"` (children first: TicketComments, Tickets, Customers, Agents, Categories) in one context, then the seeder in a second one; `← result seeded 5 customers, … · 96 statement(s) · 2 context created, 2 disposed`. Use it to put the delete demos back |
| **Save a 200-char title (fails)** (`buttonOverlongTitle`) | failure · CHECK constraint | red banner *The ticket could not be saved because the database rejected the change.*, `● fault`, and in the trace `→ SQL failed SqliteException: SQLite Error 19: 'CHECK constraint failed: CK_Tickets_Title_Length'.` then `• caught DbUpdateException → SqliteException: … → friendly message shown, full exception logged server-side`. The ticket count on the card does not move |
| **Delete a customer with tickets (refused)** (`buttonDeleteCustomer`) | failure · `DeleteBehavior.Restrict` | `• service customer #1 'Halden Logistics' has 12 tickets — DELETE goes to the database, ON DELETE RESTRICT decides`, then `→ SQL failed SqliteException: SQLite Error 19: 'FOREIGN KEY constraint failed'. — DELETE FROM "Customers" WHERE "Id" = @p0 RETURNING 1;` and the banner *This customer still has tickets and cannot be deleted.* Customers on the card: still 5 |
| **Unassign an agent (SetNull)** (`buttonDeleteAgent`) | `DeleteBehavior.SetNull` | one `DELETE FROM "Agents" WHERE "Id" = @p0 RETURNING 1;` and nothing else touches Tickets, yet `← result agent 'Priya Natarajan' deleted · unassigned tickets 18 → 32 (+14, set to NULL by the database)`. Agents on the card drop 3 → 2. Click it again after the agents run out and the chip turns amber `● nothing to do` |
| **Delete a ticket with comments (Cascade)** (`buttonDeleteTicket`) | `DeleteBehavior.Cascade` | `→ SQL DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;` — note the concurrency token in the `WHERE` — then `← result ticket SD-1001 deleted · comments 16 → 15 (−1, cascaded by the database)`. Tickets and TicketComments on the card both drop |
| **Slow count (2.5 s)** (`buttonSlowCount`), then **Count tickets** while it runs | progress · guard | the buttons are disabled and `statusLabel` reads *Counting tickets… (simulated 2.5 s latency)*; a click during the wait is logged as `• guard countButton_Click: an operation is already running — this click is ignored`; `finally` restores the buttons |
| **▶ Count ×3 rapid (guard)** (`buttonRapid`) | progress · guard | three counts started at once: `rapid click 1` runs the slow count, `rapid click 2` and `3` are dropped by `_loading` — the two guard lines sit between `context created` and the SQL |
| **Break the database** (`buttonBreak`) | failure · outage | `DevelopmentOutageSwitch.IsDown = true`, the count runs, the connection open throws: red banner *The Support Desk database is not reachable right now. Nothing was changed…*, `● fault`, trace `• caught DatabaseUnavailableException … → friendly message shown, full exception logged server-side`; the context is still disposed, and the Model & migration card reports `not refreshed` for the same reason |
| **Restore and count** (`buttonRestore`) | recovery | the switch goes off; the next click gets a **fresh** context and a working connection; `● ok` again and the card refreshes |
| **Two ops, one context (anti-pattern)** (`buttonAntiPattern`) | anti-pattern | one context, two threads counting on it at once (what a static/shared DbContext does with two sessions): `• caught InvalidOperationException: A second operation was started on this context instance before a previous operation completed…` and a red banner. Nothing in the real code path can do this — the demo lives in `SharedContextAntiPattern` |
| **Clear trace** (`buttonClear`) | – | empties the right-hand list |

The right-hand card is the **Server ⇄ Database · EF Core lifetime & SQL trace**: every context created
and disposed, every SQL statement with its duration, every service note about what it expects the
database to do, and every handler decision. Under the left card, the compact **Four lifetimes** table
from Module 1 keeps the running totals — contexts created versus disposed, always `0 alive between
clicks`.

### The Model & migration card

The left card's **Model & migration** panel is re-read after **every** operation, including the failed
ones, by `SchemaInfoService.DescribeAsync()` — one context, nine statements, and none of it hard-coded:

```
rows        Customers 5 · Agents 3 · Categories 6 · Tickets 60 · TicketComments 16
migrations  applied 1: 20260910150534_InitialCreate · pending 0
indexes     Customers: IX_Customers_Name (Name)
            TicketComments: IX_TicketComments_TicketId (TicketId)
            Tickets: IX_Tickets_AgentId (AgentId) · IX_Tickets_CategoryId (CategoryId) · IX_Tickets_CustomerId (CustomerId)
                   · IX_Tickets_Number (Number) UNIQUE · IX_Tickets_UpdatedAt (UpdatedAt) · IX_Tickets_Status_DueDate (Status, DueDate)
on delete   TicketComments.TicketId → Tickets  ON DELETE CASCADE
            Tickets.AgentId → Agents           ON DELETE SET NULL
            Tickets.CategoryId → Categories    ON DELETE RESTRICT
            Tickets.CustomerId → Customers     ON DELETE RESTRICT
checks      Tickets: CK_Tickets_Title_Length = length("Title") <= 180
```

The row counts come from five `COUNT(*)`s, the migration lines from `GetAppliedMigrationsAsync()` /
`GetPendingMigrationsAsync()` (a pending migration would appear in red), and the indexes, delete
behaviours and check constraints from EF Core's **design-time** model — the runtime model drops check
constraints, so asking `db.Model` would report none. The card refresh runs in its own trace scope and
reports one summary line (`◦ card Model & migration card refreshed · … 9 statements …`) instead of nine
SQL lines, so it never buries the operation you just clicked.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `Customer`, `Agent`, `Category`, `Ticket` and `TicketComment` entities with relationships and delete behaviours | [`docs/ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md) · `SupportDesk.Data/Entities/*.cs`, `SupportDeskContext.OnModelCreating` |
| 2 | `RowVersion` configured with `IsRowVersion` | [`docs/RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md) · `SupportDeskContext` (`IsRowVersion()`, the save behaviours and `StampTickets` in the `SaveChanges` overrides) |
| 3 | Indexes for `Status`, `DueDate`, `CustomerId` and `UpdatedAt` | [`docs/RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md) · the `HasIndex` calls in `OnModelCreating`, the `CreateIndex` operations in the migration |
| 4 | Design-time factory and the `InitialCreate` migration | [`docs/MigrationWorkflow.md`](docs/MigrationWorkflow.md) · `SupportDesk.Data/SupportDeskDesignTimeFactory.cs`, `Migrations/20260910150534_InitialCreate.cs` + `.Designer.cs` + `SupportDeskContextModelSnapshot.cs`, `artifacts/sql/supportdesk_migrations.sql` |
| 5 | Seed data: five customers, three agents, categories and at least fifty tickets | [`docs/SeedData.md`](docs/SeedData.md) · `SupportDesk.Services/DevelopmentSeeder.cs`, `SupportDesk.Web/SupportDeskDevelopmentDatabase.cs`, `btnSeed_Click` |

The module's fourth objective — *separate database schema constraints from UI/domain validation* — has
its own note: [`docs/SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md) (the CHECK constraint, the
`DbUpdateException` → friendly banner path, and what Module 5 adds on top).

## Where things live

```
Module 2/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    dotnet ef migrations script --idempotent (committed on purpose)
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    host: configuration → AddSupportDeskData → services → Build → IServiceProvider bridge → MigrateAsync + seed (Development) → UseWisej
│  ├─ Program.cs                    Wisej.NET session entry point: Application.MainPage = new TicketBrowserPage()
│  ├─ TicketBrowserPage.cs          the lab page: [Inject] services, RunAsync (guard → busy → service → catch → finally), the Module & migration card
│  ├─ TicketBrowserPage.Designer.cs Designer-generated layout (cards, two button rows, trace list)
│  ├─ SupportDeskDevelopmentDatabase.cs  Development only: MigrateAsync + DevelopmentSeeder at host start
│  ├─ appsettings.json              ConnectionStrings:SupportDesk = Data Source=App_Data/supportdesk.db
│  └─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5402)
├─ SupportDesk.Data/                EF Core lives here (the only project referencing the provider)
│  ├─ SupportDeskContext.cs         the model: Fluent rules, indexes, delete behaviours, CHECK constraint, RowVersion stamping
│  ├─ Entities/                     Customer.cs, Agent.cs, Category.cs, Ticket.cs, TicketComment.cs — no mapping attributes
│  ├─ Migrations/                   20260910150534_InitialCreate(.Designer).cs + SupportDeskContextModelSnapshot.cs
│  ├─ SupportDeskDesignTimeFactory.cs   IDesignTimeDbContextFactory for dotnet ef (no password, no host)
│  ├─ SupportDeskPaths.cs           resolves the relative SQLite path for the app, the tests and dotnet ef
│  ├─ DependencyInjection/…Extensions.cs  AddSupportDeskData: AddDbContextFactory + UseSqlite + dev diagnostics
│  └─ Diagnostics/                  lab instruments: QueryTrace (AsyncLocal sink), QueryTraceInterceptor, DevelopmentOutageSwitch
├─ SupportDesk.Services/
│  ├─ TicketQueryService.cs         CountTicketsAsync / CountTicketsSlowlyAsync (one context per call)
│  ├─ DevelopmentSeeder.cs          SeedDevelopmentDataAsync (idempotent) + ResetDevelopmentDataAsync (lab prop) + SampleData
│  ├─ ModelDemoService.cs           the four schema demos: overlong title, Restrict, SetNull, Cascade
│  ├─ SchemaInfoService.cs          what the Model & migration card shows (counts, migrations, indexes, FKs, checks)
│  └─ SharedContextAntiPattern.cs   deliberately wrong: two threads on one context
├─ SupportDesk.Tests/               xunit, SQLite in memory (SqliteTestFactory) — 18 tests
│  ├─ TicketQueryServiceTests.cs    the three Module 1 tests
│  ├─ DevelopmentSeederTests.cs     counts, idempotence, realistic data, reset
│  ├─ MigrationTests.cs             MigrateAsync applies InitialCreate once, then the seeder fills it
│  └─ ModelRulesTests.cs            Restrict / SetNull / Cascade, RowVersion, unique number, title CHECK
└─ docs/                            the five deliverables + the validation-layers note
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 1 solution and confirm the count still runs | `SupportDesk.slnx`, `countButton_Click` in `TicketBrowserPage.cs` — unchanged from Module 1 |
| 2 · The five entity classes, integer keys, both-direction navigations, nullable `AgentId`, `byte[] RowVersion`, no mapping attributes | `SupportDesk.Data/Entities/Customer.cs`, `Agent.cs`, `Category.cs`, `Ticket.cs`, `TicketComment.cs` |
| 3 · Relationships with `HasOne`/`WithMany`/`HasForeignKey` + `OnDelete` per relationship; `HasMaxLength`/`IsRequired` on the titles, statuses and lookup names | `SupportDeskContext.OnModelCreating` — the `Ticket` and `TicketComment` blocks |
| 4 · `RowVersion` with `IsRowVersion()` and the planned indexes (`Status`+`DueDate`, `CustomerId`, `UpdatedAt`, `Customer.Name`) | `SupportDeskContext.OnModelCreating` + the `StampTickets` region (SQLite has no `rowversion` type) |
| 5 · `SupportDeskDesignTimeFactory` with the app's provider and a password-free local connection | `SupportDesk.Data/SupportDeskDesignTimeFactory.cs`, `SupportDeskPaths.DevelopmentDatabaseFile()` |
| 6 · `dotnet ef migrations add InitialCreate`, read `Up`/`Down`, then `database update` | `SupportDesk.Data/Migrations/20260910150534_InitialCreate.cs` + `SupportDeskContextModelSnapshot.cs`; the commands are in [`docs/MigrationWorkflow.md`](docs/MigrationWorkflow.md) |
| 7 · Development-only `SeedDevelopmentDataAsync` (one context, early return, `AddRange`, `SaveChangesAsync`, dispose) called from `btnSeed_Click` behind the guard | `SupportDesk.Services/DevelopmentSeeder.cs`, `btnSeed_Click` → `RunAsync` in `TicketBrowserPage.cs` |
| 8 · Show every path: the seed succeeding, a second click seeding nothing, a failed save as a friendly message, a refused customer delete | `btnSeed` · `btnSeed` again · `buttonOverlongTitle` · `buttonDeleteCustomer` — see **What to click** |
| 9 · Review & run: migration and snapshot committed, `--idempotent` script produced, the count shows at least fifty, and a note on the delete and index choices | `.gitignore` (the `artifacts/sql/` exception), `artifacts/sql/supportdesk_migrations.sql`, `countButton` → 60, and [`docs/ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md) + [`docs/RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md) |

Lab code check (`labs.js` m2): an event handler, a `DbContext` created from the factory for one
operation, the model configured with the Fluent API (`IsRowVersion` / `HasIndex` / `OnDelete` /
`HasMaxLength` in `OnModelCreating`), a `try`/`catch` on the failure path, and migrations applied plus an
idempotent seed (`MigrateAsync`, `AnyAsync`, `AddRange`, `SaveChangesAsync`) — `btnSeed_Click` →
`RunAsync` → `DevelopmentSeeder.SeedDevelopmentDataAsync` covers all five.

## Self-check answers (lesson review questions)

- **You delete a customer who still has open tickets. What happens with the delete behaviour you
  configured, and what would have happened with `Cascade`?**
  `Ticket.CustomerId` is configured `OnDelete(DeleteBehavior.Restrict)`, which becomes
  `ON DELETE RESTRICT` on the foreign key in `CREATE TABLE "Tickets"`. The `DELETE FROM "Customers"
  WHERE "Id" = @p0` reaches SQLite, the engine refuses it with `SQLITE_CONSTRAINT_FOREIGNKEY` (*FOREIGN
  KEY constraint failed*, error code 19), EF Core wraps that in `DbUpdateException`, and the handler
  shows *This customer still has tickets and cannot be deleted.* Nothing changed — the customer count on
  the card is the same before and after. With `Cascade` the same click would have succeeded and taken
  every one of that customer's tickets with it, and every comment under those tickets through the second
  cascade — twelve tickets and their history gone from one button, with no error to notice. That is why
  only `TicketComment` (a row that has no meaning without its parent) is `Cascade`, and why the agent
  relationship uses `SetNull` instead: people leave, their tickets must not.

- **Two agents load ticket SD-1042, and both click Save a second apart. Which save throws, what does the
  `UPDATE` statement's `WHERE` clause contain, and what does the user see?**
  The **second** one throws. `RowVersion` is a concurrency token, so EF Core adds its *original* value —
  the token read when that agent loaded the ticket — to the update condition:
  `UPDATE "Tickets" SET "RowVersion" = @p0, "UpdatedAt" = @p1 WHERE "Id" = @p2 AND "RowVersion" = @p3
  RETURNING 1;`. The first save matches the row, writes a **new** token (stamped by
  `SupportDeskContext.SaveChanges`, because SQLite has no `rowversion` type — SQL Server would do it in
  the engine) and succeeds. A second later the other agent's `@p3` is the old token, the statement
  matches zero rows, and EF Core raises `DbUpdateConcurrencyException`. Today the user sees the banner
  *Someone else changed this row in the meantime. Nothing was saved — reload and try again.* and nothing
  was written; Module 7 replaces that with a conflict dialog offering the current database values, and a
  reload / overwrite / merge choice. Without the token the second save would have matched the row,
  overwritten the first agent's work and reported success.

- **Why does `IsRequired()` on `Title` not give the user an "Enter a title" message, and where will that
  message come from later in the course?**
  Because `IsRequired()` and `HasMaxLength(180)` are *mapping* rules: they tell the database what the
  column must guarantee (`NOT NULL`, and on SQL Server `nvarchar(180)`). They are enforced after a
  round-trip, by the engine, and they fail as a `DbUpdateException` carrying a provider message no user
  should read — the **last** line of defence, not the first. On SQLite `HasMaxLength` is not even
  enforced, which is why this module adds `CK_Tickets_Title_Length` to make the 180-character rule real
  at the database layer. The user-facing message belongs to the editor: Module 5 adds a `TicketValidator`
  on the edit model and an `ErrorProvider` next to `txtTitle`, so "Enter a title" appears before anything
  is sent — along with the rules no column constraint can express, such as "a closed ticket cannot have a
  future due date". Both layers stay: the validator so the user is never shown a provider error, the
  constraint so a bug in the editor or a second application cannot write a bad row.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests` — **18 passed**, 0 failed.
- `dotnet ef migrations add InitialCreate`, `dotnet ef migrations script --idempotent` and
  `dotnet ef migrations list` all run from the module folder with `--framework net10.0`, without starting
  the Wisej.NET host; the migration, its designer file, the snapshot and the script are committed.
- The SQL statements, service notes and result strings quoted in this README and in `docs/` were captured
  from the real services running against SQLite (the same code paths the page calls), not written from
  memory.

Facts carried over from Module 1 and still relied on here:
`Application.Services.AddService<IServiceProvider>(app.Services)` makes `[Inject]` on a Page resolve
through Microsoft DI, and application services must be **Transient** or Singleton because Wisej.NET asks
the **root** provider; after an `await` the handler continues off the original request, so
`Application.Update(this)` in `finally` pushes the final state to the browser.

**Browser verification: see the note at the end.**

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5402> in the Browser pane, every button on both rows clicked once in order,
trace read back from the page:

- Page load: `• session page created … services through [Inject] → resolved from Microsoft DI`, then the Model & migration card
  refreshed itself (`SchemaInfoService.DescribeAsync(): 9 statements … 1 context created, 1 disposed`) and showed
  rows 5 · 3 · 6 · 60 · 16, `applied 1: 20260910150534_InitialCreate · pending 0`, the eight indexes, the four ON DELETE rules and the CHECK.
- **Count tickets** → *60 tickets in the Support Desk database*, one `SELECT COUNT(*)`, context created and disposed.
- **Seed development data** on a seeded database → `SELECT EXISTS …` + `SELECT COUNT(*)`, `• service tickets already exist (60) — returning without AddRange`, no INSERT.
- **Save a 200-char title (fails)** → `→ SQL failed SqliteException: SQLite Error 19: CHECK constraint failed: CK_Tickets_Title_Length`,
  `• caught DbUpdateException … friendly message shown`, red banner, counts unchanged.
- **Delete a customer with tickets (refused)** → `FOREIGN KEY constraint failed` on `DELETE FROM "Customers" WHERE "Id" = @p0 RETURNING 1;`, banner, customers still 5.
- **Unassign an agent (SetNull)** → one `DELETE FROM "Agents"`, `← result agent Priya Natarajan deleted · unassigned tickets 18 → 32 (+14, set to NULL by the database)`.
- **Delete a ticket with comments (Cascade)** → `DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;` (the token in the WHERE), `comments 16 → 15 (−1, cascaded by the database)`.
- **Reset & reseed** → five `DELETE FROM` (children first) in one context, then the seeder in a second: `seeded 5 customers, 3 agents, 6 categories, 60 tickets, 16 comments in 18 ms · 96 statement(s) · 2 context created, 2 disposed`.
- The Module 1 row (slow count, rapid ×3, break/restore, anti-pattern) behaves exactly as in Module 1.
- Note: after the pane sat idle for a few minutes Wisej.NET showed its **Session Timeout** dialog (the default idle timeout);
  pressing OK prolongs the session and the page keeps working. Not a bug in the sample.

