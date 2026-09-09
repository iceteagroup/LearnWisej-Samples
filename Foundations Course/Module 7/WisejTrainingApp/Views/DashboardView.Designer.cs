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
            this.lblPageTitle = new Wisej.Web.Label();
            this.lblPageDescription = new Wisej.Web.Label();
            this.cardOpen = new Wisej.Web.Panel();
            this.lblOpenValue = new Wisej.Web.Label();
            this.lblOpenCaption = new Wisej.Web.Label();
            this.cardInProgress = new Wisej.Web.Panel();
            this.lblInProgressValue = new Wisej.Web.Label();
            this.lblInProgressCaption = new Wisej.Web.Label();
            this.cardClosed = new Wisej.Web.Panel();
            this.lblClosedValue = new Wisej.Web.Label();
            this.lblClosedCaption = new Wisej.Web.Label();
            this.cardCustomers = new Wisej.Web.Panel();
            this.lblCustomersValue = new Wisej.Web.Label();
            this.lblCustomersCaption = new Wisej.Web.Label();
            this.cardCommands = new Wisej.Web.Panel();
            this.lblCommandsTitle = new Wisej.Web.Label();
            this.btnCreateTicket = new Wisej.Web.Button();
            this.btnEditTicket = new Wisej.Web.Button();
            this.btnCloseTicket = new Wisej.Web.Button();
            this.lblCommandsHint = new Wisej.Web.Label();
            this.cardActivity = new Wisej.Web.Panel();
            this.lblActivityTitle = new Wisej.Web.Label();
            this.lstActivity = new Wisej.Web.ListBox();
            this.btnClearActivity = new Wisej.Web.Button();
            this.cardOpen.SuspendLayout();
            this.cardInProgress.SuspendLayout();
            this.cardClosed.SuspendLayout();
            this.cardCustomers.SuspendLayout();
            this.cardCommands.SuspendLayout();
            this.cardActivity.SuspendLayout();
            this.SuspendLayout();
            //
            // lblPageTitle  (rule 1: the page title is obvious — 18 pt bold at 32,24 on every page)
            //
            this.lblPageTitle.AutoSize = false;
            this.lblPageTitle.Font = new System.Drawing.Font("default", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(32, 24);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 34);
            this.lblPageTitle.Text = "Dashboard";
            //
            // lblPageDescription
            //
            this.lblPageDescription.AutoSize = false;
            this.lblPageDescription.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblPageDescription.Location = new System.Drawing.Point(32, 60);
            this.lblPageDescription.Name = "lblPageDescription";
            this.lblPageDescription.Size = new System.Drawing.Size(1000, 22);
            this.lblPageDescription.Text = "Ticket metrics at a glance, the commands you use most, and what happened recently.";
            //
            // cardOpen  (metric cards: 250 × 110 at y = 100, 24 px apart → x = 32, 306, 580, 854)
            //
            this.cardOpen.BackColor = System.Drawing.Color.White;
            this.cardOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardOpen.Controls.Add(this.lblOpenValue);
            this.cardOpen.Controls.Add(this.lblOpenCaption);
            this.cardOpen.Location = new System.Drawing.Point(32, 100);
            this.cardOpen.Name = "cardOpen";
            this.cardOpen.Size = new System.Drawing.Size(250, 110);
            //
            // lblOpenValue
            //
            this.lblOpenValue.AutoSize = false;
            this.lblOpenValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblOpenValue.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblOpenValue.Location = new System.Drawing.Point(20, 14);
            this.lblOpenValue.Name = "lblOpenValue";
            this.lblOpenValue.Size = new System.Drawing.Size(210, 52);
            this.lblOpenValue.Text = "0";
            //
            // lblOpenCaption
            //
            this.lblOpenCaption.AutoSize = false;
            this.lblOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOpenCaption.Location = new System.Drawing.Point(20, 70);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Size = new System.Drawing.Size(210, 22);
            this.lblOpenCaption.Text = "Open tickets";
            //
            // cardInProgress
            //
            this.cardInProgress.BackColor = System.Drawing.Color.White;
            this.cardInProgress.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardInProgress.Controls.Add(this.lblInProgressValue);
            this.cardInProgress.Controls.Add(this.lblInProgressCaption);
            this.cardInProgress.Location = new System.Drawing.Point(306, 100);
            this.cardInProgress.Name = "cardInProgress";
            this.cardInProgress.Size = new System.Drawing.Size(250, 110);
            //
            // lblInProgressValue
            //
            this.lblInProgressValue.AutoSize = false;
            this.lblInProgressValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblInProgressValue.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblInProgressValue.Location = new System.Drawing.Point(20, 14);
            this.lblInProgressValue.Name = "lblInProgressValue";
            this.lblInProgressValue.Size = new System.Drawing.Size(210, 52);
            this.lblInProgressValue.Text = "0";
            //
            // lblInProgressCaption
            //
            this.lblInProgressCaption.AutoSize = false;
            this.lblInProgressCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblInProgressCaption.Location = new System.Drawing.Point(20, 70);
            this.lblInProgressCaption.Name = "lblInProgressCaption";
            this.lblInProgressCaption.Size = new System.Drawing.Size(210, 22);
            this.lblInProgressCaption.Text = "In progress";
            //
            // cardClosed
            //
            this.cardClosed.BackColor = System.Drawing.Color.White;
            this.cardClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardClosed.Controls.Add(this.lblClosedValue);
            this.cardClosed.Controls.Add(this.lblClosedCaption);
            this.cardClosed.Location = new System.Drawing.Point(580, 100);
            this.cardClosed.Name = "cardClosed";
            this.cardClosed.Size = new System.Drawing.Size(250, 110);
            //
            // lblClosedValue
            //
            this.lblClosedValue.AutoSize = false;
            this.lblClosedValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblClosedValue.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblClosedValue.Location = new System.Drawing.Point(20, 14);
            this.lblClosedValue.Name = "lblClosedValue";
            this.lblClosedValue.Size = new System.Drawing.Size(210, 52);
            this.lblClosedValue.Text = "0";
            //
            // lblClosedCaption
            //
            this.lblClosedCaption.AutoSize = false;
            this.lblClosedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblClosedCaption.Location = new System.Drawing.Point(20, 70);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Size = new System.Drawing.Size(210, 22);
            this.lblClosedCaption.Text = "Closed";
            //
            // cardCustomers
            //
            this.cardCustomers.BackColor = System.Drawing.Color.White;
            this.cardCustomers.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardCustomers.Controls.Add(this.lblCustomersValue);
            this.cardCustomers.Controls.Add(this.lblCustomersCaption);
            this.cardCustomers.Location = new System.Drawing.Point(854, 100);
            this.cardCustomers.Name = "cardCustomers";
            this.cardCustomers.Size = new System.Drawing.Size(250, 110);
            //
            // lblCustomersValue
            //
            this.lblCustomersValue.AutoSize = false;
            this.lblCustomersValue.Font = new System.Drawing.Font("default", 26F, System.Drawing.FontStyle.Bold);
            this.lblCustomersValue.Location = new System.Drawing.Point(20, 14);
            this.lblCustomersValue.Name = "lblCustomersValue";
            this.lblCustomersValue.Size = new System.Drawing.Size(210, 52);
            this.lblCustomersValue.Text = "0";
            //
            // lblCustomersCaption
            //
            this.lblCustomersCaption.AutoSize = false;
            this.lblCustomersCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCustomersCaption.Location = new System.Drawing.Point(20, 70);
            this.lblCustomersCaption.Name = "lblCustomersCaption";
            this.lblCustomersCaption.Size = new System.Drawing.Size(210, 22);
            this.lblCustomersCaption.Text = "Customers";
            //
            // cardCommands  (row 2 · 524 × 170 at 32,234 — two metric cards plus one gap wide)
            //
            this.cardCommands.BackColor = System.Drawing.Color.White;
            this.cardCommands.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardCommands.Controls.Add(this.lblCommandsTitle);
            this.cardCommands.Controls.Add(this.btnCreateTicket);
            this.cardCommands.Controls.Add(this.btnEditTicket);
            this.cardCommands.Controls.Add(this.btnCloseTicket);
            this.cardCommands.Controls.Add(this.lblCommandsHint);
            this.cardCommands.Location = new System.Drawing.Point(32, 234);
            this.cardCommands.Name = "cardCommands";
            this.cardCommands.Size = new System.Drawing.Size(524, 170);
            //
            // lblCommandsTitle
            //
            this.lblCommandsTitle.AutoSize = false;
            this.lblCommandsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCommandsTitle.Location = new System.Drawing.Point(20, 14);
            this.lblCommandsTitle.Name = "lblCommandsTitle";
            this.lblCommandsTitle.Size = new System.Drawing.Size(484, 28);
            this.lblCommandsTitle.Text = "Commands";
            //
            // btnCreateTicket  (grouped commands: 150 × 36, 12 px apart, near the content they affect)
            //
            this.btnCreateTicket.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnCreateTicket.Location = new System.Drawing.Point(20, 56);
            this.btnCreateTicket.Name = "btnCreateTicket";
            this.btnCreateTicket.Size = new System.Drawing.Size(150, 36);
            this.btnCreateTicket.Text = "Create ticket";
            this.btnCreateTicket.ToolTipText = "Opens the Module 5 TicketDialog; Save goes through ValidateForm and TicketService.AddTicket.";
            this.btnCreateTicket.Click += new System.EventHandler(this.btnCreateTicket_Click);
            //
            // btnEditTicket
            //
            this.btnEditTicket.Location = new System.Drawing.Point(182, 56);
            this.btnEditTicket.Name = "btnEditTicket";
            this.btnEditTicket.Size = new System.Drawing.Size(150, 36);
            this.btnEditTicket.Text = "Edit ticket";
            this.btnEditTicket.ToolTipText = "Goes to the ticket queue, where a row can be selected and edited.";
            this.btnEditTicket.Click += new System.EventHandler(this.btnEditTicket_Click);
            //
            // btnCloseTicket
            //
            this.btnCloseTicket.Location = new System.Drawing.Point(344, 56);
            this.btnCloseTicket.Name = "btnCloseTicket";
            this.btnCloseTicket.Size = new System.Drawing.Size(150, 36);
            this.btnCloseTicket.Text = "Close ticket";
            this.btnCloseTicket.ToolTipText = "Closes the oldest ticket that is not Closed yet, after a Yes/No confirmation.";
            this.btnCloseTicket.Click += new System.EventHandler(this.btnCloseTicket_Click);
            //
            // lblCommandsHint
            //
            this.lblCommandsHint.AutoSize = false;
            this.lblCommandsHint.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCommandsHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblCommandsHint.Location = new System.Drawing.Point(20, 104);
            this.lblCommandsHint.Name = "lblCommandsHint";
            this.lblCommandsHint.Size = new System.Drawing.Size(484, 50);
            this.lblCommandsHint.Text = "Same TicketService + TicketDialog as the Tickets page.\nCreate → dialog · Edit → queue · Close → YesNo confirm";
            this.lblCommandsHint.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // cardActivity  (row 2 · 524 wide at 580,234 · anchored so it grows with the window)
            //
            this.cardActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.cardActivity.BackColor = System.Drawing.Color.White;
            this.cardActivity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.cardActivity.Controls.Add(this.lblActivityTitle);
            this.cardActivity.Controls.Add(this.lstActivity);
            this.cardActivity.Controls.Add(this.btnClearActivity);
            this.cardActivity.Location = new System.Drawing.Point(580, 234);
            this.cardActivity.Name = "cardActivity";
            this.cardActivity.Size = new System.Drawing.Size(524, 362);
            //
            // lblActivityTitle
            //
            this.lblActivityTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblActivityTitle.AutoSize = false;
            this.lblActivityTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblActivityTitle.Location = new System.Drawing.Point(20, 14);
            this.lblActivityTitle.Name = "lblActivityTitle";
            this.lblActivityTitle.Size = new System.Drawing.Size(484, 28);
            this.lblActivityTitle.Text = "Recent activity  ·  navigation, theme changes, ticket actions";
            //
            // lstActivity  (the event log — fed only by MainWindow.AddActivity)
            //
            this.lstActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstActivity.Font = new System.Drawing.Font("monospace", 9F);
            this.lstActivity.Location = new System.Drawing.Point(20, 52);
            this.lstActivity.Name = "lstActivity";
            this.lstActivity.Size = new System.Drawing.Size(484, 254);
            //
            // btnClearActivity
            //
            this.btnClearActivity.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.btnClearActivity.Location = new System.Drawing.Point(394, 314);
            this.btnClearActivity.Name = "btnClearActivity";
            this.btnClearActivity.Size = new System.Drawing.Size(110, 32);
            this.btnClearActivity.Text = "Clear";
            this.btnClearActivity.Click += new System.EventHandler(this.btnClearActivity_Click);
            //
            // DashboardView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.lblPageTitle);
            this.Controls.Add(this.lblPageDescription);
            this.Controls.Add(this.cardOpen);
            this.Controls.Add(this.cardInProgress);
            this.Controls.Add(this.cardClosed);
            this.Controls.Add(this.cardCustomers);
            this.Controls.Add(this.cardCommands);
            this.Controls.Add(this.cardActivity);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(1148, 620);
            this.cardOpen.ResumeLayout(false);
            this.cardInProgress.ResumeLayout(false);
            this.cardClosed.ResumeLayout(false);
            this.cardCustomers.ResumeLayout(false);
            this.cardCommands.ResumeLayout(false);
            this.cardActivity.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblPageTitle;
        private Wisej.Web.Label lblPageDescription;
        private Wisej.Web.Panel cardOpen;
        private Wisej.Web.Label lblOpenValue;
        private Wisej.Web.Label lblOpenCaption;
        private Wisej.Web.Panel cardInProgress;
        private Wisej.Web.Label lblInProgressValue;
        private Wisej.Web.Label lblInProgressCaption;
        private Wisej.Web.Panel cardClosed;
        private Wisej.Web.Label lblClosedValue;
        private Wisej.Web.Label lblClosedCaption;
        private Wisej.Web.Panel cardCustomers;
        private Wisej.Web.Label lblCustomersValue;
        private Wisej.Web.Label lblCustomersCaption;
        private Wisej.Web.Panel cardCommands;
        private Wisej.Web.Label lblCommandsTitle;
        private Wisej.Web.Button btnCreateTicket;
        private Wisej.Web.Button btnEditTicket;
        private Wisej.Web.Button btnCloseTicket;
        private Wisej.Web.Label lblCommandsHint;
        private Wisej.Web.Panel cardActivity;
        private Wisej.Web.Label lblActivityTitle;
        private Wisej.Web.ListBox lstActivity;
        private Wisej.Web.Button btnClearActivity;
    }
}
