# IntegrationLab · Application Integration Course · Module 7

Local lab build for **Module 7 · Events & Handler Contracts**. It follows the walkthrough video:
a gauge, a knob and a line chart are hosted in three `Wisej.Web.Widget` wrappers, each wires
**one** vendor callback, forwards a **compact payload**, and the server turns it into a typed
.NET event after validating every field. The right-hand card is the **Server WidgetEvent log**
from the video's finished screen ("IntegrationLab — Event Demo").

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 7/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5077
```

Then open <http://localhost:5077>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes; warning CS7022 (Program.Main ignored) is expected.

## What to try in the Event Demo window

| Button / gesture | Path | What you should see in the log |
|---|---|---|
| **Gauge 104** | success · gauge | `→ .NET→JS update(options) {"value":104}` then exactly one `← JS→.NET thresholdCrossed e.Data = {"value":104,"level":"high"}` and `• .NET ThresholdCrossed raised via OnWidgetEvent → GaugeThresholdEventArgs { Value=104, Level=high }` — banner + toast. Press it again: nothing (already above). |
| **Gauge 72** | success · gauge | the value goes down: no event (falling edge). Gauge 104 again → one event again. |
| **Knob +10** | success · knob (server-driven) | `update(options) {"value":60}` then `valueChanged e.Data = {"value":60,"source":"server"}` and `• .NET OnValueChanged page-level WidgetEvent switch → KnobValueEventArgs { Value=60, Source=server }` |
| **drag the knob** | success · knob (user-driven) | `valueChanged {"value":…,"source":"user"}` per value step, the dial pulses (`Call("pulse")`) |
| **click a chart point** | success · chart | `← JS→.NET pointClicked e.Data = {"index":3,"label":"Apr","value":68}` then `• .NET PointClicked raised via OnWebEvent → ChartPointEventArgs { Index=3, Label=Apr, Value=68 }` + toast |
| **hover / wheel-zoom / click the legend** on the chart | communication (kept local) | nothing in the log — those vendor events stay in the browser |
| **Chart: new data** | success · chart | `update(options) {"labels":[…],"series":[…]}` — the vendor re-renders; its `render` event stays local |
| **Destroy & recreate chart** | lifecycle | `update(options) {"theme":"dark"} → adapter destroys + recreates … and re-wires`; the chart turns dark; **click a point: `pointClicked` still arrives** (same `wire()` from init and from the recreate path) |
| **Noise counter** | communication (proof) | `CallAsync("getNoiseCount")` → `events kept in the browser: N (hover … zoom … render … layout … legend …) · forwarded to .NET: M` under the chart, and `vendor instance #2` after a recreate |
| **Bad payload** | failure | `→ .NET→JS Call("fireBadPayload")`, `← JS→.NET pointClicked e.Data = {"index":-1}`, `✖ rejected pointClicked rejected: index out of range (-1; 0..5)` — no .NET event, the page stays alive |
| **▶ Stream** | progress | a `Timer` replays 15 gauge readings; `thresholdCrossed` fires **once** with `warn` and **once** with `high`, not 15 times |
| **Clear log** | housekeeping | empties the log and hides the banner |

The banner under the log always shows the **last .NET event raised** (or the last rejection).

## Deliverables

| Deliverable | File |
|---|---|
| Three client event handlers | [`IntegrationLab/docs/ClientEventHandlers.md`](IntegrationLab/docs/ClientEventHandlers.md) — code in `IntegrationLab/wwwroot/gauge-init.js`, `knob-init.js`, `chart-init.js` |
| Three server-side event handlers | [`IntegrationLab/docs/ServerEventHandlers.md`](IntegrationLab/docs/ServerEventHandlers.md) — code in `IntegrationLab/Widgets/GaugeWidget.cs` (`OnWidgetEvent`), `IntegrationLab/Window1.cs` (`knob_WidgetEvent`), `IntegrationLab/Widgets/ChartWidget.cs` (`OnWebEvent`) |
| Payload contract table | [`IntegrationLab/docs/PayloadContract.md`](IntegrationLab/docs/PayloadContract.md) — DTOs in `IntegrationLab/Contracts/` |

