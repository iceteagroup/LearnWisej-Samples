using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TicketOps.Data;
using TicketOps.Domain;
using TicketOps.Infrastructure;

namespace TicketOps.Services
{
    /// <summary>
    /// Builds the signed permalink the "Copy link" action hands to the browser, and records the copy once
    /// the browser confirmed it. No Wisej.NET type in here: the same code serves a button, a batch and a test.
    ///
    /// Security shape: the id arrives from the screen (client input) and is re-loaded; the rule is the
    /// domain's (<see cref="WorkOrder.CanShareLink"/>); the URL is assembled here with a key that never
    /// leaves this class — the script only ever receives the finished string.
    /// </summary>
    public sealed class TicketLinkService : ITicketLinkService
    {
        private const string BaseUrl = "https://ticketops.local/t/";

        private readonly IWorkOrderRepository _repository;
        private readonly IAuditLogService _audit;
        private readonly byte[] _signingKey;
        private readonly ILog _log;

        public TicketLinkService(IWorkOrderRepository repository, IAuditLogService audit, string signingKey, ILog log)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _audit = audit ?? throw new ArgumentNullException(nameof(audit));
            if (string.IsNullOrEmpty(signingKey)) throw new ArgumentException("A signing key is required.", nameof(signingKey));
            _signingKey = Encoding.UTF8.GetBytes(signingKey);
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<OperationResult<TicketLink>> BuildLinkAsync(int workOrderId, SessionUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Never trust the client-sent id: re-load the record scoped to what this user may see.
            var order = await _repository.FindAsync(workOrderId);
            if (order == null)
                return OperationResult<TicketLink>.Fail("That work order does not exist or is not visible to you.");

            // The rule lives on the domain object; the service applies it.
            if (!order.CanShareLink(user, out string reason))
            {
                _log.Warn(LogLayer.Domain, "WorkOrder.CanShareLink", $"#{workOrderId} rejected for {user.Name}: {reason}");
                return OperationResult<TicketLink>.Fail(reason);
            }

            var link = new TicketLink(order.Id, BaseUrl + order.Id.ToString(CultureInfo.InvariantCulture) + "?sig=" + Sign(order.Id));
            return OperationResult<TicketLink>.Ok(link, $"Link for #{order.Id} ready.");
        }

        public Task ConfirmCopiedAsync(int workOrderId, SessionUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            _audit.Record("ticket.link.copied", workOrderId, user);
            return Task.CompletedTask;
        }

        /// <summary>A short HMAC over the id, so a link cannot be guessed by changing the number. Demo key, demo length.</summary>
        private string Sign(int workOrderId)
        {
            using (var hmac = new HMACSHA256(_signingKey))
            {
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(workOrderId.ToString(CultureInfo.InvariantCulture)));
                return Convert.ToHexString(hash, 0, 3).ToLowerInvariant();
            }
        }
    }
}
