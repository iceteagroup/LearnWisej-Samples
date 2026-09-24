# CapstoneNotes — one model, four renderers, measured

Lab 7 deliverable. Measured on this project at `http://localhost:6007`, Wisej-4 4.1.0, `net10.0`
(Kestrel), Windows 11, .NET 10.0.303, Chromium, browser viewport 1400 × 950.

## The architecture

```
Models/                  OperationsModel  ── the semantic state, one instance per session
                           TelemetrySample      the CPU reading the gauge paints
                           AssetStatus  x4      load, trend series, state
                           TopologyScene        nodes, edges, selection, viewport
                           RenderMetrics        what every render cost
                           SeverityOf / SeverityOfLoad     the one set of thresholds

Geometry/                pure arithmetic - no Graphics, no control, no session
                           GaugeGeometry        value -> angle, ring, needle, label rectangles
                           ViewportGeometry     world <-> screen, visible rectangle, hit test, zoom

Renderers/               output that owns no control
                           TopologyImageRenderer   scene + size -> PNG bytes

Controls/                the Wisej.NET adapters
                           TelemetryGauge          Paint  -> an image of a control
                           OperationsCellRenderer  CellPaint -> an image of a cell

Pages                    VisualOperationsPage — the capstone screen, all four surfaces at once
                           it owns no business state; it holds the model and renders it
```

One rule holds it together: **the model decides, the geometry measures, the renderer draws.** Before
this split, each surface kept its own thresholds and its own idea of where things sat, so moving the
warning line once made the exported image quietly contradict the screen it came from. That is not a
rendering bug; it is four copies of one business rule.

The four renderers deliberately do **not** share an interface. A `Paint` handler draws with
`System.Drawing` into a rectangle Wisej.NET owns; a `CellPaint` handler does the same inside one cell;
the Canvas issues browser commands; the exporter writes a `Bitmap`. Forcing those through one
abstraction helps nobody. What they share is the model and the geometry.

## Where each surface executes, and what crosses the network

| Surface | Runs | Crosses the network | Survives a resize |
|---|---|---|---|
| `TelemetryGauge.Paint` | server | a rendered image, per repaint | yes — the server repaints it |
| `DataGridView.CellPaint` | server, in its own request | one image per painted cell that scrolls into view | yes |
| `Wisej.Web.Canvas` | browser | the drawing commands, batched into one update | **no** — blank until `Redraw` rebuilds the scene from the model |
| `TopologyImageRenderer` | server, no control involved | the encoded PNG bytes, once | not applicable — the output is a file |

The export is rendered **from the model**, never by screenshotting the Canvas. That is what lets it run
from a background job, a scheduled report or a Linux container with no session at all.

## Measured (`Render metrics` button, this session)

| Surface | Output | Objects | Cold | Warm median |
|---|---|---|---|---|
| `gauge.paint` | 351 × 428 | 7 | 353–355 ms (first three renders) | **22.3 ms** over 9 renders |
| `topology.export` (Module 6: 1600 × 900) | 1600 × 900, 37 KB | 13 | 1043 ms | ≈ 504 ms |
| `topology.export` (Module 7: 1000 × 560) | 1000 × 560, 31 KB | 13 | — | **170.8 ms** over 3 renders |

**The optimisation: a smaller export bitmap.** 1600 × 900 → 1000 × 560 is 3.1× fewer pixels, and PNG
encoding was almost all of the old figure: **504 ms → 171 ms**, with the file dropping from 37 KB to
31 KB. The picture is the same seven nodes and stays legible at the smaller size, because the renderer
fits the scene to the page rather than rendering at a fixed scale. If a larger image is ever needed, the
size is an argument — the clamp is 200–2000 per side, because a 4000 × 3000 bitmap costs about 48 MB
before a single shape is drawn.

**A measured decision not to optimise: the gauge.** Warm, it paints in 22 ms at 351 × 428 with seven
drawn parts, which is above the ~5 ms a painted control should aim for, but it only repaints when a
value actually changes — the setters clamp, compare and return early, so one changed reading is one
repaint, not three. At the frequency this screen updates, 22 ms of server time per changed reading buys
nothing worth the complexity of caching a palette or skipping the label. The first three renders cost
~354 ms each; that is JIT and first-use cost inside the drawing stack, not the drawing itself, and it
does not recur. Both figures are written down here so the next person can disagree with evidence.

