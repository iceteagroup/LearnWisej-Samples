# Grid performance notes — the migration log entry for Module 5

**Deliverables 4–6 · "Test with a large row count. Measure initial load and common interaction time. Record
performance notes in the migration log."**

## Test data

`Domain/Services/OrderStore.cs` seeds **200,000 orders** once per process (deterministic, ~60 MB managed heap, one line
each): the five walkthrough orders first, then eight years of generated history. Every browser session queries the same
store, like a database. The seed time is printed in the trace on first load.

## How the console measures

* **Server ms** — a `Stopwatch` around the service call (`LargeOrderRepository.Query/Summarize/GetAll`); for the naive path
  it also includes building the grid rows.
* **≈ Payload** — estimated from the text of every cell that reaches the browser plus ~96 bytes of framing per row. It is an
  estimate, not a socket capture; use the browser's DevTools (Network → WS frames) for the exact number.
* **Rows** — how many rows left the server for that action.

## Results (this machine, Wisej-4 4.1.0, .NET 10, Kestrel, one session)

| Scenario | Rows fetched | Server ms | ≈ Payload | Note |
|---|---|---|---|---|
| Naive port · `GetOrders()` bound to the grid, 20,000-row grid limit | 20,000 (of 200,000 cloned + sorted) | 77 (clone + sort) + 164 (grid rows) | 2.7 MB | the desktop had no limit — at 200,000 rows the same path is ≈ 27 MB and the browser tab, not the server, is what gives up |
| Optimized · filter *Open* + VirtualMode, first paint | 50 (of 80,318 matching) | 31 (count + Σ, first sort) | 7 KB | count + Σ + one block |
| Optimized · one viewport jump (block fetch) | 50 | 0.0–0.8 | 7 KB | ordered set memoized after the first fetch |
| Optimized · filter changed (*north*) | 50 (of 10,063 matching) | 16 | 7 KB | new count + Σ, cache emptied |
| Average of 10 interactions (**Measure 10 interactions**) | 50 | 14.2 | 7 KB | the number for the migration log |

Measured 2026-09-10 on the development machine (one session, Kestrel, Debug build). The walkthrough's numbers for the
same two screens: naive **200,000 rows · 8.4 s · 96 MB** versus **50 virtual rows · 0.2 s · 38 KB**. Re-run **Measure 10
interactions** after any change to the query or the store and replace the last row.

## What was done about it

1. Default filter (*Open*) and a search box: the query the users actually run, not "show me everything".
2. Sort in the query (`OrderSort`), not in the browser.
3. `VirtualMode` + `RowCount` + `CellValueNeeded` with a 50-row block cache, memoized per filter+sort until the next write.
4. Count and Σ computed on the server (`Summarize`), shown in the footer.
5. The naive path is kept, on purpose, one button away — so the comparison can be shown to anyone who asks why.

## Self-check

* **How does the user really find a record — and what filter proves it?** By status first (Open is the working set), then
  customer / order / PO text. The default filter shrinks 200,000 rows to the ~40 % that are Open, the search box to a
  handful; the Performance card shows the same interaction costing 50 rows instead of 200,000.
* **Why does validation belong on the server, not in client script?** See `ValidationRules.md`: the browser can be
  bypassed, the batch and the API never see a form, and the data lives on the server.
