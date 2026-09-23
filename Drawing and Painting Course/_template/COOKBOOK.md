# VisualOperationsStudio cookbook — Wisej-4 4.1.0 / .NET 10

Everything in this file was verified while building the seven modules. It is the file to read before
changing any painted surface in this course.

## Structure

Each module is standalone: its own solution, its own port, designer/code-behind pairs, and the folders
`Models`, `Geometry`, `Controls`, `Renderers`, `Diagnostics` and `docs`. Do not link runtime code to a
sibling module. Keep both target frameworks and pass `-f` on the CLI. Ports **6001–6007**; the template
is **6000**. The model belongs to the page instance, never to a static field.

## Painting a Wisej.NET control: four rules that cost time to learn

1. **Subscribe to `Paint`; overriding `OnPaint` is not enough.** Wisej.NET raises the event only when
   it has a subscriber, so an override alone never runs. `TelemetryGauge` subscribes in its
   constructor: `Paint += (sender, e) => PaintGauge(e);`
2. **Derive from `Panel`, not `Control`.** A bare `Wisej.Web.Control` never paints at all — it has no
   paintable client widget of its own.
3. **`Invalidate()` on `Resize`.** Every coordinate a painted control uses comes from its own size, and
   a docked control that receives its size after construction otherwise never produces a first picture.
4. **Keep painted controls in a layout that resizes.** A gauge inside a fixed-height band never gets
   that resize. Module 3 uses a `SplitContainer` docked `Fill` for exactly this reason; a fixed
   `Dock = Top` band left all three gauges blank.

Painted surfaces also **arrive after the page does** — a large grid's cells and the gauges fill in over
the first seconds, and a screenshot taken too early shows an empty page. That is not a bug to chase.

## `Managed.System.Drawing` is not `System.Drawing.Common`

The API is close but not identical. What is missing or different, verified against
`System.Drawing.Managed.dll` 4.1.0:

| You expect | Use instead |
|---|---|
| `Graphics.Save()` / `Graphics.Restore(state)` | **`BeginContainer()` / `EndContainer(state)`** — put the restore in a `finally` |
| `Pen.StartCap` | only `Pen.EndCap` exists |
| `DrawString(text, font, brush, RectangleF, StringFormat)` | it clips glyphs when the rectangle is near the line height |
| `DrawString(text, font, brush, PointF, StringFormat)` | draws **nothing** |
| — | measure with `MeasureString` and draw with `DrawString(text, font, brush, float x, float y)` |

`Graphics` does have `BeginContainer`, `EndContainer`, `Transform`, `ResetTransform`, `SetClip`,
`MeasureString`, and the usual `Draw*`/`Fill*` family. `GraphicsPath`, `StringFormat`,
`LinearGradientBrush`, `PrivateFontCollection` and `ImageFormat` are all present.

**One implementation only.** `Wisej-4` brings `Managed.System.Drawing`, which itself depends on
`System.Drawing.Common` — but with an empty `_._` compile assembly, so nothing compiles against it.
Never add a direct `System.Drawing.Common` reference beside it. Quick check: if `Graphics.Save()`
suddenly compiles, something else is supplying the types.

## Grid cell painting

`UserPaint` is the switch and there is no default: `DataGridViewColumn.UserPaint = true` (or per cell),
then `CellPaint` fires. Keep the enabling line and the subscription in the same method — a
correct-looking handler that never runs is almost always a missing switch.

`DataGridViewCellPaintEventArgs` derives from `PaintEventArgs` and adds only `RowIndex` and
`ColumnIndex`. There is **no** `PaintBackground` / `PaintContent`: the handler owns everything in the
cell, so a Windows Forms example will not port straight across.

`CellPaint` is served by the grid's own request handler (`DataGridView.IWisejHandler.ProcessRequest`),
**on request threads** — not during the page's response. Anything it touches must be safe for concurrent
calls: share only immutable objects (a `Font` is fine), and create and dispose every `Pen` and `Brush`
inside the call. Read the row's values from `DataGridViewRow.DataBoundItem` in one step; this handler
runs again for every cell that scrolls into view.

A column with **no** `DataPropertyName` still gets cells and still raises `CellPaint` — which is how the
Trend column works, since there is no text under its sparkline.

## `Wisej.Web.Canvas`

- `Redraw` fires on **resize**, and is not guaranteed before the page is first shown. Draw the first
  scene from the page's `Load`, where the canvas already reports its laid-out size, and let `Redraw`
  handle every later rebuild. Both call the same method.
- Angles are **degrees**: `Rotate(45)` is the browser's `rotate(Math.PI / 4)`, and a full circle is
  `Arc(x, y, r, 0f, 360f, false)`.
- `FillStyle` / `StrokeStyle` take a `System.Drawing.Color` or a gradient object, not a CSS string;
  `TextFont` takes a `Font`, not the CSS `font` shorthand.
- There is **no** `measureText`. Place text with `TextAlign` and `TextBaseline`, or measure off-screen
  with `Graphics.MeasureString`.
- Colour stops are objects with a `stop` and a `color`, passed in the call:
  ```csharp
  c.CreateLinearGradient(x0, y0, x1, y1, new object[]
  {
      new { stop = 0f, color = "#1f9d6b" },
      new { stop = 1f, color = "#d93a3a" },
  });
  ```
  An array of pairs compiles and silently fills near-black instead.
- `LiveUpdate` stays **false** for a scene render (one update for the whole scene) and is switched on
  only to let a long server-side loop reveal its progress.
- `MouseDown` / `MouseMove` / `MouseUp` / `MouseWheel` / `KeyDown` come from `Control`; set
  `Focusable` and `TabStop` for the keyboard path. Each handler should change state and call the one
  render method — nothing is drawn inside a handler.

## Layout notes

- Docking is applied from the **last** `Controls.Add` to the first: add the `Fill` control first and the
  `Top`/`Bottom` bars last.
- `Label.TextAlign` is a `System.Drawing.ContentAlignment`, not `Wisej.Web.HorizontalAlignment`.
- `TableLayoutPanel` exists, but the three-gauge row is built from `Dock = Left/Left/Fill` plus a
  `Resize` handler, which proved more predictable for painted children.

## Evidence

All seven modules compile for `net10.0-windows` and `net10.0`, and every module was opened in a browser
and exercised: four surfaces and the resize (1), three gauges and the repaint count (2), 1,000 painted
rows (3), the Canvas playground (4), select/drag/pan/zoom (5), the PNG export (6), and the capstone with
its metrics, degraded checks and 22 passing geometry tests (7). No load, deployment, screen-reader or
cross-browser certification is claimed, and the Linux container run was not executed on the build
machine — `Module 6/VisualOperationsStudio/docs/CrossPlatform.md` says so and gives the command.
