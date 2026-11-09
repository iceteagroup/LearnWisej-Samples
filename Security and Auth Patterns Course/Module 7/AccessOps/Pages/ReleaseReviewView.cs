#if DEBUG
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using AccessOps.Review;
using AccessOps.UI;
using Microsoft.AspNetCore.Http;
using Wisej.Web;

namespace AccessOps.Pages
{
    /// <summary>
    /// DEBUG ONLY, intranet route, Development only - the AccessOps Release Review of the Module 7
    /// walkthrough (Admin console area): four KPI cards (Cases run · Denied at a boundary · Rerun
    /// plans · Stop conditions hit), the 12-case evidence matrix (# · Case · Route · Fixture ·
    /// Boundary · Outcome · Repo calls · Evidence), the selected case's detail (alias, entry point,
    /// expected, actual, correlation label, rerun plan) and the rehearsed rollback (the four rollout
    /// stages, the stop-condition banner and the six-step runbook). A failed check shows one plain
    /// sentence (<c>allowHtml: false</c>); the details go to the server log and the redacted audit
    /// table only. The view decides nothing itself: every case calls the real services.
    /// </summary>
    public class ReleaseReviewView : UserControl
    {
        private static readonly Dictionary<string, string> LabelColors = new()
        {
            ["Live"] = "#1f9d6b", ["Harness"] = "#1565d8", ["Fixture"] = "#c77d12", ["Simulated"] = "#7d5ae0"
        };

        private static readonly Dictionary<string, string> OutcomeColors = new()
        {
            ["Allow"] = "#1565d8", ["Deny"] = "#1f9d6b", ["Reverted"] = "#1565d8", ["Rotated"] = "#1565d8",
            ["One grant"] = "#1565d8", ["Safe deny"] = "#1f9d6b", ["Failed"] = "#d93a3a"
        };

        private static readonly string[] Stages = { "Intranet baseline", "Test tenant", "Partners 10%", "All partners" };

        private readonly Label[] _kpiValues = new Label[4];
        private readonly DataGridView grdCases = new DataGridView();
        private readonly DataGridView grdRunbook = new DataGridView();
        private readonly Panel pnlMatrix = new Panel();
        private readonly Panel pnlRollback = new Panel();
        private readonly Label lblDetail = new Label();
        private readonly Label lblBanner = new Label();
        private readonly Label[] _stageLabels = new Label[4];
        private readonly Button btnRun = new Button();
        private readonly Button btnRollback = new Button();
        private readonly Button btnRestore = new Button();
        private readonly StatusLine lblStatus = new StatusLine();

        private List<EvidenceCase> _cases = ReleaseReviewRunner.Definitions();
        private List<RunbookStep> _steps = RollbackRehearsal.Steps();
        private ReleaseReviewRunner _runner;
        private bool _busy;

