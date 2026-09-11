# Deliverable 2 — Client qx class skeleton

`Platform/SimpleGaugeControl.js` — `integrationlab.controls.SimpleGaugeControl extends wisej.web.Control`

## How it reaches the browser

- The file sits under `/Platform` and is an embedded resource (`<EmbeddedResource Include="Platform\*.js" />`),
  so its resource name is `IntegrationLab.Platform.SimpleGaugeControl.js`.
- `Properties/AssemblyInfo.cs` carries `[assembly: Wisej.Core.WisejResources]`: Wisej.NET bundles every
  embedded `/Platform/*.js` of the assembly into the client, so the class is defined before any page
  renders. No `Packages`, no `InitScript`, nothing per screen.
- `Platform/vendor-gauge.js` (the VendorGauge library) is embedded the same way, next to the class.
  Both files are written to survive concatenation (the vendor IIFE starts with `;`).
- The server selects the class with `config.className = "integrationlab.controls.SimpleGaugeControl"`.

## The skeleton

```js
qx.Class.define("integrationlab.controls.SimpleGaugeControl", {
  extend: wisej.web.Control,

  construct: function () {
    this.base(arguments);
    this.addListener("appear", this._createVendor, this);   // the DOM exists only after "appear"
    this.addListener("resize", this._onResize, this);
    this.addListener("changeTextColor", this._applyThemeColors, this);
  },

  properties: {
    appearance: { init: "simplegauge", refine: true },       // theme appearance key
    value:     { init: 0,   check: "Number", apply: "_applyValue" },
    minimum:   { init: 0,   check: "Number", apply: "_applyRange" },
    maximum:   { init: 100, check: "Number", apply: "_applyRange" },
    threshold: { init: 90,  check: "Number", apply: "_applyRange" },
    caption:   { init: "",  check: "String", apply: "_applyCaption" },
    units:     { init: "",  check: "String", apply: "_applyCaption" }
  },

  members: {
    __gauge: null,                                            // the vendor instance
    _createVendor:  function () { /* new VendorGauge(hostDiv, options) inside getContentElement().getDomElement() */ },
    _applyValue:    function (value) { if (this.__gauge) this.__gauge.setValue(value); },
    _applyRange:    function ()      { if (this.__gauge) this.__gauge.setOptions({ min, max, warnAt, threshold }); },
    _applyCaption:  function ()      { if (this.__gauge) this.__gauge.setOptions({ label, units }); }
  },

  destruct: function () { if (this.__gauge) { this.__gauge.destroy(); this.__gauge = null; } }
});
```

### Properties and apply methods

Every property mirrors one field the server writes in `OnWebRender`, with the same (camel-cased) name.
Wisej.NET sets the changed properties on the widget; qooxdoo runs the matching `apply` method, and
that is where the vendor call belongs — `_applyValue` calls `setValue`, `_applyRange` updates the
vendor options. Before the vendor exists (before "appear") the apply methods do nothing; the vendor
is created from the current property values, so nothing is lost.

### construct / appear / destruct

- `construct` only registers listeners. The content element has no DOM node yet.
- `_createVendor` runs on the first `appear`: it creates a child `<div>` inside
  `this.getContentElement().getDomElement()`, positioned inside the appearance padding, and hands
  it to `new VendorGauge(...)`. The framework keeps ownership of its own element; the vendor owns
  the child. Repeated `appear` events (hide/show, Designer re-layout) only call `resize()`.
- `destruct` destroys the vendor object and removes the child element, so a disposed control leaves
  no listeners or DOM behind.

### Events

The vendor's `thresholdexceeded` is forwarded as a qooxdoo data event:

```js
this.__gauge.on("thresholdexceeded", function (e) {
    setTimeout(function () { me.fireDataEvent("thresholdExceeded", { value: e.value, threshold: e.threshold }); }, 0);
});
```

Because the server wired `"thresholdExceeded(Data)"`, the Wisej core attaches a listener to that
event and sends `e.getData()` to the server as `e.Parameters.Data`. The one-tick deferral exists
because the vendor raises the event synchronously inside `setValue()`, i.e. while a server update
may still be being applied (the Module 1 gotcha).

### Theme

The class hard-codes no colours. `_readThemeColors()` resolves, through `qx.theme.manager.Color`:
the appearance's `textColor` (needle and readout), `simplegauge-accent` / `primary` (value arc) and
`simplegauge-track` / `windowFrame` (track). It re-applies them on `changeTextColor` and on the
theme manager's `changeTheme`, so switching the application theme restyles the gauge together with
every built-in control.

## Why it must stay free of application logic

The class translates between the framework and the vendor, and nothing else: no thresholds decided
in JavaScript, no alerts, no formatting rules, no knowledge of boilers or turbines. That is what lets
one client class serve every server control that renders to it (four tiles here, any number of
screens later), keeps the wire protocol equal to "the declared properties + the wired events", and
keeps the server the only place where state is validated and decisions are made.

## Evidence (running app)

- The gauges draw on page load with no per-page script; the browser console shows no errors.
- The dashboard streams from page load: needles move as `value` applies; the vendor is never re-created.
- Theme: with `"theme"` in `Default.json` set to `Material-3` or `FluentDark-5` (or after `Application.LoadTheme(...)`),
  background, border, text and accent of all four gauges change with the theme.
