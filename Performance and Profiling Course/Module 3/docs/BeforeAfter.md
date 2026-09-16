# `Dashboard/Refresh` — before and after

Identical scenario, identical dataset, identical build, identical warm-up. Before = the app in
`Module 2`. After = the app in this folder. Both numbers are the **median of three warm runs** taken
with **Run the three scenarios ×3**, which also prints the spread.

## The table

| | Before (Module 2) | After (Module 3) |
|---|---:|---:|
| Median of three warm runs | **476 ms** | **113 ms** |
| Spread of the three runs | 11 % of the median | 13 % of the median |
| Cold first run | 972 ms | 218 ms |
| Rows materialised into the session | 50,000 entities | **0** |
| SQL statements | 1 | 4 |
| `TicketFormatter.FormatRow` calls | 50,000 | **0** |
| Controls disposed and recreated per refresh | 6 | **0** |
| Stage: get the data | `query + materialise` 894 ms cold | `snapshot (4 aggregate queries, formatted once)` 113 ms |
| Stage: put it on screen | `format rows` 60 ms + `rebuild KPI panel` 4 ms + `rebuild chart` 10 ms | `assign` **0 ms** |
| Budget (250 ms) | over | within |

The improvement is **4.2×**, against a noise floor of 11–13 %. That is a result, not a rerun.

## The hot path genuinely changed

The Module 2 hot path left `DashboardPage.RunRefresh` and went into EF Core's materialisation pipeline
and the SQLite data reader. In Module 3 there is no materialisation frame left to find: the four
statements are aggregates, `GetSnapshot` returns four strings and fourteen numbers, and the slowest
thing in the scenario is the SQLite `AVG(...)` over 50,000 rows — which is the database doing the work
the process used to do.

That is the check the lab asks for: **the hot path has changed shape, not merely shrunk.** If the
after-trace still showed the same top frames with smaller numbers, the fix would have been a tweak, and
the next dataset size would have undone it.

## What was fixed, in the order it mattered

1. **The counting moved into the database.** Three KPI numbers are three aggregates, not 50,000 rows and
   a loop. This is the whole 4× — everything below is worth doing and would not have been measurable on
   its own.
2. **The formatting moved into the service and happens once.** `DashboardSnapshot` carries
   `OpenTicketsText`, `OverdueTicketsText`, `AverageAgeText` and `ChartRows`, already formatted in the
   session's culture. The page assigns them.
3. **The KPI controls are built once.** `RebuildKpiPanel` is gone; the three value labels are fields,
   updated in place. No construction, no layout, no theming, nothing to dispose.
4. **Immutable labels are cached at application scope.** The status and priority label table is a static
   dictionary — it is the same for every user and never changes. Per-user state stays in the session;
   nothing mutable is cached anywhere.
5. **A second click cannot queue a second run.** `_refreshing` drops it, and the status line says so.

## The prediction from Module 2, checked

`Module 2/docs/RootCause.md` predicted "well under 100 ms" and said the root cause was wrong if the
result was only 10 % better. The result is 113 ms — a little above the prediction and far outside the
noise floor, so the root cause stands: the cost was materialising rows to count them.

It also predicted that fixing the **obvious** suspects alone (the per-row formatter and the control
rebuild) would have bought about 7 %. That number came from the stage breakdown, and it is why those
two changes are items 2 and 3 in the list above rather than item 1.

## What the numbers on screen say

Identical before and after, which is the other half of a real fix:

| KPI | Module 2 | Module 3 |
|---|---:|---:|
| Open | 11,907 | 11,907 |
| Overdue | 21,642 | 21,645 |
| Avg age | 90.5 d | 90.5 d |

(The overdue count moves by a handful between runs because "overdue" is measured against the clock, not
because the implementation changed.)

## Remaining risks

- **Staleness.** The old refresh computed everything from rows read in that request. The new one returns
  a snapshot that is true at `GeneratedAt` — which the screen now shows (`as of 11:31:45`). Nothing
  caches it yet, so the window is one refresh wide; the moment anyone adds caching to `GetSnapshot`, the
  window becomes the cache lifetime and has to be a decision, not an accident.
- **Four statements instead of one.** Cheaper here, but four round-trips instead of one against a remote
  database. If latency ever dominates, the four aggregates become one statement with four subqueries —
  a change that should be made from a Database trace, not from this paragraph.
- **The average-age query is provider-specific.** `julianday()` is SQLite. Moving to SQL Server means
  rewriting that one statement, and it is the only place in the sample where that is true.
- **The chart still assumes fourteen days.** The grouped query is bounded by date, so it stays cheap as
  the table grows; the page is bounded by the fourteen bars it draws.
- **Nothing here touched `Tickets/Search` (414 ms, still over its 300 ms budget) or the customer tree
  (158 ms, over 100 ms).** They are Modules 4, 5 and 6. The dashboard being fast does not make the app
  fast.
