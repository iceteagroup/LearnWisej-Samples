# Deliverable 3 — Static-state audit report

*EnterpriseOps · Advanced Module 3 · `Security/StaticStateAudit.cs`, `Security/SharedStateAttribute.cs`,
`Security/LegacyCurrentUser.cs`, `Services/StateAuditService.cs`*

> A Wisej.NET server process hosts every session in the same memory. A static field, a singleton service or a
> cache without a tenant key is therefore shared by every user of every tenant at once.

The audit asks one question of every static in the solution: **what does it hold, and could that ever be
per-user or per-tenant?** In this sample it is not a document that goes stale — the **Run static-state audit**
button reflects over the running assembly and answers the question live.

## The rule

| Verdict | Test |
|---|---|
| `Documented` | carries `[SharedState(scope, holds, synchronization)]` with an Application- or Tenant-scope |
| `ImmutableReference` | `static readonly` and the type is immutable (string, primitive, enum, `Guid`, `DateTime`, `Type` …) |
| `Finding` | anything else — a mutable static, an undocumented `static readonly` to a mutable object, or a static documented with a per-user scope (a static can never own User/Session/Tab/Request/Workflow state) |

`[SharedState]` is not decoration. It forces the author to write down the two facts a reviewer needs — what is
in there, and how concurrent access is synchronized — and the audit reads the attribute back.

## The report, as shipped

14 statics · **4 findings** · 4 documented · 6 immutable.

| Verdict | Member | Type | What it holds |
|---|---|---|---|
| **FINDING** | `LegacyCurrentUser.UserId` | `String` | *the current user* — the classic leak |
| **FINDING** | `LegacyCurrentUser.TenantId` | `String` | *the current tenant* |
| **FINDING** | `LegacyCurrentUser.SetBySessionId` | `String` | the session that wrote it last |
| **FINDING** | `LegacyCurrentUser.SetAtUtc` | `DateTime` | when — mutable, unsynchronized |
| ok · doc | `WorkOrderStore.Shared` | `WorkOrderStore` | work order rows for every tenant; one lock around every read and write; callers get clones |
| ok · doc | `AuditTrail.Shared` | `AuditTrail` | append-only audit entries, each tagged tenant + user + correlation id; one lock; reads filtered by tenant |
| ok · doc | `TenantDirectory.All` | `ReadOnlyCollection<Tenant>` | the three tenants this deployment serves; immutable after construction |
| ok · doc | `StaticStateAudit.ImmutableTypes` | `Type[]` | the audit's own lookup table; never mutated |
| ok · imm | `UiText.*` (6) | `String` | the sentences the screens show |

The three documented object statics are the answer to the review question *"which services can safely be
shared?"* — the ones that hold **no per-user state** and **synchronize what they do hold**. The store *must* be
shared, or two browser tabs could never collide on a record and there would be nothing for optimistic
concurrency to catch. The audit trail *must* be shared, or an audit would vanish when its author closed the
browser. Everything that touches identity — the session context, the activity trace, the error log, the
`ServiceRegistry` itself — is per session. Note that there is deliberately no `ServiceRegistry.Current`.

## The finding, demonstrated

`LegacyCurrentUser` is what a pre-web sign-in helper used to do. Nothing in this sample authorizes on it; in the
code it is modelled on, something did. **Fail: static-state leak** runs `StateAuditService.DemonstrateLeak`:

1. this session "remembers" itself → `ana.ops@contoso set by session 3f1c…`;
2. another user signs in in another session → `cara.admin@northwind set by session a91b…`;
3. the static is compared with this session's own `SessionContext`, which is untouched.

The banner reads:

> Static-state leak — LegacyCurrentUser now says cara.admin@northwind, but this session is ana.ops@contoso

Which is the bug in one line: the last session to sign in overwrites the field, and the next request from
anybody else runs as that person — across a tenant boundary, which makes it a data leak rather than a glitch.

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
- **`Application.Update` from a background thread.** This module has no background job, but the rule from the
  cookbook applies to Modules 6, 11 and 13: work started with `Application.StartTask` must push its changes with
  `Application.Update(component)` and must carry its own `CommandContext`, because the session may have ended.

## Evidence — what the running app shows

| Action | What you see |
|---|---|
| **Run static-state audit** | banner `Static-state audit — 4 of 14 statics are findings: LegacyCurrentUser.UserId, LegacyCurrentUser.TenantId, …`; the trace prints every entry, findings first, each with its reason |
| **Fail: static-state leak** | trace: the static after this session, after the other session, then the `LEAK:` line and `SessionContext (untouched, per session) → ana.ops@contoso` |
| Run the audit again after the leak demo | the same four findings, now with the other session's values in `CurrentValue` — the static kept them |
