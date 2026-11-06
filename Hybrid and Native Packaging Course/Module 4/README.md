# FieldOps · Hybrid & Native Packaging · Module 4

Lab build for **Module 4 · Mobile Clients for Android and iOS**. The same three projects as Module 3;
the client now targets Android and iOS too, and the FieldOps app gains the phone screen of the
walkthrough — `WorkOrderPage`: the blue app bar with FieldOps' own back arrow and
**FieldOps · Work orders** / **· WO-4821**, the order cards (WO-4821 Riverside Pump Station,
WO-4820, WO-4817, WO-4809), the detail with **METER READING**, **NOTES** and **Submit reading**,
the *Draft restored* banner, the **Leave WO-4821?** Stay / Leave dialog, and the status strip
*Remote · FieldOps server* with the platform on the right.

## Run it

```bash
cd "Hybrid and Native Packaging Course/Module 4/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6304
```

- Browser, phone layout: open <http://localhost:6304> in a window narrower than 600 px (or add `?view=phone`).
- Windows client: as in Module 3 (`-p:WindowsPackageType=None` for a Remote-only run); with
  `FIELDOPS_URL=http://localhost:6304/?view=phone` it opens the phone layout on the desktop.
- Android emulator: `dotnet build -t:Run -f net9.0-android FieldOps.Client\FieldOps.Client.csproj`
  (the client opens `http://10.0.2.2:6304/`). iOS Simulator: same with `-f net9.0-ios`, on a Mac.
- Licence key: `FieldOps.Client/HybridLicense.props` (git-ignored, see `HybridLicense.props.example`).

## What to try

| Action | Expected result |
|---|---|
| Phone layout, tap WO-4821 | Detail full screen: *High priority · Pump P-2 inlet*, Meter reading, Notes, Submit reading. |
| Type `412.8` and a note, tap the back arrow | **Leave WO-4821?** *The reading 412.8 has not been submitted yet.* — Stay keeps you there; Leave discards the draft and pops to the list. |
| Type a reading, then open FieldOps in a new session (another client or device for the same technician) | Opens straight into WO-4821 with *Draft restored — WO-4821 · reading 412.8*. |
| In the Hybrid client: minimise / restore (Windows), take a call (phone) | Server log `lifecycle: …`; the draft is saved on `Stopped`/`Backgrounding` and restored on `Resumed` / after the reload. |
| Android back gesture | `Device.Application.BackRequested` → the same `NavigateBack`. |
| **Submit reading** with `abc` | *"abc" is not a meter reading. Enter a number such as 412.8.* |
| **Submit reading** with `412.8` | *Reading 412.8 submitted for WO-4821.*; back now pops without asking. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| Hybrid client for Android and iOS (`net9.0-android`, `net9.0-ios`), `Wisej-4-Hybrid-Native`, no-trimming/linking/AOT block | [FieldOps.Client.csproj](FieldOps.Client/FieldOps.Client.csproj) |
| `Startup.cs`: `UseWisejHybrid` from `public static MauiApp Main()`, `StartupUrl` from one class of addresses, `ReloadOnForeground` stated | [Startup.cs](FieldOps.Client/Startup.cs), [FieldOpsServer.cs](FieldOps.Client/FieldOpsServer.cs) |
| Native permissions and cleartext for the dev server | [AndroidManifest.xml](FieldOps.Client/Platforms/Android/AndroidManifest.xml), [network_security_config.xml](FieldOps.Client/Platforms/Android/Resources/xml/network_security_config.xml), [Info.plist](FieldOps.Client/Platforms/iOS/Info.plist) |
| `Device.Lifecycle.Backgrounding` / `Stopped` / `Resumed` when `Device.Valid`, save id + draft, reopen on load and resume | `WorkOrderPage_Load`, `SaveDraft`, `RestoreDraft` in [WorkOrderPage.cs](FieldOps/WorkOrderPage.cs); [Services/DraftStore.cs](FieldOps/Services/DraftStore.cs) |
| One `NavigateBack` from `Device.Application.BackRequested` and the app-bar arrow; layout by `Device.System.IsPhone` / `IsTablet` | `NavigateBack`, `ConfirmLeave` ([LeaveDialog.cs](FieldOps/LeaveDialog.cs)); routing in [Program.cs](FieldOps/Program.cs) |
| Emulator builds and screenshots | [docs/MobileBuildLoop.md](docs/MobileBuildLoop.md), [docs/screenshots](docs/screenshots) |
| Lifecycle and back design, review answers | [docs/LifecycleAndBackNavigation.md](docs/LifecycleAndBackNavigation.md) |

## What was verified, and how

- **Builds:** `FieldOps` (net10.0-windows, net10.0); `FieldOps.Client` + `FieldOps.Offline` for
  `net9.0-windows10.0.19041.0` and **`net9.0-android`** (debug APK `com.contoso.fieldops-Signed.apk`,
  14 MB): 0 errors, 0 warnings. On this machine the Android build needs the workload-manifest
  workaround described in the course cookbook (a machine problem, not a project setting).
- **Browser** (`fieldops-m4`, 360 × 640): the phone list and detail as in the video; typed 412.8 and
  a note, back arrow → the Leave dialog; Stay kept the reading; Submit reading → *Reading 412.8
  submitted for WO-4821.*; back then popped to the list without asking.
- **Windows Hybrid client** (unpackaged, licensed, phone layout via `FIELDOPS_URL`): new session →
  *Draft restored — WO-4821 · reading 412.8* (server log + capture of the client window,
  `docs/screenshots/`); minimise/restore of my own client window → `Deactivated`, `Stopped`,
  `Resumed` (draft restored), `Activated` in the server log.
- **Not run:** the Android emulator (no AVD on this machine, none created) and anything iOS (needs a
  Mac). Therefore `Backgrounding`, the reload on foreground and `BackRequested` were not observed —
  Windows raised neither.

## Where this sample differs from the lesson

- **Project name:** the video calls the client `FieldOps.Mobile`; it is the same `FieldOps.Client`
  project from Module 3 with the mobile targets added (the lesson's "one client project").
- **The phone screen is a second page.** The desktop list (`WorkOrdersPage`, Module 3) stays for
  desktops and wide browsers; `WorkOrderPage` serves phones, tablets and narrow browsers. Both are
  the same codebase and the same services.
- **Priority and status labels:** *Urgent* in the video is FieldOps' *High*; *In progress / Complete*
  are *On site / Completed*. The detail line shows the order's fault ("Pump P-2 inlet").
- **Server address:** the video's `10.0.2.2:5000` is `10.0.2.2:6304`, this module's port.
- **The Local fallback stays registered** (packaged clients only, see Module 3) although this module's
  lesson describes FieldOps as Remote; the decision record from Module 1 chose Remote + Local fallback.
- **Windows behaviour:** minimise raised `Stopped`, not `Backgrounding`, and did not reload the WebView.
