# PaintCost — what one `TelemetryGauge` repaint costs

Lab 2 deliverable. Measured on this project at `http://localhost:6002`, Wisej-4 4.1.0, `net10.0`
(Kestrel), Bootstrap-4 theme, browser viewport 1400×900, three gauges on `VisualOperationsPage`.

## The painted surface

| | Value |
|---|---|
| Painted surface per gauge | 417 × 361 px at 1400 × 900 (the card, less its padding; the face uses a 350 px disc inside it) |
| Objects created per repaint | 5 in the arcs block (2 `Pen`, 3 `SolidBrush`), 2 in the needle block (`GraphicsPath`, `SolidBrush`), 4 in the text block (2 `Font`, 2 `SolidBrush`) |
| Objects borrowed, never disposed | `e.Graphics` — the surface belongs to Wisej.NET |
| Bytes per repaint | one rendered image of the painted surface, sent over the session's WebSocket. Wisej.NET does not expose the encoded size to the application, so this is stated as "one image per repaint" rather than as a number this sample could measure honestly. |

## Repaint frequency

The point of the property setters is that this number stays low. Each setter clamps, compares, and
returns when nothing changed, so:

| Action | Gauges that asked for a repaint |
|---|---|
| **Take reading** (a new spindle value, the other two re-assigned unchanged) | **1** |
| **Take reading** where the scripted value repeats the current one | **0** |
| **Reset** from a changed spindle value | **1** |
| Browser resize | 3 — every painted control has to repaint, because every coordinate it uses came from its own size |

The readout beside the buttons reports the count for the request you just made. A handler copied into six panels
would produce six pictures, six sets of drawing objects and repaints nobody asked for; that is the
cost this design is avoiding.

## When a themed control would be cheaper

Whenever the shape carries no meaning the number does not already carry. A `ProgressBar` and a `Label`
cost one property assignment each, re-lay out for free, are readable by a screen reader and are already
themed — Module 1's surface 1 is the comparison. Reach for a painted control when the *geometry* is the
information: a ring with threshold zones, a needle position, a sparkline, a topology map.

Two more cases where painting stops paying:

- **High update frequency.** Put this gauge on a one-second timer and you have bought one server render
  and one image transfer per second, per session.
- **Many instances.** Module 3 is exactly this: a thousand rows cannot have a thousand gauge controls.

## The reading must exist as text

Each gauge sets `AccessibleDescription` from its `Value`, `Caption` and `Unit` setters, and the page
prints the same three numbers in the panel beside the gauges. A value that exists only inside a picture
has been lost for part of the audience.

## Cost control applied here

- The coordinate maths is in `GaugeGeometry`, which references no `Graphics`, no control and no session —
  so the hard part is testable, and the paint handler does no arithmetic of its own.
- `GaugeGeometry.Measure` returns an empty layout below 60 px, and the handler returns before creating a
  single drawing object.
- No image or font file is loaded inside the handler; `ResolveFont` falls back to the generic sans serif
  when the requested family is missing.
- The needle is the only transformed section. It is bracketed by `BeginContainer`/`EndContainer` with the
  restore in a `finally`, so an exception cannot leak a transform into whatever is drawn next.
