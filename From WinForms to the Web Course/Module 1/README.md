# OrderDesk.Web · From WinForms to the Web · Module 1

Local lab build for **Module 1 · Migration Discovery and Risk Mapping**. The WinForms application **LegacyOrderDesk**
(the "before", included in this folder) is assessed form by form, every item is tagged with a dependency, a risk tag and
a verdict (direct-port · port-with-adaptation · redesign · defer · remove), a first vertical slice is chosen, and that
slice runs in the browser with the business logic reused as-is. A second browser session shows why the static
"current user" is a migration task.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 1/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5601
```

Then open <http://localhost:5601>. (Visual Studio: open `OrderDesk.slnx`, set `OrderDesk.Web` as the startup project, F5.
The solution also contains `LegacyOrderDesk`, the WinForms original, which runs on Windows only.)

## What to try

| Action | What you should see |
|---|---|
| Page load | "OrderDesk — Orders": the five orders, 1042 Northwind Traders 4,820.00 Open first, with the selected order's detail on the right |
| **New Order** | `OrderService.Save` runs the same `CalculateOrderTotal` as the desktop: a Litware order for 1,280.00 appears at the top and a toast confirms |
| **Print Invoice (PDF)** | The invoice is built as a PDF on the server and opens in a modal `PdfViewer` with a Download button (replaces `PrintDocument`) |
| **Export (Download)** | `orders.csv` is built in memory and sent with `Application.Download` (replaces Excel Interop + `C:\Orders\out.xlsx`) |
| **Sign in as kelly** → **Open second session ↗** → in the new tab **Sign in as sam** → back in the first tab **Re-read state** | The Session card turns red: `AppState.CurrentUser (static)` now says sam while `Application.Session.User` still says kelly. The static is one slot for the whole server |

## Where things live

```
Module 1/
├─ OrderDesk.slnx                   both projects (OrderDesk.Web is the startup project)
├─ LegacyOrderDesk/                 the WinForms original (net10.0-windows, UseWindowsForms)
└─ OrderDesk.Web/                   the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs       session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                       copied verbatim from LegacyOrderDesk: the reused business logic
   ├─ Legacy/AppState.cs            the desktop static, copied to show the leak
   ├─ Reporting/InvoicePdfWriter.cs dependency-free PDF writer (replaces PrintDocument)
   ├─ Reporting/CsvExport.cs        in-memory export (replaces Excel Interop)
   ├─ Views/Ui.cs, InvoicePreviewForm.cs
   ├─ MainPage.cs / .Designer.cs    the Orders screen and the Session card
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **Application inventory + migration assessment workbook**: [`OrderDesk.Web/docs/MigrationAssessmentWorkbook.md`](OrderDesk.Web/docs/MigrationAssessmentWorkbook.md)
2. **First vertical slice with acceptance criteria**: [`OrderDesk.Web/docs/FirstSliceAcceptanceCriteria.md`](OrderDesk.Web/docs/FirstSliceAcceptanceCriteria.md); the slice itself is `MainPage`
3. **Migration log**: [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md), the running log every later module extends

## Self-check answers (lab guide)

- **Which business logic was reused as-is?** Everything under `Domain/`: `OrderService` (totals, discounts, tax,
  search, save), `CustomerService`, `InvoiceDocument`, the `Order`/`Customer` models. The files are copied verbatim
  because they reference neither `System.Windows.Forms` nor `Wisej.Web` and make no desktop assumption.
- **Which desktop boundary was replaced with a web-safe pattern?** *Print Invoice* (PrintDocument → a PDF generated on
  the server, shown in a `PdfViewer` / downloaded) and *Export to Excel* (Excel Interop + `C:\Orders` → bytes in memory +
  `Application.Download`). *Attach file* stays in the backlog: the slice must cross a real boundary, not all of them.
- **How is per-user state kept out of static fields?** The page stores the signed-in user in `Application.Session`;
  the legacy static `AppState.CurrentUser` is shown next to it so the two-session leak is visible. Module 4 replaces the
  static with a typed session context.
- **Why classify before porting?** Converting the easy forms first gives a false sense of progress. The workbook surfaces
  the hard items (static state, printer, Office, file system, registry, 200k-row grid) early.
- **What was tested before calling the slice complete?** The acceptance criteria in
  `docs/FirstSliceAcceptanceCriteria.md`: the five orders render, New Order produces the same total as the desktop,
  the PDF opens and downloads, the CSV downloads, and two sessions signed in as different users show the static leak.

## Runtime facts

- `Form.ShowDialog()` does not block in Wisej.NET: the invoice preview uses `ShowDialog((form, result) => form.Dispose())`.
- `Application.Navigate(Application.Url, "_blank")` opens a second session in the same server process.
- `Application.Session` is a dynamic bag; reading a member that was never set returns `null`.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
