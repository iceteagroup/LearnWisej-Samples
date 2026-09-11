# Deliverable 1 — Wizard screen flow

**Screen:** `UI/EscalationWizard.cs` / `.Designer.cs` (a `Wisej.Web.Form`, opened modally from
`UI/WorkQueuePage.btnEscalate_Click` with `await wizard.ShowDialogAsync()`).
**Diagram:** [`wizard-screen-flow.svg`](wizard-screen-flow.svg)

## The six steps

| # | Step | Controls (Designer) | What the page writes into the state | Who decides whether Next is allowed |
|---|---|---|---|---|
| 1 | Reason | `txtReason` | `State.Reason` | `EscalationWorkflow.ValidateStep(state, Reason)` — 20…500 characters |
| 2 | Attachments | `btnStageFile`, `btnDiscardFile`, `lstAttachments` | `State.Attachments` (staged ids) | `ValidateStep(…, Attachments)` — a **Critical** work order needs ≥ 1 file |
| 3 | Approver | `cboApprover`, `btnLookupApprovers`, `lblApproverHint` | `State.ApproverId` | `ValidateStep(…, Approver)` — in the directory **and** `PermissionService.CanApprove` |
| 4 | Due date | `dtpDueDate` | `State.DueAtLocal` | `ValidateStep(…, DueDate)` — ≥ 1 h, ≤ 14 d, ≤ 48 h when Critical |
| 5 | Notifications | `chkNotifyEmail`, `chkNotifyInApp`, `chkNotifySms` | `State.NotifyEmail / InApp / Sms` | `ValidateStep(…, Notifications)` — at least one channel |
| 6 | Review | `dgvSummary`, `lblResultBanner`, `lblResultDetail` | — | `Finish` → `EscalateAsync(command)` re-validates **everything** |

The rail on the left (`stepsRail`, a `FlowLayoutPanel` with six labels) is the only navigation: it shows the
current step in blue and completed steps with a ✓. The dark status strip at the bottom (`lblWizardStatus`) says
which step is open, what the workflow is doing and, at the end, the `WorkflowResult`.

## Navigation rules

- **Next** copies the controls of the current step into the state object, asks the workflow to validate that
  step, and only then advances. An invalid step stays put: `lblValidation` shows what the service said and a
  TopRight `AlertBox` repeats it. Nothing is persisted.
- **Back** never validates — going backwards may not be blocked by a rule that is not satisfied yet.
- **Finish** (Review) builds the command and runs the workflow; while it runs, the status strip follows
  `validate → authorize → persist → notify → audit` from the workflow's `IProgress` reports.
- **Cancel** asks "keep this as a draft?" — Yes saves the state, No discards it and cleans the staged uploads up.
  Closing the window with the **X** is an interruption, not a decision: `EscalationWizard_FormClosing` saves the
  draft, so only *Cancel → No* ever throws collected work away.
- **Step 3** performs an external directory lookup bounded by a 5 s `CancellationTokenSource` held in an
  **instance** field. A timeout is not an error state: the draft is untouched and the learner can retry.

## Where the state lives

One typed object — `Services/Workflow/EscalationWizardState.cs` — holds every answer plus `CompletedSteps`.
The pages read from and write to it; nothing lives only in a control. After every completed step the wizard
calls `WorkflowStateStore.Save(state)`, so the flow is resumable: reopening the wizard for the same work order
picks the draft back up (`EscalationWizard.Resumed`), and the status strip says *Draft resumed*.

## Evidence in the running app

1. Select **WO-100232** (Critical) → **Escalate work order…**.
2. Click **Next** on an empty reason → *"Reason: a reason is required"*, still on step 1.
3. Type 25 characters, **Next** → step 2. Try **Next** with no file → *"a Critical escalation needs at least one
   attachment"*. Stage `vendor_quote.pdf`, **Next**.
4. Step 3 lists four people including `ben.tech`; pick him and **Next** → *"ben.tech is a Technician — only
   Supervisors and Managers approve"*. Pick `m.weber`, **Next**.
5. Step 4: leave the default (2 days) on a Critical row → *"a Critical work order must be resolved within
   48 hours"*. Pull it back to tomorrow, **Next**, **Next**.
6. Step 6 shows the summary the command will carry; the rail shows five ✓.
7. **Cancel → Yes** at any step, then **Escalate work order…** again: the wizard reopens on the same step with
   every answer intact and the status strip reads *Draft resumed*.
