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
            if (disposing)
            {
                DetachTrace();
                if (components != null)
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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlQueue = new Wisej.Web.Panel();
            this.txtSearch = new Wisej.Web.TextBox();
            this.cboStatus = new Wisej.Web.ComboBox();
            this.cboAssigned = new Wisej.Web.ComboBox();
            this.cboSort = new Wisej.Web.ComboBox();
            this.btnSearch = new Wisej.Web.Button();
            this.cboSavedView = new Wisej.Web.ComboBox();
            this.btnApplyView = new Wisej.Web.Button();
            this.btnSaveView = new Wisej.Web.Button();
            this.btnSimulateRefresh = new Wisej.Web.Button();
            this.lblViewBadge = new Wisej.Web.Label();
            this.dgvQueue = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAssigned = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAge = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
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
            this.lblStatus = new Wisej.Web.Label();
            this.lblProgress = new Wisej.Web.Label();
            this.progressBatch = new Wisej.Web.ProgressBar();
            this.lblBanner = new Wisej.Web.Label();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnLoadEverything = new Wisej.Web.Button();
            this.btnApprovalLock = new Wisej.Web.Button();
            this.btnConcurrentEdit = new Wisej.Web.Button();
            this.btnRetryFailed = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · user · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblCorrelation);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1348, 44);
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
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(700, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(160, 44);
            this.lblTenant.Text = "tenant: contoso";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1160, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(164, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlQueue  (the designable work queue: filter bar, saved views, grid, pager, batch bar, progress, banner, footer)
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
            this.pnlQueue.Controls.Add(this.btnSimulateRefresh);
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
            this.pnlQueue.Controls.Add(this.lblStatus);
            this.pnlQueue.Controls.Add(this.lblProgress);
            this.pnlQueue.Controls.Add(this.progressBatch);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Controls.Add(this.lblStatusBar);
            this.pnlQueue.Location = new System.Drawing.Point(24, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(880, 540);
            //
            // txtSearch  (server-side search: title, customer, site or number)
            //
            this.txtSearch.Location = new System.Drawing.Point(20, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 32);
            this.txtSearch.ToolTipText = "Free text. The service turns it into a WHERE clause — the browser never filters.";
            this.txtSearch.Watermark = "search title, customer, site…";
            this.txtSearch.KeyDown += new Wisej.Web.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(228, 14);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(120, 32);
            this.cboStatus.ToolTipText = "Status filter — part of WorkQueueQuery, evaluated on the server.";
            //
            // cboAssigned
            //
            this.cboAssigned.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboAssigned.Location = new System.Drawing.Point(356, 14);
            this.cboAssigned.Name = "cboAssigned";
            this.cboAssigned.Size = new System.Drawing.Size(140, 32);
            this.cboAssigned.ToolTipText = "Assigned-to filter — part of WorkQueueQuery.";
            //
            // cboSort
            //
            this.cboSort.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboSort.Location = new System.Drawing.Point(504, 14);
            this.cboSort.Name = "cboSort";
            this.cboSort.Size = new System.Drawing.Size(160, 32);
            this.cboSort.ToolTipText = "Sort column. Clicking a grid header does the same thing — the sort lives in GridState, not in the grid.";
            //
            // btnSearch  (the walkthrough's Search: one service call, page 1)
            //
            this.btnSearch.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Location = new System.Drawing.Point(672, 14);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(188, 32);
            this.btnSearch.Text = "Search (page 1)";
            this.btnSearch.ToolTipText = "btnSearch_Click → await _queryService.SearchAsync(query) → ShowPage(result). One page of rows comes back.";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // cboSavedView  (a saved view is a stored query definition, not a cached result)
            //
            this.cboSavedView.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboSavedView.Location = new System.Drawing.Point(20, 54);
            this.cboSavedView.Name = "cboSavedView";
            this.cboSavedView.Size = new System.Drawing.Size(200, 30);
            this.cboSavedView.ToolTipText = "Saved views for this tenant and user. Applying one re-runs the query, so the rows are always current.";
            //
            // btnApplyView
            //
            this.btnApplyView.Location = new System.Drawing.Point(228, 54);
            this.btnApplyView.Name = "btnApplyView";
            this.btnApplyView.Size = new System.Drawing.Size(110, 30);
            this.btnApplyView.Text = "Apply view";
            this.btnApplyView.ToolTipText = "Loads the stored WorkQueueQuery into the grid state and searches again.";
            this.btnApplyView.Click += new System.EventHandler(this.btnApplyView_Click);
            //
            // btnSaveView
            //
            this.btnSaveView.Location = new System.Drawing.Point(346, 54);
            this.btnSaveView.Name = "btnSaveView";
            this.btnSaveView.Size = new System.Drawing.Size(180, 30);
            this.btnSaveView.Text = "★ Save current as view";
            this.btnSaveView.ToolTipText = "Serializes the current filters + sort as JSON and stores them under a name.";
            this.btnSaveView.Click += new System.EventHandler(this.btnSaveView_Click);
            //
            // btnSimulateRefresh  (review question 3: can the user repeat the search after refresh?)
            //
            this.btnSimulateRefresh.Location = new System.Drawing.Point(534, 54);
            this.btnSimulateRefresh.Name = "btnSimulateRefresh";
            this.btnSimulateRefresh.Size = new System.Drawing.Size(200, 30);
            this.btnSimulateRefresh.Text = "⟳ Simulate refresh (F5)";
            this.btnSimulateRefresh.ToolTipText = "Throws this page away and builds a new one. The grid state lives in SessionContext, so the same page, sort and filters come back.";
            this.btnSimulateRefresh.Click += new System.EventHandler(this.btnSimulateRefresh_Click);
            //
            // lblViewBadge
            //
            this.lblViewBadge.AutoSize = false;
            this.lblViewBadge.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblViewBadge.ForeColor = System.Drawing.Color.FromArgb(11, 106, 230);
            this.lblViewBadge.Location = new System.Drawing.Point(742, 54);
            this.lblViewBadge.Name = "lblViewBadge";
            this.lblViewBadge.Size = new System.Drawing.Size(118, 30);
            this.lblViewBadge.Text = "★ saved view";
            this.lblViewBadge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvQueue  (bound to one page of WorkQueueRow — the projection, never the entity)
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
            this.colAge,
            this.colVersion});
            this.dgvQueue.Location = new System.Drawing.Point(20, 92);
            this.dgvQueue.MultiSelect = true;
            this.dgvQueue.Name = "dgvQueue";
            this.dgvQueue.NoDataMessage = "No work order matches this query.";
            this.dgvQueue.ReadOnly = true;
            this.dgvQueue.RowHeadersVisible = false;
            this.dgvQueue.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQueue.Size = new System.Drawing.Size(840, 236);
            this.dgvQueue.ToolTipText = "Click to select, Ctrl+click / Shift+click to add rows. The selection is kept in GridState, so it survives paging.";
            this.dgvQueue.ColumnHeaderMouseClick += new Wisej.Web.DataGridViewCellMouseEventHandler(this.dgvQueue_ColumnHeaderMouseClick);
            this.dgvQueue.SelectionChanged += new System.EventHandler(this.dgvQueue_SelectionChanged);
            //
            // colNumber
            //
            this.colNumber.DataPropertyName = "Number";
            this.colNumber.FillWeight = 11F;
            this.colNumber.HeaderText = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.ReadOnly = true;
            this.colNumber.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colTitle
            //
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.FillWeight = 34F;
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
            this.colDue.FillWeight = 8F;
            this.colDue.HeaderText = "Due";
            this.colDue.Name = "colDue";
            this.colDue.ReadOnly = true;
            this.colDue.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colAge
            //
            this.colAge.DataPropertyName = "AgeDays";
            this.colAge.FillWeight = 7F;
            this.colAge.HeaderText = "Age (d)";
            this.colAge.Name = "colAge";
            this.colAge.ReadOnly = true;
            this.colAge.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            //
            // colVersion  (the concurrency token the batch sends back — watch it change after a reassignment)
            //
            this.colVersion.DataPropertyName = "Version";
            this.colVersion.FillWeight = 7F;
            this.colVersion.HeaderText = "v";
            this.colVersion.Name = "colVersion";
            this.colVersion.ReadOnly = true;
            this.colVersion.SortMode = Wisej.Web.DataGridViewColumnSortMode.NotSortable;
            this.colVersion.ToolTipText = "Optimistic concurrency token. The batch sends it back; a changed row fails instead of overwriting.";
            //
            // btnFirst / btnPrev / lblPage / btnNext / btnLast  (the pager — one page per click, never the table)
            //
            this.btnFirst.Location = new System.Drawing.Point(20, 336);
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(38, 28);
            this.btnFirst.Text = "⏮";
            this.btnFirst.ToolTipText = "First page";
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            this.btnPrev.Location = new System.Drawing.Point(62, 336);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(38, 28);
            this.btnPrev.Text = "◀";
            this.btnPrev.ToolTipText = "Previous page";
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.lblPage.AutoSize = false;
            this.lblPage.Font = new System.Drawing.Font("default", 10F);
            this.lblPage.Location = new System.Drawing.Point(104, 336);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(170, 28);
            this.lblPage.Text = "Page — of —";
            this.lblPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnNext.Location = new System.Drawing.Point(278, 336);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(38, 28);
            this.btnNext.Text = "▶";
            this.btnNext.ToolTipText = "Next page — one more SearchAsync, PageSize rows";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnLast.Location = new System.Drawing.Point(320, 336);
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(38, 28);
            this.btnLast.Text = "⏭";
            this.btnLast.ToolTipText = "Last page";
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            //
            // cboPageSize
            //
            this.cboPageSize.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboPageSize.Location = new System.Drawing.Point(364, 336);
            this.cboPageSize.Name = "cboPageSize";
            this.cboPageSize.Size = new System.Drawing.Size(100, 28);
            this.cboPageSize.ToolTipText = "Rows per page. The service clamps it to MaxPageSize (200) so a forged request cannot ask for the table.";
            //
            // lblSelection
            //
            this.lblSelection.AutoSize = false;
            this.lblSelection.Font = new System.Drawing.Font("default", 9F);
            this.lblSelection.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSelection.Location = new System.Drawing.Point(474, 336);
            this.lblSelection.Name = "lblSelection";
            this.lblSelection.Size = new System.Drawing.Size(386, 28);
            this.lblSelection.Text = "0 selected";
            this.lblSelection.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // cboTechnician
            //
            this.cboTechnician.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTechnician.Location = new System.Drawing.Point(20, 372);
            this.cboTechnician.Name = "cboTechnician";
            this.cboTechnician.Size = new System.Drawing.Size(150, 32);
            this.cboTechnician.ToolTipText = "The batch target. Certifications are checked per row on the server.";
            //
            // btnBatchReassign  (the batch command; becomes Cancel while it runs)
            //
            this.btnBatchReassign.Enabled = false;
            this.btnBatchReassign.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnBatchReassign.Location = new System.Drawing.Point(178, 372);
            this.btnBatchReassign.Name = "btnBatchReassign";
            this.btnBatchReassign.Size = new System.Drawing.Size(210, 32);
            this.btnBatchReassign.Text = "Reassign selected…";
            this.btnBatchReassign.ToolTipText = "btnBatchReassign_Click → confirm → await _batchWorkflow.RunAsync(command, OnProgress, token) → BatchResultDialog.";
            this.btnBatchReassign.Click += new System.EventHandler(this.btnBatchReassign_Click);
            //
            // btnSelectPage / btnClearSelection
            //
            this.btnSelectPage.Location = new System.Drawing.Point(396, 372);
            this.btnSelectPage.Name = "btnSelectPage";
            this.btnSelectPage.Size = new System.Drawing.Size(130, 32);
            this.btnSelectPage.Text = "Select page";
            this.btnSelectPage.ToolTipText = "Adds every row on this page to the server-side selection (it survives paging).";
            this.btnSelectPage.Click += new System.EventHandler(this.btnSelectPage_Click);
            this.btnClearSelection.Location = new System.Drawing.Point(534, 372);
            this.btnClearSelection.Name = "btnClearSelection";
            this.btnClearSelection.Size = new System.Drawing.Size(140, 32);
            this.btnClearSelection.Text = "Clear selection";
            this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(682, 372);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(178, 32);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblProgress / progressBatch  (responsive feedback: something within the first second)
            //
            this.lblProgress.AutoSize = false;
            this.lblProgress.Font = new System.Drawing.Font("monospace", 9F);
            this.lblProgress.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblProgress.Location = new System.Drawing.Point(20, 412);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(840, 20);
            this.lblProgress.Text = "";
            this.lblProgress.Visible = false;
            this.progressBatch.Location = new System.Drawing.Point(20, 436);
            this.progressBatch.Maximum = 100;
            this.progressBatch.Name = "progressBatch";
            this.progressBatch.Size = new System.Drawing.Size(840, 12);
            this.progressBatch.Visible = false;
            //
            // lblBanner  (partial failure / anti-pattern warning; hidden until something needs saying)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblBanner.Location = new System.Drawing.Point(20, 456);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(840, 32);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // lblStatusBar  (the walkthrough's dark footer: "Page 1 of 87 · 50 of 4,331 matching · sorted by Priority ↓ · 38 ms")
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 494);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(840, 32);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(920, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(404, 540);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(372, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(372, 450);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 502);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(372, 24);
            this.lblTraceFooter.Text = "UI → · Session: · Security: · Service: · Data: · Job:";
            //
            // pnlActions  (bottom bar: the anti-pattern, the failure paths, the recoveries, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnLoadEverything);
            this.pnlActions.Controls.Add(this.btnApprovalLock);
            this.pnlActions.Controls.Add(this.btnConcurrentEdit);
            this.pnlActions.Controls.Add(this.btnRetryFailed);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 616);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 44);
            //
            // btnLoadEverything  (the anti-pattern the video measures)
            //
            this.btnLoadEverything.Location = new System.Drawing.Point(0, 4);
            this.btnLoadEverything.Name = "btnLoadEverything";
            this.btnLoadEverything.Size = new System.Drawing.Size(230, 36);
            this.btnLoadEverything.Text = "Anti-pattern: load everything";
            this.btnLoadEverything.ToolTipText = "grid.DataSource = db.WorkOrders.ToList() — every tenant row materialized, projected and bound. Measured against one page.";
            this.btnLoadEverything.Click += new System.EventHandler(this.btnLoadEverything_Click);
            //
            // btnApprovalLock  (failure path 1 / recovery 1: a row locked by an open approval)
            //
            this.btnApprovalLock.Location = new System.Drawing.Point(238, 4);
            this.btnApprovalLock.Name = "btnApprovalLock";
            this.btnApprovalLock.Size = new System.Drawing.Size(216, 36);
            this.btnApprovalLock.Text = "Fail: approval lock on a row";
            this.btnApprovalLock.ToolTipText = "Opens an approval on the last selected row. The next batch fails that row with approval-lock and leaves the others alone.";
            this.btnApprovalLock.Click += new System.EventHandler(this.btnApprovalLock_Click);
            //
            // btnConcurrentEdit  (failure path 2: another user changed the row → stale version)
            //
            this.btnConcurrentEdit.Location = new System.Drawing.Point(462, 4);
            this.btnConcurrentEdit.Name = "btnConcurrentEdit";
            this.btnConcurrentEdit.Size = new System.Drawing.Size(200, 36);
            this.btnConcurrentEdit.Text = "Fail: concurrent edit";
            this.btnConcurrentEdit.ToolTipText = "Bumps the Version of the last selected row, as another session would. The batch fails it with stale-version.";
            this.btnConcurrentEdit.Click += new System.EventHandler(this.btnConcurrentEdit_Click);
            //
            // btnRetryFailed  (the recovery: retry the failures only, never the successes)
            //
            this.btnRetryFailed.Enabled = false;
            this.btnRetryFailed.Location = new System.Drawing.Point(670, 4);
            this.btnRetryFailed.Name = "btnRetryFailed";
            this.btnRetryFailed.Size = new System.Drawing.Size(190, 36);
            this.btnRetryFailed.Text = "Retry failed rows";
            this.btnRetryFailed.ToolTipText = "Re-reads the current version of the failed rows only and runs the batch again under a new correlation id.";
            this.btnRetryFailed.Click += new System.EventHandler(this.btnRetryFailed_Click);
            //
            // btnSwitchUser  (failure path 3 / recovery 3: the permission flags are recomputed server-side)
            //
            this.btnSwitchUser.Location = new System.Drawing.Point(868, 4);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(250, 36);
            this.btnSwitchUser.Text = "Run as ben.tech (Technician)";
            this.btnSwitchUser.ToolTipText = "CanReassign becomes false in the projection and the workflow denies every row. Click again to come back as ana.ops.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 36);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // WorkQueuePage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Name = "WorkQueuePage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Enterprise Work Queue";
            this.Load += new System.EventHandler(this.WorkQueuePage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.ComboBox cboStatus;
        private Wisej.Web.ComboBox cboAssigned;
        private Wisej.Web.ComboBox cboSort;
        private Wisej.Web.Button btnSearch;
        private Wisej.Web.ComboBox cboSavedView;
        private Wisej.Web.Button btnApplyView;
        private Wisej.Web.Button btnSaveView;
        private Wisej.Web.Button btnSimulateRefresh;
        private Wisej.Web.Label lblViewBadge;
        private Wisej.Web.DataGridView dgvQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colAssigned;
        private Wisej.Web.DataGridViewTextBoxColumn colDue;
        private Wisej.Web.DataGridViewTextBoxColumn colAge;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
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
        private Wisej.Web.Label lblStatus;
        private Wisej.Web.Label lblProgress;
        private Wisej.Web.ProgressBar progressBatch;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnLoadEverything;
        private Wisej.Web.Button btnApprovalLock;
        private Wisej.Web.Button btnConcurrentEdit;
        private Wisej.Web.Button btnRetryFailed;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnClearTrace;
    }
}
