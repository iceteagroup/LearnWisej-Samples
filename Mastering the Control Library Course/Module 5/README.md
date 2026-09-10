# OperationsConsole · Mastering the Control Library · Module 5

Local lab build for **Module 5 · DataGridView Mastery**. It follows the lesson guide, the lab / exam guide and the
walkthrough video *"The Orders grid, from bound to virtual"*: the **Orders** section of the Operations Console is now a
real `DataGridView` with

- `ordersGrid` + an `ordersSource` `BindingSource`, `AutoGenerateColumns = false` and six columns defined on purpose
  (`DataPropertyName`, `HeaderText`, `Width`, alignment, format, read-only state, sort mode, column type);
- a **status cell** with `AllowHtml` whose badge is built in `ordersGrid_CellFormatting` with `WebUtility.HtmlEncode`
  and `e.FormattingApplied = true` — the model keeps the plain status;
- a **custom editor** (`MonthCalendar` assigned to `colDueDate.Editor`, moved in and out of the cell by
  `CellBeginEdit` / `CellEndEdit`) and a **command column** (`colOpen`) that calls `OrderService.GetOrder(id)`;
- a **large-data path**: `VirtualMode = true`, `RowCount` from the service, `ordersGrid_CellValueNeeded` served by
  `OrderCache`, pages prefetched in `ordersGrid_DataRead` — one fetch per page, visible in the Event log;
- a **composed grid**: a filter strip docked Top and a status strip docked Bottom inside the same card, layered with
  `SendToBack` / `BringToFront`, with the grid still a plain child whose API is fully available.

