namespace EnterpriseOps.UI
{
    partial class ReleaseDashboardPage
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
            this.lblEnvironment = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlRelease = new Wisej.Web.Panel();
            this.lblReleaseTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.btnDeploy = new Wisej.Web.Button();
            this.btnRollback = new Wisej.Web.Button();
            this.lblRunbookTitle = new Wisej.Web.Label();
            this.flowRunbook = new Wisej.Web.FlowLayoutPanel();
            this.lblStep1 = new Wisej.Web.Label();
            this.lblStep2 = new Wisej.Web.Label();
            this.lblStep3 = new Wisej.Web.Label();
            this.lblStep4 = new Wisej.Web.Label();
            this.lblStep5 = new Wisej.Web.Label();
            this.lblStep6 = new Wisej.Web.Label();
            this.lblStep7 = new Wisej.Web.Label();
            this.lblStep8 = new Wisej.Web.Label();
            this.lblNodesTitle = new Wisej.Web.Label();
            this.dgvNodes = new Wisej.Web.DataGridView();
            this.colNode = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colBuild = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colHealth = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colLoadBalancer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblHealthJson = new Wisej.Web.Label();
            this.lblConfigTitle = new Wisej.Web.Label();
            this.cboEnvironment = new Wisej.Web.ComboBox();
            this.dgvConfig = new Wisej.Web.DataGridView();
            this.colSetting = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDevelopment = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStaging = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colProduction = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colSecret = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnSmokeTests = new Wisej.Web.Button();
            this.btnFailHealth = new Wisej.Web.Button();
            this.btnAffinity = new Wisej.Web.Button();
            this.btnInjectSecrets = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlRelease.SuspendLayout();
            this.flowRunbook.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · environment · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblEnvironment);
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
            this.lblTitle.Size = new System.Drawing.Size(460, 44);
            this.lblTitle.Text = "EnterpriseOps — Release dashboard";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblEnvironment  (which appsettings.{Environment}.json this PROCESS booted with)
            //
            this.lblEnvironment.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblEnvironment.AutoSize = false;
            this.lblEnvironment.Font = new System.Drawing.Font("monospace", 9F);
            this.lblEnvironment.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblEnvironment.Location = new System.Drawing.Point(600, 0);
            this.lblEnvironment.Name = "lblEnvironment";
            this.lblEnvironment.Size = new System.Drawing.Size(300, 44);
            this.lblEnvironment.Text = "env — · node —";
            this.lblEnvironment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(910, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(240, 44);
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
            // pnlRelease  (the release card the walkthrough designs: title + Deploy/Rollback, runbook strip,
            //              node health from HealthCheck.json, the probe body, the environment configuration table)
            //
            this.pnlRelease.BackColor = System.Drawing.Color.White;
            this.pnlRelease.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlRelease.Controls.Add(this.lblReleaseTitle);
            this.pnlRelease.Controls.Add(this.lblStatus);
            this.pnlRelease.Controls.Add(this.btnDeploy);
            this.pnlRelease.Controls.Add(this.btnRollback);
            this.pnlRelease.Controls.Add(this.lblRunbookTitle);
            this.pnlRelease.Controls.Add(this.flowRunbook);
            this.pnlRelease.Controls.Add(this.lblNodesTitle);
            this.pnlRelease.Controls.Add(this.dgvNodes);
            this.pnlRelease.Controls.Add(this.lblHealthJson);
            this.pnlRelease.Controls.Add(this.lblConfigTitle);
            this.pnlRelease.Controls.Add(this.cboEnvironment);
            this.pnlRelease.Controls.Add(this.dgvConfig);
            this.pnlRelease.Controls.Add(this.lblBanner);
            this.pnlRelease.Controls.Add(this.lblStatusBar);
            this.pnlRelease.Location = new System.Drawing.Point(24, 64);
            this.pnlRelease.Name = "pnlRelease";
            this.pnlRelease.Size = new System.Drawing.Size(812, 506);
            //
            // lblReleaseTitle
            //
            this.lblReleaseTitle.AutoSize = false;
            this.lblReleaseTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblReleaseTitle.Location = new System.Drawing.Point(20, 12);
            this.lblReleaseTitle.Name = "lblReleaseTitle";
            this.lblReleaseTitle.Size = new System.Drawing.Size(300, 28);
            this.lblReleaseTitle.Text = "Release dashboard";
            this.lblReleaseTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(322, 14);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(228, 24);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // btnDeploy  (the success + progress path: runbook steps 1–7 run one after the other)
            //
            this.btnDeploy.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeploy.Location = new System.Drawing.Point(562, 10);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(112, 32);
            this.btnDeploy.Text = "Deploy…";
            this.btnDeploy.ToolTipText = "btnDeploy_Click → await _release.DeployAsync(CurrentContext, ShowStep). The service runs the eight runbook steps; this handler only paints.";
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            //
            // btnRollback  (runbook step 8 — the recovery path)
            //
            this.btnRollback.Location = new System.Drawing.Point(682, 10);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(110, 32);
            this.btnRollback.Text = "Rollback…";
            this.btnRollback.ToolTipText = "btnRollback_Click → await _release.RollbackAsync(...). Redeploys the named previous package on the failed node and re-runs the smoke tests.";
            this.btnRollback.Click += new System.EventHandler(this.btnRollback_Click);
            //
            // lblRunbookTitle
            //
            this.lblRunbookTitle.AutoSize = false;
            this.lblRunbookTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblRunbookTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblRunbookTitle.Location = new System.Drawing.Point(20, 48);
            this.lblRunbookTitle.Name = "lblRunbookTitle";
            this.lblRunbookTitle.Size = new System.Drawing.Size(500, 18);
            this.lblRunbookTitle.Text = "RELEASE RUNBOOK — docs/ReleaseRunbook.md";
            //
            // flowRunbook  (the eight runbook chips: · pending  … running  ✓ done  ✕ failed  ↩ rolled back)
            //
            this.flowRunbook.BackColor = System.Drawing.Color.FromArgb(247, 249, 252);
            this.flowRunbook.Controls.Add(this.lblStep1);
            this.flowRunbook.Controls.Add(this.lblStep2);
            this.flowRunbook.Controls.Add(this.lblStep3);
            this.flowRunbook.Controls.Add(this.lblStep4);
            this.flowRunbook.Controls.Add(this.lblStep5);
            this.flowRunbook.Controls.Add(this.lblStep6);
            this.flowRunbook.Controls.Add(this.lblStep7);
            this.flowRunbook.Controls.Add(this.lblStep8);
            this.flowRunbook.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.flowRunbook.Location = new System.Drawing.Point(20, 68);
            this.flowRunbook.Name = "flowRunbook";
            this.flowRunbook.Padding = new Wisej.Web.Padding(6, 6, 6, 6);
            this.flowRunbook.Size = new System.Drawing.Size(772, 76);
            this.flowRunbook.WrapContents = true;
            //
            // lblStep1 … lblStep8  (one chip per runbook step; the code-behind repaints them from ReleaseProgress)
            //
            this.lblStep1.AutoSize = false;
            this.lblStep1.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep1.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep1.Size = new System.Drawing.Size(180, 26);
            this.lblStep1.Text = "· build";
            this.lblStep1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep2.AutoSize = false;
            this.lblStep2.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep2.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep2.Size = new System.Drawing.Size(180, 26);
            this.lblStep2.Text = "· config + secrets";
            this.lblStep2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep3.AutoSize = false;
            this.lblStep3.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep3.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep3.Size = new System.Drawing.Size(180, 26);
            this.lblStep3.Text = "· staging";
            this.lblStep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep4.AutoSize = false;
            this.lblStep4.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep4.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep4.Name = "lblStep4";
            this.lblStep4.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep4.Size = new System.Drawing.Size(180, 26);
            this.lblStep4.Text = "· smoke tests";
            this.lblStep4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep5.AutoSize = false;
            this.lblStep5.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep5.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep5.Name = "lblStep5";
            this.lblStep5.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep5.Size = new System.Drawing.Size(180, 26);
            this.lblStep5.Text = "· health + diagnostics";
            this.lblStep5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep6.AutoSize = false;
            this.lblStep6.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep6.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep6.Name = "lblStep6";
            this.lblStep6.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep6.Size = new System.Drawing.Size(180, 26);
            this.lblStep6.Text = "· production";
            this.lblStep6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep7.AutoSize = false;
            this.lblStep7.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep7.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep7.Name = "lblStep7";
            this.lblStep7.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep7.Size = new System.Drawing.Size(180, 26);
            this.lblStep7.Text = "· monitor";
            this.lblStep7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStep8.AutoSize = false;
            this.lblStep8.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep8.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep8.Name = "lblStep8";
            this.lblStep8.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep8.Size = new System.Drawing.Size(180, 26);
            this.lblStep8.Text = "· rollback";
            this.lblStep8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblNodesTitle
            //
            this.lblNodesTitle.AutoSize = false;
            this.lblNodesTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNodesTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblNodesTitle.Location = new System.Drawing.Point(20, 150);
            this.lblNodesTitle.Name = "lblNodesTitle";
            this.lblNodesTitle.Size = new System.Drawing.Size(500, 18);
            this.lblNodesTitle.Text = "NODES — LIVE FROM HEALTHCHECK.JSON";
            //
            // dgvNodes  (bound to NodeRow — the projection, never the ReleaseNode entity)
            //
            this.dgvNodes.AllowUserToAddRows = false;
            this.dgvNodes.AllowUserToDeleteRows = false;
            this.dgvNodes.AutoGenerateColumns = false;
            this.dgvNodes.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvNodes.BackColor = System.Drawing.Color.White;
            this.dgvNodes.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colNode,
            this.colBuild,
            this.colHealth,
            this.colLoadBalancer});
            this.dgvNodes.Location = new System.Drawing.Point(20, 170);
            this.dgvNodes.MultiSelect = false;
            this.dgvNodes.Name = "dgvNodes";
            this.dgvNodes.ReadOnly = true;
            this.dgvNodes.RowHeadersVisible = false;
            this.dgvNodes.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvNodes.Size = new System.Drawing.Size(772, 88);
            //
            // colNode
            //
            this.colNode.DataPropertyName = "Node";
            this.colNode.FillWeight = 20F;
            this.colNode.HeaderText = "Node";
            this.colNode.Name = "colNode";
            this.colNode.ReadOnly = true;
            //
            // colBuild
            //
            this.colBuild.DataPropertyName = "Build";
            this.colBuild.FillWeight = 12F;
            this.colBuild.HeaderText = "Build";
            this.colBuild.Name = "colBuild";
            this.colBuild.ReadOnly = true;
            //
            // colHealth
            //
            this.colHealth.DataPropertyName = "Health";
            this.colHealth.FillWeight = 16F;
            this.colHealth.HeaderText = "Health";
            this.colHealth.Name = "colHealth";
            this.colHealth.ReadOnly = true;
            //
            // colLoadBalancer
            //
            this.colLoadBalancer.DataPropertyName = "LoadBalancer";
            this.colLoadBalancer.FillWeight = 52F;
            this.colLoadBalancer.HeaderText = "Load balancer";
            this.colLoadBalancer.Name = "colLoadBalancer";
            this.colLoadBalancer.ReadOnly = true;
            //
            // lblHealthJson  (the body GET /healthz would return for the selected node — only safeToExpose fields)
            //
            this.lblHealthJson.AutoSize = false;
            this.lblHealthJson.BackColor = System.Drawing.Color.FromArgb(16, 23, 31);
            this.lblHealthJson.Font = new System.Drawing.Font("monospace", 8F);
            this.lblHealthJson.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblHealthJson.Location = new System.Drawing.Point(20, 264);
            this.lblHealthJson.Name = "lblHealthJson";
            this.lblHealthJson.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblHealthJson.Size = new System.Drawing.Size(772, 30);
            this.lblHealthJson.Text = "GET /healthz → …";
            this.lblHealthJson.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblConfigTitle
            //
            this.lblConfigTitle.AutoSize = false;
            this.lblConfigTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblConfigTitle.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblConfigTitle.Location = new System.Drawing.Point(20, 302);
            this.lblConfigTitle.Name = "lblConfigTitle";
            this.lblConfigTitle.Size = new System.Drawing.Size(430, 22);
            this.lblConfigTitle.Text = "ENVIRONMENT CONFIGURATION — docs/EnvironmentConfiguration.md";
            this.lblConfigTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cboEnvironment  (preview a boot of another environment — the startup-validation failure path)
            //
            this.cboEnvironment.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboEnvironment.Location = new System.Drawing.Point(628, 300);
            this.cboEnvironment.Name = "cboEnvironment";
            this.cboEnvironment.Size = new System.Drawing.Size(164, 26);
            this.cboEnvironment.ToolTipText = "Runs the same StartupValidation rules Startup.cs runs, against appsettings.json + appsettings.{Environment}.json only.";
            this.cboEnvironment.SelectedIndexChanged += new System.EventHandler(this.cboEnvironment_SelectedIndexChanged);
            //
            // dgvConfig  (bound to ConfigurationRow — every setting, its per-environment value, secret flag and owner)
            //
            this.dgvConfig.AllowUserToAddRows = false;
            this.dgvConfig.AllowUserToDeleteRows = false;
            this.dgvConfig.AutoGenerateColumns = false;
            this.dgvConfig.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvConfig.BackColor = System.Drawing.Color.White;
            this.dgvConfig.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colSetting,
            this.colDevelopment,
            this.colStaging,
            this.colProduction,
            this.colSecret,
            this.colOwner});
            this.dgvConfig.Location = new System.Drawing.Point(20, 330);
            this.dgvConfig.MultiSelect = false;
            this.dgvConfig.Name = "dgvConfig";
            this.dgvConfig.ReadOnly = true;
            this.dgvConfig.RowHeadersVisible = false;
            this.dgvConfig.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvConfig.Size = new System.Drawing.Size(772, 100);
            //
            // colSetting
            //
            this.colSetting.DataPropertyName = "Setting";
            this.colSetting.FillWeight = 20F;
            this.colSetting.HeaderText = "Setting";
            this.colSetting.Name = "colSetting";
            this.colSetting.ReadOnly = true;
            //
            // colDevelopment
            //
            this.colDevelopment.DataPropertyName = "Development";
            this.colDevelopment.FillWeight = 22F;
            this.colDevelopment.HeaderText = "Development";
            this.colDevelopment.Name = "colDevelopment";
            this.colDevelopment.ReadOnly = true;
            //
            // colStaging
            //
            this.colStaging.DataPropertyName = "Staging";
            this.colStaging.FillWeight = 21F;
            this.colStaging.HeaderText = "Staging (test)";
            this.colStaging.Name = "colStaging";
            this.colStaging.ReadOnly = true;
            //
            // colProduction
            //
            this.colProduction.DataPropertyName = "Production";
            this.colProduction.FillWeight = 21F;
            this.colProduction.HeaderText = "Production";
            this.colProduction.Name = "colProduction";
            this.colProduction.ReadOnly = true;
            //
            // colSecret
            //
            this.colSecret.DataPropertyName = "Secret";
            this.colSecret.FillWeight = 8F;
            this.colSecret.HeaderText = "Secret?";
            this.colSecret.Name = "colSecret";
            this.colSecret.ReadOnly = true;
            //
            // colOwner
            //
            this.colOwner.DataPropertyName = "Owner";
            this.colOwner.FillWeight = 10F;
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.ReadOnly = true;
            //
            // lblBanner  (failure / recovery banner; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 436);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 32);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the walkthrough's dark footer line under the dashboard)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 472);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 30);
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
            this.lblTraceFooter.Text = "UI → · Host: · Security: · Service: · Data: · Health: · Balancer: · Runbook: · UI ←";
            //
            // pnlActions  (bottom bar: the smoke tests, the three failure paths and Clear trace)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnSmokeTests);
            this.pnlActions.Controls.Add(this.btnFailHealth);
            this.pnlActions.Controls.Add(this.btnAffinity);
            this.pnlActions.Controls.Add(this.btnInjectSecrets);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 584);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnSmokeTests  (the checklist, run in-process against this node)
            //
            this.btnSmokeTests.Location = new System.Drawing.Point(0, 4);
            this.btnSmokeTests.Name = "btnSmokeTests";
            this.btnSmokeTests.Size = new System.Drawing.Size(160, 36);
            this.btnSmokeTests.Text = "Run smoke tests";
            this.btnSmokeTests.ToolTipText = "SmokeTestService runs the seven checks of docs/RollbackAndSmokeTestChecklist.md against this process.";
            this.btnSmokeTests.Click += new System.EventHandler(this.btnSmokeTests_Click);
            //
            // btnFailHealth  (failure path 1: the 2.4.2 migration breaks node B's database check)
            //
            this.btnFailHealth.Location = new System.Drawing.Point(168, 4);
            this.btnFailHealth.Name = "btnFailHealth";
            this.btnFailHealth.Size = new System.Drawing.Size(220, 36);
            this.btnFailHealth.Text = "Fail: node B health check";
            this.btnFailHealth.ToolTipText = "Arms the migration fault, then deploys: the smoke test and /healthz fail on app-node-B, the balancer routes it away and runbook step 8 is armed.";
            this.btnFailHealth.Click += new System.EventHandler(this.btnFailHealth_Click);
            //
            // btnAffinity  (failure path 2: sticky sessions off — the request lands on the other node)
            //
            this.btnAffinity.Location = new System.Drawing.Point(396, 4);
            this.btnAffinity.Name = "btnAffinity";
            this.btnAffinity.Size = new System.Drawing.Size(250, 36);
            this.btnAffinity.Text = "Fail: balancer without affinity";
            this.btnAffinity.ToolTipText = "Turns sticky sessions off and routes this session's next request: it lands on the other node, which has never heard of the session. Click again to restore affinity.";
            this.btnAffinity.Click += new System.EventHandler(this.btnAffinity_Click);
            //
            // btnInjectSecrets  (recovery for failure path 3: the platform supplies the secrets)
            //
            this.btnInjectSecrets.Location = new System.Drawing.Point(654, 4);
            this.btnInjectSecrets.Name = "btnInjectSecrets";
            this.btnInjectSecrets.Size = new System.Drawing.Size(260, 36);
            this.btnInjectSecrets.Text = "Recover: inject platform secrets";
            this.btnInjectSecrets.ToolTipText = "Re-runs the preview with EnterpriseOps__ConnectionString and EnterpriseOps__IdentityProvider__ClientSecret supplied by the environment, as a vault would.";
            this.btnInjectSecrets.Click += new System.EventHandler(this.btnInjectSecrets_Click);
            //
            // btnSwitchUser  (failure path 4: the technician presses Deploy and the SERVICE refuses —
            //                 the button stays enabled on purpose, because a hidden button is not a permission)
            //
            this.btnSwitchUser.Location = new System.Drawing.Point(922, 4);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(190, 36);
            this.btnSwitchUser.Text = "Sign in as ben.tech";
            this.btnSwitchUser.ToolTipText = "Switches the session to a Technician. Deploy and Rollback stay clickable and are refused server-side by ReleaseAuthorization.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
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
            // ReleaseDashboardPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlRelease);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "ReleaseDashboardPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Release dashboard";
            this.Load += new System.EventHandler(this.ReleaseDashboardPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlRelease.ResumeLayout(false);
            this.flowRunbook.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblEnvironment;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlRelease;
        private Wisej.Web.Label lblReleaseTitle;
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Button btnDeploy;
        private Wisej.Web.Button btnRollback;
        private Wisej.Web.Label lblRunbookTitle;
        private Wisej.Web.FlowLayoutPanel flowRunbook;
        private Wisej.Web.Label lblStep1;
        private Wisej.Web.Label lblStep2;
        private Wisej.Web.Label lblStep3;
        private Wisej.Web.Label lblStep4;
        private Wisej.Web.Label lblStep5;
        private Wisej.Web.Label lblStep6;
        private Wisej.Web.Label lblStep7;
        private Wisej.Web.Label lblStep8;
        private Wisej.Web.Label lblNodesTitle;
        private Wisej.Web.DataGridView dgvNodes;
        private Wisej.Web.DataGridViewTextBoxColumn colNode;
        private Wisej.Web.DataGridViewTextBoxColumn colBuild;
        private Wisej.Web.DataGridViewTextBoxColumn colHealth;
        private Wisej.Web.DataGridViewTextBoxColumn colLoadBalancer;
        private Wisej.Web.Label lblHealthJson;
        private Wisej.Web.Label lblConfigTitle;
        private Wisej.Web.ComboBox cboEnvironment;
        private Wisej.Web.DataGridView dgvConfig;
        private Wisej.Web.DataGridViewTextBoxColumn colSetting;
        private Wisej.Web.DataGridViewTextBoxColumn colDevelopment;
        private Wisej.Web.DataGridViewTextBoxColumn colStaging;
        private Wisej.Web.DataGridViewTextBoxColumn colProduction;
        private Wisej.Web.DataGridViewTextBoxColumn colSecret;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnSmokeTests;
        private Wisej.Web.Button btnFailHealth;
        private Wisej.Web.Button btnAffinity;
        private Wisej.Web.Button btnInjectSecrets;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnClearTrace;
    }
}
