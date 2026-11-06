# FieldOps · Hybrid & Native Packaging · Module 5

Lab build for **Module 5 · Device APIs with Wisej.Hybrid.Device**. The work-order detail page of
the walkthrough — **FieldOps · WO-4187**, *Chiller 3 — pressure fault*, *Meridian Foods · Unit 7,
Halstow Industrial Park*, the **SLA** chip, the device actions (**Take Photo** DEVICE · **Signature**
· **Navigate** DEVICE in the client; **Upload Photo · Signature · Open route** in a browser), the
`Device.Valid = true|false` note, the **PHOTOS** strip with three slots, the **SIGNATURE** pad and the
**STATUS** strip that says what the last action did, with *N photos · signed/unsigned* on the right.

## Run it

```bash
cd "Hybrid and Native Packaging Course/Module 5/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6305
```

Open <http://localhost:6305>, double-click **WO-4187 Meridian Foods**. Hybrid client: as in Modules
3–4 (`FieldOpsServer` now points at port 6305; Android emulator `http://10.0.2.2:6305/`).

## What to try

| Action | Expected result |
|---|---|
| Browser: open WO-4187 | **Upload Photo · Signature · Open route**, `Device.Valid = false`. |
| Draw in the pad, press **Signature** without a name | *Type the signer's name under the signature.* |
| Type a name, press **Signature** | *Signed by … · N KB PNG · hh:mm server time*; PNG + record in `%LOCALAPPDATA%\FieldOps\signatures\WO-4187`. |
| **Upload Photo** with a phone picture | Stored downscaled to 1600 px; *Photo uploaded · (original) KB → (stored) KB on the server*; thumbnail in the strip. |
| **Open route** | The site address opened as a maps link in a new tab. |
| Client: **Take Photo** | Reason sentence → camera prompt → camera; back out → *Capture cancelled*; a photo → *1 photo stored · NNN KB · thumbnail pushed*. |
| Client: refuse the camera / location, then refuse for good | Amber *may not use … this time* with the manual path; then red *switched off … in the phone's settings* and **Open Settings** (`Device.AppInfo.ShowSettingsUI()`). |
| Client: **Navigate** | Location → *Arrival stamped hh:mm · ±(accuracy) m · (distance) km to the site* → the maps app; no fix → *No location fix … route opens from the site address*. |
| `POST /dispatch/assign` while the client is open | *New work order* notification and a **Next job** reminder scheduled 30 minutes before the appointment. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| `DeviceCapabilities`: `Device.Valid` + `Device.Media.CaptureSupported` once per session in `Application.Session`; actions absent where they cannot work | [DeviceCapabilities.cs](FieldOps/DeviceCapabilities.cs), `WorkOrderDetail_Load` |
| `PermissionGate`: `CheckAsync`, one sentence, `RequestAsync`, `ShouldShowRationaleAsync`, Settings via `Device.AppInfo.ShowSettingsUI()` | [PermissionGate.cs](FieldOps/PermissionGate.cs), `ReportRefusal` |
| Camera and location declared natively | [AndroidManifest.xml](FieldOps.Client/Platforms/Android/AndroidManifest.xml) (`CAMERA`, `ACCESS_FINE_LOCATION`, `ACCESS_COARSE_LOCATION`), [Info.plist](FieldOps.Client/Platforms/iOS/Info.plist) (`NSCameraUsageDescription`, `NSLocationWhenInUseUsageDescription`) |
| `btnPhoto_Click` with `CapturePhotoAsync(1600, 1600, 0.7f, 0)`, null = cancelled, `PhotoStore` outside the web root, thumbnail, `Application.Update(this)`; Upload downscaled on arrival | [WorkOrderDetail.cs](FieldOps/WorkOrderDetail.cs), [Services/PhotoStore.cs](FieldOps/Services/PhotoStore.cs) |
| Signature pad → PNG → `SignatureService` with order id, typed signer, server timestamp | `btnSignature_Click`, [Services/SignatureService.cs](FieldOps/Services/SignatureService.cs) |
| `btnNavigate_Click`: LocationWhenInUse, `GetLocationAsync` High / 10 s, lat/lon/accuracy with server time, `Device.Map.TryOpenAsync`; maps link with `Application.Navigate` | `btnNavigate_Click`, `OpenMapAsync`, `LocationLog` |
| Reminder `Device.LocalNotification.Schedule("Next job", …, appointment.AddMinutes(-30))`, `Responded` opens the order | [ReminderService.cs](FieldOps/ReminderService.cs) (called by the order feed and the detail page) |
| Capability matrix document | [docs/CapabilityMatrix.md](docs/CapabilityMatrix.md) |

## What was verified, and how

- **Builds:** `FieldOps` (net10.0-windows, net10.0); `FieldOps.Client` + `FieldOps.Offline` for
  `net9.0-windows10.0.19041.0` and `net9.0-android`: 0 errors, 0 warnings.
- **Browser** (`fieldops-m5`): WO-4187 matched the fallback scene (Upload Photo, Signature, Open route,
  `Device.Valid = false`); the signature pad was drawn with pointer events, the no-name path refused,
  the named signature saved (4 184-byte PNG + JSON record on disk, logged).
- **Upload downscale** (server half), run on net10.0 with the same `PhotoStore` source: a
  4000 × 3000 image became a 1600 × 1200 JPEG; thumbnails render as data URIs.
- **Windows Hybrid client**: the session logged
  `capabilities: Device.Valid=True camera=CapturePhotoAsync navigate=Geolocation+Map reminders=LocalNotification`;
  a dispatched order logged both `Device.LocalNotification.Schedule "New work order"` and
  `"Next job" … at 17:05` (30 minutes before its appointment).
- **Not exercised:** the camera, location, maps and permission prompts (they need a click in the
  native window or a phone; no emulator here), `Responded` (needs a tap on the notification), the
  browser **Open route** and **Upload Photo** clicks (external site; native file picker).

## Where this sample differs from the lesson

- **Signer's name field.** The lab requires the signer's typed name; the walkthrough's pad shows no
  field, so a *Signer's name* box sits under the pad.
- **The status flow stays** (Module 1) as a compact *UPDATE STATUS* row under the media; the video's
  detail page does not show it.
- **Thumbnails are data URIs, not a URL handler.** The lesson describes serving photos through a
  handler that checks the session's work order; FieldOps never exposes a URL at all — the page gets a
  small server-rendered thumbnail of the order it has open. Same guarantee, less surface.
- **Biometric unlock** is described by the lesson but not built: it needs the separate
  `Wisej-4-Hybrid-Authentication` packages; the capability matrix marks it absent everywhere, as the
  video's browser scene does.
- **Distance on arrival** uses `Wisej.Hybrid.Location.CalculateDistance` (verified by reflection), and
  the site coordinates are seed data.
- Inside a Wisej `Page`, `Location` resolves to `Control.Location`; the code writes
  `Wisej.Hybrid.Location` in full.
