namespace TicketOps.Views
{
    partial class WorkOrdersView
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
            this.components = new System.ComponentModel.Container();
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelCount = new Wisej.Web.Label();
            this.searchBox = new TicketOps.Controls.GlobalSearchBox();
            this.labelShortcutHint = new Wisej.Web.Label();
            this.buttonCopyLink = new Wisej.Web.Button();
            this.gridWorkOrders = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAssigned = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnVisibility = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.labelLinkCaption = new Wisej.Web.Label();
            this.textLink = new Wisej.Web.TextBox();
            this.labelAuditCaption = new Wisej.Web.Label();
            this.listAudit = new Wisej.Web.ListBox();
            this.javaScript = new Wisej.Web.JavaScript(this.components);
            this.panelScreen.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelCount);
            this.panelScreen.Controls.Add(this.searchBox);
            this.panelScreen.Controls.Add(this.labelShortcutHint);
            this.panelScreen.Controls.Add(this.buttonCopyLink);
            this.panelScreen.Controls.Add(this.gridWorkOrders);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.labelLinkCaption);
            this.panelScreen.Controls.Add(this.textLink);
            this.panelScreen.Controls.Add(this.labelAuditCaption);
            this.panelScreen.Controls.Add(this.listAudit);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 544);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Work Orders";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelCount
            //
            this.labelCount.AutoSize = false;
            this.labelCount.Font = new System.Drawing.Font("default", 9F);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCount.Location = new System.Drawing.Point(24, 48);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(400, 20);
            this.labelCount.Text = "loading…";
            //
            // searchBox  (Controls/GlobalSearchBox: TextBox + [WebMethod] ReportShortcut)
            //
            this.searchBox.Location = new System.Drawing.Point(24, 92);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(380, 34);
            this.searchBox.Watermark = "Search work orders…";
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.searchBox.ShortcutPressed += new System.EventHandler<TicketOps.Controls.ShortcutEventArgs>(this.searchBox_ShortcutPressed);
            //
            // javaScript  (Wisej.Web.JavaScript extender: runs when the widget is created — after it exists)
            //
            this.javaScript.SetJavaScript(this.searchBox, "ticketOps.attachSearchShortcuts(this);");
            //
            // labelShortcutHint
            //
            this.labelShortcutHint.AutoSize = false;
            this.labelShortcutHint.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelShortcutHint.ForeColor = System.Drawing.Color.FromArgb(106, 125, 146);
            this.labelShortcutHint.Location = new System.Drawing.Point(412, 98);
            this.labelShortcutHint.Name = "labelShortcutHint";
            this.labelShortcutHint.Size = new System.Drawing.Size(180, 22);
            this.labelShortcutHint.Text = "Ctrl + K";
            //
            // buttonCopyLink
            //
            this.buttonCopyLink.Location = new System.Drawing.Point(606, 92);
            this.buttonCopyLink.Name = "buttonCopyLink";
            this.buttonCopyLink.Size = new System.Drawing.Size(130, 34);
            this.buttonCopyLink.Text = "⧉ Copy link";
            this.buttonCopyLink.Click += new System.EventHandler(this.buttonCopyLink_Click);
            //
            // gridWorkOrders
            //
            this.gridWorkOrders.AllowUserToAddRows = false;
            this.gridWorkOrders.AllowUserToDeleteRows = false;
            this.gridWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnPriority,
                this.columnAssigned,
                this.columnVisibility});
            this.gridWorkOrders.Location = new System.Drawing.Point(24, 138);
            this.gridWorkOrders.MultiSelect = false;
            this.gridWorkOrders.Name = "gridWorkOrders";
            this.gridWorkOrders.ReadOnly = true;
            this.gridWorkOrders.RowHeadersVisible = false;
            this.gridWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridWorkOrders.Size = new System.Drawing.Size(712, 190);
            this.gridWorkOrders.SelectionChanged += new System.EventHandler(this.gridWorkOrders_SelectionChanged);
            //
            // columns
            //
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 70;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 300;
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 90;
            this.columnAssigned.HeaderText = "Assigned";
            this.columnAssigned.Name = "columnAssigned";
            this.columnAssigned.ReadOnly = true;
            this.columnAssigned.Width = 130;
            this.columnVisibility.HeaderText = "Visibility";
            this.columnVisibility.Name = "columnVisibility";
            this.columnVisibility.ReadOnly = true;
            this.columnVisibility.Width = 110;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 336);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 22);
            this.labelSelected.Text = "Select a work order";
            //
            // labelLinkCaption
            //
            this.labelLinkCaption.AutoSize = false;
            this.labelLinkCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelLinkCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelLinkCaption.Location = new System.Drawing.Point(24, 364);
            this.labelLinkCaption.Name = "labelLinkCaption";
            this.labelLinkCaption.Size = new System.Drawing.Size(712, 18);
            this.labelLinkCaption.Text = "LINK";
            //
            // textLink
            //
            this.textLink.Font = new System.Drawing.Font("monospace", 9F);
            this.textLink.Location = new System.Drawing.Point(24, 384);
            this.textLink.Name = "textLink";
            this.textLink.ReadOnly = true;
            this.textLink.Size = new System.Drawing.Size(712, 30);
            //
            // labelAuditCaption
            //
            this.labelAuditCaption.AutoSize = false;
            this.labelAuditCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelAuditCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelAuditCaption.Location = new System.Drawing.Point(24, 424);
            this.labelAuditCaption.Name = "labelAuditCaption";
            this.labelAuditCaption.Size = new System.Drawing.Size(712, 18);
            this.labelAuditCaption.Text = "AUDIT LOG";
            //
            // listAudit
            //
            this.listAudit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listAudit.Font = new System.Drawing.Font("monospace", 9F);
            this.listAudit.Location = new System.Drawing.Point(24, 444);
            this.listAudit.Name = "listAudit";
            this.listAudit.Size = new System.Drawing.Size(712, 76);
            //
            // WorkOrdersView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 604);
            this.Controls.Add(this.panelScreen);
            this.Name = "WorkOrdersView";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.WorkOrdersView_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelCount;
        private TicketOps.Controls.GlobalSearchBox searchBox;
        private Wisej.Web.Label labelShortcutHint;
        private Wisej.Web.Button buttonCopyLink;
        private Wisej.Web.DataGridView gridWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnAssigned;
        private Wisej.Web.DataGridViewTextBoxColumn columnVisibility;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.Label labelLinkCaption;
        private Wisej.Web.TextBox textLink;
        private Wisej.Web.Label labelAuditCaption;
        private Wisej.Web.ListBox listAudit;
        private Wisej.Web.JavaScript javaScript;
    }
}
