# Live binding notes — how the board stays bound while the data moves

Deliverable of Module 5 · *Live Ticket Board: Updating Data Without Rebuilding the Screen*.

## The three pieces and what each one owns

| Piece | Where | Owns |
|---|---|---|
| `TicketSimulator` (a stand-in for the real service) | `Services/TicketSimulator.cs`, an **instance field** of `MainPage` | the authoritative ticket **records** and the feed task |
| `List<Ticket> _allTickets` — the master list | `MainPage` instance field | everything this **session** knows about, newest first |
| `BindingList<Ticket> _tickets` — the bound list | `MainPage` instance field, bound through `ticketsBindingSource` | exactly what the grid shows (the master list, filtered) |

The grid is bound **once**, in the Designer:

```csharp
this.ticketsGrid.AutoGenerateColumns = false;      // the columns are declared, not guessed
this.ticketsGrid.Columns.Add(this.colMarker);      // … colId, colTitle, colCustomer, colOwner, colStatus, colUpdatedAt
this.ticketsGrid.DataSource = this.ticketsBindingSource;
```

and the list is attached once, in `MainPage_Load`:

```csharp
ticketsBindingSource.DataSource = _tickets;        // BindingList<Ticket>
```

From that moment nothing in `MainPage.cs` assigns `ticketsGrid.DataSource` again — except `rebindButton_Click`,
which exists only to show what that costs.

## Why `BindingList<T>` + `BindingSource` and not a plain `List<T>`

- `BindingList<T>` raises `ListChanged` when items are added or removed, so the grid learns about new rows.
- `Ticket` implements `INotifyPropertyChanged`, so a changed `Status` or `Owner` is *expressible* to the binding
  infrastructure. The sample still calls `ticketsBindingSource.ResetBindings(false)` once per event, for clarity and
  because one explicit refresh per event is easier to reason about (and to throttle) than N property notifications.
- `BindingSource` sits between the grid and the list: it manages currency, and it gives the code **one place** to say
  "the data changed, re-read it" without touching `DataSource`.

## Change the list, then notify the source

`ApplyTicketEvent` (`MainPage.cs`) is the only method that mutates the bound data, and it always does the same six things:

```csharp
if (e?.Ticket == null || e.Ticket.Id <= 0) { reject; return; }         // 1 · validate first
int? selectedId = ticketsGrid.CurrentRow?.DataBoundItem is Ticket s    // 2 · remember the user's row
    ? s.Id : (int?)null;

Ticket existing = FindById(e.Ticket.Id);                              // 3 · change the LIST
if (existing == null) _allTickets.Insert(0, Copy(e.Ticket));          //      new → to the top
else { existing.Status = …; existing.Owner = …; existing.UpdatedAt = DateTime.Now; }  // existing → in place

ApplyFilter();                                                        // 4 · the filter is part of the list
ticketsBindingSource.ResetBindings(false);                            // 5 · ONE notification per event
if (selectedId.HasValue) RestoreSelection(selectedId.Value);          // 6 · give the row back
```

`RestoreSelection` walks `ticketsGrid.Rows`, matches `Rows[i].DataBoundItem as Ticket` by `Id`, sets
`Rows[i].Selected = true` and `ticketsGrid.CurrentCell = Rows[i].Cells[0]`. It matches by **id**, not by row index:
after an insert at position 0 every index has shifted by one.

## Why the whole grid is never rebound

`ticketsGrid.DataSource = null;` followed by `DataSource = LoadAllTicketsAgain()` is the fastest way to make a demo
work and the wrong thing in production. It throws away everything the browser was holding *around* the data:

- the selected row (the objects the grid pointed at do not exist any more, so nothing can be matched back),
- the scroll position and the sort order,
- the column widths the user dragged, and any cell in edit,
- the temporary "updated" markers, because the fresh objects have never been marked.

The sample proves it: **Rebind whole grid (anti-pattern)** logs
`• server rebind (anti-pattern) 25 rows rebuilt · selection #4822 → none · scroll, sort and the “updated” markers reset`
and `selectedLabel` drops to *"selected: none"*. The rows are identical; the screen state is gone. On a busy board
that happens once per event.

## The thread rule

Two threads reach this page: the request thread (clicks, the marker timer) and the feed task started by
`Application.StartTask`. The bound list belongs to the session UI, so:

> **Never mutate the bound list from a thread that is not in the session context.**