        /// <summary>Builds the review screen.</summary>
        public ReleaseReviewView()
        {
            this.Name = "ReleaseReviewView";
            this.BackColor = Color.White;

            // ---- KPI cards ------------------------------------------------------------------
            var kpis = new TableLayoutPanel
            {
                Name = "pnlKpis",
                Dock = DockStyle.Top,
                Height = 92,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(14, 14, 14, 12)
            };
            kpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            var kpiDefs = new[] { ("Cases run", "#1565d8"), ("Denied at a boundary", "#1f9d6b"), ("Rerun plans", "#c77d12"), ("Stop conditions hit", "#7d5ae0") };
            for (int i = 0; i < 4; i++)
            {
                kpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                var card = new Panel
                {
                    Name = "pnlKpi" + i,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4, 0, 4, 0),
                    Padding = new Padding(14, 6, 14, 4),
                    CssStyle = $"border:1px solid #e4eaf1; border-top:3px solid {kpiDefs[i].Item2}; border-radius:10px;"
                };
                var value = new Label
                {
                    Name = "lblKpiValue" + i,
                    Dock = DockStyle.Top,
                    Height = 32,
                    Font = Theme.Mono(24f, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(kpiDefs[i].Item2),
                    Text = i == 0 ? "0 / 12" : "0"
                };
                var title = new Label
                {
                    Name = "lblKpiTitle" + i,
                    Dock = DockStyle.Top,
                    Height = 18,
                    Font = Theme.Px(11f, FontStyle.Bold),
                    ForeColor = Theme.Muted,
                    Text = kpiDefs[i].Item1.ToUpperInvariant(),
                    CssStyle = "letter-spacing:.03em;"
                };
                card.Controls.Add(value);
                card.Controls.Add(title);
                _kpiValues[i] = value;
                kpis.Controls.Add(card, i, 0);
            }

            // ---- the evidence matrix ----------------------------------------------------------
            grdCases.Name = "grdCases";
            grdCases.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdCases, rowHeight: 34);
            grdCases.ColumnHeadersHeight = 30;
            grdCases.Columns.Add(DataGridStyle.MonoColumn("colNumber", "#", nameof(CaseRow.Number), 44, Theme.Accent, bold: true));
            grdCases.Columns.Add(DataGridStyle.Column("colCase", "Case", nameof(CaseRow.Title), 240, fill: true));
            grdCases.Columns.Add(DataGridStyle.Column("colRoute", "Route", nameof(CaseRow.Route), 92));
            grdCases.Columns.Add(DataGridStyle.MonoColumn("colFixture", "Fixture", nameof(CaseRow.Fixture), 96, Theme.Body));
            var boundary = DataGridStyle.Column("colBoundary", "Boundary", nameof(CaseRow.Boundary), 120);
            boundary.DefaultCellStyle.Font = Theme.Px(12.5f, FontStyle.Bold);
            grdCases.Columns.Add(boundary);
            var outcome = DataGridStyle.Column("colOutcome", "Outcome", nameof(CaseRow.Outcome), 116);
            outcome.AllowHtml = true;   // chip markup from a fixed list; the text is HTML-encoded
            grdCases.Columns.Add(outcome);
            grdCases.Columns.Add(DataGridStyle.MonoColumn("colRepo", "Repo calls", nameof(CaseRow.RepoCalls), 92, Theme.Ok, bold: true));
            var evidence = DataGridStyle.Column("colEvidence", "Evidence", nameof(CaseRow.Evidence), 104);
            evidence.AllowHtml = true;
            grdCases.Columns.Add(evidence);
            grdCases.SelectionChanged += (s, e) => ShowDetail();

            var gridFrame = new Panel
            {
                Name = "pnlGridFrame",
                Dock = DockStyle.Fill,
                CssStyle = "border:1px solid #e4eaf1; border-radius:10px; overflow:hidden;"
            };
            gridFrame.Controls.Add(grdCases);

            lblDetail.Name = "lblDetail";
            lblDetail.Dock = DockStyle.Bottom;
            lblDetail.Height = 104;
            lblDetail.Padding = new Padding(12, 8, 12, 8);
            lblDetail.Font = Theme.Px(12f);
            lblDetail.ForeColor = Theme.Body;
            lblDetail.AllowHtml = false;
            lblDetail.CssStyle = "white-space:pre-wrap; border:1px solid #e4eaf1; border-radius:10px; margin-top:8px; background:#f8fafc;";

            pnlMatrix.Name = "pnlMatrix";
            pnlMatrix.Dock = DockStyle.Fill;
            pnlMatrix.Padding = new Padding(18, 0, 18, 8);
            pnlMatrix.Controls.Add(gridFrame);
            pnlMatrix.Controls.Add(new Panel { Dock = DockStyle.Bottom, Height = 8 });
            pnlMatrix.Controls.Add(lblDetail);

