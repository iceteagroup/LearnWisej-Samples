# Deliverable 4 — Design-time notes (screenshot to be taken in Visual Studio)

Design-time rendering is the final test of a custom control: if it renders correctly on the design
surface it gets adopted; if it throws or shows a grey box, it gets avoided.

## What the Designer instantiates

The Wisej Designer loads the **built** `IntegrationLab.dll`, creates a real
`IntegrationLab.Controls.SimpleGaugeControl` for `simpleGaugeControl1` and renders it through the
same pipeline as at runtime: `OnWebRender` → `config` → the client class
`integrationlab.controls.SimpleGaugeControl` in an embedded browser, themed with the project's
theme and mixins. There is no separate "design renderer" to write; there is a design-mode flag on
both sides.

## Sample data on the server

`IsDesignMode()` returns true when either `Site.DesignMode` (the .NET component site) or
`IWisejComponent.DesignMode` (the Wisej.NET flag) is set. In that case `OnWebRender` writes:

```json
{"className":"integrationlab.controls.SimpleGaugeControl","appearance":"simplegauge",
 "value":93,"minimum":0,"maximum":150,"threshold":120,"caption":"SimpleGauge (design)","units":" psi",
 "wiredEvents":["thresholdExceeded(Data)"]}
```

- `value` is 62 % of the scale (`DesignTimeSampleValue()`), so the needle, the value arc and the
  readout are all visible even though a freshly dropped control has `Value = 0`.
- `caption` falls back to `"SimpleGauge (design)"` when `Caption` is empty.
- Everything else is the real property state, so what the developer sets in the Properties window
  (range, threshold, units) is what the preview shows. Nothing design-only leaks into runtime: the
  sample value is never stored in `_value`.

The **Design-time notes** button on the dashboard prints this JSON for the Boiler 1 tile at
runtime (`GetDesignTimeConfigJson()`), so the reviewer can see the design-mode output without
opening Visual Studio.

## Coping on the client

- No server round trip is needed to draw: the vendor is created on the first `appear` from the
  property values already in `config`.
- Placeholder data is fine: the vendor validates only `min < max` and a finite `value`, both
  guaranteed by the server setters.
- Repeated creation: dragging or resizing the control on the design surface fires `resize` (vendor
  `resize()`) and may re-fire `appear` (guarded: no second vendor instance). When the Designer
  re-creates the widget, `destruct` destroys the vendor and removes its element.
- Wired events are not attached in design mode (`wisej.web.DesignMode` is true), so the
  `thresholdExceeded` listener is never registered on the design surface.
- Theme: the design surface uses the same theme and the `simplegauge.mixin.theme` mixin, so the
  preview matches runtime styling.

## Designer assembly caching and restart discipline

Visual Studio loads the control assembly into the Designer process and **caches** it. After you change
`SimpleGaugeControl.cs`, `Platform/SimpleGaugeControl.js`, `Platform/vendor-gauge.js` or the theme
mixin:

1. Rebuild the project (`Build → Rebuild Solution` or `dotnet build`).
2. Close and reopen `EnterprisePage.cs [Design]`.
3. If the preview still shows the old behaviour (typical after changing the client class or the
   mixin), restart Visual Studio: the embedded browser and the loaded assembly are recycled only on
   restart.
4. A control that throws in `OnWebRender` shows an error box on the surface; fix, rebuild, reopen.

Never work around a stale Designer by editing `EnterprisePage.Designer.cs` by hand while the
surface is open — the Designer will overwrite it.

## Screenshot (to be taken by the learner)

Open `IntegrationLab.slnx` in Visual Studio, open `EnterprisePage.cs` in the Designer, select
`gaugeBoiler1` and capture:

- the four gauge tiles rendered with needle, arc and readout (not grey boxes);
- the Properties window showing `Caption`, `Maximum`, `Minimum`, `Threshold`, `Units`, `Value` and
  the `ThresholdExceeded` event;
- optionally, the Toolbox entry for `SimpleGaugeControl` (`[ToolboxItem(true)]`).

Save it as `docs/DesignTime.png` next to this file.
