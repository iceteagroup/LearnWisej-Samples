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
            this.pnlActivity = new Wisej.Web.Panel();
            this.labelActivityCard = new Wisej.Web.Label();
            this.btnRefreshSummary = new Wisej.Web.Button();
            this.btnOpenTickets = new Wisej.Web.Button();
            this.btnClearActivity = new Wisej.Web.Button();
            this.lstActivity = new Wisej.Web.ListBox();
            this.lblStatus = new Wisej.Web.Label();
            this.pnlTotal.SuspendLayout();
            this.pnlOpen.SuspendLayout();
            this.pnlInProgress.SuspendLayout();
            this.pnlClosed.SuspendLayout();
            this.pnlActivity.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTotal  (summary card 1 of 4 — 246×120, 16 px apart)
            //
            this.pnlTotal.BackColor = System.Drawing.Color.White;
            this.pnlTotal.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTotal.Controls.Add(this.lblTotalCaption);
            this.pnlTotal.Controls.Add(this.lblTotal);
            this.pnlTotal.Location = new System.Drawing.Point(0, 0);
            this.pnlTotal.Name = "pnlTotal";
            this.pnlTotal.Size = new System.Drawing.Size(246, 120);
            //
            // lblTotalCaption
            //
            this.lblTotalCaption.AutoSize = false;
            this.lblTotalCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblTotalCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTotalCaption.Location = new System.Drawing.Point(24, 16);
            this.lblTotalCaption.Name = "lblTotalCaption";
            this.lblTotalCaption.Size = new System.Drawing.Size(198, 24);
            this.lblTotalCaption.Text = "Total tickets";
            //
            // lblTotal
            //
            this.lblTotal.AutoSize = false;
            this.lblTotal.Font = new System.Drawing.Font("default", 30F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(24, 44);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(198, 56);
            this.lblTotal.Text = "0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlOpen
            //
            this.pnlOpen.BackColor = System.Drawing.Color.White;
            this.pnlOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlOpen.Controls.Add(this.lblOpenCaption);
            this.pnlOpen.Controls.Add(this.lblOpen);
            this.pnlOpen.Location = new System.Drawing.Point(262, 0);
            this.pnlOpen.Name = "pnlOpen";
            this.pnlOpen.Size = new System.Drawing.Size(246, 120);
            //
            // lblOpenCaption
            //
            this.lblOpenCaption.AutoSize = false;
            this.lblOpenCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblOpenCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblOpenCaption.Location = new System.Drawing.Point(24, 16);
            this.lblOpenCaption.Name = "lblOpenCaption";
            this.lblOpenCaption.Size = new System.Drawing.Size(198, 24);
            this.lblOpenCaption.Text = "Open";
            //
            // lblOpen
            //
            this.lblOpen.AutoSize = false;
            this.lblOpen.Font = new System.Drawing.Font("default", 30F, System.Drawing.FontStyle.Bold);
            this.lblOpen.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblOpen.Location = new System.Drawing.Point(24, 44);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Size = new System.Drawing.Size(198, 56);
            this.lblOpen.Text = "0";
            this.lblOpen.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlInProgress
            //
            this.pnlInProgress.BackColor = System.Drawing.Color.White;
            this.pnlInProgress.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlInProgress.Controls.Add(this.lblInProgressCaption);
            this.pnlInProgress.Controls.Add(this.lblInProgress);
            this.pnlInProgress.Location = new System.Drawing.Point(524, 0);
            this.pnlInProgress.Name = "pnlInProgress";
            this.pnlInProgress.Size = new System.Drawing.Size(246, 120);
            //
            // lblInProgressCaption
            //
            this.lblInProgressCaption.AutoSize = false;
            this.lblInProgressCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblInProgressCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblInProgressCaption.Location = new System.Drawing.Point(24, 16);
            this.lblInProgressCaption.Name = "lblInProgressCaption";
            this.lblInProgressCaption.Size = new System.Drawing.Size(198, 24);
            this.lblInProgressCaption.Text = "In Progress";
            //
            // lblInProgress
            //
            this.lblInProgress.AutoSize = false;
            this.lblInProgress.Font = new System.Drawing.Font("default", 30F, System.Drawing.FontStyle.Bold);
            this.lblInProgress.ForeColor = System.Drawing.Color.FromArgb(232, 161, 60);
            this.lblInProgress.Location = new System.Drawing.Point(24, 44);
            this.lblInProgress.Name = "lblInProgress";
            this.lblInProgress.Size = new System.Drawing.Size(198, 56);
            this.lblInProgress.Text = "0";
            this.lblInProgress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlClosed
            //
            this.pnlClosed.BackColor = System.Drawing.Color.White;
            this.pnlClosed.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlClosed.Controls.Add(this.lblClosedCaption);
            this.pnlClosed.Controls.Add(this.lblClosed);
            this.pnlClosed.Location = new System.Drawing.Point(786, 0);
            this.pnlClosed.Name = "pnlClosed";
            this.pnlClosed.Size = new System.Drawing.Size(246, 120);
            //
            // lblClosedCaption
            //
            this.lblClosedCaption.AutoSize = false;
            this.lblClosedCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblClosedCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblClosedCaption.Location = new System.Drawing.Point(24, 16);
            this.lblClosedCaption.Name = "lblClosedCaption";
            this.lblClosedCaption.Size = new System.Drawing.Size(198, 24);
            this.lblClosedCaption.Text = "Closed";
            //
            // lblClosed
            //
            this.lblClosed.AutoSize = false;
            this.lblClosed.Font = new System.Drawing.Font("default", 30F, System.Drawing.FontStyle.Bold);
            this.lblClosed.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblClosed.Location = new System.Drawing.Point(24, 44);
            this.lblClosed.Name = "lblClosed";
            this.lblClosed.Size = new System.Drawing.Size(198, 56);
            this.lblClosed.Text = "0";
            this.lblClosed.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlActivity  (Recent activity · what the app did — the course's event-log card)
            //
            this.pnlActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActivity.BackColor = System.Drawing.Color.White;
            this.pnlActivity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlActivity.Controls.Add(this.labelActivityCard);
            this.pnlActivity.Controls.Add(this.btnRefreshSummary);
            this.pnlActivity.Controls.Add(this.btnOpenTickets);
            this.pnlActivity.Controls.Add(this.btnClearActivity);
            this.pnlActivity.Controls.Add(this.lstActivity);
            this.pnlActivity.Controls.Add(this.lblStatus);
            this.pnlActivity.Location = new System.Drawing.Point(0, 136);
            this.pnlActivity.Name = "pnlActivity";
            this.pnlActivity.Size = new System.Drawing.Size(1032, 396);
            //
            // labelActivityCard
            //
            this.labelActivityCard.AutoSize = false;
            this.labelActivityCard.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelActivityCard.Location = new System.Drawing.Point(24, 14);
            this.labelActivityCard.Name = "labelActivityCard";
            this.labelActivityCard.Size = new System.Drawing.Size(480, 28);
            this.labelActivityCard.Text = "Recent activity  ·  every navigation and action, logged through AddActivity()";
            //
            // btnRefreshSummary
            //
            this.btnRefreshSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnRefreshSummary.Location = new System.Drawing.Point(624, 12);
            this.btnRefreshSummary.Name = "btnRefreshSummary";
            this.btnRefreshSummary.Size = new System.Drawing.Size(140, 32);
            this.btnRefreshSummary.Text = "Refresh summary";
            this.btnRefreshSummary.ToolTipText = "TicketService.GetSummary() → the four cards above.";
            this.btnRefreshSummary.Click += new System.EventHandler(this.btnRefreshSummary_Click);
            //
            // btnOpenTickets
            //
            this.btnOpenTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnOpenTickets.Location = new System.Drawing.Point(772, 12);
            this.btnOpenTickets.Name = "btnOpenTickets";
            this.btnOpenTickets.Size = new System.Drawing.Size(120, 32);
            this.btnOpenTickets.Text = "Open tickets →";
            this.btnOpenTickets.ToolTipText = "Shell.NavigateTo(\"Tickets\") — the same call the nav button makes.";
            this.btnOpenTickets.Click += new System.EventHandler(this.btnOpenTickets_Click);
            //
            // btnClearActivity
            //
            this.btnClearActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearActivity.Location = new System.Drawing.Point(900, 12);
            this.btnClearActivity.Name = "btnClearActivity";
            this.btnClearActivity.Size = new System.Drawing.Size(108, 32);
            this.btnClearActivity.Text = "Clear";
            this.btnClearActivity.Click += new System.EventHandler(this.btnClearActivity_Click);
            //
            // lstActivity
            //
            this.lstActivity.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstActivity.Font = new System.Drawing.Font("monospace", 9F);
            this.lstActivity.Location = new System.Drawing.Point(24, 56);
            this.lstActivity.Name = "lstActivity";
            this.lstActivity.Size = new System.Drawing.Size(984, 292);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(24, 358);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(984, 26);
            this.lblStatus.Text = "● ready";
            //
            // DashboardView
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlTotal);
            this.Controls.Add(this.pnlOpen);
            this.Controls.Add(this.pnlInProgress);
            this.Controls.Add(this.pnlClosed);
            this.Controls.Add(this.pnlActivity);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(1032, 532);
            this.pnlTotal.ResumeLayout(false);
            this.pnlOpen.ResumeLayout(false);
            this.pnlInProgress.ResumeLayout(false);
            this.pnlClosed.ResumeLayout(false);
            this.pnlActivity.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

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
        private Wisej.Web.Panel pnlActivity;
        private Wisej.Web.Label labelActivityCard;
        private Wisej.Web.Button btnRefreshSummary;
        private Wisej.Web.Button btnOpenTickets;
        private Wisej.Web.Button btnClearActivity;
        private Wisej.Web.ListBox lstActivity;
        private Wisej.Web.Label lblStatus;
    }
}
