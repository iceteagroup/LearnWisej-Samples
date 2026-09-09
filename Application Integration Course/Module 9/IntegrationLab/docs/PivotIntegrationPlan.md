# Read-only pivot integration plan (Module 9, deliverable 1)

**Widget:** `IntegrationLab.Widgets.WorkOrderPivot` (`Widgets/WorkOrderPivot.cs`) hosting the stand-in
`VendorPivot` library (`wwwroot/vendor-pivot.js`) with **DevExtreme-style CustomStore** semantics.
**Adapter:** `wwwroot/pivot-init.js`. **Server:** `MainPage.LoadPivot` (`[WebMethod]`) →
`GridDataController.Handle("pivot", …)` → `WorkOrderStore.Pivot(PivotRequest)`.

## 1. Goal and scope

A read-only cross-tab of work orders: rows = one field (default `site`), columns = another
(default `status`), cells = one measure (`hours` = sum of Hours, or `count`). No editing, no
paging, no layout persistence. It exists to prove two things fast: the vendor runs inside a
Wisej.NET widget container, and the data contract works end to end.

## 2. Data contract

Request (typed on the server, `Contracts/PivotContracts.cs`):

```csharp
public sealed class PivotRequest { string RowField; string ColumnField; string Measure; }
```

Whitelists live in `WorkOrderStore`: `PivotFields = site, status, priority, assignee`,
`PivotMeasures = hours, count`. Anything else is a **400** before any data is touched.

Response: the **JSON array** option from the lesson. Every non-empty cell is one tuple; the
key lists carry the axis order the server chose (canonical order for status/priority/site,
alphabetical otherwise):

```json
{
  "status": 200,
  "rowField": "site", "columnField": "status", "measure": "hours",
  "rowKeys": ["Plant A", "Plant B", "Plant C", "Depot", "Field"],
  "columnKeys": ["Open", "In progress", "On hold", "Closed"],
  "cells": [ { "row": "Plant A", "column": "Open", "value": 118.5 }, … ]
}
```

On a violation the same call returns `{ "status": 400, "message": "…" }`. The status travels
*inside* the object because a WebMethod has no HTTP status of its own; the vendor treats any
`status >= 400` as a failed load.

Why a flat array and not an OLAP-style response: the vendor computes row/column totals itself,
the payload stays small (5×4 = at most 20 tuples) and the same array can feed a bar chart or a
CSV export unchanged. An OLAP-style response (nested axes + a data matrix) becomes worth it
when there are multiple measures or hierarchical axes; the request shape would not change.

## 3. Vendor mapping (CustomStore → contract)

| DevExtreme concept | What the adapter does (`pivot-init.js`) |
|---|---|
| `store.key` | `options.rowField` |
| `store.load(loadOptions)` | `App.MainPage.LoadPivotAsync(loadOptions.rowField, loadOptions.columnField, loadOptions.measure)` — returns a Promise the vendor awaits |
| result | the object above; the vendor renders `cells` and emits `dataloaded` |
| failure | the Promise rejects, or the result has `status >= 400` → vendor `error` → adapter `error { phase:"load", status, message }` |

The callback style (`App.MainPage.LoadPivot(a, b, c, function (result) { … })`) is kept as a
comment next to the Promise call. Wisej registers both names on the main page for every
`[WebMethod]`; default parameter values are not supported, so all three arguments are always sent.

## 4. Configuration: JSON options, on purpose

The prototype has **no typed properties**. `MainPage` hands the vendor option object to
`Options` as-is:

```csharp
this.pivotWorkOrders.Options = new { rowField = "site", columnField = "status", measure = "hours" };
```

Wisej camelCases it and calls `init(options)`; replacing the object later (button
**Pivot Site×Priority**) is a first-level change, so `update(options, old)` runs and the vendor
loads again. This is the "fast now" side of the trade-off.

## 5. When to promote to typed properties

Promote before the control is reused by another developer or screen — concretely when any of
these becomes true:

- a second page needs the pivot (defaults, validation and Designer support are now worth it);
- an option must be validated server-side before it reaches the browser (`RowField` should
  throw on a non-whitelisted field the way `WorkOrderGrid.PageSize` throws on 1000);
- a reviewer has to know which of the vendor's options the application actually uses.

The promoted version looks exactly like `WorkOrderGrid`: `RowField`, `ColumnField`, `Measure`
setters that validate and write one first-level `Options` field each, plus a protected
`OnConfigureOptions(dynamic options)` for the rare extra option.

## 6. State ownership

| Where | What |
|---|---|
| Server (`WorkOrderStore`) | the work orders, the whitelists, the aggregation |
| Server (`MainPage`) | the current axes (the JSON option object) |
| Browser (`VendorPivot`) | rendering, totals arithmetic, hover, loading state |

Nothing the browser sends is trusted: the WebMethod arguments are re-validated on every call.

## 7. Evidence (what the running app shows)

- Page load: trace `→ .NET→JS render pivot → init(options) {rowField:"site",…}` followed by
  `← JS→.NET LoadPivot (WebMethod) {…} → 200 (5×4, N cells)` and `← JS→.NET dataLoaded {rows:5,columns:4,…}`;
  the pivot card shows the 5×4 table with row, column and grand totals.
- **Reload pivot**: one more WebMethod line, same result.
- **Pivot Site×Priority**: `→ .NET→JS update(options) {…columnField:"priority",measure:"count"}`
  then a WebMethod line with `measure:"count"`; the table re-renders with integer counts.
- Clicking a cell: `← JS→.NET cellClick {rowKey:"Plant A",columnKey:"Open",value:…}` and
  `• server Pivot CellClick fired in C#`.
- Failure path (not wired to a button; call `LoadPivot("site","site","hours")` from the
  browser console as `App.MainPage.LoadPivotAsync("site","site","hours")`): the trace shows
  `→ 400 rowField and columnField must differ.` and the pivot status turns red.
