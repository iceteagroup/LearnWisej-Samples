# OperationsConsole · Mastering the Control Library · Module 5

Lab build for **Module 5 · DataGridView Mastery**: the **DataGridView** section is the Orders grid —

- `ordersGrid` + an `ordersSource` `BindingSource`, `AutoGenerateColumns = false` and six explicit columns;
- a status cell with `AllowHtml` whose badge is built in `ordersGrid_CellFormatting` with `WebUtility.HtmlEncode`;
- a custom editor (`MonthCalendar` on `colDueDate.Editor`, moved by `CellBeginEdit` / `CellEndEdit`) and a command
  column (`colOpen`) that calls `OrderService.GetOrder(id)`;
- a virtual-mode path: `RowCount` from the service, `CellValueNeeded` served by `OrderCache`, pages prefetched in `DataRead`;
- a filter strip docked Top and a status strip docked Bottom, layered with `SendToBack` / `BringToFront`.

This module fills `Sections/DataGridViewPage` and adds `Orders/`, two models, two services and `docs/GridDecisions.md`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 5/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5705
```

Then open <http://localhost:5705> and select **DataGridView**.

## What to try

| Action | What you should see |
|---|---|
| Open **DataGridView** | the first 250 of 4,000 orders with status badges; amber status suggests narrowing the filter or Virtual mode |
| Type `Contoso`, **Apply** | the loader for a moment, then the matching orders and a green status |
| Pick a status, **Apply** | the same, filtered |
| Type `zzzz`, **Apply** | the grid's "No orders match this filter." card, amber status |
| Click a row | status strip `Selected: SO-100123`, StatusBar `Record: SO-100123` |
| Click **Open** in a row | an info Toast and the whole order on the status line |
| Double-click a due date, pick a later day, leave the cell | green status and Toast; the cell shows the new date |
| …pick a day in the past | red status "A due date in the past cannot be saved …"; the cell keeps the stored date |
| Tick **Virtual mode** | `Rows: 4,000`; scroll and the page-fetch count grows by one per 200-row block |
| Tick **Simulate service failure**, **Apply** | red status and an error Toast; the rows on screen stay. Untick and Apply again to recover |
| **Refresh** on the ToolBar | re-runs the current filter on the current path |

## Where things live

```
OperationsConsole/
├─ Sections/DataGridViewPage.cs / .Designer.cs   grid, strips, cell handlers
├─ Orders/OrderFilter.cs, OrderColumns.cs, OrderUpdateResult.cs
├─ Models/OrderRow.cs, OrderPage.cs
├─ Services/OrderService.cs      4,000 generated orders, GetOrders / Count / GetPage / GetOrder / UpdateDueDate, SimulateFailure
├─ Services/OrderCache.cs        the page cache behind CellValueNeeded (200-row pages, 4 pages, never throws)
└─ docs/GridDecisions.md         column, editor, large-data and composition decisions
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| In-memory `OrderService` returning `OrderRow` view models | `Services/OrderService.cs`, `Models/OrderRow.cs` |
| `ordersGrid` + `ordersSource`, `AutoGenerateColumns = false`, explicit columns, `BindOrders(IEnumerable<OrderRow>)` | the designer (`colNumber` … `colOpen`), `ConfigureGrid()`, `BindOrders()` |
| Status column `AllowHtml` + `ordersGrid_CellFormatting` with `WebUtility.HtmlEncode` and `FormattingApplied` | `colStatus.AllowHtml`; `ordersGrid_CellFormatting()` → `StatusBadge()` |
| Custom editor or command column | both: `colDueDate.Editor = dueDateCalendar` with `CellBeginEdit` / `CellEndEdit` → `ApplyDueDate()`; `colOpen` → `ordersGrid_CellContentClick()` → `OpenOrder()` |
| `VirtualMode`, `RowCount`, `OrderCache` behind `CellValueNeeded`, prefetch in `DataRead` | `ShowVirtualOrders()`, `ordersGrid_CellValueNeeded()`, `ordersGrid_DataRead()`, `Services/OrderCache.cs` |
| Filter strip and status strip, layered with `BringToFront` / `SendToBack` | `pnlFilterStrip`, `pnlGridStatus`; `ComposeGrid()` |
| Switch between bound and virtual from the filter strip | `chkVirtualMode_CheckedChanged()` → `LoadOrders()` |
| Loading, empty filter result, failed service call, rejected edit | `LoadOrdersAsync()` (`ShowLoader`), `NoDataMessage`, `ReportServiceFailure()`, `ApplyDueDate()` |
| Note on the decisions | `docs/GridDecisions.md` |

## Self-check answers

- **If `OrderRow` gains two properties, what changes with generated columns and with explicit columns?**
  Generated: two extra columns appear with default widths and formats, and the designed layout is gone. Explicit:
  nothing changes — the six columns bind by `DataPropertyName`.
- **What happens to the database when a user drags through 200,000 rows, and what absorbs it?**
  `CellValueNeeded` fires once per visible cell, so a naive handler issues hundreds of queries per screen.
  `OrderCache` absorbs it: a hit never leaves memory, a miss fetches a whole page, and `DataRead` fetches ahead.
- **Why does the badge belong in `CellFormatting` and not in the model, and why encode?**
  The badge is display, not data: markup in `OrderRow.Status` would break sorting, filtering and comparisons. Without
  encoding, any `<` a human typed becomes markup — `Bergström <Nordic> AB` would lose part of its name, and a script
  tag would run.

## Known simplifications

- 4,000 generated orders instead of the video's 200,000 — enough to show page fetches and eviction.
- The bound path is capped at 250 rows on purpose.
- The due-date column is read-only on the virtual path (the cache is a read model).
- Wisej.NET 4.1 has no `CellContentClick`; the command column is wired to `CellClick` and keeps the lab's handler name.
