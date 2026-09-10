# TicketOpsLive · Real-Time Apps with Server Push · Module 7 (capstone)

Local lab build for **Module 7 · Production Review: From Demo Push to Deployable Real-Time App**. This is the whole
of TicketOps Live in one session: the status strip, the background import monitor, the live ticket board fed by the
global `TicketHub`, the cadence controls, the session diagnostics — plus what this module adds, a **real-time health
panel**, the **effective configuration** and the **production checklist**.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 7/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5307
```

Then open <http://localhost:5307>, and click **Open another session ↗** for a second tab. (Visual Studio: open
`TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## The capstone, feature by feature

| Required feature (lab) | From | Where in this app |
|---|---|---|
| Live status strip | M01 | `statusPanel` + `HeartbeatLoop` in [`MainPage.cs`](TicketOpsLive/MainPage.cs); ▶ Start heartbeat on the bottom bar |
| Session diagnostics | M02 | **Session** tab: `diagnosticsGroupBox`, `lifecycleListBox`, `liveSessionsLabel`, [`Services/SessionRegistry.cs`](TicketOpsLive/Services/SessionRegistry.cs) |
| Background import monitor | M03 | **Import monitor** tab: `RunImport`, [`Services/ImportJob.cs`](TicketOpsLive/Services/ImportJob.cs) |
| Cadence and polling controls | M04 | **Cadence** tab: `refreshTimer_Tick`, [`Services/DashboardSimulator.cs`](TicketOpsLive/Services/DashboardSimulator.cs), `BeginPush`/`EndPush` |
| Live ticket board | M05 | **Ticket board** tab: `BindingList<Ticket>` + `BindingSource` + `ApplyTicketEvent` + `ResetBindingsQuietly` |
| Multi-user TicketHub | M06 | [`Services/TicketHub.cs`](TicketOpsLive/Services/TicketHub.cs) + `Hub_TicketChanged` |
| Production checklist and configuration review | M07 | **Health · config** tab: `RealtimeHealthSnapshot`, `RenderHealth`, `LoadConfiguration`, `BuildChecklist` |

## What to try

| Action | Path | What you should see |
|---|---|---|
| ▶ Start heartbeat | progress | one `→ push Application.Update(this, …) heartbeat #n` per second; the clock and load bar move with no click |
| ▶ Start import (Import monitor) | progress | `job J-xxxxxx · 10/200 · 5%` every 10 records, **interleaved** with the heartbeat pushes in the same trace |
| ■ Cancel | cancellation | `cancelled after record n (OperationCanceledException caught inside the task)`; Start re-enabled by `finally` |
| ☑ Throw at record 87 → ▶ Start import | failure | `FAILED on record 87 … → server log, safe message`; red banner naming the job; the server console has the stack trace |
| Publish (my tenant) / Escalate selected | multi-user | `hub.AddOrUpdate … fanned out on a thread-pool thread`, then `→ push Application.Update(_context) event <id> …` in **every** session of that tenant; escalation also pops a toast |
| Publish (other tenant) | filtering | here: `• server filtered out (tenant … ≠ this session's …)` and the counter climbs; in a session on that tenant the row appears |
| Simulate 10 ↻ | progress | a task publishes 10 tickets through the hub, one per second; the button comes back in `finally` |
| Unsubscribe / Subscribe | lifecycle | `hub subscribers` falls and this session receives nothing; Subscribe reloads the snapshot |
| Live mode (Cadence) | cadence | `refreshTimer_Tick applied model v266 · 266 event(s) coalesced into 14 update(s)`; change the cadence and watch the ratio move |
| Session counter +1 / Background update / Background fault | sessions | per-session counter vs the static trap; a push 1.5 s later with no request; a caught fault with a safe message |
| Health · config tab | this module | *Push expected* / *Polling off*, `active subscriptions`, measured `updates/min`, the running loops, the effective config, the 10-point checklist |
| End this session | lifecycle | `Application.Exit()` → one cleanup: loops stopped, hub and registry unsubscribed (server console proves it), and the other tab sees the session leave |

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart: the strip stays `○ HTTP only`,
the Health tab reads *Fallback mode* / *Polling enabled* while work runs, and the pushes arrive on the polls.

## Where things live

