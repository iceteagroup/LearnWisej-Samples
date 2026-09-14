# WisejPerfLab · Performance & Profiling · Module 1

Lab build for **Module 1 · The Wisej.NET Performance Model**: the WisejPerfLab support application with
a dashboard, a ticket grid and a customer tree over a seeded dataset of **50,000 tickets and 3,200
customer nodes**, made measurable by a `ScenarioProbe` that wraps each named scenario in a `Stopwatch`
scope and writes structured `PERF start` / `PERF end` records.

Nothing in this module is optimised. That is the point: this is the **before** the whole course is
measured against, and every later module starts from the version in the previous folder. The three
deliverables are in `docs/`: [`Scenarios.md`](docs/Scenarios.md), [`Baseline.md`](docs/Baseline.md) and
[`Budget.md`](docs/Budget.md).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 1/WisejPerfLab"
dotnet run -f net10.0 --urls http://localhost:5801
```

Measurements are taken in Release, and so are the numbers in `docs/Baseline.md`:

```bash
dotnet run -c Release -f net10.0 --urls http://localhost:5801
```

The first start creates and seeds `WisejPerfLab/App_Data/perflab.db` (about 60 MB, half a second) and
prints the row counts. Delete that folder to reseed; the seed is deterministic, so the rows come back
identical. The app header shows the dataset and the build configuration it is running under — if it
says *Debug (measure in Release)*, the numbers on screen are not baseline material.

## What to click

| Action | What you should see |
|---|---|
| **Warm up (discarded run)** | the three scenarios run once, bracketed in the PERF log by "warm-up run — the three records below are discarded" |
| **Run the three scenarios ×3** | nine runs, then three `median …` lines and a status line with all three medians |
| Dashboard → **Refresh** | KPIs and the chart repaint; the chip says `Refresh 476 ms — over the 250 ms budget   rows=50,000` |
| Tickets → **Search tickets** | 5,000 rows, and the status line admits `5,001 SQL statements` |
| Tickets → **Redraw** | the same rows rebuilt with no query at all — the allocation-only scenario Module 4 attacks |
| Tickets → **Export CSV** | the file name appears after about half a second; the progress bar never moves, because the click handler blocked on `.Result` and nothing reaches the browser until it returns |
| Tickets → double-click a row | the ticket detail form, showing `bus subscribers: 1   timer: running` — its two retention roots, on screen |
| Customers → **Load the customer tree** | 3,200 nodes and `3,201 SQL statements`, built before you open anything |
| Customers → **Expand the first branch** | one branch opens, and another 3,201 statements are issued to do it |
| **Break the database** then any scenario | a red banner, a fault status, an emptied grid — and a `PERF end … failed=DatabaseUnavailableException` record with the time spent before the failure |
| **Restore** | the next run succeeds again |
| **Clear log** | the PERF buffer of this session empties |

Open a second browser tab to see that each session gets its own PERF log: the buffer lives in
`Application.Session`, not in a static field.

## Where things live

```
WisejPerfLab/
  Startup.cs                     host builder: SQLite, the probe, the services, the Wisej DI bridge, seeding
  Program.cs                     Wisej session entry point (one MainPage per session)
  MainPage.cs / .Designer.cs     shell: three tabs, the PERF log card, warm-up / ×3 / break / restore
  Diagnostics/
    ScenarioProbe.cs             Measure(scenario, userAction, rows) -> IDisposable scope   ← the deliverable
    PerfRecord.cs                one PERF line
    PerfLogBuffer.cs             this session's records + MedianElapsed()
    SessionPerfLog.cs            finds the buffer in Application.Session
  Shell/
    PerfBudget.cs                docs/Budget.md as code: threshold + tool per scenario
    IPerfLabShell.cs             status line and failure banner
    IScenarioPage.cs             a screen that owns one measured scenario
  Pages/
    DashboardPage.cs             Dashboard/Refresh — load everything, format per row, rebuild the KPI panel
    TicketGridPage.cs            Tickets/Search, /Redraw, /Export
    CustomerTreePage.cs          Customers/LoadTree, /ExpandNode
  Forms/TicketDetailForm.cs      the detail form, with the leaks Module 4 will find
  Services/                      DashboardService, TicketSearchService, CustomerTreeService,
                                 ExportService, TicketFormatter, GlobalTicketBus
  Data/                          PerfLabContext, entities, the deterministic seeder, the outage switch
