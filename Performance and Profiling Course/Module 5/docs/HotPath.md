# The hot path of `Dashboard/Refresh`

What the scenario actually spends its time on, and what the profiler will show you when you collect it
yourself.

## The app's own evidence

Every scenario is split into named stages (`ScenarioProbe.Stage`), and `TicketFormatter.FormatRow`
counts its own calls. One warm run of the refresh, Release, 50,000 tickets:

```
PERF start  Dashboard/Refresh
    stage Refresh / query + materialise 894 ms
    stage Refresh / format rows 60 ms
    stage Refresh / rebuild KPI panel 4 ms
    stage Refresh / rebuild chart 10 ms
PERF end    Dashboard/Refresh elapsedMs=972 rows=50000
    calls TicketFormatter.FormatRow x50,000
```

Warm, the same run settles around 470 ms, with the same proportions: **materialisation dominates, and
the formatter is a rounding error.**

| Stage | Cold run | Share |
|---|---:|---:|
| query + materialise 50,000 entities | 894 ms | 92 % |
| format 50,000 display rows | 60 ms | 6 % |
| rebuild the KPI panel (3 cards, 6 controls) | 4 ms | 0.4 % |
| rebuild the chart (14 bars) | 10 ms | 1 % |

## This is the point of the module

The obvious suspects were the per-row formatter and the control rebuild — the two things the code
makes most visible, and the two things a reader would "fix" first. Between them they are **7 %** of
the scenario. Removing both, perfectly, would take a 972 ms refresh to about 900 ms and leave it four
times over budget.

The cost is one line: `db.Tickets.OrderByDescending(t => t.UpdatedAt).ToList()`. One statement, no
`AsNoTracking`, 50,000 entities with two long text columns each, materialised and tracked, to produce
**three integers and fourteen bar values**.

That is why the workflow is *profile first, then fix*. Module 3 fixes it — and the fix the lesson asks
for (compute the display values once, in the service) turns out to mean something stronger than moving
`string.Format` calls around: it means the rows never reach the page at all.

## What CPU Usage will show

Collect it yourself (Alt+F2 → CPU Usage → one Refresh click), narrow the timeline to the click, and
turn on **Show Hot Path**. Scroll past the Kestrel, ASP.NET Core and Wisej.NET dispatch frames. The
first frames in the `WisejPerfLab` namespace will be `DashboardPage.RunRefresh` →
`DashboardService.LoadAllTickets`, and below that the hot path leaves your code again and continues
into EF Core's query pipeline and the SQLite data reader — materialisation, string allocation, change
tracking.

Two readings to take, and one trap:

- **Total CPU** on `RunRefresh` is the whole scenario. **Self CPU** on it is nearly nothing: it is a
  handler that calls other things. A function with a large total and a tiny self is a *route* to the
  problem, not the problem.
- `TicketFormatter.FormatRow` will appear with a small self CPU and a very large call count.
  Caller/callee shows one caller, the loop in `RunRefresh`. Fifty thousand calls of something cheap is
  what a hot path usually looks like — here it happens not to be the expensive one, which is exactly
  the kind of thing you can only know by measuring.
- The trap: CPU Usage is a **sampling** profiler. It shows where the CPU was, in proportion. It does
  not show call counts (that is Instrumentation), it under-represents time spent waiting rather than
  working, and a function that ran 50,000 times for 1 µs each can easily be flattened into a few
  samples.

## What to record in your own note

| | |
|---|---|
| First function in your namespace | `WisejPerfLab.Pages.DashboardPage.RunRefresh` |
| Its total CPU / self CPU | *(from your trace)* |
| The function below it that dominates | `DashboardService.LoadAllTickets` → EF Core materialisation |
| A function with a striking call count | `TicketFormatter.FormatRow`, 50,000 calls, one caller |
| Agreement check | call tree, flame graph and functions-by-self-CPU must tell the same story |
