# Session memory budget

The memory half of `Budget.md`, now that it can be measured: **25 MB retained per idle session.**

## What one session costs, measured

All readings taken with **Memory snapshot A/B**, one browser tab, warm app, after a full collection.

| State | Heap | Notes |
|---|---:|---|
| Process with no session | ~18 MB | the host, EF Core, the SQLite connection pool |
| One idle session, nothing clicked | ~23.6 MB | the shell: page, three tab pages, the PERF buffer |
| After one `Tickets/Search` of 5,000 rows | ~29 MB | 5,000 `TicketRow` objects and their strings, held while the grid shows them |
| After 50 open/close of the detail form — **before** the fix | 99.6 MB | +76 MB that never comes back |
| After 50 open/close of the detail form — **after** the fix | 24.9 MB | +1.1 MB, and flat on a second round |

So a session that has done a search and is now sitting idle retains roughly **6 MB of its own** on top
of the process baseline, and the fixed detail form adds nothing lasting. That is inside the 25 MB
threshold with room for the screens Modules 5 and 6 will still touch.

## Why the threshold is what it is

25 MB × 150 concurrent sessions ≈ 3.7 GB of managed heap, which a single instance with 8 GB can hold
with headroom for the garbage collector, the page cache and the operating system. At the pre-fix
1.5 MB per detail-form visit, a support agent who opens thirty tickets in a morning would have cost
45 MB **on top** of everything else — and eight such agents would have filled the instance on their own.

That is the whole reason the memory budget is per **session** and not per request: the number multiplies
by concurrency, and nothing gives it back until the session ends.

Module 7 turns this threshold into the `maxSessions` value in `HealthCheck.json`, together with the CPU
measurements from Modules 3 and 6.

## How to re-measure it

1. Start the app, open **one** browser tab, warm it up.
2. **Memory snapshot A** — this is the idle cost of one session.
3. Do the work you want to price: a search, fifty detail forms, a tree load.
4. Return the app to idle (close what you opened) and take **Snapshot B and compare**.
5. Repeat the same round a second time. A leak grows on every round; a one-off cost does not.

The second round is the check that matters. A single A→B difference can be a cache filling, a buffer
being pooled, or the JIT: all of those stop growing. Anything that adds the same amount on every round
is retention, and the profiler's **Paths to Root** will name it.

## What is not in this number

- **Unmanaged memory.** `GC.GetTotalMemory` counts the managed heap only. The Wisej.NET client state,
  the SQLite page cache and the Kestrel buffers are outside it. For a capacity model, use the process
  working set from Task Manager or a production counter, and treat the managed heap as the part you
  control directly.
- **Other sessions.** Everything here is one tab. Two tabs are two sessions with two of everything —
  which is the point of measuring per session.
- **The database file.** 60 MB on disk, and it is not per session.
