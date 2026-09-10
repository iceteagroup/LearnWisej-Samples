namespace AdaptiveOps.Shell
{
    partial class NavigationRail
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.lblNavTitle = new Wisej.Web.Label();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnReports = new Wisej.Web.Button();
            this.btnSettings = new Wisej.Web.Button();
            this.btnHelp = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblNavTitle
            //
            this.lblNavTitle.AppearanceKey = "metric-title";
            this.lblNavTitle.AutoSize = false;
            this.lblNavTitle.CssClass = "metric-title";
            this.lblNavTitle.Dock = Wisej.Web.DockStyle.Top;
            this.lblNavTitle.Name = "lblNavTitle";
            this.lblNavTitle.Padding = new Wisej.Web.Padding(12, 0, 0, 0);
            this.lblNavTitle.Size = new System.Drawing.Size(204, 28);
            this.lblNavTitle.TabStop = false;
            this.lblNavTitle.Text = "NAVIGATION";
            this.lblNavTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // nav-item buttons  (Dock = Top; the one added LAST is docked FIRST, so Help is added first)
            //
            InitNavButton(this.btnDashboard, "Dashboard", "icon-preview", 1, "Dashboard: metric cards and the ticket grid");
            InitNavButton(this.btnTickets, "Tickets", "icon-file", 2, "Tickets: the same grid with the details editor");
            InitNavButton(this.btnReports, "Reports", "icon-print", 3, "Reports: open tickets by owner (created lazily on first use)");
            InitNavButton(this.btnSettings, "Settings", "icon-settings", 4, "Settings (placeholder view)");
            InitNavButton(this.btnHelp, "Help", "icon-help", 5, "Help (placeholder view)");
            //
            // NavigationRail  (theme appearance rail-surface)
            //
            this.AppearanceKey = "rail-surface";
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnSettings);
            this.Controls.Add(this.btnReports);
            this.Controls.Add(this.btnTickets);
            this.Controls.Add(this.btnDashboard);
            this.Controls.Add(this.lblNavTitle);
            this.Name = "NavigationRail";
            this.Padding = new Wisej.Web.Padding(8);
            this.Size = new System.Drawing.Size(212, 588);
            this.TabStop = false;
            this.ResumeLayout(false);
        }

        private void InitNavButton(Wisej.Web.Button button, string text, string icon, int tabIndex, string tooltip)
        {
            button.AccessibleName = text;
            button.AppearanceKey = "nav-item";
            button.Dock = Wisej.Web.DockStyle.Top;
            button.Display = Wisej.Web.Display.Both;
            button.ImageSource = icon;
            button.Name = "btnNav" + text;
            button.Size = new System.Drawing.Size(196, 40);
            button.TabIndex = tabIndex;
            button.Tag = text;
            button.Text = text;
            button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            button.ToolTipText = tooltip;
            button.Click += new System.EventHandler(this.btnNav_Click);
        }

        #endregion

        private Wisej.Web.Label lblNavTitle;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnReports;
        private Wisej.Web.Button btnSettings;
        private Wisej.Web.Button btnHelp;
    }
}
