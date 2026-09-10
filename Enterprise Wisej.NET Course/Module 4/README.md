# EnterpriseOps · Enterprise Wisej.NET: Architecture to Cloud · Module 4

**Real data architecture: EF Core, transactions, repositories & commands.**

The EnterpriseOps Command Center's **Approvals** screen, in front of a real Entity Framework Core
database. The fake `WorkOrderService` of the earlier modules is gone: Create, Update, Approve, Search
and Audit are now EF Core operations behind a service boundary, each with an explicit transaction
boundary and every database failure mapped to a result code, a user message and an audit row.

The screen is the walkthrough's `ApprovePanel`, grown into a working page: pick a work order, type a
comment, click **Approve**, and watch the trace card show the whole operation — authorize, create a
short-lived `DbContext`, `BEGIN`, load by tenant + id, check the version, validate the transition,
persist, audit, `COMMIT` — while the handler that started it stays two lines long.

**EF Core restored.** `Microsoft.EntityFrameworkCore.Sqlite` **10.0.12** resolved from NuGet and is what
this sample runs on (`obj/project.assets.json` → `Microsoft.EntityFrameworkCore.Sqlite/10.0.12`,
`Microsoft.EntityFrameworkCore.Sqlite.Core/10.0.12`). The hand-written-repository fallback was **not**
needed. The database is an in-memory SQLite database (`DataSource=:memory:`) created fresh per session:
no file, no server, no network.

---

## Run it

```
cd "Module 4/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5204
```

Then open <http://localhost:5204>. Every browser session gets its own database, its own
`SessionContext` and its own trace — nothing is shared and nothing is static.

Signed in as **ana.ops** (Manager), tenant **fabrikam**, with 60 seeded work orders across three
tenants. Work order **WO-2002 — Repair loading dock pump** is selected on load: `OnHold`, `v8`, exactly
as the video shows it.

---

## What to click

### The Approve card (the walkthrough's panel)

| Control | Path | What you should see |
|---|---|---|
| **Approve** with `WO-2002` selected | failure | Red banner **"This work order is on hold — resolve the hold before approving."**, detail `WO_STATE_INVALID · transaction rolled back · nothing persisted`, status bar `CommandResult.Fail — WO_STATE_INVALID · rolled back · audited`. The trace shows `validate transition OnHold → Completed ✗` then `ROLLBACK (WO_STATE_INVALID)` and an `Audit: Approve Rejected` line written **after** the rollback |
| **Approve** with any `InProgress` row selected | success | Green banner **"Work order approved."**, detail `committed · audit written · correlation …`, status bar `CommandResult.Ok — committed · v3 → v4 · audited`; the grid refreshes and the row is `Completed` with a new version |
| **Approve** with the comment box emptied | validation | `VALIDATION_FAILED` — "An approval comment is required." The trace shows the transaction opening and rolling back: validation happens in the service, not in the screen |
| **Cancel** | UI only | Clears the comment and the banner. No trace line from any layer below `UI →` — nothing was sent |
| **cboTenant** / **Search** / **row selection** | read side | `Data: SELECT … WHERE TenantId='…' … → n of m rows (AsNoTracking, projected to WorkQueueRow)`; one `DbContext` created and disposed per query |

### The bottom bar

