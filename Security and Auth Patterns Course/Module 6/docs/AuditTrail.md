# Audit trail · AccessOps Module 6

The audit trail is a business record, not a log. Logs are diagnostic and rotate; an `audit_entry`
row is evidence with an owner, a retention period and a reader who is not a developer.

| | |
|---|---|
| Owner | Head of Finance Operations (approvals, documents) and the access administrator (permissions, sign-ins) |
| Retention | 7 years from the entry date (the demo's stated policy - set it to your organisation's records schedule) |
| Readers | `ViewAuditTrail` holders: Administrator and Auditor roles (the *Admin console* audit grid) |
| Writer | `AuditService` only ([AuditService.cs](../AccessOps/Services/AuditService.cs)) |

## What is recorded

| Action | Written by | Outcomes |
|---|---|---|
| `Session.SignIn` | `LoginAuditService` (password) and `ExternalIdentityMapper` (OpenID Connect) | `Success`, `BadCredential`, `UnknownUser`, `LockedOut`, `Disabled`, `NotServiceAccount`, `ExternalSuccess`, `ExternalProvisioned`, `ExternalRejected` |
| `Session.SignOut` | `LoginAuditService` | `SignOutExplicit`, `SignOutIdle`, `TokenExpired` |
| `Approval.Approve`, `Approval.Reject` | `ApprovalService` | `Allowed`, `Denied`, `Failed` (already decided) |
| `Permission.Grant`, `Permission.Revoke` | `PermissionEditorService` | `Allowed`, `Denied` |
| `Document.Download`, `Document.Share` | `DocumentService` | `Allowed`, `Denied` |
| `Document.Upload` | `DocumentService` | `Allowed`, `Refused:<check>` |
| `Document.ShareOpen` | `ShareLinkEndpoint` (anonymous actor `share-link`) | `Allowed`, `Denied` |

Every row answers: who (`actor` - `u-<employee id>`, `oidc:<subject>` before the account is mapped,
never a name), did what (`action`), to what (`target_type`, `target_id`), when (`at_utc`, the
server clock), from where (`client_address` and `user_agent`, captured at sign-in from
`Application.UserHostAddress` / `Application.UserAgent` and carried on the `SessionContext`),
under which `correlation_id`, and how it ended (`outcome`). Denials are recorded - *row 4470 is
the interesting one*.

## Append-only

The table and its guards are created in
[AccessOpsDatabase.cs](../AccessOps/Infrastructure/AccessOpsDatabase.cs):

```sql
CREATE TABLE audit_entry (seq INTEGER PRIMARY KEY, at_utc, actor, action, target_type, target_id,
    outcome, client_address, user_agent, correlation_id, prev_hash, hash);
CREATE TRIGGER audit_entry_no_update BEFORE UPDATE ON audit_entry BEGIN SELECT RAISE(ABORT, 'audit_entry is append-only'); END;
CREATE TRIGGER audit_entry_no_delete BEFORE DELETE ON audit_entry BEGIN SELECT RAISE(ABORT, 'audit_entry is append-only'); END;
```

SQLite has no logins or grants, so the sample enforces "INSERT only" with triggers. On a server
database the same rule is a permission on the application's login, which a bug or an injected
statement cannot step around:

```sql
-- SQL Server: the application login may add and read rows, nothing else
GRANT INSERT, SELECT ON dbo.audit_entry TO accessops_app;
DENY UPDATE, DELETE ON dbo.audit_entry TO accessops_app;
```

## Gapless and hash-chained

`AuditService.Record` reads the last row inside the write transaction, takes `seq + 1` and computes
`hash = SHA-256(seq | at_utc | actor | action | target | outcome | address | agent | correlation | prev_hash)`.
`VerifyChain()` recomputes the chain from row 1 and returns the first row whose sequence, previous
hash or own hash no longer matches. It runs at start-up (`Audit chain check at start-up: intact` in
the log) and from **Verify chain** on the admin console. That is tamper evidence, not prevention.

## Same transaction as the business change

`ApprovalService.DecideAsync` and `PermissionEditorService.Change` open one transaction, make the
change, call `AuditService.Record(connection, tx, …)` and commit once - an approval without its row
is not a state the code can reach. A refusal or a failure rolls the change back and writes its own
`Denied` / `Failed` row.

## Evidence from the browser run (2026-11-06)

The admin console's audit grid after the run (newest first, trimmed):

| Seq | Action | Actor | Target | Outcome | Correlation |
|---|---|---|---|---|---|
| 18 | Document.ShareOpen | share-link | Document:4471 | Denied | e69a-4d03 |
| 17 | Document.ShareOpen | share-link | Document:4471 | Denied | fa87-03a7 |
| 16 | Document.ShareOpen | share-link | Document:4471 | Allowed | aad7-0762 |
| 15 | Document.Share | u-3001 | Document:4471 | Allowed | f499-241b |
| 14 | Document.Download | u-3001 | Document:4471 | Allowed | d32f-9c01 |
| 13 | Permission.Revoke | u-3001 | Employee:2201/ViewAuditTrail | Allowed | aef0-ae07 |
| 12 | Permission.Grant | u-3001 | Employee:2201/ViewAuditTrail | Allowed | 4b92-6ba3 |
| 4 | Approval.Reject | u-2201 | ApprovalRequest:4463 | Allowed | e2ac-dc63 |
| 3 | Approval.Approve | u-2201 | ApprovalRequest:4104 | Denied | 7581-eb92 |
| 2 | Approval.Approve | u-2201 | ApprovalRequest:4471 | Allowed | 1a7c-6c72 |
| 1 | Session.SignIn | oidc:3e77-41a0-jamie-mercer | Employee:2201 | ExternalSuccess | 35a8-8973 |

**Verify chain** then reported *Audit chain verified · every entry matches*. Breaking the trail is
covered by tests ([AuditTests.cs](../AccessOps.Tests/AuditTests.cs)): an `UPDATE` and a `DELETE`
are refused with `audit_entry is append-only`; dropping the trigger as the owner and rewriting
row 3 makes `VerifyChain()` return 3.
