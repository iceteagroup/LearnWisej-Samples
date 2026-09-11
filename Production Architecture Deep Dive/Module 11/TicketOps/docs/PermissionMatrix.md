# Permission service — role → permission matrix

*Module 11 deliverable · implemented in `Security/PermissionService.cs`, asked by `Services/TicketService.cs`*

Roles are what the directory knows about a person; permissions are what the application needs to
decide. `IPermissionService.Can(user, permission)` maps one to the other from a constant table. Nothing
is granted by default (least privilege), and a `null` user (not signed in) is always denied.

| Permission | Technician (`l.romero`) | Supervisor (`m.weber`) | Admin (`s.okafor`) | Enforced in |
|---|---|---|---|---|
| `ViewTickets` | ✔ | ✔ | ✔ | `TicketService.GetTicketsAsync` |
| `AddNote` | ✔ | ✔ | ✔ | `TicketService.AddNoteAsync` |
| `CloseTicket` | ✖ | ✔ | ✔ | `TicketService.CloseAsync` |
| `DeleteTicket` | ✖ | ✔ | ✔ | `TicketService.DeleteAsync` |
| `ViewAuditTrail` | ✖ | ✔ | ✔ | not enforced in this lab (the audit list is shown to every role so the denial is visible); production gates the list |

## Two callers, one answer, one boundary

| Caller | Question | Purpose | Is it security? |
|---|---|---|---|
| `WorkOrdersView.ApplyPermissionsToControls` | `Can(user, DeleteTicket)` → `buttonDelete.Enabled` | courtesy: honest users are not offered what they cannot do | **No** — a hint |
| `TicketService.Authorize` | `Can(user, DeleteTicket)` → continue or throw | the actual control, where the state changes | **Yes** |

## Denial contract

- The service audits the denial (`IAuditService.Denied(user, action, target, "needs Supervisor or Admin")`).
- Then it **throws** `UnauthorizedAccessException` — before `FindAsync`, before any write.
- The screen maps that exception to `Strings.AccessDenied`; the exception text never reaches the user.

## Check it in the running app

| Signed in as | Delete button | Force-enable Delete, then Delete on #2002 |
|---|---|---|
| `l.romero` (Technician) | disabled | red banner *You are not allowed to perform this action.*, #2002 still listed, audit line `⛔ DENIED DeleteTicket #2002 — l.romero (Technician): needs Supervisor or Admin` |
| `m.weber` (Supervisor) | enabled | status *Ticket #2002 deleted.*, row gone, audit line `✓ DeleteTicket #2002 — m.weber (Supervisor)` |
