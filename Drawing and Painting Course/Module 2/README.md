# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 2

Local lab build for **Module 2 · Mastering the Control Paint Event**. The inline paint handler from
Module 1 is retired in favour of a reusable `TelemetryGauge` control: ring, threshold zones, needle,
numeric value and caption, with all of its coordinate maths in a pure `GaugeGeometry` helper.
The three remaining Module 1 surfaces - `Label` + `ProgressBar`, a `Wisej.Web.Canvas` and an
off-screen `Bitmap` in a `PictureBox` - stay on the operations page under the gauges, so the same
reading can still be compared across surfaces. Surface 2, the inline `Paint` handler, is now the
`TelemetryGauge` control itself.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Drawing and Painting Course/Module 2/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6002
```

Open <http://localhost:6002>. In Visual Studio, open `VisualOperationsStudio.slnx` and press F5.

## What to try

| Action | Expected result |
|---|---|
| Press **Take reading** | The spindle gauge moves; the other two setters compare and return early, so the status line reports `Repaints this request: 1`. |
| Press it until the scripted 132 % arrives | The value is clamped to 100 %, the status line names both numbers, and the gauge turns red because 100 ≥ `CriticalThreshold`. |
| Watch the panel on the right | The same three numbers as text. Each gauge also publishes its reading through `AccessibleDescription`. |
| Narrow the browser until a gauge is under 60 px wide | `GaugeGeometry.Measure` returns an empty layout and nothing is drawn — no exception, no half-drawn ring. |
| Resize the browser | All three repaint, because every coordinate a painted control uses comes from its own size. |
| In `VisualOperationsPage.ConfigureGauge`, set `gauge.Maximum` below `gauge.Minimum` | The range degrades to an empty grey track instead of throwing. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `TelemetryGauge` with clamping, early-returning setters that invalidate once | [Controls/TelemetryGauge.cs](VisualOperationsStudio/Controls/TelemetryGauge.cs) |
| Pure geometry: ring bounds, sweep angles, needle, label rectangles | [Geometry/GaugeGeometry.cs](VisualOperationsStudio/Geometry/GaugeGeometry.cs) |
| Arcs, zones, needle and text drawn from the measured layout | `PaintGauge` / `DrawNeedle` / `DrawText` in [Controls/TelemetryGauge.cs](VisualOperationsStudio/Controls/TelemetryGauge.cs) |
| Disposal, borrowed objects, save/restore around the transform | the `using` blocks and `BeginContainer`/`EndContainer` in the same file |
| Edge cases: zero size, inverted range, value out of range, missing font | `GaugeGeometry.Measure`, `GaugeGeometry.Normalize`, `Clamp`, `ResolveFont` |
| Three gauges over one model plus the same values as text | [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) |
| `PaintCost.md` | [docs/PaintCost.md](VisualOperationsStudio/docs/PaintCost.md) |

## Notes for anyone extending the gauge

- A custom painted control must **subscribe to `Paint`**. Overriding `OnPaint` alone is not enough in
  Wisej.NET 4.1 — the event is never raised — and a bare `Wisej.Web.Control` never paints at all, which
  is why `TelemetryGauge` derives from `Panel`.
- It must also `Invalidate()` on `Resize`, or a docked control that receives its size after construction
  never produces a first picture.
- `Managed.System.Drawing` has no `Graphics.Save`/`Restore`; `BeginContainer`/`EndContainer` is the pair,
  and `Pen` has `EndCap` but no `StartCap`.
