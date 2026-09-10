# Migration log — LegacyOrderDesk → OrderDesk.Web

The running log the course videos keep opening: one dated line per accepted workaround or decision, carried forward
from module to module. The console's TracePanel is the live version of this file.

## Module 1 · Migration discovery & the first slice (2026-09-08)

* **Startup** — `Application.EnableVisualStyles()` + `Application.Run(new OrdersForm())` → `Default.json`
  (`"startup": "OrderDesk.Program.Main, OrderDesk"`) → `Program.Main` sets `Application.MainPage`; Kestrel owns the process.
* **Current user** — `static AppState.CurrentUser` is one slot for the whole server → `Application.Session.User`
  (typed context in Module 4). The static stays in `Legacy/` marked ✕ to demonstrate the leak.
* **Print Invoice** — `PrintDocument` → local printer → server-generated PDF (`InvoicePdfWriter`) in a `PdfViewer`.
* **Export** — Excel Interop + `C:\Orders\out.xlsx` → in-memory CSV (`CsvExport`) + `Application.Download`; a real
  spreadsheet writer deferred to Module 6.
* **Attach file** — `OpenFileDialog` + `C:\Orders\Attachments` → logged as a boundary, not faked; Upload control +
  storage root deferred to Module 6.
* **Connection string** — `App.config` → `Web.config` `<appSettings>` read with `System.Xml.Linq`
  (no `System.Configuration` reference).

## Module 6 · Files, reports and browser boundaries (2026-09-10)

* **Classified every path** (`Files/FileBoundaryClassifier.cs`, `docs/FileBoundaryClassification.md`): 9 touches —
  Server storage 3 · Upload 2 · Download 1 · ClientFileSystem 1 · Redesign 2. Nothing is left as "works on my PC".
* **Storage root** — every `C:\Orders\…` literal → `Web.config` `OrderDesk.StorageRoot` (default `App_Data`) resolved
  with `Path.Combine(Application.StartupPath, …)`; `uploads/ exports/ reports/` created on demand; browser file names
  reduced to a sanitized leaf name (`StorageRoot.SafeFileName`: `Path.GetFileName`, reject `..`, replace invalid chars).
* **Import** — `File.ReadAllLines(@"C:\Orders\in.csv")` → `Wisej.Web.Upload` (`.csv`, ≤ 1 MB) → `uploads/` →
  `OrdersCsvImporter` → `CustomerService.FindByName` + `OrderService.Save` (business logic reused unchanged). Two CSV
  layouts accepted (line layout; the Module 1 `CsvExport` layout for round-trips). Bad rows are skipped with a reason.
  The legacy read is kept in `Legacy/DesktopBoundaries.LocalFileImport` ✕ and executed on purpose by the console to show
  `DirectoryNotFoundException` (Windows) / `FileNotFoundException` (Linux).
* **Export** — CSV upgraded to a real `.xlsx` written by `Reporting/XlsxWriter` (ZipArchive + five OpenXML parts,
  inline strings + numeric cells) into `exports/`, then `Application.Download(path, "Orders.xlsx")` — the lesson's
  `reportService.CreateOrdersWorkbook(rows)` shape. No NuGet package because the samples take none; in a real project
  EPPlus / Open XML SDK / Aspose / Syncfusion / DevExpress replace the class one-for-one.
* **Office Automation removed from the migrated path** — `Legacy/ExcelExport.cs` stays ✕ and is never called. The
  console probes `Type.GetTypeFromProgID("Excel.Application")` only (never `Activator.CreateInstance`) and explains
  KB 257757 whether or not Office is installed; on Linux it does not even ask (`OperatingSystem.IsWindows()`).
* **Print** — `InvoicePrinter` cannot compile for `net10.0` (System.Drawing.Printing) → quoted as an excerpt in
  `Legacy/DesktopBoundaries.cs`; the web path is `ReportService.CreateInvoicePdf` → `InvoicePreviewForm` (PdfViewer,
  disposed by the caller in the `ShowDialog` callback) with a Download button.
* **Report queue** — `Reporting/ReportQueue` (static, one lock, one `Task.Run` worker, copies out, cancel token in) with
  states Queued / Running / Done / Failed / Cancelled and progress 0–100; results written to `reports/<id>-<name>.pdf`;
  pages poll with a 1 s `Wisej.Web.Timer` and redraw only when `Signature()` changes. Owner is the session's user, so a
  second session shows the same jobs with different owners. Production additions listed in `docs/ReportQueue.md`.
* **Long PDFs** — `InvoicePdfWriter` gained `WritePages` (explicit page breaks) and a linear-time xref layout; the
  Module 1 `Write(lines)` API is unchanged.
* **Per-session user without a login screen** — the first session of the process acts as *kelly*, later ones as *sam*
  (`Application.Session.User`), only so the queue's Owner column can show two users; Module 4's typed session context
  is the real answer.
* **Server-like test plan** — `docs/ServerLikeTest.md`: no-Office pass, non-admin service account, Linux container;
  both target frameworks build with 0 warnings / 0 errors on this machine; the Linux run was not executed here.
