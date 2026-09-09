# Deployment notes — targets, environments, and publishing this project

## IIS vs Kestrel vs Cloud (lesson s42 §3, junior-level checks)

| Target | Checks | For `WisejTrainingApp` |
|---|---|---|
| **IIS** | Publish the app folder, review `Web.config`, check the ASP.NET Core hosting bundle if needed, confirm the app pool, test the URL. | `dotnet publish -c Release -f net10.0-windows` → copy the publish folder to the site; app pool *No Managed Code*; `Web.config` in the publish folder must carry the real `Wisej.LicenseKey` (or the app pool identity must see `WISEJ_LICENSE_KEY`). |
| **Kestrel / self-host** | Confirm environment variables, ports, reverse proxy, HTTPS and JSON config files. | `dotnet publish -c Release -f net10.0`; run with `--urls http://localhost:5089` (or `ASPNETCORE_URLS`); put nginx / IIS ARR in front for HTTPS; set `WISEJ_LICENSE_KEY` and `TRAINING_CONNECTION_STRING` in the service's environment; ship `Default.json` next to the binaries. |
| **Cloud** | Confirm app settings, secrets, static files, logging and the deployment slot / staging setup. | App settings = the two environment variables above (or a key vault reference); `Default.html` + any `Widgets/` folder must be in the package; route `Trace` to the platform log stream; deploy to a staging slot, smoke-test, then swap. |

The same three texts are what `Services/DeploymentNotes.For(target)` puts in the notes box when `cboTarget` changes.

## Debug vs Staging vs Production (lesson s41 §2)

| Environment | Behaviour | This project |
|---|---|---|
| Debug | Detailed errors, local testing, easy troubleshooting — not safe for public users. | `Default.json` `"debug": true`, `dotnet run` from the project folder. The screen warns (amber status) when Debug is selected. |
| Staging | A production-like test area used before release. | Same build as Production, different host / slot; the optional "Staging smoke test done" item records it. |
| Production | Safe error messages, protected secrets, stable configuration, logging enabled. | `"debug": false`, secrets from the environment, `Trace` wired to a real destination, the nine required checks complete. |

**Safe release rule:** users see a useful message; developers see technical details in secure server-side logs.
Never show stack traces, file paths, license keys or connection strings in the UI.

## Publishing this project

```bash
cd "D:/Projects/LearnWisej-Samples/Foundations Course/Module 9/WisejTrainingApp"

# Kestrel / Linux-friendly build
dotnet publish -c Release -f net10.0 -o ./publish/kestrel

# IIS (Windows) build
dotnet publish -c Release -f net10.0-windows -o ./publish/iis
```

What lands in the publish folder and why it matters for the checklist:

- `WisejTrainingApp.dll`, `Wisej.Framework.dll` — the app and the framework (themes are embedded in the framework).
- `Default.html` — `CopyToPublishDirectory = Always` in the csproj (required check 4).
- `Default.json`, `Web.config` — **not** copied by the build (`CopyToOutputDirectory = Never`); the deployment step provides the production versions with `"debug": false` and the real settings (required checks 1 and 3). `Startup.cs` refuses to serve `*.json`, so `Default.json` is never downloadable (required check 7).
- No `docs/`, no source — nothing sensitive to leak.

Before go-live, run the published build once locally (`dotnet WisejTrainingApp.dll --urls http://localhost:5089`)
and walk the README's "What to try" table — that is required check 8.

## Evidence

- Change `cboTarget` IIS → Kestrel → Cloud: `txtNotes` swaps to the matching text and the log shows
  `Target → Kestrel: deployment notes loaded from Services/DeploymentNotes.cs`.
- Change `cboEnvironment` to Debug: the log explains *detailed errors, local testing — not safe for public users*
  and the status turns amber; Production logs *safe error messages, protected secrets, stable configuration, logging enabled*.
- The review summary's `Environment`, `Target` and `Notes` lines repeat the current selection and the first line of the notes.
