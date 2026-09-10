# SupportDesk · Data Binding with EF Core · Module 3

Local lab build for **Module 3 · Loading Data into BindingSource, DataGridView and Lookup Controls**. It
follows the walkthrough video: the Support Desk **ticket browser** arrives. `TicketListItem`,
`TicketSearchCriteria` and `PagedResult<T>` join the Services project, `SearchTicketsAsync` composes
`AsNoTracking`, one `Where` per filter that is set, `OrderByDescending(t => t.UpdatedAt)`, `Skip`, `Take`
and `Select` on a single `IQueryable` and executes it exactly twice, and the materialised
`List<TicketListItem>` is handed to a `BindingSource` that a `DataGridView` was bound to in the
constructor. The status and customer lookups load in `Load`, before the first search; Search, Next page and
Previous page each issue one new query behind a loading guard. The seed grew from sixty tickets to **312**,
so the status label reads *Showing 50 of 312 tickets · page 1 of 7 · page size 50* — the line the video
shows. Everything Module 1 and Module 2 could do still works, on the second and third button rows.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local
SQLite file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 3/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5403
```

Then open <http://localhost:5403>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web
project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the
first session: `MigrateAsync` applies `InitialCreate` (creating the file and `__EFMigrationsHistory` on
first start), then `DevelopmentSeeder` inserts five customers, three agents, six categories, **312
tickets** and 99 comments. Both results are printed on the server console:

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in 445 ms
```

> **Coming from Module 2?** Its `App_Data/supportdesk.db` has sixty tickets and four statuses. Module 3 has
> its own `App_Data` folder, so nothing is shared — but if you ever copy one over, delete it or press
> **Reset & reseed**, otherwise the browser will page through the old sixty.

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal` / `-shm` companions) to reset the
module completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — **40 tests** against SQLite in memory (the three Module 1 lifetime
tests, the Module 2 seeder / migration / schema-rule tests, and 22 new ones for the ticket browser:
totals and paging, every filter, the ordering, the two statements per search, the lookups, the projection
and the bound-`IQueryable` anti-pattern).

Migration commands, from the `Module 3` folder (`--framework` is required because the web project
multi-targets):

```bash
dotnet ef migrations list   --project SupportDesk.Data --startup-project SupportDesk.Web --framework net10.0
dotnet ef migrations script --idempotent --project SupportDesk.Data --startup-project SupportDesk.Web \
    --framework net10.0 --output artifacts/sql/supportdesk_migrations.sql