docs/                            Scenarios.md, Baseline.md, Budget.md
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Create the WisejPerfLab solution with dashboard, ticket grid and customer tree | `MainPage`, `Pages/*` |
| Seed once so every run starts identical; record the row counts | `Data/DatasetSeeder.cs`, `Data/PerfLabDatabase.cs`, printed at start and in the header |
| Build the three screens the obvious way; optimise nothing | `Pages/DashboardPage.cs`, `Pages/TicketGridPage.cs`, `Pages/CustomerTreePage.cs` |
| `ScenarioProbe.Measure(scenario, userAction, rowCount)` returning a `Stopwatch` scope | `Diagnostics/ScenarioProbe.cs` |
| Structured logging fields rather than a concatenated message | `ScenarioProbe.Write` — `"PERF end {Scenario} {UserAction} elapsedMs={ElapsedMs} rows={Rows}"` |
| Register it as a singleton in the host builder, reachable from the containers | `Startup.cs` steps 3–5; `Services/PerfLabServices.cs` for the tab UserControls |
| Wrap the three scenarios, scope around the whole user action | `RunRefresh`, `RunSearch`, `LoadTree` / `treeView1_BeforeExpand` |
| A scenario that throws still writes its PERF end record | `catch (DatabaseUnavailableException) { scope.Fail(ex); … }` — the `using` disposes either way |
| An empty result reports zero rows rather than failing | `RunSearch` binds an empty list and says so |
| A seeding or connection failure is reported in the status bar | `MainPage.ShowBanner` + the fault status line |
| Warm up, then three runs, take the median | **Warm up** and **Run the three scenarios ×3**; `PerfLogBuffer.MedianElapsed` |
| Write `Baseline.md` and `Budget.md` | [`docs/Baseline.md`](docs/Baseline.md), [`docs/Budget.md`](docs/Budget.md) |

## Self-check answers

**Why is a session, not a request, the unit of capacity?** Because the page, its controls, their state,
the binding sources and everything they reference stay in the server process between clicks. A request
ends; a session keeps its memory until it times out. Open the detail form and look at
`bus subscribers: 1` — that subscription outlives the form, and in Module 4 it outlives the session's
usefulness.

**Where does the time in `Dashboard/Refresh` actually go?** Not in the database: the scenario issues one
statement. It goes into materialising 50,000 entities with two long text columns, formatting five
strings per row, and disposing and rebuilding the KPI controls. That is CPU and allocation in this
process, which is why Module 2 reaches for CPU Usage and not the Database tool.

**Why wrap the whole user action instead of the query?** Because the user waits for the whole action.
The query behind the refresh is about 90 ms of the 476 ms the probe reports; a query timer would have
declared this screen healthy.

**Why is one run never a measurement?** The first run of every scenario here costs roughly twice the
median — JIT, the first query plan, the first layout. And three warm runs still spread by ±25 %
(`Tickets/Search`: 536 / 444 / 384 ms). A change smaller than that spread is not a result.

## Known simplifications

- The database is **SQLite in process**, so statements are about six times cheaper than they would be
  against SQL Server across a socket. The statement *counts* in this app are real; the wall-clock cost
  of the N+1 is flattering. `docs/Budget.md` explains what that did to the thresholds.
- The chart is `Wisej-4-ChartJS`. The lesson snippet writes `chartLoad.DataSource = snapshot.ChartRows`;
  the real control takes `Labels` plus `DataSets`, so that is what `RebuildChart` does.
- The PERF records are also shown on screen. In production they would only go to the logging pipeline —
  `ScenarioProbe` writes to `ILogger` first and to the session buffer second, and the console output of
  `dotnet run` shows exactly the same lines.
