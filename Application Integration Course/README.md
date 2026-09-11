# Application Integration Course · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide
and walkthrough video. Each screen shows what the module's lab and video build, and nothing more:
the widget(s), and a message log only where the video shows one. Each folder has its own
`README.md` (what to click, self-check answers) and a `docs/` folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · Integration Architecture & Vocabulary | `Module 1` | `TemperatureGauge : Widget` (Sensor Monitor) with the Server ⇄ Client trace and the five paths the AI exercise asks for: set value, stream, rejected value, corrupt payload, resync; ADR + triage table + diagram | `dotnet run -f net10.0 --urls http://localhost:5071` |
| 2 · JavaScript Essentials | `Module 2` | plain-JS proof page (`/wwwroot/proof/knob-proof.html`), a context-safe knob InitScript with its packages in order, debugging notes | `http://localhost:5072` |
| 3 · Rapid Widget Integration | `Module 3` | two one-off `Wisej.Web.Widget`s (gauge + jQuery-style knob), Packages / Options / init + update, server buttons that change Options and queue a client `Call` | `http://localhost:5073` |
| 4 · Reusable Widget Classes | `Module 4` | `IntegrationLab.Controls.SimpleGauge` with typed properties and hidden internals, on a demo page with no InitScript | `http://localhost:5074` |
| 5 · Custom Controls & Theming | `Module 5` | `SimpleGaugeControl : Control` + `/Platform` qx class + theme mixin, an operations dashboard that streams on load | `http://localhost:5075` |
| 6 · Client-Server Calls | `Module 6` | `Call`, `CallAsync`, a small DTO for the client state, with the command trace | `http://localhost:5076` |
| 7 · Events & Contracts | `Module 7` | gauge / knob / chart events through `OnWidgetEvent`, page-level `WidgetEvent` and `OnWebEvent`, payload contract, WidgetEvent log | `http://localhost:5077` |
| 8 · Data Endpoints | `Module 8` | postback `WebRequest` grid vs `[WebMethod]` grid over the same dataset, a Network list, comparison note | `http://localhost:5078` |
| 9 · Complex Data Widgets | `Module 9` | editable grid (Kendo-style transport → postback) + read-only pivot (DevExtreme-style store → WebMethod), CRUD contract, Remote operations list | `http://localhost:5079` |
| 10 · Production Readiness & Capstone | `Module 10` | `HeatmapWidget` capstone on an Operations Dashboard: postback loading, calls, events, background updates, create/dispose test, checklist + ADR | `http://localhost:5080` |

Run any module from its `IntegrationLab` project folder. The projects multi-target `net10.0-windows` and `net10.0`, so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 6/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5076
```

or open the `IntegrationLab.slnx` in the module folder with Visual Studio and press F5.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the Wisej.NET integration
conventions verified while building and running these samples (widget adapter shape, event wiring,
`Call`/`CallAsync`/`EvalAsync`, custom controls, postback and WebMethod behaviour, and the gotchas
found along the way). Read it before writing a new sample.

## Stand-in vendor libraries

The course ships no third-party packages, so each sample uses a small self-contained library that
behaves like one (`vendor-gauge.js`, `vendor-knob.js` as a jQuery plugin on `jquery-lite.js`,
`vendor-chart.js`, `vendor-grid.js`, `vendor-pivot.js`, `vendor-heatmap.js`). They have their own
element, options, methods, lower-case events and clear error messages, which is all the adapters
need. Swap in a real library and only the adapter InitScript changes.
