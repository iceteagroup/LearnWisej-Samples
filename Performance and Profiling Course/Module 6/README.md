# WisejPerfLab · Performance & Profiling · Module 6

Lab build for **Module 6 · Database, File I/O, Async and External Waits**. The two wait-heavy paths are
rewritten:

- **One statement per page.** The N+1 behind the grid is gone: `TicketQueryService` issues a single
  `AsNoTracking` projection with the customer name joined and only the displayed columns selected —
  **201 statements → 1**, and `Tickets/Search` **125 ms → 66 ms**. The filter and sort columns are now
  indexed.
- **The export never blocks.** `Application.StartTask` replaces `.Result`, one `StreamWriter` replaces
  5,001 file opens, and progress is reported every ten percent — **522 ms → 171 ms**, with the request
  thread free and the screen usable throughout.

Evidence: [`QueryTrace.md`](docs/QueryTrace.md) · [`ExportRework.md`](docs/ExportRework.md)

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Performance and Profiling Course/Module 6/WisejPerfLab"
dotnet run -c Release -f net10.0 --urls http://localhost:5806
```

Module 5 on 5805 is the before.

## What to click

| Action | What you should see |
|---|---|
| Tickets → **Search tickets** | `66 ms — within the 300 ms budget`, then `1 page(s) held   1 fetch(es)   1 statements` — one statement for the page, where Module 5 needed 201 |
| Tickets → **Export CSV** | the button greys out, **Cancel** lights up, the progress bar moves in ten steps, then `exported tickets-….csv in 171 ms` |
| click something else **while the export runs** | it works — the request thread was never held |
| **Cancel** during an export | `export cancelled after … ms — the partial file was left on disk` |
| **Break the database** → **Export CSV** | the button comes back, the banner says why, and the PERF end record is written with the failure |
| **Export CSV** twice quickly | the second click is dropped, and the status line says so |
| Rows **50000** → **Search tickets** → **Export CSV** | the whole open population exported, still without blocking |

## What changed since Module 5

```
Services/TicketQueryService.cs   one AsNoTracking projection query: joined, narrowed, ordered, paged
                                 + SearchAsync for callers that can await
Services/ExportService.cs        rewritten: takes TicketQueryService, one StreamWriter, ten progress
                                 reports, a CancellationToken
Pages/TicketGridPage.cs          the export runs in Application.StartTask; + Cancel; + IsDisposed checks
Pages/TicketGridPage.Designer.cs + btnCancelExport, the progress bar moved under the grid
Data/PerfLabContext.cs           + IX_Tickets_Status_UpdatedAt
Data/PerfLabDatabase.cs          + CREATE INDEX IF NOT EXISTS (EnsureCreated cannot add it to an
                                 existing file; a real app would add a migration)
```

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Database trace of the search; statements in order beside the UI actions; count, slowest, columns | [`QueryTrace.md`](docs/QueryTrace.md) |
| One `SearchTicketsAsync` with `AsNoTracking`, the customer name joined, only the grid columns, ordered by `UpdatedAt`, `Take(PageSize)` | `TicketQueryService.Query` / `SearchAsync` |
| Verify the filter and join columns are indexed before concluding the rest is inherent | `PerfLabContext.OnModelCreating`, `IX_Tickets_Status_UpdatedAt` |
| `.NET Async` and `File I/O` traces of the export; name the `.Result` and the per-line write | [`ExportRework.md`](docs/ExportRework.md) |
| Move generation into `Application.StartTask`, disable the button, stream the output, update every ten percent | `TicketGridPage.btnExport_Click`, `ReportExportProgress`, `ExportService.BuildAsync` |
| An export that throws re-enables the button and reports the failure | the `catch` / `finally` inside the `StartTask` lambda, then `FinishExport` |
| A cancelled export stops writing and says so | `btnCancelExport_Click`, `token.ThrowIfCancellationRequested()` |
| A session that navigates away mid-export is handled by checking `IsDisposed` | `ReportExportProgress` and `FinishExport` |
| Query count, slowest query and export wall clock, before and after | [`QueryTrace.md`](docs/QueryTrace.md), [`ExportRework.md`](docs/ExportRework.md) |
| Several concurrent sessions, and what was not tested | the last section of [`ExportRework.md`](docs/ExportRework.md) |

## Self-check answers

**How do you tell "slow because it is working" from "slow because it is waiting"?** Compare CPU time
with wall-clock time for the same scenario. Close together: the process is working — do less work. Far
apart: it is waiting — the fix is in the query, the file, the lock or the network. The export was the
second kind and the dashboard refresh was the first, which is why they got completely different fixes.

**Why is one statement better than 201 that are each fast?** Because the cost per statement belongs to
the environment, not to you. Here each lookup is 0.05 ms; against a database server across a socket it
is a round-trip, and 201 of them is most of a second — for the same rows.

**Why report progress every ten percent?** Every report is an `Application.Update`, and an update is a
payload the browser has to apply. Per-row progress makes the export slower than no progress at all.

**Why does `GetPage` stay synchronous?** Its caller is `CellValueNeeded`, a synchronous event. Making it
async would mean blocking on it there — the anti-pattern this module removed.

## Known simplifications

- SQLite runs **in process**, so the 201-statement N+1 cost far less here than it would against a
  database server. The statement count is real; the milliseconds are flattering.
- Concurrent exports from several sessions were not tested — see [`ExportRework.md`](docs/ExportRework.md).