            // ---- the rollback panel -------------------------------------------------------------
            var stages = new TableLayoutPanel { Name = "pnlStages", Dock = DockStyle.Top, Height = 44, ColumnCount = 7, RowCount = 1 };
            for (int i = 0; i < Stages.Length; i++)
            {
                stages.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
                var pill = new Label
                {
                    Name = "lblStage" + i,
                    Text = Stages[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = Theme.Px(13.5f, FontStyle.Bold),
                    Margin = new Padding(0, 2, 0, 2)
                };
                _stageLabels[i] = pill;
                stages.Controls.Add(pill, i * 2, 0);
                if (i < Stages.Length - 1)
                {
                    stages.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 26f));
                    stages.Controls.Add(new Label { Text = "→", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = ColorTranslator.FromHtml("#9aa9b8"), Font = Theme.Px(14f, FontStyle.Bold) }, i * 2 + 1, 0);
                }
            }

            lblBanner.Name = "lblBanner";
            lblBanner.Dock = DockStyle.Top;
            lblBanner.Height = 40;
            lblBanner.Padding = new Padding(16, 0, 16, 0);
            lblBanner.Font = Theme.Px(14f, FontStyle.Bold);
            lblBanner.TextAlign = ContentAlignment.MiddleLeft;
            lblBanner.AllowHtml = false;

            grdRunbook.Name = "grdRunbook";
            grdRunbook.Dock = DockStyle.Fill;
            DataGridStyle.Apply(grdRunbook, rowHeight: 40);
            grdRunbook.ColumnHeadersHeight = 30;
            grdRunbook.Columns.Add(DataGridStyle.MonoColumn("colStep", "Step", nameof(StepRow.Number), 54, Theme.Accent, bold: true));
            grdRunbook.Columns.Add(DataGridStyle.Column("colAction", "Rollback runbook", nameof(StepRow.Action), 300, fill: true));
            grdRunbook.Columns.Add(DataGridStyle.Column("colOwner", "Owner", nameof(StepRow.Owner), 140));
            var result = DataGridStyle.Column("colResult", "Result", nameof(StepRow.Result), 170);
            result.AllowHtml = true;
            grdRunbook.Columns.Add(result);
            grdRunbook.SelectionChanged += (s, e) => ShowStepDetail();
            var runbookFrame = new Panel { Name = "pnlRunbookFrame", Dock = DockStyle.Fill, CssStyle = "border:1px solid #e4eaf1; border-radius:10px; overflow:hidden;" };
            runbookFrame.Controls.Add(grdRunbook);

            pnlRollback.Name = "pnlRollback";
            pnlRollback.Dock = DockStyle.Fill;
            pnlRollback.Padding = new Padding(18, 4, 18, 8);
            pnlRollback.Visible = false;
            pnlRollback.Controls.Add(runbookFrame);
            pnlRollback.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            pnlRollback.Controls.Add(lblBanner);
            pnlRollback.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            pnlRollback.Controls.Add(stages);

            // ---- actions ------------------------------------------------------------------------
            var actions = new FlowLayoutPanel
            {
                Name = "pnlActions",
                Dock = DockStyle.Bottom,
                Height = 46,
                Padding = new Padding(18, 4, 18, 8),
                WrapContents = false
            };
            btnRun.Name = "btnRun";
            btnRun.Text = "Run the 12 cases";
            Theme.StyleButton(btnRun, primary: true, size: 12.5f);
            btnRun.Size = new Size(140, 32);
            btnRun.Margin = new Padding(0);
            btnRun.Click += btnRun_Click;
            btnRollback.Name = "btnRollback";
            btnRollback.Text = "Rehearse rollback";
            Theme.StyleButton(btnRollback, primary: false, size: 12.5f);
            btnRollback.Size = new Size(140, 32);
            btnRollback.Margin = new Padding(8, 0, 0, 0);
            btnRollback.Click += btnRollback_Click;
            btnRestore.Name = "btnRestore";
            btnRestore.Text = "Restore partner route";
            Theme.StyleButton(btnRestore, primary: false, size: 12.5f);
            btnRestore.Size = new Size(160, 32);
            btnRestore.Margin = new Padding(8, 0, 0, 0);
            btnRestore.Click += btnRestore_Click;
            var btnMatrix = new Button { Name = "btnMatrix", Text = "Evidence matrix", Size = new Size(130, 32), Margin = new Padding(8, 0, 0, 0) };
            Theme.StyleButton(btnMatrix, primary: false, size: 12.5f);
            btnMatrix.Click += (s, e) => ShowPanel(rollback: false);
            actions.Controls.AddRange(new Control[] { btnRun, btnRollback, btnRestore, btnMatrix });

