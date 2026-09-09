# WisejTrainingApp · Wisej.NET Foundations · Module 9

Local lab build for **Module 9 · Configuration, security and deployment review**. It is the lab's controlled
deployment review screen: release information (environment, target, version, reviewer), a role-gated
**Create Review Package** action backed by a server-side check, the lesson's nine-item required release
checklist that flips the package status from NOT READY to READY at 9 / 9, optional items, per-target
deployment notes, secrets read from secure configuration and shown masked, and a timestamped
troubleshooting log where every line is redacted before it is stored.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 9/WisejTrainingApp"
dotnet run -f net10.0 --urls http://localhost:5089
```

Then open <http://localhost:5089>. (Visual Studio: open `WisejTrainingApp.slnx`, press F5.)

To see the secrets as *configured* rather than *MISSING*, set the two environment variables first (fake values):

```powershell
$env:WISEJ_LICENSE_KEY = "training-demo-1234"
$env:TRAINING_CONNECTION_STRING = "Server=localhost;Database=Training;User Id=lab;Password=not-a-real-one"
dotnet run -f net10.0 --urls http://localhost:5089
```

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The project multi-targets
`net10.0-windows;net10.0`, so `dotnet run` needs `-f`.

## What to try

The app starts as **Support Agent** with 0 / 9 checks, so the first thing you see is the gate.

| Action | Path | What you should see |
|---|---|---|
| Look at the Release Information card | – | `cboRole` = Support Agent, `lblPermission` = *Review is restricted.* in red, **Create Review Package** disabled; status amber *Review not started.* |
| **Try review as Support Agent (server check)** | failure a | status red *Review is restricted.*; log: `Server refused (permission): Review is restricted. — the action is protected server-side, not just by btnReviewPackage.Enabled` |
| Pick **Team Lead** (or Admin) in `cboRole` | recovery | `lblPermission` = *Deployment review available.* in green, the button enables; log `Role changed → Team Lead: Deployment review available.` |
| Click **Create Review Package** with checks missing | failure b | status amber *Required checks incomplete.*; log `Review attempt → required checks incomplete (0 / 9)` |
| Tick required items one by one | progress | log `Required check 1/9 completed: Web.config reviewed and debug mode set correctly` …; `lblPackageStatus` counts `n / 9` in red |
| Tick the ninth (or click **Complete all required checks**) | progress → ready | `lblPackageStatus` = **Package status: READY for deployment review · 9 / 9 required checks** in green |
| Tick optional items | – | log `Optional item checked: … (does not change the package status)`; status unchanged |
| Change `cboTarget` IIS → Kestrel → Cloud | – | `txtNotes` swaps to the target's junior-level checks (from `Services/DeploymentNotes.cs`) |
| Change `cboEnvironment` to Debug | – | amber warning *Debug settings are for building, not for release…*; the log explains each environment |
| **Create Review Package** (Team Lead, 9 / 9) | success | log `Deployment package reviewed.`, status green *Package ready for deployment.*, a toast, and `txtSummary` filled with the safe summary: environment, target, version, reviewer, role, status, required/optional counts, masked secrets, known issues (unchecked optional items), rollback, first line of the notes |
| Set `txtVersion` to `1.4` and create again | failure (server) | status red *Version must look like major.minor.patch (e.g. 1.4.0).*; log `Review refused by the server (version): …` |
| Tick **Simulate packaging error**, create again | failure c | status red *Packaging failed. Please contact the release manager.* — no path, no stack trace on screen; log `Packaging failed: IOException — Access to path 'D:\deploy\ServiceDesk.zip' is denied (stack trace in Trace, not on screen)` |
| Untick the simulation, create again | recovery | back to *Package ready for deployment.* and a fresh summary |
| **What NOT to do** | – | four log lines explaining the unsafe hard-coded-key pattern; the fake `licensekey=DEMO-…` line arrives as `licensekey=•••[redacted]` — the redaction works |
| **Reset checklist** / **Clear log** | – | both lists unticked, summary cleared, status back to NOT READY · 0 / 9 / empty log |

The right-hand bottom card is the troubleshooting log: every user action and every server decision with a
timestamp, after `SafeLogger.Redact`. The same lines go to `System.Diagnostics.Trace` — the "server-side log".

## Where things live

```
WisejTrainingApp/
├─ Program.cs                       Wisej.NET session entry point: new ReleaseReviewWindow().Show()
├─ ReleaseReviewWindow.cs           code-behind: ApplyPermission, btnReviewPackage_Click, AllRequiredChecksComplete,
│                                   TryCreatePackage (safe error handling), checklist handlers, BuildRequest, AddLog/SetStatus
├─ ReleaseReviewWindow.Designer.cs  Designer-generated layout (InitializeComponent) — six cards + the bottom bar
├─ Models/
│  ├─ UserAccount.cs                currentUser: Name + Role (authentication answered, authorization asked)
│  ├─ ReleaseRequest.cs             what the service gets: env, target, version, reviewer, role, check counts, masked secrets, notes
│  └─ ReviewResult.cs               Success / Reason / Message (safe) / Summary
├─ Services/
│  ├─ ReleaseReviewService.cs       TryCreatePackage: server-side role check, required-checks check, version check, summary
│  ├─ SecureConfig.cs               WISEJ_LICENSE_KEY / TRAINING_CONNECTION_STRING → Web.config → missing; Mask() = ••••last4
│  ├─ SafeLogger.cs                 Log(): timestamp + Redact() (masks key=/password=/… values) + Trace mirror
│  └─ DeploymentNotes.cs            IIS / Kestrel / Cloud notes text (lesson s42 §3)
├─ Startup.cs                       Kestrel host (app.UseWisej(), static files from the project folder, never *.json)
├─ Default.json / Default.html / Web.config / Properties/launchSettings.json   (port 5089)
└─ docs/
   ├─ ReleaseChecklist.md           the nine required checks + optional items, what each means for THIS project, evidence
   ├─ DeploymentNotes.md            IIS vs Kestrel vs Cloud, Debug vs Staging vs Production, dotnet publish notes
   └─ SecretsAndLogging.md          unsafe vs safer table, SecureConfig + SafeLogger, authentication vs authorization
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| 1 · Open the project, run a release-mode build once | `dotnet build -c Release -f net10.0` from the project folder; `docs/DeploymentNotes.md` "Publishing this project" |
| 2 · Release checklist screen (required + optional items) | `RequiredItems` / `OptionalItems` in `ReleaseReviewWindow.cs`, `chkRequired` / `chkOptional` in the Designer; `docs/ReleaseChecklist.md` |
| 3 · Environment & target inputs + version field | `cboEnvironment`, `cboTarget`, `txtVersion`, `txtReviewer` (card "Release Information") |
| 4 · Deployment notes area | `txtNotes`, filled by `cboTarget_SelectedIndexChanged` from `Services/DeploymentNotes.cs` |
| 5 · Permission gate, backed by a server-side check | `ApplyPermission()` (UI: `btnReviewPackage.Enabled`, `lblPermission`) and `ReleaseReviewService.TryCreatePackage` → `IsAllowed(req.Role)`; **Try review as Support Agent** proves the server check |
| 6 · Keep secrets safe | `Services/SecureConfig.cs`; `ShowSecretStatus()` shows masked status only; `btnShowUnsafeExample_Click` |
| 7 · Troubleshooting log with timestamps, no secrets | `Services/SafeLogger.cs` (`Log`, `Redact`) through the window's `AddLog` |
| 8 · Package status that turns ready only when all required checks are complete | `AllRequiredChecksComplete()`, `UpdatePackageStatus()`, `chkRequired_AfterItemCheck` |
| 9 · Review summary from environment, target, version and notes | `ReleaseReviewService.BuildSummary` → `txtSummary` |
| 10 · Run & test: status flips to ready, safe messages only | the "What to try" table above; `TryCreatePackage` catch block for the simulated `IOException` |

