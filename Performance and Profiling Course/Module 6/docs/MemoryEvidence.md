# Memory evidence

Two snapshots, one scenario in between, and the comparison. The scenario is the one from the lab:
**open and close `TicketDetailForm` fifty times**, then return the app to a genuine idle point.

Both readings were taken with the same three buttons — **Memory snapshot A**, **Open and close the
detail form ×50**, **Snapshot B and compare** — in the Module 3 folder (before the fix) and in this
folder (after it). Each snapshot runs one full blocking collection first, which is what a Visual Studio
**Memory Usage** snapshot does too: the question is what is *retained*, not what has been allocated
recently.

## Before — Module 3

```
--  snapshot A — heap 23.6 MB   bus subscribers 0    forms created 0 / disposed 0
PERF start  Session/OpenCloseDetail rows=50
PERF end    Session/OpenCloseDetail elapsedMs=143 rows=50
--  snapshot B — heap 99.6 MB   bus subscribers 50   forms created 50 / disposed 50
--  B - A: heap +76.0 MB   subscribers +50   forms outstanding 0
```

## After — Module 4

```
--  snapshot A — heap 23.9 MB   bus subscribers 0    forms created 0 / disposed 0
PERF start  Session/OpenCloseDetail rows=50
PERF end    Session/OpenCloseDetail rows=50
--  snapshot B — heap 24.9 MB   bus subscribers 0    forms created 50 / disposed 50
--  B - A: heap +1.1 MB   subscribers 0   forms outstanding 0
```

| | Before | After |
|---|---:|---:|
| Retained after 50 open/close | **+76.0 MB** | **+1.1 MB** |
| Per form | ~1.5 MB | ~22 KB |
| Handlers left on `GlobalTicketBus.TicketChanged` | **50** | **0** |
| `System.Timers.Timer` instances still running | 50 | 0 |
| Forms created / disposed | 50 / 50 | 50 / 50 |

## The line that matters most

**Forms created 50 / disposed 50 — in both runs.**

`Dispose` ran every time, before the fix as well. Closing a form disposes it, and the disposed form was
still in the heap an hour later, holding its binding source, its 25 comment rows, a 600×400 `Bitmap`
and a 512 KB buffer. Disposal is not collection. An object is collected when **nothing references it**,
and two things still did:

1. `GlobalTicketBus.TicketChanged += OnTicketChanged` — a **static** event. Its invocation list is
   rooted for the life of the process, every entry holds the delegate, every delegate holds the form.
   The subscriber count is the leak with the guesswork removed: 0 → 50 → 0.
2. `new System.Timers.Timer(1000)` that was started and never stopped. A running timer is rooted by the
   runtime's timer queue, its `Elapsed` handler captures the form, and it goes on firing into a form
   nobody can see.

That is why the memory half of the budget is measured with snapshots and not with a stopwatch, and why
"we call Dispose" is not an answer to "does it leak".

## How to find the same thing in Visual Studio

The counters above say *that* something is retained. The profiler says *why*, and it is worth doing
once on this app so the shape is familiar:

1. Release, warm, **Debug > Performance Profiler** (Alt+F2) → **Memory Usage** → Start.
2. **Take snapshot** — this is A.
3. Open and close the ticket detail form fifty times (the lab button does exactly this).
4. Return to idle, **Take snapshot** — this is B.
5. Click the object-count difference between the two snapshots, and sort by difference.
   `WisejPerfLab.Forms.TicketDetailForm` will be **+50**.
6. Select the type, pick an instance, and open **Paths to Root**. Before the fix you will find two
   chains: one through `GlobalTicketBus.TicketChanged` (a static field, the end of every leak hunt),
   and one through the timer queue into `OnRefreshTimerElapsed`.
7. After the fix, the same comparison shows the type with no surviving instances and no path to root.

Do not force collections until the numbers look better. A rooted object survives every collection you
can ask for; if the count goes down only after ten forced collections, what you measured was the
finalizer queue, not the leak.

## Allocation, the other half

The search scenario was also measured with **.NET Object Allocation** in mind. The change in this
module is that the grid rows are projected **once per search** instead of on every redraw:

| | Before (Module 3) | After (Module 4) |
|---|---:|---:|
| `Tickets/Redraw`, 5,000 rows | rebuilt 5,000 row objects and ~25,000 strings, 3–5 ms | rebinds the existing list, ~1 ms, **0 new rows** |
| Row objects alive after a search | 5,000 (plus the 5,000 entities they were built from) | 5,000 `TicketRow`, entities dropped when the search returns |

The redraw was never slow — it was 3 ms — and that is the honest reading: this half of the module is
about **allocation pressure**, not wall clock. Fifty redraws used to leave a quarter of a million dead
row objects for the garbage collector to walk; they now leave none. What it buys is fewer collections
under load, which is a throughput argument, not a latency one — and Module 7 is where throughput is
turned into a capacity number.

The search itself is still 414 ms and still over budget: it still loads full entities and still issues
one statement per row. Those are Modules 5 and 6.
