# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 4

Local lab build for **Module 4 · Canvas and the HTML5 2D Model**. A `CanvasPlaygroundPage` ports a set
of browser Canvas 2D examples to the documented `Wisej.Web.Canvas` API: paths and fills, a placed
caption, linear and radial gradients, four transform blocks bracketed by `Save`/`Restore`, and a state
section with a clip, a reduced alpha, a dash pattern and a shadow.
The playground is the application's start page: a toolbar with `Render`, `Resize surface` and
`Progressive (LiveUpdate)`, the dark drawing surface, and a status line naming what the last render did.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Drawing and Painting Course/Module 4/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6004
```

Open <http://localhost:6004> and press **Canvas playground** on the operations page.

## What to try

| Action | Expected result |
|---|---|
| Open the playground | Five sections arrive in one update, because `LiveUpdate` is off for the main render. |
| Resize the browser | The surface goes blank, then `Redraw` calls `DrawPlayground` and the scene comes back unchanged. |
| Press **Progressive draw (LiveUpdate)** | `LiveUpdate` is switched on for one short server-side loop; the eight steps appear one at a time, then it is switched off again. |
| Look at the rotated square | The browser example rotated by `Math.PI / 4`; `Canvas.Rotate` takes **degrees**, so the port is `Rotate(45)` — a quarter turn, not a fraction of a degree. |
| Delete a `Restore()` in `DrawTransforms` and re-run | Everything drawn after that block visibly shifts, because the transform is Canvas state. |
| Press **Back to operations** | The operations page comes back with its gauges and grid intact — the same page instance, not a rebuilt one. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `DrawPlayground` subscribed to `Redraw`, opening with `ClearRect` | [CanvasPlaygroundPage.cs](VisualOperationsStudio/CanvasPlaygroundPage.cs) |
| Path, fills and a caption placed with `TextFont`/`TextAlign`/`TextBaseline` | `DrawPathsAndFills`, `Caption` |
| `CreateLinearGradient` and `CreateRadialGradient` | `DrawGradients` |
| Four transform blocks, each inside `Save`/`Restore`, including the radians→degrees port | `DrawTransforms` |
| `Clip()`, `GlobalAlpha`, `SetLineDash`, shadow | `DrawState` |
| `LiveUpdate` off for the main render, on for one progressive example | `btnProgressive_Click` |
| `Lab4Notes.md` | [docs/Lab4Notes.md](VisualOperationsStudio/docs/Lab4Notes.md) |
