namespace AdaptiveOps.Shell
{
    partial class NavigationRail
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
            this.lblNavTitle = new Wisej.Web.Label();
            this.btnNavDashboard = new Wisej.Web.Button();
            this.btnNavTickets = new Wisej.Web.Button();
            this.btnNavReports = new Wisej.Web.Button();
            this.btnNavSettings = new Wisej.Web.Button();
            this.btnNavHelp = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblNavTitle  (hidden in icon-only mode)
            //
            this.lblNavTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblNavTitle.ForeColor = System.Drawing.Color.FromArgb(103, 112, 133);
            this.lblNavTitle.Location = new System.Drawing.Point(12, 12);
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Size = new System.Drawing.Size(180, 18);
            this.lblNavTitle.Text = "NAVIGATION";
            //
            // btnNavDashboard … btnNavHelp
            //
            // Every button carries a theme icon (ImageSource = theme image name) so Display.Icon has something to
            // show, and Anchor Left|Right so the buttons shrink with the rail when the region goes to 64 px.
            //
            this.btnNavDashboard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavDashboard.ImageSource = "icon-columns";
            this.btnNavDashboard.Location = new System.Drawing.Point(12, 40);
            this.btnNavDashboard.Name = "btnNavDashboard";
            this.btnNavDashboard.Size = new System.Drawing.Size(180, 36);
            this.btnNavDashboard.Text = "Dashboard";
            this.btnNavDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavDashboard.ToolTipText = "Dashboard";
            this.btnNavDashboard.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavTickets.ImageSource = "icon-file";
            this.btnNavTickets.Location = new System.Drawing.Point(12, 84);
            this.btnNavTickets.Name = "btnNavTickets";
            this.btnNavTickets.Size = new System.Drawing.Size(180, 36);
            this.btnNavTickets.Text = "Tickets";
            this.btnNavTickets.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavTickets.ToolTipText = "Tickets";
            this.btnNavTickets.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavReports.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavReports.ImageSource = "icon-print";
            this.btnNavReports.Location = new System.Drawing.Point(12, 128);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(180, 36);
            this.btnNavReports.Text = "Reports";
            this.btnNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReports.ToolTipText = "Reports";
            this.btnNavReports.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavSettings.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavSettings.ImageSource = "icon-settings";
            this.btnNavSettings.Location = new System.Drawing.Point(12, 172);
            this.btnNavSettings.Name = "btnNavSettings";
            this.btnNavSettings.Size = new System.Drawing.Size(180, 36);
            this.btnNavSettings.Text = "Settings";
            this.btnNavSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavSettings.ToolTipText = "Settings";
            this.btnNavSettings.Click += new System.EventHandler(this.btnNav_Click);
            this.btnNavHelp.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.btnNavHelp.ImageSource = "icon-help";
            this.btnNavHelp.Location = new System.Drawing.Point(12, 216);
            this.btnNavHelp.Name = "btnNavHelp";
            this.btnNavHelp.Size = new System.Drawing.Size(180, 36);
            this.btnNavHelp.Text = "Help";
            this.btnNavHelp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavHelp.ToolTipText = "Help";
            this.btnNavHelp.Click += new System.EventHandler(this.btnNav_Click);
            //
            // NavigationRail  (the white card itself; the page's navigationPanel is the transparent Dock region around it)
            //
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.Controls.Add(this.lblNavTitle);
            this.Controls.Add(this.btnNavDashboard);
            this.Controls.Add(this.btnNavTickets);
            this.Controls.Add(this.btnNavReports);
            this.Controls.Add(this.btnNavSettings);
            this.Controls.Add(this.btnNavHelp);
            this.Name = "NavigationRail";
            this.Size = new System.Drawing.Size(204, 588);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnNavDashboard;
        private Wisej.Web.Button btnNavTickets;
        private Wisej.Web.Button btnNavReports;
        private Wisej.Web.Button btnNavSettings;
        private Wisej.Web.Button btnNavHelp;
    }
}
