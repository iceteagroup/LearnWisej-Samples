# ADR-001 · Widget integration approach

**Status:** Accepted · **Date:** 2026-09-09 · **Module:** Application Integration Course, Module 1
**Deciders:** IntegrationLab team · **Related:** [WidgetTriage.md](WidgetTriage.md), [ArchitectureDiagram.svg](ArchitectureDiagram.svg), [ClientServerContract.md](ClientServerContract.md)

## Context

Three third-party JavaScript widgets are candidates for the Sensor Monitor application:
a small temperature gauge, a rich text editor, and a data grid. Each one could be integrated
at a different depth. Wisej.NET offers four depths, from lightest to heaviest:

| Depth | What it is | Right for |
|---|---|---|
| **One-off** | A `Wisej.Web.Widget` dropped on a page with a script and options | prototype, internal dashboard, a widget used exactly once |
| **Reusable** | The same widget wrapped in a class with typed properties | many screens, no copied setup |
| **Themeable** | Reusable, plus appearance connected to the application theme | anything users see next to built-in controls |
| **Native-feeling** | A full custom control: server class + client class, designed, themed, event-driven | a standard surface dozens of screens depend on for years |

The architectural rule that constrains every option: **the server owns the application
state, the browser owns the DOM and the vendor instance.** Everything crossing the wire is
data (compact JSON out, small event payloads back), never behavior.

## Decision drivers

1. **Reuse** – how many screens use it now and in two years.
2. **Theming / Designer** – must it match the theme and work in the Wisej.NET Designer.
3. **Security** – does it accept user input that reaches the server.
4. **Maintenance** – who owns the vendor library version and its breaking changes.
5. **Cost of getting the depth wrong later.**

Principle: **choose the lightest integration that still supports the requirements.**
Move to a heavier depth only when a lighter one cannot meet a driver.

## Decision

| Candidate | Chosen approach | Why |
|---|---|---|
| **Small gauge** | **One-off → Reusable** (`TemperatureGauge : Widget`, built in this lab) | Display-only, a few numbers in, one meaningful event out. A one-off widget would do for a single dashboard, but the same gauge is already wanted on the boiler and chiller screens, so it is wrapped once in a class with typed properties (`Value`, `Minimum`, `Maximum`, `WarnAt`, `Threshold`) and reused. No theming work: the vendor colors are acceptable for an operations dashboard. |
| **Rich text editor** | **Reusable + Themeable** wrapper | Appears in several forms (work orders, shift notes) and sits next to built-in inputs, so it must follow the theme. It accepts **HTML user content**: the server must sanitize what comes back, and the security boundary is documented as part of the contract before the wrapper ships. Not native-feeling: the editor is a leaf control, not a standard surface. |
| **Data grid** | **Prefer the built-in `DataGridView`**; if a vendor grid is truly required, build it **native-feeling** | Data-bound, sortable, paged, heavy. Wisej.NET already ships a native grid that is themed, designable and event-driven. Integrate a vendor grid only for a feature the native control lacks, and then invest fully (server class + client class) because dozens of screens and years of maintenance will depend on it. A half-wrapped grid is the most expensive mistake on this list. |

## Consequences

- Every widget has a written client/server contract (state fields in, event names and payloads
  out, lifecycle, failure handling). Reviewers compare the JSON in the browser tools against it.
- The vendor object always lives **inside a container element the Wisej.NET widget provides**,
  never on the framework element itself, so layout, disposal and theming keep one owner each.
- The gauge wrapper is cheap to promote later: adding theme hooks turns it themeable without
  touching the screens that use it.
- The editor cannot ship until the sanitizer and its tests exist. That is a deliberate gate.
- Choosing the native grid means no vendor license, no vendor upgrade path to maintain, and
  a Designer experience for free.

## Evidence from the lab (TemperatureGauge)

- Setting `Value = 104` on the server renders `{"value":104}` to the widget; the vendor gauge
  crosses its threshold and **one** `thresholdExceeded` event with the current value returns,
  raising `ThresholdExceeded` in C#.
- Failure path tested: an out-of-range value is rejected on the server before any JSON is
  rendered; a malformed payload is caught in the client adapter and reported as a single
  `error` event, after which the server re-renders its authoritative state.

## Review checklist (instructor focus)

- Where does state live? On the server component. The browser holds a copy for drawing only.
- Which logic belongs on the server? Validation, thresholds, notifications, everything auditable.
- Which vendor behavior is isolated in the client adapter? Instance creation, option translation,
  vendor event names, resize, destroy.
- What failure path was tested? Server rejection and vendor exception, both visible in the UI.
- What should be reviewed before production? The contract document, the sanitizer for the
  editor, and the vendor upgrade policy.
