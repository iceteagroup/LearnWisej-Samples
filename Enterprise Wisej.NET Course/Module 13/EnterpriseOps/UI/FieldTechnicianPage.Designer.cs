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
            this.lblSession = new Wisej.Web.Label();
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
            this.lblConnection = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblStatus = new Wisej.Web.Label();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnToggleConnection = new Wisej.Web.Button();
            this.btnSync = new Wisej.Web.Button();
            this.btnDispatcherCancel = new Wisej.Web.Button();
            this.btnRevokePermission = new Wisej.Web.Button();
            this.lblDeviceCaption = new Wisej.Web.Label();
            this.cboDevice = new Wisej.Web.ComboBox();
            this.btnClearTrace = new Wisej.Web.Button();
            this.btnAntiPattern = new Wisej.Web.Button();
            this.btnRecover = new Wisej.Web.Button();
            this.lblBanner = new Wisej.Web.Label();
            this.pnlDevice.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlSync.SuspendLayout();
            this.pnlCache.SuspendLayout();
            this.pnlFieldActions.SuspendLayout();
            this.pnlFieldHeader.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // lblHeader
            //
            this.lblHeader.AutoSize = false;
            this.lblHeader.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(13, 27, 42);
            this.lblHeader.Location = new System.Drawing.Point(24, 12);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(640, 28);
            this.lblHeader.Text = "EnterpriseOps — Field Technician mode";
            //
            // lblSession  (tenant · user · session · correlation id)
            //
            this.lblSession.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblSession.AutoSize = false;
            this.lblSession.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSession.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblSession.Location = new System.Drawing.Point(684, 16);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(640, 22);
            this.lblSession.Text = "";
            this.lblSession.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlDevice  (the device frame: its width IS the device-aware layout)
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
            // pnlQueue  (Fill — added first so docking, which runs last-to-first, leaves it the rest)
            //
            this.pnlQueue.Controls.Add(this.flpQueue);
            this.pnlQueue.Controls.Add(this.lblQueueTitle);
            this.pnlQueue.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Padding = new Wisej.Web.Padding(12, 4, 12, 6);
            //
            // flpQueue  (one OfflineCommandRow per queued command)
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
            this.lblQueueTitle.Text = "COMPLETION QUEUE — 0 PENDING";
            //
            // conflictPanel  (hidden until the reconnect sync hits a conflict)
            //
            this.conflictPanel.Dock = Wisej.Web.DockStyle.Bottom;
            this.conflictPanel.Name = "conflictPanel";
            this.conflictPanel.Size = new System.Drawing.Size(858, 248);
            this.conflictPanel.Visible = false;
            //
            // pnlSync  (the device status strip: progress + one sentence)
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
            // pnlCache  (the cached work orders + the field actions; hidden while a conflict is on screen)
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
            // the cache columns
            //
            this.colCacheCode.DataPropertyName = "Code";
            this.colCacheCode.HeaderText = "WO";
            this.colCacheCode.Name = "colCacheCode";
            this.colCacheCode.Width = 84;
            this.colCacheTitle.DataPropertyName = "Title";
            this.colCacheTitle.HeaderText = "Work order";
            this.colCacheTitle.Name = "colCacheTitle";
            this.colCacheTitle.Width = 300;
            this.colCacheSite.DataPropertyName = "Site";
            this.colCacheSite.HeaderText = "Site";
            this.colCacheSite.Name = "colCacheSite";
            this.colCacheSite.Width = 130;
            this.colCacheStatus.DataPropertyName = "Status";
            this.colCacheStatus.HeaderText = "Server status";
            this.colCacheStatus.Name = "colCacheStatus";
            this.colCacheStatus.Width = 110;
            this.colCacheVersion.DataPropertyName = "Version";
            this.colCacheVersion.HeaderText = "v";
            this.colCacheVersion.Name = "colCacheVersion";
            this.colCacheVersion.Width = 44;
            this.colCacheLocal.DataPropertyName = "LocalState";
            this.colCacheLocal.HeaderText = "On this device";
            this.colCacheLocal.Name = "colCacheLocal";
            this.colCacheLocal.Width = 170;
            //
            // pnlFieldActions  (what the technician can do with the selected work order)
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
            this.lblNotes.Size = new System.Drawing.Size(400, 16);
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
            this.txtNotes.ToolTipText = "What you did. It travels inside the OfflineCommand payload and is audited with the device time.";
            //
            // btnComplete
            //
            this.btnComplete.Location = new System.Drawing.Point(0, 56);
            this.btnComplete.Name = "btnComplete";
            this.btnComplete.Size = new System.Drawing.Size(200, 34);
            this.btnComplete.Text = "Complete…";
            this.btnComplete.ToolTipText = "Online: straight to WorkOrderService. Offline: queued as an OfflineCommand, never lost.";
            this.btnComplete.Click += new System.EventHandler(this.btnComplete_Click);
            //
            // btnScan
            //
            this.btnScan.Location = new System.Drawing.Point(208, 56);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(120, 34);
            this.btnScan.Text = "Scan";
            this.btnScan.ToolTipText = "IDeviceServices.ScanDocumentAsync() — works offline; the value is validated on the server.";
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
            this.lblFieldTitle.Location = new System.Drawing.Point(0, 10);
            this.lblFieldTitle.Name = "lblFieldTitle";
            this.lblFieldTitle.Size = new System.Drawing.Size(340, 24);
            this.lblFieldTitle.Text = "Field mode";
            //
            // lblConnection  (OFFLINE / ONLINE pill)
            //
            this.lblConnection.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblConnection.AutoSize = false;
            this.lblConnection.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblConnection.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblConnection.Location = new System.Drawing.Point(700, 10);
            this.lblConnection.Name = "lblConnection";
            this.lblConnection.Size = new System.Drawing.Size(134, 22);
            this.lblConnection.Text = "ONLINE";
            this.lblConnection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblStatus);
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Location = new System.Drawing.Point(900, 50);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Padding = new Wisej.Web.Padding(10, 6, 10, 6);
            this.pnlTrace.Size = new System.Drawing.Size(424, 510);
            //
            // lstTrace
            //
            this.lstTrace.Dock = Wisej.Web.DockStyle.Fill;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Name = "lstTrace";
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(404, 28);
            this.lblStatus.Text = "● ready";
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.ForeColor = System.Drawing.Color.FromArgb(31, 45, 58);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(404, 30);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // pnlActions  (the demo bar: connectivity, sync, the two failure paths, the anti-pattern, recovery)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.BackColor = System.Drawing.Color.White;
            this.pnlActions.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActions.Controls.Add(this.btnToggleConnection);
            this.pnlActions.Controls.Add(this.btnSync);
            this.pnlActions.Controls.Add(this.btnDispatcherCancel);
            this.pnlActions.Controls.Add(this.btnRevokePermission);
            this.pnlActions.Controls.Add(this.lblDeviceCaption);
            this.pnlActions.Controls.Add(this.cboDevice);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Controls.Add(this.btnAntiPattern);
            this.pnlActions.Controls.Add(this.btnRecover);
            this.pnlActions.Controls.Add(this.lblBanner);
            this.pnlActions.Location = new System.Drawing.Point(24, 572);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 92);
            //
            // btnToggleConnection  (the reconnect simulation)
            //
            this.btnToggleConnection.Location = new System.Drawing.Point(12, 10);
            this.btnToggleConnection.Name = "btnToggleConnection";
            this.btnToggleConnection.Size = new System.Drawing.Size(160, 34);
            this.btnToggleConnection.Text = "Go offline";
            this.btnToggleConnection.ToolTipText = "Flips IDeviceServices connectivity. Going online replays the queue.";
            this.btnToggleConnection.Click += new System.EventHandler(this.btnToggleConnection_Click);
            //
            // btnSync  (progress path)
            //
            this.btnSync.Location = new System.Drawing.Point(180, 10);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(150, 34);
            this.btnSync.Text = "▶ Sync now";
            this.btnSync.ToolTipText = "Replays the queue through the server service on a background task (Application.StartTask).";
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            //
            // btnDispatcherCancel  (sets up the conflict)
            //
            this.btnDispatcherCancel.Location = new System.Drawing.Point(338, 10);
            this.btnDispatcherCancel.Name = "btnDispatcherCancel";
            this.btnDispatcherCancel.Size = new System.Drawing.Size(266, 34);
            this.btnDispatcherCancel.Text = "Dispatcher cancels the selected WO";
            this.btnDispatcherCancel.ToolTipText = "ana.ops changes the row on the server while the device is offline — the setup for a sync conflict.";
            this.btnDispatcherCancel.Click += new System.EventHandler(this.btnDispatcherCancel_Click);
            //
            // btnRevokePermission  (stale permission failure path)
            //
            this.btnRevokePermission.Location = new System.Drawing.Point(612, 10);
            this.btnRevokePermission.Name = "btnRevokePermission";
            this.btnRevokePermission.Size = new System.Drawing.Size(266, 34);
            this.btnRevokePermission.Text = "Revoke ben.tech's permission";
            this.btnRevokePermission.ToolTipText = "Removes workorder.complete on the server. The device still holds yesterday's snapshot.";
            this.btnRevokePermission.Click += new System.EventHandler(this.btnRevokePermission_Click);
            //
            // lblDeviceCaption
            //
            this.lblDeviceCaption.AutoSize = false;
            this.lblDeviceCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblDeviceCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblDeviceCaption.Location = new System.Drawing.Point(890, 18);
            this.lblDeviceCaption.Name = "lblDeviceCaption";
            this.lblDeviceCaption.Size = new System.Drawing.Size(114, 20);
            this.lblDeviceCaption.Text = "SIMULATE DEVICE";
            //
            // cboDevice  (phone / tablet / desktop)
            //
            this.cboDevice.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboDevice.Items.AddRange(new object[] { "Phone", "Tablet", "Desktop" });
            this.cboDevice.Location = new System.Drawing.Point(1008, 12);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(130, 30);
            this.cboDevice.ToolTipText = "Forces the shape IDeviceServices reports. The screen re-lays out; no screen code changes.";
            this.cboDevice.SelectedIndexChanged += new System.EventHandler(this.cboDevice_SelectedIndexChanged);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Location = new System.Drawing.Point(1146, 10);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(140, 34);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // btnAntiPattern  (the video's anti-pattern: the screen writes straight to the store)
            //
            this.btnAntiPattern.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.btnAntiPattern.Location = new System.Drawing.Point(12, 50);
            this.btnAntiPattern.Name = "btnAntiPattern";
            this.btnAntiPattern.Size = new System.Drawing.Size(318, 34);
            this.btnAntiPattern.Text = "✖ Anti-pattern: device writes straight to the store";
            this.btnAntiPattern.ToolTipText = "Bypasses the sync boundary: no permission check, no version check, no audit entry.";
            this.btnAntiPattern.Click += new System.EventHandler(this.btnAntiPattern_Click);
            //
            // btnRecover  (recovery path)
            //
            this.btnRecover.Location = new System.Drawing.Point(338, 50);
            this.btnRecover.Name = "btnRecover";
            this.btnRecover.Size = new System.Drawing.Size(266, 34);
            this.btnRecover.Text = "Recover: reconcile + re-provision device";
            this.btnRecover.ToolTipText = "Admin replays the change through the service (audited), then the device is wiped and re-downloaded.";
            this.btnRecover.Click += new System.EventHandler(this.btnRecover_Click);
            //
            // lblBanner  (one line: what just failed and what to do about it)
            //
            this.lblBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblBanner.AutoSize = false;
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblBanner.Location = new System.Drawing.Point(612, 50);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Size = new System.Drawing.Size(674, 34);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // FieldTechnicianPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lblSession);
            this.Controls.Add(this.pnlDevice);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "FieldTechnicianPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Field mode";
            this.Load += new System.EventHandler(this.FieldTechnicianPage_Load);
            this.pnlDevice.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlSync.ResumeLayout(false);
            this.pnlCache.ResumeLayout(false);
            this.pnlFieldActions.ResumeLayout(false);
            this.pnlFieldHeader.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblHeader;
        private Wisej.Web.Label lblSession;
        private Wisej.Web.Panel pnlDevice;
        private Wisej.Web.Panel pnlFieldHeader;
        private Wisej.Web.Label lblFieldTitle;
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
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnToggleConnection;
        private Wisej.Web.Button btnSync;
        private Wisej.Web.Button btnDispatcherCancel;
        private Wisej.Web.Button btnRevokePermission;
        private Wisej.Web.Label lblDeviceCaption;
        private Wisej.Web.ComboBox cboDevice;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Button btnAntiPattern;
        private Wisej.Web.Button btnRecover;
        private Wisej.Web.Label lblBanner;
    }
}
