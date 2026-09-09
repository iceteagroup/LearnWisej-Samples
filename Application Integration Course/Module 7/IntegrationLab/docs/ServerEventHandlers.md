# Deliverable 2 — Three server-side event handlers

Module 7 · Events & Handler Contracts. The three widgets handle their events in three different
styles on purpose, so the lab shows every entry point the lesson names. All three share the same
discipline: **every field of the payload is validated before it is used**, and the DTO the
application sees (`Contracts/*.cs`) is built from server state where a server copy exists.

| # | Where | Entry point | Style | Raises |
|---|---|---|---|---|
| 1 | `Widgets/GaugeWidget.cs` | `protected override void OnWidgetEvent(WidgetEventArgs e)` | the Widget catch-all, wrapped into a typed event | `ThresholdCrossed(GaugeThresholdEventArgs)` |
| 2 | `Window1.cs` | `knob.WidgetEvent += knob_WidgetEvent` — a page-level `switch (e.Type)` | the raw catch-all path the video shows | `OnValueChanged(KnobValueEventArgs)` (page method) |
| 3 | `Widgets/ChartWidget.cs` | `protected override void OnWebEvent(WisejEventArgs e)` + `base.OnWebEvent(e)` | the general control entry point shared with the framework | `PointClicked(ChartPointEventArgs)` |

## Handler 1 — `GaugeWidget.OnWidgetEvent` → typed event

```csharp
protected override void OnWidgetEvent(WidgetEventArgs e)
{
    switch (e.Type)
    {
        case "thresholdCrossed": HandleThresholdCrossed(e.Data); break;
        case "error":            /* trace the vendor failure */   break;
        default:                 base.OnWidgetEvent(e);            break;   // not ours: still reaches WidgetEvent subscribers
    }
}

private void HandleThresholdCrossed(object payload)
{
    dynamic data = payload;
    if (!PayloadReader.TryDouble(PayloadReader.Get(() => data.value), out double value)) { Reject(...); return; }
    if (value < _minimum || value > _maximum)                                              { Reject(...); return; }
    if (!PayloadReader.TryString(PayloadReader.Get(() => data.level), out string level)
        || (level != "warn" && level != "high"))                                            { Reject(...); return; }
    double line = level == "high" ? _threshold : _warnAt;
    if (value < line)                                                                      { Reject(...); return; }   // the level must agree with the server's lines

    ThresholdCrossed?.Invoke(this, new GaugeThresholdEventArgs(_value, level));            // server Value, client level
}
```

`fireWidgetEvent(name, data)` on the client is the direct path to the server-side `WidgetEvent`;
`OnWidgetEvent` is the virtual that raises it. A wrapper class derived from `Wisej.Web.Widget`
handles it internally and re-raises a **typed** .NET event, so `Window1` only ever sees
`GaugeThresholdEventArgs` — never the string `"thresholdCrossed"` nor a `dynamic`.

## Handler 2 — page-level `WidgetEvent` switch (the video's `knob_WidgetEvent`)

```csharp
this.knob.WidgetEvent += knob_WidgetEvent;

private void knob_WidgetEvent(object sender, WidgetEventArgs e)
{
    switch (e.Type)
    {
        case "valueChanged":
            if (!this.knob.TryReadValueChanged(e.Data, out KnobValueEventArgs d, out string reason))
            { AddTrace(TraceDirection.Rejected, "valueChanged", "rejected: " + reason); return; }
            OnValueChanged(d);                     // the small DTO
            break;
        case "error":   ...; break;
        default:        AddTrace(TraceDirection.Rejected, e.Type, "rejected: not in the contract"); break;
    }
}
```

`KnobWidget` does not override anything: the page subscribes to the public `WidgetEvent` and
switches on `e.Type`, reading `e.Data` as the compact object the adapter forwarded. Validation still
lives next to the state it protects (`KnobWidget.TryReadValueChanged`: finite, within
`Minimum..Maximum`, on the `Step` grid, `source` ∈ {`user`, `server`}); the page commits the value
with `AcceptClientValue`, which updates server state without re-rendering (no echo loop).

