# Deliverable 1 · Reusable Widget-derived class

`IntegrationLab.Controls.SimpleGauge` — `Controls/SimpleGauge.cs` — derives from `Wisej.Web.Widget`
and owns everything the Module 1 prototype (`TemperatureGauge` + `Window1`) used to spread over the
widget subclass and the page.

## What moved into the class

| In the prototype (Module 1) | In `SimpleGauge` | Why it moved |
|---|---|---|
| `Packages.Add(new Package { Name = "vendor-gauge", Source = "wwwroot/vendor-gauge.js" })` in a one-off subclass | constructor, from the constants `VendorPackageName` / `VendorPackageSource` | one place decides the vendor file and its load order; screens cannot get it wrong |
| `InitScript = GetResourceString("IntegrationLab.wwwroot.temperature-gauge.js")` | constructor, embedded resource `IntegrationLab.wwwroot.gauge-init.js` (`wwwroot/gauge-init.js`) | the adapter is versioned with the C# that depends on it |
| `WiredEvents = new[] { … }` | constructor: `valueChanged`, `thresholdExceeded`, `error` | the event contract is part of the class, not of a page |
| initial `Options` pushed by a `PushState()` helper | constructor writes the default options (`value`, `min`, `max`, `threshold`, `warnAt`, `label`, `units`, `animationEnabled`), then calls `OnConfigureOptions(options)` | a fresh instance renders sensibly without any property set |
| `Label`, `Units`, `WarnAt` typed properties | `Caption` (+ vendor `units`/`warnAt` become wrapper internals) | the public API says what the *application* needs, not what the vendor offers |
| page-level `Trace` wiring and `CorruptStateForTesting()` | `Trace` stays as a `[Browsable(false)]` diagnostic event; the corrupt-payload helper is gone | a framework class must not carry lab-only escape hatches into production |
| the page knew option names (`{"value":72}`) to log them | the wrapper reports what it renders through `Trace` | the page knows no option name at all (see `DemoPage.md`) |

The class summary (`/// <summary>`) states what the control is for and which vendor library it wraps
(`VendorGauge 1.0`, `wwwroot/vendor-gauge.js`): the Module 1 decision record (ADR-001) turned into code.

## Constants for the vendor resources

```csharp
public const string VendorPackageName   = "vendor-gauge";
public const string VendorPackageSource = "wwwroot/vendor-gauge.js";
public const string VendorVersion       = "1.0";
private const string InitScriptResourceName = "IntegrationLab.wwwroot.gauge-init.js";
```

Nothing else in the solution mentions `vendor-gauge.js` or `gauge-init.js` except the csproj line that
embeds the adapter:

```xml
<EmbeddedResource Include="wwwroot\gauge-init.js" LogicalName="IntegrationLab.wwwroot.gauge-init.js" />
```

## Versioning note

- The adapter is an **embedded resource**, so a screen can never load an adapter from a different build
  than the class it talks to. A stale copy in a `wwwroot` folder of another project is impossible.
- The vendor library is a **package** (served as `/wwwroot/vendor-gauge.js`), loaded once per page by name.
  Upgrading the vendor means: replace the file, bump `VendorVersion`, re-check the adapter against the vendor
  changelog (`setOptions`, `setValue`, event names), rebuild. Every screen picks it up on the next build; no
  screen changes.
- If a vendor upgrade changes an option name, only `gauge-init.js` (`_vendorOptions`) changes. The typed
  properties and their camel-cased option names (`value`, `min`, `max`, `label`, …) are the *wrapper's*
  contract, not the vendor's, so they stay stable.
- Rule of thumb: a change to `wwwroot/gauge-init.js` or `vendor-gauge.js` is a change to the class and gets
  the same review as `SimpleGauge.cs`.

## The "NuGet boundary test"

Imagine shipping `IntegrationLab.Controls` as a NuGet package to another team. Anything they would have to
edit *inside* the package belongs in the application; anything they would have to copy *out of* the package
into their pages belongs in the class.

| Question | Answer for `SimpleGauge` |
|---|---|
| Would the other team need to add a `Packages` line on their page? | No: the constructor does it. |
| Would they need an InitScript file in their project? | No: it is embedded in the package assembly. |
| Would they need to know the vendor option names? | No: `Value`, `Minimum`, `Maximum`, `Caption`, `AnimationEnabled`, `Threshold`. |
| Would they need to know the vendor event names? | No: `ValueChanged`, `ThresholdExceeded`, `WidgetError` are .NET events with typed `EventArgs`. |
| Could they want `°F` on the readout? | Yes, and that is the escape hatch: derive and override `OnConfigureOptions` — no edit inside the package. |
| Is there anything vendor-specific in `DemoPage.cs`? | No (see `DemoPage.md`, evidence section). |
| Is there anything application-specific in `SimpleGauge.cs`? | No: no reading source, no business threshold, no notification logic. `Threshold` is a number the *application* chooses. |

## Wrapper concerns vs application concerns

| Wrapper (`SimpleGauge` + `gauge-init.js`) | Application (`DemoPage`) |
|---|---|
| loads the vendor files, in order | decides which reading each gauge shows |
| creates the vendor instance in a child element, disposes it | decides what `Threshold` means for Boiler 3 vs Chiller 1 |
| translates typed properties to vendor options (`Caption` → `label`, `Threshold` → `threshold` + `warnAt`) | decides what happens when it is crossed (banner, alert, operator) |
| validates ranges and throws `ArgumentOutOfRangeException` | catches the exception and explains it to the user |
| forwards vendor events as .NET events with `EventArgs` | handles the .NET events like any Button click |
| sweep-vs-jump rendering (`AnimationEnabled`) | streams readings with a `Timer` |

## Evidence in the running app

- Page load: the trace shows `simpleGauge1.render → init {…}` and `simpleGauge2.render → init {…}`, two
  instances of one class rendering with different typed properties, and the server line
  `page  no InitScript, no Packages, no vendor name in DemoPage.cs`.
- Both gauges appear with their captions; the vendor library was loaded once (one package name).
- Every button produces `→ .NET→JS simpleGaugeN.update(options) {…}` lines emitted by the **wrapper**, then
  `← JS→.NET simpleGaugeN.valueChanged {…}` from the adapter and `• server simpleGaugeN.ValueChanged fired in C#`.
