# IntegrationLab · Application Integration Course · Module 10

Local lab build for **Module 10 · Production Readiness & Capstone**. It follows the walkthrough video: an
**unfamiliar** third-party widget (VendorHeatmap, a calendar heatmap — not Kendo, not DevExtreme) is integrated as a
reusable `HeatmapWidget : Wisej.Web.Widget` wrapper with **server calls**, **events**, **postback data loading**,
**bounded background updates** (`Application.StartTask → Call → Application.Update`), a **create/dispose leak test**
and **diagnosable failure paths** — all on one "IntegrationLab — Operations Dashboard" page.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 10/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5080
```

Then open <http://localhost:5080>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes; warning CS7022 (Program.Main ignored) is expected.

## What to try in the Operations Dashboard

| Button | Path | What you should see |
|---|---|---|
| (page load) | data loading | `HTTP GET postback ?action=load` → `HTTP 200 application/json` → `loaded {"count":168}`; status `● loaded`; stats **Widgets live 2**, **Requests/min 1** |
| click a cell | event | `← JS→.NET cellSelected {day,hour,value}` → `CellSelected` in C#; blue banner with the **server** value |
| ▶ Start / ■ Stop live updates | progress (background task) | `Application.StartTask`, then every 1500 ms `Call setCells` + gauge `setValue` + `Application.Update(page)`; tile **LIVE UPDATES** shows `● running n/40` / `● stopped · reason`; bounded to 40 pushes; stops when the page closes |
| Highlight peak | server call | the server finds the max cell in **its** copy of the data and calls `highlight(day, hour)`; one cell pulses |
| Reload data | recovery | `Call reload()` → the client fetches the endpoint again; banner clears |
| Cell count | server call with return value | `await CallAsync("getCellCount")` → toast "168 cells" |
| Create/dispose ×25 | leak test | 25 heatmaps created and disposed in the SCRATCH box; `EvalAsync("return window.__integrationLabDisposed;")` → tile **DISPOSED CLEANLY 25/25**, vendor instances alive = 1 |
| Simulate missing vendor | failure 1 | a second `HeatmapWidget` without the vendor package: the adapter guard throws **"VendorHeatmap not loaded — check Packages order."** → `error {phase:"init"}` → red banner (also visible in DevTools under `integrationlab.controls.HeatmapWidget.js`) |
| Malformed data | failure 2 | the endpoint answers `action=corrupt` with invalid JSON (DEBUG only): the vendor throws inside `load()`, **one** `error {phase:"load", status:200}` event, page stays alive; **Reload data** recovers |
| Clear trace | — | empties the right-hand trace |

The right-hand card is the live client/server trace: every message in both directions, so the JSON can be compared
with the written contract.

## Deliverables

| Deliverable | File |
|---|---|
| Capstone implementation | the project itself + [`IntegrationLab/docs/CapstoneImplementation.md`](IntegrationLab/docs/CapstoneImplementation.md) (wrapper API, lifecycle, contract tables for options / calls / events / endpoint) |
| Integration ADR | [`IntegrationLab/docs/IntegrationADR.md`](IntegrationLab/docs/IntegrationADR.md) (ADR-002: Widget wrapper vs custom Control, and why) |
| Resource and security checklist | [`IntegrationLab/docs/ResourceAndSecurityChecklist.md`](IntegrationLab/docs/ResourceAndSecurityChecklist.md) (resources, versioning, security, performance, design time — each item with its evidence) |
| Demo script and troubleshooting notes | [`IntegrationLab/docs/DemoScriptAndTroubleshooting.md`](IntegrationLab/docs/DemoScriptAndTroubleshooting.md) (step-by-step with expected trace lines; symptom → cause → fix; vendor-vs-infrastructure procedure) |

## Where things live

```
IntegrationLab/
├─ Controls/
│  ├─ HeatmapWidget.cs         the capstone wrapper: typed properties, postback endpoint, calls, events, pinned versions
│  ├─ HeatmapEventArgs.cs      CellSelected / DataLoaded / LoadFailed payloads (data, never behavior)
│  ├─ TemperatureGauge.cs      the Module 1 wrapper, reused on the dashboard (fed by the same background task)
│  └─ GaugeEventArgs.cs        gauge payloads + the shared TraceEventArgs
├─ Data/
│  ├─ HeatmapCell.cs           the only shape that crosses the wire
│  └─ LoadSampleService.cs     deterministic load data: endpoint page, live batches, design-time sample
├─ Services/
│  └─ LiveUpdateService.cs     Application.StartTask loop → Application.Update(page); bounded, disposal-aware
├─ wwwroot/
│  ├─ vendor-heatmap.js/.css   the "unfamiliar" VendorHeatmap 1.2.0 library (Packages)
│  ├─ heatmap-init.js          client adapter (InitScript, embedded): guard clause, _wire(), update, dispose, calls
│  ├─ temperature-gauge.js     Module 1 adapter (embedded)
│  └─ vendor-gauge.js          VendorGauge (Package)
├─ docs/                       the four deliverables
├─ OperationsPage.cs / .Designer.cs   IntegrationLab — Operations Dashboard
├─ Program.cs                  Application.MainPage = new OperationsPage()
└─ Startup.cs                  Kestrel host (app.UseWisej(), static files)
```

## Self-check answers (lab guide)

- **How do you prove a failure is vendor usage rather than Wisej.NET infrastructure?**
  Narrow the location in order: (1) the vendor's own demo in plain JavaScript with the same options — if it fails
  there, no Wisej.NET code is involved; (2) the injected adapter, found by its `sourceURL` in DevTools, with a
  breakpoint in `init` (container present? library loaded? options right?); (3) the console — undefined symbol =
  package order, method-not-found in a callback = lost `this`, a guard message = exactly what it says; (4) the
  network panel — 404 = path, wrong body = content type or serialization; (5) only then the server, with exception
  settings set to break when thrown. What survives all five steps is infrastructure, with evidence attached.
- **What should be cleaned up when a widget is disposed?**
  The vendor instance (`destroy()`), every event handler attached to it, observers and timers (`ResizeObserver`,
  intervals), DOM nodes the adapter created (the host element), and the references that would keep them alive
  (`this.widget = null`, `this.host = null`, handler maps cleared). The leak test proves it: 25 created, 25 disposed,
  one vendor instance left (the dashboard heatmap).
- **What belongs in a production integration checklist?**
  Resources (application paths or embedded resources, pinned versions, no CDN, deterministic order, loud failure);
  versioning (vendor + wrapper versions, breaking-change notes, an upgrade test page); security (every endpoint
  validates and authorizes, no domain object on the wire, vendor HTML sanitized, client payloads never trusted as
  state); performance (one page of data on first render, events filtered on the client, bounded background updates,
  disposal that leaves nothing behind); design time (sample data in the Designer, no services, nothing thrown).
