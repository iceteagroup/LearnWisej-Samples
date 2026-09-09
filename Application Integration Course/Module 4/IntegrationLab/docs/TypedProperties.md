# Deliverable 2 · Typed properties with defaults

All properties live in `Controls/SimpleGauge.cs`, category **Gauge**, each with `[Description]` and
`[DefaultValue]`. The Properties window shows exactly: `Value`, `Minimum`, `Maximum`, `Caption`,
`AnimationEnabled`, `Threshold` — no `Packages`, no `InitScript`, no `Options`.

## The table

| Property | Type | Default | Validation (setter) | Forces client update? | Read-only after creation? |
|---|---|---|---|---|---|
| `Value` | `double` | `0` (`DefaultReading`) | finite; `Minimum ≤ Value ≤ Maximum`, else `ArgumentOutOfRangeException` | yes — `options.value` → `update()`; the client answers with `valueChanged` | no (the whole point) |
| `Minimum` | `double` | `0` | finite; `< Maximum`, else `ArgumentOutOfRangeException` | yes — `options.min` + `options.warnAt`; re-lays out the scale; clamps `Value` if it fell below | no, but rare after design time |
| `Maximum` | `double` | `100` | finite; `> Minimum`, else `ArgumentOutOfRangeException` | yes — `options.max` + `options.warnAt`; re-lays out the scale; clamps `Value` if it rose above | no, but rare after design time |
| `Caption` | `string` | `""` | `null` → `""` | yes — `options.label` (cheap: text only) | no |
| `AnimationEnabled` | `bool` | `true` | none | yes — `options.animationEnabled`; purely visual, the needle sweeps or jumps on the *next* value | no |
| `Threshold` | `double` | `85` | finite | yes — `options.threshold` + `options.warnAt`; the vendor re-draws the bands and re-arms its rising-edge detection | no |

Not exposed on purpose (wrapper internals the application never sees):

| Vendor option | Handled by |
|---|---|
| `warnAt` (start of the warm band) | computed as `Threshold − 15 % of (Maximum − Minimum)` whenever `Minimum`, `Maximum` or `Threshold` change |
| `units` | `""` by default; a derived class sets it in `OnConfigureOptions` (e.g. `options.units = "°F"`) |
| `label` | that is `Caption` — the application word, not the vendor word |

Every setter follows the same shape:

```csharp
set
{
    // 1. validate on the server, before anything is rendered
    if (value < _minimum || value > _maximum)
        throw new ArgumentOutOfRangeException(nameof(Value), value, "...");
    // 2. only assign the option when the value actually changed
    if (_value == value) return;
    _value = value;
    // 3. camel-cased first-level option: Wisej.NET renders {"value":…} and calls update(options, old)
    dynamic options = base.Options;
    options.value = value;
}
```

## Which changes must force a client update

All of them — every typed property is *state the browser renders*. What differs is the cost:

- `Value` → a needle move (animated when `AnimationEnabled`). Cheapest and most frequent.
- `Caption`, `AnimationEnabled` → a text change / a flag read on the next move. Cheap.
- `Minimum`, `Maximum`, `Threshold` → the vendor re-lays out its bands (`setOptions` → `_layout`). Still cheap
  for this vendor, but the descriptions say the scale is re-laid out so an application developer does not put
  them in a 50 ms timer.

Nothing is read-only after creation in this class: the vendor supports changing everything through
`setOptions`. If a vendor could not (say, a chart type that requires re-creating the instance), that property
would get `[Description("… changing it re-creates the widget")]`, its setter would replace the whole nested
option object (a first-level change) and the adapter would destroy/recreate the vendor in `update`.

Setting a property to the value it already has renders **nothing**: no `update()` call, no trace line. The
trace proves it — click **Value = 72** twice and the second click logs only the `code-behind` line.

## Which defaults match the vendor, and which do not

| | Vendor (`VendorGauge` DEFAULTS) | `SimpleGauge` | Reason |
|---|---|---|---|
| `min` / `max` | 40 / 120 (a Fahrenheit gauge) | 0 / 100 | the class is a generic gauge; percent-like defaults render sensibly for any reading |
| `value` | 0 | 0 | same |
| `threshold` | 100 | 85 | with `Maximum = 100` the vendor default could never be crossed |
| `warnAt` | 85 | derived from `Threshold` | not an application concept |
| `units` | `"°F"` | `""` | the class does not know what it measures; derive + `OnConfigureOptions` |
| `label` | `""` | `Caption = ""` | same |

## Designer attributes used

- `[DefaultValue(...)]` — the Designer serializes only what differs from it. `DemoPage.Designer.cs` therefore
  contains `simpleGauge1.Caption`, `simpleGauge1.Value` and, for the second gauge, `Maximum`, `Threshold`,
  `Value` — and nothing about packages or scripts.
- `[Category("Gauge")]` — one group in the Properties window, above `Design`.
- `[Description("…")]` — the help pane text (`Value`: "The current gauge reading. Setting it queues a client
  update — no raw Options editing.").
- `[DefaultProperty("Value")]`, `[DefaultEvent("ValueChanged")]`, `[ToolboxItem(true)]` on the class.

## Events (typed, with `EventArgs`)

| Event | `EventArgs` | Raised from | Meaning |
|---|---|---|---|
| `ValueChanged` | `GaugeValueChangedEventArgs` (`Value`, `ReportedValue`, `Previous`) | `OnWidgetEvent("valueChanged")` | the browser has rendered the new value (after the sweep when animated) |
| `ThresholdExceeded` | `GaugeEventArgs` (`Value`, `ReportedValue`) | `OnWidgetEvent("thresholdExceeded")` | rising edge over `Threshold`, once per crossing |
| `WidgetError` | `GaugeErrorEventArgs` (`Phase`, `Message`) | `OnWidgetEvent("error")` | the adapter caught a vendor exception instead of crashing the page |
| `Trace` | `TraceEventArgs` | every rendered option / received event | diagnostics; `[Browsable(false)]` |

`Value` in every `EventArgs` is the **server** value; `ReportedValue` is what the client said. The wrapper
logs a `contract check` line when they disagree — the server wins.

## Evidence in the running app

- **Value = 72 / 90 / 78** and **Maximum = 120; Value = 45**: each click shows the code-behind statement, then
  `→ .NET→JS simpleGauge1.update(options) {"value":90}` (or `{"max":120,"warnAt":67}` + `{"value":45}`), then
  `← JS→.NET simpleGauge1.valueChanged {"value":90,"previous":72}` and `simpleGauge1.ValueChanged fired in C#`.
- **Value = 200 (invalid)**: `• server rejected  Value must be between Minimum (0) and Maximum (100).` and no
  `update(options)` line at all — nothing was rendered.
- **AnimationEnabled = false**: `{"animationEnabled":false}` goes out; the next value jumps instead of sweeping.
