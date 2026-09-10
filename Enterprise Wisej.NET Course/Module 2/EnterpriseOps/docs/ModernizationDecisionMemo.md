# Modernization decision memo

Deliverable 5 of the lab. Not an essay — a record of **what was decided, why, on what evidence, and who decided**.
It is written *after* the path has run, from what the run produced.

In the sample the memo is **generated**, never typed: `Services/MigrationAssessmentService.BuildDecisionMemo`
reads the dossier, the seven step states, the last harness verdict and the current theme diff, and returns the
text that the *Decision memo* tab shows. Click **Write decision memo** before the run and after it — the two
versions differ, because the evidence differs. That is the point.

---

## MODERNIZATION DECISION MEMO — TicketOps Console → EnterpriseOps baseline

**Tenant** contoso  **Author** ana.ops (Manager)  **Correlation id** per run

### DECISION

Migrate incrementally: .NET Framework 4.8 → .NET 10 and Wisej.NET 3.5 → 4.1 in **seven verifiable steps**, each
with a fallback point. The uncontrolled rewrite ("upgrade everything, fix what breaks") is rejected.

### WHY THIS PATH

- The dossier holds **7 areas** — risk **H×3 M×3 L×1**. Every High row is *breaking* **and** *user-visible*, and
  each one has a named regression proof and its own fallback point.
- The compatibility matrix has **3 blocked combinations**, each with a mitigation. None of them is "we will see" —
  see `RiskMatrix.md`.
- A rewrite has no fallback point at all. The review question *"which migration step can be rolled back
  independently?"* would answer *"none"*, which is the definition of the failure mode this module exists to avoid.
- The rejected alternative — "upgrade everything at once, fix what breaks" — is faster only until the first silent
  breakage. In this codebase the first silent breakage is the theme, and it does not raise an exception.

### EVIDENCE (from the running sample, not from hope)

- **Steps passed**: 7/7 after the fix; 3/7 with step 4 failed on the first pass.
- **Regression harness**: `FAIL — 7/10 flows, 3 theme flows broken` before the mixin was mapped;
  `PASS — 10/10 flows` afterwards.
- **Theme**: `Blue-2019 mixin (Wisej.NET 4)` — 0 differences from the 3.5 baseline (accent `#1565D8`,
  radius `7`, 3/3 priority colours).
- **Performance**: work queue query measured against a 250 ms budget in step 7.

### WHAT MUST REMAIN UNCHANGED

The answer to the review question *"what exact behavior must remain unchanged?"*, in the form the harness can
check:

1. The tab filter is decided **on the server** — `page.Total` equals the store count for the bucket (flow 3).
2. A blank title is rejected with a message and **nothing is written** (flow 4).
3. A stale `Version` is refused; the current one is accepted (flow 5).
4. `ana.ops` (Manager) may approve; `ben.tech` (Technician) may not — checked server-side (flow 1).
5. Accent `#1565D8`, corner radius `7 px`, priority colours High red / Normal amber / Low green (flows 6–8).
6. One `SessionContext` per session, held in `Application.Session`, **zero static fields** (flow 9).
7. Background job progress reaches the UI (flow 10).

Everything else is allowed to change: project layout, target framework, startup file, package set, hosting model.

### ROLLBACK

`git tag pre-fx` · `package pin 3.5` · `config backup` · `theme folder copy` · `feature flag off` ·
`standalone gate` · `previous package`.

Roll back the failed step **only**, fix on the fallback, re-run the harness. Never fix forward on a broken build —
the workflow refuses it in code, not in a guideline. See `RollbackPlan.md`.

### RISKS ACCEPTED

- **EF 6.4 stays** for this migration (Module 4 moves it). It is supported on the target; carrying it is cheaper
  than doing two hard migrations at once.
- **The two JS interop widgets** ship behind a feature flag until flow-level evidence exists. If step 5 fails they
  go off and the screens keep working.

### DECIDED BY

`ana.ops` (Manager) with `cara.admin` (Admin), reviewed against `MigrationDossier.md`, `MigrationInventory.md`,
`RiskMatrix.md`, `RegressionPlan.md` and `RollbackPlan.md`.

---

## Evidence — what the running app shows

- Click **Write decision memo** with nothing run: the memo says
  `Steps passed: 0/7` and `Regression harness: not run yet.` — an honest memo with no evidence behind it.
- Run the path, hit the step-4 failure, then write the memo again: it names the failed step and its fallback point.
- Finish the path (roll back → map the mixin → re-run) and write it once more:
  `Steps passed: 7/7`, `Regression harness: PASS — 10/10 flows`, `Theme: Blue-2019 mixin (Wisej.NET 4) — 0 difference(s)`.
