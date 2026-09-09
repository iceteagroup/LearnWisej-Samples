namespace WisejTrainingApp.Views
{
    partial class ReportsView
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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblPageDescription = new Wisej.Web.Label();
            this.cardByStatus = new Wisej.Web.Panel();
            this.lblByStatusTitle = new Wisej.Web.Label();
            this.lstByStatus = new Wisej.Web.ListBox();
            this.cardByPriority = new Wisej.Web.Panel();
            this.lblByPriorityTitle = new Wisej.Web.Label();
            this.lstByPriority = new Wisej.Web.ListBox();
            this.lblReportsSummary = new Wisej.Web.Label();
            this.cardByStatus.SuspendLayout();
            this.cardByPriority.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(32, 24);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 34);
            this.lblPageTitle.Text = "Reports";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = false;
            this.lblPageDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPageDescription.Location = new System.Drawing.Point(32, 60);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(1000, 22);
            this.lblPageDescription.Text = "Tickets by status and by priority — two cards, 524 px wide, 24 px apart, like the Dashboard row.";
            //
            // cardByStatus  (524 × 220 at 32,100)
            //
            this.cardByStatus.BackColor = System.Drawing.Color.White;
            this.cardByStatus.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardByStatus.Controls.Add(this.lblByStatusTitle);
            this.cardByStatus.Controls.Add(this.lstByStatus);
            this.cardByStatus.Location = new System.Drawing.Point(32, 100);
            this.cardByStatus.Name = "cardByStatus";
            this.cardByStatus.Size = new System.Drawing.Size(524, 220);
            //
            // lblByStatusTitle
            //
            this.lblByStatusTitle.AutoSize = false;
            this.lblByStatusTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblByStatusTitle.Location = new System.Drawing.Point(20, 14);
            this.lblByStatusTitle.Name = "lblByStatusTitle";
            this.lblByStatusTitle.Size = new System.Drawing.Size(484, 28);
            this.lblByStatusTitle.Text = "Tickets by status";
            //
            // lstByStatus
            //
            this.lstByStatus.Font = new System.Drawing.Font("monospace", 9F);
            this.lstByStatus.Location = new System.Drawing.Point(20, 52);
            this.lstByStatus.Name = "lstByStatus";
            this.lstByStatus.Size = new System.Drawing.Size(484, 148);
            //
            // cardByPriority  (524 × 220 at 580,100 — 32 + 524 + 24)
            //
            this.cardByPriority.BackColor = System.Drawing.Color.White;
            this.cardByPriority.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardByPriority.Controls.Add(this.lblByPriorityTitle);
            this.cardByPriority.Controls.Add(this.lstByPriority);
            this.cardByPriority.Location = new System.Drawing.Point(580, 100);
            this.cardByPriority.Name = "cardByPriority";
            this.cardByPriority.Size = new System.Drawing.Size(524, 220);
            //
            // lblByPriorityTitle
            //
            this.lblByPriorityTitle.AutoSize = false;
            this.lblByPriorityTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblByPriorityTitle.Location = new System.Drawing.Point(20, 14);
            this.lblByPriorityTitle.Name = "lblByPriorityTitle";
            this.lblByPriorityTitle.Size = new System.Drawing.Size(484, 28);
            this.lblByPriorityTitle.Text = "Tickets by priority";
            //
            // lstByPriority
            //
            this.lstByPriority.Font = new System.Drawing.Font("monospace", 9F);
            this.lstByPriority.Location = new System.Drawing.Point(20, 52);
            this.lstByPriority.Name = "lstByPriority";
            this.lstByPriority.Size = new System.Drawing.Size(484, 148);
            //
            // lblReportsSummary  (24 px under the cards)
            //
            this.lblReportsSummary.AutoSize = false;
            this.lblReportsSummary.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblReportsSummary.Location = new System.Drawing.Point(32, 344);
            this.lblReportsSummary.Name = "lblReportsSummary";
            this.lblReportsSummary.Size = new System.Drawing.Size(1072, 24);
            this.lblReportsSummary.Text = "";
            //
            // ReportsView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblPageTitle);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.cardByStatus);
            this.Controls.Add(this.cardByPriority);
            this.Controls.Add(this.lblReportsSummary);
            this.Name = "ReportsView";
            this.Size = new System.Drawing.Size(1148, 620);
            this.cardByStatus.ResumeLayout(false);
            this.cardByPriority.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel cardByStatus;
        private Wisej.Web.Label lblByStatusTitle;
        private Wisej.Web.ListBox lstByStatus;
        private Wisej.Web.Panel cardByPriority;
        private Wisej.Web.Label lblByPriorityTitle;
        private Wisej.Web.ListBox lstByPriority;
        private Wisej.Web.Label lblReportsSummary;
    }
}
