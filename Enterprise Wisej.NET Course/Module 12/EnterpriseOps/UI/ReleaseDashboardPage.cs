using EnterpriseOps.Domain;
using EnterpriseOps.Services;
using System;
using System.Linq;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Release dashboard: Deploy… / Rollback…, the eight-chip runbook strip and the node grid
    /// fed by HealthCheck.json.
    ///
    /// This file owns UI state only (chips, grid, labels, colours). Every decision — who may release, whether a
    /// step passed, what the balancer does with a node — belongs to ReleaseService, LoadBalancerSimulator and
    /// SmokeTestService.
    /// </summary>
    public partial class ReleaseDashboardPage : Page
    {
        // Per-session services, wired in the constructor. Instance fields — never statics. (Process-wide facts —
        // the captured configuration and the health probe — are static on purpose: a node has one health.)
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly LoadBalancerSimulator _balancer;
        private readonly ReleaseService _release;

        private CommandContext _current;
        private DateTime _lastPush = DateTime.MinValue; // Application.Update is bounded to ≥ 250 ms

        public ReleaseDashboardPage()
        {
            InitializeComponent();

            _session = ServiceRegistry.CreateSessionContext();
            _trace = new ActivityTrace();
            _balancer = new LoadBalancerSimulator(_trace, HostConfiguration.NodeName, HostConfiguration.ReleaseVersion);
            _release = new ReleaseService(_trace, _balancer, new SmokeTestService(_trace), _session);
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        private void ReleaseDashboardPage_Load(object sender, EventArgs e)
        {
            lblRunbookTitle.Text = "RELEASE RUNBOOK — " + _release.Candidate.Version;
            ShowRunbook();
            RefreshNodes();
            lblStatusBar.Text = $"{HostConfiguration.NodeName} · {_release.Candidate.Version} · {HostConfiguration.EnvironmentName} — GET /healthz ready";
        }

        /// <summary>The service runs the eight runbook steps; this handler paints them.</summary>
        private async void btnDeploy_Click(object sender, EventArgs e)
        {
            BeginBusy();
            try
            {
                var result = await _release.DeployAsync(CurrentContext, ShowStep);
                ShowReleaseResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        /// <summary>Runbook step 8 — redeploy the named previous package and prove it with smoke tests.</summary>
        private async void btnRollback_Click(object sender, EventArgs e)
        {
            BeginBusy();
            try
            {
                var result = await _release.RollbackAsync(CurrentContext, ShowStep);
                ShowReleaseResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                EndBusy();
            }
        }

        #endregion

        #region Showing results

        /// <summary>One ReleaseProgress → one chip. Pushed to the browser at most every 250 ms.</summary>
        private void ShowStep(ReleaseProgress progress)
        {
            PaintChip(ChipFor(progress.StepNumber), progress.Key, progress.State);
            if (progress.Message != null)
                lblStatusBar.Text = $"step {progress.StepNumber} {progress.Key} — {progress.Message}";
            RefreshNodes();
            Push();
        }

        /// <summary>A finished release action: banner, status bar, node grid, toast.</summary>
        private void ShowReleaseResult(ReleaseResult result)
        {
            RefreshNodes();

            if (result.Succeeded)
            {
                ShowBanner(result.Message, success: true);
                lblStatusBar.Text = result.Message;
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            string text = string.Join(" ", result.Errors);
            if (result.RollbackArmed)
                text += $"  →  runbook step 8 is armed: press Rollback to redeploy {_release.Previous.Version} on app-node-B.";

            ShowBanner(text, success: false);
            lblStatusBar.Text = $"release halted — ref {result.CorrelationId}";
            AlertBox.Show("The release stopped. The dashboard shows which step failed and what to do next.",
                MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>Re-probes every node, repaints the grid and shows the /healthz body of an unhealthy node.</summary>
        private void RefreshNodes()
        {
            _balancer.RefreshHealth();
            dgvNodes.DataSource = new BindingSource { DataSource = _balancer.ToRows() };
            PaintNodeRows();

            var unhealthy = _balancer.Nodes.Select(n => n.LastReport).FirstOrDefault(r => r != null && !r.IsHealthy);
            lblHealthJson.Visible = unhealthy != null;
            if (unhealthy != null)
                lblHealthJson.Text = unhealthy.ToSummaryLine();
        }

        private void PaintNodeRows()
        {
            for (int i = 0; i < dgvNodes.Rows.Count && i < _balancer.Nodes.Count; i++)
            {
                bool sick = !_balancer.Nodes[i].IsHealthy;
                dgvNodes.Rows[i].DefaultCellStyle.BackColor = sick ? System.Drawing.Color.FromArgb(253, 243, 243) : System.Drawing.Color.White;
                dgvNodes.Rows[i].DefaultCellStyle.ForeColor = sick ? System.Drawing.Color.FromArgb(156, 47, 47) : System.Drawing.Color.FromArgb(31, 45, 58);
            }
        }

        private void ShowRunbook()
        {
            foreach (var step in _release.Steps)
                PaintChip(ChipFor(step.Number), step.Key, step.State);
        }

        #endregion

        #region Helpers

        private Label ChipFor(int stepNumber)
        {
            switch (stepNumber)
            {
                case 1: return lblStep1;
                case 2: return lblStep2;
                case 3: return lblStep3;
                case 4: return lblStep4;
                case 5: return lblStep5;
                case 6: return lblStep6;
                case 7: return lblStep7;
                default: return lblStep8;
            }
        }

        /// <summary>· pending  … running  ✓ done  ✕ failed  ↩ rolled back.</summary>
        private static void PaintChip(Label chip, string key, RunbookStepState state)
        {
            string glyph;
            System.Drawing.Color back, fore;
            switch (state)
            {
                case RunbookStepState.Running:
                    glyph = "…"; back = System.Drawing.Color.FromArgb(255, 244, 229); fore = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                case RunbookStepState.Done:
                    glyph = "✓"; back = System.Drawing.Color.FromArgb(240, 249, 243); fore = System.Drawing.Color.FromArgb(21, 95, 51);
                    break;
                case RunbookStepState.Failed:
                    glyph = "✕"; back = System.Drawing.Color.FromArgb(253, 236, 234); fore = System.Drawing.Color.FromArgb(156, 47, 47);
                    break;
                case RunbookStepState.RolledBack:
                    glyph = "↩"; back = System.Drawing.Color.FromArgb(255, 248, 236); fore = System.Drawing.Color.FromArgb(122, 82, 16);
                    break;
                default:
                    glyph = "·"; back = System.Drawing.Color.FromArgb(244, 247, 250); fore = System.Drawing.Color.FromArgb(138, 151, 164);
                    break;
            }
            chip.Text = glyph + " " + key;
            chip.BackColor = back;
            chip.ForeColor = fore;
        }

        private CommandContext NewCommand()
        {
            _current = _session.NewCommand();
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, banner cleared.</summary>
        private void BeginBusy()
        {
            NewCommand();
            SetButtons(false);
            lblBanner.Visible = false;
        }

        /// <summary>After the awaits: buttons back on, push the pending changes to the browser.</summary>
        private void EndBusy()
        {
            SetButtons(true);
            _lastPush = DateTime.MinValue;
            Push();
        }

        /// <summary>Bounded server push: never more than one update every 250 ms while a release runs.</summary>
        private void Push()
        {
            if (IsDisposed)
                return;
            if ((DateTime.UtcNow - _lastPush).TotalMilliseconds < 250)
                return;
            _lastPush = DateTime.UtcNow;
            try
            {
                Application.Update(this);
            }
            catch (ObjectDisposedException)
            {
                // the operator closed the browser mid-release; the service run finishes and unwinds.
            }
        }

        private void SetButtons(bool enabled)
        {
            // Deploy and Rollback are not hidden for a Technician: the service refuses them server-side
            // (ReleaseAuthorization), which is the only place a permission can be enforced.
            btnDeploy.Enabled = enabled;
            btnRollback.Enabled = enabled;
        }

        private void ShowBanner(string text, bool success)
        {
            lblBanner.Text = text;
            lblBanner.BackColor = success ? System.Drawing.Color.FromArgb(233, 247, 238) : System.Drawing.Color.FromArgb(253, 236, 234);
            lblBanner.ForeColor = success ? System.Drawing.Color.FromArgb(15, 122, 58) : System.Drawing.Color.FromArgb(178, 59, 39);
            lblBanner.Visible = true;
        }

        /// <summary>Unexpected failure: log it with the correlation id, tell the operator something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} — corr {CurrentContext.CorrelationId}: {ex.Message}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", success: false);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
