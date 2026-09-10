# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 3

Local lab build for **Advanced Module 3 · Advanced Session, State, Tenant & Concurrency Design**. It follows
the walkthrough video *"Make EnterpriseOps tenant-aware"*: a tenant-aware `SessionContext` that owns who the
user is and which customer they are working in, a `CommandContext` with a correlation id on every command, a
`TenantGuard` in every service, optimistic concurrency on work orders, and the `ConflictDialog` that explains a
stale edit and offers **Reload · Compare · Cancel**.

Four failure paths are wired to buttons, not left as prose: another session saves first (the video's two-session
conflict), a cross-tenant read, a tenant id the dropdown never offered, and the static-state leak — demonstrated
safely and then caught by the audit.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, with in-memory data.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 3\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5203
```

Then open <http://localhost:5203>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.
`dotnet build -nologo -v q` passes for both target frameworks with 0 warnings and 0 errors.

**Open a second browser tab** for the real thing: two tabs are two sessions, two `SessionContext` objects and two
version tokens over one shared store. The **Fail: other session saves** button does exactly what that second tab
does, on one screen.

## What to click

Header: `cboTenant` (only the tenants the signed-in user's claims entitle them to), the signed-in user, and the
correlation id of the command running right now.

| Control | Path | What you should see |
|---|---|---|
| **work queue grid** | — | the work orders of the current tenant only, with a **Version** column |
| **Open selected in editor** | success | `txtVersion` shows the token the record was loaded at, e.g. `v7`; trace `Service: … at v7 · this tab now owns that token` |
| **Save** | success | `v7 → v8`, green banner, the queue's Version column moves |
| **Save** (empty title) | validation | amber banner *"A work order needs a title."*, `Data:` never runs — the service rejected it |
| **Save** (after another session saved) | **failure + recovery** | `Data: WorkOrderStore.TrySave → REJECTED · expected v7, found v8 · nothing written`, then the conflict dialog |
| `cboTenant` → another tenant | success | `Session: SwitchTenant: 'contoso' → 'fabrikam' (entitled)`; the editor is dropped; the queue reloads with a different set of rows |

Bottom bar:

| Button | Path | What you should see |
|---|---|---|
| **Fail: other session saves** | failure setup | a second `SessionContext` (`ben.tech`) signs in, opens the same record, saves `Completed`; banner *"…This tab still holds v7 — the next Save will be rejected, not merged."* |
| **Fail: cross-tenant read** | failure | `Security: TenantGuard REJECTED — session tenant 'contoso' ≠ requested 'fabrikam'`; red banner with the correlation id; `CrossTenantAccessException` in the error log |
| **Fail: spoofed tenant value** | failure | a client value of `northwind` arrives although the dropdown only offers contoso/fabrikam → `SessionContext.SwitchTenant REJECTED`; the dropdown snaps back; the queue is unchanged |
| **Fail: static-state leak** | failure | `LegacyCurrentUser` says `cara.admin@northwind` while this session is `ana.ops@contoso` — the classic leak, with the untouched `SessionContext` printed beside it |
| **Run static-state audit** | the deliverable, live | banner *"Static-state audit — 4 of 14 statics are findings: LegacyCurrentUser.UserId, …"*; every static classified in the trace, findings first |
| **Recover: reload latest** | recovery | re-opens the record at its current version — the same path the dialog's **Reload latest** runs |
| **Clear trace** | — | empties the right-hand card |

In the conflict dialog: **Reload latest** (discard your edits, take the current record), **Compare changes**
(field by field — the dialog stays open), **Cancel** (nothing saved, nothing lost, your edit stays on screen).

The right-hand card is the **Server · live activity trace**: every user action, service decision, data operation,
security rejection and audit entry, tagged `UI →` / `Session:` / `Security:` / `Service:` / `Data:` / `Audit:`
with a timestamp. It is how a reviewer proves from the screen alone that the handler was thin and the service
decided.

## Reproduce the video's failure path

1. select **2002 · Repair loading dock pump**, click **Open selected in editor** — `v7`;
2. change the title to *Repair loading dock pump — urgent*;
3. click **Fail: other session saves** — the store moves to `v8`;
4. click **Save** — rejected, and the conflict dialog explains why;
5. click **Compare changes** — Title, Status and Version differ;
6. click **Reload latest** — the editor rebinds at `v8`, both sessions agree, and the correlation ids tie every
   step to the audit log.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Tenant-aware SessionContext | [`docs/TenantAwareSessionContext.md`](EnterpriseOps/docs/TenantAwareSessionContext.md) · `Security/SessionContext.cs`, `Security/VerifiedIdentity.cs`, `Security/IdentityProvider.cs`, `Services/TenantDirectory.cs`, `Security/TenantGuard.cs` |
| 2 | CommandContext with correlation ID | [`docs/CommandContextCorrelationId.md`](EnterpriseOps/docs/CommandContextCorrelationId.md) · `Security/CommandContext.cs`, `SessionContext.BeginCommand`, `Security/AuditTrail.cs` |
| 3 | Static-state audit report | [`docs/StaticStateAuditReport.md`](EnterpriseOps/docs/StaticStateAuditReport.md) · `Security/StaticStateAudit.cs`, `Security/SharedStateAttribute.cs`, `Security/LegacyCurrentUser.cs`, `Services/StateAuditService.cs` |
| 4 | Optimistic concurrency implementation | [`docs/OptimisticConcurrency.md`](EnterpriseOps/docs/OptimisticConcurrency.md) · `Domain/ConcurrencyToken.cs`, `Data/WorkOrderStore.TrySave`, `Services/WorkOrderService.SaveAsync` |
| 5 | Conflict-resolution dialog | [`docs/ConflictResolutionDialog.md`](EnterpriseOps/docs/ConflictResolutionDialog.md) · `UI/ConflictDialog.cs` + `.Designer.cs`, `Services/ConflictResolutionService.cs` |
| — | State ownership map | [`docs/StateOwnershipMap.svg`](EnterpriseOps/docs/StateOwnershipMap.svg) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it locally once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5203` |
| Tenant selection | `UI/WorkOrderEditorPage.Designer.cs` (`cboTenant`) → `cboTenant_SelectedIndexChanged` → `SessionContext.SwitchTenant` |
| Tenant-aware services | `Security/TenantGuard.cs`, called first in `WorkOrderService.OpenAsync` and twice in `SaveAsync` |
| Correlation IDs | `SessionContext.BeginCommand` → `CommandContext` → trace, audit, banner, dialog footnote |
| Optimistic concurrency on work orders | `Domain/ConcurrencyToken.cs`, `Domain/WorkOrder.Version`, `Data/WorkOrderStore.TrySave` |
| Conflict dialog with Reload / Compare / Cancel | `UI/ConflictDialog.cs`, decisions in `Services/ConflictResolutionService.cs` |
| Show every path (success, validation, error) | bottom bar: 4 failure buttons + the recovery; validation on an empty title; the banner and the dark footer |
| Review & run — production-readiness note | the **Race-condition review** section of [`docs/StaticStateAuditReport.md`](EnterpriseOps/docs/StaticStateAuditReport.md) |

