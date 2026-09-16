# Monitoring plan

Which counters, which thresholds, which alerts — and, for each fix this course made, the specific
signal that would show it regressing. A fix with no regression signal is a fix waiting to be undone by
the next release.

## The counters

| Counter | Source | Normal here | Alert at | Why |
|---|---|---|---|---|
| Sessions | `Application.SessionCount`, on the Capacity tab and in the health response | 1–150 | > 135 sustained (90 % of `maxSessions`) | the binding constraint from `Capacity.md` |
| Managed heap / working set | process counters | ~24 MB idle + 6 MB per session | > 70 % of instance memory | the second line of defence in `HealthCheck.json` |
| CPU | process counters | 0–3 % at rest | > 85 % sustained for 5 min | `maxCPU` |
| `healthcheck.wx` refusals | the response body and the server log line | 0 | any, repeated | the instance is turning users away |
| `PERF end … elapsedMs` per scenario | `ILogger`, one structured record per user action | see below | p95 over budget for 10 min | the direct measure of what the user waits for |
| `PERF end … failed=` | the same records | 0 | any burst | a scenario failing still reports its time; a silent failure would not |
| SQL statements per scenario | the Database trace, and the statement counts the app prints | see below | any increase | an N+1 reappearing is a code change, not a load change |

## The PERF records are the production evidence

`ScenarioProbe` writes structured fields, not a concatenated message:

```
PERF end Dashboard Refresh elapsedMs=113 rows=14
PERF end Tickets Search elapsedMs=66 rows=5000
PERF end Tickets Export elapsedMs=171 rows=5000
```

A log pipeline can aggregate those into a p50/p95 per `Scenario`/`UserAction` without parsing prose.
Alert on the **p95 against the budget in `Budget.md`**, not on the average: the average hides the slow
runs, and the slow runs are what people complain about.

## Per fix: what regression looks like

| Module | The fix | What would show it regressing |
|---|---|---|
| 3 | the dashboard counts in the database and assigns prepared values | `Dashboard/Refresh` p95 climbing back over 250 ms; any `calls TicketFormatter…` line reappearing; the refresh issuing more than 4 statements |
| 4 | the detail form releases its roots | retained MB per idle session climbing across a shift; the bus subscriber count on the Capacity tab being greater than the number of open detail forms; the process working set growing with no session growth |
| 5 | virtual grid and lazy tree | `Tickets/Search` statements per page above 1; `Customers/LoadTree` returning more than a handful of nodes; either scenario's p95 over budget |
| 6 | one projection query, and an export that does not block | statements per page above 1 (the N+1 is back); `Tickets/Export` appearing in the **request** timing rather than as a background task; thread-pool starvation counters rising during exports |
| 7 | the health gate | refusals with no corresponding session growth (the thresholds are wrong or memory is leaking); **no** refusals while sessions exceed the model (the gate is disabled or the file did not ship) |

## The one-line check for each

- **A leak came back.** Snapshot A, fifty open/close of a screen, snapshot B. The heap difference should
  stay near zero, and `subscribers` should return to its starting value. This is the Module 4 lab and it
  takes a minute.
- **An N+1 came back.** Run the scenario and read the statement count the screen prints; confirm with a
  Database trace. One statement per page is the contract.
- **A budget is being missed.** `Run the three scenarios ×3` and compare each median with `Budget.md` —
  and check the spread line before believing a small difference.

## What to do when an alert fires

1. Reproduce the scenario by name (they are all in `Scenarios.md`). An alert without a scenario is not
   yet a bug report.
2. Warm up, three runs, take the median and the spread.
3. Pick the tool from `Budget.md` — the same table says which one settles that scenario.
4. Collect **one** trace of **one** scenario, name it with the convention in `TraceNotes.md`, and write
   the note beside it.
5. Only then change code, and record the before/after the way the module documents do.

That loop is the whole course, and the reason the tooling for it is in the application rather than in a
document: the probe, the stages, the call counter, the snapshot buttons and the health readings are all
one click away in the running app.
