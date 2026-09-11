namespace AdaptiveOps.Layout
{
    partial class FilterBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.lblAppTitle = new Wisej.Web.Label();
            this.txtSearch = new Wisej.Web.TextBox();
            this.cmbStatus = new Wisej.Web.ComboBox();
            this.btnApply = new Wisej.Web.Button();
            this.cardOpen = new AdaptiveOps.Shell.MetricCard();
            this.cardOverdue = new AdaptiveOps.Shell.MetricCard();
            this.cardMine = new AdaptiveOps.Shell.MetricCard();
            this.cardClosed = new AdaptiveOps.Shell.MetricCard();
            this.SuspendLayout();
            //
            // lblAppTitle
            //
            // No child sets Location, Dock or Anchor: the flow engine ignores them. Each child's Margin
            // is the gap to its neighbours; the flow engine honours Margin.
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtSearch  (FillWeight 1: stretches into the spare width of its row; MinimumSize 180 stops it collapsing)
            //
            this.txtSearch.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.txtSearch.MinimumSize = new System.Drawing.Size(180, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(240, 30);
            this.txtSearch.Watermark = "Search tickets";
            this.txtSearch.KeyDown += new Wisej.Web.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // cmbStatus
            //
            this.cmbStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbStatus.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(160, 30);
            //
            // btnApply  (FlowBreak: the metric cards that follow always start a new row)
            //
            this.btnApply.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 30);
            this.btnApply.Text = "Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // cardOpen
            //
            this.cardOpen.Accent = System.Drawing.Color.FromArgb(36, 84, 166);
            this.cardOpen.Name = "cardOpen";
            this.cardOpen.Title = "Open";
            //
            // cardOverdue
            //
            this.cardOverdue.Accent = System.Drawing.Color.FromArgb(180, 35, 24);
            this.cardOverdue.Name = "cardOverdue";
            this.cardOverdue.Title = "Overdue";
            //
            // cardMine
            //
            this.cardMine.Accent = System.Drawing.Color.FromArgb(181, 71, 8);
            this.cardMine.Name = "cardMine";
            this.cardMine.Title = "Assigned to me";
            //
            // cardClosed
            //
            this.cardClosed.Accent = System.Drawing.Color.FromArgb(2, 122, 72);
            this.cardClosed.Name = "cardClosed";
            this.cardClosed.Title = "Closed this week";
            //
            // FilterBar
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblAppTitle);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.cardOpen);
            this.Controls.Add(this.cardOverdue);
            this.Controls.Add(this.cardMine);
            this.Controls.Add(this.cardClosed);
            this.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.Name = "FilterBar";
            this.Padding = new Wisej.Web.Padding(8, 8, 8, 2);
            this.SetFillWeight(this.txtSearch, 1);
            this.SetFlowBreak(this.btnApply, true);
            this.Size = new System.Drawing.Size(1332, 130);
            this.WrapContents = true;
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.ComboBox cmbStatus;
        private Wisej.Web.Button btnApply;
        private AdaptiveOps.Shell.MetricCard cardOpen;
        private AdaptiveOps.Shell.MetricCard cardOverdue;
        private AdaptiveOps.Shell.MetricCard cardMine;
        private AdaptiveOps.Shell.MetricCard cardClosed;
    }
}
