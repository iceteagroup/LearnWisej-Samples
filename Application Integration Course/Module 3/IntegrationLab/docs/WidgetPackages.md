# Deliverable 1 · Widget packages configured

Where: `DashboardPage.Designer.cs` (the two `Packages.Add(...)` blocks), files in `wwwroot/`.

A `Wisej.Web.Widget` declares its dependencies through its `Packages` collection
(`List<Widget.Package>`, each with `Name`, `Source`, optional `Integrity`). Wisej.NET loads every
package **once per browser session, in list order, before the InitScript runs**. Scripts and
stylesheets are declared the same way, so the vendor CSS travels with the vendor JavaScript and is
not forgotten on the next screen.

## gauge (`Wisej.Web.Widget`, InitScript `gauge-init.js`)

| # | Name | Source | Why |
|---|---|---|---|
| 1 | `vendor-gauge` | `wwwroot/vendor-gauge.js` | the third-party library: global `VendorGauge` constructor |
| 2 | `vendor-gauge-css` | `wwwroot/vendor-gauge.css` | styles for the host the adapter builds (`div.gauge-frame`, `div.gauge-host`, the band legend) and the `flash` animation that `Call("flash")` triggers |

No ordering constraint between the two: the script does not read the stylesheet. They are listed
in "library first" order by habit.

```csharp
this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge",     Source = "wwwroot/vendor-gauge.js" });
this.gauge.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-gauge-css", Source = "wwwroot/vendor-gauge.css" });
```

## knob (`Wisej.Web.Widget`, InitScript `knob-init.js`)

| # | Name | Source | Why |
|---|---|---|---|
| 1 | `jquery` | `wwwroot/jquery-lite.js` | **must be first** — `vendor-knob.js` registers itself as `$.fn.vendorKnob` and throws `ReferenceError: jQuery is not defined` if jQuery is not loaded yet |
| 2 | `knob-css` | `wwwroot/knob.css` | styles for `div.knob-host` / the hidden `<input>` the plugin enhances |
| 3 | `vendor-knob` | `wwwroot/vendor-knob.js` | the jQuery-style plugin |

```csharp
this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "jquery",      Source = "wwwroot/jquery-lite.js" });
this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "knob-css",    Source = "wwwroot/knob.css" });
this.knob.Packages.Add(new Wisej.Web.Widget.Package { Name = "vendor-knob", Source = "wwwroot/vendor-knob.js" });
```

`jquery-lite.js` is a tiny jQuery-compatible subset (`window.$`, `$.fn` plugins, `.on/.off/.data`);
the real `jquery.min.js` can replace it without touching the plugin or the InitScript.

## Rules applied

- **Each widget lists everything it needs itself.** Two widgets on the same page can list the same
  package and the framework loads it once (cached by name). Ordering *across* widgets is not
  guaranteed, so the knob never relies on "someone else already loaded jQuery".
- **Order inside one list is the load order.** That is the only place the jQuery-before-plugin
  requirement can be expressed.
- **No CDN.** Every file lives under the application's own path (`wwwroot/`, served by
  `Startup.cs` → `UseFileServer`) so an outage or a silent version bump of a public CDN cannot
  break the dashboard. Embedding them as resources would be the other acceptable option.
- **The InitScripts are not packages.** They are embedded resources (`IntegrationLab.csproj`)
  read with `EmbeddedScript.Read(...)` and assigned to `InitScript`; the framework injects them
  after the packages have loaded.

## Evidence

- Open the browser dev tools, Network tab, reload: `vendor-gauge.js`, `vendor-gauge.css`,
  `jquery-lite.js`, `knob.css`, `vendor-knob.js` are fetched once each, then never again while
  the session lives.
- The band legend under the gauge has rounded chips and the gauge outline flashes blue on
  **Set "High"** — both come from `vendor-gauge.css`, proving the stylesheet travelled with the
  widget.
- Swap packages 1 and 3 of the knob and reload: the knob adapter reports
  `error {phase:"init", message:"jQuery is not defined …"}` in the trace and the page stays alive.
