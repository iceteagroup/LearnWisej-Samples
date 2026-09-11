# AI usage notes — EnterpriseOps Command Center

**Deliverable:** AI usage notes · **Module 14** · last verified 2026-09-10 · owner cara.admin

Which generated code was accepted, by whom, and why. One entry per change an assistant drafted, whether it
was accepted or not — a record that lists only the acceptances cannot be audited.

The running app appends to this record in memory (Capstone Review → **Sign the decision** → **AI usage
notes**); this file is the written copy that survives the session.

## Where the assistant was used

| Area | Used | Outcome |
|---|---|---|
| Service skeletons from written command definitions | yes | accepted after review, typically with one or two findings |
| Test-case lists from the failure-path matrix | yes | accepted; cheap to verify, no production risk |
| Documentation index entries (`index.json`) | yes | accepted; every `lastVerified` date supplied by a person |
| Migration inventories from legacy solutions | yes | accepted as a starting list, checked by hand |
| Permission matrix and authorization code | **no** | verification is harder than writing it (see the patterns note) |
| Concurrency and the optimistic-version path | **no** | same reason |

## PR #214 — "WorkOrder screen helper"

| | |
|---|---|
| **Author** | ai-assistant, from the prompt "add a work order screen that lists the open orders for a tenant and lets a manager approve one" |
| **Reviewer** | cara.admin (Admin) — not the author |
| **Rev 1 verdict** | **REJECTED** — 11 checklist findings over 41 lines, 9 of them blocking |
| **Rev 2 verdict** | **ACCEPTED** — no finding |

### Why rev 1 was rejected

It compiled, and in a single-user demo the screen looked right. That is the whole problem.

- **Q2 (×3)** — `static WorkOrder _current`, `static string _tenantId`, `static DataGridView _grid`. One
  process serves every browser session: every tenant would have shared one "current" work order, and the
  cached grid belonged to a session that may already have gone.
- **Q1 (×2)** — `grid.EnableSmartVirtualScroll()` and `Application.RegisterBackgroundJob(…)`. Neither exists.
  Both are plausible enough that a reviewer skimming the diff would not have blinked; neither is in
  `Wisej.Framework.xml`.
- **R9** — the tenant came from a parameter named `tenantIdFromClient`. Anyone who could change the request
  could read another tenant's work orders.
- **Q3** — the change wrote and contained no permission check at all.
- **R7** — `catch (Exception) { }`. The incident would have been invisible to the user and to the log.
- **Q5** — a background job with no `IsDisposed` guard and no `Application.Update`.
- **Q6, R10** (warnings) — `Bind` queried the database while holding a `DataGridView`; `btnApprove_Click`
  persisted inside the handler.

Sent back with the prompt header attached and links to `DataGridView.VirtualMode` and
`Application.StartTask` in the documentation.

### Why rev 2 was accepted

- an instance `WorkOrderScreenService`, session-scoped state, dependencies injected;
- authorization checked in the service before the write, refusal returned as a `CommandResult`;
- the failure path visible: `ConcurrencyException` logged with the correlation id and turned into a sentence
  a user can act on;
- `DataGridView.VirtualMode` used and **cited** from the documentation;
- the whole path testable without opening a screen.

A reviewer can explain every line of it without the tool present. That was the condition.

## Standing rules that came out of this

1. A generated change is reviewed by someone who is not its author, and the signature is recorded.
2. Any platform member a reviewer cannot find in the documentation blocks the merge, even if it "looks right".
3. Statics are read as session state until proven otherwise.
4. A prompt without the header is not a prompt from this project.

## Evidence

Capstone Review → **Load AI draft #214** → **Review generated code** → **Sign the decision**: the decision
appears in the **AI usage notes** tab with its verdict, the author, the reviewer and the reason, and the
audit line `SignReviewDecision … → ok` is written with the same correlation id. A Technician is refused: a
Technician does not sign reviews.
