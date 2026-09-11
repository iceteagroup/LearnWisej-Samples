namespace WisejTrainingApp.Views
{
    partial class DashboardView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.pnlTotal = new Wisej.Web.Panel();
            this.lblTotalCaption = new Wisej.Web.Label();
            this.lblTotal = new Wisej.Web.Label();
            this.pnlOpen = new Wisej.Web.Panel();
            this.lblOpenCaption = new Wisej.Web.Label();
            this.lblOpen = new Wisej.Web.Label();
            this.pnlInProgress = new Wisej.Web.Panel();
            this.lblInProgressCaption = new Wisej.Web.Label();
            this.lblInProgress = new Wisej.Web.Label();
            this.pnlClosed = new Wisej.Web.Panel();
            this.lblClosedCaption = new Wisej.Web.Label();
            this.lblClosed = new Wisej.Web.Label();
            this.pnlTotal.SuspendLayout();
            this.pnlOpen.SuspendLayout();
            this.pnlInProgress.SuspendLayout();
            this.pnlClosed.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(0, 0);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Text = "Dashboard";
            //
            // pnlTotal
            //
            this.pnlTotal.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTotal.Controls.Add(this.lblTotalCaption);
            this.pnlTotal.Controls.Add(this.lblTotal);
            this.pnlTotal.Location = new System.Drawing.Point(0, 50);
            this.pnlTotal.Name = "pnlTotal";
            this.pnlTotal.Size = new System.Drawing.Size(180, 96);
            //
            // lblTotalCaption
            //
            this.lblTotalCaption.AutoSize = true;
            this.lblTotalCaption.Location = new System.Drawing.Point(16, 12);
            this.lblTotalCaption.Name = "lblTotalCaption";
            this.lblTotalCaption.Text = "Total tickets";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(16, 36);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Text = "0";
            //
            // pnlOpen
            //
            this.pnlOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOpen.Controls.Add(this.lblOpenCaption);
            this.pnlOpen.Controls.Add(this.lblOpen);
            this.pnlOpen.Location = new System.Drawing.Point(196, 50);
            this.pnlOpen.Name = "pnlOpen";
            this.pnlOpen.Size = new System.Drawing.Size(180, 96);
            //
            // lblOpenCaption
            //
            this.lblOpenCaption.AutoSize = true;
            this.lblOpenCaption.Location = new System.Drawing.Point(16, 12);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Text = "Open";
            //
            // lblOpen
            //
            this.lblOpen.AutoSize = true;
            this.lblOpen.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblOpen.Location = new System.Drawing.Point(16, 36);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Text = "0";
            //
            // pnlInProgress
            //
            this.pnlInProgress.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlInProgress.Controls.Add(this.lblInProgressCaption);
            this.pnlInProgress.Controls.Add(this.lblInProgress);
            this.pnlInProgress.Location = new System.Drawing.Point(392, 50);
            this.pnlInProgress.Name = "pnlInProgress";
            this.pnlInProgress.Size = new System.Drawing.Size(180, 96);
            //
            // lblInProgressCaption
            //
            this.lblInProgressCaption.AutoSize = true;
            this.lblInProgressCaption.Location = new System.Drawing.Point(16, 12);
            this.lblInProgressCaption.Name = "lblInProgressCaption";
            this.lblInProgressCaption.Text = "In Progress";
            //
            // lblInProgress
            //
            this.lblInProgress.AutoSize = true;
            this.lblInProgress.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblInProgress.Location = new System.Drawing.Point(16, 36);
            this.lblInProgress.Name = "lblInProgress";
            this.lblInProgress.Text = "0";
            //
            // pnlClosed
            //
            this.pnlClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlClosed.Controls.Add(this.lblClosedCaption);
            this.pnlClosed.Controls.Add(this.lblClosed);
            this.pnlClosed.Location = new System.Drawing.Point(588, 50);
            this.pnlClosed.Name = "pnlClosed";
            this.pnlClosed.Size = new System.Drawing.Size(180, 96);
            //
            // lblClosedCaption
            //
            this.lblClosedCaption.AutoSize = true;
            this.lblClosedCaption.Location = new System.Drawing.Point(16, 12);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Text = "Closed";
            //
            // lblClosed
            //
            this.lblClosed.AutoSize = true;
            this.lblClosed.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblClosed.Location = new System.Drawing.Point(16, 36);
            this.lblClosed.Name = "lblClosed";
            this.lblClosed.Text = "0";
            //
            // DashboardView
            //
            this.Controls.Add(this.pnlClosed);
            this.Controls.Add(this.pnlInProgress);
            this.Controls.Add(this.pnlOpen);
            this.Controls.Add(this.pnlTotal);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(800, 500);
            this.pnlTotal.ResumeLayout(false);
            this.pnlTotal.PerformLayout();
            this.pnlOpen.ResumeLayout(false);
            this.pnlOpen.PerformLayout();
            this.pnlInProgress.ResumeLayout(false);
            this.pnlInProgress.PerformLayout();
            this.pnlClosed.ResumeLayout(false);
            this.pnlClosed.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Panel pnlTotal;
        private Wisej.Web.Label lblTotalCaption;
        private Wisej.Web.Label lblTotal;
        private Wisej.Web.Panel pnlOpen;
        private Wisej.Web.Label lblOpenCaption;
        private Wisej.Web.Label lblOpen;
        private Wisej.Web.Panel pnlInProgress;
        private Wisej.Web.Label lblInProgressCaption;
        private Wisej.Web.Label lblInProgress;
        private Wisej.Web.Panel pnlClosed;
        private Wisej.Web.Label lblClosedCaption;
        private Wisej.Web.Label lblClosed;
    }
}
