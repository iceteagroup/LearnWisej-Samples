# IntegrationLab · Application Integration Course · Module 8

Local lab build for **Module 8 · Data Endpoints: Postback, WebRequest & WebMethod**. It follows
the walkthrough video: a vendor grid pulls JSON from a **postback URL** handled in the widget's
`WebRequest` event, and the same grid, next to it, gets its rows from a **`[WebMethod]`** it
calls and awaits. The trace card shows every request, response, call and event in both directions.

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

| Button | Path | What you should see |
|---|---|---|
| (page load) | success | both grids show 10 work orders starting at **WO-1042**; the trace shows the postback triple `⇄ HTTP GET …&action=load… → 200 application/json → ← dataLoaded` on the left and `← WebMethod App.MainPage.GetWorkOrders → → return PageResult → ← dataLoaded` on the right; each card's status reads `● 10 rows · page 1/12 · N ms` |
| Reload both | success / recovery | `Call("reload")` on both: back to `action=load`, page size 10, page 1; banners clear |
| Next page | progress | `Call("setPage", n)` on both (wraps to 1 after page 12); each grid fetches through its own transport |
| Sort by status | progress | `Call("sort", "status")` on both; a second click toggles descending (the header shows ▴/▾); `sort` is whitelisted on the server |
| Invalid action | failure 1 (postback) | the vendor GETs `…&action=delete`; the handler answers `400 {"error":"unknown action"}`; the grid draws a red row `✖ HTTP 400 — unknown action`, the banner and status turn red, `← error {"status":400,…}` is traced |
| Oversized page | failure 2 (both) | `size=1000` on both: postback → `400 {"error":"size must be between 1 and 50"}`; WebMethod → `ArgumentException` on the server, a Wisej exception popup in the browser, the Promise resolves `null` and the adapter reports `error {"status":0,…}` |
| WebMethod target: … ▸ switch | communication | toggles `LookupWidget.DataSourceMode` between `"page"` (`App.MainPage.GetWorkOrders…`) and `"widget"` (`this.GetWorkOrders…` registered by `RegisterWebMethods`); `update(options)` is traced and the next `dataLoaded` says which target and call shape was used (`via`) and the property names that came back (`keys`) |
| Show postback URL | routing | traces `GetPostbackURL()` with the middle redacted (session-scoped, never stored) plus request/rejection counters |
| Clear trace | — | empties the trace list |

Clicking a row shows a small toast (`rowClick` event) to prove the vendor event wiring works too.

## Deliverables

| # | Deliverable | File |
|---|---|---|
| 1 | Postback WebRequest handler | [`IntegrationLab/docs/PostbackWebRequestHandler.md`](IntegrationLab/docs/PostbackWebRequestHandler.md) — code in [`Widgets/GridWidget.cs`](IntegrationLab/Widgets/GridWidget.cs) (`grid_WebRequest`) and [`wwwroot/grid-init.js`](IntegrationLab/wwwroot/grid-init.js) |
| 2 | WebMethod with arguments and return value | [`IntegrationLab/docs/WebMethod.md`](IntegrationLab/docs/WebMethod.md) — code in [`MainPage.cs`](IntegrationLab/MainPage.cs) (top-level), [`Widgets/LookupWidget.cs`](IntegrationLab/Widgets/LookupWidget.cs) (`RegisterWebMethods`) and [`wwwroot/lookup-init.js`](IntegrationLab/wwwroot/lookup-init.js) |
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
│  ├─ GridWidget.cs            postback path: WebRequest += grid_WebRequest, PostbackUrl
│  ├─ LookupWidget.cs          WebMethod path: [WebMethod] GetWorkOrders + OnWebRender/RegisterWebMethods
│  └─ GridEventArgs.cs         event payload types (data, never behavior)
├─ wwwroot/
│  ├─ vendor-grid.js / .css    the "third-party" VendorGrid library (Packages)
│  ├─ grid-init.js             adapter: transport.read.url = this.getPostbackUrl() + "&action=load"
│  └─ lookup-init.js           adapter: load(query) awaits App.MainPage.GetWorkOrdersAsync(...) or this.GetWorkOrdersAsync(...)
├─ docs/                       the three deliverables
├─ MainPage.cs / .Designer.cs  IntegrationLab — Work Orders (Application.MainPage)
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
