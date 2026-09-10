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

Then open <http://localhost:5208>. The session starts as **ana.ops · Manager** on tenant **fabrikam**, on
work order **2002** — the work order the walkthrough video opens (*Dock door sensor fault*, Harbor
Logistics, Dock 4), whose four history entries are the four entries the video shows.

Build only:

```
cd "Module 8/EnterpriseOps"
dotnet build -nologo -v q
```

## What to click

| Control | Path | What you should see |
|---|---|---|
| *(page load)* | success | Left: the timeline with 4 entries (grey *Created*, blue *Assigned*, amber *On hold*, red *Escalated*) and the chart — Open 38 · On hold 22 · Escalated 12 · Done 28. Trace: `Service: GetHistoryAsync(...)`, `Data: 4 history entries … (actor column not projected)`, `Component: wire payload → {"segments":[…]}`. |
| **A timeline entry** | success | The row highlights; `Client → StatusTimeline.ItemSelected → #3 Escalated …`; the footer says `ItemSelected — Escalated: approver m.weber · due Jun 14`. |
| **A chart slice** | success | `Client → WorkOrderChartWidget.SegmentClicked → key 'escalated' (12, 12%) — a key, not a work order`, then the service query, then the grid fills. Footer: `SegmentClicked("escalated") — named server event · grid filtered server-side · 12 rows`. |
| `cboWorkOrder` → **2107 · … (contoso)** | failure — permission | `Security: permission denied — work order 2107 belongs to tenant 'contoso', session tenant is 'fabrikam'`. Red banner, timeline cleared. The component decided nothing. |
| **⟳ Reload** | success | Both service calls again with a fresh correlation id (shown top right). |
| **Design-time sample mode** | design time | Amber `DESIGN-TIME SAMPLE` badges on both components and fixed sample data — **and no service call in the trace**. Exactly what the Wisej Designer renders. Press again for live data. |
| **Resource package** | packaging | The manifest: three resources, in load order, with the embedded byte counts, then `✓ every declared resource is embedded and the served copies match`. |
| **Fail: invalid palette** | failure — server-side reject | `Component: server rejected palette 'neon-pink' …`. Red banner *invalid palette refused on the server — nothing was sent to the browser*. The chart is untouched, because nothing was rendered. |
| **Fail: forged segment key** | failure — untrusted input | The service is called with `all-tenants` as if the browser had sent it: `Service: rejected — 'all-tenants' is not a reporting group (keys come from the browser; never trusted)`. Grid cleared, red banner, no query run. |
| **Fail: vendor throws** | failure — vendor error as a WidgetEvent | The next `update()` hands the vendor `segments: null`; the vendor throws, the adapter catches it, renders the fallback and raises `error`. `Client → WorkOrderChartWidget.WidgetError → phase 'update': EnterpriseOpsChart: options.segments must be an array …`. Amber banner + toast; the timeline and the grid keep working. |
| **Fail: block the vendor script** | failure — the video's path | The proxy-blocked package: `phase 'init'`, the embedded fallback list with the same numbers, footer `WidgetLoadError — vendor-opschart.js · fallback rendered · screen still works`. |
| **Recover: reload the chart** | recovery | Same wrapper, same API, chart back with the same data. |
| **Anti-pattern: leaky screen** | the video's anti-pattern | `ComponentApiGate` over `Controls/Samples/LeakyChartScreen.cs.txt` — all 8 rules fire and it builds 5 controls by hand — and over `UI/WorkOrderHistoryPage.cs`: `✓ every component interaction goes through a typed property, a method or a named event`, 0 controls built by hand. |
| **Clear trace** | — | Empties `lstTrace`. |

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project, run it locally once | `EnterpriseOps.slnx`, `EnterpriseOps/EnterpriseOps.csproj`, `Properties/launchSettings.json` (port 5208) |
| **Deliverable — StatusTimeline UserControl** | `Controls/StatusTimeline.cs` + `.Designer.cs`, `Controls/TimelineItem.cs` → `docs/StatusTimelineControl.md` |
| **Deliverable — Widget wrapper or custom control** | `Widgets/WorkOrderChartWidget.cs`, `Widgets/ChartSegment.cs`, `Widgets/opschart-init.js`, `Widgets/vendor-opschart.js` → `docs/WidgetWrapper.md` |
| **Deliverable — embedded resource package** | `Widgets/ComponentResourcePackage.cs`, the `<EmbeddedResource>` block in `EnterpriseOps.csproj`, `Widgets/opschart.css` → `docs/EmbeddedResourcePackage.md` |
| **Deliverable — event contract document** | `docs/EventContract.md` + `docs/event-contract.svg` (implemented by `WorkOrderChartWidget.OnWidgetEvent` and `opschart-init.js` `_addListener` / `_getEventData`) |
| **Deliverable — usage example screen** | `UI/WorkOrderHistoryPage.cs` + `.Designer.cs` → `docs/UsageExampleScreen.md` |
| Create the service / model boundary first | `Services/WorkOrderHistoryService.cs`, `Services/Views.cs`, `Domain/StatusGroup.cs`, `Security/AccessPolicy.cs`, `Data/FakeWorkOrderStore.cs` |
| Show every path (success, validation, error) without leaking internals | `ShowOk` / `ShowWarning` / `ShowFailure` / `ReportUnexpected` in `UI/WorkOrderHistoryPage.cs`; the generic toast text, the correlation id in the header |
| Add a failure-path demonstration, not only the happy path | four of them: invalid palette, forged segment key, vendor throws, vendor script blocked — plus the tenant-denied read and the **Recover** button |
| Record what changed / production-readiness note | `docs/EventContract.md` §6 (how to change the contract) and `docs/EmbeddedResourcePackage.md` (upgrade path) |

