# Load-balancing notes for a session-aware, real-time application

For the operations team. Written as the answer to one question: *what does EnterpriseOps need from the balancer?*

## The rule

**Every Wisej.NET session lives in the memory of exactly one server process.**

A balancer that spreads requests without affinity sends a user's second request to a node that has never heard of
them. The application does the only thing it can: it starts a new session. The user loses their grid, their wizard
step and every unsaved edit, and nothing in the logs looks like an error.

Sticky sessions are therefore a **requirement, not an optimisation**.

## How to pin

| Mechanism | How | Verdict |
|---|---|---|
| Route cookie | balancer sets a cookie naming the node; every later request follows it | **preferred** — survives NAT, works for mobile clients that change address |
| `ip_hash` (NGINX OSS) | node chosen by a hash of the client address | acceptable for a lab; breaks behind corporate NAT where hundreds of users share one address, and breaks when a phone switches network |
| Azure App Service | *ARR affinity* on (`ARRAffinity` cookie) | the platform's built-in answer |
| AWS ALB | target-group stickiness (`lb_cookie` or `app_cookie`) | the platform's built-in answer |
| Kubernetes ingress | `sessionAffinity: ClientIP` on the Service, or the ingress controller's cookie affinity | check the controller actually supports cookie affinity |
| No affinity + distributed session state | not available: Wisej.NET session state is in-process | not an option — design accordingly |

## The second constraint: long-lived WebSockets

Server push keeps a WebSocket open between the browser and **the node holding the session**. So the balancer and
every proxy must:

- forward the `Upgrade` / `Connection` headers,
- keep idle connections open longer than the session timeout,
- not "rebalance" an established connection,
- not buffer the channel.

## Scaling

| Operation | What it means here |
|---|---|
| **Scale out** | add a node; it starts empty and takes new sessions only. Existing users stay where they are — throughput improves for the *next* users, not the current ones. |
| **Scale in** | **drain**: stop sending new sessions to the node, let the pinned ones end naturally, then stop it. Cutting a node signs its users out mid-workflow. |
| **Autoscaling on CPU** | misleading: capacity here is *sessions × memory*. Scale on session count and memory, and use `limits.maxSessionsPerNode` from `HealthCheck.json` as the ceiling. |
| **Rolling release** | one node at a time, never both — see `../EnterpriseOps/docs/ReleaseRunbook.md`. |

## Health checks feed the balancer

`GET /healthz` returns **200** when every critical check passes and **503** otherwise. After
`probe.unhealthyThreshold` consecutive failures the balancer stops sending **new** sessions to that node; after
`probe.healthyThreshold` successes it returns to rotation.

**The question operations always asks first:** *what happens to the sessions already on the failed node?*
They keep being served by that node until they end or the drain window closes — the node is out of the pool for
**new** sessions only. If the node is genuinely dead, those sessions are lost and the users must sign in again.
That is why liveness (`/healthz/live`, restart) and readiness (`/healthz`, evict) are two different endpoints.

## The failure path, as the module demonstrates it

1. Release 2.4.2 goes to `app-node-B`. Its database migration does not complete.
2. `/healthz` on node B answers 503 with `database: FAIL`.
3. The balancer routes new sessions away from node B. Node A's users notice nothing — their sessions are pinned.
4. Runbook step 8 is armed. An operator presses **Rollback**: node B returns on 2.4.1, smoke tests re-run green,
   and it rejoins the rotation.
5. The incident is logged and release 2.4.2 is held for a fix.

Press **Fail: node B health check**, then **Rollback…**, and read the trace — every line above appears as a
`Balancer:` or `Runbook:` entry.

## Checklist for a new environment

- [ ] Affinity configured and **verified** (open two browsers, confirm each keeps its node).
- [ ] Affinity verified again from behind the corporate NAT, if `ip_hash` is used.
- [ ] WebSocket upgrade verified end to end (the network tab shows a `101 Switching Protocols`).
- [ ] Idle timeouts on every hop longer than the session timeout.
- [ ] `/healthz` reachable from the balancer, not from the internet.
- [ ] Drain configured (`stop_grace_period`, deregistration delay, or the platform's equivalent).
- [ ] Node name unique per instance (`EnterpriseOps__Release__NodeName`), or health reports cannot be attributed.
- [ ] Uploads on shared storage, not node-local disk.
