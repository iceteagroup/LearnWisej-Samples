namespace TicketOps.Views
{
    partial class MainPage
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
            this.pnlWorkspaceCard = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.pnlWorkspaceHost = new Wisej.Web.Panel();
            this.workspace = new TicketOps.Controls.TicketWorkspace();
            this.pnlWorkspaceCard.SuspendLayout();
            this.pnlWorkspaceHost.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlWorkspaceCard
            //
            this.pnlWorkspaceCard.BackColor = System.Drawing.Color.White;
            this.pnlWorkspaceCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlWorkspaceCard.Controls.Add(this.labelScreenTitle);
            this.pnlWorkspaceCard.Controls.Add(this.statusBanner);
            this.pnlWorkspaceCard.Controls.Add(this.pnlWorkspaceHost);
            this.pnlWorkspaceCard.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlWorkspaceCard.Name = "pnlWorkspaceCard";
            this.pnlWorkspaceCard.Size = new System.Drawing.Size(1308, 640);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Workspace";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(1260, 58);
            //
            // pnlWorkspaceHost
            //
            this.pnlWorkspaceHost.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlWorkspaceHost.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.pnlWorkspaceHost.Controls.Add(this.workspace);
            this.pnlWorkspaceHost.Location = new System.Drawing.Point(24, 80);
            this.pnlWorkspaceHost.Name = "pnlWorkspaceHost";
            this.pnlWorkspaceHost.Size = new System.Drawing.Size(1260, 536);
            //
            // workspace
            //
            this.workspace.Dock = Wisej.Web.DockStyle.Fill;
            this.workspace.Name = "workspace";
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlWorkspaceCard);
            this.Name = "MainPage";
            this.Padding = new Wisej.Web.Padding(20);
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.pnlWorkspaceCard.ResumeLayout(false);
            this.pnlWorkspaceHost.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlWorkspaceCard;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Panel pnlWorkspaceHost;
        private TicketOps.Controls.TicketWorkspace workspace;
    }
}
