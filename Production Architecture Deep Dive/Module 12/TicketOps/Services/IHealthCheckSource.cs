using TicketOps.Domain;

namespace TicketOps.Services
{
    /// <summary>
    /// Where the health manifest comes from. Infrastructure/FileHealthCheckSource reads HealthCheck.json from the
    /// application folder (Application.MapPath); a test can hand the service a manifest built in memory.
    /// </summary>
    public interface IHealthCheckSource
    {
        /// <summary>Where the manifest lives, for the trace and the diagnostics page (a path, never a secret).</summary>
        string Location { get; }

        /// <summary>Reads and parses the manifest. A missing or corrupt manifest is an unexpected failure: it throws.</summary>
        HealthReport Load();
    }
}
