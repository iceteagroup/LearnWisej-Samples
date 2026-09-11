# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 10

Local lab build for **Module 10 · Security architecture: identity, SSO, authorization, audit & secure
deployment**. It follows the walkthrough video: a simulated corporate sign-in that hands the application a set
of **claims**, a `ClaimsMapper` that turns them into the application's own roles, a `PermissionService.Demand`
that checks the **tenant guard first and the role store second**, an **audit log screen** where every Demand
lands — granted or denied — and the video's failure path: an **enabled Export button** that the service refuses
anyway, with `Missing permission: ExportData` written to the trail.

Around that spine the lab adds tenant-role enforcement, export approval with separation of duties, safe HTML
rendering of an untrusted customer note, and a security hardening checklist.

There is **no real identity provider, no password field and no credential of any kind** in this application. The
sign-in gate picks which identity the provider should assert and shows the claim list that crosses the boundary.
Nothing here is deployed anywhere.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 10\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5210
```

Then open <http://localhost:5210>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.) Requirements: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package.

## The screen

The sign-in gate (`SignInGate`) opens **first** — before any data is loaded. Pick an identity from the corporate
directory (its claims are listed on the right) and press **Sign in**, or **Stay signed out** to leave the
screen locked. To act as a second person at the same time, open the app in another browser or a private window
(a new session gets its own gate).

`AuditLogPage` ("EnterpriseOps — Work queue + audit"):

| Area | What it holds |
|---|---|
| Work queue (top left) | `dgvWorkOrders`, **✔ Approve** (`btnApprove`), **Approve pending export** (`btnApproveExport`), **⤓ Export data** (`btnExport`), and the result banner `lblBanner` |
| Customer note (top right) | the latest note on `WO-1002`, rendered by `NoteRenderService` with `AllowHtml = false` — the payload's tags show as characters |
| Audit log — sensitive commands (bottom) | the three filters **User / Permission / Result**, `dgvAudit` (Time · User · Tenant · Permission demanded · Result · Detail) and the dark status bar `lblStatusBar` |

**Approve** and **Approve pending export** appear only when `Has` says the caller holds the permission.
**Export data** is shown to every signed-in user: `ExportService` demands `ExportData` itself.

## The directory to sign in as

| Account | Tenant | Role | Use it for |
|---|---|---|---|
| `m.weber` | fabrikam | Manager | approvals, and requesting an export |
| `l.romero` | fabrikam | Technician | **the walkthrough's denied export** |
| `j.kim` | fabrikam | Auditor | reading the whole trail; releasing someone else's export |
| `d.singh` | fabrikam | Admin | the separation-of-duties refusal |
| `svc.import` | fabrikam | ServiceAccount | a non-human caller with edit rights only |
| `ana.ops` | **contoso** | Manager | another tenant — sees contoso rows only |
| `t.novak` | fabrikam | *(none)* | authenticated, entitled to nothing |

## What to click

| # | Signed in as | Action | What you should see |
|---|---|---|---|
| 1 | `l.romero` | **⤓ Export data** | **failure** (the walkthrough): red banner *"You don't have permission to export data."*; status bar `UnauthorizedAccessException — Missing permission: ExportData · audited · nothing exported`; the denial is the first row of the audit log |
| 2 | any | **Result: DENIED** filter | only refusals, including the seeded `j.kim · ExportData · DENIED` |
| 3 | `m.weber` | **✔ Approve** | `WO-…` approved; two audit rows — `ApproveWorkOrders · OK` (the decision) and `ApproveWorkOrder · OK` (the change), same correlation id |
| 4 | `m.weber` | **⤓ Export data** | 36 rows is above the 25-row threshold: amber banner, `ExportData · PENDING · awaiting a second approver` |
| 5 | `d.singh`, with no other export pending | **⤓ Export data**, then **Approve pending export** | *"An export cannot be approved by the person who requested it."* — `ApproveExport · DENIED · separation of duties` |
| 6 | `j.kim`, in a second browser while an export is pending | **Approve pending export** | released: `ApproveExport · OK` and `ExportData · OK · completed after approval` |
| 7 | `t.novak` | sign in | amber banner naming the unmapped group `CorpVPN-Users`, an empty queue because `Demand(ViewWorkOrders)` refused |

Pending exports are shared by every session of the tenant (like the audit log), so the requester and the approver
can be two people in two browsers.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Identity mapping design | [`EnterpriseOps/docs/IdentityMappingDesign.md`](EnterpriseOps/docs/IdentityMappingDesign.md) + [`identity-flow.svg`](EnterpriseOps/docs/identity-flow.svg) · code: `Security/SsoIdentityProvider.cs`, `Security/ClaimsMapper.cs`, `Security/SessionContext.cs` |
| 2 | Permission matrix | [`EnterpriseOps/docs/PermissionMatrix.md`](EnterpriseOps/docs/PermissionMatrix.md) · code: `Security/RolePermissionStore.BuildMatrix()` |
| 3 | Permission service implementation | [`EnterpriseOps/docs/PermissionServiceImplementation.md`](EnterpriseOps/docs/PermissionServiceImplementation.md) · code: `Security/SecurityArchitecturePatterns.cs`, `Security/PermissionService.cs`, `Security/TenantGuard.cs` |
| 4 | Audit log screen | [`EnterpriseOps/docs/AuditLogScreen.md`](EnterpriseOps/docs/AuditLogScreen.md) · code: `UI/AuditLogPage.*`, `Security/AuditLog.cs`, `Services/AuditQueryService.cs` |
| 5 | Security hardening checklist | [`EnterpriseOps/docs/SecurityHardeningChecklist.md`](EnterpriseOps/docs/SecurityHardeningChecklist.md) · code: `Security/HardeningChecklist.cs`, `Security/HtmlText.cs`, `Services/NoteRenderService.cs` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it locally once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5210` |
| Enterprise authentication simulation | `Security/SimulatedSsoIdentityProvider` (a claim list, no password) · `UI/SignInGate.*` · `Services/SignInService.SignInAsync` |
| Claims-to-permission mapping | `Security/ClaimsMapper` (group → `Role`) → `RolePermissionStore.SetMembership` → the matrix |
| Tenant-role enforcement | `Security/TenantGuard.DemandTenant`, called first inside `PermissionService.Demand`; `AuditLog.Snapshot` is tenant-filtered |
| Service-level authorization | `WorkOrderService.LoadQueueAsync` / `.ApproveAsync`, `ExportService.RequestExportAsync` / `.ApproveExportAsync` — each opens with `Demand` |
| Safe HTML review | `Security/HtmlText` (escape · allow-list sanitize) · `Services/NoteRenderService` chooses `AllowHtml = false` for the customer note · the surface inventory in `docs/SecurityHardeningChecklist.md` |
| Export approval | `Services/ExportService` — threshold + `ApproveExport` + separation of duties |
| Audit logging for sensitive actions | `Security/AuditLog` written by `PermissionService.Demand` and by each service next to its change |
| Show every path | success, held-for-approval and refusals each end in a banner, a status-bar line and audit rows; unexpected exceptions show a generic message with the correlation id (`AuditLogPage.ReportFailure`) |
| Review & run: production-readiness note | `docs/SecurityHardeningChecklist.md`, and the "what is missing here" section of `docs/AuditLogScreen.md` |

