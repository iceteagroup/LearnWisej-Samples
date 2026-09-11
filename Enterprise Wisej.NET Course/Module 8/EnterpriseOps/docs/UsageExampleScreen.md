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

## The handlers

```csharp
private async void chartWorkOrders_SegmentClicked(object sender, ChartSegmentEventArgs e)
{
    try { await ShowSegmentAsync(e.Key); }
    catch (Exception ex) { ReportUnexpected(ex); }
}

private void chartWorkOrders_WidgetError(object sender, WidgetErrorEventArgs e)
{
    _trace.Component($"WorkOrderChartWidget error in '{e.Phase}': {e.Message} …");
    ShowFailure("WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works");
}
```

On load, `LoadHistoryAsync` and `LoadBreakdownAsync` each make one service call and hand the typed result
to a component:

```csharp
statusTimeline.SetItems(history.Entries.Select(ToTimelineItem));
chartWorkOrders.SetSegments(result.Value.Select(c => new ChartSegment(c.Key, c.Label, c.Count)));
```

`ToTimelineItem` is the whole boundary between the domain and the components — the only place a projection
becomes component data.

## Evidence — what the running app shows

| Do this | You should see |
|---|---|
| Open the page | *Work order 2002 — history*: the timeline on the left, the chart on the right (Open 38 · On hold 22 · Escalated 12 · Done 28), status bar *Work order 2002 — history loaded · StatusTimeline + WorkOrderChartWidget*. |
| Click the red **Escalated** slice | Blue note *SegmentClicked → "escalated" · 12 work orders shown below*, the grid fills with those 12 rows, status bar *SegmentClicked("escalated") — named server event · grid filtered server-side*. |
| Block `/Widgets/vendor-opschart.js` in the browser's developer tools and reload | The chart shows the embedded fallback list with the same numbers, a red note *WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works*; the timeline keeps working. |
