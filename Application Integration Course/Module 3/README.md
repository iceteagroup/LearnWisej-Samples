# IntegrationLab · Application Integration Course · Module 3

Local lab build for **Module 3 · Rapid Widget Integration with Wisej.Web.Widget**. It follows the
walkthrough video: a `DashboardPage` (`Wisej.Web.Page`) hosts two plain `Wisej.Web.Widget`
instances — no wrapper class — a `VendorGauge` and a jQuery-style `VendorKnob`. Each widget gets
its **Packages** (JS + CSS), its **Options** (.NET values serialized to the client) and an
**InitScript** with `init(options)` / `update(options, old)` and a method the server reaches with
`Call`. Server buttons change both widgets' Options and queue a client call.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 3/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5073
```

Then open <http://localhost:5073>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
Build check: `dotnet build -nologo -v q` (warning CS7022 about `Program.Main` is expected).

## What to try on the Dashboard

| Action | What you should see |
|---|---|
| Set "High" / Set "Peak" / Set "Idle" | the needle and the knob move, **then** the gauge flashes (High, Idle) or the knob pulses (Peak); Peak / Idle also replace the `bands` array, so the legend under the gauge is redrawn. The list on the right shows `→ .NET→JS gauge.Options {"value":88}`, `knob.Options {"level":78}` and `gauge.Call("flash") — queued to client` |
| drag / wheel the knob | `← JS→.NET knob.valueChanged {"value":63}`; the server clamps it and writes it back to `Options.level` |

The right-hand card, *⟶ Client · options & queued calls*, lists what the server sends: the full
`Options` at page load (camel-cased names such as `range.minValue`), every Options change and every
queued `Call`. A vendor failure caught by either adapter shows a red banner under the widgets.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | **Widget packages configured** | [docs/WidgetPackages.md](IntegrationLab/docs/WidgetPackages.md) · `DashboardPage.Designer.cs` (`Packages.Add`) · `wwwroot/vendor-gauge.js`, `vendor-gauge.css`, `jquery-lite.js`, `knob.css`, `vendor-knob.js` |
| 2 | **InitScript and update implementation** | [docs/InitAndUpdate.md](IntegrationLab/docs/InitAndUpdate.md) · `wwwroot/gauge-init.js`, `wwwroot/knob-init.js` (embedded, see `IntegrationLab.csproj`) |
| 3 | **Server button that changes widget options and calls the client** | [docs/ServerButton.md](IntegrationLab/docs/ServerButton.md) · `DashboardPage.cs` (`ApplySetPoint`, `QueueCall`, `button*_Click`) |

## Where things live

```
IntegrationLab/
├─ DashboardPage.cs / .Designer.cs   the Dashboard page: server buttons, two Widgets, the client list
├─ LabHelpers.cs                     EmbeddedScript.Read (InitScript from the assembly), LabJson (list JSON via the Wisej serializer)
├─ wwwroot/
│  ├─ gauge-init.js                  InitScript of the gauge widget (embedded resource)
│  ├─ knob-init.js                   InitScript of the knob widget (embedded resource)
│  ├─ vendor-gauge.js / .css         the "third-party" gauge library + its stylesheet (Packages)
│  ├─ jquery-lite.js                 jQuery-compatible subset (Package, must load before the plugin)
│  └─ vendor-knob.js / knob.css      the jQuery-style knob plugin + its stylesheet (Packages)
├─ docs/
│  ├─ WidgetPackages.md              deliverable 1
│  ├─ InitAndUpdate.md               deliverable 2
│  └─ ServerButton.md                deliverable 3
├─ Program.cs                        Application.MainPage = new DashboardPage()
└─ Startup.cs                        Kestrel host (app.UseWisej(), static files from wwwroot/)
```

## Self-check answers (lab guide)

- **When is `update` called?**
  Every time the server changes a **first-level** field of `Widget.Options` after the first render
  (`init`). It receives the full new options and the previous ones (`old`), and it is *not* called
  for a nested change (`Options.bands[0].color = …`) until `Update()` or `Options.Notify("bands")`
  is called — which is why the server buttons replace the whole `bands` array.
- **What makes Options flexible but error-prone?**
  Anything assigned to it is serialized as-is: anonymous types, arrays, nested objects — no types,
  no validation, no versioning. That gets a widget on screen in minutes, but names are camel-cased
  on the way out (`MinValue` → `minValue`), nested changes are not detected, a domain object
  dropped into it is shipped to the browser in full, and a typo is discovered only in JavaScript.
  Fine for a prototype; a reusable control moves the important settings into typed .NET
  properties (Module 4).
- **Why might a widget need to be destroyed and recreated?**
  Some vendor settings are consumed only by the constructor: the element it binds to, a rendering
  mode, a chart type. `VendorGauge` binds to its host element and has no re-parent API, so a
  `style` change that needs a different host means `destroy()` + `new VendorGauge(...)`. The
  adapter makes that explicit in `update()` — and only for that option, only when it changed.
