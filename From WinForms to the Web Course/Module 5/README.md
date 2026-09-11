# OrderDesk.Web · From WinForms to the Web · Module 5

Local lab build for **Module 5 · DataGridView, Validation and Performance**. The orders grid is ported over a
**production-sized store (200,000 orders)** and made usable over a network with a default filter, server-side sort,
**virtual rows** (50-row blocks on demand) and a Σ row computed on the server. Validation moves out of the form into
`OrderValidator`, and the edit dialog shows its messages field by field through an `ErrorProvider`.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 5/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5605
```

Then open <http://localhost:5605>. (Visual Studio: open `OrderDesk.slnx`, F5.) The first session of the process seeds
the 200,000-row store (about a second, ~60 MB); every later session finds it ready.

## What to try

| Action | What you should see |
|---|---|
| Page load | "OrderDesk — Orders": the Open orders, newest first (**1042 Northwind Traders 4,820.00 Open** on top), the Σ footer with the count and total of every Open order, and the Performance panel: `Rows fetched 50 (virtual)` out of ~80,000 matching |
| Scroll the grid | one more block fetch per new block of 50 rows; blocks already held are not fetched again |
| Type *north* in the search box → **Apply** | a new count + Σ on the server; only Northwind Traders orders remain, still 50 rows per block |
| Status *All* / sort *Total ↓* → **Apply** | the query changes; the browser sorts nothing |
| **Edit…** (or double-click a row) → change the status → *Save* | the dialog closes, a toast "Order n saved.", the grid re-counts and shows the new value |
| **New order** → *Save* without filling anything | the ErrorProvider marks Customer and PO number, "Add at least one order line." arrives as a toast, and the dialog stays open |

## Where things live

```
Module 5/
└─ OrderDesk.Web/                   the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs       session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Domain/                       Module 1 business logic unchanged, plus:
   │  ├─ Services/OrderQuery.cs     OrderQuery (filter · sort · page), PagedResult<T>, OrderSummary
   │  ├─ Services/OrderStore.cs     OrderStore.Large (200,000 orders, seeded once), LargeOrderRepository, OrderQueryService
   │  └─ Services/OrderValidator.cs OrderValidator → ValidationResult (field-level)
   ├─ Dialogs/EditOrderDialog.cs    the ported edit dialog: ErrorProvider + server-side validator
   ├─ Views/Ui.cs                   toast helper
   ├─ MainPage.cs / .Designer.cs    the orders grid, the Σ footer and the Performance panel
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **Ported orders grid**: `MainPage` over `OrderStore.Large`; binding, edit flow and Σ row working
2. **Filtering + virtual rows**: [`docs/VirtualRowPattern.md`](OrderDesk.Web/docs/VirtualRowPattern.md)
3. **Server-side validation**: [`docs/ValidationRules.md`](OrderDesk.Web/docs/ValidationRules.md)
4. **Performance notes in the migration log**: [`docs/GridPerformanceNotes.md`](OrderDesk.Web/docs/GridPerformanceNotes.md) and the Module 5 section of [`docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md)

## Self-check answers (lab guide)

- **Which business logic was reused as-is?** `OrderService` (totals, discounts, tax, save), `CustomerService`, the
  `Order`/`Customer` models — the edit dialog still ends in `OrderService.Save`, which still calls `CalculateOrderTotal`.
  `OrderQuery`, `OrderQueryService` and `OrderValidator` were *added* next to them; nothing from Module 1 changed.
- **Which desktop boundary was replaced with a web-safe pattern?** "Load every row" (`OrdersForm.ReloadGrid`) → a
  server-side query with a default filter, sort in the query, `VirtualMode` with 50-row blocks and a server-computed Σ.
- **How is per-user state kept out of static fields?** The 200,000-row store is a static — deliberately: reference data
  behind a lock, shared by every session like a database. The query, the block cache and the selected row are fields
  of the page, i.e. per session.
- **What was tested before calling the migrated feature complete?** A production-like row count (200,000), the
  before/after numbers in `GridPerformanceNotes.md`, the Performance panel for common interactions, and the edit
  dialog's valid and invalid paths.
- **How does the user really find a record — and what filter proves it?** Status first (Open is the working set), then
  customer / order / PO text; the interaction costs 50 rows instead of 200,000.
- **Why does validation belong on the server, not in client script?** The browser can be bypassed and never sees a
  batch or an API; the rule in `Domain/` runs on every path that reaches the data.

## Runtime facts

- `DataGridView.VirtualMode` + `RowCount` + `CellValueNeeded` behave as in WinForms; a `Dictionary` keyed by block start is
  all the caching the page needs. Column sorting is off (`SortMode = NotSortable`) because the grid holds a page.
- `Form.ShowDialog()` does not block in Wisej.NET: the edit dialog is shown with `ShowDialog((form, result) => …)` and
  disposed in the callback.
- The edit dialog edits **one** order line (the first); saving an order that had several lines replaces them with that
  line. The store is per process, so the change lasts until the app restarts.
- The payload figure is an estimate (cell text + ~96 bytes of framing per row); DevTools → Network → WS frames has the
  exact figure.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
