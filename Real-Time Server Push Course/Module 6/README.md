# TicketOpsLive · Real-Time Apps with Server Push · Module 6

Local lab build for **Module 6 · From One Session to Many: TicketHub Events**. The ticket simulation is refactored
into a global `TicketHub`: a thread-safe singleton that owns the shared ticket list and raises `TicketChanged`.
Every browser session loads a snapshot, subscribes, filters by its own tenant and updates its own bound list and
notification panel in its own context.

## Run it

```bash
cd "D:/Projects/LearnWisej-Samples/Real-Time Server Push Course/Module 6/TicketOpsLive"
dotnet run -f net10.0 --urls http://localhost:5306
```

Then open <http://localhost:5306> in **two or three tabs** — every tab is a separate session talking to the same hub.

## What to try

1. Tabs A and B on **Contoso**, tab C on **Northwind** (the `Tenant` combo).
2. In tab A select a row and click **Publish Event**: A and B update (B with no click at all); C shows nothing.
3. In tab C click **Publish Event** with no row selected: a new Northwind ticket appears in C only.
4. Select a row and click **Escalate selected**: every session of that tenant updates the row *and* shows a toast.
   Added/Updated events never pop a toast — that is the event-type filter.
5. Click **Unsubscribe** in tab B: publishes from the other tabs no longer reach it. **Subscribe** brings it back.
6. Close tab B: its session ends, `ApplicationExit` unsubscribes it, and the other tabs keep working.

`notificationCountLabel` counts what *this* session received (the lab's extension challenge), even though every
event comes from the same global service.

## Lab tasks → where in the code

| Lab task | Where |
|---|---|
| Singleton `TicketHub` with a thread-safe ticket list | [`Services/TicketHub.cs`](TicketOpsLive/Services/TicketHub.cs): `Lazy<TicketHub>`, private ctor, `lock (_sync)` |
| `TicketChanged` events | `TicketHub.TicketChanged` + [`Models/TicketChangedEventArgs.cs`](TicketOpsLive/Models/TicketChangedEventArgs.cs) |
| Load a snapshot when `MainPage` starts | [`MainPage.cs`](TicketOpsLive/MainPage.cs) `MainPage_Load` → `LoadSnapshot(_hub.GetSnapshot())` |
| Subscribe the page to `TicketChanged` | `Subscribe()` (Load and the Subscribe button) |
| Unsubscribe on application exit | `Application_ApplicationExit` → `Unsubscribe()`, plus `MainPage_Disposed` — [`docs/SubscriptionCleanup.md`](TicketOpsLive/docs/SubscriptionCleanup.md) |
| Two sessions both receive updates | open two tabs (steps above) |
| Tenant metadata and one filtered event type | `Ticket.TenantId` / `TicketChangedEventArgs.TenantId`; the tenant check and the `"Escalated"`-only toast in `Hub_TicketChanged` |
| Extension: per-session notification count | `_notificationCount` → `notificationCountLabel` |

## Self-check

- **The hub stores no page or control references** — it stores `Ticket` values, hands out clones and raises the
  event on a thread-pool thread; it never calls `Application.Update`.
- **Each session updates its own bound list** — `_tickets` is an instance field; `ApplyTicketEvent` runs inside
  `Application.Update(_context, …)`.
- **Closing or refreshing a session leaves no broken subscriber** — `Unsubscribe()` runs from `ApplicationExit` and
  `Disposed`, idempotently; the handler also checks `IsDisposed`.
- **Different filters, one event source** — the tenant filter lives in the session, not in the hub.
- **Broadcast vs targeted** — the hub broadcasts every event to every subscriber with its metadata (`TenantId`,
  `ChangeType`); each session decides what to render (tenant-scoped) and what interrupts the user (escalations
  only). A targeted update to one operator would add an assignee field and the same session-side check.
