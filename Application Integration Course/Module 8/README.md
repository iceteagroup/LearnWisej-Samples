# IntegrationLab · Application Integration Course · Module 8

Local lab build for **Module 8 · Data Endpoints: Postback, WebRequest & WebMethod**. It follows
the walkthrough video: a vendor grid pulls JSON from a **postback URL** handled in the widget's
`WebRequest` event, and the same grid, next to it, gets its rows from a **`[WebMethod]`** it
calls and awaits. The **Network** card lists every request, response and call.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 8/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5078
```

Then open <http://localhost:5078>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
Build check: `dotnet build -nologo -v q` (warning CS7022 about `Program.Main` is expected).

## What to try in the Work Orders page

| Action | What you should see |
|---|---|
| (page load) | both grids show 10 work orders starting at **WO-1042**, footer `page 1 / 12`; the Network card shows `⇄ HTTP [postback] GET …&action=load…` → `200 application/json` for the left grid and `← [webmethod] App.MainPage.GetWorkOrders {…}` → `→ return PageResult {…}` for the middle one |
| the grid's **‹ Prev / Next ›** buttons | each grid fetches the next page through its own transport (GET with `page=2` / a WebMethod call with `"page":2`) |
| a click on a column header | sorts by that column (a second click toggles descending); `sort` is whitelisted on the server |
| a click on a row | a small toast names the selected work order (`rowClick` event) |

Failures surface as an error toast plus the vendor grid's red error row. To see the rejections,
re-send the grid's postback request from DevTools with `action=delete` or `size=1000` (answered
`400 {"error"}`), or call `App.MainPage.GetWorkOrdersAsync(1, 1000, "", false)` in the console
(`ArgumentException`, Wisej popup, the Promise resolves `null`).

## Deliverables

| # | Deliverable | File |
|---|---|---|
| 1 | Postback WebRequest handler | [`IntegrationLab/docs/PostbackWebRequestHandler.md`](IntegrationLab/docs/PostbackWebRequestHandler.md) — code in [`Widgets/GridWidget.cs`](IntegrationLab/Widgets/GridWidget.cs) (`grid_WebRequest`) and [`wwwroot/grid-init.js`](IntegrationLab/wwwroot/grid-init.js) |
| 2 | WebMethod with arguments and return value | [`IntegrationLab/docs/WebMethod.md`](IntegrationLab/docs/WebMethod.md) — code in [`MainPage.cs`](IntegrationLab/MainPage.cs), [`Widgets/LookupWidget.cs`](IntegrationLab/Widgets/LookupWidget.cs) and [`wwwroot/lookup-init.js`](IntegrationLab/wwwroot/lookup-init.js) |
| 3 | Comparison note: URL data source vs client callback | [`IntegrationLab/docs/ComparisonNote.md`](IntegrationLab/docs/ComparisonNote.md) |

## Where things live (matches the video's solution tree)

```
IntegrationLab/
├─ Data/
│  ├─ WorkOrder.cs             entity + the WorkOrderRow DTO mapping
│  ├─ WorkOrderService.cs      120 deterministic work orders, LoadPage(page, size, sort, desc)
│  ├─ PageRequest.cs           the validation both endpoints share (bounds + sort whitelist)
│  └─ PageResult.cs            wire shape {rows,total,page,size,sort,desc}
├─ Widgets/
│  ├─ GridWidget.cs            postback path: WebRequest += grid_WebRequest
│  ├─ LookupWidget.cs          WebMethod path: ExecuteGetWorkOrders, called by MainPage.GetWorkOrders
│  └─ GridEventArgs.cs         event payload types (data, never behavior)
├─ wwwroot/
│  ├─ vendor-grid.js / .css    the "third-party" VendorGrid library (Packages)
│  ├─ grid-init.js             adapter: transport.read.url = this.getPostbackUrl() + "&action=load"
│  └─ lookup-init.js           adapter: load(query) awaits App.MainPage.GetWorkOrdersAsync(...)
├─ docs/                       the three deliverables
├─ MainPage.cs / .Designer.cs  IntegrationLab — Work Orders (Application.MainPage, [WebMethod] GetWorkOrders)
├─ Program.cs                  Wisej.NET session entry point
└─ Startup.cs                  Kestrel host (app.UseWisej())
```

## Self-check answers (lab guide)

- **When is postback better than WebMethod?**
  When the widget owns the fetching and wants a URL it controls (data-source grids, pivot grids,
  viewers, uploaders), when the response is a file, image or stream rather than a value, or when
  the request must reach one component instance and use its state. WebMethod is the cleaner shape
  when your own code needs to call a server method and await a value.
- **Why should action parameters be validated?**
  The browser can be scripted by anyone, so `action` is untrusted input. Compared against a fixed
  list (`{ "load" }`) it can only select a known behavior; used to build a method name or a query
  it would let the caller choose what runs on the server. The same goes for paging (bounded
  integers) and sort (whitelisted columns).
- **What content type should JSON responses use?**
  `application/json`, set explicitly on every response before the body is written.
