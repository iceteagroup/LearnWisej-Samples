# Virtual rows design — the optimized orders grid (Module 5 · lab step "Replace full-table loading with server-side filter or virtualized access")

## The question first

"Ask how the user really finds data — default filters, a search box, date ranges — not *show me all 200,000 rows*."
The desk works Open orders; the toolbar therefore defaults **Status = Open**, offers a search box (order number,
customer, owner, PO) and a sort combo. Every one of those becomes an `OrderQuery` that travels to the service; only the
requested slice comes back.

## Pieces

```
OptimizedOrdersPanel (Pages/)                  OrderPageCache (Services/)             OrderService / OrderStore (Domain/, unchanged)
──────────────────────────────                 ──────────────────────────             ─────────────────────────────────────────────
toolbar → BuildQuery() ──────────────────────► new OrderPageCache(service, query, 50)
Load():  count = cache.Count() ◄──────────────  service.Count(query) ◄──────────────── CountMatching(Filter(query))
         grid.RowCount = count   (VirtualMode: row shells only, no values)
         cache.Prefetch(0, 49) ◄──────────────  service.Search(query{Skip 0, Take 50}) ◄ Filter → Sort → Skip → Take
grid.DataRead(first,last) ────────────────────► cache.Prefetch(first, last)  (one Search per missing block)
grid.CellValueNeeded(row, col) ───────────────► cache.Get(row) → Order → e.Value
footer Σ Total ◄──────────────────────────────  cache.SumTotal() = service.Search(query).Sum(Total)  (server-side)
```

- **`VirtualMode = true`** — the grid asks for values (`CellValueNeeded`) instead of storing them; `RowCount` is the
  server-side count, so the scrollbar is right without loading anything.
- **`BlockSize = 50`** — the grid requests rows in blocks; the page cache uses the same block size for `Skip/Take`, so one
  scroll into new territory is exactly one `OrderService.Search` call (`DataRead` fires first with `FirstIndex/LastIndex`;
  the cache prefetches every block in that range, then the `CellValueNeeded` calls hit the cache).
- **One cache per query** — filter, search or sort changed → `Apply` (or the grid's ⟳ tool, or Enter in the search box) →
  a new `OrderPageCache`, a new `Count`, a new first block. Nothing is kept in a static; the cache lives in the page,
  i.e. in the session (Module 4 rule: per-user state never in statics; immutable lookups may be).
- **Summary row** — in VirtualMode the grid holds no values, so a grid-side `AddSummaryRows` would only see loaded
  rows. The Σ Total footer is computed on the server over *every* matching row (`SumTotal`) and its cost is printed
  (`summary _ ms on the server`). A client-side sum over the 50 visible rows would be wrong, which is the point.
  `DataGridView.AddSummaryRows(SummaryType.Sum, SummaryRowPosition.Below, groupCol, sumCol)` remains the tool for bound,
  grouped grids and was deliberately not used here.
- **Tool buttons** — `grid.Tools.Add("refresh" | "top" | "edit", icon)`, handled in `ToolClick` by `e.Tool.Name`; the
  video's ＋ ⤓ ⟳ toolbar, added after the basic bind worked.
- **Edit flow** — the current row's `Order` comes from the cache (`SelectedOrder`), the dialog edits a `Clone()`,
  `OrderService.Save` writes to the store, and the grid reloads (new cache, same query) so the change is visible.

## What was measured and why it matters

| | naive port | optimized |
|---|---|---|
| rows fetched from the store on open | 200,000 (`GetAll`) | 50 (`Search`, one block) + a `Count` |
| rows the server grid holds | 20,000 / 200,000 `DataGridViewRow`s with values | ≈44,400 row shells, no values |
| est. payload if every row shipped | ~9.6 MB / ~96 MB | ~38 KB |
| what a second user costs | the same again | the same 50 rows again |
| sort / filter | in memory, after loading everything | in the query, before anything is loaded |

Numbers: [performance-notes.md](performance-notes.md).

## Deliberately not done

- **Live updates / server push** on the grid. The lesson: treat live updates as a business feature, not a default. Order
  rows do not change under the clerk's feet in this workflow. When they do (Module 7's activity feed), push the affected
  rows with `Application.Update`, not a full reload, and only to screens that show them.
- **Client-side caching of user-specific permissions.** The store is shared, immutable reference data (may stay static); row
  permissions would be per user and belong in `Application.Session`.
- **Live search on every keystroke.** `Apply`/Enter keeps event frequency low; a debounce would be the next step, measured first.

## Evidence

Click **Optimized load ✓**: the trace shows the four service calls in order (`Count`, `RowCount`, `Search skip=0 take=50`, `Σ Total`) and
the `✓ ok optimized load` line; the tab shows 1042 Northwind Traders, 1040 Fabrikam Inc, 1038 Globex Corp first (the Open walkthrough
orders), the footer `Σ Total (Open) $… · 44,4xx rows · virtual · block 50`. Scroll: `• server ← DataRead  client needs rows 50–99 → page cache`
then `• server OrderService.Search(query)  … skip=50 take=50 → 50 rows · _ ms · block 1`. Change Status to `Invoiced` and Apply:
`← JS→.NET toolbar  Apply / refresh → status=Invoiced …`, a new `Count`, first row 1039 Adventure Works $12,400.00. Type `Contoso`
and press Enter: `search="Contoso"`, first row 1041 Contoso Ltd $1,290.50 when Status is `Any status`.
