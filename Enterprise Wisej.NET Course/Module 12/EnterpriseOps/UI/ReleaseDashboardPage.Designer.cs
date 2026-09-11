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
            this.pnlRelease = new Wisej.Web.Panel();
            this.lblReleaseTitle = new Wisej.Web.Label();
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
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlRelease.SuspendLayout();
            this.flowRunbook.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(860, 44);
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
            // pnlRelease
            //
            this.pnlRelease.BackColor = System.Drawing.Color.White;
            this.pnlRelease.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlRelease.Controls.Add(this.lblReleaseTitle);
            this.pnlRelease.Controls.Add(this.btnDeploy);
            this.pnlRelease.Controls.Add(this.btnRollback);
            this.pnlRelease.Controls.Add(this.lblRunbookTitle);
            this.pnlRelease.Controls.Add(this.flowRunbook);
            this.pnlRelease.Controls.Add(this.lblNodesTitle);
            this.pnlRelease.Controls.Add(this.dgvNodes);
            this.pnlRelease.Controls.Add(this.lblHealthJson);
            this.pnlRelease.Controls.Add(this.lblBanner);
            this.pnlRelease.Controls.Add(this.lblStatusBar);
            this.pnlRelease.Location = new System.Drawing.Point(24, 64);
            this.pnlRelease.Name = "pnlRelease";
            this.pnlRelease.Size = new System.Drawing.Size(812, 380);
            //
            // lblReleaseTitle
            //
            this.lblReleaseTitle.AutoSize = false;
            this.lblReleaseTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblReleaseTitle.Location = new System.Drawing.Point(20, 12);
            this.lblReleaseTitle.Name = "lblReleaseTitle";
            this.lblReleaseTitle.Size = new System.Drawing.Size(520, 28);
            this.lblReleaseTitle.Text = "Release dashboard — EnterpriseOps";
            this.lblReleaseTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnDeploy
            //
            this.btnDeploy.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.btnDeploy.Location = new System.Drawing.Point(562, 10);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(112, 32);
            this.btnDeploy.Text = "Deploy…";
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            //
            // btnRollback
            //
            this.btnRollback.Location = new System.Drawing.Point(682, 10);
            this.btnRollback.Name = "btnRollback";
            this.btnRollback.Size = new System.Drawing.Size(110, 32);
            this.btnRollback.Text = "Rollback…";
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
            this.lblRunbookTitle.Text = "RELEASE RUNBOOK";
            //
            // flowRunbook
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
            // lblStep1
            //
            this.lblStep1.AutoSize = false;
            this.lblStep1.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep1.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep1.Size = new System.Drawing.Size(180, 26);
            this.lblStep1.Text = "· build";
            this.lblStep1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep2
            //
            this.lblStep2.AutoSize = false;
            this.lblStep2.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep2.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep2.Size = new System.Drawing.Size(180, 26);
            this.lblStep2.Text = "· config + secrets";
            this.lblStep2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep3
            //
            this.lblStep3.AutoSize = false;
            this.lblStep3.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep3.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep3.Size = new System.Drawing.Size(180, 26);
            this.lblStep3.Text = "· staging";
            this.lblStep3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep4
            //
            this.lblStep4.AutoSize = false;
            this.lblStep4.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep4.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep4.Name = "lblStep4";
            this.lblStep4.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep4.Size = new System.Drawing.Size(180, 26);
            this.lblStep4.Text = "· smoke tests";
            this.lblStep4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep5
            //
            this.lblStep5.AutoSize = false;
            this.lblStep5.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep5.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep5.Name = "lblStep5";
            this.lblStep5.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep5.Size = new System.Drawing.Size(180, 26);
            this.lblStep5.Text = "· health + diagnostics";
            this.lblStep5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep6
            //
            this.lblStep6.AutoSize = false;
            this.lblStep6.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep6.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep6.Name = "lblStep6";
            this.lblStep6.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep6.Size = new System.Drawing.Size(180, 26);
            this.lblStep6.Text = "· production";
            this.lblStep6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep7
            //
            this.lblStep7.AutoSize = false;
            this.lblStep7.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStep7.Margin = new Wisej.Web.Padding(0, 0, 6, 6);
            this.lblStep7.Name = "lblStep7";
            this.lblStep7.Padding = new Wisej.Web.Padding(8, 0, 8, 0);
            this.lblStep7.Size = new System.Drawing.Size(180, 26);
            this.lblStep7.Text = "· monitor";
            this.lblStep7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStep8
            //
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
            // dgvNodes
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
            // lblHealthJson
            //
            this.lblHealthJson.AutoSize = false;
            this.lblHealthJson.BackColor = System.Drawing.Color.FromArgb(16, 23, 31);
            this.lblHealthJson.Font = new System.Drawing.Font("monospace", 8F);
            this.lblHealthJson.ForeColor = System.Drawing.Color.FromArgb(255, 154, 168);
            this.lblHealthJson.Location = new System.Drawing.Point(20, 264);
            this.lblHealthJson.Name = "lblHealthJson";
            this.lblHealthJson.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.lblHealthJson.Size = new System.Drawing.Size(772, 30);
            this.lblHealthJson.Text = "";
            this.lblHealthJson.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblHealthJson.Visible = false;
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 300);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 32);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 8F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 338);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 30);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ReleaseDashboardPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlRelease);
            this.Name = "ReleaseDashboardPage";
            this.Size = new System.Drawing.Size(860, 468);
            this.Text = "EnterpriseOps — Release dashboard";
            this.Load += new System.EventHandler(this.ReleaseDashboardPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlRelease.ResumeLayout(false);
            this.flowRunbook.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlRelease;
        private Wisej.Web.Label lblReleaseTitle;
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
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
    }
}
