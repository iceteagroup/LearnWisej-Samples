# OrderDesk.Web · From WinForms to the Web · Module 6

Local lab build for **Module 6 · Files, Reports, and Browser Boundaries**. It follows the lesson and the walkthrough
video: every file, Office and printer touch of **LegacyOrderDesk** is classified (server storage · upload · download ·
ClientFileSystem · redesign), the import that read `C:\Orders\in.csv` becomes **Upload → configured storage root →
importer → the reused business logic**, *Export to Excel* becomes a managed **.xlsx + `Application.Download`**, *Print
Invoice* becomes a **server PDF in a `PdfViewer`**, and long reports go through a **process-wide report queue** with
progress, cancel, view and download that every browser session sees. The legacy paths are kept ✕ and run on purpose so
their failure on the server is visible and explained.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 6/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5606
```

Then open <http://localhost:5606>. (Visual Studio: open `OrderDesk.slnx`, F5.) Use a browser window at least
1400 × 760 — the trace and the queue card on the right are anchored and sized against the first browser size the
client reports.

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. No Office, no printer.

## What to try

There is no login in this module: the **first** session of the process acts as **kelly**, every later one as **sam**
(`Application.Session.User`, per session), so the queue's Owner column can show two users. A reload while the old
session is still alive counts as a later session. `<project>` below is
`D:\Projects\LearnWisej-Samples\From WinForms to the Web Course\Module 6\OrderDesk.Web`; trace lines are quoted without
their timestamp.

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()` · `• server session new browser session a4f9c2e1 · 1 session(s) in this process` · `• server Application.Session.User = kelly  (this session only)` · `⚠ boundary storage root C:\Orders\…  ⇒  <project>\App_Data  (OrderDesk.StorageRoot = "App_Data" · 0 file(s) · separator '\')` · `• server StorageRoot uploads/ exports/ reports/ under App_Data · OS Microsoft Windows 10.0.26200` · `• server OrderService.GetOrders 5 orders in the shared repository` · `• server Wisej.Web.Timer polling ReportQueue every 1 s (redraws only when a job changed)`. Card A lists 9 paths with detail `9 paths · Import orders: File.ReadAllLines(@"C:\Orders\in.csv")`; card B reads `storage root  App_Data\  (Web.config OrderDesk.StorageRoot = "App_Data")` / `uploads       App_Data\uploads\  ·  0 file(s)` / `last import   —  (download the sample CSV, then upload it)`; `comboOrders` shows `1042 · Northwind Traders · 4,820.00 · Open` first; queue counts `0 job(s) · 0 queued · 0 running · 0 done`. Status `● storage root ready · kelly · queue polling` |
| `buttonClassAll` / `buttonClassServer` / `buttonClassUpload` / `buttonClassDownload` / `buttonClassClient` / `buttonClassRedesign` (**All / Server storage / Upload / Download / ClientFileSystem / Redesign**) | success (deliverable 1) | The boundary grid filters: 9 · 3 · 2 · 1 · 1 · 2 rows; trace `← JS→.NET boundaries.filter class = Upload → 2 rows`; the detail line reads `2 of 9 · Upload · Import orders: …` and, on row selection (`gridBoundaries`), `→ <replacement>` and `Evidence: …` |
| `buttonSampleCsv` (**Download sample CSV ⬇**) | success | Trace `→ .NET→JS Application.Download wwwroot\samples\order-batch.csv → order-batch.csv (8 rows, layout Customer,PO,Sku,Qty,UnitPrice)`; the browser saves `order-batch.csv`; status `● sample CSV sent to the browser — upload it back` |
| `upload` (**Upload order-batch.csv…**) — the `Wisej.Web.Upload` control is a button: click it, the browser's file picker opens (filtered to `.csv`, one file, ≤ 1 MB), choose `<project>\wwwroot\samples\order-batch.csv` (or the copy you just downloaded) and confirm | success (deliverable 2) | Trace `← JS→.NET Upload.Uploaded order-batch.csv · 388 bytes · text/csv` (Chrome with Excel installed reports `application/vnd.ms-excel`) · `⚠ boundary Import orders File.ReadAllLines(C:\Orders\in.csv)  ⇒  Upload → App_Data\uploads\order-batch.csv` · `• server OrdersCsvImporter.Import layout Customer,PO,Sku,Qty,UnitPrice · 8 data row(s) → 5 order(s), 0 skipped` · five `• server OrderService.Save` lines: `order 1043 · Northwind Traders · PO NW-88240 · 2 line(s) · total 1,800.00`, `order 1044 · Fabrikam Inc · PO FB-2214 · 2 line(s) · total 1,200.00`, `order 1045 · Tailspin Toys · PO TT-0093 · 2 line(s) · total 5,652.50` (5 % discount applied by the reused rule), `order 1046 · Wide World Importers · PO WW-7710 · 1 line(s) · total 990.50`, `order 1047 · Litware Inc · PO LW-3301 · 1 line(s) · total 2,060.00` · `• server OrderService.GetOrders 10 orders in the shared repository`. Card B: `uploads  App_Data\uploads\  ·  1 file(s)` and `last import   order-batch.csv → 5 order(s) saved, 0 skipped · 1043, 1044, 1045, 1046, 1047`. Toast `5 order(s) imported from order-batch.csv — the file never had a C:\ path.` `comboOrders` now starts with `1047 · Litware Inc · 2,060.00 · Open`. Status `● 5 order(s) imported through Upload`. A second upload of the same file saves 1048–1052 |
| `upload` with a non-.csv file or a file > 1 MB | failure | Over the limit the control refuses it before a byte reaches the server: trace `← JS→.NET Upload.Error FileTooLarge · <name> · <message>`, red banner `✕ Upload refused (FileTooLarge): … MaxFileSize is 1 MB; the limit is enforced before a byte reaches the importer.`, status `● upload refused`. A renamed non-CSV that passes the browser filter is rejected by the server: red banner `✕ <name>: only .csv files are imported. The browser filter (AllowedFileTypes) is a convenience, the server decides.`, status `● upload rejected`. A CSV with an unknown header: amber banner `Nothing imported from <name>: header '…' is neither the line layout (…,Sku,Qty,UnitPrice) nor the export layout (…,Status,Total). Download the sample CSV to see the accepted layout.` |
| `buttonLegacyImport` (**Legacy import C:\Orders\in.csv**) | **failure** (desktop path on the server) | Trace `← JS→.NET legacy import File.ReadAllLines(@"C:\Orders\in.csv") — as the desktop app did` · `⚠ boundary ✕ legacy import DirectoryNotFoundException: Could not find a part of the path 'C:\Orders\in.csv'.` Red banner `✕ Legacy import failed with DirectoryNotFoundException: the path is on the user's PC; the server has no such disk. File.ReadAllLines runs where the code runs — on the server. Web replacement: Upload → App_Data\uploads\… → OrdersCsvImporter.` Status `● desktop path assumption broke on the server` (red). If `C:\Orders\in.csv` happens to exist on this PC: `⚠ boundary ✕ legacy import n line(s) read from C:\Orders\in.csv — on the SERVER's disk` and the amber banner `✕ C:\Orders\in.csv exists here only because the server IS your PC today. Deployed, the same call reads the server's C: drive: every user would import the same file, and nobody could put theirs there. Use the Upload control.`; on Linux the exception is `FileNotFoundException` |
| `comboOrders` | – | Chooses the order for *Print Invoice → PDF* (newest first) |
| `buttonPrintPdf` (**Print Invoice → PDF**) | recovery (deliverable 3) | Trace `⚠ boundary Print Invoice PrintDocument → local printer  ⇒  server PDF (n bytes) → PdfViewer` · `→ .NET→JS InvoicePreviewForm Invoice-1042.pdf  (PdfViewer.PdfStream, modal)`. A modal window `Invoice-1042.pdf  ·  generated on the server` shows the invoice in a `PdfViewer`; its `buttonDownload` (**⬇ Download**) saves `Invoice-1042.pdf`, its `buttonClose` (**Close**) logs `• server InvoicePreviewForm closed and disposed`. Status `● invoice 1042 rendered on the server` |
| `buttonLegacyPrint` (**Legacy print**) | **failure** (printer = the server's) | Trace `← JS→.NET legacy print InvoicePrinter.Print(order) — PrintDocument + PrintPreviewDialog` · `⚠ boundary ✕ Print Invoice PrintDocument + PrintPreviewDialog (System.Drawing.Printing) → verdict Redesign: InvoiceDocument lines → InvoicePdfWriter on the server → PdfViewer (InvoicePreviewForm) or Download`. Red banner `✕ Legacy print: PrintDocument targets the printer attached to the SERVER (a web server has none) and PrintPreviewDialog is a WinForms window on the server's desktop that would block this request. System.Drawing.Printing does not even compile for net10.0 — the code is quoted in Legacy/DesktopBoundaries.cs. Verdict Redesign → InvoiceDocument lines → InvoicePdfWriter on the server → PdfViewer (InvoicePreviewForm) or Download.` Status `● printer assumption explained — use Print Invoice → PDF` (red) |
| `buttonExportXlsx` (**Export .xlsx ⬇**) | recovery (deliverable 2) | Trace `⚠ boundary Export to Excel Excel.Application + C:\Orders\out.xlsx  ⇒  XlsxWriter → App_Data\exports\Orders-20260910-HHmmss-fff.xlsx (n bytes)` · `→ .NET→JS Application.Download Orders.xlsx (5 rows) — no Excel.exe, no COM, no dialog, any number of sessions at once` (10 rows after the import); the browser saves `Orders.xlsx` (one sheet "Orders", numeric Total); status `● Orders.xlsx generated on the server and streamed to the browser` |
| `buttonExportCsv` (**Export .csv ⬇**) | recovery (Module 1, round-trip) | Trace `→ .NET→JS Application.Download orders.csv (n bytes, layout Order,Customer,Owner,PO,Status,Total) — upload it back to test the second layout`; status `● orders.csv streamed to the browser`. Uploading that file imports `layout Order,Customer,Owner,PO,Status,Total · 5 data row(s) → 5 order(s), 0 skipped`, one `IMPORT` line per order |
| `buttonLegacyExcel` (**Legacy Excel Interop**) | **failure** (Office Automation) | Trace `← JS→.NET legacy Excel export probe Type.GetTypeFromProgID("Excel.Application") — the COM object is NOT created`, then one of: Office installed here → `⚠ boundary ✕ Excel Interop ProgID found: Office is installed on THIS machine — unsupported from a server process (KB 257757)`, amber banner `✕ Excel is installed on THIS machine, but automating it from a server process is unsupported (Microsoft KB 257757): one interactive Excel per export, no concurrency, hangs on dialogs, runs as the service account. The console did not create the COM object. Migrated path: Export .xlsx (XlsxWriter + Application.Download).`, status `● Office present, Office Automation still not a server strategy`; no Office → `⚠ boundary ✕ Excel Interop ProgID missing → ExcelExport.ExportOrders throws InvalidOperationException before writing a byte`, red banner `✕ ProgID Excel.Application not found → the desktop export throws InvalidOperationException ("Excel is not installed on this machine"). This is the real server case: a web server has no Office. Migrated path: …`, status `● desktop export cannot run here`; Linux → red banner `✕ Linux …: no COM, no registry, no Excel.Application — the desktop export cannot even ask for the ProgID. …`, status `● no Office Automation on this platform`. No `EXCEL.EXE` ever starts |
| `buttonQueueSummary` (**Queue Q2 summary**) | success (deliverable 4) | Trace `← JS→.NET queue report Q2 summary · ≈ 3 s — Done first, try View ▸ and Download ⬇` · `• server ReportQueue.Enqueue job 1 · owner kelly · result → App_Data\reports\0001-Q2-summary.pdf`; grid row `1 · Q2 summary · kelly · Running · n% · 0s elapsed`; within ~3 s `• server job 1 (kelly) Q2 summary → Done · Q2-summary.pdf`, row `Done · 100% · Q2-summary.pdf · view ▸`, counts `1 job(s) · 0 queued · 0 running · 1 done`; status `● job 1 queued · the page keeps responding while the worker renders` |
| `buttonQueueBatch` (**Queue invoice batch ×1,204**) | **progress** (≈ 36 s) | Trace `← JS→.NET queue report Invoice batch × 1,204 orders · one page per order, ~30 ms each ≈ 36 s` · `• server ReportQueue.Enqueue job 2 · owner kelly · result → App_Data\reports\0002-Invoice-batch-1204.pdf` · `• server job 2 (kelly) Invoice batch × 1,204 orders → Running`; the Progress column climbs (`12%`, `37%` …), Result shows `ns elapsed`, `progressJob` follows the selected row; the page stays responsive (press other buttons meanwhile); at the end `→ Done · Invoice-batch-1204.pdf` |
| `buttonQueueStatement` (**Queue monthly statement**) while the batch runs | success (serialised) | `• server ReportQueue.Enqueue job 3 · owner kelly · result → App_Data\reports\0003-Monthly-statement.pdf`; row `Queued · waiting · —` until the batch is Done or Cancelled, then `→ Running` for ≈ 15 s, then `→ Done · Monthly-statement.pdf`; counts `3 job(s) · 1 queued · 1 running · 1 done` |
| `buttonCancelJob` (**Cancel selected**) on the running batch | recovery (cancel) | Trace `← JS→.NET ReportQueue.Cancel job 2 (Invoice batch × 1,204 orders, Running) → cancellation requested`; next tick `• server job 2 (kelly) Invoice batch × 1,204 orders → Cancelled`; row `Cancelled · 37% · stopped · —`; the statement starts by itself. On a queued job the row turns `Cancelled` immediately. The button is disabled for Done / Failed / Cancelled rows (reaching the handler anyway shows the amber `Job n is already Done — only Queued or Running jobs can be cancelled.`) |
| `buttonViewResult` (**View result ▸**) on a Done row | success | Trace `→ .NET→JS InvoicePreviewForm Q2-summary.pdf (n bytes) from App_Data\reports\0001-Q2-summary.pdf`; the PDF opens in the modal `PdfViewer` (`buttonDownload` / `buttonClose` as above); closing logs `• server InvoicePreviewForm closed and disposed`. Disabled while the row is not Done (the handler would show `Job n has no result yet (Running).` / `Select a job first.`) |
| `buttonDownloadResult` (**Download result ⬇**) on a Done row | success | Trace `→ .NET→JS Application.Download App_Data\reports\0001-Q2-summary.pdf → Q2-summary.pdf`; the browser saves `Q2-summary.pdf` |
| `buttonSecondSession` (**Open second session ↗**) → in the new tab `buttonQueueStatement` → back in the first tab | success (multi-user) | First tab: `→ .NET→JS Application.Navigate same URL, target _blank → a second browser session sees the same queue`. New tab: `• server session new browser session 7e0d… · 2 session(s) in this process`, `• server Application.Session.User = sam  (this session only)`, and one `• server job n (kelly) … → <status>` line per existing job (the queue is shared); the same grid rows and counts. A job queued there shows `owner sam` — `• server ReportQueue.Enqueue job 4 · owner sam · …` — and appears in the first tab within a second with `• server job 4 (sam) Monthly statement → Running`. Cancel it from either tab |
| `buttonClear` (**Clear**) | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).

## Where things live

```
Module 6/
└─ OrderDesk.Web/                        the Wisej.NET 4 app (net10.0-windows;net10.0), port 5606
   ├─ Program.cs / Startup.cs            session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config   ← appSettings OrderDesk.StorageRoot = "App_Data"
   ├─ Domain/                            ✓ the reused business logic (OrderService.Save computes the imported totals)
   ├─ Files/StorageRoot.cs               ✓ the configured root: Path.Combine(Application.StartupPath, …), uploads/ exports/ reports/, SafeFileName
   ├─ Files/FileBoundaryClassifier.cs    deliverable 1 as data (9 rows, five classes)
   ├─ Files/OrdersCsvImporter.cs         ✓ Stream → orders (two layouts) → CustomerService.FindByName + OrderService.Save
   ├─ Reporting/ReportService.cs         ✓ CreateOrdersWorkbook(rows) → path · CreateInvoicePdf(order) → bytes
   ├─ Reporting/XlsxWriter.cs            ✓ dependency-free .xlsx (ZipArchive + five OpenXML parts)
   ├─ Reporting/InvoicePdfWriter.cs      ✓ dependency-free PDF (+ WritePages for batches)
   ├─ Reporting/CsvExport.cs             ✓ the Module 1 CSV export (round-trip layout)
   ├─ Reporting/ReportQueue.cs           ✓ process-wide queue: one lock, one Task.Run worker, progress, cancel, copies out
   ├─ Reporting/ReportBuilders.cs        ✓ the three jobs (invoice batch, monthly statement, Q2 summary)
   ├─ Legacy/DesktopBoundaries.cs        ✕ C:\Orders\in.csv read, Excel ProgID probe, quoted InvoicePrinter / attach code
   ├─ Legacy/ExcelExport.cs              ✕ the desktop Office Automation export — never called
   ├─ Views/InvoicePreviewForm.cs        PdfViewer + Download in a modal Form (caller disposes)
   ├─ Views/TracePanel.cs, Ui.cs         shared console helpers
   ├─ wwwroot/samples/order-batch.csv    the sample import file (8 rows → 5 orders), served by Download sample CSV
   ├─ MainPage.cs / .Designer.cs         the lab console (four cards + trace)
   ├─ App_Data/                          created at run time: uploads/, exports/Orders-<stamp>.xlsx, reports/<id>-<name>.pdf
   └─ docs/                              the lab deliverables + migration-log.md
```

## Deliverables

| # | Lab step / deliverable | Document | Code |
|---|---|---|---|
| 1 | File boundary classification (find the import/export and report workflows, classify every path) | [`OrderDesk.Web/docs/FileBoundaryClassification.md`](OrderDesk.Web/docs/FileBoundaryClassification.md) | [`Files/FileBoundaryClassifier.cs`](OrderDesk.Web/Files/FileBoundaryClassifier.cs) (card A) |
| 2 | Upload + export workflow (replace local file assumptions; remove Office Automation from the migrated path) | [`OrderDesk.Web/docs/UploadExportWorkflow.md`](OrderDesk.Web/docs/UploadExportWorkflow.md) | [`Files/StorageRoot.cs`](OrderDesk.Web/Files/StorageRoot.cs), [`Files/OrdersCsvImporter.cs`](OrderDesk.Web/Files/OrdersCsvImporter.cs), [`Reporting/XlsxWriter.cs`](OrderDesk.Web/Reporting/XlsxWriter.cs), [`Legacy/ExcelExport.cs`](OrderDesk.Web/Legacy/ExcelExport.cs) ✕ |
| 3 | Report output → PDF / generated document download | [`OrderDesk.Web/docs/ReportGeneration.md`](OrderDesk.Web/docs/ReportGeneration.md) | [`Reporting/ReportService.cs`](OrderDesk.Web/Reporting/ReportService.cs), [`Reporting/InvoicePdfWriter.cs`](OrderDesk.Web/Reporting/InvoicePdfWriter.cs), [`Views/InvoicePreviewForm.cs`](OrderDesk.Web/Views/InvoicePreviewForm.cs) |
| 4 | Queued report job (progress, status, cancel, download) | [`OrderDesk.Web/docs/ReportQueue.md`](OrderDesk.Web/docs/ReportQueue.md) | [`Reporting/ReportQueue.cs`](OrderDesk.Web/Reporting/ReportQueue.cs), [`Reporting/ReportBuilders.cs`](OrderDesk.Web/Reporting/ReportBuilders.cs) (card D) |
| 5 | Test on a clean server-like environment | [`OrderDesk.Web/docs/ServerLikeTest.md`](OrderDesk.Web/docs/ServerLikeTest.md) | `Legacy/DesktopBoundaries.cs` ✕ (the probes the console runs) |
| 6 | Migration log | [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) | the trace panel is the live version |

## Self-check answers (lab guide + storyboard)

- **Which business logic was reused as-is?** `Domain/` unchanged: the importer resolves customers with
  `CustomerService.FindByName` and saves through `OrderService.Save`, which computes the totals (Tailspin Toys gets its
  5 % discount: 5,950.00 → 5,652.50); the invoice PDF and every queued report start from `InvoiceDocument.Build` and
  `OrderService.CalculateOrderTotal`. Only the *transport* of files changed, never a rule.
- **Which desktop boundary was replaced with a web-safe pattern?** Three, one per card: `File.ReadAllLines(@"C:\Orders\in.csv")`
  → `Wisej.Web.Upload` + `StorageRoot` (the server chooses the path, the browser never names one); `Excel.Application`
  + `C:\Orders\out.xlsx` → `XlsxWriter` + `Application.Download(path, "Orders.xlsx")`; `PrintDocument` + `PrintPreviewDialog`
  → `InvoicePdfWriter` + `PdfViewer` / Download. The fourth, the frozen for-each print loop, became the report queue.
- **How is per-user state kept out of static fields?** The only static in the module is the `ReportQueue` — shared by
  design, like a table, and locked. What is per user (`Application.Session.User`, the job `Owner`, the session's
  trace) lives in the session; the worker thread has no session and never reads one. Storage folders are
  configuration, not user state.
- **What was tested before calling the migrated feature complete?** Upload of `order-batch.csv` (5 orders, right
  totals), the legacy import failing with `DirectoryNotFoundException`, `Orders.xlsx` opening in a spreadsheet, the
  PDF opening in the viewer and downloading, the three queue jobs (progress, cancel, view, download), a second session
  seeing and queuing on the same queue — and the environment checklist in `ServerLikeTest.md` (no Office, non-admin
  service account, Linux paths). Both target frameworks build clean.
- **Pause & predict — What breaks if two users export at the same time through Office Interop?** Each export starts
  its own `EXCEL.EXE` under the service account (no interactive desktop): the second instance may fail to register,
  both may deadlock on a modal prompt nobody can see, and both write the **same** `C:\Orders\out.xlsx` on the server
  — the second `SaveAs` collides with or overwrites the first. Microsoft does not support it (KB 257757). The managed
  writer runs in-process for any number of sessions and stamps each file with a millisecond timestamp.
- **Pause & predict — Which path code is business logic, and which assumed a Windows disk?** Business logic: parsing
  rows into `Order` + `OrderLine`, resolving customers, computing totals, building invoice lines and statements — all
  platform-neutral and reused. Windows-disk assumptions: every `C:\Orders\…` literal, `%APPDATA%`, `HKCU`, backslash
  string concatenation, `Process.Start(path)`, `Type.GetTypeFromProgID`, `System.Drawing.Printing`. Those became a
  configured root + `Path.Combine`, Upload/Download, or a redesign — and they are exactly what would break on a
  case-sensitive Linux disk with `/` separators.

## Runtime facts

- **Upload**: `Wisej.Web.Upload` is a button that opens the browser's picker; `AllowedFileTypes = ".csv"` and
  `MaxFileSize = 1048576` filter on the client (`Error` fires with `FileTooLarge`), the server validates again. `Uploaded`
  delivers `UploadedEventArgs.Files` — for each `e.Files[i]`: `FileName`, `ContentLength`, `ContentType`,
  `InputStream` (copy it to a server path you chose; the browser's path is never known).
- **PdfViewer**: `PdfStream = new MemoryStream(bytes)`, `ViewerType = PdfViewerType.Auto`, inside a `Form` shown with
  `ShowDialog((form, result) => form.Dispose())` — modal dialogs do not block, and the caller disposes.
- **Download**: `Application.Download(path, name)` streams a file from the server disk (the export, a queue result);
  `Application.Download(stream, name)` streams bytes in memory (the CSV, the invoice from the viewer). The browser saves
  it; the server never writes to a user path.
- **The worker never touches controls.** `ReportQueue`'s worker runs in `Task.Run`, outside any Wisej session: no
  `Application.*`, no `Application.Update`. Pages poll it with a `Wisej.Web.Timer` and redraw only when
  `ReportQueue.Signature()` changes. `Application.StartTask` is for work owned by one session; a process-wide queue is
  not.
- **Paths**: `Path.Combine(Application.StartupPath, "App_Data", …)` everywhere; `Application.StartupPath` is the
  project folder under `dotnet run`. `Web.config` `OrderDesk.StorageRoot` may be relative (under the project) or
  absolute (`Path.IsPathRooted` decides). Browser-supplied names go through `StorageRoot.SafeFileName` (leaf name only,
  `..` rejected).
- **Linux paths are case-sensitive** and use `/`: `uploads` ≠ `Uploads`; `@"C:\Orders\in.csv"` is a relative file name
  there (backslashes are ordinary characters), so the legacy import throws `FileNotFoundException` instead of
  `DirectoryNotFoundException`. `Registry`, `Type.GetTypeFromProgID` and `System.Drawing.Printing` are Windows-only —
  the last does not compile for `net10.0`.
- `Application.Navigate(Application.Url, "_blank")` opens a second session in the same process; `Application.Session`
  is a dynamic bag (`session.User`), one per browser session.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
  Delete `App_Data/` to reset uploads, exports and report results (the in-memory orders reset on restart anyway).
