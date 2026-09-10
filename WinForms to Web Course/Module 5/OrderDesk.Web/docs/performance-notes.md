# Performance notes — measured grid loads (Module 5 · lab steps "Test with a large row count", "Measure initial load and common interaction time")

Data set: `OrderStore.Create(200000, 42)` — 200,000 orders in a private in-memory store (the five walkthrough orders
first, then generated history). Seeded in `Application.StartTask` when the page opens; seed time is the first ✓ line in the
trace. Server-side milliseconds are `Stopwatch` measurements taken by the app; the payload column is an **estimate**
(`rows × 5 columns × 96 B + 14 KB grid definition`, calibrated to the video's 200,000 rows ≈ 96 MB and 50 rows ≈ 38 KB).
Wisej.NET already streams grid rows in blocks, so the real bytes per request are smaller than the estimate — measure them in
DevTools → Network; the server numbers (rows created, ms, heap) are the cost that multiplies by every session.

## The table the reviewer fills in

Replace the `_` placeholders with what the **Measure** button prints (`★ log performance notes` lines, also written to
`App_Data/performance-notes.md` under the project folder) and what **Bind all 200k ✕** shows.

| Mode | Runs | Min ms | Avg ms | Max ms | Rows fetched from store | Rows in grid (server) | Δ heap | Est. payload |
|---|---|---|---|---|---|---|---|---|
| store seed (`OrderStore.Create`) | 1 | — | _ | — | — | 200,000 orders | _ | — |
| naive 20k (`GetAll()` → `Take(20000)` → `DataSource`) | 5 | _ | _ | _ | 200,000 | 20,000 | +_ MB | ~9.6 MB |
| naive 200k (`Bind all 200k ✕`) | 1 | — | _ | — | 200,000 | 200,000 | +_ MB | ~96.0 MB |
| optimized (`Count` + `RowCount` + first 50-row block, filter Open) | 5 | _ | _ | _ | 50 | ≈44,400 virtual | ≈0 | ~38 KB |
| common interaction: scroll one new block | — | _ | _ | _ | 50 | — | — | ~38 KB per block |
| common interaction: change filter / search / sort (Apply) | — | _ | _ | _ | 50 | new count | — | ~38 KB |

Expected shape (from the machine this was built on, not yet run in the browser): `GetAll()` sorts 200,000 rows by Id on every call
(tens of ms); binding 20,000 rows costs a few hundred ms and tens of MB; binding 200,000 costs seconds and hundreds of MB; the
optimized path is a `Count` over 200,000 rows (a few ms), a `Search` of 50 rows (a few ms, includes the sort of the ≈44,400 filtered
rows), and a Σ over the filtered set (a few ms).

## What the app prints, and where

- Performance card (right of the Orders card): MODE · ROWS FETCHED FROM STORE · ROWS IN GRID (SERVER) · SERVER TIME (STOPWATCH) · Δ HEAP · EST. PAYLOAD IF EVERY ROW SHIPS —
  red after a naive load, green after an optimized load.
- Trace, per load: `• server OrderService.GetAll()  200,000 rows in _ ms`, `• server grid.DataSource = list  20,000 rows bound in _ ms total · Δ heap +_ MB · 20,000 DataGridViewRow objects on the server`
  vs `• server OrderService.Count(query) … → 44,4xx rows · _ ms`, `• server OrderService.Search(query) … skip=0 take=50 → 50 rows · _ ms · block 0`, `✓ ok optimized load  first paint _ ms · 50 of 44,4xx rows fetched`.
- Trace, after **Measure** (Timer, 700 ms between runs, naive 20k and optimized alternating, 5 runs each):
  `★ log performance notes  server-side Stopwatch, 200,000-order store, this machine:` followed by
  `★ log naive 20k  min _ · avg _ · max _ ms (5 runs) · 200,000 fetched · 20,000 in grid · ~9.6 MB` and
  `★ log optimized  min _ · avg _ · max _ ms (5 runs) · 50 fetched · 44,4xx in grid · ~38 KB`, then `• server File.WriteAllText  App_Data/performance-notes.md`.
- Naive grid footer: `DataSource = list · 20,000 of 200,000 rows bound in _ ms · Δ heap +_ MB · est. payload ~9.6 MB`.
- Optimized grid footer: `Σ Total (Open)  $… · 44,4xx rows · virtual · block 50 · summary _ ms on the server`.

## How to measure the real payload

1. Open DevTools → Network, filter `wisej.wx`.
2. Click **Naive load (20k) ✕** and note the response size of the request that follows; scroll the grid and watch the block requests.
3. Click **Optimized load ✓** and compare: one request carrying ≈50 rows; each scroll into a new block is one small request.
4. Record both next to the server numbers above. The lesson's rule: measure payload size *and* event frequency, then decide.

## Decisions recorded in migration-log.md

- Default filter **Open**, server-side search/sort/paging through `OrderQuery`, `VirtualMode` with `BlockSize = 50`.
- The naive bind stays in the sample as the measurable "before"; it is not part of the product.
- No live updates on the orders grid: nothing in the desk's workflow changes under the user's feet; if it did, push
  only the affected block with `Application.Update`, not the whole grid.
