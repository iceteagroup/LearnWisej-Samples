# Resource and security checklist · `HeatmapWidget` (VendorHeatmap 1.2.0)

The production checklist from the Module 10 lesson, applied to this integration. A reviewer can apply the same
list to any integration library; each row says **where the evidence is in this sample**.
✔ = done and demonstrable · ◐ = done with a caveat · ☐ = open, must be closed before production.

## 1. Resources

| # | Check | Status | Evidence in this sample |
|---|---|---|---|
| 1.1 | Every package path is an application path or an embedded resource | ✔ | `HeatmapWidget.VendorStylePath = "wwwroot/vendor-heatmap.css"`, `VendorScriptPath = "wwwroot/vendor-heatmap.js"`; the adapter is `EmbeddedResource IntegrationLab.wwwroot.heatmap-init.js` (csproj) |
| 1.2 | No CDN is required | ✔ | `Startup.cs` serves the project folder; nothing references an external host |
| 1.3 | Load order is deterministic | ✔ | `Packages` list order: css, then js; the adapter runs only after both (framework guarantee), and its guard throws if the order is wrong |
| 1.4 | A missing/mis-ordered resource fails loudly | ✔ | `heatmap-init.js` → `throw new Error("VendorHeatmap not loaded — check Packages order.")`; dashboard button **Simulate missing vendor** |
| 1.5 | The injected script is named in DevTools | ✔ | `//# sourceURL=integrationlab.controls.HeatmapWidget.js` (last line of the adapter) |
| 1.6 | Subresource integrity | ◐ | `Package.Integrity` left empty: same-origin application paths. Set SRI hashes if the files ever move to a CDN |

## 2. Versioning

| # | Check | Status | Evidence in this sample |
|---|---|---|---|
| 2.1 | Vendor version recorded | ✔ | `HeatmapWidget.VendorVersion = "1.2.0"`; the file header `/*! VendorHeatmap 1.2.0 */` and `VendorHeatmap.version` |
| 2.2 | Wrapper version recorded | ✔ | `HeatmapWidget.WrapperVersion = "1.0.0"` |
| 2.3 | Version mismatch is detected, not silent | ✔ | adapter guard: `VendorHeatmap.version !== options.vendorVersion` → clear error at init |
| 2.4 | Breaking-change notes | ✔ | contract tables in [CapstoneImplementation.md](CapstoneImplementation.md); bump `WrapperVersion` when a table row changes |
| 2.5 | An upgrade has a test page | ✔ | the Operations Dashboard **is** the test page: load, event, both calls, background task, leak test, both failure paths, recovery — see [DemoScriptAndTroubleshooting.md](DemoScriptAndTroubleshooting.md) |
| 2.6 | Upgrade procedure another developer can follow | ✔ | replace `vendor-heatmap.js/.css`, bump `VendorVersion`, run the demo script; if the adapter guard fires, the version constant was forgotten |

## 3. Security

| # | Check | Status | Evidence in this sample |
|---|---|---|---|
| 3.1 | Every endpoint validates its input | ✔ | `OnWebRequest`: `action` must be in `AllowedActions {"load"}`, `days` must parse and be 1..14; anything else → **400 text/plain** |
| 3.2 | Every endpoint authorizes | ☐ | the postback URL is session-bound (Wisej.NET routes it to this component in this session), but there is no role check. Add the application's authorization call at the top of `OnWebRequest` before shipping |
| 3.3 | No domain object crosses the wire | ✔ | the endpoint and `SetCells` serialise `HeatmapCell` records only (`System.Text.Json`, camelCase); events carry `{day, hour, value}` |
| 3.4 | Client payloads are never trusted as state | ✔ | `OnWidgetEvent`: day/hour re-validated against the grid; the .NET event carries the **server** value; a contract-check trace line is written when the client value differs |
| 3.5 | Raw HTML from the vendor is sanitised | ✔ (n/a) | the vendor renders SVG from numbers only; `title` is written with `textContent`, never `innerHTML` |
| 3.6 | Call arguments are validated on the server | ✔ | `Highlight(day, hour)` throws `ArgumentOutOfRangeException` before any JSON is sent; `SetCells` validates every cell |
| 3.7 | Test-only surfaces do not ship | ✔ | `action=corrupt`, `LoadWithActionForTesting`, `CreateWithMissingVendorScript` are `#if DEBUG` |
| 3.8 | Credentials | ✔ | the vendor fetches with `credentials: "same-origin"`; nothing is put in query strings except `action`/`days` |

