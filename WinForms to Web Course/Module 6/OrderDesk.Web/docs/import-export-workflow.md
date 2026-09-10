# Import / export workflow — upload → process on the server → download (Module 6)

> Lab deliverable 1 of the video: **Upload + process workflow** (user file → server → result) and
> deliverable 2: **Server-generated export** (spreadsheet, no Office Interop). The pattern for user
> files is always the same: *upload → process on the server → return a download or a viewer*.

## Before (LegacyOrderDesk)

```csharp
// the batch import read the user's local disk
var csv = File.ReadAllText(@"C:\Orders\in.csv");                 // ✕

// the export fallback wrote to the user's local disk
var path = LocalExport.WriteCsv(_rows);                           // ✕ C:\Orders\out.csv
MessageBox.Show("Wrote " + path);
```

Both lines are correct on one desktop and wrong on a server: `File.*` runs where the code runs, and
the code now runs on the web server under a service account.

## After (OrderDesk.Web)

### Import — `Wisej.Web.Upload` → `CsvImportService`

```csharp
// MainPage.Designer.cs
this.upload.AllowedFileTypes = ".csv";
this.upload.MaxFileSize = 1048576;
this.upload.Uploaded += this.upload_Uploaded;

// MainPage.cs
private void upload_Uploaded(object sender, UploadedEventArgs e)
{
    for (int i = 0; i < e.Files.Count; i++)
    {
        var file = e.Files[i];                                        // Wisej.Core.HttpPostedFile
        var name = DocumentStorage.SafeFileName(file.FileName);
        byte[] bytes; using (var ms = new MemoryStream()) { file.InputStream.CopyTo(ms); bytes = ms.ToArray(); }

        // 1. stage under the configured root — never a user path, never C:\
        var dest = Path.Combine(DocumentStorage.Imports, DocumentStorage.TimeStamp() + "-" + name);
        File.WriteAllBytes(dest, bytes);

        // 2. process with the reused business logic
        var result = _import.Import(Encoding.UTF8.GetString(bytes));   // OrderService.Save → OrderValidator
    }
}
```

- The CSV contract is the one `LocalExport.ToCsv` already wrote on the desktop:
  `Order,Customer,Owner,Total,Status,Date`. Nothing about the *format* changed — only where the bytes
  come from.
- `CsvImportService` looks the customer up by name (`OrderService.Customers`), parses status/date/total
  with the invariant culture and saves through `OrderService.Save`, so the **same `OrderValidator`**
  the edit dialog uses rejects a row without an owner. Unknown customers are rejected before saving.
- Good rows are saved (a second upload of the same file updates them — idempotent); rejected rows
  are listed in the Import card and in the trace, one line each.
- `App_Data/sample-import.csv` (5 rows) is the reviewer's file: rows 2001–2003 are good, 2004 has an
  unknown customer (`Initech`), 2005 has no owner → **3 saved · 2 rejected**. `Download sample CSV`
  hands it to the browser as `order-batch.csv` (the file name the video uses) so it can be uploaded back.

### Export — `XlsxWriter` → `Application.Download`

```csharp
var bytes = XlsxWriter.OrdersWorkbook(rows);                      // managed OpenXML, framework only
File.WriteAllBytes(Path.Combine(DocumentStorage.Reports, "orders.xlsx"), bytes);   // server copy
Application.Download(new MemoryStream(bytes), "orders.xlsx");     // → the browser saves it
```

`Application.Download(Stream, fileName)` is the browser boundary: the server never knows (or needs)
a path on the user's machine. See [office-automation-replacement.md](office-automation-replacement.md)
for what the writer produces.

## Testing on a clean server-like environment (lab step 6)

- The project multi-targets `net10.0` (no `-windows`): `System.Drawing.Printing`, `System.Windows.Forms`
  and Office Interop are simply not available there, so anything that still referenced them would not
  build. `dotnet build` of both targets is the first "clean environment" test.
- Nothing in the module needs Excel, a printer, SQL Server, the network or any path outside the project
  folder: `App_Data/` is created on demand, uploads and reports go under it.
- `Import (C:\Orders) ✕` and `Export (Excel Interop) ✕` are the two desktop assumptions kept on purpose
  so the failure is visible instead of described.

## Evidence (what the running app shows)

| Action | Trace lines |
|---|---|
| **Download sample CSV** | `← JS→.NET click  Download sample CSV` · `• server File.ReadAllBytes  App_Data/sample-import.csv · 251 bytes (server storage: a template we own)` · `→ .NET→JS Application.Download  order-batch.csv → browser · now pick it in "Upload CSV…"` |
| **Upload CSV…** with `order-batch.csv` | `← JS→.NET Upload.Uploading  order-batch.csv · 251 bytes` · `← JS→.NET Upload.Progress  251 / 251 bytes` · `← JS→.NET Upload.Uploaded  order-batch.csv · 251 bytes · text/csv` · `• server File.WriteAllBytes  App_Data/imports/<timestamp>-order-batch.csv · 251 bytes` · `★ log upload + configured root …` · `• server CsvImportService.Import  5 data rows · columns Order,Customer,Owner,Total,Status,Date (the LocalExport.ToCsv contract)` · `✓ ok OrderService.Save  row 2 · order 2001 · Northwind Traders · $1,250.00 · Open` (×3) · `✖ fail OrderValidator / lookup  row 5 · unknown customer 'Initech'` · `✖ fail OrderValidator / lookup  row 6 · Owner: Assign an owner before saving.` · `• server OrderService.GetAll()  8 rows bound · 1042, 1041, 1040, 1039, 1038 + 3 imported` · `→ .NET→JS Toast  "order-batch.csv imported: 3 saved, 2 rejected."` — the Orders grid now shows 2001–2003 under the five, the Import card lists the 5 results, green banner |
| **Import (C:\Orders) ✕** | `• server LocalExport.ReadCsv  File.ReadAllText(@"C:\Orders\in.csv")  ← the desktop code, unchanged` · `✖ fail File.ReadAllText  FileNotFoundException: Could not find file 'C:\Orders\in.csv' — looked on the SERVER …` · `★ log local read → upload …` · red banner, status `● alarm` |
| **Export (managed .xlsx) ✓** | `• server XlsxWriter.OrdersWorkbook  5 rows · ~2.6 KB · ZipArchive parts: [Content_Types].xml, _rels/.rels, xl/workbook.xml, xl/_rels/workbook.xml.rels, xl/styles.xml, xl/worksheets/sheet1.xml` · `• server File.WriteAllBytes  App_Data/reports/orders.xlsx (server export, kept under the storage root)` · `→ .NET→JS Application.Download  orders.xlsx → browser (Content-Disposition attachment)` · `★ log Excel Interop → XlsxWriter + Download …` — the browser saves `orders.xlsx`; it opens in Excel / LibreOffice with a bold frozen header and `#,##0.00` totals |
