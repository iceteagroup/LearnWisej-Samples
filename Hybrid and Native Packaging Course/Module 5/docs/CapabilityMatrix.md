# FieldOps capability matrix

Computed once per session by `DeviceCapabilities` (kept in `Application.Session["Capabilities"]`)
from `Device.Valid` and `Device.Media.CaptureSupported`; every screen obeys it. An action the client
cannot perform is **absent**, not disabled.

| Feature | Hybrid client (Android / iOS / Windows / Mac) | Browser tab | Installed PWA |
|---|---|---|---|
| **Photo** | **Take Photo** → `PermissionGate` (Camera) → `Device.Media.CapturePhotoAsync(1600, 1600, 0.7f, 0)`; `null` = *Capture cancelled*; stored outside the web root; thumbnail pushed with `Application.Update(this)`. On a client without a camera (`CaptureSupported` false) → Upload. | **Upload Photo** (`Upload`); the original file is downscaled to 1600 px on arrival | as the browser (`Device.Valid` is false) |
| **Signature** | Canvas in the page → PNG → `SignatureService` (order, typed signer, server time) | same canvas, same pipeline | same |
| **Navigate** | **Navigate** → `PermissionGate` (LocationWhenInUse) → `Device.Geolocation.GetLocationAsync(High, 10 s)` → `LocationLog` (lat, lon, accuracy, **server** time) → `Device.Map.TryOpenAsync(site)`; no fix → route from the site address | **Open route**: the site address, a maps link in a new tab (`Application.Navigate`) | same as the browser |
| **Reminder** | `Device.LocalNotification.Schedule("Next job", …, appointment − 30 min)` when the appointment is assigned (order feed) or first opened; `Responded` opens that order | the job list, kept current by Wisej.NET server push | same as the browser |
| **Biometric unlock** | absent — the Authentication extension (`Wisej-4-Hybrid-Authentication` + `.UseWisejAuthentication()`) is not part of this build | absent | absent |

## Permission outcomes (per press, never in a loop)

| `Device.Permissions` answer | What the screen does |
|---|---|
| `CheckAsync` → `Granted` | continue silently |
| not granted | show the one sentence of reason, then `RequestAsync` |
| `RequestAsync` → `Granted` | continue |
| `Denied` and `ShouldShowRationaleAsync` → true | amber banner *may not use the … this time*, the manual path stays (Upload / route from the address); ask again on the next press |
| `Denied` and no rationale, or `Restricted` | red banner *switched off … in the phone's settings*, **Open Settings** → `Device.AppInfo.ShowSettingsUI()`; never re-prompt |

Every outcome is logged: `permission: Camera granted after the reason`, `permission: LocationWhenInUse Denied, rationale=False`, …

## What each client showed in this lab

| Client | Logged at session start | Exercised |
|---|---|---|
| Browser (Chromium, browser pane) | `Device.Valid=False camera=Upload navigate=maps link reminders=job list (server push) biometrics=absent platform=Browser` | WO-4187 showed **Upload Photo · Signature · Open route** and `Device.Valid = false`; a signature with no name → *Type the signer's name under the signature.*; with a name → *Signed by R. Okafor · 5 KB PNG · hh:mm server time* (PNG + JSON record written under `%LOCALAPPDATA%\FieldOps\signatures\WO-4187`) |
| Windows Hybrid client (unpackaged) | `Device.Valid=True camera=CapturePhotoAsync navigate=Geolocation+Map reminders=LocalNotification biometrics=absent platform=WinUI` | a dispatched order scheduled `Device.LocalNotification.Schedule "Next job" WO-4830 at 17:05` |
| Android / iOS | — | built (Android APK) but not run: no emulator here; iOS needs a Mac |
| Installed PWA | — | same code path as the browser (`Device.Valid` false), not installed here |

Not exercised on this machine: the camera, location and map calls and every permission prompt —
they need a click inside the native window or a phone. The browser **Open route** and **Upload
Photo** clicks were not made in the browser pane (an external maps site; a native file picker). The
Upload path's server half (`PhotoStore.SaveDownscaled`) was run separately on net10.0: a
4000 × 3000 image was stored as 1600 × 1200 JPEG.
