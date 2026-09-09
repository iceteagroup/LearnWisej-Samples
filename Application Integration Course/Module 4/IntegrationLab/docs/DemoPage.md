# Deliverable 3 · Demo page with no custom InitScript

`DemoPage.cs` + `DemoPage.Designer.cs` (`Wisej.Web.Page`, set as `Application.MainPage` in `Program.cs`).
Two `SimpleGauge` instances dropped from the Toolbox, configured through typed properties, driven by the
exact statements the walkthrough shows.

## Evidence: the page contains no InitScript

What the designer file says about the gauges — the complete gauge sections:

```csharp
this.simpleGauge1.Caption = "Boiler 3";
this.simpleGauge1.Location = new System.Drawing.Point(40, 56);
this.simpleGauge1.Name = "simpleGauge1";
this.simpleGauge1.Size = new System.Drawing.Size(480, 250);
this.simpleGauge1.Value = 72D;
this.simpleGauge1.ValueChanged += …;
this.simpleGauge1.ThresholdExceeded += …;
this.simpleGauge1.WidgetError += …;
this.simpleGauge1.Trace += …;

this.simpleGauge2.Caption = "Chiller 1";
this.simpleGauge2.Location = new System.Drawing.Point(40, 314);
this.simpleGauge2.Maximum = 60D;
this.simpleGauge2.Name = "simpleGauge2";
this.simpleGauge2.Size = new System.Drawing.Size(200, 100);
this.simpleGauge2.Threshold = 50D;
this.simpleGauge2.Value = 18D;
…
```

Search proof (run from `Module 4/IntegrationLab`):

```
grep -n "InitScript\|Packages\|Options\|Call(\|VendorGauge\|vendor-gauge\|gauge-init\|<script\|function (" DemoPage.cs DemoPage.Designer.cs
```

returns only comments and UI label strings that *say* the page has none of them (the class summary, the
`render` comment, the `"no InitScript, no Packages…"` trace line and two label texts). There is no
JavaScript in the page, no vendor name, no package path, no option name and no `Options`/`Call` access.

If `VendorGauge` were replaced tomorrow, `SimpleGauge.cs` and `gauge-init.js` would change; `DemoPage.cs`
and `DemoPage.Designer.cs` would not.

## What is on the screen

- **Left card** — `simpleGauge1` "Boiler 3" (0..100, threshold 85) and `simpleGauge2` "Chiller 1"
  (0..60, threshold 50): two instances of the same class, different typed properties only. A status label
  (● ready / gauge updated / streaming / alarm / rejected / fault), an alarm/error banner that appears and
  disappears, and the authoritative server state of both gauges.
- **Right card** — *Server ⇄ Client · live message trace*: `→ .NET→JS` lines are emitted by the **wrapper**
  when a typed property renders an option; `← JS→.NET` lines are the adapter's events; `• server` lines are
  the page's own code-behind statements and the .NET events it handles.
- **Bottom bar** — the buttons below.

## What each button proves

| Button | Code-behind, verbatim | Trace / screen | What it proves |
|---|---|---|---|
| **Value = 72** | `simpleGauge1.Value = 72;` | on a fresh page the gauge is already at 72: only the `code-behind` line appears, **no** `update(options)` | a setter that does not change anything renders nothing |
| **Value = 90** | `simpleGauge1.Value = 90;` | `→ update(options) {"value":90}` · needle sweeps · `← valueChanged {"value":90,"previous":72}` · `← thresholdExceeded {"value":90,"threshold":85}` · **ThresholdExceeded fired in C#** → red banner + toast | one typed property = one client update; a vendor event arrives as a normal .NET event |
| **Maximum = 120; Value = 45** | `simpleGauge1.Maximum = 120;` `simpleGauge1.Value = 45;` | `→ update(options) {"max":120,"warnAt":67}` then `{"value":45}` (one `update()` on the client carrying both) · scale re-laid out · banner clears when `valueChanged` reports 45 | two typed properties validate against each other and batch into one round trip |
| **Value = 78** | `simpleGauge1.Value = 78;` | `{"value":78}` · warm band, no threshold event | ordinary update; the rising-edge detection did not re-fire |
| **Value = 200 (invalid)** | `simpleGauge1.Value = 200;` | `• server rejected  Value must be between Minimum (0) and Maximum (120).` · orange banner · toast · **no** `→ update(options)` line · needle does not move | validation lives in the wrapper, on the server, before rendering; the page only catches `ArgumentOutOfRangeException` |
| **AnimationEnabled = false / true** | `simpleGauge1.AnimationEnabled = false; simpleGauge2.AnimationEnabled = false;` | `{"animationEnabled":false}` on both gauges; the next value **jumps** instead of sweeping; toggle back and it sweeps again | a purely visual property is still a typed property; the page never touches CSS or the vendor |
| **▶ Stream both gauges** | `simpleGauge1.Value = …; simpleGauge2.Value = …;` every 700 ms from a `Timer` | 12 readings; status counts them; Boiler 3 crosses 85 once (→ banner), Chiller 1 crosses 50 once (→ banner); both `ValueChanged` events fire per tick; banner clears when both are back under threshold | progress path; two instances, independent state, one class; the vendor raises its rising-edge event exactly once per crossing even while the needle sweeps |
| **Clear trace** | — | empties the list | — |

The **Value = 200 (invalid)** button is the failure path; **Value = 78** (or any valid click) after it is
the recovery: the orange banner disappears and the gauge is unchanged because nothing was ever rendered.

`WidgetError` (the vendor throwing inside the adapter) is handled by the page (banner "Vendor failure inside
the wrapper…", status *fault*) but there is no button that triggers it: the typed API makes it impossible to
send the vendor a value it rejects. That is the point of Module 4 compared with Module 1's "Corrupt payload".

## What the page does with the .NET events

| Event | Page reaction |
|---|---|
| `ValueChanged` | trace line, status "gauge updated — wrapper handled the client call", refresh the server-state label, clear the alarm banner when every gauge is back under its threshold |
| `ThresholdExceeded` | trace line, red banner naming the gauge by its `Caption`, status *alarm*, top-right toast |
| `WidgetError` | trace line, orange banner, status *fault* |
| `Trace` | one line in the right-hand card, prefixed with the gauge name |

The handlers are wired in the designer file exactly like a Button's `Click`, and both gauges share them
(`sender` tells them apart).
