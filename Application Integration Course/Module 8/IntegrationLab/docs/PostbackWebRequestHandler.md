# Deliverable 1 · Postback WebRequest handler

`IntegrationLab.Widgets.GridWidget` (`Widgets/GridWidget.cs`) turns a `Wisej.Web.Widget` into a
small web service for the vendor grid it hosts. The grid owns the fetching; the widget owns the data.

## 1. The URL: a route to one component instance

Grids, pivot grids, viewers and uploaders expect a **URL**, not pushed data. Wisej.NET provides a
postback URL that already carries the identifiers the framework needs to deliver the request to
**this session** and **this component instance**:

| Side | Call | Where in this lab |
|---|---|---|
| Client (wrapper) | `this.getPostbackUrl()` | `wwwroot/grid-init.js` → `init()`: `this._baseUrl = this.getPostbackUrl()` |
| Server | `((IWisejHandler)this).GetPostbackURL()` (extension method in `Wisej.Core.IWisejHandlerExtension`) | `GridWidget.PostbackUrl` (the page does not need it: the adapter builds the URL on the client) |

The wrapper appends its own query parameter to name the action and hands the result to the vendor
as its data-source address:

```js
dataSource: { transport: { read: { url: this.getPostbackUrl() + "&action=load" } } }
```

The vendor then appends its paging arguments, so one request looks like

```
GET /…postback…?…&action=load&page=1&size=10&sort=id&desc=false
```

Two grids on the same page get two different URLs; each request lands on the component that owns
the data. The URL is **session-scoped and short-lived**: it is not a public API address, it is
never stored and never shared between users.

## 2. The handler: `WebRequest`

Subscribing `this.WebRequest += grid_WebRequest` in the constructor is what makes the widget a
postback endpoint. When a request arrives at the postback URL the event fires with the raw
`HttpRequest` and `HttpResponse` (`Wisej.Web.WebRequestEventArgs.Request / .Response`).

```csharp
private void grid_WebRequest(object sender, WebRequestEventArgs e)
{
    // 1. action: compared against a fixed list, never used to build a method name
    var action = e.Request.QueryString["action"];
    if (Array.IndexOf(KnownActions, action) < 0) { Reject(e, 400, "unknown action"); return; }

    // 2. paging parsed as bounded integers, sort mapped to a whitelist
    if (!PageRequest.TryParse(e.Request.QueryString, out var request, out var error)) { Reject(e, 400, error); return; }

    // 3. data produced in the component's context (this instance, this session)
    PageResult page = _service.LoadPage(request);

    // 4. explicit content type, then the body
    e.Response.ContentType = "application/json";
    e.Response.Write(JsonSerializer.Serialize(page, JsonOptions));
}
```

The handler runs **with the component instance**: it can read the widget's state (`PageSize`)
and the session's services (`WorkOrderService`) without any of that being passed from
the browser. That is the whole advantage over a detached controller: tenant, filters and permissions
are already known here.

Response body (`Data/PageResult.cs`, camelCase):

```json
{"rows":[{"id":"WO-1042","asset":"Boiler 3","status":"Open","priority":"High","assignee":"A. Ortega","dueDate":"2026-09-21","hours":4.5}, …],
 "total":120,"page":1,"size":10,"sort":"id","desc":false}
```

## 3. Validation table

Every data endpoint accepts input from the browser, and the browser can be scripted by anyone.
`Data/PageRequest.cs` holds the rules; the postback handler and the WebMethod both use it.

| Parameter | Raw value from | Rule | On failure |
|---|---|---|---|
| `action` | `QueryString["action"]` | must be in `KnownActions = { "load" }` (exact string compare) | `400 {"error":"unknown action"}` |
| `page` | `QueryString["page"]` | integer, `1..10000`; missing → 1 | `400 {"error":"page must be …"}` |
| `size` | `QueryString["size"]` | integer, `1..50`; missing → 10 | `400 {"error":"size must be between 1 and 50"}` |
| `sort` | `QueryString["sort"]` | mapped (case-insensitive) onto `SortableColumns = id, asset, status, priority, assignee, dueDate, hours`; missing → `id`; the canonical name is what reaches the service, and the service's `switch` is the second whitelist | `400 {"error":"sort must be one of: …"}` |
| `desc` | `QueryString["desc"]` | `"true"`/`"1"` → true, anything else → false | never fails |

Error responses carry **a status code and a short generic reason**. No exception type, no stack,
no file path, no hint about the server beyond "you asked wrong". The Network list shows the
`400` response, the vendor draws a red row `HTTP 400 — unknown action`, and the adapter raises
one `error {status:400, message, phase}` event, which the page shows as an error toast.

## 4. Content types

The content type is set **explicitly on every response**, before the body is written.

| Body | `Response.ContentType` | Note |
|---|---|---|
| JSON for a grid (this lab) | `application/json` | a JSON body served as `text/plain` or `text/html` confuses some libraries; the vendor grid here refuses anything but `application/json` |
| error JSON | `application/json` | same type as the success body, so the client parses it the same way |
| PDF / file for a viewer or download | the real MIME type (`application/pdf`, `text/csv`, …) + `Content-Disposition` via `Response.AppendHeader` | `HttpResponse.WriteFile / WriteStream / TransmitFile` |
| image for a thumbnail | `image/png` etc. | `HttpResponse.WriteImage(image, format)` |

## 5. Evidence (what the running app shows)

| Path | Action | Network list | Grid |
|---|---|---|---|
| success | page load | `⇄ HTTP [postback] GET ?…&action=load&page=1&size=10…` then `⇄ HTTP [postback] 200 application/json {"rows":10,"total":120,…}` | 10 rows, WO-1042 first, footer `page 1 / 12` |
| progress | the grid's **Next ›** button, a click on a column header | the same GET/200 pair with `page=2` / `sort=status` | page 2 / sorted by status |
| failure | re-send the grid's request from the browser's DevTools with `action=delete` or `size=1000` | `⇄ HTTP [postback] 400 application/json {"error":"unknown action"}` / `{"error":"size must be between 1 and 50"}` | unchanged; the response body in DevTools is the short JSON error |

Files: `Widgets/GridWidget.cs` (handler + `PostbackUrl`), `wwwroot/grid-init.js` (adapter),
`wwwroot/vendor-grid.js` (the "third-party" grid), `Data/PageRequest.cs` (validation),
`Data/WorkOrderService.cs` (dataset).
