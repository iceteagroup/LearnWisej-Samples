namespace EnterpriseOps.UI
{
    partial class WorkQueuePage
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
            this.pnlHeader = new Wisej.Web.Panel();
            this.lblTitle = new Wisej.Web.Label();
            this.pnlQueue = new Wisej.Web.Panel();
            this.btnEscalate = new Wisej.Web.Button();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.lblCompensationTitle = new Wisej.Web.Label();
            this.lstCompensation = new Wisej.Web.ListBox();
            this.btnRetryNotification = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(860, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(560, 44);
            this.lblTitle.Text = "EnterpriseOps — Work queue";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlQueue
            //
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.btnEscalate);
            this.pnlQueue.Controls.Add(this.dgvWorkQueue);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Controls.Add(this.lblCompensationTitle);
            this.pnlQueue.Controls.Add(this.lstCompensation);
            this.pnlQueue.Controls.Add(this.btnRetryNotification);
            this.pnlQueue.Location = new System.Drawing.Point(24, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(812, 446);
            //
            // btnEscalate
            //
            this.btnEscalate.Location = new System.Drawing.Point(20, 14);
            this.btnEscalate.Name = "btnEscalate";
            this.btnEscalate.Size = new System.Drawing.Size(230, 36);
            this.btnEscalate.Text = "Escalate work order…";
            this.btnEscalate.Click += new System.EventHandler(this.btnEscalate_Click);
            //
            // dgvWorkQueue
            //
            this.dgvWorkQueue.AllowUserToAddRows = false;
            this.dgvWorkQueue.AutoGenerateColumns = false;
            this.dgvWorkQueue.BackColor = System.Drawing.Color.White;
            this.dgvWorkQueue.Columns.Add(this.colNumber);
            this.dgvWorkQueue.Columns.Add(this.colTitle);
            this.dgvWorkQueue.Columns.Add(this.colCustomer);
            this.dgvWorkQueue.Columns.Add(this.colStatus);
            this.dgvWorkQueue.Columns.Add(this.colPriority);
            this.dgvWorkQueue.Columns.Add(this.colVersion);
            this.dgvWorkQueue.Location = new System.Drawing.Point(20, 60);
            this.dgvWorkQueue.MultiSelect = false;
            this.dgvWorkQueue.Name = "dgvWorkQueue";
            this.dgvWorkQueue.ReadOnly = true;
            this.dgvWorkQueue.RowHeadersVisible = false;
            this.dgvWorkQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkQueue.Size = new System.Drawing.Size(772, 196);
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.HeaderText = "Work order";
            this.colNumber.Name = "colNumber";
            this.colNumber.Width = 100;
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 250;
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.Width = 160;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 100;
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.Width = 90;
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.HeaderText = "v";
            this.colVersion.Name = "colVersion";
            this.colVersion.Width = 50;
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(154, 42, 24);
            this.lblBanner.Location = new System.Drawing.Point(20, 264);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(772, 34);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblCompensationTitle
            //
            this.lblCompensationTitle.AutoSize = false;
            this.lblCompensationTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblCompensationTitle.Location = new System.Drawing.Point(20, 306);
            this.lblCompensationTitle.Name = "lblCompensationTitle";
            this.lblCompensationTitle.Size = new System.Drawing.Size(600, 26);
            this.lblCompensationTitle.Text = "Manual-review queue — open compensations: 0";
            //
            // lstCompensation
            //
            this.lstCompensation.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCompensation.Location = new System.Drawing.Point(20, 338);
            this.lstCompensation.Name = "lstCompensation";
            this.lstCompensation.Size = new System.Drawing.Size(596, 88);
            //
            // btnRetryNotification
            //
            this.btnRetryNotification.Location = new System.Drawing.Point(626, 338);
            this.btnRetryNotification.Name = "btnRetryNotification";
            this.btnRetryNotification.Size = new System.Drawing.Size(166, 36);
            this.btnRetryNotification.Text = "Retry notification";
            this.btnRetryNotification.Click += new System.EventHandler(this.btnRetryNotification_Click);
            //
            // WorkQueuePage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Name = "WorkQueuePage";
            this.Size = new System.Drawing.Size(860, 534);
            this.Text = "EnterpriseOps — Work queue";
            this.Load += new System.EventHandler(this.WorkQueuePage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.Button btnEscalate;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblCompensationTitle;
        private Wisej.Web.ListBox lstCompensation;
        private Wisej.Web.Button btnRetryNotification;
    }
}
