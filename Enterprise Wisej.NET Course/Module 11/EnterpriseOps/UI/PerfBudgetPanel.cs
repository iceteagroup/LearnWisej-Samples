using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using EnterpriseOps.Diagnostics;
using Wisej.Web;

namespace EnterpriseOps.UI
{
    /// <summary>
    /// The performance budget table as a reusable control: one row per operation, budget vs measured, and
    /// the row turns red the moment a budget is broken. It only *displays* BudgetRow — OK / OVER was decided
    /// by PerformanceBudget.Record(); the panel has no threshold of its own.
    /// </summary>
    public partial class PerfBudgetPanel : UserControl
    {
        private static readonly Color OverBack = Color.FromArgb(253, 243, 243);
        private static readonly Color OverFore = Color.FromArgb(192, 57, 43);
        private static readonly Color OkFore = Color.FromArgb(31, 138, 76);
        private static readonly Color IdleFore = Color.FromArgb(90, 107, 125);
        private static readonly Font ValueFont = new Font("monospace", 9F, FontStyle.Bold);

        private const int MeasuredCell = 2;
        private const int StatusCell = 3;

        public PerfBudgetPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Rebinds every row and returns how long it took — that number is the "Binding refresh" measurement.
        /// </summary>
        public long Bind(IReadOnlyList<BudgetRow> rows)
        {
            var watch = Stopwatch.StartNew();

            this.dgvBudgets.Rows.Clear();
            foreach (BudgetRow row in rows)
            {
                int index = this.dgvBudgets.Rows.Add(row.Operation, row.BudgetText, row.MeasuredText, row.StatusText);
                Style(this.dgvBudgets.Rows[index], row);
            }

            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        /// <summary>Updates one row in place (used by the live tick so the whole grid is not rebuilt every second).</summary>
        public void UpdateRow(BudgetRow row)
        {
            if (row == null)
                return;

            for (int i = 0; i < this.dgvBudgets.Rows.Count; i++)
            {
                DataGridViewRow gridRow = this.dgvBudgets.Rows[i];
                if (gridRow.Cells[0].Value as string != row.Operation)
                    continue;

                gridRow.Cells[MeasuredCell].Value = row.MeasuredText;
                gridRow.Cells[StatusCell].Value = row.StatusText;
                Style(gridRow, row);
                return;
            }
        }

        private static void Style(DataGridViewRow gridRow, BudgetRow row)
        {
            Color fore = row.IsOver ? OverFore : (row.Status == BudgetStatus.Ok ? OkFore : IdleFore);

            gridRow.DefaultCellStyle.BackColor = row.IsOver ? OverBack : Color.White;
            gridRow.Cells[MeasuredCell].Style.ForeColor = fore;
            gridRow.Cells[MeasuredCell].Style.Font = ValueFont;
            gridRow.Cells[StatusCell].Style.ForeColor = fore;
            gridRow.Cells[StatusCell].Style.Font = ValueFont;
        }
    }
}
