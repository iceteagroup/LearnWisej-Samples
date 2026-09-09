# Deliverable 1 — Server control class

`Controls/SimpleGaugeControl.cs` — `IntegrationLab.Controls.SimpleGaugeControl : Wisej.Web.Control`

## Why a Control and not a Widget wrapper

Module 1 hosted the same VendorGauge inside a `Wisej.Web.Widget` with an Options bag and a per-screen
InitScript. That is the right depth for a prototype or a one-off. The Operations Dashboard needs the
gauge on every screen, styled by the corporate theme, and droppable from the Toolbox: that is a
**custom control** — a server class paired with a client class under `/Platform`. It costs more code
once and pays back on every screen that uses it without thinking about it.

## Shape of the class

| Part | What it does |
|---|---|
| `AppearanceKey = "simplegauge"` (constructor) | joins the theme system; `Themes/simplegauge.mixin.theme` styles this key |
| `Value`, `Minimum`, `Maximum`, `Threshold`, `Caption`, `Units` | typed, validated setters; each calls `Update()` only when the value actually changed |
| `OnWebRender(dynamic config)` | the **render contract**: writes `className`, `appearance`, the camel-cased state and the wired events |
| `OnWebEvent(WisejEventArgs e)` | the **event contract**: `"thresholdExceeded"` → typed `ThresholdExceeded` event; everything else → `base.OnWebEvent(e)` |
| `IsDesignMode()`, `DesignTimeSampleValue()` | design-time rendering support (see `DesignTimeNotes.md`) |
| `Trace` event, `RenderCount`, `LastRenderedJson` | lab diagnostics only, never rendered |

## Validation lives on the server

```csharp
public double Value
{
    get => _value;
    set
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(...);
        if (value < _minimum || value > _maximum)           throw new ArgumentOutOfRangeException(...);
        if (_value == value) return;          // no change → no render
        _value = value;
        SyncAlarmState();                     // custom theme state "alarm" (mixin restyles the border)
        Update();                             // schedule OnWebRender; Wisej.NET ships only the diff
    }
}
```

A rejected value throws before anything is rendered: the browser keeps the last good reading. The
dashboard's **Invalid value (999 psi)** button exercises exactly this path.

`Minimum`/`Maximum` clamp the current `Value` into the new range instead of throwing, because the
Designer assigns properties in alphabetical order (`Maximum` before `Minimum` before `Value`).

## OnWebRender — intentional state only

```csharp
protected override void OnWebRender(dynamic config)
{
    base.OnWebRender((object)config);
    config.className = "integrationlab.controls.SimpleGaugeControl";   // the client qx class
    config.appearance = this.AppearanceKey;                            // "simplegauge"
    config.value     = designMode ? DesignTimeSampleValue() : _value;  // camel-cased, one per client property
    config.minimum   = _minimum;
    config.maximum   = _maximum;
    config.threshold = _threshold;
    config.caption   = _caption;
    config.units     = _units;
    AddWiredEvent(config, "thresholdExceeded(Data)");                  // "(Data)" = carry e.getData()
}
```

Nothing that exists only for the Designer, for business logic or for diagnostics (`RenderCount`,
`LastRenderedJson`, the `Trace` event) is written onto `config`, so it never reaches the browser.
Wisej.NET diffs the config against the previous render and sends only the changed fields — during
streaming each tile costs one `{"value": …}` per tick.

## OnWebEvent — the server decides

```csharp
case "thresholdExceeded":
    dynamic parameters = e.Parameters;          // WisejEventArgs.Parameters is typed object
    dynamic data = parameters?.Data;            // the map the client passed to fireDataEvent
    double reported = ToDouble(data?.value);
    ThresholdExceeded?.Invoke(this, new GaugeThresholdEventArgs(_value, reported, _threshold));
    break;
default:
    base.OnWebEvent(e);                         // never swallow pointer/focus/resize events
```

The .NET event carries the **server** value; the reported value is kept only for the contract check
in the trace (`client reported X but server Value is Y: server wins`).

## Evidence (running app)

- Page load: four `→ .NET→JS render #1 boiler1 {"className":"integrationlab.controls.SimpleGaugeControl","appearance":"simplegauge","value":70,…}` lines.
- ▶ Stream live: one `render #n` line per tile per tick; when a reading crosses its threshold one
  `← JS→.NET thresholdExceeded {"value":…,"threshold":…}` comes back and `• server ThresholdExceeded in C#` follows, the tile shows a red banner and gets a red border (theme state `alarm`).
- Invalid value: `• server rejected boiler1 Value must be between 0 and 150.` — no render line follows.
