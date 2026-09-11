# Deliverable 2 · Context-safe Wisej.NET InitScript

**File:** `wwwroot/gauge-init.js` (embedded resource `IntegrationLab.wwwroot.gauge-init.js`, assigned to
`gaugeKnob.InitScript` in `Window1.cs`). The difference between a broken and a working script is
one line, shown below.

## The bug, in one sentence

Inside the functions of an InitScript `this` is the Wisej.NET client widget, so `this.container` and
`this.fireWidgetEvent` exist. Inside a **vendor callback** `this` is whatever the vendor decided — for
VendorKnob it is the `<input>` element that dispatched `knobchange`. Code written as if the widget were
still in scope compiles, runs, and fails at the first callback:

```
TypeError: this.fireWidgetEvent is not a function
```

Third-party libraries are outside the Wisej.NET contract; they never preserve your object context.

## Three ways to keep the context

| Way | Code | When to use it |
|---|---|---|
| **Closure** (used here) | `var me = this;` before the callback, then `me.fireWidgetEvent(...)` inside it | The default. Simplest to read in review; works for timers, promises and fetch callbacks too — only a captured reference survives until they run. |
| **Bind** | `this._onKnobChange = function (e) { this.fireWidgetEvent(...) }.bind(this);` | When the handler is declared elsewhere (a method, a shared helper) and cannot see a local variable. `bind` returns a new function; keep that reference if you need to `off()` it later. |
| **Route through a method** | `$(input).on("knobchange", function (e) { me.onKnobChange(e); });` with `this.onKnobChange = function (e) { this.fireWidgetEvent(...) }` | When several vendor events should share widget-side logic (validation, throttling, translation of payloads). The tiny closure only forwards; the method has the right `this` because it is called as `me.onKnobChange(...)`. |

Arrow functions would also inherit `this`, but the course keeps InitScripts in ES5 style so they read
the same as the vendor samples they are translated from.

## Annotated copy of `gauge-init.js`

```js
window.app = window.app || {};                 // DevTools helper only: app.getWidget("gaugeKnob")
app.widgets = app.widgets || {};
app.getWidget = app.getWidget || function (name) { return app.widgets[name] || null; };

this.init = function (options) {
    var me = this;                             // (1) capture the widget BEFORE any vendor call

    // debugger;                               // (2) uncomment to pause here with DevTools open

    this.container.innerHTML = "<input class='knob'/>";   // (3) the element the vendor wants,
    var input = this.container.firstChild;                //     created INSIDE the container
    input.placeholder = "raw <input> - plugin not applied";
    this.inputEl = input;

    try {                                                  // (4) apply the plugin
        $(input).vendorKnob({ value: options.value, min: options.min, max: options.max, step: options.step,
                              label: options.label, units: options.units, color: options.color });
    }
    catch (ex) { this.instance = this.widget = null; this._reportError("init", ex.message); return; }

    this.instance = $(input).data("vendorKnob");           // (5) the vendor instance lives ON THE WIDGET
    this.widget = this.instance;                           //     alias for Wisej.NET's Instance proxy

    var registryName = options.name || (typeof this.getName === "function" ? this.getName() : null) || "widget";
    this._registryName = registryName;
    app.widgets[registryName] = this;                      //     DevTools: app.getWidget("gaugeKnob")

    this._onKnobChange = function (e) {                    // (6) vendor event → widget event
        me.fireWidgetEvent("valueChanged", { value: e.detail.value });   // "me", never "this"
    };
    $(input).on("knobchange", this._onKnobChange);

    if (typeof ResizeObserver !== "undefined") {           // (7) resize → vendor
        this._resizeObserver = new ResizeObserver(function () { if (me.instance) me.instance.resize(); });
        this._resizeObserver.observe(this.container);
    }

    var frameworkDispose = this.dispose;                   // (8) wrap, never replace, dispose
    this.dispose = function () {
        try {
            if (me._resizeObserver) { me._resizeObserver.disconnect(); me._resizeObserver = null; }
            if (me.inputEl) $(me.inputEl).off("knobchange", me._onKnobChange);
            if (me.instance) { me.instance.destroy(); me.instance = null; me.widget = null; }
            if (me.inputEl && me.inputEl.parentNode) me.inputEl.parentNode.removeChild(me.inputEl);
            me.inputEl = null;
            if (me._registryName && app.widgets[me._registryName] === me) delete app.widgets[me._registryName];
        }
        finally { if (typeof frameworkDispose === "function") frameworkDispose.apply(me, arguments); }
    };
};

this.update = function (options, old) {                    // (9) server Options changed → re-sync only the diff
    if (!this.instance) return;
    var changed = {}, keys = ["value", "min", "max", "step", "label", "units", "color"];
    for (var i = 0; i < keys.length; i++) {
        var k = keys[i];
        if (options[k] === undefined) continue;
        if (old && old[k] === options[k]) continue;
        changed[k] = options[k];
    }
    try {
        if (changed.value !== undefined && Object.keys(changed).length === 1) this.instance.setValue(changed.value, true);
        else this.instance.setOptions(changed);              // both apply silently: no knobchange, no echo
    }
    catch (ex) { this._reportError("update", ex.message); }
};

this._addListener = function (name, handler) { };          // (10) events are fired directly (see 6)
this._removeListener = function (name, handler) { };

this._reportError = function (phase, message) {            // (11) one contract event per caught failure,
    var me = this;                                         //      deferred so it survives update()
    setTimeout(function () { me.fireWidgetEvent("error", { phase: phase, message: message }); }, 0);
};

this.pulse = function () { if (this.instance) this.instance.pulse(); };     // reachable with Call("pulse")
this.getState = function () { return { value: this.instance ? this.instance.getValue() : null, hasInstance: !!this.instance }; };

//# sourceURL=gauge-init.js                                // (12) names the injected script in Sources
```

