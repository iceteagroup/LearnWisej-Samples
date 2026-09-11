# Deliverable 1 — Three client event handlers

Module 7 · Events & Handler Contracts. Each adapter is a Wisej.NET `InitScript` (embedded
resource, `wwwroot/*-init.js`) whose `this` is the `wisej.web.Widget` wrapper. Each one wires
**one** vendor callback for the event the application cares about, decides in the browser when
that event is meaningful, and forwards a **compact payload** — never the vendor event object.

| # | Adapter | Vendor | Vendor callbacks subscribed | Forwarded to .NET (contract name + payload) |
|---|---|---|---|---|
| 1 | `wwwroot/gauge-init.js` | `VendorGauge` (`vendor-gauge.js`) | `rangechange` (one) | `thresholdCrossed { value, level }` — rising edge, once per crossing |
| 2 | `wwwroot/knob-init.js` | `VendorKnob` jQuery plugin (`jquery-lite.js` + `vendor-knob.js`) | `knobchange` DOM event on the plugin's `<input>` (one) | `valueChanged { value, source }` |
| 3 | `wwwroot/chart-init.js` | `VendorChart` (`vendor-chart.js` + `vendor-chart.css`) | `pointclick` (one) | `pointClicked { index, label, value }` |

## Handler 1 — gauge: `thresholdCrossed` (a business decision)

```js
this._wire = function () {                       // ONE vendor callback, attached in one place
    var me = this;                               // keep the widget: vendor "this" is not the widget
    this._onVendorRange = function (e) { me._onChange(e.value); };
    this.widget.on("rangechange", this._onVendorRange);
};

this._onChange = function (v) {                  // the client decides when it is meaningful
    var level = null;
    if (v >= this._threshold)      { if (!this.above)  level = "high"; this.above = true; this.warned = true; }
    else if (v >= this._warnAt)    { if (!this.warned) level = "warn"; this.above = false; this.warned = true; }
    else                           { this.above = false; this.warned = false; }
    if (level) this._raise("thresholdCrossed", { value: v, level: level });   // once per crossing
};
```

- `above` / `warned` are the lesson's edge flags: a reading that stays above the line produces
  nothing, a reading that falls back resets them, one change crosses at most one line → **one event**.
- The vendor also offers `thresholdexceeded`; the adapter ignores it and derives its own edge, so the
  payload would be identical with a different gauge library (review question 2).
- **Timing.** The crossing is caused by a *server* change (`gauge.Value = 104` → `update()` →
  `setOptions` → the vendor fires `rangechange` synchronously inside `update()`). A synchronous
  `fireWidgetEvent` there is dropped (verified in Module 1), so `_raise` hands the payload to the
  handler the framework registered through `_addListener` — `wisej.web.Widget._onEvent`, which
  calls `_getEventData(type, e)` and `fireWidgetEvent` on the next tick.

## Handler 2 — knob: `valueChanged` (server-owned state changed)

```js
this._wire = function () {
    var me = this;
    this._onKnobChange = function (e) {          // the plugin dispatches "knobchange" on its <input>
        var value = e.detail.value;
        if (me._applying) {                      // we are inside update(): a server Options change echoed back
            me._raise("valueChanged", { value: value, source: "server" });   // deferred path
            return;
        }
        me.fireWidgetEvent("valueChanged", { value: value, source: "user" });  // user drag: direct is fine
    };
    $(this.input).on("knobchange", this._onKnobChange);
};
```

- One vendor callback, two sources. `update()` sets `me._applying = true` around
  `widget.setValue(options.value)`, so the adapter can tell an echo of server state from a user gesture.
- User-driven events are never dropped; the video's direct `me.fireWidgetEvent(...)` is used as-is.
- Wheel steps, pointer capture, the dial redraw and `pulse()` stay in the browser.

## Handler 3 — chart: `pointClicked` (user drill-down intent)

```js
this._wire = function () {                       // ONE vendor callback, attached in one place
    var me = this;
    this._onPointClick = function (e) {          // the only business event
        me.fireWidgetEvent("pointClicked", { index: e.index, label: e.label, value: e.value });
    };
    this.widget.on("pointclick", this._onPointClick);
};
```

The vendor `pointclick` event carries `{ seriesIndex, index, label, value, x, y, domEvent }`; the
contract keeps three primitives. The chart's other five vendor events are never subscribed, so they
stay in the browser.

### Noise table — what the chart vendor fires vs what reaches .NET

| Vendor event | Fires when | Forwarded? | Why |
|---|---|---|---|
| `hover` | every pointer move over the plot (per visible series) | no | purely visual; dozens per second |
| `zoom` | mouse wheel over the plot | no | browser-side view state, not server state |
| `render` | after every redraw (data, zoom, legend, resize) | no | an implementation detail of the vendor |
| `layout` | after `resize()` | no | the framework already owns layout |
| `legendclick` | a legend entry toggled | no | local display preference |
| `pointclick` | a point circle clicked | **yes → `pointClicked`** | a user drill-down intent the server must act on |

### Re-wiring after destroy / recreate

`VendorChart` cannot change `theme` after construction (`setOptions({theme})` throws). The
adapter's `update()` therefore destroys the instance and creates a new one — and calls the **same
`_wire()`** it called from `init`:

```js
this._createVendor = function (options) {        // create AND wire — the only place both happen
    this.widget = new VendorChart(this.host, { series: options.series, labels: options.labels, theme: options.theme });
    this._theme = options.theme;
    this._wire();
};
this.update = function (options, old) {
    if (options.theme !== this._theme) { this._destroyVendor(); this._createVendor(options); return; }
    this.widget.setData({ series: options.series, labels: options.labels });
};
```

Attaching in `init` only would leave the second instance silent. Set `chart.Theme = "dark"` (in the
Designer or from code) and a point click still arrives: the recreate path ran the same `_wire()`.

### Detach on dispose

Every adapter wraps the framework `dispose` and runs `_unwire()` (`off` / jQuery `.off`) before
`widget.destroy()`, so no vendor handler outlives its widget (the slow-browser leak the lesson warns about).

## Evidence (what the running app shows)

- The gauge follows live readings (72 → 104 → 72, one every 700 ms). Each cycle the log shows exactly
  one `← JS→.NET thresholdCrossed e.Data = {"value":85,"level":"warn"}` and one
  `{"value":102,"level":"high"}`, not one line per reading.
- Dragging the knob → `← JS→.NET valueChanged e.Data = {"value":…,"source":"user"}` and the dial pulses.
- Clicking a chart point → `← JS→.NET pointClicked e.Data = {"index":3,"label":"Apr","value":68}`;
  hover, wheel-zoom and legend clicks add nothing to the log.