## Degraded paths (the **Run degraded checks** button runs all four)

| Case | Result |
|---|---|
| Empty data | `Empty data: a readable 14 KB image saying "No nodes to display", not a blank file.` |
| Zero-sized surface | `Zero-sized surface: the layout comes back empty, so the handler returns before it creates a single drawing object.` |
| Invalid range | `Inverted range (minimum 100, maximum 0): normalises to 0, which draws an empty track rather than throwing.` |
| Missing font | `Missing font family: resolved to Microsoft Sans Serif (runtime fallback) - the chain ends in a family the runtime always has.` |

They are exercised as code rather than described in a comment, because "it degrades gracefully" is a
claim, not a fact, until something runs it. A failed export is caught in the page and reaches the user
as a message in the status line, not as a blank space where a picture was expected.

## Accessibility

- **Every value the graphics show** is listed in an ordinary table beside the gauges: the three
  readings with their units, how many machines are normal / warning / critical, how many topology nodes
  there are, and what is selected.
- Each gauge sets `AccessibleDescription` from its `Value`, `Caption` and `Unit` setters, so the reading
  is available as a sentence, not only as pixels.
- The painted Health cell draws its number inside the bar, and the `Status (AllowHtml)` column carries a
  **word** as well as a colour (`Warning 40%`), so severity never depends on colour vision alone.
- The topology editor has a node list beside the surface, `Tab` moves to the next node, and the arrow
  keys nudge the selected one. The status line names the selection in text.

## Test matrix

| | Covered | How |
|---|---|---|
| Geometry: value→angle, clamping, inverted range, empty layout | yes | 22 xUnit tests, `dotnet test`, no Wisej session or licence needed |
| Geometry: world↔screen round trip, hit test, culling, zoom clamp | yes | same suite |
| Windows, .NET 10, `-f net10.0` | yes | every figure in this document |
| Windows, `net10.0-windows` | compiles | designer target only; not the deployed one |
| Linux container | **not run here** | Docker Desktop's Linux engine was not running on this machine. The command is in Module 6's `CrossPlatform.md`; the renderer has no `Wisej.Web` dependency and takes its types from `System.Drawing.Managed.dll`, which is the part that decides the outcome. |
| iOS / Android / macOS Hybrid | reasoned, not run | managed rendering travels; host resources do not. Ship the font, do not borrow it. |
| Chromium | yes | all browser verification in this course |
| Firefox, Safari | not run | no cross-browser certification is claimed |
| Load / concurrency | not run | the per-session model is the design answer; it has not been load-tested here |

## One rejected alternative per surface

| Surface | Rejected | Why |
|---|---|---|
| Gauge | A themed `ProgressBar` with a coloured bar | It cannot express a ring with threshold zones and a needle. Module 1's surface 1 is the honest comparison, and it is still the right default when a number is all the meaning there is. |
| Grid cells | One gauge control per row | A thousand rows would be a thousand server-side controls alive in one session. The `AllowHtml` column stays beside the painted ones precisely because markup wins for text, icons and accessibility. |
| Topology | A client-side JavaScript canvas library | It would move the scene, the hit testing and the selection into the browser, and the server would stop being the place the plant is defined. Worth it when the gesture is long, the render is expensive or the link is slow — `InteractionNotes.md` states the threshold. |
| Export | Screenshotting the Canvas | The browser's bitmap is not something the server can depend on, and an export must run without a session at all. Rendering the model again is both more reliable and more portable. |

## The final design checklist

- [x] The visual rebuilds from model state — `Redraw` calls the same `RenderScene` a resize needs.
- [x] The surface suits the update frequency — painted for geometry, markup for text, Canvas for the
      interactive scene, off-screen for the file.
- [x] Critical information exists semantically — the values table, `AccessibleDescription`, the word
      beside every severity colour.
- [x] Disposables are owned and released — every `Pen`, `Brush`, `Font`, `GraphicsPath`, `Bitmap` and
      stream this code creates is in a `using`; `e.Graphics` is never disposed.
- [x] Fonts and images are explicit — `ResolveFont` ends in a family the runtime always has and records
      which one it used.
- [x] Realistic data scale has been profiled — 1,000 rows bound, scrolled, and painted per visible cell.
- [x] High-frequency animation stays on the client — `LiveUpdate` is off for every main render, and the
      one progressive example says so in its own status line.
