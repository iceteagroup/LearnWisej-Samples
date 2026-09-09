using System.Collections.Generic;

namespace EnterpriseOps.Services.Workflow
{
    /// <summary>The three numbers on the dashboard. Computed by the workflow, displayed by the KPI tiles.</summary>
    public sealed class DashboardKpis
    {
        public int OpenIncidents { get; set; }
        public int SlaAtRisk { get; set; }
        public int DeploymentsToday { get; set; }
    }

    /// <summary>
    /// One row of dgvIncidents. A projection built for the grid — the WorkOrder entity never crosses into the UI.
    /// Property names are the grid's DataPropertyName values.
    /// </summary>
    public sealed class IncidentRow
    {
        public string Id { get; set; }          // "INC-1042"
        public string Title { get; set; }
        public string Priority { get; set; }    // High / Medium / Low / Critical (display words)
        public string State { get; set; }       // open / triage / watch / escalated / mitigated
        public bool ChangedByThisRefresh { get; set; }
    }

    /// <summary>
    /// The typed result of DashboardWorkflow.LoadAsync / RefreshAsync. When <see cref="CommandResult.Succeeded"/>
    /// is false the errors are user-safe (a policy reason) and the data members are empty.
    /// </summary>
    public sealed class DashboardResult : CommandResult
    {
        public DashboardKpis Kpis { get; private set; } = new DashboardKpis();
        public List<IncidentRow> Incidents { get; } = new List<IncidentRow>();
        public long ElapsedMs { get; private set; }

        /// <summary>The footer text: "Loaded 12 incidents" / "Refreshed — INC-1042 mitigated".</summary>
        public string Summary { get; private set; } = "";

        public static DashboardResult Loaded(string correlationId, DashboardKpis kpis, IEnumerable<IncidentRow> rows, string summary, long elapsedMs)
        {
            var result = new DashboardResult { Succeeded = true, CorrelationId = correlationId, Kpis = kpis, Summary = summary, ElapsedMs = elapsedMs };
            result.Incidents.AddRange(rows);
            return result;
        }

        public static DashboardResult Denied(string correlationId, string reason, long elapsedMs)
        {
            var result = new DashboardResult { Succeeded = false, CorrelationId = correlationId, Summary = "Not permitted", ElapsedMs = elapsedMs };
            result.Errors.Add(reason);
            return result;
        }
    }
}
