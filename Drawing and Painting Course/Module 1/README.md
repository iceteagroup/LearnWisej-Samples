# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 1

Local lab build for **Module 1 · The Wisej.NET Drawing Architecture**. One `TelemetrySample` is rendered
on four surfaces at once: a Label plus ProgressBar, a `Panel` with a `Paint` handler, a `Wisej.Web.Canvas`
driven by `Redraw`, and an off-screen `Bitmap` built with `Graphics.FromImage`.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

Requirements: .NET 10 SDK and NuGet access to `Wisej-4` 4.1.0. Use a Wisej development license or trial
as required by your installation. No database setup or external service is needed.

```bash
cd "Drawing and Painting Course/Module 1/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6001
```

Open <http://localhost:6001>. In Visual Studio, open `VisualOperationsStudio.slnx` and press F5.
The project targets both `net10.0-windows` and `net10.0`, so the CLI needs `-f` when running.

## What to try

| Action | Expected result |
|---|---|
| Press **New reading** | All four areas show the same number. Each one arrived there differently: a property, an image, drawing commands, image bytes. |
| Press it four times | The fourth reading is deliberately out of range. `TelemetrySample.Reading` clamps it and the status line reports the raw and the clamped value. |
| Resize the browser window | Surfaces 1, 2 and 4 come back on their own. The canvas goes blank until `Redraw` rebuilds it from the model. |
| Make the window very narrow, then widen it again | Nothing throws: the paint handler and the canvas scene return immediately on a zero-sized area. |

Every browser session owns its own `TelemetrySample`. Nothing is persisted.

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| One model instance every surface reads | [Models/TelemetrySample.cs](VisualOperationsStudio/Models/TelemetrySample.cs) |
| The four surfaces and the named controls | [VisualOperationsPage.Designer.cs](VisualOperationsStudio/VisualOperationsPage.Designer.cs) |
| `Paint` handler drawing from `e.Graphics` inside `e.ClipRectangle` | `panelPainted_Paint` in [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) |
| `Redraw` handler that rebuilds the whole scene | `canvasSurface_Redraw` / `DrawCanvasScene` in [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) |
| Off-screen `Bitmap` via `Graphics.FromImage` | `RenderOffScreen` in [VisualOperationsPage.cs](VisualOperationsStudio/VisualOperationsPage.cs) |
| `SurfaceNotes.md` | [docs/SurfaceNotes.md](VisualOperationsStudio/docs/SurfaceNotes.md) |
