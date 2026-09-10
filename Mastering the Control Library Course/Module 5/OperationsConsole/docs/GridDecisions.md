# Grid decisions — Orders (Module 5 · DataGridView Mastery)

The lab's last step asks for "a short note explaining your column, editor and large-data decisions". This is that
note, for `Sections/DataGridViewPage` in the Operations Console.

---

## 1. Column decisions

`AutoGenerateColumns` is **false** and stays false. Generated columns follow `OrderRow`; the six columns below follow
the user's task. Adding a property to the view model tomorrow moves nothing on this screen.

| Column | Type | Binds to | Width | Alignment / format | Read-only | Why |
|---|---|---|---|---|---|---|
| `colNumber` | `DataGridViewTextBoxColumn` | `Number` | 120 | left | yes | The stable ID. Never edited, always the value the command column and `ConsoleLog.Record(...)` use. |
| `colCustomer` | `DataGridViewTextBoxColumn` | `Customer` | 280, `AutoSizeMode = Fill`, min 160 | left | yes | The only column allowed to absorb spare width, so the screen resizes without a horizontal scrollbar. Plain text — never HTML. |
| `colDueDate` | `DataGridViewDateTimePickerColumn` | `DueDate` | 140 | `Format = "d"`, `DateTimePickerFormat.Short` | **no** | The one editable value. A typed column means the built-in editor already parses and commits a date; the `MonthCalendar` is the *optional* custom editor on top of it. |
| `colTotal` | `DataGridViewTextBoxColumn` | `Total` | 120 | `MiddleRight`, `"C2"` | yes | Money reads right-aligned; the format lives in the cell style, not in the model, so a currency change is one property. |
| `colStatus` | `DataGridViewTextBoxColumn`, `AllowHtml = true` | `Status` | 150 | left | yes | The badge cell. The markup is built in `CellFormatting`; the bound value stays the plain status text. |
| `colOpen` | `DataGridViewButtonColumn` (`UseColumnTextForButtonValue`) | – | 90 | – | – | The command cell. One button per row is the cheap end of "cell controls": a button cell is rendered by the grid, not a real `Button` control per row. |

**Why a badge and not an embedded control.** The status needs a coloured pill — icons, colour, rounded background.
Cell HTML costs one string per visible cell; a `Label` or `Panel` in every cell would be one server control *and* one
browser widget per row, which is the pitfall the module names. `ordersGrid_CellFormatting` therefore builds
`<span style="…">…</span>` with `WebUtility.HtmlEncode(status)` and sets `e.FormattingApplied = true`; nothing is
written back to `OrderRow`, and no rule is decided there. Business validation lives in `OrderService`.

**Why encoding is not optional.** The generated customers include `Novak & Sons` and `Bergström <Nordic> AB` — human
text really does contain `&` and `<`. The **Inject unsafe status** button stores
`On hold <script>alert('xss')</script>` through the service and the badge shows that as *text*: with `AllowHtml` on a
column, whatever you interpolate is markup, so the encode call is what makes the column safe.

## 2. Editor decisions

The due-date column ships with the built-in date editor of `DataGridViewDateTimePickerColumn`. The
**MonthCalendar editor** check box swaps it for the lab's custom editor:

```csharp
colDueDate.Editor = chkCalendarEditor.Checked ? dueDateCalendar : null;   // any control can be an editor
```

A `MonthCalendar` raises no change event the grid listens to, so the value is moved by hand — this is the classic
"editor that looks right but never commits":

- `ordersGrid_CellBeginEdit` remembers the order number and the **stored** due date, and copies that date into
  `dueDateCalendar.SelectionStart`.
- `ordersGrid_CellEndEdit` reads `dueDateCalendar.SelectionStart` back (or the cell value when the built-in editor is
  in use), and hands it to `ApplyDueDate`.

`ApplyDueDate` is the only place a due date is written, and the command-row buttons call the same method — so
"**Due date + 7 days**" and "**Due date last week (rejected)**" exercise exactly the code path the in-cell editor
uses. `OrderService.UpdateDueDate` owns the rules (no past date, not on a cancelled order, not more than a year out)
and answers with an `OrderUpdateResult`, not an exception: a rejected edit puts the stored value back into the cell,
turns the status red and shows a Toast the user can act on.

