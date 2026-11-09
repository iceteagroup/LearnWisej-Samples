using System.Security.Principal;
using AccessOps.Identity;
using AccessOps.Security;
using Microsoft.Extensions.Logging;

namespace AccessOps.Documents
{
    /// <summary>
    /// The partner route's Export path. It has no rule of its own: the read goes through
    /// <see cref="RecordPolicy.Read(IPrincipal, string, int, string)"/>, the one check both routes
    /// call (current grant from the durable store, account, revoked, expired, read right, tenant),
    /// which denies before the repository runs. That is why a tab that was open before sign-out is
    /// denied with zero repository reads (fixture F05) and why tenant A's Reader cannot export
    /// tenant B's record 17 (F02).
    /// </summary>
    public sealed class ExportService
    {
        private readonly RecordPolicy _policy;
        private readonly CountingDocumentRepository _counter;
        private readonly ILogger<ExportService> _logger;

        /// <summary>Creates the service.</summary>
        public ExportService(RecordPolicy policy, CountingDocumentRepository counter, ILogger<ExportService> logger)
        {
            _policy = policy;
            _counter = counter;
            _logger = logger;
        }

        /// <summary>Exports one record for <paramref name="principal"/> (<c>Application.User</c>).</summary>
        public DocumentResult Export(IPrincipal principal, string tenant, int recordId)
        {
            var readsBefore = _counter.ReadCount;
            var result = _policy.Read(principal, tenant, recordId, "export");
            _logger.LogInformation("export {Decision} reason={Reason} grant={Grant} repositoryReads=+{Reads} correlation={CorrelationId}",
                result.Decision, result.Reason ?? "-", Grant.Shorten(Subject.From(principal).GrantId), _counter.ReadCount - readsBefore, result.CorrelationId);
            return result;
        }
    }
}
