# InteractionNotes — what a server-side interaction loop costs

Lab 5 deliverable. Measured on this project at `http://localhost:6005`, Wisej-4 4.1.0, `TopologyPage`,
Chromium, browser viewport 1400×900, a local server on the same machine.

## The measurement

The page counts the pointer messages it handles during a gesture and prints the total when the mouse
comes up: *"Drag finished: N requests, one render each."*

| Gesture | Requests handled | What each one did |
|---|---|---|
| A short drag of `Pump 2` (about 120 px, well under a second) | **3** | convert, move the node in world units, re-render the whole scene |
| A slow two-second drag across the surface | **10–25**, depending on how fast the pointer moves | the same |
| A wheel zoom | 1 | `ZoomAbout`, then one render |
| An arrow-key nudge | 1 | move 5 world units (20 with Shift), then one render |

Wisej.NET already coalesces pointer moves — the browser fires far more `mousemove` events than the
numbers above — but every one that survives is a full round trip: request, handler, whole-scene render,
response. On a local server that is invisible. Over a 60 ms link it is a drag that lags a quarter of a
second behind the pointer, and every user of the application is paying for it on the same server.

## Where the line is

This sample would keep the loop on the server. Seven nodes, one render of a few dozen drawing calls,
and an editor people use for a few seconds at a time: the round trip is the cheapest part of the
system, and the model stays in one place, which is the whole reason the scene is authoritative.

I would move the interaction loop into client JavaScript when any of these is true:

- **The gesture is continuous and long** — a pan across a large plant, a rubber-band selection, anything
  the user holds for seconds rather than a moment.
- **The render is not free** — more than a few hundred primitives per frame, or a scene where culling,
  simplification and layer caching have already been tried. That is the order to try them in: fewer
  primitives, then skip what is off-screen, then simplify the geometry, then cache the layers that
  rarely change. Moving the loop into the browser is the last step, not the first.
- **The latency is real** — anything beyond a local network. A pointer that visibly trails the mouse is
  a worse editor than one that is slightly out of date with the server.
- **Sessions are many** — one server-side drag is one render; a hundred concurrent editors dragging is a
  hundred renders a tick.

The split that works: the browser owns the *gesture* (a ghost rectangle following the pointer), the
server owns the *result* (one message on mouse-up carrying the final position). `LiveUpdate` stays off
for the main render either way — it sends each call as it is made, which is the opposite of what a
batched scene render wants.

## What the model owes the renderer

- **Nodes and edges carry stable ids** and their own world geometry. Selection is a flag on the node,
  not an index into whatever the last render happened to draw.
- **The scene lives on the page instance**, so it belongs to this session. A mutable `static` field here
  would be one plant shared by every user of the application.
- **`RenderScene` decides nothing.** It clears, applies `Translate` and `Scale` once, draws edges then
  nodes, and restores. The Redraw event calls the same method, which is why a browser resize simply
  rebuilds the picture instead of losing it.
- **Hit testing runs the conversion backwards.** `ToWorld` undoes the pan and the zoom, and the node
  list is walked backwards so the one drawn last — and therefore on top — is the one the user gets.
  The Canvas offers no point-in-path test to lean on, so this arithmetic is ours.

## Culling

The visible world rectangle is computed once per render and every node and edge outside it is skipped.
The header above the node list reports the split: with the demonstration plant it reads
**"6 drawn, 1 culled"** — the `Outstation` node sits at world (1500, 520), outside the viewport until
you pan or zoom out to it.

## Accessibility

The list beside the surface holds the same nodes in the same order, selection is synchronised both
ways, `Tab` moves to the next node and the arrow keys nudge the selected one (`Shift` for a larger
step). The status line names the selection and its world position in text. None of the information in
the picture exists only as pixels.
