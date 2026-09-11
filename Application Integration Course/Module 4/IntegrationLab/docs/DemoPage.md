# Deliverable 3 · Demo page with no custom InitScript

`DemoPage.cs` + `DemoPage.Designer.cs` (`Wisej.Web.Page`, set as `Application.MainPage` in `Program.cs`).
One `SimpleGauge` dropped from the Toolbox, configured through typed properties, driven by the exact
statements the walkthrough shows.

## Evidence: the page contains no InitScript

What the designer file says about the gauge — the complete gauge section:

```csharp
this.simpleGauge1.Caption = "Boiler 3";
this.simpleGauge1.Location = new System.Drawing.Point(40, 56);
this.simpleGauge1.Name = "simpleGauge1";
this.simpleGauge1.Size = new System.Drawing.Size(480, 270);
this.simpleGauge1.Value = 72D;
this.simpleGauge1.ValueChanged += …;
this.simpleGauge1.ThresholdExceeded += …;
this.simpleGauge1.WidgetError += …;
```

Search proof (run from `Module 4/IntegrationLab`):

```
grep -n "InitScript\|Packages\|Options\|Call(\|VendorGauge\|vendor-gauge\|gauge-init\|<script\|function (" DemoPage.cs DemoPage.Designer.cs
```

returns only the class summary comment that *says* the page has none of them. There is no JavaScript in
the page, no vendor name, no package path, no option name and no `Options`/`Call` access.

If `VendorGauge` were replaced tomorrow, `SimpleGauge.cs` and `gauge-init.js` would change; `DemoPage.cs`
and `DemoPage.Designer.cs` would not.

## What is on the screen

- **Left card** — "Boiler 3": `simpleGauge1` (0..100, threshold 85) and a banner that appears when the
  threshold is crossed, a value is rejected or the adapter reports a failure.
- **Right card** — *DemoPage.cs · code-behind*: the statements each button runs, followed by
  `✓ gauge updated · Value = …` when the gauge raises `ValueChanged`.
- **Bottom bar** — the four statements from the walkthrough.

## What each button proves

| Button | Code-behind, verbatim | Screen | What it proves |
|---|---|---|---|
| **Value = 72** | `simpleGauge1.Value = 72;` | on a fresh page the gauge is already at 72: only the statement is listed, no `✓ gauge updated` | a setter that does not change anything renders nothing |
| **Value = 90** | `simpleGauge1.Value = 90;` | the needle sweeps; `ThresholdExceeded` → red banner "Boiler 3 reached 90 (threshold 85)"; `✓ gauge updated · Value = 90` | one typed property = one client update; a vendor event arrives as a normal .NET event |
| **Maximum = 120; Value = 45** | `simpleGauge1.Maximum = 120;` `simpleGauge1.Value = 45;` | the scale re-lays out and the needle moves in one round trip; the banner clears when `ValueChanged` reports 45 | two typed properties validate against each other and batch into one update |
| **Value = 78** | `simpleGauge1.Value = 78;` | warm band, no threshold event | ordinary update; the rising-edge detection did not re-fire |

Failure handling stays in the normal flow: a statement that breaks a rule of the wrapper (a `Value`
outside `Minimum..Maximum`) throws `ArgumentOutOfRangeException` in the setter, before anything is
rendered; the page catches it, lists `✖ Value must be between …` and shows an orange banner. `WidgetError`
(the vendor throwing inside the adapter) is handled the same way.

## What the page does with the .NET events

| Event | Page reaction |
|---|---|
| `ValueChanged` | `✓ gauge updated · Value = …` in the code-behind list; clears the alarm banner once the reading is back under `Threshold` |
| `ThresholdExceeded` | red banner naming the gauge by its `Caption` |
| `WidgetError` | orange banner with the phase and message |

The handlers are wired in the designer file exactly like a Button's `Click`.
