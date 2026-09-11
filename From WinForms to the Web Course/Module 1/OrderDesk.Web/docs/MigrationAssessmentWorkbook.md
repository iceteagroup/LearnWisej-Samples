# Deliverable 1 + 2 · Migration assessment workbook — LegacyOrderDesk

The inventory of LegacyOrderDesk 3.2 (one WinForms executable, one user per process) and the verdict for every
item.

## Inventory

| Area | Item | Depends on today |
|---|---|---|
| Forms & controls | `LoginForm`, `OrdersForm` (grid + detail + 4 actions + MenuStrip + StatusStrip), `EditOrderDialog` (modal), `SettingsForm` (modal), customer lookup (the customer `ComboBox` in `EditOrderDialog` + `CustomerService`) | standard WinForms controls, `DataGridView`, `MenuStrip`, `GroupBox` |
| Business logic | `OrderService` (totals, discount, tax, search, save), `CustomerService`, `InvoiceDocument` | plain C#, no UI |
| Data access | `InMemoryOrderRepository` (stands in for the SQL database; the real system reads a connection string from `App.config`) | plain C# (lab) / `App.config` (real system) |
| State | `AppState.CurrentUser / CurrentCompany / CurrentCustomer / CurrentFilter / LastSearch` (static), `AppState.Countries` (static readonly) | process-per-user |
| Desktop boundary | Print Invoice (`PrintDocument`, `PrintPreviewDialog`), Export to Excel (late-bound `Excel.Application`, `C:\Orders\out.xlsx`), Attach file (`OpenFileDialog`, `C:\Orders\Attachments`), Settings (`HKCU\Software\LegacyOrderDesk`), window size restore | local printer, Office, user's disk, user's registry |
| Deployment | `Program.Main` (`EnableVisualStyles`, `Application.Run`), per-PC installer / ClickOnce | one .exe per user |

## Workbook

Verdicts: **Direct-port** (standard controls, ordinary handlers) · **Port-with-adaptation** (compatible but carries a
desktop assumption) · **Redesign** (depends on a desktop-only component) · **Defer** (rare/admin path, not needed for
the first web release) · **Remove** (no longer meaningful in a browser). Effort S/M/L. ✓ = in the first slice.

| Form / feature | Dependency | Risk tag | Verdict | Effort | Slice | Why | Replacement |
|---|---|---|---|---|---|---|---|
| Startup (`Program.Main`) | `Application.Run` / `EnableVisualStyles` | startup | Adapt | S | ✓ | One .exe per user becomes one server for many sessions; `Application.Run` does not exist. | `Default.json` startup → `Program.Main` sets `Application.MainPage` (Module 2). |
| Login → current user | static `AppState.CurrentUser` | static-state | Adapt | M | ✓ | A static field is one slot for the whole server: the second user overwrites the first. | `Application.Session` behind a typed `UserContext` (Module 4). |
| Orders screen (grid + detail) | `DataGridView` · standard controls | grid-volume | Adapt | M | ✓ | Ports cleanly, but the desktop habit loads every row (200k in production). | Same screen; server-side filter + virtual rows (Module 5). |
| Edit Order dialog | modal `Form` · `DialogResult` | direct-port | Direct-port | S |  | Ordinary handlers and a modal result — the event-driven model comes across. | None needed; add disposal (`using` / Dispose in the callback) and a Toast instead of MessageBox (Module 3). |
| Customer lookup | standard controls | direct-port | Direct-port | S |  | Standard controls, simple binding, no local dependency. | None needed. |
| `OrderService` (totals, discounts, search) | none (plain C#) | direct-port | Direct-port | S | ✓ | No UI or desktop assumption — moves unchanged. | None needed; copy the file. |
| Print Invoice | `PrintDocument` → local printer | report | Redesign | M | ✓ | The server has no printer attached to the user's desk. | Server-generated PDF shown in `PdfViewer` or downloaded (Module 6). |
| Export to Excel | Excel Interop · `C:\Orders\out.xlsx` | office | Redesign | M | ✓ | Office Automation is not supported server-side; the path is on the user's PC. | Managed spreadsheet writer + `Application.Download` (Module 6). |
| Attach file | `OpenFileDialog` · `C:\Orders\Attachments` | file-system | Redesign | M |  | `File.Open` no longer means the user's disk. | `Upload` control → server storage root (Module 6). |
| Settings (grid density, export folder) | HKCU registry | registry | Adapt | S |  | HKCU on the server is the service account's registry, shared by everyone. | Per-user profile store on the server + browser storage for UI prefs (Module 4). |
| Connection string | `App.config` (real system; the lab store is in-memory) | startup | Adapt | S |  | `App.config` is gone; configuration belongs to the web host. | `Web.config` / appsettings (Module 2). |
| Menu bar (File · View · Reports · Help) | `MenuStrip` | direct-port | Direct-port | S |  | `Wisej.Web.MenuBar` maps directly. | None needed (navigation reviewed in Module 3). |
| Window size restore | registry + `Form.Size` | registry | Remove | S |  | The browser window belongs to the user; the server should not resize it. | Remove; responsive layout instead (Module 7). |
| Installer / ClickOnce | per-PC install | startup | Defer | L |  | Not needed to prove the migration; the web app is deployed once. | IIS / Linux / container deployment checklist (Module 7). |

Totals: 14 items · 4 direct-port · 5 adapt · 3 redesign · 1 defer · 1 remove · 6 in the first slice.

## Ranking by effort and risk

1. **Static current user** (M, static-state) — invisible in single-user testing, corrupts data in production. First thing the slice must expose.
2. **Orders grid volume** (M, grid-volume) — works in the lab, fails at production row counts.
3. **Print Invoice / Export to Excel** (M, report / office) — the two boundaries the users see every day; both need a server-side document path.
4. **Attach file** (M, file-system) — needs an upload workflow and a storage root; deferred out of the slice on purpose.
5. **Startup, connection string, settings** (S) — mechanical, done in Modules 2 and 4.
6. **Window size restore** (remove), **installer** (defer).
