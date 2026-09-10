using System;

namespace TicketOps.Domain
{
    /// <summary>A message delivered to an operator by INotificationService (toast, e-mail, …).</summary>
    public sealed class Notification
    {
        public int OperatorId { get; init; }
        public string Message { get; init; }
        public string Channel { get; init; }
        public DateTime SentAt { get; init; }

        public override string ToString() => $"[{Channel}] to operator {OperatorId}: {Message}";
    }
}