| Button | Path | What you should see |
|---|---|---|
| **Create work order** | success | A new `WO-90nn` is inserted and audited in one transaction; the trace shows `INSERT WorkOrders … the UNIQUE index decides`, `SaveChanges affected 1 row(s)`, `INSERT AuditEntries`, `COMMIT`; the row appears in the grid |
| **Fail: duplicate number** | failure — provider exception | Creates a work order with the **selected row's** number: `Data: ROLLBACK ← DbUpdateException / SqliteException`, `Service: ErrorMap: DbUpdateException → WO_NUMBER_IN_USE`, banner "That work order number is already in use for this tenant. Choose another number." |
| **Fail: stale version** | failure — optimistic concurrency | Approves an `InProgress` row with `ExpectedVersion = v−1`. The trace says `check concurrency: user saw v2, database has v3 → MISMATCH (the database will reject the UPDATE)`, then `DbUpdateConcurrencyException → WO_CONCURRENCY`. The database decided, not an `if` |
| **Fail: slow query (timeout)** | failure — timeout | The next command sleeps 1500 ms inside its transaction with a 400 ms budget: `simulated slow query: 1500 ms inside the transaction…`, `ROLLBACK ← TaskCanceledException`, `DB_TIMEOUT`, and a user message carrying the **correlation id** to quote to support |
| **Sign in as ben.tech** | failure — authorization, then recovery | Switches the user to a Technician and immediately re-runs the same command: `Security: authorize ben.tech (Technician) → Approve DENIED`, `PERMISSION_DENIED`, and **no `BEGIN TRANSACTION` line at all**. Click again (now **Sign in as ana.ops**) to come back |
| **Batch approve (one tx each)** | progress | Six candidates, six commands, six transactions. The status bar counts `n committed · m rejected · k to go` between transactions (`Application.Update`); `InProgress` rows commit, `OnHold` rows are rejected, nothing is half-saved |
| **Wrong lifetime: session DbContext** | the anti-pattern | Reads through a `DbContext` held in a field. First click: the header's `live` counter sticks at 1. Approve/update that row, click again: the field-held context still reports the **old** status and version next to a fresh context's truth — "STALE" |
| **Recover: dispose the context** | recovery | Disposes the field-held context, releases its tracked entities, `live` returns to 0, and the next read is correct again |
| **Audit log** | dialog | `await dialog.ShowDialogAsync()` over `AuditLogRow` projections: every committed and every rejected command with its code, user and correlation id. As **ben.tech** the query service refuses before running anything: `Security: authorize ben.tech (Technician) → ReadAudit DENIED (no query was run)` |
| **Clear trace** | — | Empties the trace card. The database and the audit log are untouched |

### Reading the header bar

`tenant: fabrikam · Signed in: ana.ops · Manager · ctx 34 · live 0 · corr 7c41aa90`

`ctx` is how many `DbContext` instances this session has created; `live` is how many are still alive.
**Between operations `live` must read 0** — that is the lifetime decision as a number you can watch.
`corr` is the correlation id of the last command: it appears in the banner, in the trace and in the
audit row.

---

## Lab steps → where in the code

| Lab step / deliverable | Where |
|---|---|
| Open the project and run it once | `EnterpriseOps.slnx`, `EnterpriseOps/EnterpriseOps.csproj` (`net10.0-windows;net10.0`, `Wisej-4` 4.1.0 + `Microsoft.EntityFrameworkCore.Sqlite` 10.0.12), port 5204 |
| Replace fake persistence with EF Core behind a service boundary | `Services/IWorkOrderCommandService.cs` + `Services/IWorkOrderQueryService.cs` (the boundary) → `Data/WorkOrderCommandService.cs`, `Data/WorkOrderQueryService.cs`, `Data/WorkOrderRepository.cs`, `Data/EnterpriseOpsDbContext.cs`, `Data/SessionDatabase.cs` (the implementation) |
| **Create** | `WorkOrderCommandService.CreateAsync` · `CreateWorkOrderCommand` · `btnCreate_Click` |
| **Update** | `WorkOrderCommandService.UpdateAsync` · `UpdateWorkOrderCommand` (status edges guarded by `Domain/WorkOrderTransitions.CanChangeStatus`) |
| **Approve** | `WorkOrderCommandService.ApproveAsync` · `ApproveWorkOrderCommand` · `btnApprove_Click` |
| **Search** | `WorkOrderQueryService.SearchAsync` → `PagedResult<WorkQueueRow>` · `btnSearch_Click`, `cboTenant_SelectedIndexChanged` |
| **Audit** | `WorkOrderCommandService.WriteRejectionAuditAsync` + the in-transaction `repository.AddAudit(…)`; read side `WorkOrderQueryService.GetAuditAsync` → `AuditQueryResult` · `UI/AuditLogDialog.cs` |
| Deliverable 1 · Data access boundary diagram | [`EnterpriseOps/docs/DataAccessBoundary.md`](EnterpriseOps/docs/DataAccessBoundary.md) + [`data-access-boundary.svg`](EnterpriseOps/docs/data-access-boundary.svg) |
| Deliverable 2 · DbContext lifetime decision | [`EnterpriseOps/docs/DbContextLifetimeDecision.md`](EnterpriseOps/docs/DbContextLifetimeDecision.md) · code: `Data/SessionDatabase.cs`, `Data/SessionLongContextAntiPattern.cs` |
| Deliverable 3 · Command and result classes | [`EnterpriseOps/docs/CommandAndResultClasses.md`](EnterpriseOps/docs/CommandAndResultClasses.md) · code: `Services/Commands/*.cs`, `Services/NullTrace.cs` |
| Deliverable 4 · Transaction example | [`EnterpriseOps/docs/TransactionExample.md`](EnterpriseOps/docs/TransactionExample.md) · code: `WorkOrderCommandService.RunAsync` + `ApproveAsync` |
| Deliverable 5 · Error mapping table | [`EnterpriseOps/docs/ErrorMappingTable.md`](EnterpriseOps/docs/ErrorMappingTable.md) · code: `Data/ErrorMap.cs` |
| Show every path (success, validation, error) without leaking internals | `ShowResult` / `PaintResult` / `Warn` / `ShowUnexpected` in `UI/ApprovalsPage.cs` — the banner only ever renders `CommandResult.UserMessage` and `CommandResult.Detail` |
| Review & run: production-readiness note | "Production readiness" below |

