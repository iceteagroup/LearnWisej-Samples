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
    /// in front of the reviewer instead of asking them to take a checklist on trust.
    ///
    /// Six tabs, one per deliverable:
    ///  • Capstone package        — every deliverable, its file, and the evidence it is really that deliverable.
    ///  • AI prompt library       — the project-rules header and the task prompts, read from docs/PromptLibrary.md.
    ///  • Review checklist        — the ten questions asked of every change, generated or not.
    ///  • Generated-code review   — the failure path: pull request #214 runs through the checklist and is stopped.
    ///  • Documentation index     — docs/index.json read back, every path resolved (MCP-ready shape).
    ///  • AI usage notes          — which generated code was accepted, by whom and why.
    ///
    /// The boundary, again: nothing on this screen decides anything. Whether a deliverable passes, whether a
    /// line breaks a rule and whether this user may sign a review are decided in EnterpriseOps.Services, and
    /// the trace on the right shows each decision as it is taken.
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
            _trace.LineAdded += trace_LineAdded;
        }

        private CommandContext CurrentContext => _current ?? NewCommand();

        #region Event handlers — thin, one service call each

        private async void CapstoneReviewPage_Load(object sender, EventArgs e)
        {
            ShowTraceHistory();
            ShowSignedIn();
            ShowChecklist();
            ShowPrompts();
            ShowDocumentationIndex();
            ShowDecisions();

            txtGeneratedCode.Text = GeneratedCodeSamples.GeneratedDraft;
            lblReviewSource.Text = GeneratedCodeSamples.GeneratedDraftName + " · the text below is reviewed exactly as pasted";
            _trace.Ui("CapstoneReviewPage_Load → prompt library, checklist, documentation index");

            await VerifyPackageAsync();
        }

        /// <summary>The success path: the package verifies itself, deliverable by deliverable.</summary>
        private async void btnVerifyPackage_Click(object sender, EventArgs e)
        {
            await VerifyPackageAsync();
        }

        /// <summary>The heart of the module: run the checklist over the change in the box.</summary>
        private async void btnReviewGeneratedCode_Click(object sender, EventArgs e)
        {
            btnReviewGeneratedCode.Enabled = false;
            NewCommand();
            _trace.Ui($"btnReviewGeneratedCode_Click → GeneratedCodeReviewService.ReviewAsync corr={CurrentContext.CorrelationId}");

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
                tabCapstone.SelectedTab = tabReview;
            }
        }

        private void btnLoadDraft_Click(object sender, EventArgs e)
        {
            LoadSample(GeneratedCodeSamples.GeneratedDraft, GeneratedCodeSamples.GeneratedDraftName);
        }

        /// <summary>The recovery: the same feature after the checklist sent it back.</summary>
        private void btnLoadFixed_Click(object sender, EventArgs e)
        {
            LoadSample(GeneratedCodeSamples.AcceptedRevision, GeneratedCodeSamples.AcceptedRevisionName);
        }

        private void btnSignDecision_Click(object sender, EventArgs e)
        {
            NewCommand();
            ReviewReport report = _session.Services.Review.LastReport;
            _trace.Ui($"btnSignDecision_Click → RecordDecision({report?.PullRequest ?? "nothing reviewed yet"})");

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

        /// <summary>Failure path: a document the index promises and the disk does not have.</summary>
        private async void btnMissingDeliverable_Click(object sender, EventArgs e)
        {
            bool simulating = !_session.Services.CapstonePackage.SimulateMissingDeliverable;
            _session.Services.CapstonePackage.SimulateMissingDeliverable = simulating;
            _session.Services.DocumentationIndex.SimulateMissingDocument = simulating;
            btnMissingDeliverable.Text = simulating ? "Recover: document written" : "Fail: missing document";

            _trace.Docs(simulating
                ? "simulating a missing deliverable (docs/SecurityReviewSignOff.md) — no file on disk is touched"
                : "the missing deliverable is back in the package");

            ShowDocumentationIndex();
            await VerifyPackageAsync();
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
            _trace.Ui("btnDashboard_Click → Application.MainPage = CommandCenterDashboard");
            Application.MainPage = _dashboard != null && !_dashboard.IsDisposed
                ? _dashboard
                : new CommandCenterDashboard(_session);
        }

        private void btnClearTrace_Click(object sender, EventArgs e)
        {
            _trace.Clear();
            lstTrace.Items.Clear();
        }

        #endregion

        #region Screen work

        private async Task VerifyPackageAsync()
        {
            btnVerifyPackage.Enabled = false;
            NewCommand();
            _trace.Ui($"verify capstone package corr={CurrentContext.CorrelationId}");

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
            lblReviewSource.Text = name + " · the text below is reviewed exactly as pasted";
            dgvFindings.DataSource = null;
            SetVerdict("● pending review — press \"Review generated code\"", System.Drawing.Color.FromArgb(90, 107, 125));
            tabCapstone.SelectedTab = tabReview;
            _trace.Ui($"loaded {name} into the review box ({code.Split('\n').Length} lines)");
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

            _trace.Docs("resources/list → " + _session.Services.DocumentationIndex.DescribeResourceList());
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

        private CommandContext NewCommand()
        {
            _current = _session.BeginCommand();
            lblCorrelation.Text = "corr " + _current.CorrelationId;
            return _current;
        }

        private void ShowSignedIn()
        {
            lblTenant.Text = "tenant: " + _session.Tenant.Id;
            lblUser.Text = $"Signed in: {_session.User.Name} · {_session.User.Role}";
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

        private void ShowTraceHistory()
        {
            lstTrace.Items.Clear();
            foreach (string line in _trace.Lines)
                lstTrace.Items.Add(line);
            SelectLastTraceLine();
        }

        private void trace_LineAdded(object sender, string line)
        {
            if (IsDisposed)
                return;
            lstTrace.Items.Add(line);
            SelectLastTraceLine();
        }

        private void SelectLastTraceLine()
        {
            if (lstTrace.Items.Count > 0)
                lstTrace.SelectedIndex = lstTrace.Items.Count - 1;
        }

        /// <summary>Called from Dispose: the session's trace outlives this screen, so the handler must not.</summary>
        private void DetachTrace()
        {
            if (_trace != null)
                _trace.LineAdded -= trace_LineAdded;
        }

        #endregion
    }
}
