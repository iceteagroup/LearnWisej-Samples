# Deliverable 5 — Rollback and smoke test checklist

## Part A — Smoke test checklist

Run after every deployment, in every environment, against the **real** deployed instance.
Seven checks, minutes not hours. Implemented in `Services/SmokeTestService.cs`; press **Run smoke tests**.

| # | Check | Blocking | Passes when | Fails because |
|---|---|---|---|---|
| 1 | The application page renders | **yes** | `Default.html` is served and bootstraps `wisej.wx` | wrong content root, missing static files, proxy rewriting the path |
| 2 | A session starts | **yes** | the node hands out a session id | session store misconfigured, node out of memory |
| 3 | The health check returns healthy | **yes** | `GET /healthz` → 200 with every critical check OK | any critical dependency down |
| 4 | A known query returns rows | **yes** | the probe tenant's open work orders come back | migration incomplete, wrong connection string, empty database |
| 5 | The WebSocket transport is available | no | server push is enabled and the proxy forwards `Upgrade` | proxy strips `Upgrade`/`Connection`, idle timeout too short |
| 6 | The configuration contract is satisfied | **yes** | `StartupValidation` passed for this environment | a required setting or secret missing |
| 7 | The rollback artifact exists | **yes** | the previous package is named and retained | nothing to roll back to — do not promote |

A **blocking** failure stops the release and arms rollback. A non-blocking failure is recorded and triaged; it
degrades the node without evicting it.

### Manual additions for a real production release

- Sign in with a real (test) corporate account through the production identity provider.
- Open one screen per integration the release touched.
- Confirm the log sink is receiving lines from the new build number.
- Confirm the release version shown in the UI matches the artifact that was deployed.

## Part B — Rollback checklist

**Trigger:** any blocking smoke test fails, `/healthz` reports a critical failure, or error rates climb during
monitoring (runbook steps 4, 5 or 7).

**Who may run it:** a Manager or an Admin (`Security/ReleaseAuthorization.cs`). Enforced server-side.

| # | Step | Confirmation |
|---|---|---|
| 1 | Name the target: the previous artifact **2.4.1** (`enterpriseops-2.4.1.zip` / image tag `enterpriseops:2.4.1`) | the artifact is still retained by the pipeline |
| 2 | Take the failed node out of rotation | the balancer already did it — confirm it in the node grid |
| 3 | Let pinned sessions drain, or accept the loss explicitly and say so in the incident | sessions on the *other* node are untouched — that is what affinity bought |
| 4 | Redeploy the previous package on the failed node | build column shows `2.4.1` |
| 5 | Re-run the **whole** smoke test checklist on the rolled-back node | all blocking checks green |
| 6 | Return the node to rotation | `receiving traffic · N sessions pinned` |
| 7 | Verify the other nodes are still healthy and still serving | no second incident caused by the fix |
| 8 | Log the incident, hold the release, and record what the pipeline should have caught | the fix goes back through steps 1–8 |

### The database question, asked before the release, not during it

Rollback of *code* is easy; rollback of a *schema* is not. 2.4.2 carries a migration, so the rule is: migrations
must be **backward compatible for one release** — the previous build must run against the new schema. If a
migration cannot be made backward compatible, it is a separate, gated release with its own runbook.

### What happens to sessions already on the failed node

They keep answering on the old node until they end or the drain window closes; the balancer only stops sending
**new** sessions. Say this in the incident report, because it is the first question operations asks.

## Evidence in the running sample

| Action | What you see |
|---|---|
| **Run smoke tests** | Seven `Health: smoke n. …: PASS` lines in the trace and a green banner with the elapsed time. |
| **Fail: node B health check** → the deploy runs | Check 3 and check 4 fail on `app-node-B`; the banner names the failing step and says step 8 is armed. |
| **Rollback…** | Node B goes to `2.4.1`, the checklist re-runs green, the chip turns `↩`, banner: *"Users never saw a broken node."* |
| **Sign in as ben.tech** then **Rollback…** | Refused before anything moves: `Security: ben.tech (Technician) may NOT roll back — rule: Manager or Admin`. |
