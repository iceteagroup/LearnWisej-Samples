# TicketOpsLive · Real-Time Apps with Server Push · Module 5

Local lab build for **Module 5 · Live Ticket Board: Updating Data Without Rebuilding the Screen**: a live
`DataGridView` (`ticketsGrid`) bound through a `BindingSource` (`ticketsBindingSource`) to a `BindingList<Ticket>`,
fed by a simulated ticket service. New tickets arrive at the top, existing tickets change in place, a temporary
"updated" marker (●) fades after three seconds, and the row the user selected survives every event.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 5/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5305
```

Then open <http://localhost:5305>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

## What to try

Click a row first — every update below keeps that row selected.

| Control | What you should see |
|---|---|
| New ticket every second (20 s) | one new row per second at the top, with a ● marker, without any click; the selection does not move |
| Stop feed | the feed ends before the next ticket; the start button comes back |
| Randomize statuses | three rows change in place; no row moves |
| (your selected row changes) | the amber warning "Ticket #n changed on the server while you have it open …" — nothing is reloaded; **Dismiss** closes it |
| Escalated only | the grid shows only escalated tickets and keeps updating while the feed runs |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: the rows still arrive (about a
second late) because polling is requested while the feed runs.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| A `DataGridView` named `ticketsGrid` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs): `AutoGenerateColumns = false`, full-row select, seven columns |
| `Ticket` model and `TicketStatus` enum | [`Models/Ticket.cs`](TicketOpsLive/Models/Ticket.cs), [`Models/TicketStatus.cs`](TicketOpsLive/Models/TicketStatus.cs) |
| `BindingList<Ticket>` through a `BindingSource` | `MainPage_Load` → `ticketsBindingSource.DataSource = _tickets` (the grid is bound once, in the Designer) |
| A new ticket event every second for 20 seconds | `newTicketsButton_Click` → [`Services/TicketSimulator.cs`](TicketOpsLive/Services/TicketSimulator.cs) `StartNewTickets(20, 1000)` |
| A button that randomly changes ticket statuses | `changeStatusButton_Click` → `RandomizeStatuses(3)` |
| Preserve the selected row | `ApplyTicketEvent` captures the selected id, `RestoreSelection(id)` puts it back |
| Show `UpdatedAt` in a readable format | `colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss"` |
| Walkthrough: "updated" marker and the conflict rule | `markerTimer_Tick`, `ShowTicketChangeWarning` |
| Extension: an Escalated filter that keeps working live | `escalatedOnlyCheckBox_CheckedChanged` + `ApplyFilter()` |

## Self-check

- **New tickets appear without rebinding** — `_allTickets.Insert(0, …)` then `ResetBindings(false)`; `ticketsGrid.DataSource` is assigned once.
- **Existing tickets update in place** — only `Status`, `Owner` and `UpdatedAt` change on the existing instance.
- **The selected row is preserved** — captured by id before the change, restored by id after it.
- **The update rate stays controlled** — one ticket per second from the feed; Randomize changes three rows inside one request (no push); `markerTimer` resets the bindings only when a marker expired.
- **Why is the bound list session-owned?** It is part of one user's UI state (their filter, their order, their
  current row). A static list would let one user's filter empty another user's grid, add every session's threads as
  writers of the same `BindingList`, and keep closed sessions alive. Each `MainPage` owns its list and its feed; the
  page stops the feed and unsubscribes when it is disposed.
