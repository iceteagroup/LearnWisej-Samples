# Production-readiness note — the approval workflow

*Module 6 deliverable · TicketOps Console*

## What is ready

| Concern | State | Where |
|---|---|---|
| The dialog exposes a typed result; callers never read its controls | ✔ `ApprovalDialog.Result` is the only public member the caller uses; `radioApprove`, `radioReject`, `textComments` are private | `Dialogs/ApprovalDialog.cs`, `Dialogs/ApprovalDialogResult.cs` |
| Changes are applied only on `DialogResult.OK` | ✔ one gate in `buttonReview_Click`: `outcome != DialogResult.OK \|\| !result.Confirmed → return` | `Views/WorkOrderQueue.cs` |
| Cancel and ✕ leave the world untouched | ✔ `Result` defaults to `NotConfirmed()`; the dialog never mutates the `WorkOrder` it displays | `Dialogs/ApprovalDialog.cs` |
| Validation happens inside the dialog, before it can close | ✔ Reject + empty comments → `DialogResult.None`, error label, focus on the comments | `ApprovalDialog.buttonConfirm_Click` |
| Server-side re-check of what the UI checked | ✔ `ApprovalService` refuses unconfirmed results and rejections without comments regardless of the caller | `Services/ApprovalService.cs` (Gate 1, Gate 2) |
| The domain rule lives in the domain | ✔ `WorkOrder.CanDecide` / `Decide`; the service asks, the dialog never knew | `Domain/WorkOrder.cs` |
| One UI action = one transaction | ✔ status + audit + notification are buffered and committed together; a throw discards the working copy | `Data/InMemoryWorkOrderRepository.Commit` |
| Failure paths visible without leaking internals | ✔ expected refusals are `OperationResult.Fail(message)` shown in the banner; the outage's `sql01:1433` text stays in the trace, the user sees `Strings.ActionFailed` | `WorkOrderQueue.ShowResult` / `ReportFailure` |
| `async void` handlers own their try/catch | ✔ `buttonReview_Click`, `timerTests_Tick`, the two failure buttons | `Views/WorkOrderQueue.cs` |
| No per-user state in statics | ✔ `grep -rn "static" TicketOps/*.cs`: `Strings` constants, `SeedData` iterators, `OperationResult.Ok/Fail`, `StatusBanner.ColorFor`, the `Money` culture, `AppComposition.CurrentUser` const — all pure | — |
| Designer-friendly | ✔ every Form has a parameterless constructor and a `.Designer.cs` with layout only | `Views/`, `Dialogs/` |
| Testable without a browser | ✔ six cases in `Diagnostics/ResultHandlingTests.cs` run against `ApprovalService` with no Wisej.NET type | `docs/TestCases.md` |

## What must change before this ships

1. **Identity.** `AppComposition.CurrentUser` is a constant demo approver. Production takes `DecidedBy` from the
   authenticated principal (Module 11: `Application.User`) *inside the service*, never from the dialog or a screen field.
2. **Authorization.** `ApprovalService.ExecuteAsync` has three gates (confirmed, comments, `CanDecide`) but no
   *"may this user approve this amount?"* check. Add it as Gate 0 in the service — it is the one place a bulk job or an
   API cannot bypass — and return `OperationResult.Fail(Strings.NotAuthorized)`.
3. **Real transaction.** `IWorkOrderTransaction` is implemented in memory. The SQL implementation wraps a `DbTransaction`
   and keeps the same contract: `Update`, `RecordAudit`, `QueueNotification`, `CommitAsync`; `Dispose` without commit rolls back.
   The service does not change. Consider the outbox pattern for the notification so it is sent after — and only after — the commit.
4. **Concurrency.** Two approvers can open the dialog on the same pending order. `CanDecide` catches the second
   commit in this sample because the service re-reads the row, but a SQL store needs a row version (optimistic
   concurrency) or `UPDATE … WHERE Status = 'Pending'` with a row-count check inside the transaction.
5. **Dialog lifetime.** The dialog is created per click and disposed by the `using` block after `ShowDialogAsync`
   returns (Wisej.NET does not dispose modal dialogs automatically). Keep it that way; do not cache dialogs in fields.
6. **Reject reasons.** Free-text comments are enough for the lab. Production usually adds a reason code (a
   `ComboBox`) to the result so reporting can group rejections — another property on `ApprovalDialogResult`,
   not a new method parameter on the service (`ApprovalCommand` absorbs it).
7. **Audit retention and PII.** `ApprovalRecord` stores the approver's identity and comments; agree retention
   and access rules with the data owner before it lands in a real table.
8. **Tests.** Move `ResultHandlingTests` to an xUnit project as-is; add browser tests (Playwright) for the three dialog
   exits the unit cases cannot cover.

## Evidence

Run the app (`http://localhost:5106`): the **What to click** table in the README walks the success path, the two
failure paths, the error path with its recovery and the test run; every row states the trace lines that prove the
row above. `dotnet build -nologo -v q` — 0 warnings, 0 errors for `net10.0-windows` and `net10.0`.
