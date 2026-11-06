# The FieldOps mobile build loop

## Project

`FieldOps.Client/FieldOps.Client.csproj` (template: *Wisej.NET 4 Hybrid Client Application*),
`Wisej-4-Hybrid-Native` 4.1.7, MAUI 9.0.120. Target frameworks:

| Built on | Target frameworks |
|---|---|
| Windows | `net9.0-android`, `net9.0-windows10.0.19041.0` |
| macOS | `net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst` |

The fixed block for every configuration: `Optimize` false, `EnableAssemblyILStripping` false,
`MtouchLink` None, `MtouchUseLlvm` False, `RunAOTCompilation` False (plus `PublishTrimmed` False).

## Addresses

`FieldOpsServer.cs` holds every environment: Development `http://10.0.2.2:6304/` on Android (the
emulator's way to the developer's machine — its own `localhost` is the device) and
`http://localhost:6304/` elsewhere; Staging and Production over HTTPS; `FIELDOPS_URL` overrides it
(for example a Dev Tunnels URL that gives every platform one HTTPS address).

## Cleartext for the development server

- Android: `android:networkSecurityConfig="@xml/network_security_config"` in
  `Platforms/Android/AndroidManifest.xml`; `Platforms/Android/Resources/xml/network_security_config.xml`
  permits cleartext to `10.0.2.2` and `localhost` only (the template's blanket
  `usesCleartextTraffic="true"` was removed).
- iOS: `NSAppTransportSecurity` → `NSAllowsLocalNetworking` in `Platforms/iOS/Info.plist` (replacing
  the template's `NSAllowsArbitraryLoads`).

## Permissions

`Platforms/Android/AndroidManifest.xml` now also declares `android.permission.VIBRATE`. Camera and
location (Module 5) are already declared by the template, with the matching usage descriptions in
`Info.plist`. A new permission is a **client** change: rebuild, then a store release.

## Commands

```powershell
dotnet workload restore FieldOps.Client\FieldOps.Client.csproj
dotnet build -t:Run -f net9.0-android FieldOps.Client\FieldOps.Client.csproj   # emulator running
dotnet build -t:Run -f net9.0-ios     FieldOps.Client/FieldOps.Client.csproj   # on a Mac, Simulator
```

Inspect the page: `chrome://inspect` (Android), Safari → Develop (iOS); F12 in the Windows client.
`DebugMode` is on in Debug builds by default. Server-side changes need no client rebuild: restart the
FieldOps server and bring the app to the foreground.

## What happened on the build machine

| Step | Result |
|---|---|
| `dotnet build -f net9.0-android` | Built: `bin/Debug/net9.0-android/com.contoso.fieldops-Signed.apk` (14 MB, debug-signed) |
| Run on an Android emulator | **Not run** — no Android Virtual Device exists on this machine and none was created |
| `net9.0-ios` build / Simulator | **Not built** — needs a Mac with Xcode |
| Windows client, phone layout (`FIELDOPS_URL=http://localhost:6304/?view=phone`) | Ran; see `screenshots/windows-client-phone-view-draft-restored.png` (a capture of the client's own window) |

The lab asks for one emulator or simulator screenshot per platform; neither could be produced here.
The screenshot in this folder is the same phone screen in the Windows Hybrid client.