## Where things live

```
Module 10/
└─ EnterpriseOps/
   ├─ Domain/          WorkOrder · WorkOrderStatus · Priority · Tenant · WorkQueueRow · WorkOrderNote
   ├─ Security/        SecurityArchitecturePatterns.cs · PermissionService · RolePermissionStore · TenantGuard
   │                   ClaimsMapper · SsoIdentityProvider · SessionContext · CommandContext · Role
   │                   AuditLog · HtmlText · HardeningChecklist
   ├─ Services/        SignInService · WorkOrderService · ExportService · AuditQueryService
   │                   NoteRenderService · ServiceRegistry · CommandResult
   ├─ Data/            SeedData · InMemoryWorkOrderRepository · InMemoryNoteStore
   ├─ Diagnostics/     ActivityTrace — server-side log (System.Diagnostics.Trace)
   ├─ UI/              AuditLogPage.cs / .Designer.cs · SignInGate.cs / .Designer.cs
   ├─ docs/            the five deliverables + identity-flow.svg
   ├─ Program.cs       session composition root — creates the graph, opens the page, signs nobody in
   └─ Startup.cs       Kestrel host (app.UseWisej(); *.json is never served)
```

Data: 40 in-memory work orders (36 fabrikam, 4 contoso), 3 customer notes on `WO-1002` (the third carries an
injection payload), and 12 seeded audit entries including the five the video shows.

