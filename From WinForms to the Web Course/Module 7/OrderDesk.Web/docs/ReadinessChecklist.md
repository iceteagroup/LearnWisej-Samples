# Final readiness checklist — the lesson's eight items, with evidence and where it comes from

The lesson's Final Readiness Checklist, answered for OrderDesk.Web. Items that can be seen in the running app say how;
items that were tests run earlier in the course say where the test lives instead of pretending to re-run it.

| # | Item | Evidence | Source |
|---|---|---|---|
| 1 | Application starts through Wisej.NET startup files and shows the correct main view | `Default.json` `startup` → `OrderDesk.Program.Main` → `Application.MainPage = new MainPage()` | Module 2 (`Default.html / Default.json / Program.cs / Startup.cs`) |
| 2 | No per-user state remains in unsafe static fields | the signed-in user lives in `Application.Session` (`UserSessionContext`, read by `AuthService.Current`); the only statics are readonly shared state behind locks (the repository, the audit list, the account table) and `AuditLog._version`, a counter mutated under the lock | Module 4 moved `AppState.CurrentUser` into the session; `grep static` over the project |
| 3 | All registry, local file, Office Automation and process-launch assumptions are reviewed | registry → session / profile decision (M4) · `C:\Orders` → storage root under `App_Data` (M6) · Excel Interop → a file built on the server (M1/M6) · `Process.Start` → none left | Module 1 workbook + Modules 4 and 6; `grep` finds no `Microsoft.Win32.Registry`, no `C:\`, no `Excel.Application`, no `Process.Start` in this project |
| 4 | Large grids are tested with production-like row counts | Module 5: `VirtualMode` + `CellValueNeeded` over 200,000 orders; paging via `OrderQuery.Skip/Take` — not re-run here | Module 5 `GridPerformanceNotes.md`; the capstone keeps the five-order grid on purpose |
| 5 | Transient dialogs are disposed | `InvoicePreviewForm` is shown with `ShowDialog((form, result) => form.Dispose())` — the caller disposes in the callback (Module 3 rule) | `MainPage.PrintInvoice` |
| 6 | AllowHtml is used only with trusted or sanitized content | one label with `AllowHtml = true` on user data (`labelSanitized`), fed only by `HtmlSanitizer.Sanitize` (whitelist `b`, `i`, `br`) | `docs/SecurityReview.md` item 3; `MainPage.Designer.cs` |
| 7 | Responsive behaviour is tested at desktop, tablet and phone sizes | `ClientProfiles.json`: Phone ≤600 · Tablet 601–1024 · Desktop ≥1025; `ResponsiveLayout` applies three layouts — tested = what `docs/ResponsiveProfiles.md` lists, nothing more | resized browser; no real phone |
| 8 | Deployment configuration, secrets, logs, temp paths and health checks are documented | `docs/DeploymentChecklist.md`, `deploy/iis/web.config`, `deploy/docker/Dockerfile`, `deploy/docker/docker-compose.yml`, `deploy/appsettings.Production.notes.md`; `GET /health` (`Startup.cs`); logs in `App_Data/logs` (`AppLog`) | the files in `deploy/` and `docs/` |

What is deliberately **not** claimed: a re-run of the 200,000-row test (item 4, Module 5), a phone device (item 7),
an executed IIS/Docker deployment (item 8 — the files exist and are documented; running them is the deployment).
