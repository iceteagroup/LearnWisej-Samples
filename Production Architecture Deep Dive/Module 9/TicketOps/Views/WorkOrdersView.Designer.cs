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
            this.labelClipboardCaption = new Wisej.Web.Label();
            this.textLink = new Wisej.Web.TextBox();
            this.labelAuditCaption = new Wisej.Web.Label();
            this.listAudit = new Wisej.Web.ListBox();
            this.progressBatch = new Wisej.Web.ProgressBar();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonBatch = new Wisej.Web.Button();
            this.buttonCopyUnknown = new Wisej.Web.Button();
            this.buttonCopyConfidential = new Wisej.Web.Button();
            this.buttonClipboardDenied = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerBatch = new Wisej.Web.Timer(this.components);
            this.javaScript = new Wisej.Web.JavaScript(this.components);
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Work Orders: search + grid + copy link — display and input only)
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
            this.panelScreen.Controls.Add(this.labelClipboardCaption);
            this.panelScreen.Controls.Add(this.textLink);
            this.panelScreen.Controls.Add(this.labelAuditCaption);
            this.panelScreen.Controls.Add(this.listAudit);
            this.panelScreen.Controls.Add(this.progressBatch);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
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
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
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
            this.searchBox.Watermark = "Search work orders…  (Ctrl+K from anywhere)";
            this.searchBox.ToolTipText = "Global search. Ctrl+K focuses it in the browser (embedded script via the JavaScript extender); the server is told through the [WebMethod] ReportShortcut. Esc clears it.";
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
            this.labelShortcutHint.Text = "Ctrl + K  ·  ? = shortcuts";
            //
            // buttonCopyLink
            //
            this.buttonCopyLink.Location = new System.Drawing.Point(606, 92);
            this.buttonCopyLink.Name = "buttonCopyLink";
            this.buttonCopyLink.Size = new System.Drawing.Size(130, 34);
            this.buttonCopyLink.Text = "⧉ Copy link";
            this.buttonCopyLink.ToolTipText = "Success path: ITicketLinkService.BuildLinkAsync(id) → BrowserApi.CopyToClipboardAsync (navigator.clipboard.writeText, awaited) → ConfirmCopiedAsync (audit) only after the browser confirmed";
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
            // clipboard chip: the last link the SERVER built (also the manual fallback)
            //
            this.labelClipboardCaption.AutoSize = false;
            this.labelClipboardCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelClipboardCaption.ForeColor = System.Drawing.Color.FromArgb(31, 138, 76);
            this.labelClipboardCaption.Location = new System.Drawing.Point(24, 364);
            this.labelClipboardCaption.Name = "labelClipboardCaption";
            this.labelClipboardCaption.Size = new System.Drawing.Size(712, 18);
            this.labelClipboardCaption.Text = "CLIPBOARD · the link the server built and signed (select it here if the browser refuses the copy)";
            this.textLink.Font = new System.Drawing.Font("monospace", 9F);
            this.textLink.Location = new System.Drawing.Point(24, 384);
            this.textLink.Name = "textLink";
            this.textLink.ReadOnly = true;
            this.textLink.Size = new System.Drawing.Size(712, 30);
            this.textLink.Watermark = "no link built yet — select a work order and press Copy link";
            //
            // audit log strip (server)
            //
            this.labelAuditCaption.AutoSize = false;
            this.labelAuditCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.labelAuditCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelAuditCaption.Location = new System.Drawing.Point(24, 424);
            this.labelAuditCaption.Name = "labelAuditCaption";
            this.labelAuditCaption.Size = new System.Drawing.Size(712, 18);
            this.labelAuditCaption.Text = "AUDIT LOG (SERVER) · written only after the browser confirmed the copy";
            this.listAudit.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listAudit.Font = new System.Drawing.Font("monospace", 9F);
            this.listAudit.Location = new System.Drawing.Point(24, 444);
            this.listAudit.Name = "listAudit";
            this.listAudit.Size = new System.Drawing.Size(712, 76);
            //
            // progressBatch
            //
            this.progressBatch.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressBatch.Location = new System.Drawing.Point(24, 530);
            this.progressBatch.Maximum = 6;
            this.progressBatch.Name = "progressBatch";
            this.progressBatch.Size = new System.Drawing.Size(712, 18);
            this.progressBatch.Visible = false;
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            //
            // panelActions  (bottom bar: progress / failures / clipboard denied / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonBatch);
            this.panelActions.Controls.Add(this.buttonCopyUnknown);
            this.panelActions.Controls.Add(this.buttonCopyConfidential);
            this.panelActions.Controls.Add(this.buttonClipboardDenied);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonBatch.Location = new System.Drawing.Point(0, 4);
            this.buttonBatch.Name = "buttonBatch";
            this.buttonBatch.Size = new System.Drawing.Size(170, 36);
            this.buttonBatch.Text = "▶ Verify 6 links";
            this.buttonBatch.ToolTipText = "Progress path: a Timer builds and verifies one link per tick through ITicketLinkService — no clipboard call, because a Timer tick is not a user gesture";
            this.buttonBatch.Click += new System.EventHandler(this.buttonBatch_Click);
            this.buttonCopyUnknown.Location = new System.Drawing.Point(180, 4);
            this.buttonCopyUnknown.Name = "buttonCopyUnknown";
            this.buttonCopyUnknown.Size = new System.Drawing.Size(190, 36);
            this.buttonCopyUnknown.Text = "Copy link for #9999";
            this.buttonCopyUnknown.ToolTipText = "Failure path 1: a forged client id — the service re-loads it, finds nothing and refuses; nothing crosses to the browser";
            this.buttonCopyUnknown.Click += new System.EventHandler(this.buttonCopyUnknown_Click);
            this.buttonCopyConfidential.Location = new System.Drawing.Point(380, 4);
            this.buttonCopyConfidential.Name = "buttonCopyConfidential";
            this.buttonCopyConfidential.Size = new System.Drawing.Size(210, 36);
            this.buttonCopyConfidential.Text = "Copy confidential #2006";
            this.buttonCopyConfidential.ToolTipText = "Failure path 2: WorkOrder.CanShareLink (domain rule) says no — the rule lives on the server, never in the script";
            this.buttonCopyConfidential.Click += new System.EventHandler(this.buttonCopyConfidential_Click);
            this.buttonClipboardDenied.Location = new System.Drawing.Point(600, 4);
            this.buttonClipboardDenied.Name = "buttonClipboardDenied";
            this.buttonClipboardDenied.Size = new System.Drawing.Size(220, 36);
            this.buttonClipboardDenied.Text = "Simulate clipboard denied";
            this.buttonClipboardDenied.ToolTipText = "Client-side error path: the script rejects like a browser that denied clipboard permission — no audit entry, a fallback sentence, the link stays selectable. Click again to restore and copy successfully.";
            this.buttonClipboardDenied.Click += new System.EventHandler(this.buttonClipboardDenied_Click);
            this.buttonOutage.Location = new System.Drawing.Point(830, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path: the repository throws (also while Copy link re-checks the id); the log gets the details, the user gets a safe message. Click again to recover.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerBatch
            //
            this.timerBatch.Interval = 350;
            this.timerBatch.Tick += new System.EventHandler(this.timerBatch_Tick);
            //
            // WorkOrdersView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "WorkOrdersView";
            this.Text = "TicketOps Console — Module 9 · JavaScript interop";
            this.Load += new System.EventHandler(this.WorkOrdersView_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
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
        private Wisej.Web.Label labelClipboardCaption;
        private Wisej.Web.TextBox textLink;
        private Wisej.Web.Label labelAuditCaption;
        private Wisej.Web.ListBox listAudit;
        private Wisej.Web.ProgressBar progressBatch;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonBatch;
        private Wisej.Web.Button buttonCopyUnknown;
        private Wisej.Web.Button buttonCopyConfidential;
        private Wisej.Web.Button buttonClipboardDenied;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerBatch;
        private Wisej.Web.JavaScript javaScript;
    }
}
