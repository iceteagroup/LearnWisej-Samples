# Upload / export workflow — the import and export redesign (lab deliverable 2)

Lab steps covered: *find one file import/export workflow*, *replace local file assumptions with upload/download or
server storage*, *remove direct Office Automation from the migrated path*.

## Before (LegacyOrderDesk)

```csharp
// import
var lines = File.ReadAllLines(@"C:\Orders\in.csv");                       // the user's disk

// export
var path = ExcelExport.ExportOrders(_orderService.GetOrders(), @"C:\Orders\out.xlsx");   // Excel Interop + the user's disk
MessageBox.Show("Exported to " + path);
```

Both lines assume that the process runs on the machine whose disk they name and that Excel is installed next to it.
On a web server neither is true: the code runs as a service account on a machine the user has never seen.

## After (OrderDesk.Web, Module 6)

### Import = Upload → storage root → importer → business logic

```csharp
// MainPage.upload_Uploaded
var file = e.Files[i];                                     // Wisej.Core.HttpPostedFile
path = StorageRoot.UploadPath(file.FileName);              // sanitized leaf name under App_Data/uploads
using (var target = File.Create(path))
    file.InputStream.CopyTo(target);                       // client file → server storage

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
  * `Order,Customer,Owner,PO,Status,Total` — an export layout; one order per row with one "imported total" line.
* Business logic is reused untouched: `CustomerService.FindByName` resolves the customer, `OrderService.Save` computes
  the total (discounts included — Tailspin Toys gets its 5 %) and assigns the id. The importer never learns where the
  bytes came from; a `Stream` is a `Stream`.
* Unknown customers, bad quantities and short rows are **skipped with a reason**, not thrown.

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
* The desktop `ExcelExport` is not carried over: Office Automation from a server process is unsupported (Microsoft
  KB 257757).

## Evidence

| Action | What you should see |
|---|---|
| **Upload order-batch.csv…** → pick `wwwroot/samples/order-batch.csv` | `order-batch.csv → 5 order(s) saved, 0 skipped` next to the button, a toast, and orders 1043–1047 at the top of the invoice combo (Tailspin Toys at 5,652.50 after its 5 % discount) |
| Upload a non-.csv or > 1 MB file | a warning toast: the browser refuses files over 1 MB, the server refuses anything that is not `.csv` |
| **Export .xlsx ⬇** | the browser saves `Orders.xlsx` (one sheet "Orders", numeric Total); the file stays under `App_Data/exports/` |
