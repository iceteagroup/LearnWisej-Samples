# Demo script and troubleshooting notes · Operations Dashboard (Module 10)

Run: `dotnet run -f net10.0 --urls http://localhost:5080` from `IntegrationLab/`, open <http://localhost:5080>.
Keep DevTools open (Console + Network): the network panel shows every postback request, the console every
adapter error.

## Demo script

| Step | Do | Expect on screen | Expect in DevTools |
|---|---|---|---|
| 1 | Open the page | the heatmap draws 7 × 24 coloured cells; tiles `Widgets live 2`, `Requests/min 1`, `Errors 0`, `Disposed cleanly —` | Network: one `…&action=load` request → `200 application/json`, ~4 KB, `{"cells":[…168 cells…]}` |
| 2 | Click a cell (e.g. Wed 15:00) | banner `● Wed 15:00 → load 97.x` (the **server** value) | — |
| 3 | **Highlight peak** | one cell pulses red; banner `▲ Peak load: …` | — |
| 4 | **Cell count** | toast "The client widget holds 168 cells." | — |
| 5 | **▶ Start live updates** | cells shift every 1.5 s; the gauge needle moves; the button reads `■ Stop live updates` | no new postback requests: each push is `Call("setCells")` + the gauge value in one `Application.Update(page)` |
| 6 | Wait, or press **■ Stop live updates** | the button returns to `▶ Start live updates` (after 40 pushes at the latest) | — |
| 7 | **Create/dispose ×25** | small heatmaps flicker in the Create/dispose test box (~5 s); tile `Disposed cleanly` → `25/25` in green; banner `✔ Disposed cleanly 25/25` | Console: `window.__integrationLabCreated` and `window.__integrationLabDisposed` are both 25; `VendorHeatmap.liveInstances()` is 1 |
| 8 | **Reload data** | the heatmap fetches again; `Requests/min` goes up by one | Network: another `…&action=load` → `200` |
| 9 | Close the tab while live updates run | (server console) nothing thrown; the task ends with reason `page disposed` | — |

Failures show up in the banner and the `Errors` tile. To see the endpoint's validation, re-send the postback
request from the Network panel with `action=x` (answered `400 text/plain Unknown action "x".`). To see the guard
clause, remove the `vendor-heatmap` package from `HeatmapWidget` and reload the page: the console shows
`VendorHeatmap not loaded — check Packages order.` under `integrationlab.controls.HeatmapWidget.js`, and the banner
shows it as an `init` failure.

## Troubleshooting notes (symptom → cause → fix)

| Symptom | Cause | Fix |
|---|---|---|
| Console: `VendorHeatmap not loaded — check Packages order.` (error event `phase:"init"`) | the vendor script is not in `Packages`, or is listed after something that needs it, or its path 404s | add `new Package { Name = "vendor-heatmap", Source = HeatmapWidget.VendorScriptPath }` **before** anything that uses it; check the Network panel for the file |
| Console: `VendorHeatmap 1.3.0 is loaded but HeatmapWidget expects 1.2.0 …` | the vendor file was upgraded without the wrapper | bump `HeatmapWidget.VendorVersion`, run this demo script as the acceptance test |
| `Uncaught TypeError: Cannot read properties of undefined (reading 'setData')` inside a callback | **lost `this`**: a vendor callback ran with the vendor, not the wrapper, as `this` | capture `var me = this;` in `init` and use `me` in callbacks (the adapter does); never pass wrapper methods unbound |
| Widget stays blank, no error event, Network shows the postback URL with **404** | the resource path worked in development and not on the server (virtual directory, case, missing file in publish) | use application paths (`wwwroot/...`) and make sure the files are published; `Startup.cs` must serve the folder |
| Error event `phase:"load"` with `not valid JSON … Content-Type was "text/html"` | the endpoint answered an HTML error page or the wrong handler | inspect the response body in the Network panel **before** touching the server; check `ContentType = "application/json"` and that `action` is one the handler accepts |
| Error event `phase:"load"`, HTTP **400** `Unknown action "…"` | the adapter built a URL with an action the server does not allow | the server is right: only `load` is allowed; fix the client caller |
| The Designer shows the old control after a code change | the Designer caches loaded assemblies | restart Visual Studio; it is part of the discipline, not a workaround. Design-mode code must never need a service (the wrapper pushes `sampleCells`) |
| The Designer crashes / shows an exception on the control | server code threw in design mode | run the design-mode path (`DesignMode == true`) without endpoints; attach the debugger to the Designer process if needed |
| `ObjectDisposedException` from the background task, or nothing updates after the user navigated away | the task kept pushing to a disposed page/widget | check `IsDisposed` before every push, catch `ObjectDisposedException`, stop the task from the page's `Disposed` event (`LiveUpdateService` does all three) |
| Live updates seem to arrive late or in bursts | a push per property instead of one per meaningful change, or interval too short | change everything inside **one** `Application.Update(page, () => …)`; keep the interval bounded (≥ 250 ms here) |
| An event fired by the vendor during a server update never reaches C# | `fireWidgetEvent` called synchronously while `update()` is being applied is dropped | wire vendor events through `_addListener` (deferred by the framework) as the adapter does, or defer with `setTimeout(…, 0)` |
| Exception inside `OnWebRequest` shows up only as a blank widget | the framework catches handler exceptions; the browser sees a 500 with no detail | Visual Studio → Exception Settings → break when thrown for the exceptions the handler raises; watch the Network response |
| `Disposed cleanly 24/25` (or vendor instances alive > 1) after the create/dispose test | a creation path skipped `_wire()`, or a dispose path skipped `destroy()` | every creation goes through `_wire()`, every destruction through the wrapped `dispose()`; compare `window.__integrationLabCreated/__integrationLabDisposed` and `VendorHeatmap.liveInstances()` in the console |

## How to prove a failure is vendor usage, not Wisej.NET infrastructure

Narrow the location, in this order; stop at the first step that reproduces the failure.

1. **Vendor demo, plain JavaScript.** Open a static page with `vendor-heatmap.css` + `vendor-heatmap.js`, a `<div>`,
   and the exact options the wrapper passes (read them from `options` at a breakpoint in `init`, step 2). If it
   fails there, it is the vendor (or our usage of it) — no Wisej.NET code is involved yet.
2. **The injected adapter.** In DevTools → Sources find `integrationlab.controls.HeatmapWidget.js` (the `sourceURL`).
   Put a breakpoint on the first line of `init`: does `this.container` exist? is `typeof VendorHeatmap` `"function"`?
   what is in `options`? A `debugger;` statement works too — remove it before shipping.
3. **The console.** Undefined symbol at load time → package order. Method-not-found inside a callback → lost `this`.
   A guard-clause message → exactly what it says.
4. **The network panel.** A 404 → resource path. A response body that is not what the vendor expects → content type
   or serialisation shape; check it **before** touching the server.
5. **The server.** Exception settings → break when thrown; breakpoints in `OnWebRequest` / `OnWidgetEvent`;
   remember the Designer runs the same server code in design mode and caches assemblies.

If steps 1–4 are clean and step 5 shows the handler running correctly, what remains is infrastructure: session,
routing, WebSocket, or a framework bug — and you now have the evidence to report it.
