namespace TicketOps.Diagnostics
{
    partial class DiagnosticsPage
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
            this.labelAccess = new Wisej.Web.Label();
            this.buttonRefresh = new Wisej.Web.Button();
            this.labelRefreshed = new Wisej.Web.Label();
            this.panelRuntime = new Wisej.Web.Panel();
            this.labelRuntimeTitle = new Wisej.Web.Label();
            this.keyServer = new Wisej.Web.Label();
            this.valueServer = new Wisej.Web.Label();
            this.keyPort = new Wisej.Web.Label();
            this.valuePort = new Wisej.Web.Label();
            this.keyMode = new Wisej.Web.Label();
            this.valueMode = new Wisej.Web.Label();
            this.keyVersion = new Wisej.Web.Label();
            this.valueVersion = new Wisej.Web.Label();
            this.keyFramework = new Wisej.Web.Label();
            this.valueFramework = new Wisej.Web.Label();
            this.keySessions = new Wisej.Web.Label();
            this.valueSessions = new Wisej.Web.Label();
            this.keyWebSocket = new Wisej.Web.Label();
            this.valueWebSocket = new Wisej.Web.Label();
            this.keyUptime = new Wisej.Web.Label();
            this.valueUptime = new Wisej.Web.Label();
            this.panelHealth = new Wisej.Web.Panel();
            this.labelHealthTitle = new Wisej.Web.Label();
            this.gridChecks = new Wisej.Web.DataGridView();
            this.columnName = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnDetail = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelOverall = new Wisej.Web.Label();
            this.labelJsonCaption = new Wisej.Web.Label();
            this.textJson = new Wisej.Web.TextBox();
            this.labelLoad = new Wisej.Web.Label();
            this.progressLoad = new Wisej.Web.ProgressBar();
            this.panelRuntime.SuspendLayout();
            this.panelHealth.SuspendLayout();
            this.SuspendLayout();
            //
            // labelAccess  (the role header: "Role required: Supervisor · ✓ m.weber (Supervisor)")
            //
            this.labelAccess.AutoSize = false;
            this.labelAccess.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelAccess.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelAccess.Location = new System.Drawing.Point(24, 2);
            this.labelAccess.Name = "labelAccess";
            this.labelAccess.Size = new System.Drawing.Size(580, 22);
            this.labelAccess.Text = "Role required: Supervisor · checking…";
            //
            // buttonRefresh
            //
            this.buttonRefresh.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonRefresh.Location = new System.Drawing.Point(620, 0);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(112, 28);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.ToolTipText = "Success path: IDiagnosticsService.GetSnapshotAsync() — role check, runtime facts, live health check";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // labelRefreshed
            //
            this.labelRefreshed.AutoSize = false;
            this.labelRefreshed.Font = new System.Drawing.Font("default", 8.5F);
            this.labelRefreshed.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelRefreshed.Location = new System.Drawing.Point(24, 26);
            this.labelRefreshed.Name = "labelRefreshed";
            this.labelRefreshed.Size = new System.Drawing.Size(400, 16);
            this.labelRefreshed.Text = "refreshed —";
            //
            // panelRuntime  (server-side facts from Wisej.Web.Application)
            //
            this.panelRuntime.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.panelRuntime.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelRuntime.Controls.Add(this.labelRuntimeTitle);
            this.panelRuntime.Controls.Add(this.keyServer);
            this.panelRuntime.Controls.Add(this.valueServer);
            this.panelRuntime.Controls.Add(this.keyPort);
            this.panelRuntime.Controls.Add(this.valuePort);
            this.panelRuntime.Controls.Add(this.keyMode);
            this.panelRuntime.Controls.Add(this.valueMode);
            this.panelRuntime.Controls.Add(this.keyVersion);
            this.panelRuntime.Controls.Add(this.valueVersion);
            this.panelRuntime.Controls.Add(this.keyFramework);
            this.panelRuntime.Controls.Add(this.valueFramework);
            this.panelRuntime.Controls.Add(this.keySessions);
            this.panelRuntime.Controls.Add(this.valueSessions);
            this.panelRuntime.Controls.Add(this.keyWebSocket);
            this.panelRuntime.Controls.Add(this.valueWebSocket);
            this.panelRuntime.Controls.Add(this.keyUptime);
            this.panelRuntime.Controls.Add(this.valueUptime);
            this.panelRuntime.Location = new System.Drawing.Point(24, 46);
            this.panelRuntime.Name = "panelRuntime";
            this.panelRuntime.Size = new System.Drawing.Size(340, 236);
            //
            // labelRuntimeTitle
            //
            this.labelRuntimeTitle.AutoSize = false;
            this.labelRuntimeTitle.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelRuntimeTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelRuntimeTitle.Location = new System.Drawing.Point(12, 8);
            this.labelRuntimeTitle.Name = "labelRuntimeTitle";
            this.labelRuntimeTitle.Size = new System.Drawing.Size(316, 18);
            this.labelRuntimeTitle.Text = "RUNTIME · Wisej.Web.Application.*";
            //
            // runtime rows (key · value)
            //
            InitRow(this.keyServer, this.valueServer, "keyServer", "valueServer", "Server", 34);
            InitRow(this.keyPort, this.valuePort, "keyPort", "valuePort", "Port", 58);
            InitRow(this.keyMode, this.valueMode, "keyMode", "valueMode", "Runtime mode", 82);
            InitRow(this.keyVersion, this.valueVersion, "keyVersion", "valueVersion", "Product version", 106);
            InitRow(this.keyFramework, this.valueFramework, "keyFramework", "valueFramework", "Framework", 130);
            InitRow(this.keySessions, this.valueSessions, "keySessions", "valueSessions", "Sessions", 154);
            InitRow(this.keyWebSocket, this.valueWebSocket, "keyWebSocket", "valueWebSocket", "WebSocket", 178);
            InitRow(this.keyUptime, this.valueUptime, "keyUptime", "valueUptime", "Uptime", 202);
            //
            // panelHealth  (HealthCheck.json + live probes)
            //
            this.panelHealth.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.panelHealth.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelHealth.Controls.Add(this.labelHealthTitle);
            this.panelHealth.Controls.Add(this.gridChecks);
            this.panelHealth.Controls.Add(this.labelOverall);
            this.panelHealth.Location = new System.Drawing.Point(380, 46);
            this.panelHealth.Name = "panelHealth";
            this.panelHealth.Size = new System.Drawing.Size(352, 236);
            //
            // labelHealthTitle
            //
            this.labelHealthTitle.AutoSize = false;
            this.labelHealthTitle.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelHealthTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelHealthTitle.Location = new System.Drawing.Point(12, 8);
            this.labelHealthTitle.Name = "labelHealthTitle";
            this.labelHealthTitle.Size = new System.Drawing.Size(328, 18);
            this.labelHealthTitle.Text = "HEALTH CHECKS · HealthCheck.json + live probes";
            //
            // gridChecks
            //
            this.gridChecks.AllowUserToAddRows = false;
            this.gridChecks.AllowUserToDeleteRows = false;
            this.gridChecks.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnName,
                this.columnStatus,
                this.columnDetail});
            this.gridChecks.Location = new System.Drawing.Point(12, 32);
            this.gridChecks.MultiSelect = false;
            this.gridChecks.Name = "gridChecks";
            this.gridChecks.ReadOnly = true;
            this.gridChecks.RowHeadersVisible = false;
            this.gridChecks.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridChecks.Size = new System.Drawing.Size(328, 138);
            //
            // columns
            //
            this.columnName.HeaderText = "Dependency";
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            this.columnName.Width = 92;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 78;
            this.columnDetail.HeaderText = "Detail";
            this.columnDetail.Name = "columnDetail";
            this.columnDetail.ReadOnly = true;
            this.columnDetail.Width = 150;
            //
            // labelOverall
            //
            this.labelOverall.AutoSize = false;
            this.labelOverall.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelOverall.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelOverall.Location = new System.Drawing.Point(12, 178);
            this.labelOverall.Name = "labelOverall";
            this.labelOverall.Size = new System.Drawing.Size(328, 50);
            this.labelOverall.Text = "Status: —";
            //
            // labelJsonCaption
            //
            this.labelJsonCaption.AutoSize = false;
            this.labelJsonCaption.Font = new System.Drawing.Font("default", 8.5F);
            this.labelJsonCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelJsonCaption.Location = new System.Drawing.Point(24, 290);
            this.labelJsonCaption.Name = "labelJsonCaption";
            this.labelJsonCaption.Size = new System.Drawing.Size(708, 18);
            this.labelJsonCaption.Text = "Health JSON as a probe sees it · GET /HealthCheck.json is the static manifest (UseFileServer); the live probes are merged in below";
            //
            // textJson
            //
            this.textJson.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.textJson.Font = new System.Drawing.Font("monospace", 9F);
            this.textJson.Location = new System.Drawing.Point(24, 310);
            this.textJson.Multiline = true;
            this.textJson.Name = "textJson";
            this.textJson.ReadOnly = true;
            this.textJson.ScrollBars = Wisej.Web.ScrollBars.Vertical;
            this.textJson.Size = new System.Drawing.Size(708, 120);
            this.textJson.Text = "";
            //
            // labelLoad
            //
            this.labelLoad.AutoSize = false;
            this.labelLoad.Font = new System.Drawing.Font("default", 9F);
            this.labelLoad.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelLoad.Location = new System.Drawing.Point(24, 440);
            this.labelLoad.Name = "labelLoad";
            this.labelLoad.Size = new System.Drawing.Size(330, 18);
            this.labelLoad.Text = "load test: idle";
            //
            // progressLoad
            //
            this.progressLoad.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressLoad.Location = new System.Drawing.Point(360, 442);
            this.progressLoad.Maximum = 40;
            this.progressLoad.Name = "progressLoad";
            this.progressLoad.Size = new System.Drawing.Size(372, 14);
            this.progressLoad.Visible = false;
            //
            // DiagnosticsPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelAccess);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.labelRefreshed);
            this.Controls.Add(this.panelRuntime);
            this.Controls.Add(this.panelHealth);
            this.Controls.Add(this.labelJsonCaption);
            this.Controls.Add(this.textJson);
            this.Controls.Add(this.labelLoad);
            this.Controls.Add(this.progressLoad);
            this.Name = "DiagnosticsPage";
            this.Size = new System.Drawing.Size(756, 476);
            this.panelRuntime.ResumeLayout(false);
            this.panelHealth.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>One "key · value" row of the runtime card (layout only).</summary>
        private static void InitRow(Wisej.Web.Label key, Wisej.Web.Label value, string keyName, string valueName, string caption, int top)
        {
            key.AutoSize = false;
            key.Font = new System.Drawing.Font("default", 9F);
            key.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            key.Location = new System.Drawing.Point(12, top);
            key.Name = keyName;
            key.Size = new System.Drawing.Size(110, 20);
            key.Text = caption;
            value.AutoSize = false;
            value.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            value.Location = new System.Drawing.Point(122, top);
            value.Name = valueName;
            value.Size = new System.Drawing.Size(206, 20);
            value.Text = "—";
        }

        #endregion

        private Wisej.Web.Label labelAccess;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Label labelRefreshed;
        private Wisej.Web.Panel panelRuntime;
        private Wisej.Web.Label labelRuntimeTitle;
        private Wisej.Web.Label keyServer;
        private Wisej.Web.Label valueServer;
        private Wisej.Web.Label keyPort;
        private Wisej.Web.Label valuePort;
        private Wisej.Web.Label keyMode;
        private Wisej.Web.Label valueMode;
        private Wisej.Web.Label keyVersion;
        private Wisej.Web.Label valueVersion;
        private Wisej.Web.Label keyFramework;
        private Wisej.Web.Label valueFramework;
        private Wisej.Web.Label keySessions;
        private Wisej.Web.Label valueSessions;
        private Wisej.Web.Label keyWebSocket;
        private Wisej.Web.Label valueWebSocket;
        private Wisej.Web.Label keyUptime;
        private Wisej.Web.Label valueUptime;
        private Wisej.Web.Panel panelHealth;
        private Wisej.Web.Label labelHealthTitle;
        private Wisej.Web.DataGridView gridChecks;
        private Wisej.Web.DataGridViewTextBoxColumn columnName;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnDetail;
        private Wisej.Web.Label labelOverall;
        private Wisej.Web.Label labelJsonCaption;
        private Wisej.Web.TextBox textJson;
        private Wisej.Web.Label labelLoad;
        private Wisej.Web.ProgressBar progressLoad;
    }
}
