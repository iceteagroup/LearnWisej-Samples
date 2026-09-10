# SupportDesk · Data Binding with EF Core · Module 6

Local lab build for **Module 6 · Async loads, related data, filtering and performance**. It follows the
walkthrough video: the ticket browser that worked fine with fifty seed tickets stops scaling the moment a
screen's `CellFormatting`-style code starts reading navigation properties per row — Module 6 measures that
cost with a log instead of a hunch, keeps the naive version in the solution as a named, tested anti-pattern
(`TicketQueryService.SearchTicketsNaiveAsync`), and proves the fix (the existing Module 3
`SearchTicketsAsync`) is faster with numbers, not adjectives. A **Before / after card** on the page shows the
last measurement of each branch — statements, database milliseconds, tracked entities, rows — with a **×**
improvement factor computed straight from the trace scope's own counters. A new `TicketDetailService` is the
module's related-data decision table in code: `Include` for the editor's controlled aggregate, a filtered
read for the last five comments, explicit loading for the full history, never lazy loading — there is none
anywhere in this solution. A **"Long job (background)"** button runs a report-style task through
`Application.StartTask`, with its own `DbContext` and progress pushed through `Application.Update`. Everything
Module 1–5 could do still works, on the second, third, fourth and fifth button rows.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 solution on this machine with a local SQLite
file.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Data Binding with EF Core Course/Module 6/SupportDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5406
```

Then open <http://localhost:5406>. (Visual Studio: open `SupportDesk.slnx`, press F5 — the port and the
Development environment are in `SupportDesk.Web/Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package, EF Core 10.0.12
(`Microsoft.EntityFrameworkCore.Sqlite` + `Design`) and the global `dotnet-ef` 10.0.12 tool. The web
project multi-targets `net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

In Development the host **migrates and seeds** `SupportDesk.Web/App_Data/supportdesk.db` before the first
session, exactly as in Modules 2–5 — the model did not change in Module 6, so `InitialCreate` is still the
only migration and the committed script under `artifacts/sql/` is unchanged:

```
[SupportDesk] MigrateAsync: 1 pending migration(s) applied, 1 applied in total (20260910150534_InitialCreate)
[SupportDesk] Development seed: seeded 5 customers, 3 agents, 6 categories, 312 tickets, 99 comments in ... ms
```

In Development the server console also now carries a second log channel, next to the migration/seed lines
above: `[EF] Executed DbCommand (…ms) …` for every SQL statement — see
[`docs/LoggingInDevelopment.md`](docs/LoggingInDevelopment.md).

> **Coming from Module 5?** Its `App_Data/supportdesk.db` already has data. Module 6 has its own `App_Data`
> folder, so nothing is shared — but if you ever copy one over, delete it or press **Reset & reseed**.

Delete `SupportDesk.Web/App_Data/supportdesk.db` (and its `-wal`/`-shm` companions) to reset the module
completely; **Reset & reseed** on the page does the same thing without a restart.

Tests: `dotnet test SupportDesk.Tests` — **83 tests** against SQLite in memory (the 68 carried over from
Modules 1–5 unchanged, plus 6 new in `TicketPerformanceTests.cs`, 7 new in `TicketDetailServiceTests.cs`'s
own class and 2 new in `BackgroundJobTests`, nested in the same file — 15 new tests in total).

## What to click

The **ticket browser card** and the **editor's fields** are unchanged from Module 5. A new **Before / after
card** sits under the ticket grid, left column: it starts with a neutral prompt and fills in once either
branch button has run at least once. Bottom bar **row 1** is now the Module 6 performance paths (four new
lab controls) plus **Slow search (2.5 s)** / **Break the database** / **Restore and search** — moved here
from their old rows since this row's own buttons open the same failure and progress paths — and **Clear
trace**, always anchored right on the newest module's row; **row 2** ("Module 5 · validation") keeps the five
negative-case demos working, with **Slow save (2.5 s)** moved onto it; **row 3** ("Module 3+4 · editor")
merges the former Module 3 and Module 4 rows — Add/Edit/Simulate delete plus the bound-`IQueryable`
anti-pattern — into one row so the page does not grow a sixth row for one module's worth of new buttons;
**row 4** ("Module 2 · model") and **row 5** ("Module 1 · lifetimes") are unchanged in content, just
renumbered down. Every Module 1–5 button still does exactly what its own module's README describes.

| Action | Path | What you should see |
|---|---|---|
| **Run naive search** (`btnNaiveSearch`) | performance · anti-pattern | Runs `TicketQueryService.SearchTicketsNaiveAsync` with whatever filters are set. Trace: `◦ context created`, one `→ SQL` line for the whole matching set (no `LIMIT`, no `WHERE` unless a filter is set), then one `→ SQL` line **per distinct customer/agent/category** the capped 50 rows had not already seen (14 more on the default no-filter search — **15 statements total**, not the "1 + 3 × 50" the lesson illustrates; see the callout below), `← result Naive: 50 row(s) shown of 312 matching (no SQL paging) · 326 tracked entities`. The Before / after card's **naive** column fills in |
| **Run optimised search** (`btnOptimisedSearch`) | performance · the fix | Runs the existing `TicketQueryService.SearchTicketsAsync` with the same filters. Trace: `◦ context created`, exactly **two** `→ SQL` lines (`SELECT COUNT(*) …` then the paged, projected `SELECT`), `← result Optimised: 50 row(s) shown of 312 matching · page 1 of 7`. The Before / after card's **optimised** column fills in, and the **×** improvement factors appear once both columns have a value |
| **Compare (both, same filters)** (`btnCompare`) | performance · head to head | Runs the naive branch, then the optimised branch, with whatever filters are on the controls right now — both trace blocks appear in order, the card ends up showing both, the grid ends up showing the optimised branch's rows (it ran last) |
| **Long job (background)** (`btnLongJob`) | progress · background task | `labelLongJob` (under the Before / after table) starts at "starting…", then steps through the five statuses one at a time (`step 1/5 — Open: 93 ticket(s)` … `step 5/5 — Closed: 53 ticket(s)` on the fresh seed), each step logged to the trace too, ending "done — counted tickets in every status". The button disables itself for the run and re-enables when it finishes; every other button on the page stays usable while it runs — `Application.StartTask` keeps its own `DbContext`, created inside the task, never the page's (the page never has one) |
| **Slow search (2.5 s)**, **Break the database**, **Restore and search** | unchanged, moved to row 1 | See the Module 3 README — same buttons, same behaviour, new position (Break/Restore are global switches; where their button sits on the page does not change what they affect) |
| **Edit an existing ticket** | related-data decision table | The dialog's new **Comments** panel (below the "Lab: reuse an existing ticket number" checkbox) shows up to five recent comments, newest first (`TicketDetailService.LoadRecentCommentsAsync` — filtered, no-tracking, one statement), or "No comments yet." Trace: `• editor comments panel: TicketDetailService.LoadRecentCommentsAsync(#…) — N shown (the last 5)` |
| **Show full history** (`btnShowFullHistory`, inside the editor) | related-data · explicit loading | Only enabled once an existing ticket's comments have loaded. Click it: the list replaces itself with every comment on the ticket (`TicketDetailService.LoadAllCommentsAsync` — a tracked ticket read plus one explicit `Collection(...).LoadAsync()`, two statements). Trace: `• editor comments panel: "Show full history" → TicketDetailService.LoadAllCommentsAsync(#…) — N shown (explicit load, 1 extra statement beyond the ticket read)` |
| **Add ticket** | related-data · new ticket | Comments panel shows "Comments appear here once the ticket is saved." and **Show full history** stays disabled — there is no ticket id yet for either query to run against |
| Add ticket / Edit ticket / Delete / Cancel / Simulate delete / the five Module 5 validation demos / Search / paging / Module 1–3 buttons | unchanged | See the Module 1–5 READMEs — every path still works |

The right-hand card is unchanged: **Server ⇄ Database · EF Core lifetime & SQL trace**. Every naive search,
optimised search, comments-panel load and background-job step reaches this list the same way a save or a
search always has.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | EF Core logging enabled in development with the unoptimised SQL and timing recorded | [`docs/LoggingInDevelopment.md`](docs/LoggingInDevelopment.md) · `SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`, `SupportDesk.Web/appsettings.Development.json` |
| 2 | Grid query rewritten as a no-tracking projection | [`docs/NoTrackingProjection.md`](docs/NoTrackingProjection.md) · `SupportDesk.Services/TicketQueryService.cs` (`SearchTicketsAsync`, unchanged since Module 3, measured next to the new naive branch) |
| 3 | Paging with a page size of 50 and indexed filters | [`docs/PagingAndIndexedFilters.md`](docs/PagingAndIndexedFilters.md) · `TicketQueryService.ApplyFilters` (shared by both branches), `RunSearchAsync` |
| 4 | No lazy-loading N+1 behaviour left in display code | [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md) · `SupportDesk.Services/TicketDetailService.cs`, `SupportDesk.Web/TicketEditorForm.cs` (`RenderComments`) |
| 5 | Lab notes documenting the measured before-and-after improvement | [`docs/BeforeAfterMeasurements.md`](docs/BeforeAfterMeasurements.md) |

The Module 1–5 deliverables and their notes are unchanged and still in `docs/`:
[`ModelAndDeleteBehaviours.md`](docs/ModelAndDeleteBehaviours.md), [`RowVersionAndIndexes.md`](docs/RowVersionAndIndexes.md),
[`MigrationWorkflow.md`](docs/MigrationWorkflow.md), [`SeedData.md`](docs/SeedData.md),
[`SchemaVsUiValidation.md`](docs/SchemaVsUiValidation.md), [`TicketSearchService.md`](docs/TicketSearchService.md),
[`TicketBrowserBinding.md`](docs/TicketBrowserBinding.md), [`PagingInTheDatabase.md`](docs/PagingInTheDatabase.md),
[`LookupComboBoxes.md`](docs/LookupComboBoxes.md), [`SearchPagingAndTheLoadingGuard.md`](docs/SearchPagingAndTheLoadingGuard.md),
[`EditorFormAndBindingSource.md`](docs/EditorFormAndBindingSource.md), [`ControlDataBindings.md`](docs/ControlDataBindings.md),
[`LoadAndSaveFlows.md`](docs/LoadAndSaveFlows.md), [`DeleteConfirmationAndReload.md`](docs/DeleteConfirmationAndReload.md),
[`DialogResultAndGridRefresh.md`](docs/DialogResultAndGridRefresh.md), [`ValidationLayers.md`](docs/ValidationLayers.md),
[`ErrorProviderFeedback.md`](docs/ErrorProviderFeedback.md), [`CrossFieldRuleAndSaveGating.md`](docs/CrossFieldRuleAndSaveGating.md),
[`DuplicateNumberHandling.md`](docs/DuplicateNumberHandling.md), [`NegativeTests.md`](docs/NegativeTests.md).

## Where things live

```
Module 6/
├─ SupportDesk.slnx                 the four projects
├─ artifacts/sql/
│  └─ supportdesk_migrations.sql    unchanged since Module 2 — the model did not change in Module 6
├─ SupportDesk.Web/                 the Wisej.NET application (net10.0-windows;net10.0)
│  ├─ Startup.cs                    + AddTransient<TicketDetailService>()
│  ├─ TicketBrowserPage.cs          + btnNaiveSearch/btnOptimisedSearch/btnCompare/btnLongJob_Click,
│  │                                  RunNaiveSearchAsync, RunOptimisedSearchAsync, RenderBeforeAfterCard,
│  │                                  BranchMeasurement, RunAsync gained an optional onMeasured callback
│  ├─ TicketBrowserPage.Designer.cs + panelBeforeAfter (the Before / after card); panelActions row 1 =
│  │                                  Module 6 (4 new buttons + Slow search/Break/Restore/Clear); row 2 =
│  │                                  Module 5 validation + Slow save; row 3 = Module 3+4 merged; row 4/5 =
│  │                                  Module 2/1, unchanged inside, renumbered down; page grown to 2040×1460
│  ├─ TicketEditorForm.cs           + Details injection, LoadRecentCommentsAsync, btnShowFullHistory_Click,
│  │                                  RenderComments — called once from LoadEditorAsync
│  └─ TicketEditorForm.Designer.cs  + lblCommentsCaption, lstComments, btnShowFullHistory; Delete/Cancel/Save
│                                     row moved down, form grown to 620×710
├─ SupportDesk.Data/
│  └─ DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs  + LogTo(DbLoggerCategory.Database.Command, Information)
│     appsettings.Development.json (Web)  + Microsoft.EntityFrameworkCore.Database.Command: Information
├─ SupportDesk.Services/
│  ├─ TicketQueryService.cs         + SearchTicketsNaiveAsync/RunSearchNaiveAsync (the anti-pattern),
│  │                                  ApplyFilters (factored out, shared by both branches),
│  │                                  CountTicketsPerStatusAsync (the background job)
│  ├─ TicketDetailService.cs        NEW — LoadForEditorAsync (Include), LoadRecentCommentsAsync (filtered,
│  │                                  identity resolution), LoadAllCommentsAsync (explicit loading)
│  ├─ TicketBrowsing.cs             + NaiveSearchResult record
│  └─ (TicketCommandService.cs / DevelopmentSeeder.cs / ModelDemoService.cs / TicketValidator.cs / ... — unchanged)
├─ SupportDesk.Tests/               xunit, SQLite in memory — 83 tests
│  ├─ TicketPerformanceTests.cs     NEW — naive vs optimised: statement counts, tracked entities, same rows
│  │                                  same order, the null-FK fix-up fact, a filtered naive run
│  ├─ TicketDetailServiceTests.cs   NEW — the three related-data methods' statement counts, plus
│  │                                  BackgroundJobTests (own context, all 5 statuses, counts sum to the total)
│  └─ (everything from Modules 1–5 — unchanged, still green)
└─ docs/                            the five Module 6 deliverables + the twenty Module 1–5 notes
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the Module 5 solution; switch the ticket browser to a naive branch that loads tracked tickets with no paging and reads related names through navigation properties, like `CellFormatting` would | `SupportDesk.Services/TicketQueryService.cs` — `SearchTicketsNaiveAsync`/`RunSearchNaiveAsync`, kept in the solution next to the optimised branch rather than replacing it — see [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md) |
| 2 · Enable EF Core logging (`LogTo` or Microsoft.Extensions.Logging), keep `EnableSensitiveDataLogging` behind `IsDevelopment`, run the naive search once and record statements/ms/SQL | `SupportDesk.Data/DependencyInjection/SupportDeskDataServiceCollectionExtensions.cs`; `SupportDesk.Web/appsettings.Development.json` — see [`docs/LoggingInDevelopment.md`](docs/LoggingInDevelopment.md) |
| 3 · Rewrite the grid query as a no-tracking projection (`AsNoTracking`, composed filters, `OrderByDescending`, `Select` into `TicketListItem`, `ToListAsync`) | Already `TicketQueryService.SearchTicketsAsync`/`RunSearchAsync` since Module 3 — see [`docs/NoTrackingProjection.md`](docs/NoTrackingProjection.md) |
| 4 · Add paging with a page size of 50: `CountAsync` for the total, then `Skip`/`Take` before the projection, as a `PagedResult<TicketListItem>` | Already in `RunSearchAsync` since Module 3; the naive branch's own (lack of) paging is the deliberate contrast — see [`docs/PagingAndIndexedFilters.md`](docs/PagingAndIndexedFilters.md) |
| 5 · Every filter index-friendly: `StartsWith` prefix match on Number, Status/DueDate/CustomerId/UpdatedAt hit the Module 2 indexes | `TicketQueryService.ApplyFilters` — factored out in Module 6 so both branches share exactly the same filters — see [`docs/PagingAndIndexedFilters.md`](docs/PagingAndIndexedFilters.md) |
| 6 · Remove every navigation access from display code; load the editor's related data with `Include`, a filtered `Include`/equivalent, or explicit loading | `SupportDesk.Services/TicketDetailService.cs` (all three methods); `SupportDesk.Web/TicketEditorForm.cs` (`RenderComments` reads only `CommentSummary` fields) — see [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md) |
| 7 · Keep the async guard honest: await each operation before the next on the same context, disable Search/Next/Previous/the branch buttons while a page loads, re-enable in `finally`; long jobs get their own context and `Application.StartTask` | `TicketBrowserPage.SetBusy` (gained `btnNaiveSearch`/`btnOptimisedSearch`/`btnCompare`), `btnLongJob_Click` — one context created inside the task, never the page's |
| 8 · Show every path: loading state, empty page, no-results search, failed query, without exposing SQL | Unchanged failure/empty-result handling from Module 3 (`RunAsync`'s catch blocks, `"No tickets match these filters"`), now shared by the naive and optimised branch methods too |
| 9 · Review & run: confirm the optimised search still shows two statements for a page of 50, write the after numbers beside the before numbers, note which optimisations were not applied because the log gave no reason to | **Self-check answers** below; [`docs/BeforeAfterMeasurements.md`](docs/BeforeAfterMeasurements.md) — no index was added in Module 6 because the log never showed a scan the existing indexes did not already cover |

## Self-check answers (the lab's student review questions)

- **Your log showed 151 statements for one screen of 50 tickets. Which line of code caused the 150 extra
  ones, and why did nothing in the application warn you?**

  The line is the per-row loop inside `TicketQueryService.RunSearchNaiveAsync`:

  ```csharp
  await db.Entry(t).Reference(x => x.Customer).LoadAsync(token);
  await db.Entry(t).Reference(x => x.Agent).LoadAsync(token);
  await db.Entry(t).Reference(x => x.Category).LoadAsync(token);
  ```

  three explicit loads per row, run once for every one of the fifty capped rows — exactly the queries a
  `CellFormatting` handler reading `ticket.Customer.Name` would have triggered one at a time under lazy
  loading, except this solution never turns lazy loading on (`UseLazyLoadingProxies` is never added), so
  these three lines are written out loud instead of hidden inside a property getter. Nothing warned the
  developer for the same reason nothing would have warned them under lazy loading either: every one of these
  calls succeeds, returns a value, and looks exactly like ordinary code — `await db.Entry(t).Reference(...).LoadAsync()`
  is not an error, a warning, or even unusual EF Core usage in isolation. The only place the cost becomes
  visible is a log of how many statements one screen actually sent, which is precisely why Module 6 turns
  `LogTo` on and keeps `QueryTrace` reporting to the page's trace card — profiling, not code review, is what
  catches this.

  **On this solution's own seed the measured number is not 151, it is 15** — a fact worth stating plainly
  rather than quietly matching the lesson's number, because the reason is itself worth knowing: EF Core's
  automatic reference fix-up resolves a foreign key from the tracked graph, with zero database round trips,
  the moment any earlier row has already loaded the same related entity in the same context (verified
  directly — `SupportDesk.Tests/TicketPerformanceTests.cs`,
  `A_reference_load_for_a_null_foreign_key_sends_nothing`, and the statement-count test next to it). This
  seed has only 5 customers, 3 agents and 6 categories behind 312 tickets, so only the first occurrence of
  each of those fourteen values among the capped fifty rows costs a statement — `1 + 14 = 15`. Measured
  separately, forcing every per-row lookup through an independent query instead of the fix-up-aware
  `Entry(...).Reference(...)` call produces **131** statements for the same fifty rows — close to the
  lesson's 151, and the number a support desk with thousands of real, distinct customers would actually
  see, because at that scale almost no row's customer has already been loaded by an earlier row. The 15 on
  this seed is not evidence the anti-pattern is smaller here; it is evidence that a demo with a small lookup
  table can accidentally hide most of an N+1 problem's cost from a quick look at the numbers — which is why
  `docs/RelatedDataDecisions.md` and `docs/BeforeAfterMeasurements.md` both report the 131 number next to the
  15, not instead of it. Full detail: [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md#why-the-measured-statement-count-is-not-1--3--rows).

- **The editor needs the ticket, its customer and category, and the last five comments; the comments panel
  opens the full history on demand. Which loading strategy serves each of those three needs, and why is
  projection the wrong answer for the editor?**

  Three needs, three strategies, all in `TicketDetailService`:

  - **The ticket, its customer and category** — `LoadForEditorAsync`, `Include(t => t.Customer).Include(t =>
    t.Category)`, tracked, one statement. This is a fixed-size, controlled aggregate: it never grows as the
    database grows, so eager-loading it with the root is both correct and cheap.
  - **The last five comments** — `LoadRecentCommentsAsync`, a filtered read equivalent to a filtered
    `Include(t => t.Comments.OrderByDescending(c => c.CreatedAt).Take(5))`, no-tracking, one statement. This
    is a collection that *can* grow without bound over a ticket's lifetime, so only asking for a small,
    ordered slice — never the whole thing — is what keeps this call cheap regardless of how many comments a
    ticket eventually accumulates.
  - **The full history, on demand** — `LoadAllCommentsAsync`, explicit loading
    (`db.Entry(ticket).Collection(t => t.Comments).LoadAsync()`), only runs when `btnShowFullHistory` is
    clicked. This is the textbook case for explicit loading: a detail the screen does not need until the
    user specifically asks for it, so nothing is paid until they do.

  **Why projection is the wrong answer for the editor, specifically:** the ticket browser's grid projects to
  `TicketListItem` because the grid is read-only — nothing it shows will ever flow into a `SaveChangesAsync`
  call. The editor is the opposite: `TicketCommandService.SaveAsync` loads a **tracked** `Ticket` by key in
  its own context and writes the approved fields back onto it. A flat DTO like `TicketListItem` can never
  become that write — it has no identity EF Core's change tracker recognises, no way to be attached, and no
  path back to a row in the database. Projection is the right choice exactly where nothing downstream will
  ever save the result; the editor is precisely the screen where something will, which is also why
  `LoadForEditorAsync` deliberately stays tracked rather than adding `AsNoTracking()` the way the browser's
  grid query does.

- **A teammate proposes compiled queries and context pooling for the ticket browser before looking at the
  log. What would you measure first, and what would have to be true of the numbers before either change is
  worth making?**

  Measure the log first, not guess at it: run `Run naive search` and `Run optimised search` (or `Compare`)
  and read the **Before / after card** and the `[EF]`-prefixed console output — statement count, database
  milliseconds and wall-clock time, exactly the numbers `docs/BeforeAfterMeasurements.md` records. On this
  solution's own numbers, that log already tells you the whole story: the optimised branch sends **two**
  statements and averages **0.92 ms** wall-clock against a real file-based database, next to the naive
  branch's **15** statements and **18.79 ms** for the same filters. Nothing in that comparison points at
  query-compilation overhead or context-construction overhead as the bottleneck — it points at **round-trip
  count**, which compiled queries and pooling do not reduce at all; they only make each individual round trip
  a little cheaper to prepare.

  For **compiled queries** to be worth adding, the log would have to show the same query shape running very
  frequently (thousands of times a session, not a handful of clicks) with LINQ-to-SQL translation itself
  measurably dominating the time — not the SQL execution, not the round trip, the *translation step before*
  the round trip — which nothing in this course's traces has ever isolated on its own; `QueryTrace` reports
  execution time, not compilation time, so proving this specific claim would need a different, dedicated
  measurement first. For **context pooling** (`AddPooledDbContextFactory`) to be worth adding, the log would
  have to show context construction itself as a meaningful fraction of a request's time under real load — and
  the team would separately have to confirm no context here carries per-user or per-tenant state that pooling's
  reset-and-reuse could leak between sessions (this solution's `SupportDeskContext` carries none, but that is
  a fact to check, not assume, before pooling anything). Until the log says otherwise, the actual fix already
  shipped: fewer, cheaper round trips, paging and projection in SQL — exactly what "measure, change one thing,
  measure again" is supposed to catch before either specialised tool gets added on a hunch.

## Verified / unverified

Built and tested on this machine (Wisej-4 4.1.0, .NET 10, EF Core 10.0.12, SQLite):

- `dotnet build SupportDesk.slnx -nologo -v q` — succeeds with **0 warnings, 0 errors**.
- `dotnet test SupportDesk.Tests -nologo -v q` — **83 passed**, 0 failed (68 carried over from Modules 1–5
  unchanged, 6 new in `TicketPerformanceTests.cs`, 7 new in `TicketDetailServiceTests.cs`'s own class, 2 new
  in `BackgroundJobTests` nested in the same file — see `SupportDesk.Tests/TicketPerformanceTests.cs` and
  `SupportDesk.Tests/TicketDetailServiceTests.cs` for the exact list).
- Every number in [`docs/BeforeAfterMeasurements.md`](docs/BeforeAfterMeasurements.md),
  [`docs/RelatedDataDecisions.md`](docs/RelatedDataDecisions.md) and
  [`docs/LoggingInDevelopment.md`](docs/LoggingInDevelopment.md) was captured from a console check calling
  the exact shipped `TicketQueryService`/`TicketDetailService` methods against SQLite (in memory for
  statement counts and SQL text, a real file for timing) — not written from memory, not estimated:
  - naive branch, no filter, 50-row cap: **15 statements**, **326 tracked entities**, 50 rows shown of 312
    matching, 0.49 ms of SQLite command time (in memory), 18.79 ms average wall-clock (file-based, 5 warmed
    runs).
  - optimised branch, same criteria: **2 statements**, **0 tracked entities**, 50 rows shown of 312 total,
    0.22 ms of SQLite command time, 0.92 ms average wall-clock.
  - `TicketDetailService.LoadForEditorAsync`: **1 statement**. `LoadRecentCommentsAsync`: **1 statement**.
    `LoadAllCommentsAsync`: **2 statements** (1 extra beyond the ticket read).
  - `TicketQueryService.CountTicketsPerStatusAsync`: **5 statements, 1 context**, counts sum to 312
    (93 Open, 69 In Progress, 32 Waiting, 65 Resolved, 53 Closed on the fresh seed).
- `Naive_and_optimised_return_the_same_rows_in_the_same_order_for_the_same_criteria` proves the two branches
  are genuinely interchangeable from the grid's point of view — the fix changes *how* the rows are produced,
  never *what* they are.
- The null-FK / already-tracked fix-up fact the 15-statement number depends on is verified directly, not
  assumed: `A_reference_load_for_a_null_foreign_key_sends_nothing` shows zero commands for a
  `Reference().LoadAsync()` call against a `null` foreign key.

Facts carried over from Modules 1–5 and still relied on here: `Application.Services.AddService<IServiceProvider>(app.Services)`
makes `[Inject]` resolve through Microsoft DI for a `Page` and a `Form` constructed with `new`; application
services must be Transient or Singleton for the same root-provider reason (`TicketDetailService` is
registered `AddTransient`, exactly like every other service in this solution); `Application.Update(this)` in
`finally` pushes the final state after an `await`; a `DbContext` never becomes session state — Module 6's
background job creates its own inside `Application.StartTask`, the same rule as every ordinary click handler,
just from a background thread instead of an awaited one.

**Deviations from the lab guide's exact wording, and why:**

1. **The naive branch is a real method kept in the solution
   (`TicketQueryService.SearchTicketsNaiveAsync`), not a temporary edit to `SearchTicketsAsync` that gets
   reverted.** The lab guide's narrative ("switch `SearchTicketsAsync` to the naive branch … then fix it")
   reads as a single method changing shape twice. This solution instead keeps both branches side by side,
   permanently, behind two separate buttons and one shared `ApplyFilters` helper — the only way the "Before /
   after" card and the `Compare` button (both explicitly asked for by this module's build instructions) can
   show a live, re-runnable comparison instead of a one-time before/after screenshot. `SearchTicketsAsync`
   itself never regressed to the naive shape at any point in this module's history.
2. **The naive branch's statement count is measured at 15 for the default search, not the lesson's
   illustrative 151**, because of EF Core's automatic reference fix-up on this seed's small lookup tables —
   see the first self-check answer above and `docs/RelatedDataDecisions.md` for the full explanation,
   including the 131-statement number measured with fix-up deliberately bypassed. This is reported as a
   finding, not smoothed over: the exact number the reviewer's own machine will show is 15, and that is what
   is claimed here.
3. **The naive branch caps its in-memory shaping at fifty rows (`TicketQueryService.NaivePageCap`)** rather
   than shaping every matching row — chosen from the two options this module's build instructions offered
   ("cap … at the first 50 matching tickets in memory, or the full 1 + 3 × N when uncapped") so the naive
   branch's per-row cost is deterministic and directly comparable to the optimised branch's fixed page size,
   while its `TotalMatching` and its SQL still show the un-paged, whole-table load underneath the cap.
4. **`TicketDetailService.LoadAllCommentsAsync` chooses "inside one context" (a tracked ticket read plus
   `Collection(...).LoadAsync()`) over "a separate query"**, the two options this module's build instructions
   named — see `docs/RelatedDataDecisions.md` for the reasoning (the two reads belong to one operation; the
   ticket read is nearly free once its key is already known).
5. **Control names**: `btnNaiveSearch`, `btnOptimisedSearch`, `btnCompare`, `btnLongJob`, `labelLongJob`,
   `panelBeforeAfter`/`labelBeforeAfter`, `lstComments`, `btnShowFullHistory` are this module's own names —
   neither the lab guide nor `labs.js` names any Module 6 controls specifically (`labs.js`'s `m6` check only
   looks for regex patterns like `AsNoTracking`, `.Skip(`, `.Include(`, all present in the shipped code).
6. **`TicketEditModel` still has no `RowVersion` property**, unchanged from Modules 4–5's documented
   deviation — Module 7 adds it, with the conflict dialog it needs to be useful.

**Not verified here — for the browser reviewer.** The application was not started (by instruction), so
everything below is Wisej.NET behaviour that only a running page and a running dialog can confirm:

1. The **Before / after card** actually renders its HTML table correctly (`AllowHtml = true` on
   `labelBeforeAfter`), stays at its neutral prompt until a branch button has run, and updates live after
   each run, including the **×** improvement factors.
2. **Run naive search**, **Run optimised search** and **Compare** actually disable the guarded buttons
   (`searchButton`, paging, the two branch buttons, `btnCompare`) while running and re-enable them in
   `finally`, and that clicking a second one while the first is still running is dropped by the `_loading`
   guard the same way every other Module 1–5 progress path already relies on.
3. **Long job (background)** actually runs on a background thread that keeps the Wisej.NET session context
   (per the course cookbook, verified in sibling course samples but not re-verified here, since this is the
   first time this solution combines `Application.StartTask` with an `async` EF Core call bridged through
   `.GetAwaiter().GetResult()` inside the task's `Action` — see the "why" note below), that `labelLongJob` and
   the trace really do update five times with a visible pause between each, and that the button re-enables
   itself when the job finishes or fails.
4. The editor's **Comments panel** (`lstComments`, `btnShowFullHistory`) actually renders the formatted lines,
   that the panel's neutral/disabled state on **Add ticket** looks right, and that **Show full history**
   really does replace the five-row preview with every comment on the ticket.
5. `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` appears top-right if
   `LoadAllCommentsAsync` ever fails from the running app (not reproduced in this session — the tests exercise
   the failure path at the service layer only).
6. The reviewer-visible flow end to end: click **Run naive search**, watch the trace and the Before / after
   card; click **Run optimised search**, watch the card's optimised column and the **×** factors appear; click
   **Compare** and watch both run in sequence; open an existing ticket with comments and confirm the preview,
   then click **Show full history**; click **Long job (background)** and watch all five steps land with a
   visible pause between them.
7. **Why `.GetAwaiter().GetResult()` inside `Application.StartTask`'s `Action`, not an `async Action`**:
   `Application.StartTask` takes a plain `Action` (verified in sibling course samples — no course sample
   before this one has combined it with `async`/`await` EF Core calls, all of them keep the task body
   synchronous instead). Bridging the async `TicketQueryService.CountTicketsPerStatusAsync` call back to
   synchronous with `.GetAwaiter().GetResult()` inside that `Action`, on the dedicated background thread
   `Application.StartTask` already provides (not the browser request thread, so no classic ASP.NET
   `SynchronizationContext` deadlock risk applies), is this module's answer to a combination the cookbook does
   not yet document — flagged here explicitly as the one genuinely new Wisej.NET/EF Core interaction this
   module introduces, for the reviewer to confirm live.

**Browser verification: see the note at the end.**

## Browser results (reviewer, 2026-09-10)

Run on this machine at <http://localhost:5406> in the Browser pane; trace read back from the page. No defect found.

- **Compare (both, same filters)** with no filters set → the naive half: `• service naive branch: … tracked Tickets.Where(...), no Skip/Take, no Select: the whole matching set is about to be loaded`,
  one `SELECT … FROM "Tickets" AS "t" ORDER BY "t"."UpdatedAt" DESC` without LIMIT, then fourteen single-row lookups
  (`SELECT … FROM "Customers" AS "c" WHERE "c"."Id" = @p LIMIT 1`, the same for `"Agents"` and `"Categories"`),
  `• service naive branch: 312 tracked Ticket(s) matched (no SQL paging), 50 shaped in memory (capped) · 326 entities held tracked by this context — none of them will ever be saved`,
  `← result Naive: 50 row(s) shown of 312 matching (no SQL paging) · 326 tracked entities · 15 statement(s) · 0.7 ms in the database`;
  then the optimised half: `SELECT COUNT(*)` + the projected `LIMIT/OFFSET` join, `← result Optimised: 50 row(s) shown of 312 matching · page 1 of 7 · 2 statement(s) · 0.2 ms`.
- The **Before / after** card fills in from the two trace scopes: statements 15 vs 2, database ms 0.7 vs 0.2, tracked entities 326 vs 0, rows shown 50 vs 50, matching 312 vs 312 —
  the same 15-vs-2 the lab notes measured (EF Core's reference fix-up resolves repeated Customer/Agent/Category keys from the tracked graph, which is why it is 15 and not 151 on a seed with 5 customers, 3 agents and 6 categories).
- **Long job (background)** → `• background Long job (background): Application.StartTask → TicketQueryService.Count…`, the label under the card steps through the five statuses and ends at
  *Long job (background): done — counted tickets in every status*, `• background Long job (background) finished — the one context created for the job is …disposed` (about three seconds end to end).
- **Edit ticket SD-1002** → the editor's **Comments** panel: `• editor comments panel: TicketDetailService.LoadRecentCommentsAsync(#2) — 0 shown (the last 5)` (this ticket has no comments);
  **Show full history** → `• editor comments panel: "Show full history" → TicketDetailService.LoadAllCommentsAsync(#2) — 0 shown (explicit load, 1 extra statement beyond the ticket read)`.
- The Module 1–5 rows behave as in their own modules (grid, paging, editor, validation).

