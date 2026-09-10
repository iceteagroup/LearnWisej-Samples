using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The diagnostics page's one call. Role-protected here, in the service, so a second screen (or an API)
    /// cannot forget the check: a Technician gets a failed result with a safe message, a Supervisor gets the
    /// runtime facts plus the live health report.
    /// </summary>
    public interface IDiagnosticsService
    {
        Task<OperationResult<DiagnosticsSnapshot>> GetSnapshotAsync();
    }
}
