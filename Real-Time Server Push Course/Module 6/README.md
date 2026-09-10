# TicketOpsLive · Real-Time Apps with Server Push · Module 6

Local lab build for **Module 6 · From One Session to Many: TicketHub Events**. The ticket simulation of the earlier
modules is refactored into a **global `TicketHub` service**: a thread-safe singleton that owns the shared ticket list
and raises `TicketChanged`, while every browser session subscribes, filters by its own tenant and updates its own UI
in its own context.

This is the first sample of the course where **one tab is not enough**. Open two or three.

Nothing here is deployed anywhere; it is a plain Wisej.NET 4 project on this machine.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 6/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5306
```

Then open <http://localhost:5306>. (Visual Studio: open `TicketOpsLive.slnx`, press F5.)

Requirements already on this machine: .NET 10 SDK, the `Wisej-4` 4.1.0 NuGet package.

## The two-tab (three-tab) demo

The hub lives in the server **process**, so every browser tab pointed at `localhost:5306` is a separate session
talking to the same hub.

1. Open **tab A** at <http://localhost:5306> and leave `TENANT` on **Contoso**.
2. Click **Open another session ↗** in tab A — that is `Application.Navigate("/", "_blank")`, a brand-new session.
   This is **tab B**; leave it on **Contoso** too. Both traces now say `hub subscribers = 2`.
3. Open a third tab (**tab C**) and switch its `TENANT` to **Northwind**. All three say `hub subscribers = 3`.
4. **A Contoso publish reaches A and B.** In tab A select a row and click **Publish ticket (my tenant)**. Tab A gets
   its own push; **tab B updates with no click at all**; tab C logs `• server filtered out (tenant Contoso ≠ …)` and
   its `filtered out` counter goes up. The same `event <id>` appears in A's and B's traces.
5. **A Northwind publish reaches only C.** Click **Publish for the other tenant** in tab A. Now A and B count it as
   filtered out and C is the one that renders the new ticket.
6. **Only escalations interrupt.** Select a row in tab C and click **Escalate selected**: C (and any other Northwind
   session) shows the row as `Escalated` *and* an `AlertBox` toast in the top-right corner. `Added` and `Updated`
   never pop a toast — that is the event-type filter.
7. **Closing B unsubscribes.** Close tab B. Publish again from A: A's trace now reads `subscribers 2`, and the server
   console prints `TicketHub: unsubscribed (ApplicationExit) · subscribers now 2`. Nothing broke, nothing leaked.
8. **An unsubscribed session receives nothing.** Instead of closing a tab, click **Unsubscribe** in it: the count
   drops, `hub subscribers` turns red, and every publish from the other tabs leaves that tab completely silent.
   **Subscribe** brings it back.

Watch the two kinds of number while you do this: `hub: N tickets · M events published` is **global** and identical in
every tab; `N notifications in this session` and `filtered out` are **per session** and different in every tab.

## What to try

| Button | Path | What you should see |
|---|---|---|
| (page load) | — | `← request MainPage_Load … IsWebSocket=false`, then `• server hub.GetSnapshot() 6 ticket(s) in the hub → 3 shown for tenant "Contoso"` and `• server hub.TicketChanged += subscribed (page load) · hub subscribers = n` |
| `TENANT` combo | filtering | the board reloads from the same snapshot with the other tenant's tickets; the hub is not told anything — `same hub, same events, a different filter` |
| **Publish ticket (my tenant)** | success · broadcast to a tenant | with a row selected it advances that ticket's status and owner; with nothing selected it opens a new one. `• server hub.AddOrUpdate #id → TicketChanged (tenant …, subscribers n)`, then `→ push Application.Update(_context) event … applied in THIS session` — **and the same push in every other session of that tenant** |
| **Publish for the other tenant** | targeted update | the event is published, this session drops it (`• server filtered out (tenant …)`, `filtered out` +1, blue banner) and the other tenant's sessions render it |
| **Escalate selected** | event-type filter | `ChangeType = "Escalated"` → matching sessions render the row *and* show an `AlertBox` toast (TopRight, 4 s). Needs a selected row, otherwise an info banner |
| **Subscribe** / **Unsubscribe** | subscription lifecycle | `hub.TicketChanged += / -= Hub_TicketChanged`; `hub subscribers` moves by one; an unsubscribed session receives nothing at all until it subscribes again |
| **Publish invalid ticket** | failure | a ticket with an empty `Title` and an empty `TenantId`: the hub throws `ArgumentException` **before** the lock → red banner, and `• server hub state after the rejection tickets 6 → 6, events 4 → 4 — unchanged, and no subscriber was called` |
| **Burst 20 events** | progress · cadence | `Application.StartTask` publishes 20 updates 150 ms apart; 20 `→ push` lines here and in every other session of the tenant, then the `finally` line `burst completed — 20/20 events published`. A second click while it runs is refused |
| **Open another session ↗** | multi-session | `Application.Navigate("/", "_blank")` — a new tab, a new session, a new `MainPage`, a new subscription |
| **Clear trace** | — | empties the trace and the notification list; the counters keep counting |

