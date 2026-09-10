# Deployment & release patterns — EnterpriseOps

The document the walkthrough opens in the editor: the environment configuration table and the release runbook,
side by side, because they are the two halves of the same contract. The full deliverables live under
`../EnterpriseOps/docs/`; this page is the index an on-call engineer starts from.

## The package

| Artifact | What it is |
|---|---|
| `../EnterpriseOps/docs/DeploymentArchitecture.md` (+ `.svg`) | proxy · nodes · sessions · shared services · pipeline |
| `../EnterpriseOps/docs/EnvironmentConfiguration.md` | every setting, per environment, owned, secrets flagged |
| `../EnterpriseOps/HealthCheck.json` (+ `docs/HealthCheck.md`) | the probe contract monitors and balancers read |
| `../EnterpriseOps/docs/ReleaseRunbook.md` | eight steps, rollback included |
| `../EnterpriseOps/docs/RollbackAndSmokeTestChecklist.md` | proven before production |
| `Dockerfile`, `docker-compose.yml`, `Dockerfile.notes.md` | sample container notes — **read, never built in this lab** |
| `nginx.conf`, `nginx.proxy.notes.md` | reverse-proxy configuration and its review notes |
| `LoadBalancerNotes.md` | sticky sessions, draining, scaling, health-aware routing |

## Environment configuration (summary)

| Setting | Dev | Test (Staging) | Production | Secret? | Owner |
|---|---|---|---|---|---|
| Connection string | local db | test db | `●●●●●●●●` | **YES** | platform |
| Logging level | `Debug` | `Information` | `Information` | no | ops |
| Upload limit | 100 MB | 50 MB | 25 MB | no | product |
| Feature flags | all on | release set | release set | no | product |
| Identity provider | dev stub | test IdP | corp SSO | **PARTLY** | security |

> **Configuration is a contract.** Every setting has an owner and a per-environment value — and secrets come from
> the environment, never from files in the package.

## Release runbook (summary)

1. Confirm build number and release notes.
2. Verify configuration and secrets.
3. Deploy to staging.
4. Run smoke tests.
5. Check `HealthCheck.json` and diagnostics.
6. Deploy production.
7. Monitor logs and health.
8. **Execute rollback if a smoke test or health check fails.**

## The three review questions this package answers

| Question | Answer | Where |
|---|---|---|
| What requires sticky sessions? | Rich **in-process** session state: a Wisej.NET session lives in one node's memory, and its WebSocket stays open to that node. | `LoadBalancerNotes.md` |
| Where are secrets stored? | In the platform's secret store, injected as `EnterpriseOps__*` environment variables at deploy time. Never in `appsettings*.json`, never in the image. | `../EnterpriseOps/docs/EnvironmentConfiguration.md` |
| How do operations know a node is unhealthy? | `GET /healthz` answers 503 after a critical check fails; the balancer evicts it after `unhealthyThreshold` probes; the dashboard, the trace and the monitoring alert all say which check failed. | `../EnterpriseOps/docs/HealthCheck.md` |

## Choosing a target

A Wisej.NET application runs anywhere ASP.NET Core runs, so the choice is organisational, not technical:

- **IIS** — for teams with Windows operations. Watch application-pool recycling and idle timeout: both end
  in-memory sessions. Install the WebSocket Protocol feature.
- **Kestrel behind NGINX/Apache** — for Linux hosts. The proxy owns TLS and the WebSocket upgrade.
- **Azure App Service / AWS** — the platform owns scaling, TLS and log sinks, and supplies affinity
  (ARR affinity / ALB stickiness) and the health probe path.
- **Containers** — change the unit of deployment, not the hosting requirements. The host still needs affinity,
  WebSocket passthrough, a reachable probe and a secret store.

Whatever the target, verify three things before committing: WebSockets pass through every layer, the health check
endpoint is reachable by the infrastructure, and the application sees the real client address and scheme.
