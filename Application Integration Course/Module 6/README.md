# IntegrationLab · Application Integration Course · Module 6

Local lab build for **Module 6 · Client-Server Calls & Serialization**. It follows the walkthrough
video: the reusable gauge (`SimpleGauge : Widget`, "Boiler 3") gains a command API — the server
**sets the value** and **resets the animation** with one-way `Call`, **reads the rendered size** with an
awaited `CallAsync`, and **gets the selected client-side state** back as a small camel-cased DTO.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Application Integration Course\Module 6\IntegrationLab"
dotnet run -f net10.0 --urls http://localhost:5076
```

Then open <http://localhost:5076>. (Visual Studio: open `IntegrationLab.slnx`, press F5 — the port
is in `Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes; warning CS7022 (Program.Main ignored) is expected.

## What to click in the Gauge Commands window

The four commands under the gauge; the right-hand card is the **Server ⇄ Client · command trace**.

| Button | Mechanism | What you should see |
|---|---|---|
| **Set value (72)** | `Call("setValue", 72)` — one-way | one `→ .NET→JS Call("setValue", 72)` line; the needle sweeps to 72 |
| **Read rendered size** | `await CallAsync("getRenderedSize")` | `→ await CallAsync("getRenderedSize")`, then `← JS→.NET result {"width":480,"height":244}` on a **later** timestamp; the next statement picks "wide"/"compact" and updates the caption |
| **Reset animation** | `Call("resetAnimation")` — one-way | one queued call, the gauge plays its settle animation, nothing comes back |
| **Get selected state** | `await CallAsync("getSelectedState")` → `GaugeStateDto` | `← JS→.NET result {"value":72,"isAboveThreshold":false,"width":480,"height":244,"isAnimating":false}` |

A queued `Call` produces one line; an awaited `CallAsync` produces a `←` result line later. Failures
(a rejected value, a browser that does not reply within 5 s, a vendor error reported by the adapter)
show on the banner under the gauge.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Server-to-client method calls | [`IntegrationLab/docs/ServerToClientCalls.md`](IntegrationLab/docs/ServerToClientCalls.md) · code: `Controls/SimpleGauge.cs` (`SetValue`, `ResetAnimation`, `GetRenderedSizeAsync`, `GetSelectedStateAsync`) and `wwwroot/gauge-init.js` |
| 2 | One `CallAsync` / `EvalAsync` example | [`IntegrationLab/docs/CallAsyncExample.md`](IntegrationLab/docs/CallAsyncExample.md) · `GetRenderedSizeAsync` awaited in `Window1.buttonReadSize_Click` |
| 3 | DTO used for client state return | [`IntegrationLab/docs/DtoContract.md`](IntegrationLab/docs/DtoContract.md) · `Contracts/GaugeStateDto.cs`, `Contracts/RenderedSize.cs` |
| — | Client object model note | [`IntegrationLab/docs/ClientObjectModel.md`](IntegrationLab/docs/ClientObjectModel.md) |

## Where things live

```
IntegrationLab/
├─ Controls/
│  ├─ SimpleGauge.cs           server widget: typed state + command API (Call / CallAsync)
│  └─ GaugeEventArgs.cs        event payload types (data, never behavior)
├─ Contracts/
│  ├─ GaugeStateDto.cs         the client-state DTO (5 primitives, camelCase on the wire)
│  └─ RenderedSize.cs          the CallAsync("getRenderedSize") result
├─ wwwroot/
│  ├─ gauge-init.js            client wrapper (InitScript, embedded): setValue / resetAnimation / getRenderedSize / getSelectedState
│  └─ vendor-gauge.js          the "third-party" VendorGauge library (Package)
├─ docs/                       the three deliverables + the object-model note
├─ Window1.cs / .Designer.cs   Gauge Commands window (async void handlers with try/catch)
├─ Program.cs                  Wisej.NET session entry point
└─ Startup.cs                  Kestrel host (app.UseWisej())
```

## Self-check answers (lab guide)

- **When should server code await a client result?**
  Only when the next server statement cannot run without it — here, choosing a layout from the
  rendered size, or mapping and validating the selected state. If the value would only be logged,
  use `Call` and keep the handler moving. Every awaited call also needs a bound (the lab uses a 5 s
  `ClientReplyTimeout`, because a closed tab never replies) and validation before the value is trusted.
- **Why does camel-casing matter when passing objects to JavaScript?**
  Wisej.NET serializes `SelectedValue` as `selectedValue` and `IsAnimating` as `isAnimating`; the
  JavaScript side must read the camelCase names or it silently gets `undefined`. Coming back, a
  `CallAsync` result keeps its JavaScript names: `result.width` works, `result.Width` is `null` with
  no error, which turns into zeros in the DTO. Map once, by JavaScript names, into a typed DTO.
- **Why should DTOs be small?**
  A DTO with five primitives is cheap to serialize, readable in one trace line, and stable: adding a
  property is safe, and the rare rename is versioned in minutes. A DTO that mirrors a domain entity
  changes whenever the entity does and drags along everything the entity references — a work order
  with a `Customer` navigation property would ship the customer's tax id and credit limit to the
  browser. Never pass a domain object across the wire; pass an object built for the crossing.
