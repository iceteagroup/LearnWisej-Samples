# Import runbook — operating the background CSV import

*Module 7 deliverable · TicketOps Console · what an operator does, what they see, what to do when it goes wrong*

## The file

`Data/sample-tickets.csv` — header `TicketId,Title,Priority,Assignee,DueDate,HoursLogged`, 600 data rows.
Ticket ids run 7001–7600. Four rows are deliberately bad and are **skipped, not fatal**:

| Row | Problem | Rule that catches it | Log line |
|---|---|---|---|
| 58 | empty title | `TicketImportRules.TryParseRow` — title is required | `⚠ Row 58 skipped — title is required` |
| 214 | due date `2026-06-31` | due date must parse as `yyyy-MM-dd` (June has 30 days) | `⚠ Row 214 skipped — invalid due date "2026-06-31"` |
| 287 | assignee `kbo` | assignee must be in the known directory (ana, jle, mrt, pko, sva) | `⚠ Row 287 skipped — unknown assignee "kbo"` |
| 412 | ticket id 7411 again (row 411 has it) | `ITicketRepository.InsertAsync` returns false | `⚠ Row 412 skipped — duplicate ticket id #7411` |

A clean full run ends with **596 imported · 4 skipped** and 602 tickets in the repository (6 seeded + 596).

`Data/sample-tickets-wrong-header.csv` has the header `Id,Summary,Prio,Owner,Due,Hours` and exists only to
show the validation path.

## Normal run

1. Open the app (`http://localhost:5107`). The file line reads `sample-tickets.csv · 600 rows · …`,
   the count reads `6 tickets in the repository`, status **● ready**.
2. Click **▶ Start import**. Status **● importing n/600 — UI stays responsive**; the bar and the
   percent advance; the import log shows `Importing sample-tickets.csv (600 rows)…`, then
   `Row 60 imported — 10%` … and the four `⚠ … skipped` lines as they happen.
3. While it runs, click **↻ Refresh list**: the count grows, a `✔ List refreshed — n tickets (request
   thread; the import kept running)` line appears, the bar keeps moving. The screen was never blocked.
4. About 6–7 seconds after the start: `✔ Import complete — 596 imported · 4 skipped.`, bar at 100 %,
   status **● 596 imported · 4 skipped**, count `602 tickets in the repository`, Start enabled again.

## Cancel

1. Click **▶ Start import**, then **⏹ Cancel** while it runs.
2. Cancel is disabled, status **● cancelling — finishing the current row**, log `⏹ Cancel requested…`.
3. Within one row (≤ 10 ms) the task stops **between two rows**: log `⏹ Import cancelled by user — n of 600
   rows imported, resume at row m.`, amber banner *Import cancelled. The rows already imported were kept;
   resume to continue.*, the Start button now reads **▶ Resume import (row m)**.
4. Click **▶ Resume import (row m)**: the run continues from row m to 600 and ends with a completion line
   for the remaining rows. Nothing is written twice; nothing was half-written.

## Data store outage during the import (error path + recovery)

1. Click **▶ Start import**, then **Simulate data outage** while it runs (the button turns into
   **Recover the data store**).
2. The task's next `InsertAsync` throws. The run **stops cleanly before that row**:
   - trace: `✖ [DATA] InMemoryTicketRepository — outage: INSERT INTO Tickets (Id=7233) failed — timeout
     connecting to sql01:1433 …` and `✖ [SVC] ImportService.ImportAsync — stopped at row 233: repository
     unavailable … [DataOutageException]` — the driver message stays here;
   - screen: red banner *The import stopped because the data store is unavailable. Recover it, then resume.*,
     log `✖ Import stopped at row 233 — the data store is unavailable (n rows imported in this run)`, status
     **● stopped — data store unavailable**, Start reads **▶ Resume import (row 233)**.
3. Click **Recover the data store** (status **● data store recovered**), then **▶ Resume import (row 233)**.
   The run continues from the row that failed and completes.
4. If the outage is still on when you click Resume, the very first write fails the same way — the state is
   unchanged and the Resume row is the same. Nothing is lost by retrying.

While the store is down, **↻ Refresh list** also fails: the request-thread path shows the same safe
message (*The action could not be completed. Check the log for details.*) and a red toast, with the
`✖ [DATA]` detail in the trace.

## Wrong file (validation path)

Click **Import wrong-header file**: `⚠ [SVC] ImportService.OpenAsync — rejected: header "Id,Summary,…" lacks
TicketId, Title, Priority, Assignee, DueDate, HoursLogged`, orange banner *The file is missing required
columns: TicketId, Title, …*, status **● rejected**. No task starts, nothing is written, the sample file
stays selected.

## Starting twice

**Start import twice** issues two starts in one request. The first starts the task, the second is refused
under the lock: `⚠ [UI] ImportPage.TryBeginRun — buttonStartImport_Click: refused — _isRunning is true`,
banner *An import is already running…*. The same guard refuses **Reset sample data** during a run.

## Running the same file again

After a complete run every id already exists, so a second **▶ Start import** skips all 600 rows as
duplicates (`0 imported · 600 skipped`). That is the rule working, not a bug. Click **Reset sample data**
first to re-seed the store and move the resume row back to 1.

## Closing the tab mid-import

The page's `Disposed` handler cancels the token. The task stops at its next row; every callback checks
`IsDisposed` and pushes nothing. No error, no orphan thread past the current row.

## No WebSocket

Set `"enableWebSocket": false` in `Default.json` and restart. `IsWebSocket=false` appears in the SERVER
STATE line; on **▶ Start import** the trace shows `[SESSION] ImportPage.BeginPush — no WebSocket when the
task started → Application.StartPolling(1000)`, the browser polls once a second (console: `Wisej: Poll
request.`), the same pushes arrive about a second late, and the final push logs `Application.EndPolling()`.

## Evidence

Each path above names the exact trace lines, banner texts and button states the running app shows; the
reviewer script in the README walks them in order.
