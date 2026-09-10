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
| `ViewAuditTrail` | ✖ | ✔ | ✔ | UI caption only in this lab (the list is shown to every role so the evidence is visible); production gates the list |

## Two callers, one answer, one boundary

| Caller | Question | Purpose | Is it security? |
|---|---|---|---|
| `WorkOrdersView.ApplyPermissionsToControls` | `Can(user, DeleteTicket)` → `buttonDelete.Visible` | courtesy: honest users do not see what they cannot do | **No** — a hint |
| `TicketService.Authorize` | `Can(user, DeleteTicket)` → continue or throw | the actual control, where the state changes | **Yes** |

## Denial contract

- The service audits the denial (`IAuditService.Denied(user, action, target, "needs Supervisor or Admin")`).
- Then it **throws** `UnauthorizedAccessException` — before `FindAsync`, before any write.
- The screen maps that exception to `Strings.AccessDenied`; the exception text never reaches the user.

## Evidence

| Signed in as | Delete button | *Call DeleteAsync directly* | Trace |
|---|---|---|---|
| `l.romero` (Technician) | hidden | red banner *You are not allowed to perform this action.*, #2002 still listed | `[SVC] ⚠ PermissionService.Can — l.romero [Technician] DeleteTicket → denied (needs Supervisor or Admin)` · `[INFRA] ⚠ AuditLog — [AUDIT] ⛔ DENIED DeleteTicket #2002 — l.romero (Technician): needs Supervisor or Admin` · `[SVC] ⚠ TicketService.DeleteTicket — denied before any read or write → throw UnauthorizedAccessException` |
| `m.weber` (Supervisor) | shown | status *● Ticket #2002 deleted.*, row gone | `[SVC] PermissionService.Can — m.weber [Supervisor] DeleteTicket → allowed` · `[DATA] InMemoryTicketRepository.DeleteAsync — #2002 deleted (5 rows left)` · `[INFRA] AuditLog — [AUDIT] ✓ DeleteTicket #2002 — m.weber (Supervisor)` |
