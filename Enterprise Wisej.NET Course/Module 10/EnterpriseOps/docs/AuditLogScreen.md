# Audit log screen — EnterpriseOps

Deliverable 4 of Module 10. `UI/AuditLogPage.cs` / `.Designer.cs` — the bottom card of the running screen.

## What an entry answers

Who did what, to which record, in which tenant, when, and how it ended.

| Field | Source | Grid column |
|---|---|---|
| `AtUtc` | `DateTime.UtcNow` at the moment of the decision | **Time** (local) |
| `UserId` | `CommandContext.UserId` — the `sub` claim | **User** |
| `TenantId` | `CommandContext.TenantId` — the `tid` claim | **Tenant** |
| `Action` | the `Permission` demanded, or a command name (`SignIn`, `ApproveWorkOrder`, `ApproveExport`) | **Permission demanded** |
| `Result` | `Ok` · `Denied` · `Pending` · `Failed` | **Result** |
| `Target` | `WO-1002`, `export #3 · 36 rows` | **Detail · correlation id** |
| `Detail` | the named reason and the values that matter | **Detail · correlation id** |
| `CorrelationId` | `CommandContext.CorrelationId` | **Detail · correlation id** |

The correlation id is the join key: the same eight characters appear in the Detail column, in the status bar
and error banners (`ref …`), in the server log lines of that click, and (from Module 11) in the structured logs.

## Properties of the log

- **Append-only.** `AuditLog.Write` adds; nothing updates or deletes; there is no "clear audit" button.
- **Written by the service that executes the command, inside the same operation.** An action cannot succeed
  without being recorded, because the record is written where the decision is taken
  (`PermissionService.Demand`) and where the change is made (`WorkOrderService.ApproveAsync`).
- **Denials are recorded.** A wrongly enabled button that lets a user request `ExportData` is caught by the
  service, refused with a named reason, and written as `DENIED`. Repeated denials from one account are the
  signal that someone is probing — and they exist only because denials are logged.
- **Application-scoped, tenant-filtered.** The log is shared by every session in the process (an audit that
  vanished with a session would not be an audit), so writes take a lock and `Snapshot(tenantId)` never returns
  another tenant's rows. It is the documented exception to "no per-user state in statics".

## Reading it

`AuditQueryService.QueryAsync` decides the scope with `Has(ViewAuditLog)`:

- with the permission → every entry of the caller's tenant;
- without it → the caller's own entries only.

The three combo boxes (**User: any**, **Permission: any**, **Result: any**) are read into an `AuditFilter` and
applied **by the service**. The grid binds to `AuditRow`, a projection — never to `AuditEntry` itself. Denied
rows are painted red, which is a UI decision made from `AuditRow.IsDenied`, a flag the service set.

## What is missing here, and required in production

The lesson calls the audit trail *tamper-evident*. This sample is in memory, so it is not:

- durable storage, separate from the operational database, with its own retention policy;
- a hash chain or signed sequence number so that a deleted or edited row is detectable;
- write access for the application and read access for reviewers — never the same account;
- alerting on patterns (n denials from one subject inside m minutes).

All four are on the hardening checklist.

## Evidence in the running app

The screen opens with twelve seeded entries, including the five from the walkthrough video
(`svc.import · EditWorkOrders · OK`, `m.weber · ExportData · OK`, `j.kim · ExportData · DENIED`,
`m.weber · ApproveWorkOrders · OK`, `l.romero · ApproveWorkOrders · DENIED`).

Then, live:

| Do this | New audit row |
|---|---|
| Sign in as anyone | `SignIn · OK · claims mapped → Manager` |
| **Export data** as `l.romero` (the button is offered to every signed-in user) | `ExportData · DENIED · missing permission for roles Technician` — the first row of the grid |
| **Approve** as `m.weber` | two rows: `ApproveWorkOrders · OK` (the decision) and `ApproveWorkOrder · OK · WO-…` (the change) |
| **Export data** as `m.weber` | `ExportData · PENDING · awaiting a second approver` |
| **Approve pending export** as `d.singh` on his own request | `ApproveExport · DENIED · separation of duties` |
| **Approve pending export** as `j.kim` | `ApproveExport · OK` and `ExportData · OK · completed after approval` |
| Set **Result: DENIED** in the filter | only the refusals — the probing view |
