# Permission matrix — EnterpriseOps

Deliverable 2 of Module 10. Rows are permissions, columns are roles, each cell is granted, denied or
conditional. The matrix is the artifact product owners, security reviewers and developers can all read, so it
exists before the code — and here it exists **as** code: `Security/RolePermissionStore.BuildMatrix()` is this
table, so the document and the running application cannot drift apart.

Every cell is evaluated **inside a tenant**. A role held in tenant A grants nothing in tenant B; the tenant
guard runs before the matrix is consulted.

## The matrix

| Permission | Technician | Manager | Admin | Auditor | ServiceAccount |
|---|:--:|:--:|:--:|:--:|:--:|
| `ViewWorkOrders` | ✅ | ✅ | ✅ | ✅ | ✅ |
| `EditWorkOrders` | ✅ | ✅ | ✅ | ❌ | ✅ |
| `ApproveWorkOrders` | ❌ | ✅ | ✅ | ❌ | ❌ |
| `ExportData` | ❌ | ⚠️ | ⚠️ | ❌ | ❌ |
| `AdminDiagnostics` | ❌ | ❌ | ✅ | ❌ | ❌ |
| `ViewAuditLog` | ❌ | ✅ | ✅ | ✅ | ❌ |
| `ApproveExport` | ❌ | ❌ | ✅ | ✅ | ❌ |

✅ granted ⚠️ conditional ❌ denied

## The conditional cells

`ExportData` is conditional for Manager and Admin. The matrix says *may ask*; a rule that the matrix cannot
express says *may complete*:

- an export of more than **25 rows** (`ExportService.ApprovalThreshold`) is held and needs a second person
  holding `ApproveExport`;
- that person may **not** be the requester (separation of duties).

Separation of duties is a rule about *people*, not about roles, so it lives next to the command in
`ExportService.ApproveExportAsync` — and both refusals are audited.

Note the deliberate shape of the last two rows: the roles that may *release* a bulk export (Admin, Auditor) are
almost disjoint from the roles that may *request* one (Manager, Admin). Only an Admin holds both, and when an
Admin tries to approve their own request the service refuses.

## Why permissions and not roles

Services ask `Demand(context, Permission.ApproveWorkOrders)` — never "is this caller a Manager?". Adding a
"Senior technician who may approve below €500" role then changes one row of this table and no service at all.

## Workflow steps map onto the matrix

The escalation workflow the course builds in other modules needs three different cells, and the matrix makes
that explicit rather than hiding it inside a state machine:

| Workflow step | Permission demanded |
|---|---|
| Submit an escalation | `EditWorkOrders` |
| Approve it | `ApproveWorkOrders` |
| Export the closed set for the customer | `ExportData` (+ `ApproveExport` above the threshold) |
| Read what happened afterwards | `ViewAuditLog` |

## Membership (who holds which role, per tenant)

Membership is the operational half and changes weekly; it is **not** in this table. It is written by
`RolePermissionStore.SetMembership` from the mapped claims at sign-in, and reloaded by
`SessionContext.RefreshPermissions` without a new login. See `IdentityMappingDesign.md` for the directory.

## Reading the audit log

`ViewAuditLog` grants the whole tenant's trail. Without it, a signed-in caller still sees **their own** entries
— being able to see the security events recorded against your own account needs no privilege. That is why the
walkthrough's technician can watch their own denial land in the grid while never seeing anyone else's row.
`AuditQueryService` implements this with `Has`, not `Demand`: the answer narrows the query, it does not refuse it.

## Evidence in the running app

**Export data** is offered to every signed-in user; `ExportService` decides. **Approve** and **Approve pending
export** are shown only when `Has` says yes.

| Signed in as | Buttons the screen offers | What the services do |
|---|---|---|
| `l.romero` (Technician) | Export data | **Export data** → `Missing permission: ExportData`, audited DENIED |
| `m.weber` (Manager) | Approve · Export data | Export → 36 rows > 25 → export #3 held for a second person |
| `d.singh` (Admin) | Approve · Approve pending export · Export data | approving their own export → refused, separation of duties, audited DENIED |
| `j.kim` (Auditor) | Approve pending export · Export data | releases someone else's export; a request of their own is refused |
| `svc.import` (ServiceAccount) | Export data | can view and edit only — a service account with `ExportData` is a breach waiting for a mis-configured job |
| `t.novak` (no mapped group) | Export data | every `Demand` refuses; the queue never loads |
