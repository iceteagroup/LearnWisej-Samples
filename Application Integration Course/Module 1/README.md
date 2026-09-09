# IntegrationLab · Application Integration Course · Module 1

Local lab build for **Module 1 · Integration Architecture and Vocabulary**. It follows the
walkthrough video: a `TemperatureGauge : Widget` hosts a third-party gauge inside a Wisej.NET
widget container, `Value` is a server property, and `ThresholdExceeded` is a normal .NET event
that fires when the reading crosses 100°.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine (the original build lives in D:ProjectsPlayIntegrationLab).

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Application Integration Course/Module 1/IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5071
```

Then open <http://localhost:5071>. (Visual Studio: open `IntegrationLab.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try in the Sensor Monitor window

| Button | Path | What you should see |
|---|---|---|
| Set 72° / 88° / 104° / 79° | success | trace line `→ .NET→JS setValue(104) {"value":104}`, the gauge re-renders, and at 104° one `← JS→.NET thresholdExceeded {"value":104}` comes back and `ThresholdExceeded` fires in C# (red banner + alert) |
| ▶ Stream readings | progress | a `Timer` (a Component with no visual surface) replays 14 readings; the status label counts them; the threshold event fires exactly once on the way up |
| Set 150° (invalid) | failure 1 | the server property setter rejects it; nothing is rendered; banner + alert explain why |
| Corrupt payload | failure 2 | `{"value":"n/a"}` is shipped on purpose; the vendor throws, the client adapter catches it and reports **one** `error` event; the page stays alive |
| Resync from server | recovery | the authoritative server state is re-rendered and the banner clears |

The right-hand card is the live client/server trace: every message in both directions, so the
JSON can be compared with the written contract.

## Where things live (matches the video's solution tree)

```
IntegrationLab/
├─ Widgets/
│  ├─ TemperatureGauge.cs      server Component/Control: owns state, raises .NET events
│  └─ GaugeEventArgs.cs        event payload types (data, never behavior)
├─ wwwroot/
│  ├─ temperature-gauge.js     client Widget adapter (InitScript, embedded resource)
│  └─ vendor-gauge.js          the "third-party" VendorGauge library (Package)
├─ docs/
│  ├─ ArchitectureDiagram.svg  deliverable 1 (+ .png)
│  ├─ WidgetTriage.md          deliverable 2
│  ├─ IntegrationDecisionRecord.md   deliverable 3 (ADR-001)
│  └─ ClientServerContract.md  the documented contract the lesson insists on
├─ Window1.cs / .Designer.cs   Sensor Monitor
├─ Program.cs                  Wisej.NET session entry point
└─ Startup.cs                  Kestrel host (app.UseWisej())
```

## Self-check answers (lab guide)

- **Why is a Control always a Component, but not every Component a Control?**
  A Component is any server object Wisej.NET can render (the `Timer` here is one). A Control is a
  Component that also occupies space on the page (`TemperatureGauge`). Surface is the extra
  requirement, so the set of Controls is inside the set of Components.
- **Why should most third-party widgets be hosted inside a container element?**
  The framework owns its element (layout, visibility, theme, disposal) and the vendor wants to own
  one too. Giving the vendor a child element (`div.temperature-gauge-host`) keeps one owner each;
  resize, destroy and theme become explicit adapter calls instead of a fight over the same node.
- **What state belongs on the server and what can stay local in the browser?**
  Anything auditable or decision-making is server state: `Value`, limits, thresholds, permissions.
  Purely visual, transient things stay in the browser: needle animation, hover, the vendor's
  internal range bookkeeping. The browser never holds the authoritative value.