**Polling fallback.** Set `"enableWebSocket": false` in `Default.json` and restart. `IsWebSocket` stays false; the
**Burst** button requests `Application.StartPolling(1000)` while the task runs (`• server Application.StartPolling(1000)`)
and `EndPolling()` when it ends. Note the honest limitation this exposes: a session that is *idle* — one that never
started a task of its own — has no polling running, so a hub event published by **another** tab has no channel to
travel on and lands on the next request that session makes. With the WebSocket on (the default) it arrives instantly.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Create a singleton `TicketHub` with a thread-safe ticket list | [`Services/TicketHub.cs`](TicketOpsLive/Services/TicketHub.cs) — `Lazy<TicketHub>`, private ctor, `lock (_sync)`, `List<Ticket>` |
| Add `TicketChanged` events | `TicketHub.TicketChanged` (explicit `add`/`remove` so `SubscriberCount` is exact) + [`Models/TicketChangedEventArgs.cs`](TicketOpsLive/Models/TicketChangedEventArgs.cs) |
| Load a snapshot when `MainPage` starts | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `MainPage_Load` → `LoadSnapshot(_hub.GetSnapshot(), "page load")` |
| Subscribe the page to `TicketChanged` | `Subscribe()` (from `MainPage_Load` and `subscribeButton_Click`) |
| Unsubscribe on application exit | `Application_ApplicationExit` → `Unsubscribe("ApplicationExit")`, plus `MainPage_Disposed` and the **Unsubscribe** button — [`docs/SubscriptionCleanup.md`](TicketOpsLive/docs/SubscriptionCleanup.md) |
| Open two browsers / sessions and verify both receive updates | `openSessionButton_Click` → `Application.Navigate("/", "_blank")`; the demo above |
| Add tenant or role metadata and filter at least one event type | `Ticket.TenantId` + `TicketChangedEventArgs.TenantId` / `ChangeType`; the tenant filter and the `"Escalated"`-only toast in `Hub_TicketChanged` |
| Show every path (success / progress / cancellation / failure) | success `publishButton_Click`, progress `burstButton_Click` → `RunBurst`, stop `_bursting` / `IsDisposed` / `MainPage_Disposed`, failure `invalidButton_Click` → `HandleRejectedPublish` |
| Extension challenge: a per-session notification count | `_notificationCount` (instance field) → `notificationCountLabel` in `RenderCounters()` |
| Deliverable: hub design note | [`docs/TicketHubDesign.md`](TicketOpsLive/docs/TicketHubDesign.md) |
| Deliverable: cleanup rules | [`docs/SubscriptionCleanup.md`](TicketOpsLive/docs/SubscriptionCleanup.md) |
| Deliverable: broadcast vs. targeted | [`docs/BroadcastVsTargeted.md`](TicketOpsLive/docs/BroadcastVsTargeted.md) |

## Where things live

```
TicketOpsLive/
├─ MainPage.cs                  the subscriber: context capture, snapshot, tenant filter, Application.Update(_context, …),
│                               subscribe/unsubscribe, the burst task, BeginPush/EndPush
├─ MainPage.Designer.cs         board card (grid + tenant combo), notifications card, trace card, action bar
├─ Models/
│  ├─ Ticket.cs                 INotifyPropertyChanged + TenantId + Clone()/CopyFrom()
│  ├─ TicketStatus.cs           New · Assigned · Waiting · Resolved · Escalated
│  └─ TicketChangedEventArgs.cs Ticket · ChangeType · Message · EventId · PublishedBy · TenantId
├─ Services/
│  └─ TicketHub.cs              the global singleton: lock, snapshot of clones, AddOrUpdate, TicketChanged
├─ Program.cs                   Application.MainPage = new MainPage()
├─ Startup.cs                   Kestrel host (app.UseWisej())
├─ Default.json                 Wisej.NET application config (add "enableWebSocket": false to see the fallback)
└─ docs/
   ├─ TicketHubDesign.md        ownership, hub API, snapshots, thread safety, why the hub never calls Update
   ├─ SubscriptionCleanup.md    every subscription and its unsubscribe; refresh / close / exit / timeout
   └─ BroadcastVsTargeted.md    broadcast · tenant-scoped · targeted · role-scoped, and backpressure
```

## Self-check answers