## Where things live

```
Module 3/
└─ EnterpriseOps/
   ├─ UI/
   │  ├─ WorkOrderEditorPage.cs / .Designer.cs   the screen: queue, editor, trace, failure/recovery bar
   │  └─ ConflictDialog.cs / .Designer.cs        the stale-edit dialog: Reload · Compare · Cancel
   ├─ Domain/
   │  ├─ WorkOrder.cs                            entity + Version (the concurrency token)
   │  ├─ ConcurrencyToken.cs                     the token as a value object
   │  ├─ Tenant.cs  WorkOrderStatus.cs  WorkQueueRow.cs
   ├─ Services/
   │  ├─ WorkOrderService.cs                     tenant guard → validation → concurrency check
   │  ├─ ConflictResolutionService.cs            the dialog's decisions: summarize, compare, record the choice
   │  ├─ ConflictInfo.cs  SaveWorkOrderCommand.cs  SaveWorkOrderResult.cs  WorkOrderEditModel.cs
   │  ├─ OtherSessionSimulator.cs                "Session B — tab 2", without a second browser
   │  ├─ StateAuditService.cs                    runs the static-state audit and the leak demonstration
   │  ├─ TenantDirectory.cs                      immutable reference data
   │  └─ ServiceRegistry.cs                      per-session composition root (deliberately no .Current)
   ├─ Security/
   │  ├─ SessionContext.cs                       session state: identity, entitlements, current tenant
   │  ├─ CommandContext.cs                       request state: user, tenant, correlation id, timestamp
   │  ├─ TenantGuard.cs                          rejects cross-tenant access before any business logic
   │  ├─ StaticStateAudit.cs  SharedStateAttribute.cs
   │  ├─ LegacyCurrentUser.cs                    DELIBERATELY BAD: the static that holds "the current user"
   │  ├─ IdentityProvider.cs  VerifiedIdentity.cs  AuditTrail.cs
   ├─ Data/
   │  ├─ WorkOrderStore.cs                       shared, locked, clones on read; TrySave = the version check
   │  ├─ IWorkOrderStore.cs  SeedData.cs         52 work orders across three tenants (2002 = the video's record)
   ├─ Diagnostics/  ActivityTrace.cs  IActivityTrace.cs  ErrorLog.cs
   ├─ Resources/    UiText.cs                    immutable statics — what a static is actually for
   ├─ docs/         the five deliverables + the state ownership map
   ├─ Program.cs                                 session entry point: Application.MainPage = new WorkOrderEditorPage()
   └─ Startup.cs                                 Kestrel host (app.UseWisej())
```

## Student review questions, answered against this sample

