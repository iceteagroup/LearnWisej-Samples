using System;
using System.Threading.Tasks;
using EnterpriseOps.Domain;

namespace EnterpriseOps.Integrations
{
    /// <summary>What the gateway reports back — typed, so the workflow never parses a message string.</summary>
    public class NotificationReceipt
    {
        public string MessageId;
        public NotificationChannels Channels;
        public DateTime SentUtc;

        public override string ToString() => $"{MessageId} via {Channels}";
    }

    /// <summary>The external step failed (SMTP timeout, provider 5xx, …). Nothing can be assumed about delivery.</summary>
    public class NotificationFailedException : Exception
    {
        public NotificationFailedException(string message) : base(message) { }
    }

    /// <summary>
    /// The external system in the workflow. It sits outside every database transaction, which is why the
    /// workflow needs a compensating action when it fails after the escalation was persisted.
    /// </summary>
    public interface INotificationGateway
    {
        Task<NotificationReceipt> SendApproverNotificationAsync(Escalation escalation, Approver approver);
    }
}