---

## Where things live

```
Module 4/
  EnterpriseOps.slnx
  README.md                              this file
  EnterpriseOps/
    Program.cs                           Application.MainPage = new UI.ApprovalsPage();
    Startup.cs  Default.html  Default.json  Web.config
    Properties/launchSettings.json       http://localhost:5204
    UI/
      ApprovalsPage.cs / .Designer.cs    the screen: queue · ApprovePanel · trace · bottom bar
      AuditLogDialog.cs / .Designer.cs   read-only audit view over AuditLogRow projections
    Domain/
      WorkOrder.cs  AuditEntry.cs  Tenant.cs
      WorkOrderStatus.cs  Priority.cs  WorkOrderTransitions.cs   pure rules, nothing to mock
    Services/
      IWorkOrderCommandService.cs        the write boundary
      IWorkOrderQueryService.cs          the read boundary
      SessionContext.cs                  per-session tenant/user + correlation-id factory
      IActivityTrace.cs  NullTrace.cs    the trace sink (a page in the app, nothing in a test)
      FaultInjector.cs                   makes the DB_TIMEOUT mapping visible on screen
      Commands/                          CreateWorkOrderCommand · UpdateWorkOrderCommand ·
                                         ApproveWorkOrderCommand · CommandContext · CommandResult
      Queries/                           WorkQueueQuery · WorkQueueRow · WorkOrderHeader ·
                                         PagedResult<T> · AuditLogRow · AuditQueryResult
    Data/
      EnterpriseOpsDbContext.cs          the unit of work + the model (unique index, concurrency token)
      SessionDatabase.cs                 the session-long SQLite connection + short-lived context factory
      SeedData.cs                        60 deterministic work orders, WO-2002 as the video shows it
      IWorkOrderRepository.cs            load/save the aggregate by tenant + id
      WorkOrderRepository.cs             the EF Core implementation (owns no transaction)
      WorkOrderCommandService.cs         authorize → BEGIN → load → check → validate → persist → audit → COMMIT
      WorkOrderQueryService.cs           AsNoTracking projections for screens
      ErrorMap.cs                        provider exception → code + user message + audit line
      SessionLongContextAntiPattern.cs   the wrong lifetime, kept so the failure can be shown
      DataArchitecturePatterns.cs        the lesson's pattern card, annotated with where each part lives
    Security/
      Permissions.cs                     roles, operations, server-side rules
    docs/                                the five deliverables + the boundary SVG
```

---

## Student review questions, answered against this sample

**Can a save be tested without creating a Form?**
Yes. `WorkOrderCommandService` takes a `SessionDatabase`, an `IActivityTrace`, a `FaultInjector` and a
timeout callback — none of which is a UI type — and `Services/NullTrace.cs` exists so the trace sink can
be supplied without a screen. A test constructs `new SessionDatabase(new NullTrace())` (in-memory SQLite,
`EnsureCreated` + seed), calls `ApproveAsync(command, context, ct)` and asserts on `result.ErrorCode`.
Grep for `using Wisej.Web` under `Domain/`, `Services/` and `Data/`: there is none. The full snippet is
in `docs/CommandAndResultClasses.md`.

