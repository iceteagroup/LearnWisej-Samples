# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 10

Local lab build for **Module 10 · Security architecture: identity, SSO, authorization, audit & secure
deployment**. It follows the walkthrough video: a simulated corporate sign-in that hands the application a set
of **claims**, a `ClaimsMapper` that turns them into the application's own roles, a `PermissionService.Demand`
that checks the **tenant guard first and the role store second**, an **audit log screen** where every Demand
lands — granted or denied — and the video's failure path: a wrongly **enabled button** that the service refuses
anyway, with `Missing permission: ExportData` written to the trail.

Around that spine the lab adds what the lab guide asks for: tenant-role enforcement, export approval with
separation of duties, a safe-HTML review of every `AllowHtml` surface on the running screen, and a security
hardening checklist.

There is **no real identity provider, no password field and no credential of any kind** in this application. The
sign-in gate picks which identity the provider should assert and shows the claim list that crosses the boundary;
everything after that boundary is what the module is about. Nothing here is deployed anywhere.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 10\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5210
```

Then open <http://localhost:5210>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes for both target frameworks with 0 warnings and 0 errors.

## The screen

The sign-in gate opens **first** — before any data is loaded, because nothing in the application can obtain a
`CommandContext` until it closes. Pick an identity and press **Sign in**.

| Area | What it holds |
|---|---|
| Header | screen name · tenant (from the `tid` claim) · signed-in user and roles · the correlation id of the current command |
| Work queue (top left) | the tenant's work orders and the three sensitive commands, the status line, the denial banner |
| Audit log (bottom left) | the deliverable: Time · User · Tenant · Permission demanded · Result · Detail, with the three filters and the dark footer |
| Live activity trace (top right) | every layer's decision: `UI →` `Identity:` `Security:` `Service:` `Data:` `Audit:` `UI ←` |
| Customer note (bottom right) | one untrusted note, rendered escaped, sanitized, or (deliberately) raw |
| Bottom bar | switch identity · the three failure paths · the recoveries · the AllowHtml review · clear trace |

## The directory to sign in as

| Account | Tenant | Role | Use it for |
|---|---|---|---|
| `m.weber` | fabrikam | Manager | approvals, and requesting an export |
| `l.romero` | fabrikam | Technician | **the walkthrough's denied export** |
| `j.kim` | fabrikam | Auditor | reading the whole trail; releasing someone else's export |
| `d.singh` | fabrikam | Admin | the separation-of-duties refusal |
| `svc.import` | fabrikam | ServiceAccount | a non-human caller with edit rights only |
| `ana.ops` | **contoso** | Manager | the cross-tenant path — a real manager, wrong tenant |
| `t.novak` | fabrikam | *(none)* | authenticated, entitled to nothing |

## What to click

### The walkthrough's failure path (do this first)

| # | Action | Path | What you should see |
|---|---|---|---|
| 1 | Sign in as **`l.romero`** | success | header reads `l.romero · Technician`; **Approve** and **Export data** are not on the toolbar at all — `Has` said no |
| 2 | **Break the UI: enable Export** | the UI bug | both buttons appear, amber banner: the screen now offers what the caller may not do |
| 3 | **⤓ Export data** | **failure** | red banner *"You don't have permission to export data."*; footer `UnauthorizedAccessException — Missing permission: ExportData · thrown by PermissionService.Demand · nothing left the server`; the trace shows `Service: ExportService…Demand(ExportData…)` then `Security: …DENIED`; **the denial is the first row of the audit log** |
| 4 | Set **Result: DENIED** | the probing view | only refusals — including the seeded `j.kim · ExportData · DENIED` and `l.romero · ApproveWorkOrders · DENIED` |

### The rest of the paths

| Button | Path | What you should see |
|---|---|---|
| **✔ Approve** (as `m.weber`) | success | `WO-…` approved; **two** audit rows — `ApproveWorkOrders · OK` (the decision) and `ApproveWorkOrder · OK` (the change), same correlation id |
| **⤓ Export data** (as `m.weber`) | progress → held | 36 rows is above the 25-row threshold: amber banner, `ExportData · PENDING · awaiting a second approver`, nothing exported |
| **Approve pending export** (as `d.singh`, on his own request) | **failure** | *"An export cannot be approved by the person who requested it."* — the permission passed, the rule about people did not; `ApproveExport · DENIED · separation of duties` |
| **Approve pending export** (as `j.kim`) | recovery | released: `ApproveExport · OK` and `ExportData · OK · completed after approval` |
| **Fail: cross-tenant approve** | **failure** | the tenant guard rejects a contoso record from a fabrikam session *before* the role store is consulted; `ApproveWorkOrders · DENIED · cross-tenant: session 'fabrikam' requested 'contoso'` |
| **Fail: render note as raw HTML** | **failure** | the customer note becomes live markup — a red defacement line appears and the browser tab is renamed; `UnsafeHtmlRender · FAILED` in the audit log |
| **Recover: escape / sanitize** | recovery | first `AllowHtml = false` (the characters are shown), click again for the allow-list sanitizer: `<b>` survives, the `onerror` and the `javascript:` link do not |
| **AllowHtml review** | review | walks the live control tree by reflection; green when nothing carries unsanitized untrusted text, red naming the finding right after the unsafe render |
| **Switch identity (SSO)** | — | signs out, reopens the gate; the screen locks in between, and the audit log keeps every identity's entries |
| **Clear trace** | — | empties the trace card only — the audit log is append-only and no button clears it |

Sign in as **`t.novak`** to see "authenticated, entitled to nothing": amber banner naming the unmapped group
`CorpVPN-Users`, no command buttons, an empty queue because `Demand(ViewWorkOrders)` refused.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Identity mapping design | [`EnterpriseOps/docs/IdentityMappingDesign.md`](EnterpriseOps/docs/IdentityMappingDesign.md) + [`identity-flow.svg`](EnterpriseOps/docs/identity-flow.svg) · code: `Security/SsoIdentityProvider.cs`, `Security/ClaimsMapper.cs`, `Security/SessionContext.cs` |
| 2 | Permission matrix | [`EnterpriseOps/docs/PermissionMatrix.md`](EnterpriseOps/docs/PermissionMatrix.md) · code: `Security/RolePermissionStore.BuildMatrix()` |
| 3 | Permission service implementation | [`EnterpriseOps/docs/PermissionServiceImplementation.md`](EnterpriseOps/docs/PermissionServiceImplementation.md) · code: `Security/SecurityArchitecturePatterns.cs`, `Security/PermissionService.cs`, `Security/TenantGuard.cs` |
| 4 | Audit log screen | [`EnterpriseOps/docs/AuditLogScreen.md`](EnterpriseOps/docs/AuditLogScreen.md) · code: `UI/AuditLogPage.*`, `Security/AuditLog.cs`, `Services/AuditQueryService.cs` |
| 5 | Security hardening checklist | [`EnterpriseOps/docs/SecurityHardeningChecklist.md`](EnterpriseOps/docs/SecurityHardeningChecklist.md) · code: `Security/HardeningChecklist.cs`, `Services/SecurityReviewService.cs`, `Security/HtmlText.cs` |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it locally once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5210` |
| Enterprise authentication simulation | `Security/SimulatedSsoIdentityProvider` (a claim list, no password) · `UI/SignInGate.*` · `Services/SignInService.SignInAsync` |
| Claims-to-permission mapping | `Security/ClaimsMapper` (group → `Role`) → `RolePermissionStore.SetMembership` → the matrix |
| Tenant-role enforcement | `Security/TenantGuard.DemandTenant`, called first inside `PermissionService.Evaluate`; `AuditLog.Snapshot` is tenant-filtered |
| Service-level authorization | `WorkOrderService.LoadQueueAsync` / `.ApproveAsync`, `ExportService.RequestExportAsync` / `.ApproveExportAsync` — each opens with `Demand` |
| Safe HTML review | `Security/HtmlText` (escape · allow-list sanitize) · `Services/NoteRenderService` decides · `Services/SecurityReviewService.Review` inspects the live tree |
| Export approval | `Services/ExportService` — threshold + `ApproveExport` + separation of duties |
| Audit logging for sensitive actions | `Security/AuditLog` written by `PermissionService.Demand` and by each service next to its change |
| Show every path | success · held-for-approval · three refusals · three recoveries, each with a banner, a status colour, a footer line and audit rows |
| Review & run: production-readiness note | `docs/SecurityHardeningChecklist.md`, and the "what is missing here" section of `docs/AuditLogScreen.md` |

