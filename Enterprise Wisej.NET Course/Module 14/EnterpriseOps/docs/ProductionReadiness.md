# Production readiness statement — EnterpriseOps Command Center

**Deliverable:** Production readiness statement · **Module 14** · last verified 2026-09-10 · owner ana.ops

What is ready, what is not, and who owns each open item. Written to be read by someone who has never met this
team, and to be wrong in public rather than comfortable in private.

## Ready to defend

| Area | Evidence | From |
|---|---|---|
| Architecture baseline | services own logic, handlers are thin; the review gate is run over the reference screen | modules 1–5 |
| Security review | authorization checked in services, denials audited, tenant taken from the session | module 10 |
| Failure paths demonstrated | stale version, permission denied, missing deliverable, rejected generated change — each with a recovery | every module |
| Operations runbook | diagnostics probes with evidence, correlation ids end to end, health card | modules 11–12 |
| Offline and field scope | sync queue and conflict workflow | module 13 |
| Documentation index | every claim cites a source; the index is machine-readable and verified against disk | module 14 |
| AI usage discipline | prompt header in, no undocumented APIs out, decisions recorded | module 14 |

## The four questions, answered

**Where does state live?**
In one `SessionContext` per browser session, created in `Program.Main` and parked in `Application.Session`.
Services are per session (`ServiceRegistry`). The only statics are immutable reference data: the permission
matrix and the documented-API catalog. Checklist rule Q2 rejects any change that adds another.

**Who can call what?**
`PermissionService` holds the matrix: a Technician may view the work queue; a Manager or Admin may also
approve, run diagnostics, sign a generated-code review and verify the capstone package. Every check happens
inside a service, before any read or write, and every refusal is written to the audit log with its
correlation id. The tenant is never taken from the caller.

**What can fail, and what happens then?**
Expected failures travel as a typed `CommandResult` and are shown as one sentence a user can act on: a stale
version, a refusal, a cross-tenant request, a work order in a status that cannot be approved, a missing
deliverable. Unexpected failures are caught in the handler, logged with the correlation id, and hidden behind
a generic message that quotes that id. Nothing leaks a stack trace to the browser.

**What changes in production?**
Configuration comes from the environment, not the code. The in-memory store stands in for the Module 4
EF Core context; the audit log stands in for a table written in the same transaction as the command. The
diagnostics probes are the ones an on-call engineer reads first. A bad release is reversed with the Module 12
rollback.

## Open items

Not done. Each has an owner and a date, and none of them is "we'll get to it".

| # | Open item | Why it matters | Owner | By |
|---|---|---|---|---|
| 1 | Security review sign-off is not filed (`docs/SecurityReviewSignOff.md`) | the review happened; the signed record did not | ana.ops | 2026-09-24 |
| 2 | Rollback rehearsed in staging only, never against production-shaped data | an unrehearsed rollback is a plan, not a control | cara.admin | 2026-10-08 |
| 3 | Audit log is per session in this build; production needs it in the command transaction | an audit line that can be lost is not an audit line | cara.admin | 2026-10-08 |
| 4 | The documented-API catalog is pinned to Wisej-4 4.1.0 | after an upgrade, Q1 findings would be stale until it is regenerated | ben.tech | at each upgrade |
| 5 | No load test above 50 concurrent sessions | the paging and background-task budgets are untested at scale | ana.ops | 2026-10-22 |
| 6 | Documentation MCP endpoint is not deployed | `docs/index.json` is MCP-shaped but nothing serves it yet | ben.tech | 2026-11-05 |

Item 1 is the one the app can show: press **Fail: missing document** on the Capstone Review screen and the
package stops passing, exactly as it should while that document does not exist.

## Known limitations of this build

- In-memory data only: no database, no network, no cloud account, nothing deployed. The sample is a lab.
- The review gate is deterministic text and structure analysis, not a compiler. It answers the questions a
  reviewer would otherwise have to remember to ask; it does not replace the reviewer.
- A Q1 finding means the member is not in the documentation catalog. That is a reason to stop, not proof the
  API does not exist — and after a framework upgrade, see open item 4.
- "MCP-ready" describes the **shape** of `docs/index.json`, not a running server. No AI or network call is
  made anywhere in this application.

## Sign-off

The team has walked the Command Center as a production architecture review, with the demo script, and can
answer the four questions above without the slides.

- **State ownership** — explained: per-session, never static.
- **Security implications** — explained: authorization in services, denials audited, tenant from the session.
- **Failure paths** — demonstrated: each one with its recovery, in front of the reviewer.
- **Production behaviour** — explained: configuration, diagnostics, rollback, and the six open items above.

Explained, not assumed.

| Role | Name | Date |
|---|---|---|
| Application owner | ana.ops (Manager) | 2026-09-10 |
| Security and audit | cara.admin (Admin) | 2026-09-10 |
| Field operations | ben.tech (Technician) | 2026-09-10 |

## Evidence

Capstone Review → **✓ Verify package** shows every deliverable of this statement resolved on disk and
carrying the section that proves it. **Fail: missing document** shows what this statement's open item 1 looks
like from the inside: the package refuses to pass while a listed document is not there.
