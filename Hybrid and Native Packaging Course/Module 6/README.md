# FieldOps · Hybrid & Native Packaging · Module 6 (capstone)

Lab build for **Module 6 · Signing, Distribution, Updates and the FieldOps Capstone**. The walkthrough's
client screen — **FieldOps · Today**, the grid Order · Task · Location · State (WO-4118 Pump seal
replacement · Bay 3 · Kirkwall, WO-4119, WO-4121, plus WO-4187 from Module 5), the
**Take Photo · Signature · Navigate** bar and the version note — and the update screen an outdated
client gets instead: **This FieldOps app must be updated**, *Installed 2.6.0 / Required 2.7.0*,
**Open Company Portal**, `Device.Launcher.TryOpenAsync(storePage)`.

## Run it

```bash
cd "Hybrid and Native Packaging Course/Module 6/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6306
```

Client as in Modules 3–5 (it is now version **2.6.0**). Change the WinUI line of
`FieldOps/ClientPolicy.json` (`minimum` / `latest`) and restart the client to see the three outcomes;
the policy is re-read on every check, no server restart needed.

## What to try

| Action | Expected result |
|---|---|
| Browser | List loads; no version check (`Device.Valid` false); footer *4 orders · browser*. |
| Client 2.6.0, policy 2.4.0 / 2.6.0 | Admitted; first launch of 2.6.0 → *What's new in 2.6.0: …* (dismiss by clicking it). |
| Policy latest 2.7.0 | *A newer FieldOps client (2.7.0) is available — update when you can. ✕*; work orders load. |
| Policy minimum 2.7.0 | The update screen; no work order loaded; Export / Open in new window / the device bar are gone; **Open Company Portal** → `Device.Launcher.TryOpenAsync`. |
| Any device feature (photo, signature, navigate, reminder, permission prompt) | One `usage:` line in the server log and `%LOCALAPPDATA%\FieldOps\usage.jsonl`: feature, outcome, platform, model, OS version, client version — no names, images or coordinates. |
| An unhandled exception in a session | `UNHANDLED session … client WinUI 2.6.0` in the server log. |

## Lab tasks → where in the code

| Lab step / deliverable | Implementation |
|---|---|
| Release build: no trimming/linking/AOT, `config.LicenseKey`, production `StartupUrl`, `MauiIcon`, (paid) `HybridSplashScreen` | [FieldOps.Client.csproj](FieldOps.Client/FieldOps.Client.csproj), [Startup.cs](FieldOps.Client/Startup.cs), [FieldOpsServer.cs](FieldOps.Client/FieldOpsServer.cs) |
| Signed Windows, Android and iOS packages; key custody | signing block in the .csproj + [Signing.props.example](FieldOps.Client/Signing.props.example); [docs/SigningAndPackages.md](docs/SigningAndPackages.md) |
| Enterprise distribution + store listing draft with privacy / data safety | [docs/DistributionAndStoreListing.md](docs/DistributionAndStoreListing.md) |
| `ClientReleasePolicy` (per-platform MinimumClient / LatestClient), `btnStartShift_Click` with `Device.Versioning` + `Device.System.Platform`, `ClientUpdatePanel`, notice, what's new | [Services/ClientReleasePolicy.cs](FieldOps/Services/ClientReleasePolicy.cs), [ClientPolicy.json](FieldOps/ClientPolicy.json), [WorkOrdersPage.cs](FieldOps/WorkOrdersPage.cs), [ClientUpdatePanel.cs](FieldOps/ClientUpdatePanel.cs) |
| Update strategy (Remote / Local / PWA / browser) and the compatibility rule | [docs/VersionCompatibilityAndUpdates.md](docs/VersionCompatibilityAndUpdates.md) |
| `UsageTelemetry` per device feature; crash reporting client and server; `config.LogUrl` | [Services/UsageTelemetry.cs](FieldOps/Services/UsageTelemetry.cs), calls in WorkOrderDetail / PermissionGate / ReminderService; [CrashReporting.cs](FieldOps.Client/CrashReporting.cs); `Application.ThreadException` in [Program.cs](FieldOps/Program.cs); `/client-log` in [Startup.cs](FieldOps/Startup.cs) |
| Release checklist for every delivery model | [docs/TelemetryAndReleaseChecklist.md](docs/TelemetryAndReleaseChecklist.md) |

## What was verified, and how

- **Builds:** `FieldOps`; `FieldOps.Client` + `FieldOps.Offline` for `net9.0-windows10.0.19041.0` and
  `net9.0-android` (Debug): 0 errors, 0 warnings.
- **Release packages** (no signing identity exists here, none was created):
  `dotnet publish -c Release -p:AppxPackageSigningEnabled=false` → unsigned
  `FieldOps.Client_2.6.0.260_x64.msix` (44.6 MB); `dotnet publish -f net9.0-android -c Release` →
  `com.contoso.fieldops.aab` (81 MB) and `com.contoso.fieldops-Signed.aab`, signed by the build with the
  machine's **debug** keystore because no `Signing.props` (upload key) exists — not a releasable
  signature. iOS / Mac Catalyst not built (no Mac). Nothing was installed.
- **Browser** (`fieldops-m6`): the Today list; the browser session skipped the version check; a
  signature produced `usage: {"feature":"signature.save","outcome":"ok","client":"browser",...}`.
- **Windows Hybrid client** (unpackaged, licensed) against three policies, from the server log and
  captures of the client window (`docs/screenshots/`): 2.4.0/2.6.0 → admitted + *What's new in 2.6.0*;
  2.4.0/2.7.0 → notice + `version.check behind-latest`; 2.7.0/2.7.0 → update screen,
  `BLOCKED WinUI 2.6.0`, `version.check update-required`.
- **Not exercised:** the store button's `TryOpenAsync` (needs a click in the window), the client crash
  path (no crash was provoked), Android/iOS runs.

## Where this sample differs from the lesson

- **Signed packages:** the lab asks for signed MSIX/AAB/iOS packages. The configuration and commands are
  complete, but the build machine has no signing identities and the brief forbids creating or trusting
  them, so the produced MSIX is unsigned and the AAB carries only the debug signature.
- **Unpackaged version source:** the first client run was blocked as `1.0.0` although
  `ApplicationDisplayVersion` was 2.6.0 — an unpackaged Windows client reports the assembly version.
  `Properties/AssemblyInfo.cs` now carries 2.6.0.0 as well.
- **Start shift** also runs automatically when the list loads ("checked on startup"), and stays as a
  button to re-check after an update. The video shows no such button.
- **The note strip** replaces the video's note line: *Admitted · client 2.6.0 is current*, the notice, or
  *What's new* (one at a time).
- **Export report / Open in new window** (Module 3) stay in the top strip; the video's capstone window
  does not show them.
- **States:** the video's *Closed* is FieldOps' *Completed*.
- **Crash reporting** is a hook (`CrashReporting.Report`) plus `/client-log`, not a third-party SDK — the
  samples ship no commercial libraries.
- **Client project name:** the video's `FieldOps.HybridClient` is `FieldOps.Client`.