```

The model did not change in Module 3, so `InitialCreate` is still the only migration and the committed
script is unchanged.

## What to click

The **ticket browser card** carries the success path: type in `searchTextBox`, pick a status and a
customer, tick a due-date bound if you want one, then **Search**, **Next page ▶**, **◀ Previous page**.
Bottom bar **row 1** is the Module 3 lab props, **row 2** ("Module 2 · model") keeps every Module 2 path
working and **row 3** ("Module 1 · lifetimes") every Module 1 path. `Clear trace` is anchored right on
row 1.

| Action | Path | What you should see |
|---|---|---|
| *(page load)* | lookups first, then the first search | `• lookup status lookup: the fixed TicketStatuses.All list (5 values) — no statement is sent`, one `→ SQL SELECT "c"."Id", "c"."Name" FROM "Customers" AS "c" ORDER BY "c"."Name"`, then `◦ lookups statusComboBox 6 rows, customerComboBox 6 rows — filled BEFORE the first search`, and finally the first two-statement search. `statusLabel` → *Showing 50 of 312 tickets · page 1 of 7 · page size 50*, `◀ Previous page` disabled |
| **Search** (`searchButton`) with everything empty | success | two statements — `→ SQL SELECT COUNT(*) FROM "Tickets" AS "t"` and `→ SQL SELECT "t0"."Id", "t0"."Number", … LIMIT @p1 OFFSET @p …` — then `• service materialised 50 TicketListItem rows of 312 matching · 0 entities tracked (AsNoTracking) — the list outlives this context` and `← result Showing 50 of 312 tickets · page 1 of 7 · page size 50 · 2 statement(s) · … ms in the database · 1 context created, 1 disposed` |
| **Next page ▶** (`nextPageButton`) | paging in the database | `• paging nextPageButton_Click → page 2: one new query with OFFSET 50, not a cached copy of the result set`, then the same **two** statements with a larger `OFFSET`; the grid shows `SD-1051` … `SD-1100` and the label reads *page 2 of 7*. On page 7 the label reads *Showing 12 of 312 tickets · page 7 of 7* and `Next page ▶` is disabled |
| **◀ Previous page** (`prevPageButton`) | paging | the same, with a smaller `OFFSET`; disabled again on page 1 |
| Type `SD-104` and **Search** | index-friendly filter | the `COUNT` grows a `WHERE "t"."Number" LIKE @text_startswith ESCAPE '\' OR instr("t"."Title", @text) > 0 OR instr("c"."Name", @text) > 0`; `StartsWith` is the half `IX_Tickets_Number` can seek. `SD-10` alone matches **99** tickets, `printer` matches **20** |
| Pick a status, or a customer, or tick both due dates | filters composed in SQL | one extra clause per filter that is set, in both statements: `AND "t"."Status" = @status`, `AND "t"."CustomerId" = @customerId`, `AND "t"."DueDate" IS NOT NULL AND "t"."DueDate" >= @dueFrom`. Leave a filter alone and it does not appear at all — the empty search's `COUNT` has no `WHERE` |
| Search for something that matches nothing | empty result | `statusLabel` → *No tickets match these filters · page size 50*, the chip turns amber `● no matches`, the grid empties, both paging buttons are disabled — and it is still two statements |
| **Slow search (2.5 s)** (`buttonSlowSearch`), then **Search** while it runs | progress · guard | every button greys out and `statusLabel` reads *Loading tickets… (simulated 2.5 s latency)*; the click during the wait is logged as `• guard searchButton_Click: an operation is already running — this click is ignored`, sitting between `◦ context #n created` and the first `→ SQL`; `finally` restores the buttons |
| **Break the database** (`buttonBreak`) | failure · outage | `DevelopmentOutageSwitch.IsDown = true`, the search runs, the connection open throws: red banner and a top-right `AlertBox` *The Support Desk database is not reachable right now. Nothing was changed — please try again in a moment.*, `● fault`, trace `• caught DatabaseUnavailableException … → friendly message shown, full exception logged server-side`. The context is still disposed, the grid keeps the rows it had, and **Search is enabled again** |
| **Restore and search** (`buttonRestore`) | recovery | the switch goes off; the next click gets a **fresh** context and a working connection — an ordinary two-statement search, `● ok` |
| **Bind IQueryable (anti-pattern)** (`buttonBindQuery`) | anti-pattern | the query is built, the context disposed, then enumerated the way a bound grid would: `• caught ObjectDisposedException: Cannot access a disposed context instance.` and the banner *The grid was given the query instead of the rows: by the time it enumerated, the DbContext was gone. Execute with ToListAsync and bind the list.* Note there is **no `→ SQL` line at all** — the query never reached the database |
| **Seed development data** (`btnSeed`) | Module 2 · idempotence | already seeded at start, so the first click reports *nothing to seed: 312 tickets already exist — Tickets.AnyAsync() was true, so the seeder returned before AddRange*, the chip turns amber, and the trace shows two statements and **no** `INSERT`. After a **Reset & reseed** or a deleted database file it reports *seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in … ms* with 426 statements in one context |
| **Reset & reseed** (`buttonReset`) | Module 2 · lab prop | five `DELETE FROM "…"` (children first) in one context, then the seeder in a second one: `← result seeded … 312 tickets, 99 comments · 431 statement(s) · 2 context created, 2 disposed`. Search afterwards to see the browser back at 312 |
| **Save a 200-char title (fails)** (`buttonOverlongTitle`) | Module 2 · CHECK constraint | red banner *The ticket could not be saved because the database rejected the change.* and `→ SQL failed SqliteException: SQLite Error 19: 'CHECK constraint failed: CK_Tickets_Title_Length'.` |
| **Delete a customer with tickets (refused)** (`buttonDeleteCustomer`) | Module 2 · `Restrict` | `• service customer #1 'Halden Logistics' has 60 tickets — DELETE goes to the database, ON DELETE RESTRICT decides`, then `SqliteException: … 'FOREIGN KEY constraint failed'.` and the banner *This customer still has tickets and cannot be deleted.* |
| **Unassign an agent (SetNull)** (`buttonDeleteAgent`) | Module 2 · `SetNull` | one `DELETE FROM "Agents" …` and `← result agent 'Priya Natarajan' deleted · unassigned tickets 96 → 179 (+83, set to NULL by the database)`. Search again and the grid shows `— unassigned —` on many more rows |
| **Delete a ticket with comments (Cascade)** (`buttonDeleteTicket`) | Module 2 · `Cascade` | `→ SQL DELETE FROM "Tickets" WHERE "Id" = @p0 AND "RowVersion" = @p1 RETURNING 1;` — the concurrency token in the `WHERE` — then `← result ticket SD-1001 deleted · comments 99 → 97 (−2, cascaded by the database)` |
| **Count tickets** (`countButton`) | Module 1 | `statusLabel` → *312 tickets in the Support Desk database*; one `SELECT COUNT(*)`, one context created and disposed |
| **Slow count (2.5 s)** / **▶ Count ×3 rapid (guard)** | Module 1 · guard | unchanged: the first count runs, the guard drops the other two |
| **Two ops, one context (anti-pattern)** (`buttonAntiPattern`) | Module 1 · anti-pattern | unchanged: `• caught InvalidOperationException: A second operation was started on this context instance…` |
| **Clear trace** (`buttonClear`) | – | empties the right-hand list |

