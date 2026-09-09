# Deliverable 3 · Server button that changes widget options and calls the client

Where: `DashboardPage.cs` (`ApplySetPoint`, `QueueCall`, the `button*_Click` handlers),
`DashboardPage.Designer.cs` (the bottom bar).

## The buttons

| Button | Options changed | Client Call | What you see |
|---|---|---|---|
| **Set "High"** | `gauge.Options.value = 88`, `knob.Options.level = 78` | `gauge.Call("flash")` | needle 88 (warm), knob 78, gauge outline flashes blue |
| **Set "Peak"** | `gauge.Options.value = 104`, `gauge.Options.bands = PeakBands()` (whole array replaced), `knob.Options.level = 92` | `knob.Call("pulse")` | needle 104 (high), legend redrawn 0–80 / 80–95 / 95–120, knob scales up briefly |
| **Set "Idle"** | `gauge.Options.value = 64`, `gauge.Options.bands = DefaultBands()`, `knob.Options.level = 40` | `gauge.Call("flash")` | needle 64 (normal), default legend, flash |
| **▶ Stream** | a `Timer` (a Component with no visual surface) assigns `value` / `level` 20 → 95 → 20 every 600 ms | — | both widgets follow; status counts `streaming n/17` |
| **Change nested (no notify)** | `gauge.Options.bands[0].color = "#1a86ff"` **in place** | — | failure path: nothing renders, red banner, `[nested change NOT sent]` |
| **Notify / Update()** | `gauge.Update()` | — | recovery: the legend chip turns blue |
| **Destroy & recreate** | `gauge.Options.style = "compact"` / `"card"` | — | the InitScript destroys and recreates the vendor instance; `recreated` event in the trace |
| **Clear trace** | — | — | empties the trace list |
| *drag the knob* | (client → server) `valueChanged {value}`; the server clamps it and writes it back to `knob.Options.level` | — | status `● knob 63% (user)` |

## One handler, read top to bottom

```csharp
private void buttonHigh_Click(object sender, EventArgs e)
{
    ApplySetPoint("High", 88, 78, null, _bandsName);   // 1. state: Options first
    QueueCall(this.gauge, "flash");                     // 2. behavior: Call second
    SetStatus("high", StatusKind.Warn);
}

private void ApplySetPoint(string name, double gaugeValue, double knobLevel, DynamicObject[] bands, string bandsName)
{
    dynamic gaugeOptions = this.gauge.Options;
    gaugeOptions.value = gaugeValue;          // first-level field → rendered as {"value":88}
    if (bands != null) gaugeOptions.bands = bands;   // whole array replaced → first-level change
    dynamic knobOptions = this.knob.Options;
    knobOptions.level = knobLevel;            // → {"level":78}
}

private void QueueCall(Widget widget, string function)
{
    widget.Call(function);                    // queued; flushed with the next response
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

The trace prints the two lines the walkthrough shows:

```
• server   Set "High"                     Options = { value: 88, level: 78 }  →  update(options)
→ .NET→JS  gauge.Options                  {"value":88}
→ .NET→JS  knob.Options                   {"level":78}
→ .NET→JS  gauge.Call("flash")            — queued to client
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

Three places show it:

1. the page-load trace line is produced by the framework's own serializer
   (`Wisej.Core.WisejSerializer`, `CamelCase`) — `"range":{"minValue":0,"maxValue":120}`;
2. the gauge adapter reports the keys **as they arrived in the browser**:
   `← JS→.NET initialized {"keys":[…],"rangeKeys":["minValue","maxValue"],"bandKeys":["from","to","color"]}`;
3. the caption drawn by the vendor reads `options.label.text` — if the spelling were wrong the
   label would be blank.

The `bands` array is built from `Wisej.Core.DynamicObject` instances (`from`, `to`, `color`)
instead of anonymous objects, because anonymous-type properties are read-only and the failure path
above needs to mutate a band in place. Replacing the whole array is still a first-level change.

## What stays where

| Server (`DashboardPage`) | Client (`gauge-init.js`, `knob-init.js`) |
|---|---|
| `_gaugeValue`, `_knobLevel`, `_style`, the bands: the authoritative state | the host elements, the vendor instances, resize, destroy |
| validation of the user's knob value (clamped to 0..100) | vendor option names (`warnAt`, `threshold`, `value`) |
| the decision to flash / pulse | how flash / pulse are drawn |

Only small, purpose-built objects go into `Options` — never a domain entity: everything assigned
is serialized in full and shipped to the browser.

## Evidence

Click Set "High" → Set "Peak" → Set "Idle": the needle and the knob follow each set-point, the
flash / pulse happens **after** the move, and the trace shows one `update` per widget plus one
queued `Call` per click. Change nested → nothing moves; Notify / Update() → the legend chip turns
blue. Drag the knob → `valueChanged` comes back and the status label shows the user's value.
