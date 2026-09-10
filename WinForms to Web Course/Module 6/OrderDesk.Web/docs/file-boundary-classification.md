# File boundary classification — LegacyOrderDesk → OrderDesk.Web (Module 6)

> Lab step 1 (*find one file import/export workflow and one report workflow*) and step 2 (*replace
> local file assumptions with upload/download or server storage*). The rule from the lesson: migrated
> file code runs **on the server** unless upload/download or client file-system mediation is used
> explicitly. So every path in the desktop app is a decision now, taken in the table below with the
> five categories of the video: **server storage · upload · download · ClientFileSystem · redesign**.

## The inventory

| Desktop code (LegacyOrderDesk) | What it assumed | Where it would run now | Classification | Decision in OrderDesk.Web |
|---|---|---|---|---|
| `LocalExport.WriteCsv` → `C:\Orders\out.csv` | "the file system" is the user's PC; C:\ exists | the **server's** C: drive, service account | **download** (result for the user) + **server storage** (a copy for the desk) | `XlsxWriter` / `LocalExport.ToCsv` bytes → `Application.Download(stream, "orders.xlsx")`; a copy under `App_Data/reports/` |
| `File.ReadAllText(@"C:\Orders\in.csv")` (the batch import) | the user drops a file in a local folder | the server looks in *its* `C:\Orders` — `FileNotFoundException`, or worse, a stranger's file | **upload** | `Wisej.Web.Upload` (`.csv`) → `HttpPostedFile.InputStream` → staged under `App_Data/imports/<timestamp>-<name>.csv` → `CsvImportService` |
| `ExcelExport.ExportToExcel` (`Excel.Application`, `Workbook.SaveAs(@"C:\Orders\out.xlsx")`) | Excel installed, interactive desktop, local path | COM class factory fails (`0x80040154`) or hangs unattended | **redesign** (Office Interop is not a server strategy) | managed OpenXML writer (`Services/XlsxWriter.cs`) → download |
| `InvoicePrinter.Print` (`PrintDocument.Print()`) | a printer attached to this machine | the server has no user printer; `System.Drawing.Printing` does not exist on net10.0 Linux | **redesign** → server PDF | `Services/PdfWriter.cs` → `PdfViewer.PdfStream`, `Application.Download`, `App_Data/reports/Invoice-NNNN.pdf` |
| `App.config` next to the exe | one config per install | Module 2: `Web.config` | **server storage** (configuration) | `AppConfig` (System.Xml.Linq) — `OrderDesk.StorageRoot = App_Data` |
| `UserPreferences` (HKCU registry) | one user per machine | Module 4: the server's registry, wrong user | **redesign** → per-user server profile / browser storage | done in Module 4 (`App_Data/users/<name>.json`) |
| Report templates / sample files shipped with the app | read next to the exe | `Application.StartupPath` + configured root — fine | **server storage** | `App_Data/sample-import.csv` (created on demand by `DocumentStorage.EnsureSampleImport`) |
| "Open the exported file in Explorer" (`Process.Start(path)`) | a shell on the user's machine | would start a process **on the server** | **redesign** → download / viewer | the browser opens what `Application.Download` hands it |
| Direct access to a client folder (drag a folder of scans) | — | impossible from server code | **ClientFileSystem** — only if the product needs it, chosen deliberately | not needed for OrderDesk; documented as the fifth option |

## The storage root

```xml
<!-- Web.config -->
<add key="OrderDesk.StorageRoot" value="App_Data"/>
```

```csharp
// Services/DocumentStorage.cs
var setting = AppConfig.StorageRootSetting.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
var root = Path.IsPathRooted(setting) ? setting : Path.Combine(AppConfig.AppRoot, setting);
Directory.CreateDirectory(root);
```

- One configured root, every path built with `Path.Combine` — never `C:\…`, never a string with a
  hard-coded separator. On Windows the separator is `\`, in a Linux container it is `/`; the same
  binary runs in both (`TargetFrameworks` `net10.0-windows;net10.0`).
- Linux file names are **case-sensitive**: `Invoice-1042.pdf` and `invoice-1042.pdf` are different
  files. The sample keeps one naming convention (`Invoice-<id>.pdf`, `<timestamp>-<name>.csv`) and
  never relies on case-insensitive lookups.
- Uploaded names are untrusted: `DocumentStorage.SafeFileName` keeps the file name only (no `..`, no
  path) and replaces anything but letters, digits, `.`, `-`, `_`.
- Sub-folders: `imports/` (staged uploads), `reports/` (generated PDFs, the xlsx copy). Both are
  ignored by git (`App_Data/.gitignore`); `sample-import.csv` is committed.

## Evidence (what the running app shows)

- On load the trace prints `• server AppConfig (Web.config)  OrderDesk.StorageRoot = App_Data · Web.config`,
  `• server DocumentStorage.Root  <full path>\App_Data` and the finding
  `★ log C:\Orders → storage root  Path.Combine(app folder, "App_Data") · created on demand · '\' separator on this OS`.
- **Import (C:\Orders) ✕**: `✖ fail File.ReadAllText  FileNotFoundException: Could not find file 'C:\Orders\in.csv' — looked on the SERVER (machine …, account …), not on the browser user's machine.`
  followed by `★ log local read → upload  server code reads the SERVER's disk (…); the user's C: drive is reachable only through Upload` and the red banner.
- **Upload CSV…** (the `Upload` control): `← JS→.NET Upload.Uploading`, `Upload.Progress`, `Upload.Uploaded  order-batch.csv · 251 bytes · text/csv`,
  then `• server File.WriteAllBytes  App_Data/imports/20260910-113000-order-batch.csv · 251 bytes` and
  `★ log upload + configured root  user file → Upload → Path.Combine(storage root, "imports", name) → processed on the server`.
- **Show storage root**: `AppConfig.AppRoot`, the Web.config value and the resolved root, `Environment  <machine> · <account> · <OS> · separator '\'`,
  the finding `★ log Path.Combine, not C:\  Linux deployments: forward slashes and case-sensitive names …` and one `  file` line per file under `App_Data/`.
