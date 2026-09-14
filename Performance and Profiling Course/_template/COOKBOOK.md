# WisejPerfLab cookbook — Performance & Profiling (Wisej-4 4.1.0 · EF Core 10.0.12 · .NET 10)

What was verified while building and running the seven samples of this course, and the conventions they
share. Read this before writing a new sample for it.

## The course in one sentence

One application, **WisejPerfLab** — a support dashboard, a ticket grid, a customer tree, a detail form
and a CSV export over 50,000 tickets and 3,200 customer nodes — measured in Module 1, profiled in
Module 2, and then fixed one bucket at a time: CPU (3), memory (4), UI surface (5), waiting (6),
capacity (7).

## The folders are cumulative

`Module N` is the complete application **as it stands after module N**. Each folder was produced by
copying the previous one and applying that module's change, so:

- the **before** for any module is the previous folder, running on its own port;
- a fix is compared against the folder that still has the problem, not against a remembered number;
- `Module 1` is the scaffold every later module was built from. There is no separate skeleton project,
  because nothing was ever built from one.

Ports **5801–5807** (Module N → 580N). Preview entries `perflab-m1` … `perflab-m7` in the gitignored
`.claude/launch.json` of the LearnWisej repository.

One deliberate exception to the cumulative rule: `Module 3` also carries `Diagnostics/MemoryProbe.cs`,
the detail-form counters and the three snapshot buttons — with the **unfixed** form — so the *before*
reading of the Module 4 lab can be taken on the code as it stood. Both READMEs say so.

## Measuring, in this app

- `ScenarioProbe.Measure(scenario, userAction, rows)` returns a scope: `PERF start` on entry,
  `PERF end … elapsedMs=… rows=…` on dispose, through `ILogger` (structured fields) **and** into the
  session's `PerfLogBuffer`. The scope wraps the **whole user action**, never the query alone.
- `scope.Stage("name")` (Module 2 on) times a phase inside the scenario. This is what found the real
  hot path: `query + materialise 894 ms` against `format rows 60 ms`.
- `CallCounter.Count("Type.Method")` (Module 2 on) is the call count an Instrumentation trace gives and
  a sampling trace cannot. The probe resets it at scope start and writes the counts at scope end.
- `PerfLogBuffer.MedianElapsed` / `Samples` / `SpreadPercent` — three runs, a median and a noise floor.
  On this machine the spread is **10–25 %**; a smaller difference is not a finding.
- `MemoryProbe.Take()` (Module 4 on) — heap after one collection, bus subscribers, forms created and
  disposed. `Compare(a, b)` prints `B - A`.
- `PerfBudget` is `docs/Budget.md` as code, so the status line says *over* or *within* instead of only
  printing a number.

**The budgets in this sample are tighter than the course lesson's** (250 ms rather than 800 ms for the
refresh). The lesson's numbers were measured against SQL Server, where every statement is a network
hop; this sample runs SQLite in process, where the same work is about twice as fast. Keeping the
lesson's thresholds would have let the naive code pass. `Module 1/docs/Budget.md` shows the reasoning.

## Verified runtime facts

- **`Application.Session["PerfLog"]`** is a per-session bag and works from inside
  `Application.StartTask` (the task keeps the session context). It is how the probe writes into the
  right session's timeline while being a stateless singleton in Microsoft DI.
- **`Application.SessionCount`** is the live session count; two browser tabs are two sessions.
- **`Wisej.Core.HealthCheck`** reads `HealthCheck.json` from the application root: `Enabled`,
  `MaxSessions`, `MaxMemory`, `MaxCPU`, `ReturnCode`, `RetryAfter`, `ResponseHeader`, `ReturnUrl` are
  all public and were read back correctly at start. `HealthCheck.IsServerAvailable` is an assignable
  hook. **`GetMemoryUsed()` and `GetCPULoad()` are not public** — `Health/ServerLoad.cs` computes the
  same two percentages from the process.
- **`healthcheck.wx` under Kestrel answered `200` whatever the limits were** (Wisej-4 4.1.0,
  `dotnet run`); the built-in handler belongs to the classic System.Web pipeline. Module 7 serves the
  URL from its own middleware, before `UseWisej()`, using the same configuration and the same
  availability function. Verified: `200` with a reason body when healthy, `503` + `Retry-After: 10` when
  over the limit, and the existing session keeps working.
- **`DataGridView` virtual mode**: `VirtualMode = true`, `RowCount = n`, then `CellValueNeeded` per cell
  and `DataRead(e.FirstIndex, e.LastIndex)` for the block the client is about to read. The cells arrive
  **after** the response that set `RowCount` — a screenshot taken immediately shows an empty grid, which
  is not a bug. Read the grid's client model (`getTableModel().getValue(row, col)`) a second later.
- **Wisej controls virtualise rendering, not your server.** A 3,200-node `TreeView` produced **nine**
  `TreeNode` widgets in the browser; a bound `DataGridView` with 11,907 rows fetched rows as it
  scrolled. The cost of a large surface is server objects, server CPU and statements.
- **Updates travel over the WebSocket**, so `performance.getEntriesByType('resource')` reports nothing
  for them and per-click byte counts are not available from the Performance API. Read the frames in the
  browser's Network panel, or disable the WebSocket to force long-polling.
