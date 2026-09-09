# Release checklist — what each check means for this project

Lesson s42 §4 gives nine **required** checks. The lab screen (`ReleaseReviewWindow`, card "Release checklist")
lists them verbatim in `chkRequired`; the package status turns green only at 9 / 9. Optional items live in
`chkOptional` and never change the status — unchecked ones are reported as *known issues* in the summary.

## Required (gate the package status)

| # | Check | What it means for `WisejTrainingApp` | Where to look |
|---|---|---|---|
| 1 | Web.config reviewed and debug mode set correctly | `Web.config` carries `Wisej.LicenseKey` (empty here — see check 2) and `Wisej.DefaultTheme`. `Default.json` has `"debug": true` — set it to `false` for Staging/Production so the client does not expose debug output. | `Web.config`, `Default.json` |
| 2 | Wisej.NET license key handled securely | The key is **not** in source. `Services/SecureConfig.cs` reads `WISEJ_LICENSE_KEY` (environment) and falls back to the `Wisej.LicenseKey` appSetting; the screen shows only `configured (masked ••••1234)` or `MISSING — set WISEJ_LICENSE_KEY`. | `Services/SecureConfig.cs`, card "Secrets" |
| 3 | Default/startup window, theme and URL settings checked | `Default.json`: `"url": "Default.html"`, `"startup": "WisejTrainingApp.Program.Main, WisejTrainingApp"`, `"theme": "Bootstrap-4"`. `Program.Main` opens `ReleaseReviewWindow`. | `Default.json`, `Program.cs` |
| 4 | Themes and static resources included | `Default.html` (the page that loads `wisej.wx`) is `CopyToPublishDirectory = Always` in the csproj; built-in themes ship inside `Wisej.Framework.dll`. Modules with a `Widgets/` folder must publish it too — `Startup.cs` serves the project folder. | `WisejTrainingApp.csproj`, `Startup.cs` |
| 5 | Logging destination configured | `Services/SafeLogger.cs` keeps the per-session log and mirrors every line to `System.Diagnostics.Trace`. In production, point a `TraceListener` (or a real logging provider) at a file or the host's log stream. | `Services/SafeLogger.cs` |
| 6 | Authentication and authorization plan reviewed | Authentication = `currentUser` (Name/Role) — in a real app it comes from the identity provider. Authorization = `ApplyPermission()` (UI gate: `btnReviewPackage.Enabled`) **and** `ReleaseReviewService.TryCreatePackage` (server gate: `Admin` or `Team Lead`). | `ReleaseReviewWindow.cs`, `Services/ReleaseReviewService.cs` |
| 7 | Sensitive files are not publicly downloadable | `Startup.cs` serves static files with `UseWhen(path does not end with .json)`, so `Default.json` is never served; `Web.config` and `Default.json` are `CopyToOutputDirectory = Never`. Nothing under `docs/` contains secrets. | `Startup.cs`, `WisejTrainingApp.csproj` |
| 8 | Release build tested locally before deployment | `dotnet build -c Release -f net10.0` then `dotnet run -c Release -f net10.0 --urls http://localhost:5089` and walk the "What to try" table in the README. | README |
| 9 | Deployment target requirements checked | The notes box (`txtNotes`) is pre-filled per target from `Services/DeploymentNotes.cs`; `docs/DeploymentNotes.md` has the IIS / Kestrel / Cloud table. | `Services/DeploymentNotes.cs`, `docs/DeploymentNotes.md` |

## Optional (do not gate)

| Item | Why it is optional here |
|---|---|
| Theme Builder tweaks reviewed | This project uses the stock `Bootstrap-4` theme; only a custom theme needs a second look. |
| Screenshots redacted | Training material only — but a screenshot of the "Secrets" card must show the masked status, never a value. |
| Staging smoke test done | Recommended before Production; the lab has no staging slot. |
| Rollback plan written | The summary prints "NOT documented — write the rollback plan before go-live" until this is ticked. |

## How the gate is implemented

```csharp
// ReleaseReviewWindow.cs
private bool AllRequiredChecksComplete()
{
    return chkRequired.Items.Count > 0
        && chkRequired.CheckedItems.Count == chkRequired.Items.Count;
}

private void chkRequired_AfterItemCheck(object sender, ItemCheckEventArgs e)
{
    bool done = chkRequired.GetItemChecked(e.Index);
    AddLog(done ? $"Required check {chkRequired.CheckedItems.Count}/9 completed: …" : "Required check unchecked …");
    UpdatePackageStatus();   // red "NOT READY · n / 9" → green "READY for deployment review · 9 / 9"
}
```

`chkRequired.CheckOnClick = true`, so one click toggles an item. The service repeats the same count check
from the `ReleaseRequest` it receives (`RequiredChecked < RequiredTotal` → "Required checks incomplete."),
so a client that skips the UI still cannot package an unreviewed release.

## Evidence

- On load `lblPackageStatus` reads **Package status: NOT READY · 0 / 9 required checks** in red.
- Tick items one by one: the log shows `Required check 1/9 completed: Web.config reviewed and debug mode set correctly` … and the count in the status label follows.
- At the ninth tick the label flips to **Package status: READY for deployment review · 9 / 9 required checks** in green. Untick one and it returns to NOT READY · 8 / 9.
- Optional items log `Optional item checked: … (does not change the package status)` and leave the status alone.
- **Complete all required checks** does the nine ticks at once and logs one line; **Reset checklist** clears both lists and the summary.
