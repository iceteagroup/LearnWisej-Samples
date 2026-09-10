# Deliverable 2 — the widget wrapper: `WorkOrderChartWidget`

`EnterpriseOps.Widgets.WorkOrderChartWidget : Wisej.Web.Widget` — a stable C# API in front of a third-party
JavaScript chart.

Files: `Widgets/WorkOrderChartWidget.cs`, `Widgets/ChartSegment.cs`, `Widgets/opschart-init.js` (the client
adapter, embedded), `Widgets/vendor-opschart.js` (the "vendor" library), `Widgets/opschart.css`.

> **Naming note.** The walkthrough video's solution tree shows the vendor library as
> `Resources/chart.widget.js` and the component stylesheet as `Resources/timeline.css`. This sample follows
> the course cookbook's folder-per-layer convention instead — `Widgets/vendor-opschart.js` and
> `Widgets/opschart.css` — so the vendor name and the version are visible in the file name, which is the
> packaging rule the lesson actually asks for. Everything else (class names, event names, the failure path)
> matches the video.

## The three layers

```
WorkOrderHistoryPage          C#   knows SetSegments / Palette / SegmentClicked / WidgetError
        │
WorkOrderChartWidget          C#   knows Options field names, WiredEvents, the package list, validation
        │  (Options ↓ / WidgetEvent ↑)
opschart-init.js  (adapter)   JS   knows the vendor's constructor, its option names and its event names
        │
EnterpriseOpsChart 1.2        JS   knows nothing about Wisej.NET, .NET, or work orders
```

Each layer knows only the layer directly below it. A vendor upgrade changes the bottom two rows; a vendor
**replacement** changes the bottom two rows. The screen never changes — that is what "wrap behavior, not
just visuals" buys.

## The public API

Named after what the *application* wants, never after a vendor function.

| Member | What it is for |
|---|---|
| `SetSegments(params ChartSegment[])` / `SetSegments(IEnumerable<ChartSegment>)` | Replace the whole breakdown. Validates on the server (at least one segment, non-empty keys, no negative values, no duplicate keys) before anything crosses. |
| `Caption` | The uppercase caption drawn above the bar. |
| `Palette` | `ops` \| `mono` \| `highcontrast`. The set is **closed on the server**: anything else throws `ArgumentOutOfRangeException` and nothing is rendered. |
| `ShowLegend` | Whether the legend is drawn. |
| `SampleMode` | Design-time sample data (on automatically in the Designer). Never calls a service. |
| `SelectedKey` (read-only) / `Select(key)` | The highlighted slice. `Select` moves the highlight without raising `SegmentClicked`. |
| `IsFallbackRendered`, `VendorVersion`, `DescribeWirePayload()` | Diagnostics the lab screen prints. |
| `SegmentClicked` | **Named server event**: the user clicked a slice. `ChartSegmentEventArgs { Key, Label, Value, Percent }`. |
| `WidgetError` | The adapter caught a vendor failure and already fell back. `WidgetErrorEventArgs { Phase, Message, FallbackRendered }`. |
| `Trace` | Diagnostics only: every option that went down, every event that came up. |

### What is deliberately hidden

`Packages`, `InitScript`, `WiredEvents`, `Options`, `Call`, `CallAsync` are shadowed read-only with
`[Browsable(false)]`, `[EditorBrowsable(Never)]` and `[DesignerSerializationVisibility(Hidden)]`. They are
exactly what the prototype edits on every page (`Controls/Samples/LeakyChartScreen.cs.txt`); once the class
owns the setup, letting one screen change them breaks that screen only — the worst kind of bug. The class
itself always goes through `base.`.

`SimulateBlockedVendor` and `SimulateVendorFailure()` are diagnostics-only members for the lab's failure
buttons, kept out of the Properties window.

## The client adapter (`opschart-init.js`)

The rules it follows, all of them verified in the Application Integration course samples:

1. The vendor object lives in a **child element** (`this.host`), never on `this.container` — the framework
   owns `container`.
2. The vendor instance is stored in `this.widget`, so `Instance.xxx()` reaches it.
3. Events go through `_addListener` / `_removeListener` / `_getEventData`. The framework registers one
   handler per `WiredEvents` entry and defers the round trip itself; a `fireWidgetEvent` raised
   *synchronously inside* `update()` would be dropped, so `_reportError` always defers with `setTimeout`.
4. Every vendor call is inside `try/catch`. A vendor failure becomes the `error` event plus the embedded
   fallback — never an exception thrown at the screen.
5. `dispose` is **wrapped**, not replaced: disconnect the `ResizeObserver`, destroy the vendor, remove the
   host, then call the framework's original `dispose`.
6. The adapter never defines `getWidth`, `getHeight`, `destroy`, `resize`, `show`, `hide` or `getValue` —
   those belong to the framework wrapper.

## Server-side validation, twice

The wrapper validates on the way **down** (an option the component does not support never reaches the
browser) and on the way **up** (`OnWidgetEvent` drops a `pointSelected` whose key is not one this widget
rendered, before any screen handler runs). The service validates the key a **third** time
(`StatusGroup.IsKnown`) because a screen could call it with anything.

That is not paranoia: the browser is the one part of the system the server does not control.

## Evidence — what the running app shows

| Do this | You should see |
|---|---|
| Open the page | A stacked bar: Open 38 · On hold 22 · Escalated 12 · Done 28 (the walkthrough's numbers, computed by the service for tenant `fabrikam`). The trace prints the wire payload — four `{key,label,value}` objects and nothing else. |
| Click the red **Escalated** slice | Footer: `SegmentClicked("escalated") — named server event · grid filtered server-side · 12 rows`. The trace shows `Client → WorkOrderChartWidget.SegmentClicked → key 'escalated' (12, 12%) — a key, not a work order`, then the service query. |
| **Fail: invalid palette** | Red banner *invalid palette refused on the server — nothing was sent to the browser*; the trace has `Component: server rejected palette 'neon-pink'…`. The chart on screen is unchanged. |
| **Fail: vendor throws** | The adapter hands the vendor `segments: null`, the vendor throws `EnterpriseOpsChart: options.segments must be an array…`, the adapter catches it, renders the fallback list and raises `error`. Amber banner + toast; the timeline and the grid keep working. |
| **Fail: block the vendor script** | The walkthrough's proxy block: no vendor object, `error` with phase `init`, the embedded fallback list with the same numbers, footer `WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works`. |
| **Recover: reload the chart** | The same wrapper, the same API, the chart back with the same data. |
