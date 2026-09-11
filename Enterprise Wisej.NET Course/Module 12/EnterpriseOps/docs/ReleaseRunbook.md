# Deliverable 4 — Release runbook

**Release:** EnterpriseOps 2.4.2 · **Previous (rollback target):** 2.4.1
**Who may run it:** a Manager or an Admin (`Security/ReleaseAuthorization.cs`). `ben.tech` may watch, not release.
**Where it is executed in the sample:** `Services/ReleaseService.DeployAsync` — the same eight steps, in the same
order, one chip and one `Runbook:` log line each.

> A release procedure that cannot be reversed is not finished. That is why step 8 exists and why it is rehearsed.

## The eight steps

### 1 — Confirm build number and release notes
The pipeline produced **one** versioned artifact: `enterpriseops-2.4.2.zip` (image tag `enterpriseops:2.4.2`).
Confirm the version, the notes, and that the same artifact is what staging already ran. Never rebuild per environment.
*Fails if:* the artifact is unversioned or was rebuilt — stop, there is nothing to promote or to roll back to.

### 2 — Verify configuration and secrets
Run the configuration contract for the target environment (`StartupValidation`). Every required key present, every
secret coming from the environment, `AllowedHosts` naming the real host, `Logging:Level` not `Debug`.
*Fails if:* any key is missing → the release stops **before** a node is touched, because the node would refuse to boot.

### 3 — Deploy to staging
Take the target node out of rotation, let its pinned sessions drain, install the new package.
*Fails if:* the node will not start — its sessions have already drained, so nothing is lost; investigate and stop.

### 4 — Run smoke tests
The seven checks of `RollbackAndSmokeTestChecklist.md`, against the real staged environment. Minutes, not hours.
*Fails if:* any blocking check fails → **arm step 8**.

### 5 — Check HealthCheck.json and diagnostics
Ask the node the same question the balancer will ask: `GET /healthz`. Read the per-check detail, not just the status.
*Fails if:* a critical check fails → the balancer would evict this node in production → **arm step 8**.

### 6 — Deploy production
Put the node back in rotation on the new package. Roll node by node; never both at once, or there is no healthy
node left holding sessions.

### 7 — Monitor logs and health
Watch the 503 rate of `/healthz`, the error log and the session count for at least one probe cycle
(`intervalSeconds` × `unhealthyThreshold` = 30 s here, longer in production).
*Fails if:* health flaps or errors climb → **arm step 8**.

### 8 — Execute rollback if a smoke test or a health check fails
Redeploy the named previous artifact (2.4.1) on the failed node, re-run the smoke tests, return it to rotation,
log the incident, hold the release for a fix. See `RollbackAndSmokeTestChecklist.md` for the checklist form.

## Roles and hand-offs

| Step | Who | Automated? |
|---|---|---|
| 1–2 | pipeline | yes — a failure blocks the run |
| 3–5 | pipeline, watched by the release manager | yes, gated |
| 6 | release manager (Manager/Admin) presses **Deploy** | manual gate |
| 7 | on-call engineer | monitored |
| 8 | release manager or on-call engineer presses **Rollback** | manual, rehearsed |

## Evidence in the running sample

| Action | What the runbook strip does |
|---|---|
| First **Deploy…** | chips 1–3 green, **chip 4 red** — the smoke test's health check and known-query check both fail (`app-node-B { "status": "Unhealthy" … database: FAIL }`) — chip 8 amber and armed; the node grid shows `ROUTED AWAY — no new sessions`. The release never reaches step 5: catching it at step 4 is exactly what smoke tests are for. Step 5 is the net for a node that degrades *after* the smoke tests pass. |
| **Rollback…** | chip 8 turns `↩`, node B returns on 2.4.1, smoke tests re-run green, the banner reads *"Users never saw a broken node."* |
| **Deploy…** again (the fix shipped) | chips 1–7 turn green one after the other; chip 8 stays grey with `rollback not needed — 2.4.1 stays retained and redeployable`. |
| **Deploy…** as a Technician | refused by `ReleaseAuthorization` before anything runs. |
