# Deliverable 1 · Paged query service

`Services/WorkQueues/WorkQueueQueryService.cs` — implements `IWorkQueueQueryService` from
`Services/WorkQueues/HighVolumeGridPatterns.cs`.

```csharp
public interface IWorkQueueQueryService
{
    Task<PagedResult<WorkQueueRow>> SearchAsync(WorkQueueQuery query);
}
```

## What goes in, what comes out

`WorkQueueQuery` carries **everything the server needs to answer one grid request**: the tenant, the free-text
search, the status and assigned-to filters, the sort column and direction, and `Page` + `PageSize`. Nothing about
the request is implicit and nothing is left to the browser.

`PagedResult<WorkQueueRow>` carries **the rows of that page and the total count**. The total is what lets the pager
say *Page 3 of 47* without anybody ever loading 47 pages.

## The answer to review question 1

> How many records are loaded for the first screen?

Exactly `PageSize` — 50 by default, clamped to `MaxPageSize = 200`. The screen loads one page and a count, and it
does that on every filter change, every sort change and every page click. The trace proves it on each search:

```
Service: SearchAsync page 1 size 50 · status Open · assigned any · search "" · sort Priority desc
Data:    50 of 2,319 matching rows in 3 ms — skip 0, take 50; page payload ≈ 13.7 KB
```

If the answer were "all of them", the design would not have started. The bottom bar has an
**Anti-pattern: load everything** button that measures exactly that alternative — see `PerformanceNotes.md`.

## The pipeline

| Step | Method | What it does |
|---|---|---|
| 1 · tenant guard | `SearchAsync` | Rejects a query whose `TenantId` is not the session's, whatever the UI sent. Traced as `Security:`. |
| 2 · normalize | `Normalize` | Clamps `Page ≥ 1` and `PageSize` into `1…MaxPageSize`, trims blank filters to `null`. A forged request cannot ask for the table. |
| 3 · filter | `ApplyFilters` | Tenant, then status (`Open` = anything but Completed/Cancelled, or an exact `WorkOrderStatus`), then assigned-to, then the free text over number / title / customer / site. |
| 4 · sort | `ApplySort` | One of `Number · Title · Status · Priority · AssignedTo · DueAt · AgeDays`, with `ThenBy(o => o.Id)` as a deterministic tie-breaker so the same query always returns the same page. |
| 5 · page | `Skip/Take` | `Skip((Page - 1) * PageSize).Take(PageSize)` — the only rows that leave the data layer. |
| 6 · project | `Project` | Entity → `WorkQueueRow`, including the permission flags. See `SearchProjectionModel.md`. |

Against a real database the same shape becomes
`Where(…).OrderBy(…).ThenBy(…).Skip(…).Take(…).Select(…)` and the database does the filtering, because it has the
indexes and the memory for it. `Data/WorkOrderStore.cs` stands in for the table here: 6,000 deterministically
seeded work orders across three tenants (contoso 2,977 · fabrikam 1,776 · northwind 1,247), held in memory for the
life of the process.

## Why a deterministic tie-breaker matters

Without `ThenBy(o => o.Id)`, two rows with the same priority can swap places between the request for page 3 and the
request for page 4 — and a row is then shown twice, or never. Any server-side pager needs a total order, not just
the column the user clicked.

## State ownership

The service is **stateless per call**; it holds no page and no cursor. The page, sort, filters and cross-page
selection live in `Services/WorkQueues/GridState.cs`, owned by `SessionContext` — not by `WorkQueuePage`. That is
what makes *Simulate refresh* work: the page object is discarded and rebuilt, and the new page reads the same grid
state back. `LastElapsedMs` and `LastPayloadBytes` on the service are measurements of the last call, used only by
the footer and by the notes.

## Security

Two rules, both server-side:

1. A query is always scoped to `SessionContext.TenantId`. A query naming another tenant throws
   `UnauthorizedAccessException` and is traced — it never returns an empty result that looks like "no data".
2. `PageSize` is clamped. A client asking for `PageSize = 1000000` gets 200.

## Evidence in the running app

| Do this | The trace / footer shows |
|---|---|
| Open the app | `Data: 50 of 2,319 matching rows in … ms — skip 0, take 50; page payload ≈ 13.7 KB` |
| Click **▶** | a second `SearchAsync page 2` with `skip 50, take 50` — the first page is not kept |
| Type `pump`, press Enter | `search "pump"` in the service line, ~110 matching rows, page reset to 1 |
| Click the **Priority** header twice | `sort Priority asc` then `desc`, page reset to 1 both times |
| Choose **200 / page** | `size 200`; choosing a larger value than `MaxPageSize` would be clamped |
