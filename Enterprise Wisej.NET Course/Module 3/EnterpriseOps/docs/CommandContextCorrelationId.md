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

`WorkOrderEditorPage.BeginBusy(commandName)` is the only place a command starts:

```csharp
private void BeginBusy(string commandName)
{
    NewCommand(commandName);          // _current = _session.BeginCommand(commandName) — fresh correlation id
    SetControlsEnabled(false);
}
```

Every service call inside that handler uses `CurrentContext`, so the id is the same from the click to the last
audit line. The command names in this screen: `work-queue.load`, `work-order.open`, `work-order.save`,
`session.switch-tenant`.

## Where the id shows up

| Place | Example |
|---|---|
| Dark footer `lblStatusBar` | `Saved — v7 → v8 · correlation 5d11e9b2` / `Save rejected — expected v7, found v8 · correlation 8f3a21c4` |
| Server log (`System.Diagnostics.Trace`), every layer | `Service:  WorkOrderService.SaveAsync(#2002) expecting v7 (correlation 8f3a21c4)` |
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
2. **A second session in the same process.** A second browser tab builds a second `SessionContext`, gets its
   own `CommandContext` from it, and calls the *same* `WorkOrderService` class. The service cannot tell which
   session is calling and does not need to: everything it must know is in the context.
3. **Reviewability.** A service whose tenant comes from an argument can be read in one screenful and tested
   without a browser.

## The audit trail

`Security/AuditTrail.cs` is append-only and application-scoped — an audit that died with its author's session
would be worthless. Every entry carries `CorrelationId`, `TenantId`, `UserId`, an action and a detail, writes go
through one lock, and `Snapshot(tenantId)` filters by tenant so the trail is shared but the *view* of it is not.

## Evidence — what the running app shows

| Action | What you see |
|---|---|
| **Save** (success) | footer `Saved — v7 → v8 · correlation …`; the audit entry `work-order.save — #2002 v7 → v8 …` carries the same id |
| **Save** (stale) | footer `Save rejected — expected v7, found v8 · correlation 8f3a21c4`; the dialog footnote repeats the id; in the server log `Data: … REJECTED`, `Service: … returning a ConflictInfo` and the `work-order.save.conflict` audit entry share it |
| Conflict dialog → any button | audit entry `work-order.conflict.reload — #2002 expected v7, found v8 → Reload`, same correlation id |
| A save from a second browser tab | a *different* id — a different command in a different session, correctly not sharing this one's id |
