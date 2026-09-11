# Validation rules — the Work Order editor

*Module 5 deliverable · TicketOps Console*

Every rule is plain C# with no UI type. The rules that depend only on the command live in
`Validation/WorkOrderValidator.cs` (pure: command in, `ValidationResult` out). The rules that need the stored
record or the caller's role live in `Domain/WorkOrderRules.cs` and run in the service only.

## Layered rules

| Layer | Rule | Where | Error channel | Message |
|---|---|---|---|---|
| Required | Title present | validator | field `Title` | Title is required. |
| Format | Title ≤ 120 characters | validator | field `Title` | Use at most 120 characters. |
| Required | Assignee present | validator | field `AssigneeId` | Assign the order to a technician. |
| Range | Cost in $0 … $10,000 | validator | field `EstimatedCost` | Cost must be between $0 and $10,000. |
| Range | Hours in 0 … 999 | validator | field `EstimatedHours` | Hours must be between 0 and 999. |
| Cross-field | Due date not in the past while the order is still open (New / Assigned / InProgress / OnHold) | validator | field `DueDate` | Due date can't be in the past. |
| Cross-field | A Closed order has no future due date | validator | field `DueDate` | A closed order cannot have a due date in the future. |
| State machine | `FromStatus → ToStatus` is in the transition table | validator (editor's `FromStatus`) **and** rules (stored status) | summary | Cannot move a {From} order to {To}. |
| Business | A Closed order is frozen; only a Supervisor may reopen it (→ Assigned) | rules | summary | Closed work orders cannot be edited. Ask a Supervisor to reopen it. / Only a Supervisor may reopen a closed work order. |
| Authorization | Cost above $2,500 needs a Supervisor | rules | summary | Only a Supervisor may set a cost above $2,500. |
| Existence | The order still exists | service | summary | The work order no longer exists. Refresh the list. |

## The transition table (`Domain/WorkOrderTransitions.cs`)

| From | Allowed to | Note |
|---|---|---|
| New | Assigned, Cancelled | |
| Assigned | InProgress, Cancelled | |
| InProgress | OnHold, Completed | |
| OnHold | InProgress, Cancelled | |
| Completed | Closed | |
| Closed | — | reopen to Assigned is **Privileged** (Supervisor only), not a transition |
| Cancelled | — | |

`Classify(from, to)` answers `Legal` (same status or a row above), `Privileged` (Closed → Assigned) or `Illegal`.
The validator flags only `Illegal`; the rules flag `Privileged` unless the session's role is Supervisor.

## Where each kind runs

| Kind | Editor pre-check (UX) | Service (guard) |
|---|---|---|
| Required, format, range, cross-field | yes — `_validator.Validate` in `buttonSave_Click` | yes — the same class, step 1 of `SaveAsync` |
| State machine | yes, against what the editor knew | yes, against the **stored** status (TC-15) |
| Business (closed freeze) | no — the editor does not know the store | yes |
| Authorization (role) | no — the editor does not know the role | yes — `SessionContext.Role`, never the command |

## Why the command carries no role

`SaveWorkOrderCommand` is client input: an import, a replayed request or a rogue client can build one. If it carried
`ActorRole = Supervisor` the server would be trusting the caller's claim. The role is read from the per-session
`SessionContext` that `AppComposition` created on the server, and only `WorkOrderRules.Check` looks at it.

## Evidence

`ValidationTestCases` TC-02 … TC-10 exercise the validator rows above, TC-11 … TC-15 the rules rows. In the running
app the same rules show up as glyphs and the summary panel (see `ErrorUxGuidelines.md`).
