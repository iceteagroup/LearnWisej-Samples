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
            this.lblPageDescription = new Wisej.Web.Label();
            this.pnlOpenCard = new Wisej.Web.Panel();
            this.lblOpenCaption = new Wisej.Web.Label();
            this.lblOpenValue = new Wisej.Web.Label();
            this.pnlInProgressCard = new Wisej.Web.Panel();
            this.lblInProgressCaption = new Wisej.Web.Label();
            this.lblInProgressValue = new Wisej.Web.Label();
            this.pnlClosedCard = new Wisej.Web.Panel();
            this.lblClosedCaption = new Wisej.Web.Label();
            this.lblClosedValue = new Wisej.Web.Label();
            this.pnlCommands = new Wisej.Web.Panel();
            this.lblCommandsTitle = new Wisej.Web.Label();
            this.btnNewTicket = new Wisej.Web.Button();
            this.btnRefresh = new Wisej.Web.Button();
            this.pnlActivity = new Wisej.Web.Panel();
            this.lblActivityTitle = new Wisej.Web.Label();
            this.lstActivity = new Wisej.Web.ListBox();
            this.pnlOpenCard.SuspendLayout();
            this.pnlInProgressCard.SuspendLayout();
            this.pnlClosedCard.SuspendLayout();
            this.pnlCommands.SuspendLayout();
            this.pnlActivity.SuspendLayout();
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
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = true;
            this.lblPageDescription.Location = new System.Drawing.Point(0, 38);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Text = "Today's ticket queue at a glance.";
            //
            // pnlOpenCard
            //
            this.pnlOpenCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOpenCard.Controls.Add(this.lblOpenCaption);
            this.pnlOpenCard.Controls.Add(this.lblOpenValue);
            this.pnlOpenCard.Location = new System.Drawing.Point(0, 76);
            this.pnlOpenCard.Name = "pnlOpenCard";
            this.pnlOpenCard.Size = new System.Drawing.Size(200, 96);
            //
            // lblOpenCaption
            //
            this.lblOpenCaption.AutoSize = true;
            this.lblOpenCaption.Location = new System.Drawing.Point(16, 12);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Text = "Open";
            //
            // lblOpenValue
            //
            this.lblOpenValue.AutoSize = true;
            this.lblOpenValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblOpenValue.Location = new System.Drawing.Point(16, 36);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Text = "0";
            //
            // pnlInProgressCard
            //
            this.pnlInProgressCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlInProgressCard.Controls.Add(this.lblInProgressCaption);
            this.pnlInProgressCard.Controls.Add(this.lblInProgressValue);
            this.pnlInProgressCard.Location = new System.Drawing.Point(216, 76);
            this.pnlInProgressCard.Name = "pnlInProgressCard";
            this.pnlInProgressCard.Size = new System.Drawing.Size(200, 96);
            //
            // lblInProgressCaption
            //
            this.lblInProgressCaption.AutoSize = true;
            this.lblInProgressCaption.Location = new System.Drawing.Point(16, 12);
            this.lblInProgressCaption.Name = "lblInProgressCaption";
            this.lblInProgressCaption.Text = "In Progress";
            //
            // lblInProgressValue
            //
            this.lblInProgressValue.AutoSize = true;
            this.lblInProgressValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblInProgressValue.Location = new System.Drawing.Point(16, 36);
            this.lblInProgressValue.Name = "lblInProgressValue";
            this.lblInProgressValue.Text = "0";
            //
            // pnlClosedCard
            //
            this.pnlClosedCard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlClosedCard.Controls.Add(this.lblClosedCaption);
            this.pnlClosedCard.Controls.Add(this.lblClosedValue);
            this.pnlClosedCard.Location = new System.Drawing.Point(432, 76);
            this.pnlClosedCard.Name = "pnlClosedCard";
            this.pnlClosedCard.Size = new System.Drawing.Size(200, 96);
            //
            // lblClosedCaption
            //
            this.lblClosedCaption.AutoSize = true;
            this.lblClosedCaption.Location = new System.Drawing.Point(16, 12);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Text = "Closed";
            //
            // lblClosedValue
            //
            this.lblClosedValue.AutoSize = true;
            this.lblClosedValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblClosedValue.Location = new System.Drawing.Point(16, 36);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Text = "0";
            //
            // pnlCommands
            //
            this.pnlCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCommands.Controls.Add(this.lblCommandsTitle);
            this.pnlCommands.Controls.Add(this.btnNewTicket);
            this.pnlCommands.Controls.Add(this.btnRefresh);
            this.pnlCommands.Location = new System.Drawing.Point(0, 188);
            this.pnlCommands.Name = "pnlCommands";
            this.pnlCommands.Size = new System.Drawing.Size(308, 110);
            //
            // lblCommandsTitle
            //
            this.lblCommandsTitle.AutoSize = true;
            this.lblCommandsTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblCommandsTitle.Location = new System.Drawing.Point(16, 12);
            this.lblCommandsTitle.Name = "lblCommandsTitle";
            this.lblCommandsTitle.Text = "Commands";
            //
            // btnNewTicket
            //
            this.btnNewTicket.Location = new System.Drawing.Point(16, 50);
            this.btnNewTicket.Name = "btnNewTicket";
            this.btnNewTicket.Size = new System.Drawing.Size(130, 34);
            this.btnNewTicket.Text = "New Ticket";
            this.btnNewTicket.Click += new System.EventHandler(this.btnNewTicket_Click);
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new System.Drawing.Point(158, 50);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(130, 34);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // pnlActivity
            //
            this.pnlActivity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActivity.Controls.Add(this.lblActivityTitle);
            this.pnlActivity.Controls.Add(this.lstActivity);
            this.pnlActivity.Location = new System.Drawing.Point(324, 188);
            this.pnlActivity.Name = "pnlActivity";
            this.pnlActivity.Size = new System.Drawing.Size(308, 250);
            //
            // lblActivityTitle
            //
            this.lblActivityTitle.AutoSize = true;
            this.lblActivityTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblActivityTitle.Location = new System.Drawing.Point(16, 12);
            this.lblActivityTitle.Name = "lblActivityTitle";
            this.lblActivityTitle.Text = "Recent activity";
            //
            // lstActivity
            //
            this.lstActivity.Location = new System.Drawing.Point(16, 44);
            this.lstActivity.Name = "lstActivity";
            this.lstActivity.Size = new System.Drawing.Size(274, 190);
            //
            // DashboardView
            //
            this.Controls.Add(this.pnlActivity);
            this.Controls.Add(this.pnlCommands);
            this.Controls.Add(this.pnlClosedCard);
            this.Controls.Add(this.pnlInProgressCard);
            this.Controls.Add(this.pnlOpenCard);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.lblPageTitle);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(660, 460);
            this.pnlOpenCard.ResumeLayout(false);
            this.pnlOpenCard.PerformLayout();
            this.pnlInProgressCard.ResumeLayout(false);
            this.pnlInProgressCard.PerformLayout();
            this.pnlClosedCard.ResumeLayout(false);
            this.pnlClosedCard.PerformLayout();
            this.pnlCommands.ResumeLayout(false);
            this.pnlCommands.PerformLayout();
            this.pnlActivity.ResumeLayout(false);
            this.pnlActivity.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel pnlOpenCard;
        private Wisej.Web.Label lblOpenCaption;
        private Wisej.Web.Label lblOpenValue;
        private Wisej.Web.Panel pnlInProgressCard;
        private Wisej.Web.Label lblInProgressCaption;
        private Wisej.Web.Label lblInProgressValue;
        private Wisej.Web.Panel pnlClosedCard;
        private Wisej.Web.Label lblClosedCaption;
        private Wisej.Web.Label lblClosedValue;
        private Wisej.Web.Panel pnlCommands;
        private Wisej.Web.Label lblCommandsTitle;
        private Wisej.Web.Button btnNewTicket;
        private Wisej.Web.Button btnRefresh;
        private Wisej.Web.Panel pnlActivity;
        private Wisej.Web.Label lblActivityTitle;
        private Wisej.Web.ListBox lstActivity;
    }
}
