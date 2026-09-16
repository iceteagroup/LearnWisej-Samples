namespace WisejPerfLab.Pages
{
    partial class DashboardPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Designer generated code

        private void InitializeComponent()
        {
            this.panelKpis = new Wisej.Web.Panel();
            this.chartLoad = new Wisej.Web.Ext.ChartJS.ChartJS();
            this.btnRefresh = new Wisej.Web.Button();
            this.lblElapsed = new Wisej.Web.Label();
            this.lblScenario = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // panelKpis
            //
            this.panelKpis.Location = new System.Drawing.Point(14, 12);
            this.panelKpis.Name = "panelKpis";
            this.panelKpis.Size = new System.Drawing.Size(846, 92);
            this.panelKpis.TabIndex = 0;
            //
            // chartLoad
            //
            this.chartLoad.ChartType = Wisej.Web.Ext.ChartJS.ChartType.Bar;
            this.chartLoad.Location = new System.Drawing.Point(14, 116);
            this.chartLoad.Name = "chartLoad";
            this.chartLoad.Size = new System.Drawing.Size(846, 228);
            this.chartLoad.TabIndex = 1;
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(14, 356);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(140, 36);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += this.btnRefresh_Click;
            //
            // lblElapsed
            //
            this.lblElapsed.Font = new System.Drawing.Font("monospace", 9F);
            this.lblElapsed.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblElapsed.Location = new System.Drawing.Point(166, 366);
            this.lblElapsed.Name = "lblElapsed";
            this.lblElapsed.Size = new System.Drawing.Size(694, 18);
            this.lblElapsed.TabIndex = 3;
            this.lblElapsed.Text = "not measured yet";
            //
            // lblScenario
            //
            this.lblScenario.Font = new System.Drawing.Font("monospace", 8F);
            this.lblScenario.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblScenario.Location = new System.Drawing.Point(14, 398);
            this.lblScenario.Name = "lblScenario";
            this.lblScenario.Size = new System.Drawing.Size(846, 18);
            this.lblScenario.TabIndex = 4;
            this.lblScenario.Text = "scenario Dashboard/Refresh — budget 250 ms — tool: CPU Usage — fixed in Module 3: 4 aggregate queries, values assigned";
            //
            // DashboardPage
            //
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblScenario);
            this.Controls.Add(this.lblElapsed);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.chartLoad);
            this.Controls.Add(this.panelKpis);
            this.Name = "DashboardPage";
            this.Size = new System.Drawing.Size(876, 440);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelKpis;
        private Wisej.Web.Ext.ChartJS.ChartJS chartLoad;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Label lblElapsed;
        private Wisej.Web.Label lblScenario;
    }
}
