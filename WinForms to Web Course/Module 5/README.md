# OrderDesk · From WinForms to the Web · Module 5

Local lab build for **Module 5 · DataGridView, Validation & Performance**. It follows the walkthrough video
(`wisej-wf-data-grid-validation`: the naive port pulls 200k rows and paints in 8.4 s; a default filter + virtual rows
makes the same screen instant; validation is a reusable server rule with field-level messages) and the lab guide
(port the customer/order grid, add validation to the edit workflow, test with a large row count, replace full-table
loading with a server-side filter or virtualized access, measure, record performance notes).

The screen is the `OrdersForm` grid of `LegacyOrderDesk`, ported twice into one Orders card: **Naive port ✕**
(`DataSource = service.GetAll()`, the WinForms habit) and **Optimized ✓** (Status filter defaulting to Open, search,
sort, `VirtualMode` + `RowCount = Count(query)`, cells from a page cache that calls `OrderService.Search` one 50-row
block at a time, grid tool buttons, a server-side Σ Total footer). A 200,000-order private store is seeded in the
background when the page opens; a Performance card and the trace show Stopwatch timings, rows, heap delta and an
estimated payload for every load. `EditOrderDialog` validates through `OrderValidator` and shows `ErrorProvider`
field messages; `OrderService.Save` validates again and throws `ValidationException`.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/WinForms to Web Course/Module 5/OrderDesk.Web"
dotnet run -f net10.0 --urls http://localhost:5105
```

Then open <http://localhost:5105>. (Visual Studio: open `OrderDesk.slnx`, press F5 — `Properties/launchSettings.json` uses port 5105.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package. No other packages.

## What to try

The page opens immediately with the six buttons disabled and the Orders card header `● seeding 200,000 orders in the background…`.
After a moment the header turns green (`● 200,000 orders ready · seeded in _n_ ms`), the trace shows
`✓ ok OrderStore.Create(200000, 42)  200,000 orders ready in _n_ ms · private store, not OrderStore.Shared()` and
`→ .NET→JS Application.Update(this)  store ready — buttons enabled, pushed over the WebSocket from the task thread`, and the buttons enable.

| Action | Path | What you should see |
|---|---|---|
| **Naive load (20k) ✕** | failure / measure | Tab *Naive port* fills: 1042 Northwind Traders $4,820.00 Open · 1041 Contoso Ltd $1,290.50 Shipped · 1040 Fabrikam Inc $760.00 Open · 1039 Adventure Works $12,400.00 Invoiced · 1038 Globex Corp $3,090.00 Open, then history. Trace: `• server OrderService.GetAll()  200,000 rows in _ ms — the whole table, exactly what OrdersForm.ReloadGrid did`, `• server grid.DataSource = list  20,000 rows bound in _ ms total · Δ heap +_ MB · 20,000 DataGridViewRow objects on the server`, `→ .NET→JS DataGridView rows  20,000 rows · ~9.6 MB if every row is shipped (rows × 5 cols × 96 B + 14 KB grid definition) …`, `✖ fail naive port  capped at 20,000 rows for the demo — Bind all 200k ✕ binds the real table`, `★ log full-table load  desktop habit: SELECT * then bind …`. Performance card red; banner `Naive port: 20,000 of 200,000 rows bound in _ ms · … — and that is only a tenth of the table.`; status `● idle — naive grid bound (20,000 rows)` |
| **Bind all 200k ✕** | failure (the real thing) | First `✖ fail warning  binding all 200,000 rows for real — expect seconds and a large heap …`, status `● working — binding 200,000 rows…`. Then the same three server lines with 200,000 rows and `~96.0 MB`, `★ log multiply by every session  the video's 8.4 s / 96 MB screen …`, banner `✖ 200,000 rows bound in _ ms · Δ heap +_ MB · ~96.0 MB if every row ships — multiply by every session.`, status `● alarm — the whole table is bound in one session's grid`. Expect this click to take seconds. |
| **Optimized load ✓** | success | Tab *Optimized* shows the Open orders (1042, 1040, 1038 first), footer `Σ Total (Open)  $… · 44,4xx rows · virtual · block 50 · summary _ ms on the server`. Trace: `• server OrderService.Count(query)  status=Open search="" sort=Id desc skip=0 take=all → 44,4xx rows · _ ms`, `• server grid.RowCount = 44,4xx  VirtualMode — the grid holds row shells, values come from CellValueNeeded`, `• server OrderService.Search(query)  status=Open … skip=0 take=50 → 50 rows · _ ms · block 0`, `• server Σ Total (Open)  $… over 44,4xx rows · _ ms — server-side; a client sum over 50 visible rows would be wrong`, `→ .NET→JS DataGridView rows  50 rows · ~38 KB — one block of 50 …`, `✓ ok optimized load  first paint _ ms · 50 of 44,4xx rows fetched · Δ heap …`, `★ log virtual rows + default filter …`. Performance card green; banner `✓ Optimized: status=Open … → 44,4xx virtual rows, 50 fetched for the first paint in _ ms · ~38 KB. Scroll the grid: every new block is one Search call in the trace.` |
| Scroll the optimized grid | success (virtual rows) | Per new block: `• server ← DataRead  client needs rows 50–99 → page cache` then `• server OrderService.Search(query)  … skip=50 take=50 → 50 rows · _ ms · block 1`. Nothing else is fetched. |
| Toolbar: Status `Invoiced` → **Apply**; type `Contoso` + Enter; sort `Total ↓`; grid tools ⟳ / ⇡ / ✎ | success (server-side query) | `← JS→.NET toolbar  Apply / refresh → status=Invoiced …`, a new `Count`/`Search` pair, first row 1039 Adventure Works $12,400.00. Search `Contoso` with `Any status` → first row 1041 Contoso Ltd. The ⟳ tool logs `← JS→.NET ← ToolClick  grid tool "refresh"` and reloads; ⇡ scrolls to row 0; ✎ opens the edit dialog for the current row. |
| **Measure** | progress (Timer) | Buttons disable, status `● working — measuring: naive 20k vs optimized, 5 runs each`; every 700 ms one run (`• server measure 1/5  naive 20k → _ ms`, `• server measure 1/5  optimized → _ ms`, …) while both tabs and the Performance card update. After 10 runs: `★ log performance notes  server-side Stopwatch, 200,000-order store, this machine:`, `★ log naive 20k  min _ · avg _ · max _ ms (5 runs) · 200,000 fetched · 20,000 in grid · ~9.6 MB`, `★ log optimized  min _ · avg _ · max _ ms (5 runs) · 50 fetched · 44,4xx in grid · ~38 KB`, `• server File.WriteAllText  App_Data/performance-notes.md (under the project folder, never C:\Orders)`; banner `Measured — … Copy these into docs/performance-notes.md.` |
| **Edit selected → validate** | success + field-level validation | `• server EditOrderDialog  new EditOrderDialog(order 1042 · Northwind Traders · $4,820.00) from button — inside using, ShowDialog() blocks this handler` (with no grid loaded it uses 1042; otherwise the current row of the active tab). In the dialog: set Owner to `(none)` → Save → ErrorProvider icon on Owner (`Assign an owner before saving.`), summary `1 field needs attention: Owner`, trace `✖ fail ErrorProvider → Owner  …`, `• server OrderValidator.Validate  1 error(s) → ErrorProvider.SetError on Owner (dialog stays open)`. Set Quantity to 200 (Northwind, limit $50,000) → the Total label turns red → Save → `✖ fail ErrorProvider → Total  Total $91,580.00 exceeds the credit limit $50,000.00.` Fix both (Owner Dana, Quantity 10) → Save → `✓ ok OrderValidator.Validate  valid …`, `• server ShowDialog()  returned DialogResult.OK — the handler resumed here`, `✓ ok OrderService.Save  order 1042 saved · Northwind Traders · owner Dana · $4,820.00 · Open`, `→ .NET→JS Toast  "Order 1042 saved." — non-blocking (Module 3 policy)`, the active grid reloads, `✓ ok dialog disposed  dlg.IsDisposed = True after the using block …`. Cancel → `• server EditOrderDialog  Cancel → DialogResult.Cancel`, status `● idle — edit cancelled, nothing saved`. |
| Double-click a row (either grid) | same as above | `← JS→.NET CellDoubleClick  naive grid · order 1041` (or `CellDoubleClick / tool  optimized grid · order …`) then the dialog flow. |
| **Save invalid ✕** | failure (server boundary) | `• server order 1039 clone  Owner = null · Lines[0].Quantity = 99,999 → Total $147,235,275.00 vs credit limit $80,000.00 (Adventure Works)`, `✖ fail OrderService.Save  ValidationException — 2 field(s)`, `✖ fail   Owner  Assign an owner before saving.`, `✖ fail   Total  Total $147,235,275.00 exceeds the credit limit $80,000.00.`, banner `✖ ValidationException from OrderService.Save — Owner: … · Total: …`, `✓ ok store unchanged  order 1039 still $12,400.00 · owner Sam · Invoiced`, `★ log validation boundary  the rule lives in OrderValidator and the service enforces it …`; status `● alarm — invalid save rejected on the server; store unchanged`. Safe to click repeatedly. |
| Tabs *Naive port ✕* / *Optimized ✓* | – | `← JS→.NET tab  Naive port ✕` / `Optimized ✓`. **Clear** (trace header) empties the trace. |

Every button is idempotent; nothing needs a printer, Excel, SQL Server, the network or files outside the project folder
(Measure writes `App_Data/performance-notes.md` under `OrderDesk.Web`).

## Where things live

```
Module 5/
├─ OrderDesk.slnx · .gitignore
└─ OrderDesk.Web/
   ├─ MainPage.cs / MainPage.Designer.cs   the module screen: app bar, Orders card (TabControl: naive / optimized), Performance card,
   │                                        status + banner, six buttons, Measure Timer, TracePanel; background seed via Application.StartTask
   ├─ Pages/NaiveOrdersPanel.cs            the OrdersForm grid ported the WinForms way (DataSource = list), timed
   ├─ Pages/OptimizedOrdersPanel.cs        filter toolbar + VirtualMode grid + DataRead/CellValueNeeded + tool buttons + Σ footer
   ├─ Dialogs/EditOrderDialog.cs           Wisej.Web.Form: Customer / Owner / Status / Quantity, OrderValidator → ErrorProvider
   ├─ Services/BigOrderData.cs             OrderStore.Create(200000, 42) — the private production-like store, seed time
   ├─ Services/OrderPageCache.cs           block cache: one OrderService.Search(Skip/Take) per 50-row block, server-side Σ
   ├─ Services/GridMetrics.cs              LoadMetrics, PayloadEstimate (rows × 5 × 96 B + 14 KB), PerformanceLog (trace table + Markdown)
   ├─ Domain/                              shared, unchanged: Order, OrderStore, OrderService, OrderValidator, OrderQuery, User
   ├─ Shared/                              TracePanel, Palette, Notify (lab props)
   ├─ Program.cs · Startup.cs · Default.html · Default.json · Web.config · Properties/launchSettings.json (port 5105)
   └─ docs/                               the lab deliverables (below) + migration-log.md
