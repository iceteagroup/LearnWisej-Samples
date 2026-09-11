namespace EnterpriseOps.UI
{
    partial class FieldTechnicianPage
    {
        /// <summary>Required designer variable.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Clean up any resources being used.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblHeader = new Wisej.Web.Label();
            this.pnlDevice = new Wisej.Web.Panel();
            this.pnlQueue = new Wisej.Web.Panel();
            this.flpQueue = new Wisej.Web.FlowLayoutPanel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.conflictPanel = new EnterpriseOps.UI.SyncConflictPanel();
            this.pnlSync = new Wisej.Web.Panel();
            this.prgSync = new Wisej.Web.ProgressBar();
            this.lblSyncStatus = new Wisej.Web.Label();
            this.pnlCache = new Wisej.Web.Panel();
            this.dgvCache = new Wisej.Web.DataGridView();
            this.colCacheCode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCacheTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCacheSite = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCacheStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCacheVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCacheLocal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlFieldActions = new Wisej.Web.Panel();
            this.lblNotes = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.btnComplete = new Wisej.Web.Button();
            this.btnScan = new Wisej.Web.Button();
            this.lblScanResult = new Wisej.Web.Label();
            this.lblCacheTitle = new Wisej.Web.Label();
            this.pnlFieldHeader = new Wisej.Web.Panel();
            this.lblFieldTitle = new Wisej.Web.Label();
            this.btnToggleConnection = new Wisej.Web.Button();
            this.lblConnection = new Wisej.Web.Label();
            this.pnlDevice.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlSync.SuspendLayout();
            this.pnlCache.SuspendLayout();
            this.pnlFieldActions.SuspendLayout();
            this.pnlFieldHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // lblHeader
            //
            this.lblHeader.AutoSize = false;
            this.lblHeader.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblHeader.Location = new System.Drawing.Point(24, 12);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(860, 28);
            this.lblHeader.Text = "EnterpriseOps — Field Technician mode";
            //
            // pnlDevice
            //
            this.pnlDevice.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlDevice.BackColor = System.Drawing.Color.White;
            this.pnlDevice.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDevice.Controls.Add(this.pnlQueue);
            this.pnlDevice.Controls.Add(this.conflictPanel);
            this.pnlDevice.Controls.Add(this.pnlSync);
            this.pnlDevice.Controls.Add(this.pnlCache);
            this.pnlDevice.Controls.Add(this.pnlFieldHeader);
            this.pnlDevice.Location = new System.Drawing.Point(24, 50);
            this.pnlDevice.Name = "pnlDevice";
            this.pnlDevice.Size = new System.Drawing.Size(860, 510);
            //
            // pnlQueue
            //
            this.pnlQueue.Controls.Add(this.flpQueue);
            this.pnlQueue.Controls.Add(this.lblQueueTitle);
            this.pnlQueue.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Padding = new Wisej.Web.Padding(12, 4, 12, 6);
            //
            // flpQueue
            //
            this.flpQueue.AutoScroll = true;
            this.flpQueue.Dock = Wisej.Web.DockStyle.Fill;
            this.flpQueue.FlowDirection = Wisej.Web.FlowDirection.TopDown;
            this.flpQueue.Name = "flpQueue";
            this.flpQueue.WrapContents = false;
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(200, 24);
            this.lblQueueTitle.Text = "COMPLETION QUEUE — EMPTY";
            //
            // conflictPanel
            //
            this.conflictPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.conflictPanel.Name = "conflictPanel";
            this.conflictPanel.Size = new System.Drawing.Size(858, 248);
            this.conflictPanel.Visible = false;
            //
            // pnlSync
            //
            this.pnlSync.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.pnlSync.Controls.Add(this.prgSync);
            this.pnlSync.Controls.Add(this.lblSyncStatus);
            this.pnlSync.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlSync.Name = "pnlSync";
            this.pnlSync.Size = new System.Drawing.Size(858, 36);
            //
            // prgSync
            //
            this.prgSync.BarColor = System.Drawing.Color.FromArgb(31, 174, 90);
            this.prgSync.Location = new System.Drawing.Point(12, 9);
            this.prgSync.Maximum = 1;
            this.prgSync.Name = "prgSync";
            this.prgSync.Size = new System.Drawing.Size(150, 18);
            this.prgSync.Value = 0;
            //
            // lblSyncStatus
            //
            this.lblSyncStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSyncStatus.AutoSize = false;
            this.lblSyncStatus.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblSyncStatus.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblSyncStatus.Location = new System.Drawing.Point(174, 9);
            this.lblSyncStatus.Name = "lblSyncStatus";
            this.lblSyncStatus.Size = new System.Drawing.Size(672, 18);
            this.lblSyncStatus.Text = "";
            //
            // pnlCache
            //
            this.pnlCache.Controls.Add(this.dgvCache);
            this.pnlCache.Controls.Add(this.pnlFieldActions);
            this.pnlCache.Controls.Add(this.lblCacheTitle);
            this.pnlCache.Dock = Wisej.Web.DockStyle.Top;
            this.pnlCache.Name = "pnlCache";
            this.pnlCache.Padding = new Wisej.Web.Padding(12, 0, 12, 4);
            this.pnlCache.Size = new System.Drawing.Size(858, 250);
            //
            // dgvCache
            //
            this.dgvCache.AllowUserToAddRows = false;
            this.dgvCache.AllowUserToDeleteRows = false;
            this.dgvCache.AutoGenerateColumns = false;
            this.dgvCache.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colCacheCode,
                this.colCacheTitle,
                this.colCacheSite,
                this.colCacheStatus,
                this.colCacheVersion,
                this.colCacheLocal});
            this.dgvCache.Dock = Wisej.Web.DockStyle.Fill;
            this.dgvCache.MultiSelect = false;
            this.dgvCache.Name = "dgvCache";
            this.dgvCache.ReadOnly = true;
            this.dgvCache.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCache.ShowRowErrors = false;
            //
            // colCacheCode
            //
            this.colCacheCode.DataPropertyName = "Code";
            this.colCacheCode.HeaderText = "WO";
            this.colCacheCode.Name = "colCacheCode";
            this.colCacheCode.Width = 84;
            //
            // colCacheTitle
            //
            this.colCacheTitle.DataPropertyName = "Title";
            this.colCacheTitle.HeaderText = "Work order";
            this.colCacheTitle.Name = "colCacheTitle";
            this.colCacheTitle.Width = 300;
            //
            // colCacheSite
            //
            this.colCacheSite.DataPropertyName = "Site";
            this.colCacheSite.HeaderText = "Site";
            this.colCacheSite.Name = "colCacheSite";
            this.colCacheSite.Width = 130;
            //
            // colCacheStatus
            //
            this.colCacheStatus.DataPropertyName = "Status";
            this.colCacheStatus.HeaderText = "Server status";
            this.colCacheStatus.Name = "colCacheStatus";
            this.colCacheStatus.Width = 110;
            //
            // colCacheVersion
            //
            this.colCacheVersion.DataPropertyName = "Version";
            this.colCacheVersion.HeaderText = "v";
            this.colCacheVersion.Name = "colCacheVersion";
            this.colCacheVersion.Width = 44;
            //
            // colCacheLocal
            //
            this.colCacheLocal.DataPropertyName = "LocalState";
            this.colCacheLocal.HeaderText = "On this device";
            this.colCacheLocal.Name = "colCacheLocal";
            this.colCacheLocal.Width = 170;
            //
            // pnlFieldActions
            //
            this.pnlFieldActions.Controls.Add(this.lblNotes);
            this.pnlFieldActions.Controls.Add(this.txtNotes);
            this.pnlFieldActions.Controls.Add(this.btnComplete);
            this.pnlFieldActions.Controls.Add(this.btnScan);
            this.pnlFieldActions.Controls.Add(this.lblScanResult);
            this.pnlFieldActions.Dock = Wisej.Web.DockStyle.Bottom;
            this.pnlFieldActions.Name = "pnlFieldActions";
            this.pnlFieldActions.Size = new System.Drawing.Size(834, 96);
            //
            // lblNotes
            //
            this.lblNotes.AutoSize = false;
            this.lblNotes.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblNotes.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblNotes.Location = new System.Drawing.Point(0, 4);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(300, 16);
            this.lblNotes.Text = "COMPLETION NOTES";
            //
            // txtNotes
            //
            this.txtNotes.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.txtNotes.Location = new System.Drawing.Point(0, 22);
            this.txtNotes.MaxLength = 120;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(834, 28);
            this.txtNotes.Text = "seal replaced, torqued to spec";
            //
            // btnComplete
            //
            this.btnComplete.Location = new System.Drawing.Point(0, 56);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(200, 34);
            this.btnComplete.Text = "Complete…";
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            //
            // btnScan
            //
            this.btnScan.Location = new System.Drawing.Point(208, 56);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(120, 34);
            this.btnScan.Text = "Scan";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            //
            // lblScanResult
            //
            this.lblScanResult.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblScanResult.AutoSize = false;
            this.lblScanResult.Font = new System.Drawing.Font("monospace", 8F);
            this.lblScanResult.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblScanResult.Location = new System.Drawing.Point(336, 62);
            this.lblScanResult.Name = "lblScanResult";
            this.lblScanResult.Size = new System.Drawing.Size(498, 22);
            this.lblScanResult.Text = "";
            //
            // lblCacheTitle
            //
            this.lblCacheTitle.AutoSize = false;
            this.lblCacheTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblCacheTitle.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCacheTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblCacheTitle.Name = "lblCacheTitle";
            this.lblCacheTitle.Size = new System.Drawing.Size(400, 24);
            this.lblCacheTitle.Text = "WORK ORDERS — LOCAL CACHE (SQLITE)";
            //
            // pnlFieldHeader
            //
            this.pnlFieldHeader.Controls.Add(this.lblFieldTitle);
            this.pnlFieldHeader.Controls.Add(this.btnToggleConnection);
            this.pnlFieldHeader.Controls.Add(this.lblConnection);
            this.pnlFieldHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlFieldHeader.Name = "pnlFieldHeader";
            this.pnlFieldHeader.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.pnlFieldHeader.Size = new System.Drawing.Size(858, 42);
            //
            // lblFieldTitle
            //
            this.lblFieldTitle.AutoSize = false;
            this.lblFieldTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblFieldTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblFieldTitle.Location = new System.Drawing.Point(12, 10);
            this.lblFieldTitle.Name = "lblFieldTitle";
            this.lblFieldTitle.Size = new System.Drawing.Size(110, 24);
            this.lblFieldTitle.Text = "Field mode";
            //
            // btnToggleConnection
            //
            this.btnToggleConnection.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnToggleConnection.Location = new System.Drawing.Point(600, 6);
            this.btnToggleConnection.Name = "btnToggleConnection";
            this.btnToggleConnection.Size = new System.Drawing.Size(100, 30);
            this.btnToggleConnection.Text = "Go offline";
            this.btnToggleConnection.Click += new System.EventHandler(this.btnToggleConnection_Click);
            //
            // lblConnection
            //
            this.lblConnection.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblConnection.AutoSize = false;
            this.lblConnection.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblConnection.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblConnection.Location = new System.Drawing.Point(708, 10);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(138, 22);
            this.lblConnection.Text = "ONLINE";
            this.lblConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // FieldTechnicianPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.pnlDevice);
            this.Name = "FieldTechnicianPage";
            this.Size = new System.Drawing.Size(908, 584);
            this.Text = "EnterpriseOps — Field mode";
            this.Load += new System.EventHandler(this.FieldTechnicianPage_Load);
            this.pnlDevice.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlSync.ResumeLayout(false);
            this.pnlCache.ResumeLayout(false);
            this.pnlFieldActions.ResumeLayout(false);
            this.pnlFieldHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblHeader;
        private Wisej.Web.Panel pnlDevice;
        private Wisej.Web.Panel pnlFieldHeader;
        private Wisej.Web.Label lblFieldTitle;
        private Wisej.Web.Button btnToggleConnection;
        private Wisej.Web.Label lblConnection;
        private Wisej.Web.Panel pnlCache;
        private Wisej.Web.Label lblCacheTitle;
        private Wisej.Web.DataGridView dgvCache;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheCode;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheSite;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheVersion;
        private Wisej.Web.DataGridViewTextBoxColumn colCacheLocal;
        private Wisej.Web.Panel pnlFieldActions;
        private Wisej.Web.Label lblNotes;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Button btnComplete;
        private Wisej.Web.Button btnScan;
        private Wisej.Web.Label lblScanResult;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.FlowLayoutPanel flpQueue;
        private EnterpriseOps.UI.SyncConflictPanel conflictPanel;
        private Wisej.Web.Panel pnlSync;
        private Wisej.Web.ProgressBar prgSync;
        private Wisej.Web.Label lblSyncStatus;
    }
}
