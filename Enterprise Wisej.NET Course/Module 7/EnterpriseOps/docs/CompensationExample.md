# Deliverable 5 — Compensation example

**The case:** the notification fails **after** the escalation was persisted.
**Files:** `Services/Workflow/EscalationWorkflow.cs` (step 4 and step 5), `Services/Workflow/CompensationLog.cs`,
`Integrations/FakeNotificationGateway.cs`, `Security/AuditLog.cs`.

## Why a transaction is not the answer

`persist` writes to the escalation store and the work-order store — a database transaction covers both.
`notify` goes to an e-mail / in-app / SMS provider, which is outside every transaction: once the provider has
accepted the message, no `ROLLBACK` reaches it, and a timeout does not even tell you whether it was accepted.
A distributed transaction across the two is rarely available and rarely worth its cost.

So the rule for every step that crosses a system boundary is: **decide, before writing the code, what the system
should look like if this step fails after the previous ones succeeded.**

## The design decision

| Step that fails | Can it be undone? | What the system should look like | Compensating action |
|---|---|---|---|
| `persist` | yes — nothing external happened yet | as if the escalation was never requested | remove the row, revert the work order's status + version, record `RevertedPersist` |
| `notify` | **no** — and the escalation is still valid | escalation exists, everybody knows the approver was not told | keep the escalation, `NotificationStatus.ManualReview`, queue `NotificationOutstanding`, audit "approver NOT notified" |
| `audit` | **no** — the notification is already out | escalation and notification stand, the gap is visible | record `AuditGap`, alert ops, resolve manually (a sent message cannot be unsent) |

Deleting a valid escalation because an e-mail bounced is the failure this module is about: it looks like a
rollback, it is a *false transaction*, and it throws away the record that anything happened.

## The code

```csharp
// ── 4. notify (external — outside any transaction) ──────────────────────────
try
{
    var receipt = await _notifications.SendApproverNotificationAsync(escalation, approver);
    _escalations.SetNotificationStatus(escalation.Id, NotificationStatus.Sent);
    Report("notify", StepStatus.Succeeded, receipt.MessageId);
}
catch (NotificationFailedException ex)
{
    // COMPENSATION: the escalation is valid and stays. Record that the notification is outstanding
    // and queue it for manual review / retry — do not pretend the whole thing never happened.
    _escalations.SetNotificationStatus(escalation.Id, NotificationStatus.ManualReview);
    var entry = _compensation.Record(CompensationKind.NotificationOutstanding, command.WorkOrderId, escalation.Id,
        "notify", ex.Message, $"manual-review queued: retry approver notification for {escalation.Number}", command.CorrelationId);
    compensationAction = $"manual-review queued (#{entry.Id})";
    Report("notify", StepStatus.Compensated, $"{ex.Message} → {compensationAction}");
}
```

and the result the caller gets:

```csharp
new WorkflowResult(true, "Escalation created — approver NOT notified.", compensationAction)
{
    Outcome = WorkflowOutcome.CreatedWithCompensation,
    EscalationId = escalation.Id,
    NextAction = "the manual-review queue retries the notification; nothing to redo",
}
```

`Success` is **true**: a valid escalation exists. The compensation is not an error code, it is the second half of
the truth. The `audit` step then records *why* the notification is missing, so the audit trail explains the gap
instead of hiding it.

## Resumable: the workflow finishes later

`RetryNotificationAsync(entry, session)` is the compensation's other half. It re-sends, sets
`NotificationStatus.Sent`, resolves the log entry and writes `escalation.notify.retried` with a **new**
correlation id. If the provider is still down, the entry stays open and the user is told so — the workflow is
allowed to finish later, it is not allowed to disappear.

## The counter-example

The same flow written inside a screen handler would re-invent the rules, talk to the stores and the gateway
directly, carry no correlation id and write no audit entry — and when the notification throws, its `catch` would
**delete** the escalation and revert the work order. Same failure, two outcomes: the workflow keeps a valid
escalation and a queued retry; the page loses both.

## Evidence in the running app

The simulated notification failure lives in `FakeNotificationGateway`: its SMTP relay times out on the **first
e-mail of the session** and delivers every send after that.

1. Select a work order → **Escalate work order…** → complete the six steps (E-mail checked) → **Finish**.
2. The amber banner reads *"Escalation created — approver NOT notified."* with
   `persist ✓ · notify ✕ · CompensationAction: manual-review queued (#1) · audited`, and the status strip shows the
   `WorkflowResult` line.
3. Close the wizard: the escalation is in the grid (status `Escalated`), and the **manual-review queue** shows
   `#1 OPEN  NotificationOutstanding  ESC-1041  notify ✕ smtp timeout after 30s → manual-review queued…`.
4. Select it → **Retry notification** → the entry is resolved, `escalation.notify.retried` is audited, and the
   banner reports that the approver was notified on retry.

The audit-gap branch (step 5) is ordinary code in `EscalationWorkflow`; nothing in the sample forces the audit
store to fail, so it is covered by the matrix as a test case rather than clicked in the app.
