# Virtual rows — the pattern the optimized grid uses

**Deliverable 2 · "Replace full-table loading with server-side filter or virtualized access."**

## The desktop habit, and why it does not cross the wire

`LegacyOrderDesk.OrdersForm.ReloadGrid` did `ordersGrid.DataSource = _orderService.Search(filter)`. On one PC with the
database on the LAN that felt instant, so nobody asked how many rows it was. The console keeps that code as
`Legacy/DesktopGridHabits.LoadWholeTable` and runs it on purpose (**Naive port: load all rows**): the service clones and
sorts all 200,000 orders on the server for *this* session, then hands rows to a browser that can show twelve. Multiply by
every open session.

## What the web screen does instead

```
toolbar ──► OrderQuery { Status, Text, SortBy, Descending }        (filter + sort model, no page yet)
              │
              ├─ OrderQueryService.Count(query)      → grid.RowCount           one integer crosses the wire
              ├─ OrderQueryService.Summarize(query)  → footer "Σ n rows · total"  count + sum computed where the data is
              │
grid asks ──► CellValueNeeded(rowIndex, columnIndex)
              │   block = rowIndex - rowIndex % 50
              │   if the block is not cached: OrderQueryService.Page(query.WithPage(block, 50))   → 50 clones, memoized
              └─► e.Value = the cell of the cached row
```

* `DataGridView.VirtualMode = true` and `RowCount = n` tell the grid how big the table is without giving it any rows.
* `CellValueNeeded` is raised per cell for the rows the viewport displays; the page keeps a `Dictionary<int, IReadOnlyList<Order>>`
  keyed by block start so one server query serves 50 × 5 cells.
* `LargeOrderRepository.Query` sorts once per filter+sort key and memoizes the ordered list until the next write, so
  block 2…n of the same query are `Skip/Take` over a list that already exists (the trace says *ordered set memoized*).
* Any write (`OrderService.Save` after the edit dialog) bumps the repository version; the page clears its block cache and
  re-counts, so the row shows the new values without reloading anything else.
* Sorting happens in the query (`OrderSort` + `Descending`); the grid's own column sorting is switched off
  (`SortMode = NotSortable`) so the browser never sorts a partial table.

## Rules that came out of the lab

1. Filter **before** fetch — the default filter (*Open*) is what the users work in; "All" is a choice, not the default.
2. Count and summarize on the server; never derive totals from the rows the browser happens to hold.
3. Fetch in blocks the size of a viewport plus margin (50 here); memoize per filter+sort; invalidate on write.
4. Keep the grid's client-side features (sorting, filtering menus) off when the grid is virtual — they only see a page.
5. Measure with production-like row counts (`OrderStore.Large`, 200,000 rows) — see `GridPerformanceNotes.md`.

## Evidence (what the running app shows)

| Action | Trace |
|---|---|
| page load | `OrderQueryService.Count status=Open · sort=Date desc → n rows`, `grid.RowCount n (VirtualMode …)`, then one `block fetched rows 0–49` as the grid paints |
| scroll / click a row far down | one `block fetched rows k–k+49 … (ordered set memoized)` per new block, nothing for blocks already held |
| **Apply filter** | a new count + Σ, the cache emptied, blocks fetched again on demand |
| **Naive port** | `⚠ boundary load-everything OrderService.GetOrders() cloned + sorted 200,000 rows in … ms …` and a red banner |
