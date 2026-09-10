namespace TicketOps.Views
{
    partial class ReleaseConsole
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
            this.diagnosticsPage = new TicketOps.Diagnostics.DiagnosticsPage();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonHealth = new Wisej.Web.Button();
            this.buttonLoad = new Wisej.Web.Button();
            this.buttonTechnician = new Wisej.Web.Button();
            this.buttonDegrade = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerFirstRefresh = new Wisej.Web.Timer(this.components);
            this.timerLoad = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (TicketOps — Diagnostics: the role-protected page)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.diagnosticsPage);
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
            this.labelScreenTitle.Size = new System.Drawing.Size(360, 30);
            this.labelScreenTitle.Text = "TicketOps — Diagnostics";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // diagnosticsPage  (Diagnostics/DiagnosticsPage: display only, Refresh raises RefreshRequested)
            //
            this.diagnosticsPage.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.diagnosticsPage.Location = new System.Drawing.Point(1, 82);
            this.diagnosticsPage.Name = "diagnosticsPage";
            this.diagnosticsPage.Size = new System.Drawing.Size(756, 476);
            this.diagnosticsPage.RefreshRequested += new System.EventHandler(this.diagnosticsPage_RefreshRequested);
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Service → Infra → Data";
            this.tracePanel.Footer = "UI shows · SVC decides · INFRA reads the manifest and the runtime · DATA is probed · SESSION is this node's live state · ⚠ handled · ✖ failure (details stay in the log)";
            //
            // panelActions  (bottom bar: success / progress / failures / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonHealth);
            this.panelActions.Controls.Add(this.buttonLoad);
            this.panelActions.Controls.Add(this.buttonTechnician);
            this.panelActions.Controls.Add(this.buttonDegrade);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonHealth.Location = new System.Drawing.Point(0, 4);
            this.buttonHealth.Name = "buttonHealth";
            this.buttonHealth.Size = new System.Drawing.Size(170, 36);
            this.buttonHealth.Text = "✓ Run health check";
            this.buttonHealth.ToolTipText = "Success path: IHealthCheckService.CheckAsync() reads HealthCheck.json (Application.MapPath), probes database / storage / websocket, aggregates → Healthy · HTTP 200";
            this.buttonHealth.Click += new System.EventHandler(this.buttonHealth_Click);
            this.buttonLoad.Location = new System.Drawing.Point(180, 4);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(200, 36);
            this.buttonLoad.Text = "▶ Simulate load (40 units)";
            this.buttonLoad.ToolTipText = "Progress path: a Timer opens 4 work units per tick through ILoadTestService → ITicketService.SaveAsync; the count climbs, then diagnostics refresh";
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            this.buttonTechnician.Location = new System.Drawing.Point(390, 4);
            this.buttonTechnician.Name = "buttonTechnician";
            this.buttonTechnician.Size = new System.Drawing.Size(200, 36);
            this.buttonTechnician.Text = "Sign in as Technician";
            this.buttonTechnician.ToolTipText = "Failure path 1 (permission): IDiagnosticsService says Access denied to a Technician — no runtime facts shown. Click again to sign back in as Supervisor.";
            this.buttonTechnician.Click += new System.EventHandler(this.buttonTechnician_Click);
            this.buttonDegrade.Location = new System.Drawing.Point(600, 4);
            this.buttonDegrade.Name = "buttonDegrade";
            this.buttonDegrade.Size = new System.Drawing.Size(160, 36);
            this.buttonDegrade.Text = "Degrade 'storage'";
            this.buttonDegrade.ToolTipText = "Failure path 2 (dependency): storage reports Degraded → status Degraded, HTTP 200, the node keeps serving and the app keeps working. Click again to restore.";
            this.buttonDegrade.Click += new System.EventHandler(this.buttonDegrade_Click);
            this.buttonOutage.Location = new System.Drawing.Point(770, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(230, 36);
            this.buttonOutage.Text = "Simulate repository outage";
            this.buttonOutage.ToolTipText = "Error path: the repository throws like a real driver; the probe reports database Unhealthy → HTTP 503, the log keeps the details, the user sees a safe message. Click again to recover.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timers
            //
            this.timerFirstRefresh.Interval = 900;
            this.timerFirstRefresh.Tick += new System.EventHandler(this.timerFirstRefresh_Tick);
            this.timerLoad.Interval = 150;
            this.timerLoad.Tick += new System.EventHandler(this.timerLoad_Tick);
            //
            // ReleaseConsole
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "ReleaseConsole";
            this.Text = "TicketOps Console — Module 12 · Deployment, diagnostics & capstone delivery";
            this.Load += new System.EventHandler(this.ReleaseConsole_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private TicketOps.Diagnostics.DiagnosticsPage diagnosticsPage;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonHealth;
        private Wisej.Web.Button buttonLoad;
        private Wisej.Web.Button buttonTechnician;
        private Wisej.Web.Button buttonDegrade;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerFirstRefresh;
        private Wisej.Web.Timer timerLoad;
    }
}
