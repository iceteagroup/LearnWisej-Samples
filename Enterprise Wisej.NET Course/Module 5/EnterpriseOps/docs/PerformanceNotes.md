# Deliverable 5 · Performance notes — Enterprise Work Queue

Measured, not guessed. Users judge a screen by how long they wait, not by how many rows it could hold, so the
numbers below are taken at the moments they notice.

## The data set the numbers come from

`Data/WorkOrderStore.cs` seeds **6,000 work orders** deterministically (`new Random(5)`), so every run measures the
same table.

| | rows | open | pages at 50 / page |
|---|---:|---:|---:|
| contoso | 2,977 | 2,319 | 47 |
| fabrikam | 1,776 | 1,379 | 28 |
| northwind | 1,247 | 961 | 20 |
| **total** | **6,000** | **4,659** | |

The default view — *My critical queue*, `Status = Open`, sorted by `Priority` descending — is therefore **50 of
2,319 matching rows**, 47 pages.

## Payload: one page vs the table

Serialized exactly as the projection travels (`JsonSerializer.SerializeToUtf8Bytes`, contoso, same machine):

| What | Rows | Bytes | Ratio |
|---|---:|---:|---:|
| One page of `WorkQueueRow` (the default view) | 50 | **13,996 B ≈ 13.7 KB** | 1× |
| Whole tenant, same projection (the anti-pattern) | 2,977 | **825,599 B ≈ 806 KB** | **59×** |
| Whole tenant as `WorkOrder` entities | 2,977 | 1,003,018 B ≈ 979 KB | 72× |

Two lessons in one table:

1. **Paging is the big win.** 59× less data crosses the wire and sits in the session, and the ratio grows with the
   table — the page size is constant, the table is not.
2. **The projection is the second win.** Even without paging, `WorkQueueRow` is ~18% smaller than the entity here,
   and that gap widens with every navigation property a real entity drags along. The projection also removes the
   per-row work the screen would otherwise do (status label, age, overdue, permission flags).

The server log prints the page figure after every search (`Data: … page payload ≈ … KB`); the whole-tenant figures
were taken once, with the same projection and the same serializer, for these notes.

## Perceived-performance budget

The three moments a user notices, with the target and where the actual is shown.

| Moment | Budget | Where the actual is shown | Notes |
|---|---|---|---|
| Open the screen → first rows | ≤ 500 ms | status bar: `… ms`, log `Data: 50 of 2,319 … in N ms` | one `SearchAsync`, `PageSize` rows. `SearchAsync` adds a deliberate `Task.Delay(15)` to stand in for a database that is not on this machine |
| Change a filter → grid updates | ≤ 300 ms | same status-bar line, page reset to 1 | if this is slow, the filter is not reaching the database |
| Click a batch action → first progress | ≤ 1,000 ms | the progress bar and `Reassigning to … — 0 of N…` | shown **before** the first row is processed, so the wait never looks like a freeze |
| Batch itself | as slow as its writes | `Job: done … in N ms` | `BatchReassignWorkflow.DelayFor` paces the lab: 450 ms per row up to 10 rows (3 rows ≈ 1.4 s, every line readable), 60 ms above that (a 50-row page ≈ 3 s) |

Re-measure on your machine and write the numbers down; a budget nobody checks is a wish.

## What to tune when a number is bad

| Symptom | Usual cause | Fix |
|---|---|---|
| Slow first load | too many columns, or no index for the default sort | shrink the projection; index the default `ORDER BY` |
| Slow filter change | the filter is not translated to SQL — it is being applied in memory after a `ToList()` | keep the whole chain `IQueryable` until `Skip/Take` |
| Page 400 much slower than page 1 | deep `OFFSET` | keyset ("seek") paging on the sort key + id |
| Rows appear twice while paging | non-deterministic sort | add a total-order tie-breaker — this sample uses `ThenBy(o => o.Id)` |
| Batch feels frozen | no feedback before the first row | report progress before the loop, and after every row |
| Memory grows per session | the session is holding a result set | hold the **query**, not the rows: `GridState` keeps a `WorkQueueQuery`, never a list |

## Virtualized thinking

Every list on this screen is designed as if the table has no upper bound:

* the grid binds one page, and the pager asks for the next one;
* `PageSize` is clamped server-side to `MaxPageSize = 200`, so no request can ask for the table;
* the technician and status drop-downs are **small closed sets**, not "every row of a table" — a drop-down over an
  unbounded set would need the same paging treatment (a searchable picker), which is the usual place this rule is
  forgotten;
* the cross-page selection stores keys and versions (`Dictionary<int, WorkQueueRow>`), so selecting many rows costs
  bytes, not megabytes — a "select all 2,319 matching" feature would store the **query**, not the keys;
* an export would stream pages, not materialize the result.

## Measurement method

* `WorkQueueQueryService.LastElapsedMs` — a `Stopwatch` around filter + sort + skip/take + project.
* `WorkQueueQueryService.LastPayloadBytes` — the projected page serialized to UTF-8 JSON, i.e. what would actually
  travel.
* The whole-tenant row — the same two measurements over every contoso row, projected with
  `WorkQueueQueryService.Project`, taken once for these notes.
* `BatchResult.ElapsedMs` — a `Stopwatch` around the whole workflow, reported with the per-row report.

The running figures are written to the server log (`System.Diagnostics.Trace`), so a reviewer can read them without a
profiler.
