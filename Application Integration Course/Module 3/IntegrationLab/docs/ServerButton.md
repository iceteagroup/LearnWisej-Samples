# Deliverable 3 · Server button that changes widget options and calls the client

Where: `DashboardPage.cs` (`ApplySetPoint`, `QueueCall`, the `button*_Click` handlers),
`DashboardPage.Designer.cs` (the server toolbar above the widgets).

## The buttons

| Button | Options changed | Client Call | What you see |
|---|---|---|---|
| **Set "High"** | `gauge.Options.value = 88`, `knob.Options.level = 78` | `gauge.Call("flash")` | needle 88 (warm), knob 78, gauge outline flashes blue |
| **Set "Peak"** | `gauge.Options.value = 104`, `gauge.Options.bands = PeakBands()` (whole array replaced), `knob.Options.level = 92` | `knob.Call("pulse")` | needle 104 (high), legend redrawn 0–80 / 80–95 / 95–120, knob scales up briefly |
| **Set "Idle"** | `gauge.Options.value = 64`, `gauge.Options.bands = DefaultBands()`, `knob.Options.level = 40` | `gauge.Call("flash")` | needle 64 (normal), default legend, flash |
| *drag the knob* | (client → server) `valueChanged {value}`; the server clamps it and writes it back to `knob.Options.level` | — | the knob keeps the user's value |

## One handler, read top to bottom

```csharp
private void buttonHigh_Click(object sender, EventArgs e)
{
    ApplySetPoint(88, 78, null);        // 1. state: Options first
    QueueCall(this.gauge, "flash");     // 2. behavior: Call second
}

private void ApplySetPoint(double gaugeValue, double knobLevel, DynamicObject[] bands)
{
    dynamic gaugeOptions = this.gauge.Options;
    gaugeOptions.value = gaugeValue;                  // first-level field → rendered as {"value":88}
    if (bands != null) gaugeOptions.bands = bands;    // whole array replaced → first-level change
    dynamic knobOptions = this.knob.Options;
    knobOptions.level = knobLevel;                    // → {"level":78}
}

private void QueueCall(Widget widget, string function)
{
    widget.Call(function);                            // queued; flushed with the next response
}
```

## Call ordering and queued semantics

`Control.Call("flash")` names a function the InitScript defined on the wrapper
(`this.flash = function () {…}`) and runs it with `this` = the wrapper. It **does not execute
immediately**: Wisej.NET queues it and sends it to the browser with the next response, together
with every property change made in the same request. One button click therefore produces **one
consistent batch**: `{value: 88}` for the gauge, `{level: 78}` for the knob, then `flash()`.

The batch is applied **in order**, which is why each handler sets the Options first and issues
the Call second — `flash()` runs after `update(options, old)` has moved the needle, so the vendor
method sees the updated state. `Call` returns nothing to the server; the value-returning variants
(`CallAsync`, `Call(fn, callback)`) are Module 6's subject.

The *⟶ Client · options & queued calls* list on the right shows one click:

```
→ .NET→JS  gauge.Options          {"value":88}
→ .NET→JS  knob.Options           {"level":78}
→ .NET→JS  gauge.Call("flash")    — queued to client
```

## Camel-casing evidence

`Options` is a dynamic object; anything assigned to it (anonymous types, arrays, nested objects)
is serialized to JSON by Wisej.NET, and **property names are camel-cased**:

```csharp
gaugeOptions.range = new { MinValue = 0D, MaxValue = 120D };          // C#
// arrives as  { range: { minValue: 0, maxValue: 120 } }               // JavaScript
gaugeOptions.label = new { Text = "Boiler 3 — temperature", Units = "°F" };
// arrives as  { label: { text: "…", units: "°F" } }
```

Two places show it:

1. the page-load lines of the trace are produced by the framework's own serializer
   (`Wisej.Core.WisejSerializer`, `CamelCase`) — `"range":{"minValue":0,"maxValue":120}`;
2. the caption drawn by the vendor reads `options.label.text` — if the spelling were wrong the
   label would be blank.

The `bands` array is built from `Wisej.Core.DynamicObject` instances (`from`, `to`, `color`).
Replacing the whole array is a first-level change; editing one band in place is not (see
`InitAndUpdate.md`, "the notify rule").

## What stays where

| Server (`DashboardPage`) | Client (`gauge-init.js`, `knob-init.js`) |
|---|---|
| the widgets' `Options`: the authoritative state | the host elements, the vendor instances, resize, destroy |
| validation of the user's knob value (clamped to 0..100) | vendor option names (`warnAt`, `threshold`, `value`) |
| the decision to flash / pulse | how flash / pulse are drawn |

Only small, purpose-built objects go into `Options` — never a domain entity: everything assigned
is serialized in full and shipped to the browser.

## Evidence

Click Set "High" → Set "Peak" → Set "Idle": the needle and the knob follow each set-point, the
flash / pulse happens **after** the move, and the list on the right shows one `Options` line per
widget plus one queued `Call` per click. Drag the knob → `valueChanged` comes back and the server
keeps the user's value.