Lab code check (`labs.js` m9) — paste `btnReviewPackage_Click` + `AllRequiredChecksComplete` from
`ReleaseReviewWindow.cs`: it has a `_Click` handler, checks `currentUser.Role == "Admin" || … "Team Lead"`,
gates on `AllRequiredChecksComplete()` / *Required checks incomplete* / *Package ready*, hard-codes no
password / connection / license value, and logs with `AddLog` and `lblStatus`.

## Self-check

**s41 — You're ready when you can locate the key configuration files, separate debug from production, and keep secrets safe.**

- **Which files do you inspect before release, and why?** `Program.cs` (which window starts), `Web.config` (debug flag,
  license key setting, hosting settings for IIS), `Default.json` (startup class, theme, URL, `debug`), the theme /
  static files (`Default.html`, any `Widgets/`), and the logging setup (`SafeLogger` → `Trace`). The deployed app must
  open the right entry point, look right, and be troubleshootable without exposing internals.
- **Debug vs staging vs production?** Debug = detailed errors and local testing, never for public users; Staging = a
  production-like rehearsal; Production = safe messages, protected secrets, stable configuration, logging on. The screen
  turns the status amber when Debug is selected as a reminder.
- **Where do the license key and connection string live?** Not in a page, a button event, a screenshot or Git. In
  secure configuration or environment variables — here `WISEJ_LICENSE_KEY` / `TRAINING_CONNECTION_STRING`, read by
  `SecureConfig`, which returns only `configured (masked ••••1234)` or `MISSING — set …`.

