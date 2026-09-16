# The export — from a blocked thread to a background workflow

## What the traces showed

Two things, and the lab asks for both to be named precisely.

**1. `_exportService.BuildAsync(...).Result` in `btnExport_Click`.** A blocking wait on an asynchronous
method, on the request thread. A **.NET Async** trace shows the await that never yields: the task is
running, the thread is doing nothing, and it is held for the whole export. A thread held is a thread no
other session can use — with one user it looks free, and it is the first thing to fall over under
concurrency. It also means the progress callback fired into a screen the browser could not see: nothing
reaches the client until the handler returns, so the progress bar jumped from empty to gone.

**2. `File.AppendAllText` per line in the export writer.** A **File I/O** trace shows one open, one
write and one close per row — 5,001 file handles for a 5,000-row export. Correct output, and a solid
wall of writes in the trace.

## What replaced them

```csharp
Application.StartTask(async () =>
{
    try     { exported = await _export.BuildAsync(filter, ReportExportProgress, token); }
    catch (OperationCanceledException) { cancelled = true; }
    catch (Exception ex)               { failure = ex.Message; scope.Fail(ex); }
    finally { scope.Dispose(); FinishExport(exported, failure, cancelled, scope.ElapsedMs); }
});
```

- **`Application.StartTask`** — the click starts the work and returns. The request thread is free, the
  rest of the screen keeps working, and the task keeps the session context, so it may touch controls and
  call `Application.Update`.
- **Every exception is caught inside the lambda.** One escaping the body of a `StartTask` shows the red
  Wisej.NET application-error dialog, even when the awaited task is handled elsewhere.
- **One `StreamWriter`**, opened once, `await writer.WriteLineAsync(...)` per row, closed by
  `await using`. The file is streamed, so exporting a million rows costs the same memory as a thousand.
- **The rows come from the single projection query** (`TicketQueryService.SearchAsync`) — no entities,
  no long text columns.
- **Progress every ten percent**, not per row. Each report is an `Application.Update`, which is an update
  the browser has to apply: five thousand of them would have cost more than the export.

## Measured

| | Before (Module 5) | After (Module 6) |
|---|---:|---:|
| `Tickets/Export`, 5,000 rows | **522 ms** | **171 ms** |
| Request thread held | for the whole export | **not at all** |
| File operations | 5,001 open/write/close | 1 open, 5,000 buffered writes, 1 close |
| Progress updates reaching the browser | 0 (the handler had not returned) | 10 |
| Statements | 1 + 5,000 (the old search path) | 1 |
| Budget (400 ms) | over | within |

## The three failure paths

- **It throws.** The catch records the failure on the probe scope, `FinishExport` re-enables the button,
  the status goes red and the banner says why. The PERF end record is still written, with the time spent
  before the failure.
- **It is cancelled.** **Cancel** trips the `CancellationTokenSource`; the writer loop checks the token
  every row, stops writing and says so. The partial file is left on disk deliberately — deleting a file
  a user may have been watching is a decision, not a default.
- **The session navigates away.** `ReportExportProgress` and `FinishExport` both check `IsDisposed`
  before touching a control. Without that check, an export finishing after a page has gone throws on a
  background thread where nobody sees it.

## Concurrency — what was and was not tested

The export was run repeatedly in one session, and a second export while one is running is dropped with
*"an export is already running — the second click was dropped"*.

**Not tested:** several sessions exporting at once. Two browser tabs would be two sessions and two
background tasks, and the interesting number is how many concurrent exports the SQLite file and the
thread pool tolerate before the interactive screens slow down. That test belongs with the capacity work
in Module 7, and the honest thing to record here is that it has not been done — a remaining risk, not a
finding.

## Why `GetPage` is still synchronous

`CellValueNeeded` is a synchronous event: the grid asks for a cell and expects a value. Making the query
`async` would mean blocking on it inside the handler — the exact anti-pattern this module removed from
the export. So there are two entry points to the same statement: `GetPage` (synchronous, for the grid,
answering from the page cache so it is called once per page and not once per cell) and `SearchAsync`
(awaited, for the export, which runs on a background task and can await properly).