- **`Application.StartTask`** keeps the session, so the lambda may touch controls and call
  `Application.Update(this)`. **Catch every exception inside the lambda** — one escaping it shows the
  red Wisej application-error dialog. Check `IsDisposed` before touching a control: the session may have
  gone while the work ran.
- **`Form.Close()` disposes the form** (verified: `forms created 50 / disposed 50`). It does **not**
  release what the form subscribed to — a static event and a running `System.Timers.Timer` kept all
  fifty disposed forms in the heap, +76 MB.
- **A partial class has one `Dispose(bool)`.** When a form needs its own, the override moves from the
  `.Designer.cs` into the `.cs` and takes over `components?.Dispose()`.
- **`Control.Created` exists** — a static counter called `Created` on a `Form` subclass hides it
  (CS0108). Name it `CreatedCount`.
- **`Wisej.Web.Ext.ChartJS`**: `chart.Labels = string[]`, `chart.DataSets.Clear()/.Add(new BarDataSet
  { Label, Data (object[]), BackgroundColor, BorderColor, BorderWidth })`, then `chart.UpdateData(0)`.
  There is no `DataSource` property — the lesson snippet's `chartLoad.DataSource = …` is pseudocode.
- **EF Core on SQLite**: `AsNoTracking()` + a `Select` projection with `t.Customer.Name` produces one
  joined statement; `GroupBy(t => t.UpdatedAt.Date)` translates; **an average over a date difference
  does not** — `Dashboard/Refresh` uses one `SqlQueryRaw<double?>` with `julianday()` and says so.
  `EnsureCreated` does nothing to an existing file, so an index added in a later module is created with
  `CREATE INDEX IF NOT EXISTS` at start.
- **Seeding**: 50,000 tickets and 3,200 customer nodes with one prepared command in one transaction and
  `PRAGMA journal_mode=OFF` takes about **half a second**; through EF Core it would take minutes.

## Numbers this course quotes (Release, warm, medians of three, this machine)

| Scenario | M1 | M3 | M5 | M6/M7 |
|---|---:|---:|---:|---:|
| `Dashboard/Refresh` | 476 ms | **113 ms** | 113 ms | 113 ms |
| `Tickets/Search` | 444 ms | 414 ms | 125 ms | **66 ms** |
| `Customers/LoadTree` | 312 ms | 510 ms* | **62 ms** | 62 ms |
| `Customers/ExpandNode` | 188 ms | 158 ms | **9 ms** | 9 ms |
| `Tickets/Export` | 522 ms | 522 ms | 522 ms | **171 ms** |
| statements per ticket page | 5,001 | 5,001 | 201 | **1** |
| retained per 50 detail forms | +76 MB | +76 MB | +1.1 MB | +1.1 MB |

\* the tree numbers move between runs because the per-node count query is I/O bound; the shape (3,201
statements) is what matters, not the millisecond.

## UI conventions (so the seven samples look like one application)

- `MainPage : Page`, `1348 × 748`, background `Color.FromArgb(238, 242, 247)`, white cards with
  `BorderStyle.Solid`, titles `"default" 14F Bold` / `12F Bold`, grey text `Color.FromArgb(90, 107, 125)`,
  monospace `9F` for every number.
- **Header**: title, module subtitle, and the dataset plus build configuration on the right — a screen
  that says *Debug (measure in Release)* is telling you not to trust its numbers.
- **Left**: a `TabControl` of the screens, each a `UserControl` docked `Fill`, each implementing
  `IScenarioPage` (`Scenario`, `UserAction`, `RunScenario()`), so the shell can drive the warm-up and the
  three runs through exactly the handler the button runs.
- **Right**: the PERF card — a monospace `ListBox` of `PerfRecord.ToString()` with a legend.
- **Bottom**: the lab bar. Warm up · Run the three scenarios ×3 · Break the database · Restore · Trace
  note · Clear log, and from Module 4 a second row with the memory snapshots.
- **Status line** `● text` in green / blue / amber / red, and a red **banner** for a failure, hidden
  until needed. Every failure path ends in one of those two — a scenario that fails never leaves the
  screen blank.
- Designer-style `*.Designer.cs` with explicit `Location`/`Size`, code-behind in `*.cs` with `#region`
  blocks per path.

## Docs (`docs/`) and README

`docs/` accumulates across modules: Module 1's `Scenarios.md`, `Baseline.md` and `Budget.md` travel
with every later folder, and each module adds its own deliverables. The capstone report in Module 7
references all of them.

Each module README carries: **Run it** (exact command and port), **What to click** (action → what you
should see, with the real numbers), **What changed since Module N-1** (a file list), **Lab steps → where
in the code**, **Self-check answers**, and **Known simplifications**. Never quote a number that was not
measured; when the sample cannot show something (update byte counts over the WebSocket, a real
multi-session load test), say so in Known simplifications rather than inventing it.

## Course sources

- Module specifications: `D:/Projects/Netlify/WWW-LearnWisej/assets/courses/performance-and-profiling/specs/module-0N.md`
- Lab guide steps: `assets/courses/performance-and-profiling/labs/mN.json`
- Lesson prose: `assets/courses/performance-and-profiling/lessons/ppNs1.html` (concepts) and `ppNs2.html`
- Walkthrough scenes: `scripts/walkthrough/performance-and-profiling/mN/`
