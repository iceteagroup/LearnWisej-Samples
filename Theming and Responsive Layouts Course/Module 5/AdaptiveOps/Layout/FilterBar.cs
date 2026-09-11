using System;
using System.Globalization;
using Wisej.Web;

namespace AdaptiveOps.Layout
{
    /// <summary>
    /// The filter and metric-card area as a FlowLayoutPanel (layout in FilterBar.Designer.cs). The
    /// children flow left to right and wrap; none of them sets Location, Dock or Anchor. The search box
    /// has FillWeight 1 (it stretches into the spare width of its row, never below its MinimumSize of
    /// 180) and the Apply button has FlowBreak, so the metric cards that follow always start a new row.
    /// </summary>
    public partial class FilterBar : FlowLayoutPanel
    {
        public const string AllStatuses = "All statuses";

        /// <summary>Raised when the user clicks Apply or presses Enter in the search box.</summary>
        public event EventHandler Apply;

        public FilterBar()
        {
            InitializeComponent();

            this.cmbStatus.Items.Add(AllStatuses);
            this.cmbStatus.Items.AddRange(Enum.GetNames(typeof(Models.TicketStatus)));
            this.cmbStatus.SelectedIndex = 0;
        }

        /// <summary>Raw search text; "/pattern/" is treated as a regular expression by the page.</summary>
        public string SearchText
        {
            get => this.txtSearch.Text;
            set => this.txtSearch.Text = value;
        }

        /// <summary>The selected status name, or null for "All statuses".</summary>
        public string StatusFilter
        {
            get
            {
                var text = this.cmbStatus.SelectedItem as string;
                return string.IsNullOrEmpty(text) || text == AllStatuses ? null : text;
            }
            set => this.cmbStatus.SelectedItem = value ?? AllStatuses;
        }

        /// <summary>The four metric cards.</summary>
        public void SetMetrics(int open, int overdue, int mine, int closedThisWeek)
        {
            this.cardOpen.Value = N(open);
            this.cardOverdue.Value = N(overdue);
            this.cardMine.Value = N(mine);
            this.cardClosed.Value = N(closedThisWeek);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Apply?.Invoke(this, EventArgs.Empty);
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Apply?.Invoke(this, EventArgs.Empty);
        }

        private static string N(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
