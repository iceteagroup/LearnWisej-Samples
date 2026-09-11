# Background task notes — threading model, update rate, what is thread-safe

*Module 7 deliverable ("Thread-safety notes") · TicketOps Console · the CSV import*

The import of `Data/sample-tickets.csv` (600 rows, 4 of them deliberately bad) runs on a background
task started from the **▶ Start import** click. This note says which thread does what, how often the
browser is updated, which state two threads can touch, and how each piece is protected.

## 1. The two threads of one session

```
request thread                                   import task (Application.StartTask)
──────────────────────────────                   ─────────────────────────────────────────────
buttonStartImport_Click
  lock(_sync): _isRunning? → refuse
              _isRunning = true
              _importCancel = new CTS
  ShowRunStarted()        (controls, in context)
  BeginPush()             (StartPolling if no socket)
  Application.StartTask(…) ───────────────────▶  RunImportAsync
  return  ← round-trip closes                      IImportService.ImportAsync
                                                     per row: token? → parse → InsertAsync (repo lock)
                                                     onProgress(progress)  ─┐  worker thread
                                                                            │
buttonRefreshList_Click / Cancel                     OnImportProgress  ◀─────┘
  lock(_sync) snapshot                                 ShouldPush?
  ITicketService.CountAsync (repo lock)                Application.Update(this, () => {      ← in context
                                                         IsDisposed? → return
                                                         ApplyProgress
                                                       })  → ONE flush to the browser
                                                     …
                                                   finally:
                                                     lock(_sync): _isRunning=false, _resumeAtRow
                                                     PushFinalState → Application.Update(this, …)
                                                       ShowRunFinished, EndPush
```

- **Request thread** — every click and `Load`. Already in the session context; control changes travel
  back with the response, no `Update` needed.
- **Import task** — one per run, started with `Application.StartTask`, bound to this session. It runs
  `ImportService.ImportAsync`, which never sees a control. The only code on this thread that touches
  controls is inside `Application.Update(this, () => …)`.
- `this.Disposed` cancels the token; every callback checks `this.IsDisposed` before touching a control
  and catches `ObjectDisposedException` around the push. Closing the tab mid-import ends the task at its
  next row with nothing pushed.

## 2. Marshaling rule — the two-step

1. **Get onto the context.** From the worker, `Application.Update(this, callback)` (or
   `Application.RunInContext(this, callback)` when nothing needs to be pushed) restores the session for
   the duration of the callback. `this` (the Form) is the context object.
2. **Flush.** `Application.Update` pushes every pending change in one message. Without it the server
   objects change and the browser shows nothing until the next click — the final reset in
   `PushFinalState` is no exception: it runs inside `Application.Update(this, …)` so the page never stays
   stuck on "importing".

## 3. Update rate — bounded on purpose

| Event | Push? | Why |
|---|---|---|
| `Started` | yes | the first log line appears at once |
| every 25th imported row (`PushEveryRows`) | yes | ~4 pushes/s at 10 ms/row: smooth bar, no flood |
| every 10 % milestone (row 60, 120, …) | yes | the import-log line "Row 60 imported — 10%" |
| every skipped row | yes | a per-row error is worth seeing immediately |
| the last row | yes | the bar reaches 100 % before the final state |
| any other imported row | no | the model is exact on the server; the browser does not need it |
| final state (`finally`) | yes, always | buttons restored, outcome shown, `EndPolling` if it was on |

600 rows → about 40 pushes instead of 600.

**Polling fallback.** `Application.IsWebSocket` is `false` during `Load` (the socket opens after the
first response), so `StartPolling` must never be called there. `BeginPush()` runs in the click: with a
socket it does nothing; without one it calls `Application.StartPolling(1000)` and `EndPush()` calls
`Application.EndPolling()` from the final push. Set `"enableWebSocket": false` in `Default.json` to see
the fallback: the same pushes arrive about a second late, carried by the polls.

## 4. What is shared, and how it is protected

| State | Written by | Read by | Protection |
|---|---|---|---|
| `_isRunning` | click (start), task (`finally`) | Start click | `lock (_sync)` on every access |
| `_importCancel` (CTS) | click (new), task (`finally` → null + Dispose) | Cancel click, `Disposed` handler | `lock (_sync)`; a cancelled source is never reused; `ObjectDisposedException` caught around `Cancel()` |
| `_resumeAtRow` | task (`finally`) | Start click | `lock (_sync)` |
| `ImportProgress` objects | worker (created) | callback in context | immutable — no lock needed |
| `_file`, `_pushers`, `_polling` | request thread | callbacks running in context | request-thread only; callbacks run in context (serialized with requests) |
| `InMemoryTicketRepository._tickets` | task (`InsertAsync`) | Refresh click (`CountAsync`), `FinishAsync` | the repository's own `lock (_gate)` around every dictionary access, reads included; check-and-insert is one locked step |
| the run's counters and error list | task only | — | local variables in `ImportAsync` — the best lock is the one you never need |

Rules applied:

- Lock the **data**, briefly, on **every** access (reads too). Never hold `_sync` while touching a control
  or calling `Application.Update`.
- Two fields that must stay consistent (`_isRunning` + `_importCancel`) change under the same lock.
- Nothing is `static` except the constant tables in `TicketImportRules` and the seed data. Every session
  has its own repository, log, services, page and running import (`AppComposition`).

## 5. Per session vs shared

| Per session (one per browser tab) | Shared by all sessions |
|---|---|
| `ActivityLog`, `InMemoryTicketRepository` and its tickets, `TicketService`, `ImportService`, `ImportFileLocator`, `ImportPage`, the running task and its `CancellationTokenSource` | the CSV file on disk (read-only), `TicketImportRules` constants, the code |

Two tabs importing at once never share a dictionary; the repository lock exists for the two threads of
**one** session. In a real deployment the repository would be a database shared by every session, and
"duplicate id" would be enforced by a unique key — the `InsertAsync` returning `false` models exactly that.

## 6. Three outcomes, one consistent state

| Outcome | What stops it | Rows already written | Next action |
|---|---|---|---|
| Completed | end of file | all valid rows; bad rows listed | — (a second run skips every row as a duplicate) |
| Cancelled | `token.IsCancellationRequested` between two rows | kept | ▶ Resume import (row n) |
| Faulted | the repository throws on a write | kept; the failing row was **not** written | ▶ Resume import (row n) once the store answers |

A row that breaks a rule is none of these: it is skipped, logged (`⚠ Row 214 skipped — invalid due date
"2026-06-31"`) and the run continues.

## Evidence

- Start the import: the click returns immediately; the bar moves every ~250 ms.
- Click **↻ Refresh list** during the run: the count grows and the import log gets "List refreshed — 237
  tickets" while the bar keeps moving.
- The final state always arrives (green or amber) with the Start button enabled — even after Cancel.
