# Grid port — the OrdersForm grid on the web (Module 5 · lab step "Port the customer/order grid")

The desktop screen was `OrdersForm` in `LegacyOrderDesk`: a `System.Windows.Forms.DataGridView` with four
designer columns bound to a `BindingList<Order>` that `ReloadGrid()` filled from `OrderService.GetAll()` — every
row, every time. This document records how that grid was ported and why it now exists twice in the sample.

## What moved, what changed

| OrdersForm (WinForms) | OrderDesk.Web (Wisej.NET) | Note |
|---|---|---|
| `System.Windows.Forms.DataGridView ordersGrid` | `Wisej.Web.DataGridView` in `Pages/NaiveOrdersPanel.cs` and `Pages/OptimizedOrdersPanel.cs` | same type name, same column classes, same events (`CellDoubleClick`) |
| `colId` Order · `colCustomer` Customer (Fill) · `colTotal` Total (C2, right) · `colStatus` Status | kept 1:1, plus `colOwner` Owner | Owner is what the validation rule checks, so the grid shows it |
| `BindingList<Order> _rows` + `DataSource = _rows` | `DataSource = List<Order>` (naive) / `VirtualMode = true` + `RowCount` (optimized) | `DataBoundItem` still gives the row's `Order` on the naive grid |
| `AutoGenerateColumns = false`, `ReadOnly`, `RowHeadersVisible = false`, `MultiSelect = false`, `FullRowSelect` | identical property names | compiled without change |
| `ordersGrid_CellDoubleClick` → `EditOrderDialog.ShowDialog(this)` | `EditRequested` event → `MainPage.EditOrder` → `using (dlg) { dlg.ShowDialog() }` | Module 3 disposal rule; Module 5 adds field-level validation |
| `MessageBox.Show("Saved.")` | `Notify.Saved(...)` (Toast) | Module 3 notification policy |
| `ReloadGrid()` after save | `ReloadActiveGrid()` — naive: `GetAll()` again; optimized: new page cache for the same query | the optimized reload costs one `Count` and one 50-row block |
| `statusLabel.Text = "Ready · n orders"` | grid footer label | naive: rows bound / ms / Δ heap; optimized: server-side Σ Total |

Business logic did not move at all: `OrderService`, `OrderStore`, `OrderCalculator`, `OrderValidator`, `OrderQuery`
are the shared `Domain/*.cs` files, unchanged since Module 1.

## Why two grids

The lab step is *port the grid, then test it with a production-like row count*. The naive port is the honest
"before": it compiles and works with 5,000 rows, so nobody notices the problem until the data is real. Module 5
seeds **200,000 orders** (`OrderStore.Create(200000, 42)`, a private store, not `OrderStore.Shared()`) and keeps the
naive grid next to the fixed one so both can be measured on the same data, in the same session, with the same
Stopwatch:

- **Naive port ✕** (`NaiveOrdersPanel`) — `service.GetAll()` returns the whole table (200,000 rows), the grid binds
  20,000 of them by default (`Naive load (20k) ✕`) or all of them (`Bind all 200k ✕`). Every row becomes a
  `DataGridViewRow` on the server; the footer shows rows, ms and heap delta.
- **Optimized ✓** (`OptimizedOrdersPanel`) — a filter toolbar (Status defaults to **Open**), a search box, a sort
  combo, `VirtualMode = true`, `RowCount = service.Count(query)`, cells filled from a page cache that calls
  `service.Search(query)` with `Skip/Take = 50` per block, grid tool buttons, and a Σ Total footer computed on the
  server over every matching row. Design notes in [virtual-rows-design.md](virtual-rows-design.md).

## Evidence (what the running app shows)

- Page open: the trace starts with `• server Program.Main`, `• server Application.StartTask  seeding OrderStore.Create(200000, 42) in the background`,
  then — pushed from the task thread — `✓ ok OrderStore.Create(200000, 42)  200,000 orders ready in _n_ ms · private store, not OrderStore.Shared()`.
  The Orders card header turns green: `● 200,000 orders ready · seeded in _n_ ms`. Until then the six buttons are disabled.
- **Naive load (20k) ✕**: tab *Naive port* shows 1042 Northwind Traders $4,820.00 Open, 1041 Contoso Ltd $1,290.50 Shipped,
  1040 Fabrikam Inc $760.00 Open, 1039 Adventure Works $12,400.00 Invoiced, 1038 Globex Corp $3,090.00 Open, then the generated
  history. Trace: `• server OrderService.GetAll()  200,000 rows in _n_ ms — the whole table, exactly what OrdersForm.ReloadGrid did`,
  `• server grid.DataSource = list  20,000 rows bound in _n_ ms total · Δ heap +_n_ MB · 20,000 DataGridViewRow objects on the server`,
  `→ .NET→JS DataGridView rows  20,000 rows · ~9.6 MB if every row is shipped (…)`, `✖ fail naive port  capped at 20,000 rows for the demo …`,
  `★ log full-table load  desktop habit: SELECT * then bind …`. Performance card in red.
- **Bind all 200k ✕**: `✖ fail warning  binding all 200,000 rows for real — expect seconds …`, then the same lines with 200,000 rows and
  `~96.0 MB`, `★ log multiply by every session`, banner `✖ 200,000 rows bound in _n_ ms · Δ heap +_n_ MB · ~96.0 MB if every row ships — multiply by every session.`
- **Optimized load ✓**: tab *Optimized* shows the same first rows filtered to Open (1042, 1040, 1038, then history). Trace:
  `• server OrderService.Count(query)  status=Open search="" sort=Id desc skip=0 take=all → 44,4xx rows · _n_ ms`,
  `• server grid.RowCount = 44,4xx  VirtualMode — the grid holds row shells, values come from CellValueNeeded`,
  `• server OrderService.Search(query)  status=Open … skip=0 take=50 → 50 rows · _n_ ms · block 0`, `• server Σ Total (Open)  $… over 44,4xx rows`,
  `→ .NET→JS DataGridView rows  50 rows · ~38 KB — one block of 50 …`, `✓ ok optimized load  first paint _n_ ms · 50 of 44,4xx rows fetched`.
  Scrolling adds `• server ← DataRead  client needs rows a–b → page cache` and one `OrderService.Search(query)` line per new block.
- Double-clicking a row in either grid opens the edit dialog (`← JS→.NET CellDoubleClick  naive grid · order 1042`).