## Where things live

```
Module 10/
└─ EnterpriseOps/
   ├─ Domain/          WorkOrder · WorkOrderStatus · Priority · Tenant · WorkQueueRow · WorkOrderNote
   ├─ Security/        SecurityArchitecturePatterns.cs   Permission · IPermissionService · IRolePermissionStore · ITenantGuard
   │                   PermissionService.cs              Demand / Has — tenant guard, then the role store, always audited
   │                   RolePermissionStore.cs            the permission matrix + per-tenant membership
   │                   TenantGuard.cs                    CrossTenantAccessException
   │                   ClaimsMapper.cs                   groups → roles, tid → tenant, unknown → nothing
   │                   SsoIdentityProvider.cs            the simulated OIDC boundary + the directory
   │                   SessionContext.cs                 identity → session → CommandContext (the only source)
   │                   CommandContext.cs   Role.cs
   │                   AuditLog.cs                       append-only, shared, tenant-filtered on read
   │                   HtmlText.cs                       Escape · Sanitize (allow-list) · DescribeMarkup
   │                   HardeningChecklist.cs             the checklist as code
   ├─ Services/        SignInService · WorkOrderService · ExportService · AuditQueryService
   │                   NoteRenderService · SecurityReviewService · ServiceRegistry · CommandResult
   ├─ Data/            SeedData · InMemoryWorkOrderRepository · InMemoryNoteStore
   ├─ Diagnostics/     ActivityTrace
   ├─ UI/              AuditLogPage.cs / .Designer.cs    the screen the walkthrough builds
   │                   SignInGate.cs / .Designer.cs      the gate (simulated SSO)
   ├─ docs/            the five deliverables + identity-flow.svg
   ├─ Program.cs       session composition root — creates the graph, opens the page, signs nobody in
   └─ Startup.cs       Kestrel host (app.UseWisej(); *.json is never served)
```

