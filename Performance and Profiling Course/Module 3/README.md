# WisejPerfLab · Performance & Profiling · Module 3

Lab build for **Module 3 · CPU Hot Paths, Event Handlers and Server-Side UI Work**. The dashboard
refresh is fixed: **476 ms → 113 ms**, medians of three warm runs, against a noise floor of 13 %.

The fix is the one the lesson asks for — a `DashboardSnapshot` of display-ready values built once in
the service, assigned to controls that already exist — applied where the profiler said the time
actually was. See [`BeforeAfter.md`](docs/BeforeAfter.md) and [`SnapshotDesign.md`](docs/SnapshotDesign.md).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 3/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5803
```

To see the before and after side by side, run Module 2 on 5802 at the same time and click **Refresh**
in both.

## What to click

| Action | What you should see |
|---|---|
| **Warm up (discarded run)**, then Dashboard → **Refresh** | `Refresh 109 ms — within the 250 ms budget   4 queries   as of 11:31:45` |
| the PERF log for that run | `stage Refresh / snapshot (4 aggregate queries, formatted once) 109 ms`, `stage Refresh / assign 0 ms` — and **no** `calls TicketFormatter.FormatRow` line, because it is never called |
| **Run the three scenarios ×3** | `median Dashboard/Refresh = 113 ms — within the 250 ms budget`, spread 13 % |
| Click **Refresh** twice quickly | the second click is dropped: *a refresh is already running — the second click was dropped* |
| **Break the database** → **Refresh** | the button comes back, the banner explains, the KPIs keep their last good values, and the PERF end record is still written with `failed=DatabaseUnavailableException` |
| Compare the KPI numbers with Module 2 | identical: 11,907 open, ~21,64x overdue, 90.5 d average age |

The other two scenarios are untouched and still over budget — `Tickets/Search` 414 ms, tree expand
158 ms. They are Modules 4, 5 and 6.

## One thing here belongs to Module 4

This folder also carries `Diagnostics/MemoryProbe.cs`, the detail-form counters and the three buttons
**Memory snapshot A** / **Open and close the detail form ×50** / **Snapshot B and compare** — with the
**unfixed** detail form. They are here so the *before* reading of the Module 4 lab can be taken on the
code as it stood: `B - A: heap +76.0 MB   subscribers +50`. Module 4 fixes the form and the same three
buttons report `+1.1 MB   subscribers 0`.

## What changed since Module 2

```
Models/DashboardSnapshot.cs    new — OpenTicketsText, OverdueTicketsText, AverageAgeText, ChartRows, GeneratedAt
Services/DashboardService.cs   + GetSnapshot(): 4 aggregate queries, formatted once, no entity returned
                               LoadAllTickets() kept but no longer called by anything
Services/TicketFormatter.cs    status/priority labels are now a static lookup, not a per-call string.Format
                               + FormatDay() for the chart axis
Pages/DashboardPage.cs         rewritten: KPI cards built once in the constructor, values assigned,
                               RebuildKpiPanel deleted, _refreshing guard added
```

Nothing else moved. `ScenarioProbe`, the budget, the PERF log and the scenario definitions are
identical to Module 2, so the before/after table compares the same measurement.

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Collect your own before trace; caller/callee on `TicketFormatter.FormatRow` to see what drives 50,000 calls | Module 2 folder; [`HotPath.md`](docs/HotPath.md) |
| `DashboardSnapshot` with `OpenTicketsText`, `OverdueTicketsText`, `AverageAgeText`, `ChartRows` | `Models/DashboardSnapshot.cs` |
| Build it once in `DashboardService.GetSnapshot()` | `Services/DashboardService.cs` |
| Move every formatting call out of the page, including culture-aware number and duration formatting | `GetSnapshot()` calls `TicketFormatter.FormatCount` / `FormatAge` / `FormatDay` |
| Cache only the immutable status and priority labels at application scope; per-user state in the session | `TicketFormatter.StatusLabels`; [`SnapshotDesign.md`](docs/SnapshotDesign.md) |
| Rewrite `btnRefresh_Click` as assignments inside the probe scope, disable and re-enable in a `finally` | `DashboardPage.RunRefresh` |
| Delete `RebuildKpiPanel`; update the existing controls in place | `BuildKpiPanel()` in the constructor, `_lblOpenValue` and friends |
| A refresh that throws still re-enables the button and reports the failure | the `catch` / `finally` pair in `RunRefresh` |
| An empty result shows zeros rather than blank labels | `FormatCount(0)` is `"0"`; the aggregates return 0, never null |
| A refresh clicked twice cannot queue a second run | `_refreshing` |
| Rerun the identical scenario and confirm the hot path changed rather than only shrinking | [`BeforeAfter.md`](docs/BeforeAfter.md) |
| Before/after table as medians of three, plus remaining risks including staleness | [`BeforeAfter.md`](docs/BeforeAfter.md) |

## Self-check answers

**Why did fixing the "obvious" offenders matter so little?** Because the stage breakdown in Module 2
measured them: the 50,000 `FormatRow` calls were 60 ms and the control rebuild 4 ms, out of 972 ms. The
expensive thing was materialising 50,000 tracked entities to count three of their properties. Reading
the profile before choosing the fix is the entire module.

**Is a view model always the answer?** The view model is what makes the real answer possible. Once the
page needs only three strings and fourteen numbers, the query is free to be an aggregate. A view model
that still carries 50,000 rows would have saved the 60 ms of formatting and nothing else.

**What did the fix cost?** A staleness window, four statements instead of one, and one
provider-specific SQL string. All three are written down in [`BeforeAfter.md`](docs/BeforeAfter.md) —
an optimisation with no stated cost usually means the cost was not looked for.

**Why keep the probe and the budget identical?** Because a before/after comparison is only valid if the
instrument did not change with the code.
