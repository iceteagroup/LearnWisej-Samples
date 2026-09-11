# OrderDesk.Web · From WinForms to the Web · Module 6

Local lab build for **Module 6 · Files, Reports, and Browser Boundaries**. The import that read `C:\Orders\in.csv`
becomes **Upload → configured storage root → importer → the reused business logic**, *Export to Excel* becomes a managed
**.xlsx + `Application.Download`**, *Print Invoice* becomes a **server PDF in a `PdfViewer`**, and long reports go
through a **process-wide report queue** (progress, cancel, view, download) that every browser session sees.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 6/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5606
```

Then open <http://localhost:5606>. (Visual Studio: open `OrderDesk.slnx`, F5.) No Office, no printer needed.

There is no login in this module: the **first** session of the process acts as **kelly**, every later one as **sam**
(`Application.Session.User`), so the queue's Owner column can show two users.

## What to try

| Action | What you should see |
|---|---|
| **Upload order-batch.csv…** → pick `OrderDesk.Web/wwwroot/samples/order-batch.csv` | `order-batch.csv → 5 order(s) saved, 0 skipped`, a toast, and orders 1043–1047 at the top of the invoice list (Tailspin Toys 5,652.50 after its 5 % discount). The file is stored under `App_Data/uploads/` |
| Upload a non-.csv or a file over 1 MB | a warning toast; nothing is imported |
| Pick an order → **Print Invoice (PDF)** | the invoice PDF, built on the server, opens in a modal `PdfViewer` with a Download button |
| **Export .xlsx ⬇** | the browser saves `Orders.xlsx` (sheet "Orders"), written without Office under `App_Data/exports/` |
| **Queue Q2 summary** | a job appears (`Running`, then `Done` after ~3 s); **View result ▸** opens it, **Download result ⬇** saves it |
| **Queue invoice batch ×1,204**, then **Queue monthly statement** | the batch runs (~36 s, progress climbing) while the statement waits; the page stays responsive. **Cancel selected** stops the batch and the statement starts |
| **Open second session ↗** → queue a job there | the new tab (owner sam) sees the same queue; the job appears in both tabs within a second |

## Where things live

```
Module 6/
└─ OrderDesk.Web/                        the Wisej.NET 4 app (net10.0-windows;net10.0), port 5606
   ├─ Program.cs / Startup.cs            session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config   ← appSettings OrderDesk.StorageRoot = "App_Data"
   ├─ Domain/                            the reused business logic (OrderService.Save computes the imported totals)
   ├─ Files/StorageRoot.cs               the configured root: Path.Combine(Application.StartupPath, …), uploads/ exports/ reports/, SafeFileName
   ├─ Files/OrdersCsvImporter.cs         Stream → orders (two layouts) → CustomerService.FindByName + OrderService.Save
   ├─ Reporting/ReportService.cs         CreateOrdersWorkbook(rows) → path · CreateInvoicePdf(order) → bytes
   ├─ Reporting/XlsxWriter.cs            dependency-free .xlsx (ZipArchive + five OpenXML parts)
   ├─ Reporting/InvoicePdfWriter.cs      dependency-free PDF (+ WritePages for batches)
   ├─ Reporting/ReportQueue.cs           process-wide queue: one lock, one Task.Run worker, progress, cancel
   ├─ Reporting/ReportBuilders.cs        the three jobs (invoice batch, monthly statement, Q2 summary)
   ├─ Views/InvoicePreviewForm.cs        PdfViewer + Download in a modal Form (caller disposes)
   ├─ Views/Ui.cs                        colours + toast helper
   ├─ wwwroot/samples/order-batch.csv    the sample import file (8 rows → 5 orders)
   ├─ MainPage.cs / .Designer.cs         import, invoice, export and the report queue
   ├─ App_Data/                          created at run time: uploads/, exports/Orders-<stamp>.xlsx, reports/<id>-<name>.pdf
   └─ docs/                              the lab deliverables + migration-log.md
