# Deliverable 5 — the usage example screen

`EnterpriseOps.UI.WorkOrderHistoryPage` — the screen that proves the two components are finished.

Files: `UI/WorkOrderHistoryPage.cs`, `UI/WorkOrderHistoryPage.Designer.cs`.

## What "finished" means here

A component is finished when a screen developer can use it **from the Designer and from a handful of typed
lines**, without reading its internals. This screen is the test:

* both components are declared in `InitializeComponent()` like any other control, with their properties
  set as designer property assignments (`Caption`, `EmptyText`, `TimeFormat`, `ShowLegend`) and their
  events wired the normal way;
* the code-behind contains **no** package name, **no** script, **no** option name, **no** JavaScript, **no**
  vendor name and **no** wire format;
* every handler is a few lines: it calls a service or a component method, and shows the result.

## The handlers, in full

```csharp
private async void btnReload_Click(object sender, EventArgs e)
{
    try { await ReloadAsync(); }
    catch (Exception ex) { ReportUnexpected(ex); }
}

private async void chartWorkOrders_SegmentClicked(object sender, ChartSegmentEventArgs e)
{
    _trace.Client($"WorkOrderChartWidget.SegmentClicked → key '{e.Key}' … — a key, not a work order");
    try { await ShowSegmentAsync(e.Key); }
    catch (Exception ex) { ReportUnexpected(ex); }
}
```

`ReloadAsync` → `LoadHistoryAsync` + `LoadBreakdownAsync`, each one service call whose typed result is
handed to a component:

```csharp
statusTimeline.SetItems(history.Entries.Select(ToTimelineItem));
chartWorkOrders.SetSegments(result.Value.Select(c => new ChartSegment(c.Key, c.Label, c.Count)));
```

`ToTimelineItem` is the whole boundary between the domain and the components — the only place a projection
becomes component data.

## The component API gate

`Services/ComponentApiGate.cs` answers the lab's first review question mechanically. It strips comments and
string literals from a source file (so a screen that *documents* the contract is not punished for it) and
counts the places where the screen reaches past a component's public API:

| Token | Why it is a leak |
|---|---|
| `Packages.Add` | registers vendor packages on a screen — one forgotten screen ships a broken chart |
| `InitScript` | pastes a client adapter into a screen — five screens, five versions of the adapter |
| `WiredEvents` | declares the event contract per screen instead of once in the component |
| `.Options` | the vendor's option names leak into the application |
| `Application.Eval` | drives the browser from a screen; nothing on the server knows what happened |
| `EnterpriseOpsChart` | hard-codes the vendor's global name |
| `new Widget` | a raw Widget, so failure handling is nobody's job |
| `WidgetEvent` | the raw untyped event instead of a named server event with typed arguments |

It also counts controls the screen builds by hand (`new Label`, `new Panel`) — a copy-pasted component
looks exactly like that.

The **Anti-pattern: leaky screen** button runs it over two files:

| File | Result |
|---|---|
| `Controls/Samples/LeakyChartScreen.cs.txt` — the walkthrough's opening scene, kept as `.cs.txt` so it never compiles | all 8 rules fire, plus 5 controls built by hand |
| `UI/WorkOrderHistoryPage.cs` — this screen | 0 findings, 0 controls built by hand |

## What to click

See the README's **What to click** table; every path is listed there with what the trace should say.

## Evidence — the boundary, proven three ways

1. **The trace.** Every line is tagged with the layer that produced it: `UI →` (a click), `Component:` (a
   decision the wrapper or the timeline took), `Client →` (something the browser said), `Service:`,
   `Data:`, `Security:`, `Package:`. A reviewer can read the sequence of an interaction without a debugger.
2. **`DescribeWirePayload()`**, traced on every load: the exact JSON the wrapper would send. Four objects of
   three fields — no entity, no tenant, no user.
3. **The gate**, run over this screen's own source file at run time. It is the difference between claiming
   the boundary holds and showing it.
