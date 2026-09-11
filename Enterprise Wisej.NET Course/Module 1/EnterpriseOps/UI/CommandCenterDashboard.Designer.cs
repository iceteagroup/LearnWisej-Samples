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
            this.pnlDashboard = new Wisej.Web.Panel();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblUser = new Wisej.Web.Label();
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
            this.pnlHeader.SuspendLayout();
            this.pnlDashboard.SuspendLayout();
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
            this.lblTitle.Size = new System.Drawing.Size(420, 44);
            this.lblTitle.Text = "EnterpriseOps — Command Center";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlDashboard
            //
            this.pnlDashboard.BackColor = System.Drawing.Color.White;
            this.pnlDashboard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlDashboard.Controls.Add(this.btnRefresh);
            this.pnlDashboard.Controls.Add(this.lblUser);
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
            // btnRefresh
            //
            this.btnRefresh.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(20, 14);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 36);
            this.btnRefresh.Text = "⟳ Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // lblUser
            //
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 10F);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblUser.Location = new System.Drawing.Point(392, 19);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(400, 26);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // kpiOpenIncidents
            //
            this.kpiOpenIncidents.Accent = System.Drawing.Color.FromArgb(192, 57, 43);
            this.kpiOpenIncidents.Caption = "OPEN INCIDENTS";
            this.kpiOpenIncidents.Location = new System.Drawing.Point(20, 64);
            this.kpiOpenIncidents.Name = "kpiOpenIncidents";
            this.kpiOpenIncidents.Size = new System.Drawing.Size(248, 84);
            this.kpiOpenIncidents.Value = "—";
            //
            // kpiSlaAtRisk
            //
            this.kpiSlaAtRisk.Accent = System.Drawing.Color.FromArgb(185, 119, 14);
            this.kpiSlaAtRisk.Caption = "SLA AT RISK";
            this.kpiSlaAtRisk.Location = new System.Drawing.Point(282, 64);
            this.kpiSlaAtRisk.Name = "kpiSlaAtRisk";
            this.kpiSlaAtRisk.Size = new System.Drawing.Size(248, 84);
            this.kpiSlaAtRisk.Value = "—";
            //
            // kpiDeploymentsToday
            //
            this.kpiDeploymentsToday.Accent = System.Drawing.Color.FromArgb(31, 138, 76);
            this.kpiDeploymentsToday.Caption = "DEPLOYMENTS TODAY";
            this.kpiDeploymentsToday.Location = new System.Drawing.Point(544, 64);
            this.kpiDeploymentsToday.Name = "kpiDeploymentsToday";
            this.kpiDeploymentsToday.Size = new System.Drawing.Size(248, 84);
            this.kpiDeploymentsToday.Value = "—";
            //
            // dgvIncidents
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
            // lblBanner
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
            // lblStatusBar
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
            // CommandCenterDashboard
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlDashboard);
            this.Name = "CommandCenterDashboard";
            this.Size = new System.Drawing.Size(860, 594);
            this.Text = "EnterpriseOps — Command Center";
            this.Load += new System.EventHandler(this.CommandCenterDashboard_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlDashboard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlDashboard;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblUser;
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
    }
}
