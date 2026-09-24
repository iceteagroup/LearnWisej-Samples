# IconDesk · Icons & Images in Wisej.NET · Module 2

Local lab build for **Module 2 · Bitmaps, SVGs and PictureBox: Formats, Sizing and Loading**. An
`ImageLab` page that makes the difference visible: one 360 x 200 raster shown through
`ImageSource` in four `SizeMode` values, one JPEG fetched with `LoadAsync` while the page stays
responsive, one SVG on `ImageSource` at 16, 24, 32 and 48 pixels against a 16-pixel bitmap blown
up to the same sizes, and a report naming the property behind each box.

Module 1's `CommandPage` is carried forward unchanged; the application opens on the page this
module builds. This folder is the complete application at this point in the course, with its own
solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 2/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6202
```

Open <http://localhost:6202>.

## The screen

`IconDesk — ImageLab`: the four size-mode frames under **ONE RASTER ASSET · FOUR SIZE MODES**,
then the `LoadAsync` box beside the SVG-versus-bitmap box, then the report panel. Nothing else.

## What to try

| Action | Expected result |
|---|---|
| Compare the four frames | Same picture, same box size, four results. `Normal` shows the top-left corner at 1:1 (the caption bar is outside the box), `Zoom` letterboxes the whole picture, `Cover` fills and crops the sides, `StretchImage` fills and turns the sun into an ellipse. |
| Watch the page open | The `LoadAsync` box says *waiting for the host…* for ~2.5 s. Every other control answers immediately while it waits - the handler that started the load returned long ago. |
| Hover the picture once it arrives | The tooltip reports `The picture is on ImageSource ("slow/hero.jpg") and Image is still null`. `LoadAsync` hands the URL to the **browser**; the server never downloads or decodes it. |
| Compare the SVG row with the bitmap row | One request for `/Images/glyph-save.svg` in the network panel and four clean renderings. The bitmap row is one 16-pixel `Image` object stretched to the same sizes: soft by 32, unusable by 48. |
| Read the report | Five boxes answer `ImageSource` and one answers `Image`, each read from the control rather than remembered. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| One raster in Normal, Zoom, Cover and StretchImage | `ShowRasterInEverySizeMode` + `pnlModes` in [ImageLabPage.Designer.cs](IconDesk/ImageLabPage.Designer.cs) |
| A JPEG fetched with `LoadAsync` without blocking | `OnLoad` / `picRemote_LoadCompleted` in [ImageLabPage.cs](IconDesk/ImageLabPage.cs) |
| One SVG on `ImageSource` at four sizes, against a bitmap | `ShowVectorAtEverySize` / `ShowBitmapAtEverySize` |
| Report naming `Image` or `ImageSource` per box | `Report` |
| Format note | [docs/FormatNotes.md](IconDesk/docs/FormatNotes.md) |

## Notes for anyone extending the lab

- **`LoadAsync` sets `ImageSource`, not `Image`.** The browser performs the request. A URL only the
  server can reach will not work this way - fetch it in C# and assign `Image` instead.
- `ShowLoader` is not switched off for you. `LoadCompleted` is the signal; the lab turns the
  loader off there and swaps the waiting label for the picture.
- `/slow/hero.jpg` is served by this application's own `Startup.cs` with a deliberate 2.5-second
  delay. It exists so the asynchronous behaviour can be demonstrated with no internet access and
  with the same delay every run. It is an ordinary minimal-API endpoint, mapped **before**
  `UseFileServer`.
- `Images/glyph-save.svg` is written the way a recolourable line icon has to be written: one
  meaningful `fill` on the root so Wisej.NET treats it as an icon, `fill="none"` on every path,
  and `stroke="currentColor"` so the strokes follow the colour Wisej.NET injects. A literal stroke
  colour renders correctly and then never changes colour again - see Module 3.
- `Microsoft.AspNetCore.Http.HttpContext` has to be written out in full in `Startup.cs`:
  `Wisej.Core` defines an `HttpContext` too, and importing both makes the name ambiguous.
