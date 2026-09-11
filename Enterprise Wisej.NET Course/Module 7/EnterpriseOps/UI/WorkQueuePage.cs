using System;
using System.Collections.Generic;
using System.Linq;
using EnterpriseOps.Security;
using EnterpriseOps.Services;
using EnterpriseOps.Services.Workflow;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — the work queue that opens the Escalation Wizard modally, and the manual-review queue
    /// the workflow's compensations land in (an outstanding notification is retried from here).
    /// </summary>
    public partial class WorkQueuePage : Page
    {
        // The per-session service graph: page and wizard share it, two browser sessions share nothing.
        private readonly SessionServices _services;

        private List<WorkQueueRow> _rows = new List<WorkQueueRow>();
        private List<CompensationEntry> _queue = new List<CompensationEntry>();

        public WorkQueuePage()
        {
            InitializeComponent();

            _services = new SessionServices("contoso", UserDirectory.AnaOps);
            _services.Compensation.Changed += compensation_Changed;
        }

        #region Event handlers — thin, one service (or one dialog) each

        private void WorkQueuePage_Load(object sender, EventArgs e)
        {
            LoadQueue();
            RenderCompensationQueue();
        }

        /// <summary>Open the wizard modally and show the typed result it brings back.</summary>
        private async void btnEscalate_Click(object sender, EventArgs e)
        {
            var row = SelectedRow();
            if (row == null)
            {
                ShowBanner("Select a work order in the queue first.", BannerKind.Warn);
                return;
            }

            try
            {
                var wizard = new EscalationWizard(_services, row.Id);
                DialogResult answer = await wizard.ShowDialogAsync();
                ShowWizardOutcome(answer, wizard);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                LoadQueue();
                RenderCompensationQueue();

                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        /// <summary>The queued compensation is retried, and the workflow finishes later.</summary>
        private async void btnRetryNotification_Click(object sender, EventArgs e)
        {
            var entry = SelectedCompensation();
            if (entry == null)
            {
                ShowBanner("Select an entry in the manual-review queue first.", BannerKind.Warn);
                return;
            }

            try
            {
                btnRetryNotification.Enabled = false;
                var result = await _services.Workflow.RetryNotificationAsync(entry, _services.Session);
                ShowResult(result);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex);
            }
            finally
            {
                btnRetryNotification.Enabled = true;
                RenderCompensationQueue();
                LoadQueue();

                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        private void compensation_Changed()
        {
            RenderCompensationQueue();
        }

        #endregion

        #region Rendering — UI state only

        private void LoadQueue()
        {
            _rows = _services.WorkQueue.Load(_services.Session);
            dgvWorkQueue.DataSource = new BindingSource { DataSource = _rows };
        }

        private void RenderCompensationQueue()
        {
            _queue = _services.Compensation.Entries.OrderBy(c => c.Status).ThenBy(c => c.Id).ToList();
            lstCompensation.Items.Clear();
            foreach (var entry in _queue)
                lstCompensation.Items.Add(entry.ToString());

            int open = _services.Compensation.ManualReviewQueue.Count;
            lblCompensationTitle.Text = $"Manual-review queue — open compensations: {open}";
        }

        /// <summary>What the wizard brought back: a typed result, or a cancel that says what happened to the draft.</summary>
        private void ShowWizardOutcome(DialogResult answer, EscalationWizard wizard)
        {
            if (answer == DialogResult.OK && wizard.Result != null)
            {
                ShowResult(wizard.Result);
                return;
            }

            ShowBanner(wizard.DraftKept
                    ? "Escalation cancelled — the draft is kept; \"Escalate work order…\" resumes it."
                    : "Escalation cancelled — the draft was discarded. Nothing was saved.",
                BannerKind.Warn);
        }

        /// <summary>One typed result in, one screen state out. Switch on the Outcome, never on the message.</summary>
        private void ShowResult(WorkflowResult result)
        {
            switch (result.Outcome)
            {
                case WorkflowOutcome.Created:
                    ShowBanner(result.Message, BannerKind.Ok);
                    AlertBox.Show(result.Message, MessageBoxIcon.Information,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    break;

                case WorkflowOutcome.CreatedWithCompensation:
                    ShowBanner($"{result.Message}  ·  {result.CompensationAction}  ·  next: {result.NextAction}", BannerKind.Warn);
                    AlertBox.Show(result.Message, MessageBoxIcon.Warning,
                        alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
                    break;

                case WorkflowOutcome.ValidationFailed:
                    ShowBanner(result.Message + "  ·  " + string.Join(" · ", result.FieldErrors.Select(f => $"{WizardSteps.Title(f.Step)}/{f.Field}: {f.Message}")), BannerKind.Error);
                    break;

                default:
                    ShowBanner($"{result.Message}  ·  next: {result.NextAction}  ·  correlation {result.CorrelationId}", BannerKind.Error);
                    break;
            }
        }

        private enum BannerKind { Ok, Warn, Error }

        private void ShowBanner(string text, BannerKind kind)
        {
            lblBanner.Text = text;
            lblBanner.BackColor = kind switch
            {
                BannerKind.Ok => System.Drawing.Color.FromArgb(240, 249, 243),
                BannerKind.Warn => System.Drawing.Color.FromArgb(255, 248, 236),
                _ => System.Drawing.Color.FromArgb(253, 236, 234),
            };
            lblBanner.ForeColor = kind switch
            {
                BannerKind.Ok => System.Drawing.Color.FromArgb(21, 95, 51),
                BannerKind.Warn => System.Drawing.Color.FromArgb(122, 82, 16),
                _ => System.Drawing.Color.FromArgb(154, 42, 24),
            };
            lblBanner.Visible = true;
        }

        private WorkQueueRow SelectedRow()
        {
            int index = dgvWorkQueue.CurrentRow?.Index ?? -1;
            return index >= 0 && index < _rows.Count ? _rows[index] : null;
        }

        private CompensationEntry SelectedCompensation()
        {
            int index = lstCompensation.SelectedIndex;
            return index >= 0 && index < _queue.Count ? _queue[index] : null;
        }

        private void ReportUnexpected(Exception ex)
        {
            _services.Trace.Write($"UI: unexpected {ex.GetType().Name}: {ex.Message}");
            ShowBanner("The action could not be completed. Please try again.", BannerKind.Error);
            AlertBox.Show("The action could not be completed.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
