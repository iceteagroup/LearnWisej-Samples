# Deliverable 3 — Command and result classes

Every write in this application is a plain object handed to a service, and every write answers with a
plain object. Neither of them knows what a `Form` is, which is the module's first review question.

## The classes

| Class | File | What it carries |
|---|---|---|
| `CreateWorkOrderCommand` | `Services/Commands/CreateWorkOrderCommand.cs` | tenant, user, number, title, customer, site, priority, due date |
| `UpdateWorkOrderCommand` | `Services/Commands/UpdateWorkOrderCommand.cs` | id, tenant, user, title, customer, site, priority, optional status, **expected version** |
| `ApproveWorkOrderCommand` | `Services/Commands/ApproveWorkOrderCommand.cs` | id, tenant, user, comment, **expected version** |
| `CommandContext` | `Services/Commands/CommandContext.cs` | tenant, user, role, correlation id — *who* is doing it |
| `CommandResult` | `Services/Commands/CommandResult.cs` | success, user message, error code, detail, correlation id, field errors, new version |
| `IWorkOrderCommandService` | `Services/IWorkOrderCommandService.cs` | `CreateAsync` · `UpdateAsync` · `ApproveAsync` |

```csharp
public sealed record ApproveWorkOrderCommand(
    int WorkOrderId,
    string TenantId,
    string UserId,
    string Comment,
    int ExpectedVersion);
```

```csharp
public interface IWorkOrderCommandService
{
    Task<CommandResult> CreateAsync (CreateWorkOrderCommand  command, CommandContext context, CancellationToken cancellationToken);
    Task<CommandResult> UpdateAsync (UpdateWorkOrderCommand  command, CommandContext context, CancellationToken cancellationToken);
    Task<CommandResult> ApproveAsync(ApproveWorkOrderCommand command, CommandContext context, CancellationToken cancellationToken);
}
```

### Why the command and the context are separate

The command is *what* to do; the context is *who is doing it, for which tenant, under which correlation
id*. Splitting them means the same command object can be replayed by an import job under a service
identity, and it means no caller can smuggle a different role in through the command payload — the
context is built once, by `SessionContext.NewCommandContext()`, from server-side session state.

### Why `ExpectedVersion` is on the command

Optimistic concurrency is a fact about *what the user was looking at*, so it belongs with the intent. It
becomes the `WHERE` clause of the `UPDATE`; see `TransactionExample.md`.

### Why `CommandResult` is not an exception

An exception is a control-flow mechanism for the unexpected. "This work order is on hold" is not
unexpected — it is one of the outcomes the operation is for. So the failure paths return a result with a
**stable code** (`WO_STATE_INVALID`), a **safe message** and a **correlation id**, and the exceptional
paths are caught at the boundary and mapped to the same shape (`ErrorMappingTable.md`). A caller has one
thing to check, no matter what went wrong.

```csharp
public static CommandResult Ok(string message, string correlationId, int? workOrderId = null, int? newVersion = null);
public static CommandResult Fail(string message, string code, string correlationId,
                                 IReadOnlyList<string> errors = null, string detail = null);
```

## The handler that uses them

`UI/ApprovalsPage.cs` — the whole Approve path in the screen:

```csharp
private async void btnApprove_Click(object sender, EventArgs e)
{
    if (!RequireSelection())
        return;

    HideResult();
    SetStatusBar("ApproveAsync — short-lived DbContext · transaction open…");
    SetButtonsEnabled(false);
    try
    {
        var result = await _commands.ApproveAsync(BuildApproveCommand(), NewCommand(), CancellationToken.None);
        ShowResult(result);
    }
    catch (Exception ex)
    {
        ShowUnexpected(ex);
    }
    finally
    {
        SetButtonsEnabled(true);
        await SafeRefreshQueueAsync();
    }
}

private ApproveWorkOrderCommand BuildApproveCommand()
{
    var row = SelectedRow;
    return new ApproveWorkOrderCommand(
        row.Id, _session.TenantId, _session.UserId, txtComment.Text, row.Version);
}
```

Build a command, hand it over, show the result. No validation beyond "is a row selected", no query, no
transaction, no `DbContext`, no knowledge that a database exists. The `catch` is a last line of defence
for a bug in the screen: every *expected* failure already came back as a `CommandResult`.

## "Can a save be tested without creating a Form?"

Yes — and nothing in the sample needs changing to do it. A test would be:

```csharp
var trace    = new NullTrace();                       // any IActivityTrace
using var db = new SessionDatabase(trace);            // in-memory SQLite, EnsureCreated + seed
var service  = new WorkOrderCommandService(db, trace, () => TimeSpan.FromSeconds(5));

var context  = new CommandContext("fabrikam", "ana.ops", Role.Manager, "test0001");
var result   = await service.ApproveAsync(
    new ApproveWorkOrderCommand(2002, "fabrikam", "ana.ops", "ok", 8), context, CancellationToken.None);

Assert.False(result.Success);
Assert.Equal("WO_STATE_INVALID", result.ErrorCode);   // WO-2002 is seeded OnHold
```

The only types involved are the command, the context, the result and the service. `Wisej.Web` is not
referenced by `WorkOrderCommandService.cs`, `WorkOrderQueryService.cs`, `WorkOrderRepository.cs`,
`ErrorMap.cs` or anything under `Domain/` and `Services/` — grep them and there is no `using Wisej.Web`.

## Evidence in the running app

* **Approve** on a work order that is `InProgress` → `CommandResult.Ok`, green banner
  *"Work order approved."*, detail line `committed · audit written · correlation 7c41aa90`, status bar
  `CommandResult.Ok — committed · v8 → v9 · audited`.
* **Approve** on `WO-2002` (seeded `OnHold`) → `CommandResult.Fail`, red banner *"This work order is on
  hold — resolve the hold before approving."*, detail `WO_STATE_INVALID · transaction rolled back ·
  nothing persisted`.
* The same correlation id appears in the banner detail and in the **Audit log** row — one id to quote to
  support.
