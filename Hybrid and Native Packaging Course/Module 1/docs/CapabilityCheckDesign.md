# Capability check design — `Device.Valid`

## The rule

1. Ask **whether the Hybrid client is there**: `Device.Valid` (namespace `Wisej.Hybrid`, package
   `Wisej-4-Hybrid`). It is false in a browser tab and in an installed PWA, true only inside the
   Wisej.NET Hybrid client.
2. Then ask **about the feature**: `Device.Media.CaptureSupported` for the camera. A valid client on
   hardware without a camera is still valid.
3. Use `&&` so `Device.Media` is only consulted once a client is present.
4. Every gated feature has a **browser answer** that still gets the job done. A disabled button is
   not an answer; a separate mobile-only screen is not one either.
5. Never read the user agent. A phone browser matches "Android"/"iPhone" and would get a camera
   button nobody can serve; the Windows Hybrid client fails that test and would lose a camera it has.

## Where it lives in the code

- `FieldOps/Capabilities.cs` — `CanTakePhoto => Device.Valid && Device.Media.CaptureSupported`,
  used by the list page's action strip.
- `FieldOps/WorkOrderDetail.cs` — `WorkOrderDetail_Load` writes the gate out in full, exactly as the
  lesson shows it: `btnTakePhoto.Visible = camera; uploadPhoto.Visible = !camera;`.
- `FieldOps/PhotoActions.cs` — the two paths behind the gate: `Device.Media.CapturePhotoAsync`
  (null = cancelled; the caller disposes the image) and the `Upload` control's posted file.

## The feature map FieldOps will grow into

| Feature | Hybrid client (`Device.Valid` true) | Browser / PWA (`Device.Valid` false) | Module |
|---|---|---|---|
| Photo | `btnTakePhoto` → `Device.Media.CapturePhotoAsync` | `uploadPhoto` (`Upload` control) | 1, 5 |
| Report export | `Device.FileSystem.WriteBytes` into `MyDocuments` | `Application.Download` | 3 |
| New-order alert | `Device.LocalNotification.Schedule` | the list refreshes through server push | 3 |
| Second window | `Device.Windows.TryOpenAsync` (desktop only, `Device.System.IsDesktop`) | `Application.Navigate(url, "_blank")` | 3 |
| Install offer | never (already installed) | in-page Install button after the first session | 2 |
| Navigate | `Device.Geolocation` + `Device.Map.TryOpenAsync` | typed address + maps link | 5 |
| Reminder | `Device.LocalNotification.Schedule` | on-screen job list | 5 |

## Verified in this module

Run in a plain browser (Chrome engine, the course's browser pane): the list and detail page show
**Attach photo (upload)** and no **Take photo** — `Device.Valid` is false. When the Windows Hybrid
client opens FieldOps (Module 3), the server logs `Device.Valid=True Platform=WinUI`, so the same gate
takes the other branch there.
