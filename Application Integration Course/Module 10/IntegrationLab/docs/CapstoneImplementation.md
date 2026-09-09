# Capstone implementation · `IntegrationLab.Controls.HeatmapWidget`

**Module:** Application Integration Course, Module 10 · **Wrapper version:** 1.0.0 · **Vendor:** VendorHeatmap 1.2.0
**Related:** [IntegrationADR.md](IntegrationADR.md) · [ResourceAndSecurityChecklist.md](ResourceAndSecurityChecklist.md) · [DemoScriptAndTroubleshooting.md](DemoScriptAndTroubleshooting.md)

## What was integrated

The capstone asks for an **unfamiliar** third-party widget family (not Kendo, not DevExtreme) wrapped as a reusable
Wisej.NET component with server calls, at least one event, and postback or WebMethod data loading.

The widget is **VendorHeatmap** (`wwwroot/vendor-heatmap.js` + `vendor-heatmap.css`): a calendar heatmap that draws a
day × hour grid of cells coloured by load, fetches its own data from a URL, and emits its own lower-case events
(`cellselect`, `loaded`, `error`, and a noisy `cellhover`). It knows nothing about Wisej.NET.

The wrapper is `HeatmapWidget : Wisej.Web.Widget`. It ships as three files:

| File | Side | Owns |
|---|---|---|
| `Controls/HeatmapWidget.cs` | server | typed properties, validation, the postback endpoint, server calls, .NET events, pinned versions |
| `wwwroot/heatmap-init.js` (embedded resource, `InitScript`) | client | the host `<div>`, the vendor instance, guard clauses, resize, recreate, dispose |
| `wwwroot/vendor-heatmap.js` / `.css` (Packages) | client | the vendor library itself, loaded once per page |

`OperationsPage` (the Operations Dashboard) is the demo screen. It also hosts the Module 1 `TemperatureGauge`
wrapper, fed by the same background task, so the finished screen is a small dashboard of integrated widgets.

## Wrapper API (what other screens use)

```csharp
var heatmap = new HeatmapWidget { Days = 7, Hours = 24, WarnAt = 60, HighAt = 85, Title = "Line load" };
heatmap.CellSelected += (s, e) => ...;           // e.Day, e.Hour, e.Value (server value), e.ReportedValue
heatmap.DataLoaded   += (s, e) => ...;           // e.Count
heatmap.LoadFailed   += (s, e) => ...;           // e.Phase ("init" | "load" | "update" | "call"), e.Status, e.Message

heatmap.Highlight(day, hour);                    // Call("highlight", day, hour)   — validated on the server first
heatmap.ClearHighlight();                        // Call("clearHighlight")
heatmap.Reload();                                // Call("reload")                — fetch the endpoint again
heatmap.SetCells(cells);                         // Call("setCells", cells)       — used by the background task
int n = await heatmap.GetCellCountAsync();       // await CallAsync("getCellCount")

heatmap.Cells;            // IReadOnlyList<HeatmapCell>: the server's copy of the data (authoritative)
heatmap.FindPeak();       // decision made on the server copy
heatmap.IsLoaded;         // the client wrapper initialised (framework)
heatmap.IsDataLoaded;     // the vendor accepted a dataset (endpoint answered, or SetCells ran)
heatmap.RequestsPerMinute;
HeatmapWidget.VendorVersion; HeatmapWidget.WrapperVersion; HeatmapWidget.VendorScriptPath; HeatmapWidget.VendorStylePath;
```

`Packages`, `InitScript`, `Options` and `WiredEvents` are overridden as **non-browsable, non-serialised** escape
hatches: they still work for advanced callers, but the Designer and IntelliSense show the typed surface only.

## Lifecycle

| Moment | Server | Client adapter (`heatmap-init.js`) | Vendor call |
|---|---|---|---|
| construct | adds the two packages (css, then js), embeds the adapter, declares `WiredEvents`, pushes Options | — | — |
| render | `OnWebRender`: design mode → `options.sampleCells`; otherwise pushes `options.postbackUrl` once as a fallback | — | — |
| packages load | — | framework loads `vendor-heatmap.css`, then `vendor-heatmap.js`, once per page | — |
| init | — | **guard clause first**: `VendorHeatmap` missing → `throw new Error("VendorHeatmap not loaded — check Packages order.")`; version mismatch → clear message. Then host `<div>` → `_wire()` → initial load | `new VendorHeatmap(host, opts)`, `load()` (or `setData(sampleCells)` in design mode) |
| listeners | — | `_addListener(name, handler)` once per wired event; handlers are kept so a recreate can re-attach them | `on("cellselect" / "loaded" / "error")` |
| update | typed setter changes a first-level Options field | `update(options, old)`: thresholds/title/palette → **re-sync in place**; days/hours → destroy + `_wire()` (**one** function for every creation) | `setOptions({...})` or `destroy()` + `new` |
| data | `OnWebRequest` answers `?action=load` with `{ cells }` | `this.getPostbackUrl() + "&action=load"` is the vendor `dataUrl` | `fetch` → `setData` → `loaded` |
| background | `Application.StartTask` → `SetCells` → `Application.Update(page)` | `setCells(cells)` | `setData(cells)` |
| resize | — | `ResizeObserver` on the host | `resize()` |
| dispose | `Dispose()` | destroys the vendor, disconnects the observer, drops handlers, removes the host, nulls references, `window.__integrationLabDisposed++` | `destroy()` |

