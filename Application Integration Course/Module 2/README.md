# IntegrationLab · Application Integration Course · Module 2

Local lab build for **Module 2 · JavaScript Essentials**. It follows the walkthrough video: a plain
jQuery-style knob plugin is proven in an isolated HTML page first, then rewritten as the `InitScript` of a
plain `Wisej.Web.Widget` named `gaugeKnob`, with `Packages` declaring `jQuery → vendor CSS → vendor JS`
in load order and a `valueChanged` event fired from the vendor callback through a captured widget
reference (`var me = this`).

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 2/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5072
```

Then open <http://localhost:5072>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

- **Knob Demo window** — drag or wheel the Pressure knob. Every change reaches .NET as `valueChanged`;
  the status in the card shows the value the server recorded (`● 72 psi`). A vendor failure caught by the
  adapter shows a red banner.
- **DevTools (F12)** — as in the video: `app.getWidget("gaugeKnob").instance` in the Console returns the
  vendor object stored on the widget; `gauge-init.js` is listed in Sources thanks to `//# sourceURL`;
  uncomment the `debugger;` at the top of `init` to pause and inspect `this`, `options` and `this.container`.
- **Plain-JS proof** — <http://localhost:5072/wwwroot/proof/knob-proof.html>: the same plugin with no
  Wisej.NET, the callback wired the broken and the fixed way, and load-order checks.

## Where things live

```
IntegrationLab/
├─ wwwroot/
│  ├─ proof/knob-proof.html    deliverable 1: the plain-JS proof (no Wisej.NET)
│  ├─ gauge-init.js            deliverable 2: the context-safe InitScript (embedded resource)
│  ├─ jquery-lite.js           package 1 — tiny jQuery-compatible subset (window.$, $.fn)
│  ├─ vendor-knob.css          package 2 — vendor stylesheet (+ red fallback for the un-enhanced input)
│  └─ vendor-knob.js           package 3 — the "third-party" jQuery plugin, $.fn.vendorKnob
├─ docs/
│  ├─ PlainJsProof.md          deliverable 1 — what the proof page shows and how it was verified
│  ├─ ContextSafeInitScript.md deliverable 2 — closure / bind / method, annotated gauge-init.js, load order
│  └─ DebuggingNotes.md        deliverable 3 — where the instance is stored, sourceURL, debugger, symptom table
├─ Window1.cs / .Designer.cs   Knob Demo (one Wisej.Web.Widget: gaugeKnob)
├─ Program.cs                  Wisej.NET session entry point
└─ Startup.cs                  Kestrel host (app.UseWisej(), static files from the project folder)
```

## Deliverables

1. **Working plain JavaScript proof** — [`IntegrationLab/wwwroot/proof/knob-proof.html`](IntegrationLab/wwwroot/proof/knob-proof.html), described in [`IntegrationLab/docs/PlainJsProof.md`](IntegrationLab/docs/PlainJsProof.md)
2. **Context-safe Wisej.NET InitScript** — [`IntegrationLab/wwwroot/gauge-init.js`](IntegrationLab/wwwroot/gauge-init.js), explained in [`IntegrationLab/docs/ContextSafeInitScript.md`](IntegrationLab/docs/ContextSafeInitScript.md)
3. **Debugging notes showing where the widget instance is stored** — [`IntegrationLab/docs/DebuggingNotes.md`](IntegrationLab/docs/DebuggingNotes.md)

## Self-check answers (lab guide)

- **What does `var me = this;` protect against?**
  Losing the widget inside code the vendor calls back later. In an InitScript function `this` is the
  Wisej.NET widget; inside a vendor callback (or a timer, promise or fetch callback) `this` is whatever the
  caller decided — for VendorKnob the `<input>` that dispatched `knobchange`. `me` is a local variable
  captured by the closure, so it still points at the widget when the callback runs; `this.fireWidgetEvent`
  would be `undefined` there and throw `TypeError: … is not a function`. `.bind(this)` or routing through a
  widget method give the same guarantee.
- **When should you use `this.container.innerHTML`?**
  When the vendor needs a specific element you must create — an `<input>` to enhance, a `<canvas>` to draw
  on, an `<svg>` root, an element with a specific id — and only to create that child **inside**
  `this.container`, which is the element Wisej.NET reserves for the widget's content. Never on the widget's
  own root element, never to change its classes or replace it, and never to place vendor markup elsewhere in
  the document; the framework relies on the container for layout, visibility, theming and disposal, and the
  adapter's `dispose` must remove what it created.
- **Why is a plain JS proof useful before Wisej.NET integration?**
  It separates vendor problems from adapter problems. In a 100-line page with three tags you control the
  load order, the host element and the callback wiring directly, and failures are visible immediately
  (`jQuery is not defined`, an empty box because the element was wrong, a `TypeError` in the callback).
  Once the proof works, moving it into an InitScript is mechanical — the tags become `Packages`, the page
  script becomes `init`, the instance moves to `this.instance` — and anything that breaks afterwards is
  known to be an integration issue (context, container, lifecycle), not the library.
