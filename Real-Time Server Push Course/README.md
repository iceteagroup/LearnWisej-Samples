# Real-Time Apps with Server Push · lab samples

One runnable Wisej.NET 4 application per module, built from the course's lesson guide, lab guide and walkthrough
video. All seven build pieces of the same application, **TicketOps Live** (a Web Page Application named
`TicketOpsLive` with a `MainPage`, exactly as the lab guide asks). Every sample follows the same layout: the module's
feature on the left, a **Server → Browser · live push trace** on the right, and a button bar that exercises the
success path, a progress path, at least one failure path and the recovery. Each folder has its own `README.md`
(what to click, lab tasks → code, self-check answers) and a `docs/` folder with the lab deliverables.

Requirements already on this machine: .NET 10 SDK and the `Wisej-4` 4.1.0 NuGet package. Nothing is deployed anywhere.

| Module | Folder | What it builds | Run |
|---|---|---|---|
| 1 · WebSocket Push vs. Polling | `Module 1` | live status strip + simulated heartbeat (`Application.StartTask` → `Application.Update`), the three update mechanisms side by side, polling fallback, cadence experiment (100 pushes vs 10) | `dotnet run -f net10.0 --urls http://localhost:5301` |
| 2 · Who Owns the UI | `Module 2` | session inspector + lifecycle logger: `Application.Session` counter vs the static-field trap, `Application.Current` captured for out-of-bound updates, `ApplicationExit` cleanup, a global session registry that stores no page references | `http://localhost:5302` |
| 3 · Progress Without Refresh | `Module 3` | background import monitor: 200 records, push every 10, cooperative cancellation, simulated failure at record 87, `finally` restores the UI, JobId correlation, the blocking-handler anti-pattern | `http://localhost:5303` |
| 4 · When Push Is Not Enough | `Module 4` | update-cadence panel: `Wisej.Web.Timer` refresh at 250 ms / 1 s / 5 s, model events faster than the UI, dirty-flag coalescing, overlap guard, polling start/stop with live mode | `http://localhost:5304` |
| 5 · Live Ticket Board | `Module 5` | `DataGridView` bound through `BindingSource` to a `BindingList<Ticket>`, rows added and updated in place, selection preserved, "updated" marker, conflict warning, Escalated filter, the rebind anti-pattern | `http://localhost:5305` |
| 6 · From One Session to Many | `Module 6` | global `TicketHub` singleton (thread-safe, snapshots, `TicketChanged` with tenant metadata) feeding many sessions through `Application.Update(context, …)`, tenant filtering, per-session notification count, unsubscribe on exit | `http://localhost:5306` |
| 7 · Production Review (capstone) | `Module 7` | the complete TicketOps Live: status strip, session diagnostics, import monitor, cadence controls, ticket board, TicketHub, real-time health panel (push vs fallback), `Default.json` / `HealthCheck.json` review, production checklist, demo script | `http://localhost:5307` |

Run any module from its `TicketOpsLive` project folder. The projects multi-target `net10.0-windows` and `net10.0`,
so `dotnet run` needs a framework (`-f net10.0`, or `-f net10.0-windows`), e.g.

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 3/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5303
```

or open the `TicketOpsLive.slnx` in the module folder with Visual Studio and press F5.

To see the **polling fallback** in any module, add `"enableWebSocket": false` to the module's `Default.json` and
restart: `Application.IsWebSocket` stays false, the samples request `StartPolling(1000)` while a task runs and the
browser console shows one `Wisej: Poll request.` per second. Multi-session modules (2, 6, 7) are meant to be opened in
two or three browser tabs at once.

## `_template`

The scaffold every module was built from, plus `COOKBOOK.md`: the real-time conventions verified while building and
running these samples (`StartTask` + `Update`, the context callback, what `IsWebSocket` reads during Load, where
`StartPolling` belongs, cadence numbers, the trace card, the docs layout). Read it before writing a new sample.
