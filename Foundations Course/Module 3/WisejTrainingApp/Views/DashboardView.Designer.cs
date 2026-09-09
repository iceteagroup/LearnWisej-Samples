namespace WisejTrainingApp.Views
{
    partial class DashboardView
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
            this.lblViewTitle = new Wisej.Web.Label();
            this.pnlOpenTickets = new Wisej.Web.Panel();
            this.lblOpenTicketsCaption = new Wisej.Web.Label();
            this.lblOpenTicketsValue = new Wisej.Web.Label();
            this.pnlCustomers = new Wisej.Web.Panel();
            this.lblCustomersCaption = new Wisej.Web.Label();
            this.lblCustomersValue = new Wisej.Web.Label();
            this.pnlSystemStatus = new Wisej.Web.Panel();
            this.lblSystemStatusCaption = new Wisej.Web.Label();
            this.lblSystemStatusValue = new Wisej.Web.Label();
            this.lblSystemStatusDetail = new Wisej.Web.Label();
            this.btnRunHealthCheck = new Wisej.Web.Button();
            this.pnlActivity = new Wisej.Web.Panel();
            this.lblActivityCaption = new Wisej.Web.Label();
            this.lstEventLog = new Wisej.Web.ListBox();
            this.lblActivityFooter = new Wisej.Web.Label();
            this.pnlOpenTickets.SuspendLayout();
            this.pnlCustomers.SuspendLayout();
            this.pnlSystemStatus.SuspendLayout();
            this.pnlActivity.SuspendLayout();
            this.SuspendLayout();
            //
            // lblViewTitle
            //
            this.lblViewTitle.AutoSize = false;
            this.lblViewTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblViewTitle.Location = new System.Drawing.Point(0, 0);
            this.lblViewTitle.Name = "lblViewTitle";
            this.lblViewTitle.Size = new System.Drawing.Size(400, 30);
            this.lblViewTitle.Text = "Dashboard  ·  DashboardView (UserControl)";
            //
            // pnlOpenTickets  (card 1)
            //
            this.pnlOpenTickets.BackColor = System.Drawing.Color.White;
            this.pnlOpenTickets.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOpenTickets.Controls.Add(this.lblOpenTicketsCaption);
            this.pnlOpenTickets.Controls.Add(this.lblOpenTicketsValue);
            this.pnlOpenTickets.Location = new System.Drawing.Point(0, 44);
            this.pnlOpenTickets.Name = "pnlOpenTickets";
            this.pnlOpenTickets.Size = new System.Drawing.Size(330, 124);
            //
            // lblOpenTicketsCaption
            //
            this.lblOpenTicketsCaption.AutoSize = false;
            this.lblOpenTicketsCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblOpenTicketsCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOpenTicketsCaption.Location = new System.Drawing.Point(20, 14);
            this.lblOpenTicketsCaption.Name = "lblOpenTicketsCaption";
            this.lblOpenTicketsCaption.Size = new System.Drawing.Size(290, 22);
            this.lblOpenTicketsCaption.Text = "OPEN TICKETS";
            //
            // lblOpenTicketsValue
            //
            this.lblOpenTicketsValue.AutoSize = false;
            this.lblOpenTicketsValue.Font = new System.Drawing.Font("default", 28F, System.Drawing.FontStyle.Bold);
            this.lblOpenTicketsValue.Location = new System.Drawing.Point(20, 44);
            this.lblOpenTicketsValue.Name = "lblOpenTicketsValue";
            this.lblOpenTicketsValue.Size = new System.Drawing.Size(290, 60);
            this.lblOpenTicketsValue.Text = "0";
            //
            // pnlCustomers  (card 2)
            //
            this.pnlCustomers.BackColor = System.Drawing.Color.White;
            this.pnlCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlCustomers.Controls.Add(this.lblCustomersCaption);
            this.pnlCustomers.Controls.Add(this.lblCustomersValue);
            this.pnlCustomers.Location = new System.Drawing.Point(350, 44);
            this.pnlCustomers.Name = "pnlCustomers";
            this.pnlCustomers.Size = new System.Drawing.Size(330, 124);
            //
            // lblCustomersCaption
            //
            this.lblCustomersCaption.AutoSize = false;
            this.lblCustomersCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblCustomersCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomersCaption.Location = new System.Drawing.Point(20, 14);
            this.lblCustomersCaption.Name = "lblCustomersCaption";
            this.lblCustomersCaption.Size = new System.Drawing.Size(290, 22);
            this.lblCustomersCaption.Text = "CUSTOMERS";
            //
            // lblCustomersValue
            //
            this.lblCustomersValue.AutoSize = false;
            this.lblCustomersValue.Font = new System.Drawing.Font("default", 28F, System.Drawing.FontStyle.Bold);
            this.lblCustomersValue.Location = new System.Drawing.Point(20, 44);
            this.lblCustomersValue.Name = "lblCustomersValue";
            this.lblCustomersValue.Size = new System.Drawing.Size(290, 60);
            this.lblCustomersValue.Text = "0";
            //
            // pnlSystemStatus  (card 3, with the quick action)
            //
            this.pnlSystemStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlSystemStatus.BackColor = System.Drawing.Color.White;
            this.pnlSystemStatus.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlSystemStatus.Controls.Add(this.lblSystemStatusCaption);
            this.pnlSystemStatus.Controls.Add(this.lblSystemStatusValue);
            this.pnlSystemStatus.Controls.Add(this.lblSystemStatusDetail);
            this.pnlSystemStatus.Controls.Add(this.btnRunHealthCheck);
            this.pnlSystemStatus.Location = new System.Drawing.Point(700, 44);
            this.pnlSystemStatus.Name = "pnlSystemStatus";
            this.pnlSystemStatus.Size = new System.Drawing.Size(380, 124);
            //
            // lblSystemStatusCaption
            //
            this.lblSystemStatusCaption.AutoSize = false;
            this.lblSystemStatusCaption.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblSystemStatusCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSystemStatusCaption.Location = new System.Drawing.Point(20, 14);
            this.lblSystemStatusCaption.Name = "lblSystemStatusCaption";
            this.lblSystemStatusCaption.Size = new System.Drawing.Size(200, 22);
            this.lblSystemStatusCaption.Text = "SYSTEM STATUS";
            //
            // lblSystemStatusValue
            //
            this.lblSystemStatusValue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblSystemStatusValue.AutoSize = false;
            this.lblSystemStatusValue.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.lblSystemStatusValue.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSystemStatusValue.Location = new System.Drawing.Point(20, 40);
            this.lblSystemStatusValue.Name = "lblSystemStatusValue";
            this.lblSystemStatusValue.Size = new System.Drawing.Size(340, 30);
            this.lblSystemStatusValue.Text = "Not checked yet";
            //
            // lblSystemStatusDetail
            //
            this.lblSystemStatusDetail.AutoSize = false;
            this.lblSystemStatusDetail.Font = new System.Drawing.Font("default", 8F);
            this.lblSystemStatusDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSystemStatusDetail.Location = new System.Drawing.Point(20, 74);
            this.lblSystemStatusDetail.Name = "lblSystemStatusDetail";
            this.lblSystemStatusDetail.Size = new System.Drawing.Size(180, 40);
            this.lblSystemStatusDetail.Text = "Click Run Health Check.";
            this.lblSystemStatusDetail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnRunHealthCheck  (the quick action from lesson s12)
            //
            this.btnRunHealthCheck.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRunHealthCheck.Location = new System.Drawing.Point(204, 78);
            this.btnRunHealthCheck.Name = "btnRunHealthCheck";
            this.btnRunHealthCheck.Size = new System.Drawing.Size(156, 32);
            this.btnRunHealthCheck.Text = "Run Health Check";
            this.btnRunHealthCheck.ToolTipText = "Pings three simulated services; every third run the email gateway is slow.";
            this.btnRunHealthCheck.Click += new System.EventHandler(this.btnRunHealthCheck_Click);
            //
            // pnlActivity  (Recent activity · the event log, fed by the shell's Log())
            //
            this.pnlActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActivity.BackColor = System.Drawing.Color.White;
            this.pnlActivity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActivity.Controls.Add(this.lblActivityCaption);
            this.pnlActivity.Controls.Add(this.lstEventLog);
            this.pnlActivity.Controls.Add(this.lblActivityFooter);
            this.pnlActivity.Location = new System.Drawing.Point(0, 188);
            this.pnlActivity.Name = "pnlActivity";
            this.pnlActivity.Size = new System.Drawing.Size(1080, 348);
            //
            // lblActivityCaption
            //
            this.lblActivityCaption.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblActivityCaption.AutoSize = false;
            this.lblActivityCaption.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblActivityCaption.Location = new System.Drawing.Point(20, 14);
            this.lblActivityCaption.Name = "lblActivityCaption";
            this.lblActivityCaption.Size = new System.Drawing.Size(1040, 30);
            this.lblActivityCaption.Text = "Recent activity  ·  every navigation and permission decision the shell logged";
            //
            // lstEventLog
            //
            this.lstEventLog.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstEventLog.Font = new System.Drawing.Font("monospace", 9F);
            this.lstEventLog.Location = new System.Drawing.Point(20, 52);
            this.lstEventLog.Name = "lstEventLog";
            this.lstEventLog.Size = new System.Drawing.Size(1040, 250);
            //
            // lblActivityFooter
            //
            this.lblActivityFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblActivityFooter.AutoSize = false;
            this.lblActivityFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblActivityFooter.Location = new System.Drawing.Point(20, 310);
            this.lblActivityFooter.Name = "lblActivityFooter";
            this.lblActivityFooter.Size = new System.Drawing.Size(1040, 26);
            this.lblActivityFooter.Text = "nav button → NavigateTo(page) → pnlContent.Controls.Clear/Add → lblBreadcrumb + lblStatus → this list";
            //
            // DashboardView
            //
            this.Controls.Add(this.lblViewTitle);
            this.Controls.Add(this.pnlOpenTickets);
            this.Controls.Add(this.pnlCustomers);
            this.Controls.Add(this.pnlSystemStatus);
            this.Controls.Add(this.pnlActivity);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(1080, 536);
            this.Load += new System.EventHandler(this.DashboardView_Load);
            this.pnlOpenTickets.ResumeLayout(false);
            this.pnlCustomers.ResumeLayout(false);
            this.pnlSystemStatus.ResumeLayout(false);
            this.pnlActivity.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblViewTitle;
        private Wisej.Web.Panel pnlOpenTickets;
        private Wisej.Web.Label lblOpenTicketsCaption;
        private Wisej.Web.Label lblOpenTicketsValue;
        private Wisej.Web.Panel pnlCustomers;
        private Wisej.Web.Label lblCustomersCaption;
        private Wisej.Web.Label lblCustomersValue;
        private Wisej.Web.Panel pnlSystemStatus;
        private Wisej.Web.Label lblSystemStatusCaption;
        private Wisej.Web.Label lblSystemStatusValue;
        private Wisej.Web.Label lblSystemStatusDetail;
        private Wisej.Web.Button btnRunHealthCheck;
        private Wisej.Web.Panel pnlActivity;
        private Wisej.Web.Label lblActivityCaption;
        private Wisej.Web.ListBox lstEventLog;
        private Wisej.Web.Label lblActivityFooter;
    }
}
