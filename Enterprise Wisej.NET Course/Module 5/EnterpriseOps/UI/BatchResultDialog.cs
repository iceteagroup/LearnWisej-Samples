using System;
using System.Collections.Generic;
using System.Globalization;
using EnterpriseOps.Services.WorkQueues;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The per-row result report. A batch never ends in a single "done": this dialog lists every row of the
    /// command with its outcome and the reason, and offers a retry for the failed rows only.
    ///
    /// It owns no logic — it is handed a <see cref="BatchResult"/> the workflow produced and it shows it.
    /// The retry decision leaves as <c>DialogResult.Retry</c>; <see cref="WorkQueuePage"/> re-reads the failed
    /// rows and runs the workflow again under a new correlation id.
    /// </summary>
    public partial class BatchResultDialog : Form
    {
        private static readonly System.Drawing.Color Green = System.Drawing.Color.FromArgb(31, 138, 76);
        private static readonly System.Drawing.Color Red = System.Drawing.Color.FromArgb(156, 47, 47);
        private static readonly System.Drawing.Color Grey = System.Drawing.Color.FromArgb(90, 107, 125);

        /// <summary>Designer / default constructor.</summary>
        public BatchResultDialog()
        {
            InitializeComponent();
        }

        public BatchResultDialog(BatchResult result) : this()
        {
            ShowReport(result);
        }

        /// <summary>Fills the dialog from the report. Colours come from the outcome, nothing is recomputed here.</summary>
        private void ShowReport(BatchResult result)
        {
            if (result == null)
                return;

            bool allGood = result.Failed == 0;
            lblReportTitle.Text = $"Batch result — {result.Summary}";
            lblReportTitle.ForeColor = allGood ? Green : System.Drawing.Color.FromArgb(122, 82, 16);
            pnlReportHeader.BackColor = allGood
                ? System.Drawing.Color.FromArgb(233, 247, 238)
                : System.Drawing.Color.FromArgb(255, 248, 236);

            lblReportCorrelation.Text = "correlation " + result.CorrelationId;

            var rows = new List<BatchRowResult>(result.Rows);
            dgvResults.DataSource = rows;
            PaintRows(rows);

            lblReportFooter.Text = string.Format(CultureInfo.InvariantCulture,
                "{0} rows · {0} audit entries · {1} ms · target {2}",
                result.Rows.Count, result.ElapsedMs, result.TargetTechnician);

            btnRetry.Enabled = result.Failed > 0;
            btnRetry.Text = result.Failed > 0 ? $"Retry {result.Failed} failed row(s)" : "Retry failed rows";
            AcceptButton = result.Failed > 0 ? btnRetry : btnCloseReport;
            Text = allGood ? "Batch result — all rows changed" : "Batch result — partial failure";
        }

        /// <summary>Green for a success, red for a failure, grey for a skipped row. UI state only.</summary>
        private void PaintRows(IReadOnlyList<BatchRowResult> rows)
        {
            for (int i = 0; i < dgvResults.Rows.Count && i < rows.Count; i++)
            {
                var outcome = rows[i].Outcome;
                dgvResults.Rows[i].DefaultCellStyle.ForeColor =
                    outcome == BatchRowOutcome.Succeeded ? Green :
                    outcome == BatchRowOutcome.Failed ? Red : Grey;
            }
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Retry;
            Close();
        }

        private void btnCloseReport_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
