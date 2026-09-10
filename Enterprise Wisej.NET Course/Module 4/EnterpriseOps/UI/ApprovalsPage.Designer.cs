namespace EnterpriseOps.UI
{
    partial class ApprovalsPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used. The session's SQLite connection and the anti-pattern's
        /// field-held DbContext are owned by this screen, so they are released here (see
        /// <see cref="ApprovalsPage.DisposeSessionResources"/> in the code-behind).
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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblContexts = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlQueue = new Wisej.Web.Panel();
            this.lblQueueTitle = new Wisej.Web.Label();
            this.cboTenant = new Wisej.Web.ComboBox();
            this.txtSearch = new Wisej.Web.TextBox();
            this.btnSearch = new Wisej.Web.Button();
            this.dgvWorkQueue = new Wisej.Web.DataGridView();
            this.colNumber = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblQueueFooter = new Wisej.Web.Label();
            this.pnlApprove = new Wisej.Web.Panel();
            this.lblApproveTitle = new Wisej.Web.Label();
            this.lblWorkOrder = new Wisej.Web.Label();
            this.lblVersion = new Wisej.Web.Label();
            this.lblCommentCaption = new Wisej.Web.Label();
            this.txtComment = new Wisej.Web.TextBox();
            this.btnApprove = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.lblResult = new Wisej.Web.Label();
            this.lblResultDetail = new Wisej.Web.Label();
            this.lblApproveFooter = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnCreate = new Wisej.Web.Button();
            this.btnDuplicate = new Wisej.Web.Button();
            this.btnStale = new Wisej.Web.Button();
            this.btnSlowQuery = new Wisej.Web.Button();
            this.btnSwitchUser = new Wisej.Web.Button();
            this.btnBatchApprove = new Wisej.Web.Button();
            this.btnWrongLifetime = new Wisej.Web.Button();
            this.btnFixLifetime = new Wisej.Web.Button();
            this.btnAudit = new Wisej.Web.Button();
            this.btnClearTrace = new Wisej.Web.Button();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlQueue.SuspendLayout();
            this.pnlApprove.SuspendLayout();
            this.pnlTrace.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlHeader  (slim header bar: screen name · tenant · user · live DbContext count · correlation id)
            //
            this.pnlHeader.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlHeader.Controls.Add(this.lblTitle);
            this.pnlHeader.Controls.Add(this.lblTenant);
            this.pnlHeader.Controls.Add(this.lblUser);
            this.pnlHeader.Controls.Add(this.lblContexts);
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
            this.lblTitle.Text = "EnterpriseOps — Approvals";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblTenant
            //
            this.lblTenant.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblTenant.AutoSize = false;
            this.lblTenant.Font = new System.Drawing.Font("default", 9F);
            this.lblTenant.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblTenant.Location = new System.Drawing.Point(560, 0);
            this.lblTenant.Name = "lblTenant";
            this.lblTenant.Size = new System.Drawing.Size(180, 44);
            this.lblTenant.Text = "tenant: fabrikam";
            this.lblTenant.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblUser
            //
            this.lblUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblUser.AutoSize = false;
            this.lblUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblUser.ForeColor = System.Drawing.Color.White;
            this.lblUser.Location = new System.Drawing.Point(748, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(260, 44);
            this.lblUser.Text = "Signed in: ana.ops · Manager";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblContexts  (the lifetime decision, live: how many DbContexts were created, how many are alive)
            //
            this.lblContexts.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblContexts.AutoSize = false;
            this.lblContexts.Font = new System.Drawing.Font("monospace", 9F);
            this.lblContexts.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblContexts.Location = new System.Drawing.Point(1012, 0);
            this.lblContexts.Name = "lblContexts";
            this.lblContexts.Size = new System.Drawing.Size(180, 44);
            this.lblContexts.Text = "ctx 0 · live 0";
            this.lblContexts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblContexts.ToolTipText = "DbContexts created this session / still alive. Between operations \"live\" must read 0.";
            //
            // lblCorrelation
            //
            this.lblCorrelation.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.lblCorrelation.AutoSize = false;
            this.lblCorrelation.Font = new System.Drawing.Font("monospace", 9F);
            this.lblCorrelation.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblCorrelation.Location = new System.Drawing.Point(1196, 0);
            this.lblCorrelation.Name = "lblCorrelation";
            this.lblCorrelation.Size = new System.Drawing.Size(128, 44);
            this.lblCorrelation.Text = "corr —";
            this.lblCorrelation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // pnlQueue  (the read side: WorkQueueRow projections, never the entity)
            //
            this.pnlQueue.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.lblQueueTitle);
            this.pnlQueue.Controls.Add(this.cboTenant);
            this.pnlQueue.Controls.Add(this.txtSearch);
            this.pnlQueue.Controls.Add(this.btnSearch);
            this.pnlQueue.Controls.Add(this.dgvWorkQueue);
            this.pnlQueue.Controls.Add(this.lblQueueFooter);
            this.pnlQueue.Location = new System.Drawing.Point(24, 56);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(500, 460);
            //
            // lblQueueTitle
            //
            this.lblQueueTitle.AutoSize = false;
            this.lblQueueTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblQueueTitle.Location = new System.Drawing.Point(16, 12);
            this.lblQueueTitle.Name = "lblQueueTitle";
            this.lblQueueTitle.Size = new System.Drawing.Size(468, 28);
            this.lblQueueTitle.Text = "Work queue · the read side";
            //
            // cboTenant
            //
            this.cboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboTenant.Location = new System.Drawing.Point(16, 46);
            this.cboTenant.Name = "cboTenant";
            this.cboTenant.Size = new System.Drawing.Size(140, 30);
            this.cboTenant.ToolTipText = "Every query and every command is filtered by tenant — a command can never touch another tenant's row.";
            this.cboTenant.SelectedIndexChanged += new System.EventHandler(this.cboTenant_SelectedIndexChanged);
            //
            // txtSearch
            //
            this.txtSearch.Location = new System.Drawing.Point(164, 46);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 30);
            this.txtSearch.Watermark = "Search number, title, customer, site";
            //
            // btnSearch
            //
            this.btnSearch.Location = new System.Drawing.Point(372, 46);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(112, 30);
            this.btnSearch.Text = "Search";
            this.btnSearch.ToolTipText = "btnSearch_Click → await _queries.SearchAsync(query, ct). One short-lived DbContext, AsNoTracking, projected.";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // dgvWorkQueue  (explicit columns bound to WorkQueueRow — the projection, never the entity)
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
            this.dgvWorkQueue.Size = new System.Drawing.Size(468, 328);
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
            // lblQueueFooter
            //
            this.lblQueueFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblQueueFooter.AutoSize = false;
            this.lblQueueFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblQueueFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblQueueFooter.Location = new System.Drawing.Point(16, 422);
            this.lblQueueFooter.Name = "lblQueueFooter";
            this.lblQueueFooter.Size = new System.Drawing.Size(468, 30);
            this.lblQueueFooter.Text = "IWorkOrderQueryService · AsNoTracking · projected to WorkQueueRow · one DbContext per query";
            //
            // pnlApprove  (the walkthrough's ApprovePanel: comment, Approve/Cancel, the result banner)
            //
            this.pnlApprove.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.pnlApprove.BackColor = System.Drawing.Color.White;
            this.pnlApprove.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlApprove.Controls.Add(this.lblApproveTitle);
            this.pnlApprove.Controls.Add(this.lblWorkOrder);
            this.pnlApprove.Controls.Add(this.lblVersion);
            this.pnlApprove.Controls.Add(this.lblCommentCaption);
            this.pnlApprove.Controls.Add(this.txtComment);
            this.pnlApprove.Controls.Add(this.btnApprove);
            this.pnlApprove.Controls.Add(this.btnCancel);
            this.pnlApprove.Controls.Add(this.lblResult);
            this.pnlApprove.Controls.Add(this.lblResultDetail);
            this.pnlApprove.Controls.Add(this.lblApproveFooter);
            this.pnlApprove.Location = new System.Drawing.Point(536, 56);
            this.pnlApprove.Name = "pnlApprove";
            this.pnlApprove.Size = new System.Drawing.Size(380, 460);
            //
            // lblApproveTitle
            //
            this.lblApproveTitle.AutoSize = false;
            this.lblApproveTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblApproveTitle.Location = new System.Drawing.Point(16, 12);
            this.lblApproveTitle.Name = "lblApproveTitle";
            this.lblApproveTitle.Size = new System.Drawing.Size(348, 28);
            this.lblApproveTitle.Text = "Approve work order";
            //
            // lblWorkOrder
            //
            this.lblWorkOrder.AutoSize = false;
            this.lblWorkOrder.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblWorkOrder.Location = new System.Drawing.Point(16, 44);
            this.lblWorkOrder.Name = "lblWorkOrder";
            this.lblWorkOrder.Size = new System.Drawing.Size(348, 26);
            this.lblWorkOrder.Text = "—";
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = false;
            this.lblVersion.Font = new System.Drawing.Font("monospace", 9F);
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblVersion.Location = new System.Drawing.Point(16, 70);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(348, 24);
            this.lblVersion.Text = "select a work order";
            //
            // lblCommentCaption
            //
            this.lblCommentCaption.AutoSize = false;
            this.lblCommentCaption.Font = new System.Drawing.Font("default", 8F, System.Drawing.FontStyle.Bold);
            this.lblCommentCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.lblCommentCaption.Location = new System.Drawing.Point(16, 98);
            this.lblCommentCaption.Name = "lblCommentCaption";
            this.lblCommentCaption.Size = new System.Drawing.Size(348, 20);
            this.lblCommentCaption.Text = "APPROVAL COMMENT";
            //
            // txtComment
            //
            this.txtComment.Location = new System.Drawing.Point(16, 118);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(348, 72);
            this.txtComment.Text = "Parts verified and installed; pressure test passed.";
            this.txtComment.Watermark = "Why is this work order being approved?";
            //
            // btnApprove  (the one-line handler the lesson asks for)
            //
            this.btnApprove.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnApprove.Location = new System.Drawing.Point(16, 200);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new System.Drawing.Size(130, 40);
            this.btnApprove.Text = "Approve";
            this.btnApprove.ToolTipText = "btnApprove_Click → var result = await _commands.ApproveAsync(BuildApproveCommand(), ctx, ct); ShowResult(result);";
            this.btnApprove.Click += new System.EventHandler(this.btnApprove_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(154, 200);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.ToolTipText = "Clears the comment and the banner. Pure UI state — no service, no database.";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lblResult  (CommandResult.UserMessage — never an exception, never SQL)
            //
            this.lblResult.AutoSize = false;
            this.lblResult.BackColor = System.Drawing.Color.FromArgb(240, 249, 243);
            this.lblResult.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblResult.ForeColor = System.Drawing.Color.FromArgb(21, 95, 51);
            this.lblResult.Location = new System.Drawing.Point(16, 252);
            this.lblResult.Name = "lblResult";
            this.lblResult.Padding = new Wisej.Web.Padding(12, 8, 12, 8);
            this.lblResult.Size = new System.Drawing.Size(348, 64);
            this.lblResult.Text = "";
            this.lblResult.Visible = false;
            //
            // lblResultDetail  (CommandResult.Detail — the code, the correlation id, what happened to the transaction)
            //
            this.lblResultDetail.AutoSize = false;
            this.lblResultDetail.Font = new System.Drawing.Font("monospace", 8F);
            this.lblResultDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblResultDetail.Location = new System.Drawing.Point(16, 318);
            this.lblResultDetail.Name = "lblResultDetail";
            this.lblResultDetail.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblResultDetail.Size = new System.Drawing.Size(348, 46);
            this.lblResultDetail.Text = "";
            this.lblResultDetail.Visible = false;
            //
            // lblApproveFooter
            //
            this.lblApproveFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblApproveFooter.AutoSize = false;
            this.lblApproveFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblApproveFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblApproveFooter.Location = new System.Drawing.Point(16, 372);
            this.lblApproveFooter.Name = "lblApproveFooter";
            this.lblApproveFooter.Size = new System.Drawing.Size(348, 80);
            this.lblApproveFooter.Text = "The handler builds ApproveWorkOrderCommand and shows CommandResult. It has no DbContext, no LINQ and no transaction — those live in EnterpriseOps.Data.WorkOrderCommandService.";
            //
            // pnlTrace  (Server · live activity trace)
            //
            this.pnlTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlTrace.BackColor = System.Drawing.Color.White;
            this.pnlTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlTrace.Controls.Add(this.lblTraceTitle);
            this.pnlTrace.Controls.Add(this.lstTrace);
            this.pnlTrace.Controls.Add(this.lblTraceFooter);
            this.pnlTrace.Location = new System.Drawing.Point(928, 56);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(396, 460);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(364, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(364, 368);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 420);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(364, 32);
            this.lblTraceFooter.Text = "UI → · Security: · Service: · Data: · Audit:";
            //
            // pnlActions  (bottom bar: success, progress, four failure paths, the recovery, the audit, clear)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnCreate);
            this.pnlActions.Controls.Add(this.btnDuplicate);
            this.pnlActions.Controls.Add(this.btnStale);
            this.pnlActions.Controls.Add(this.btnSlowQuery);
            this.pnlActions.Controls.Add(this.btnSwitchUser);
            this.pnlActions.Controls.Add(this.btnBatchApprove);
            this.pnlActions.Controls.Add(this.btnWrongLifetime);
            this.pnlActions.Controls.Add(this.btnFixLifetime);
            this.pnlActions.Controls.Add(this.btnAudit);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 528);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 88);
            //
            // btnCreate  (success path: INSERT WorkOrders + INSERT AuditEntries in one transaction)
            //
            this.btnCreate.Location = new System.Drawing.Point(0, 2);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(252, 38);
            this.btnCreate.Text = "Create work order";
            this.btnCreate.ToolTipText = "CreateWorkOrderCommand → validate → INSERT → audit → COMMIT. The new row appears in the grid.";
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            //
            // btnDuplicate  (failure path: the UNIQUE index decides)
            //
            this.btnDuplicate.Location = new System.Drawing.Point(262, 2);
            this.btnDuplicate.Name = "btnDuplicate";
            this.btnDuplicate.Size = new System.Drawing.Size(252, 38);
            this.btnDuplicate.Text = "Fail: duplicate number";
            this.btnDuplicate.ToolTipText = "Creates a work order with the selected row's number: SqliteException 19 UNIQUE → WO_NUMBER_IN_USE.";
            this.btnDuplicate.Click += new System.EventHandler(this.btnDuplicate_Click);
            //
            // btnStale  (failure path: optimistic concurrency)
            //
            this.btnStale.Location = new System.Drawing.Point(524, 2);
            this.btnStale.Name = "btnStale";
            this.btnStale.Size = new System.Drawing.Size(252, 38);
            this.btnStale.Text = "Fail: stale version";
            this.btnStale.ToolTipText = "Approves with ExpectedVersion = v−1: UPDATE … WHERE Version = v−1 affects 0 rows → WO_CONCURRENCY.";
            this.btnStale.Click += new System.EventHandler(this.btnStale_Click);
            //
            // btnSlowQuery  (failure path: the command timeout)
            //
            this.btnSlowQuery.Location = new System.Drawing.Point(786, 2);
            this.btnSlowQuery.Name = "btnSlowQuery";
            this.btnSlowQuery.Size = new System.Drawing.Size(252, 38);
            this.btnSlowQuery.Text = "Fail: slow query (timeout)";
            this.btnSlowQuery.ToolTipText = "The next command sleeps 1500 ms inside its transaction with a 400 ms budget → DB_TIMEOUT + correlation id.";
            this.btnSlowQuery.Click += new System.EventHandler(this.btnSlowQuery_Click);
            //
            // btnSwitchUser  (failure path: server-side authorization, then the recovery)
            //
            this.btnSwitchUser.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnSwitchUser.Location = new System.Drawing.Point(1048, 2);
            this.btnSwitchUser.Name = "btnSwitchUser";
            this.btnSwitchUser.Size = new System.Drawing.Size(252, 38);
            this.btnSwitchUser.Text = "Sign in as ben.tech";
            this.btnSwitchUser.ToolTipText = "A Technician may not approve: PERMISSION_DENIED before any transaction opens. Click again to come back as ana.ops.";
            this.btnSwitchUser.Click += new System.EventHandler(this.btnSwitchUser_Click);
            //
            // btnBatchApprove  (progress path: one transaction per work order, committed or rolled back on its own)
            //
            this.btnBatchApprove.Location = new System.Drawing.Point(0, 46);
            this.btnBatchApprove.Name = "btnBatchApprove";
            this.btnBatchApprove.Size = new System.Drawing.Size(252, 38);
            this.btnBatchApprove.Text = "Batch approve (one tx each)";
            this.btnBatchApprove.ToolTipText = "Six candidates, six commands, six transactions: InProgress rows commit, OnHold rows are rejected. Nothing half-saved.";
            this.btnBatchApprove.Click += new System.EventHandler(this.btnBatchApprove_Click);
            //
            // btnWrongLifetime  (the lifetime decision's failure path)
            //
            this.btnWrongLifetime.Location = new System.Drawing.Point(262, 46);
            this.btnWrongLifetime.Name = "btnWrongLifetime";
            this.btnWrongLifetime.Size = new System.Drawing.Size(252, 38);
            this.btnWrongLifetime.Text = "Wrong lifetime: session DbContext";
            this.btnWrongLifetime.ToolTipText = "Reads through a DbContext held in a field: tracked entities pile up and the values go stale. Approve something, then click again.";
            this.btnWrongLifetime.Click += new System.EventHandler(this.btnWrongLifetime_Click);
            //
            // btnFixLifetime  (the recovery)
            //
            this.btnFixLifetime.Location = new System.Drawing.Point(524, 46);
            this.btnFixLifetime.Name = "btnFixLifetime";
            this.btnFixLifetime.Size = new System.Drawing.Size(252, 38);
            this.btnFixLifetime.Text = "Recover: dispose the context";
            this.btnFixLifetime.ToolTipText = "Disposes the field-held context. The next read comes from a short-lived one and is correct again.";
            this.btnFixLifetime.Click += new System.EventHandler(this.btnFixLifetime_Click);
            //
            // btnAudit
            //
            this.btnAudit.Location = new System.Drawing.Point(786, 46);
            this.btnAudit.Name = "btnAudit";
            this.btnAudit.Size = new System.Drawing.Size(252, 38);
            this.btnAudit.Text = "Audit log";
            this.btnAudit.ToolTipText = "Every committed and every rejected command, with its error code and correlation id. Managers and admins only.";
            this.btnAudit.Click += new System.EventHandler(this.btnAudit_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1048, 46);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(252, 38);
            this.btnClearTrace.Text = "Clear trace";
            this.btnClearTrace.Click += new System.EventHandler(this.btnClearTrace_Click);
            //
            // lblStatusBar  (the walkthrough's dark footer: "ApproveAsync — short-lived DbContext · transaction open…")
            //
            this.lblStatusBar.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(24, 626);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(1300, 36);
            this.lblStatusBar.Text = "Loading…";
            this.lblStatusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // ApprovalsPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlQueue);
            this.Controls.Add(this.pnlApprove);
            this.Controls.Add(this.pnlTrace);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.lblStatusBar);
            this.Name = "ApprovalsPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "EnterpriseOps — Approvals";
            this.Load += new System.EventHandler(this.ApprovalsPage_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlQueue.ResumeLayout(false);
            this.pnlApprove.ResumeLayout(false);
            this.pnlTrace.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblTenant;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.Label lblContexts;
        private Wisej.Web.Label lblCorrelation;
        private Wisej.Web.Panel pnlQueue;
        private Wisej.Web.Label lblQueueTitle;
        private Wisej.Web.ComboBox cboTenant;
        private Wisej.Web.TextBox txtSearch;
        private Wisej.Web.Button btnSearch;
        private Wisej.Web.DataGridView dgvWorkQueue;
        private Wisej.Web.DataGridViewTextBoxColumn colNumber;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colPriority;
        private Wisej.Web.DataGridViewTextBoxColumn colVersion;
        private Wisej.Web.Label lblQueueFooter;
        private Wisej.Web.Panel pnlApprove;
        private Wisej.Web.Label lblApproveTitle;
        private Wisej.Web.Label lblWorkOrder;
        private Wisej.Web.Label lblVersion;
        private Wisej.Web.Label lblCommentCaption;
        private Wisej.Web.TextBox txtComment;
        private Wisej.Web.Button btnApprove;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblResult;
        private Wisej.Web.Label lblResultDetail;
        private Wisej.Web.Label lblApproveFooter;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnCreate;
        private Wisej.Web.Button btnDuplicate;
        private Wisej.Web.Button btnStale;
        private Wisej.Web.Button btnSlowQuery;
        private Wisej.Web.Button btnSwitchUser;
        private Wisej.Web.Button btnBatchApprove;
        private Wisej.Web.Button btnWrongLifetime;
        private Wisej.Web.Button btnFixLifetime;
        private Wisej.Web.Button btnAudit;
        private Wisej.Web.Button btnClearTrace;
        private Wisej.Web.Label lblStatusBar;
    }
}
