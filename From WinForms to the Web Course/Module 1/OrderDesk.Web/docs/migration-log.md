# migration-log.md — LegacyOrderDesk → OrderDesk.Web

One line per accepted decision or workaround. Later modules append to this file.

## Module 1 · Migration discovery (2026-09-09)

- **Assessment first.** 14 items inventoried and classified (4 direct-port · 5 adapt · 3 redesign · 1 defer · 1 remove) before any namespace was replaced. See `MigrationAssessmentWorkbook.md`.
- **Startup → `Default.json` + `Program.Main`.** `Application.EnableVisualStyles` / `Application.Run` have no equivalent; `Program.Main(NameValueCollection)` runs once per browser session and sets `Application.MainPage`. Kestrel (`Startup.cs`, `app.UseWisej()`) owns the process.
- **Business logic copied verbatim.** `Domain/` (Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData) has no UI or desktop dependency and moved unchanged. Totals are identical on both sides.
- **Static current user is a defect, not a style issue.** `AppState.CurrentUser` is one slot per server process; a second session overwrites it. Kept in `Legacy/` next to `Application.Session.User` so the leak is reproducible; the typed session context is Module 4's job.
- **Print Invoice → server PDF.** `PrintDocument` targets a printer attached to the server. The shared `InvoiceDocument` lines are written by a dependency-free `InvoicePdfWriter` and shown in a `PdfViewer` (+ Download). No printer, no local path.
- **Export to Excel → bytes + `Application.Download`.** Excel Interop is not supported in a server process and `C:\Orders\out.xlsx` is the user's disk. First slice ships CSV built in memory; Module 6 upgrades to a managed .xlsx writer and a report queue.
- **Attach file deferred to Module 6.** `OpenFileDialog` + `C:\Orders\Attachments` cannot exist on the server, so the first slice leaves it out instead of faking it. Replacement: `Upload` control + configured storage root.
- **Connection string → `Web.config`.** `App.config` is gone; settings live in the web host's configuration (`<appSettings>` now; a `<connectionStrings>` section is added and read in Module 2).
- **Window-size restore removed.** The browser window belongs to the user; responsive layout (Module 7) replaces it.
- **Installer / ClickOnce deferred.** The web app is deployed once; checklist in Module 7.
- **Modal dialogs do not block in Wisej.NET.** `ShowDialog(callback)` / `ShowDialogAsync()`; the caller disposes the dialog when it closes (Module 3 rule, already applied to the invoice preview).