Notes on the numbered points:

1. `me` is the **only** reference to the widget that survives into (6), (7), (8) and (11).
2. See `docs/DebuggingNotes.md`.
3. See "Why `this.container.innerHTML`" below.
4. Errors from the vendor are caught and reported as one `error` event; the page stays alive.
5. `this.instance` is what the course and the walkthrough call it; `this.widget` is the name Wisej.NET's
   server-side `Instance` proxy targets, so `this.gaugeKnob.Instance.pulse()` would reach the vendor
   without further changes (not exercised in this lab).
6. The event is caused by the user (drag / wheel), so a synchronous `fireWidgetEvent` is correct. An
   event caused by a **server** update must be deferred (cookbook gotcha) — which is why (9) applies
   values silently and never triggers (6).
9. Wisej.NET passes the full Options object and the previous one; only changed, defined fields go to
   the vendor.
10. Wisej.NET calls `_addListener` once per `WiredEvents` entry. This adapter does not need the framework's
    deferred dispatcher because its only user-driven event is fired directly; the empty overrides document
    that choice. `WiredEvents` on the server still lists `valueChanged` and `error` as the documented
    contract.

## The broken version of step (6)

```js
this._onKnobChange = function (e) {
    this.fireWidgetEvent("valueChanged", { value: e.detail.value });   // ✕ this = <input>
};
```

It runs, and fails at the first turn of the knob with `TypeError: this.fireWidgetEvent is not a
function` in the console; .NET simply never hears about the value. The plain-JS proof page
(`wwwroot/proof/knob-proof.html`) wires the callback both ways so the difference can be seen.

## Why `this.container.innerHTML = "<input class='knob'/>"`

The safe host for any vendor object is `this.container`: the element the Wisej.NET widget reserves for
content. VendorKnob does not accept a generic block element — it **enhances an `<input>`** (hides it, draws
the dial next to it, stores its value in it, dispatches `knobchange` on it). So the adapter creates that
`<input>` *inside* the container and hands only the child to the vendor. What must never happen:

- mutating the widget's own root element (classes, attributes, replacing it) — the framework relies on it
  for layout, visibility, theming and disposal;
- appending the vendor's element anywhere else in the document (a global overlay, `document.body`) — it
  would outlive the widget and escape Wisej.NET's disposal.

Review test that passes for this file: search the InitScript for anything other than `this.container`
(or a child created inside it) being handed to the vendor — there is none. `innerHTML` is used
deliberately instead of `createElement` because the lesson shows it and because it is the shortest way to
state "the container's content is entirely mine"; the framework owns the container, the adapter owns
everything inside it. The dispose wrapper removes the child again.

## Load order (Packages)

`Window1.Designer.cs` declares, for `gaugeKnob`:

| # | Package name | Source | Why here |
|---|---|---|---|
| 1 | `jquery-lite` | `wwwroot/jquery-lite.js` | the plugin registers itself on `$.fn`; must be a global before the plugin runs |
| 2 | `vendor-knob-css` | `wwwroot/vendor-knob.css` | before the widget paints; after the library's own base CSS if there were one |
| 3 | `vendor-knob` | `wwwroot/vendor-knob.js` | defines `$.fn.vendorKnob`; throws `ReferenceError: jQuery is not defined` if 1 is missing |
| – | InitScript | `gauge-init.js` | runs last, uses `$` and `$.fn.vendorKnob` |

Wisej.NET loads packages in list order and caches them **by name**, so a second widget listing the same
names does not load them again. Listing `vendor-knob.js` before `jquery-lite.js` on a fresh page gives
`ReferenceError: jQuery is not defined` in the console at load time and the adapter's `init` reports
`error {phase:"init"}`. Checklist from the lesson, applied:

- every file the vendor sample includes is listed, in the order it includes them;
- jQuery is loaded once for the page (shared package name across widgets);
- versions are pinned in the path in real projects (`jquery-3.7.1.min.js`), not here, because the course
  ships stand-in libraries.

## Evidence (what the running app shows)

- Turn the knob: `valueChanged {"value":…}` reaches .NET and the status in the card shows the value
  the server recorded (`● 72 psi`); the server writes it back to `Options.value` and `update()` applies it
  silently, so there is no echo.
- A vendor failure caught in `init` / `update` arrives as `error {phase, message}`: red banner, status
  `● fault`.