## Where things live

```
IntegrationLab/
├─ Contracts/
│  ├─ GaugeThresholdEventArgs.cs   thresholdCrossed { value, level }   → DTO (primitives only)
│  ├─ KnobValueEventArgs.cs        valueChanged { value, source }      → DTO
│  └─ ChartPointEventArgs.cs       pointClicked { index, label, value }→ DTO
├─ Widgets/
│  ├─ GaugeWidget.cs               : Widget — OnWidgetEvent → typed ThresholdCrossed
│  ├─ KnobWidget.cs                : Widget — state + TryReadValueChanged; the PAGE handles WidgetEvent
│  ├─ ChartWidget.cs               : Widget — OnWebEvent (+ base call) → typed PointClicked; CallAsync noise counter
│  ├─ PayloadReader.cs             guarded reads of untrusted dynamic fields
│  └─ TraceEventArgs.cs            log lines for the UI
├─ wwwroot/
│  ├─ gauge-init.js / knob-init.js / chart-init.js   the three client adapters (InitScript, embedded)
│  ├─ vendor-gauge.js, jquery-lite.js, vendor-knob.js  shared "third-party" libraries (Packages)
│  └─ vendor-chart.js + vendor-chart.css               the noisy chart library written for this module
├─ docs/                           the three deliverables
├─ Window1.cs / .Designer.cs       IntegrationLab — Event Demo
├─ Program.cs / Startup.cs         session entry point / Kestrel host
```

## Self-check answers (lab guide)

- **Why not forward every vendor event?**
  A chart fires hover, zoom, render, layout, legend and click callbacks — dozens per second while the
  pointer moves. Forwarding them all is traffic and noise, and it couples the application to the
  vendor's internals (the app would switch on vendor names and dig through vendor objects). The
  wrapper picks the events that are **business decisions or server-owned state changes** —
  `thresholdCrossed`, `valueChanged`, `pointClicked` — decides in the browser when they are
  meaningful (rising edge, once), and sends a small payload. "Noise counter" shows the ratio.
- **What is the difference between `WidgetEvent` and `OnWebEvent`?**
  `WidgetEvent` (virtual `OnWidgetEvent(WidgetEventArgs)`) is the single catch-all of a
  `Wisej.Web.Widget`: it only ever receives the `{Type, Data}` pairs the wrapper sent with
  `fireWidgetEvent`. `OnWebEvent(WisejEventArgs)` is the general event entry point of every control,
  shared with the framework (focus, resize, pointer … and the `widgetEvent` message itself); custom
  controls receive their `fireEvent` / `fireDataEvent` there when the server config wires the name.
  Because it is shared, an override must pass everything it does not own to `base.OnWebEvent(e)` —
  swallowing an unknown event breaks behavior unrelated to the integration. `ChartWidget` shows both:
  it handles `pointClicked` in `OnWebEvent`, calls base for the rest, and keeps `OnWidgetEvent` as a fallback.
- **What makes an event payload stable?**
  It is **small, named and versionable**: a name the integration chose, a handful of primitives the
  wrapper extracts (`index`, `label`, `value`), no vendor objects, DOM nodes or whole rows. A vendor
  upgrade that changes its event shape touches one function in one adapter, and a different library
  could produce the same object. The server treats every field as untrusted and validates it; a key in
  the payload is a lookup key, never proof of permission.

## Unverified at runtime (build passes; check in the browser)

- `ChartWidget.OnWebEvent`: the incoming `widgetEvent` parameters are read as `e.Parameters.Event = { type, data }`
  (derived from the client source: `fireWidgetEvent` → `fireDataEvent("widgetEvent", {type, data})`, wired as
  `widgetEvent(Event)`), with a flat `{ type, data }` fallback; if neither matches, the event goes to base and
  `OnWidgetEvent` handles it — the log line says which path ran.
- `CallAsync("getNoiseCount")` (`Noise counter`) returns the adapter's counter object as `dynamic`; the
  continuation runs in the `async void` click handler.
