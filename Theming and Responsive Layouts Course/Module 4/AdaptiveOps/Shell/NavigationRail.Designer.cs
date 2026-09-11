namespace AdaptiveOps.Shell
{
    partial class NavigationRail
    {
        private System.ComponentModel.IContainer components = null;

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
            this.lblNavTitle = new Wisej.Web.Label();
            this.btnNavDashboard = new Wisej.Web.Button();
            this.btnNavTickets = new Wisej.Web.Button();
            this.btnNavReports = new Wisej.Web.Button();
            this.btnNavAssets = new Wisej.Web.Button();
            this.btnNavSchedules = new Wisej.Web.Button();
            this.btnNavSettings = new Wisej.Web.Button();
            this.btnNavHelp = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblNavTitle  (AutoSize = true: the content decides the size)
            //
            this.lblNavTitle.AutoSize = true;
            this.lblNavTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblNavTitle.Location = new System.Drawing.Point(12, 12);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // Section buttons: Anchor Top | Left | Right, so they stretch with the rail and stay put vertically.
            //
            this.btnNavDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavDashboard.Location = new System.Drawing.Point(12, 40);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(186, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavTickets.Location = new System.Drawing.Point(12, 84);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(186, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavReports.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavReports.Location = new System.Drawing.Point(12, 128);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(186, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavAssets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavAssets.Location = new System.Drawing.Point(12, 172);
            this.btnNavAssets.Name = "btnNavAssets";
            this.btnNavAssets.Size = new System.Drawing.Size(186, 36);
            this.btnNavAssets.Text = "Assets";
            this.btnNavAssets.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavSchedules.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavSchedules.Location = new System.Drawing.Point(12, 216);
            this.btnNavSchedules.Name = "btnNavSchedules";
            this.btnNavSchedules.Size = new System.Drawing.Size(186, 36);
            this.btnNavSchedules.Text = "Schedules";
            this.btnNavSchedules.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavSettings.Location = new System.Drawing.Point(12, 260);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(186, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavHelp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavHelp.Location = new System.Drawing.Point(12, 304);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(186, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // NavigationRail
            //
            // AutoScroll + ScrollBars.Hidden: in a short browser window the rail scrolls by wheel or
            // touch instead of clipping the last sections, and no scrollbar steals width.
            //
            this.AutoScroll = true;
            this.AutoScrollMargin = new System.Drawing.Size(0, 12);
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblNavTitle);
            this.Controls.Add(this.btnNavDashboard);
            this.Controls.Add(this.btnNavTickets);
            this.Controls.Add(this.btnNavReports);
            this.Controls.Add(this.btnNavAssets);
            this.Controls.Add(this.btnNavSchedules);
            this.Controls.Add(this.btnNavSettings);
            this.Controls.Add(this.btnNavHelp);
            this.Name = "NavigationRail";
            this.ScrollBars = Wisej.Web.ScrollBars.Hidden;
            this.Size = new System.Drawing.Size(212, 588);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavAssets;
        private Wisej.Web.Button btnNavSchedules;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;
    }
}
