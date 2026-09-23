# Lab4Notes — porting browser Canvas 2D examples to `Wisej.Web.Canvas`

Lab 4 deliverable. Verified against this project at `http://localhost:6004`, Wisej-4 4.1.0,
`CanvasPlaygroundPage`, browser viewport 1400×900.

## Every Canvas member this page uses, and the browser member it replaced

| Wisej.NET member | Browser member | Note on the port |
|---|---|---|
| `ClearRect(x, y, w, h)` | `clearRect` | Casing only. First call of `DrawPlayground`, always. |
| `BeginPath()` | `beginPath` | Casing only. Skip it and a later `Stroke` drags the previous outline along. |
| `MoveTo` / `LineTo` / `Stroke` | `moveTo` / `lineTo` / `stroke` | Casing only; coordinates are `int`, not `double`. |
| `Fill()` | `fill` | Casing only. |
| `Arc(x, y, radius, start, end, counterClockwise)` | `arc` | **Angles are degrees**, not radians: a full circle is `0f, 360f`. |
| `FillRect` / `StrokeRect` | `fillRect` / `strokeRect` | Casing only. |
| `FillStyle` / `StrokeStyle` | `fillStyle` / `strokeStyle` | A **`System.Drawing.Color`**, or a gradient object — not a CSS string. |
| `LineWidth` | `lineWidth` | `int`, not a float. |
| `TextFont` | `font` | A **`System.Drawing.Font`**, not the CSS `"bold 12px sans-serif"` shorthand. |
| `TextAlign` | `textAlign` | The `CanvasTextAlign` enum: `Start, End, Center, Left, Right`. |
| `TextBaseline` | `textBaseline` | The `CanvasTextBaseline` enum: `Alphabetic, Top, Hanging, Middle, Ideographic, Bottom`. |
| `FillText(text, x, y)` | `fillText` | Casing only. |
| `CreateLinearGradient(x0, y0, x1, y1, colorStops)` | `createLinearGradient` + `addColorStop` | The stops are passed **as part of the call**, not added afterwards. |
| `CreateRadialGradient(x0, y0, r0, x1, y1, r1, colorStops)` | `createRadialGradient` + `addColorStop` | Same. |
| `Save()` / `Restore()` | `save` / `restore` | Casing only. |
| `Translate` / `Scale` | `translate` / `scale` | Casing only. |
| `Rotate(degrees)` | `rotate(radians)` | **The unit changes.** `rotate(Math.PI / 4)` becomes `Rotate(45)`. |
| `SetTransform(a, b, c, d, e, f)` | `setTransform` | Casing only; the last two arguments are `int`. |
| `Clip()` | `clip` | Casing only; applies the current path. |
| `GlobalAlpha` | `globalAlpha` | `float`. |
| `SetLineDash(int[])` | `setLineDash` | Takes `int[]`, not a list of numbers. |
| `ShadowColor` / `ShadowBlur` / `ShadowOffsetX` / `ShadowOffsetY` | same names | `ShadowColor` is a `Color`. |
| `LiveUpdate` | *(no equivalent)* | Wisej.NET only: whether each call is sent as it is made, or the whole scene in one update. |

## The member I could not find, and what I did instead

**`measureText`.** There is no text-measurement member on `Wisej.Web.Canvas`. Everything is placed
with `TextAlign` and `TextBaseline` instead — see `Caption` and `SectionTitle`, which set
`TextAlign = Left` and `TextBaseline = Top` and draw from a known origin. Where a measurement is
genuinely needed, measure off-screen with `Graphics.MeasureString` (Module 2 does exactly that for the
gauge text) and send the result as coordinates.

Two other browser habits that do not survive the port: style **strings** (`"#1f9d6b"`, `"bold 12px
sans-serif"`) become `Color` and `Font` values, and **radians** become degrees.

## The colour-stop shape that actually works

`colorStops` is an `object[]`, and each element must serialize to an object with a `stop` and a
`color` — the client reads `colorStops[i].stop` and `colorStops[i].color` and calls `addColorStop`:

```csharp
c.CreateLinearGradient(x0, y0, x1, y1, new object[]
{
    new { stop = 0f,   color = "#1f9d6b" },
    new { stop = 0.6f, color = "#e8a13c" },
    new { stop = 1f,   color = "#d93a3a" },
});
```

An array of pairs (`new object[] { 0f, Color.Green }`) compiles and draws a solid near-black fill
instead of failing, which is an easy hour to lose.

## `LiveUpdate`

`LiveUpdate` is **false** for the main render: the five sections are issued together and arrive in one
update. **Progressive draw** turns it on for one short server-side loop, draws a step per iteration and
turns it off again, and the eight steps then appear one at a time.

Turn it on only when a long server-side operation should reveal its progress as it works. It is not a
way to animate: a request per frame is the wrong architecture, and a drawing loop that needs a frame
rate belongs in client JavaScript.

## Two failures worth seeing on purpose

- **Resize the browser.** The surface goes blank until `Redraw` calls `DrawPlayground`, which rebuilds
  the scene from nothing. That is why the page has exactly one drawing method and the Redraw handler
  calls it — a handler that only patched the last change would be broken by the first resize.
- **Remove one `Restore()`** from a transform block. Everything drawn afterwards drifts out of place,
  because the transform is Canvas state and not an argument to a single call. Neither failure shows up
  in the designer, which is exactly why they cost so much time.