- **Can tenant ID be spoofed from a UI control?**
  No. `cboTenant` is filled from the session's `EntitledTenants`, and whatever comes back from the browser goes
  through `SessionContext.SwitchTenant`, which re-checks the entitlement against the verified claims — the
  **Fail: spoofed tenant value** button sends `northwind` and watches it bounce. From there the tenant only ever
  travels on the `CommandContext`, and `TenantGuard.DemandTenant` compares it with the tenant of the record
  before any business logic runs. `SaveWorkOrderCommand` does carry a `TenantId`; the service **checks** it
  rather than trusting it. Tenant isolation is not a grid filter: a filter is a visual condition, the guard is a
  rejection.

- **What happens when the same work order is edited in two sessions?**
  Both sessions load the record and its version. The first save succeeds and moves `v7 → v8`. The second save
  arrives claiming `v7`, `WorkOrderStore.TrySave` compares inside its lock, sees `v8`, and writes **nothing** —
  the service returns a `ConflictInfo` rather than throwing, and the screen opens the conflict dialog, which
  explains the stale edit and offers Reload, Compare and Cancel. The choice is recorded in the audit trail with
  the correlation id. Nothing is silently overwritten and nothing is lost: after **Reload latest** the user
  re-applies their edit on `v8`.

- **Which services can safely be shared?**
  The ones that hold no per-user state and synchronize what they do hold. In this sample that is exactly two:
  `WorkOrderStore.Shared` (rows for every tenant, one lock around every read and write, clones handed out) and
  `AuditTrail.Shared` (append-only, one lock, reads filtered by tenant) — plus genuinely immutable reference
  data, `TenantDirectory` and `UiText`. Both objects say so in a `[SharedState]` attribute that the audit reads
  back. Everything that touches identity is per session: `SessionContext`, `ActivityTrace`, `ErrorLog`,
  `WorkOrderService`, and the `ServiceRegistry` itself — which is why there is no `ServiceRegistry.Current`.

## Instructor acceptance criteria, answered

- **Follows the course architecture baseline.** Folder-per-layer with matching namespaces (`UI`, `Domain`,
  `Services`, `Data`, `Security`, `Diagnostics`, `Resources`), typed commands and results across the boundary,
  entities never leaving `Data`, `Page` + `.Designer.cs` for every screen.
- **UI event handlers remain thin and explainable.** Every handler in `WorkOrderEditorPage.cs` is
  begin-busy → one service call → show the result → `catch` → `finally`. The longest is 29 lines counting its
  guard clause, comments, blank lines and braces; none of them contains a second service call or a decision.
  `ConflictDialog`'s three handlers are four lines or fewer.
- **Service-level logic can be reviewed without opening the designer.** The tenant check, the validation, the
  concurrency check, the conflict summary, the field comparison and the audit entry are all in
  `Services/` and `Security/`. Open `WorkOrderService.SaveAsync` alone and the whole rule is on one screen.
- **At least one failure path is demonstrated.** Four, each with a recovery: the stale save (→ conflict dialog →
  Reload), the cross-tenant read (→ guard rejection, generic message, correlation id), the spoofed tenant value
  (→ session refuses, nothing changes), the static-state leak (→ the audit names it as a finding).
- **The student can explain state ownership, security implications and production behaviour.**
  [`docs/StateOwnershipMap.svg`](EnterpriseOps/docs/StateOwnershipMap.svg) maps all eight scopes onto files in
  this project; the security implications are in `TenantAwareSessionContext.md` and `StaticStateAuditReport.md`;
  the production behaviour — what a second tab does, what a background job must carry, why no `await` happens
  inside a lock — is in the race-condition review.

## Verified / unverified

Used from the course cookbook and **verified** on this framework build by earlier course samples: `Page` as the
main screen with `Application.MainPage`, `AlertBox.Show(…, alignment: TopRight, autoCloseDelay: 4000)`,
`await dialog.ShowDialogAsync()` in an `async void` handler with `Application.Update(this)` after the awaits,
`async void` + `Task.Delay`, `DataGridView` with `AutoGenerateColumns = false` and explicit
`DataGridViewTextBoxColumn`s bound to a projection, the `"default"` / `"monospace"` fonts,
`Panel.BorderStyle = BorderStyle.Solid`, `ListBox` trace card, `try/catch/finally` handler shape.

Relied on and **not yet verified at runtime** (they compile; the reviewer should confirm on screen):

- `Application.Session.Context = sessionContext` and `Application.SessionId` — the per-session bag
  (`UI/WorkOrderEditorPage.StoreSessionContext`, `CurrentSessionId`). Both are wrapped in `try/catch` so a
  missing session degrades to a generated id and a trace line rather than a failure.
- `Wisej.Web.FlowLayoutPanel` with per-button `Margin` for the dialog's action row (`pnlActions` — the control
  type the walkthrough's properties panel shows).
- `ComboBox.Items.Add(object)` with `Tenant` objects rendered through `ToString()`, plus
  `ComboBoxStyle.DropDownList`.
- `DataGridView.DataSource` bound to an `IReadOnlyList<T>` whose runtime type is `List<T>`
  (`dgvVersions`, `dgvCompare`).
- `DataGridView.CurrentRow.Index` used as the index into the bound list (`SelectedWorkOrderId`).
