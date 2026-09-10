# Sync conflict screen

**Deliverable 4 of Module 13.** `UI/SyncConflictPanel.cs` — a conflict is a workflow, not a crash and not
a silent overwrite.

## What a conflict is

The same record changed **on the device** and **on the server** while they were apart. In the lab's
failure path:

* 13:50, in the office: the dispatcher (`ana.ops`) **cancels** WO-1037 — "site rescheduled to Thursday
  crew". The row becomes `Cancelled`, `Version` 2 → 3.
* 14:32, in a basement with no signal: the technician (`ben.tech`) **completes** WO-1037 — "relief valve
  replaced, torqued to spec". The command is queued with `BaseVersion = 2`.
* On reconnect the replay finds 2 ≠ 3 and returns `CommandResult.Conflict` with the server's snapshot
  attached.

Neither version is simply wrong. The dispatcher had information the technician did not; the technician
did work the dispatcher does not know about. Only a person can decide, so the screen asks one.

## What the panel shows

```
┌──────────────────────────────────────────────────────────────────────┐
│ Sync conflict — WO-1037                                              │
│ The server changed this work order while you were offline.           │
│ Nothing has been applied.                                            │
│ ┌──────────────────────────────────────────────────────────────────┐ │
│ │ YOUR LOCAL CHANGE · 14:32 OFFLINE                                │ │
│ │ Completed — "relief valve replaced, torqued to spec"             │ │
│ └──────────────────────────────────────────────────────────────────┘ │
│ ┌──────────────────────────────────────────────────────────────────┐ │
│ │ SERVER VERSION · 13:50 BY ANA.OPS                                │ │
│ │ Cancelled — "site rescheduled to Thursday crew"                  │ │
│ └──────────────────────────────────────────────────────────────────┘ │
│ [ Keep server — attach my notes ]  [ Apply my completion… ]          │
└──────────────────────────────────────────────────────────────────────┘
```

Three design rules, all visible above:

1. **Both versions, in full, with who and when.** Not "a conflict occurred".
2. **Nothing has been applied.** The technician is told that before they are asked anything.
3. **Two named outcomes**, in the product's words, not the database's. No "merge", no "force".

While the panel is open the cache list is hidden: the conflict takes the screen, because carrying on and
resolving it later is how conflicts get abandoned.

## The two resolutions

| Button | What the server does | Who may | Command ends as |
|---|---|---|---|
| **Keep server — attach my notes** | `WorkOrderService.AttachNotes` — the status stays `Cancelled`; the technician's field notes are appended to the work order and audited with the device time. | any technician | `Rejected` — "kept server — notes preserved" |
| **Apply my completion…** | `WorkOrderService.Complete` with `OverrideServerChange = true` — requires **`workorder.override`**. | Manager / Admin | `Synced` if allowed; stays `Conflict` with the refusal on it if not |

`ben.tech` is a Technician, so **Apply my completion…** is *refused by the server*, not hidden by the
client. The refusal is audited (`Override … Rejected … workorder.override required (Manager/Admin)`) and
the panel stays open with the real rule on screen:

> Applying your completion over the dispatcher's change needs workorder.override (Manager). Ask the
> dispatcher, or keep the server version and attach your notes.

Showing the button and letting the server refuse is deliberate. A greyed-out button teaches nothing and
hides the rule; a refused attempt teaches the rule and leaves an audit trail of who tried.

## Whose decision it is

The resolution rules are a **product** decision, written down before the code:

* A cancelled work order may not be silently completed — a cancellation usually means someone else was
  dispatched, or the site is unsafe.
* Field information is never lost. Whatever the outcome, the technician's notes end up attached to the
  record.
* Overriding an office decision is a supervisory act, so it needs a supervisory permission.

The panel enforces none of that itself. It raises `KeepServerRequested` / `ApplyMineRequested`; the page
calls `SyncWorkflow`; the workflow calls `WorkOrderService`. The rules are reviewable without opening the
designer.

## Evidence — what the running app shows

| Claim | Where you see it |
|---|---|
| The conflict is detected on the server, by version | Trace: `Service: WO-1037 version mismatch: command based on v2, server is v3 (Cancelled v3 by ana.ops 13:50 …) → Conflict` |
| The replay stops rather than guessing | Trace: `Job: replay stopped at WO-1037: 0 command(s) stay PendingSync so the order is preserved — a person decides` |
| Both versions reach the screen | The two boxes in the panel, filled from `SyncConflict.LocalBody` / `.ServerBody` |
| Nothing is lost by keeping the server version | After **Keep server**: banner reads "…stays Cancelled, your notes are attached to it, and the audit log holds both versions"; the queue card turns grey `Rejected · kept server — notes preserved` |
| The override rule is enforced server-side | After **Apply my completion…** as `ben.tech`: red toast with the `workorder.override` message; the card stays red `Conflict`; the audit log gains an `Override … Rejected` entry |
| The queue resumes afterwards | With more than one command queued, resolving the conflict traces `UI → resuming the replay — N command(s) still pending behind the conflict` |
