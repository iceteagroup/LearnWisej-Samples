# Client/server contract · `IntegrationLab.Widgets.TemperatureGauge`

Documented **before** the integration is relied on, as the lesson requires. Everything that
crosses the wire is data. Both directions can be logged (the Sensor Monitor window does this)
and compared against this page.

## Ownership

| Owner | Owns | Never touches |
|---|---|---|
| Server (`TemperatureGauge : Wisej.Web.Widget`) | `Value`, `Minimum`, `Maximum`, `WarnAt`, `Threshold`, `Label`, `Units`, validation, `.NET` events | the DOM |
| Client (`wwwroot/temperature-gauge.js`) | the host `<div>`, the `VendorGauge` instance, vendor event wiring, resize, destroy | the authoritative value |

## State out (server → client)

Rendered from `Widget.Options` as compact JSON. First render sends every field to `init(options)`;
later changes send only changed first-level fields to `update(options, old)`.

| Field | Type | Default | Meaning |
|---|---|---|---|
| `value` | number | 72 | current reading, must be within `min..max` (validated on the server) |
| `min` | number | 40 | gauge start |
| `max` | number | 120 | gauge end |
| `warnAt` | number | 85 | start of the "warm" band |
| `threshold` | number | 100 | start of the "high" band; rising-edge event source |
| `label` | string | "" | small caption drawn by the vendor |
| `units` | string | "°F" | suffix of the readout |

Example after `gauge.Value = 104;`

```json
{"value":104}
```

## Events in (client → server)

Registered in `WiredEvents` on the server. On the client the framework calls the adapter's
`_addListener(name, handler)` once per wired name; the adapter subscribes the vendor event and the
framework's handler defers the round-trip (next tick), calls `_getEventData(type, e)` for the
payload, and fires `widgetEvent` to the server. `OnWidgetEvent` then raises the matching .NET event.

Gotcha found while testing: calling `fireWidgetEvent` **synchronously** from inside a vendor
callback that runs during `update()` is silently dropped, because the client is still applying a
server update. Wiring through `_addListener` avoids that.

| Wire event | Payload | .NET event | When |
|---|---|---|---|
| `thresholdExceeded` | `{"value":104}` | `ThresholdExceeded(GaugeEventArgs)` | once, when the reading crosses `threshold` upward |
| `rangeChanged` | `{"range":"warm","value":88}` | `RangeChanged(GaugeEventArgs)` | when the band changes (normal / warm / high) |
| `error` | `{"phase":"update","message":"…"}` | `WidgetError(GaugeErrorEventArgs)` | the adapter caught a vendor exception |

Vendor event names (`thresholdexceeded`, `rangechange`) never leave the adapter; the adapter
translates them into the names above. The server uses **its own** `Value` when raising events
and logs a contract-check line if the client-reported value differs.

## Lifecycle

| Moment | Client adapter | Vendor call |
|---|---|---|
| init | creates `div.temperature-gauge-host` inside `this.container`, stores the instance in `this.widget` | `new VendorGauge(host, options)` |
| update | applies changed options | `setOptions({...})` → `setValue(v)` |
| resize | `ResizeObserver` on the host | `resize()` |
| dispose | destroys vendor, removes host, then calls the framework dispose | `destroy()` |

## Failure handling

| Failure | Where it is caught | What the user sees |
|---|---|---|
| Out-of-range `Value` (e.g. 150) | server property setter throws `ArgumentOutOfRangeException`; nothing is rendered | red banner + alert; gauge keeps the last good value |
| Malformed payload (`{"value":"n/a"}`) | adapter `update()` catches the vendor exception, fires one `error` event | red banner "Vendor failure during update…", status "fault" |
| Recovery | `ResyncFromServer()` re-renders the authoritative state | gauge redraws, banner clears |
| Vendor script missing | `init()` catches, fires `error {phase:"init"}` | banner, no page crash |

## Security boundary

The gauge accepts no user input: nothing in the event payloads is echoed into the DOM or
stored. Payload numbers are converted with `Convert.ToDouble` and never trusted as state.
