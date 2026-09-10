using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using System;
using System.Linq;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Release dashboard (Module 12: cloud, containers, load balancing and release engineering).
    ///
    /// Left card:   the release title with Deploy… / Rollback…, the eight-chip runbook strip, the node grid
    ///              fed by HealthCheck.json, the body GET /healthz would return, and the environment
    ///              configuration table read from the real appsettings files.
    /// Right card:  the live activity trace — Host: / Security: / Service: / Data: / Health: / Balancer: / Runbook:.
    /// Bottom bar:  run the smoke tests, the three failure paths (health check, no affinity, missing secret)
    ///              and Clear trace. The recovery for a failed release is Rollback — runbook step 8.
    ///
    /// The boundary: this file owns UI state only (chips, grids, labels, colours). Every decision — who may
    /// release, whether a step passed, what the balancer does with a node, whether an environment would even
    /// boot — belongs to ReleaseService, LoadBalancerSimulator, SmokeTestService, ConfigurationService and
    /// StartupValidation. That is why every handler below is a few lines and calls a service.
    /// </summary>
    public partial class ReleaseDashboardPage : Page
    {
        // Per-session services, wired in the constructor. Instance fields — never statics: two operators
        // must never share a trace, a release run or a balancer view. (Process-wide facts — the captured
        // configuration and the health probe — are static on purpose: a node has one health, whoever asks.)
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly ConfigurationService _configuration;
        private readonly LoadBalancerSimulator _balancer;
        private readonly SmokeTestService _smokeTests;
        private readonly ReleaseService _release;

        private CommandContext _current;
        private bool _secretsInjected;                  // the platform's vault, simulated for the preview
        private DateTime _lastPush = DateTime.MinValue; // Application.Update is bounded to ≥ 250 ms

        public ReleaseDashboardPage()
        {
            InitializeComponent();

            _session = ServiceRegistry.CreateSessionContext();
            _trace = new ActivityTrace();
            _trace.EntryAdded += trace_EntryAdded;

            _configuration = new ConfigurationService(_trace);
            _balancer = new LoadBalancerSimulator(_trace, HostConfiguration.NodeName, HostConfiguration.ReleaseVersion);
            _smokeTests = new SmokeTestService(_trace);
            _release = new ReleaseService(_trace, _balancer, _smokeTests, _session);
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        private void ReleaseDashboardPage_Load(object sender, EventArgs e)
        {
            _trace.Ui($"{Name}_Load → session {_session.SessionId} pinned to {HostConfiguration.NodeName}");
            _trace.Host($"Startup.cs captured environment={HostConfiguration.EnvironmentName} · release={HostConfiguration.ReleaseVersion} · previous={HostConfiguration.PreviousVersion}");
            _trace.Health($"HealthCheck.json loaded from {HealthProbeService.ContractPath} — {HealthProbeService.Contract.Checks.Count} checks, probe every {HealthProbeService.Contract.Probe.IntervalSeconds}s");
            _trace.Security(ReleaseAuthorization.Explain(_session, "deploy"));

            lblReleaseTitle.Text = "Release " + _release.Candidate.Version;
            lblEnvironment.Text = $"env {HostConfiguration.EnvironmentName} · node {HostConfiguration.NodeName}";
            ShowSignedIn();

            LoadEnvironments();
            ShowRunbook();
            RefreshNodes();
            ShowConfigurationTable();
            SetStatus("ready", StatusKind.Ok);
            lblStatusBar.Text = $"{HostConfiguration.NodeName} · {_release.Candidate.Version} · {HostConfiguration.EnvironmentName} — GET /healthz ready";
        }

        /// <summary>Success + progress path: the service runs the eight runbook steps, this handler paints them.</summary>
        private async void btnDeploy_Click(object sender, EventArgs e)
        {
            BeginBusy("deploying…");
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

        /// <summary>Recovery path: runbook step 8 — redeploy the named previous package and prove it with smoke tests.</summary>
        private async void btnRollback_Click(object sender, EventArgs e)
        {
            BeginBusy("rolling back…");
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

        /// <summary>The smoke test checklist, run in-process against this node.</summary>
        private async void btnSmokeTests_Click(object sender, EventArgs e)
        {
            BeginBusy("smoke tests…");
            try
            {
                var run = await _smokeTests.RunAsync(CurrentContext, _balancer.NodeA, _session.SessionId);
                ShowSmokeRun(run);
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

        /// <summary>Failure path 1: arm the 2.4.2 migration fault, then run the very same deploy.</summary>
        private void btnFailHealth_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnFailHealth_Click → ReleaseService.InjectMigrationFault(), then the normal Deploy");
            _release.InjectMigrationFault();
            btnDeploy_Click(sender, e);
        }

        /// <summary>Failure path 2 / recovery 2: sticky sessions off, route one request, then back on.</summary>
        private void btnAffinity_Click(object sender, EventArgs e)
        {
            _trace.Ui($"btnAffinity_Click → LoadBalancerSimulator.SetAffinity({(!_balancer.AffinityEnabled).ToString().ToLowerInvariant()}), then route this session's next request");
            _balancer.SetAffinity(!_balancer.AffinityEnabled);
            var decision = _balancer.Route(_session.SessionId, HostConfiguration.NodeName);
            ShowRoutingDecision(decision);
        }

        /// <summary>Recovery 3: the platform hands the node its secrets, and the same preview passes.</summary>
        private void btnInjectSecrets_Click(object sender, EventArgs e)
        {
            _secretsInjected = !_secretsInjected;
            _trace.Ui($"btnInjectSecrets_Click → preview {SelectedEnvironment} with platform secrets {(_secretsInjected ? "injected" : "removed")}");
            btnInjectSecrets.Text = _secretsInjected ? "Fail: remove platform secrets" : "Recover: inject platform secrets";
            ShowEnvironmentPreview();
        }

        /// <summary>Failure path 3: preview a boot of another environment with only what the repository contains.</summary>
        private void cboEnvironment_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowEnvironmentPreview();
        }

        /// <summary>
        /// Failure path 4 / recovery 4: release as a Technician. Deploy and Rollback stay clickable —
        /// the SERVICE refuses, which is the only place a permission can be enforced.
        /// </summary>
        private void btnSwitchUser_Click(object sender, EventArgs e)
        {
            bool toTechnician = _session.Role != "Technician";
            _session.User = toTechnician ? "ben.tech" : "ana.ops";
            _session.Role = toTechnician ? "Technician" : "Manager";
            _trace.Ui($"btnSwitchUser_Click → session now {_session.User} ({_session.Role}); the buttons do NOT change");
            _trace.Security(ReleaseAuthorization.Explain(_session, "deploy"));
            ShowSignedIn();
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
        }

        #endregion

        #region Showing results — UI state only, no decisions

        /// <summary>One ReleaseProgress → one chip. Pushed to the browser at most every 250 ms.</summary>
        private void ShowStep(ReleaseProgress progress)
        {
            PaintChip(ChipFor(progress.StepNumber), progress.Key, progress.State);
            if (progress.Message != null)
                lblStatusBar.Text = $"step {progress.StepNumber} {progress.Key} — {progress.Message}";
            RefreshNodes();
            Push();
        }

        /// <summary>A finished release action: banner, status, footer, node grid, toast.</summary>
        private void ShowReleaseResult(ReleaseResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;
            RefreshNodes();

            if (result.Succeeded)
            {
                ShowBanner(result.Message, BannerKind.Success);
                SetStatus("release healthy", StatusKind.Ok);
                lblStatusBar.Text = result.Message;
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                _trace.UiResult("ShowReleaseResult: Succeeded=true → green banner, nodes repainted");
                return;
            }

            string text = string.Join(" ", result.Errors);
            if (result.RollbackArmed)
                text += $"  →  runbook step 8 is armed: press Rollback to redeploy {_release.Previous.Version} on app-node-B.";

            ShowBanner(text, BannerKind.Error);
            SetStatus(result.FailedStepKey == null ? "refused" : "halted at " + result.FailedStepKey, StatusKind.Error);
            lblStatusBar.Text = $"release halted — ref {result.CorrelationId}";
            AlertBox.Show("The release stopped. The dashboard shows which step failed and what to do next.",
                MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
            _trace.UiResult($"ShowReleaseResult: Succeeded=false → red banner, rollbackArmed={result.RollbackArmed}");
        }

        /// <summary>The checklist run: every check on its own trace line, the verdict on the banner.</summary>
        private void ShowSmokeRun(SmokeTestRun run)
        {
            lblCorrelation.Text = "corr " + run.CorrelationId;
            lblStatusBar.Text = run.Summary;
            ShowBanner(run.Summary, run.Passed ? BannerKind.Success : BannerKind.Error);
            SetStatus(run.Passed ? "smoke tests green" : $"{run.Failures} smoke test(s) failed", run.Passed ? StatusKind.Ok : StatusKind.Error);
            RefreshNodes();
            _trace.UiResult($"ShowSmokeRun: {run.Checks.Count} checks painted, blocking failures {(run.Passed ? 0 : run.Failures)}");
        }

        /// <summary>What the balancer did with the next request of THIS session.</summary>
        private void ShowRoutingDecision(RoutingDecision decision)
        {
            btnAffinity.Text = _balancer.AffinityEnabled ? "Fail: balancer without affinity" : "Recover: sticky sessions on";
            lblStatusBar.Text = $"balancer → {decision.NodeName} · affinity {(_balancer.AffinityEnabled ? "on" : "off")} · {decision.Message}";

            if (decision.SessionPreserved)
            {
                ShowBanner($"Affinity on — {decision.Message}. Server-side session state survives because every request comes back to the node that holds it.", BannerKind.Success);
                SetStatus("sessions pinned", StatusKind.Ok);
            }
            else
            {
                ShowBanner($"No affinity — {decision.Message}. This is why sticky sessions are a requirement, not an optimisation.", BannerKind.Error);
                SetStatus("session lost on the other node", StatusKind.Error);
            }
            _trace.UiResult($"ShowRoutingDecision: node {decision.NodeName}, sessionPreserved={decision.SessionPreserved.ToString().ToLowerInvariant()}");
        }

        /// <summary>The startup-validation preview for the selected environment — the missing-setting failure path.</summary>
        private void ShowEnvironmentPreview()
        {
            string environmentName = SelectedEnvironment;
            if (string.IsNullOrEmpty(environmentName))
                return;

            var preview = _configuration.Preview(environmentName, _secretsInjected);
            lblConfigTitle.Text = $"ENVIRONMENT CONFIGURATION — preview boot of {environmentName}";

            if (preview.WouldStart)
            {
                ShowBanner($"{environmentName}: startup validation passed ({preview.Validation.Passed.Count} rules) — the node would boot and join the balancer" +
                           (preview.SecretsInjected ? " · secrets supplied by the environment, not by a file." : "."), BannerKind.Success);
                SetStatus(environmentName.ToLowerInvariant() + " would start", StatusKind.Ok);
            }
            else
            {
                ShowBanner($"{environmentName}: {preview.Validation.Errors.Count} configuration error(s) — {preview.Validation.Errors[0]}. The host throws at startup, so a misconfigured node never joins the balancer.", BannerKind.Error);
                SetStatus(environmentName.ToLowerInvariant() + " would refuse to boot", StatusKind.Error);
            }
            lblStatusBar.Text = $"preview {environmentName} · secrets {(preview.SecretsInjected ? "injected" : "absent")} · {preview.Validation.Errors.Count} error(s), {preview.Validation.Passed.Count} passed";
        }

        /// <summary>Re-probes every node and repaints the grid and the /healthz line.</summary>
        private void RefreshNodes()
        {
            _balancer.RefreshHealth();
            dgvNodes.DataSource = new BindingSource { DataSource = _balancer.ToRows() };
            PaintNodeRows();

            var report = _balancer.Nodes.Select(n => n.LastReport).FirstOrDefault(r => r != null && !r.IsHealthy)
                         ?? _balancer.NodeA.LastReport;
            lblHealthJson.Text = report == null ? "GET /healthz → …" : "GET /healthz → " + report.ToSummaryLine();
            lblHealthJson.ForeColor = report != null && !report.IsHealthy
                ? System.Drawing.Color.FromArgb(255, 154, 168)
                : System.Drawing.Color.FromArgb(167, 233, 196);
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

        private void ShowConfigurationTable()
        {
            dgvConfig.DataSource = new BindingSource { DataSource = _configuration.BuildTable() };
        }

        private void ShowRunbook()
        {
            foreach (var step in _release.Steps)
                PaintChip(ChipFor(step.Number), step.Key, step.State);
        }

        #endregion

        #region Helpers (small, reusable)

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

        private void LoadEnvironments()
        {
            foreach (string environmentName in ConfigurationService.Environments)
                cboEnvironment.Items.Add(environmentName);

            int index = Array.IndexOf(ConfigurationService.Environments, HostConfiguration.EnvironmentName);
            cboEnvironment.SelectedIndex = index < 0 ? 0 : index;
        }

        /// <summary>The environment the configuration card is previewing (not necessarily the one this process booted with).</summary>
        private string SelectedEnvironment => cboEnvironment.SelectedItem == null ? null : cboEnvironment.SelectedItem.ToString();

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

        /// <summary>· pending  … running  ✓ done  ✕ failed  ↩ rolled back — the strip the walkthrough shows.</summary>
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
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, amber status. Sent before the first await.</summary>
        private void BeginBusy(string text)
        {
            NewCommand();
            SetButtons(false);
            SetStatus(text, StatusKind.Warn);
            HideBanner();
        }

        /// <summary>After the awaits: buttons back on, push the pending changes (the request that started the handler is long gone).</summary>
        private void EndBusy()
        {
            SetButtons(true);
            _lastPush = DateTime.MinValue;
            Push();
        }

        /// <summary>Who the session belongs to. The release buttons stay enabled for everyone on purpose.</summary>
        private void ShowSignedIn()
        {
            lblUser.Text = $"Signed in: {_session.User} · {_session.Role}";
            btnSwitchUser.Text = _session.Role == "Technician" ? "Sign in as ana.ops" : "Sign in as ben.tech";
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
            // Deploy and Rollback are NOT hidden or disabled for a Technician: the service refuses them
            // server-side (ReleaseAuthorization), which is the only place a permission can be enforced.
            btnDeploy.Enabled = enabled;
            btnRollback.Enabled = enabled;
            btnSmokeTests.Enabled = enabled;
            btnFailHealth.Enabled = enabled;
            btnAffinity.Enabled = enabled;
            btnInjectSecrets.Enabled = enabled;
            btnSwitchUser.Enabled = enabled;
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind == StatusKind.Error
                ? System.Drawing.Color.FromArgb(224, 86, 59)
                : kind == StatusKind.Warn
                    ? System.Drawing.Color.FromArgb(232, 161, 60)
                    : System.Drawing.Color.FromArgb(31, 157, 87);
        }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            switch (kind)
            {
                case BannerKind.Success:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(233, 247, 238);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(15, 122, 58);
                    break;
                case BannerKind.Warning:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(255, 244, 229);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
                    break;
                default:
                    lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
                    lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
                    break;
            }
            lblBanner.Visible = true;
        }

        private void HideBanner() => lblBanner.Visible = false;

        /// <summary>Unexpected failure: log it with the correlation id, tell the operator something generic, keep the screen usable.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} — corr {CurrentContext.CorrelationId} (details stay in the server log)");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>The trace sink: one line per layer decision; null clears. The only place that knows about lstTrace.</summary>
        private void trace_EntryAdded(string line)
        {
            if (line == null)
            {
                lstTrace.Items.Clear();
                return;
            }
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        #endregion
    }
}
