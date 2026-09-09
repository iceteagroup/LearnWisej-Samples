# Deliverable 3 · Comparison note: URL data source vs client callback

Same read-only dataset (120 work orders), same vendor grid, two endpoints. The lab makes the
trade-off visible by putting them side by side; this note records the decision rule.

## The table

| | **Postback URL** — `WebRequest` handler | **`[WebMethod]`** — RPC-style call |
|---|---|---|
| **Best for** | grids, pivot grids, viewers, uploaders — widgets with a data-source abstraction that *own the fetching* and want an address | the integration's own code calling a server method: look up a record, validate a value, fetch a small lookup list |
| **Shape** | raw HTTP — you receive an `HttpRequest` and write the `HttpResponse` yourself | RPC — arguments and return value are marshaled for you |
| **Lives with** | the component instance (`GridWidget.grid_WebRequest`): its state and the session's services are in scope | a static method, or an instance method on a top-level container (`MainPage.GetWorkOrders`); child controls opt in with `RegisterWebMethods` |
| **Returns** | anything: JSON, a PDF stream, an image, a CSV — with the right content type and disposition | a typed object the client **awaits** (Promise or callback) |
| **Input arrives as** | query string / form / body — strings you parse and bound | typed parameters (`int page, int size, string sort, bool desc`) you still bound |
| **Errors travel as** | a status code (`400`) + optional short body | an exception action → framework popup; the Promise resolves `null` (no status code) |
| **Who calls it** | the vendor library (`fetch`/XHR with its own parameters) | your adapter / your JavaScript |

**Rule of thumb from the walkthrough:** postback when the widget wants a **URL it controls**;
WebMethod when your code wants to **call and await a value**.

## When postback beats WebMethod

1. **The vendor already has a data-source abstraction** (`transport.read.url`, `dataSource:
   { url }`, `src`, `action`): give it the URL and let it page, sort and retry its own way. Wrapping
   that in a WebMethod means re-implementing the vendor's transport in the adapter.
2. **The response is not a JSON object**: files, images, PDF streams, CSV exports, large row
   batches. A WebMethod return value must be marshaled as a value; a postback writes bytes and sets
   `Content-Type` / `Content-Disposition`.
3. **The request must land on a specific component instance** and use its state: two grids, two
   URLs, two handlers — with no correlation code in the browser.
4. **The consumer is outside your JavaScript**: an `<img src>`, an `<iframe>`, a download link, a
   viewer that streams ranges.

WebMethod wins when the caller is your own wrapper code and the answer is a value: the argument
marshaling and the awaitable return remove the URL building, the `fetch`, the status handling and
the JSON parsing from the adapter, which is exactly what the lookup grid shows.

## Both are entry points — the three security questions

Both bypass the normal event pipeline, so both need the validation and authorization a button
handler would get. Before either endpoint ships, answer, in the handler itself:

1. **Is the current user allowed to see these rows in this tenant?** Checked *in the handler*,
   from session state — never from a browser-supplied argument. (In this lab the handler runs in
   the component's context; a real app asks its authorization service here.)
2. **Are paging and filter parameters bounded and mapped to a whitelist?** `page ≤ 10000`,
   `size ≤ 50`, `sort ∈ {id, asset, status, priority, assignee, dueDate, hours}`, `action ∈
   {load}` — `Data/PageRequest.cs`, shared by both endpoints. A request for a million rows is
   refused; the sort string never reaches a query.
3. **Does an error response reveal nothing about the server beyond a status code?** The postback
   answers `400 {"error":"unknown action"}` — no exception type, no stack, no path. The WebMethod
   throws `ArgumentException` with the same short message; the framework popup is a development
   convenience and a production app plugs in `Wisej.onException` to show a neutral message.

Plus the two habits that make the comparison fair:

- **The content type is explicit on every response** (`application/json` for JSON): a JSON body
  served as text confuses some libraries, an HTML body served as JSON confuses all of them.
- **The postback URL is session-scoped and short-lived**: displayed redacted, never stored, never
  shared between users.

## Self-check answers

- **When is postback better than WebMethod?** When the widget owns the fetching and wants a URL
  (data-source grids, viewers, uploaders), when the response is a file/image/stream rather than a
  value, or when the request must reach one component instance and use its state.
- **Why should action parameters be validated?** The browser can be scripted by anyone; `action`
  is untrusted input. Compared against a fixed list it can only select a known behavior; used to
  build a method name or a query it becomes remote code/data selection.
- **What content type should JSON responses use?** `application/json`.

## Evidence

Run the app and click **Reload both**: the left trace triple is `⇄ HTTP GET … → 200
application/json → dataLoaded`, the middle one is `WebMethod … → return PageResult →
dataLoaded`. **Oversized page** shows the two error shapes side by side: `400` on the left,
`ArgumentException → popup, Promise null` on the right.