The course is cumulative: everything Module 1 built (the shell, the `Shell/` contract, the Event log card) is here
unchanged, and this module only replaces the body of `Sections/DataGridViewPage` and adds its own `Orders/`,
`Models/`, `Services/` and `docs/` files.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Mastering the Control Library Course/Module 5/OperationsConsole"
dotnet run -f net10.0 --urls http://localhost:5705
```

Then open <http://localhost:5705> and click **DataGridView** in the navigation. (Visual Studio: open
`OperationsConsole.slnx` in this folder, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

Everything below is on the Orders page; every step writes to the **Event log** card on the right and to the status
area at the bottom of the shell.

| Action | Path | What you should see |
|---|---|---|
| Open **DataGridView** | success | 4 000 orders generated in memory; the grid shows the first 250 with badges; log `compose pnlGridCard · dock order = pnlFilterStrip (Top) → pnlGridStatus (Bottom) → ordersGrid (Fill)` and `OrderService.GetOrders(no filter, max 250) → 250 of 4000 matching rows` |
| — | bound-path boundary | amber status "Showing the first 250 of 4 000 matching orders — narrow the filter, or tick Virtual mode."; strip `Rows: 250 of 4,000 (bound)` |
| Type `Contoso`, press **Apply** | loading + success | the grid shows its loader and the buttons grey out for ~0.4 s, then a green status with the match count; the log shows one `GetOrders` call |
| Pick a status in the ComboBox, **Apply** | success | the same, filtered; the badge colour of every row matches the chosen status |
| Type `zzzz`, **Apply** | empty result | the grid's own "No orders match this filter." card, amber status, no exception anywhere |
| **Clear** | recovery | filter emptied, the first 250 orders again |
| Click a row | selection | strip `Selected: SO-100123`, the shell's diagnostic panel shows `Record: SO-100123` |
| Click **Open** in a row | command column | log `OrderService.GetOrder("SO-100123") → Confirmed, due 2026-10-02 [fetch #n]`, an info Toast, the whole order on the status line |
| **Open selected** (command row) | same path from a button | identical log lines — the button and the cell call the same method |
| **Due date + 7 days** | accepted edit | log `OrderService.UpdateDueDate(…) → accepted`, green status, `icon-check` Toast, the cell shows the new date |
| **Due date last week (rejected)** | rejected edit | log `→ rejected: due date is in the past`, **red** status "A due date in the past cannot be saved — pick … or later.", warning Toast, the cell keeps the stored date |
| Tick **MonthCalendar editor**, then double-click a due-date cell | custom editor | log `colDueDate.Editor = dueDateCalendar (MonthCalendar)`, then `CellBeginEdit … → MonthCalendar (custom editor)`; pick a day, press Tab or click elsewhere → `CellEndEdit … editor returned …` and the accepted / rejected path above |
| Untick it, double-click a due-date cell | built-in editor | `colDueDate.Editor = null` — the `DataGridViewDateTimePickerColumn` edits the cell itself; the same commit logic runs |
| Select a row, **Inject unsafe status** | encoding | the badge shows `On hold <script>alert('xss')</script>` as text, nothing executes; amber status explains that `WebUtility.HtmlEncode` is what makes `AllowHtml` safe |
| Tick **Virtual mode** | large-data path | log `virtual path · RowCount = 4000, rows created: 0 — values arrive through CellValueNeeded`, then `DataRead rows 0–29 → 1 page fetch(es) · 1 page fetch · … · 1/4 pages in memory`; strip `Rows: 4,000 (virtual)` |
| Scroll / drag the scrollbar | one fetch per page | each new block logs **one** `OrderService.GetPage(first=…, count=200)`; scrolling back over held rows logs `DataRead … → 0 page fetch(es)`; the strip's cache line counts hits, misses and pages |
| Scroll far, then far back | cache ceiling | after four pages: `OrderCache dropped the oldest page (rows 0–199 of 4000) — 4 pages is the ceiling` |
| Tick **Simulate service failure**, then **Load orders** | failure | log `✗ the orders service did not answer — OrderServiceException` + the message, red status "The orders service is not answering. The list on screen is unchanged …", error Toast; the rows on screen stay |
| …with **Virtual mode** on, scroll into a new page | failure on the virtual path | `✗ OrderCache could not fetch the page holding row …`, the same friendly sentence, empty cells instead of an exception page |
| Untick it, **Load orders** | recovery | green status, the list is back |
| **Refresh** in the shell's command bar | ISection | `btnRefresh → DataGridViewPage.RefreshSection()` re-runs the current filter on the current path and moves both "last refresh" stamps |

## Where things live

```
OperationsConsole/
├─ Sections/DataGridViewPage.cs / .Designer.cs   THE MODULE 5 SCREEN — grid, strips, cell handlers, commands
├─ Orders/
│  ├─ OrderFilter.cs             what the filter strip asks for (search + status), Describe(), Clone(), SameAs()
│  ├─ OrderColumns.cs            column index ↔ OrderRow property map used by the virtual path
│  └─ OrderUpdateResult.cs       accepted / rejected + a message written for the user
├─ Models/
│  ├─ OrderRow.cs                the view model: Number, Customer, DueDate, Total, Status
│  └─ OrderPage.cs               one page of the filtered result (FirstIndex, Rows, TotalCount)
├─ Services/
│  ├─ OrderService.cs            4 000 generated orders, GetOrders / Count / GetPage / GetOrder / UpdateDueDate /
│  │                             SetStatus, the business rules, SimulateFailure and the FetchCount service log
│  └─ OrderCache.cs              the page cache behind CellValueNeeded (200-row pages, 4 pages, never throws)
├─ docs/GridDecisions.md         deliverable: column, editor, large-data and composition decisions + Evidence
├─ MainPage.cs / .Designer.cs    the shell from Module 1 (untouched)
├─ Shell/                        IConsoleShell, ConsoleLog, ISection (untouched)
├─ Sections/                     the other five section pages (untouched)
├─ Models/  Services/            SectionKey, SectionInfo, SectionCatalog from Module 1 (untouched)
└─ Program.cs / Startup.cs       session entry point and Kestrel host (untouched)
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| A small in-memory `OrderService` returning `OrderRow` view models (number, customer, due date, total, status) | `Services/OrderService.cs`, `Models/OrderRow.cs` — 4 000 deterministic rows, `Statuses`, `SimulateFailure`, `FetchCount` |
| `ordersGrid` + `ordersSource`, `AutoGenerateColumns = false`, every column explicit, bound in `BindOrders(IEnumerable<OrderRow>)` | `Sections/DataGridViewPage.Designer.cs` (`colNumber` … `colOpen`), `DataGridViewPage.ConfigureGrid()` and `BindOrders()` |
| Status column `AllowHtml` + `ordersGrid_CellFormatting` with `WebUtility.HtmlEncode` and `FormattingApplied` | `colStatus.AllowHtml = true` in the designer; `DataGridViewPage.ordersGrid_CellFormatting()` → `StatusBadge()` |
| Custom editor **or** command column | both: `colDueDate.Editor = dueDateCalendar` in `ApplyEditorChoice()` with `ordersGrid_CellBeginEdit` / `ordersGrid_CellEndEdit` / `EditedDueDate()` / `ApplyDueDate()`, and `colOpen` handled by `ordersGrid_CellContentClick()` → `OpenOrder()` |
| Large-data path: `VirtualMode`, `RowCount`, `OrderCache` behind `CellValueNeeded`, prefetch in `DataRead` | `ShowVirtualOrders()`, `ordersGrid_CellValueNeeded()`, `ordersGrid_DataRead()`, `Services/OrderCache.cs` |
| Compose the grid: filter strip above, status strip below, layered with `BringToFront` / `SendToBack` | `pnlFilterStrip`, `pnlGridStatus` in the designer; `ComposeGrid()` + `DescribeDockOrder()` |
| Switch between the bound and the virtual version from the filter strip; business calls stay in `OrderService` | `chkVirtualMode_CheckedChanged()` → `LoadOrders()` → `ShowBoundOrders()` / `ShowVirtualOrders()`; no cell handler calls anything but a named method |
| Show every path: loading, empty filter result, failed service call, rejected edit — no leaked internals | `LoadOrdersAsync()` (`ShowLoader`), `ordersGrid.NoDataMessage`, `ReportServiceFailure()`, `ApplyDueDate()` rejection branch |
| Run with realistic volume, scroll the virtual grid watching the service log, write the decisions note | `docs/GridDecisions.md` (with an Evidence table) |

## Deliverables

- [`docs/GridDecisions.md`](OperationsConsole/docs/GridDecisions.md) — column, editor, large-data and composition
  decisions, plus the Evidence table of what each path shows at runtime.
- [`docs/ControlSelection.md`](OperationsConsole/docs/ControlSelection.md) — Module 1's deliverable, unchanged.

## Self-check answers

**If `OrderRow` gains two new properties next sprint, what changes on the screen with generated columns, and what
changes with your explicit columns?**
With `AutoGenerateColumns = true` the grid rebuilds its columns from the type: two extra columns appear at the end,
every width and format is the default, the total is no longer right-aligned because the generated column has no cell
style, and the layout the designer chose is gone — a model change silently redesigned a screen. With the explicit
columns here, nothing changes at all: the six columns bind by `DataPropertyName`, so the new properties are simply not
shown until someone decides where they belong. That is the point of turning generation off once the screen is stable.

**Your `CellValueNeeded` handler works with 200 orders. What happens to the database when a user drags the scrollbar
through 200 000, and which part of your design absorbs that?**
`CellValueNeeded` fires once per visible **cell**, so a naive handler issues six queries per row and hundreds per
screen; dragging the scrollbar through a large set turns into thousands of round trips and the database, not the grid,
falls over. `OrderCache` absorbs it: the handler is one line (`_orderCache.GetValue(e.RowIndex, e.ColumnIndex)`), a hit
never leaves memory, and a miss fetches a whole 200-row page. `DataRead` gets the block ahead of the cells, so the page
is usually already there. What gives first at ten times the volume is not the grid or the cache but the query itself —
`Count` and `GetPage` must become an indexed `WHERE` with `OFFSET/FETCH` instead of a walk over everything.

**Why does the status badge belong in `CellFormatting` rather than in the `OrderRow` model, and what would go wrong if
a customer name were placed in that HTML without encoding?**
Because the badge is display, not data. Storing markup in `OrderRow.Status` would break sorting, filtering and any
comparison (`Status == "On hold"` stops matching), would follow the row into exports and into other screens, and would
tie the model to one theme's colours. In `CellFormatting` the markup exists only for the moment the cell is rendered,
and the model still holds "On hold". Without encoding, any `<` in text a human typed becomes markup: a customer called
`Bergström <Nordic> AB` would lose part of its name, and a value like `<script>…</script>` — see the **Inject unsafe
status** button — would be script the browser runs, in every session that looks at the row. `WebUtility.HtmlEncode` is
what makes `AllowHtml` safe.

**Which control in this module is doing the most important work, and why?**
`DataGridView` itself — but the decisive work is done by three of its features rather than by the control being on the
screen: `AutoGenerateColumns = false` (the screen is designed), `AllowHtml` + `CellFormatting` (rich display without a
control per cell) and `VirtualMode` + `CellValueNeeded` + `DataRead` (rows on demand). `BindingSource` is the quiet
second: it gives the bound path a stable data source and currency without the screen knowing where the rows came from.

**Which part of the implementation belongs in a reusable UserControl or service?**
The service half already is one: `OrderService` (rules and data) and `OrderCache` (the paging strategy) are reusable as
they stand — `OrderCache` only needs the service and a page size. The filter strip is the candidate for a UserControl
(`OrderFilterStrip` exposing `Filter` and an `Applied` event); the grid itself is deliberately **not** wrapped, because
a wrapper hides the `DataGridView` API the lab tells you to keep reachable.

## Known simplifications

- The lab video scrolls 200 000 orders; this sample generates **4 000** deterministic rows, which is enough for the
  cache to show hits, misses, page fetches and eviction while keeping the project instant to start.
- The bound path is capped at 250 rows on purpose (the strip says so) — binding a six-figure result is the pitfall the
  module is about.
- The due-date column is read-only on the virtual path: the cache is a read model, so writes go through the command
  row and the cache is then dropped and re-read.
- Wisej.NET 4.1 has no `CellContentClick`; the command column is wired to `CellClick` ("fired when any part of a cell
  is clicked") and the handler keeps the lab's name.
- The loading state uses `await Task.Delay(400)` to stand in for a real query, so `ShowLoader` is actually visible.