```

## Deliverables

1. **Ported orders grid** — [`OrderDesk.Web/docs/grid-port.md`](OrderDesk.Web/docs/grid-port.md) (`Pages/NaiveOrdersPanel.cs`, `Pages/OptimizedOrdersPanel.cs`)
2. **Filtering + virtual rows** — [`OrderDesk.Web/docs/virtual-rows-design.md`](OrderDesk.Web/docs/virtual-rows-design.md) (`Services/OrderPageCache.cs`)
3. **Server-side validation, field-level messages** — [`OrderDesk.Web/docs/validation-pattern.md`](OrderDesk.Web/docs/validation-pattern.md) (`Dialogs/EditOrderDialog.cs`, `Domain/OrderValidator.cs`)
4. **Performance notes** — [`OrderDesk.Web/docs/performance-notes.md`](OrderDesk.Web/docs/performance-notes.md) (placeholders the reviewer overwrites with what Measure prints)
5. **Migration log** — [`OrderDesk.Web/docs/migration-log.md`](OrderDesk.Web/docs/migration-log.md) (Modules 1–5)

## Self-check answers

- **Why should grids be tested with production-like row counts?** Because small test data hides load and interaction
  problems: the naive port here is perfectly fine with the 5,000-row store of the other modules and only shows its cost —
  seconds, hundreds of MB of server heap per session, an estimated 96 MB if every row shipped — at 200,000 rows. That is
  why this module seeds 200,000 orders before calling the grid "ported".
- **Which Wisej.NET grid capability helps with large datasets?** Virtual rows (`DataGridView.VirtualMode` with `RowCount`
  and `CellValueNeeded`, rows requested in `BlockSize` blocks), together with server-side filtering and sorting through
  `OrderQuery`. (The published answer key for this question marks "Registry keys"; that is a key error — registry keys are the
  Module 4 desktop boundary, not a grid feature.)
- **Where should important business validation remain?** On the server / business layer, with UI feedback:
  `OrderValidator` is the rule, `OrderService.Save` enforces it (`ValidationException`), and the dialog renders the same
  result inline through `ErrorProvider`. Client-side checks are a nice touch, never the security boundary.
- **When should live updates be used?** When the business workflow benefits from real-time changes — not for every control
  by default. The orders grid gets none; Module 7's activity feed does.
- **Main outcome of Module 5 / the lab activity / the four learning objectives:** port the data-heavy parts without creating
  slow or chatty browser screens — port the grid, add validation to the edit workflow, test with a large row count, replace
  full-table loading with a virtualized pattern, measure, record. Objectives shown here: data binding ported with the rules on
  the server (`OrderValidator`), virtual rows / paging-like access for the large set (`OrderPageCache`), no unnecessary data
  loaded just because the desktop did (default filter Open, 50-row blocks), and server push used only where a screen benefits.
- **True/false facts:** grids must be tested with realistic counts and optimized with server-side filtering, lazy loading or
  virtual rows — true. Validation stays in server/business code with clear UI feedback — true. Live updates when useful, not
  indiscriminately — true. Preserve business logic and workflow before modernizing the UI — true (parity first). Standard
  controls move from `System.Windows.Forms` to `Wisej.Web` — true, with compiler-guided property review (`DataGridView`
  compiled here almost unchanged). Static fields are a safe place for per-user state — false (Module 4; the page cache lives
  in the page/session). Registry access and local file paths are desktop boundaries to replace — true. The first milestone
  should be a complete UI redesign — false (functional parity). A vertical slice should prove startup, navigation, data, modal
  workflow, files/reports and session context — true. Server-side Office COM is the recommended way to generate browser
  reports — false (Module 6).

### Pause & predict (video deliverables scene)

- **How does the user really find a record — and what filter proves it?** The order desk works the Open queue and looks
  orders up by number, customer, owner or PO. The proof is the toolbar's default: Status = Open turns 200,000 rows into
  ≈44,400 candidates before anything is fetched, and the search box narrows to a handful (`Contoso` → 1041). If the default
  filter had not changed the count, it would have been the wrong question.
- **Why does validation belong on the server, not in client script?** The server owns the session and the data, so it is the
  only place a rule cannot be bypassed; the same `OrderValidator` then serves the dialog, a batch import and a web API
  unchanged, and its field-level result is what the UI renders. `Save invalid ✕` shows a request that never went through a
  dialog being rejected by `OrderService.Save` with the store untouched.

## Notes for the reviewer

Compile-checked against Wisej-4 4.1.0 (`dotnet build`, 0 errors / 0 warnings on `net10.0-windows` and `net10.0`) but **not run** in a browser:

- `DataGridView.VirtualMode` + `RowCount` + `CellValueNeeded` + `DataRead` (`FirstIndex/LastIndex` prefetch) + `BlockSize = 50` — the whole optimized path.
  If cells stay empty, check that `CellValueNeeded` fires with `ColumnIndex` 0–4 in the column order Order · Customer · Owner · Total · Status.
- `Form.ShowDialog()` without a callback inside `using` — the API reference says it suspends the server thread. If the dialog
  disappears immediately (i.e. it did not block), switch `MainPage.EditOrder` to `dlg.ShowDialog((form, result) => { … form.Dispose(); })`.
- `ErrorProvider(components)` + `ContainerControl = this` + `SetError` on a `ComboBox` / `NumericUpDown`; the icon should appear
  at the right edge of the field (24 px are left free).
- Grid tool buttons: `grid.Tools.Add("refresh", "icon-refresh")`, `"icon-up"`, `"icon-edit"` — if the theme lacks an icon the
  button renders blank but still fires `ToolClick`.
- `TabControl.TabPages.Add`, `TextBox.Watermark`, `NoDataMessage`, `Application.Update(this)` called from the `StartTask` thread
  after the seed (the six buttons enable and the header turns green without a click).
- **Bind all 200k ✕** really binds 200,000 rows: expect the click to take seconds and the process heap to grow by hundreds of MB.
  Clicking it twice is safe (the previous rows are released). **Measure** takes about 7–10 s (10 runs, 700 ms apart).
- The generated history continues below order 1037 and, for a set this large, into negative order numbers — a quirk of the shared
  seed, harmless; the five walkthrough orders are always first.
- Currency formatting follows the server culture (`$` on this machine).
- `App_Data/performance-notes.md` is created under `OrderDesk.Web` when Measure finishes; `App_Data` is gitignored-safe scratch, nothing is written elsewhere.
