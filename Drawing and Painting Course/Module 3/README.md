# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 3

Local lab build for **Module 3 · DataGridView Cell Painting**. The operations grid arrives: 1,000
generated `MachineStatus` rows, with `UserPaint` on the Health and Trend columns only and one guarded
`CellPaint` handler behind both. A third column renders the same severity through `AllowHtml`, so the
two approaches can be compared on the same scroll.
The screen is the grid alone: `Machine`, `Site`, the painted `Health` and `Trend` cells, the
`Status · HTML` chip and the last reading, over a footer that counts the bound rows.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Drawing and Painting Course/Module 3/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6003
```

Open <http://localhost:6003>. In Visual Studio, open `VisualOperationsStudio.slnx` and press F5.
Painted surfaces fill in over the first seconds — the gauges last, because they are the largest images.

## What to try

| Action | Expected result |
|---|---|
| Look at a healthy row (`Press 001`, 82 %) | A full green bar with its number, and a nearly flat sparkline. |
| Look at a row past the warning threshold (`Press 004`, 40 %) | The bar switches to amber and keeps its number readable. Below 35 % it turns red. |
| Find a row whose Trend cell is empty (every twelfth row has one reading) | Nothing is drawn — fewer than two points is not a line — and nothing throws. |
| Drag the scrollbar from top to bottom | Every newly visible cell calls the handler again. That is why there is no lookup, allocation or file access in it. |
| Click a row | Both painted cells redraw with the selected row's colours. |
| Compare the two status columns | `Status (AllowHtml)` says the same thing in words a screen reader can read. `Lab3Notes.md` records which one this sample would ship. |
| Comment out `this.colHealth.UserPaint = true;` | `CellPaint` stops firing for Health; the cell falls back to ordinary content; Trend keeps painting. |
| Press **Take reading** | The gauges from Module 2 still behave: one value moves, one repaint. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `MachineStatus` plus a generator for any number of rows | [Models/MachineStatus.cs](VisualOperationsStudio/Models/MachineStatus.cs) |
| Explicit columns, `UserPaint` on two of them, one subscription | `ConfigureUserPaintedColumns` in [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) and [VisualOperationsPage.Designer.cs](VisualOperationsStudio/VisualOperationsPage.Designer.cs) |
| The guarded routing handler | `gridOperations_CellPaint` in [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) |
| `DrawHealthBar` and `DrawSparkline` as pure helpers | [Controls/OperationsCellRenderer.cs](VisualOperationsStudio/Controls/OperationsCellRenderer.cs) |
| 1,000 bound rows and the `AllowHtml` comparison column | `MachineStatusGenerator.Generate(1000)`, `colStatusHtml` |
| `Lab3Notes.md` | [docs/Lab3Notes.md](VisualOperationsStudio/docs/Lab3Notes.md) |
