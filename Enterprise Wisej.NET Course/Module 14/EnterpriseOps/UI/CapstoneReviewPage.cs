using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnterpriseOps.Diagnostics;
using EnterpriseOps.Domain;
using EnterpriseOps.Services;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// EnterpriseOps — Capstone review. The delivery screen: it presents the capstone package and checks it
    /// in front of the reviewer.
    ///
    /// Six tabs, one per deliverable:
    ///  • Capstone package        — every deliverable, its file, and the evidence it is really that deliverable.
    ///  • AI prompt library       — the project-rules header and the task prompts, read from docs/PromptLibrary.md.
    ///  • Review checklist        — the ten questions asked of every change, generated or not.
    ///  • Generated-code review   — pull request #214 runs through the checklist and is stopped; rev 2 passes.
    ///  • Documentation index     — docs/index.json read back, every path resolved (MCP-ready shape).
    ///  • AI usage notes          — which generated code was accepted, by whom and why.
    ///
    /// Nothing on this screen decides anything: whether a deliverable passes, whether a line breaks a rule and
    /// whether this user may sign a review are decided in EnterpriseOps.Services.
    /// </summary>
    public partial class CapstoneReviewPage : Page
    {
        private readonly SessionContext _session;
        private readonly ActivityTrace _trace;
        private readonly CommandCenterDashboard _dashboard;

        private CommandContext _current;
        private List<PromptEntry> _prompts = new List<PromptEntry>();

        /// <summary>Designer / default constructor.</summary>
        public CapstoneReviewPage() : this(SessionContext.CreateDefault(), null)
        {
        }

        public CapstoneReviewPage(SessionContext session, CommandCenterDashboard dashboard)
        {
            InitializeComponent();

            _session = session;
            _dashboard = dashboard;
            _trace = session.Services.Trace;
        }

        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers

        private async void CapstoneReviewPage_Load(object sender, EventArgs e)
        {
            ShowChecklist();
            ShowPrompts();
            ShowDocumentationIndex();
            ShowDecisions();

            txtGeneratedCode.Text = GeneratedCodeSamples.GeneratedDraft;
            lblReviewSource.Text = GeneratedCodeSamples.GeneratedDraftName;

            await VerifyPackageAsync();
        }

        private async void btnVerifyPackage_Click(object sender, EventArgs e)
        {
            await VerifyPackageAsync();
        }

        private async void btnReviewGeneratedCode_Click(object sender, EventArgs e)
        {
            btnReviewGeneratedCode.Enabled = false;
            NewCommand();

            try
            {
                ReviewOutcome outcome = await _session.Services.Review.ReviewAsync(
                    txtGeneratedCode.Text, CurrentPullRequest(), CurrentContext);

                ShowReviewOutcome(outcome);
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The review could not be completed.");
            }
            finally
            {
                btnReviewGeneratedCode.Enabled = true;

                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        private void btnLoadDraft_Click(object sender, EventArgs e)
        {
            LoadSample(GeneratedCodeSamples.GeneratedDraft, GeneratedCodeSamples.GeneratedDraftName);
        }

        private void btnLoadFixed_Click(object sender, EventArgs e)
        {
            LoadSample(GeneratedCodeSamples.AcceptedRevision, GeneratedCodeSamples.AcceptedRevisionName);
        }

        private void btnSignDecision_Click(object sender, EventArgs e)
        {
            NewCommand();
            ReviewReport report = _session.Services.Review.LastReport;

            CommandResult result = _session.Services.Review.RecordDecision(report, ReasonFor(report), CurrentContext);
            if (!result.Succeeded)
            {
                Warn(result.ErrorText);
                return;
            }

            ShowDecisions();
            tabCapstone.SelectedTab = tabDecisions;
            AlertBox.Show("Decision recorded in the AI usage notes.", MessageBoxIcon.Information,
                alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        private void lstPrompts_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = lstPrompts.SelectedIndex;
            if (index < 0 || index >= _prompts.Count)
                return;

            PromptEntry entry = _prompts[index];
            txtPrompt.Text = _session.Services.PromptLibrary.Expand(entry);
            lblPromptsStatus.Text = entry.IncludesHeader
                ? $"docs/PromptLibrary.md · {entry.Id} — starts from the project-rules header"
                : $"docs/PromptLibrary.md · {entry.Id} — ⚠ does not pull in the project-rules header";
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Application.MainPage = _dashboard != null && !_dashboard.IsDisposed
                ? _dashboard
                : new CommandCenterDashboard(_session);
        }

        #endregion

        #region Showing results

        private async Task VerifyPackageAsync()
        {
            btnVerifyPackage.Enabled = false;
            NewCommand();

            try
            {
                CapstoneVerification verification = await _session.Services.CapstonePackage.VerifyAsync(CurrentContext);
                dgvPackage.DataSource = new BindingSource { DataSource = verification.Checks };

                if (verification.Result.Succeeded)
                    SetPackageStatus($"● ready to submit — {verification.Passed}/{verification.Checks.Count} checks pass",
                        System.Drawing.Color.FromArgb(31, 157, 87));
                else
                    SetPackageStatus($"● {verification.Result.ErrorText} ({verification.Passed}/{verification.Checks.Count} checks pass)",
                        System.Drawing.Color.FromArgb(224, 86, 59));
            }
            catch (Exception ex)
            {
                ReportUnexpected(ex, "The capstone package could not be verified.");
            }
            finally
            {
                btnVerifyPackage.Enabled = true;

                // The awaits end after the request returned: push the final UI state to the browser.
                Application.Update(this);
            }
        }

        private void ShowReviewOutcome(ReviewOutcome outcome)
        {
            if (outcome.Report == null)
            {
                dgvFindings.DataSource = null;
                SetVerdict("● " + outcome.Result.ErrorText, System.Drawing.Color.FromArgb(224, 86, 59));
                return;
            }

            ReviewReport report = outcome.Report;
            dgvFindings.DataSource = new BindingSource { DataSource = report.Findings };

            int blocking = report.Findings.Count(f => f.Severity == ReviewSeverity.Reject);
            switch (report.Verdict)
            {
                case ReviewVerdict.Rejected:
                    SetVerdict($"● REJECTED — {blocking} blocking finding(s) of {report.Findings.Count} over {report.LinesReviewed} lines · {report.RulesRun} rules run",
                        System.Drawing.Color.FromArgb(224, 86, 59));
                    break;
                case ReviewVerdict.AcceptedWithWarnings:
                    SetVerdict($"● ACCEPTED WITH WARNINGS — {report.Findings.Count} finding(s), none blocking · {report.RulesRun} rules run",
                        System.Drawing.Color.FromArgb(232, 161, 60));
                    break;
                default:
                    SetVerdict($"● ACCEPTED — no checklist finding over {report.LinesReviewed} lines · {report.RulesRun} rules run",
                        System.Drawing.Color.FromArgb(31, 157, 87));
                    break;
            }
        }

        private void LoadSample(string code, string name)
        {
            txtGeneratedCode.Text = code;
            lblReviewSource.Text = name;
            dgvFindings.DataSource = null;
            SetVerdict("● pending review — press \"Review generated code\"", System.Drawing.Color.FromArgb(90, 107, 125));
        }

        private void ShowChecklist()
        {
            dgvChecklist.DataSource = new BindingSource { DataSource = _session.Services.Review.Rules.ToList() };
            lblChecklistTitle.Text = $"docs/GeneratedCodeReviewChecklist.md — {_session.Services.Review.Rules.Count} questions, "
                + $"asked of every change · API catalog: {DocumentedApiCatalog.MemberCount} documented members from {DocumentedApiCatalog.Source}";
        }

        private void ShowPrompts()
        {
            _prompts = _session.Services.PromptLibrary.Load();
            lstPrompts.Items.Clear();
            foreach (PromptEntry entry in _prompts)
                lstPrompts.Items.Add($"{entry.Id} · {entry.Title}");

            if (_prompts.Count > 0)
                lstPrompts.SelectedIndex = 0;
            else
                lblPromptsStatus.Text = "docs/PromptLibrary.md — not found next to the running project";
        }

        private void ShowDocumentationIndex()
        {
            List<DocEntry> entries = _session.Services.DocumentationIndex.Reload();
            dgvDocs.DataSource = new BindingSource { DataSource = entries };

            int broken = entries.Count(entry => !entry.Exists);
            lblDocsStatus.Text = broken == 0
                ? $"docs/index.json — {entries.Count} resources, every path resolves (MCP resources/list shape)"
                : $"docs/index.json — {entries.Count} resources, ⚠ {broken} would answer 404";
            lblDocsStatus.ForeColor = broken == 0
                ? System.Drawing.Color.FromArgb(31, 157, 87)
                : System.Drawing.Color.FromArgb(224, 86, 59);
        }

        private void ShowDecisions()
        {
            List<ReviewDecision> decisions = _session.Services.Review.Decisions.ToList();
            dgvDecisions.DataSource = new BindingSource { DataSource = decisions };
            lblDecisionsStatus.Text = decisions.Count == 0
                ? "No decision signed in this session — docs/AIUsageNotes.md holds the written record."
                : $"{decisions.Count} decision(s) signed in this session · the written record is docs/AIUsageNotes.md";
        }

        /// <summary>The name the change is reviewed under — the draft, the fixed revision, or a pasted change.</summary>
        private string CurrentPullRequest()
        {
            string code = txtGeneratedCode.Text ?? "";
            if (code.Contains("WorkOrderScreenHelper"))
                return GeneratedCodeSamples.GeneratedDraftName;
            if (code.Contains("WorkOrderScreenService"))
                return GeneratedCodeSamples.AcceptedRevisionName;
            return "Pasted change · reviewed in session " + Application.SessionId;
        }

        private static string ReasonFor(ReviewReport report)
        {
            if (report == null)
                return "";
            switch (report.Verdict)
            {
                case ReviewVerdict.Rejected:
                    return "Rejected: " + string.Join("; ", report.Findings
                        .Where(f => f.Severity == ReviewSeverity.Reject)
                        .Select(f => $"{f.RuleId} L{f.Line}")) + " — sent back with the prompt header attached.";
                case ReviewVerdict.AcceptedWithWarnings:
                    return "Accepted with warnings: " + string.Join("; ", report.Findings.Select(f => $"{f.RuleId} L{f.Line}"))
                        + " — the reviewer can explain each one without the tool present.";
                default:
                    return "Accepted: no checklist finding; every platform API used is in the documentation catalog.";
            }
        }

        #endregion

        #region Helpers

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            return _current;
        }

        private void SetPackageStatus(string text, System.Drawing.Color colour)
        {
            lblPackageStatus.Text = text;
            lblPackageStatus.ForeColor = colour;
        }

        private void SetVerdict(string text, System.Drawing.Color colour)
        {
            lblVerdict.Text = text;
            lblVerdict.ForeColor = colour;
        }

        private void Warn(string text)
            => AlertBox.Show(text, MessageBoxIcon.Warning, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);

        private void ReportUnexpected(Exception ex, string message)
        {
            _trace.Service($"unhandled {ex.GetType().Name}: {ex.Message} [corr {CurrentContext.CorrelationId}]");
            AlertBox.Show($"{message} Quote correlation id {CurrentContext.CorrelationId} when you report it.",
                MessageBoxIcon.Error, alignment: System.Drawing.ContentAlignment.TopRight, autoCloseDelay: 4000);
        }

        #endregion
    }
}
