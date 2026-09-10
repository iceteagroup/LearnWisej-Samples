# Validation test cases

*Module 5 deliverable · TicketOps Console · Work Order editor*

The rules are pure C# (`Validation/WorkOrderValidator.cs`, `Domain/WorkOrderRules.cs`), so every case below
is one method call with no form on screen. The same table lives in code as `Validation/ValidationTestCases.All()`;
**▶ Run 15 test cases** in the app executes it through `ValidationTestRunner` and writes one `[DOMAIN]` line per
case (`… → PASS` / `… → FAIL`). Moving the file into an xUnit project needs no change — nothing in it references Wisej.NET.

"Today" is pinned to 2026-09-10 inside the runner so the due-date cases never rot.

## The baseline command

Every case starts from this valid command and changes one thing ("the valid command, except …"):

| Field | Value |
|---|---|
| Id | 2002 |
| Title | Repair loading dock pump |
| AssigneeId | T. Nguyen |
| DueDate | today + 10 days |
| EstimatedCost | 2,150.00 |
| EstimatedHours | 6 |
| FromStatus → ToStatus | Assigned → InProgress |

## Validator cases (`WorkOrderValidator.Validate(command)`)

| # | Case | Input change | Expected result | Message |
|---|---|---|---|---|
| TC-01 | valid edit passes every rule | — | valid | — |
| TC-02 | empty title | `Title = "   "` | field error **Title** | Title is required. |
| TC-03 | title longer than 120 characters | 121 × `x` | field error **Title** | Use at most 120 characters. |
| TC-04 | no assignee | `AssigneeId = ""` | field error **AssigneeId** | Assign the order to a technician. |
| TC-05 | cost above $10,000 | `EstimatedCost = 25,000` | field error **EstimatedCost** | Cost must be between $0 and $10,000. |
| TC-06 | hours out of range | `EstimatedHours = 1,200` | field error **EstimatedHours** | Hours must be between 0 and 999. |
| TC-07 | due date in the past on an open order | `DueDate = today − 8` | field error **DueDate** | Due date can't be in the past. |
| TC-08 | closed order with a future due date | Completed → Closed, `DueDate = today + 10` | field error **DueDate** | A closed order cannot have a due date in the future. |
| TC-09 | illegal transition | New → Completed | 1 summary error | Cannot move a New order to Completed. |
| TC-10 | three problems collected at once | empty title, past due date, cost 25,000 | field errors **Title + DueDate + EstimatedCost** | all three, in one result |

## Rule cases (`WorkOrderRules.Check(stored, command, role)`)

These need state the browser has no authority over — the status the store holds and the role the server knows —
so they run in the service, after the validator and before the write.

| # | Case | Stored status | Role | Command | Expected result | Message |
|---|---|---|---|---|---|---|
| TC-11 | closed order edited by a Technician | Closed (#2006) | Technician | Closed → Closed | 1 summary error | Closed work orders cannot be edited. Ask a Supervisor to reopen it. |
| TC-12 | Supervisor reopens a closed order | Closed (#2006) | Supervisor | Closed → Assigned | valid | — |
| TC-13 | Technician sets cost above the $2,500 threshold | Assigned | Technician | cost 9,500 | 1 summary error | Only a Supervisor may set a cost above $2,500. |
| TC-14 | Supervisor sets cost above the threshold | Assigned | Supervisor | cost 9,500 | valid | — |
| TC-15 | stale editor: stored status moved on | Completed | Supervisor | Assigned → InProgress | 1 summary error | Cannot move a Completed order to InProgress. |

TC-15 is why the service re-reads the stored record: the editor remembered "Assigned", but another user completed
the order meanwhile. The validator (which only knows the editor's `FromStatus`) says the move is legal; the rules,
run against the stored status, say no.

## Cases the app exercises end to end (not just the rules)

| Path | Button | Proves |
|---|---|---|
| Error path | **Simulate write outage** then **Save** (the button saves for you) | validation and rules pass, `COMMIT tx#n` fails, `tx#n rolled back — 0 rows changed`, the user sees one safe sentence, the editor keeps the edits, `VerifyNothingPartial` re-reads the row and finds `v1 … unchanged` |
| Recovery | **Recover the data store** | the same command commits: `tx#n+1 committed`, `#2002 now v2` |
| UI bypass | **Bypass: crafted command** | a command that never touched the editor is still rejected by the server's role rule |

## Evidence

- Click **▶ Run 15 test cases**: the progress bar advances one case per tick, the status reads
  **● running TC-07 · 7/7 passed**, and the trace shows fifteen `[DOMAIN] WorkOrderValidator.Validate — TC-nn "…" · expected … · got … → PASS`
  / `[DOMAIN] WorkOrderRules.Check — …` lines, then `[UI] test run complete — 15/15 PASS` and status **● 15/15 test cases passed**.
- To watch a case fail, change an expectation in `ValidationTestCases.cs` (e.g. TC-09 `ExpectedSummaryErrors = 0`): the
  runner logs `⚠ … → FAIL` and the status turns orange **● 14/15 test cases passed**. Nothing else in the app changes,
  because the runner never touches a control or the repository.
