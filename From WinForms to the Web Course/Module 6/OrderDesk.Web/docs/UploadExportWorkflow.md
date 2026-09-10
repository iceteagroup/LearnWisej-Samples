# Upload / export workflow — the import and export redesign (lab deliverable 2)

Lab steps covered: *find one file import/export workflow*, *replace local file assumptions with upload/download or
server storage*, *remove direct Office Automation from the migrated path*.

## Before (LegacyOrderDesk)

```csharp
// import
var lines = File.ReadAllLines(@"C:\Orders\in.csv");                       // ✕ the user's disk

// export
var path = ExcelExport.ExportOrders(_orderService.GetOrders(), @"C:\Orders\out.xlsx");   // ✕ Excel Interop + the user's disk
MessageBox.Show("Exported to " + path);
```

Both lines assume that the process runs on the machine whose disk they name and that Excel is installed next to it.
On a web server neither is true: the code runs as a service account on a machine the user has never seen.

## After (OrderDesk.Web, Module 6)

### Import = Upload → storage root → importer → business logic

```csharp
// MainPage.upload_Uploaded
var file = e.Files[i];                                     // Wisej.Core.HttpPostedFile
string path = StorageRoot.UploadPath(file.FileName);       // ✓ sanitized leaf name under App_Data/uploads
using (var target = File.Create(path))
    file.InputStream.CopyTo(target);                       // ✓ client file → server storage

ImportResult result;
using (var stream = File.OpenRead(path))
    result = OrdersCsvImporter.Import(stream, _customerService, _orderService, SessionUser);
```

* `Wisej.Web.Upload` (`AllowedFileTypes = ".csv"`, `MaxFileSize = 1 MB`, single file). The browser filters early; the
  server re-checks the extension and rejects anything that is not a leaf name (`StorageRoot.SafeFileName` strips
  directories, rejects `..`, replaces invalid characters).
* `OrdersCsvImporter` (`Files/OrdersCsvImporter.cs`) parses two layouts:
  * `Customer,PO,Sku,Qty,UnitPrice` — one row per order line; rows sharing Customer + PO form one order
    (the shipped sample `wwwroot/samples/order-batch.csv`: 8 rows → 5 orders).
  * `Order,Customer,Owner,PO,Status,Total` — the layout `CsvExport` writes (Module 1); one order per row with one
    "imported total" line, so an export can be re-imported (round-trip test: *Export .csv ⬇* then upload it).
* Business logic is reused untouched: `CustomerService.FindByName` resolves the customer, `OrderService.Save` computes
  the total (discounts included — Tailspin Toys gets its 5 %) and assigns the id. The importer never learns where the
  bytes came from; a `Stream` is a `Stream`.
* Unknown customers, bad quantities and short rows are **skipped with a reason**, not thrown: the trace shows one
  `import.skipped` line each.

### Export = managed writer → storage root → Download

```csharp
// MainPage.buttonExportXlsx_Click — the lesson's snippet
var rows = _orderService.Search(new OrderFilter());
var path = _reportService.CreateOrdersWorkbook(rows);      // XlsxWriter → App_Data/exports/Orders-<stamp>.xlsx
Application.Download(path, "Orders.xlsx");
```

* `Reporting/XlsxWriter.cs` writes a real `.xlsx` (a ZIP of five XML parts) with `System.IO.Compression.ZipArchive`
  — no Excel, no COM, no dialog, no per-user process. Any number of sessions can export at the same instant; the file
  name carries a millisecond timestamp so they never collide. See `ReportGeneration.md` for the part list.
* `Legacy/ExcelExport.cs` stays in the project marked ✕ and is never called. The console's *Legacy Excel Interop*
  button only runs the first line of it (`Type.GetTypeFromProgID("Excel.Application")`) and explains the result — it
  does **not** create the COM object, because doing that from a server process is exactly the unsupported thing
  (Microsoft KB 257757).

## Evidence (card B and card C)

| Button | Path | What the console shows |
|---|---|---|
| Download sample CSV ⬇ | success | `→ .NET→JS Application.Download  wwwroot\samples\order-batch.csv → order-batch.csv (8 rows, …)`; the browser saves the file |
| Upload order-batch.csv… | success | `← JS→.NET Upload.Uploaded  order-batch.csv · 388 bytes · text/csv` (Chrome on a PC with Excel installed reports the type as `application/vnd.ms-excel`) · `⚠ boundary Import orders  File.ReadAllLines(C:\Orders\in.csv)  ⇒  Upload → App_Data\uploads\order-batch.csv` · `• server OrdersCsvImporter.Import  layout Customer,PO,Sku,Qty,UnitPrice · 8 data row(s) → 5 order(s), 0 skipped` · five `• server OrderService.Save  order 1043 · Northwind Traders · PO NW-88240 · 2 line(s) · total 1,800.00` lines · Toast "5 order(s) imported from order-batch.csv — the file never had a C:\ path." · the order combo in card C grows to 10 entries |
| Upload (a non-.csv or > 1 MB file) | failure | `Upload.Error FileTooLarge …` or the server-side extension check; red banner "✕ Upload refused …" / "✕ …: only .csv files are imported" |
| Legacy import C:\Orders\in.csv | failure | `⚠ boundary ✕ legacy import  DirectoryNotFoundException: Could not find a part of the path 'C:\Orders\in.csv'.`; red banner "✕ Legacy import failed with DirectoryNotFoundException: the path is on the user's PC; the server has no such disk. …". If the folder happens to exist on the developer's PC the banner turns amber and explains that the server *is* that PC today |
| Export .xlsx ⬇ | recovery | `⚠ boundary Export to Excel  Excel.Application + C:\Orders\out.xlsx  ⇒  XlsxWriter → App_Data\exports\Orders-20260910-….xlsx (1,9xx bytes)` · `→ .NET→JS Application.Download  Orders.xlsx (n rows) — no Excel.exe, no COM, no dialog …`; the file opens in Excel / LibreOffice with the "Orders" sheet |
| Export .csv ⬇ | recovery (Module 1) | `→ .NET→JS Application.Download  orders.csv (… bytes, layout Order,Customer,Owner,PO,Status,Total) — upload it back to test the second layout` |
| Legacy Excel Interop | failure | Office installed here: amber banner "✕ Excel is installed on THIS machine, but automating it from a server process is unsupported (Microsoft KB 257757): one interactive Excel per export, no concurrency, hangs on dialogs, runs as the service account. …". No Office: red banner "✕ ProgID Excel.Application not found → the desktop export throws InvalidOperationException … This is the real server case". Linux: red banner "✕ Linux …: no COM, no registry, no Excel.Application …" |

The storage-root label in card B updates after every upload: `uploads  App_Data\uploads\  ·  1 file(s)` and
`last import  order-batch.csv → 5 order(s) saved, 0 skipped · 1043, 1044, 1045, 1046, 1047`.
