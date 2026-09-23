# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 7

Local lab build for **Module 7 · Production Drawing Architecture and Capstone**. One `OperationsModel`
sits behind all four surfaces — the painted gauges, the painted grid cells, the Canvas topology and the
PNG export — with a shared geometry layer, a `RenderMetrics` recorder, one measured optimisation, four
degraded paths that are actually executed, an accessible table of every value the graphics show, and a
test project for the geometry.
This folder is the complete application at the end of the course.

## Run it

```bash
cd "Drawing and Painting Course/Module 7/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6007
```

```bash
cd "Drawing and Painting Course/Module 7"
dotnet test VisualOperationsStudio.Tests/VisualOperationsStudio.Tests.csproj
```

Open <http://localhost:6007>. The tests need no Wisej session, licence or browser: the geometry layer is
ordinary arithmetic, which is the point of keeping it separate.

## The demo script

| Step | What to look for |
|---|---|
| **Take reading** | The spindle gauge moves and the values table follows it. One changed reading is one repaint. |
| Resize the browser | Gauges and cells repaint from the server; the Canvas goes blank and `Redraw` rebuilds it. |
| Scroll the grid | Every newly visible painted cell calls the one guarded `CellPaint` handler. |
| **Plant topology** → drag and zoom a node | The same model, a different surface. |
| **Export PNG** | The bytes are rendered from the model, never screenshotted from the Canvas. |
| Back, then **Render metrics** | Median times per surface, and the last few individual renders with their sizes and object counts. |
| **Run degraded checks** | Empty data, zero-sized surface, inverted range and missing font, each one executed and reported. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `Models`, `Geometry`, `Renderers` and the Wisej adapter folders | the project layout |
| One `OperationsModel` behind all four surfaces, held per session | [Models/OperationsModel.cs](VisualOperationsStudio/Models/OperationsModel.cs) |
| Coordinate maths lifted into the geometry layer | [Geometry/GaugeGeometry.cs](VisualOperationsStudio/Geometry/GaugeGeometry.cs), [Geometry/ViewportGeometry.cs](VisualOperationsStudio/Geometry/ViewportGeometry.cs) |
| Unit tests: value→angle, inverted range, hit testing, culling | [VisualOperationsStudio.Tests/GeometryTests.cs](VisualOperationsStudio.Tests/GeometryTests.cs) — 22 tests |
| `RenderMetrics.Record`, called from the gauge adapter and the exporter | [Diagnostics/RenderMetrics.cs](VisualOperationsStudio/Diagnostics/RenderMetrics.cs), `TelemetryGauge.Rendered`, `btnExport_Click` |
| One optimisation, measured before and after | the export size, in [docs/CapstoneNotes.md](VisualOperationsStudio/docs/CapstoneNotes.md) |
| The four degraded paths, executed | [Diagnostics/DegradedStateCheck.cs](VisualOperationsStudio/Diagnostics/DegradedStateCheck.cs) |
| The accessible table and the word beside every colour | `PublishReadings`, `MachineStatus.SeverityHtml` |
| `CapstoneNotes.md` | [docs/CapstoneNotes.md](VisualOperationsStudio/docs/CapstoneNotes.md) |
