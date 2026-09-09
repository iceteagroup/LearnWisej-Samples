# Deliverable 1 · Working plain JavaScript proof

**File:** `wwwroot/proof/knob-proof.html` — open it from the Knob Demo with **Open plain-JS proof ↗**,
or directly at <http://localhost:5072/wwwroot/proof/knob-proof.html> while the app runs.
It is served by the app's static file server (`Startup.cs` → `UseFileServer()` over the project folder);
it contains **no Wisej.NET** — the point is to prove the vendor sample before any adapter code exists.

## What the page loads, in order

```html
<script src="../jquery-lite.js"></script>          <!-- 1. window.$ / window.jQuery, $.fn -->
<link rel="stylesheet" href="../vendor-knob.css" /> <!-- 2. vendor stylesheet -->
<script src="../vendor-knob.js"></script>           <!-- 3. the plugin: registers $.fn.vendorKnob -->
```

The same three files, in the same order, become the widget's `Packages` list in Wisej.NET
(`Window1.Designer.cs`). Nothing else changes between the proof and the adapter except *who* creates
the `<input>` and *where* the instance is kept.

## What the page does

1. **The vendor sample, isolated.** Creates `<input class="knob">` inside `div.knob-host` and calls
   `$(input).vendorKnob({ value: 40, min: 0, max: 100, step: 1, label: "Pressure", units: " psi" })`.
   The plugin hides the input and inserts `div.vendor-knob` (an SVG dial) right after it. The vendor
   event `knobchange` (dispatched on the `<input>`, payload `e.detail.value`) is shown in the readout.
   *Set 25 / 60 / 85* and *Turn +5* call `instance.setValue(...)`; *Log instance to console* prints
   `$(input).data("vendorKnob")` and its prototype methods.
2. **Load-order checks** run at page load and show green/red dots for `typeof jQuery`,
   `typeof $.fn.vendorKnob`, "vendor-knob.css applied" (computed border-radius of the host),
   "plugin applied to `<input>`" and "vendor children inside host".
3. **Callback wired the BROKEN way.** A `fakeWidget` object stands in for the Wisej.NET wrapper.
   `fakeWidget.wireBroken()` registers a `knobchange` handler that calls `this.fireWidgetEvent(...)`.
   Inside the handler `this` is the `<input>` (the element that dispatched the event), so the call
   throws; the page catches it and prints the exact text:

   ```
   TypeError: this.fireWidgetEvent is not a function
   inside the callback: this = HTMLInputElement (not fakeWidget)
   → the event never reaches the widget
   ```
4. **Callback wired the FIXED way.** `fakeWidget.wireFixed()` captures `var me = this` first and the
   handler calls `me.fireWidgetEvent(...)`. The page prints
   `fireWidgetEvent("valueChanged", {"value":41}) reached fakeWidget ✓`.
5. **Wrong load order (text).** Explains what happens when the two script tags are swapped: the
   plugin's guard throws at load time, before any of your code runs —

   ```
   Uncaught ReferenceError: jQuery is not defined — vendor-knob.js must be loaded after jQuery.
   ```

   — and later `$(input).vendorKnob` is `undefined` (`TypeError: $(...).vendorKnob is not a function`).
   The raw dashed `<input>` stays visible because the plugin never hid it (`vendor-knob.css` styles the
   un-enhanced input on purpose so the failure is visible in the page, not only in the console).

## How it was verified

- The page loads with three `200` responses in the Network panel, in the listed order, and all five
  load-order checks are green.
- Dragging the dial / using the wheel updates the readout with `knobchange  e.detail = {"value":…}`.
- *Wire the broken way* → *Turn +5* shows the red `TypeError` box; *Wire the fixed way* → *Turn +5*
  shows the green "reached fakeWidget ✓" box. *Unwire* resets both.
- Swapping the first and third tags in the `<head>` and reloading reproduces the `ReferenceError` in the
  console, no dial, red dashed input (restore the order afterwards).
- In the console, `$(document.querySelector("input.knob")).data("vendorKnob")` returns the `VendorKnob`
  instance with `setValue`, `setOptions`, `getValue`, `pulse`, `resize`, `destroy`.

## Why the proof comes first

Handing a library the wrong element (a `div` when it wants an `input`, a `div` when it wants a `canvas`)
produces an empty box and no error. Getting the load order wrong produces an error that names a vendor
symbol, not your code. Both are far easier to see in a 100-line page with three tags than inside a
framework that injects scripts at runtime. Once the proof works, moving it into an InitScript is a
mechanical change (`docs/ContextSafeInitScript.md`), and anything that breaks afterwards is an
adapter problem, not a vendor problem.
