# Module 2 lab note - which asset travels as bytes and which travels as a string

Read off the running Image lab and the browser network panel, not from the documentation.

## The four size modes

All four boxes are 220 x 150 and all four show the same 480 x 300 `photo.png` through `Image`.
The only variable is `SizeMode`, and the picture was drawn with a hard border and a caption in one
corner precisely so the differences cannot be argued about:

| SizeMode | What you see | When it is the right answer |
|---|---|---|
| `Normal` | The top-left corner of the picture at 1:1. The border and the caption are outside the box. | You know the asset is already the right size and you want no resampling at all. |
| `Zoom` | The whole picture, letterboxed. Border and caption both visible. | Anything where losing part of the image would be wrong - a chart, a diagram, a scan. |
| `Cover` | The box is filled edge to edge, aspect kept, the outside cropped. | Thumbnails, hero images, avatars: fill the space, accept the crop. |
| `StretchImage` | The box is filled and the picture is distorted. | Almost never. Keep it for the rare case where the source aspect is meaningless. |

## The asynchronous fetch, and a surprise

`picRemote` is filled by `LoadAsync`. The important behaviour is the obvious one: the handler
returns immediately, the other controls stay responsive, and `LoadCompleted` arrives later.

The surprise is where the picture ends up:

```
the slow local endpoint: LoadCompleted after 2750 ms. The picture is on ImageSource
("slow/photo.jpg") and Image is still null - the browser fetched it, not the server.
```

`LoadAsync` sets **`ImageSource`**, not `Image`. The server hands the URL to the client and the
browser performs the request. Two consequences worth carrying into production:

- The URL must be reachable **from the browser**, not from the server. A URL that only the server
  can resolve - something inside your network, or behind server-side credentials - will not load
  this way. Fetch it in C# and assign `Image` instead.
- Nothing about the asset is decoded server-side, so there is no server memory cost and no PNG
  re-encoding. For a large photo that is a real saving.

The lab ships two buttons. **Fetch the slow JPEG** points at `/slow/photo.jpg`, an endpoint this
application serves with a deliberate 2.5-second delay, so the asynchronous behaviour can be
demonstrated on a machine with no internet access and the delay is the same every run.
**Fetch a remote JPEG** points at a public URL, and is allowed to fail: on a restricted network
`LoadCompleted` reports that nothing arrived, every other picture on the page is unaffected, and
that is exactly the failure mode asynchronous loading is bought for.

## One SVG, four sizes

`picSvg16` through `picSvg48` all carry the same string, `Images/logo.svg`, on `ImageSource`.
The browser requests `/Images/logo.svg` **once** and renders it at each control's own size, so the
16-pixel copy is not a shrunken 48-pixel bitmap - it is the same vector drawn small. That is the
argument for vector interface icons in one screenshot.

## Which pipeline each lab asset uses

| Asset | Pipeline | Travels as |
|---|---|---|
| `photo.png` (four size-mode boxes) | `Image` | One `component.wx` request returning PNG bytes that Wisej.NET produced from the server-side image object. |
| `photo.jpg` (via `/slow/photo.jpg`) | `ImageSource`, set by `LoadAsync` | An ordinary browser request to that URL. The server never decodes it. |
| `logo.svg` (four sizes) | `ImageSource` | One request for `/Images/logo.svg`, still an SVG at the browser. |
| `open.png`, `print.png`, `delete.png` (Module 1) | `Image` / `ImageList` | `component.wx` PNG requests. |
| `icon-check` (Module 1) | `ImageSource` | Nothing separate - it arrives inline with the theme as a `data:image/svg+xml` URL. |

The rule that falls out of the table: anything your code produced or received as a .NET object
goes through `Image`; anything that exists as a file or a name the browser can fetch should stay a
string, because then it keeps its format, its cacheability and - for SVG - its resolution
independence.
