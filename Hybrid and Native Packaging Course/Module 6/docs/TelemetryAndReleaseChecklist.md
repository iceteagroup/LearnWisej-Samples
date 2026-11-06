# Crash and usage telemetry, and the release checklist

## Crash reporting

| Where | What | Code |
|---|---|---|
| Hybrid client (native) | Unhandled `AppDomain` and unobserved `Task` exceptions → local `crash.log` in `FileSystem.AppDataDirectory` + a short line POSTed to `<server>/client-log`. Your crash service's SDK call goes in `CrashReporting.Report`. | `FieldOps.Client/CrashReporting.cs`, `Startup.cs` |
| Hybrid client (bridge) | `HybridConfig.LogUrl = <server>/client-log` — the client's POST logging | `Startup.cs` |
| FieldOps server | `Application.ThreadException` per session → `UNHANDLED session … client WinUI 2.6.0 …` | `Program.cs` |
| FieldOps server | `/client-log` endpoint writes what clients POST into the server log | `Startup.cs` |

## Usage events (`UsageTelemetry.Record(feature, outcome)`)

Fields: `at` (UTC), `feature`, `outcome`, `client` (hybrid/browser), `platform`
(`Device.System.Platform` or Browser), `model` (`Device.System.Model`), `osVersion`
(`Device.System.Version`), `clientVersion` (from the version check), `session` (8 chars).
**Never** photo or signature contents, names, or coordinates.

| Feature | Outcomes |
|---|---|
| `photo.capture` | ok, cancelled, failed |
| `photo.upload` | ok (browser fallback) |
| `signature.save` | ok, failed |
| `location.resolve` | ok, no-fix |
| `map.open` | ok, no-app, browser-link |
| `reminder.schedule` | ok |
| `permission.camera`, `permission.locationwheninuse` | already granted, granted after the reason, `Denied, rationale=True`, `Denied, rationale=False` |
| `version.check` | behind-latest, update-required |
| `update.store` | opened, not-opened |

Observed in this lab (server log):

```
usage: {"feature":"signature.save","outcome":"ok","client":"browser","platform":"Browser","model":null,...}
usage: {"feature":"version.check","outcome":"behind-latest","client":"hybrid","platform":"WinUI","model":"Precision 3260","osVersion":"10.0.26200.9711","clientVersion":"2.6.0",...}
usage: {"feature":"version.check","outcome":"update-required","client":"hybrid","platform":"WinUI",...,"clientVersion":"2.6.0",...}
```

## Release checklist — FieldOps 2.6.0

### Browser
- [ ] Server 2.6.0 deployed; `ClientPolicy.json` reviewed (minimum unchanged unless a dependency changed)
- [ ] Cold-session smoke test: list, detail, status flow, two windows
- [ ] Browser fallbacks exercised: Upload Photo, Open route, Export → download

### Installed PWA
- [ ] `fieldops-shell-vN` bumped in `sw.js` **and** `ShellVersion.cs` if any shell file changed
- [ ] Update flow verified from the previous build (reload bar, one cache left)
- [ ] Offline page still honest (last synced summary, nothing editable)

### Hybrid client — Remote (Android, iOS, Windows, macOS)
- [ ] Server deploy seen on each platform (support log: `Device.Valid=True Platform=…`)
- [ ] Store release **only** if FieldOps.Client changed (permission, icon, extension, Hybrid package bump)
- [ ] Minimum client version reviewed per platform; staged rollout set; crash dashboard watched day 1

### Hybrid client — Local fallback (FieldOps.Offline)
- [ ] Any change to the shared screens = signed store release per platform
- [ ] MSIX certificate, Android upload key, Apple certificate/profile valid (expiry checked)
- [ ] Offline path tested on a packaged client: network off > 5 s → banner, change saved on device, replayed on reconnect

### Per platform signing
- [ ] Windows MSIX: `Get-AuthenticodeSignature` = Valid, timestamped, publisher = manifest
- [ ] Android: upload-key fingerprint matches Play Console
- [ ] iOS / macOS: `codesign -dv` shows the distribution identity; profile not expiring this quarter

### What this checklist failed to catch when walked through once
- The **unpackaged** Windows build reports the assembly version, not `ApplicationDisplayVersion` — a
  forgotten `AssemblyVersion` bump blocks every unpackaged laptop at the version check.
- The **Local fallback** only exists in the packaged Windows client; an unpackaged "quick install"
  silently loses offline support (support log: `OfflineUrl=(not started)`).
