# Deliverable 3 — the embedded resource package

`EnterpriseOps.Components 1.2.0` — everything the two components need, compiled into the assembly.

Files: `Widgets/ComponentResourcePackage.cs` (the one ordered list), the `<EmbeddedResource>` block in
`EnterpriseOps.csproj`, and the three resources themselves.

## The convention

One folder per component. Versioned names for vendor libraries. **One place** that lists what is loaded and
in what order.

| # | Kind | File | Version | Delivered as | Embedded logical name |
|---|---|---|---|---|---|
| 1 | `VendorScript` | `Widgets/vendor-opschart.js` | vendor **1.2** | Widget package `eops-chart-vendor` → `/Widgets/vendor-opschart.js` | `EnterpriseOps.Widgets.vendor-opschart.js` |
| 2 | `ComponentStyle` | `Widgets/opschart.css` | package **1.2.0** | Widget package `eops-chart-style` → `/Widgets/opschart.css` | `EnterpriseOps.Widgets.opschart.css` |
| 3 | `Adapter` | `Widgets/opschart-init.js` | package **1.2.0** | `InitScript` — **never served** | `EnterpriseOps.Widgets.opschart-init.js` |

Load order matters and is not an accident: the vendor script loads first, then **the component's own
stylesheet, so its overrides win**. Packages are loaded once per page and de-duplicated by name, so ten
charts on one screen still load one copy of the vendor.

Every class name in `opschart.css` is prefixed `eops-`, so the component cannot collide with the Wisej.NET
theme or with another component on the same page. The dark-theme rules hang off the framework's `qx-dark`
body class, so the fallback list is readable in a dark theme too — the lesson's "images and styles are
theme-aware where possible".

## Why the served copies exist as well

A Widget package is fetched by the browser over HTTP, so a served copy has to exist somewhere. The
**embedded copy is the source of truth**; the files under `Widgets/` are the same files, served as they are.
Loose copies pasted into each application are what drift out of sync within a release — which is why the
list lives in one class and ships with the assembly.

## The three questions the lesson asks

**Can the component be added to a new project by referencing one assembly?**
Yes for the code and the adapter — they are in the assembly. The two package files must be reachable at
`/Widgets/…`; a real component library would ship them through `Widget.GetResourceURL` or a build target
that copies them. In this single-project sample they are the same files on disk.

**Is the vendor version visible in the resource names and the release notes?**
`ComponentResourcePackage.VendorVersion = "1.2"`, also returned by `WorkOrderChartWidget.VendorVersion`.
The vendor stamps its own version into the chart header (`EnterpriseOpsChart 1.2`), so a screenshot of a
bug report says which version drew it.

**Does upgrading the vendor change one folder and one list?**
Yes: `Widgets/` and the `_all` array in `ComponentResourcePackage`. The wrapper, the adapter and the csproj
entry move together; no screen changes.

## Evidence — what the running app shows

Open the page with the browser's developer tools on the **Network** tab: `/Widgets/vendor-opschart.js`
loads before `/Widgets/opschart.css`, and no request ever fetches `opschart-init.js` — the adapter arrives
inside the widget's `InitScript`.
