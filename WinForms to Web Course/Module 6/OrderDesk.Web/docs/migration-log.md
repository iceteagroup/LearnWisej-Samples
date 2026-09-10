# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One dated line per accepted workaround or decision. Carried forward module by module; Module 6 adds
the file / report / Office entries at the end.

- 2026-09-10 · M1 · Assessment workbook created; screens tagged; first slice = startup + login/user context + Orders read-only grid + EditOrderDialog + CSV export via Download; acceptance = parity first, modernization second, deployment third.
- 2026-09-10 · M1 · Business logic (OrderService, OrderCalculator, OrderValidator, OrderStore) reused as-is — shared `Domain/*.cs`.
- 2026-09-10 · M2 · New Wisej.NET shell (`OrderDesk.Web`, Wisej-4 4.1.0, net10.0-windows;net10.0); `Application.Run` → `Program.Main` + `Default.json startup`; `App.config` → `Web.config` read via `AppConfig` (System.Xml.Linq).
- 2026-09-10 · M2 · Compiler errors classified: namespace swap, renamed types (MenuStrip→MenuBar, StatusStrip→StatusBar, ToolStripMenuItem→MenuItem), designer-only properties removed, startup rewritten.
- 2026-09-10 · M3 · Navigation = left nav + MenuBar + StatusBar; `EditOrderDialog` → `Wisej.Web.Form` shown with `ShowDialog` inside `using`; `MessageBox.Show("Saved.")` → `Toast`; validation MessageBox stays modal.
- 2026-09-10 · M4 · `AppState` statics → `UserContext.Current` in `Application.Session`; `UserPreferences` (HKCU) → per-user server profile (`App_Data/users`); cleanup on `ApplicationExit` / `SessionTimeout`; two-session test passed.
- 2026-09-10 · M5 · Orders grid → VirtualMode + server-side `OrderQuery` (default filter Open, BlockSize paging); naive full bind measured vs optimized; validation → `OrderValidator` + `ErrorProvider` field messages.
- 2026-09-10 · M6 · C:\Orders → `App_Data` storage root from Web.config; Excel Interop → managed XlsxWriter + `Application.Download`; PrintDocument → server PDF in `PdfViewer`; long reports → `ReportQueue` worker.
- 2026-09-10 · M6 · Every file path classified (docs/file-boundary-classification.md): server storage / upload / download / ClientFileSystem / redesign; `DocumentStorage` is the only class that builds paths (`Path.Combine`, created on demand, forward-slash-safe, case-consistent names for Linux).
- 2026-09-10 · M6 · Batch import `File.ReadAllText(@"C:\Orders\in.csv")` → `Wisej.Web.Upload` (.csv, 1 MB max) staged under `App_Data/imports/<timestamp>-<name>.csv`; rows saved through `OrderService.Save` so `OrderValidator` rejects bad rows (unknown customer, missing owner) — the CSV contract stays `LocalExport.ToCsv`'s columns.
- 2026-09-10 · M6 · `Legacy/InvoicePrinter` kept as a stub that throws on the server (System.Drawing.Printing absent on net10.0); `PdfWriter.Invoice` renders the same content; `Invoice-<id>.pdf` shown via `PdfViewer.PdfStream`, downloadable, and stored under `App_Data/reports/`.
- 2026-09-10 · M6 · Report generation serialized by one process-wide worker (`ReportQueue`, `Application.StartTask`); status pushed to every open page with `Application.Update(page, …)`; jobs carry the queuing session; `App_Data/imports` + `App_Data/reports` git-ignored.
- 2026-09-10 · M6 · Office Interop removed from the migrated path; the Interop-shaped stub stays only behind `Export (Excel Interop) ✕` to demonstrate the `0x80040154` failure. Production choice deferred to M7: a licensed spreadsheet/PDF library replaces `XlsxWriter`/`PdfWriter` without touching the call sites.
