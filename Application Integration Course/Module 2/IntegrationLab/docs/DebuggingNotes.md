# Deliverable 3 · Debugging notes — where the widget instance is stored

These notes are the first page of the troubleshooting guide for the Pressure knob. They record where the
vendor object lives, how that was verified, and how to read the failures the lab reproduces.

## Where the instance is stored

| Reference | Set by | Use it for |
|---|---|---|
| `this.instance` (on the Wisej.NET client widget) | `gauge-init.js`, step 5 | the adapter's own `update`, `dispose`, `pulse`, `getState`; what the course and the walkthrough show |
| `this.widget` (same object) | `gauge-init.js`, step 5 | the name Wisej.NET's server-side `Widget.Instance` proxy targets: `this.gaugeKnob.Instance.pulse()` reaches the vendor |
| `$(input).data("vendorKnob")` | the plugin itself (`$.fn.vendorKnob`) | the vendor's own convention; the adapter copies it to `this.instance` once, in `init` |
| `app.getWidget("gaugeKnob")` | the registry at the top of `gauge-init.js` | DevTools shortcut: returns the Wisej.NET widget wrapper; `.instance` is the vendor object, `.container` the host element |

The DOM tree under the widget, after `init`:

```
div  (this.container — owned by Wisej.NET; do not touch)
├─ input.knob            (created by the adapter, hidden by the plugin, holds the value)
└─ div.vendor-knob       (inserted by the plugin right after the input; the SVG dial)
   └─ svg
```

## How to confirm it (Chrome / Edge DevTools, F12)

**Console**

```js
app.getWidget("gaugeKnob")                     // › wisej.web.Widget {…}  — the wrapper
app.getWidget("gaugeKnob").instance            // › VendorKnob {input: input.knob, opts: {…}, value: 40, host: div.vendor-knob, …}
app.getWidget("gaugeKnob").instance.getValue() // › 40
app.getWidget("gaugeKnob").instance.setValue(72)   // dial moves, "knobchange" fires → valueChanged reaches .NET (the status shows ● 72 psi)
Object.keys(Object.getPrototypeOf(app.getWidget("gaugeKnob").instance))
                                               // › ["_build", "getValue", "setValue", "setOptions", "pulse", "resize", "destroy", "_draw"]
```

`app.getWidget` is a four-line registry added by the InitScript (Wisej.NET has none of its own). Two
built-in alternatives that need no registry:

```js
// 1. From the Elements panel: select the knob's host element, then in the Console
qx.ui.core.Widget.getWidgetByElement($0)       // the qooxdoo/Wisej widget that owns the selected element
// 2. From the server-side id shown in the Elements panel (id="id_…")
Wisej.Core.getComponent("id_…")
```

Logging the instance right after creation is the fastest check that it exists and which methods it exposes
— `console.log(this.instance)` after step 5 (remove before committing).

**Sources — `//# sourceURL=gauge-init.js`**

An InitScript is injected at runtime; without the comment it is an anonymous `VM123` script and no
breakpoint can be set on it by name. With `//# sourceURL=gauge-init.js` as the **last line**, the Sources
panel lists `gauge-init.js` (search with Ctrl+P) as if it were a real file; set breakpoints in it, step
through `init`, `update`, `_onKnobChange`.

**`debugger;`**

`gauge-init.js` has a commented-out `debugger;` at the top of `init`. Uncomment it, keep DevTools open,
reload: execution pauses before the vendor object exists. The Scope panel then shows exactly what the
walkthrough shows: `this` = the widget, `options` = the server Options as JSON, `this.container` = the
host element. Step over the `$(input).vendorKnob(...)` line and `this.instance` appears. Put a second
breakpoint inside `_onKnobChange` and turn the knob: the Scope shows `me` = widget and `this` =
`input.knob`, which is exactly why `this.fireWidgetEvent` would throw a `TypeError` there.
`debugger;` is a no-op when DevTools is closed, but it stops every page load when it is open — commit it
commented out.

**Elements**

Select the knob and confirm the tree above: the vendor's children are **inside** the widget's container,
not elsewhere in the document. The un-enhanced `<input class="knob">` is styled red/dashed by
`vendor-knob.css`, so if the plugin never ran you see it in the page.

**Network**

Filter by `wwwroot/`: `jquery-lite.js`, `vendor-knob.css`, `vendor-knob.js` — three `200`s, in that order,
loaded **once** per page (Wisej.NET caches packages by name, so another widget listing the same names
does not load them again).

**Server side**

`Widget.IsLoaded` tells whether the client widget has initialized: `this.gaugeKnob.IsLoaded` is `false`
until the packages have loaded and `init` has run.

## Symptom → cause → fix

| Symptom | Cause | Fix |
|---|---|---|
| Console: `TypeError: this.fireWidgetEvent is not a function` inside the `knobchange` handler; .NET never receives `valueChanged` (the status in the card does not move) | `this` inside a vendor callback is the vendor's choice (here the `<input>`), not the widget | capture `var me = this` before the callback and call `me.fireWidgetEvent(...)` (or `.bind(this)`, or route through a widget method) — `gauge-init.js` |
| Console at load time: `Uncaught ReferenceError: jQuery is not defined — vendor-knob.js must be loaded after jQuery.`; the dial never appears, a red dashed input is visible; later `$(...).vendorKnob is not a function`; red banner "Pressure knob: init failed — …" | the plugin ran before its library — Packages (or script tags) in the wrong order | list jQuery first, then the vendor CSS, then the vendor JS; the InitScript always runs last |
| Empty box, **no** error | the vendor was handed the wrong element (a `div` when it wants an `<input>`/`canvas`), or its CSS did not load | check the vendor sample for the element it enhances; create that element inside `this.container`; check the Network panel for the stylesheet |
| Event fires in the console but never reaches .NET, and it happened right after a server change | `fireWidgetEvent` called synchronously inside a vendor callback that ran during `update()` — dropped | apply server values silently (`setValue(v, true)` / `setOptions`) so they do not echo; if the vendor must raise the event, defer with `setTimeout(..., 0)` |
| Two copies of jQuery on the page, plugins registered on the wrong one | different package names for the same library across widgets, or the app already ships jQuery | one package name for one library, shared by every widget; check what the application already loads before adding a copy |
| Breakpoint cannot be set, script shows as `VM…` | InitScript injected at runtime without a name | `//# sourceURL=gauge-init.js` as the last line |
| Knob keeps working after the widget is removed; memory grows | vendor object not destroyed, listeners not removed | wrap `dispose`: `off("knobchange")`, `instance.destroy()`, remove the child element, then call the framework dispose |

## Evidence checklist for the review

- Every turn of the knob reaches .NET as `valueChanged`: the status in the card shows the new value.
- `app.getWidget("gaugeKnob").instance` returns the `VendorKnob` in the console; `gauge-init.js` is listed in Sources.
- The Network panel shows the three packages, once each, in load order.
