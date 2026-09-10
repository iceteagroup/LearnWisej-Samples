using System;
using System.Globalization;
using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// The "LAYOUT COST" card: how many controls the page tree holds right now against a budget, with a
    /// per-region breakdown in the tooltip. The bar is a two-column <see cref="TableLayoutPanel"/> whose
    /// Percent column styles are set from the data — the layout engine draws the proportion, no Bounds
    /// arithmetic and no CssStyle. The fill and track colours are the <c>cost-bar-fill</c> /
    /// <c>cost-bar-track</c> theme appearances.
    /// </summary>
    public partial class LayoutCostCard : UserControl
    {
        public LayoutCostCard()
        {
            InitializeComponent();
        }

        /// <summary>Upper bound the bar is drawn against. 400 controls is the budget the architecture note sets for one page.</summary>
        public int Budget { get; set; } = 400;

        /// <summary>Total controls currently in the tree.</summary>
        public int Total { get; private set; }

        /// <summary>
        /// Updates value, bar and tooltip. <paramref name="breakdown"/> is the per-region text shown in the tooltip.
        /// </summary>
        public void Update(int total, string breakdown)
        {
            Total = total;
            this.lblValue.Text = total.ToString(CultureInfo.InvariantCulture) + " ctrls";

            int used = Budget <= 0 ? 100 : (int)Math.Round(Math.Min(100.0, 100.0 * total / Budget));
            used = Math.Max(1, used);
            this.costBar.ColumnStyles[0].Width = used;
            this.costBar.ColumnStyles[1].Width = 100 - used;

            string tip = $"Layout cost: {total} controls of a {Budget} budget ({used}%). {breakdown}";
            this.ToolTipText = tip;
            this.AccessibleDescription = tip;
        }
    }
}