## Where things live

```
Module 8/
  EnterpriseOps.slnx
  README.md                                  ← this file
  EnterpriseOps/
    Program.cs                               session composition root → UI.WorkOrderHistoryPage
    Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json           port 5208
    Controls/
      StatusTimeline.cs / .Designer.cs       the reusable UserControl
      TimelineItem.cs                        TimelineItem · TimelineItemCollection · TimelineSeverity · TimelineItemEventArgs
      Samples/LeakyChartScreen.cs.txt        THE ANTI-PATTERN (never compiled; read by the gate)
    Widgets/
      WorkOrderChartWidget.cs                the Wisej.Web.Widget wrapper
      ChartSegment.cs                        the DTO + ChartSegmentEventArgs · WidgetErrorEventArgs · ComponentTraceEventArgs
      ComponentResourcePackage.cs            the one ordered list of resources + Verify()
      vendor-opschart.js                     the "vendor" SVG chart library (embedded AND served)
      opschart.css                           the component stylesheet (embedded AND served, loads after the vendor)
      opschart-init.js                       the client adapter (embedded, InitScript, never served)
    UI/
      WorkOrderHistoryPage.cs / .Designer.cs the usage example screen
    Services/
      WorkOrderHistoryService.cs             the only place that reads data or decides
      ComponentApiGate.cs                    measures whether a screen needs a component's internals
      ActivityTrace.cs  SessionContext.cs  CommandResult.cs  Views.cs
    Security/AccessPolicy.cs                 tenant isolation for reads
    Domain/  Data/                           WorkOrder · StatusGroup · the in-memory store (150 rows, seeded)
    docs/                                    the five deliverables + event-contract.svg
```

The video's solution tree shows six projects (`EnterpriseOps.Controls`, `EnterpriseOps.UI`, …); this sample
keeps one project with **folder-per-layer** namespaces that map 1:1 onto that split, so it runs with one
`dotnet run` (ADR-001 of Module 1).

## Student review questions, answered against this sample

**1. Can another developer use the component without reading its internals?**

Yes, and the sample measures it rather than claiming it. Both components are dropped from the Toolbox and
configured with typed properties in `InitializeComponent()`; the code-behind uses `SetItems`, `SetSegments`,
`Caption`, `Palette`, `SampleMode`, `Select` and two named events. `Packages`, `InitScript`, `WiredEvents`,
`Options`, `Call` and `CallAsync` are shadowed read-only, hidden from the Properties window, from IntelliSense
and from designer serialization. Press **Anti-pattern: leaky screen**: the gate finds all 8 kinds of
internals in the leaky screen and 0 in this one.

**2. What data crosses from server to client?**

Down: `segments[{key,label,value}]`, `palette`, `caption`, `showLegend`, `badge`, `selectedKey` and the two
diagnostics flags — nothing else. Up: `pointSelected {key,label,value,percent}` and
`error {phase,message}`. No `WorkOrder`, no tenant id, no user, no version, no correlation id. The timeline
gets `TimelineItem {At, Status, Message, Severity}`, which is the projection minus the actor. The screen
traces `DescribeWirePayload()` on every load so the claim is checkable, and the full table is
`docs/EventContract.md`.

**3. Does the component render safely in the designer?**

Both override `OnCreateControl` and turn `SampleMode` on when `DesignMode` is true. Sample mode fills fixed
data, shows an amber `DESIGN-TIME SAMPLE` badge, calls no service and cannot throw. Press **Design-time
sample mode** in the running app to see exactly what the Designer shows — and note that the trace records no
service call.

## Instructor acceptance criteria, answered

