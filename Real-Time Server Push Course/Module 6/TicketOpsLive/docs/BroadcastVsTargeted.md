# Broadcast vs. targeted update

The question the instructor asks in the review: *"who receives this event, and who decides?"*

## The four shapes

| Shape | Who receives it | Typical use | In this sample |
|---|---|---|---|
| **Broadcast** | every subscriber | severe system alerts on every operations console; "the nightly import finished" | the hub's fan-out itself: `TicketChanged` goes to **all** subscribers, always |
| **Tenant-scoped** | sessions of one tenant | a ticket event that only concerns one customer organisation | `e.TenantId` vs. `_tenant` in `Hub_TicketChanged`; **Publish for the other tenant** shows the miss |
| **Targeted (per user)** | one operator | "ticket 4822 was assigned to you" | `e.PublishedBy` is already carried; a session would compare it (or an `AssignedTo`) with `Application.SessionId` (not `ClientId`, which is the browser and is shared by its tabs) |
| **Role-scoped** | users holding a role | admin-only events, supervisor dashboards | same mechanism, a `Role` field on the event args and a role on the session |

There is a fifth axis that is easy to miss: filtering by **event type**. This sample uses it — every subscriber of
the right tenant renders `Added` / `Updated` / `Escalated`, but only `Escalated` also raises an `AlertBox` toast.
The rule "everyone sees it in the list, only escalations interrupt you" is a filter, and it lives in the session.

## Metadata and let the session decide

The hub does **not** manage subscription groups. It carries metadata and every session applies its own rule:

```csharp
public class TicketChangedEventArgs : EventArgs
{
    public Ticket  Ticket      { get; set; }   // the payload (a snapshot)
    public string  ChangeType  { get; set; }   // "Added" | "Updated" | "Escalated"
    public string  Message     { get; set; }   // the human line
    public string  EventId     { get; set; }   // followable across sessions
    public string  PublishedBy { get; set; }   // the publishing session's SessionId
    public string  TenantId    { get; set; }   // the routing metadata
}
```

Why this and not groups, at least for the first version:

- **the hub stays dumb and stateless about UI.** Subscription groups mean the hub keeps a map of who-wants-what,
  which is one short step away from keeping a map of who-*is*-what — the bag of session references again;
- **sessions change their mind.** In this sample the operator switches `tenantComboBox` at runtime; with groups
  that would be a re-registration round trip into a locked structure, with metadata it is one field assignment;
- **two sessions can filter differently while sharing one event source** — the lab's acceptance criterion. Tab A on
  Contoso and tab C on Northwind receive the identical event object and reach opposite conclusions;
- it is trivially testable: the filter is a pure comparison in the page, with no hub state involved.

The cost is honest and worth stating: **every event reaches every subscriber**, so the fan-out is O(sessions) even
when one session cares. That is fine for a few hundred dashboards and one event per human action; it is not fine for
a firehose. When the fan-out itself becomes the cost, move the filter into the hub — group subscribers by tenant and
walk only the matching list — but keep the shape: the hub still stores subscriptions, never controls.

## Backpressure

`AddOrUpdate` fans out on a thread-pool thread right after each publish, so a publisher that loops without pausing pushes every
subscriber at its own speed. The sample controls this at the source (`RunBurst` sleeps 150 ms between publishes),
which is the simplest correct answer. When the source cannot be slowed:

| Strategy | What it does | When to use it |
|---|---|---|
| **Drop duplicates** | discard a low-priority event that supersedes nothing | repeated "still running" pings |
| **Batch and summarise** | collect for N ms, then send one "12 tickets changed" event | bulk imports |
| **Dirty flag + timer** | mark the session dirty, redraw on a `Wisej.Web.Timer` tick | dashboards, counters |
| **Latest-value-wins** | keep only the newest state per key | a gauge, a queue depth, a price |
| **Bounded per-session queue** | queue events per subscriber, cap the size, drop the oldest | slow clients that must not slow the publisher |
| **Fan out off the publisher's thread** | `Task.Run` the `Raise` loop — what `TicketHub.Raise` does (verified: a synchronous fan-out on the publisher's request thread leaked the other session's control changes into the publisher's response) | always, in this sample |

The judgement behind all of them: *business applications rarely need to render every intermediate state — they need
to render the state the user can act upon.* Twenty grid repaints in three seconds teach the operator nothing; the
final one does.

## Evidence

Three tabs: A and B on **Contoso**, C on **Northwind**.

| Action | Tab A | Tab B | Tab C |
|---|---|---|---|
| A clicks **Publish ticket (my tenant)** | `→ push … Updated #4822 · published by THIS session` | `→ push … published by another session` | `• server filtered out (tenant Contoso ≠ this session's Northwind) event … — dropped, nothing rendered`, `filtered out: 1` |
| A clicks **Publish for the other tenant** | `• server filtered out (tenant Northwind ≠ … Contoso)`, info banner | same, filtered out | `→ push … Added #9023` and a new notification |
| C selects #9023, clicks **Escalate selected** | filtered out | filtered out | `→ push … Escalated #9023`, `• server event type filter ChangeType=Escalated → AlertBox toast` and a toast in the top-right corner |
| A clicks **Burst 20 events** | 20 `→ push` lines, 150 ms apart, then the `finally` line | the same 20 pushes, no clicks | `filtered out` climbs by 20, nothing renders |

`hub events published` is identical in all three tabs at every moment; `notifications in this session` and
`filtered out` are different in all three. That pair of facts *is* broadcast-vs-targeted, on screen.

*(Expected behaviour — the sample was compiled, not executed.)*
