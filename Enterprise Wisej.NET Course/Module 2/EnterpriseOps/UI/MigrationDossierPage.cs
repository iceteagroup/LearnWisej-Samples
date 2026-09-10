using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Migration Dossier. The screen the walkthrough builds: the dossier for the Intermediate
    /// TicketOps Console extended into the Advanced EnterpriseOps baseline.
    ///
    /// Left card:   Build dossier · Run migration path · Cancel, then six tabs —
    ///              Dossier (area · current · target · risk · regression proof · rollback · state),
    ///              Inventory (current state → target state), Compatibility / risk matrix,
    ///              Incremental path (seven steps, each with its check and its fallback point),
    ///              Regression plan (the ten key flows) and the Decision memo.
    ///              Below them the banner and the dark footer that prints the harness verdict.
    /// Right card:  the live activity trace — UI → / Service: / Data: / Security: / Job:.
    /// Bottom bar:  the recoveries (roll back the failed step, map the theme mixin), the permission failure path
    ///              (map as ben.tech), navigation to the migrated WorkOrdersPage, the memo, Reset and Clear trace.
    ///
    /// The failure path the video shows: the app compiles and runs, but step 4 "Check themes" fails the visual
    /// diff — accent color lost · corner radius 0 · priority colors dropped — so the path halts at the fallback
    /// point "theme folder copy". Roll back, map the mixin, re-run: 10/10.
    ///
    /// The boundary: this file owns UI state only (grids, labels, banner, buttons). Every decision — the risk of a
    /// row, whether a step passed, whether a rollback is allowed, what the memo says — is made in
    /// EnterpriseOps.Services. Every handler is a few lines and calls one service.
    /// </summary>
    public partial class MigrationDossierPage : Page
    {
        // Per-session services, handed in by Program.Main. Instance fields — never statics.
        private readonly ServiceRegistry _services;
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;

        // The token source lives in an instance field so Cancel can reach a run that is already awaiting.
        private CancellationTokenSource _cancellation;
        private readonly Stopwatch _sincePush = Stopwatch.StartNew();
        private CommandContext _current;

        /// <summary>Designer / default constructor: a standalone session so the screen opens in the Wisej.NET Designer.</summary>
        public MigrationDossierPage()
            : this(NewStandaloneRegistry())
        {
        }

        public MigrationDossierPage(ServiceRegistry services)
        {
            InitializeComponent();

            _services = services;
            _session = services.Session;
            _trace = services.Trace;
        }

        /// <summary>The command running right now: same tenant and user, one correlation id per click.</summary>
        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each (the shape the lab code check expects)

        private void MigrationDossierPage_Load(object sender, EventArgs e)
        {
            _trace.Attach(AppendTrace);
            _trace.Ui($"MigrationDossierPage_Load → session {_session}, trace buffer {_trace.Lines.Count} line(s) replayed");

            ShowSignedIn();
            lblTenant.Text = "tenant: " + _session.TenantId;

            // The plan exists before anything runs: the seven steps and the ten flows are bound on load.
            BindSteps();
            BindFlows();
            SetStatus("dossier not built — click Build dossier", StatusKind.Warn);
        }

        /// <summary>Success path: the assessment service builds the dossier and computes the risk of every row.</summary>
        private async void btnBuildDossier_Click(object sender, EventArgs e)
        {
            BeginBusy("building the dossier…");
            try
            {
                var result = await _services.Assessment.AssessAsync(CurrentContext);
                ShowAssessment(result);
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

        /// <summary>
        /// Progress path + the video's failure path: seven verifiable steps. The workflow decides; this handler
        /// only shows what came back. Step 4 fails on the unmapped theme and the path halts at its fallback point.
        /// </summary>
        private async void btnRunPath_Click(object sender, EventArgs e)
        {
            BeginBusy("running the migration path…");
            btnCancel.Enabled = true;
            _cancellation = new CancellationTokenSource();
            try
            {
                var result = await _services.Workflow.RunAsync(CurrentContext, OnStepProgress, OnFlowProgress, _cancellation.Token);
                ShowWorkflowResult(result);
            }
            catch (OperationCanceledException)
            {
                ShowBanner("Migration path cancelled — the steps that already passed keep their state.", BannerKind.Warning);
                SetStatus("cancelled", StatusKind.Warn);
                _trace.Ui("btnRunPath_Click → cancelled through the CancellationTokenSource; no step was marked passed while it was running");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                btnCancel.Enabled = false;
                _cancellation?.Dispose();
                _cancellation = null;
                BindSteps();
                BindFlows();
                BindDossier();
                EndBusy();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnCancel_Click → CancellationTokenSource.Cancel()");
            _cancellation?.Cancel();
        }

        /// <summary>Recovery 1: roll the failed step back to its fallback point. The workflow checks the permission.</summary>
        private async void btnRollback_Click(object sender, EventArgs e)
        {
            BeginBusy("rolling back…");
            try
            {
                var result = await _services.Workflow.RollbackAsync(CurrentContext);
                ShowCommandResult(result, "rolled back");
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                BindSteps();
                BindDossier();
                EndBusy();
            }
        }

        /// <summary>Recovery 2: resource mapping — the 3.x theme ported to a 4.x mixin. Refused on a broken build.</summary>
        private void btnMapTheme_Click(object sender, EventArgs e)
        {
            try
            {
                var result = _services.Workflow.MapThemeMixin(CurrentContext);
                ShowCommandResult(result, "theme mapped");
                ShowThemeFooter();
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Failure path: a Technician asks for the same mapping. PermissionService denies it server-side.</summary>
        private void btnSwitchUser_Click(object sender, EventArgs e)
        {
            bool backToManager = _session.Role == Role.Technician;
            _session.SwitchUser(backToManager ? "ana.ops" : "ben.tech", backToManager ? Role.Manager : Role.Technician);
            ShowSignedIn();

            if (backToManager)
            {
                _trace.Ui($"btnSwitchUser_Click → SessionContext.SwitchUser({_session.UserName} · {_session.Role}) — back on the manager, nothing else attempted");
                SetStatus($"signed in as {_session.UserName}", StatusKind.Ok);
                HideBanner();
                return;
            }

            _trace.Ui($"btnSwitchUser_Click → SessionContext.SwitchUser({_session.UserName} · {_session.Role}), then Map theme mixin");
            btnMapTheme_Click(sender, e);
        }

        /// <summary>Navigation: the migrated TicketOps screen, painted from the theme map the migration left behind.</summary>
        private void btnWorkOrders_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnWorkOrders_Click → Application.MainPage = WorkOrdersPage (same ServiceRegistry, same SessionContext — flow 9 walks this path)");
            _trace.Detach();
            Application.MainPage = new WorkOrdersPage(_services);
        }

        /// <summary>The modernization decision memo, written by the service from the evidence the run produced.</summary>
        private void btnMemo_Click(object sender, EventArgs e)
        {
            try
            {
                txtMemo.Text = _services.Assessment.BuildDecisionMemo(
                    CurrentContext, _services.Workflow.Steps, _services.Workflow.LastHarness, _services.Theme);
                tabDossier.SelectedIndex = tabDossier.TabPages.IndexOf(tabPageMemo);
                SetStatus("decision memo written from evidence", StatusKind.Ok);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>Back to the state right after the package upgrade, so the whole path can be replayed.</summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            _trace.Ui("btnReset_Click → MigrationWorkflow.Reset()");
            _services.Workflow.Reset();
            BindSteps();
            BindFlows();
            BindDossier();
            HideBanner();
            lblStatusBar.Text = "Regression harness: not run";
            SetStatus("reset — theme unmapped, 7 steps pending", StatusKind.Warn);
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            lstTrace.Items.Clear();
        }

        #endregion

        #region Progress observers — called by the workflow between awaits

        /// <summary>One step changed state: rebind the steps grid and push, at most every 250 ms.</summary>
        private void OnStepProgress(MigrationStep step)
        {
            BindSteps();
            lblStatusBar.Text = $"step {step.Number}/7 {step.Name} — {step.Detail}";
            Push();
        }

        /// <summary>One regression flow finished: rebind the flows grid and push, at most every 250 ms.</summary>
        private void OnFlowProgress(RegressionFlow flow)
        {
            BindFlows();
            Push();
        }

        /// <summary>Bounded push: the request that started the handler is long gone, so pending changes need Application.Update.</summary>
        private void Push()
        {
            if (_sincePush.ElapsedMilliseconds < 250)
                return;
            _sincePush.Restart();
            Application.Update(this);
        }

        #endregion

        #region Showing results — UI state only, no decisions

        /// <summary>The AssessmentResult → the dossier, inventory and compatibility grids plus the counts.</summary>
        private void ShowAssessment(AssessmentResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            BindDossier();
            dgvInventory.DataSource = new BindingSource { DataSource = new List<InventoryItem>(result.Inventory) };
            dgvCompatibility.DataSource = new BindingSource { DataSource = new List<CompatibilityEntry>(result.Compatibility) };
            PaintRiskColumn(dgvCompatibility, colMatrixRisk);

            tabDossier.SelectedIndex = tabDossier.TabPages.IndexOf(tabPageDossier);
            HideBanner();
            lblStatusBar.Text = $"Dossier: {result.Dossier.Count} areas · risk H×{result.HighCount} M×{result.MediumCount} L×{result.LowCount} · {result.BlockedCount} blocked combinations mitigated";
            SetStatus(result.Summary, StatusKind.Ok);
        }

        /// <summary>The WorkflowResult → the dark footer, the banner and the status. A failed step is never an exception.</summary>
        private void ShowWorkflowResult(WorkflowResult result)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Succeeded)
            {
                HideBanner();
                lblStatusBar.Text = result.Harness != null
                    ? $"Regression harness: {result.Harness.Verdict} · {result.Message}"
                    : result.Message;
                SetStatus($"{result.CompletedSteps}/7 steps passed", StatusKind.Ok);
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            var step = result.FailedStep;
            string fallback = step == null ? "" : $"  →  fall back to \"{step.FallbackPoint}\"";
            ShowBanner(result.Message + fallback, BannerKind.Error);
            lblStatusBar.Text = result.Harness != null
                ? $"Regression harness: {result.Harness.Verdict}"
                : result.Message;
            SetStatus(step == null ? "refused" : $"step {step.Number} failed — {step.Name}", StatusKind.Error);
            tabDossier.SelectedIndex = tabDossier.TabPages.IndexOf(result.Harness == null ? tabPageSteps : tabPageFlows);
            AlertBox.Show("The migration path stopped at a failed check. Roll back to its fallback point.",
                MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>A typed CommandResult → green banner or red banner. Errors are messages, never exceptions.</summary>
        private void ShowCommandResult(CommandResult result, string okStatus)
        {
            lblCorrelation.Text = "corr " + result.CorrelationId;

            if (result.Succeeded)
            {
                ShowBanner(result.Message, BannerKind.Success);
                SetStatus(okStatus, StatusKind.Ok);
                return;
            }

            ShowBanner(result.ErrorText, BannerKind.Warning);
            SetStatus("refused — see the banner", StatusKind.Warn);
        }

        /// <summary>The footer after a theme move: how far the current theme map is from the 3.5 baseline.</summary>
        private void ShowThemeFooter()
        {
            var diff = _services.Theme.Diff();
            lblStatusBar.Text = diff.Count == 0
                ? $"Theme: {_services.Theme.Current.Name} — 0 differences from the 3.5 baseline"
                : $"Theme: {_services.Theme.Current.Name} — {diff.Count} difference(s): {string.Join(" · ", diff)}";
        }

        /// <summary>Unexpected failure: keep the details in the trace, tell the user something generic with the correlation id.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} (ref {CurrentContext.CorrelationId}) — {ex.Message}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
            SetStatus("failed — see the log", StatusKind.Error);
            AlertBox.Show("The action could not be completed. Check the log for details.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion

        #region Binding

        private void BindDossier()
        {
            dgvDossier.DataSource = new BindingSource { DataSource = new List<DossierRow>(_services.Assessment.Dossier) };
            PaintRiskColumn(dgvDossier, colRisk);
            PaintStateColumn(dgvDossier, colRowState);
        }

        private void BindSteps()
        {
            dgvSteps.DataSource = new BindingSource { DataSource = new List<MigrationStep>(_services.Workflow.Steps) };
            for (int i = 0; i < dgvSteps.Rows.Count && i < _services.Workflow.Steps.Count; i++)
                PaintCell(dgvSteps.Rows[i], colStepState, StepColor(_services.Workflow.Steps[i].State));
        }

        private void BindFlows()
        {
            dgvFlows.DataSource = new BindingSource { DataSource = new List<RegressionFlow>(_services.Harness.Flows) };
            for (int i = 0; i < dgvFlows.Rows.Count && i < _services.Harness.Flows.Count; i++)
                PaintCell(dgvFlows.Rows[i], colFlowResult, OutcomeColor(_services.Harness.Flows[i].Outcome));
        }

        /// <summary>H red, M amber, L green — the dossier's own colour code, read back from the bound row.</summary>
        private static void PaintRiskColumn(DataGridView grid, DataGridViewColumn column)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                object value = row.Cells[column]?.Value;
                PaintCell(row, column, (value as string) switch
                {
                    "H" => System.Drawing.Color.FromArgb(192, 57, 43),
                    "M" => System.Drawing.Color.FromArgb(185, 119, 14),
                    _ => System.Drawing.Color.FromArgb(31, 138, 76),
                });
            }
        }

        private static void PaintStateColumn(DataGridView grid, DataGridViewColumn column)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                string text = row.Cells[column]?.Value as string ?? "";
                PaintCell(row, column,
                    text.StartsWith("✓") ? System.Drawing.Color.FromArgb(31, 138, 76) :
                    text.StartsWith("✕") ? System.Drawing.Color.FromArgb(192, 57, 43) :
                    text.StartsWith("↶") ? System.Drawing.Color.FromArgb(185, 119, 14) :
                    System.Drawing.Color.FromArgb(90, 107, 125));
            }
        }

        private static void PaintCell(DataGridViewRow row, DataGridViewColumn column, System.Drawing.Color color)
        {
            var cell = row.Cells[column];
            if (cell == null)
                return;
            var style = cell.Style ?? new DataGridViewCellStyle();
            style.ForeColor = color;
            cell.Style = style;
        }

        private static System.Drawing.Color StepColor(StepState state)
        {
            return state switch
            {
                StepState.Passed => System.Drawing.Color.FromArgb(31, 138, 76),
                StepState.Failed => System.Drawing.Color.FromArgb(192, 57, 43),
                StepState.RolledBack => System.Drawing.Color.FromArgb(185, 119, 14),
                StepState.Running => System.Drawing.Color.FromArgb(21, 101, 216),
                _ => System.Drawing.Color.FromArgb(140, 155, 170),
            };
        }

        private static System.Drawing.Color OutcomeColor(FlowOutcome outcome)
        {
            return outcome switch
            {
                FlowOutcome.Pass => System.Drawing.Color.FromArgb(31, 138, 76),
                FlowOutcome.Fail => System.Drawing.Color.FromArgb(192, 57, 43),
                _ => System.Drawing.Color.FromArgb(140, 155, 170),
            };
        }

        #endregion

        #region Helpers (small, reusable)

        private enum StatusKind { Ok, Warn, Error }
        private enum BannerKind { Success, Warning, Error }

        /// <summary>A standalone per-session registry for the Designer and for a page created without Program.Main.</summary>
        private static ServiceRegistry NewStandaloneRegistry()
        {
            var session = new SessionContext("contoso", "ana.ops", Role.Manager);
            return new ServiceRegistry(session, () => session);
        }

        private CommandContext NewCommand()
        {
            _current = CommandContext.From(_session);
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off, amber status. Sent before the first await.</summary>
        private void BeginBusy(string text)
        {
            NewCommand();
            SetButtons(false);
            SetStatus(text, StatusKind.Warn);
        }

        /// <summary>After the awaits: buttons back on, push the pending changes.</summary>
        private void EndBusy()
        {
            SetButtons(true);
            _sincePush.Restart();
            Application.Update(this);
        }

        private void SetButtons(bool enabled)
        {
            btnBuildDossier.Enabled = enabled;
            btnRunPath.Enabled = enabled;
            btnRollback.Enabled = enabled;
            btnMapTheme.Enabled = enabled;
            btnSwitchUser.Enabled = enabled;
            btnWorkOrders.Enabled = enabled;
            btnMemo.Enabled = enabled;
            btnReset.Enabled = enabled;
        }

        private void ShowSignedIn()
        {
            lblUser.Text = $"Signed in: {_session.UserName} · {_session.Role}";
            btnSwitchUser.Text = _session.Role == Role.Technician
                ? "Back to ana.ops (Manager)"
                : "Fail: map theme as ben.tech";
        }

        private void SetStatus(string text, StatusKind kind)
        {
            lblStatus.Text = "● " + text;
            lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(224, 86, 59),
                StatusKind.Warn => System.Drawing.Color.FromArgb(232, 161, 60),
                _ => System.Drawing.Color.FromArgb(31, 157, 87),
            };
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

        private void HideBanner()
        {
            lblBanner.Visible = false;
        }

        /// <summary>The trace sink — the only place on this page that knows about lstTrace.</summary>
        private void AppendTrace(string line)
        {
            lstTrace.Items.Add(line);
            lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        #endregion
    }
}
