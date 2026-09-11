# Deliverable 3 · The first vertical slice and its acceptance criteria

The first slice is intentionally small but representative: it must prove **startup**, **user/session context**, a
**screen**, the **business logic**, one **report** and one **file** boundary — so the hard problems show up in week
one, not after all the easy forms have been ported.

## Scope

| Slice item | LegacyOrderDesk | OrderDesk.Web (this module) |
|---|---|---|
| Startup | `Program.Main` → `Application.Run(new OrdersForm())` | `Default.json` → `OrderDesk.Program.Main` → `Application.MainPage = new MainPage()` |
| User context | `LoginForm` → static `AppState.CurrentUser` | `Sign in as kelly / sam` → `Application.Session.User` (the static is kept next to it to show the leak) |
| Screen | `OrdersForm` grid + detail | Orders card: grid + detail, same five orders |
| Business logic | `OrderService.Save` / `CalculateOrderTotal` | the same file, unchanged (`New Order`) |
| Report | `InvoicePrinter.Print` → `PrintDocument` | `InvoicePdfWriter` → `PdfViewer` (+ Download) |
| File boundary | `ExcelExport` → Interop + `C:\Orders\out.xlsx` | `CsvExport` → `Application.Download("orders.csv")` |
| Deliberately outside | `Attach file` (`OpenFileDialog`), Settings (registry), Edit dialog, menus, installer | kept in the backlog, scheduled in Modules 3, 4, 6, 7 |

Parity first, modernization second, deployment third: the slice changes no business rule and no workflow order.

## Acceptance criteria

| # | Criterion | How it is verified | Status |
|---|---|---|---|
| A1 | The app starts through the Wisej.NET startup files and shows the Orders screen in a browser | open `http://localhost:5601`; the "OrderDesk — Orders" page appears | ✓ |
| A2 | The five orders render with the same totals as the desktop (1042 · 4,820.00 · Open first) | Orders grid on load | ✓ |
| A3 | `New Order` saves through the reused `OrderService` and the total equals the desktop formula (5 × 190 + 330 = 1,280.00, no discount for Litware) | click New Order; order 1043 · 1,280.00 appears at the top and a toast confirms | ✓ |
| A4 | Print Invoice produces a document without a printer: a PDF generated on the server opens in the page and can be downloaded | click Print Invoice (PDF); PdfViewer window; Download saves `Invoice-1042.pdf` | ✓ |
| A5 | Export produces a file without Office or a local path: the browser receives `orders.csv` | click Export (Download) | ✓ |
| A6 | Two browser sessions signed in as different users keep separate identities | kelly in tab A, sam in tab B, Re-read state in A shows the static slot overwritten (red) and the session value intact | ✓ (the static leak is reproduced; the fix lands in Module 4) |
| A7 | Every desktop boundary that is not crossed yet is recorded in the backlog, not silently faked | Attach file is in the workbook as *redesign* and in `migration-log.md` | ✓ |
| A8 | The desktop build still works untouched on the migration branch | `dotnet build` of `LegacyOrderDesk` (Windows) | ✓ |

## Evidence

The Orders card of `MainPage` is the slice: New Order, Print Invoice (PDF) and Export (Download) map to A3–A5; the
Session card (Sign in as kelly / sam, Open second session, Re-read state) maps to A6.
