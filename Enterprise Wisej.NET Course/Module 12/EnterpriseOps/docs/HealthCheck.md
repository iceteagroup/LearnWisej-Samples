# Deliverable 3 — HealthCheck.json

The file itself is **`../HealthCheck.json`** (the project root, next to `Default.json` and `appsettings*.json`),
because the running process reads it at startup: `Startup.cs` → `HealthProbeService.LoadHealthCheckFile(contentRoot)`.
This page is the annotated version an operations reviewer reads.

## The contract

```jsonc
{
  "endpoints": { "readiness": "/healthz", "liveness": "/healthz/live" },
  "probe":  { "intervalSeconds": 10, "timeoutSeconds": 3,
              "unhealthyThreshold": 3, "healthyThreshold": 2,
              "expectedStatusCode": 200, "unhealthyStatusCode": 503 },
  "limits": { "maxSessionsPerNode": 500 },
  "checks": [ … ],
  "safeToExpose": { "status": true, "release": true, "node": true, "checks": true,
                    "exceptionMessages": false, "connectionStrings": false }
}
```

| Check | Critical | What it actually does in the sample |
|---|---|---|
| `configuration` | **yes** | `StartupValidation` succeeded for the active environment |
| `database` | **yes** | the known query returns rows (`WorkOrderRepository.CountOpen`) |
| `storage` | **yes** | writes and deletes a probe file under `EnterpriseOps:Storage:Root` |
| `websocket` | no | `Default.json` does not disable the WebSocket transport |
| `sessionStore` | no | sessions started on this node is below `limits.maxSessionsPerNode` |
| `theme` | no | the theme named in `Default.json` resolves |

**Readiness rule:** every *critical* check must be OK, otherwise `/healthz` answers **503** and the balancer stops
sending new sessions. A non-critical failure degrades the node — it is reported, it does not evict.

**Liveness never inspects dependencies.** `/healthz/live` answers 200 while the process can run code, so an
orchestrator restarts only genuinely dead nodes instead of restarting a healthy node whose database is slow.

## Two endpoints, two audiences

| Endpoint | Who calls it | How often | What a failure means |
|---|---|---|---|
| `/healthz` | load balancer, ingress, App Service health probe | every 10 s, 3 failures in a row | take the node **out of rotation** |
| `/healthz/live` | orchestrator (Kubernetes, ECS, compose healthcheck) | every 10 s | **restart the container** |

## What may leave the process

`safeToExpose` is honoured in code, not just in prose: `HealthCheckResult.Detail` never carries an exception
message (only the exception *type*) and never a connection string. Anyone who can reach the probe endpoint can
read the body, so it says *what* failed, never *how to exploit it*.

## How operations learn a node is unhealthy

1. The balancer's own probe flips the node out of rotation after `unhealthyThreshold` failures — automatic, no human.
2. The release dashboard shows the node red with the failing check and the one-line probe body.
3. The activity trace records the balancer's decision: `Balancer: app-node-B failed readiness (…) — routed away`.
4. Monitoring alerts on the 503 rate of `/healthz`, which is why the endpoint answers a *status code*, not just a body.

## Evidence in the running sample

- The page's dark `GET /healthz → …` line is the real probe body for the node that needs attention.
- **Fail: node B health check** arms the 2.4.2 migration fault, deploys, and the same probe answers
  `{ "status": "Unhealthy", … database: FAIL · storage: OK · websocket: OK }`.
- Browse to `http://localhost:5212/healthz` — 200 with the body, or 503 once a critical check fails.
- Browse to `http://localhost:5212/HealthCheck.json` — **not served**: `Startup.cs` keeps every `.json` out of the
  file server, so the probe contract, `Default.json` and `appsettings*.json` stay private.
