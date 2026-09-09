# Deliverable 2 · WebMethod with arguments and return value

When the caller is the integration's **own JavaScript** rather than a vendor data source, a URL is
the wrong shape: the wrapper would have to build a query string and parse a raw HTTP response to
get a value. `[WebMethod]` gives the browser a **function**: typed arguments marshaled in, a
return value marshaled back, awaited by the caller.

## 1. Signature

```csharp
using Wisej.Core;

[WebMethod]
public object GetWorkOrders(int page, int size, string sort, bool desc)
```

- `Wisej.Core.WebMethodAttribute` marks a **public** method.
- Parameters are marshaled from the JavaScript arguments to the declared .NET types (`1 → int`,
  `"status" → string`, `true → bool`). **Default parameter values are not supported**: every
  argument is passed, always.
- The return value (`PageResult` here, declared as `object`) is serialized and handed to the
  caller. Return plain DTOs (`Data/PageResult.cs`): strings, numbers, lists; nothing the browser
  should not see.

Both copies of the method delegate to one implementation, `LookupWidget.ExecuteGetWorkOrders`,
so validation and tracing are identical whichever one the client calls.

## 2. Where a WebMethod can live

| Placement | Registration | Client call | In this lab |
|---|---|---|---|
| **Static** method in `Program` (or any static class) | automatic | `App.MethodName(...)` | not used |
| **Instance** method on a **top-level container** (Page, Form, Desktop) | automatic: Wisej searches top-level containers | `App.MainPage.MethodName(...)` for `Application.MainPage`; other top-level containers are reachable through the `App` namespace by name | (a) `MainPage.GetWorkOrders` — `MainPage.cs` |
| **Instance** method on a **child control** | manual: override `OnWebRender(dynamic config)`, call `base.OnWebRender((object)config)` then `RegisterWebMethods(config)`; the render carries `webMethods: ["GetWorkOrders"]` | `this.MethodName(...)` from the widget's own InitScript (`this` is the wrapper = the component) | (b) `LookupWidget.GetWorkOrders` — `Widgets/LookupWidget.cs` |

The **WebMethod target** button switches `LookupWidget.DataSourceMode` between `"page"` (a) and
`"widget"` (b); the `dataLoaded` trace line reports which one was used in its `via` field.

## 3. Marshaling on the client — the two call shapes

Verified in the framework's client code (`Wisej.Core.registerWebMethods`): for every registered
method `Name`, Wisej creates **two** functions on the target object:

```js
// 1. Promise style — resolves with the marshaled return value
var result = await App.MainPage.GetWorkOrdersAsync(page, size, sort, desc);

// 2. Trailing-callback style — the first function argument becomes the callback
App.MainPage.GetWorkOrders(page, size, sort, desc, function (result) { grid.dataSource.data(result.rows); });
```

Either way the call goes out as an HTTP request of type `methodCall` with
`{ targetId, methodName, parameters }`, and the callback / Promise receives the response's
`returnValue`. The adapter (`wwwroot/lookup-init.js`, `_callWebMethod`) prefers the `…Async`
Promise, falls back to the callback style, and reports an `error` if neither function exists on the
chosen target — so the page tells you which shape worked.

Wisej components passed as arguments are marshaled by id; everything else is sent as JSON.

## 4. How errors surface

The WebMethod applies the same rules as the postback handler (`Data/PageRequest.cs`: bounded
`page`/`size`, whitelisted `sort`) and throws `ArgumentException` on bad input.

| | Postback handler | WebMethod |
|---|---|---|
| Rejection travels as | HTTP status code `400` + short JSON body | a Wisej **exception action** in the response |
| Client sees | `fetch()` response with `response.ok === false`; the vendor parses `{"error"}` and draws the red row | the framework's default exception handler shows a message popup (`Wisej.onException`); the awaited Promise / callback receives **`null`** — there is no status code |
| Adapter reaction | `error {status:400, message:"size must be between 1 and 50"}` | the `load()` function treats `null` as failure: `error {status:0, message:"WebMethod returned null: the server rejected the call …"}` |
| Lab UI | red row + banner `✖ HTTP 400: …` | red row + banner `✖ WebMethod returned null …` plus the Wisej popup |

Because a WebMethod has no status code to inspect, an endpoint that must report *why* a call was
refused should return a result object with an error field rather than throw. This lab throws on
purpose, to make the difference visible.

## 5. Evidence (what the running app shows)

| Path | Click | Trace lines |
|---|---|---|
| success (a) | (page load) / **Reload both** | `← JS→.NET [webmethod] WebMethod App.MainPage.GetWorkOrders {"page":1,"size":10,"sort":"","desc":false}` → `→ .NET→JS [webmethod] return PageResult {"rows":10,"total":120,…}` → `← JS→.NET [webmethod] dataLoaded {…,"via":"App.MainPage.GetWorkOrdersAsync (top-level Page) → Promise","keys":"rows,total,page,size,sort,desc"}` |
| success (b) | **WebMethod target …switch to widget** | `→ .NET→JS [webmethod] update(options) {"dataSourceMode":"widget"}` then the same triple with `WebMethod this.GetWorkOrders (LookupWidget)` and `via: "this.GetWorkOrdersAsync (LookupWidget, RegisterWebMethods) → Promise"` |
| progress | **Next page**, **Sort by status** | `Call("setPage") 2` / `Call("sort") "status"` followed by the call/return/dataLoaded triple |
| failure | **Oversized page** | `WebMethod … {"page":1,"size":1000,…}` → `• server [webmethod] ArgumentException size must be between 1 and 50 → client: Wisej exception popup, Promise resolves null` → `← JS→.NET [webmethod] error {"status":0,"message":"WebMethod returned null…"}` |
| recovery | **Reload both** | `Call("reload")` then call/return/dataLoaded |

The `keys` field of `dataLoaded` shows the property names of the marshaled object as the browser
received them (the adapter tolerates both `rows` and `Rows`).

Files: `MainPage.cs` (a), `Widgets/LookupWidget.cs` (b + shared implementation),
`wwwroot/lookup-init.js` (adapter with both call shapes), `Data/PageRequest.cs` (validation).
