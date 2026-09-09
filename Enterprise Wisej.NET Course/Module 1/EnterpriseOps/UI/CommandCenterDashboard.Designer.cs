namespace EnterpriseOps.UI
{
    partial class CommandCenterDashboard
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlDashboard = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
            this.kpiOpenIncidents = new EnterpriseOps.Controls.KpiTile();
            this.kpiSlaAtRisk = new EnterpriseOps.Controls.KpiTile();
            this.kpiDeploymentsToday = new EnterpriseOps.Controls.KpiTile();
            this.dgvIncidents = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colIncident = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colState = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnFeedDown = new Wisej.Web.Button();
            this.btnFeedRestore = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnGateLegacy = new Wisej.Web.Button();
            this.btnGateScreen = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(420, 44);
            this.lblTitle.Text = "EnterpriseOps — Command Center";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: contoso";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1160, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(164, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlDashboard  (the designable dashboard: toolbar row, KPI tiles, incidents grid, banner, footer)
            //
            this.pnlDashboard.BackColor = System.Drawing.Color.White;
            this.pnlDashboard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDashboard.Controls.Add(this.btnRefresh);
            this.pnlDashboard.Controls.Add(this.lblStatus);
            this.pnlDashboard.Controls.Add(this.kpiOpenIncidents);
            this.pnlDashboard.Controls.Add(this.kpiSlaAtRisk);
            this.pnlDashboard.Controls.Add(this.kpiDeploymentsToday);
            this.pnlDashboard.Controls.Add(this.dgvIncidents);
            this.pnlDashboard.Controls.Add(this.lblBanner);
            this.pnlDashboard.Controls.Add(this.lblStatusBar);
            this.pnlDashboard.Location = new System.Drawing.Point(24, 64);
            this.pnlDashboard.Name = "pnlDashboard";
            this.pnlDashboard.Size = new System.Drawing.Size(812, 506);
            //
            // btnRefresh  (the walkthrough's Refresh — double-click in the Designer created btnRefresh_Click)
            //
            this.btnRefresh.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(20, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 36);
            this.btnRefresh.Text = "⟳ Refresh";
            this.btnRefresh.ToolTipText = "btnRefresh_Click → await _workflow.RefreshAsync(CurrentContext) → ShowResult(result). One service call.";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(170, 19);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(622, 26);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // kpiOpenIncidents / kpiSlaAtRisk / kpiDeploymentsToday  (three shared KpiTile controls)
            //
            this.kpiOpenIncidents.Accent = System.Drawing.Color.FromArgb(192, 57, 43);
            this.kpiOpenIncidents.Caption = "OPEN INCIDENTS";
            this.kpiOpenIncidents.Location = new System.Drawing.Point(20, 64);
            this.kpiOpenIncidents.Name = "kpiOpenIncidents";
            this.kpiOpenIncidents.Size = new System.Drawing.Size(248, 84);
            this.kpiOpenIncidents.Value = "—";
            this.kpiSlaAtRisk.Accent = System.Drawing.Color.FromArgb(185, 119, 14);
            this.kpiSlaAtRisk.Caption = "SLA AT RISK";
            this.kpiSlaAtRisk.Location = new System.Drawing.Point(282, 64);
            this.kpiSlaAtRisk.Name = "kpiSlaAtRisk";
            this.kpiSlaAtRisk.Size = new System.Drawing.Size(248, 84);
            this.kpiSlaAtRisk.Value = "—";
            this.kpiDeploymentsToday.Accent = System.Drawing.Color.FromArgb(31, 138, 76);
            this.kpiDeploymentsToday.Caption = "DEPLOYMENTS TODAY";
            this.kpiDeploymentsToday.Location = new System.Drawing.Point(544, 64);
            this.kpiDeploymentsToday.Name = "kpiDeploymentsToday";
            this.kpiDeploymentsToday.Size = new System.Drawing.Size(248, 84);
            this.kpiDeploymentsToday.Value = "—";
            //
            // dgvIncidents  (explicit columns bound to IncidentRow — the projection, never the entity)
            //
            this.dgvIncidents.AllowUserToAddRows = false;
            this.dgvIncidents.AllowUserToDeleteRows = false;
            this.dgvIncidents.AutoGenerateColumns = false;
            this.dgvIncidents.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvIncidents.BackColor = System.Drawing.Color.White;
            this.dgvIncidents.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colId,
            this.colIncident,
            this.colPriority,
            this.colState});
            this.dgvIncidents.Location = new System.Drawing.Point(20, 162);
            this.dgvIncidents.MultiSelect = false;
            this.dgvIncidents.Name = "dgvIncidents";
            this.dgvIncidents.ReadOnly = true;
            this.dgvIncidents.RowHeadersVisible = false;
            this.dgvIncidents.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvIncidents.Size = new System.Drawing.Size(772, 232);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.FillWeight = 14F;
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            //
            // colIncident
            //
            this.colIncident.DataPropertyName = "Title";
            this.colIncident.FillWeight = 52F;
            this.colIncident.HeaderText = "Incident";
            this.colIncident.Name = "colIncident";
            this.colIncident.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 16F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colState
            //
            this.colState.DataPropertyName = "State";
            this.colState.FillWeight = 18F;
            this.colState.HeaderText = "State";
            this.colState.Name = "colState";
            this.colState.ReadOnly = true;
            //
            // lblBanner  (failure / review-gate banner; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 404);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 36);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the walkthrough's dark footer: "Loaded 12 incidents — EnterpriseOps.Services.Workflow · 41 ms")
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 450);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 36);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(852, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(472, 506);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(440, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(440, 416);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 468);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(440, 28);
            this.lblTraceFooter.Text = "UI → · Security: · Integration: · Data: · Service: · Diagnostics: · Architecture: · UI ←";
            //
            // pnlActions  (bottom bar: failure paths, recoveries, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnFeedDown);
            this.pnlActions.Controls.Add(this.btnFeedRestore);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnGateLegacy);
            this.pnlActions.Controls.Add(this.btnGateScreen);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 584);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnFeedDown  (failure path 1: the integration throws)
            //
            this.btnFeedDown.Location = new System.Drawing.Point(0, 4);
            this.btnFeedDown.Name = "btnFeedDown";
            this.btnFeedDown.Size = new System.Drawing.Size(180, 36);
            this.btnFeedDown.Text = "Fail: ops feed down";
            this.btnFeedDown.ToolTipText = "OperationsFeed goes offline; the next Refresh throws IntegrationUnavailableException → logged, generic message, correlation id.";
            this.btnFeedDown.Click += new System.EventHandler(this.btnFeedDown_Click);
            //
            // btnFeedRestore  (recovery 1)
            //
            this.btnFeedRestore.Location = new System.Drawing.Point(188, 4);
            this.btnFeedRestore.Name = "btnFeedRestore";
            this.btnFeedRestore.Size = new System.Drawing.Size(200, 36);
            this.btnFeedRestore.Text = "Recover: feed back online";
            this.btnFeedRestore.ToolTipText = "The feed answers again; the same btnRefresh_Click succeeds.";
            this.btnFeedRestore.Click += new System.EventHandler(this.btnFeedRestore_Click);
            //
            // btnSwitchUser  (failure path 2 / recovery 2: policy)
            //
            this.btnSwitchUser.Location = new System.Drawing.Point(396, 4);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(240, 36);
            this.btnSwitchUser.Text = "Refresh as ben.tech (Technician)";
            this.btnSwitchUser.ToolTipText = "DashboardPolicy denies Technicians: Succeeded=false, amber banner, nothing queried. Click again to come back as ana.ops.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
            //
            // btnGateLegacy  (failure path 3: the 74-line handler)
            //
            this.btnGateLegacy.Location = new System.Drawing.Point(644, 4);
            this.btnGateLegacy.Name = "btnGateLegacy";
            this.btnGateLegacy.Size = new System.Drawing.Size(220, 36);
            this.btnGateLegacy.Text = "Review gate: legacy handler";
            this.btnGateLegacy.ToolTipText = "Runs ReviewGate.CheckEventHandler on Architecture/Samples/OrderEntryLegacy.cs.txt — 74 lines, no service call: 2 issues.";
            this.btnGateLegacy.Click += new System.EventHandler(this.btnGateLegacy_Click);
            //
            // btnGateScreen  (recovery 3: the checklist applied to this screen)
            //
            this.btnGateScreen.Location = new System.Drawing.Point(872, 4);
            this.btnGateScreen.Name = "btnGateScreen";
            this.btnGateScreen.Size = new System.Drawing.Size(200, 36);
            this.btnGateScreen.Text = "Review gate: this screen";
            this.btnGateScreen.ToolTipText = "The same gate on UI/CommandCenterDashboard.cs: every handler ≤ 20 lines and calls a service.";
            this.btnGateScreen.Click += new System.EventHandler(this.btnGateScreen_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // CommandCenterDashboard
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDashboard);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "CommandCenterDashboard";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterDashboard_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDashboard.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlDashboard;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblStatus;
        private EnterpriseOps.Controls.KpiTile kpiOpenIncidents;
        private EnterpriseOps.Controls.KpiTile kpiSlaAtRisk;
        private EnterpriseOps.Controls.KpiTile kpiDeploymentsToday;
        private Wisej.Web.DataGridView dgvIncidents;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colIncident;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colState;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnFeedDown;
        private Wisej.Web.Button btnFeedRestore;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnGateLegacy;
        private Wisej.Web.Button btnGateScreen;
        private Wisej.Web.Button btnClearTrace;
    }
}
