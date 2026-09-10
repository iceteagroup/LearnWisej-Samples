using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Fake profile: records every notification instead of sending it, so a test can assert
    /// "the assignee was told". The instance id in the trace shows the Transient lifetime at work:
    /// every resolve produces a different one.
    /// </summary>
    public sealed class FakeNotificationService : INotificationService
    {
        private readonly List<Notification> _sent = new List<Notification>();
        private readonly ILog _log;

        public FakeNotificationService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>What this instance has "sent" — the assertion surface for the presenter tests.</summary>
        public IReadOnlyList<Notification> Sent => _sent;

        public Notification Notify(int operatorId, string message)
        {
            var n = new Notification { OperatorId = operatorId, Message = message, Channel = "toast", SentAt = DateTime.Now };
            _sent.Add(n);
            _log.Info(LogLayer.Service, $"FakeNotificationService#{RuntimeHelpers.GetHashCode(this):x4}.Notify",
                $"to operator {operatorId}: \"{message}\" (recorded, not sent · transient instance)");
            return n;
        }
    }
}
