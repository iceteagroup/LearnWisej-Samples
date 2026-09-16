# WisejPerfLab · Performance & Profiling · Module 4

Lab build for **Module 4 · Memory, Allocations and Session Leaks**. Both halves of the memory problem
are fixed and measured:

- **Retention.** Fifty open/close rounds of the ticket detail form used to retain **+76 MB** and leave
  **50** handlers on a static event. They now retain **+1.1 MB** and leave **0**.
- **Allocation.** The grid rows are projected into `TicketRow` **once per search** instead of being
  rebuilt on every redraw.

Evidence: [`MemoryEvidence.md`](docs/MemoryEvidence.md) · [`DisposalChecklist.md`](docs/DisposalChecklist.md) ·
[`SessionMemoryBudget.md`](docs/SessionMemoryBudget.md)

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 4/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5804
```

For the **before** reading, run Module 3 on 5803 and use the same three buttons there — that folder
carries the same measuring instruments and the unfixed form.

## What to click

| Action | What you should see |
|---|---|
| **Memory snapshot A** | `snapshot A — heap 23.9 MB   bus subscribers 0   forms created 0 / disposed 0` |
| **Open and close the detail form ×50** | one `Session/OpenCloseDetail rows=50` record |
| **Snapshot B and compare** | `B - A: heap +1.1 MB   subscribers 0   forms outstanding 0` — green status |
| the same three in **Module 3** | `B - A: heap +76.0 MB   subscribers +50   forms outstanding 0` — red status |
| Tickets → **Search tickets**, then **Redraw** | `5,000 rows rebound in 1 ms (no query, no new rows)` — the redraw allocates nothing |
| Tickets → double-click a row, then close the form by its **X** | `forms disposed` goes up by one and the subscriber count stays where it was — the X and the Close button take the same path |
| Repeat the ×50 round a second time | the same +1 MB, not +2 MB: a one-off cost does not grow, retention does |

The line worth staring at is `forms created 50 / disposed 50`, which is true **before and after**.
Disposal always ran. What changed is that nothing holds the disposed forms any more.

## What changed since Module 3

```
Diagnostics/MemoryProbe.cs     snapshots: heap after a collection, bus subscribers, forms created/disposed
Models/TicketRow.cs            new — the row the grid binds, display strings built once
Forms/TicketDetailForm.cs      + Dispose(bool): unsubscribe, stop and dispose the timer, null the
                               DataSource, dispose the image, drop the buffer, dispose components
                               + the timer callback checks IsDisposed
                               + static Created / DisposedCount counters for the readings
Forms/TicketDetailForm.Designer.cs   its Dispose(bool) moved into the .cs (a partial class has one)
Pages/TicketGridPage.cs        the search projects TicketRow once and keeps it; Redraw rebinds
MainPage.cs / .Designer.cs     + the three snapshot buttons and the memory readout
```

`Module 3` also carries `MemoryProbe`, the form counters and the three buttons — with the **unfixed**
form — so the before reading can be taken on the code as it stood. That is the first half of this
module's lab.

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Run the search under .NET Object Allocation; record bytes, top type, allocating path | the `Tickets/Search` stages; the projection is now one pass in `TicketGridPage.ProjectRows` |
| Project the result once into a lightweight `TicketRow` with its display strings already built | `Models/TicketRow.cs`, `TicketGridPage.ProjectRows` |
| Memory Usage: warm up, snapshot A, 50 × open/close, idle, snapshot B | **Memory snapshot A** → **Open and close the detail form ×50** → **Snapshot B and compare** |
| Compare the snapshots; capture the path to root for two instances | [`MemoryEvidence.md`](docs/MemoryEvidence.md) — the static `GlobalTicketBus.TicketChanged` and the running `System.Timers.Timer` |
| `Dispose(bool disposing)`: unsubscribe, stop and dispose the timer, null the DataSource, dispose the image | `Forms/TicketDetailForm.cs` |
| The timer callback checks `IsDisposed` before touching controls | `OnRefreshTimerElapsed` |
| Closing while a refresh is in flight does not throw | `_released` + the `ObjectDisposedException` catch |
| The window X follows the same disposal path as the Close button | both end in `Dispose(bool)`; `btnClose_Click` only calls `Close()` |
| Rerun the scenario and show the count back at baseline without forcing collections to massage it | one collection per snapshot, the same on both sides; [`MemoryEvidence.md`](docs/MemoryEvidence.md) |
| Per-session memory budget in measured MB, and the disposal checklist | [`SessionMemoryBudget.md`](docs/SessionMemoryBudget.md), [`DisposalChecklist.md`](docs/DisposalChecklist.md) |

## Self-check answers

**Why is "we call Dispose" not an answer to "does it leak"?** Because disposal is not collection.
Both runs disposed all fifty forms; one of them kept all fifty in the heap, because a static event and a
running timer still referenced them. Collection happens when nothing references the object.

**Which two roots, exactly?** A `static` event (`GlobalTicketBus.TicketChanged`) whose invocation list is
rooted for the process, and a `System.Timers.Timer` that was started and never stopped — rooted by the
runtime's timer queue, holding the form through its `Elapsed` handler.

**Why measure per session rather than per request?** Because the memory stays between clicks. 25 MB per
idle session × 150 sessions is the number that decides how many users an instance can hold; a
per-request figure would have been the same in both runs above.

**Why not just force a few collections?** A rooted object survives all of them. Forcing collections
until the number looks acceptable measures the finalizer queue, not retention. One collection before a
reading is fair — it is what a Memory Usage snapshot does — and the honest test is whether a **second**
round of the same scenario adds the same amount again.

**Was the allocation fix worth it if the redraw was only 3 ms?** For latency, barely. For throughput,
yes: fifty redraws used to leave a quarter of a million dead objects for the collector to walk, and
under concurrency that is CPU spent on garbage collection instead of on requests. It is a throughput
argument, and Module 7 is where throughput becomes a capacity number.

## Known simplifications

- `MemoryProbe` reads the **managed heap** only (`GC.GetTotalMemory`). The Wisej.NET client state, the
  SQLite page cache and the Kestrel buffers live outside it. For a capacity model, use the process
  working set as well — see [`SessionMemoryBudget.md`](docs/SessionMemoryBudget.md).
- The counters say *that* something is retained. Only **Paths to Root** in a Memory Usage snapshot says
  *why*; the doc walks through collecting it.
