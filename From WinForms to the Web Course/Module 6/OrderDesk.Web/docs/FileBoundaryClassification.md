# File boundary classification — LegacyOrderDesk (lab deliverable 1)

The browser is not the desktop. Every `File.*`, `Directory.*`, `Registry.*`, `Process.Start`, `PrintDocument` and
`Excel.Application` call that LegacyOrderDesk made ran on the **user's PC**. After the migration the same call runs on
the **server**, unless a client-mediated feature (Upload, Download, ClientFileSystem) is used on purpose. This table
classifies every file, Office and printer touch of the desktop app into one of the five answers the lesson gives.
The same rows are data in `Files/FileBoundaryClassifier.cs` and drive card A of the console.

| Feature (LegacyOrderDesk) | Legacy API · path | Class | Replacement in OrderDesk.Web |
|---|---|---|---|
| Import orders | `File.ReadAllLines(@"C:\Orders\in.csv")` | **Upload** | `Wisej.Web.Upload` (.csv, ≤ 1 MB) → `StorageRoot.Uploads` → `OrdersCsvImporter` → `OrderService.Save` |
| Export to Excel | `Excel.Application` (Interop) → `workbook.SaveAs(@"C:\Orders\out.xlsx")` | **Download** | `XlsxWriter` (managed, no Office) → `StorageRoot.Exports` → `Application.Download(path, "Orders.xlsx")` |
| Office Automation itself | `Activator.CreateInstance(Type.GetTypeFromProgID("Excel.Application"))` | **Redesign** | never on the server: EPPlus / Open XML SDK / Aspose / Syncfusion / DevExpress — here a hand-written .xlsx writer |
| Print Invoice | `PrintDocument` + `PrintPreviewDialog` (System.Drawing.Printing) | **Redesign** | `InvoiceDocument` lines → `InvoicePdfWriter` on the server → `PdfViewer` (`InvoicePreviewForm`) or Download |
| Attach file | `OpenFileDialog` → `File.Copy(…, @"C:\Orders\Attachments\<order>")` | **Upload** | the same Upload control → `StorageRoot.Uploads/<order>/` (or a database blob) |
| Open an attachment | `Process.Start(path)` — the default app on the user's PC | **ClientFileSystem** | `Application.Download` (the browser opens it); direct client file access only where the platform permits it |
| Settings · Export folder | `HKCU\Software\LegacyOrderDesk\ExportFolder = C:\Orders` | **Server storage** | `Web.config` `OrderDesk.StorageRoot` resolved with `Path.Combine(Application.StartupPath, …)`; the user never picks a server folder |
| Long reports (invoice batch) | for-each order → `PrintDocument`, UI frozen until the last page | **Server storage** | `ReportQueue` job → PDF under `StorageRoot.Reports`; pages poll status; View ▸ / Download ⬇ when Done |
| Application log | `File.AppendAllText(%APPDATA%\LegacyOrderDesk\app.log)` | **Server storage** | host logging (ILogger / console / a configured server folder), one log for all sessions tagged with the session id |

Counts: Server storage 3 · Upload 2 · Download 1 · ClientFileSystem 1 · Redesign 2.

## The four patterns (plus "redesign")

1. **Upload** — a client file must reach the server. `Wisej.Web.Upload` sends the bytes over HTTP; `Uploaded` hands the
   server an `HttpPostedFile` (`FileName`, `ContentLength`, `InputStream`). The server chooses the destination
   (`StorageRoot.UploadPath(file.FileName)`), never the browser. `AllowedFileTypes` and `MaxFileSize` filter early; the
   server validates again (extension, sanitized leaf name).
2. **Download** — a server-generated file must reach the user. `Application.Download(stream, name)` for bytes in memory,
   `Application.Download(path, name)` for a file under the storage root. The browser saves or opens it; the server never
   writes to a user path.
3. **Server storage** — the server owns the file: templates, exports, report results, logs. One configured root
   (`OrderDesk.StorageRoot`), sub-folders created on demand, every path built with `Path.Combine`. In production the
   root is a mounted volume, a share or a blob store; the code does not change.
4. **ClientFileSystem** — the application must work with the user's files directly (a scanner folder, a local cache)
   and the platform allows it (a desktop-hosted Wisej client, the browser File System Access API behind a user gesture).
   Rare; every row above is served by Upload/Download instead.
5. **Redesign** — no mapping exists. Office Automation and local printers are not "moved", they are replaced:
   managed document writers, server-side PDF, PdfViewer.

## Linux path notes

* Paths are **case-sensitive**: `App_Data/Uploads` and `App_Data/uploads` are two folders. `StorageRoot` uses one
  spelling (`uploads`, `exports`, `reports`) and nothing else composes those names.
* The separator is `/`. A literal `@"C:\Orders\in.csv"` is a *relative file name* on Linux (backslashes are ordinary
  characters), so `File.ReadAllLines` throws `FileNotFoundException` instead of `DirectoryNotFoundException` — the
  console's "Legacy import" button catches both.
* `Path.Combine`, `Path.GetFileName`, `Path.DirectorySeparatorChar`, `Path.GetRelativePath` do the right thing on both
  platforms; string concatenation with `"\\"` does not. `Path.GetFileName` strips both separator styles on .NET Core.
* `Application.StartupPath` is the content root under `dotnet run` (the project folder) and the deployed folder under
  IIS / systemd / a container. `Path.Combine(Application.StartupPath, "App_Data")` therefore works everywhere.
* `Type.GetTypeFromProgID`, `Registry.CurrentUser`, `System.Drawing.Printing` are Windows-only; the last one does not
  even compile for `net10.0`, which is why `InvoicePrinter` survives only as a quoted excerpt in
  `Legacy/DesktopBoundaries.cs`.

## Evidence

* **Card A** lists the nine rows above; the six filter buttons (All · Server storage · Upload · Download ·
  ClientFileSystem · Redesign) reduce the grid and the trace logs `← JS→.NET boundaries.filter  class = Upload → 2 rows`.
  Selecting a row shows *legacy call → replacement → evidence* under the grid.
* **Load trace** shows the configured root replacing the literals:
  `⚠ boundary storage root  C:\Orders\…  ⇒  <project>\App_Data  (OrderDesk.StorageRoot = "App_Data" · n file(s) · separator '\')`.
* Every other card is the evidence of one row: card B (Import), card C (Export, Office, Print), card D (long reports).
