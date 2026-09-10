using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdaptiveOps.Governance;
using Wisej.Web;

namespace AdaptiveOps.Dialogs
{
    /// <summary>
    /// "Governance review": the production checklist of docs/ProductionChecklist.md as a grid of rules,
    /// each row computed against the running application by <see cref="GovernanceReview"/>. Failures sort
    /// first. Pass/fail is text ("✓ pass" / "✕ FAIL") plus the summary label's theme state — never colour alone.
    /// </summary>
    public partial class GovernanceDialog : Form
    {
        public GovernanceDialog()
        {
            InitializeComponent();
        }

        /// <summary>Fills the grid and the summary. Returns the number of failing rules.</summary>
        public int ShowResults(IReadOnlyList<GovernanceResult> results, string profileName, string context)
        {
            int failed = results.Count(r => !r.Pass);

            this.gridRules.SuspendLayout();
            try
            {
                this.gridRules.Rows.Clear();
                foreach (var r in results.OrderBy(r => r.Pass).ThenBy(r => r.Id, StringComparer.Ordinal))
                {
                    int index = this.gridRules.Rows.Add(new object[] { r.Pass ? "✓ pass" : "✕ FAIL", r.Id + " · " + r.Rule, r.Evidence });
                    this.gridRules.Rows[index].Tag = r;
                }
            }
            finally
            {
                this.gridRules.ResumeLayout(true);
            }

            this.lblSummary.RemoveState("ok");
            this.lblSummary.RemoveState("error");
            this.lblSummary.AddState(failed == 0 ? "ok" : "error");
            this.lblSummary.CssClass = failed == 0 ? "gov-summary gov-summary-pass" : "gov-summary gov-summary-fail";
            this.lblSummary.ImageSource = failed == 0 ? "icon-check" : "icon-error";
            this.lblSummary.Text = failed == 0
                ? $"All {results.Count} rules pass — the console is release-ready on the {profileName} profile."
                : $"{failed} of {results.Count} rules FAIL on the {profileName} profile — fix before release.";

            this.lblFooter.Text = $"Computed at {DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture)} · {context} · rules: docs/ProductionChecklist.md";
            return failed;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
