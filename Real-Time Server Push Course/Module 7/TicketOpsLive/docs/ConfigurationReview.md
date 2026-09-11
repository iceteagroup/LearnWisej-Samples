# Configuration review — what this app runs with, and what production needs

The **Health · config** tab shows these values as the *server resolved them*, read through
`Application.Configuration` rather than parsed from the file, so what you see is what the session is actually using.

## `Default.json`

```json
{
  "url": "Default.html",
  "startup": "TicketOpsLive.Program.Main, TicketOpsLive",
  "theme": "Bootstrap-4",
  "debug": true,
  "sessionTimeout": 180,
  "pollingInterval": 1000
}
```

| Key | This lab | Production | Why it matters for real time |
|---|---|---|---|
| `sessionTimeout` | `180` (seconds) | measured from real idle behaviour | a timed-out session is *gone*: its loops stop and its subscriptions are removed. Too short kills long imports; too long parks memory. Wisej.NET shows a "prolong the session?" dialog before it fires (verified: it appears after roughly two minutes of no requests, and a WebSocket push alone does not count as activity). |
| `pollingInterval` | `1000` | choose per session count | the fallback rate. One request per second per fallback session is a real cost; this app only asks for polling while a task is running. |
| `debug` | `true` | **`false`** | production bundles and minifies the client. Ship `true` and you serve a slower, larger client and a more talkative console. |
| `enableWebSocket` | absent (on) | on | set to `false` to rehearse the fallback: `IsWebSocket` stays false, the Health tab reads *Fallback mode*, and the pushes arrive on the polls. |
| `maxSessions` | `-1` (unbounded) | a number you measured | an unbounded server fails at the worst moment instead of refusing the session that would have broken it. |
| `responseTimeout` | not set | consider for slow first responses | affects long initial HTTP responses; a WebSocket does not time out the same way. |

Read them in code the way the Health tab does — one guarded read each, so a member that a future build renames
cannot take the page down:

```csharp
try { value = Application.Configuration.SessionTimeout.ToString(CultureInfo.InvariantCulture); }
catch (Exception) { value = "n/a (not exposed by this build)"; }
```

## `HealthCheck.json`

```json
{
  "maxMemory": "2GB",
  "maxSessions": 500,
  "cpuThreshold": 85
}
```

It ships next to `Default.json` and the app reads it from `Application.StartupPath` for display. The point of the
file is that a load balancer can ask the instance whether it is still healthy and stop sending it new sessions
before it falls over — draining beats crashing, because in a server-side session model a crash takes every session
on that instance with it.

**Unverified here:** these three keys are the ones the walkthrough shows, and this sample only *reads and displays*
the file. Whether this exact schema is what the deployed Wisej.NET health-check endpoint consumes was not executed
in this lab — confirm against the Wisej.NET deployment documentation for your hosting model before relying on it.

## Deployment requirements

- **WebSocket through the whole path.** Browser → corporate proxy → load balancer → web server → app. NGINX and
  friends proxy WebSocket only when configured to forward the `Upgrade` and `Connection` headers. Test from a
  user's network, not from the server.
- **Sticky sessions (or equivalent routing).** Server-side session state lives in one process. Without affinity a
  request lands on an instance that has never heard of the session.
- **ASP.NET Core hosting** for .NET builds (`app.UseWisej()` in `Startup.cs`, as here); IIS module/handler settings
  instead for .NET Framework apps.
- **A place for the logs.** `Console.Error` is the sample's stand-in; production wants a logging framework with the
  session id and the job id kept as fields, not baked into a message string.

## What the reviewer should check in the running app

1. **Health · config** tab: `sessionTimeout 180`, `pollingInterval 1000`, `debug True` (and the note that it must be
   false in production), `enableWebSocket True`, `maxSessions -1`, then the three `HealthCheck.json` values.
2. Set `"enableWebSocket": false`, restart, and watch the same tab read *Fallback mode* and *Polling enabled* while
   the heartbeat still updates the strip.
3. Leave the tab idle for about two minutes and watch the session-timeout dialog appear — that is `sessionTimeout`
   doing its job, and the reason long work must not assume the UI is still there.
