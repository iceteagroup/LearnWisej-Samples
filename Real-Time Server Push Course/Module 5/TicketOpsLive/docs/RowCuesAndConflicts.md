# Row cues and conflicts — making a live grid trustworthy

Deliverable of Module 5. The lesson's list of row cues, and the conflict rule, as implemented in this sample.

> A live grid should show what changed without becoming noisy. Too many animations or row jumps make users think the
> application is unstable.

## 1 · A temporary "updated" marker

`Ticket.RecentlyUpdated` is set by `ApplyTicketEvent` for every row it adds or changes, and `Ticket.Marker` (a derived,
never-stored property) returns `"●"` while it is set. The marker column is bound to it:

```csharp
this.colMarker.DataPropertyName = "Marker";
this.colMarker.HeaderText = "●";
this.colMarker.Width = 36;
this.colMarker.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
this.colMarker.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
```

`markerTimer` (a `Wisej.Web.Timer`, 1000 ms) clears markers older than **3 seconds**. The timestamp lives in a
dictionary, not on the model, so nothing about the ageing policy leaks into the data:

```csharp
private readonly Dictionary<int, DateTime> _markerSince = new Dictionary<int, DateTime>();
```

Two things make the timer cheap:

- it resets the bindings **only when a marker actually expired** (`if (expired.Count == 0) return;`) — an idle board
  costs one empty tick per second and no traffic at all;
- its `Tick` runs as a normal request, so whatever it changes is returned with that request. No `Application.Update()`
  is needed, and none is called.

The marker is a cue, not an animation: it appears, it fades, it never moves anything.

## 2 · Sort new tickets to the top

New rows are inserted at position 0 of the master list (`_allTickets.Insert(0, ticket)`), so the newest ticket is
always the first row and the user never has to hunt for what just arrived. Existing rows **do not move** when they
change — only their cells change. That is the difference between "the board is alive" and "the board is jumping".

## 3 · Keep the user's selection stable

Every event remembers `ticketsGrid.CurrentRow?.DataBoundItem as Ticket` before it touches the list and restores it
afterwards by **id**:

```csharp
for (int i = 0; i < ticketsGrid.Rows.Count; i++)
    if (ticketsGrid.Rows[i].DataBoundItem is Ticket t && t.Id == id)
    {
        ticketsGrid.Rows[i].Selected = true;
        ticketsGrid.CurrentCell = ticketsGrid.Rows[i].Cells[0];
        return true;
    }
```

Matching by index would break the moment a row is inserted at the top. `ticketsGrid.AutoSelectFirstRow = false` keeps
the grid from grabbing the first row for itself when the list is repopulated, and `_restoringSelection` marks the
re-selection as code-driven so the trace does not pretend the user clicked.

`selectedLabel` is the visible proof: it keeps reading `selected: #4822 · Waiting · K. Brandt · updated 14:02:37`
while twenty tickets arrive above it. When the selected row is filtered away (Escalated filter on), the trace says
`selection #4822 filtered out (row not shown)` instead of silently selecting something else.

## 4 · Never move a row the user is working on

The sample never sorts, never re-orders and never removes an existing row while it is live. Rows only:

- appear at the top (a new ticket), or
- change their own cells in place (a status or owner change).

If the board did need re-sorting, the rule is to do it on an explicit user action (a column click), not on an incoming
event — and never while `ticketsGrid.IsCurrentCellInEditMode` is true. The grid here is `ReadOnly = true` with
`AllowUserToAddRows = false` / `AllowUserToDeleteRows = false`, so there is no half-typed edit for an event to destroy;
in an editable board the same guard would be an `if` around the update, deferring it until the edit is committed.

## 5 · Status columns and small indicators, not modal interruptions

Nothing in this module opens a dialog. Status is a column, freshness is a 36-pixel marker, and the conflict is a strip
under the grid. A `MessageBox` in a live grid would fire once per event and make the screen unusable.

## The conflict rule: warn, don't reload

> If a ticket is being read or edited and it changes, show a **non-blocking** warning that the ticket changed and let
> the user decide.

Implemented in `ApplyTicketEvent`:

```csharp
if (!added && selectedId.HasValue && selectedId.Value == existing.Id)
    ShowTicketChangeWarning(existing);
```

`ticketChangeLabel` (amber, hidden until needed) says:

```
⚠ Ticket #4822 changed while selected — status Escalated, owner K. Brandt, at 14:02:37.
  The row was updated in place; nothing was reloaded and your selection did not move.
```

with a `dismissButton` next to it. Selecting another row hides it too (`ticketsGrid_SelectionChanged`). What the app
deliberately does **not** do: reload the row from the server, reset the grid, steal the focus, or block with a dialog.

Wisej.NET does not remove the need for data-layer concurrency. A production system would still carry a row version or
timestamp and refuse a stale save; what the UI adds is telling the user **at the right moment** that what they are
looking at has moved on.

## Evidence — what the running app shows

| Action | What you see | Trace |
|---|---|---|
| any event on a row | a `●` in the first column for ~3 s | — |
| 3 s later | the marker disappears, nothing else moves | `• server markerTimer_Tick 2 “updated” marker(s) older than 3 s cleared · ResetBindings(false) · selection #4822 kept` |
| select #4822, then **New ticket every second ×20** | rows pile up **above** #4822; the blue row stays under the cursor; `selectedLabel` keeps saying `#4822` | `→ push Application.Update(this, …) #4831 added · selection kept #4822 · ResetBindings(false)` |
| select #4822, then **Randomize statuses** until #4822 is picked | the amber strip appears under the grid; the row does not move and is not reloaded | `• server conflict #4822 changed while selected → warn, don't reload (ticketChangeLabel, non-blocking)` |
| **Dismiss** | the strip disappears, the row is untouched | `← request dismissButton_Click the user dismissed the conflict warning — the row was never reloaded` |
| **Rebind whole grid (anti-pattern)** | the selection is gone, every marker is gone | `• server rebind (anti-pattern) … selection #4822 → none · scroll, sort and the “updated” markers reset` |
| **Escalated only** while the feed runs | the grid shows only escalated tickets and keeps updating; new non-escalated tickets are counted but not shown | `← request escalatedOnlyCheckBox_CheckedChanged filter ON (Escalated only) · 3 of 25 rows shown · selection kept #4822` |

(Expected behaviour: written against the verified patterns of Modules 1 and 3 and a clean build; this module was not
executed in a browser while it was written.)
