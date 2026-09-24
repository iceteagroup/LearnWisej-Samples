# SurfaceNotes — what each surface puts on the network, and what survives a resize

Lab 1 deliverable. Measured against this project running on `http://localhost:6001`, Wisej-4 4.1.0,
`net10.0` (Kestrel), Bootstrap-4 theme, browser viewport 1400×900.

All four surfaces read the single `TelemetrySample` field on `VisualOperationsPage`. None of them
keeps a copy of the value.

## What crosses the network

| Surface | Control | Where the pixels are produced | What travels |
|---|---|---|---|
| 1 | `lblReading` + `progressReading` | Nowhere — the theme renders both | A text property and an integer. No image, no drawing commands. |
| 2 | `panelPainted` (`Paint`) | On the server, into a bitmap Wisej.NET owns | A rendered image of the panel, re-sent on every `Invalidate()`. |
| 3 | `canvasSurface` (`Wisej.Web.Canvas`) | In the browser | The drawing calls themselves — `ClearRect`, `FillStyle`, `FillRect` — batched into one update. |
| 4 | `picExport` ← `RenderOffScreen` | On the server, in a `Bitmap` no control ever owned | The encoded image bytes, once per rebuild. |

Surfaces 2 and 4 cost server CPU and bytes on **every** refresh. Put either on a one-second timer and
you have bought one image transfer per second, per session.

## What survived the browser resize

Resize the window and watch each area:

| Surface | Survived? | Why |
|---|---|---|
| 1 Label + ProgressBar | yes | The theme re-laid them out. There is nothing of ours to restore. |
| 2 `Panel.Paint` | yes | Wisej.NET repainted the panel on the server at the new size and sent a new image. |
| 3 `Wisej.Web.Canvas` | **no** | The browser dropped its bitmap. The area stays blank until `Redraw` runs. |
| 4 off-screen `Bitmap` | yes | It is an image, not a live surface. The resize never touched it. |

That blank rectangle is the whole lesson. The canvas kept *pixels*, and pixels do not survive.
Because the reading lives in `TelemetrySample`, `DrawCanvasScene` can put the scene back exactly as
it was — which is why the `Redraw` handler rebuilds the entire scene instead of patching the last change.

## First draw vs. redraw (verified here)

`Canvas.Redraw` fires on resize, and it is **not** guaranteed to fire before the page is first shown.
This project therefore draws the first scene from `VisualOperationsPage_Load`, where the canvas already
reports its laid-out size (658×243 at 1400×760), and lets `Redraw` handle every later rebuild. Both call
the same `DrawCanvasScene`, so there is one drawing path, not two.

## Why the label and the progress bar stay the default

They are readable by a screen reader, they cost nothing per refresh, they re-lay out for free, and they
are already themed. Custom painting is worth its cost only when the shape itself carries the meaning —
a gauge ring, a sparkline, a topology map. A number does not.

## Degraded paths exercised

- A reading pushed outside 0–100 is clamped by `TelemetrySample.Reading` (status line reports both the
  raw and the clamped value) instead of drawing off the surface.
- `panelPainted_Paint` and `DrawCanvasScene` return immediately on a zero-sized or not-yet-laid-out area.
- A failure inside `RenderOffScreen` is caught: `picExport` is cleared, the status line explains, and the
  other three surfaces keep showing the reading.
