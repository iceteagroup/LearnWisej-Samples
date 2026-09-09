# IntegrationLab · Application Integration Course · Module 4

Local lab build for **Module 4 · Reusable Widget Classes**. It follows the walkthrough video: the gauge
prototype from Module 1 becomes `IntegrationLab.Controls.SimpleGauge`, a class that derives from
`Wisej.Web.Widget`, owns its packages, InitScript and default options, exposes typed properties
(`Value`, `Minimum`, `Maximum`, `Caption`, `AnimationEnabled`, `Threshold`) and hides the escape hatches.
A demo page places two of them from the Toolbox and drives them with normal .NET properties — no
InitScript, no Packages, no vendor name on the page.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 4/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5074
```

Then open <http://localhost:5074>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes with the expected warning CS7022 (`Program.Main` ignored as entry point).

## What to click in the Demo page

| Button | Code-behind (verbatim) | Path | What you should see |
|---|---|---|---|
| Value = 72 | `simpleGauge1.Value = 72;` | success | on a fresh page only the `code-behind` trace line: the gauge is already at 72, nothing is rendered |
| Value = 90 | `simpleGauge1.Value = 90;` | success + event | `→ .NET→JS simpleGauge1.update(options) {"value":90}`, the needle sweeps, `← JS→.NET valueChanged`, `← thresholdExceeded`, **ThresholdExceeded fired in C#** → red banner + toast |
| Maximum = 120; Value = 45 | `simpleGauge1.Maximum = 120; simpleGauge1.Value = 45;` | success | `{"max":120,"warnAt":67}` and `{"value":45}` go out together; the scale re-lays out; the banner clears |
| Value = 78 | `simpleGauge1.Value = 78;` | success | ordinary update, warm band, no threshold event |
| Value = 200 (invalid) | `simpleGauge1.Value = 200;` | failure | `• server rejected  Value must be between Minimum (0) and Maximum (120).`, orange banner + toast, **no** `update(options)` line, needle unchanged |
| AnimationEnabled = false / true | `simpleGauge1.AnimationEnabled = …; simpleGauge2.AnimationEnabled = …;` | visual property | the next value jumps instead of sweeping (and back) |
| ▶ Stream both gauges | `simpleGauge1.Value = …; simpleGauge2.Value = …;` from a `Timer` | progress | 12 readings into both gauges; each crosses its own threshold once; status counts; banner clears at the end |
| Clear trace | — | — | empties the trace |

The right-hand card is the live client/server trace. The `→ .NET→JS` lines are written by the **wrapper**,
not by the page: the page does not know a single option name.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Reusable Widget-derived class | [`IntegrationLab/Controls/SimpleGauge.cs`](IntegrationLab/Controls/SimpleGauge.cs), [`IntegrationLab/Controls/GaugeEventArgs.cs`](IntegrationLab/Controls/GaugeEventArgs.cs), adapter [`IntegrationLab/wwwroot/gauge-init.js`](IntegrationLab/wwwroot/gauge-init.js) (embedded) — write-up [`docs/ReusableWidgetClass.md`](IntegrationLab/docs/ReusableWidgetClass.md) |
| 2 | Typed properties with defaults | same class — table and rationale in [`docs/TypedProperties.md`](IntegrationLab/docs/TypedProperties.md); what is hidden and why in [`docs/HiddenMembers.md`](IntegrationLab/docs/HiddenMembers.md) |
| 3 | Demo page using the class without custom InitScript | [`IntegrationLab/DemoPage.cs`](IntegrationLab/DemoPage.cs), [`IntegrationLab/DemoPage.Designer.cs`](IntegrationLab/DemoPage.Designer.cs) — evidence and button-by-button proof in [`docs/DemoPage.md`](IntegrationLab/docs/DemoPage.md) |

## Where things live (matches the video's solution tree)

```
IntegrationLab/
├─ Controls/
│  ├─ SimpleGauge.cs           the reusable class: packages, InitScript, defaults, typed properties, hidden members
│  └─ GaugeEventArgs.cs        typed EventArgs for ValueChanged / ThresholdExceeded / WidgetError (+ Trace)
├─ wwwroot/
│  ├─ gauge-init.js            client adapter (embedded resource IntegrationLab.wwwroot.gauge-init.js)
│  └─ vendor-gauge.js          the "third-party" VendorGauge 1.0 library (Package, served as /wwwroot/…)
├─ docs/
│  ├─ ReusableWidgetClass.md   deliverable 1: what moved into the class, constants, versioning, NuGet boundary test
│  ├─ TypedProperties.md       deliverable 2: property table (type, default, validation, client update, read-only)
│  ├─ HiddenMembers.md         which members are hidden, how, why; the OnConfigureOptions escape hatch
│  └─ DemoPage.md              deliverable 3: evidence the page has no InitScript; what each button proves
├─ DemoPage.cs / .Designer.cs  the Demo page: two SimpleGauge instances from the Toolbox, typed properties only
├─ Program.cs                  Application.MainPage = new DemoPage()
└─ Startup.cs                  Kestrel host (app.UseWisej())
```

## Self-check answers (lab guide)

- **Which members should be hidden from normal users of the wrapper?**
  Every member whose misuse could break the widget silently: `Packages` (resource registration),
  `InitScript` (vendor bootstrap), the raw `Options` object (untyped JSON config), `WiredEvents` (the event
  contract) and the low-level `Call`/`CallAsync`. `SimpleGauge` shadows them read-only with
  `[Browsable(false)]`, `[EditorBrowsable(Never)]` and `[DesignerSerializationVisibility(Hidden)]`, so they
  are out of the Properties window, out of IntelliSense and out of the designer file. The sanctioned escape
  hatch is `protected virtual OnConfigureOptions(dynamic options)` for derived classes.
- **What property changes should force client updates?**
  Every change to state the browser renders — here all six typed properties — but only when the value
  actually changed (the setter returns early otherwise, so nothing goes over the wire). Validation happens
  before the assignment, so an invalid value never reaches the client. Properties that make the vendor
  re-lay out (`Minimum`, `Maximum`, `Threshold`) say so in their description; a property that would force
  the vendor to be re-created would say that too and be used sparingly.
- **What belongs in the wrapper and what belongs in the application?**
  The wrapper owns the vendor: loading its files, creating and disposing the instance in a child element,
  translating typed properties to vendor options, validating ranges, forwarding vendor events as .NET
  events, sweep-vs-jump rendering. The application owns meaning: which reading each gauge shows, what
  `Threshold` means for Boiler 3, what happens when it is crossed, when to stream. The NuGet boundary test:
  if the wrapper shipped as a package to another team, nothing they need is inside it and nothing
  vendor-specific is in their pages — `DemoPage.cs` passes that test.