The dispose wrapper is installed **before** anything that can fail, so a widget whose guard threw is still safe to dispose.

## Contract

### Options (server → client, compact JSON)

| Field | Type | Default | Meaning |
|---|---|---|---|
| `days` | int | 7 | day rows (1..14); a change recreates the vendor grid |
| `hours` | int | 24 | hour columns (1..24); a change recreates the vendor grid |
| `thresholds` | `{warn, high}` | `{60, 85}` | colour bands; replaced whole (nested object = first-level change) |
| `title` | string | "" | caption drawn by the vendor |
| `palette` | string[4] | cold/busy/warn/high | vendor colours |
| `vendorVersion` | string | "1.2.0" | the version the adapter insists on |
| `sampleCells` | cell[] | — | **design mode only**: rendered instead of fetching |
| `postbackUrl` | string | — | fallback for adapters without `getPostbackUrl()` |

### Calls (server → client)

| .NET method | Client function | Returns | Notes |
|---|---|---|---|
| `Highlight(day, hour)` | `highlight(day, hour)` | — | ranges validated on the server (`ArgumentOutOfRangeException`) |
| `ClearHighlight()` | `clearHighlight()` | — | |
| `Reload()` | `reload()` | — | always resets `dataUrl` to `action=load` first (recovery) |
| `SetCells(cells)` | `setCells(cells)` | — | validated on the server; server copy updated |
| `GetCellCountAsync()` | `getCellCount()` | number | `await CallAsync` |
| `LoadWithActionForTesting(action)` (DEBUG) | `loadWithAction(action)` | — | failure path |
| — | `getDiagnostics()` | `{created, disposed, vendorInstances}` | leak-test helper |

### Events (client → server, `WiredEvents`)

| Wire event | Payload | .NET event | Vendor source |
|---|---|---|---|
| `cellSelected` | `{"day":2,"hour":15,"value":97.1}` | `CellSelected(HeatmapCellEventArgs)` | `cellselect` (user click) |
| `loaded` | `{"count":168}` | `DataLoaded(HeatmapLoadedEventArgs)` | `loaded` |
| `error` | `{"phase":"init","status":0,"message":"VendorHeatmap not loaded — check Packages order."}` | `LoadFailed(HeatmapErrorEventArgs)` | vendor `error`, or the adapter itself (`_reportError`) |

`cellhover` is deliberately **not** wired: it fires on every pointer move and would be a round trip each time.
The server re-validates `day`/`hour` against its grid and logs a contract-check line when the reported value
differs from its own copy; the .NET event always carries the server value.

### Endpoint (postback)

| | |
|---|---|
| URL | `((IWisejHandler)widget).GetPostbackURL()` on the server = `this.getPostbackUrl()` on the client, plus `&action=load` |
| Method | GET (the vendor uses `fetch` with `credentials: "same-origin"`) |
| Query | `action` ∈ `{ "load" }` (required); `days` optional integer 1..14 |
| 200 | `application/json` · `{"cells":[{"day":0,"hour":0,"value":6.2}, …]}` — one page of data (`days × hours` cells) |
| 400 | `text/plain` · `Unknown action "x".` / `days must be an integer between 1 and 14.` |
| DEBUG | `action=corrupt` → `application/json` with an invalid body (the "Malformed data" failure path) |

Only `HeatmapCell` records are serialised (`System.Text.Json`, camelCase). No domain object crosses the wire.

## Evidence (what the running dashboard shows)

- **Load**: trace `← JS→.NET HTTP GET postback ?action=load` → `→ .NET→JS HTTP 200 application/json {"cells":[…168 cells…]}` → `← JS→.NET loaded {"count":168}` → `• server DataLoaded fired in C#`; status `● loaded`.
- **Event**: clicking a cell → `← JS→.NET cellSelected {...}` → banner with the **server** value.
- **Calls**: Highlight peak → `→ .NET→JS Call highlight(d,h)`; Cell count → `→ .NET→JS CallAsync getCellCount()` then `← JS→.NET getCellCount → return 168`.
- **Background**: Start live updates → `• server Application.StartTask …` then, every 1500 ms, `→ .NET→JS Call setCells(cells)`, `→ .NET→JS setValue(…)` (gauge) and `• server Application.Update(page) push n/40 …`; stops after 40 pushes, on the button, or when the page closes.
- **Leak test**: Create/dispose ×25 → `Disposed cleanly 25/25` in the stats strip; trace shows `created 25, disposed 25, vendor instances alive 1`.
- **Failures**: Simulate missing vendor → `error {"phase":"init", … "VendorHeatmap not loaded — check Packages order."}`; Malformed data → `error {"phase":"load","status":200, … "not valid JSON …"}`; Reload data recovers both.
