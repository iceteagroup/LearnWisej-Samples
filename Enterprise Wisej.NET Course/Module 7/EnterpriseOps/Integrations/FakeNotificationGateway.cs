using System;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;

namespace EnterpriseOps.Integrations
{
    /// <summary>
    /// In-memory stand-in for the e-mail / in-app / SMS provider. FailNextSend(reason) arms the
    /// lab's failure path: the next send times out AFTER the escalation was persisted.
    /// </summary>
    public class FakeNotificationGateway : INotificationGateway
    {
        private readonly ActivityTrace _trace;
        private string _failNextReason;
        private int _sequence = 5000;

        public FakeNotificationGateway(ActivityTrace trace)
        {
            _trace = trace;
        }

        public bool IsArmedToFail => _failNextReason != null;

        /// <summary>Lab switch: the next send throws NotificationFailedException(reason).</summary>
        public void FailNextSend(string reason)
        {
            _failNextReason = reason;
            _trace.Write($"Integrations: notification gateway armed — the next send will fail ({reason})");
        }

        public async Task<NotificationReceipt> SendApproverNotificationAsync(Escalation escalation, Approver approver)
        {
            _trace.Write($"Integrations: sending {escalation.Channels} notification for {escalation.Number} to {approver.Id}…");
            await Task.Delay(400);                  // the round trip to the provider

            if (_failNextReason != null)
            {
                string reason = _failNextReason;
                _failNextReason = null;
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
