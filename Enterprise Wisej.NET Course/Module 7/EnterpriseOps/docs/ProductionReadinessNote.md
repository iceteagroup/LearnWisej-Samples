# Production-readiness note — Escalation Wizard (Module 7)

The lab's last step asks for a short note before the module is called done. This is that note: state ownership,
security, failure paths and what would change on the way to production.

## State ownership

| State | Owner | Lifetime |
|---|---|---|
| Field values while typing | the controls of the visible step | until `CollectCurrentStep()` copies them, i.e. the next Next/Back/Cancel |
| The answers + which steps are complete | `EscalationWizardState` | the draft, saved to `WorkflowStateStore` after every completed step |
| Draft store | `WorkflowStateStore` (per session here) | survives a browser refresh and a cancelled wizard; removed when the escalation is created |
| Tenant, user, correlation id | `SessionContext` (per session, never static) | the browser session; a new correlation id per user action |
| Escalations, work orders, staging | `Data/*` stores | the session (in-memory fakes) |
| Compensations | `CompensationLog` | the session; open entries are the manual-review queue |

**In production** the draft store is a table keyed by user + work order (not a session object), so a draft also
survives a new login on another device; staged uploads become blobs with a TTL and a cleanup job for drafts that
are abandoned; `CompensationLog` becomes a durable queue with a retry schedule and a dead-letter path.

## Security

- Escalating is authorised in the workflow (`PermissionService.CanEscalate`) during `authorize`, after
  validation and **before** anything is written — never in the wizard, which has no idea what a role is.
- The tenant of the session and the tenant of the work order are compared on every command; a mismatch is
  `Unauthorized` and is traced by `Security:`.
- Who may *approve* is a second rule (`CanApprove`) applied to the directory entry, which is why the directory
  can list `ben.tech` without him being a legal choice.
- Failure messages to the user are generic ("The action could not be completed…") plus a correlation id; the
  details go to the trace / log. Nothing leaks a store name, a stack trace or a provider error to the browser.
- A `[WebMethod]`-style entry point would need the same check again server-side — the rule lives in the service
  precisely so that every entry point shares it.

## Failure behaviour

Every cell of [`FailurePathMatrix.md`](FailurePathMatrix.md) has a demonstrated path in the app. The two that
matter most in review:

- **notify fails after persist** → the escalation is kept and a compensation is queued (never a false rollback).
- **audit fails after notify** → the gap is recorded and alerted, because a sent notification cannot be unsent.

## What is deliberately simplified in this sample

| Sample | Production |
|---|---|
| In-memory stores, no database, no transaction | a real `DbContext` with a transaction around persist + audit-intent, and the outbox pattern for the notification |
| `FakeNotificationGateway` / `FakeApproverDirectory` with `FailNextSend` / `HangNextLookup` switches | real providers behind the same interfaces, with retry/back-off and circuit breakers |
| The manual-review queue is a `List<CompensationEntry>` in memory | a durable queue + a scheduled retry job, alerting on entries older than the SLA |
| 5 s directory timeout, 150–400 ms simulated latencies | timeouts from configuration, measured against real p95s |
| Attachments are four sample file names | real uploads, virus scanning, size limits, blob storage with a staging TTL |
| One session, one user (`ana.ops`) | authentication, per-request identity, and the same `SessionContext` shape hanging off it |

## Ready / not ready

**Ready:** the transition has one owner, the command and result are typed, every failure path is designed and
demonstrated, the audit trail explains its own gaps, and the workflow is callable — and testable — without the UI.

**Not ready without the production column above:** durability (drafts, compensations, audit) and the real
integrations. Nothing in the screen or the workflow has to change for that; only `Data/`, `Integrations/` and the
composition root in `Services/SessionServices.cs` do.
