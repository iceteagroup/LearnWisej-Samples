# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 8

**Custom Controls, Extensions, Widget Wrappers & Reusable Components** — *Build a StatusTimeline control &
a chart widget.*

The EnterpriseOps Command Center's **Work order history** screen, built out of two reusable components:

* `EnterpriseOps.Controls.StatusTimeline` — a composed `UserControl` with typed properties
  (`Items : IList<TimelineItem>`, `SelectedItem`, `Caption`, `TimeFormat`, `SampleMode`), a named server
  event (`ItemSelected`) and a design-time sample mode;
* `EnterpriseOps.Widgets.WorkOrderChartWidget : Wisej.Web.Widget` — a stable C# API in front of a
  third-party SVG chart, with an embedded client adapter, an embedded resource package, a `pointSelected`
  event that becomes the named server event `SegmentClicked`, and an `error` event that becomes
  `WidgetError` with a polite fallback already on screen.

Everything is in memory. No database, no network, no cloud account, nothing deployed.

## Run it

```
cd "Module 8/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5208
```

Then open <http://localhost:5208>. The session is **ana.ops · Manager** on tenant **fabrikam**, on work
order **2002** — the work order the walkthrough video opens (*Dock door sensor fault*), whose four history
entries are the four entries the video shows.

## The screen

`WorkOrderHistoryPage` (948 × 590): a blue title bar *EnterpriseOps — Work order history*, then one white
card with the heading `lblCardTitle` (*Work order 2002 — history*), `statusTimeline` (left),
`chartWorkOrders` (right), the note `lblBanner` (hidden until there is something to say), the grid
`dgvSegment` and the dark status bar `lblStatusBar`.

## What to click

| Do this | Path | What you should see |
|---|---|---|
| *(page load)* | success | Timeline with 4 entries (grey *Created*, blue *Assigned*, amber *On hold*, red *Escalated*); chart Open 38 · On hold 22 · Escalated 12 · Done 28; status bar *Work order 2002 — history loaded · StatusTimeline + WorkOrderChartWidget*. |
| **A timeline entry** | `ItemSelected` | The row highlights; the status bar shows *Escalated: approver m.weber · due Jun 14*. |
| **A chart slice** (e.g. red *Escalated*) | `SegmentClicked` | Blue note *SegmentClicked → "escalated" · 12 work orders shown below*; the grid shows the 12 rows; status bar *SegmentClicked("escalated") — named server event · grid filtered server-side*. |
| Block `/Widgets/vendor-opschart.js` in the browser's developer tools (Network → Block request URL), then reload | failure — the video's path | The chart renders the embedded fallback list with the same numbers; red note *WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works*; the timeline keeps working and the error is logged server-side (`System.Diagnostics.Trace`). |

Other failures are handled as ordinary code: a tenant-denied read (`AccessPolicy`) or a segment key the
service does not know shows a red note with the correlation id; an unexpected exception shows the note and
a toast.

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project, run it locally once | `EnterpriseOps.slnx`, `EnterpriseOps/EnterpriseOps.csproj`, `Properties/launchSettings.json` (port 5208) |
| **Deliverable — StatusTimeline UserControl** | `Controls/StatusTimeline.cs` + `.Designer.cs`, `Controls/TimelineItem.cs` → `docs/StatusTimelineControl.md` |
| **Deliverable — Widget wrapper or custom control** | `Widgets/WorkOrderChartWidget.cs`, `Widgets/ChartSegment.cs`, `Widgets/opschart-init.js`, `Widgets/vendor-opschart.js` → `docs/WidgetWrapper.md` |
| **Deliverable — embedded resource package** | `Widgets/ComponentResourcePackage.cs`, the `<EmbeddedResource>` block in `EnterpriseOps.csproj`, `Widgets/opschart.css` → `docs/EmbeddedResourcePackage.md` |
| **Deliverable — event contract document** | `docs/EventContract.md` + `docs/event-contract.svg` (implemented by `WorkOrderChartWidget.OnWidgetEvent` and `opschart-init.js` `_addListener` / `_getEventData`) |
| **Deliverable — usage example screen** | `UI/WorkOrderHistoryPage.cs` + `.Designer.cs` → `docs/UsageExampleScreen.md` |
| Service / model boundary | `Services/WorkOrderHistoryService.cs`, `Services/Views.cs`, `Domain/StatusGroup.cs`, `Security/AccessPolicy.cs`, `Data/FakeWorkOrderStore.cs` |
| Show every path without leaking internals | `ShowInfo` / `ShowFailure` / `ReportUnexpected` in `UI/WorkOrderHistoryPage.cs`; the adapter's fallback + `WidgetError` |
| Production-readiness note | `docs/EventContract.md` §5–§6 (failure semantics, how to change the contract), `docs/EmbeddedResourcePackage.md` (upgrade path) |

## Where things live

```
EnterpriseOps/
  Program.cs                               session composition root → UI.WorkOrderHistoryPage
  Controls/  StatusTimeline.cs / .Designer.cs · TimelineItem.cs
  Widgets/   WorkOrderChartWidget.cs · ChartSegment.cs · ComponentResourcePackage.cs
             vendor-opschart.js · opschart.css · opschart-init.js (embedded, InitScript)
  UI/        WorkOrderHistoryPage.cs / .Designer.cs
  Services/  WorkOrderHistoryService.cs · ActivityTrace.cs (server log) · SessionContext.cs · CommandResult.cs · Views.cs
  Security/AccessPolicy.cs   Domain/   Data/
  docs/      the five deliverables + event-contract.svg
```

The video's solution tree shows six projects; this sample keeps one project with folder-per-layer namespaces
that map 1:1 onto that split, so it runs with one `dotnet run`.

## Student review questions, answered against this sample

1. **Can another developer use the component without reading its internals?** Both components are dropped
   from the Toolbox and configured with typed properties in `InitializeComponent()`; the code-behind uses
   `SetItems`, `SetSegments`, `Select`, `Clear` and three named events. `Packages`, `InitScript`,
   `WiredEvents`, `Options`, `Call` and `CallAsync` are shadowed and hidden.
2. **What data crosses from server to client?** Down: `segments[{key,label,value}]`, `palette`, `caption`,
   `showLegend`, `badge`, `selectedKey`. Up: `pointSelected {key,label,value,percent}` and
   `error {phase,message}`. No entity, tenant, user, version or correlation id — see `docs/EventContract.md`.
3. **Does the component render safely in the designer?** Both override `OnCreateControl` and turn
   `SampleMode` on when `DesignMode` is true: fixed data, an amber `DESIGN-TIME SAMPLE` badge, no service
   call, nothing that can throw.

## Verified / unverified

Verified on this framework build (other course samples): the `Widget` API (`Packages`, `InitScript` from
`GetResourceString`, `WiredEvents`, `Options` → `update(options, old)`, `OnWidgetEvent`), first-level
camel-casing of `Options`, the adapter shape (`_addListener` / `_getEventData`, deferred errors, wrapped
`dispose`), serving `Widgets/…` from the project folder.

Unverified — compiles on both target frameworks, needs a runtime look: `FlowLayoutPanel` (`TopDown`,
`WrapContents = false`, `AutoScroll`) as the timeline's row host; `DesignMode` being true inside
`OnCreateControl` in the Wisej Designer (and whether the Designer renders the `Widget` subclass live);
`Cursors.Hand` on a row `Panel` and `Label.Click` forwarding to the row; the `qx-dark` rules in
`opschart.css`.
