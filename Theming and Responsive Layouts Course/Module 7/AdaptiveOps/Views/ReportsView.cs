using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Views
{
    /// <summary>
    /// The Reports view: open and overdue tickets per owner in a three-column <see cref="TableLayoutPanel"/>.
    /// It is created lazily by <c>MainPage.ShowView("Reports")</c> the first time the rail asks for it —
    /// a page that starts with the dashboard only pays for the report controls when somebody needs them,
    /// and the "Layout cost" card shows the difference. The rows are data-driven, so they are built in
    /// <see cref="Bind"/> rather than in the Designer.
    /// </summary>
    public partial class ReportsView : UserControl
    {
        private readonly List<Control> _rowControls = new List<Control>();

        public ReportsView()
        {
            InitializeComponent();
        }

        /// <summary>Rebuilds the table from the tickets. Returns the number of controls the rows use.</summary>
        public int Bind(IReadOnlyList<Ticket> tickets, DateTime today, string createdNote)
        {
            foreach (var c in _rowControls)
            {
                this.tableReport.Controls.Remove(c);
                c.Dispose();
            }
            _rowControls.Clear();

            var groups = tickets.GroupBy(t => t.Owner)
                                .Select(g => new { Owner = g.Key, Open = g.Count(t => t.IsOpen), Overdue = g.Count(t => t.IsOverdue(today)) })
                                .OrderByDescending(g => g.Open).ThenBy(g => g.Owner, StringComparer.Ordinal)
                                .ToList();

            this.tableReport.SuspendLayout();
            try
            {
                this.tableReport.RowCount = groups.Count + 1;
                while (this.tableReport.RowStyles.Count < this.tableReport.RowCount)
                    this.tableReport.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));

                int row = 1;
                foreach (var g in groups)
                {
                    AddCell(g.Owner, 0, row, "muted-label");
                    AddCell(g.Open.ToString(CultureInfo.InvariantCulture), 1, row, "subheading-label");
                    AddCell(g.Overdue.ToString(CultureInfo.InvariantCulture), 2, row, g.Overdue > 0 ? "status-label" : "muted-label", g.Overdue > 0 ? "error" : null);
                    row++;
                }
            }
            finally
            {
                this.tableReport.ResumeLayout(true);
            }

            this.lblNote.Text = createdNote;
            return _rowControls.Count;
        }

        private void AddCell(string text, int column, int row, string appearanceKey, string state = null)
        {
            var label = new Label
            {
                AppearanceKey = appearanceKey,
                AutoSize = false,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Name = $"cell_{row}_{column}",
                TabStop = false,
                Text = text,
                TextAlign = column == 0 ? System.Drawing.ContentAlignment.MiddleLeft : System.Drawing.ContentAlignment.MiddleRight
            };
            if (state != null)
                label.AddState(state);

            this.tableReport.Controls.Add(label, column, row);
            _rowControls.Add(label);
        }
    }
}