## 4. Performance

| # | Check | Status | Evidence in this sample |
|---|---|---|---|
| 4.1 | First render loads one page of data | ✔ | `?action=load` answers exactly `days × hours` cells (168); no history, no pagination needed for this grid |
| 4.2 | Events are filtered on the client | ✔ | `WiredEvents = { cellSelected, loaded, error }`; the vendor's per-pointer-move `cellhover` is never forwarded |
| 4.3 | Background updates are bounded | ✔ | `LiveUpdateService`: one push every 1500 ms, at most 40 pushes; constructor refuses < 250 ms or > 1000 pushes |
| 4.4 | One flush per meaningful change | ✔ | `live_Updated` changes the heatmap (`Call setCells`), the gauge (`Options.value`) and labels inside **one** `Application.Update(page, …)` |
| 4.5 | Task handles a disposed widget / closed page | ✔ | loop checks `IsDisposed`, catches `ObjectDisposedException`; `OperationsPage.Disposed → _live.Dispose()` |
| 4.6 | Update without recreation where possible | ✔ | adapter `update()`: thresholds/title/palette via `setOptions`; only days/hours recreate, through the single `_wire()` |
| 4.7 | Disposal destroys the vendor and detaches everything | ✔ | adapter `dispose()`: `widget.destroy()`, `ResizeObserver.disconnect()`, handlers cleared, host removed, references nulled |
| 4.8 | Can be created and disposed repeatedly without growth | ✔ | **Create/dispose ×25** → `Disposed cleanly 25/25`, `VendorHeatmap.liveInstances() == 1`; the vendor ignores a fetch that completes after `destroy()` |
| 4.9 | Request budget visible | ✔ | stats tile **Requests/min** from `HeatmapWidget.RequestsPerMinute` |

## 5. Design time

| # | Check | Status | Evidence in this sample |
|---|---|---|---|
| 5.1 | Renders sample data in the Designer without services | ✔ | `OnWebRender`: `DesignMode` → `options.sampleCells = LoadSampleService.DesignSample(...)`; adapter uses `setData(sampleCells)` and skips `load()` |
| 5.2 | Nothing throws in design mode | ✔ | the sample path never touches the endpoint; property setters validate but the Designer defaults are valid |
| 5.3 | Escape hatches hidden | ✔ | `Packages/InitScript/Options/WiredEvents` overridden with `[Browsable(false)]`, `[EditorBrowsable(Never)]`, `[DesignerSerializationVisibility(Hidden)]` |
| 5.4 | Designer caching discipline documented | ✔ | [DemoScriptAndTroubleshooting.md](DemoScriptAndTroubleshooting.md) → "Designer shows the old control" |

## The three closing questions

- **Can the wrapper be created and disposed a hundred times without memory growth?** Yes — the ×25 test is a
  loop; set `LeakCycles` to 100 and the counters must still match. Every creation goes through `_wire()`, every
  destruction through `dispose()`, and the vendor tracks its own live instances.
- **Can the failure of the vendor script be told apart from a failure of the Wisej.NET infrastructure?** Yes —
  see the procedure in [DemoScriptAndTroubleshooting.md](DemoScriptAndTroubleshooting.md): vendor demo first,
  then the named script in DevTools, then the network panel, then the server.
- **Does another developer have everything they need to upgrade the vendor without the original author?** Yes —
  pinned versions, the contract tables, the upgrade procedure (2.6) and the demo script as the acceptance test.
