using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using EnterpriseOps.Domain;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Migration Dossier: the dossier for the Intermediate TicketOps Console extended into the
    /// Advanced EnterpriseOps baseline.
    ///
    /// Toolbar: Build dossier · Run migration path · Cancel · Roll back failed step · Map theme mixin · Work orders.
    /// Six tabs: Dossier (area · current · target · risk · regression proof · rollback · state), Inventory
    /// (current state → target state), Compatibility / risk matrix, Incremental path (seven steps, each with its
    /// check and its fallback point), Regression plan (the ten key flows) and the Decision memo. Below them the
    /// banner and the dark footer with the harness verdict.
    ///
    /// The failure path: the app compiles and runs, but step 4 "Check themes" fails the visual diff — accent color
    /// lost · corner radius 0 · priority colors dropped — so the path halts at the fallback point "theme folder
    /// copy". Roll back, map the mixin, re-run: 10/10.
    ///
    /// This file owns UI state only (grids, banner, footer, buttons). Every decision — the risk of a row, whether a
    /// step passed, whether a rollback is allowed, what the memo says — is made in EnterpriseOps.Services.
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

        #region Event handlers

        private void MigrationDossierPage_Load(object sender, EventArgs e)
        {
            // The plan exists before anything runs: the seven steps and the ten flows are bound on load.
            BindSteps();
            BindFlows();

            if (_services.Assessment.IsBuilt)
            {
                BindDossier();
                RefreshMemo();
            }
        }

        /// <summary>Success path: the assessment service builds the dossier and computes the risk of every row.</summary>
        private async void btnBuildDossier_Click(object sender, EventArgs e)
        {
            BeginBusy();
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
                RefreshMemo();
                EndBusy();
            }
        }

        /// <summary>
        /// Seven verifiable steps. The workflow decides; this handler only shows what came back. With the theme
        /// still unmapped, step 4 fails and the path halts at its fallback point.
        /// </summary>
        private async void btnRunPath_Click(object sender, EventArgs e)
        {
            BeginBusy();
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
                RefreshMemo();
                EndBusy();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cancellation?.Cancel();
        }

        /// <summary>Roll the failed step back to its fallback point. The workflow checks the permission.</summary>
        private async void btnRollback_Click(object sender, EventArgs e)
        {
            BeginBusy();
            try
            {
                var result = await _services.Workflow.RollbackAsync(CurrentContext);
                ShowCommandResult(result);
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
            finally
            {
                BindSteps();
                BindDossier();
                RefreshMemo();
                EndBusy();
            }
        }

        /// <summary>Resource mapping — the 3.x theme ported to a 4.x mixin. Refused on a broken build.</summary>
        private void btnMapTheme_Click(object sender, EventArgs e)
        {
            NewCommand();
            try
            {
                var result = _services.Workflow.MapThemeMixin(CurrentContext);
                ShowCommandResult(result);
                ShowThemeFooter();
                RefreshMemo();
            }
            catch (Exception ex)
            {
                ReportFailure(ex);
            }
        }

        /// <summary>The migrated TicketOps screen, painted from the theme map the migration left behind.</summary>
        private void btnWorkOrders_Click(object sender, EventArgs e)
        {
            Application.MainPage = new WorkOrdersPage(_services);
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
            BindDossier();
            dgvInventory.DataSource = new BindingSource { DataSource = new List<InventoryItem>(result.Inventory) };
            dgvCompatibility.DataSource = new BindingSource { DataSource = new List<CompatibilityEntry>(result.Compatibility) };
            PaintRiskColumn(dgvCompatibility, colMatrixRisk);

            tabDossier.SelectedIndex = tabDossier.TabPages.IndexOf(tabPageDossier);
            HideBanner();
            lblStatusBar.Text = $"Dossier: {result.Dossier.Count} areas · risk H×{result.HighCount} M×{result.MediumCount} L×{result.LowCount} · {result.BlockedCount} blocked combinations mitigated";
        }

        /// <summary>The WorkflowResult → the dark footer and the banner. A failed step is never an exception.</summary>
        private void ShowWorkflowResult(WorkflowResult result)
        {
            if (result.Succeeded)
            {
                HideBanner();
                lblStatusBar.Text = result.Harness != null
                    ? $"Regression harness: {result.Harness.Verdict} · {result.Message}"
                    : result.Message;
                AlertBox.Show(result.Message, MessageBoxIcon.Information, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                return;
            }

            var step = result.FailedStep;
            string fallback = step == null ? "" : $"  →  fall back to \"{step.FallbackPoint}\"";
            ShowBanner(result.Message + fallback, BannerKind.Error);
            lblStatusBar.Text = result.Harness != null
                ? $"Regression harness: {result.Harness.Verdict}"
                : result.Message;
            tabDossier.SelectedIndex = tabDossier.TabPages.IndexOf(result.Harness == null ? tabPageSteps : tabPageFlows);
            AlertBox.Show("The migration path stopped at a failed check. Roll back to its fallback point.",
                MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        /// <summary>A typed CommandResult → green banner or amber banner. Errors are messages, never exceptions.</summary>
        private void ShowCommandResult(CommandResult result)
        {
            if (result.Succeeded)
                ShowBanner(result.Message, BannerKind.Success);
            else
                ShowBanner(result.ErrorText, BannerKind.Warning);
        }

        /// <summary>The footer after a theme move: how far the current theme map is from the 3.5 baseline.</summary>
        private void ShowThemeFooter()
        {
            var diff = _services.Theme.Diff();
            lblStatusBar.Text = diff.Count == 0
                ? $"Theme: {_services.Theme.Current.Name} — 0 differences from the 3.5 baseline"
                : $"Theme: {_services.Theme.Current.Name} — {diff.Count} difference(s): {string.Join(" · ", diff)}";
        }

        /// <summary>The decision memo, rewritten by the service from the evidence the dossier and the run produced.</summary>
        private void RefreshMemo()
        {
            if (!_services.Assessment.IsBuilt)
                return;
            txtMemo.Text = _services.Assessment.BuildDecisionMemo(
                CurrentContext, _services.Workflow.Steps, _services.Workflow.LastHarness, _services.Theme);
        }

        /// <summary>Unexpected failure: log the details, tell the user something generic with the correlation id.</summary>
        private void ReportFailure(Exception ex)
        {
            _trace.Service($"unhandled {ex.GetType().Name} (ref {CurrentContext.CorrelationId}) — {ex.Message}");
            ShowBanner($"The action could not be completed. Check the log for details.  (ref {CurrentContext.CorrelationId})", BannerKind.Error);
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

        #region Helpers

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
            return _current;
        }

        /// <summary>A new command starts: fresh correlation id, buttons off. Sent before the first await.</summary>
        private void BeginBusy()
        {
            NewCommand();
            SetButtons(false);
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
            btnWorkOrders.Enabled = enabled;
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

        #endregion
    }
}