The right-hand card is the **Server ⇄ Database · EF Core lifetime & SQL trace**: every context created and
disposed, every SQL statement with its duration, every service note about what it decided, and every
handler decision. Under both cards, the Module 2 **Model & migration** panel is still re-read after every
operation (`rows Customers 5 · Agents 3 · Categories 6 · Tickets 312 · TicketComments 99`), and the
Module 1 **four lifetimes** table now also reports what the BindingSource is holding:

```
session state    Wisej.NET   id a1b2c3d4 · started 09:41:02
UI object        this Page   1 page, the grid, the trace list and a BindingSource holding 50 TicketListItem row(s)
request / thread one click   7 handler run(s) · continuations may resume on any thread
unit of work     DbContext   9 created · 9 disposed · 0 alive between clicks
```

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | `TicketListItem` and `TicketSearchCriteria` records with `SearchTicketsAsync` | [`docs/TicketSearchService.md`](docs/TicketSearchService.md) · `SupportDesk.Services/TicketBrowsing.cs`, `TicketQueryService.SearchTicketsAsync` |
| 2 | `DataGridView` bound to `TicketListItem` through a `BindingSource` | [`docs/TicketBrowserBinding.md`](docs/TicketBrowserBinding.md) · `TicketBrowserPage.Designer.cs` (`ticketBindingSource`, `ticketsDataGridView`, the nine columns), `LoadTicketsAsync` |
| 3 | Filtering and paging composed in the query so the database does the work | [`docs/PagingInTheDatabase.md`](docs/PagingInTheDatabase.md) · `TicketQueryService.RunSearchAsync` |
| 4 | Lookup ComboBoxes loaded before the first search, showing names and storing keys | [`docs/LookupComboBoxes.md`](docs/LookupComboBoxes.md) · `TicketQueryService.GetStatusesAsync` / `GetCustomersAsync`, `TicketBrowserPage.LoadLookupsAsync`, `TicketStatuses` |
| 5 | Search, Next Page and Previous Page buttons with a total count and a loading guard | [`docs/SearchPagingAndTheLoadingGuard.md`](docs/SearchPagingAndTheLoadingGuard.md) · `searchButton_Click` / `nextPageButton_Click` / `prevPageButton_Click` → `LoadTicketsAsync` → `RunAsync`, `UpdatePagingButtons` |