**s42 — You're ready for the lab when you can build a release checklist screen with required and optional checks, deployment notes, a troubleshooting log, and a package-status message that changes when the required checks are complete.**

- **Authentication vs authorization?** Authentication answers *who is the user* (`currentUser.Name`); authorization
  answers *what may they do* (`currentUser.Role` → `canReviewDeployment`). A Support Agent manages tickets; a Team Lead
  or Admin reviews deployments.
- **Why is hiding the button not enough?** Because the client can be bypassed. `ReleaseReviewService.TryCreatePackage`
  repeats the role check from the request, and **Try review as Support Agent** shows the server refusing even though
  the UI was never involved.
- **What goes in the troubleshooting log?** Start/finish of the review, validation and review failures, exceptions with
  developer detail — all timestamped — and never passwords, keys or private data (`SafeLogger.Redact`).
- **When does the package status change?** Only when all nine required items are checked
  (`chkRequired.CheckedItems.Count == chkRequired.Items.Count`); optional items never move it.

## Verified / unverified

Built with `dotnet build -nologo -v q` on this machine (Wisej-4 4.1.0, .NET 10): 0 errors, 0 warnings, both targets.
Not run in the browser by the author — the reviewer runs it. Calls the cookbook marks *unverified* that this module
depends on:

- `CheckedListBox.CheckOnClick`, `CheckedListBox.AfterItemCheck` (`ItemCheckEventHandler` / `ItemCheckEventArgs.Index`),
  `GetItemChecked(i)`, `SetItemChecked(i, value)`, `CheckedItems.Count` — the whole checklist gate. Two things to confirm at
  runtime: that `GetItemChecked(e.Index)` already reflects the new state inside `AfterItemCheck`, and whether
  `SetItemChecked` raises `AfterItemCheck` (the window suppresses per-item logging during bulk changes with `_bulkUpdate`
  either way).
- `ComboBox.DropDownStyle = DropDownList`, `Items.AddRange`, `SelectedIndex` set in `Load` raising `SelectedIndexChanged`
  (the defaults — Production / IIS / Support Agent — are applied through those handlers, so if the event does not fire
  on a programmatic change, `txtNotes` and `lblPermission` would stay at their Designer text).
- `TextBox.Multiline` + `ReadOnly` + `Watermark` for `txtSummary`.
- `AlertBox.Show(..., alignment: TopRight, autoCloseDelay: 4000)` (verified in the Integration course, same build).
- `Application.StartupPath` as the folder holding `Web.config` under `dotnet run` (the Module 1 sample relies on the same fact).
