# Module 2 lab note - which asset travels as bytes and which travels as a string

Read off the running ImageLab page and the browser network panel, not from the documentation.

## The four size modes

All four boxes are 220 x 150 and all four show the same 360 x 200 `workbench.png` through
`ImageSource`. The only variable is `SizeMode`, and the picture was drawn with a caption bar along
the bottom precisely so the differences cannot be argued about:

| SizeMode | What you see | When it is the right answer |
|---|---|---|
| `Normal` | The top-left corner of the picture at 1:1. The caption bar is outside the box. | You know the asset is already the right size and you want no resampling at all. |
| `Zoom` | The whole picture, letterboxed. The caption bar is visible. | Anything where losing part of the image would be wrong - a chart, a diagram, a scan. |
| `Cover` | The box is filled edge to edge, aspect kept, the left and right edges cropped. | Thumbnails, hero images, avatars: fill the space, accept the crop. |
| `StretchImage` | The box is filled and the picture is distorted - the sun is an ellipse. | Almost never. Keep it for the rare case where the source aspect is meaningless. |

The full set is `Normal, StretchImage, AutoSize, CenterImage, Zoom, Cover`.

## The asynchronous fetch, and a surprise

`picRemote` is filled by `LoadAsync`. The important behaviour is the obvious one: the handler
returns immediately, the other controls stay responsive, and `LoadCompleted` arrives later.

The surprise is where the picture ends up:

```
LoadCompleted after 2733 ms. The picture is on ImageSource ("slow/hero.jpg") and Image is
still null - the browser fetched it, not the server.
```

`LoadAsync` sets **`ImageSource`**, not `Image`. The server hands the URL to the client and the
browser performs the request. Two consequences worth carrying into production:

- The URL must be reachable **from the browser**, not from the server. A URL that only the server
  can resolve - something inside your network, or behind server-side credentials - will not load
  this way. Fetch it in C# and assign `Image` instead.
- Nothing about the asset is decoded server-side, so there is no server memory cost and no PNG
  re-encoding. For a large photo that is a real saving.

`/slow/hero.jpg` is an endpoint this application serves with a deliberate 2.5-second delay, so the
asynchronous behaviour can be demonstrated on a machine with no internet access and the delay is
the same every run. The ImageLab screen has no status strip, so the finding is put on the
picture's tooltip and in this note.

## One SVG at four sizes, and one bitmap at four sizes

`picSvg16` through `picSvg48` all carry the same string, `Images/glyph-save.svg`, on
`ImageSource`. The browser gets the vector and renders it at each control's own size, so the
16-pixel copy is not a shrunken 48-pixel bitmap - it is the same artwork drawn small.

`picLegacy16` through `picLegacy48` are the control group: one 16-pixel `System.Drawing.Image`
object assigned to all four, with `SizeMode = StretchImage`. Nothing regenerates raster artwork,
it is only ever resampled, so 32 is soft and 48 is unusable. That is the argument for vector
interface icons in one screenshot.

`glyph-save.svg` had to be written carefully. Wisej.NET does not hand the browser a URL for an SVG
image source: it reads the file and inlines it as a `data:image/svg+xml;base64` background, and on
the way through it injects `fill` and `color` on the root element. A first version of this file
used `fill="none"` on a `<g>` and a literal stroke colour, and the injected root fill won - the
outline glyph came back as a solid blue blob. The working form puts `fill="none"` on **each path**
and strokes with `currentColor`, so the injected `color` is what the strokes take.

## Which pipeline each lab asset uses

| Asset | Pipeline | Travels as |
|---|---|---|
| `workbench.png` (four size-mode boxes) | `ImageSource` | One ordinary browser request for `/Images/workbench.png`, reused by all four boxes. |
| `hero.jpg` (via `/slow/hero.jpg`) | `ImageSource`, set by `LoadAsync` | An ordinary browser request to that URL. The server never decodes it. |
| `glyph-save.svg` (four sizes) | `ImageSource` | Inlined by Wisej.NET as a `data:image/svg+xml` background, recoloured to the control's `ForeColor`. |
| `glyph-save-16.png` (bitmap row) | `Image` | One `component.wx` request returning PNG bytes from the server-side image object. |
| `open.png`, `print.png`, `delete.png` (Module 1) | `Image` / `ImageList` | `component.wx` PNG requests. |
| `icon-save` (Module 1) | `ImageSource` | Nothing separate - it arrives inline with the theme as a `data:image/svg+xml` URL. |

The rule that falls out of the table: anything your code produced or received as a .NET object
goes through `Image`; anything that exists as a file or a name the browser can fetch should stay a
string, because then it keeps its format, its cacheability and - for SVG - its resolution
independence.
