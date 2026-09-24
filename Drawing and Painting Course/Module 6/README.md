# VisualOperationsStudio · Drawing & Painting in Wisej.NET · Module 6

Local lab build for **Module 6 · System.Drawing.Managed and Cross-Platform Rendering**. A
`TopologyImageRenderer` turns the same topology model into PNG bytes with no Wisej control and no
session involved: a `Bitmap`, a `Graphics` over it, pens, brushes and a `GraphicsPath`, a font resolved
through a fallback chain, and `Application.Download` handing the bytes to the browser.
The topology page is the application's start page: a toolbar with `Fit to view`, `Reset zoom` and
`Export PNG`, the live Canvas surface, and a status line reporting the export's time, size and
resolved font.

This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Drawing and Painting Course/Module 6/VisualOperationsStudio"
dotnet run -f net10.0 --urls http://localhost:6006
```

Open <http://localhost:6006>, press **Plant topology**, then **Export PNG**.

## What to try

| Action | Expected result |
|---|---|
| **Export PNG** | The browser downloads `plant-topology.png`. The status line reports the size, the time, how many objects were drawn and which font family was actually resolved. |
| Export twice | The first call is slower (JIT and the font probe); after that it settles. Both numbers are recorded in `CrossPlatform.md`. |
| Drag a node, then export | The image follows the model, because the export renders the model again — it never screenshots the Canvas. |
| Empty the scene (`TopologyScene.CreatePlant` returning no nodes) | A readable image saying "No nodes to display", not a blank file. |
| Drop a `.ttf` into a `Fonts/` folder beside the application and export | The status line names that family instead of the runtime fallback. |
| Ask for 4000 × 3000 in `btnExport_Click` | The renderer clamps each side to 200–2000 rather than allocating ~48 MB. |
| Open `VisualOperationsStudio.csproj` | One drawing implementation: `Wisej-4` brings `Managed.System.Drawing`, and there is no direct `System.Drawing.Common` reference. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| `byte[] Render(TopologyScene, int, int)` with no `Wisej.Web` anywhere in the file | [Renderers/TopologyImageRenderer.cs](VisualOperationsStudio/Renderers/TopologyImageRenderer.cs) |
| `Bitmap` + `Graphics.FromImage`, edges with a `Pen`, nodes with a `GraphicsPath`, labels, legend | `Render`, `DrawNode`, `DrawLegend` |
| `ResolveFont` with a `PrivateFontCollection` first and a known-present fallback, recorded | `ResolveFont`, `LoadSuppliedFamily`, `ResolvedFontFamily` |
| `MemoryStream` + `ImageFormat.Png` + `Application.Download` | `Encode`, `btnExport_Click` in [TopologyPage.cs](VisualOperationsStudio/TopologyPage.cs) |
| Disposal audit and bounded output size | the `using` blocks, `Clamp`, `MinimumSide`/`MaximumSide` |
| Reference-graph check, Windows and Linux evidence, the mobile implications | [docs/CrossPlatform.md](VisualOperationsStudio/docs/CrossPlatform.md) |

`CrossPlatform.md` is explicit about what was and was not run here: the Windows figures are measured,
and the Linux container run was **not** executed on this machine — the command to collect it is in the
document.