## Student review questions, answered against this sample

**Where is authorization enforced?** In the service that executes the action, as the first non-trivial
statement of every public method: `LoadQueueAsync` opens with `Demand(ViewWorkOrders)`, `ApproveAsync` with
`Demand(ApproveWorkOrders, order.TenantId)`, `RequestExportAsync` with `Demand(ExportData, context.TenantId)`.
`PermissionService.Demand` checks the tenant guard first, then the matrix, throws naming the missing permission,
and writes an audit entry either way. The screen's only authorization-shaped lines are two `Has` calls in
`ApplyPermissionsToUi`, and they only hide buttons.

**Which UI elements render HTML?** Every control and grid column with an `AllowHtml` property is a candidate;
the inventory in `docs/SecurityHardeningChecklist.md` lists the ones on this screen whose text comes from outside
(`lblNote`, `lblNoteSource`, `colUser`, `colDetail`, `lblBanner`, `lblStatusBar`, and the gate's lists). All of
them keep `AllowHtml = false`; the customer note's setting is chosen by `NoteRenderService`.

**Can an enabled button bypass service rules?** No. **Export data** is enabled for a Technician who does not hold
`ExportData`; the click reaches `ExportService.RequestExportAsync`, which demands the permission before it reads a
row, refuses, writes the denial to the audit log and returns a result the screen shows as a red banner.

## Instructor acceptance criteria, answered against this sample

- **Architecture baseline** — one project, folder-per-layer, services built once per session in
  `ServiceRegistry`, typed results (`CommandResult`, `ExportResult`, `WorkQueueResult`, `AuditQueryResult`),
  projections bound to the grids, and no user or tenant state in a static field — except the documented
  application-scoped `AuditLog.Shared`, locked on write and tenant-filtered on read.
- **Thin handlers** — every handler in `AuditLogPage.cs` is `BeginBusy` / one awaited service call / `Show…` /
  `catch` / `EndBusy`. No handler decides a permission, builds a claim or chooses an encoding.
- **Service logic reviewable without the designer** — the security story is in `Security/` and `Services/`.
- **A failure path** — the Technician's refused export (the walkthrough), plus the refused self-approval of an
  export and the unmapped account.
- **State ownership** — the screen owns UI state; the session owns identity; the role store owns membership; the
  audit log is application-scoped on purpose. The in-memory audit log is not tamper-evident, the sanitizer is a
  lab-sized allow-list, and cookies, CSP, headers, uploads and the reverse proxy are deployment-owned checklist
  items.

## Verified / unverified

Unverified (from the Wisej.NET XML documentation, build passes — please confirm at runtime): `Label.AllowHtml`
(that `AllowHtml = false` shows `<b>urgent</b>` as characters), `DataGridViewRow.DataBoundItem`, and
`ComboBox.DropDownStyle = DropDownList` with `Items.Add(string)` + `SelectedIndexChanged`.
