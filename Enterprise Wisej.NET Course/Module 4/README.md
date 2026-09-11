# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 4

**Real data architecture: EF Core, transactions, repositories & commands.**

The EnterpriseOps Command Center's **Approvals** screen, in front of a real Entity Framework Core
database. The fake `WorkOrderService` of the earlier modules is gone: Create, Update, Approve, Search
and Audit are EF Core operations behind a service boundary, each with an explicit transaction boundary
and every database failure mapped to a result code, a user message and an audit row.

The screen is the walkthrough's approve panel next to the work queue: pick a work order, type a comment,
click **Approve**. The handler builds an `ApproveWorkOrderCommand`, hands it to the service and shows the
`CommandResult`; the service authorizes, creates a short-lived `DbContext`, opens a transaction, loads by
tenant + id, checks the version, validates the transition, persists, audits and commits.

**EF Core.** `Microsoft.EntityFrameworkCore.Sqlite` **10.0.12**, over an in-memory SQLite database
(`DataSource=:memory:`) created fresh per session: no file, no server, no network.

---

## Run it

```
cd "Module 4/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5204
```

Then open <http://localhost:5204>. Every browser session gets its own database and its own
`SessionContext` — nothing is shared and nothing is static.

Signed in as **ana.ops** (Manager), tenant **fabrikam**, with 60 seeded work orders across three
tenants. Work order **WO-2002 — Repair loading dock pump** is selected on load: `OnHold`, `v8`, exactly
as the video shows it.

---

## What to click

