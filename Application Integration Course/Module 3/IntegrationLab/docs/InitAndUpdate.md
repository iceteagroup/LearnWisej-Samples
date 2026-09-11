# Deliverable 2 · InitScript and update implementation

Where: `wwwroot/gauge-init.js`, `wwwroot/knob-init.js` (embedded resources, assigned to
`Widget.InitScript` in `DashboardPage.Designer.cs`).

The InitScript runs with `this` = the Wisej.NET wrapper (`wisej.web.Widget`). `this.container`
is the element the framework owns. The framework calls, in order:
packages load → `init(options)` → "loaded" → `_addListener(name, handler)` once per
`WiredEvents` entry → later `update(options, old)` on every first-level `Options` change.

## init(options) — runs once

```js
this.init = function (options) {
    var me = this;
    this.container.innerHTML = "<div class='knob-host'></div>";        // the element the plugin wants
    var host = this.container.firstChild;
    var input = document.createElement("input");                        // a jQuery plugin enhances an <input>
    host.appendChild(input);

    $(input).vendorKnob(this._toVendorOptions(options));                // create the vendor object …
    this.instance = $(input).data("vendorKnob");                        // … store it somewhere predictable
    this.widget = this.instance;                                        // Wisej.NET convention (Instance.xxx() targets this.widget)

    $(input).on("knobchange", function (e) {                            // vendor event → contract event
        me.fireWidgetEvent("valueChanged", { value: e.detail.value });  // user-driven: synchronous is fine
    });
    // ResizeObserver → instance.resize();  dispose wrapper → instance.destroy()  (see the file)
};
```

The gauge does the same with a plain constructor (`new VendorGauge(host, …)`), inside a child
`div.gauge-host` — never on `this.container` itself, so the framework keeps ownership of its own
element. Both adapters wrap (never replace) `this.dispose` to destroy the vendor object and remove
the host before the framework disposes the wrapper.

## update(options, old) — runs every time .NET changes Options

Called after the first render whenever the server assigns a **first-level** field of `Options`.
It receives the full new options and the previous ones. Both adapters compare against `old` and
push only what changed:

```js
this.update = function (options, old) {
    if (!this.widget) return;
    old = old || {};
    // 1. the option the vendor cannot take after construction → destroy & recreate (gauge only)
    if (options.style !== old.style) { this._recreate(options, "style changed"); return; }

    // 2. options with a clean setOptions path — only the changed ones
    var changes = {};
    if (!this._same(options.range, old.range)) { changes.min = options.range.minValue; changes.max = options.range.maxValue; }
    if (!this._same(options.label, old.label)) { changes.label = options.label.text; changes.units = options.label.units; }
    if (!this._same(options.bands, old.bands)) { /* bands → warnAt / threshold, redraw the legend */ }
    if (Object.keys(changes).length) this.widget.setOptions(changes);

    // 3. the frequently changing top-level value
    if (options.value !== old.value) this.widget.setValue(options.value);
};
```

Why the comparison matters:

- **cheap** — Set "High" changes only `value` / `level`; the bands are not re-sent to the vendor, no
  legend is redrawn, no animation restarts for an unrelated option;
- **nested objects compare as whole values** — `_same` is a JSON comparison, so replacing the
  `bands` array (Set "Peak" / Set "Idle") re-syncs the bands even if one color changed, exactly
  as the lesson describes; that is why the frequently changing reading sits at the top level;
- **recreation happens only for the one option that needs it, and only when it changed.**

The knob sets its value **silently** (`setValue(level, true)`): a server-driven change must not
echo back as `valueChanged`. Only the user's drag / wheel raises that event.

## Vendor-specific translation stays in the adapter

`VendorGauge` has no bands API; it knows `warnAt` and `threshold`. The adapter translates:
`warnAt` = start of the second band, `threshold` = start of the last band (sorted by `from`), and
draws the band colors itself as a legend (`div.gauge-bands`). The page never learns those vendor
names. Likewise the knob's contract option is `level`; the vendor calls it `value`.

## Destroy and recreate

`options.style` (`"card"` / `"compact"`) selects the host element the gauge is bound to: the
compact style is a different DOM structure (dark, padded host). `VendorGauge` binds to its element
in the constructor and has no re-parent / `setElement` API — `resize()` only redraws into the
element it was given. So the adapter makes the vendor's limitation explicit:

```js
this._recreate = function (options) {
    disconnect the ResizeObserver; this.widget.destroy(); remove the old frame;
    this._buildHost(options.style);
    this.widget = new VendorGauge(this.host, this._toVendorOptions(options));   // built from the FULL options
    this._renderBands(options.bands);
    re-observe;
};
```

The dashboard renders the gauge with `style = "card"` (set in `DashboardPage.Designer.cs`); assigning
`gauge.Options.style = "compact"` from the server rebuilds the gauge inside a dark, padded host, with
every other option re-applied from the full `options` object.

## Nested options: the notify rule

The `Widget` detects changes to **first-level fields only**. `Options.bands[0].color = "#1a86ff"`
changes the server copy but renders nothing — the client never gets `update()`. That is why the
server buttons replace the whole `bands` array instead of editing a band in place. When a nested
change is unavoidable, `gauge.Update()` re-renders the Options (the client's `_same` comparison then
finds the changed band), and `((dynamic)Options).Notify("bands")` is the targeted alternative the
Wisej.NET docs offer.

## Events and errors

- `WiredEvents` lists what each adapter may raise: gauge `error`; knob `valueChanged`, `error`.
  Every one lands in `widget.WidgetEvent` on the server (`e.Type`, `e.Data`).
- Adapter-raised events go through the framework handler registered by `_addListener` when one
  exists (it defers the round trip), otherwise through `setTimeout(fireWidgetEvent, 0)` — an event
  fired synchronously while the framework is still applying a server render is dropped.
- Every vendor call in `init` / `update` is wrapped in `try/catch` and reported as one
  `error {phase, message}` event; the page stays alive and the banner says what failed.

## Evidence

| Action | Trace shows |
|---|---|
| page load | `→ .NET→JS gauge init(options) {"value":72,"range":{"minValue":0,"maxValue":120},"bands":[…],"label":{…},"style":"card"}` and the same for the knob |
| Set "High" | one `update()` per widget; gauge needle moves to 88, knob to 78, legend untouched |
| Set "Peak" | bands array replaced → legend redrawn (95–120 red), needle at 104 |
| Set "Idle" | default bands back, needle at 64, knob at 40 |
| drag the knob | `← JS→.NET knob.valueChanged {"value":63}`; the server writes the value back to `Options.level` |
