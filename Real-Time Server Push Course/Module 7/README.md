# TicketOpsLive · Real-Time Apps with Server Push · Module 7 (capstone)

Local lab build for **Module 7 · Production Review: From Demo Push to Deployable Real-Time App**: the whole of
TicketOps Live in one page — the status strip, the background import monitor, the live ticket board fed by the
global `TicketHub`, the cadence controls, the session diagnostics — plus what this module adds: the real-time health
panel, the effective configuration and the production checklist.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 7/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5307
```

Then open <http://localhost:5307>, and **Session → Open second window** for a second tab. (Visual Studio: open
`TicketOpsLive.slnx`, press F5.) The lab's demo script, step by step: [`docs/DemoScript.md`](TicketOpsLive/docs/DemoScript.md).

## The capstone, feature by feature

| Required feature (lab) | From | Where in this app |
|---|---|---|
| Live status strip | M01 | `statusPanel` + `HeartbeatLoop` in [`MainPage.cs`](TicketOpsLive/MainPage.cs); ▶ Start heartbeat / ■ Stop in the strip |
| Session diagnostics | M02 | **Session** tab: `diagnosticsGroupBox`, `lifecycleListBox`, `liveSessionsLabel`, [`Services/SessionRegistry.cs`](TicketOpsLive/Services/SessionRegistry.cs) |
| Background import monitor | M03 | **Import monitor** tab: Start Import / Cancel Import / Fail at 87, `RunImport`, [`Services/ImportJob.cs`](TicketOpsLive/Services/ImportJob.cs) |
| Cadence and polling controls | M04 | **Cadence** tab: `liveModeCheckBox`, `cadenceComboBox`, `refreshTimer_Tick`, [`Services/DashboardSimulator.cs`](TicketOpsLive/Services/DashboardSimulator.cs) |
| Live ticket board | M05 | **Ticket board** tab: `BindingList<Ticket>` + `BindingSource` + `ApplyTicketEvent`, Escalated filter |
| Multi-user TicketHub | M06 | [`Services/TicketHub.cs`](TicketOpsLive/Services/TicketHub.cs) + `Hub_TicketChanged` (tenant filter, escalation toast, Subscribe / Unsubscribe) |
| Production checklist and configuration review | M07 | **Health · config** tab: `RealtimeHealthSnapshot`, `RenderHealth`, `healthTimer`, `LoadConfiguration`, the checklist |

## What to try

| Action | What you should see |
|---|---|
| ▶ Start heartbeat | the clock and load bar move once per second with no click |
| Start Import / Cancel Import / Fail at 87 | progress every 10 records while the heartbeat keeps running; a cancelled and a failed run both re-enable Start |
| Publish Event / New ticket events (10) / Escalate selected | the rows and notifications appear in every session of the same tenant; escalation also pops a toast |
| Tenant / Escalated only | this session's board changes; other sessions are unaffected |
| Unsubscribe / Subscribe | this session stops (and resumes) receiving hub events |
| Live mode, Cadence | events received grow faster than updates applied; the cadence changes the tick rate |
| Counter +1 / Background update | a per-session counter; a pushed log line 1.5 s later with no click |
| Health · config | *Push expected* / *Polling off*, active subscriptions, measured updates/min, the effective `Default.json` and `HealthCheck.json`, the ten-point checklist |

**Polling fallback.** Add `"enableWebSocket": false` to `Default.json` and restart: `connectionLabel` shows
*No WebSocket*, the Health tab reads *Fallback mode* / *Polling enabled* while work runs, and pushes arrive on the polls.

## Deliverables

- [`docs/ArchitectureNote.md`](TicketOpsLive/docs/ArchitectureNote.md) — the one-page architecture note
- [`docs/DemoScript.md`](TicketOpsLive/docs/DemoScript.md) — the lab's nine-step demo
- [`docs/ProductionChecklist.md`](TicketOpsLive/docs/ProductionChecklist.md) — the ten questions + security + observability
- [`docs/ConfigurationReview.md`](TicketOpsLive/docs/ConfigurationReview.md) — `Default.json` / `HealthCheck.json`, sticky sessions, proxy notes

## Self-check (acceptance criteria)

- **No duplicate loops or subscriptions** — every start is guarded (`_heartbeatRunning`, `_importRunning`,
  `_feedRunning`, `DashboardSimulator.Start()`); `SubscribeToHub` is a no-op when already subscribed.
- **The UI stays responsive** — the import, heartbeat, feed and simulator all run on tasks; no handler does long work.
- **Completion, cancellation and failure states** — the import has all three; the heartbeat and the feed have
  completion and failure; each restores the UI in `finally`.
- **The hub keeps no page or control references** — `TicketHub` stores `Ticket` values, hands out clones and raises
  on a thread-pool thread.
- **Bound grids update incrementally** — `ApplyTicketEvent` inserts or copies in place, calls `ResetBindings(false)`
  once and restores the selection; the grid is never rebound.
- **Deployment assumptions** — see the configuration review and the production checklist.
