# Deliverable 2 — CommandContext with correlation ID

*EnterpriseOps · Advanced Module 3 · `Security/CommandContext.cs`, `Security/SessionContext.BeginCommand`,
`Security/AuditTrail.cs`*

## The shape

```csharp
public sealed record CommandContext(
    string UserId,
    string TenantId,
    string CorrelationId,
    DateTimeOffset RequestedAt,
    string CommandName);
```

Request-scoped. Built by the handler at the start of a user action and passed **explicitly** into every service,
repository and job the action touches. Nothing downstream reaches back into the session to find out who it is
working for — which is the whole point, because a background job outlives the session that started it.

```csharp
// SessionContext.cs
public CommandContext BeginCommand(string commandName)
    => new CommandContext(UserId, TenantId, NewCorrelationId(), DateTimeOffset.UtcNow, commandName);

public static string NewCorrelationId() => Guid.NewGuid().ToString("N").Substring(0, 8);
```

Eight hex characters: short enough for a status bar, unique enough for a log search.

## One id per user action

`WorkOrderEditorPage.BeginBusy(text, commandName)` is the only place a command starts:

```csharp
private void BeginBusy(string text, string commandName)
{
    NewCommand(commandName);          // fresh correlation id → header label
    SetButtons(false);
    SetStatus(text, StatusKind.Warn);
    lblStatusBar.Text = $"{commandName} · tenant {_session.TenantId} · corr {_current.CorrelationId}";
}
```

Every service call inside that handler uses `CurrentContext`, so the id is the same from the click to the last
audit line. The command names in this screen: `work-queue.load`, `work-order.open`, `work-order.save`,
`work-order.reload`, `session.switch-tenant`, `demo.other-session`, `demo.cross-tenant`, `state.audit`,
`state.leak-demo`.

## Where the id shows up

| Place | Example |
|---|---|
| Header label `lblCorrelation` | `corr 8f3a21c4` |
| Dark footer `lblStatusBar` | `work-order.save · tenant contoso · corr 8f3a21c4` |
| Activity trace, every layer | `Service:  WorkOrderService.SaveAsync(#2002) expecting v7 (correlation 8f3a21c4)` |
| Audit trail | `work-order.save.conflict — #2002 expected v7, found v8 (saved by ben.tech)` |
| Conflict dialog footnote | `correlation 8f3a21c4 — expected v7, found v8` |
| Error log | `[8f3a21c4] CrossTenantAccessException: Cross-tenant access denied.` |
| The user-facing failure message | `The action could not be completed. Check the log for details.  (ref 8f3a21c4)` |

The last row is the reason the id is short and visible: the user can read it out, and it is the only piece of
internal state that is safe to show them.

## Why the context travels instead of the session

Three reasons, all of them visible in the sample:

1. **Background work.** A job started with `Application.StartTask` can outlive the session; a service that read
   `Application.Session` would fault or, worse, read a *different* session. A `CommandContext` is a value —
   it is still valid when the browser has gone.
2. **A second session in the same process.** `Services/OtherSessionSimulator.cs` builds a second
   `SessionContext`, gets its own `CommandContext` from it, and calls the *same* `WorkOrderService` class. The
   service cannot tell which session is calling and does not need to: everything it must know is in the context.
3. **Reviewability.** A service whose tenant comes from an argument can be read in one screenful and tested
   without a browser.

## The audit trail

`Security/AuditTrail.cs` is append-only and application-scoped — an audit that died with its author's session
would be worthless. Every entry carries `CorrelationId`, `TenantId`, `UserId`, an action and a detail, writes go
through one lock, and `Snapshot(tenantId)` filters by tenant so the trail is shared but the *view* of it is not.

## Evidence — what the running app shows

| Action | What you see |
|---|---|
| Any button | the header's `corr …` changes once, and every trace line of that action repeats the same id |
| **Save** (success) | `Audit: work-order.save — #2002 v7 → v8 …` with the id from the header |
| **Save** (stale) | three lines share one id: `Data: … REJECTED`, `Service: … returning a ConflictInfo`, `Audit: work-order.save.conflict` |
| Conflict dialog → any button | `Audit: conflict resolution recorded — #2002 expected v7, found v8 → Reload (correlation 8f3a21c4)` |
| **Fail: other session saves** | a *different* id appears on the `session B` lines — a different command in a different session, correctly not sharing this one's id |