**What is inside the transaction?**
Load the aggregate by tenant + id → set the original `Version` to what the user saw → validate the
transition against `WorkOrderTransitions` → write the changed columns → write the audit row → commit.
One `SaveChanges`, two rows, one commit. **Outside** it: the permission check (before it opens) and the
*rejection* audit row (after it rolls back, in its own context and transaction, so the record of the
failure is not undone by the failure). Nothing that cannot be rolled back — no e-mail, no queue message,
no file — is ever inside. See `docs/TransactionExample.md`.

**How are concurrency and unique key errors surfaced?**
Neither is checked with an `if`; both are decided by the database and caught at the boundary.
`Version` is `IsConcurrencyToken()`, so the update carries `WHERE Id = @id AND Version = @expected`;
0 rows → `DbUpdateConcurrencyException` → `WO_CONCURRENCY` → *"This work order was changed by someone
else while you were editing it. Reload and try again."* `Number` has the unique index
`UX_WorkOrders_Tenant_Number`, so a duplicate → `DbUpdateException` wrapping `SqliteException` 19 with
`UNIQUE` in the text → `WO_NUMBER_IN_USE` → *"That work order number is already in use for this tenant.
Choose another number."* Both are audited with the constraint or the row count — never with a stack
trace. Click **Fail: stale version** and **Fail: duplicate number** to watch both. See
`docs/ErrorMappingTable.md`.

---

## Instructor acceptance criteria, answered

**The implementation follows the course architecture baseline.** Folder-per-layer with matching
namespaces (`EnterpriseOps.UI` / `.Domain` / `.Services` / `.Data` / `.Security`), one web project,
`Program.Main` → `Application.MainPage`, the shared EnterpriseOps vocabulary (`WorkOrder`, `Tenant`,
`WorkOrderStatus`, `Priority`, `CommandResult`, `PagedResult<T>`, `WorkQueueQuery`, `WorkQueueRow`,
`CommandContext`, `SessionContext`, users `ana.ops` / `ben.tech` / `cara.admin`), the header bar with
tenant · user · correlation id, the live activity trace card, and a bottom bar with success, progress,
failure and recovery paths.

**UI event handlers remain thin and explainable.** Every handler in `UI/ApprovalsPage.cs` is: guard →
build a command → `await` one service call → `ShowResult(result)`, wrapped in `try/catch/finally`. The
longest is `btnBatchApprove_Click`, which loops over service calls and reports progress; it still
contains no query, no transaction and no `DbContext`. `btnCancel_Click` and `btnClearTrace_Click` touch
UI state only and call nothing.

**Service-level logic can be reviewed without opening the designer.** `Data/WorkOrderCommandService.cs`
is readable end to end as prose: authorize, open a bounded unit of work, run the body, commit or roll
back, map, audit. The domain rules are in `Domain/WorkOrderTransitions.cs` (no dependencies at all), the
security rules in `Security/Permissions.cs`, the failure vocabulary in `Data/ErrorMap.cs`. None of these
files references Wisej.

**At least one failure path is demonstrated.** Six, plus two recoveries: invalid transition
(`WO_STATE_INVALID`), empty comment (`VALIDATION_FAILED`), duplicate number (`WO_NUMBER_IN_USE`), stale
version (`WO_CONCURRENCY`), timeout (`DB_TIMEOUT`), technician denied (`PERMISSION_DENIED`) — plus the
wrong-`DbContext`-lifetime demonstration and its disposal, and signing back in as `ana.ops`.

**The student can explain state ownership, security implications and production behaviour.**

*State ownership.* The screen owns UI state only: the selected row, the comment text, the banner colours
and the trace list. `SessionContext` owns per-session state (tenant, user, command timeout, last
correlation id) as an **instance** field on the page — never a static. `SessionDatabase` owns the
session's SQLite connection and dies with the screen (`DisposeSessionResources`). The `DbContext` owns
nothing beyond one operation. The database owns the truth about versions and uniqueness.

