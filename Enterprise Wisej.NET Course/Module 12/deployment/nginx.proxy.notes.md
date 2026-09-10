# Reverse-proxy notes

Companion to `nginx.conf`. **Nothing here is started by the lab** — it is the configuration review an operations
team should be able to do without reading the application source.

## The four things a proxy must get right for Wisej.NET

### 1. Forward the WebSocket upgrade

```nginx
proxy_http_version 1.1;
proxy_set_header Upgrade    $http_upgrade;
proxy_set_header Connection $connection_upgrade;
```

Server push, background job progress and live diagnostics all ride the WebSocket. If the proxy strips `Upgrade`,
Wisej.NET falls back to polling: the app still works, everything "live" becomes laggy, and nothing logs an error.
That is the hardest deployment bug in this module to diagnose, so test it explicitly.

### 2. Do not time the connection out

```nginx
proxy_read_timeout 3600s;
proxy_send_timeout 3600s;
proxy_buffering    off;
```

A push channel is idle whenever the user is reading. Idle timeouts must exceed the Wisej.NET session timeout on
**every** hop: proxy, load balancer, WAF, cloud ingress, and any corporate middlebox. Buffering must be off or
pushed updates arrive in bursts.

### 3. Tell the application who is calling and over what

```nginx
proxy_set_header Host              $host;
proxy_set_header X-Real-IP         $remote_addr;
proxy_set_header X-Forwarded-For   $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $scheme;
```

`Startup.cs` enables `UseForwardedHeaders` for `XForwardedFor | XForwardedProto`. Without it:

- every audit and log line records the proxy's address instead of the user's;
- the application believes it is on `http://`, so redirects and secure-cookie decisions are wrong.

**Security note:** forwarded headers are client-controllable. In the lab `KnownIPNetworks` and `KnownProxies` are
cleared so any hop is trusted (a compose network). In production, name the proxy or the ingress subnet, or a
client can spoof its own address into your audit log.

### 4. Pin the session

See `LoadBalancerNotes.md`. `ip_hash` is the open-source approximation; a route cookie is the real answer.

## TLS

Terminate at the proxy, speak plain HTTP on the internal network, and let the application learn the real scheme
from `X-Forwarded-Proto`. If the internal hop must also be TLS, the certificate has to be trusted by the proxy —
the application does not change.

## What the proxy must NOT serve

`Default.json`, `appsettings*.json`, `HealthCheck.json` and `Web.config` are configuration, not content. The
application already refuses to serve them; `nginx.conf` denies them again, because two locks on a door that leaks
secrets is not paranoia.

## Restricting the probe

`/healthz` describes internal state. It is safe by design (`safeToExpose` in `HealthCheck.json` keeps exception
messages and connection strings out of it), but it should still only be reachable from the balancer and the
monitoring network — hence the `allow`/`deny` block.

## Apache and IIS equivalents

| Concern | NGINX | Apache | IIS |
|---|---|---|---|
| WebSocket | `proxy_set_header Upgrade` | `mod_proxy_wstunnel` + `RewriteRule .* ws://…` | enable the *WebSocket Protocol* feature; ARR passes it through |
| Forwarded headers | `X-Forwarded-*` | `ProxyPreserveHost On`, `RemoteIPHeader` | ARR sets `X-Forwarded-For`; enable it in the server farm |
| Affinity | `ip_hash` / sticky cookie | `stickysession=ROUTEID` on `ProxyPass` | ARR *Client Affinity* (`ARRAffinity` cookie) |
| Timeouts | `proxy_read_timeout` | `ProxyTimeout` | ARR *Time-out (seconds)*; also the app pool idle timeout |
| Recycling | n/a | n/a | **set app-pool idle timeout to 0 and disable periodic recycling** — a recycle kills every in-memory session |

The IIS row matters: the default application-pool recycle is a scheduled outage for a session-aware application.
