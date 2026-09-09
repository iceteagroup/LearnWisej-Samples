using System;
using System.Collections.Generic;
using System.Linq;

namespace EnterpriseOps.Services.Jobs
{
    /// <summary>A notification is a record too: read/unread state, so nothing is lost when a browser closes at the wrong moment.</summary>
    public sealed class Notification
    {
        public int Id { get; set; }
        public string TenantId { get; set; }
        public string User { get; set; }        // recipient; null = every user of the tenant with the Role
        public string Role { get; set; }
        public Guid? JobId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedUtc { get; set; }
        public bool IsRead { get; set; }

        public override string ToString() => $"{(IsRead ? "  " : "● ")}{CreatedUtc.ToLocalTime():HH:mm:ss}  {Title} — {Message}";
    }

    /// <summary>
    /// Accepts a message for a user or a role in a tenant, stores it, and lets any live session of that user
    /// pick it up (the bell). If the user is not logged in, the notification waits until they are.
    /// </summary>
    public interface INotificationService
    {
        Notification Publish(string tenantId, string user, string role, Guid? jobId, string title, string message);
        IReadOnlyList<Notification> For(string tenantId, string user, string role);
        int UnreadCount(string tenantId, string user, string role);
        int MarkAllRead(string tenantId, string user, string role);

        /// <summary>Raised on the publishing thread (the queue worker) — observers hop into their own session context.</summary>
        event EventHandler<Notification> Published;
    }

    public sealed class InMemoryNotificationService : INotificationService
    {
        private readonly object _gate = new object();
        private readonly List<Notification> _items = new List<Notification>();
        private int _nextId = 1;

        public event EventHandler<Notification> Published;

        public Notification Publish(string tenantId, string user, string role, Guid? jobId, string title, string message)
        {
            var item = new Notification
            {
                TenantId = tenantId, User = user, Role = role, JobId = jobId,
                Title = title, Message = message, CreatedUtc = DateTime.UtcNow
            };
            lock (_gate)
            {
                item.Id = _nextId++;
                _items.Add(item);
            }
            Published?.Invoke(this, item);
            return item;
        }

        /// <summary>Tenant-aware: a user only ever sees notifications addressed to them (or their role) in their tenant.</summary>
        public IReadOnlyList<Notification> For(string tenantId, string user, string role)
        {
            lock (_gate)
                return _items.Where(n => Matches(n, tenantId, user, role))
                    .OrderByDescending(n => n.CreatedUtc).ToList();
        }

        public int UnreadCount(string tenantId, string user, string role)
        {
            lock (_gate)
                return _items.Count(n => !n.IsRead && Matches(n, tenantId, user, role));
        }

        public int MarkAllRead(string tenantId, string user, string role)
        {
            lock (_gate)
            {
                int count = 0;
                foreach (var n in _items.Where(n => !n.IsRead && Matches(n, tenantId, user, role)))
                {
                    n.IsRead = true;
                    count++;
                }
                return count;
            }
        }

        private static bool Matches(Notification n, string tenantId, string user, string role) =>
            n.TenantId == tenantId && (n.User == user || (n.User == null && n.Role == role));
    }
}
