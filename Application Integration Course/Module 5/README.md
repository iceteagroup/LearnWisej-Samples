# IntegrationLab · Application Integration Course · Module 5

Local lab build for **Module 5 · Custom Controls & Theming**. It follows the walkthrough video: a
custom `SimpleGaugeControl` made of a server class (`Controls/SimpleGaugeControl.cs : Wisej.Web.Control`)
and a client qx class under `/Platform` (`integrationlab.controls.SimpleGaugeControl extends wisej.web.Control`),
joined to the theme by the appearance key `simplegauge`, rendering live in the Designer, and dropped
four times onto the **IntegrationLab — Operations Dashboard** page, streaming live.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 5/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5075
```

Then open <http://localhost:5075>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes; warning CS7022 (Program.Main ignored) is expected.

## What you see on the Operations Dashboard

- A KPI row (Active lines, Avg. throughput, Alerts, Uptime) and four tiles — Boiler 1, Boiler 2,
  Turbine, Coolant — each the same `SimpleGaugeControl`.
- The page streams from load (`● live`): a `Timer` drifts the four readings every 800 ms, and only
  `value` changes on the wire.
- Boiler 2, Turbine and Coolant cross their thresholds periodically (Boiler 1 never does): the client
  class fires `thresholdExceeded`, `ThresholdExceeded` is raised in C#, the tile shows a red banner and a
  red border (theme state `alarm`), the **Alerts** KPI counts up and the status reads `● alarm (n)`. The
  banner clears when the reading falls back.
- A value outside `Minimum..Maximum` is rejected by the control's setter on the server: nothing is
  rendered, the tile's banner and a toast show the message, and streaming stops.
- Theming: set `"theme"` in `Default.json` to `Material-3` or `FluentDark-5` (default `Bootstrap-4`) and
  reload. The same four controls are restyled by the theme together with every built-in control.
- Design time: open `EnterprisePage.cs` in the Wisej Designer; the gauges render live
  (`docs/DesignTimeNotes.md`).

## Where things live (matches the video's solution tree)

```
IntegrationLab/
├─ Controls/
│  ├─ SimpleGaugeControl.cs     server control: typed properties, OnWebRender, OnWebEvent, design-time support
│  └─ GaugeEventArgs.cs         event payload types (data, never behavior)
├─ Platform/
│  ├─ SimpleGaugeControl.js     client qx class (embedded, bundled by [assembly: WisejResources])
│  └─ vendor-gauge.js           the "third-party" VendorGauge library, embedded next to the class
├─ Themes/
│  └─ simplegauge.mixin.theme   appearance entry for the "simplegauge" key + colour aliases
├─ Properties/
│  ├─ AssemblyInfo.cs           [assembly: Wisej.Core.WisejResources]
│  └─ launchSettings.json       http://localhost:5075
├─ docs/
│  ├─ ServerControlClass.md     deliverable 1
│  ├─ ClientQxClass.md          deliverable 2
│  ├─ ThemeAppearanceEntry.md   deliverable 3
│  └─ DesignTimeNotes.md        deliverable 4 (screenshot to be added by the learner)
├─ EnterprisePage.cs / .Designer.cs   Operations Dashboard (Wisej.Web.Page, Application.MainPage)
├─ wwwroot/vendor-gauge.js      the untouched shared course library (not loaded by this module)
├─ Program.cs                   Application.MainPage = new EnterprisePage();
└─ Startup.cs                   Kestrel host (app.UseWisej())
```

`Platform/vendor-gauge.js` is the shared library plus one optional `colors` option, so the gauge can
be repainted from theme colours; the rest of its API is unchanged.

## Deliverables

1. **Server control class** — [`IntegrationLab/Controls/SimpleGaugeControl.cs`](IntegrationLab/Controls/SimpleGaugeControl.cs), explained in [`docs/ServerControlClass.md`](IntegrationLab/docs/ServerControlClass.md)
2. **Client qx class skeleton** — [`IntegrationLab/Platform/SimpleGaugeControl.js`](IntegrationLab/Platform/SimpleGaugeControl.js), explained in [`docs/ClientQxClass.md`](IntegrationLab/docs/ClientQxClass.md)
3. **Theme / appearance entry** — [`IntegrationLab/Themes/simplegauge.mixin.theme`](IntegrationLab/Themes/simplegauge.mixin.theme), explained in [`docs/ThemeAppearanceEntry.md`](IntegrationLab/docs/ThemeAppearanceEntry.md)
4. **Design-time screenshot or notes** — [`docs/DesignTimeNotes.md`](IntegrationLab/docs/DesignTimeNotes.md) (notes; the screenshot is taken in Visual Studio and saved as `docs/DesignTime.png`)

## Self-check answers (lab guide)

- **When is a custom control worth the extra work?**
  When the widget is part of the product platform rather than a one-off: it appears on many screens,
  it must follow the theme, it must be droppable and configurable in the Designer, and it must feel
  like a built-in control to the people who build screens with it. A `Widget` wrapper with Options
  and an InitScript is faster for prototypes; a `Control` with a `/Platform` class costs more once
  and is repaid by every screen that uses it without thinking about it.
- **What is the role of `OnWebRender`?**
  It is the render half of the contract between the two halves of the control. The server writes
  onto `config` only the properties the client class declares (camel-cased) plus the wired events —
  intentional state, not whatever happens to live on the server object. Wisej.NET diffs it against
  the previous render and sends only the changes. `OnWebEvent` is the other half; together they are
  the whole wire protocol.
- **Why does theme integration matter for enterprise adoption?**
  An appearance key makes the control belong to the design system: the theme owner restyles it from
  one JSON entry, dark and light variants come for free, the Designer preview matches runtime, and
  the control can be reused across products without code changes. A control that ignores the theme
  looks pasted in, gets forked per product, or gets avoided.

## Instructor review focus (where things are)

- **State** lives on the server (`_value`, `_minimum`, …); the browser never holds the truth.
- **Logic** on the server: validation in the setters, the alarm state, the `ThresholdExceeded` decision.
- **Vendor behaviour** isolated in the client class: creation on `appear`, apply methods, resize,
  theme colours, destruction — no application logic.
- **Failure path tested**: server rejection in the `Value` setter, caught by the page and shown on the
  tile banner. A malformed value can never reach the vendor because the typed properties are the only way in.
- **Before production**: verify the mixin is deployed with the app (`Themes/` copied to output)
  and add the Designer screenshot.
