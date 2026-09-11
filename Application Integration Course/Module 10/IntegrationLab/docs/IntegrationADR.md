# ADR-002 · Calendar heatmap: `Wisej.Web.Widget` wrapper, not a custom `Control`

**Status:** Accepted · **Date:** 2026-09-09 · **Module:** Application Integration Course, Module 10 (capstone)
**Deciders:** IntegrationLab team · **Supersedes/extends:** ADR-001 (Module 1, integration depth)
**Related:** [CapstoneImplementation.md](CapstoneImplementation.md), [ResourceAndSecurityChecklist.md](ResourceAndSecurityChecklist.md), [DemoScriptAndTroubleshooting.md](DemoScriptAndTroubleshooting.md)

## Context

The Operations Dashboard needs a line-load calendar heatmap (7 days × 24 hours) that loads its own data, is
refreshed from a background task, lets an operator click a cell, and can be pointed at the peak by the server.
The library chosen is **VendorHeatmap 1.2.0**, a JavaScript widget family nobody on the team had used before —
which is the point of the capstone: the lifecycle, event and data-loading patterns learned on Kendo and DevExtreme
must transfer to an unfamiliar library.

Wisej.NET offers two ways to host it:

| Option | What it is | Cost |
|---|---|---|
| **A. `Wisej.Web.Widget` wrapper** (reusable depth from ADR-001) | server class with typed properties + an embedded `InitScript` adapter; the framework owns the container, packages, options diffing, wired events, `Call/CallAsync`, postback | one C# file, one JS file; no client class, no theme file |
| **B. custom `Wisej.Web.Control`** (native-feeling depth) | server class + a qooxdoo client class (`Platform/*.js`) + theme mixin; the vendor is bundled with the client class; properties become qx properties with `apply` methods | a client class, `[assembly: WisejResources]`, theme work, more surface to maintain |

The architectural rule from ADR-001 still constrains both: **the server owns the application state, the browser
owns the DOM and the vendor instance; everything on the wire is data.**

## Decision drivers

1. **Reuse** – the heatmap appears on the operations dashboard now and on two line-status screens next quarter; it is not a standard surface dozens of screens will depend on for years.
2. **Data loading** – the vendor fetches from a URL: it wants an endpoint, not options, for its dataset.
3. **Server calls with return values** – "highlight the peak" and "how many cells do you hold" are imperative, not state.
4. **Theming / Designer** – it must open in the Designer without services; it does not have to match the theme pixel-for-pixel (it is a data visual with its own palette).
5. **Production readiness** – deterministic resource loading, repeated create/dispose without leaks, bounded background updates, diagnosable failures.
6. **Maintenance** – one person wrote it; another must be able to upgrade the vendor.

Principle (ADR-001): **choose the lightest integration that still supports the requirements.**

## Decision

**Option A: `HeatmapWidget : Wisej.Web.Widget`.**

| Driver | Why the Widget wrapper wins |
|---|---|
| Reuse | Typed properties (`Days`, `Hours`, `WarnAt`, `HighAt`, `Title`) and .NET events give every screen the same surface; the raw `Packages/InitScript/Options/WiredEvents` are hidden as non-browsable escape hatches. |
| Data loading | `Widget` already implements `IWisejHandler`: the wrapper answers `WebRequest` with JSON, and the adapter passes `this.getPostbackUrl() + "&action=load"` straight to the vendor as `dataUrl`. A custom Control would need its own handler plumbing. |
| Calls | `Call("highlight", d, h)` and `await CallAsync("getCellCount")` run named adapter functions with no extra client code. |
| Designer | `OnWebRender` detects `DesignMode` and pushes `sampleCells` through Options: the Designer renders a picture, never calls the endpoint, never throws. |
| Production | Packages are application paths with pinned versions; the adapter has one `_wire()` path for create and recreate, re-syncs in place on `update`, and a dispose that destroys the vendor and counts itself; the background task is bounded and disposal-aware. All of that lives in two files a reviewer can read in one sitting. |
| Maintenance | The vendor version is pinned in one constant, checked at init with a clear message, and the upgrade path is documented in the checklist. |

Option B was rejected because none of its extra powers (theme integration, a qx property system, bundling with the
client library) is a requirement, while its extra cost (a client class, theme mixin, Designer restarts on every client
change) is real. If the heatmap becomes a standard surface — many screens, theme-critical, years of life — promote it
along the ADR-001 ladder; the typed properties and the documented contract make that promotion cheap.

## Consequences

- Vendor-specific JavaScript exists in exactly one place: `wwwroot/heatmap-init.js`. Pages never touch the vendor.
- Every failure the vendor or the adapter can produce arrives as **one** `error` event with `phase`, `status`
  and a message that names the cause; the page shows it and offers recovery (`Reload()`), never a blank widget.
- The noisy `cellhover` vendor event is filtered in the adapter and never reaches the server.
- The postback endpoint is a real HTTP surface: it validates `action` and `days`, returns 400 on anything else, and
  serialises only `HeatmapCell` records.
- The wrapper cannot be themed from the theme file; palette is an option. Accepted for a data visual.

## Evidence from the lab

- Postback: `GET …&action=load` → `200 application/json` → `loaded {"count":168}` on first render.
- Server call with return value: `CallAsync("getCellCount")` → `168`.
- Event: cell click → `cellSelected` → `CellSelected` in C#, banner shows the server value.
- Background: `Application.StartTask` → `Call setCells` + gauge value → `Application.Update(page)` per push, bounded to 40.
- Create/dispose ×25: `Disposed cleanly 25/25`, vendor instances alive = 1.
- Failure path: a missing vendor script → `error {phase:"init"}` with "VendorHeatmap not loaded — check Packages order."; the banner and the `Errors` tile show it.

## Review checklist (instructor focus)

- Where does state live? `Days/Hours/WarnAt/HighAt/Title` and the dataset copy (`Cells`) on the server; the browser holds a copy for drawing only.
- Which logic belongs on the server? Validation of every property, call argument, event payload and query parameter; the peak decision; the request budget.
- Which vendor behavior is isolated in the client adapter? Instance creation, option translation, event names, `dataUrl`, resize, recreate, destroy, the guard clauses.
- What failure paths were tested? Missing vendor script (init guard), malformed endpoint response (vendor `load()`), out-of-range `Highlight` (server), disposed widget during the background task (task loop).
- What should be reviewed before production? [ResourceAndSecurityChecklist.md](ResourceAndSecurityChecklist.md); removing the DEBUG-only actions is automatic (`#if DEBUG`), the authorization hook on the endpoint is the one item marked open.
