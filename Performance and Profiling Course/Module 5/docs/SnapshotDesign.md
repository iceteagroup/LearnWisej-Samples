# The snapshot, and where work belongs

The Module 3 change in one sentence: **the page stopped computing and started assigning.**

## The handler, before and after

Before, `RunRefresh` owned the query, the loop, the counting, the formatting and the control lifetime.
After, it owns the button state and four assignments:

```csharp
using var scope = _probe.Measure("Dashboard", "Refresh");

if (_refreshing) return;          // a second click is dropped, not queued
_refreshing = true;
btnRefresh.Enabled = false;
try
{
    var snapshot = _dashboardService.GetSnapshot();   // counted and formatted once

    _lblOpenValue.Text    = snapshot.OpenTicketsText; // assign, do not format
    _lblOverdueValue.Text = snapshot.OverdueTicketsText;
    _lblAvgAgeValue.Text  = snapshot.AverageAgeText;
    ApplyChart(snapshot.ChartRows);
}
finally { btnRefresh.Enabled = true; _refreshing = false; }
```

The `finally` matters as much as the rest: a refresh that throws must still give the button back. The
failure path is in the `catch` above it — the probe writes its end record either way, the status line
goes red, and the banner explains what happened.

## Why a view model and not "just cache the query"

A cache would have kept the same 50,000 rows and made them older. The snapshot is a different shape:
it contains **only what the screen shows**, in the form the screen shows it. Three strings, fourteen
labelled numbers, and the time it was true.

That shape has three consequences worth stating:

- the query can be an aggregate, because nobody downstream needs the rows;
- the formatting can happen once, because there is one value per KPI rather than one per row;
- the page cannot accidentally do per-row work, because it never sees a row.

## What may be cached, and where

| Data | Where it belongs | Why |
|---|---|---|
| Status and priority labels | **application scope** (a static dictionary in `TicketFormatter`) | immutable, identical for every user, tiny |
| The current filter, the selected tab, the last snapshot time | **session** | per user by definition; a static field here would show one user another user's state |
| Ticket rows, counts, anything the database owns | **nowhere** | cache them and you have invented a staleness window; if you need one, decide its length on purpose |

This is also the memory rule Module 4 measures: every object you decide to keep is memory per session
multiplied by your session count, and every static field you keep is memory that no session will ever
release.

## The double-click guard

`_refreshing` is a page field, checked on entry and cleared in the `finally`. Wisej.NET does deliver the
second click — the button is disabled on the server and the disable does not reach the browser until
the handler returns, so a fast second click is already in flight. Dropping it is correct: two refreshes
in a row produce the same screen, and the second one is work nobody is waiting for. The status line
says the click was dropped rather than silently ignoring it.

## What did not change

`ScenarioProbe`, the budget, the PERF log, the failure path and the scenario definition are all exactly
as they were in Module 2. That is deliberate: **the measurement has to survive the fix**, or the
before/after table is comparing two different things.
