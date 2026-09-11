namespace EnterpriseOps.UI
{
    partial class ApprovalsPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. The session's SQLite connection is owned by this screen,
        /// so it is released here (see <see cref="ApprovalsPage.DisposeSessionResources"/>).
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                DisposeSessionResources();
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
            this.lblQueueTitle = new Wisej.Web.Label();
            this.txtSearch = new Wisej.Web.TextBox();
            this.btnSearch = new Wisej.Web.Button();
            this.btnAudit = new Wisej.Web.Button();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.pnlApprove = new Wisej.Web.Panel();
            this.lblWorkOrder = new Wisej.Web.Label();
            this.lblVersion = new Wisej.Web.Label();
            this.lblCommentCaption = new Wisej.Web.Label();
            this.txtComment = new Wisej.Web.TextBox();
            this.btnApprove = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.lblResult = new Wisej.Web.Label();
            this.lblResultDetail = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlApprove.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(940, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 44);
            this.lblTitle.Text = "EnterpriseOps — Approvals";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlQueue
            //
            this.pnlQueue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.lblQueueTitle);
            this.pnlQueue.Controls.Add(this.txtSearch);
            this.pnlQueue.Controls.Add(this.btnSearch);
            this.pnlQueue.Controls.Add(this.btnAudit);
            this.pnlQueue.Controls.Add(this.dgvWorkQueue);
            this.pnlQueue.Location = new System.Drawing.Point(24, 56);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(500, 400);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(16, 12);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(468, 28);
            this.lblQueueTitle.Text = "Work queue";
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(16, 46);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(236, 30);
            this.txtSearch.Watermark = "Search number, title, customer, site";
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(260, 46);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(108, 30);
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnAudit
            //
            this.btnAudit.Location = new System.Drawing.Point(376, 46);
            this.btnAudit.Name = "btnAudit";
            this.btnAudit.Size = new System.Drawing.Size(108, 30);
            this.btnAudit.Text = "Audit log";
            this.btnAudit.Click += new System.EventHandler(this.btnAudit_Click);
            //
            // dgvWorkQueue
            //
            this.dgvWorkQueue.AllowUserToAddRows = false;
            this.dgvWorkQueue.AllowUserToDeleteRows = false;
            this.dgvWorkQueue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.dgvWorkQueue.AutoGenerateColumns = false;
            this.dgvWorkQueue.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWorkQueue.BackColor = System.Drawing.Color.White;
            this.dgvWorkQueue.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colNumber,
            this.colTitle,
            this.colStatus,
            this.colPriority,
            this.colVersion});
            this.dgvWorkQueue.Location = new System.Drawing.Point(16, 86);
            this.dgvWorkQueue.MultiSelect = false;
            this.dgvWorkQueue.Name = "dgvWorkQueue";
            this.dgvWorkQueue.ReadOnly = true;
            this.dgvWorkQueue.RowHeadersVisible = false;
            this.dgvWorkQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWorkQueue.Size = new System.Drawing.Size(468, 298);
            this.dgvWorkQueue.SelectionChanged += new System.EventHandler(this.dgvWorkQueue_SelectionChanged);
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.FillWeight = 20F;
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 42F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 19F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "Priority";
            this.colPriority.FillWeight = 15F;
            this.colPriority.HeaderText = "Priority";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            //
            // colVersion
            //
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.FillWeight = 10F;
            this.colVersion.HeaderText = "v";
            this.colVersion.Name = "colVersion";
            this.colVersion.ReadOnly = true;
            //
            // pnlApprove
            //
            this.pnlApprove.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlApprove.BackColor = System.Drawing.Color.White;
            this.pnlApprove.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlApprove.Controls.Add(this.lblWorkOrder);
            this.pnlApprove.Controls.Add(this.lblVersion);
            this.pnlApprove.Controls.Add(this.lblCommentCaption);
            this.pnlApprove.Controls.Add(this.txtComment);
            this.pnlApprove.Controls.Add(this.btnApprove);
            this.pnlApprove.Controls.Add(this.btnCancel);
            this.pnlApprove.Controls.Add(this.lblResult);
            this.pnlApprove.Controls.Add(this.lblResultDetail);
            this.pnlApprove.Location = new System.Drawing.Point(536, 56);
            this.pnlApprove.Name = "pnlApprove";
            this.pnlApprove.Size = new System.Drawing.Size(380, 400);
            //
            // lblWorkOrder
            //
            this.lblWorkOrder.AutoSize = false;
            this.lblWorkOrder.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblWorkOrder.Location = new System.Drawing.Point(16, 12);
            this.lblWorkOrder.Name = "lblWorkOrder";
            this.lblWorkOrder.Size = new System.Drawing.Size(348, 48);
            this.lblWorkOrder.Text = "Approve work order";
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = false;
            this.lblVersion.Font = new System.Drawing.Font("monospace", 9F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVersion.Location = new System.Drawing.Point(16, 60);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(348, 22);
            this.lblVersion.Text = "select a work order";
            //
            // lblCommentCaption
            //
            this.lblCommentCaption.AutoSize = false;
            this.lblCommentCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCommentCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblCommentCaption.Location = new System.Drawing.Point(16, 90);
            this.lblCommentCaption.Name = "lblCommentCaption";
            this.lblCommentCaption.Size = new System.Drawing.Size(348, 20);
            this.lblCommentCaption.Text = "APPROVAL COMMENT";
            //
            // txtComment
            //
            this.txtComment.Location = new System.Drawing.Point(16, 110);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(348, 72);
            this.txtComment.Text = "Parts verified and installed; pressure test passed.";
            this.txtComment.Watermark = "Why is this work order being approved?";
            //
            // btnApprove
            //
            this.btnApprove.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnApprove.Location = new System.Drawing.Point(16, 194);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(130, 40);
            this.btnApprove.Text = "Approve";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(154, 194);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblResult
            //
            this.lblResult.AutoSize = false;
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(240, 249, 243);
            this.lblResult.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblResult.ForeColor = System.Drawing.Color.FromArgb(21, 95, 51);
            this.lblResult.Location = new System.Drawing.Point(16, 246);
            this.lblResult.Name = "lblResult";
            this.lblResult.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblResult.Size = new System.Drawing.Size(348, 64);
            this.lblResult.Text = "";
            this.lblResult.Visible = false;
            //
            // lblResultDetail
            //
            this.lblResultDetail.AutoSize = false;
            this.lblResultDetail.Font = new System.Drawing.Font("monospace", 8F);
            this.lblResultDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblResultDetail.Location = new System.Drawing.Point(16, 312);
            this.lblResultDetail.Name = "lblResultDetail";
            this.lblResultDetail.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblResultDetail.Size = new System.Drawing.Size(348, 46);
            this.lblResultDetail.Text = "";
            this.lblResultDetail.Visible = false;
            //
            // lblStatusBar
            //
            this.lblStatusBar.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(24, 468);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(892, 36);
            this.lblStatusBar.Text = "Ready";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ApprovalsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Controls.Add(this.pnlApprove);
            this.Controls.Add(this.lblStatusBar);
            this.Name = "ApprovalsPage";
            this.Size = new System.Drawing.Size(940, 528);
            this.Text = "EnterpriseOps — Approvals";
            this.Load += new System.EventHandler(this.ApprovalsPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlApprove.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.Button btnSearch;
        private Wisej.Web.Button btnAudit;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Panel pnlApprove;
        private Wisej.Web.Label lblWorkOrder;
        private Wisej.Web.Label lblVersion;
        private Wisej.Web.Label lblCommentCaption;
        private Wisej.Web.TextBox txtComment;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblResult;
        private Wisej.Web.Label lblResultDetail;
        private Wisej.Web.Label lblStatusBar;
    }
}
