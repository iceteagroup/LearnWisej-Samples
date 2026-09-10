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
            this.cboStatusFilter = new Wisej.Web.ComboBox();
            this.btnApply = new Wisej.Web.Button();
            this.btnSwitchEngine = new Wisej.Web.Button();
            this.btnAddCards = new Wisej.Web.Button();
            this.btnCellCollision = new Wisej.Web.Button();
            this.btnNoWrap = new Wisej.Web.Button();
            this.btnRestore = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.lblProgress = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // Row 1: title · search (FillWeight 1) · status filter · Apply (FlowBreak)
            //
            // No child sets Location, Dock or Anchor: the flow engine would ignore them. Each child's
            // Margin (0,0,8,6) is the gap to its neighbours — the flow engine honours Margin, Dock does not.
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(250, 30);
            this.lblAppTitle.Text = "Adaptive Operations Console";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtSearch  (FillWeight 1: stretches into the spare width of row 1; MinimumSize 180 stops it collapsing)
            //
            this.txtSearch.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.txtSearch.MinimumSize = new System.Drawing.Size(180, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(240, 30);
            this.txtSearch.Watermark = "Search id, title, owner or notes — /regex/ allowed";
            this.txtSearch.KeyDown += new Wisej.Web.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // cboStatusFilter
            //
            this.cboStatusFilter.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatusFilter.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.cboStatusFilter.Name = "cboStatusFilter";
            this.cboStatusFilter.Size = new System.Drawing.Size(160, 30);
            //
            // btnApply  (FlowBreak: the NEXT child starts a new row, whatever the width)
            //
            this.btnApply.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 30);
            this.btnApply.Text = "Apply";
            this.btnApply.ToolTipText = "Filter the ticket grid by the search text and the status. A /regex/ that does not parse is the failure case.";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // Row 2: the lab buttons (success · progress · failure · failure · recovery) and the progress label (FillWeight 1)
            //
            this.btnSwitchEngine.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnSwitchEngine.Name = "btnSwitchEngine";
            this.btnSwitchEngine.Size = new System.Drawing.Size(124, 30);
            this.btnSwitchEngine.Text = "Switch engine";
            this.btnSwitchEngine.ToolTipText = "Success path: move the metric cards Flow, Table, Flex and log the card sizes each engine produces.";
            this.btnSwitchEngine.Click += new System.EventHandler(this.btnSwitchEngine_Click);
            this.btnAddCards.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnAddCards.Name = "btnAddCards";
            this.btnAddCards.Size = new System.Drawing.Size(100, 30);
            this.btnAddCards.Text = "Add cards";
            this.btnAddCards.ToolTipText = "Progress path: a Wisej.Web.Timer adds one metric card per tick. Flow wraps, Table grows rows, Flex redistributes the weights.";
            this.btnAddCards.Click += new System.EventHandler(this.btnAddCards_Click);
            this.btnCellCollision.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnCellCollision.Name = "btnCellCollision";
            this.btnCellCollision.Size = new System.Drawing.Size(114, 30);
            this.btnCellCollision.Text = "Cell collision";
            this.btnCellCollision.ToolTipText = "Failure path: Controls.Add(card, 0, 0) into the TableLayoutPanel cell that already holds the Open card.";
            this.btnCellCollision.Click += new System.EventHandler(this.btnCellCollision_Click);
            this.btnNoWrap.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnNoWrap.Name = "btnNoWrap";
            this.btnNoWrap.Size = new System.Drawing.Size(140, 30);
            this.btnNoWrap.Text = "No-wrap overflow";
            this.btnNoWrap.ToolTipText = "Failure path: WrapContents = false on the Flow host and on this bar. The row overflows and is clipped (or scrolled).";
            this.btnNoWrap.Click += new System.EventHandler(this.btnNoWrap_Click);
            this.btnRestore.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(90, 30);
            this.btnRestore.Text = "Restore";
            this.btnRestore.ToolTipText = "Recovery: default arrangement. Flow engine, four cards, wrapping on, filters cleared.";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            this.btnClearTrace.Margin = new Wisej.Web.Padding(0, 0, 8, 6);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(100, 30);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // lblProgress  (FillWeight 1 on row 2)
            //
            this.lblProgress.AutoEllipsis = true;
            this.lblProgress.AutoSize = false;
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblProgress.Margin = new Wisej.Web.Padding(0, 0, 0, 6);
            this.lblProgress.MinimumSize = new System.Drawing.Size(120, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(300, 30);
            this.lblProgress.Text = "";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FilterBar  (FlowLayoutPanel)
            //
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblAppTitle);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.cboStatusFilter);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSwitchEngine);
            this.Controls.Add(this.btnAddCards);
            this.Controls.Add(this.btnCellCollision);
            this.Controls.Add(this.btnNoWrap);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnClearTrace);
            this.Controls.Add(this.lblProgress);
            this.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.Name = "FilterBar";
            this.Padding = new Wisej.Web.Padding(8, 8, 8, 2);
            this.SetFillWeight(this.txtSearch, 1);
            this.SetFlowBreak(this.btnApply, true);
            this.SetFillWeight(this.lblProgress, 1);
            this.Size = new System.Drawing.Size(1332, 88);
            this.WrapContents = true;
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.ComboBox cboStatusFilter;
        private Wisej.Web.Button btnApply;
        private Wisej.Web.Button btnSwitchEngine;
        private Wisej.Web.Button btnAddCards;
        private Wisej.Web.Button btnCellCollision;
        private Wisej.Web.Button btnNoWrap;
        private Wisej.Web.Button btnRestore;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Label lblProgress;
    }
}
