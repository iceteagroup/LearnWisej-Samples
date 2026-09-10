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
| Deliberately outside | `Attach file` (`OpenFileDialog`), Settings (registry), Edit dialog, menus, installer | logged as boundaries, scheduled in Modules 3, 4, 6, 7 |

Parity first, modernization second, deployment third: the slice changes no business rule and no workflow order.

## Acceptance criteria

| # | Criterion | How it is verified | Status |
|---|---|---|---|
| A1 | The app starts through the Wisej.NET startup files and shows the Orders screen in a browser | open `http://localhost:5601`; trace line `startup Default.json → OrderDesk.Program.Main → …` | ✓ |
| A2 | The five orders render with the same totals as the desktop (1042 · 4,820.00 · Open first) | Orders grid on load | ✓ |
| A3 | `New Order` saves through the reused `OrderService` and the total equals the desktop formula (5 × 190 + 330 = 1,280.00, no discount for Litware) | click New Order; trace `OrderService.Save order 1043 · CalculateOrderTotal = 1,280.00` | ✓ |
| A4 | Print Invoice produces a document without a printer: a PDF generated on the server opens in the page and can be downloaded | click Print (PDF); PdfViewer window; Download saves `Invoice-1042.pdf` | ✓ |
| A5 | Export produces a file without Office or a local path: the browser receives `orders.csv` | click Export ⬇ | ✓ |
| A6 | Two browser sessions signed in as different users keep separate identities | kelly in tab A, sam in tab B, Re-read in A shows the static slot corrupted and the session value intact | ✓ (the static leak is reproduced; the fix lands in Module 4) |
| A7 | Every desktop boundary that is not crossed yet is logged, not silently faked | Attach file… shows the banner and a `⚠ boundary` trace line | ✓ |
| A8 | The desktop build still works untouched on the migration branch | `dotnet build` of `LegacyOrderDesk` (Windows) | ✓ |

## Evidence

The console's **First slice** card is the slice; its four buttons map to A3–A5 and A7, the **Session check** card (its banners appear in the Orders card, under the buttons) to
A6. The trace legend (`• server`, `→ .NET→JS`, `← JS→.NET`, `⚠ boundary`) is the evidence trail the instructor
review asks for: which logic was reused, which boundary was replaced, how per-user state is handled, what was tested.
