using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WisejPerfLab.Data;
using WisejPerfLab.Diagnostics;
using WisejPerfLab.Models;
using WisejPerfLab.Services;
using WisejPerfLab.Shell;
using Wisej.Web;
using Wisej.Web.Ext.ChartJS;

namespace WisejPerfLab.Pages
{
    /// <summary>
    /// The support dashboard: three KPIs and a load chart behind one Refresh button.
    /// </summary>
    /// <remarks>
    /// Module 3 rewrote this handler. It used to load 50,000 entities, format five strings per row,
    /// count in memory, dispose the KPI cards and build new ones. It now asks
    /// <see cref="DashboardService.GetSnapshot"/> for display-ready values and <b>assigns</b> them to
    /// controls that already exist. What is left in the handler is the shape the lesson asks for:
    /// disable the button, assign, re-enable in a <c>finally</c>.
    /// </remarks>
    public partial class DashboardPage : UserControl, IScenarioPage
    {
        private static readonly Color OpenColor = Color.FromArgb(21, 101, 216);
        private static readonly Color OverdueColor = Color.FromArgb(224, 90, 59);
        private static readonly Color AgeColor = Color.FromArgb(125, 90, 224);

        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly DashboardService _dashboardService;

        // The KPI value labels, built once and kept. Module 2 threw these away on every refresh.
        private Label _lblOpenValue;
        private Label _lblOverdueValue;
        private Label _lblAvgAgeValue;

        // The guard: a second click while a refresh is in flight is dropped, not queued.
        private bool _refreshing;

        public DashboardPage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();
            _dashboardService = PerfLabServices.Get<DashboardService>();

            ConfigureChart();
            BuildKpiPanel();
        }

        public string Scenario => "Dashboard";

        public string UserAction => "Refresh";

        /// <summary>The shell runs the scenario through exactly the handler the button runs.</summary>
        public void RunScenario() => RunRefresh(fromShell: true);

        private void btnRefresh_Click(object sender, EventArgs e) => RunRefresh(fromShell: false);

        #region The measured scenario

        private void RunRefresh(bool fromShell)
        {
            // Clicked twice in quick succession: the second click is dropped. Without this, two refreshes
            // run one after the other and the second one is work nobody is waiting for.
            if (_refreshing)
            {
                _shell.SetStatus(ShellState.Busy, "a refresh is already running — the second click was dropped");
                return;
            }

            using var scope = _probe.Measure(Scenario, UserAction);

            _refreshing = true;
            btnRefresh.Enabled = false;
            if (!fromShell)
                _shell.ClearBanner();

            try
            {
                DashboardSnapshot snapshot;
                using (scope.Stage("snapshot (4 aggregate queries, formatted once)"))
                    snapshot = _dashboardService.GetSnapshot();

                using (scope.Stage("assign"))
                {
                    _lblOpenValue.Text = snapshot.OpenTicketsText;        // assign, do not format
                    _lblOverdueValue.Text = snapshot.OverdueTicketsText;
                    _lblAvgAgeValue.Text = snapshot.AverageAgeText;
                    ApplyChart(snapshot.ChartRows);
                }

                scope.Rows = snapshot.ChartRows.Count;

                var elapsed = scope.ElapsedMs;
                lblElapsed.Text =
                    $"Refresh {PerfBudget.Describe(Scenario, UserAction, elapsed)}   " +
                    $"{snapshot.QueryCount} queries   as of {snapshot.GeneratedAt:HH:mm:ss}";
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Dashboard/Refresh {PerfBudget.Describe(Scenario, UserAction, elapsed)}");
            }
            catch (DatabaseUnavailableException ex)
            {
                // The failure path still produces a measurement, and the button still comes back.
                scope.Fail(ex);
                lblElapsed.Text = "refresh failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Dashboard/Refresh failed — the ticket database is unreachable");
                _shell.ShowBanner("Dashboard refresh failed: the ticket database is unreachable. The numbers on screen are from the last successful refresh.");
            }
            finally
            {
                btnRefresh.Enabled = true;
                _refreshing = false;
            }
        }

        #endregion

        #region Control work — done once

        /// <summary>
        /// Builds the three KPI cards, once, in the constructor. The refresh updates the value labels in
        /// place: no construction, no layout pass, no theming, and no controls to dispose.
        /// </summary>
        private void BuildKpiPanel()
        {
            panelKpis.Controls.Add(BuildKpiCard("OPEN", OpenColor, 0, out _lblOpenValue));
            panelKpis.Controls.Add(BuildKpiCard("OVERDUE", OverdueColor, 284, out _lblOverdueValue));
            panelKpis.Controls.Add(BuildKpiCard("AVG AGE", AgeColor, 568, out _lblAvgAgeValue));
        }

        private static Panel BuildKpiCard(string caption, Color color, int x, out Label valueLabel)
        {
            var card = new Panel
            {
                Location = new Point(x, 0),
                Size = new Size(278, 88),
                BackColor = Color.White,
                BorderStyle = BorderStyle.Solid
            };

            card.Controls.Add(new Label
            {
                Text = caption,
                Location = new Point(14, 12),
                Size = new Size(240, 16),
                ForeColor = Color.FromArgb(90, 107, 125),
                Font = new Font("default", 8F, FontStyle.Bold)
            });

            valueLabel = new Label
            {
                Text = "—",
                Location = new Point(14, 34),
                Size = new Size(240, 36),
                ForeColor = color,
                Font = new Font("monospace", 18F, FontStyle.Bold)
            };
            card.Controls.Add(valueLabel);

            return card;
        }

        /// <summary>
        /// Puts the prepared series on the chart in one go. The lesson writes this as
        /// <c>chartLoad.DataSource = snapshot.ChartRows</c>; the ChartJS control takes
        /// <c>Labels</c> plus <c>DataSets</c>, so that is what this does — still one assignment of
        /// values that were prepared in the service.
        /// </summary>
        private void ApplyChart(IReadOnlyList<ChartRow> rows)
        {
            var colors = Enumerable.Repeat(OpenColor, rows.Count).ToArray();

            chartLoad.Labels = rows.Select(r => r.Label).ToArray();
            chartLoad.DataSets.Clear();
            chartLoad.DataSets.Add(new BarDataSet
            {
                Label = "Tickets updated",
                Data = rows.Select(r => (object)r.Count).ToArray(),
                BackgroundColor = colors,
                BorderColor = colors,
                BorderWidth = 1
            });
            chartLoad.UpdateData(0);
        }

        private void ConfigureChart()
        {
            chartLoad.Options.Legend.Display = true;
            chartLoad.Options.Legend.Position = HeaderPosition.Top;
            chartLoad.Options.Tooltips.Enabled = true;
        }

        #endregion
    }
}
