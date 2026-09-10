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
**embedded copy is the source of truth**, and `ComponentResourcePackage.Verify()` compares the two:

* a resource declared in the list but not embedded → *"declared in the package but is not an
  EmbeddedResource in the csproj"*;
* a package with no file at the served path → *"the browser would get a 404"*;
* a served file whose text differs from the embedded copy → *"the served file was edited by hand"*.

That is what stops the failure mode the lesson names: loose files copied into each application drift out of
sync within a release.

## The three questions the lesson asks

**Can the component be added to a new project by referencing one assembly?**
Yes for the code and the adapter — they are in the assembly. The two package files must be reachable at
`/Widgets/…`; a real component library would ship them through `Widget.GetResourceURL` or a build target
that copies them, and `Verify()` is what tells you the copy is right. In this single-project sample they are
the same files on disk.

**Is the vendor version visible in the resource names and the release notes?**
`ComponentResourcePackage.VendorVersion = "1.2"`, printed in the manifest line and returned by
`WorkOrderChartWidget.VendorVersion`. The vendor also stamps its own version into the chart header
(`EnterpriseOpsChart 1.2`), so a screenshot of a bug report says which version drew it.

**Does upgrading the vendor change one folder and one list?**
Yes: `Widgets/` and the `_all` array in `ComponentResourcePackage`. The wrapper, the adapter and the csproj
entry move together; no screen changes.

## Evidence — what the running app shows

Press **Resource package**. The trace prints:

```
Package: EnterpriseOps.Components 1.2.0 · vendor EnterpriseOpsChart 1.2 · assembly EnterpriseOps
Package:   1. VendorScript vendor-opschart.js v1.2 · package 'eops-chart-vendor' → /Widgets/vendor-opschart.js · NNNN bytes embedded
Package:   2. ComponentStyle opschart.css v1.2.0 · package 'eops-chart-style' → /Widgets/opschart.css · NNNN bytes embedded
Package:   3. Adapter opschart-init.js v1.2.0 · InitScript (never served) · NNNN bytes embedded
Package: ✓ every declared resource is embedded and the served copies match
```

Then edit one character of `Widgets/opschart.css` on disk and press the button again **without rebuilding**:
the last line becomes `✗ Widgets/opschart.css on disk differs from the copy embedded in the assembly — the
served file was edited by hand`, and the status turns amber. Rebuild and it is clean again.
