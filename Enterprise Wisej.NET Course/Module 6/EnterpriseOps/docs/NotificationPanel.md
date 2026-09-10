# Deliverable 4 — Notification panel

Notifications are how a job result reaches a person who is **no longer watching**. If the answer to "did the
import finish?" is only on a screen that was open at the time, there is no answer.

## The model

`Services/Jobs/NotificationService.cs`

```csharp
public sealed class Notification
{
    int Id; string TenantId; string User; string Role; Guid? JobId;
    string Title; string Message; DateTime CreatedUtc; bool IsRead;
}
```

A notification is a **record**, not an event: it has read/unread state, so nothing is lost when a browser
closes at the wrong moment. Addressing is `user` **or** `role` inside a tenant — `User = null, Role = "Manager"`
reaches every manager of that tenant.

```csharp
public interface INotificationService
{
    Notification Publish(string tenantId, string user, string role, Guid? jobId, string title, string message);
    IReadOnlyList<Notification> For(string tenantId, string user, string role);
    int UnreadCount(string tenantId, string user, string role);
    int MarkAllRead(string tenantId, string user, string role);
    event EventHandler<Notification> Published;
}
```

`For` filters on tenant **and** recipient: a user never sees another tenant's news, and never sees another
user's private notifications.

## Who publishes, and when

The queue's `NotifyingSink` (`Services/Jobs/JobQueue.cs`) — **after** the store has been updated, never
before, so a notification can never point at a status that does not exist yet. It publishes for:

- the `validated` milestone (the user learns the file was readable and how big it is), and
- every final state: `Completed`, `CompletedWithErrors`, `Failed`, `Canceled`.

Per-batch progress produces **no** notification. Progress is not news; it is what the progress bar is for.
Each published notification also appends a `Notify:` line to the job history, so the job detail shows that
someone was told.

If nobody is logged in, `Publish` still stores it — the notification simply waits until a session of that
user asks for it.

## The panel

`UI/ImportCenterPage` — `pnlNotifications` (`lstNotifications` + `btnMarkRead`) and the header's `btnBell`.

- `btnBell` shows the unread count: `🔔 Notifications (3)`.
- `lstNotifications` lists the records newest first; unread rows are marked `●`.
- A newly arrived unread notification also raises a toast
  (`AlertBox.Show(…, ContentAlignment.TopRight, autoCloseDelay: 6000)`) — top-right so it never covers the
  buttons.
- **Mark all read** calls the service, which writes a `Service:` trace line with the count.

The panel is painted by the observer's push loop like everything else, so a notification written by the
queue worker appears without the user doing anything.

## Evidence in the running app

- Start any import: within a second the bell shows `(1)` and a toast says
  `IMP-… — milestone: validated`. Neither came from a request.
- Press **Reopen** mid-import and wait: the completion notification arrives on the **new** page. The job was
  told nothing about the page swap.
- The seeded `fabrikam` job's notifications never appear for `ana.ops@contoso`.