## Handler 3 — `ChartWidget.OnWebEvent` with the base call

```csharp
protected override void OnWebEvent(WisejEventArgs e)
{
    if (e.Type == "widgetEvent" && TryReadWidgetEvent(e, out string type, out object data) && type == "pointClicked")
    {
        HandlePointClicked(data, "OnWebEvent");
        return;                                    // handled: do not raise WidgetEvent a second time for it
    }
    base.OnWebEvent(e);                            // focus, resize, pointer, widget events we do not own → the framework
}
```

`OnWebEvent` is the general event entry point of **every** control, shared with the framework: the
same method receives focus, resize, pointer, drag/drop … and the `widgetEvent` message that carries a
`fireWidgetEvent` call. For custom controls (Module 5 pattern) `fireEvent` / `fireDataEvent` land
here too, provided the server config wires the event name. Two rules follow:

1. **Never swallow what you do not own.** `base.OnWebEvent(e)` is mandatory for every other event,
   otherwise behavior that has nothing to do with the integration breaks.
2. **Read the parameters defensively.** On the client `fireWidgetEvent(type, data)` is
   `fireDataEvent("widgetEvent", { type, data })`, and the framework wires it as `widgetEvent(Event)`,
   so the object arrives as `e.Parameters.Event = { type, data }` (`TryReadWidgetEvent` also accepts a
   flat `{ type, data }`). If the shape is not recognised the event goes to `base`, which raises
   `OnWidgetEvent` — overridden here as a **fallback** with the same validation — so nothing is lost
   and the log line says which path ran (`raised via OnWebEvent` / `raised via OnWidgetEvent (fallback)`).

Validation in `HandlePointClicked`: `index` must be an integer within `0..Labels.Length-1`
(`{ index: -1 }` from the "Bad payload" button is rejected with `rejected: index out of range`),
`label` must be a string, `value` a finite number. The **index is a lookup key**: `Label` and `Value`
in `ChartPointEventArgs` come from the server arrays, the client copies are only compared and logged
("server wins").

## `WidgetEvent` vs `OnWebEvent` — the self-check

| | `WidgetEvent` / `OnWidgetEvent(WidgetEventArgs)` | `OnWebEvent(WisejEventArgs)` |
|---|---|---|
| Lives on | `Wisej.Web.Widget` (and every `Control`, but only a Widget wrapper fires it) | every `Wisej.Web.Control` |
| Sees | only `{Type, Data}` pairs the wrapper sent with `fireWidgetEvent` | every client event addressed to the control, framework ones included |
| Data | `e.Type` (string), `e.Data` (dynamic, the compact payload) | `e.Type`, `e.Parameters` (dynamic, per wired-event argument names) |
| Fired by | `fireWidgetEvent(name, data)` | `fireEvent(name)` / `fireDataEvent(name, data)` for names wired in the server config; the framework for built-ins |
| Contract with base | `default: base.OnWidgetEvent(e)` keeps subscribers working | **must** call `base.OnWebEvent(e)` for everything not handled |
| Use it for | `Widget` wrappers around a vendor library (Modules 2, 7) | custom controls on the client class model (Module 5), or when you must see the raw pipeline |

## Evidence

- Log after **Gauge 104**: `← JS→.NET thresholdCrossed e.Data = {"value":104,"level":"high"}` then `• .NET ThresholdCrossed raised via OnWidgetEvent → GaugeThresholdEventArgs { Value=104, Level=high }`, banner and toast.
- Log after **Knob +10** or a drag: `← JS→.NET valueChanged e.Data = {…}` then `• .NET OnValueChanged page-level WidgetEvent switch → KnobValueEventArgs {…}`; the dial pulses on a user change.
- Log after a point click: `← JS→.NET pointClicked e.Data = {"index":3,"label":"Apr","value":68}` then `• .NET PointClicked raised via OnWebEvent → ChartPointEventArgs {…}`.
- Log after **Bad payload**: `→ .NET→JS Call("fireBadPayload")`, `← JS→.NET pointClicked e.Data = {"index":-1}`, `✖ rejected pointClicked rejected: index out of range (-1; 0..5)` — no .NET event, the page stays alive.
