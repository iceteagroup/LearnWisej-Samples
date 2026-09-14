using System;
using System.Drawing;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Health;
using WisejPerfLab.Services;
using WisejPerfLab.Shell;
using Wisej.Core;
using Wisej.Web;

namespace WisejPerfLab.Pages
{
    /// <summary>
    /// The capacity screen: what this instance is holding, what it has been told its limits are, where
    /// those limits came from, and what it last answered a load balancer.
    /// </summary>
    /// <remarks>
    /// Everything on this screen is a reading or a measured number. The thresholds come from
    /// <see cref="CapacityModel"/> (Modules 3 to 6), the live values from <see cref="ServerLoad"/> and
    /// <c>Application.SessionCount</c>, the configuration from <c>HealthCheck.json</c>, and the verdict
    /// from the same function the real <c>healthcheck.wx</c> endpoint calls.
    /// </remarks>
    public partial class CapacityPage : UserControl, IScenarioPage
    {
        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;

        public CapacityPage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();

            ShowReadings();
        }

        public string Scenario => "Capacity";

        public string UserAction => "HealthCheck";

        /// <summary>Asked by the shell alongside the other scenarios: cheap, and it must stay cheap.</summary>
        public void RunScenario() => AskHealthCheck();

        private void btnRefreshHealth_Click(object sender, EventArgs e) => AskHealthCheck();

        private void AskHealthCheck()
        {
            using var scope = _probe.Measure(Scenario, UserAction);

            // The same function the healthcheck.wx endpoint calls. A health check that is expensive is a
            // health check that makes the instance less healthy every time the load balancer asks.
            var available = HealthCheck.IsServerAvailable == null || HealthCheck.IsServerAvailable();

            ShowReadings();

            lblAnswer.ForeColor = available ? Color.FromArgb(31, 157, 87) : Color.FromArgb(224, 86, 59);
            lblAnswer.Text =
                $"healthcheck.wx → {(available ? "200 OK" : HealthCheck.ReturnCode + " (Retry-After: " + HealthCheck.RetryAfter + ")")}\r\n" +
                HealthPolicy.LastAnswer + $"   refusals since start: {HealthPolicy.Refusals}";

            _shell.SetStatus(available ? ShellState.Ok : ShellState.Fault,
                available
                    ? $"healthcheck: available — {Application.SessionCount} session(s)"
                    : "healthcheck: refusing new sessions — " + HealthPolicy.LastAnswer);
        }

        private void btnRefuseNew_Click(object sender, EventArgs e)
        {
            HealthPolicy.RefuseNewSessions();
            AskHealthCheck();

            _shell.ShowBanner(
                "The session limit is now the number of sessions that already exist. Open a second browser tab: " +
                "it will be refused with " + HealthCheck.ReturnCode + " and a Retry-After header, while this session keeps working.");
        }

        private void btnRestoreLimit_Click(object sender, EventArgs e)
        {
            HealthPolicy.RestoreConfiguredLimit();
            _shell.ClearBanner();
            AskHealthCheck();
        }

        private void ShowReadings()
        {
            var snapshot = MemoryProbe.Take(collect: false);

            lblLive.Text =
                $"sessions {Application.SessionCount}   memory {ServerLoad.MemoryPercent()} %   " +
                $"CPU {ServerLoad.CpuPercent()} %   managed heap {snapshot.HeapMb:N1} MB";

            lblConfig.Text = "HealthCheck.json — " + HealthPolicy.DescribeConfiguration() +
                             $"   (in force: maxSessions {HealthPolicy.EffectiveMaxSessions})";

            lblModel.Text = string.Join("\r\n", new[]
            {
                "The capacity model — every number measured in this application (docs/Capacity.md)",
                "",
                $"  memory   retained per idle session       {CapacityModel.RetainedMbPerIdleSession,6:N1} MB   (Module 4, snapshot A/B)",
                $"           budgeted per session            {CapacityModel.BudgetedMbPerSession,6:N1} MB   (docs/Budget.md)",
                $"           usable instance memory          {CapacityModel.UsableMemoryGb,6:N1} GB   (of {CapacityModel.InstanceMemoryGb:N0} GB)",
                $"           headroom kept                   {CapacityModel.HeadroomFraction,6:P0}",
                $"           -> maxSessions                  {CapacityModel.MaxSessionsFromMemory,6:N0}",
                "",
                $"  CPU      Dashboard/Refresh               {CapacityModel.DashboardRefreshMs,6:N0} ms   (Module 3)",
                $"           Tickets/Search                  {CapacityModel.TicketSearchMs,6:N0} ms   (Module 6)",
                $"           Tickets/Export (background)     {CapacityModel.ExportMs,6:N0} ms   (Module 6)",
                $"           -> refreshes per second per core {CapacityModel.RefreshesPerSecondPerCore,5:N1}"
            });
        }
    }
}
