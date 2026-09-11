# Unit-style test cases — result handling

*Module 6 deliverable · TicketOps Console*

The cases below are implemented in `Diagnostics/ResultHandlingTests.cs`. Every case builds its own fixture — a freshly seeded `InMemoryWorkOrderRepository` and an
`ApprovalService` — and hands the service an `ApprovalDialogResult` exactly as the dialog would. No
dialog, no control, no browser is involved, which is the point: the file would move to an xUnit project
unchanged (`[Fact]` per method, `Assert.True(outcome.Passed)`).

Fixture facts: the seed holds 6 work orders (WO-2001 **Approved**, WO-2006 **Rejected**, four pending) and
2 audit rows. `PendingOrder` = 2002 (*Repair loading dock pump*), `DecidedOrder` = 2001.

| Id | Case | Arrange | Act | Assert |
|---|---|---|---|---|
| **RH-1** | Approve, confirmed | fresh fixture | `ApplyAsync(2002, Confirm(Approve, "Quote checked…"))` | `Succeeded`; WO-2002 `Status == Approved`, `DecidedBy == "test-approver"`; `AuditCount == 3`; `NotificationCount == 1` |
| **RH-2** | Reject with comments, confirmed | fresh fixture | `ApplyAsync(2002, Confirm(Reject, "Pump model mismatch — needs a re-quote first."))` | `Succeeded`; `Status == Rejected`; `DecisionComments` equals the comments; `AuditCount == 3` |
| **RH-3** | Reject without comments (forged past the dialog) | fresh fixture | `ApplyAsync(2002, Confirm(Reject, "   "))` | `!Succeeded`; `Message == Strings.CommentsRequiredToReject`; `Status == Pending`; `AuditCount == 2`; `NotificationCount == 0` |
| **RH-4** | Not confirmed (Cancel / ✕) | fresh fixture | `ApplyAsync(2002, NotConfirmed())` | `!Succeeded`; `Message == Strings.DecisionNotConfirmed`; `Status == Pending`; `DecidedBy == null`; `AuditCount == 2` |
| **RH-5** | Decide an already-decided order | fresh fixture, snapshot WO-2001 | `ApplyAsync(2001, Confirm(Reject, "Trying to flip a closed decision."))` | `!Succeeded`; message contains *already approved*; status, `DecidedBy`, `DecisionComments` unchanged; `AuditCount == 2` |
| **RH-6** | Repository outage during commit, then recovery | fresh repository wrapped in the test double `FailingCommitRepository`, `FailCommits = true` | `ApplyAsync(2002, Confirm(Approve, …))` → catch; `FailCommits = false`; same call again | first call throws; after it `Status == Pending`, `DecidedBy == null`, `AuditCount == 2`, `NotificationCount == 0` (nothing partially applied); retry `Succeeded`, `Status == Approved`, `AuditCount == 3` |

## What each case proves

- **RH-1 / RH-2** — a confirmed result becomes one committed unit: status, audit row and notification change together.
- **RH-3** — the comments rule holds on the server side of the boundary, not only in the dialog's Confirm handler.
- **RH-4** — Cancel and ✕ are indistinguishable to the service and both are refused; the handler's gate is the first line, this is the second.
- **RH-5** — the domain rule (`WorkOrder.CanDecide`) is enforced by the service; the dialog never knew it and the screen never checked it.
- **RH-6** — the transaction boundary is real: when the commit fails, none of the three buffered writes reaches the store, and the same confirmed result applies cleanly once the store is back.

## Not covered here (needs the browser)

The dialog's own behaviour — *Confirm with Reject and empty comments keeps the dialog open*, *Cancel/✕ leave
`Result.Confirmed == false`*, *the caller never reads a control* — is exercised by hand in the browser; see
the README's *What to try* table, exactly as the lab guide's step 9 asks.
