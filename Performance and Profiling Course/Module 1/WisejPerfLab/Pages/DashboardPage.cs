using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
    /// This is the scenario the whole course starts from, written the way it is usually written first.
    /// Everything the refresh does happens in the handler: load the recent tickets as entities, format
    /// every row, count, throw the KPI controls away and build new ones, then rebuild the chart.
    /// Module 1 only measures it. Module 2 profiles it. Module 3 fixes it.
    /// </remarks>
    public partial class DashboardPage : UserControl, IScenarioPage
    {
        private static readonly Color OpenColor = Color.FromArgb(21, 101, 216);
        private static readonly Color OverdueColor = Color.FromArgb(224, 90, 59);
        private static readonly Color AgeColor = Color.FromArgb(125, 90, 224);

        private readonly IPerfLabShell _shell;
        private readonly ScenarioProbe _probe;
        private readonly DashboardService _dashboard;

        public DashboardPage(IPerfLabShell shell)
        {
            InitializeComponent();

            _shell = shell;
            _probe = PerfLabServices.Get<ScenarioProbe>();
            _dashboard = PerfLabServices.Get<DashboardService>();

            ConfigureChart();
            RebuildKpiPanel(0, 0, 0);
        }

        public string Scenario => "Dashboard";

        public string UserAction => "Refresh";

        /// <summary>The shell runs the scenario through exactly the handler the button runs.</summary>
        public void RunScenario() => RunRefresh(fromShell: true);

        private void btnRefresh_Click(object sender, EventArgs e) => RunRefresh(fromShell: false);

        #region The measured scenario

        private void RunRefresh(bool fromShell)
        {
            // The scope goes around the whole user action: the query, the formatting and the control work.
            // A scope around the query alone would have reported 90 ms for a refresh the user waits over a
            // second for.
            using var scope = _probe.Measure(Scenario, UserAction);

            btnRefresh.Enabled = false;
            if (!fromShell)
                _shell.ClearBanner();

            try
            {
                var tickets = _dashboard.LoadAllTickets();
                scope.Rows = tickets.Count;

                var now = DateTime.Now;
                var open = 0;
                var overdue = 0;
                var totalAgeHours = 0d;

                // One formatted row object per ticket, on every refresh. The KPIs need three numbers;
                // this loop builds five strings per row to get them.
                var rows = new List<TicketDisplayRow>(tickets.Count);
                foreach (var ticket in tickets)
                {
                    var row = TicketFormatter.FormatRow(ticket, string.Empty, now);
                    rows.Add(row);

                    if (ticket.Status == "Open")
                        open++;

                    var ageHours = (now - ticket.CreatedAt).TotalHours;
                    totalAgeHours += ageHours;

                    if (ticket.Status != "Closed" && ageHours > 48)
                        overdue++;
                }

                var averageAge = rows.Count == 0 ? 0 : totalAgeHours / rows.Count;

                RebuildKpiPanel(open, overdue, averageAge);
                RebuildChart(tickets.Select(t => t.UpdatedAt).ToList());

                var elapsed = scope.ElapsedMs;
                lblElapsed.Text = $"Refresh {PerfBudget.Describe(Scenario, UserAction, elapsed)}   rows={rows.Count:N0}";
                _shell.SetStatus(PerfBudget.StateFor(Scenario, UserAction, elapsed),
                    $"Dashboard/Refresh {PerfBudget.Describe(Scenario, UserAction, elapsed)}");
            }
            catch (DatabaseUnavailableException ex)
            {
                // The failure path still produces a measurement: the probe writes its end record with the
                // time spent before the failure, and the screen says what happened.
                scope.Fail(ex);
                lblElapsed.Text = "refresh failed after " + scope.ElapsedMs + " ms";
                _shell.SetStatus(ShellState.Fault, "Dashboard/Refresh failed — the ticket database is unreachable");
                _shell.ShowBanner("Dashboard refresh failed: the ticket database is unreachable. The numbers on screen are from the last successful refresh.");
            }
            finally
            {
                btnRefresh.Enabled = true;
            }
        }

        #endregion

        #region Control work

        /// <summary>
        /// Throws the KPI cards away and builds new ones. Construction, layout and theming run again on
        /// every refresh, for three numbers that could have been assigned to three existing labels.
        /// </summary>
        private void RebuildKpiPanel(int open, int overdue, double averageAgeHours)
        {
            foreach (Control existing in panelKpis.Controls.Cast<Control>().ToList())
            {
                panelKpis.Controls.Remove(existing);
                existing.Dispose();
            }

            panelKpis.Controls.Add(BuildKpiCard("OPEN", TicketFormatter.FormatCount(open), OpenColor, 0));
            panelKpis.Controls.Add(BuildKpiCard("OVERDUE", TicketFormatter.FormatCount(overdue), OverdueColor, 284));
            panelKpis.Controls.Add(BuildKpiCard("AVG AGE", TicketFormatter.FormatAge(TimeSpan.FromHours(averageAgeHours)), AgeColor, 568));
        }

        private static Panel BuildKpiCard(string caption, string value, Color color, int x)
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

            card.Controls.Add(new Label
            {
                Text = value,
                Location = new Point(14, 34),
                Size = new Size(240, 36),
                ForeColor = color,
                Font = new Font("monospace", 18F, FontStyle.Bold)
            });

            return card;
        }

        /// <summary>Tickets updated per day over the last two weeks, as one bar series.</summary>
        private void RebuildChart(IReadOnlyList<DateTime> updatedAt)
        {
            var today = DateTime.Today;
            var labels = new List<string>();
            var counts = new List<object>();

            for (var offset = 13; offset >= 0; offset--)
            {
                var day = today.AddDays(-offset);
                labels.Add(day.ToString("dd MMM", CultureInfo.CurrentCulture));
                counts.Add(updatedAt.Count(u => u.Date == day));
            }

            var colors = Enumerable.Repeat(OpenColor, counts.Count).ToArray();

            chartLoad.Labels = labels.ToArray();
            chartLoad.DataSets.Clear();
            chartLoad.DataSets.Add(new BarDataSet
            {
                Label = "Tickets updated",
                Data = counts.ToArray(),
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
