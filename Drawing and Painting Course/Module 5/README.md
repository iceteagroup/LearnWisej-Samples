# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 5

Local lab build for **Module 5 · Interactive Canvas and Hit Testing**. The Canvas playground becomes a
topology editor: `NodeModel`, `EdgeModel` and a per-session `TopologyScene` in world coordinates, one
`RenderScene` method, `ToWorld`/`ToScreen` conversions, hit testing, drag, pan, zoom about the pointer,
off-screen culling and a keyboard path.
The three remaining Module 1 surfaces - `Label` + `ProgressBar`, a `Wisej.Web.Canvas` and an
off-screen `Bitmap` in a `PictureBox` - stay on the operations page under the gauges, so the same
reading can still be compared across surfaces. Surface 2, the inline `Paint` handler, is now the
`TelemetryGauge` control itself.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Drawing and Painting Course/Module 5/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6005
```

Open <http://localhost:6005> and press **Plant topology** on the operations page.

## What to try

| Action | Expected result |
|---|---|
| Click a pump | It is selected, drawn with a heavier blue border, and the list beside the surface highlights the same row. |
| Drag it | It moves in world units, its edges follow, and the status line reports how many requests the gesture cost. |
| Release, then resize the browser | The node keeps its new position: `Redraw` calls the same `RenderScene`, which rebuilds the scene from the model. |
| Click empty space and drag | The selection clears and the viewport pans instead. |
| Wheel over the surface, then click a node | Zoom happens about the pointer, and selection still lands on the right node, because `ToWorld` accounts for the new zoom. |
| Watch the list header | It reports `(6 drawn, 1 culled)`: `Outstation` sits outside the visible world rectangle until you pan or zoom out to it. |
| `Tab` on the surface, then the arrow keys | The next node is selected and the selected one is nudged — 5 world units, or 20 with `Shift`. No mouse required. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `NodeModel`, `EdgeModel`, per-session `TopologyScene` | [Models/TopologyScene.cs](VisualOperationsStudio/Models/TopologyScene.cs) |
| `RenderScene` as the only method that draws | [TopologyPage.cs](VisualOperationsStudio/TopologyPage.cs) |
| `ToWorld`, `ToScreen`, `HitTest` walking the list backwards | `TopologyScene` |
| `MouseDown`, `MouseMove`, `MouseUp` changing state and then rendering | `canvasTopology_MouseDown` / `_MouseMove` / `_MouseUp` |
| Zoom about the pointer from the wheel or buttons | `TopologyScene.ZoomAbout`, `canvasTopology_MouseWheel`, `ZoomFromCentre` |
| Visible-region culling computed once per render | `TopologyScene.VisibleWorld`, `RenderScene` |
| Keyboard selection and nudging, plus the node list | `canvasTopology_KeyDown`, `listNodes` |
| `InteractionNotes.md` | [docs/InteractionNotes.md](VisualOperationsStudio/docs/InteractionNotes.md) |