**Command column.** Wisej.NET 4.1 has no `CellContentClick` (WinForms' event); it raises `CellClick`, "fired when any
part of a cell is clicked". The handler keeps the lab's name, `ordersGrid_CellContentClick`, checks
`e.ColumnIndex == colOpen.Index`, resolves the row's **ID** and calls `OrderService.GetOrder(id)`. It never reads
business data out of cell text.

## 3. Large-data decisions

| Decision | What was chosen | Why |
|---|---|---|
| Default path | **Bound**, capped at 250 rows | A person works with a filtered set. `GetOrders` returns the first 250 matches plus the real match count, and the strip says "Showing the first 250 of 1 342 — narrow the filter, or tick Virtual mode." Rows are never hidden silently. |
| Large path | `VirtualMode = true`, `RowCount` from `OrderService.Count(filter)` | Nothing is created for rows nobody looks at: 4 000 rows, 0 row objects. |
| Values | `ordersGrid_CellValueNeeded` → `OrderCache.GetValue(rowIndex, columnIndex)` | One line. A service call in this handler would run once per **visible cell** — fourteen rows × six columns is eighty-four calls for one screen. |
| Cache | `OrderCache`, 200-row pages, up to 4 pages, dropped oldest-first | A miss fetches a whole page, never a cell. Four pages because the visible block usually straddles a boundary and a single-page cache would re-fetch every second row. |
| Prefetch | `ordersGrid_DataRead` → `OrderCache.Prefetch(e.FirstIndex, e.LastIndex)` | The documented hook: the client says which block it is about to read, so the page is in memory before the first `CellValueNeeded` of that block. |
| Invalidation | `OrderCache.Reset(filter)` on every filter change, mode change and write | A page only means anything together with the filter it was fetched for. A stale row is worse than one extra fetch. |
| Editing | The due-date column is read-only on the virtual path | The cache is a read model. The command-row buttons still write through the service, and the cache is then dropped so the change is re-read — visible in the Event log as a page fetch. |
| Failure | `OrderCache` never throws | It records `LastError`, answers `null` for those cells, and the screen says one friendly sentence. An exception escaping `CellValueNeeded` would put internals in front of the user. |

**What breaks first at ten times the data.** Not the grid and not the cache — `OrderService.Count` and
`OrderService.GetPage` both walk the whole in-memory list to apply the filter, so the *query* is what gives first. In
production that is the database's job (an indexed `WHERE` plus `OFFSET/FETCH`), and the cache in front of it does not
change. The second thing to give is the bound path, which is why it is capped rather than trusted.

## 4. Composition decisions

The grid is a plain child of `pnlGridCard`, not wrapped in a UserControl — the full `DataGridView` API stays visible
to the page. Three children, docked:

```csharp
pnlFilterStrip.SendToBack();    // docked first  → the Top strip spans the card
pnlGridStatus.SendToBack();     // docked next   → the Bottom strip spans the card
ordersGrid.BringToFront();      // docked last   → Fill takes the space between them
```

Docking is applied from the **last** child to the first, so the strips have to sit behind the grid; that is exactly
what `SendToBack` / `BringToFront` say. `ComposeGrid()` logs the resulting order to the Event log
(`compose pnlGridCard · dock order = pnlFilterStrip (Top) → pnlGridStatus (Bottom) → ordersGrid (Fill)`), so the
layering is verifiable and not folklore.

The filter strip carries the search box (Fill), the status `ComboBox`, **Apply**, **Clear** and the **Virtual mode**
toggle; the status strip carries row count, selected order, cache state and last refresh.

---

## Evidence — what the running app shows

| Path | Trigger | What you see |
|---|---|---|
| Explicit columns | open the section | Six columns with the widths, alignment and formats above; totals right-aligned as `€1,234.00`, dates short, no extra column ever appears |
| Loading | **Load orders** / **Apply** / **Refresh** | The grid shows its loader, the command buttons grey out, status is amber "Loading orders…", then the result arrives (~0.4 s) |
| Bound success | **Apply** with `search = Contoso` | Log `OrderService.GetOrders(search "Contoso", max 250) → 173 of 173 matching rows [fetch #1]`, green status, strip "Rows: 173 of 173 (bound)" |
| Bound cap | **Clear** the filter | Amber status "Showing the first 250 of 4 000 matching orders — narrow the filter, or tick Virtual mode.", strip "Rows: 250 of 4 000 (bound)" |
| HTML status cell | any load | The Status column shows coloured pills; the model still holds "On hold" (the search box matches on the plain text) |
| Encoding | select a row → **Inject unsafe status** | The badge reads `On hold <script>alert('xss')</script>` as visible text, nothing runs, amber status explains why |
| Custom editor | tick **MonthCalendar editor**, double-click a due date | Log `colDueDate.Editor = dueDateCalendar (MonthCalendar)`, then `CellBeginEdit SO-100123 · stored due date 2026-10-02 → MonthCalendar (custom editor)`; picking a day and leaving the cell logs `CellEndEdit … editor returned 2026-10-15` |
| Accepted edit | select a row → **Due date + 7 days** | Log `OrderService.UpdateDueDate(…) → accepted`, green status, `icon-check` Toast, the cell shows the new date |
| Rejected edit | select a row → **Due date last week (rejected)** | Log `→ rejected: due date is in the past`, red status "A due date in the past cannot be saved — pick … or later.", warning Toast, the cell keeps the stored date |
| Empty result | search `zzzz` → **Apply** | The grid's `NoDataMessage` card, amber status "No orders match search "zzzz" …", no exception |
| Virtual mode | tick **Virtual mode** | Log `virtual path · RowCount = 4000, rows created: 0`, then `DataRead rows 0–29 → 1 page fetch(es) · 1 page fetch · 0 cache hits · 1 misses · 1/4 pages in memory` |
| One fetch per page | drag the scrollbar | Each new block logs a single `OrderService.GetPage(first=…, count=200)`; scrolling back over rows already held logs `DataRead … → 0 page fetch(es)` |
| Service failure | tick **Simulate service failure**, then **Load orders** | Log `✗ the orders service did not answer — OrderServiceException`, red status "The orders service is not answering. The list on screen is unchanged …", error Toast; the grid still shows the last good result |
| Recovery | untick it, **Load orders** | The list loads again, status green |
