# Final readiness checklist — the lesson's eight items, with evidence and where it comes from

`Diagnostics/ReadinessChecklist.cs` is the same list as code: **Run readiness checklist** walks the eight items,
calls each `Evidence()` and prints the result in the grid and the trace. Items that can be proven by the running
process are (reflection, file system, session state); items that were tests run earlier in the course say where the
test lives instead of pretending to re-run it.

| # | Item | Evidence | Source |
|---|---|---|---|
| 1 | Application starts through Wisej.NET startup files and shows the correct main view | `Default.json startup → OrderDesk.Program.Main → Application.MainPage = MainPage; this session is on MainPage` | live: `Application.MainPage.GetType()`; Module 2 (`Default.html / Default.json / Program.cs / Startup.cs`) |
| 2 | No per-user state remains in unsafe static fields | `StaticStateAudit: 0 open item(s) · N static field(s) scanned · readonly shared state in: AppLog, AuditLog, AuthService, HealthCheck, HtmlSanitizer, InMemoryOrderRepository, ResponsiveLayout, SampleData, ThemeService, Ui · user context lives in Application.Session (UserSessionContext)` | live: `Diagnostics/StaticStateAudit.cs` reflects over every static field of the assembly; writable = open item, except `AuditLog._version` (a counter mutated under the lock). Module 4 moved `AppState.CurrentUser` into the session. **Static-state audit** button prints every field. |
| 3 | All registry, local file, Office Automation and process-launch assumptions are reviewed | `registry → session/profile store (M4) · C:\Orders → storage root <App_Data> (M6) · Excel Interop → CsvExport in memory (M1/M6) · Process.Start → none left (SecurityReview.md)` | Module 1 workbook + Modules 4 and 6; `grep` of the project finds no `Microsoft.Win32.Registry`, no `C:\`, no `Excel.Application`, no `Process.Start` outside `Legacy/` copies (Module 7 ships none) |
| 4 | Large grids are tested with production-like row counts | `Module 5: VirtualMode + CellValueNeeded against a private InMemoryOrderRepository of 200,000 orders; paging via OrderQuery.Skip/Take — not re-run here` | Module 5 console (timing table: load-all vs server-side filter + virtual block cache); the capstone keeps the five-order grid on purpose |
| 5 | Transient dialogs are disposed | `InvoicePreviewForm: ShowDialog((form, result) => form.Dispose()) — the caller disposes in the callback (Module 3 rule); no blocking ShowDialog() return values in the project` | `MainPage.PrintInvoice`; Module 3's leak counter; the trace line `• server InvoicePreviewForm closed and disposed` after closing the preview |
| 6 | AllowHtml is used only with trusted or sanitized content | `1 label with AllowHtml = true on user data → fed only by HtmlSanitizer (whitelist b, i, br); the Raw label exists as the documented failure path and is off by default` | `docs/SecurityReview.md` item 3; `MainPage.Designer.cs` (`labelSanitized.AllowHtml = true`, `labelRaw` empty until the unsafe button) |
| 7 | Responsive behaviour is tested at desktop, tablet and phone sizes | `ClientProfiles.json: Phone ≤600 · Tablet 601–1024 · Desktop ≥1025; ResponsiveLayout applies three layouts; active now: Desktop · browser W×H — tested = what docs/ResponsiveProfiles.md lists, nothing more` | live: `Application.ActiveProfile`; `docs/ResponsiveProfiles.md` (simulate buttons + browser resize; no real phone) |
| 8 | Deployment configuration, secrets, logs, temp paths and health checks are documented | `5/5 deployment files present · /health endpoint in Startup.cs · logs in <App_Data>\logs` | live: `File.Exists` on `docs/DeploymentChecklist.md`, `deploy/iis/web.config`, `deploy/docker/Dockerfile`, `deploy/docker/docker-compose.yml`, `deploy/appsettings.Production.notes.md`; `Startup.cs`; `AppLog.LogFolder` |

## Evidence (in the running app, Readiness & deploy tab)

- **Run readiness checklist** → `gridReadiness` has the eight rows, `#` in green; trace `• server readiness 1/8 ✓ Starts
  through … — Default.json startup → …` through `readiness 8/8`; banner *✓ Readiness checklist: 8/8 items with evidence
  from the running app (items 4 and 8 point at docs and files …)*; status `● readiness 8/8`.
- **Static-state audit** → one trace line per static field (`✓ readonly AuditLog.Entries`, `✕ writable AuditLog._version`,
  …), then `• server StaticStateAudit 0 open item(s) · …`; green banner *✓ 0 open items · N static fields scanned …*.
- **Health check** → the ✓ lines in `labelHealth`; **GET /health** → the JSON in a new tab and in the trace.

What is deliberately **not** claimed: a re-run of the 200,000-row test (item 4, Module 5), a phone device (item 7),
an executed IIS/Docker deployment (item 8 — the files exist and are documented; running them is the deployment).
