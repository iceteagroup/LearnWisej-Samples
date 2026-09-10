# Regression test plan — ten key flows

Deliverable 3 of the lab. The harness protects **behavior, not compilation**. Every flow below calls the same
service the screen calls and compares the outcome with what the Wisej.NET 3.5 app did. A flow can fail on a build
that has zero errors and zero warnings — that is the whole point.

Source of truth in the sample: `Data/MigrationInventoryStore.Flows()` (the plan) and
`Services/RegressionHarnessService.ExecuteAsync` (the assertions). Shown on the
*Regression plan (10 flows)* tab of `MigrationDossierPage`.

| # | Flow | Screen | Category | Expected (the 3.5 behaviour) | How the harness proves it |
|---|---|---|---|---|---|
| 1 | Login + role check | Login / any | Security | `ana.ops` may approve, `ben.tech` may not | `PermissionService.IsAllowed` for both roles; **both** halves must hold |
| 2 | Open work queue (Open tab) | `WorkOrdersPage` | Behavior | only New/Assigned/OnHold rows of the tenant, due date ascending | `WorkOrderService.Query(Open)`; asserts the statuses and the order |
| 3 | Filter *In progress* (server-side) | `WorkOrdersPage` | Behavior | the filter runs on the server; only InProgress/Escalated rows | compares `page.Total` with `CountInStore` — equal means the server filtered |
| 4 | New work order — validation | `WorkOrdersPage` | Behavior | a blank title is rejected with a message, nothing saved | `Save({ Title = "   " })`; asserts the error **and** that the row count did not move |
| 5 | Approve with stale version | `WorkOrdersPage` | Behavior | a stale `Version` is rejected; the current one is accepted | `Approve(v-1)` must fail with "stale", `Approve(v)` must succeed |
| 6 | Theme: accent colour on header + tabs | `WorkOrdersPage` | Theme | accent `#1565D8` as on 3.5 | token diff against the 3.5 baseline map |
| 7 | Theme: corner radius on tabs + buttons | `WorkOrdersPage` | Theme | radius `7 px` as on 3.5 | token diff against the 3.5 baseline map |
| 8 | Theme: priority colours in the grid | `WorkOrdersPage` | Theme | High red, Normal amber, Low green | all three tokens must match; counts the missing ones |
| 9 | Session context survives navigation | Dossier ⇄ Work Orders | Session | the same `SessionContext` instance, no static user state | `ReferenceEquals(Application.Session.Context, session)` **and** reflection: `SessionContext` must have **0** static fields |
| 10 | Background import progress reaches UI | `ImportCenterPage` | Background task | 3 progress ticks observed | counts the ticks of a simulated job |

## Five categories, because five kinds of thing break

`Security · Behavior · Theme · Session · BackgroundTask` (`Domain/RegressionFlow.FlowCategory`). A plan made only
of "does the screen open" flows would have passed the broken build in the walkthrough. The theme flows exist
precisely because the compiler has nothing to say about them.

## Which flows run at which step

| Step | Flows | Why |
|---|---|---|
| 3 Compare behavior | 1–5 | security + behaviour, before anything visual is judged |
| 4 Check themes | **1–10 (all)** | a theme change can touch every screen, so nothing is assumed |
| 5 Verify sessions | 9–10 | session ownership and the background job that pushes to the UI |

Steps 1, 2, 6 and 7 are proved by the build, the session start, the staging deploy and a timed query instead —
see `MigrationPath.svg` and the *Incremental path* tab.

## The screens most likely to hide timing or state assumptions

A student review question, answered against this sample:

1. **`WorkOrdersPage`** — the tab filter. On 3.5 it could have been a client-side filter over a fully loaded grid
   and nobody would have noticed. Flow 3 compares the page total with the store count so a client-side filter
   fails the flow rather than passing it quietly.
2. **`ImportCenterPage`** (the background import job) — a `System.Timers.Timer` in `Global.asax` writes to a
   screen from a thread that no longer owns the session. On Wisej.NET 4 that becomes
   `Application.StartTask` + `Application.Update`. Flow 10 counts the ticks that actually arrive.
3. **Any screen after navigation** — user identity that lived in a `static` "worked" for one user on a developer
   machine. Flow 9 fails a build that has even one static field on `SessionContext`.

## What a run looks like

```
Job:      regression harness → 10 flow(s) for contoso (corr 4f2b91ae)
Service:  flow  1 Login + role check → PASS — ana.ops (Manager) allowed, ben.tech (Technician) denied
Service:  flow  6 Theme: accent color on header + tabs → FAIL — accent color lost (#6B7C8F ≠ baseline #1565D8)
Service:  flow  7 Theme: corner radius on tabs + buttons → FAIL — corner radius 0 (baseline 7)
Service:  flow  8 Theme: priority colors in the grid → FAIL — priority colors dropped (3/3 missing)
Job:      regression harness: FAIL — 7/10 flows, 3 theme flows broken (…)
```

and after the mixin is mapped:

```
Job:      regression harness: PASS — 10/10 flows
```

## Evidence — what the running app shows

- **Run migration path** → the *Regression plan* tab fills in live, one row at a time, `Result on 4` coloured green
  or red; the dark footer carries the verdict the video prints:
  `Regression harness: FAIL — 7/10 flows, 3 theme flows broken` · then
  `Regression harness: PASS — 10/10 flows · Migration path complete — 7/7 steps passed, 7/7 dossier rows proven`.
- Flows 2–5 can also be run by hand on `WorkOrdersPage`: the three tab buttons, **+ New work order**,
  **Fail: new work order, blank title** and **Fail: approve with stale version**.
- Flows 6–8 are visible rather than described: open `WorkOrdersPage` before mapping the mixin and the app bar is
  grey, the tabs are square and the priority column is monochrome.
