# Production-readiness note — Work Order validation & save

*Module 5 · lab step 9*

## What is production-ready in this sample

- **The rules run where they cannot be skipped.** `WorkOrderService.SaveAsync` re-validates every command and applies
  `WorkOrderRules` with the server-side role before any write. The editor's pre-check is a convenience; deleting it
  would change the UX, not the safety.
- **No partial writes.** The order and its audit entry are staged in one `IWorkOrderTransaction` and committed once.
  A commit failure rolls back and surfaces as an exception; `RowVersion` proves the row is untouched.
- **Errors are collected and mapped.** Field and summary errors travel as data (`ValidationResult`) to one display
  method; unexpected failures are logged in full and shown as `Strings.SaveFailed` only.
- **Rules are testable without a browser.** `ValidationTestCases` runs against the real validator and rules with a
  pinned "today"; it moves into an xUnit project unchanged.
- **Nothing per-user is static.** Log, session context, repository, validator and service are created per session
  in `AppComposition`.

## What a real deployment still needs

| Gap | Why it matters | Where it goes |
|---|---|---|
| A real transaction | the in-memory `Transaction` stages and applies; SQL needs `BeginTransaction`/`Commit` on one connection | `Data/SqlWorkOrderRepository` implementing the same `IWorkOrderRepository` — the service does not change |
| Optimistic concurrency | two users editing #2002: the second save must fail with "changed since you opened it" | compare `RowVersion` in `Upsert`; surface as a summary error, not an exception |
| A confirm step for irreversible moves | Cancelled / Closed should ask first | Module 6 (`ApprovalDialog`) between validate and persist |
| Real authentication | the **Acting as** combo simulates the session's role for the lab | Module 11: `Application.User` populates `SessionContext.Role` at login; the combo disappears |
| Per-field `Validating` events | tab-out feedback before Save | call the same `WorkOrderValidator` from `Control.Validating`; do not add a second set of rules |
| Audit persistence and retention | the audit list lives in memory | write the audit row in the same transaction as today, to a real table |
| Localised messages | messages are English constants | Module 10: move them to `.resx`; the field/summary split stays |

## Review checklist (applied)

| Check | Result | Evidence |
|---|---|---|
| Every important rule is re-validated on the server | ✔ | `SaveAsync` step 1 + 2; **Bypass: crafted command** is rejected without ever touching the editor |
| Writes are transactional — no path can leave the record half-saved | ✔ | `tx#n rolled back — 0 rows changed`, `re-read #2002 … unchanged, nothing partial` |
| Failure paths are visible, logged, explained without leaking internals | ✔ | `sql01:1433` appears in the trace only; the banner shows `Strings.SaveFailed` |
| Business logic is not trapped in event handlers | ✔ | `WorkOrderEditor.cs` contains no rule about a work order; `grep -n "Threshold\|IsLegalTransition" Views/*.cs` finds nothing |
| The screen remains usable in the Designer | ✔ | `WorkOrderEditor.Designer.cs` holds the whole layout, parameterless constructor present |
| The deliverable can be reviewed without running the whole course | ✔ | this `docs/` folder + `README.md`; the app runs standalone on port 5105 |