Data: 40 in-memory work orders (36 fabrikam, 4 contoso), 3 customer notes on `WO-1002` (the third carries the
injection payload), and 12 seeded audit entries including the five the video shows.

## Student review questions, answered against this sample

**Where is authorization enforced?**
In the service that executes the action, as the first non-trivial statement of every public method:
`LoadQueueAsync` opens with `Demand(ViewWorkOrders)`, `ApproveAsync` with
`Demand(ApproveWorkOrders, order.TenantId)`, `RequestExportAsync` with `Demand(ExportData, context.TenantId)`.
`PermissionService.Demand` checks the tenant guard first — so a cross-tenant attempt never reaches the role
store — then the matrix, and it throws naming the missing permission. Every one of those calls writes an audit
entry, granted or denied. The screen contains exactly three authorization-shaped lines, all of them `Has` calls
in `ApplyPermissionsToUi`, and all of them are convenience: they hide buttons, they protect nothing. The one
deliberate variation is `AuditQueryService`, which uses `Has(ViewAuditLog)` to choose the *scope* of a query
rather than to refuse it, while the tenant filter it sits on is unconditional.

**Which UI elements render HTML?**
Press **AllowHtml review** and the running screen answers for itself: `SecurityReviewService` walks the control
tree by reflection and reports every control **and grid column** exposing a public `bool AllowHtml` — in
Wisej.NET that is `Label`, `ButtonBase`, `ListBox`, `ComboBox`, `GroupBox`, `TabPage`, `ToolTip`,
`DataGridViewColumn` and more. What reflection cannot know is where each surface's text comes from, so that is
declared in `SecurityReviewService.TextSources` and reviewed like code; anything undeclared is reported as
author-written constant text, so a control added next week still appears for judgement. On this screen the
surfaces carrying text from outside are `lblNote` (a customer note), `lblNoteSource`, `lstTrace` (which quotes
provider group names), `colUser` and `colDetail`, `lblUser` and `lblTenant` (claim values) and `lblBanner`. All
of them keep `AllowHtml = false` except `lblNote` when the note is deliberately rendered unsafely, or when it
goes through the allow-list sanitizer.

**Can an enabled button bypass service rules?**
No, and there is a button that proves it. **Break the UI: enable Export** forces `btnExport` and `btnApprove`
visible and enabled for a caller who holds neither permission — the same thing a merge mistake, a stale cache or
a tampered client does. Clicking **Export data** afterwards reaches exactly the same
`ExportService.RequestExportAsync`, which demands `ExportData` before it reads a single row, throws
`Missing permission: ExportData`, writes the denial to the audit log, and returns a result the screen renders as
a red banner. Nothing was read, nothing left the server, and the attempt is on the record. A hidden button is a
courtesy to the user; the check that matters runs where the action executes.