```

## Deliverables

| # | Lab step / deliverable | Document | Code |
|---|---|---|---|
| 1 | File boundary classification | [`OrderDesk.Web/docs/FileBoundaryClassification.md`](OrderDesk.Web/docs/FileBoundaryClassification.md) | — |
| 2 | Upload + export workflow, no Office Automation | [`OrderDesk.Web/docs/UploadExportWorkflow.md`](OrderDesk.Web/docs/UploadExportWorkflow.md) | [`Files/StorageRoot.cs`](OrderDesk.Web/Files/StorageRoot.cs), [`Files/OrdersCsvImporter.cs`](OrderDesk.Web/Files/OrdersCsvImporter.cs), [`Reporting/XlsxWriter.cs`](OrderDesk.Web/Reporting/XlsxWriter.cs) |
| 3 | Report output → PDF / generated document download | [`OrderDesk.Web/docs/ReportGeneration.md`](OrderDesk.Web/docs/ReportGeneration.md) | [`Reporting/ReportService.cs`](OrderDesk.Web/Reporting/ReportService.cs), [`Reporting/InvoicePdfWriter.cs`](OrderDesk.Web/Reporting/InvoicePdfWriter.cs), [`Views/InvoicePreviewForm.cs`](OrderDesk.Web/Views/InvoicePreviewForm.cs) |
| 4 | Queued report job (progress, status, cancel, download) | [`OrderDesk.Web/docs/ReportQueue.md`](OrderDesk.Web/docs/ReportQueue.md) | [`Reporting/ReportQueue.cs`](OrderDesk.Web/Reporting/ReportQueue.cs), [`Reporting/ReportBuilders.cs`](OrderDesk.Web/Reporting/ReportBuilders.cs) |
| 5 | Test on a clean server-like environment | [`OrderDesk.Web/docs/ServerLikeTest.md`](OrderDesk.Web/docs/ServerLikeTest.md) | — |
| 6 | Migration log | [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) | — |

## Self-check answers (lab guide + video)

- **Which business logic was reused as-is?** `Domain/` unchanged: the importer resolves customers with
  `CustomerService.FindByName` and saves through `OrderService.Save`, which computes the totals (Tailspin Toys gets its
  5 % discount: 5,950.00 → 5,652.50); the invoice PDF and every queued report start from `InvoiceDocument.Build` and
  `OrderService.CalculateOrderTotal`. Only the *transport* of files changed, never a rule.
- **Which desktop boundary was replaced with a web-safe pattern?** `File.ReadAllLines(@"C:\Orders\in.csv")` →
  `Wisej.Web.Upload` + `StorageRoot`; `Excel.Application` + `C:\Orders\out.xlsx` → `XlsxWriter` +
  `Application.Download(path, "Orders.xlsx")`; `PrintDocument` + `PrintPreviewDialog` → `InvoicePdfWriter` + `PdfViewer`.
  The frozen for-each print loop became the report queue.
- **How is per-user state kept out of static fields?** The only static is the `ReportQueue` — shared by design, like a
  table, and locked. What is per user (`Application.Session.User`, the job `Owner`) lives in the session; the worker
  thread has no session and never reads one.
- **What was tested before calling the migrated feature complete?** Upload of `order-batch.csv` (5 orders, right
  totals), `Orders.xlsx` opening in a spreadsheet, the PDF opening in the viewer and downloading, the three queue jobs
  (progress, cancel, view, download), a second session seeing the same queue — and the checklist in `ServerLikeTest.md`.
- **What breaks if two users export at the same time through Office Interop?** Each export starts its own `EXCEL.EXE`
  under the service account: instances fail to register or deadlock on a prompt nobody sees, and both write the
  **same** `C:\Orders\out.xlsx`. The managed writer runs in-process for any number of sessions.
- **Which path code is business logic, and which assumed a Windows disk?** Parsing rows, resolving customers, computing
  totals and building invoice lines are platform-neutral and reused. Every `C:\Orders\…` literal, `%APPDATA%`, `HKCU`,
  backslash concatenation, `Process.Start(path)`, `Type.GetTypeFromProgID` and `System.Drawing.Printing` assumed a
  Windows desktop; they became a configured root + `Path.Combine`, Upload/Download, or a redesign.

## Runtime facts

- **Upload**: `Wisej.Web.Upload` is a button that opens the browser's picker; `AllowedFileTypes = ".csv"` and
  `MaxFileSize = 1048576` filter on the client (`Error` fires with `FileTooLarge`), the server validates again.
  `Uploaded` delivers `e.Files[i]` with `FileName`, `ContentLength`, `ContentType`, `InputStream`.
- **PdfViewer**: `PdfStream = new MemoryStream(bytes)` inside a `Form` shown with `ShowDialog((form, result) => form.Dispose())`.
- **Download**: `Application.Download(path, name)` streams a file from the server disk; `Application.Download(stream, name)` streams bytes in memory.
- **The queue worker never touches controls.** It runs in `Task.Run`, outside any Wisej session; pages poll it with a
  `Wisej.Web.Timer` and redraw only when `ReportQueue.Signature()` changes.
- **Paths**: `Path.Combine(Application.StartupPath, "App_Data", …)` everywhere; `Web.config` `OrderDesk.StorageRoot` may be relative or absolute.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
  Delete `App_Data/` to reset uploads, exports and report results.
