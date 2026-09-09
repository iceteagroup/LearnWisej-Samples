# IntegrationLab · Application Integration Course · Module 6

Local lab build for **Module 6 · Client-Server Calls & Serialization**. It follows the walkthrough
video: the reusable gauge (`SimpleGauge : Widget`, "Boiler 3") gains a command API — the server
**sets the value** and **resets the animation** with one-way `Call`, **reads the rendered size** with an
awaited `CallAsync` (and the `EvalAsync` variant), and **gets the selected client-side state** back as a
small camel-cased DTO. Two failure paths show what goes wrong when the rules are broken: reading a
result with Pascal-case names, and leaking a domain object into the options.

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

Under the gauge — the four commands from the walkthrough's finished screen:

| Button | Mechanism | What you should see |
|---|---|---|
| **Set value (72)** | `Call("setValue", 72)` — one-way | `update(options) {"value":72}`, `Call("setValue", 72) one-way · queued · no result` and `next statement runs immediately` on **one** timestamp; the needle sweeps to 72 |
| **Read rendered size** | `await CallAsync("getRenderedSize")` | `CallAsync(...) awaiting the browser…`, then `← result {"width":480,"height":244} (n ms round trip)` on a **later** timestamp; the next statement picks "wide"/"compact" and updates the caption; the result box shows the `RenderedSize` DTO |
| **Reset animation** | `Call("resetAnimation")` — one-way | one queued call, the gauge plays its settle animation, nothing comes back |
| **Get selected state** | `await CallAsync("getSelectedState")` → `GaugeStateDto` | `← result {"value":72,"isAboveThreshold":false,"width":480,"height":244,"isAnimating":false}` and the DTO as camelCase JSON in the result box |

Bottom bar:

| Button | Path | What you should see |
|---|---|---|
| **Set value (104 · alarm)** | success + event | the sweep crosses the threshold; `← thresholdExceeded {"value":104}` arrives and `ThresholdExceeded` fires in C# (red banner) |
| **Eval width** | awaited (`EvalAsync`) | `EvalAsync("this.measureWidth()")` → `← result 480 (n ms round trip)` |
| **Camel-case pitfall** | failure 1 | same round trip read twice: `result.Width (PascalCase) → null … no error, just wrong data` vs `result.width (camelCase) → 480`; result box shows `wrong:` zeros against `right:` values; orange banner |
| **Leak a domain object** | failure 2 + recovery | `{"debugDump":{… 1180 chars …}}` goes out with a peek at the serialized `customer`; the browser answers `← leakDetected {"bytes":…,"keys":…,"sample":"…customer.taxId, customer.creditLimit"}`; red banner; the server removes it (`{"debugDump":null}`) and the status reads `recovered` |
| **▶ Stream values** | progress | a `Timer` issues one `Call("setValue")` per tick (900 ms) — nothing awaited; click **Get selected state** during the stream to catch `isAnimating: true` and a `contract check … server wins` line |
| **Clear trace** | — | empties the right-hand card |

The right-hand card is the live **Server ⇄ Client · command trace**: every command, result and event
with a timestamp, so a queued `Call` (all lines share one time) can be told apart from an awaited
`CallAsync` (the `←` line comes later, with the measured round trip).

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Server-to-client method calls | [`IntegrationLab/docs/ServerToClientCalls.md`](IntegrationLab/docs/ServerToClientCalls.md) · code: `Controls/SimpleGauge.cs` (`SetValue`, `ResetAnimation`, `GetRenderedSizeAsync`, `GetWidthViaEvalAsync`, `GetSelectedStateAsync`) and `wwwroot/gauge-init.js` |
| 2 | One `CallAsync` / `EvalAsync` example | [`IntegrationLab/docs/CallAsyncExample.md`](IntegrationLab/docs/CallAsyncExample.md) · `GetRenderedSizeAsync` awaited in `Window1.buttonReadSize_Click` |
| 3 | DTO used for client state return | [`IntegrationLab/docs/DtoContract.md`](IntegrationLab/docs/DtoContract.md) · `Contracts/GaugeStateDto.cs`, `Contracts/RenderedSize.cs` (and the counter-example `Contracts/DomainWorkOrder.cs`) |
| — | Client object model note | [`IntegrationLab/docs/ClientObjectModel.md`](IntegrationLab/docs/ClientObjectModel.md) |

## Where things live

```
IntegrationLab/
├─ Controls/
│  ├─ SimpleGauge.cs           server Control: typed state + command API (Call / CallAsync / EvalAsync)
│  └─ GaugeEventArgs.cs        event payload types (data, never behavior)
├─ Contracts/
│  ├─ GaugeStateDto.cs         the client-state DTO (5 primitives, camelCase on the wire)
│  ├─ RenderedSize.cs          the CallAsync("getRenderedSize") result
│  └─ DomainWorkOrder.cs       DELIBERATELY BAD: the domain object that must never cross the wire
├─ wwwroot/
│  ├─ gauge-init.js            client wrapper (InitScript, embedded): setValue / resetAnimation / getRenderedSize / getSelectedState / measureWidth
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
  no error — the "Camel-case pitfall" button shows the zeros that produces. Map once, by JavaScript
  names, into a typed DTO.
- **Why should DTOs be small?**
  A DTO with five primitives is cheap to serialize, readable in one trace line, and stable: adding a
  property is safe, and the rare rename is versioned in minutes. A DTO that mirrors a domain entity
  changes whenever the entity does and drags along everything the entity references — the "Leak a
  domain object" button shows a customer's tax id and credit limit reaching the browser because a
  work order had a `Customer` navigation property. Never pass a domain object across the wire; pass
  an object built for the crossing.
