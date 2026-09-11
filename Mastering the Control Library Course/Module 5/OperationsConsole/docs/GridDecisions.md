# Grid decisions — Orders (Module 5 · DataGridView Mastery)

The lab's last step asks for "a short note explaining your column, editor and large-data decisions". This is that
note, for `Sections/DataGridViewPage` in the Operations Console.

## 1. Column decisions

`AutoGenerateColumns` is **false** and stays false. Generated columns follow `OrderRow`; the six columns below follow
the user's task.

| Column | Type | Binds to | Width | Alignment / format | Read-only | Why |
|---|---|---|---|---|---|---|
| `colNumber` | `DataGridViewTextBoxColumn` | `Number` | 120 | left | yes | The stable ID the command column and `ShellStatus.Record(...)` use. |
| `colCustomer` | `DataGridViewTextBoxColumn` | `Customer` | 280, `AutoSizeMode = Fill`, min 160 | left | yes | The only column that absorbs spare width. Plain text — never HTML. |
| `colDueDate` | `DataGridViewDateTimePickerColumn` | `DueDate` | 140 | `Format = "d"` | **no** | The one editable value, edited by a `MonthCalendar` assigned to `Editor`. |
| `colTotal` | `DataGridViewTextBoxColumn` | `Total` | 120 | `MiddleRight`, `"C2"` | yes | Money reads right-aligned; the format lives in the cell style. |
| `colStatus` | `DataGridViewTextBoxColumn`, `AllowHtml = true` | `Status` | 150 | left | yes | The badge cell. The markup is built in `CellFormatting`; the bound value stays plain text. |
| `colOpen` | `DataGridViewButtonColumn` | – | 90 | – | – | The command cell, rendered by the grid — not a `Button` control per row. |

**Why a badge and not an embedded control.** Cell HTML costs one string per visible cell; a control in every cell
would be one server control and one browser widget per row. `ordersGrid_CellFormatting` builds `<span …>` with
`WebUtility.HtmlEncode(status)` and sets `e.FormattingApplied = true`; nothing is written back to `OrderRow`.

**Why encoding is not optional.** The generated customers include `Novak & Sons` and `Bergström <Nordic> AB` — human
text really does contain `&` and `<`. With `AllowHtml` on a column, whatever you interpolate is markup.

## 2. Editor decisions

```csharp
colDueDate.Editor = dueDateCalendar;   // any control can be a column editor
```

A `MonthCalendar` raises no change event the grid listens to, so the value is moved by hand:

- `ordersGrid_CellBeginEdit` remembers the order number and the stored due date, and copies it into the calendar.
- `ordersGrid_CellEndEdit` reads `dueDateCalendar.SelectionStart` back and hands it to `ApplyDueDate`.

`OrderService.UpdateDueDate` owns the rules (no past date, not on a cancelled order, not more than a year out) and
answers with an `OrderUpdateResult`, not an exception: a rejected edit puts the stored value back into the cell,
turns the status red and shows a Toast.

**Command column.** Wisej.NET 4.1 has no `CellContentClick`; it raises `CellClick`. The handler keeps the lab's
name, `ordersGrid_CellContentClick`, checks `e.ColumnIndex == colOpen.Index`, resolves the row's ID and calls
`OrderService.GetOrder(id)`.

## 3. Large-data decisions

| Decision | What was chosen | Why |
|---|---|---|
| Default path | **Bound**, capped at 250 rows | `GetOrders` returns the first 250 matches plus the real match count, and the status says so. Rows are never hidden silently. |
| Large path | `VirtualMode = true`, `RowCount` from `OrderService.Count(filter)` | Nothing is created for rows nobody looks at. |
| Values | `ordersGrid_CellValueNeeded` → `OrderCache.GetValue(rowIndex, columnIndex)` | A service call here would run once per visible cell. |
| Cache | `OrderCache`, 200-row pages, up to 4 pages | A miss fetches a whole page, never a cell. |
| Prefetch | `ordersGrid_DataRead` → `OrderCache.Prefetch(e.FirstIndex, e.LastIndex)` | The page is in memory before the first `CellValueNeeded` of that block. |
| Invalidation | `OrderCache.Reset(filter)` on every filter change, mode change and write | A page only means anything with the filter it was fetched for. |
| Editing | The due-date column is read-only on the virtual path | The cache is a read model. |
| Failure | `OrderCache` never throws | It records `LastError`, answers `null`, and the screen says one friendly sentence. |

**What breaks first at ten times the data.** Not the grid and not the cache — `Count` and `GetPage` walk the whole
in-memory list to apply the filter, so the *query* gives first. In production that is an indexed `WHERE` plus
`OFFSET/FETCH`; the cache in front of it does not change.

## 4. Composition decisions

The grid is a plain child of `pnlGridCard`, not wrapped in a UserControl. Three children, docked:

```csharp
pnlFilterStrip.SendToBack();    // docked first  → the Top strip spans the card
pnlGridStatus.SendToBack();     // docked next   → the Bottom strip spans the card
ordersGrid.BringToFront();      // docked last   → Fill takes the space between them
```

The filter strip carries the search box, the status `ComboBox`, **Apply**, the **Virtual mode** toggle and the
**Simulate service failure** switch; the status strip carries row count, selected order, page fetches and last refresh.

## Evidence — what the running app shows

| Path | Trigger | What you see |
|---|---|---|
| Explicit columns | open the section | Six columns with the widths, alignment and formats above |
| Loading | **Apply** / **Refresh** | The grid shows its loader and Apply greys out, then the result arrives |
| Bound cap | open with no filter | Amber status "Showing the first 250 of 4,000 matching orders — narrow the filter, or tick Virtual mode …" |
| HTML status cell | any load | The Status column shows coloured pills |
| Custom editor | double-click a due date | A `MonthCalendar` opens; picking a day and leaving the cell saves it through the service |
| Rejected edit | pick a day in the past | Red status "A due date in the past cannot be saved …", warning Toast, the cell keeps the stored date |
| Empty result | search `zzzz` → **Apply** | The grid's "No orders match this filter." card, amber status |
| Virtual mode | tick **Virtual mode** | `Rows: 4,000`; the status strip counts page fetches |
| One fetch per page | drag the scrollbar | The page-fetch count grows by one per new 200-row block |
| Service failure | tick **Simulate service failure**, then **Apply** | Red status "The orders service is not answering …", error Toast; the grid keeps the last good result |
