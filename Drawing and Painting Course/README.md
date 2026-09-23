# Drawing & Painting in Wisej.NET · lab samples

Seven runnable Wisej.NET 4 applications following the course specifications, lesson guides, lab steps
and walkthroughs. Each module is a cumulative snapshot of **VisualOperationsStudio**, a plant
operations screen that renders the same telemetry on four different surfaces. The final module is the
capstone.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · The Wisej.NET Drawing Architecture | `Module 1` | One `TelemetrySample` on four surfaces at once: a Label and a ProgressBar, a `Panel.Paint` handler, a `Wisej.Web.Canvas` driven by `Redraw`, and an off-screen `Bitmap` from `Graphics.FromImage`. | `http://localhost:6001` |
| 2 · Mastering the Control Paint Event | `Module 2` | A reusable `TelemetryGauge` — ring, threshold zones, needle, value, caption — with clamping setters that repaint once, and all the coordinate maths in a pure `GaugeGeometry`. | `http://localhost:6002` |
| 3 · DataGridView Cell Painting | `Module 3` | An operations grid of 1,000 machines with `UserPaint` on the Health and Trend columns, one guarded `CellPaint` handler, and an `AllowHtml` status column beside them for comparison. | `http://localhost:6003` |
| 4 · Canvas and the HTML5 2D Model | `Module 4` | A Canvas playground: paths, fills, a placed caption, linear and radial gradients, four transform blocks inside `Save`/`Restore`, and a clip, alpha, dash and shadow section. | `http://localhost:6004` |
| 5 · Interactive Canvas and Hit Testing | `Module 5` | The playground becomes a topology editor: a per-session `TopologyScene`, world↔screen conversions, hit testing, drag, pan, zoom about the pointer, culling and a keyboard path. | `http://localhost:6005` |
| 6 · System.Drawing.Managed and Cross-Platform Rendering | `Module 6` | A `TopologyImageRenderer` that turns the same model into PNG bytes with no control and no session, with a font fallback chain and a bounded output size. | `http://localhost:6006` |
| 7 · Production Drawing Architecture and Capstone | `Module 7` | One `OperationsModel` behind all four surfaces, a shared geometry layer, `RenderMetrics`, one measured optimisation, four executed degraded paths, an accessible values table and 22 geometry tests. | `http://localhost:6007` |

## Run a module

Requirements: .NET 10 SDK, NuGet access to `Wisej-4` 4.1.0 and a Wisej development license or trial.
Open a module's `VisualOperationsStudio.slnx` in Visual Studio and press F5, or run:

```bash
cd "Drawing and Painting Course/Module 7/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6007
```

Each project targets `net10.0-windows` and `net10.0`, so the CLI needs `-f`. Ports 6001–6007 let the
snapshots run side by side. Nothing requires a database, a third-party service or a deployment.

The capstone's geometry tests run from `Module 7` and need no Wisej session, licence or browser:

```bash
cd "Drawing and Painting Course/Module 7"
dotnet test VisualOperationsStudio.Tests/VisualOperationsStudio.Tests.csproj
```

## Conventions

- Designer-style `.Designer.cs` files hold the named controls; short handlers and helpers live in the
  code-behind, in the same folders the labs name: `Models`, `Geometry`, `Controls`, `Renderers`,
  `Diagnostics`.
- Every surface reads the model. No renderer keeps a copy of a value, and from Module 7 there is exactly
  one `OperationsModel` per session behind all four.
- The coordinate maths lives in `Geometry` and references no `Graphics`, no control and no session,
  which is what makes it testable.
- Painted surfaces fill in over the first seconds after a page opens; that is how Wisej.NET delivers
  them, not a fault in the sample.
- Every session owns its own model. Nothing is persisted.

## Lab deliverables

Each module's written deliverable is in its `docs` folder, and each one records what was actually
measured rather than what was expected:

| Module | Document |
|---|---|
| 1 | `SurfaceNotes.md` — what crosses the network per surface, and what survives a resize |
| 2 | `PaintCost.md` — painted surface size, objects per repaint, repaint frequency, when a themed control wins |
| 3 | `Lab3Notes.md` — the event members, and painted cells vs. `AllowHtml` on 1,000 rows |
| 4 | `Lab4Notes.md` — every Canvas member and the browser member it replaced, including the one that does not exist |
| 5 | `InteractionNotes.md` — the request cost of a server-side drag, and where the line is |
| 6 | `CrossPlatform.md` — the reference graph, the platform matrix, the measured export, the font chain |
| 7 | `CapstoneNotes.md` — the layer diagram, the boundary, the before/after measurement, the test matrix |

## `_template`

The working Module 1 baseline on port 6000, plus **`COOKBOOK.md`**: the framework behaviour, the
`Managed.System.Drawing` API differences and the painting gotchas found while building this course.

## Verification

All seven module solutions and the template compile for both target frameworks. Every module was opened
in a browser and exercised — the four surfaces and the resize, the gauges and their repaint count, 1,000
painted rows, the Canvas playground, select/drag/pan/zoom, the PNG export, and the capstone's metrics
and degraded checks. The capstone's 22 geometry tests pass. The Linux container run described in
Module 6 was **not** executed on the build machine; `CrossPlatform.md` says so and gives the command.
No load, deployment, screen-reader or cross-browser certification is claimed.
