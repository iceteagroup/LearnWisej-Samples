# TicketHub — design note

The lab deliverable of Module 6: a shared service that safely feeds many Wisej.NET sessions.

## 1. Ownership — the architecture change of the module

| Owns | What it owns | In this sample |
|---|---|---|
| **The page** | its controls and everything drawn on them | `ticketsGrid`, `notificationsList`, `labelBanner`, `_tickets`, `_notificationCount`, `_filteredOut` — all `MainPage` **instance** fields |
| **The session** | its Wisej context | `_context = Application.Current`, captured in `MainPage_Load` while inside a request |
| **The global service** | shared data and domain events | `TicketHub.Instance` — a `List<Ticket>` behind a `lock`, and the `TicketChanged` event |

The rule underneath the table: **the global service must never become a bag of form references.** `TicketHub` has no
field, parameter or event argument that can hold a `Page`, a `Control` or an `IWisejComponent`. What it stores is
values (`Ticket`), and what it publishes is values (`TicketChangedEventArgs`). If you can point at a control from
inside the service, the design has already failed — you have coupled a global object to one session's lifetime.

## 2. The hub API

```csharp
public sealed class TicketHub
{
    public static TicketHub Instance { get; }                       // Lazy<T>, ExecutionAndPublication
    public event EventHandler<TicketChangedEventArgs> TicketChanged; // explicit add/remove under the lock
    public int SubscriberCount { get; }                              // exact, because the accessors count
    public int EventsPublished { get; }
    public int TicketCount { get; }

    public IReadOnlyList<Ticket> GetSnapshot();                      // clones, under the lock
    public Ticket Find(int id);                                      // a clone, or null
    public int NextTicketId(string tenantId);
    public TicketChangedEventArgs AddOrUpdate(Ticket ticket, string publishedBy, string changeType = null);
}
```

`AddOrUpdate` is the single write path. It

1. **validates first, outside the lock** — `Id > 0`, `Title` not empty, `TenantId` not empty, otherwise
   `ArgumentException`. A rejected publish leaves the list and the counters exactly as they were and calls nobody;
2. takes the lock, inserts a **clone** (or copies the fields into the existing record and moves it to the top),
   bumps `EventsPublished` and builds the event args — including a **clone** of the ticket;
3. releases the lock and **then** raises `TicketChanged`.

The private constructor seeds six tickets across two tenants (`Contoso` 4801/4822/4830, `Northwind` 9001/9014/9022),
so the first session to connect already has something to look at.

Returning the published `TicketChangedEventArgs` is the one liberty this sample takes with the lesson's signature: it
lets the publishing page print the event id in its own trace, which is what makes an event followable across two
browser tabs.

## 3. Snapshots, not references

```csharp
// Bad — the caller can mutate global state, and enumerates a list the hub may be changing
public List<Ticket> Tickets => _tickets;

// Better — a detached copy, produced under the lock
public IReadOnlyList<Ticket> GetSnapshot()
{
    lock (_sync)
        return _tickets.Select(t => t.Clone()).ToList();
}
```

The page clones **again** on the way in (`LoadSnapshot`, `ApplyTicketEvent`), so no two sessions ever bind the same
`Ticket` instance. That matters more than it looks: `Ticket` implements `INotifyPropertyChanged`, and a shared
instance would raise `PropertyChanged` into every session's binding infrastructure from whatever thread happened to
change it. The clone is cheap here; the ownership boundary it draws is the point.

## 4. Thread safety

- Every read and write of `_tickets`, `_subscriberCount` and `_eventsPublished` is inside `lock (_sync)`.
- `Lazy<TicketHub>(…, LazyThreadSafetyMode.ExecutionAndPublication)` guarantees exactly one instance even if two
  sessions touch `Instance` at the same millisecond.
- The `TicketChanged` accessors are written by hand so that `+=` / `-=` and the subscriber count change together,
  atomically. A plain field-like event would still be thread-safe for `+=`, but `SubscriberCount` could never be
  exact — and the demo needs it to be exact, because that number is the leak detector.
- **The event is raised outside the lock.** Subscribers do real work inside their handler (they render a grid and
  push a WebSocket frame). Holding a process-wide lock across that work would serialize every session behind the
  slowest one, and would deadlock the moment a handler called back into the hub — which `Hub_TicketChanged` does,
  every time, through `RenderCounters()`.
- `Raise` walks `GetInvocationList()` and wraps each handler in `try/catch`: a session that died between the guard
  and the call is logged to the server console and skipped. **One dead session never breaks the fan-out.**

## 5. Why the hub never calls `Application.Update`

Because it owns no session, so it has no context to push into. `Application.Update` needs an `IWisejComponent`
belonging to a specific session; the hub would have to store one per subscriber — i.e. become the bag of references
the lesson forbids — and it would then be responsible for the lifetime of every page on the server.

Instead the hub announces, and each session reacts in its own context:

```csharp
private void Hub_TicketChanged(object sender, TicketChangedEventArgs e)
{
    if (this.IsDisposed) return;                                     // dead-session guard
    if (!string.Equals(e.TenantId, _tenant)) { /* count it, drop it */ return; }   // the session filters

    Application.Update(_context, () =>                                // the session restores ITS context
    {
        ApplyTicketEvent(e);
        notificationsList.Items.Insert(0, e.Message);
        _notificationCount++;
    });
}
```

The handler runs on the **publisher's** thread — the request thread of the tab that clicked Publish, or the burst
task's thread. It has no session context of its own. `Application.Update(_context, …)` is what puts it back into the
right session before touching a control, and flushes every change of the callback in **one** push.

## 6. Cadence and backpressure

`AddOrUpdate` queues one fan-out per publish on the thread pool (in publish order), so every publish costs one push per subscribed session. The consequence is that **cadence belongs to the publisher**: `RunBurst` sleeps 150 ms between
publishes. A hub that emits faster than sessions can render needs backpressure — see `BroadcastVsTargeted.md`.

## Evidence

Two tabs, both on tenant **Contoso**. Tab A clicks **Publish ticket (my tenant)** with row 4822 selected. Expected:

**Tab A (the publisher)**

```
← request  publishButton_Click             the browser sent the click
• server   hub.AddOrUpdate                 #4822 → TicketChanged (tenant Contoso, subscribers 2) · update · raised outside the hub lock, fanned out on a thread-pool thread (never on this request thread)
→ push     Application.Update(_context)    event 3f9c21ab · Updated #4822 · published by THIS session — applied in THIS session
• server   fan-out complete                event 3f9c21ab · Updated · delivered to 2 subscriber(s) · hub events = 1
```

**Tab B (a foreign session, no click at all)**

```
→ push     Application.Update(_context)    event 3f9c21ab · Updated #4822 · published by another session 8c1d44a2… — applied in THIS session
```

Same `event 3f9c21ab` in both traces: one domain event, two sessions, two independent renders. The `→ push` line in
tab A appears *before* `fan-out complete` because the fan-out happens **inside** the `AddOrUpdate` call.

`hub: 6 tickets · 1 events published` reads the same in both tabs (it is global); `notifications in this session`
and `filtered out` differ per tab (they are instance fields).
