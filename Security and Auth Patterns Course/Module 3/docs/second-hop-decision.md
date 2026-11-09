# Second-hop classification and migration decision — fixture F04 (Module 3 deliverable 5)

**Status: lab environment only.** There is no second server and no domain here, so F04 was not
reproduced. This is the classification procedure and the decision record AccessOps uses.

F04: the scoped read works on the local NTFS fixture and fails against the remote resource
(`\\files01\confidential` over SMB, or a SQL integrated login to `sql01`), for the same user.

## Classify — three questions, in this order, each answered separately

| # | Question | Evidence | If "no" |
|---|---|---|---|
| 1 | **First hop:** did the browser reach IIS with **Kerberos**? | client `klist` shows a ticket for `HTTP/accessops.contoso.test`; server 4624 package `Kerberos` (Module 2 `provider-evidence.md`). A Negotiate header alone proves nothing; **NTLM cannot be delegated**. | fix SPN / browser intranet policy / FQDN first — no delegation is possible over NTLM |
| 2 | **Delegation:** may the pool account delegate to *that exact* target SPN? | AD: the account that holds `HTTP/accessops.contoso.test` (machine account for kernel mode) has **constrained** delegation to `cifs/files01.lab.local` (SMB) or `MSSQLSvc/sql01.lab.local:1433` (SQL); the user is **not** "Account is sensitive and cannot be delegated" / not in Protected Users; on the web server `klist` (in the pool's context) or event 4769 on the DC shows the S4U2proxy ticket for the target | domain team configures constrained (or resource-based constrained) delegation to the target — never unconstrained |
| 3 | **Backend authorization:** does the share/NTFS/SQL permission grant **this user**? | share + NTFS ACL on `\\files01\confidential` (or `sys.server_principals` / database role on sql01) | backend owner grants the user (or a group) — not AccessOps |

Change code only after all three answers are in. Lesson review question 3: "the user opens the share
from their desktop" rules out **backend authorization** (3) for that user; **first hop** (1) and
**delegation** (2) remain.

Record for the lab:

| Check | Result | Evidence (time-correlated) |
|---|---|---|
| First hop | Kerberos / NTLM / undetermined | |
| Delegation target | `cifs/files01.lab.local` allowed? user sensitive? | |
| Backend ACL | user granted? | |
| Classification | first-hop / delegation / backend | |

## Decision for AccessOps

| | Option A · delegated user access | Option B · restricted service identity + explicit audit |
|---|---|---|
| Who opens the remote file | the user's token, delegated (constrained) | a dedicated, narrowly privileged service account (the pool's identity or a gMSA), never the user |
| Authorization | the backend ACL stays authoritative | AccessOps decides: grant, tenant and record (`DocumentService` / `DocumentCatalog`), then the service reads |
| Audit | backend logs the user | AccessOps audit names the person on whose behalf it read (opaque subject + document + decision + correlation ID); backend logs the service account |
| Needs | Kerberos end to end, constrained delegation per target SPN, user not sensitive, every new target configured by the domain team | the service account has read on exactly the approved share, nothing else; no delegation at all |
| Fails when | NTLM anywhere on the first hop; a new backend not in the delegation list; a proxy that breaks Kerberos | AccessOps' own policy is wrong (so it is tested: PolicyHarness) |

**Decision: Option B** for the confidential-documents share. Reasons: the documents are already
classified and granted per tenant in AccessOps, so the application decision is the source of truth;
a restricted service identity needs no delegation configuration per target and works over any first
hop; every read is attributed in the AccessOps audit to the person it was made for. Option A is kept
for resources whose owners require their own ACL to be authoritative (e.g. a department share managed
outside AccessOps) — then constrained delegation to that single target SPN, documented with the domain
team, never unconstrained.

What changes in code for Option B: the remote read no longer runs inside `RunImpersonated`; it runs
as the process identity **after** `DocumentCatalog.ResolveApprovedPath` allowed it, and writes the
`file.read` audit event with the subject. The local scoped read of Module 3 stays as the lab's proof
of token scope.
