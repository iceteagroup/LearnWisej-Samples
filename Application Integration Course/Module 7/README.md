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

| Widget | What happens | What you should see in the log |
|---|---|---|
| **Gauge** | follows live boiler readings (72 → 104 → 72, one every 700 ms) set by the server | once per cycle `← JS→.NET thresholdCrossed e.Data = {"value":85,"level":"warn"}` and `{"value":102,"level":"high"}` — one event per crossing, not per reading; the high crossing raises an "operator notified" toast |
| **Knob** | drag the dial | `← JS→.NET valueChanged e.Data = {"value":…,"source":"user"}` per value step; the server commits the value and the dial pulses (`Call("pulse")`) |
| **Chart** | click a point | `← JS→.NET pointClicked e.Data = {"index":3,"label":"Apr","value":68}` and a drill-down toast |
| **Chart** | hover, wheel-zoom, click the legend | nothing — those vendor events stay in the browser |

A payload that fails server validation is logged as `✖ rejected …` with the reason, and no .NET
event is raised.

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
│  ├─ ChartWidget.cs               : Widget — OnWebEvent (+ base call) → typed PointClicked
│  ├─ PayloadReader.cs             guarded reads of untrusted dynamic fields
│  └─ TraceEventArgs.cs            log lines for the WidgetEvent log
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
  meaningful (rising edge, once), and sends a small payload.
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
  `OnWidgetEvent` handles it with the same validation.
