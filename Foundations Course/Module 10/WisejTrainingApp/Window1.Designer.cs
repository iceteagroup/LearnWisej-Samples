namespace WisejTrainingApp
{
    partial class Window1
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.pnlNav = new Wisej.Web.Panel();
            this.btnDashboard = new Wisej.Web.Button();
            this.btnTickets = new Wisej.Web.Button();
            this.btnArchitecture = new Wisej.Web.Button();
            this.btnCodeReview = new Wisej.Web.Button();
            this.btnDeployment = new Wisej.Web.Button();
            this.btnNextSteps = new Wisej.Web.Button();
            this.pnlContent = new Wisej.Web.Panel();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.lblAppTitle);
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1000, 56);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = true;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 16F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.Location = new System.Drawing.Point(20, 12);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Text = "Mini Helpdesk";
            //
            // pnlNav
            //
            this.pnlNav.Controls.Add(this.btnDashboard);
            this.pnlNav.Controls.Add(this.btnTickets);
            this.pnlNav.Controls.Add(this.btnArchitecture);
            this.pnlNav.Controls.Add(this.btnCodeReview);
            this.pnlNav.Controls.Add(this.btnDeployment);
            this.pnlNav.Controls.Add(this.btnNextSteps);
            this.pnlNav.Dock = Wisej.Web.DockStyle.Left;
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(190, 596);
            //
            // btnDashboard
            //
            this.btnDashboard.Location = new System.Drawing.Point(12, 16);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(166, 36);
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            //
            // btnTickets
            //
            this.btnTickets.Location = new System.Drawing.Point(12, 60);
            this.btnTickets.Name = "btnTickets";
            this.btnTickets.Size = new System.Drawing.Size(166, 36);
            this.btnTickets.Text = "Tickets";
            this.btnTickets.Click += new System.EventHandler(this.btnTickets_Click);
            //
            // btnArchitecture
            //
            this.btnArchitecture.Location = new System.Drawing.Point(12, 104);
            this.btnArchitecture.Name = "btnArchitecture";
            this.btnArchitecture.Size = new System.Drawing.Size(166, 36);
            this.btnArchitecture.Text = "Architecture";
            this.btnArchitecture.Click += new System.EventHandler(this.btnArchitecture_Click);
            //
            // btnCodeReview
            //
            this.btnCodeReview.Location = new System.Drawing.Point(12, 148);
            this.btnCodeReview.Name = "btnCodeReview";
            this.btnCodeReview.Size = new System.Drawing.Size(166, 36);
            this.btnCodeReview.Text = "Code Review";
            this.btnCodeReview.Click += new System.EventHandler(this.btnCodeReview_Click);
            //
            // btnDeployment
            //
            this.btnDeployment.Location = new System.Drawing.Point(12, 192);
            this.btnDeployment.Name = "btnDeployment";
            this.btnDeployment.Size = new System.Drawing.Size(166, 36);
            this.btnDeployment.Text = "Deployment";
            this.btnDeployment.Click += new System.EventHandler(this.btnDeployment_Click);
            //
            // btnNextSteps
            //
            this.btnNextSteps.Location = new System.Drawing.Point(12, 236);
            this.btnNextSteps.Name = "btnNextSteps";
            this.btnNextSteps.Size = new System.Drawing.Size(166, 36);
            this.btnNextSteps.Text = "Next Steps";
            this.btnNextSteps.Click += new System.EventHandler(this.btnNextSteps_Click);
            //
            // pnlContent
            //
            this.pnlContent.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new Wisej.Web.Padding(20);
            //
            // lblStatus
            //
            this.lblStatus.Dock = Wisej.Web.DockStyle.Bottom;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatus.Size = new System.Drawing.Size(1000, 28);
            this.lblStatus.Text = "Ready.";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // Window1
            //
            this.ClientSize = new System.Drawing.Size(1000, 660);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.pnlHeader);
            this.Name = "Window1";
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "Mini Helpdesk";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlNav.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Panel pnlNav;
        private Wisej.Web.Button btnDashboard;
        private Wisej.Web.Button btnTickets;
        private Wisej.Web.Button btnArchitecture;
        private Wisej.Web.Button btnCodeReview;
        private Wisej.Web.Button btnDeployment;
        private Wisej.Web.Button btnNextSteps;
        private Wisej.Web.Panel pnlContent;
        private Wisej.Web.Label lblStatus;
    }
}
