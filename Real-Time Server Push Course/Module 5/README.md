# TicketOpsLive · Real-Time Apps with Server Push · Module 5

Local lab build for **Module 5 · Live Ticket Board: Updating Data Without Rebuilding the Screen**. It is the
lab's live `DataGridView` (`ticketsGrid`) bound through a `BindingSource` (`ticketsBindingSource`) to a
`BindingList<Ticket>`, fed by a simulated ticket service: **new tickets arrive at the top**, **existing tickets change
in place**, a temporary *updated* marker fades after three seconds, and the row the user selected survives every event.

Next to it, the two things that make the point: the **anti-pattern** button that rebinds the whole grid (and loses the
selection, the scroll and the markers), and the **corrupt event** button that proves the update path validates before
it touches the bound list.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 5/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5305
```

Then open <http://localhost:5305>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## What to try

Click a row first (say **#4822**) — everything below is about protecting that row.

| Button | Path | What you should see |
|---|---|---|
| (page load) | — | five seeded tickets, newest on top; trace `• server bind once … AutoGenerateColumns = false, 7 columns` and `• server seed 5 tickets added to the bound list · ResetBindings(false) · the grid was never rebound` |
| click any row | selection | `selectedLabel` → `selected: #4822 · Waiting · K. Brandt · updated 14:02:37`; trace `← request ticketsGrid_SelectionChanged …` |
| **New ticket every second ×20** | progress · out-of-bound push | one new row per second **without any click**, each with a `●` marker; trace `→ push Application.Update(this, …) #4823 added · selection kept #4822 · ResetBindings(false)`; SERVER STATE counts `events applied` and `pushes` |
| **New ticket every second ×20** (again, while running) | guard | the button is disabled; if a click gets through, `• server newTicketsButton_Click refused — the feed is already running` — no second loop |
| **■ Stop feed** | cancellation | `_feed.Stop()`; within one second `→ push … feed stopped — stopped by operator after 7 of 20 tickets (finally block of StartNewTickets)`, the start button comes back |
| **Randomize statuses** | success · in-request | three existing rows change in place; **no row moves**; trace `• server in-request update #4820 updated · selection kept #4822 · ResetBindings(false)` ×3 — these changes ride back on the click's own response, no push |
| **Randomize statuses** until it picks your selected row | conflict | the amber strip appears: *"⚠ Ticket #4822 changed while selected — status Escalated … nothing was reloaded and your selection did not move"* + **Dismiss**; trace `• server conflict … warn, don't reload` |
| **Escalated only** (checkbox) | extension challenge | the grid shows only escalated tickets and **keeps updating** while the feed runs; trace `← request escalatedOnlyCheckBox_CheckedChanged filter ON (Escalated only) · 3 of 25 rows shown · selection kept #4822` (or `selection #4822 filtered out (row not shown)`) |
| **Rebind whole grid (anti-pattern)** | anti-pattern | the same rows come back — and `selectedLabel` says *"selected: none"*, every `●` is gone; trace `• server rebind (anti-pattern) 25 rows rebuilt · selection #4822 → none · scroll, sort and the “updated” markers reset` + an amber banner |
| **Apply corrupt event** | failure | trace `• server ApplyTicketEvent REJECTED — Ticket is null · the bound list was not touched, the grid is intact (25 rows)`, a red banner, the server console gets the detail; the grid is unchanged. Click again for the second shape (`Ticket.Id = 0`) |
| (wait 3 s after any event) | row cue | the `●` markers fade; trace `• server markerTimer_Tick 2 “updated” marker(s) older than 3 s cleared · ResetBindings(false) · selection #4822 kept` |
| **Clear trace** | — | empties the right-hand list |

**Two tabs.** Open <http://localhost:5305> twice: each tab has its own board, its own feed and its own filter, and
SERVER STATE shows a different `ClientId`. That is what "the bound list is session-owned" means.

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart: the feed logs
`• server Application.StartPolling(1000) no WebSocket when the task started → fallback polling ON until the feed ends`,
the browser console shows one `Wisej: Poll request.` per second, the rows still arrive (≈1 s late) and the feed's
`finally` logs `Application.EndPolling() … fallback polling OFF`.

