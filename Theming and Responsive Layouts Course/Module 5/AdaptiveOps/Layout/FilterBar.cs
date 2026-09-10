using System;
using System.Collections.Generic;
using Wisej.Web;

namespace AdaptiveOps.Layout
{
    /// <summary>The commands the FilterBar raises; the page decides what each one does.</summary>
    public enum FilterBarCommand
    {
        Apply,
        SwitchEngine,
        AddCards,
        CellCollision,
        NoWrapOverflow,
        Restore,
        ClearTrace
    }

    public sealed class FilterBarCommandEventArgs : EventArgs
    {
        public FilterBarCommandEventArgs(FilterBarCommand command)
        {
            Command = command;
        }

        public FilterBarCommand Command { get; }
    }

    /// <summary>
    /// The toolbar / filter bar as a FlowLayoutPanel (layout in FilterBar.Designer.cs). The children
    /// flow left to right and wrap; none of them sets Location, Dock or Anchor — the flow engine ignores
    /// all three. The extended properties do the work: the search box has FillWeight 1 (it stretches into
    /// whatever width its row has left, never below its MinimumSize of 180), the Apply button has
    /// FlowBreak (the lab buttons that follow always start a new row, whatever the width), and the
    /// progress label has FillWeight 1 on that second row. Every child's Margin is honoured by the flow engine.
    /// </summary>
    public partial class FilterBar : FlowLayoutPanel
    {
        /// <summary>Raised for every button; <see cref="FilterBarCommandEventArgs.Command"/> says which one.</summary>
        public event EventHandler<FilterBarCommandEventArgs> Command;

        public const string AllStatuses = "All statuses";

        public FilterBar()
        {
            InitializeComponent();

            this.cboStatusFilter.Items.Add(AllStatuses);
            this.cboStatusFilter.Items.AddRange(Enum.GetNames(typeof(Models.TicketStatus)));
            this.cboStatusFilter.SelectedIndex = 0;
        }

        /// <summary>The search box (exposed for the acceptance check: FillWeight 1, MinimumSize 180).</summary>
        public TextBox SearchBox => this.txtSearch;

        /// <summary>The Apply button (exposed for the acceptance check: FlowBreak true).</summary>
        public Button ApplyButton => this.btnApply;

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
                var text = this.cboStatusFilter.SelectedItem as string;
                return string.IsNullOrEmpty(text) || text == AllStatuses ? null : text;
            }
            set => this.cboStatusFilter.SelectedItem = value ?? AllStatuses;
        }

        /// <summary>Text of the progress label on the second row.</summary>
        public string ProgressText
        {
            get => this.lblProgress.Text;
            set => this.lblProgress.Text = value;
        }

        /// <summary>Disabled while the Add-cards timer runs.</summary>
        public bool AddCardsEnabled
        {
            get => this.btnAddCards.Enabled;
            set => this.btnAddCards.Enabled = value;
        }

        /// <summary>
        /// One trace line with the extended properties as the container stores them, read back with the
        /// Get* counterparts: "FlowLayoutPanel LeftToRight · WrapContents True · FillWeight(search)=1 · FlowBreak(apply)=True …".
        /// </summary>
        public string Describe()
        {
            return $"FlowLayoutPanel {this.FlowDirection} · WrapContents {this.WrapContents} · AutoScroll {this.AutoScroll} · Padding {this.Padding.Left} · "
                 + $"FillWeight(txtSearch)={this.GetFillWeight(this.txtSearch)} min {this.txtSearch.MinimumSize.Width} → {this.txtSearch.Width} px · "
                 + $"FlowBreak(btnApply)={this.GetFlowBreak(this.btnApply)} · FillWeight(lblProgress)={this.GetFillWeight(this.lblProgress)} → {this.lblProgress.Width} px · "
                 + $"{this.Controls.Count} children in {CountRows()} row(s) · bar {this.Width}×{this.Height}";
        }

        /// <summary>Rows = distinct Top values of the children (the flow engine writes the bounds back).</summary>
        public int CountRows()
        {
            var tops = new HashSet<int>();
            foreach (Control c in this.Controls)
                if (c.Visible)
                    tops.Add(c.Top);
            return tops.Count;
        }

        /// <summary>Right edge of the right-most child minus the client width: positive means the row overflows (clipped or scrolled).</summary>
        public int Overflow()
        {
            int right = 0;
            foreach (Control c in this.Controls)
                if (c.Visible && c.Right > right)
                    right = c.Right;
            return right - this.ClientSize.Width;
        }

        private void btnApply_Click(object sender, EventArgs e) => Raise(FilterBarCommand.Apply);
        private void btnSwitchEngine_Click(object sender, EventArgs e) => Raise(FilterBarCommand.SwitchEngine);
        private void btnAddCards_Click(object sender, EventArgs e) => Raise(FilterBarCommand.AddCards);
        private void btnCellCollision_Click(object sender, EventArgs e) => Raise(FilterBarCommand.CellCollision);
        private void btnNoWrap_Click(object sender, EventArgs e) => Raise(FilterBarCommand.NoWrapOverflow);
        private void btnRestore_Click(object sender, EventArgs e) => Raise(FilterBarCommand.Restore);
        private void btnClearTrace_Click(object sender, EventArgs e) => Raise(FilterBarCommand.ClearTrace);

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Raise(FilterBarCommand.Apply);
        }

        private void Raise(FilterBarCommand command)
        {
            Command?.Invoke(this, new FilterBarCommandEventArgs(command));
        }
    }
}
