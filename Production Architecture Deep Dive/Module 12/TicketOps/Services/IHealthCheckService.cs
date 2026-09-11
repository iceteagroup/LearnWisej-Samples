using System.Threading.Tasks;
using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// The health check: read what HealthCheck.json declares, probe each dependency, aggregate.
    /// A dependency that fails is a result (Degraded or Unhealthy in the report), never an exception —
    /// the probe exists to report that failure, not to crash on it. Only a missing/corrupt manifest throws.
    /// </summary>
    public interface IHealthCheckService
    {
        Task<HealthReport> CheckAsync();
    }
}
