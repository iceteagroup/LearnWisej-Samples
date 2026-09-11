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
            this.timerFirstRefresh = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
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
            this.panelScreen.Size = new System.Drawing.Size(760, 528);
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
            this.diagnosticsPage.Size = new System.Drawing.Size(756, 444);
            this.diagnosticsPage.RefreshRequested += new System.EventHandler(this.diagnosticsPage_RefreshRequested);
            //
            // timerFirstRefresh  (one-shot: the first refresh waits for the WebSocket connection)
            //
            this.timerFirstRefresh.Interval = 900;
            this.timerFirstRefresh.Tick += new System.EventHandler(this.timerFirstRefresh_Tick);
            //
            // ReleaseConsole
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 588);
            this.Controls.Add(this.panelScreen);
            this.Name = "ReleaseConsole";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.ReleaseConsole_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private TicketOps.Diagnostics.DiagnosticsPage diagnosticsPage;
        private Wisej.Web.Timer timerFirstRefresh;
    }
}
