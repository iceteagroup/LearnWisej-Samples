# OrderDesk.Web · From WinForms to the Web · Module 5

Local lab build for **Module 5 · DataGridView, Validation and Performance**. It follows the lesson and the walkthrough
video: the orders grid is ported over a **production-sized store (200,000 orders)**, the desktop habit of loading every
row is run on purpose and measured, then the same screen is fixed with a default filter, server-side sort, **virtual
rows** (50-row blocks on demand) and a Σ row computed on the server. Validation moves out of the form into
`OrderValidator`, which the edit dialog (field-level messages through an `ErrorProvider`), a batch and a direct call all
share.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/From WinForms to the Web Course/Module 5/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5605
```

Then open <http://localhost:5605>. (Visual Studio: open `OrderDesk.slnx`, F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. The first session of the process
seeds the 200,000-row store (about a second, ~60 MB); every later session finds it ready — the trace says which happened.

## What to try

| Action | Path | What you should see |
|---|---|---|
| Page load | success | Trace: `OrderStore.Large 200,000 orders generated in … ms`, `OrderQueryService.Count status=Open · sort=Date desc → n rows`, `grid.RowCount n (VirtualMode …)`, then `block fetched rows 0–49 …` as the grid paints. The first row is **1042 Northwind Traders 4,820.00 Open**; the footer shows the Σ of every Open order, summarized on the server |
| Scroll the grid, or click a row far down | success | one `block fetched rows k–k+49 … (ordered set memoized)` per new block — and nothing for blocks already cached; the Performance card counts blocks, ms and ≈ KB |
| Type *north* in the search box → **Apply filter** | success | a new count + Σ on the server, the cache emptied; only Northwind Traders orders remain, still 50 rows per block |
| Change **Status** to *All* / the sort to *Total ↓* → **Apply filter** | success | the query changes, the browser sorts nothing — `OrderQueryService.Count status=all · sort=Total desc → 200,000 rows` |
| **Naive port: load all rows** (limit 20,000 by default) | failure | `⚠ boundary load-everything OrderService.GetOrders() cloned + sorted 200,000 rows in … ms; 20,000 rows built …`, a red banner, a red row in the Performance card (≈ MB, not KB). Raise the limit to 200,000 to reproduce the video's 8.4 s if your browser is patient |
| **Optimized: filter + virtual rows** | recovery | back to `RowCount` + blocks: the same screen, the same data, kilobytes instead of megabytes |
| **Edit selected…** (or double-click a row) → change the status → *Save* | success (deliverable 3) | `OrderValidator.Validate order n: valid`, `OrderService.Save order n · total … · ordered result set invalidated`, a toast "Order n saved.", the grid re-fetches its blocks and shows the new value |
| **New order (dialog)** → *Save* without filling anything | success (field-level) | the ErrorProvider marks Customer, PO number and Quantity; "Add at least one order line." arrives as a toast; the dialog stays open; trace `OrderValidator.Validate order 0: Customer: … · PoNumber: …` |
| **Validate a bad order (no form)** | success | the field list appears in the validation card, green banner: the same class, no dialog, no MessageBox |
| **Desktop rule → MessageBox** | failure | the one blocking MessageBox LegacyOrderDesk had ("Select a customer."); the banner lists what it missed |
| **Batch 2,000** | progress | the status counts 200 per tick; result *2,000 validated · 0 invalid* — the rule ran with no UI on screen |
| **Measure 10 interactions** | progress | ten random viewport jumps, one Performance row each, then *Average of 10* — the number for the migration log |
| **Open second session ↗** | success | a second tab: the same seeded store (`already seeded by an earlier session`), its own grid and blocks |
| **Clear** | – | Empties the trace |

The right-hand card is the **migration log · live trace**: every user action (`← JS→.NET`), every business-logic call
(`• server`), everything pushed to the browser (`→ .NET→JS`) and every desktop habit hit and replaced (`⚠ boundary`).

## Where things live

```
Module 5/
└─ OrderDesk.Web/                   the Wisej.NET 4 app (net10.0-windows;net10.0)
   ├─ Program.cs / Startup.cs       session entry point (Application.MainPage) / Kestrel host (app.UseWisej())
   ├─ Domain/                       ✓ Module 1 business logic unchanged, plus the Module 5 additions:
   │  ├─ Services/OrderQuery.cs     OrderQuery (filter · sort · page), PagedResult<T>, OrderSummary
   │  ├─ Services/OrderStore.cs     OrderStore.Large (200,000 orders, seeded once), LargeOrderRepository, OrderQueryService
   │  └─ Services/OrderValidator.cs OrderValidator → ValidationResult (field-level)
   ├─ Dialogs/EditOrderDialog.cs    the ported edit dialog: ErrorProvider + server-side validator
   ├─ Legacy/DesktopGridHabits.cs   ✕ ReloadGrid's "load every row" and the in-form MessageBox rule, kept to run on purpose
   ├─ Views/TracePanel.cs, Ui.cs
   ├─ MainPage.cs / .Designer.cs    the lab console (grid card · validation card · performance card)
   └─ docs/                         the lab deliverables
