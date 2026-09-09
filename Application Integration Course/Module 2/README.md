# IntegrationLab · Application Integration Course · Module 2

Local lab build for **Module 2 · JavaScript Essentials**. It follows the walkthrough video: a plain
jQuery-style knob plugin is proven in an isolated HTML page first, then rewritten as the `InitScript` of a
plain `Wisej.Web.Widget` named `gaugeKnob`, with `Packages` declaring `jQuery → vendor CSS → vendor JS`
in load order and a `valueChanged` event fired from the vendor callback through a captured widget
reference (`var me = this`). A second widget runs the broken script so the bug can be seen, not read about.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 2/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5072
```

Then open <http://localhost:5072>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try in the Knob Demo window

Two knobs sit side by side. Both host the same VendorKnob plugin with the same Packages and the same
Options; only the InitScript differs.

| Action | Path | What you should see |
|---|---|---|
| Drag / wheel the **left** knob (`gaugeKnob`, `gauge-init.js`) | success (events) | `← JS→.NET gaugeKnob.valueChanged {"value":…}` then `→ .NET→JS update(options) {"value":…}`; the status turns green: "valueChanged reached .NET ✓" |
| Drag / wheel the **right** knob (`gaugeKnobBroken`, `gauge-init.broken.js`) | failure 1 (lost context) | no `valueChanged`; instead `← JS→.NET gaugeKnobBroken.contextError {"message":"this.fireWidgetEvent is not a function","thisWas":"HTMLInputElement",…}`, a red banner explaining that `this` was the `<input>`, status "context lost" |
| Set 25 / Set 60 / Set 85 | success (state) | `→ .NET→JS update(options) ×2 {"value":…}` — **both** knobs move, because `update()` is a widget method and is not affected by the callback bug |
| ▶ Stream | progress | a `Timer` replays 14 readings every 600 ms through `Options.value`; the status counts them; click again to stop |
| Wrong package order | failure 2 (load order) | a third widget `gaugeKnobWrongOrder` is created at runtime inside the empty slot with `vendor-knob.js` listed **before** `jquery-lite.js`; the console shows `ReferenceError: jQuery is not defined`, the slot shows the raw red-dashed `<input>`, and after 2 s the server checks `IsLoaded` and shows the banner "widget never initialized — check Packages order (console: jQuery is not defined)" (or, if the loader still ran `init`, the caught "init failed — $(…).vendorKnob is not a function" banner) |
| Open plain-JS proof ↗ | deliverable 1 | `wwwroot/proof/knob-proof.html` opens in a new tab: the same plugin, no Wisej.NET, with the broken/fixed callback demo and the load-order checks |
| Clear trace | – | empties the right-hand list |

The right-hand card is the live client/server trace: every message in both directions, so what the
video shows in DevTools can be compared with what the server saw.

### About the wrong-order demo

`vendor-knob.js` checks the `jQuery` global once, when its script executes. The Knob Demo page already
has jQuery (loaded by `gaugeKnob`, in the right order), so a widget that merely lists the packages the
wrong way round would not fail here. The runtime widget therefore lists `wwwroot/hide-jquery.js`
first — two lines that drop the `jQuery`/`$` globals to reproduce a fresh page — then the plugin, then the
stylesheet, then jQuery. Package names differ from `gaugeKnob`'s because Wisej.NET caches packages by
name. The file is a lab prop and is documented as such.

## Where things live

```
IntegrationLab/
├─ wwwroot/
│  ├─ proof/knob-proof.html    deliverable 1: the plain-JS proof (no Wisej.NET)
│  ├─ gauge-init.js            deliverable 2: the context-safe InitScript (embedded resource)
│  ├─ gauge-init.broken.js     the broken twin: this.fireWidgetEvent inside the vendor callback
│  ├─ jquery-lite.js           package 1 — tiny jQuery-compatible subset (window.$, $.fn)
│  ├─ vendor-knob.css          package 2 — vendor stylesheet (+ red fallback for the un-enhanced input)
│  ├─ vendor-knob.js           package 3 — the "third-party" jQuery plugin, $.fn.vendorKnob
│  ├─ hide-jquery.js   lab prop for the wrong-order button (parks the jQuery global)
│  └─ vendor-gauge.js          shared course library, unused in this module
├─ docs/
│  ├─ PlainJsProof.md          deliverable 1 — what the proof page shows and how it was verified
│  ├─ ContextSafeInitScript.md deliverable 2 — closure / bind / method, annotated gauge-init.js, load order
│  └─ DebuggingNotes.md        deliverable 3 — where the instance is stored, sourceURL, debugger, symptom table
├─ Window1.cs / .Designer.cs   Knob Demo (two Wisej.Web.Widget instances + runtime wrong-order widget)
├─ Program.cs                  Wisej.NET session entry point
└─ Startup.cs                  Kestrel host (app.UseWisej(), static files from the project folder)
```

## Deliverables

1. **Working plain JavaScript proof** — [`IntegrationLab/wwwroot/proof/knob-proof.html`](IntegrationLab/wwwroot/proof/knob-proof.html), described in [`IntegrationLab/docs/PlainJsProof.md`](IntegrationLab/docs/PlainJsProof.md)
2. **Context-safe Wisej.NET InitScript** — [`IntegrationLab/wwwroot/gauge-init.js`](IntegrationLab/wwwroot/gauge-init.js), explained in [`IntegrationLab/docs/ContextSafeInitScript.md`](IntegrationLab/docs/ContextSafeInitScript.md) (broken twin: [`gauge-init.broken.js`](IntegrationLab/wwwroot/gauge-init.broken.js))
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
