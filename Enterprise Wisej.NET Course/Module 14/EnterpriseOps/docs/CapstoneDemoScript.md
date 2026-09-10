# Capstone demo script — EnterpriseOps Command Center

**Deliverable:** Capstone demo script · **Module 14** · last verified 2026-09-10 · owner ana.ops

The capstone is delivered as a **production architecture review**, not a feature demo. Each stop pairs a
working screen with a failure path, because the review is about what happens when things go wrong.

Run it with the trace card visible at all times: it is the evidence that the handler asked and the service
decided.

```bash
cd "D:/Projects/LearnWisej-Samples/Enterprise Wisej.NET Course/Module 14/EnterpriseOps"
dotnet run -f net10.0 --urls http://localhost:5214
```

Total: about twelve minutes, plus questions. Say the sentence in **bold** at each stop before clicking.

## Stop 1 · Where state lives (2 min)

**"One process, many sessions — nothing about a user is static."**

- Point at the header: tenant, signed-in user, correlation id.
- `Program.Main` creates one `SessionContext` per browser session and parks it in `Application.Session`;
  `ServiceRegistry` builds that session's services. The trace opens with
  `Service: session services ready · tenant contoso · docs …`.
- Show `Security/Permissions.cs`: the permission matrix **is** static — immutable reference data. The
  distinction between a static constant and static state is the one the review checklist enforces (Q2).

**Expected question:** "What happens with two browsers?" — Two `SessionContext` objects, two stores, one
catalog and one permission matrix.

## Stop 2 · A command through the layers (2 min)

**"Every command is authorized in the service, versioned in the store and audited either way."**

- Select a work order that is `InProgress` or `Escalated`; press **✓ Approve selected**.
- Read the trace top to bottom: `UI → btnApprove_Click`, `Service: ApproveWorkOrderCommand #… v… corr=…`,
  `Security: ApproveWorkOrder granted to ana.ops`, `Data: update work_order #… → v…`, `Data: audit ← …`.
- The footer shows the audit line; the KPI cards recompute from `DashboardService`.

**Expected question:** "Where is the authorization?" — In `WorkOrderService.ApproveAsync`, before the read.
The handler cannot skip it, because the handler cannot reach the store.

## Stop 3 · Two failure paths and a recovery (3 min)

**"The review is about what happens when things go wrong."**

- **Stale version:** with a row selected, press **Fail: stale version**. Another session bumps the row's
  version; the approve carries the version the screen saw and the store refuses it. Red banner: *"#… changed
  while you were looking at it (you saw v3, it is now v4)."* The denial is audited.
- **Recovery:** **Recover: reload queue**, then approve again. Same code path, current version, success.
- **Permission denied:** **Switch to ben.tech**, then **✓ Approve selected**. The service refuses,
  `Security: ApproveWorkOrder denied — ben.tech (Technician) is not allowed to approve work orders`, and the
  refusal is written to the audit log. A denial nobody records is an invisible attack.
- Switch back to `ana.ops`.

## Stop 4 · Operations (2 min)

**"What would the team do at two in the morning?"**

- Press **▶ Run health check**. Six probes fill in one at a time — session context, work-order store, audit
  log, documentation index, capstone package, review engine — each with the evidence it found and how long
  it took.
- Press **■ Cancel** mid-run on the second pass: the `CancellationTokenSource` lives in an instance field and
  the run stops at the next probe.
- Point out that the diagnostics card answers questions, not "OK": *120 rows total, 40 visible to contoso*
  is evidence; a green tick is not.

## Stop 5 · The capstone package (2 min)

**"The package proves the system, not just the screens."**

- **Capstone review →**, then **✓ Verify package**. Every deliverable is checked against the file on disk:
  its size, and the section that proves it is really that deliverable. Green: *ready to submit*.
- **Documentation index** tab: `docs/index.json` read back, every path resolved. The trace shows the
  `resources/list` answer a documentation MCP endpoint would return.
- **Fail: missing document** → an indexed document that is not on disk. The index turns red (*would answer
  404*) and the package stops passing. Press it again to recover.

## Stop 6 · The AI review gate (3 min)

**"Generated code is a draft from an unknown contributor."**

- **Generated-code review** tab → **Load AI draft #214**. Read the code aloud: it is short, it compiles, and
  in a single-user demo the screen works.
- **▶ Review generated code** →
  `● REJECTED — 9 blocking finding(s) of 11 over 41 lines · 10 rules run`.
- Walk two findings: `Q2 L10 static WorkOrder _current` (every tenant shares one selection) and
  `Q1 L18 grid.EnableSmartVirtualScroll()` (not in `Wisej.Framework.xml` — the API does not exist).
- **Load rev 2 (fixed)** → **▶ Review generated code** → `● ACCEPTED — no checklist finding`.
- **Sign the decision** → the **AI usage notes** tab records what was accepted, by whom and why.

**Expected question:** "Could the gate be wrong?" — Yes; a Q1 finding proves nobody has shown the API exists,
not that it does not. Answer it in the decision record; do not delete the finding.

## Stop 7 · The readiness statement (1 min)

**"What is not done, and who owns it."**

- Open `docs/ProductionReadiness.md` and read the **Open items** table out loud, with the owners.
- Close on the sign-off: state ownership, security implications, failure paths, production behaviour —
  explained, not assumed.

## If something goes wrong during the demo

| Symptom | Cause | What to do |
|---|---|---|
| the package tab shows `docs/ folder not found` | the app was started from somewhere other than the project folder | stop, `cd` into `EnterpriseOps`, run again |
| **Approve** is disabled | no row selected, or signed in as `ben.tech` | select a row; switch back to `ana.ops` |
| every approve is refused with "only work that is InProgress or Escalated" | the selected row is `Completed` or `New` | change the filter to **Open** and pick another row |
| the trace is empty | it was cleared | it is per session; keep demonstrating, it refills |
