# Hybrid & Native Packaging Course — FieldOps

Lab samples for the LearnWisej course **Hybrid & Native Packaging**. FieldOps is a field-technician
work-order app: one Wisej.NET application reached as a browser tab, an installed PWA and the
**Wisej.NET Hybrid** client (.NET MAUI) on Windows, Android, iOS and macOS. Each `Module N` folder is
the complete solution at the end of that module (cumulative by copy).

| Module | Folder | Port | What it adds |
|---|---|---|---|
| 1 | `Module 1` | 6301 | `WorkOrder` + `WorkOrderService` (forward-only status rule, version check), list, detail and status flow, the `Device.Valid` photo gate; delivery comparison, decision record, capability design, architecture |
| 2 | `Module 2` | 6302 | manifest + icons, `sw.js` (shell cache-first, session untouched), server-timed install offer, `offline.html` with the last synced summary, update bar + build-stamp check |
| 3 | `Module 3` | 6303 | `FieldOps.Client` (Hybrid client) + `FieldOps.Offline` (Local app); title bar, export with `Device.FileSystem`, new-order notification via server push, second window, Local fallback, MSIX |
| 4 | `Module 4` | 6304 | Android/iOS targets, `FieldOpsServer` addresses, `ReloadOnForeground`, `Device.Lifecycle` + draft restore, one `NavigateBack` for `BackRequested` and the app-bar arrow, phone/tablet layout |
| 5 | `Module 5` | 6305 | `DeviceCapabilities`, `PermissionGate`, Take Photo / Upload, signature pad, Navigate (geolocation + maps), local reminder, capability matrix |
| 6 | `Module 6` | 6306 | signing configuration, distribution plan + store listing, `ClientReleasePolicy` + `Device.Versioning` check with the update screen, usage and crash telemetry, release checklist |

## Projects in a module

```
Module N/
  FieldOps.slnx
  FieldOps/            the Wisej.NET web app (net10.0-windows;net10.0) - every module
  FieldOps.Client/     the Wisej.NET Hybrid client (.NET MAUI, net9.0 targets) - Module 3 on
  FieldOps.Offline/    the Local app (embedded EmbedIO server) - Module 3 on; compiles the FieldOps screens
  docs/                the module's written deliverables
  README.md            what to try, lab steps -> code, what was verified, differences from the lesson
```

## Versions (pinned)

`Wisej-4` 4.1.7 · `Wisej-4-Hybrid` 4.1.7 · `Wisej-4-Hybrid-Native` 4.1.7 · `EmbedIO` 3.5.2 ·
`Microsoft.Maui.Controls` 9.0.120 (the Hybrid native package targets net9.0 platforms). No previews.

## Prerequisites

- .NET 10 SDK (the web app) and the .NET MAUI workloads (`dotnet workload restore FieldOps.Client\FieldOps.Client.csproj`).
- Windows: the Windows App SDK runtime; Windows Developer Mode to deploy the packaged client from Visual Studio.
- Android: the Android SDK and an emulator (the emulator reaches your machine at `10.0.2.2`).
- iOS and Mac Catalyst: a Mac with a compatible Xcode, the matching .NET SDK and MAUI workloads, and signing.

## Run

```bash
cd "Hybrid and Native Packaging Course/Module 3/FieldOps"
dotnet run -f net10.0 --urls http://localhost:6303
```

Windows client, Remote only (no package):

```powershell
dotnet build ..\FieldOps.Client\FieldOps.Client.csproj -f net9.0-windows10.0.19041.0 -p:WindowsPackageType=None
..\FieldOps.Client\bin\Debug\net9.0-windows10.0.19041.0\win10-x64\FieldOps.Client.exe
```

The Local fallback needs the **packaged** Windows client: open `FieldOps.slnx` in Visual Studio,
start `FieldOps.Client` with the *Windows Machine* (MSIX) profile. See Module 3. Each module's
`FieldOps.slnx` marks the client **Deploy**, which that profile requires, and on Windows the client
lists `net9.0-windows10.0.19041.0` first so Visual Studio debugs the Windows target rather than the
Android one (check the framework in the debug-target dropdown if the Output pane shows emulator
checks). If Visual Studio still reports "The project needs to be deployed before we can debug", tick
Deploy for FieldOps.Client in Build > Configuration Manager, and make sure Windows Developer Mode is on.

## The Hybrid licence key

The key is never committed. Each module's client reads it at build time from
`FieldOps.Client/HybridLicense.props` (git-ignored — copy `HybridLicense.props.example` and put your
key in it) or from the `WISEJ_HYBRID_LICENSE` environment variable; the build compiles it into
`obj/HybridLicense.g.cs` and `Startup.cs` passes it to `config.LicenseKey`. Without a key the client
runs with a *Get a free Hybrid License from Wisej.com* band at the top of the window.
Module 6 adds `Signing.props` (also git-ignored) for release signing identifiers.

## What was and was not verified

Every web app ran in a browser (the course's browser pane); the Windows Hybrid client of Modules 3–6
ran unpackaged against its module's server, proven from the server log and from captures of the
client's own window. Android was **built** (APK/AAB) but not run (no emulator on the build machine);
iOS and Mac Catalyst were not built (no Mac); the **Local fallback was not run** (it needs the packaged
client, i.e. registering an MSIX, which the build machine may not do); no package was signed or
installed. Each module README says exactly what ran.

Read [`_template/COOKBOOK.md`](_template/COOKBOOK.md) before extending a module: it records the
platform facts these samples depend on (the Local app's package-identity requirement, the unpackaged
version source, `Device.TitleBar`, template casing, the MAUI 9 pin).
