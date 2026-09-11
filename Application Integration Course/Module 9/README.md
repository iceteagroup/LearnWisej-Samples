# IntegrationLab · Application Integration Course · Module 9

Local lab build for **Module 9 · Complex Data-Bound Widgets**. It follows the walkthrough video:
an **editable grid** (`WorkOrderGrid : Widget`, Kendo-style DataSource → the widget's postback URL)
and a **read-only pivot** (`WorkOrderPivot : Widget`, DevExtreme-style CustomStore → a `[WebMethod]`
on the page) feed the **same vendor-free server contract** — `GridOperationRequest`,
`GridDataController.Handle(action, …)`, `WorkOrderStore`. Two vendors, one pattern.

The "vendor" libraries are small stand-ins written in the style of the real ones (`VendorGrid`,
`VendorPivot`); nothing is fetched from a CDN. Nothing here is deployed anywhere; it is a plain
Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 9/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5079
```

Then open <http://localhost:5079>. (Visual Studio: open `IntegrationLab.slnx`, press F5 — the
launch profile already uses port 5079.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with the expected CS7022 warning only.

## What to try on the "IntegrationLab — Data Widgets" page

Everything happens in the two widgets; the **Remote operations** card on the right lists each
operation and event as it reaches the server.

| Gesture | What you should see |
|---|---|
| (page load) | `← JS→.NET load {skip:0,take:20} → 200 (20/150)` and `← JS→.NET LoadPivot (WebMethod) {…} → 200 (5×4, N cells)` |
| the grid's ‹ › pager | `load {skip:20,take:20} → 200 (20/150)` (remote paging) |
| a click on a column header | `load {…,sort:"status asc"} → 200` (remote sort); click again for `desc` |
| double-click a cell, edit, Enter | `rowUpdated {rowKey:"WO-1043",changes:{status:"Closed"}}` (+ toast), then `update {…} → 200 WO-1043 saved` |
| type `abc` into an Hours cell | `update {…} → 400 hours must be a number …`, then `error {phase:"update",status:400,…}` and an error toast |
| **+ Add row**, fill in, save | `create {"values":{…}} → 200 → WO-1151`, followed by a reload |
| a row's ✕ | `destroy {"rowKey":"WO-1010"} → 200 WO-1010 removed (149 left)` |
| click a grid cell / a pivot cell | `cellClick {rowKey,field,value}` / `cellClick {rowKey,columnKey,value}` |

## Deliverables

| # | Deliverable | File |
|---|---|---|
| 1 | Read-only pivot integration plan | [`IntegrationLab/docs/PivotIntegrationPlan.md`](IntegrationLab/docs/PivotIntegrationPlan.md) |
| 2 | Editable grid operation contract | [`IntegrationLab/docs/GridOperationContract.md`](IntegrationLab/docs/GridOperationContract.md) |
| 3 | Load/update/insert/delete server handlers | [`IntegrationLab/docs/ServerHandlers.md`](IntegrationLab/docs/ServerHandlers.md) — code in `Widgets/WorkOrderGrid.cs` (postback), `MainPage.cs` (WebMethod), `Data/GridDataController.cs`, `Data/WorkOrderStore.cs` |
| 4 | Event payload contracts for cell click and row update | [`IntegrationLab/docs/EventPayloadContracts.md`](IntegrationLab/docs/EventPayloadContracts.md) |

## Where things live (matches the video's solution tree)

```
IntegrationLab/
├─ Contracts/
│  ├─ GridOperationRequest.cs   Skip / Take / Sort[] / Filter[]  (+ SortDescriptor, FilterDescriptor)
│  ├─ GridOperationResult.cs    Rows / Total / Skip / Take
│  ├─ RowOperationRequests.cs   RowUpdateRequest { RowKey, Changes } · RowInsertRequest { Values } · RowDeleteRequest { RowKey }
│  ├─ PivotContracts.cs         PivotRequest { RowField, ColumnField, Measure } · PivotResult · PivotCell
│  ├─ OperationResult.cs        Status / Body / Summary  · DataContractException(400|404)
│  ├─ WidgetEventArgs.cs        CellClickEventArgs · RowUpdatedEventArgs · PivotCellClickEventArgs · DataWidgetErrorEventArgs · TraceEventArgs
│  └─ JsonCodec.cs              camelCase JSON in/out, JsonElement / DynamicObject normalization
├─ Data/
│  ├─ WorkOrder.cs              the row: id, asset, status, priority, assignee, hours, site
│  ├─ WorkOrderStore.cs         in-memory store: Load / Insert / Update / Delete / Pivot with all validation
│  └─ GridDataController.cs     Handle(action, args, body) — the one handler both entry points call
├─ Widgets/
│  ├─ WorkOrderGrid.cs          typed properties, WebRequest (postback) handler, CellClick / RowUpdated events
│  ├─ GridColumn.cs             typed column definition
│  └─ WorkOrderPivot.cs         JSON-options prototype, CellClick / DataLoaded events
├─ wwwroot/
│  ├─ grid-init.js              client adapter: Kendo-style DataSource → postback URL (embedded InitScript)
│  ├─ pivot-init.js             client adapter: DevExtreme-style CustomStore → App.MainPage.LoadPivotAsync (embedded InitScript)
│  ├─ vendor-grid.js / .css     the "third-party" editable grid (Package)
│  └─ vendor-pivot.js / .css    the "third-party" read-only pivot (Package)
├─ docs/                        the four deliverables
├─ MainPage.cs / .Designer.cs   the lab page (Application.MainPage), [WebMethod] LoadPivot
├─ Program.cs                   Wisej.NET session entry point
└─ Startup.cs                   Kestrel host (app.UseWisej())
```

## Self-check answers (lab guide)

- **Why are grids harder than gauges?**
  A gauge has one value flowing out and one or two events flowing back; the whole contract fits
  in a sentence. A grid is defined by *operations on data*: load with paging/sorting/filtering,
  insert, update, delete, each with its own request and result shape, its own validation and its
  own failure modes (400, 404), plus separate concerns like layout state and export. The server
  must stay in control of every one of them, so the contract (`GridOperationRequest`, the row
  requests, the status codes) has to be designed before any vendor code is written.
- **What changes when remote operations are enabled?**
  Until then the vendor pages and sorts in memory and the server is asked once. Afterwards every
  user interaction — a header click, a page change, an edit — is a server call. The calls must be
  fast (indexes, bounded `MaxTake`), safe (whitelisted fields and operators, never the vendor's
  raw expressions) and honest about failure (a clean 4xx the vendor can show, not a stack trace).
  The Remote operations card shows exactly that: one `← JS→.NET` line per interaction.
- **Why should Kendo and DevExtreme be taught as examples, not as the whole course?**
  Their APIs differ (a DataSource with transport settings vs. a CustomStore with load/modify
  functions, a sort string vs. a JSON array) but the adapter responsibilities are identical: load
  resources in order, create the widget in its container, pass options, wire the few meaningful
  events, feed data through a contract the server owns, handle remote operations, expose a .NET API
  sized for the application. In this lab the two vendors collapse into two InitScripts of a few
  dozen lines each against one `GridDataController`; a third library would be a third InitScript,
  not a new course.
