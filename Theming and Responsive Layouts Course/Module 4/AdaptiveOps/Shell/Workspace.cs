using System;
using System.Collections.Generic;
using System.Globalization;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Centre workspace region of the console: metric cards (Dock Top) above the ticket grid
    /// (Dock Fill, MinimumSize 300×160). The shell uses a few members (metrics, tickets, selection,
    /// title) and never reaches a card, a column or a row.
    /// </summary>
    public partial class Workspace : UserControl
    {
        private bool _suppressSelection;

        public Workspace()
        {
            InitializeComponent();
        }

        /// <summary>Raised when the user selects another ticket row.</summary>
        public event EventHandler SelectionChanged;

        /// <summary>Title above the grid ("Tickets", "Reports", …).</summary>
        public string Title
        {
            get => this.lblWorkspaceTitle.Text;
            set => this.lblWorkspaceTitle.Text = value;
        }

        /// <summary>Id of the selected ticket row, or null.</summary>
        public string SelectedTicketId
        {
            get
            {
                var row = this.gridTickets.SelectedRows.Count > 0
                    ? this.gridTickets.SelectedRows[0]
                    : this.gridTickets.CurrentRow;
                return row?.Tag as string;
            }
        }

        public void SetMetrics(int open, int overdue, int mine, int closedThisWeek)
        {
            this.lblOpenValue.Text = N(open);
            this.lblOverdueValue.Text = N(overdue);
            this.lblMineValue.Text = N(mine);
            this.lblClosedValue.Text = N(closedThisWeek);
        }

        /// <summary>
        /// Refills the grid and re-selects <paramref name="keepSelectedId"/> (or the first row).
        /// Returns the id that ended up selected, or null when the grid is empty.
        /// </summary>
        public string ShowTickets(IReadOnlyList<Ticket> tickets, string keepSelectedId)
        {
            int reselect = -1;

            _suppressSelection = true;
            try
            {
                this.gridTickets.Rows.Clear();
                foreach (var t in tickets)
                {
                    int index = this.gridTickets.Rows.Add(new object[]
                    {
                        t.Id,
                        t.Title,
                        t.Priority.ToString(),
                        t.Status.ToString(),
                        t.Owner,
                        t.DueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                    });
                    this.gridTickets.Rows[index].Tag = t.Id;
                    if (t.Id == keepSelectedId)
                        reselect = index;
                }

                if (reselect < 0 && this.gridTickets.Rows.Count > 0)
                    reselect = 0;

                this.gridTickets.ClearSelection();
                if (reselect >= 0)
                    this.gridTickets.Rows[reselect].Selected = true;
            }
            finally
            {
                _suppressSelection = false;
            }

            return reselect >= 0 ? (string)this.gridTickets.Rows[reselect].Tag : null;
        }

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
