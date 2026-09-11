# Load / update / insert / delete server handlers (Module 9, deliverable 3)

Two entry points, one handler. The vendor-facing code decides *how* a request arrives; the
controller and store decide *what* is allowed. Neither knows which vendor sent it.

```
Kendo-style DataSource ──HTTP GET/POST──▶ WorkOrderGrid.HandleWebRequest  ─┐
                                          (Widget.WebRequest, postback URL) │
                                                                            ├─▶ GridDataController.Handle(action, args, body) ─▶ WorkOrderStore
DevExtreme-style CustomStore ──WebMethod──▶ MainPage.LoadPivot              │        Load / Insert / Update / Delete / Pivot
                                          ([WebMethod] on the Page)        ─┘
```

## 1. Entry point A — postback handler (`Widgets/WorkOrderGrid.cs`, `HandleWebRequest`)

Wired in the constructor with `this.WebRequest += this.HandleWebRequest;`. Subscribing is what
makes Wisej render the wrapper's `postbackUrl`, which `grid-init.js` reads as
`this.getPostbackUrl()` and hands to the vendor's transport. Every transport call is a plain HTTP
request to that URL plus `&action=…`.

Steps, in order:

1. `action = e.Request.QueryString["action"]` — validated against the fixed list
   `load, create, update, destroy` **before** anything else is read. `pivot` is a valid controller
   action but is rejected here on purpose: it belongs to the WebMethod entry point.
2. **Verified at runtime:** Wisej.NET raises `WebRequest` for **GET** requests to the postback URL only; a POST to `postback.wx` is consumed by the framework pipeline and answered with `[{"type":0}]`. The vendor adapter therefore sends modify operations as `GET …&action=update&payload=<url-encoded JSON>` and the handler reads `e.Request.QueryString["payload"]` (the POST/`InputStream` branch is kept for hosts where a body does arrive)
   (UTF-8). `GET` has no body.
3. `DataController.Handle(action, e.Request.QueryString, body)` returns an `OperationResult`
   (`Status`, `Body`, `Summary`).
4. `e.Response.StatusCode = result.Status; e.Response.ContentType = "application/json"; e.Response.Write(json)`.
5. The operation is traced (`← JS→.NET load {skip:0,take:20,sort:"status asc"} → 200 (20/150)`)
   and `Application.Update(this)` pushes the trace line to the browser, because the postback
   thread is outside the normal Wisej request/response cycle.

Not yet verified at runtime (implemented per the cookbook and the Wisej.Framework 4.1.0 API
surface, which was checked by reflection): that the vendor's `fetch` to the postback URL raises
`WebRequest`, that `InputStream` still holds the POST body, and that non-200 status codes reach the
browser unchanged.

## 2. Entry point B — WebMethod (`MainPage.cs`, `LoadPivot`)

```csharp
[WebMethod]
public object LoadPivot(string rowField, string columnField, string measure)
{
    var args = new NameValueCollection { ["rowField"] = rowField, ["columnField"] = columnField, ["measure"] = measure };
    var result = _controller.Handle("pivot", args, null);
    AddTrace(…);                       // ← JS→.NET LoadPivot (WebMethod) {…} → 200 (5×4, 20 cells)
    return result.Body;                // { status, rowKeys, columnKeys, cells } or { status: 400, message }
}
```

`MainPage` is a `Wisej.Web.Page` and is `Application.MainPage`, so Wisej registers the method on
the client as `App.MainPage.LoadPivot(…, callback)` and `App.MainPage.LoadPivotAsync(…)` (Promise).
The pivot adapter uses the Promise form. Typed arguments replace the query string; the status is
returned inside the body because a WebMethod has no HTTP status. Default parameter values are
not supported by Wisej WebMethods, so the adapter always passes all three.

## 3. The shared handler (`Data/GridDataController.cs`)

```csharp
public OperationResult Handle(string action, NameValueCollection args, string body)
```

| action | args used | body | store call | 200 body | failures |
|---|---|---|---|---|---|
| `load` | `skip`, `take`, `sort`, `filter` | – | `Load(GridOperationRequest)` | `{ rows, total, skip, take }` | 400 bad paging / field / op |
| `create` | – | `{ values }` | `Insert(RowInsertRequest)` | `{ row }` | 400 bad field / value / JSON |
| `update` | – | `{ rowKey, changes }` | `Update(RowUpdateRequest)` | `{ row }` | 400 bad field / value / JSON, **404** unknown key |
| `destroy` | – | `{ rowKey }` | `Delete(RowDeleteRequest)` | `{ rowKey, deleted }` | 400 missing key, **404** unknown key |
| `pivot` | `rowField`, `columnField`, `measure` | – | `Pivot(PivotRequest)` | `{ status, rowField, columnField, measure, rowKeys, columnKeys, cells }` | 400 bad field / measure / same field twice |

Validation happens in the store and surfaces as `DataContractException(statusCode, message)`;
`Handle` turns it into `OperationResult.Fail(status, message)` → `{ "status": n, "message": "…" }`.
Malformed JSON is a 400 with the parser message, never a 500. Unknown actions are refused before
any parsing.

`ParseLoadArguments` is the only vendor-aware helper and it is deliberately tolerant: `sort` may be
the Kendo string (`status asc,priority desc`) or the DevExtreme JSON array
(`[{"selector":"status","desc":false}]`); `filter` is a JSON array of `{ field, op, value }`.

## 4. Store (`Data/WorkOrderStore.cs`)

In-memory, per session (one instance per `MainPage`), 150 deterministic work orders
(`WO-1001`…`WO-1150`, `Random(9)`), guarded by a lock. Update validates on a clone first so a bad
value leaves the row untouched; insert assigns the next key; delete removes by key; pivot aggregates
`Hours` (sum) or counts per (row, column) pair and returns the axes in canonical order. Replacing it
with a database changes nothing above it: the contract types are the boundary.

## 5. Evidence (what the running app shows)

Every in-grid gesture (page, sort, edit, Add row, ✕) and every pivot load writes one
`← JS→.NET <action> … → <status> <summary>` line to the Remote operations list (see
`GridOperationContract.md` §6 and `PivotIntegrationPlan.md` §7 for the exact lines). A rejected
edit (e.g. `abc` in Hours) shows a 400 produced by the store, formatted by the controller, written
by the postback handler, and reported back by the vendor as one `error` event.