The right-hand card is the live push trace: `→ push` lines are pushes made by the feed task (`Application.Update`),
`← request` lines are requests the browser sent, `• server` lines are server decisions.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Add a `DataGridView` named `ticketsGrid` | [`MainPage.Designer.cs`](TicketOpsLive/MainPage.Designer.cs) — `ticketsGrid` block: `AutoGenerateColumns = false`, `SelectionMode = FullRowSelect`, `MultiSelect = false`, `ReadOnly = true`, seven declared columns |
| Create the `Ticket` model and `TicketStatus` enum | [`Models/Ticket.cs`](TicketOpsLive/Models/Ticket.cs) (`INotifyPropertyChanged`, `RecentlyUpdated`/`Marker`), [`Models/TicketStatus.cs`](TicketOpsLive/Models/TicketStatus.cs) |
| Bind a `BindingList<Ticket>` through a `BindingSource` | `MainPage` fields `_tickets` / `_allTickets`; `MainPage_Load` → `ticketsBindingSource.DataSource = _tickets;` (the grid's `DataSource` is set once, in the Designer) |
| A button that creates a new ticket event every second for 20 seconds | `newTicketsButton_Click` → [`Services/TicketSimulator.cs`](TicketOpsLive/Services/TicketSimulator.cs) `StartNewTickets(20, 1000)` → `Application.StartTask(NewTicketLoop)` |
| A button that randomly changes ticket statuses | `changeStatusButton_Click` → `TicketSimulator.RandomizeStatuses(3)` (synchronous, in-request) |
| Preserve the selected row when possible | `ApplyTicketEvent` remembers `ticketsGrid.CurrentRow?.DataBoundItem as Ticket`; `RestoreSelection(id)` walks `ticketsGrid.Rows` and sets `Selected` / `CurrentCell`; `selectedLabel` shows the result |
| Show `UpdatedAt` in a readable format | `colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss"` in the Designer — the model keeps a real `DateTime` |
| Extension challenge: an Escalated filter that keeps working live | `escalatedOnlyCheckBox_CheckedChanged` + `ApplyFilter()`; the master list `_allTickets` and the bound list `_tickets` are separate, and the filter is re-applied inside every event |
| Show every path (success / progress / cancellation / failure) | `changeStatusButton_Click` · `newTicketsButton_Click` · `stopButton_Click` · `corruptButton_Click` + the validation block of `ApplyTicketEvent`; `labelBanner`, `labelStatus`, the trace |
| Walkthrough deliverable: row cues + the conflict rule | `markerTimer_Tick`, `ShowTicketChangeWarning`, [`docs/RowCuesAndConflicts.md`](TicketOpsLive/docs/RowCuesAndConflicts.md) |
| Walkthrough deliverable: live-binding notes | [`docs/LiveBindingNotes.md`](TicketOpsLive/docs/LiveBindingNotes.md) |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs               ApplyTicketEvent, RestoreSelection, ApplyFilter, markerTimer_Tick,
│                            the conflict warning, the rebind anti-pattern, BeginPush/EndPush
├─ MainPage.Designer.cs      the board card (grid + 7 columns + BindingSource + Timer), the trace card,
│                            the action bar (opens in the Wisej Designer)
├─ Models/
│  ├─ Ticket.cs              INotifyPropertyChanged; UpdatedAt is a DateTime; RecentlyUpdated → Marker
│  ├─ TicketStatus.cs        New · Assigned · Waiting · Resolved · Escalated
│  └─ TicketChangedEventArgs.cs   the lab's model event + FeedStoppedEventArgs (the completion callback)
├─ Services/
│  └─ TicketSimulator.cs     session-owned feed: Seed, StartNewTickets(20, 1000), RandomizeStatuses(3),
│                            Stop, IsRunning, SendCorruptEvent; raises TicketChanged / FeedStopped
├─ Program.cs                Application.MainPage = new MainPage()
├─ Startup.cs                Kestrel host (app.UseWisej())
├─ Default.json              Wisej.NET application config (add "enableWebSocket": false to see the fallback)
└─ docs/
   ├─ LiveBindingNotes.md    BindingList + BindingSource, why the grid is never rebound, the thread rule, evidence
   └─ RowCuesAndConflicts.md the five row cues and the warn-don't-reload conflict rule, with evidence
```

## Self-check — the acceptance criteria, mapped to the code

- [x] **New tickets appear without rebinding the grid from scratch.**
  `ApplyTicketEvent` calls `_allTickets.Insert(0, ticket)` and then `ticketsBindingSource.ResetBindings(false)`.
  `ticketsGrid.DataSource` is assigned exactly once, in `InitializeComponent()` — the only other assignment in the
  whole project is inside `rebindButton_Click`, the deliberate anti-pattern.
- [x] **Existing tickets update in place.**
  When `FindById(e.Ticket.Id)` returns a ticket, only `Status`, `Owner` and `UpdatedAt` are written on that instance.
  No row is removed, inserted or re-ordered, so the row keeps its position on screen.
- [x] **The selected row is preserved when the row still exists.**
  The selected `Id` is captured from `ticketsGrid.CurrentRow?.DataBoundItem` **before** the list changes and restored
  by `RestoreSelection(id)`, which matches by id (not by index) across `ticketsGrid.Rows`. `selectedLabel` and the
  trace (`selection kept #4822`) show it on every event. When the row is not shown any more (the Escalated filter),
  the trace says so instead of moving the selection somewhere else.
- [x] **The update rate remains controlled.**
  The feed pushes once per ticket, one ticket per second, and never inside a tight loop (`Thread.Sleep(1000)` between
  tickets, no sleep after the last one). `Randomize statuses` changes three tickets inside one request — three list
  changes, **zero** pushes. `markerTimer` resets the bindings only when a marker actually expired. The final state is
  always pushed, from the task's `finally` block via `Feed_FeedStopped`.
- [x] **The student can explain why the bound list is session-owned.** See below.

### Why the bound list is session-owned

`_tickets` (the `BindingList<Ticket>` behind the grid), `_allTickets` (the master list), `_markerSince` and `_feed` are
all **instance fields of `MainPage`**, and Wisej.NET creates one `MainPage` per browser session. That matters for three
separate reasons:

1. **Correctness.** A bound collection is part of one session's UI state: it carries that user's filter, that user's
   order and the currency (which row is "current") of that user's grid. A `static` list would mean one user turning on
   *Escalated only* empties every other user's grid, and one user's `Stop feed` stops everybody's feed.
2. **Threading.** Two threads already reach this page — the request thread and the feed task. Making the list global
   would add every *other* session's threads to that list, all mutating the same `BindingList` while grids read it.
   Session-owned plus "only mutate inside `Application.Update(context, …)`" keeps the set of writers to one at a time.
3. **Lifetime.** The list dies with the page. `MainPage_Disposed` stops the feed and unsubscribes the handlers, and
   the feed loop checks `_isPageAlive()` before every ticket, so a closed tab frees everything. A global service
   holding page or control references is the classic leak: sessions that ended cannot be collected, and pushing to
   them throws.

The service in this sample is the source of truth (it owns the ticket **records**); each session owns a **projection**
of it in its bound list, and the two are connected only by events. That is the shape that scales to real multi-user
data, and it is exactly what Module 6's global `TicketHub` formalises.

### Lesson checkpoint

> *Maintain a live grid that receives new rows and row updates from a background source, while preserving the user's
> context and avoiding wasteful rebinding.*

The board does all three: rows arrive from `Application.StartTask` with no browser request, updates land on the
existing objects, the selection (and the scroll, sort and markers) survive because the binding is never thrown away —
and the **Rebind whole grid (anti-pattern)** button is there to show, side by side, what is lost when it is.

## APIs used here that are not yet verified at runtime

This module builds clean (0 warnings, 0 errors, both `net10.0` and `net10.0-windows`) but **was not run in a browser**
while it was written. Everything in the following list comes from the Wisej.NET 4.1.0 XML documentation / the
Foundations cookbook and should be checked on the first run:

- `Wisej.Web.DataGridView`: `AutoGenerateColumns`, `AutoSelectFirstRow`, `SelectionMode = DataGridViewSelectionMode.FullRowSelect`,
  `MultiSelect`, `ReadOnly`, `RowHeadersVisible`, `ShowFocusCell`, `AllowUserToAddRows`, `AllowUserToDeleteRows`,
  `DataSource`, `Columns.Add(column)`, `Rows.Count` / `Rows[i]`, `CurrentRow`, `CurrentCell`, the `SelectionChanged` event.
- `Wisej.Web.DataGridViewRow`: `DataBoundItem`, `Selected`, `Cells[0]`.
- `Wisej.Web.DataGridViewTextBoxColumn` / `DataGridViewColumn`: `DataPropertyName`, `HeaderText`, `Name`, `Width`,
  `ReadOnly`, `SortMode`, and the lazily created `DefaultCellStyle` with `Format = "HH:mm:ss"`, `Alignment`, `ForeColor`.
  In particular: whether `Format` really formats a bound `DateTime` in the browser, and whether a column bound to the
  **read-only derived** property `Ticket.Marker` renders (and never tries to write back).
- `Wisej.Web.BindingSource`: the `BindingSource(IContainer)` constructor, `DataSource = BindingList<Ticket>`,
  `ResetBindings(false)` — and specifically whether one `ResetBindings(false)` per event is enough for the grid to pick
  up both a new row and a changed cell.
- `Wisej.Web.Timer(components)` with `Interval` / `Tick` / `Start()` — the assumption that its `Tick` is a normal
  request, so no `Application.Update()` is needed for what it changes.
- `Wisej.Web.CheckBox.CheckedChanged`.
- `Application.Update(this, callback)` called **inside a request** (the `Randomize statuses` and `Apply corrupt event`
  paths raise `TicketChanged` on the request thread, and the handler always goes through `Application.Update`). Modules
  1 and 3 verified the callback form on a task thread only.
- Whether setting `Rows[i].Selected` / `CurrentCell` from server code raises `SelectionChanged` (the code guards it
  with `_restoringSelection`, so either behaviour is safe — but the trace's "user selected" lines depend on it).
