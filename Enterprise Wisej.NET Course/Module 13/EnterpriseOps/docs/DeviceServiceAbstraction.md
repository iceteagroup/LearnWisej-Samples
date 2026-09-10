# Device service abstraction

**Deliverable 2 of Module 13.** Camera, scanner, haptics, connectivity and shape all reach the
application through one interface, so screens never contain platform code.

## The contract

`Hybrid/HybridOfflinePatterns.cs`:

```csharp
public interface IDeviceServices
{
    Task<bool> IsOnlineAsync();
    Task<string?> ScanDocumentAsync();
    Task VibrateAsync();
}
```

Three business-friendly verbs. Not `GetNavigatorOnLine()`, not `OpenAndroidCamera()`. A screen asks
*"am I online?"* and *"scan something for me"*; how that happens is not its problem.

Shape detection is the same idea one level up: `BrowserDeviceServices.Describe()` returns a
`DeviceInfo` value (`Profile`, `FieldWidth`, `TouchTargetHeight`, `ShowsContextColumns`). The page reads
that value; it never reads a user agent.

## The implementations

| Implementation | Shape | `IsOnlineAsync` | `ScanDocumentAsync` | `VibrateAsync` |
|---|---|---|---|---|
| `BrowserDeviceServices` (this sample) | browser / connected + simulated offline | the simulated connectivity flag flipped by **Go offline / Go online** | a rotating list of asset tags, one of them deliberately wrong | traced, no-op |
| *A Hybrid shell build* | remote / local | the platform reachability callback | the native camera + barcode decoder | the platform haptic API |
| *A kiosk build* | connected | always `true` | a wedge scanner's keyboard input | not available → returns immediately |
| *A test double* | — | whatever the test needs | a known constant | counts calls |

**Nothing above the interface changes between those four.** That is the whole return on the abstraction:

1. One form serves the office and the field.
2. Tests substitute a fake device that returns a known photo or barcode.
3. A new device or shell needs a new implementation, not a search through every screen for platform calls.

## How the screen uses it

`UI/FieldTechnicianPage.cs` holds **two** fields on purpose:

```csharp
private readonly BrowserDeviceServices _shell;   // the implementation: simulate switch, connectivity
private readonly IDeviceServices _device;        // what the screen is allowed to call
```

Every field action goes through `_device`. `_shell` exists only for the *lab controls* that stand in for
the shell itself (the connectivity toggle and the Simulate switch) — in production those are the
platform's job, not a button.

```csharp
if (await _device.IsOnlineAsync())
    await _service.CompleteAsync(command, ctx);   // straight to the server
else
    await _queue.EnqueueAsync(offline);           // durable, explicit, visible
```

## Device output is untrusted input

A scanned value is data from outside the system, exactly like a text box:

* **Online** — validated immediately: `WorkOrderService.ValidateScannedAssetAsync` rejects anything that
  is not an `ASSET-…` tag and warns when the asset does not belong to the work order's site.
* **Offline** — the value travels inside the queued command's payload and is validated **on the server at
  sync time**. The device never decides that a scan is acceptable.

A GPS position would follow the same rule: a hint for the technician, never proof of where the work was
done.

## Device-aware layout, from the same value

| Profile | Frame width | Touch targets | Context columns |
|---|---|---|---|
| Phone | 380 px | 44 px | hidden (`Site`, `v`, scan result) |
| Tablet | 620 px | 40 px | shown |
| Desktop | 860 px | 34 px | shown |

`FieldTechnicianPage.ApplyDeviceLayout()` is the only method that reads `DeviceInfo`, and it changes
sizes and column visibility — never behaviour. A phone and a desktop run the same handlers, the same
services and the same queue.

## Evidence — what the running app shows

| Claim | Where you see it |
|---|---|
| Screens call the abstraction, not the platform | Trace: `Device: IDeviceServices.IsOnlineAsync() → False` on every **Complete…** click |
| Detection happens once, as a value | Trace at load: `Device: device detected → Desktop · reported "Desktop" · Chrome on Windows · screen 1920×1080` |
| The switch changes layout only | Pick **Phone** in *Simulate device*: the frame narrows to 380 px, buttons grow to 44 px, `Site` and `v` disappear — the banner says "No screen logic changed." |
| The scanner works offline | Go offline, press **Scan**: `Device: IDeviceServices.ScanDocumentAsync() → "ASSET-Pump-88121" (untrusted until the server validates it)` |
| Scanned values are server-validated | Online, keep pressing **Scan**: the four simulated tags rotate, so one matches the selected work order's site (`✓ ASSET-Subs-44107 matches WO-1038 (Substation B).`), others belong elsewhere (`✖ … does not belong to WO-1038's site.`) and the fourth is not an asset tag at all (`✖ The scanned code is not an asset tag.`) |
| Haptics go through the interface | Complete offline: `Device: IDeviceServices.VibrateAsync() → shell haptic (no-op in the browser)` |
