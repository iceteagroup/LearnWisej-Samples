# Deliverable 4 — Failure path matrix

One row per step of the workflow, one column per way that step can end. Each cell says **what the system does ·
what the user sees · what is persisted · what is audited**. The matrix is written before the code and doubles as
the test plan: every cell is a test of `EscalationWorkflow`, and the cells marked ▶ are the manual checks in the
demo script.

Legend: *state* = the wizard state object in `WorkflowStateStore`. "nothing persisted" means no escalation row,
no work-order change, no staging commit.

## A · The wizard steps (before the command exists)

| Step | Confirm (Next) | Cancel | Timeout | Validation failure | External failure |
|---|---|---|---|---|---|
| **1 Reason** | state.Reason saved, step marked complete, draft saved · rail ✓ · nothing persisted · not audited | ▶ ask keep/discard: keep → draft saved; discard → draft + staged uploads removed · nothing persisted | n/a (no external call) | ▶ `ValidateStep(Reason)` → stay on step, `lblValidation` + TopRight alert · nothing persisted | n/a |
| **2 Attachments** | staged files recorded in state (still in `AttachmentStaging`) · draft saved | ▶ discard → `AttachmentStaging.DiscardAll` cleans every staged upload; keep → they stay staged with the draft | n/a (upload is simulated in-memory) | ▶ Critical + 0 files → "a Critical escalation needs at least one attachment" · stay on step | upload rejected (not simulated) → same shape as validation: stay, message, nothing persisted |
| **3 Approver** | ApproverId in state · draft saved | as step 1 | **5 s `CancellationTokenSource`**: list keeps its previous content, hint says "the directory did not answer… your draft is saved", user can retry · draft untouched · nothing persisted | ▶ not in directory / not a Supervisor-Manager → stay on step with the reason (`PermissionService.CanApprove`) | directory down = the timeout cell; the workflow is never called |
| **4 Due date** | DueAtLocal in state · draft saved | as step 1 | n/a | ▶ < 1 h, > 14 d, or > 48 h on a Critical row → stay on step | n/a |
| **5 Notifications** | channel flags in state · draft saved | as step 1 | n/a | ▶ no channel selected → "pick at least one channel" | n/a |
| **6 Review** | ▶ `BuildCommand` → `EscalateAsync` (see part B) | as step 1 — a cancel here still discards or keeps the whole draft | n/a | ▶ command-level rules (self-approval, unknown work order) → `ValidationFailed`, wizard jumps to `FirstFailingStep` · nothing persisted | see part B |

**Browser refresh / closed tab** at any step behaves like *timeout*: the state object is on the server, so the
next open resumes at the same step with the same answers (`EscalationWizard.Resumed`).

## B · `EscalateAsync` — the five orchestration steps

| Step | Confirm | Cancel | Timeout | Validation failure | External failure |
|---|---|---|---|---|---|
| **validate** | continue to authorize | not cancellable (sub-second) | n/a | ▶ `ValidationFailed` + `FieldErrors` · **nothing persisted** · not audited · user returns to the failing step | n/a |
| **authorize** | continue to persist | n/a | n/a | wrong status / already escalated → `ValidationFailed` ("WO-x is Escalated") · nothing persisted | role or tenant denied → `Unauthorized`, safe message + correlation id · nothing persisted · the denial is traced by `Security:` |
| **persist** | escalation row created, work order → `Escalated` (version bumped), staging committed | n/a | a store timeout arrives here as an exception → same as the external-failure cell | stale `WorkOrderVersion` → exception → reverted (below) | **can** be undone: escalation removed, work order reverted, `RevertedPersist` compensation recorded, `Failed` returned · user is told to reload and retry |
| **notify** | receipt id in the log, `NotificationStatus.Sent` | n/a | SMTP timeout is the external-failure cell (a timeout tells you nothing about delivery) | n/a — the channels were validated in step 5 | ▶ **cannot** be undone: the escalation is **kept**, `NotificationStatus.ManualReview`, `NotificationOutstanding` compensation queued, `CreatedWithCompensation` + `CompensationAction: manual-review queued (#n)`; audited as "approver NOT notified" |
| **audit** | `escalation.created` written with the correlation id | n/a | as external failure | n/a | a sent notification cannot be unsent: `AuditGap` compensation recorded, ops alerted, escalation and notification both stand, `CreatedWithCompensation` |

## C · Recovery paths

| Compensation | How it is cleared | Result |
|---|---|---|
| `NotificationOutstanding` | ▶ **Retry notification** → `RetryNotificationAsync(entry, session)` | send succeeds → entry `Resolved`, `escalation.notify.retried` audited, `NotificationStatus.Sent`; send fails again → entry stays **Open**, the user is told it is still queued |
| `AuditGap` | an operator closes it after follow-up (`CompensationLog.Resolve`); **Retry notification** refuses it — the message is out | entry `Resolved` with who closed it and why |
| `RevertedPersist` | nothing to follow up: the work order is back where it was | entry is the record that it happened |

## How to run the whole matrix

| Button / action | Cell |
|---|---|
| Escalate → Next with an empty reason | A · 1 · validation |
| Escalate a Critical row → Next on step 2 with no file | A · 2 · validation |
| Pick `ben.tech` as approver → Next | A · 3 · validation |
| Default due date on WO-100232 → Next | A · 4 · validation |
| Uncheck all three channels → Next | A · 5 · validation |
| Pick `ana.ops` as approver → Finish | A · 6 / B · validate |
| Cancel → No | A · any · cancel (discard + staging cleanup) |
| Cancel → Yes, then reopen | A · any · cancel (draft kept) = resume |
| Finish the first e-mail escalation of the session (the simulated SMTP relay times out) | B · notify · external |
| **Retry notification** on the queued entry | C · NotificationOutstanding |

The remaining cells (A · 3 timeout, B · authorize denial, B · persist, B · audit, C · AuditGap) are ordinary code
paths in the wizard and the workflow; nothing in the sample forces them, so they are the workflow's test cases.