## Instructor acceptance criteria, answered against this sample

- **Follows the course architecture baseline** — one project, folder-per-layer (`Domain` · `Security` ·
  `Services` · `Data` · `Diagnostics` · `UI` · `docs`), namespaces following the folders, services built once
  per session in `ServiceRegistry`, typed commands and results (`CommandResult`, `ExportResult`,
  `WorkQueueResult`, `AuditQueryResult`), projections (`WorkQueueRow`, `AuditRow`) bound to the grids instead of
  entities, and no user or tenant state in a static field — with one documented exception, the application-scoped
  `AuditLog.Shared`, which is locked on write and tenant-filtered on read.
- **UI event handlers remain thin and explainable** — every handler in `AuditLogPage.cs` is a `BeginBusy` /
  `await one service call` / `Show…` / `catch` / `finally EndBusy` sandwich. The longest one is nine statements.
  No handler decides a permission, builds a claim, chooses an encoding or filters a grid.
- **Service-level logic can be reviewed without opening the designer** — the whole security story is in
  `Security/` and `Services/`: what may be demanded (`SecurityArchitecturePatterns.cs`), how it is decided
  (`PermissionService.cs`), who holds what (`RolePermissionStore.cs`), what is recorded (`AuditLog.cs`), how
  untrusted text is encoded (`HtmlText.cs`, `NoteRenderService.cs`). The `.Designer.cs` files contain layout
  and one `ToolTipText` per button naming the service call behind it.
- **At least one failure path is demonstrated** — four, each with its recovery: the wrongly-enabled Export
  (recovery: sign in as a Manager), the cross-tenant approval (recovery: the same command on an own-tenant
  record), the raw-HTML render (recovery: escape, then sanitize), and the requester approving their own export
  (recovery: a second person releases it).
- **State ownership, security implications and production behaviour** — the screen owns UI state only; the
  session owns identity; the role store owns membership; the audit log is application-scoped on purpose and says
  why in its own doc comment. What this sample is *not* is written down too: the audit log is in memory and
  therefore not tamper-evident, the sanitizer is a lab-sized allow-list rather than a maintained library, and
  cookies, CSP, secure headers, uploads and the reverse proxy are deployment-owned items on the checklist rather
  than code in this module.

## Verified / unverified

Verified while building the Application Integration and Foundations course samples (same framework build), and
used here:

- `await form.ShowDialogAsync()` with no blocking `ShowDialog`, `DialogResult` set inside the dialog before
  `Close()` — the sign-in gate.
- `async void` handlers with `await Task.Delay(…)`, `try/catch/finally`, and `Application.Update(this)` after
  the awaits.
- `AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`.
- `DataGridView` with `AutoGenerateColumns = false`, explicit columns, `DataSource` bound to a `List<T>`,
  `FullRowSelect`, `Rows[i].DefaultCellStyle.BackColor/ForeColor`.
- `Application.Session.<name> = …` as a dynamic per-session bag; `Application.MainPage = new UI.<Page>()`.
- Fonts `new Font("default", 12F, FontStyle.Bold)` / `new Font("monospace", 9F)`, `Label.TextAlign` with
  `System.Drawing.ContentAlignment`, `Panel.BorderStyle = Wisej.Web.BorderStyle.Solid`.

Unverified — implemented from the Wisej.NET XML documentation and reflection over `Wisej.Framework.dll`, build
passes, please confirm at runtime:

- `Label.AllowHtml` (and the `AllowHtml` property on `ButtonBase`, `ListBox`, `ComboBox`, `GroupBox`, `TabPage`,
  `DataGridViewColumn` …) — the whole safe-HTML demonstration and the reflection-based review depend on it. In
  particular: that `AllowHtml = false` renders the characters of `<b>urgent</b>` rather than interpreting them,
  and that `AllowHtml = true` interprets them.
- `DataGridViewRow.DataBoundItem` for reading the selected projection (`SelectedRow`).
- `ComboBox.DropDownStyle = ComboBoxStyle.DropDownList` with `Items.Add(string)` and `SelectedIndexChanged`.
- `Label.BorderStyle` / `Label.Padding` on the mapped-identity box of the gate.
