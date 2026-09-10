using System;
using System.Collections.Generic;
using System.Globalization;
using AdaptiveOps.Lab;
using AdaptiveOps.Models;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Centre workspace region of the console (Dock = Fill in MainPage, MinimumSize on the region).
    /// Its local layout — metric cards (Dock Top), the error banner (Dock Top, hidden), the tabbed
    /// ticket grid (Dock Fill, grid MinimumSize 300×160) and the "Layout &amp; theme · live trace"
    /// card (Dock Bottom) — lives in Workspace.Designer.cs. The second tab hosts the module's
    /// "before": <see cref="ResizeCodeTwin"/>, a shell positioned by a Resize handler.
    ///
    /// The shell talks to the workspace through a handful of members (metrics, tickets, selection,
    /// banner, title, trace) and never reaches a card, a column or the list box.
    /// </summary>
    public partial class Workspace : UserControl
    {
        /// <summary>True while the grid is being refilled, so SelectionChanged does not re-enter.</summary>
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

        /// <summary>The four metric cards.</summary>
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

        /// <summary>The banner is a docked Panel hidden by default: showing it pushes the tabs down, no Bounds involved.</summary>
        public void ShowBanner(string text)
        {
            this.lblBanner.Text = text;
            this.bannerPanel.Visible = true;
        }

        public void HideBanner()
        {
            this.bannerPanel.Visible = false;
            this.lblBanner.Text = string.Empty;
        }

        /// <summary>
        /// Appends one line to the "Layout &amp; theme · live trace" card and selects it.
        /// Lines start with "→ " (server to client), "← " (client to server), "• " (server decision)
        /// or "✕ " (a deliberate anti-pattern being shown).
        /// </summary>
        public void AddTrace(string line)
        {
            while (this.listTrace.Items.Count >= 400)
                this.listTrace.Items.RemoveAt(0);

            string time = DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture);
            this.listTrace.Items.Add($"{time}  {line}");
            this.listTrace.SelectedIndex = this.listTrace.Items.Count - 1;
        }

        public void ClearTrace()
        {
            this.listTrace.Items.Clear();
        }

        /// <summary>One line for the trace: the grid's size and its MinimumSize guard rail.</summary>
        public string DescribeGrid()
        {
            return $"grid {this.gridTickets.Width}×{this.gridTickets.Height} (MinimumSize {this.gridTickets.MinimumSize.Width}×{this.gridTickets.MinimumSize.Height})";
        }

        /// <summary>Number of times the resize-code twin's handler has run (0 until its tab is shown).</summary>
        public int TwinResizeCount => this.twin.ResizeCount;

        private void gridTickets_SelectionChanged(object sender, EventArgs e)
        {
            if (_suppressSelection)
                return;

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            AddTrace(this.tabs.SelectedIndex == 1
                ? "← client tab: Resize-code twin — five panels with no Dock, Bounds recomputed in a Resize handler; drag the browser edge and watch this trace"
                : "← client tab: Tickets — the docked grid; nothing runs on resize except the reporting below");
        }

        private void twin_Traced(object sender, TraceEventArgs e)
        {
            AddTrace(e.Line);
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
