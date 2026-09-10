# Office Automation replacement — Excel Interop → managed spreadsheet writer (Module 6)

> Lab step 5: *remove direct Office Automation from the migrated path*. Lesson fact: Microsoft does not
> recommend or support unattended, non-interactive server-side Office Automation; it can hang or behave
> unstably. Video: "Office Interop is not a server strategy — replace it with managed libraries."

## Why it fails on a server (and why it *looks* like it might work)

`Legacy/ExcelExport.cs` keeps the desktop code shape:

```csharp
xl = new Excel.Application();          // ✕ COM server, needs an interactive user session
var wb = xl.Workbooks.Add();
ws.Cells[row, 1] = "Order"; …
wb.SaveAs(@"C:\Orders\out.xlsx");      // ✕ local path again
```

- `Excel.Application` is an out-of-process COM server. `CoCreateInstance` needs Excel installed *and* an
  interactive desktop / user profile. From IIS or Kestrel under a service account it fails with
  `0x80040154 REGDB_E_CLASSNOTREG` (nothing registered), or — when someone installs Office on the web
  server "to make it work" — it starts, then blocks on a hidden dialog, leaks `EXCEL.EXE` processes, and
  is unsupported by Microsoft (KB 257757).
- Even when it runs, Office is single-instance-minded: two sessions exporting at the same time share one
  Excel automation server — one user's workbook can pick up the other's sheet, or both hang. That is the
  video's *Pause & predict* question.
- The course package ships no Interop reference, so `Legacy/ExcelExport.cs` carries a stand-in with the
  same call shape that throws the real Interop `COMException` unless `ORDERDESK_HAS_EXCEL=1` in an
  interactive session. The lab copy also refuses to write outside the project folder.

## The replacement — `Services/XlsxWriter.cs`

A minimal **OpenXML SpreadsheetML** writer on `System.IO.Compression.ZipArchive` (framework only):

| Part | Content |
|---|---|
| `[Content_Types].xml` | the content types of the package (workbook, worksheet, styles, rels) |
| `_rels/.rels` | root relationship → `xl/workbook.xml` |
| `xl/workbook.xml` | one `<sheet name="Orders" sheetId="1" r:id="rId1"/>` |
| `xl/_rels/workbook.xml.rels` | worksheet + styles relationships |
| `xl/styles.xml` | 3 cell formats: default, **bold** header, `#,##0.00` (built-in numFmtId 4) for totals |
| `xl/worksheets/sheet1.xml` | `<dimension>`, a frozen header pane, column widths, `<sheetData>` with **inline strings** (`t="inlineStr"`) and plain numeric `<v>` cells |

```csharp
var bytes = XlsxWriter.OrdersWorkbook(orders);      // Order · Customer · Owner · Total · Status · Date
Application.Download(new MemoryStream(bytes), "orders.xlsx");
```

Verified while building: the package re-opens with `ZipArchive`, all six parts are well-formed XML,
`sheet1.xml` has 6 rows for 5 orders (`A2=1042 | B2=Northwind Traders | … | D2=4820.00 | E2=Open`).
Excel, LibreOffice and Google Sheets open this shape.

**In a real project** use a server-safe library instead of hand-writing parts: Open XML SDK, EPPlus,
Syncfusion, Aspose, DevExpress, ClosedXML… — pick by licence and feature needs (formulas, charts,
templates). None of them needs Office installed, all run unattended, all are just bytes at the end.
The same rule applies to Word (document API), Outlook (a mail API / SMTP) and Access (a database).

## Decision record

| Before | After | Why |
|---|---|---|
| `Microsoft.Office.Interop.Excel` at the export button | `XlsxWriter` (or a spreadsheet library) + `Application.Download` | unattended Office is unsupported and multi-user-unsafe; a managed writer is thread-safe, fast, portable to Linux |
| `Workbook.SaveAs(@"C:\Orders\out.xlsx")` | `Application.Download(stream, "orders.xlsx")` + optional copy under `App_Data/reports/` | the file must cross the browser boundary; the server keeps only what the desk needs |
| `PrintDocument` for the invoice | server PDF (`PdfWriter`) in a `PdfViewer` / download | same reasoning, see [report-pipeline.md](report-pipeline.md) |

## Evidence (what the running app shows)

| Action | Trace lines |
|---|---|
| **Export (Excel Interop) ✕** | `← JS→.NET click  Export (Excel Interop) ✕` · `• server ExcelExport.ExportToExcel  new Excel.Application()  ← Microsoft.Office.Interop.Excel shape, on the SERVER` · `✖ fail Excel.Application (COM)  HRESULT 0x80040154 · Retrieving the COM class factory for component with CLSID {00024500-0000-0000-C000-000000000046} failed (Excel.Application). Office Automation needs Excel installed in an interactive user session.` · `★ log Office Interop → managed writer  Office needs an interactive desktop + user profile; unattended on a server it fails or deadlocks (unsupported by Microsoft). Replace with a spreadsheet library + Download.` · red banner `✖ Excel Interop failed on the server (0x80040154 Excel.Application) — Office Automation is not a server strategy`, status `● alarm` |
| **Export (managed .xlsx) ✓** | `• server XlsxWriter.OrdersWorkbook  5 rows · … bytes · ZipArchive parts: [Content_Types].xml, _rels/.rels, xl/workbook.xml, xl/_rels/workbook.xml.rels, xl/styles.xml, xl/worksheets/sheet1.xml` · `• server File.WriteAllBytes  App_Data/reports/orders.xlsx …` · `→ .NET→JS Application.Download  orders.xlsx → browser (Content-Disposition attachment)` · `★ log Excel Interop → XlsxWriter + Download  same rows, no Office on the server; …` · green banner, status `● idle`; after an import the workbook also contains the imported rows |
