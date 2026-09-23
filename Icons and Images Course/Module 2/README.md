# IconDesk · Icons & Images in Wisej.NET · Module 2

Local lab build for **Module 2 · Raster and Vector: the Two Pipelines**. An `ImageLab` page that
makes the difference visible: one 480 x 300 raster shown through `Image` in four `SizeMode` values,
one JPEG fetched with `LoadAsync` while the page stays responsive, and one SVG on `ImageSource`
rendered at 16, 24, 32 and 48 pixels.
The Module 1 command panel is unchanged and reachable from the **Image lab** button.
This folder is the complete application at this point in the course, with its own solution and port.

## Run it

```bash
cd "Icons and Images Course/Module 2/IconDesk"
dotnet run -f net10.0 --urls http://localhost:6202
```

Open <http://localhost:6202> and press **Image lab**.

## What to try

| Action | Expected result |
|---|---|
| Compare the four boxes | Same picture, same box size, four results. `Normal` shows the top-left corner at 1:1, `Zoom` letterboxes the whole picture, `Cover` fills and crops, `StretchImage` fills and distorts. |
| Watch the page open | `picRemote` shows a spinner for ~2.5 s. Every other control answers immediately while it spins - the handler that started the load returned long ago. |
| Read the status line after it arrives | `The picture is on ImageSource ("slow/photo.jpg") and Image is still null`. `LoadAsync` hands the URL to the **browser**; the server never downloads or decodes it. |
| Press **Fetch a remote JPEG** | On a machine with outbound access the picture changes. On a restricted network nothing arrives and the page says so - and the rest of the page is untouched. |
| Look at the four SVG boxes | One request for `/Images/logo.svg` in the network panel, four crisp renderings. The 16-pixel copy is drawn small, not shrunk. |
| Press **Report every PictureBox** | Each box names the property that supplied its picture, read from the control rather than guessed. |

## Lab tasks → where in the code

| Deliverable | Implementation |
|---|---|
| One raster in Normal, Zoom, Cover and StretchImage | `ShowRasterInEverySizeMode` + `layoutSizeModes` in [ImageLabPage.Designer.cs](IconDesk/ImageLabPage.Designer.cs) |
| A JPEG fetched with `LoadAsync` without blocking | `Fetch` / `picRemote_LoadCompleted` in [ImageLabPage.cs](IconDesk/ImageLabPage.cs) |
| One SVG on `ImageSource` at four sizes | `ShowVectorAtEverySize` |
| Report naming `Image` or `ImageSource` per box | `Report` |
| Format note | [docs/FormatNotes.md](IconDesk/docs/FormatNotes.md) |

## Notes for anyone extending the lab

- **`LoadAsync` sets `ImageSource`, not `Image`.** The browser performs the request. A URL only the
  server can reach will not work this way - fetch it in C# and assign `Image` instead.
- `ShowLoader` is not switched off for you. `LoadCompleted` is the signal; the lab turns the
  spinner off there.
- `/slow/photo.jpg` is served by this application's own `Startup.cs` with a deliberate 2.5-second
  delay. It exists so the asynchronous behaviour can be demonstrated with no internet access and
  with the same delay every run. It is an ordinary minimal-API endpoint, mapped **before**
  `UseFileServer`.
- `Microsoft.AspNetCore.Http.HttpContext` has to be written out in full in `Startup.cs`:
  `Wisej.Core` defines an `HttpContext` too, and importing both makes the name ambiguous.
