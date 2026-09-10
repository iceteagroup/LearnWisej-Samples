# Report generation — server-side documents instead of printers and Office (lab deliverable 3)

Lab step covered: *convert report output to PDF or generated document download*. Both desktop report paths of
LegacyOrderDesk are replaced by documents built **on the server** from the **same** domain data and handed to the
browser — as a `PdfViewer` for viewing, or as `Application.Download` for saving. Neither path needs a printer, Office,
or a client file path. The lesson's shape, line for line:

```csharp
var rows = orderService.Search(currentFilter);
var path = reportService.CreateOrdersWorkbook(rows);
Application.Download(path, "Orders.xlsx");
```

`Reporting/ReportService.cs` is that `reportService`: it knows the storage root and the two managed writers, and
nothing about printers, Excel or the user's PC.

## Print Invoice → server PDF

| | LegacyOrderDesk | OrderDesk.Web |
|---|---|---|
| content | `InvoiceDocument.Build(order, service)` → text lines | **the same call** — the shared content rule is reused unchanged |
| rendering | `PrintDocument.PrintPage` + `Graphics.DrawString` (GDI+ on the user's PC) | `InvoicePdfWriter.Write(lines, title)` — PDF bytes, no GDI, no printer |
| preview | `PrintPreviewDialog.ShowDialog()` (a WinForms window) | `InvoicePreviewForm` — a modal `Form` with a `PdfViewer` (`PdfStream = new MemoryStream(pdf)`, `ViewerType = Auto`) and a ⬇ Download button; shown with `ShowDialog((form, result) => form.Dispose())` |
| output | the printer on the user's desk | `PdfViewer` in the browser, or `Application.Download(new MemoryStream(pdf), "Invoice-1042.pdf")` |

`Reporting/InvoicePdfWriter.cs` is a dependency-free writer (Courier text on Letter pages, PDF 1.4, one content stream
per page, xref table computed in one pass). Module 6 added `WritePages(pages, title)` — one PDF page per entry, used by
the 1,204-page invoice batch — and a linear-time byte layout; the Module 1 `Write(lines)` API is unchanged. In a real
project a server-safe PDF library replaces the class; what matters is that the PDF is produced on the server for any
session.

`Legacy/DesktopBoundaries.cs` quotes the desktop `InvoicePrinter.Print` as a comment because `System.Drawing.Printing`
does not compile for `net10.0` — the file could not be carried over even if one wanted to.

## Export to Excel → managed `.xlsx` + Download

| | LegacyOrderDesk | OrderDesk.Web |
|---|---|---|
| engine | `Excel.Application` via `Type.GetTypeFromProgID` + `Activator.CreateInstance` (Office Automation) | `XlsxWriter.OrdersWorkbook(orders)` — `System.IO.Compression.ZipArchive` + five XML parts |
| output path | `workbook.SaveAs(@"C:\Orders\out.xlsx")` — the user's disk | `StorageRoot.ExportPath($"Orders-{stamp}.xlsx")` — `App_Data/exports/Orders-20260910-143012-345.xlsx` on the server |
| delivery | "Exported to C:\Orders\out.xlsx" in a `MessageBox` | `Application.Download(path, "Orders.xlsx")` — the browser saves it |
| concurrency | one `EXCEL.EXE` per export, per user; a "Save As" prompt hangs the request for ever | any number of sessions at once; the millisecond timestamp in the name keeps them apart |

The five parts of the package (`Reporting/XlsxWriter.cs`), which is all a single-sheet workbook needs:

| Part | Content |
|---|---|
| `[Content_Types].xml` | default types for `rels` / `xml`; overrides for `/xl/workbook.xml` (sheet.main) and `/xl/worksheets/sheet1.xml` (worksheet) |
| `_rels/.rels` | `rId1` → `xl/workbook.xml` (officeDocument) |
| `xl/workbook.xml` | `<sheets><sheet name="Orders" sheetId="1" r:id="rId1"/></sheets>` |
| `xl/_rels/workbook.xml.rels` | `rId1` → `worksheets/sheet1.xml` |
| `xl/worksheets/sheet1.xml` | `<cols>` widths, header row, one `<row>` per order: text as inline strings (`t="inlineStr"`), `Order` and `Total` as numeric `<v>` cells (invariant culture) |

Columns: `Order, Customer, Owner, PO, Status, Total`. No shared-strings table, no styles part — Excel and LibreOffice
open the file as-is. In a real project EPPlus / Open XML SDK / Aspose.Cells / Syncfusion / DevExpress replace the class
one-for-one; the samples take no NuGet dependencies, which is why it is hand-written.

## Long reports → the queue

A report that takes seconds or minutes is not generated in the request: it is queued (`ReportQueue.md`), rendered by
the worker with the same writers, stored under `App_Data/reports/`, and then viewed (`PdfViewer`) or downloaded
(`Application.Download(path, name)`) from the queue card.

## Evidence (card C)

| Button | Path | What the console shows |
|---|---|---|
| `buttonPrintPdf` — *Print Invoice → PDF* | recovery | `⚠ boundary Print Invoice  PrintDocument → local printer  ⇒  server PDF (n bytes) → PdfViewer` · `→ .NET→JS InvoicePreviewForm  Invoice-1042.pdf  (PdfViewer.PdfStream, modal)`; the window `Invoice-1042.pdf  ·  generated on the server` opens with the invoice; ⬇ Download saves it; Close logs `• server InvoicePreviewForm  closed and disposed`; status `● invoice 1042 rendered on the server` |
| `buttonLegacyPrint` — *Legacy print* | failure (explained) | `⚠ boundary ✕ Print Invoice  PrintDocument + PrintPreviewDialog (System.Drawing.Printing) → verdict Redesign: InvoiceDocument lines → InvoicePdfWriter on the server → PdfViewer (InvoicePreviewForm) or Download`; red banner `✕ Legacy print: PrintDocument targets the printer attached to the SERVER (a web server has none) and PrintPreviewDialog is a WinForms window on the server's desktop that would block this request. System.Drawing.Printing does not even compile for net10.0 — the code is quoted in Legacy/DesktopBoundaries.cs. Verdict Redesign → …` |
| `buttonExportXlsx` — *Export .xlsx ⬇* | recovery | `⚠ boundary Export to Excel  Excel.Application + C:\Orders\out.xlsx  ⇒  XlsxWriter → App_Data\exports\Orders-<stamp>.xlsx (n bytes)` · `→ .NET→JS Application.Download  Orders.xlsx (5 rows) — no Excel.exe, no COM, no dialog, any number of sessions at once`; the browser saves `Orders.xlsx`; the file stays under `App_Data/exports/` |
| `buttonLegacyExcel` — *Legacy Excel Interop* | failure (explained) | see `UploadExportWorkflow.md` — the ProgID is probed, the COM object is never created |
