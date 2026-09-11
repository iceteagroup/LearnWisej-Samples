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
            this.txtSearch = new Wisej.Web.TextBox();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.cboAssigned = new Wisej.Web.ComboBox();
            this.cboSort = new Wisej.Web.ComboBox();
            this.btnSearch = new Wisej.Web.Button();
            this.cboSavedView = new Wisej.Web.ComboBox();
            this.btnApplyView = new Wisej.Web.Button();
            this.btnSaveView = new Wisej.Web.Button();
            this.lblViewBadge = new Wisej.Web.Label();
            this.dgvQueue = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssigned = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAge = new Wisej.Web.DataGridViewTextBoxColumn();
            this.btnFirst = new Wisej.Web.Button();
            this.btnPrev = new Wisej.Web.Button();
            this.lblPage = new Wisej.Web.Label();
            this.btnNext = new Wisej.Web.Button();
            this.btnLast = new Wisej.Web.Button();
            this.cboPageSize = new Wisej.Web.ComboBox();
            this.lblSelection = new Wisej.Web.Label();
            this.cboTechnician = new Wisej.Web.ComboBox();
            this.btnBatchReassign = new Wisej.Web.Button();
            this.btnSelectPage = new Wisej.Web.Button();
            this.btnClearSelection = new Wisej.Web.Button();
            this.lblProgress = new Wisej.Web.Label();
            this.progressBatch = new Wisej.Web.ProgressBar();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
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
            this.pnlHeader.Size = new System.Drawing.Size(928, 44);
            //
            // lblTitle
            //
            this.lblTitle.AutoSize = false;
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(24, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(460, 44);
            this.lblTitle.Text = "EnterpriseOps — Enterprise Work Queue";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlQueue
            //
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.txtSearch);
            this.pnlQueue.Controls.Add(this.cboStatus);
            this.pnlQueue.Controls.Add(this.cboAssigned);
            this.pnlQueue.Controls.Add(this.cboSort);
            this.pnlQueue.Controls.Add(this.btnSearch);
            this.pnlQueue.Controls.Add(this.cboSavedView);
            this.pnlQueue.Controls.Add(this.btnApplyView);
            this.pnlQueue.Controls.Add(this.btnSaveView);
            this.pnlQueue.Controls.Add(this.lblViewBadge);
            this.pnlQueue.Controls.Add(this.dgvQueue);
            this.pnlQueue.Controls.Add(this.btnFirst);
            this.pnlQueue.Controls.Add(this.btnPrev);
            this.pnlQueue.Controls.Add(this.lblPage);
            this.pnlQueue.Controls.Add(this.btnNext);
            this.pnlQueue.Controls.Add(this.btnLast);
            this.pnlQueue.Controls.Add(this.cboPageSize);
            this.pnlQueue.Controls.Add(this.lblSelection);
            this.pnlQueue.Controls.Add(this.cboTechnician);
            this.pnlQueue.Controls.Add(this.btnBatchReassign);
            this.pnlQueue.Controls.Add(this.btnSelectPage);
            this.pnlQueue.Controls.Add(this.btnClearSelection);
            this.pnlQueue.Controls.Add(this.lblProgress);
            this.pnlQueue.Controls.Add(this.progressBatch);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Controls.Add(this.lblStatusBar);
            this.pnlQueue.Location = new System.Drawing.Point(24, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(880, 546);
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(20, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 32);
            this.txtSearch.Watermark = "search title, customer, site…";
            this.txtSearch.KeyDown += new Wisej.Web.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(228, 14);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(120, 32);
            //
            // cboAssigned
            //
            this.cboAssigned.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboAssigned.Location = new System.Drawing.Point(356, 14);
            this.cboAssigned.Name = "cboAssigned";
            this.cboAssigned.Size = new System.Drawing.Size(140, 32);
            //
            // cboSort
            //
            this.cboSort.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboSort.Location = new System.Drawing.Point(504, 14);
            this.cboSort.Name = "cboSort";
            this.cboSort.Size = new System.Drawing.Size(160, 32);
            //
            // btnSearch
            //
            this.btnSearch.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(672, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(188, 32);
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // cboSavedView
            //
            this.cboSavedView.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboSavedView.Location = new System.Drawing.Point(20, 54);
            this.cboSavedView.Name = "cboSavedView";
            this.cboSavedView.Size = new System.Drawing.Size(200, 30);
            //
            // btnApplyView
            //
            this.btnApplyView.Location = new System.Drawing.Point(228, 54);
            this.btnApplyView.Name = "btnApplyView";
            this.btnApplyView.Size = new System.Drawing.Size(110, 30);
            this.btnApplyView.Text = "Apply view";
            this.btnApplyView.Click += new System.EventHandler(this.btnApplyView_Click);
            //
            // btnSaveView
            //
            this.btnSaveView.Location = new System.Drawing.Point(346, 54);
            this.btnSaveView.Name = "btnSaveView";
            this.btnSaveView.Size = new System.Drawing.Size(180, 30);
            this.btnSaveView.Text = "★ Save current as view";
            this.btnSaveView.Click += new System.EventHandler(this.btnSaveView_Click);
            //
            // lblViewBadge
            //
            this.lblViewBadge.AutoSize = false;
            this.lblViewBadge.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblViewBadge.ForeColor = System.Drawing.Color.FromArgb(11, 106, 230);
            this.lblViewBadge.Location = new System.Drawing.Point(534, 54);
            this.lblViewBadge.Name = "lblViewBadge";
            this.lblViewBadge.Size = new System.Drawing.Size(326, 30);
            this.lblViewBadge.Text = "★ Saved view";
            this.lblViewBadge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvQueue
            //
            this.dgvQueue.AllowUserToAddRows = false;
            this.dgvQueue.AllowUserToDeleteRows = false;
            this.dgvQueue.AutoGenerateColumns = false;
            this.dgvQueue.AutoSelectFirstRow = false;
            this.dgvQueue.AutoSizeColumnsMode = Wisej.Web.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvQueue.BackColor = System.Drawing.Color.White;
            this.dgvQueue.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
            this.colNumber,
            this.colTitle,
            this.colStatus,
            this.colPriority,
            this.colAssigned,
            this.colDue,
            this.colAge});
            this.dgvQueue.Location = new System.Drawing.Point(20, 92);
            this.dgvQueue.MultiSelect = true;
            this.dgvQueue.Name = "dgvQueue";
            this.dgvQueue.NoDataMessage = "No work order matches this query.";
            this.dgvQueue.ReadOnly = true;
            this.dgvQueue.RowHeadersVisible = false;
            this.dgvQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQueue.Size = new System.Drawing.Size(840, 280);
            this.dgvQueue.ColumnHeaderMouseClick += new Wisej.Web.DataGridViewCellMouseEventHandler(this.dgvQueue_ColumnHeaderMouseClick);
            this.dgvQueue.SelectionChanged += new System.EventHandler(this.dgvQueue_SelectionChanged);
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.FillWeight = 12F;
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 37F;
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.ReadOnly = true;
            this.colTitle.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "StatusText";
            this.colStatus.FillWeight = 11F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colPriority
            //
            this.colPriority.DataPropertyName = "PriorityText";
            this.colPriority.FillWeight = 10F;
            this.colPriority.HeaderText = "Priority ▼";
            this.colPriority.Name = "colPriority";
            this.colPriority.ReadOnly = true;
            this.colPriority.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colAssigned
            //
            this.colAssigned.DataPropertyName = "AssignedTo";
            this.colAssigned.FillWeight = 12F;
            this.colAssigned.HeaderText = "Assigned";
            this.colAssigned.Name = "colAssigned";
            this.colAssigned.ReadOnly = true;
            this.colAssigned.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colDue
            //
            this.colDue.DataPropertyName = "DueText";
            this.colDue.FillWeight = 9F;
            this.colDue.HeaderText = "Due";
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            this.colDue.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colAge
            //
            this.colAge.DataPropertyName = "AgeDays";
            this.colAge.FillWeight = 9F;
            this.colAge.HeaderText = "Age (d)";
            this.colAge.Name = "colAge";
            this.colAge.ReadOnly = true;
            this.colAge.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // btnFirst
            //
            this.btnFirst.Location = new System.Drawing.Point(20, 380);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(38, 28);
            this.btnFirst.Text = "⏮";
            this.btnFirst.ToolTipText = "First page";
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            //
            // btnPrev
            //
            this.btnPrev.Location = new System.Drawing.Point(62, 380);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(38, 28);
            this.btnPrev.Text = "◀";
            this.btnPrev.ToolTipText = "Previous page";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            //
            // lblPage
            //
            this.lblPage.AutoSize = false;
            this.lblPage.Font = new System.Drawing.Font("default", 10F);
            this.lblPage.Location = new System.Drawing.Point(104, 380);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(170, 28);
            this.lblPage.Text = "Page — of —";
            this.lblPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnNext
            //
            this.btnNext.Location = new System.Drawing.Point(278, 380);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(38, 28);
            this.btnNext.Text = "▶";
            this.btnNext.ToolTipText = "Next page";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            //
            // btnLast
            //
            this.btnLast.Location = new System.Drawing.Point(320, 380);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(38, 28);
            this.btnLast.Text = "⏭";
            this.btnLast.ToolTipText = "Last page";
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            //
            // cboPageSize
            //
            this.cboPageSize.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPageSize.Location = new System.Drawing.Point(364, 380);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(100, 28);
            //
            // lblSelection
            //
            this.lblSelection.AutoSize = false;
            this.lblSelection.Font = new System.Drawing.Font("default", 9F);
            this.lblSelection.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSelection.Location = new System.Drawing.Point(474, 380);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(386, 28);
            this.lblSelection.Text = "0 selected";
            this.lblSelection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTechnician
            //
            this.cboTechnician.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTechnician.Location = new System.Drawing.Point(20, 416);
            this.cboTechnician.Name = "cboTechnician";
            this.cboTechnician.Size = new System.Drawing.Size(150, 32);
            //
            // btnBatchReassign
            //
            this.btnBatchReassign.Enabled = false;
            this.btnBatchReassign.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnBatchReassign.Location = new System.Drawing.Point(178, 416);
            this.btnBatchReassign.Name = "btnBatchReassign";
            this.btnBatchReassign.Size = new System.Drawing.Size(210, 32);
            this.btnBatchReassign.Text = "Reassign selected…";
            this.btnBatchReassign.Click += new System.EventHandler(this.btnBatchReassign_Click);
            //
            // btnSelectPage
            //
            this.btnSelectPage.Location = new System.Drawing.Point(396, 416);
            this.btnSelectPage.Name = "btnSelectPage";
            this.btnSelectPage.Size = new System.Drawing.Size(130, 32);
            this.btnSelectPage.Text = "Select page";
            this.btnSelectPage.Click += new System.EventHandler(this.btnSelectPage_Click);
            //
            // btnClearSelection
            //
            this.btnClearSelection.Location = new System.Drawing.Point(534, 416);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(140, 32);
            this.btnClearSelection.Text = "Clear selection";
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            //
            // lblProgress
            //
            this.lblProgress.AutoSize = false;
            this.lblProgress.Font = new System.Drawing.Font("monospace", 9F);
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblProgress.Location = new System.Drawing.Point(20, 456);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(840, 20);
            this.lblProgress.Text = "";
            this.lblProgress.Visible = false;
            //
            // progressBatch
            //
            this.progressBatch.Location = new System.Drawing.Point(20, 480);
            this.progressBatch.Maximum = 100;
            this.progressBatch.Name = "progressBatch";
            this.progressBatch.Size = new System.Drawing.Size(840, 12);
            this.progressBatch.Visible = false;
            //
            // lblBanner
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 456);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(840, 36);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 500);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(840, 32);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // WorkQueuePage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Name = "WorkQueuePage";
            this.Size = new System.Drawing.Size(928, 634);
            this.Text = "EnterpriseOps — Enterprise Work Queue";
            this.Load += new System.EventHandler(this.WorkQueuePage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.ComboBox cboAssigned;
        private Wisej.Web.ComboBox cboSort;
        private Wisej.Web.Button btnSearch;
        private Wisej.Web.ComboBox cboSavedView;
        private Wisej.Web.Button btnApplyView;
        private Wisej.Web.Button btnSaveView;
        private Wisej.Web.Label lblViewBadge;
        private Wisej.Web.DataGridView dgvQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssigned;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.DataGridViewTextBoxColumn colAge;
        private Wisej.Web.Button btnFirst;
        private Wisej.Web.Button btnPrev;
        private Wisej.Web.Label lblPage;
        private Wisej.Web.Button btnNext;
        private Wisej.Web.Button btnLast;
        private Wisej.Web.ComboBox cboPageSize;
        private Wisej.Web.Label lblSelection;
        private Wisej.Web.ComboBox cboTechnician;
        private Wisej.Web.Button btnBatchReassign;
        private Wisej.Web.Button btnSelectPage;
        private Wisej.Web.Button btnClearSelection;
        private Wisej.Web.Label lblProgress;
        private Wisej.Web.ProgressBar progressBatch;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
    }
}