* **Follows the course architecture baseline** — folder-per-layer namespaces (`UI`, `Controls`, `Widgets`,
  `Services`, `Security`, `Domain`, `Data`); typed commands and results (`CommandResult<T>`,
  `PagedResult<T>`, `WorkQueueRow`, `HistoryEntryView`); per-session services created in the page
  constructor, no statics; one correlation id per operation, shown in the header and in every trace line.
* **UI event handlers remain thin and explainable** — the longest handler is 10 lines; each one calls a
  service or a component method inside `try/catch` and then shows the result. `ComponentApiGate` reports 0
  internals touched in `UI/WorkOrderHistoryPage.cs`.
* **Service-level logic can be reviewed without opening the designer** — `Services/WorkOrderHistoryService.cs`
  contains every read and every rule (which statuses form a reporting group, whether a segment key is real,
  what a projection carries); `Security/AccessPolicy.cs` contains the tenant check.
* **At least one failure path is demonstrated** — four in the bottom bar (bad option refused server-side,
  forged segment key refused by the service, the vendor throwing, the vendor script blocked), plus the
  tenant-denied read from the picker, plus one explicit recovery.
* **The student can explain state ownership, security implications and production behaviour** —
  `docs/EventContract.md` §4 (trust boundary) and §5 (failure semantics), `docs/WidgetWrapper.md`
  (three layers, hidden members), `docs/EmbeddedResourcePackage.md` (upgrade path and drift detection).

## Verified / unverified

**Verified** (executed in the browser while building the Application Integration and Foundations samples on
this framework build; re-used here):

* `Wisej.Web.Widget`: `Packages` (`List<Widget.Package>` with `Name`/`Source`), `InitScript` from
  `GetResourceString("<logical name>")`, `WiredEvents` as `string[]`, `Options` as a dynamic object whose
  first-level changes call `update(options, old)`, `Control.OnWidgetEvent(WidgetEventArgs)` with
  `e.Type` / dynamic `e.Data`.
* Anonymous objects assigned into `Options` are serialized with **camel-cased** property names
  (`new { Key = … }` → `key`).
* The client adapter shape: vendor object in a child of `this.container` and stored in `this.widget`;
  events through `_addListener` / `_removeListener` / `_getEventData`; a `fireWidgetEvent` raised
  synchronously *inside* `update()` is dropped, so errors are deferred; wrap — never replace — `dispose`.
* The static file server serves the project folder, so `Source = "Widgets/vendor-opschart.js"` is fetched
  as `/Widgets/vendor-opschart.js`; `Default.json` / `Web.config` are never served.
* `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`; `async void`
  handlers with `await`; fonts `new Font("default", …)` / `new Font("monospace", 9F)`;
  `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`; `DataGridView` with `AutoGenerateColumns = false` and
  explicit columns bound to a projection.

**Unverified — implemented, compiles on both target frameworks, needs a runtime look:**

* `Wisej.Web.FlowLayoutPanel` with `FlowDirection.TopDown`, `WrapContents = false` and `AutoScroll = true`
  as the row host of `StatusTimeline` (the lesson's own code sample uses `FlowLayoutPanel`, and the type and
  both properties exist in `Wisej.Framework.dll` 4.1.0). If the rows stack wrongly, dock them
  (`DockStyle.Top`, added in reverse) instead.
* `Component.DesignMode` being true inside `OnCreateControl` for a `UserControl` and for a `Widget` in the
  Wisej Designer — this is what switches `SampleMode` on. `SampleMode` is a public property, so the
  behaviour is reachable at run time either way (the **Design-time sample mode** button).
* Whether the Wisej Designer renders a `Widget` subclass live enough to show the sample chart. The
  `StatusTimeline` sample (standard controls only) certainly renders; the chart may show as an empty
  container in the Designer even though `SampleMode` is on.
* `Cursors.Hand` on a `Panel`, `Control.ToolTipText` on a custom control, and `Label.Click` bubbling from a
  child label to its row panel (the sample wires the handler on both, so it works either way).
* `ComponentResourcePackage.Verify()` reading the served files from the project folder: it resolves the root
  by walking up from `Environment.CurrentDirectory` / `AppContext.BaseDirectory` to `EnterpriseOps.csproj`,
  which is correct under `dotnet run` from the project folder. The same resolution is used by
  `ComponentApiGate` to read the two source files.
* The dark-theme rules in `opschart.css` hang off the framework's `qx-dark` body class.

Not used here, but listed in the cookbook and worth noting: `Widget.Instance` (the dynamic vendor proxy) and
the qooxdoo-class control variant (`Platform/*.js` + `[assembly: WisejResources]` + `OnWebRender`) — the
lesson's optional advanced path, which this sample deliberately does not take because a composed
`UserControl` is what the lab asks for.
