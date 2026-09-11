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

A file whose header lacks the required columns is refused by `ImportService.OpenAsync` before any task
starts: the banner lists the missing columns and **▶ Start import** stays disabled.

## Normal run

1. Open the app (`http://localhost:5107`). The file line reads `sample-tickets.csv · 600 rows`, the count
   reads `6 tickets in the repository`, status **● Ready to import sample-tickets.csv.**
2. Click **▶ Start import**. Status **● Importing… n of 600 rows — UI remains responsive.**; the bar and the
   percent advance; the import log shows `Importing sample-tickets.csv (600 rows)…`, then
   `Row 60 imported — 10%` … and the four `⚠ … skipped` lines as they happen.
3. While it runs, click **↻ Refresh list**: the count grows, a `✔ List refreshed — n tickets` line appears,
   the bar keeps moving. The screen was never blocked.
4. About 6–7 seconds after the start: `✔ Import complete — 596 imported · 4 skipped.`, bar at 100 %,
   count `602 tickets in the repository`, Start enabled again.

## Cancel

1. Click **▶ Start import**, then **⏹ Cancel** while it runs.
2. Cancel is disabled, status **● cancelling — finishing the current row**, log `⏹ Cancel requested…`.
3. Within one row (≤ 10 ms) the task stops **between two rows**: log `⏹ Import canceled by user — n of 600
   rows imported, resume at row m.`, amber banner *Import cancelled. The rows already imported were kept;
   resume to continue.*, the Start button now reads **▶ Resume import (row m)**.
4. Click **▶ Resume import (row m)**: the run continues from row m to 600. Nothing is written twice;
   nothing was half-written.

## The data store fails during the import

The task's next `InsertAsync` throws. The run **stops cleanly before that row**: the exception goes to the
log, the screen shows only the red banner *The import stopped because the data store is unavailable.
Recover it, then resume.*, the import log reads `✖ Import stopped at row n …`, and Start reads
**▶ Resume import (row n)**. Once the store answers, Resume continues from the row that failed.

## Starting twice

A second start while a run is active — even one that arrives before the disabled button reaches the
browser — is refused under the lock (`TryBeginRun`), with the banner *An import is already running…*.

## Running the same file again

After a complete run every id already exists, so a second **▶ Start import** skips all 600 rows as
duplicates (`0 imported · 600 skipped`). That is the rule working, not a bug.

## Closing the tab mid-import

The page's `Disposed` handler cancels the token. The task stops at its next row; every callback checks
`IsDisposed` and pushes nothing. No error, no orphan thread past the current row.

## No WebSocket

Set `"enableWebSocket": false` in `Default.json` and restart. On **▶ Start import** the page calls
`Application.StartPolling(1000)`, the browser polls once a second, the same pushes arrive about a second
late, and the final push calls `Application.EndPolling()`.
