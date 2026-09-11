# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 3

Local lab build for **Advanced Module 3 · Advanced Session, State, Tenant & Concurrency Design**. It follows
the walkthrough video *"Make EnterpriseOps tenant-aware"*: a tenant-aware `SessionContext` that owns who the
user is and which customer they are working in, a `CommandContext` with a correlation id on every command, a
`TenantGuard` in every service, optimistic concurrency on work orders, and the `ConflictDialog` that explains a
stale edit and offers **Reload · Compare · Cancel**.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine, with in-memory data.

## Run it

```bash
cd "D:\Projects\LearnWisej-Samples\Enterprise Wisej.NET Course\Module 3\EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5203
```

Then open <http://localhost:5203>. (Visual Studio: open `EnterpriseOps.slnx`, press F5 — the port is in
`Properties/launchSettings.json`.)

## What to click

The screen: the header with `cboTenant` (only the tenants the signed-in user's claims entitle them to), the work
queue for the current tenant (`dgvWorkQueue`, with a **Version** column), **Open selected in editor**, the editor
(`txtTitle`, `cboStatus`, read-only `txtVersion`, **Save**), a banner for validation and errors, and the dark
footer `lblStatusBar`.

| Control | Path | What you should see |
|---|---|---|
| **Open selected in editor** | success | `txtVersion` shows the token the record was loaded at, e.g. `v7`; footer `Editing — expected version v7` |
| **Save** | success | footer `Saved — v7 → v8 · correlation …`; the queue's Version column moves |
| **Save** (empty title) | validation | amber banner *"A work order needs a title."* — the service rejected it, nothing written |
| **Save** (after another tab saved) | failure + recovery | footer `Save rejected — expected v7, found v8 · correlation …`, then the conflict dialog |
| `cboTenant` → another tenant | success | the editor is dropped; the queue reloads with a different set of rows |

In the conflict dialog: **Reload latest** (discard your edits, take the current record), **Compare changes**
(field by field — the dialog stays open), **Cancel** (nothing saved, nothing lost, your edit stays on screen).

## Reproduce the video's failure path

Open <http://localhost:5203> in **two browser tabs** — two sessions, two `SessionContext` objects, one store.

1. in both tabs select **2002 · Repair loading dock pump**, click **Open selected in editor** — `v7`;
2. tab 1: change the status, click **Save** — the store moves to `v8`;
3. tab 2: change the title to *Repair loading dock pump — urgent*, click **Save** — rejected, and the conflict
   dialog explains why;
4. click **Compare changes** — Title, Status and Version differ;
5. click **Reload latest** — the editor rebinds at `v8`, and the correlation ids tie every step to the audit log.

## Deliverables

| # | Deliverable | Where |
|---|---|---|
| 1 | Tenant-aware SessionContext | [`docs/TenantAwareSessionContext.md`](EnterpriseOps/docs/TenantAwareSessionContext.md) · `Security/SessionContext.cs`, `Security/VerifiedIdentity.cs`, `Security/IdentityProvider.cs`, `Services/TenantDirectory.cs`, `Security/TenantGuard.cs` |
| 2 | CommandContext with correlation ID | [`docs/CommandContextCorrelationId.md`](EnterpriseOps/docs/CommandContextCorrelationId.md) · `Security/CommandContext.cs`, `SessionContext.BeginCommand`, `Security/AuditTrail.cs` |
| 3 | Static-state audit report | [`docs/StaticStateAuditReport.md`](EnterpriseOps/docs/StaticStateAuditReport.md) · `Security/SharedStateAttribute.cs` |
| 4 | Optimistic concurrency implementation | [`docs/OptimisticConcurrency.md`](EnterpriseOps/docs/OptimisticConcurrency.md) · `Domain/ConcurrencyToken.cs`, `Data/WorkOrderStore.TrySave`, `Services/WorkOrderService.SaveAsync` |
| 5 | Conflict-resolution dialog | [`docs/ConflictResolutionDialog.md`](EnterpriseOps/docs/ConflictResolutionDialog.md) · `UI/ConflictDialog.cs` + `.Designer.cs`, `Services/ConflictResolutionService.cs` |
| — | State ownership map | [`docs/StateOwnershipMap.svg`](EnterpriseOps/docs/StateOwnershipMap.svg) |

## Lab steps → where in the code

| Lab step | Where |
|---|---|
| Open the project and run it locally once | `EnterpriseOps.slnx` · `dotnet run -f net10.0 --urls http://localhost:5203` |
| Tenant selection | `UI/WorkOrderEditorPage.Designer.cs` (`cboTenant`) → `cboTenant_SelectedIndexChanged` → `SessionContext.SwitchTenant` |
| Tenant-aware services | `Security/TenantGuard.cs`, called first in `WorkOrderService.OpenAsync` and twice in `SaveAsync` |
| Correlation IDs | `SessionContext.BeginCommand` → `CommandContext` → audit, footer, banner, dialog footnote |
| Optimistic concurrency on work orders | `Domain/ConcurrencyToken.cs`, `Domain/WorkOrder.Version`, `Data/WorkOrderStore.TrySave` |
| Conflict dialog with Reload / Compare / Cancel | `UI/ConflictDialog.cs`, decisions in `Services/ConflictResolutionService.cs` |
| Show every path (success, validation, error) | Save → footer; empty title → banner; stale save → conflict dialog; unexpected errors → generic banner + correlation id |
| Review & run — production-readiness note | the **Race-condition review** section of [`docs/StaticStateAuditReport.md`](EnterpriseOps/docs/StaticStateAuditReport.md) |

## Where things live

```
Module 3/
└─ EnterpriseOps/
   ├─ UI/           WorkOrderEditorPage (queue + editor), ConflictDialog (Reload · Compare · Cancel)
   ├─ Domain/       WorkOrder (+ Version), ConcurrencyToken, Tenant, WorkOrderStatus, WorkQueueRow
   ├─ Services/     WorkOrderService, ConflictResolutionService, ConflictInfo, SaveWorkOrderCommand/Result,
   │                WorkOrderEditModel, TenantDirectory, ServiceRegistry (per-session, no .Current)
   ├─ Security/     SessionContext, CommandContext, TenantGuard, AuditTrail, SharedStateAttribute,
   │                IdentityProvider, VerifiedIdentity
   ├─ Data/         WorkOrderStore (shared, locked, clones on read), SeedData (2002 = the video's record)
   ├─ Diagnostics/  ActivityTrace (server log via System.Diagnostics.Trace), ErrorLog
   ├─ Resources/    UiText
   └─ docs/         the five deliverables + the state ownership map
```

## Student review questions, answered against this sample

- **Can tenant ID be spoofed from a UI control?**
  No. `cboTenant` is filled from the session's `EntitledTenants`, and whatever comes back from the browser goes
  through `SessionContext.SwitchTenant`, which re-checks the entitlement against the verified claims. From there
  the tenant only ever travels on the `CommandContext`, and `TenantGuard.DemandTenant` compares it with the
  tenant of the record before any business logic runs. Tenant isolation is not a grid filter: a filter is a
  visual condition, the guard is a rejection.

- **What happens when the same work order is edited in two sessions?**
  Both sessions load the record and its version. The first save succeeds and moves `v7 → v8`. The second save
  arrives claiming `v7`, `WorkOrderStore.TrySave` compares inside its lock, sees `v8`, and writes **nothing** —
  the service returns a `ConflictInfo` rather than throwing, and the screen opens the conflict dialog. The choice
  is recorded in the audit trail with the correlation id.

- **Which services can safely be shared?**
  The ones that hold no per-user state and synchronize what they do hold: `WorkOrderStore.Shared` and
  `AuditTrail.Shared`, plus immutable reference data (`TenantDirectory`, `UiText`). Everything that touches
  identity is per session — which is why there is no `ServiceRegistry.Current`.

## Instructor acceptance criteria, answered

- **Follows the course architecture baseline.** Folder-per-layer with matching namespaces, typed commands and
  results across the boundary, entities never leaving `Data`, `Page`/`Form` + `.Designer.cs` for every screen.
- **UI event handlers remain thin and explainable.** Every handler in `WorkOrderEditorPage.cs` is begin-busy →
  one service call → show the result → `catch` → `finally`; `ConflictDialog`'s handlers are four lines or fewer.
- **Service-level logic can be reviewed without opening the designer.** The tenant check, validation,
  concurrency check, conflict summary, field comparison and audit entry are all in `Services/` and `Security/`.
- **At least one failure path is demonstrated.** The stale save from a second tab → conflict dialog → Reload.
  Validation (empty title) and unexpected errors (generic banner + correlation id) are handled the same way.
- **The student can explain state ownership, security implications and production behaviour.** See
  [`docs/StateOwnershipMap.svg`](EnterpriseOps/docs/StateOwnershipMap.svg), `TenantAwareSessionContext.md` and the
  race-condition review in `StaticStateAuditReport.md`.

## Verified / unverified

Relied on and **not yet verified at runtime** (they compile; confirm on screen):

- `Application.Session.Context = sessionContext` and `Application.SessionId` (`StoreSessionContext`,
  `CurrentSessionId`), both wrapped in `try/catch`.
- `Wisej.Web.FlowLayoutPanel` with per-button `Margin` for the dialog's action row (`pnlActions`).
- `ComboBox.Items.Add(object)` with `Tenant` objects rendered through `ToString()`.
- `DataGridView.DataSource` bound to an `IReadOnlyList<T>` (`dgvVersions`, `dgvCompare`) and
  `DataGridView.CurrentRow.Index` as the index into the bound list (`SelectedWorkOrderId`).