The Module 2 deliverables and their notes are unchanged and still in `docs/`:
[`ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md),
[`RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md),
[`MigrationWorkflow.md`](docs/MigrationWorkflow.md),
[`SeedData.md`](docs/SeedData.md) (updated for 312 tickets) and
[`SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md).

## Where things live

```
Module 3/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    dotnet ef migrations script --idempotent (unchanged since Module 2)
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    host: configuration → AddSupportDeskData → services → Build → IServiceProvider bridge → MigrateAsync + seed (Development) → UseWisej
│  ├─ Program.cs                    Wisej.NET session entry point: Application.MainPage = new TicketBrowserPage()
│  ├─ TicketBrowserPage.cs          the lab page: LoadLookupsAsync, ReadCriteria, LoadTicketsAsync, the guard, paging, the anti-pattern handler
│  ├─ TicketBrowserPage.Designer.cs Designer layout: the browser card (filters, grid, paging), the trace card, the Module 2 card, three button rows
│  ├─ SupportDeskDevelopmentDatabase.cs  Development only: MigrateAsync + DevelopmentSeeder at host start
│  ├─ appsettings.json              ConnectionStrings:SupportDesk = Data Source=App_Data/supportdesk.db
│  └─ Default.json / Default.html / Web.config / Properties/launchSettings.json (port 5403)
├─ SupportDesk.Data/                EF Core lives here (the only project referencing the provider)
│  ├─ SupportDeskContext.cs         the model: Fluent rules, indexes, delete behaviours, CHECK constraint, RowVersion stamping
│  ├─ Entities/                     Customer.cs, Agent.cs, Category.cs, Ticket.cs, TicketComment.cs — no mapping attributes
│  ├─ Migrations/                   20260910150534_InitialCreate(.Designer).cs + SupportDeskContextModelSnapshot.cs
│  ├─ SupportDeskDesignTimeFactory.cs / SupportDeskPaths.cs
│  ├─ DependencyInjection/…Extensions.cs  AddSupportDeskData: AddDbContextFactory + UseSqlite + dev diagnostics
│  └─ Diagnostics/                  QueryTrace (AsyncLocal sink), QueryTraceInterceptor, DevelopmentOutageSwitch
├─ SupportDesk.Services/
│  ├─ TicketBrowsing.cs             TicketListItem, TicketSearchCriteria, PagedResult<T>, LookupItem, TicketStatuses
│  ├─ TicketQueryService.cs         SearchTicketsAsync / SearchTicketsSlowlyAsync / GetStatusesAsync / GetCustomersAsync + the Module 1 counts
│  ├─ BoundIQueryableAntiPattern.cs deliberately wrong: the query bound instead of the list
│  ├─ DevelopmentSeeder.cs          312 deterministic tickets (seeded Random) + the idempotent seed and the reset
│  ├─ ModelDemoService.cs           the four Module 2 schema demos
│  ├─ SchemaInfoService.cs          what the Model & migration card shows
│  └─ SharedContextAntiPattern.cs   deliberately wrong: two threads on one context
├─ SupportDesk.Tests/               xunit, SQLite in memory — 40 tests
│  ├─ TicketSearchTests.cs          the Module 3 deliverable: totals, paging, filters, ordering, 2 statements, lookups, projection, anti-pattern
│  ├─ Support/SeededSupportDesk.cs  one seeded in-memory database shared by the read-only browser tests
│  ├─ TicketQueryServiceTests.cs    the three Module 1 tests
│  ├─ DevelopmentSeederTests.cs     counts (312), idempotence, realistic data, reset
│  ├─ MigrationTests.cs             MigrateAsync applies InitialCreate once, then the seeder fills it
│  └─ ModelRulesTests.cs            Restrict / SetNull / Cascade, RowVersion, unique number, title CHECK
└─ docs/                            the five Module 3 deliverables + the five Module 2 notes
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 2 solution, confirm `InitialCreate` is applied and the seed is present | `SupportDesk.slnx`; the **Model & migration** card reports `applied 1: 20260910150534_InitialCreate · pending 0` and the row counts on every operation |
| 2 · `TicketListItem`, `TicketSearchCriteria` and `PagedResult<T>` in `SupportDesk.Services` | `SupportDesk.Services/TicketBrowsing.cs` |
| 3 · `SearchTicketsAsync`: a context from the factory, `AsNoTracking`, one `Where` per filter, `CountAsync`, `OrderByDescending`, `Skip`, `Take`, `Select`, `ToListAsync` | `TicketQueryService.SearchTicketsAsync` → `RunSearchAsync` — see [`docs/PagingInTheDatabase.md`](docs/PagingInTheDatabase.md) |
| 4 · `ticketBindingSource` + `ticketsDataGridView`, `AutoGenerateColumns = false`, a `DataPropertyName` per column, the grid bound in the constructor | `TicketBrowserPage.Designer.cs` — `InitializeComponent` adds the nine columns and only then sets `ticketsDataGridView.DataSource = ticketBindingSource` |
| 5 · Lookups in `Load` with `DisplayMember`/`ValueMember` and an "All" entry, before the first search | `TicketBrowserPage.LoadLookupsAsync`, called before `LoadTicketsAsync` — see [`docs/LookupComboBoxes.md`](docs/LookupComboBoxes.md) |
| 6 · `LoadTicketsAsync` with the guard: early return on `_loading`, buttons off, status label, criteria from `SelectedValue`, one awaited call, `result.Items.ToList()` to the BindingSource, restored in `finally` | `TicketBrowserPage.LoadTicketsAsync` + `RunAsync` + `ReadCriteria` |
| 7 · Wire Search (reset the page index), Next and Previous; show returned / total / page size; disable Previous on page 1 and Next on the last | `searchButton_Click`, `nextPageButton_Click`, `prevPageButton_Click`, `UpdatePagingButtons`, `const int PageSize = 50` |
| 8 · Confirm each search sends exactly two statements, a `COUNT` and a paged `SELECT` with the filters and `LIMIT`/`OFFSET` | the trace card, and `TicketSearchTests.A_search_sends_exactly_two_statements_a_COUNT_and_a_paged_SELECT` |
| 9 · Show every path: loading, an empty result, and a failed database call — without exposing the connection string or the SQL — and search again afterwards | `buttonSlowSearch` · a filter that matches nothing · `buttonBreak` → `buttonRestore`; `Fail()` shows business language in the banner and a top-right `AlertBox` and puts the exception in the server-side trace |
| 10 · Review & run: page forward and back watching the SQL log, and write the DbContext-lifetime note | **Next page ▶** / **◀ Previous page** with the trace open, and **Self-check answers** below |

Lab code check (`labs.js` m3): an event handler, async execution (`ToListAsync` / `CountAsync` / `await`),
projected items bound through a `BindingSource` (`BindingSource` / `TicketListItem` / `AsNoTracking` /
`.Select(`), a `try`/`catch` on the failure path, and a guarded load that pages in the database
(`_loading` / `finally` / `.Skip(` / `.Take(`) — `searchButton_Click` → `LoadTicketsAsync` → `RunAsync` →
`TicketQueryService.SearchTicketsAsync` covers all five.

## Self-check answers (the lab's student review questions)

- **Which lifetime did you choose for the `DbContext` inside `SearchTicketsAsync`, and what would break if
  the page kept one context alive for its whole life?**

  One context **per search**: `await using var db = await _dbFactory.CreateDbContextAsync(token);` is the
  first line of the method and the context is disposed before it returns — the page never holds one, never
  sees one, and the trace proves it (`◦ context #n created` … `◦ context #n disposed (0 tracked entities
  released)` inside every single click, `0 alive between clicks` in the lifetimes table).

  A page-lived context would break in four escalating ways. *First*, concurrency: EF Core allows exactly
  one operation at a time on an instance, so the operator who clicks **Search** while **Next page** is
  still running gets `InvalidOperationException: A second operation was started on this context instance
  before a previous operation completed` — the failure the Module 1 button on row 3 reproduces on demand.
  The `_loading` guard would become load-bearing rather than a courtesy, and any code path that forgot it
  would be a crash. *Second*, memory: the change tracker is per instance, so every entity any query ever
  materialised stays referenced for the life of the tab. Multiply by the number of open browsers — in
  Wisej.NET each tab is a separate server-side session — and a screen that "works on my machine" becomes a
  memory profile that grows all day. (`AsNoTracking` mitigates this for the browser but not for the
  editor.) *Third*, staleness: a tracked entity is returned from the identity map on the next query, so
  the grid would keep showing values another operator overwrote ten minutes ago, and the concurrency token
  in Module 7 would be checked against a snapshot nobody can explain. *Fourth*, recovery: **Break the
  database** puts a context into a failed state; a per-operation context is disposed and the next click
  simply gets a new one, which is exactly what **Restore and search** demonstrates. A shared context would
  have to be detected as poisoned and rebuilt by hand, and until it was, every subsequent click would fail
  for a reason that had nothing to do with the click.

  The cost of the choice is one `CreateDbContextAsync` per operation — an object allocation and a pooled
  connection open, measured in fractions of a millisecond in the trace. That is the whole bill.

- **After a search, which data is held on the server for this session, and which stays only in the
  database? What does a second concurrent operator cost?**

  Held for the session: the `TicketBrowserPage` itself with all its controls, the `ticketBindingSource`,
  the **fifty `TicketListItem` records** it currently holds, the two lookup lists (five statuses plus an
  "All" row; five customers plus an "All" row), the current row the operator selected, `_pageIndex` and
  `_totalCount`, and the trace list. That is the lifetimes table's `UI object` row: *a BindingSource
  holding 50 TicketListItem row(s)*. Each record is ten fields — three short strings joined by the
  database, four more short strings, two dates and an int — so the page is holding a few tens of kilobytes
  of rows, not a table.

  Only in the database: the other 262 tickets, and on every ticket the `Description` (up to 4000
  characters), `RowVersion`, `CreatedAt`, `IsUrgent`, the foreign keys, the `Comments` collection and the
  `Customer`, `Agent` and `Category` entities behind the three names. None of it was selected, so none of
  it crossed the wire. Also not held: any `DbContext`, any change-tracker snapshot (`AsNoTracking`), and
  any query — the BindingSource has a list, so nothing on the page can start a database operation by being
  scrolled.

  A second operator costs a second copy of *that*: their own session, page, BindingSource, fifty records
  and lookup lists — a few tens of kilobytes plus the Wisej.NET session overhead — and, while they are
  actually clicking, one short-lived `DbContext` and one pooled connection. Nothing is shared and nothing
  is contended, which is the point: the per-session cost is bounded by the **page size**, not by the size
  of the table, so 312 tickets and 300 000 tickets cost the same per operator. The counter-example is one
  click away — a page that loaded every ticket as a tracked entity would cost each operator the whole
  table plus a snapshot of it, and ten operators would cost ten copies.

- **If the customer lookup grew to fifty thousand rows, which parts of the page would you change and which
  parts of the query service would stay exactly as they are?**

  **`SearchTicketsAsync` does not change at all.** It takes `int? CustomerId` and turns it into
  `Where(t => t.CustomerId == customerId)`; where that integer came from — a ComboBox, an autocomplete, a
  lookup dialog, a URL — was never its business. Neither the projection, the two statements, the paging nor
  the ordering are affected. `TicketSearchCriteria` does not change either. That is the payoff of putting
  the key, not the display text, into the criteria in the first place.

  **`GetCustomersAsync` changes shape**, because "return every customer" stops being a sensible request. It
  becomes a *searching* lookup — the same pattern as the ticket search, one level down:
  `SearchCustomersAsync(string text, int take = 20)` with `Where(c => c.Name.StartsWith(text))`,
  `OrderBy(c => c.Name)`, `Take(take)`, `Select` into `LookupItem`, `AsNoTracking` —
  plus a `GetCustomerAsync(int id)` for the one row needed to *display* a customer the criteria already
  carry (a filter restored from a saved view, or the customer of the row being edited in Module 4).
  `IX_Customers_Name` from Module 2 already makes the prefix seek cheap.

  **The page changes most.** `customerComboBox` stops being `DropDownList` with a bound list — fifty
  thousand items is unusable for a human and a serious payload for the browser — and becomes either a
  `DropDown` ComboBox with `AutoComplete` querying on keystroke (debounced, `Take(20)`, behind the same
  loading discipline), or a small "Choose customer…" button opening a dialog with its own paged grid, the
  ticket browser in miniature. Either way the page keeps a single `int? _customerId` plus the label to show
  for it, `ReadCriteria()` reads that field instead of `SelectedValue`, and `LoadLookupsAsync` stops
  loading customers up front — it can no longer be "load everything before the first search", so the
  browser must be able to search with no customer chosen, which it already can. The status lookup is
  untouched: five fixed values will still be five fixed values.

  The general rule the change illustrates: a lookup is only a ComboBox while it is small. The **service
  boundary** — keys in, projected rows out, one operation per context — is what lets the UI be rewritten
  without touching the query.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx -nologo -v q` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests -nologo -v q` — **40 passed**, 0 failed.
- The SQL quoted in this README and in `docs/` was captured from the real services running against SQLite
  in memory (a console check and the tests, the same code paths the page calls), not written from memory:
  the two statements per search, `LIMIT @p OFFSET @p`, `LIKE @text_startswith ESCAPE '\'`,
  `instr("t"."Title", @text) > 0`, the `LEFT JOIN "Agents"`, and
  `SELECT "c"."Id", "c"."Name" FROM "Customers" AS "c" ORDER BY "c"."Name"`.
- The seed numbers are measured, not estimated: 312 tickets and 99 comments in 426 statements and one
  context; a second seed is 2 statements and no `INSERT`; **Reset & reseed** is 431 statements in two
  contexts. Status distribution `Open` 93, `In Progress` 69, `Resolved` 65, `Closed` 53, `Waiting` 32;
  96 tickets unassigned; 214 with a due date, 61 of them overdue; `UpdatedAt` distinct on all 312 rows.
- Filter totals quoted above are measured: empty 312, `SD-10` 99, `printer` 20.
- The bound-`IQueryable` anti-pattern really throws `ObjectDisposedException: Cannot access a disposed
  context instance.` — it is executed, not described.

Facts carried over from Module 1 and Module 2 and still relied on here:
`Application.Services.AddService<IServiceProvider>(app.Services)` makes `[Inject]` on a Page resolve
through Microsoft DI, and application services must be **Transient** or Singleton because Wisej.NET asks
the **root** provider; after an `await` the handler continues off the original request, so
`Application.Update(this)` in `finally` pushes the final state to the browser.

**Not verified here — for the browser reviewer.** The application was not started (by instruction), so
everything below is Wisej.NET behaviour that only a running page can confirm:

1. The page renders at 1700 × 1130 with `AutoScroll`: the browser card, the trace card, the Module 2 card
   and the three button rows all fit and are readable.
2. `ticketsDataGridView.DataSource = ticketBindingSource` assigned in `InitializeComponent` (with the nine
   columns added first and the BindingSource still empty) constructs without throwing, and the columns bind
   by `DataPropertyName` when the first list arrives.
3. `AutoSizeColumnsMode = Fill` with the `FillWeight` values gives a readable Title column at 892 px.
4. `colDueDate` renders `yyyy-MM-dd` and blank for null; `colAgentName` renders `— unassigned —` for null
   (`DefaultCellStyle.NullValue`); `colUpdatedAt` renders `yyyy-MM-dd HH:mm`.
5. `ComboBox.SelectedValue` returns the boxed `ValueMember` value (`string` for `statusComboBox`, `int` for
   `customerComboBox`) for a `DropDownList` bound to a `List<T>` of records, so `is string` / `is int`
   match and the "All" rows really yield null criteria.
6. `DateTimePicker.ShowCheckBox = true` + `Checked` behaves as "unticked = no filter" (flagged
   **unverified** in the cookbook), and `Format = Short` shows a short date.
7. `searchTextBox.Watermark` shows the placeholder text.
8. The guard is visible: clicking **Search** during **Slow search (2.5 s)** is delivered by Wisej.NET and
   produces the `• guard …` line, and the buttons visibly grey out and come back.
9. `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` appears top-right on the failure paths
   and closes itself.
10. `Application.Update(this)` in `finally` pushes the final grid, status label and trace to the browser
    after the awaited continuation resumes off the original request.
11. The reviewer-visible numbers: *Showing 50 of 312 tickets · page 1 of 7 · page size 50* on load,
    *page 7 of 7* with 12 rows at the end, `◀ Previous page` disabled on page 1 and `Next page ▶` disabled
    on page 7.

**Browser verification: see the note at the end.**

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5403> in the Browser pane; trace read back from the page:

- Page load: lookups first (`• lookup status lookup … no statement is sent`, one `SELECT "c"."Id", "c"."Name" FROM "Customers"`, `◦ lookups statusComboBox 6 rows, customerComboBox 6 rows — filled BEFORE the first search`), then the first two-statement search.
  **Found and fixed:** a `DateTimePicker` with `ShowCheckBox = true` starts **ticked** in Wisej.NET, so the first load was filtered
  (`WHERE "t"."DueDate" IS NOT NULL AND "t"."DueDate" >= @dueFrom AND … <= @dueTo`, *Showing 50 of 61*). The Designer now sets
  `Checked = false` on both pickers; after the fix the first load reads *Showing 50 of 312 tickets · page 1 of 7 · page size 50*.
- **Search** with nothing set → `SELECT COUNT(*) FROM "Tickets" AS "t"` (no WHERE) + the projected `SELECT … LIMIT @p OFFSET @p` with the
  two INNER JOINs and the LEFT JOIN on Agents; `• service materialised 50 TicketListItem rows of 312 matching · 0 entities tracked`;
  `← result Showing 50 of 312 tickets · page 1 of 7 · page size 50 · 2 statement(s) · 1 context created, 1 disposed`.
- **Next page ▶** → `• paging nextPageButton_Click → page 2: one new query with OFFSET 50`, the same two statements, *page 2 of 7*.
- **Slow search (2.5 s)** → `• service simulated latency of 2.5 s inside the unit of work`, every button greyed out for 2.5 s, then the two statements.
  A click on **Search** while it ran was **not delivered at all** because the button was disabled — the `_loading` guard is the second line of defence
  (the Module 1 "Count ×3 rapid" button shows it firing when the clicks come from code).
- **Break the database** → `• caught DatabaseUnavailableException … friendly message shown`, red banner + top-right AlertBox, grid keeps its rows,
  `◦ card Model & migration card not refreshed: DatabaseUnavailableException`; **Restore and search** → a normal two-statement search, `● ok`.
- **Bind IQueryable (anti-pattern)** → no `→ SQL` line at all, then `• caught ObjectDisposedException: Cannot access a disposed context instance.`
  and the explanation lines; the banner reads as documented.
- The grid renders nine columns (`— unassigned —` for null agents, `yyyy-MM-dd` due dates), the page fits at 1700 × 1130 with `AutoScroll`,
  and the Module 2 / Module 1 rows behave as in their own modules.

