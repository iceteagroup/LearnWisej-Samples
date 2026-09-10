using System;
using System.Runtime.CompilerServices;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Production profile: notifications go out by e-mail. This stand-in logs the SMTP send it would
    /// perform (no network in the lab). Stateless, hence Transient.
    /// </summary>
    public sealed class EmailNotificationService : INotificationService
    {
        private readonly ILog _log;

        public EmailNotificationService(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public Notification Notify(int operatorId, string message)
        {
            var n = new Notification { OperatorId = operatorId, Message = message, Channel = "email", SentAt = DateTime.Now };
            _log.Info(LogLayer.Service, $"EmailNotificationService#{RuntimeHelpers.GetHashCode(this):x4}.Notify",
                $"would send via smtp.ticketops.local:587 → operator {operatorId}: \"{message}\"");
            return n;
        }
    }
}
