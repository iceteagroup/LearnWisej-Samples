# Deliverable 3 — Static-state audit report

*EnterpriseOps · Advanced Module 3 · `Security/SharedStateAttribute.cs` and every static in the solution*

> A Wisej.NET server process hosts every session in the same memory. A static field, a singleton service or a
> cache without a tenant key is therefore shared by every user of every tenant at once.

The audit asks one question of every static in the solution: **what does it hold, and could that ever be
per-user or per-tenant?**

## The rule

| Verdict | Test |
|---|---|
| `Documented` | carries `[SharedState(scope, holds, synchronization)]` with an Application- or Tenant-scope |
| `ImmutableReference` | `static readonly` and the type is immutable (string, primitive, enum, `Guid`, `DateTime`, `Type` …) |
| `Finding` | anything else — a mutable static, an undocumented `static readonly` to a mutable object, or a static documented with a per-user scope (a static can never own User/Session/Tab/Request/Workflow state) |

`[SharedState]` is not decoration. It forces the author to write down the two facts a reviewer needs — what is
in there, and how concurrent access is synchronized.

## The report

7 statics · **0 findings** · 3 documented · 4 immutable.

| Verdict | Member | Type | What it holds |
|---|---|---|---|
| ok · doc | `WorkOrderStore.Shared` | `WorkOrderStore` | work order rows for every tenant; one lock around every read and write; callers get clones |
| ok · doc | `AuditTrail.Shared` | `AuditTrail` | append-only audit entries, each tagged tenant + user + correlation id; one lock; reads filtered by tenant |
| ok · doc | `TenantDirectory.All` | `ReadOnlyCollection<Tenant>` | the three tenants this deployment serves; immutable after construction |
| ok · imm | `UiText.*` (4) | `String` | the sentences the screens show |

The three documented object statics are the answer to the review question *"which services can safely be
shared?"* — the ones that hold **no per-user state** and **synchronize what they do hold**. The store *must* be
shared, or two browser tabs could never collide on a record and there would be nothing for optimistic
concurrency to catch. The audit trail *must* be shared, or an audit would vanish when its author closed the
browser. Everything that touches identity — the session context, the diagnostic log, the error log, the
`ServiceRegistry` itself — is per session. Note that there is deliberately no `ServiceRegistry.Current`.

## What a finding looks like

The classic leak is a pre-web sign-in helper that "remembers" the current user in process-wide memory:

```csharp
public static class CurrentUser          // FINDING — mutable, per-user, unsynchronized
{
    public static string UserId;
    public static string TenantId;
}
```

The last session to sign in overwrites the field, and the next request from anybody else runs as that person —
across a tenant boundary, which makes it a data leak rather than a glitch. The fix is the one this sample uses:
the identity lives in a `SessionContext` stored in `Application.Session`, and every command carries a
`CommandContext` built from it.

## Race-condition review

Thread safety is the second half of the audit. The shared objects, and what protects them:

| Shared object | Concurrent access | Strategy |
|---|---|---|
| `WorkOrderStore._rows` | reads from every session; `TrySave` from any of them | one `lock (_gate)` around every read and every write. `TrySave` compares the version **and** writes inside the same lock, so no two saves can both see `v7`. Reads return `Clone()`s, so a caller mutating what it got cannot corrupt the store |
| `AuditTrail._entries` | appended by every session | one `lock (_gate)` around append and snapshot; the snapshot is a copy |
| `TenantDirectory.All` | read by every session | never mutated after construction — safe without a lock |
| `UiText.*` | read by every session | immutable strings |
| `ActivityTrace`, `ErrorLog`, `SessionContext`, `ServiceRegistry` | one session only | not shared, so nothing to synchronize |

Two notes a reviewer should check:

- **No `await` inside a lock.** The store's methods are synchronous; the simulated latency (`Task.Delay`) lives
  in the service, outside the lock. Holding a lock across an `await` in a Wisej app would hold it across a
  browser round trip.
- **`Application.Update` from a background thread.** This module has no background job, but the rule applies to
  Modules 6, 11 and 13: work started with `Application.StartTask` must push its changes with
  `Application.Update(component)` and must carry its own `CommandContext`, because the session may have ended.
