# Version compatibility rule and update strategy

## The rule (server 2.6.0)

`FieldOps/ClientPolicy.json`, read by `ClientReleasePolicy` on every check:

| Platform (`Device.System.Platform`) | MinimumClient | LatestClient | Update from |
|---|---|---|---|
| Android | 2.4.0 | 2.6.0 | Managed Google Play |
| iOS | 2.4.0 | 2.6.0 | Company Portal (ABM custom app) |
| MacCatalyst | 2.4.0 | 2.6.0 | Company Portal |
| WinUI | 2.4.0 | 2.6.0 | Company Portal (Intune) |

`btnStartShift_Click` (also run when the page loads, before any work-order query):

1. `Device.Valid` false (browser, PWA) or the Local app → no check, load the work orders.
2. Read `Device.Versioning.CurrentVersion` (and `CurrentBuild`) and `Device.System.Platform`; tag the
   session with `ClientPlatform` / `ClientVersion` for every later log line and usage event.
3. Below **MinimumClient** → the blocking `ClientUpdatePanel` (*This FieldOps app must be updated*,
   Installed / Required, **Open <store>** → `Device.Launcher.TryOpenAsync(storeUrl)`), **no work order loaded**.
4. Below **LatestClient** → a dismissible notice *A newer FieldOps client is available*.
5. `IsFirstLaunchForCurrentVersion` → a one-time *What's new in 2.6.0* note (not
   `IsFirstLaunchForCurrentBuild`, which would fire on every rebuild).

Versions are compared as major.minor.build (`2.6` = `2.6.0` = `2.6.0.0`).

### When to raise MinimumClient

Only when the server starts **depending** on something an older client cannot do: an extension the old
client never registered (for example biometric sign-in, `.UseWisejAuthentication()`), a permission its
manifest lacks, or a newer `Wisej-4-Hybrid-Native` paired with the server's `Wisej-4-Hybrid`. New
screens, new handlers, fixes — never: they reached every Remote client the moment the server was deployed.

## Update strategy

| Client | How a change reaches it | Store release needed when |
|---|---|---|
| Browser | next session after the server deploy | never |
| Installed PWA | service-worker update flow: bump `fieldops-shell-vN`, reload bar (Module 2) | never |
| Hybrid **Remote** (Android, iOS, Windows, macOS) | next load after the server deploy (`ReloadOnForeground` reloads on return) | only when **FieldOps.Client** changes: a permission, an icon, an extension registration, `FieldOpsServer` addresses, a `Wisej-4-Hybrid-Native` bump |
| Hybrid **Local** (FieldOps.Offline inside the client) | **every** change, even a one-line screen fix: signed store release per platform | always |

## Verified on the Windows Hybrid client (server log)

```
version: WinUI client 2.6.0 (build 0) against minimum 2.4.0, latest 2.6.0  -> admitted, "What's new in 2.6.0" (first launch of 2.6.0)
version: WinUI client 2.6.0 against minimum 2.4.0, latest 2.7.0           -> usage version.check behind-latest, notice shown
version: WinUI client 2.6.0 against minimum 2.7.0, latest 2.7.0           -> BLOCKED, update screen, no orders loaded
```

Found on the way: an **unpackaged** Windows client reports the **assembly** version
(`AssemblyVersion` in `Properties/AssemblyInfo.cs`) as `Device.Versioning.CurrentVersion` — with the
template's `1.0.0.0` it was blocked although `ApplicationDisplayVersion` was 2.6.0. FieldOps keeps both
at 2.6.0. A packaged client reports its MSIX package version.

## Review answers

- **Four server deploys since the Android client was installed:** none of them should have raised the
  minimum unless the server began to depend on a client capability (extension, permission, native
  package pairing). None needed a store release — Remote clients took each deploy on the next load.
- **One fix, Remote vs Local on iOS:** Remote — commit → CI → deploy the FieldOps server → the
  technician's next load. Local — commit → client build on the Mac → sign → App Store Connect /
  ABM custom app → review → MDM assignment → device install. Shorten it with a pre-approved pilot group,
  automated signing on the Mac agent, and by keeping the Local app small (the fallback, not the product).
- **Upload fallback rising on iOS only:** (1) the camera permission is denied on iOS
  (`permission.camera` events with `Denied`/`DeniedForGood`), (2) the iOS client cannot capture
  (`CaptureSupported` false — e.g. a simulator or managed restriction) so the screen offers Upload,
  (3) technicians use the PWA/Safari on iPhones instead of the client (`client = browser`,
  `platform = Browser`). The `client`, `platform`/`model` and the preceding `permission.*` outcome
  in the event tell them apart.
