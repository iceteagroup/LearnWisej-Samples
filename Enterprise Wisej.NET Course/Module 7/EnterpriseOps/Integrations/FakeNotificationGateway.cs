using System;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Integrations
{
    /// <summary>
    /// In-memory stand-in for the e-mail / in-app / SMS provider, with a simulated notification failure:
    /// the SMTP relay times out on the first e-mail of the session (after the escalation was persisted)
    /// and delivers every send after that, so a retry from the manual-review queue goes through.
    /// </summary>
    public class FakeNotificationGateway : INotificationGateway
    {
        private readonly ActivityTrace _trace;
        private bool _smtpRelayReady;
        private int _sequence = 5000;

        public FakeNotificationGateway(ActivityTrace trace)
        {
            _trace = trace;
        }

        public async Task<NotificationReceipt> SendApproverNotificationAsync(Escalation escalation, Approver approver)
        {
            _trace.Write($"Integrations: sending {escalation.Channels} notification for {escalation.Number} to {approver.Id}…");
            await Task.Delay(400);                  // the round trip to the provider

            if (escalation.Channels.HasFlag(NotificationChannels.Email) && !_smtpRelayReady)
            {
                _smtpRelayReady = true;
                const string reason = "smtp timeout after 30s";
                _trace.Write($"Integrations: send FAILED — {reason}");
                throw new NotificationFailedException(reason);
            }

            var receipt = new NotificationReceipt
            {
                MessageId = $"msg-{++_sequence}",
                Channels = escalation.Channels,
                SentUtc = DateTime.UtcNow,
            };
            _trace.Write($"Integrations: sent {receipt}");
            return receipt;
        }
    }
}
