# Deliverable 3 · Saved view definition

`Services/WorkQueues/SavedView.cs` (the record), `Data/SavedViewStore.cs` (where they live),
`Services/WorkQueues/GridState.cs` (what the session remembers).

## A saved view is a stored query, not a cached result

```csharp
public sealed class SavedView
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TenantId { get; set; }     // scope
    public string Owner { get; set; }        // scope
    public WorkQueueQuery Definition { get; set; }
    public DateTime CreatedUtc { get; set; }
}
```

`Definition` is a `WorkQueueQuery` — the same object the pager, the sort and the filter bar produce. Applying a
view puts the definition back into `GridState.Query` and runs `SearchAsync` again, so **the rows are always
current**. Nothing about the result set is stored, so a view can never go stale.

The stored form is JSON, exactly what a `nvarchar` column would hold:

```json
{"TenantId":"contoso","SearchText":null,"Status":"Open","AssignedTo":null,
 "SortBy":"Priority","Descending":true,"Page":1,"PageSize":50}
```

`SavedViewStore.Serialize` / `Deserialize` are that column's read and write — a definition, never a row set.

## Rules

| Rule | Where | Why |
|---|---|---|
| A view never stores a page | `SavedViewStore.Add` writes `definition with { Page = 1 }` | "My critical queue" means a filter, not *page 7 of yesterday's data* |
| A view is scoped to (tenant, owner) | `SavedView.TenantId` / `Owner`; `SessionContext.SwitchTenant` rebuilds the store | A dispatcher's saved view must not leak into another tenant |
| A view is a value, not a reference to rows | `WorkQueueQuery` is a `record` | Two definitions compare by value, which is how the ★ badge knows the screen still *is* the saved view |
| Editing a filter drops the badge | `WorkQueuePage.ReadFiltersIntoGridState` compares `query with { Page = 1 }` to the view definition | The user is told the moment their screen stops being the saved view |

Two views ship with every session, created in `SavedViewStore`'s constructor:

* **My critical queue** — `Status = Open`, sorted by `Priority` descending. The chip the walkthrough shows.
* **Overdue HVAC** — `SearchText = "HVAC"`, `Status = Open`, sorted by `DueAt` ascending.

## Grid state: what survives a refresh

`GridState` is the server-side answer to review question 3.

```csharp
public sealed class GridState
{
    public WorkQueueQuery Query { get; set; }                 // filters + sort + page + page size
    public string SavedViewName { get; set; }                 // null once the user edits a filter
    public Dictionary<int, WorkQueueRow> Selected { get; }     // selection, across pages
    public int LastTotalCount { get; set; }
    public int LastPageCount { get; set; }
    public DateTime LastLoadedUtc { get; set; }
}
```

It is owned by `SessionContext`, which lives in `Application.Session` — per user, never a static. `WorkQueuePage`
holds no copy of the query; it reads `Grid.Query` and writes it back. When a new `WorkQueuePage` is built for the same
session, its `Load` handler reads the same `GridState`, rewrites the filter controls from it, re-runs the same query
and restores the same selection. The user is back where they were — including the page number and the sort
direction.

Switching tenant (`SessionContext.SwitchTenant`) resets the grid state and reloads the saved views on purpose:
a filter or a selection must never survive a tenant change.

## Evidence in the running app

| Do this | What you should see |
|---|---|
| Choose **Overdue HVAC**, click **Apply view** | the stored query runs again; the filter bar and sort change with it; the badge reads ★ Saved view: “Overdue HVAC” |
| Change the status filter, click **Search** | the badge falls back to *custom filters* |
| Click **★ Save current as view** | the new view appears in the drop-down and the badge shows its name |
| Go to page 4, select two rows, reload the browser (F5) | the screen comes back on page 4, same sort, same two rows selected — because none of it was ever in the page |
