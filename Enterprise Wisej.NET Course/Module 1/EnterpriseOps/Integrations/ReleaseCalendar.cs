using System.Collections.Generic;
using EnterpriseOps.Diagnostics;

namespace EnterpriseOps.Integrations
{
    /// <summary>
    /// Fake of the release/deployment calendar (the CI/CD system in Module 12). Answers the "Deployments today"
    /// KPI per tenant. In-memory and deterministic so the tile reads 2 for contoso every run.
    /// </summary>
    public sealed class ReleaseCalendar
    {
        private readonly ActivityTrace _trace;
        private readonly Dictionary<string, int> _deploymentsToday = new Dictionary<string, int>
        {
            ["contoso"] = 2,
            ["fabrikam"] = 0,
            ["northwind"] = 1,
        };

        public ReleaseCalendar(ActivityTrace trace)
        {
            _trace = trace;
        }

        public int CountDeploymentsToday(string tenantId)
        {
            int count = _deploymentsToday.TryGetValue(tenantId, out int n) ? n : 0;
            _trace.Integration($"ReleaseCalendar.CountDeploymentsToday({tenantId}) → {count}");
            return count;
        }
    }
}