| Control | Path | What you should see |
|---|---|---|
| **Approve** with `WO-2002` selected | failure (the video's first click) | Red banner **"This work order is on hold — resolve the hold before approving."**, detail `WO_STATE_INVALID · transaction rolled back · nothing persisted`, status bar `CommandResult.Fail — WO_STATE_INVALID · rolled back · audited` |
| **Approve** with an `InProgress` row selected | success (the video's second click) | Green banner **"Work order approved."**, detail `committed · audit written · correlation …`, status bar `CommandResult.Ok — committed · v3 → v4 · audited`; the grid refreshes and the row is `Completed` with a new version |
| **Approve** with the comment box emptied | validation | `VALIDATION_FAILED` — "An approval comment is required." Validation happens in the service, not in the screen |
| **Cancel** | UI only | Clears the comment and the banner; nothing is sent to the service |
| **txtSearch** + **Search** | read side | The grid reloads with `WorkQueueRow` projections (one short-lived `DbContext`, `AsNoTracking`) |
| **Audit log** | dialog | `await dialog.ShowDialogAsync()` over `AuditLogRow` projections: every committed and every rejected command with its code, user and correlation id. The query service checks the `ReadAudit` permission before running anything |

The services write each step (`Security:`, `Data:`, `Service:`, `Audit:`) to the server log through
`Services/ActivityTrace.cs` → `System.Diagnostics.Trace` (the debugger's Output window).

---

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx`, `EnterpriseOps/EnterpriseOps.csproj` (`net10.0-windows;net10.0`, `Wisej-4` 4.1.0 + `Microsoft.EntityFrameworkCore.Sqlite` 10.0.12), port 5204 |
| Replace fake persistence with EF Core behind a service boundary | `Services/IWorkOrderCommandService.cs` + `Services/IWorkOrderQueryService.cs` (the boundary) → `Data/WorkOrderCommandService.cs`, `Data/WorkOrderQueryService.cs`, `Data/WorkOrderRepository.cs`, `Data/EnterpriseOpsDbContext.cs`, `Data/SessionDatabase.cs` |
| **Create** | `WorkOrderCommandService.CreateAsync` · `CreateWorkOrderCommand` |
| **Update** | `WorkOrderCommandService.UpdateAsync` · `UpdateWorkOrderCommand` (status edges guarded by `Domain/WorkOrderTransitions.CanChangeStatus`) |
| **Approve** | `WorkOrderCommandService.ApproveAsync` · `ApproveWorkOrderCommand` · `btnApprove_Click` |
| **Search** | `WorkOrderQueryService.SearchAsync` → `PagedResult<WorkQueueRow>` · `btnSearch_Click` |
| **Audit** | in-transaction `repository.AddAudit(…)` + `WorkOrderCommandService.WriteRejectionAuditAsync`; read side `WorkOrderQueryService.GetAuditAsync` → `AuditQueryResult` · `btnAudit_Click` → `UI/AuditLogDialog.cs` |
| Deliverable 1 · Data access boundary diagram | [`docs/DataAccessBoundary.md`](EnterpriseOps/docs/DataAccessBoundary.md) + [`data-access-boundary.svg`](EnterpriseOps/docs/data-access-boundary.svg) |
| Deliverable 2 · DbContext lifetime decision | [`docs/DbContextLifetimeDecision.md`](EnterpriseOps/docs/DbContextLifetimeDecision.md) · code: `Data/SessionDatabase.cs` |
| Deliverable 3 · Command and result classes | [`docs/CommandAndResultClasses.md`](EnterpriseOps/docs/CommandAndResultClasses.md) · code: `Services/Commands/*.cs`, `Services/NullTrace.cs` |
| Deliverable 4 · Transaction example | [`docs/TransactionExample.md`](EnterpriseOps/docs/TransactionExample.md) · code: `WorkOrderCommandService.RunAsync` + `ApproveAsync` |
| Deliverable 5 · Error mapping table | [`docs/ErrorMappingTable.md`](EnterpriseOps/docs/ErrorMappingTable.md) · code: `Data/ErrorMap.cs` |
| Show every path without leaking internals | `ShowResult` / `Warn` / `ShowUnexpected` in `UI/ApprovalsPage.cs` — the banner only renders `CommandResult.UserMessage` and `CommandResult.Detail` |
| Review & run: production-readiness note | "Production readiness" below |

---

## Where things live

```
EnterpriseOps/
  Program.cs                           Application.MainPage = new UI.ApprovalsPage();
  UI/          ApprovalsPage (work queue · approve panel · status bar), AuditLogDialog
  Domain/      WorkOrder, AuditEntry, Tenant, WorkOrderStatus, Priority, WorkOrderTransitions
  Services/    IWorkOrderCommandService, IWorkOrderQueryService, SessionContext,
               IActivityTrace + ActivityTrace (server log) + NullTrace (tests),
               Commands/ (Create/Update/ApproveWorkOrderCommand, CommandContext, CommandResult),
               Queries/  (WorkQueueQuery, WorkQueueRow, PagedResult<T>, AuditLogRow, AuditQueryResult)
  Data/        EnterpriseOpsDbContext, SessionDatabase, SeedData, IWorkOrderRepository, WorkOrderRepository,
               WorkOrderCommandService, WorkOrderQueryService, ErrorMap, DataArchitecturePatterns (the video's file)
  Security/    Permissions (roles, operations, server-side rules)
  docs/        the five deliverables + the boundary SVG
```

---

## Student review questions, answered against this sample

**Can a save be tested without creating a Form?** Yes. `WorkOrderCommandService` takes a
`SessionDatabase`, an `IActivityTrace` and a timeout callback — none of them a UI type. A test constructs
`new SessionDatabase(new NullTrace())`, calls `ApproveAsync(command, context, ct)` and asserts on
`result.ErrorCode`. There is no `using Wisej.Web` under `Domain/`, `Services/` or `Data/`. See
`docs/CommandAndResultClasses.md`.

**What is inside the transaction?** Load the aggregate by tenant + id → set the original `Version` to
what the user saw → validate the transition → write the changed columns → write the audit row → commit.
**Outside** it: the permission check (before it opens) and the *rejection* audit row (after it rolls back,
in its own context and transaction). See `docs/TransactionExample.md`.

**How are concurrency and unique key errors surfaced?** Both are decided by the database and caught at
the boundary. `Version` is `IsConcurrencyToken()`, so a stale update affects 0 rows →
`DbUpdateConcurrencyException` → `WO_CONCURRENCY`. `Number` has the unique index
`UX_WorkOrders_Tenant_Number`, so a duplicate → `DbUpdateException` / `SqliteException` 19 →
`WO_NUMBER_IN_USE`. Both come back as a `CommandResult` with a safe message and are audited without a
stack trace. See `docs/ErrorMappingTable.md`.

---

## Instructor acceptance criteria, answered

* **Course architecture baseline.** Folder-per-layer with matching namespaces, one web project,
  `Program.Main` → `Application.MainPage`, the shared EnterpriseOps vocabulary.
* **Thin UI event handlers.** Every handler in `UI/ApprovalsPage.cs` is: guard → build a command →
  `await` one service call → `ShowResult(result)`, wrapped in `try/catch/finally`. No query, no
  transaction, no `DbContext`.
* **Service logic reviewable without the designer.** `Data/WorkOrderCommandService.cs` reads end to end:
  authorize, open a bounded unit of work, run the body, commit or roll back, map, audit. Domain rules in
  `Domain/WorkOrderTransitions.cs`, security in `Security/Permissions.cs`, failure vocabulary in
  `Data/ErrorMap.cs`.
* **At least one failure path demonstrated.** Approve on `WO-2002` (`WO_STATE_INVALID`, the video's
  failure path) and Approve with an empty comment (`VALIDATION_FAILED`).
* **State ownership, security, production behaviour.** The screen owns UI state only; `SessionContext`
  and `SessionDatabase` are per-session instances that die with the page; the `DbContext` owns one
  operation. Authorization runs server-side before any transaction; every query and command is filtered
  by tenant. Every command is bounded by a timeout, mapped to a stable code and audited with a
  correlation id.

---

## Production readiness

* **Would not change.** The boundary, the command and result classes, the transaction shape, the error
  codes and messages, the per-operation context lifetime, the audit-after-rollback rule.
* **Provider.** Swap `UseSqlite(connection)` for `UseSqlServer(connectionString)` in `SessionDatabase`,
  drop the session-long connection (a real server has a pool), and swap the provider checks in
  `ErrorMap.Map` (SQL Server: 2601/2627 unique, 547 foreign key, −2 timeout). The result codes stay.
* **Concurrency token.** `int Version` becomes `rowversion` (`byte[]`), as the walkthrough's
  `ApproveWorkOrderCommand` sketches.
* **Schema.** `EnsureCreated()` + `SeedData` becomes EF Core migrations applied by the pipeline.
* **Concurrency control.** `SessionDatabase.Gate` exists because one in-memory SQLite connection is
  shared by a session; with a pooled connection it can go.
* **Retries.** A real provider needs a transient-fault policy (EF Core's execution strategy) around the
  transaction.
* **Diagnostics.** The `ActivityTrace` lines become structured log events keyed by the correlation id
  (Module 11).

---

## Verified / unverified

Cookbook facts marked **(unverified)** used here: the `DataGridView` details (`AutoGenerateColumns = false`
with explicit columns, `AutoSizeColumnsMode = Fill` + `FillWeight`, `FullRowSelect`, `Rows[i].Selected`,
`SelectedRows[0].DataBoundItem`, a `BindingSource` over a `List<T>` with `ResetBindings(false)`).

New in this module: `Microsoft.EntityFrameworkCore.Sqlite` 10.0.12 over an open
`SqliteConnection("DataSource=:memory:")`, `EnsureCreated()`, `IsConcurrencyToken()` on an `int`,
`HasIndex(…).IsUnique()`, `Entry(entity).Property(x => x.Version).OriginalValue`,
`Database.BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`, and `SqliteException.SqliteErrorCode == 19`.

## Build

```
cd "Module 4/EnterpriseOps"
dotnet build -nologo -v q
```

Both target frameworks (`net10.0-windows` and `net10.0`) build clean.
