# Hidden members · "easy to use, hard to misuse"

Once `SimpleGauge` owns its setup, the members the prototype edited on every page become dangerous:
a developer who edits `Packages` on one instance breaks that instance and nobody else's — the hardest
kind of bug to find. The self-check answer is: **hide every member whose misuse could break the widget
silently.**

## What is hidden, how, and why

| Member (from `Wisej.Web.Widget` / `Control`) | Hidden how | Why its misuse is silent |
|---|---|---|
| `Packages` | `public new List<Package> Packages => base.Packages;` + `[Browsable(false)]` `[EditorBrowsable(Never)]` `[DesignerSerializationVisibility(Hidden)]` | adding a second gauge library or a wrong path on one instance: that instance renders blank, others work |
| `InitScript` | same shadow, **getter only** (the base setter is gone from the public surface) | a page-level script replaces the adapter for one instance: events stop, no error |
| `Options` | same shadow, getter only, typed `object` | `options.valeu = 90` is a typo the browser ignores; `options.min = 200` bypasses the range check |
| `WiredEvents` | same shadow, getter only | removing `"thresholdExceeded"` on one instance: the .NET event never fires again, nothing says why |
| `Call(...)`, `Call(..., callback, ...)`, `CallAsync(...)` | `public new … => base.…` + `[EditorBrowsable(Never)]` | `Call("setValue", 90)` moves the needle without moving `Value`: server and browser disagree |

Effects, in order of who notices:

1. **Properties window** — `[Browsable(false)]`: the grid shows only the *Gauge* category and *Design*.
2. **Designer serialization** — `[DesignerSerializationVisibility(Hidden)]`: `DemoPage.Designer.cs` can never
   contain a `Packages`/`InitScript`/`Options` line, even if someone edits them at design time.
3. **IntelliSense** — `[EditorBrowsable(EditorBrowsableState.Never)]`: they do not appear on `simpleGauge1.`
   in another project (Visual Studio hides them for referenced assemblies).
4. **Compiler** — the shadows are read-only, so `simpleGauge1.InitScript = "…"` and
   `simpleGauge1.Options = new { … }` no longer compile against `SimpleGauge`.

The class itself always goes through `base.Packages`, `base.InitScript`, `base.WiredEvents`, `base.Options`.

## Honest limits of `new` shadows

- A shadow hides, it does not seal. `((Widget)simpleGauge1).Options = …` still compiles and still works,
  because Wisej.NET exposes those members as public, non-virtual properties on `Widget`. That is acceptable:
  the cast is a deliberate act that shows up in code review, not a slip in the Properties window.
- `Call(...)` shadows are `public new` with `[EditorBrowsable(Never)]`, not `protected`: C# does not let a
  derived class reduce the accessibility of a public base method, and a `protected new` overload would still
  leave the public base overload reachable. Hiding it from IntelliSense is the strongest available signal.
- Hiding does not remove the framework's own use of these members; Wisej.NET reads `Packages`, `InitScript`
  and `Options` on the base type when it renders. The class populates them in the constructor.

## The escape hatch that stays

```csharp
/// Called once from the constructor after the default options have been written.
protected virtual void OnConfigureOptions(dynamic options) { }
```

A derived class extends the vendor options without ever touching `Options` from application code:

```csharp
public class FahrenheitGauge : SimpleGauge
{
    protected override void OnConfigureOptions(dynamic options)
    {
        options.units = "°F";          // vendor detail, one place, reviewable
    }
}
```

`OnValueChanged`, `OnThresholdExceeded` and `OnWidgetError` are `protected virtual` for the same reason: a
derived class can intercept the events before subscribers see them.

Note for reviewers: `OnConfigureOptions` is a virtual call from a constructor. It is safe here because it
only receives the options object and the base fields are already initialized, but a derived class must not
rely on its own constructor having run yet (use field initializers, not constructor logic, for anything it
needs inside the override).

## What is *not* hidden, and why

| Member | Status | Reason |
|---|---|---|
| `Value`, `Minimum`, `Maximum`, `Caption`, `AnimationEnabled`, `Threshold` | public, categorized, described | the API |
| `ValueChanged`, `ThresholdExceeded`, `WidgetError` | public events | the API |
| `IsLoaded` | inherited, visible | harmless, occasionally useful ("has the browser created the vendor yet?") |
| `VendorPackageName`, `VendorPackageSource`, `VendorVersion` | public constants | read-only facts, useful for a health check page or a deployment script |
| `Size`, `Location`, `Anchor`, `Visible`, … | inherited from `Control` | ordinary layout; a gauge is a control like any other |

## Evidence in the running app

- Open `DemoPage.Designer.cs`: the gauge section contains only `Caption`, `Location`, `Name`, `Size`,
  `Value` and event hook-ups.
- Open `DemoPage.cs`: no `Options`, no `Call`, no `InitScript`, no `Packages` (see `DemoPage.md`).
- Try to add `this.simpleGauge1.InitScript = "x";` to `DemoPage.cs`: it does not compile.