```

## Deliverables

1. **Ported orders grid** — `MainPage` card A over `OrderStore.Large`; binding, edit flow and Σ row working
2. **Filtering + virtual rows** — [`docs/VirtualRowPattern.md`](OrderDesk.Web/docs/VirtualRowPattern.md) (the `RowCount` / `CellValueNeeded` / block-cache pattern and the rules that came out of it)
3. **Server-side validation** — [`docs/ValidationRules.md`](OrderDesk.Web/docs/ValidationRules.md) (the rules, who calls them, evidence)
4. **Performance notes in the migration log** — [`docs/GridPerformanceNotes.md`](OrderDesk.Web/docs/GridPerformanceNotes.md) and the Module 5 section of [`docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md)

## Self-check answers (lab guide)

- **Which business logic was reused as-is?** `OrderService` (totals, discounts, tax, save), `CustomerService`, the
  `Order`/`Customer` models — the edit dialog still ends in `OrderService.Save`, which still calls `CalculateOrderTotal`.
  The Module 5 classes were *added* next to them (`OrderQuery`, `OrderQueryService`, `OrderValidator`); nothing from
  Module 1 changed.
- **Which desktop boundary was replaced with a web-safe pattern?** "Load every row" (`OrdersForm.ReloadGrid`) → a
  server-side query with a default filter, sort in the query, `VirtualMode` with 50-row blocks and a server-computed Σ.
  The console runs the desktop habit on purpose so the difference is measured, not argued.
- **How is per-user state kept out of static fields?** The 200,000-row store is a static — deliberately: immutable-shaped
  reference data behind a lock, shared by every session like a database (Module 4's rule for what *may* stay static).
  The query, the block cache and the selected row are fields of the page, i.e. per session.
- **What was tested before calling the migrated feature complete?** A production-like row count (200,000), the naive
  and the optimized path side by side with server ms and estimated payload in the Performance card, ten timed
  interactions, the edit dialog's valid and invalid paths, the batch, and a second session (`Open second session ↗`).
- **How does the user really find a record — and what filter proves it?** Status first (Open is the working set), then
  customer / order / PO text; the Performance card shows the interaction costing 50 rows instead of 200,000.
- **Why does validation belong on the server, not in client script?** The browser can be bypassed and never sees the
  batch or the API; the rule in `Domain/` runs on every path that reaches the data.

## Runtime facts

- `DataGridView.VirtualMode` + `RowCount` + `CellValueNeeded` behave as in WinForms; a `Dictionary` keyed by block start is
  all the caching the page needs. Column sorting is switched off (`SortMode = NotSortable`) because the grid holds a page.
- `Form.ShowDialog()` does not block in Wisej.NET: the edit dialog is shown with `ShowDialog((form, result) => …)` and
  disposed in the callback.
- The edit dialog edits **one** order line (the first); saving an order that had several lines replaces them with that
  line (1042 goes from 4,820.00 to 3,500.00 after a save). The store is per process, so the change lasts until the app
  restarts — a lab simplification, not a business rule.
- The payload column is an estimate (cell text + ~96 bytes of framing per row); DevTools → Network → WS frames has the
  exact figure.
- The projects multi-target `net10.0-windows;net10.0`, so `dotnet run` needs `-f net10.0` (or `-f net10.0-windows`).