`TicketSimulator` never touches a control and never touches `_tickets`. It raises `TicketChanged`, and the page's
handler enters the context before it does anything:

```csharp
private void Feed_TicketChanged(object sender, TicketChangedEventArgs e)
{
    if (this.IsDisposed) return;
    Application.Update(this, () => ApplyTicketEvent(e));   // apply in context, push once
}
```

`Application.Update(context, callback)` runs the callback in this session's context and then flushes the pending
changes in **one** push, so twenty tickets cost twenty pushes — not twenty pushes per property. The same handler is
used when the event is raised inside a click (`Randomize statuses`); there the changes simply travel back with the
response of that click, and the trace says `• server in-request update` instead of `→ push`.

The simulator's own record list is guarded by a `lock`, because `RandomizeStatuses` runs on the request thread while
the feed task may be adding a record. It hands the page a **snapshot** (`Snapshot(record)`), never the record itself,
so the grid can never read a half-written object.

## Session-owned, not global

Everything that describes "this user's board" is an instance field of `MainPage`: `_allTickets`, `_tickets`,
`_markerSince`, `_feed`, the counters. A `static` list here would mean one collection shared by every browser tab on
the server — one user's Escalated filter would empty another user's grid, one user's Stop would stop everyone's feed,
and two request threads would mutate the same `BindingList` at the same time. **SERVER STATE** prints the session's
own `ClientId` / `SessionId` next to the counters so the reviewer can open a second tab and see two independent boards.

Cleanup follows from the same rule: `MainPage_Disposed` calls `_feed.Stop()` and unsubscribes both handlers, and the
feed loop asks `_isPageAlive()` (`() => !this.IsDisposed`) before every ticket, so a closed tab ends the loop within a
second and the simulator never keeps a disposed page alive.

## Formatting stays in the view

`UpdatedAt` is a real `DateTime` on the model. The grid formats it:

```csharp
this.colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss";
this.colUpdatedAt.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
```

Storing `"14:02:37"` on the model would make sorting, ageing and time-zone handling impossible later.

## Evidence — what the running app shows

| Action | Trace | What it proves |
|---|---|---|
| page load | `• server bind once ticketsBindingSource.DataSource = _tickets … AutoGenerateColumns = false, 7 columns` | the grid is bound exactly once |
| page load | `• server seed 5 tickets added to the bound list · ResetBindings(false) · the grid was never rebound` | the board starts populated through the same path as live events |
| **New ticket every second ×20** | `• server Application.StartTask 20 tickets, one every 1000 ms …` then one line per second `→ push Application.Update(this, …) #4823 added · selection kept #4822 · ResetBindings(false)` | rows arrive with **no click**, one push per ticket, and the selection is still #4822 |
| **Randomize statuses** | `• server in-request update #4820 updated · selection kept #4822 · ResetBindings(false)` ×3 | existing rows change **in place**, inside the request — no push needed |
| **Rebind whole grid** | `• server rebind (anti-pattern) … selection #4822 → none · scroll, sort and the “updated” markers reset` | the cost of `DataSource = null` |
| **Apply corrupt event** | `• server ApplyTicketEvent REJECTED — Ticket is null · the bound list was not touched, the grid is intact (25 rows)` | validation happens before the list is touched |
| **Stop feed** | `← request stopButton_Click _feed.Stop() → the loop exits at its next check (≤ 1 s)` then `→ push … feed stopped — stopped by operator after 7 of 20 tickets (finally block of StartNewTickets)` | cooperative stop, UI restored from the task's `finally` |

(Expected behaviour: the sample builds clean and follows the verified patterns of Modules 1 and 3, but this module was
not executed in a browser while it was written — the grid and `BindingSource` calls are listed as unverified in the
module README.)

## Verified in the browser

- 20 tickets arrived one per second at the top of the grid while row #4822 stayed selected and highlighted; every push logged `selection kept #4822 · ResetBindings(false)`.
- Randomize statuses changed rows in place (no row moved); when the selected row changed, the conflict strip appeared instead of a reload.
- The rebind anti-pattern reset the selection and every marker; the corrupt event was rejected with the grid intact (25 rows).
- Finding: `BindingSource.ResetBindings(false)` raises the grid's `SelectionChanged` synchronously on the server, twice, pointing at whatever now sits at the old row index. The sample mutes its handler for the duration (`ResetBindingsQuietly()`) and restores the selection afterwards; without that, every push produced two phantom "the user selected" lines.
