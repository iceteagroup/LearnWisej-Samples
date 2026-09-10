# Capstone report — LegacyOrderDesk → OrderDesk.Web

The migration report the capstone presents: what was **reused**, **adapted**, **modernized**, which **risks remain**,
and what the **next slice** is — across all seven modules, with the deliverable that holds the detail.

**One line:** assess → port a slice → make it multi-user safe → cross the browser boundary → modernize → ship.
Parity first, then targeted modernization.

## Acceptance criteria (Lab 7)

| Criterion | Met by |
|---|---|
| It runs in a browser — and you can explain what was reused vs rebuilt | `http://localhost:5607`; the three tables below; every console button traces which side of the line it is on (`• server` reused logic, `⚠ boundary` replaced desktop assumption) |
| It survives two concurrent users with logging and rollback | **Open second session ↗**, kelly in one tab, sam in the other: separate session contexts, one audit log seen by both (`AuditLog (other session)` trace line), sam's Export denied, kelly's Download allowed; `docs/DeploymentChecklist.md` — backup & rollback (build output vs storage root) |

## Reused — moved unchanged

| What | Evidence | Module |
|---|---|---|
| `Domain/`: `Order`, `Customer`, `OrderLine`, `OrderService` (totals, discount, tax, search, save), `CustomerService`, `InvoiceDocument`, `SampleData` | copied verbatim from `LegacyOrderDesk`; no `System.Windows.Forms`, no desktop assumption; **New Order** computes 1,280.00 on both sides | 1 (`MigrationAssessmentWorkbook.md`: 4 direct-port items) |
| The workflow: search → select → New Order / Print / Export | same order of operations in `OrdersForm` and the Orders tab | 1, 3 |
| The event-driven model: handlers, `DialogResult`, `DataGridView` binding | `System.Windows.Forms` → `Wisej.Web` namespace swap, classified compiler-error log | 2 |
| Standard controls: grid, labels, buttons, menus | ported one-for-one; `MenuBar`/`ToolBar` map directly | 2, 3 |

## Adapted — kept, with a web-safe replacement for the desktop assumption

| Desktop assumption ✕ | Web replacement ✓ | Module |
|---|---|---|
| `Application.Run`, `EnableVisualStyles`, one .exe per user | `Default.json` → `Program.Main` sets `Application.MainPage` once per browser session; Kestrel owns the process | 2 |
| `App.config` | `Web.config` `<appSettings>` read with `System.Xml.Linq` (`AppConfig`) | 2 |
| Blocking `ShowDialog()` / `MessageBox` | `ShowDialog(callback)` / `ShowDialogAsync`, the caller disposes; `Ui.Toast` for confirmations | 3 |
| Static `AppState.CurrentUser` — one slot per server | `UserSessionContext` in `Application.Session`; the two-session corruption test | 4 |
| HKCU registry settings | per-user profile store under the storage root + browser `localStorage` for UI prefs | 4 |
| Load-all grid | server-side filter + `VirtualMode` block cache, `OrderValidator` + `ErrorProvider` | 5 |
| `OpenFileDialog` + `C:\Orders` | `Upload` control → storage root; every path classified | 6 |
| `PrintDocument` → printer | `InvoicePdfWriter` → `PdfViewer` / `Application.Download` | 1, 6 |
| Excel Interop + `C:\Orders\out.xlsx` | in-memory CSV (1) → managed .xlsx writer + report queue (6) | 1, 6 |
| "the login form set the user, every button trusted it" | `AuthService.Demand(permission)` on every server action; `DownloadGuard` for files; audit log | 7 (`SecurityReview.md`) |

## Modernized — after parity, in Module 7 only

| Change | Detail |
|---|---|
| Theme + mixin | `Bootstrap-4` / `Material-3` / `FluentDark-5` live via `Application.LoadTheme`; `Themes/orderdesk.mixin.theme` merged (button radius 14) — `ModernizationNotes.md` |
| Shell of the Orders screen | watermark search box, `ToolBar` tool buttons instead of four push buttons, toast instead of MessageBox — zero lines of `Domain/` changed |
| Responsive | `ClientProfiles.json` (Phone ≤600 · Tablet 601–1024 · Desktop ≥1025) + `ResponsiveLayout` (three layouts, event-driven and simulated) — `ResponsiveProfiles.md` |
| Security as a web app | server-side auth per action, salted hashes, download guard, AllowHtml policy with sanitizer, session lifecycle, audit log — `SecurityReview.md` |
| Operations | dashboard (KPIs, orders-by-status `Canvas` chart, live activity feed), `/health` endpoint, `AppLog`, readiness checklist as code, static-state audit — `ReadinessChecklist.md` |
| Deployment | IIS `web.config`, Dockerfile + compose, production settings notes — `DeploymentChecklist.md` |

## Risks that remain (migration debt)

1. **No database.** `InMemoryOrderRepository.Shared` stands in for SQL; the process restart empties it. Next: a
   repository over the real database with the same `IOrderRepository` contract (the services do not change).
2. **Demo accounts in code.** `AuthService` holds two salted hashes; production maps OpenID Connect / Windows
   authentication claims to the same `Manager` / `Clerk` roles and keeps `Demand()` as it is. No sign-in rate limit.
3. **Two export paths.** The capstone snapshot exports CSV (`CsvExport`); Module 6's .xlsx writer and the report
   queue were not carried into this console to keep it readable. Merge them for the release.
4. **Theme is process-wide.** `LoadTheme` restyles every session — right for a house style, not a per-user toggle.
5. **Responsive claims are narrow.** Three layouts, one screen, tested with simulate buttons and a resized browser;
   no phone device (`ResponsiveProfiles.md`).
6. **Session affinity untested behind a real load balancer**; the container/IIS assets are written, not executed here.
7. **Audit log is in memory** (`AuditLog`), mirrored to the file log; a production sink (table / SIEM) is one
   `Record` implementation away.

## The next slice

The **Edit Order dialog with a real repository**: `EditOrderDialog` (Module 3) over a database-backed
`IOrderRepository`, `OrderValidator` (Module 5) on save, `AuthService.Demand("orders.delete")` on delete, audited,
with the .xlsx export and report queue (Module 6) merged into the Orders toolbar. It is the first slice that changes
data users care about, and every guard it needs already exists.

## Where the evidence lives

`docs/MigrationAssessmentWorkbook.md` and `FirstSliceAcceptanceCriteria.md` (Module 1), the Module 2–6 deliverables
in their folders, and in this module `ModernizationNotes.md`, `ResponsiveProfiles.md`, `SecurityReview.md`,
`DeploymentChecklist.md`, `ReadinessChecklist.md`, plus the running log `migration-log.md`.
