# Demo script and troubleshooting notes · Operations Dashboard (Module 10)

Run: `dotnet run --urls http://localhost:5080` from `IntegrationLab/`, open <http://localhost:5080>.
The right-hand card is the live trace; every expected line below appears there.

## Demo script

| Step | Do | Expect on screen | Expected trace lines |
|---|---|---|---|
| 1 | Open the page | heatmap draws 7 × 24 coloured cells, status `● loaded`, stats `Widgets live 2`, `Requests/min 1` | `• server packages wwwroot/vendor-heatmap.css, wwwroot/vendor-heatmap.js (vendor 1.2.0, wrapper 1.0.0)` · `→ .NET→JS render → init(options) {"days":7,"hours":24,…}` · `← JS→.NET HTTP GET postback ?action=load` · `→ .NET→JS HTTP 200 application/json {"cells":[…168 cells…]}` · `← JS→.NET loaded {"count":168}` · `• server DataLoaded fired in C# count=168` |
| 2 | Click a cell (e.g. Wed 15:00) | blue banner `● Wed 15:00 → load 97.x (server value)` | `← JS→.NET cellSelected {"day":2,"hour":15,"value":…}` · `• server CellSelected fired in C# …` |
| 3 | **Highlight peak** | one cell pulses red; banner names it | `→ .NET→JS Call highlight(d,h) [d,h]` · `• server peak …` |
| 4 | **Cell count** | toast "The client widget holds 168 cells." | `→ .NET→JS CallAsync getCellCount() []` · `← JS→.NET getCellCount → return 168` · `• server CallAsync result client holds 168 cells; server holds 168` |
| 5 | **▶ Start live updates** | tile `LIVE UPDATES` → `● running n/40`; cells shift every 1.5 s; gauge needle moves; the button reads `■ Stop live updates` | `• server Application.StartTask bounded: one push every 1500 ms, at most 40 pushes, stops on page dispose` then per push: `→ .NET→JS Call setCells(cells) [168 cells, peak …]` · `→ .NET→JS setValue(…) {"value":…}` · `• server Application.Update(page) push n/40: setCells + gauge …°F in one flush` |
| 6 | Wait, or press **■ Stop live updates** | tile → `● stopped · stopped by operator` (or `· completed` after 40) | `• server task stop requested …` · `• server task stopped stopped by operator after n pushes` |
| 7 | **Create/dispose ×25** | small heatmaps flicker in the SCRATCH box (5 s); tile `DISPOSED CLEANLY` → `25/25` in green; green banner | `• server leak test create + dispose HeatmapWidget ×25 …` · `← JS→.NET EvalAsync __integrationLabDisposed 25` · `• server leak test result created 25, disposed 25, vendor instances alive 1 (expected 1), scratch loads answered n` |
| 8 | **Simulate missing vendor** | red banner `✖ init failed: VendorHeatmap not loaded — check Packages order. → fix: …`; status `● fault (second widget)`; DevTools console shows the same error under `integrationlab.controls.HeatmapWidget.js` | `• server simulate second HeatmapWidget created WITHOUT the vendor-heatmap package …` · `← JS→.NET error {"phase":"init","status":0,"message":"VendorHeatmap not loaded — check Packages order."}` · `• server LoadFailed fired in C# phase=init (broken widget)` |
| 9 | **Malformed data** | orange banner `✖ Vendor failure during load (HTTP 200): VendorHeatmap.load: the response is not valid JSON …`; the old cells stay on screen; status `● fault` | `→ .NET→JS Call loadWithAction("corrupt") …` · `← JS→.NET HTTP GET postback ?action=corrupt` · `→ .NET→JS HTTP 200 application/json {"cells": [ {"day": 0, "hour": 1, "value": } (malformed on purpose)` · `← JS→.NET error {"phase":"load","status":200,…}` · `• server LoadFailed fired in C# phase=load status=200` |
| 10 | **Reload data** | banner clears, status `● loaded` | `→ .NET→JS Call reload() []` · `← JS→.NET HTTP GET postback ?action=load` · `→ .NET→JS HTTP 200 …` · `← JS→.NET loaded {"count":168}` |
| 11 | Close the tab while live updates run | (server console) nothing thrown; the task ends with reason `page disposed` | — |

Optional: open DevTools → Network before step 1 to see the postback request (`…&action=load`, `application/json`,
~4 KB) and before step 9 to see the same URL with `action=corrupt` and a body that is not JSON.

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
| `Disposed cleanly 24/25` (or vendor instances alive > 1) after the leak test | a creation path skipped `_wire()`, or a dispose path skipped `destroy()` | every creation goes through `_wire()`, every destruction through the wrapped `dispose()`; compare `window.__integrationLabCreated/__integrationLabDisposed` and `VendorHeatmap.liveInstances()` in the console |

## How to prove a failure is vendor usage, not Wisej.NET infrastructure

Narrow the location, in this order; stop at the first step that reproduces the failure.

1. **Vendor demo, plain JavaScript.** Open a static page with `vendor-heatmap.css` + `vendor-heatmap.js`, a `<div>`,
   and the exact options the wrapper passes (copy them from the `render → init(options)` trace line). If it fails
   there, it is the vendor (or our usage of it) — no Wisej.NET code is involved yet.
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

## About the two simulations

- **Simulate missing vendor.** Wisej.NET loads packages once per page and the dashboard has already loaded
  `vendor-heatmap.js`, so "forgetting the package" cannot be reproduced literally on the same page. The DEBUG
  factory `HeatmapWidget.CreateWithMissingVendorScript()` therefore omits the package **and** prepends a preamble to
  the adapter that hides `window.VendorHeatmap` while that one widget initialises; the guard clause then sees the
  page exactly as a page without the package would. The page restores the global (`window.__restoreVendorHeatmap()`)
  as soon as the error event arrives. On a fresh page with the package really missing, the same guard produces the
  same message — that is the point.
- **Malformed data.** `action=corrupt` exists only under `#if DEBUG`. The body is syntactically invalid JSON with the
  correct content type, which is the most common real-world shape of this failure (a proxy or error page in the
  middle). The vendor's `load()` throws a message that quotes the content type and the first bytes of the body.
