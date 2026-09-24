using System;
using System.Collections.Generic;
using System.Drawing;
using VisualOperationsStudio.Controls;
using VisualOperationsStudio.Models;
using Wisej.Web;

namespace VisualOperationsStudio
{
    /// <summary>
    /// The operations grid: a thousand bound machines, of which only Health and Trend are painted.
    /// One <c>UserPaint</c> switch per column and one <c>CellPaint</c> subscription for the whole grid -
    /// not one control per row. The Status column builds the same state a second time through
    /// <c>AllowHtml</c>, so the two techniques can be scrolled side by side and compared.
    /// </summary>
    public partial class VisualOperationsPage : Page
    {
        private readonly List<MachineStatus> machines = MachineStatusGenerator.Generate(1000);

        public VisualOperationsPage()
        {
            InitializeComponent();

            ConfigureUserPaintedColumns();
            this.gridOperations.DataSource = this.machines;
            this.lblFooter.Text = $"{this.machines.Count:N0} machines bound · painted cells: Health, Trend";
        }

        private void VisualOperationsPage_Load(object sender, EventArgs e)
        {
            this.gridOperations.Scroll += this.gridOperations_Scroll;
        }

        /// <summary>
        /// User painting comes before cell painting: a column is switched into user-painted mode, and
        /// only then does the grid raise CellPaint for it. The handler is subscribed once, here.
        /// </summary>
        private void ConfigureUserPaintedColumns()
        {
            this.colHealth.UserPaint = true;
            this.colTrend.UserPaint = true;
            this.gridOperations.CellPaint += this.gridOperations_CellPaint;
        }

        /// <summary>
        /// One handler for the whole grid. It returns at once for a header row or a column it does not
        /// own, then takes its geometry from the clip rectangle and the row's values from the bound
        /// object in a single step. No allocation beyond the two renderers, no control creation, no
        /// side effects: this runs again for every cell that scrolls into view, on request threads.
        /// </summary>
        private void gridOperations_CellPaint(object sender, DataGridViewCellPaintEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var isHealth = e.ColumnIndex == this.colHealth.Index;
            var isTrend = e.ColumnIndex == this.colTrend.Index;
            if (!isHealth && !isTrend)
                return;

            var row = this.gridOperations.Rows[e.RowIndex];
            var machine = row.DataBoundItem as MachineStatus;
            if (machine == null)
                return;

            var area = Rectangle.Inflate(e.ClipRectangle, -12, -8);

            if (isHealth)
                OperationsCellRenderer.DrawHealthBar(e.Graphics, area, machine.Health, machine.Tone, row.Selected);
            else
                OperationsCellRenderer.DrawSparkline(e.Graphics, SparklineBounds(e.ClipRectangle), machine.RecentReadings, machine.Tone, row.Selected);
        }

        /// <summary>The walkthrough's Trend cell: a 94 x 18 line, left aligned inside the 118 px column.</summary>
        private static Rectangle SparklineBounds(Rectangle cell)
        {
            var width = Math.Min(94, Math.Max(0, cell.Width - 24));
            return new Rectangle(cell.X + 12, cell.Y + (cell.Height - 18) / 2, width, 18);
        }

        /// <summary>The footer says which rows are on screen, which is the number the lab asks you to watch.</summary>
        private void gridOperations_Scroll(object sender, ScrollEventArgs e)
        {
            var first = this.gridOperations.FirstDisplayedRowIndex;
            if (first < 0)
                return;

            var visible = Math.Max(1, this.gridOperations.VisibleRowCount);
            var last = Math.Min(this.machines.Count, first + visible);

            this.lblFooter.Text =
                $"rows {first + 1:N0}–{last:N0} of {this.machines.Count:N0} · painted cells: Health, Trend";
        }
    }
}