- **Which session owns the UI?**
  The one the controls belong to. Every browser tab gets its own `MainPage` with its own `ticketsGrid`,
  `BindingList<Ticket>`, tenant and counters. The hub owns none of it — it owns data and an event. When a hub event
  arrives, it arrives on the *publisher's* thread; the subscriber has to put itself back into its own session with
  `Application.Update(_context, …)` before it may touch a control. SERVER STATE prints each session's own `ClientId`.

- **Broadcast vs. targeted update — explain the difference.**
  Broadcast means every subscriber receives the event; targeted means only some sessions do. This hub always
  broadcasts and carries the metadata (`TenantId`, `ChangeType`, `PublishedBy`) that lets each session decide. The
  Contoso tabs render a Contoso event and drop a Northwind one; every tenant-matching tab renders an `Escalated`
  event, but only that type also pops a toast. Alternatively the hub could keep subscription groups and walk only
  the matching list — better for very large fan-outs, worse for a first version, because the hub starts holding
  per-session state. Full table in [`docs/BroadcastVsTargeted.md`](TicketOpsLive/docs/BroadcastVsTargeted.md).

- **Why does the hub not call `Application.Update()`?**
  It owns no session, so it has no context to update. To call it, the hub would have to store an `IWisejComponent`
  per subscriber — i.e. become a bag of session references, responsible for the lifetime of every page on the
  server. It announces instead; the sessions react.

- **Why snapshots instead of exposing the list?**
  `public List<Ticket> Tickets => _tickets;` hands a caller the live collection: UI code can mutate global state by
  accident, and it enumerates a list another thread may be changing. `GetSnapshot()` returns clones produced under
  the lock, so the ownership boundary is explicit.

- **What would a leaked subscription look like?**
  The hub would keep a handler pointing at a disposed `MainPage`, holding that page, its grid and its list alive for
  the life of the process, and it would keep calling into a dead session on every publish. `hub subscribers` is
  printed in every tab precisely so the reviewer can see the number go **down** when a tab closes.

- **How is the background work cancelled and cleaned up?**
  `RunBurst` is bounded (20 iterations) and its loop condition is `_bursting && !this.IsDisposed`;
  `MainPage_Disposed` sets `_bursting = false`; exceptions are caught inside the task; the `finally` block re-enables
  the button, calls `EndPush()` and pushes the final state through `Application.Update(_context, …)`.

- **How is push cadence controlled?**
  By the publisher. `TicketHub.AddOrUpdate` queues one fan-out per publish, so a loop that publishes without pausing would
  push every subscribed session at its own speed. `RunBurst` sleeps 150 ms between publishes, and the final state is
  always pushed from `finally`. When the source cannot be slowed, use backpressure — the table in
  [`docs/BroadcastVsTargeted.md`](TicketOpsLive/docs/BroadcastVsTargeted.md).

## Acceptance criteria → code

| Criterion | Where it is satisfied |
|---|---|
| The hub does not store page or control references | `TicketHub`'s only instance fields are `_sync`, `_tickets` (values), the `TicketChanged` delegate and two counters. No `Page`, no `Control`, no `IWisejComponent` appears anywhere in the file — it does not even `using Wisej.Web` |
| Each session updates its own bound list | `_tickets` is a `BindingList<Ticket>` instance field bound through `ticketsBindingSource`; `LoadSnapshot` / `ApplyTicketEvent` clone into it |
| Closing or refreshing a session does not leave a broken subscriber behind | `Application_ApplicationExit` **and** `MainPage_Disposed` both call the idempotent `Unsubscribe`; `Hub_TicketChanged` opens with `if (this.IsDisposed) return;`; `SafeUpdate` catches `ObjectDisposedException`; `TicketHub.Raise` catches and logs a failing subscriber |
| Multiple sessions can show different filters while sharing the same domain event source | `_tenant` is per session and switchable at runtime from `tenantComboBox`; the same event object reaches every subscriber and each one decides |
| The student can explain broadcast vs. targeted update | [`docs/BroadcastVsTargeted.md`](TicketOpsLive/docs/BroadcastVsTargeted.md) and the self-check above |
| Extension: a per-session notification count | `_notificationCount`, incremented inside the `Application.Update(_context, …)` callback, shown in `notificationCountLabel` |

## Not verified at runtime

This sample was written and compiled (`dotnet build -nologo -v q` — 0 warnings, 0 errors) but **not executed**. The
APIs the cookbook marks *(unverified)* and that this module leans on are `Application.Current` as the stored context,
`Application.Update(context, callback)` from a foreign thread, `Application.ApplicationExit`,
`Application.Navigate(url, target)` and the whole `DataGridView` / `BindingSource` binding path. Everything above is
what the code is written to do — confirm it in the browser.