            // Dock order is the reverse of Controls.Add.
            this.Controls.Add(pnlMatrix);
            this.Controls.Add(pnlRollback);
            this.Controls.Add(kpis);
            this.Controls.Add(actions);
            this.Controls.Add(lblStatus);

            BindCases();
            BindSteps(stopped: false, drill: false);
            lblStatus.Show("Ready · 12 cases, run them before promotion", StatusTone.Idle);
        }

        // ---- the evidence run --------------------------------------------------------------------

        private async void btnRun_Click(object sender, EventArgs e)
        {
            if (_busy)
                return;
            _busy = true;
            SetButtons(false);
            ShowPanel(rollback: false);

            // captured on the Wisej.NET thread, before the first await
            var context = new ReviewContext
            {
                SessionPrincipal = Application.User,
                Request = AppHost.Get<IHttpContextAccessor>().HttpContext
            };
            _runner = new ReleaseReviewRunner(AppHost.Services);
            _cases = ReleaseReviewRunner.Definitions();
            BindCases();

            foreach (var c in _cases)
            {
                c.Running = true;
                BindCases();
                lblStatus.Show($"Running case {c.Number} · {c.Title}", StatusTone.Idle);
                Application.Update(this);
                await _runner.RunAsync(c, context);
                BindCases();
                Application.Update(this);
            }

            if (_runner.AuditHoldsCredentialMaterial())
                _cases.Last().StopCondition ??= "Credential material in a log or a hidden column";
            BindCases();

            var failed = _cases.Where(c => !c.Passed).ToList();
            var stops = _cases.Count(c => c.StopCondition != null);
            if (failed.Count == 0)
            {
                lblStatus.Show($"12 of 12 cases as expected · {_cases.Count(c => c.IsDeny)} denied at a boundary with 0 repository calls · no stop condition", StatusTone.Ok);
            }
            else
            {
                lblStatus.Show($"{failed.Count} case(s) did not pass: {string.Join(", ", failed.Select(c => c.Number))}" + (stops > 0 ? " · stop condition hit" : ""), StatusTone.Bad);
                // one plain sentence; the details are in the server log and the audit table only
                AlertBox.Show($"{failed.Count} release check(s) did not pass (case {string.Join(", ", failed.Select(c => c.Number))}). The details are in the audit log.",
                    MessageBoxIcon.Warning, allowHtml: false);
            }

            SetButtons(true);
            _busy = false;
            Application.Update(this);
        }

        // ---- the rollback rehearsal ----------------------------------------------------------------

        private async void btnRollback_Click(object sender, EventArgs e)
        {
            if (_busy)
                return;
            _busy = true;
            SetButtons(false);

            var context = new ReviewContext
            {
                SessionPrincipal = Application.User,
                Request = AppHost.Get<IHttpContextAccessor>().HttpContext
            };
            _runner ??= new ReleaseReviewRunner(AppHost.Services);
            var rehearsal = new RollbackRehearsal(_runner, AppHost.Services);
            var stop = _cases.Select(c => c.StopCondition).FirstOrDefault(s => s != null);
            _steps = RollbackRehearsal.Steps();
            BindSteps(stopped: stop != null, drill: stop == null, stop);
            ShowPanel(rollback: true);
            Application.Update(this);

            foreach (var step in _steps)
            {
                await rehearsal.RunAsync(step, context, Infrastructure.Correlation.NewLabel($"rb-{step.Number}"));
                BindSteps(stopped: stop != null, drill: stop == null, stop, current: step.Number);
                Application.Update(this);
            }

            var bad = _steps.Where(s => !s.Ok).ToList();
            if (bad.Count == 0)
            {
                lblStatus.Show($"Rollback rehearsed · partner route off after {rehearsal.SecondsToPartnerOff} s · Reader still reads · stale tab still denied", StatusTone.Ok);
            }
            else
            {
                lblStatus.Show($"Rollback step(s) {string.Join(", ", bad.Select(s => s.Number))} did not complete", StatusTone.Bad);
                AlertBox.Show($"Rollback step {string.Join(", ", bad.Select(s => s.Number))} did not complete. The details are in the audit log.",
                    MessageBoxIcon.Warning, allowHtml: false);
            }

            SetButtons(true);
            _busy = false;
            Application.Update(this);
        }

        private async void btnRestore_Click(object sender, EventArgs e)
        {
            if (_busy)
                return;
            _busy = true;
            SetButtons(false);
            _runner ??= new ReleaseReviewRunner(AppHost.Services);
            var status = await new RollbackRehearsal(_runner, AppHost.Services).RestoreAsync();
            lblStatus.Show(status == 503 || status == 0
                    ? "The partner route did not come back. Check the partner process."
                    : $"Partner route on again · it answers {status}",
                status == 503 || status == 0 ? StatusTone.Bad : StatusTone.Ok);
            SetButtons(true);
            _busy = false;
            Application.Update(this);
        }

        // ---- binding -----------------------------------------------------------------------------

        private void SetButtons(bool enabled)
        {
            btnRun.Enabled = enabled;
            btnRollback.Enabled = enabled;
            btnRestore.Enabled = enabled;
        }

        private void ShowPanel(bool rollback)
        {
            pnlRollback.Visible = rollback;
            pnlMatrix.Visible = !rollback;
        }

        private void BindCases()
        {
            var selected = grdCases.CurrentRow?.Index ?? 0;
            grdCases.DataSource = _cases.Select(c => new CaseRow(c)).ToList();
            foreach (DataGridViewRow row in grdCases.Rows)
            {
                if (row.DataBoundItem is CaseRow r && r.Source.Running)
                    row.DefaultCellStyle.BackColor = Theme.AccentWash;
            }
            var running = _cases.FirstOrDefault(c => c.Running);
            var index = running != null ? _cases.IndexOf(running) : Math.Min(selected, grdCases.Rows.Count - 1);
            if (index >= 0 && grdCases.Rows.Count > 0)
                grdCases.CurrentCell = grdCases.Rows[index].Cells[0];

            var done = _cases.Count(c => c.Done);
            _kpiValues[0].Text = $"{done} / 12";
            _kpiValues[1].Text = _cases.Count(c => c.Done && c.IsDeny).ToString();
            _kpiValues[2].Text = _cases.Count(c => c.Done && c.Label is EvidenceLabel.Fixture or EvidenceLabel.Harness).ToString();
            _kpiValues[3].Text = _cases.Count(c => c.StopCondition != null).ToString();
            ShowDetail();
        }

        private void ShowDetail()
        {
            if (grdCases.CurrentRow?.DataBoundItem is not CaseRow row)
                return;
            var c = row.Source;
            lblDetail.Text = c.Done
                ? $"Case {c.Number} · {c.Alias} · {c.EntryPoint} · correlation {c.Correlation}\nExpected: {c.Expected}\nActual: {c.Actual}" +
                  (c.StopCondition != null ? $"\nSTOP CONDITION: {c.StopCondition}" : "") +
                  (c.RerunPlan != null ? $"\nRerun plan ({c.Label}): {c.RerunPlan}" : "")
                : $"Case {c.Number} · expected: {c.Expected}" + (c.RerunPlan != null ? $"\nRerun plan ({c.Label}): {c.RerunPlan}" : "");
        }

        private void BindSteps(bool stopped, bool drill, string stop = null, int current = 0)
        {
            for (int i = 0; i < _stageLabels.Length; i++)
            {
                var red = (stopped || drill) && i == 2;
                var on = i <= 2;
                var color = red ? (stopped ? "#d93a3a" : "#c77d12") : on ? "#1565d8" : "#9aa9b8";
                var wash = red ? (stopped ? "#fdecec" : "#fdf3e1") : on ? "#eaf3ff" : "#f7f9fb";
                _stageLabels[i].ForeColor = ColorTranslator.FromHtml(color);
                _stageLabels[i].BackColor = ColorTranslator.FromHtml(wash);
                _stageLabels[i].CssStyle = $"border:1.5px solid {color}; border-radius:9px;";
            }

            var text = stopped
                ? "Stop condition hit: " + stop + "."
                : "Rollback drill: no stop condition hit in the last run; the partner route is switched off as a rehearsal.";
            lblBanner.Text = text;
            lblBanner.ForeColor = ColorTranslator.FromHtml(stopped ? "#a12828" : "#8a5a00");
            lblBanner.BackColor = ColorTranslator.FromHtml(stopped ? "#fdecec" : "#fff4e0");
            lblBanner.CssStyle = $"border:1px solid {(stopped ? "#f2b8b8" : "#f0d9a8")}; border-radius:9px;";

            grdRunbook.DataSource = _steps.Select(s => new StepRow(s)).ToList();
            if (current > 0 && current <= grdRunbook.Rows.Count)
            {
                grdRunbook.Rows[current - 1].DefaultCellStyle.BackColor = Theme.AccentWash;
                grdRunbook.CurrentCell = grdRunbook.Rows[current - 1].Cells[0];
            }
        }

        private void ShowStepDetail()
        {
            if (grdRunbook.CurrentRow?.DataBoundItem is StepRow row && !string.IsNullOrEmpty(row.Source.Detail))
                lblStatus.Show($"Step {row.Source.Number} · {row.Source.Detail}", row.Source.Ok ? StatusTone.Idle : StatusTone.Bad);
        }

        /// <summary>The mock's chip; the colours come from a fixed list and the text is HTML-encoded.</summary>
        internal static string Chip(string text, string color)
            => $"<span style=\"font-size:11.5px;font-weight:800;color:{color};background:{color}1a;border:1px solid {color}55;border-radius:999px;padding:2px 9px;white-space:nowrap\">{WebUtility.HtmlEncode(text)}</span>";

        /// <summary>A row of the evidence matrix (display values only).</summary>
        private sealed class CaseRow
        {
            public CaseRow(EvidenceCase c) => this.Source = c;

            public EvidenceCase Source { get; }
            public string Number => this.Source.Number.ToString();
            public string Title => this.Source.Title;
            public string Route => this.Source.Route;
            public string Fixture => this.Source.Fixture;
            public string Boundary => this.Source.Done ? this.Source.Boundary : "";
            public string Outcome => this.Source.Done
                ? Chip(this.Source.Outcome, OutcomeColors.TryGetValue(this.Source.Outcome, out var c) ? c : "#5a6b7d")
                : this.Source.Running ? "<span style=\"font-size:11.5px;color:#1565d8;font-weight:700\">running</span>" : "";
            public string RepoCalls => this.Source.Done ? this.Source.RepositoryCalls.ToString() : "";
            public string Evidence => this.Source.Done ? Chip(this.Source.Label.ToString(), LabelColors[this.Source.Label.ToString()]) : "";
        }

        /// <summary>A row of the runbook grid.</summary>
        private sealed class StepRow
        {
            public StepRow(RunbookStep s) => this.Source = s;

            public RunbookStep Source { get; }
            public string Number => this.Source.Number.ToString();
            public string Action => this.Source.Action;
            public string Owner => this.Source.Owner;
            public string Result => this.Source.Result == "pending"
                ? "<span style=\"font-size:12px;color:#9aa9b8\">pending</span>"
                : Chip(this.Source.Result, !this.Source.Ok ? "#d93a3a" : this.Source.Number == 4 ? "#1f9d6b" : this.Source.Number == 6 ? "#d93a3a" : "#1565d8");
        }
    }
}
#endif
