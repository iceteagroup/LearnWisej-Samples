# What the FieldOps PWA deliberately does not do offline

FieldOps is a Wisej.NET application: every control is a C# object in a session on the server, and
every click — opening a work order, Start job, Update status, attaching a photo — runs a handler
there. The service worker (`sw.js`) can return files. It cannot run C#.

## What the installed PWA does with no network

- Launches from its icon in its own window (`display: standalone`).
- Serves the shell from `fieldops-shell-vN`: `Default.html`, `offline.html`, the manifest and icons.
- Answers the failed page load with `offline.html`: a "No connection to the FieldOps server"
  banner, the **last synced** summary (technician, open orders, next site, due before 14:00, the
  first three rows) with its time, and **Retry connection**.

## What it deliberately does not do

| Not done | Why |
|---|---|
| Cache `app.wx`, `wisej.wx`, `resource.wx` responses or the WebSocket | A replayed response describes a session the server has already forgotten: the screen would render controls whose handlers no longer exist, and the first click would fail or act on stale state. |
| Show the work-order grid or the detail form offline | A form with no server behind it invites typing that is then lost. |
| Queue status changes or photos | Nothing on the offline page looks saved. A queued change would need conflict rules, retries and an audit trail — that is a sync design, not a cache. |
| Reload into the old session when the signal returns | Retry probes the server and then loads `/` into a **fresh** session. |

## When the requirement outgrows the PWA

If technicians must **open orders and change their status in the plant room**, a better offline page
will never satisfy it. That requirement calls for a **Wisej.NET Hybrid Local Application**: the same
app on an embedded EmbedIO server inside the native client, registered in the client's `Startup.cs`
with `UseWisejOffline<OfflineStartup>()`, combined with the remote `StartupUrl` and
`config.OfflineTimeout = 5000` so the client switches to the local app five seconds after the
connection is lost. The price: the local app's updates ship through the app stores, and sync is ours
to design. FieldOps builds that client in Module 3.

Until then: the installed PWA is still the browser. `Device.Valid` is false there, so the camera and
location use the same browser fallbacks as a plain tab.
