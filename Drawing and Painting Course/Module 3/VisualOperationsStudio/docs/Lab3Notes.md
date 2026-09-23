# Lab3Notes — painted cells vs. `AllowHtml`, on a 1,000-row grid

Lab 3 deliverable. Measured on this project at `http://localhost:6003`, Wisej-4 4.1.0, `net10.0`
(Kestrel), Bootstrap-4 theme, `gridOperations` bound to 1,000 generated `MachineStatus` rows.

## The event members this lab uses

| Member | Where | What it is for |
|---|---|---|
| `DataGridViewColumn.UserPaint` | `ConfigureUserPaintedColumns` | The switch. Nothing in a Wisej.NET grid is user-painted by default; without it the grid never raises `CellPaint` for that surface. Set on the Health and Trend columns only. |
| `DataGridView.CellPaint` | subscribed once on the grid | One subscription for the whole screen instead of one per row. |
| `DataGridViewCellPaintEventArgs.RowIndex` | first guard | Negative means a header, not a data row. |
| `DataGridViewCellPaintEventArgs.ColumnIndex` | second guard | Anything that is not one of the two target columns returns immediately. |
| `PaintEventArgs.Graphics` | inherited | The surface. It belongs to Wisej.NET and is never disposed here. |
| `PaintEventArgs.ClipRectangle` | inherited | The cell's own rectangle, origin `0,0`, e.g. `{0,0,199,31}` for a Health cell. |
| `DataGridViewRow.DataBoundItem` | in the handler | The bound `MachineStatus` in one O(1) step — no searching a list while the grid scrolls. |
| `DataGridViewRow.Selected` | in the handler | Selected rows get their own track and line colours, or a painted cell looks broken the moment somebody clicks it. |

`DataGridViewCellPaintEventArgs` in Wisej.NET derives from `PaintEventArgs` and adds only `RowIndex`
and `ColumnIndex`. There is **no** `PaintBackground` or `PaintContent` helper: the handler owns
everything that appears in the cell. A Windows Forms example copied straight across will not work.

## Verified while building this

- `CellPaint` is served by the grid's own request handler
  (`DataGridView.IWisejHandler.ProcessRequest`), not during the page's response. It runs on request
  threads, so anything it touches must be safe for concurrent calls — this is why the one shared object
  in `OperationsCellRenderer` is a `Font` (immutable) and every `Pen` and `Brush` is created and disposed
  inside the call.
- Painted surfaces arrive after the page does: a large grid's cells and the gauges fill in over the first
  seconds. Painted controls must also live in a layout that resizes with the browser — a gauge inside a
  fixed-height band never receives the resize that produces its first picture, which is why the page uses
  a `SplitContainer` rather than a fixed top band.
- A column with no `DataPropertyName` still gets cells, and still raises `CellPaint` — which is what the
  Trend column relies on, since there is no text under its sparkline.

## Painted columns vs. the `AllowHtml` column

The grid carries both, side by side, on the same 1,000 rows: `Health (painted)` and `Trend (painted)`
against `Status (AllowHtml)`, which renders the same severity as a coloured markup chip.

| | Painted cell | `AllowHtml` cell |
|---|---|---|
| What it can express | any geometry — a proportional bar, a polyline over twelve readings | text, colour, an icon, a badge |
| Cost per visible cell | a server render and an image | a string already in the row's data |
| Cost on a 1,000-row scroll | one handler call per painted cell that scrolls into view | none beyond the markup already sent |
| Selection | must be handled by hand (`row.Selected`) | follows the theme |
| Accessibility | pixels; the value has to be published separately | real text a screen reader can read |
| Searchable / selectable text | no | yes |

**Which one would I ship?** The `AllowHtml` status chip, for the severity. It says `Warning 40%` in
words and colour, it is readable by assistive technology, it costs nothing per scroll, and it follows
the theme. Painting earns its place only in the Trend column, where the information *is* the shape:
twelve readings as a line cannot be expressed in markup without shipping a chart per row.

The Health bar sits in between, and is kept here as the honest comparison: it is a proportional bar,
which markup could approximate with a styled `div`. It draws its number inside the cell for exactly
that reason — a painted cell showing only a colour has lost the value.

A third option exists and beats both when the user has to *interact* with the cell rather than only
read it: a real cell type with an editor.

## Turning the switch off

Remove `this.colHealth.UserPaint = true;` from `ConfigureUserPaintedColumns` and run again: `CellPaint`
stops firing for Health (the guard on `e.ColumnIndex` never matches it), the cell falls back to ordinary
content, and Trend keeps painting. Forgetting that one line is the commonest reason a correct-looking
handler never runs, which is why the switch and the subscription live in the same method.
