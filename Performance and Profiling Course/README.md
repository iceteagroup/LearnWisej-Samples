# Performance & Profiling · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide and
walkthrough video. The course is cumulative — every lab opens the `WisejPerfLab` solution of the
previous module — so each `Module N` folder holds the **complete application as it stands after module
N**, and the folder before it is the *before* for that module's measurements.

**WisejPerfLab** is a support desk: a dashboard, a ticket grid, a customer tree, a ticket detail form
and a CSV export over a seeded dataset of **50,000 tickets and 3,200 customer nodes** in a local SQLite
file. Module 1 builds it the obvious way and measures it. Module 2 profiles it. Modules 3 to 6 fix one
cost bucket each. Module 7 turns the measurements into a capacity model and a health check.

Requirements: the .NET 10 SDK, the `Wisej-4` 4.1.0 package and `Wisej-4-ChartJS` 4.1.0, plus EF Core
10.0.12 with the SQLite provider. Nothing is deployed anywhere; the dataset is created on first start
(about half a second) and lives in `WisejPerfLab/App_Data/`.

| Module | Folder | What it changes | Headline measurement | Run |
|---|---|---|---|---|
| 1 · The Wisej.NET Performance Model | `Module 1` | the application, three named scenarios, `ScenarioProbe`, the baseline and the budget | refresh **476 ms** against a 250 ms budget | `dotnet run -c Release -f net10.0 --urls http://localhost:5801` |
| 2 · Visual Studio Profiling Workflow | `Module 2` | stages inside each scenario, a call counter, the trace note, the noise floor | the formatter everyone blames is **7 %** of the refresh | `http://localhost:5802` |
| 3 · CPU Hot Paths and Server-Side UI Work | `Module 3` | `DashboardSnapshot`, aggregates in the database, KPI cards built once | refresh **476 → 113 ms** | `http://localhost:5803` |
| 4 · Memory, Allocations and Session Leaks | `Module 4` | `Dispose(bool)` on the detail form, rows projected once | **+76 MB → +1.1 MB** per 50 open/close | `http://localhost:5804` |
| 5 · Data Controls and Large UI Surfaces | `Module 5` | virtual grid with a page cache, tree loaded a level at a time | search **990 → 125 ms**, tree **510 → 62 ms** | `http://localhost:5805` |
| 6 · Database, File I/O, Async and Waits | `Module 6` | one projection query per page, an index, the export on `Application.StartTask` | **201 statements → 1**, export **522 → 171 ms** | `http://localhost:5806` |
| 7 · Scale, Health Checks and Final Tuning | `Module 7` | `HealthCheck.json`, the Capacity screen, the health endpoint, the final report | `503` + `Retry-After` when full, existing sessions untouched | `http://localhost:5807` |

Run any module from its `WisejPerfLab` project folder (the projects multi-target `net10.0-windows` and
`net10.0`):

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 3/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5803
```

or open `WisejPerfLab.slnx` in the module folder with Visual Studio and press F5.

**Measure in Release.** Every number in these folders was taken in Release with no debugger attached,
after a discarded warm-up run, as the median of three. The application header says which build it is
running under, and says *Debug (measure in Release)* when it is not.

## How to use these samples

Two folders side by side is the whole idea:

```bash
# the before
cd "Module 5/WisejPerfLab" && dotnet run -c Release -f net10.0 --urls http://localhost:5805
# the after
cd "Module 6/WisejPerfLab" && dotnet run -c Release -f net10.0 --urls http://localhost:5806
```

In both: **Warm up (discarded run)**, then **Run the three scenarios ×3**, then read the medians and the
spread the application prints for itself.

## The lab controls

Every module has them, and they are the lab rather than decoration:

| Control | What it is for |
|---|---|
| **Warm up (discarded run)** | the run nobody records — JIT, the first query plan, the first layout |
| **Run the three scenarios ×3** | nine runs, three medians, and the spread as a percentage of each median |
| **Break the database** / **Restore** | the failure path: a scenario that fails still reports its elapsed time, and says so on screen |
| **Trace note for the last run** (M2+) | the note that belongs beside a `.diagsession`, built from what the app knows |
| **Memory snapshot A / ×50 / Snapshot B** (M3+) | the leak, as a number: heap, bus subscribers, forms created and disposed |
| **Capacity tab** (M7) | live readings, the configured thresholds, the model behind them, and what the health URL last answered |

## What is measured, and what is not

The PERF records measure **server handler time** — the query, the projection and the control mutation.
They do not include serialising the update, sending it or the browser applying it; that is the fifth
cost bucket, and `Module 5/docs/LargeSurfaces.md` explains what could and could not be read for it in a
browser where every update travels over the WebSocket.

The database is **SQLite, in process**. Statement counts are properties of the code and carry over to
any database; the milliseconds are flattering, because a statement here costs about 0.05 ms and a
statement against a database server costs a network round-trip. Every module README says so where it
matters.

## `_template`

[`_template/COOKBOOK.md`](_template/COOKBOOK.md): the conventions these samples share and every
Wisej.NET, EF Core and browser fact that was verified while building them — including the ones that
cost an hour. Read it before writing a new sample for this course.