*Security.* Authorization is server-side, in the services, before any transaction opens — a disabled
button is a courtesy, `Permissions.IsAllowed` is the rule. Reading the audit log is authorized inside
`WorkOrderQueryService.GetAuditAsync`, so a screen that forgot to hide the button still gets nothing.
Every command and every query is filtered by `TenantId`, and the repository only ever loads *by tenant +
id*, so cross-tenant access is impossible by construction rather than by review. No user message, no
banner and no audit row contains SQL, a schema name or a stack trace.

*Production behaviour.* Every command is bounded by a timeout and mapped to a stable code; every outcome
— committed or rejected — is audited with a correlation id that the user can read off the banner.
Contexts are short-lived, so memory does not grow with session age. A batch runs one transaction per
item so a single bad row cannot undo the good ones. The only piece that is a lab convenience is
`FaultInjector`, which exists so `DB_TIMEOUT` can be shown on demand.

---

## Production readiness

What would change on the way to a real deployment, and what would not:

* **Would not change.** The boundary, the command and result classes, the transaction shape, the error
  mapping's codes and messages, the per-operation context lifetime, the audit-after-rollback rule.
* **Provider.** Swap `UseSqlite(connection)` for `UseSqlServer(connectionString)` (or Npgsql) in
  `SessionDatabase`, drop the session-long connection (a real server has a connection pool), and swap
  the provider checks in `ErrorMap.Map` — SQL Server reports 2601/2627 for a unique violation, 547 for a
  foreign key and −2 for a timeout. The result codes stay the same.
* **Concurrency token.** `int Version` becomes `rowversion`/`timestamp` (`byte[]`), as the walkthrough's
  `ApproveWorkOrderCommand` sketches. `ExpectVersion` becomes a one-line `OriginalValue` assignment
  without the manual increment.
* **Schema.** `EnsureCreated()` + `SeedData` becomes EF Core migrations, applied by the deployment
  pipeline and not by the application at start-up.
* **Concurrency control.** `SessionDatabase.Gate` exists because one in-memory SQLite connection is
  shared by a session; with a pooled server connection it can go, and the per-operation context becomes
  genuinely independent.
* **Retries.** A real provider needs a transient-fault policy (EF Core's execution strategy) around the
  transaction, and `DB_UNAVAILABLE` becomes "retried n times, then reported".
* **Diagnostics.** The trace card is a teaching device; in production the same lines are structured log
  events keyed by the correlation id (Module 11).

---

## Verified / unverified

Framework facts used here that the cookbook marks **(unverified)** — implemented, compiled and worth a
runtime check by the reviewer:

* `DataGridView` details: `AutoGenerateColumns = false` with explicit `DataGridViewTextBoxColumn`s,
  `AutoSizeColumnsMode = Fill` + `FillWeight`, `SelectionMode = FullRowSelect`, `MultiSelect = false`,
  `RowHeadersVisible = false`, `Rows[i].Selected = true`, `SelectedRows[0].DataBoundItem`,
  `CurrentRow.DataBoundItem`, and binding through a `BindingSource` whose `DataSource` is a `List<T>`
  (`ResetBindings(false)` after replacing it).

Everything else is from the cookbook's **verified** list: `Page` as the main screen with
`Application.MainPage`, `Panel` cards with `BorderStyle.Solid`, `Label` fonts `"default"` / `"monospace"`,
`AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000)`,
`await dialog.ShowDialogAsync()` from an `async void` handler with `DialogResult` set inside the dialog,
`Application.Update(this)` after an `await` to push progress, `Application.SessionId`, and
`ComboBox.DropDownStyle = ComboBoxStyle.DropDownList` / `TextBox.Watermark`.

Not from the cookbook, and new in this module: `Microsoft.EntityFrameworkCore.Sqlite` 10.0.12 against an
open `SqliteConnection("DataSource=:memory:")`, `EnsureCreated()`, `IsConcurrencyToken()` on an `int`,
`HasIndex(…).IsUnique()`, `Entry(entity).Property(x => x.Version).OriginalValue`,
`Database.BeginTransactionAsync` / `CommitAsync` / `RollbackAsync`, and
`SqliteException.SqliteErrorCode == 19` for `SQLITE_CONSTRAINT`.

## Build

```
$ cd "Module 4/EnterpriseOps"
$ dotnet build -nologo -v q
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Both target frameworks (`net10.0-windows` and `net10.0`) build clean. The app was **not** run by the
builder; the reviewer runs it.
