# Before / inside / after identity and outcome matrix (Module 3 deliverable 2)

Columns as the lab asks: app subject, request identity, effective identity
(`WindowsIdentity.GetCurrent()` → `Name`, `User`, `ImpersonationLevel`), outcome. Every identity the
trace reads is disposed by the caller (`using`); the trace copies values only.

## A · Observed on this machine — 2026-11-09 (Kestrel Negotiate, Chromium browser, `http://localhost:6403`)

Setup: `tools/Setup-Fixtures.ps1` in **local mode** (`acl-fixtures.md`). One Windows account
(`DEVBOX\dev`) runs the app *and* signs in from the browser, so "pool" and "user" are the same SID.
SIDs shortened to `S-1-5-21-…-1007`; the full values are in the app log only.

| Document | Step | App subject | Request identity | Effective name · SID · level · logon group | Outcome |
|---|---|---|---|---|---|
| DOC-201 (`labuser-only`) | Before | acct …7f3a | DEVBOX\dev | DEVBOX\dev · …-1007 · None · INTERACTIVE | Readable as pool |
| DOC-201 | Inside | acct …7f3a | DEVBOX\dev | DEVBOX\dev · …-1007 · **Impersonation** · INTERACTIVE | **Read 18 KB** (18426 bytes) |
| DOC-201 | After | acct …7f3a | DEVBOX\dev | DEVBOX\dev · …-1007 · None · INTERACTIVE | Scope closed (same token as before: True) |
| DOC-202 (`pool-only`, local: deny for this account) | Before | acct …7f3a | DEVBOX\dev | … · None | Access denied (ACL) |
| DOC-202 | Inside | acct …7f3a | DEVBOX\dev | … · **Impersonation** | **Access denied (ACL)** — `UnauthorizedAccessException` |
| DOC-202 | After | acct …7f3a | DEVBOX\dev | … · None | Reverted (same token as before: True) |
| DOC-203 F03 (`labuser-only`, missing) | Before | acct …7f3a | DEVBOX\dev | … · None | File missing |
| DOC-203 F03 | Inside | acct …7f3a | DEVBOX\dev | … · **Impersonation** | `FileNotFoundException` |
| DOC-203 F03 | After | acct …7f3a | DEVBOX\dev | … · None | **Reverted** (same token as before: True) |
| DOC-204 (`tenant-b`) | Policy | acct …7f3a | DEVBOX\dev | … · None | **Denied before any scope** (reason `tenant`, no Before/Inside/After) |

Screen messages (status line, four distinct, plain):

| Path | Status line | Last-read chip |
|---|---|---|
| allowed | "DOC-201 opened · effective identity back to the pool" (green) | Read as user |
| ACL denial | "DOC-202 could not be opened with your Windows account. The request was logged." (red) | Denied (ACL) |
| missing file (F03) | "DOC-203 could not be opened. The file is missing; the request was logged." (amber) | File missing |
| policy denial | "DOC-204 is outside your access. Nothing was opened." (red) | Not allowed |

Audit (opaque subject, correlation ID): `scoped-read A / DOC-201 allowed` + `file.read DOC-201 allowed`;
`scoped-read A / DOC-202 allowed` + `file.read DOC-202 failed acl`; `scoped-read A / DOC-203 allowed` +
`file.read DOC-203 failed missing`; `scoped-read B / DOC-204 denied tenant` (no `file.read`).

**What this run proves and what it does not.** It proves: the policy runs before any scope
(DOC-204 has no Before/Inside/After row); the read inside the scope runs on an impersonation token
(`ImpersonationLevel = Impersonation`) built from the WebSocket request's `WindowsIdentity`
(`IHttpContextAccessor`), and `RunImpersonated` reverts on return and on throw (After = Before, also
after F03 and after the ACL exception); the four outcomes are distinct on screen while the log keeps
the path and the exception. It does **not** prove the ACL flip ("the user-only read fails before and
succeeds inside"): the NTLM loopback logon on one machine gives the app a token of the *same*
interactive logon session (same SID, same INTERACTIVE group — observed), so no ACL can tell the
process from the signed-in user here. That flip is in part B.

## B · Lab environment (IIS Windows Authentication, domain accounts) — expected, not run here

Setup: `tools/Setup-Fixtures.ps1 -PoolAccount "IIS APPPOOL\AccessOps Intranet" -LabUser "LAB\dana.ortiz"`.

| Document | Step | App subject | Request identity | Effective token | Outcome |
|---|---|---|---|---|---|
| DOC-201 (`labuser-only`) | Before | acct …7f3a | LAB\dana.ortiz | IIS APPPOOL\AccessOps Intranet | Access denied (ACL) |
| DOC-201 | Inside | acct …7f3a | LAB\dana.ortiz | LAB\dana.ortiz · Impersonation | Read 18 KB |
| DOC-201 | After | acct …7f3a | LAB\dana.ortiz | IIS APPPOOL\AccessOps Intranet | Scope closed |
| DOC-202 (`pool-only`) | Before | acct …7f3a | LAB\dana.ortiz | IIS APPPOOL\AccessOps Intranet | Readable as pool |
| DOC-202 | Inside | acct …7f3a | LAB\dana.ortiz | LAB\dana.ortiz · Impersonation | Access denied (ACL) |
| DOC-202 | After | acct …7f3a | LAB\dana.ortiz | IIS APPPOOL\AccessOps Intranet | Reverted |
| DOC-203 F03 | Inside / After | | | user · Impersonation / pool | FileNotFoundException / Reverted |
| DOC-204 | Policy | | | pool | Denied before any scope |

Mark each row "lab" or "local" when you attach the matrix. If both identities read both files in
every row, the evidence is void: check inherited ACLs and local Administrators membership of the
lab user and the pool (lesson review question 1).
