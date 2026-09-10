# Sample container notes

**Nothing in this lab builds or runs a container.** `Dockerfile` and `docker-compose.yml` in this folder are
deliverables to read and review. These notes say what a reviewer should look for and why each choice is there.

## What a container changes, and what it does not

A Docker image changes the **unit of deployment**, not the hosting problem. The image bundles the application and
its runtime so the same artifact runs on a laptop, in test and in production. It still needs a host, and that host
still needs sticky sessions, WebSocket passthrough, a reachable health probe and a secret store.

## Review checklist for the Dockerfile

| Line | Why |
|---|---|
| `dotnet/sdk:10.0` → `dotnet/aspnet:10.0` | multi-stage: the SDK never ships to production |
| `COPY *.csproj` + `restore` before `COPY . .` | a code change does not invalidate the restore layer |
| `-f net10.0` | only the cross-platform target framework goes into a Linux image; `net10.0-windows` is for IIS/Windows hosts |
| `ASPNETCORE_URLS=http://+:8080` | TLS terminates at the proxy; the container speaks plain HTTP on the internal network |
| `WEBSITE_PATH=/app` | Wisej.NET serves the project folder — in a published image that folder is `/app` |
| `EnterpriseOps__Release__NodeName` set to a placeholder | every node must know its own name, or the health report cannot be attributed |
| `HEALTHCHECK … /healthz/live` | the **liveness** probe. The orchestrator restarts dead processes |
| readiness is *not* in `HEALTHCHECK` | "not ready" must take the node out of rotation, not restart it — that decision belongs to the balancer |
| `USER $APP_UID` | not root |
| no secret anywhere | `StartupValidation` refuses to boot without the injected environment variables, so a misconfigured image fails loudly at start instead of quietly at runtime |

## What must NOT go in the image

| Never in the image | Where it belongs |
|---|---|
| connection strings, client secrets, API keys | environment variables injected at deploy time from a vault |
| the `_storage` upload folder as image content | a mounted volume or object storage — a rebuilt node must not lose files |
| environment-specific `appsettings` overrides baked in | the same image for every environment; only configuration differs |
| a license key | supplied per environment |

## Wisej.NET-specific points

- **Sessions are in-process.** A container restart ends every session it held. Rolling deployments must drain
  (`stop_grace_period`), and the balancer must pin sessions or scaling out is pointless.
- **The WebSocket must reach the container.** Proxy, ingress and any WAF have to forward `Upgrade`/`Connection`
  and keep idle connections open longer than the session timeout.
- **Static content is the project folder.** Anything the app fetches by path (`Widgets/*.js`, images) has to be in
  the published output, so check `CopyToPublishDirectory` on those items.
- **The `.json` files are private.** `Startup.cs` keeps `Default.json`, `appsettings*.json` and `HealthCheck.json`
  out of the file server; `nginx.conf` denies them a second time.

## Sizing and scaling

Session state lives in RAM, so a node's capacity is measured in **sessions**, not requests per second.
`HealthCheck.json` names the limit (`limits.maxSessionsPerNode`) and the `sessionStore` check reports against it,
so a node that is nearly full is visible before it is full. Scale out by adding nodes; scale in by draining one.
