# OrderDesk.Web · From WinForms to the Web · Module 1

Local lab build for **Module 1 · Migration Discovery and Risk Mapping**. It follows the lesson and the walkthrough
video: the WinForms application **LegacyOrderDesk** (the "before", included in this folder) is assessed form by form,
every item is tagged with a dependency, a risk tag and one of the four verdicts (direct-port · port-with-adaptation ·
redesign · defer, plus *remove*), a first vertical slice is chosen, and that slice is **run in the browser** with the
business logic reused as-is. A second browser session proves why the static "current user" is a migration task.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 1/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5601
```

Then open <http://localhost:5601>. (Visual Studio: open `OrderDesk.slnx`, set `OrderDesk.Web` as the startup project, F5.
The solution also contains `LegacyOrderDesk`, the WinForms original — it runs on Windows only.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace: `• server startup Default.json → OrderDesk.Program.Main → Application.MainPage = new MainPage()` and `session new browser session xxxxxxxx · 1 session(s) in this process`; the workbook shows 14 items · 6 in the first slice; the Orders grid shows 1042 Northwind Traders 4,820.00 Open first |
| **All / Direct-port / Adapt / Redesign / Defer** | success (deliverable 2) | The workbook filters by verdict; the count label updates; the trace logs `← JS→.NET workbook.filter verdict = redesign → 3 rows`; selecting a row shows *Why* and *Replacement* underneath |
| **New Order** | success (deliverable 3) | `OrderService.Save` runs the same `CalculateOrderTotal` the desktop uses: a Litware order for 1,280.00 appears at the top, a toast confirms, trace `• server OrderService.Save order 1043 · CalculateOrderTotal = 1,280.00 (business logic reused, unchanged)` |
| **Print (PDF)** | recovery | The desktop `PrintDocument → local printer` boundary is replaced: the invoice lines from the shared `InvoiceDocument` become a PDF on the server and open in a modal `PdfViewer` with a Download button; trace `⚠ boundary Print Invoice PrintDocument → local printer ⇒ server PDF (n bytes) → PdfViewer`; closing the window disposes it |
| **Export ⬇** | recovery | `Excel.Application + C:\Orders\out.xlsx` is replaced by bytes built in memory and `Application.Download("orders.csv")` — the browser saves the file, the server never touched a local path |
| **Attach file…** | failure (logged, not faked) | Orange banner: `OpenFileDialog` and `C:\Orders\Attachments` assume the user's disk → verdict redesign → Upload control + server storage root (Module 6). Trace `⚠ boundary Attach file …` |
| **Sign in as kelly** → **Open second session ↗** → in the new tab **Sign in as sam** → back in the first tab **Re-read state** | failure (the static leak) | Red banner (in the Orders card, under the buttons): `✕ Static state leaked across sessions: AppState.CurrentUser = sam, but this session signed in as kelly`. The session card shows the static slot and `Application.Session.User` side by side; status turns red |
| **Re-read state** in the second tab | recovery | `Application.Session.User = sam` and the static slot agree there — per-session state is the fix (Module 4 makes it a typed context) |
| **Clear** | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop boundary hit and replaced (`⚠ boundary`).

## Where things live

```
Module 1/
├─ OrderDesk.slnx                   both projects (OrderDesk.Web is the startup project)
├─ LegacyOrderDesk/                 the WinForms original (net10.0-windows, UseWindowsForms)
│  ├─ Program.cs                    EnableVisualStyles + LoginForm + Application.Run(OrdersForm)
│  ├─ LoginForm / OrdersForm / EditOrderDialog / SettingsForm (+ .Designer.cs)
│  ├─ AppState.cs                   ✕ static CurrentUser / CurrentCompany / CurrentFilter
│  ├─ Settings/RegistrySettings.cs  ✕ HKCU preferences
│  ├─ Reporting/InvoicePrinter.cs   ✕ PrintDocument → local printer
│  ├─ Reporting/ExcelExport.cs      ✕ late-bound Excel Interop + C:\Orders\out.xlsx
│  └─ Domain/                       ✓ Order, Customer, OrderService, CustomerService, InvoiceDocument, SampleData
└─ OrderDesk.Web/                   the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs       session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Properties/launchSettings.json   port 5601 for F5 (dotnet run needs --urls)
   ├─ Default.html / Default.json / Web.config
   ├─ Domain/                       ✓ copied verbatim from LegacyOrderDesk — the reused business logic
   ├─ Legacy/AppState.cs            ✕ copied on purpose, to show the leak live
   ├─ Migration/AssessmentWorkbook.cs   deliverables 1 + 2 as data (14 items)
   ├─ Reporting/InvoicePdfWriter.cs     dependency-free PDF writer (replaces PrintDocument)
   ├─ Reporting/CsvExport.cs            in-memory export (replaces Excel Interop) — Module 6 makes it .xlsx
   ├─ Views/TracePanel.cs, Ui.cs, InvoicePreviewForm.cs
   ├─ MainPage.cs / .Designer.cs    the lab console
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **Application inventory + migration assessment workbook** — [`OrderDesk.Web/docs/MigrationAssessmentWorkbook.md`](OrderDesk.Web/docs/MigrationAssessmentWorkbook.md) (the same rows as [`Migration/AssessmentWorkbook.cs`](OrderDesk.Web/Migration/AssessmentWorkbook.cs), which the app shows and filters)
2. **First vertical slice with acceptance criteria** — [`OrderDesk.Web/docs/FirstSliceAcceptanceCriteria.md`](OrderDesk.Web/docs/FirstSliceAcceptanceCriteria.md); the slice itself is the Orders card of `MainPage`
3. **Migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md), the running log every later module extends

## Self-check answers (lab guide)

- **Which business logic was reused as-is?** Everything under `Domain/`: `OrderService` (totals, discounts, tax,
  search, save), `CustomerService`, `InvoiceDocument`, the `Order`/`Customer` models. The files are copied verbatim
  because they reference neither `System.Windows.Forms` nor `Wisej.Web` and make no desktop assumption.
- **Which desktop boundary was replaced with a web-safe pattern?** Two in the first slice: *Print Invoice* (PrintDocument
  → a PDF generated on the server, shown in a `PdfViewer` / downloaded) and *Export to Excel* (Excel Interop + `C:\Orders`
  → bytes in memory + `Application.Download`). *Attach file* is deliberately only logged: the slice must cross a real
  boundary, not all of them.
- **How is per-user state kept out of static fields?** The console keeps the legacy static `AppState.CurrentUser`
  next to `Application.Session.User` so the leak is visible: one static slot per server versus one value per browser
  session. Module 4 replaces the static with a typed session context.
- **Why classify before porting?** Converting the easy forms first gives a false sense of progress. The workbook surfaces
  the hard items (static state, printer, Office, file system, registry, 200k-row grid) early, and the first slice is
  chosen to prove startup, session context, a screen, a modal window (the invoice preview), a report and one file boundary at once.
- **What was tested before calling the slice complete?** The acceptance criteria in
  `docs/FirstSliceAcceptanceCriteria.md`: the five orders render, New Order produces the same total as the desktop,
  the PDF opens and downloads, the CSV downloads, and two sessions signed in as different users show the static leak.

## Runtime facts

- `Form.ShowDialog()` does not block in Wisej.NET: the invoice preview uses `ShowDialog((form, result) => form.Dispose())`.
- `Application.Navigate(Application.Url, "_blank")` opens a second session in the same server process — the cheapest multi-user test there is.
- `Application.Session` is a dynamic bag; reading a member that was never set returns `null`.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
