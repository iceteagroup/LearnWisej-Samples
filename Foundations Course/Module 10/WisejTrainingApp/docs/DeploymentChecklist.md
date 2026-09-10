# Deployment checklist — the s42 §4 required list applied to this project

Lesson s42 §4 (Module 9) gives nine **required** checks. The **Deployment** screen
(`Views/DeploymentView.cs`) lists them verbatim in `chkRequired`; `lblPackageStatus` reads
**Package status: NOT READY — n required check(s) still open** in red until all nine are ticked, then
**Package status: READY — all required checks complete.** in green. The right-hand card (`lblDeploymentNotes`)
carries the short form of this note.

## Required (gate the package status)

| # | Check | What it means for this project | Where to look |
|---|---|---|---|
| 1 | Web.config reviewed and debug mode set correctly | `Web.config` holds only `Wisej.LicenseKey` (empty) and `Wisej.DefaultTheme = Bootstrap-4`. The debug switch is in `Default.json`: `"debug": true` today — set it to `false` for Staging/Production so the client does not expose debug output. Both files are `CopyToOutputDirectory = Never` in the csproj; the publish step must carry them. | `Web.config`, `Default.json`, `WisejTrainingApp.csproj` |
| 2 | Wisej.NET license key handled securely | The key is **not** in source: `Wisej.LicenseKey` is an empty string on purpose. Supply it from the host — IIS: the server's `Web.config` transform / appSettings; Kestrel: an environment variable read at startup; Cloud: an app setting or Key Vault reference. Never commit it, never log it. | `Web.config` |
| 3 | Default/startup window, theme and URL settings checked | `Default.json`: `"url": "Default.html"`, `"startup": "WisejTrainingApp.Program.Main, WisejTrainingApp"`, `"theme": "Bootstrap-4"`. `Program.Main` opens `Window1` (the shell), which navigates to Dashboard in `Window1_Load`. The dev URL is `http://localhost:5090` (`Properties/launchSettings.json`). | `Default.json`, `Program.cs`, `Properties/launchSettings.json` |
| 4 | Themes and static resources included | `Default.html` (the page that loads `wisej.wx`) is `CopyToPublishDirectory = Always`. The four themes `cboTheme` offers (Bootstrap-4, BootstrapDark-4, Blue-1, Material-3) are built into `Wisej.Framework.dll`; this module has no `Widgets/` folder or custom theme file, so nothing else must ship. | `WisejTrainingApp.csproj`, `Default.html`, `Window1.Designer.cs` (`cboTheme.Items`) |
| 5 | Logging destination configured | Today every action goes to `Window1.AddActivity` (timestamped, into the Dashboard's `lstActivity`) and the job to `JobsView.LogJob / LogError` (`lstJobLog`) — per session, gone when the tab closes. Production: forward both helpers to `ILogger` / Serilog with a file, Seq or Application Insights sink (see `docs/NextSteps.md`). The lines already follow s42 §2: start/finish of workflows, validation failures, exceptions with type and message, no secrets. | `Window1.cs` (`AddActivity`), `Views/JobsView.cs` (`LogJob`, `LogError`) |
| 6 | Authentication and authorization plan reviewed | `lblUser` ("Signed in as: Support Agent") is a placeholder; there is no sign-in and every command is available. Plan: identity provider → user + roles in `Application.Session`; Delete and the Deployment screen gated **server-side** in `TicketService` / the view, not only by hiding buttons (s42 §1). | `Window1.Designer.cs` (`lblUser`), `docs/NextSteps.md` (Authentication) |
| 7 | Sensitive files are not publicly downloadable | `Startup.cs` serves static files only when the path does not end in `.json`, so `Default.json` is never served; `Web.config` and `Default.json` are `CopyToOutputDirectory = Never`. Nothing under `docs/` contains a secret. | `Startup.cs`, `WisejTrainingApp.csproj` |
| 8 | Release build tested locally before deployment | `dotnet publish -c Release -f net10.0` (or `dotnet build -c Release -f net10.0` then `dotnet run -c Release -f net10.0 --urls http://localhost:5090`) and walk the README's "What to try" table once: create, validation failure, edit, delete Yes/No, job with and without the simulated error, theme change, every nav screen. | README |
| 9 | Deployment target requirements checked | **Kestrel / self-host**: `--urls`, ports, a reverse proxy (nginx / IIS ARR) with HTTPS, `Default.json` present next to the binaries. **IIS**: the ASP.NET Core hosting bundle, an app pool (No Managed Code), the published folder with `Web.config`. **Cloud**: app settings for the license key, `"debug": false`, static files published, the logging sink configured, a staging slot before swap. | `lblDeploymentNotes` on the Deployment screen, `Startup.cs` |

## Optional (do not gate)

| Item | Note for this project |
|---|---|
| Theme Builder tweaks reviewed | Stock themes only; nothing custom to review. |
| Screenshots redacted | Training data only (Northwind, Contoso, …); no real customer data in any screenshot. |
| Staging smoke test done | Recommended before Production; the lab has no staging slot. |
| Rollback plan written | Keep the previous published folder; a rollback is a folder swap plus the same `Default.json`. |

## How the gate is implemented

```csharp
// Views/DeploymentView.cs
private void chkRequired_AfterItemCheck(object sender, ItemCheckEventArgs e)
{
    UpdatePackageStatus();
    Shell.AddActivity($"Deployment check \"{RequiredChecks[e.Index]}\" → {(e.NewValue == CheckState.Checked ? "done" : "undone")}");
}

private void UpdatePackageStatus()
{
    int done = chkRequired.CheckedItems.Count;
    int total = chkRequired.Items.Count;
    bool ready = done == total;

    lblPackageStatus.Text = ready
        ? "Package status: READY — all required checks complete."
        : $"Package status: NOT READY — {total - done} required check(s) still open.";
    lblPackageStatus.ForeColor = ready ? OkColor : ErrorColor;
    ShowStatus(lblStatus, ready ? "release package ready" : $"{done} of {total} required checks done", ready ? StatusKind.Ok : StatusKind.Warn);
}
```

`chkRequired.CheckOnClick = true`, so one click toggles an item. **Check all** (`btnCheckAll_Click`) is the
recovery path — it ticks all nine and logs one line; **Reset** (`btnResetChecklist_Click`) clears them and
the status returns to NOT READY.

## Evidence

- On opening the Deployment screen, `lblPackageStatus` reads **Package status: NOT READY — 9 required
  check(s) still open.** in red and `lblStatus` is amber `● 0 of 9 required checks done`.
- Tick items one by one: the count in both labels follows, and the Dashboard activity log gets
  `Deployment check "Web.config reviewed and debug mode set correctly" → done` per tick.
- At the ninth tick the label flips to **Package status: READY — all required checks complete.** in green
  and `lblStatus` to `● release package ready`. Untick one and it is NOT READY — 1 required check(s) still open.
- **Check all** → READY in one click, log line `btnCheckAll_Click → all required checks marked done → package READY`.
  **Reset** → NOT READY, log line `btnResetChecklist_Click → checklist cleared → package NOT READY`.
- The notes card lists, for each of the nine checks, the one-line answer for this project (Web.config,
  license key, startup, theme, static files, logging, auth plan, sensitive files, release build, target).