```
TicketOpsLive/
├─ MainPage.cs                one page, one region per module: heartbeat · import · board+hub · cadence · session · health
├─ MainPage.Designer.cs       status strip, the five-tab feature panel, the trace card, the action bar
├─ Models/
│  ├─ Ticket.cs · TicketStatus.cs · TicketChangedEventArgs.cs      the board's shapes (with TenantId)
│  ├─ SessionInfo.cs · SessionsChangedEventArgs.cs                 what the registry keeps: values only
│  └─ RealtimeHealthSnapshot.cs                                    this module's health contract
├─ Services/
│  ├─ TicketHub.cs            global singleton, thread-safe, snapshots, Task.Run fan-out, no controls ever
│  ├─ SessionRegistry.cs      global singleton keyed on SessionId (ClientId is the browser)
│  ├─ ImportJob.cs            JobId, outcome, elapsed, summary — the correlation key
│  └─ DashboardModel.cs · DashboardSimulator.cs                    50 ms model + the session-owned task
├─ Default.json               sessionTimeout 180 · pollingInterval 1000 · debug true (false in production)
├─ HealthCheck.json           maxMemory · maxSessions · cpuThreshold (read and displayed by the Health tab)
└─ docs/
   ├─ ArchitectureNote.md     the one-page architecture deliverable (state, work, delivery, cleanup, deployment)
   ├─ ProductionChecklist.md  the 10 questions + security + observability, each with where and evidence
   ├─ ConfigurationReview.md  Default.json / HealthCheck.json keys, debug:false, sticky sessions, proxy notes
   └─ DemoScript.md           the lab's 9-step demo with the expected trace lines, two tabs
```

## Self-check (acceptance criteria → where it is satisfied)

- **The application runs without duplicate loops or duplicate subscriptions** — every start is guarded
  (`_heartbeatRunning`, `_importRunning`, `_feedRunning`, `DashboardSimulator.Start()` returns false when running);
  `SubscribeToHub` is a no-op when already subscribed. Verified: a second Start is refused and logged.
- **The UI remains responsive during background work** — no handler does long work; the import, heartbeat, feed and
  simulator all run on tasks. Verified: heartbeat and import pushed concurrently while the tabs stayed clickable.
- **Every long-running operation has completion, cancellation and failure states** — the import has all three
  (`Complete`/`Cancel`/`Fail` on `ImportJob`); the heartbeat and the feed have completion and failure; each restores
  the UI in `finally`.
- **The multi-user hub does not keep direct references to pages or controls** — `TicketHub` has no `Wisej.Web`
  using at all; it stores `Ticket` values, hands out clones, and raises on a thread-pool thread.
- **Bound grids update incrementally** — `ApplyTicketEvent` inserts or copies in place and calls
  `ResetBindings(false)` once, then restores the selection; the grid is never rebound.
- **The student can defend all deployment assumptions** — [`docs/ConfigurationReview.md`](TicketOpsLive/docs/ConfigurationReview.md)
  and [`docs/ProductionChecklist.md`](TicketOpsLive/docs/ProductionChecklist.md), summarised on the Health tab.
- **Lesson checkpoint** — WebSocket path verified end to end, session routing stable (sticky sessions), fallback
  defined (`StartPolling` only while work runs), subscriptions cleaned up (one idempotent `Cleanup`), updates
  throttled (push every 10 records, one refresh per tick), data protected (safe messages, encoded content), and
  operations observable (`JobId`, `EventId`, `SessionId`, measured `updates/min`).

## Verified in the browser

- Heartbeat + import running together: interleaved `→ push` lines, 200-record job failing at 87 with the detail in
  the server console and a safe message on screen, Start re-enabled by `finally`.
- Ticket board: publish added #4831, escalate flipped #4801 to Escalated with a toast, a Northwind publish counted
  as `filtered out (wrong tenant): 1` while the Contoso board stayed correct.
- Cadence: 266 model events coalesced into 14 UI updates at 1 s, `refused=0`.
- Health tab: `Push expected (WebSocket)`, `Polling off`, `active subscriptions: 2 (hub 1 + sessions 1)`,
  `63 updates/min · last 0.9 s ago · loops: heartbeat, dashboard simulator, refreshTimer @ 1 s`, the six resolved
  `Default.json` values and the three `HealthCheck.json` thresholds, and all ten checklist items.

## APIs used here that the cookbook does not mark verified

- `Wisej.Web.TabControl` / `TabPage`: `TabPages.Add`, `SelectedIndex`, `SelectedTab`, `SelectedIndexChanged`.
  **Two findings from this build:** a `TabPage.Text` containing `&` renders the ampersand as a mnemonic (it became
  "Health <u> </u>config", so the tab is named `Health · config`), and a tab page's controls are **created lazily**
  — they do not exist client-side until that tab is shown for the first time.
- `Wisej.Web.CheckedListBox`: `CheckOnClick`, `Items.Add`, `CheckedItems.Count`, `AfterItemCheck` with
  `ItemCheckEventArgs.Index` / `NewValue`. Verified only that it renders and fills; the check event was not clicked
  in the browser.
- `Application.Configuration`: `SessionTimeout`, `PollingInterval`, `EnableWebSocket`, `Debug`, `MaxSessions`,
  `ThemeName` — all read successfully (180 / 1000 / True / True / -1 / Bootstrap-4). Each read is still wrapped in
  a try/catch so a renamed member cannot break the page.
- `Application.StartupPath` resolved to the project folder under `dotnet run`, so `HealthCheck.json` was found and
  displayed; under a published deployment it may resolve elsewhere — the panel says so instead of failing.
- The `HealthCheck.json` **schema** (`maxMemory`, `maxSessions`, `cpuThreshold`) is shown as documentation; this
  sample only reads and displays the file. Confirm it against the Wisej.NET deployment docs before relying on it.
