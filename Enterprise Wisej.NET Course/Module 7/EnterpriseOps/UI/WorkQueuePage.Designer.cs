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
            this.lblTenant = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.lblCorrelation = new Wisej.Web.Label();
            this.pnlQueue = new Wisej.Web.Panel();
            this.btnEscalate = new Wisej.Web.Button();
            this.lblStatus = new Wisej.Web.Label();
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
            this.btnResolveManually = new Wisej.Web.Button();
            this.lblStatusBar = new Wisej.Web.Label();
            this.pnlTrace = new Wisej.Web.Panel();
            this.lblTraceTitle = new Wisej.Web.Label();
            this.lstTrace = new Wisej.Web.ListBox();
            this.lblTraceFooter = new Wisej.Web.Label();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnFailNotify = new Wisej.Web.Button();
            this.btnFailAudit = new Wisej.Web.Button();
            this.btnFailDirectory = new Wisej.Web.Button();
            this.btnRunHeadless = new Wisej.Web.Button();
            this.btnAntiPattern = new Wisej.Web.Button();
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
            this.lblTitle.Size = new System.Drawing.Size(560, 44);
            this.lblTitle.Text = "EnterpriseOps — Work queue · Module 7 · workflow UX";
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
            this.lblUser.Font = new System.Drawing.Font("default", 9F);
            this.lblUser.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblUser.Location = new System.Drawing.Point(870, 0);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(280, 44);
            this.lblUser.Text = "user: ana.ops (Manager)";
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
            // pnlQueue  (the work queue + the manual-review queue the compensation fills)
            //
            this.pnlQueue.BackColor = System.Drawing.Color.White;
            this.pnlQueue.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.pnlQueue.Controls.Add(this.btnEscalate);
            this.pnlQueue.Controls.Add(this.lblStatus);
            this.pnlQueue.Controls.Add(this.dgvWorkQueue);
            this.pnlQueue.Controls.Add(this.lblBanner);
            this.pnlQueue.Controls.Add(this.lblCompensationTitle);
            this.pnlQueue.Controls.Add(this.lstCompensation);
            this.pnlQueue.Controls.Add(this.btnRetryNotification);
            this.pnlQueue.Controls.Add(this.btnResolveManually);
            this.pnlQueue.Controls.Add(this.lblStatusBar);
            this.pnlQueue.Location = new System.Drawing.Point(24, 64);
            this.pnlQueue.Name = "pnlQueue";
            this.pnlQueue.Size = new System.Drawing.Size(812, 490);
            //
            // btnEscalate  (the success path: the wizard, opened modally)
            //
            this.btnEscalate.Location = new System.Drawing.Point(20, 14);
            this.btnEscalate.Name = "btnEscalate";
            this.btnEscalate.Size = new System.Drawing.Size(230, 36);
            this.btnEscalate.Text = "Escalate work order…";
            this.btnEscalate.ToolTipText = "new EscalationWizard(services, workOrderId) → await ShowDialogAsync() → show the typed WorkflowResult.";
            this.btnEscalate.Click += new System.EventHandler(this.btnEscalate_Click);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.lblStatus.Location = new System.Drawing.Point(262, 19);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(530, 26);
            this.lblStatus.Text = "● ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // dgvWorkQueue  (WorkQueueRow projections — never the WorkOrder entity)
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
            // lblBanner  (failure banner — one line, no internals)
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
            // lstCompensation  (what the workflow recorded instead of pretending it was atomic)
            //
            this.lstCompensation.Font = new System.Drawing.Font("monospace", 9F);
            this.lstCompensation.Location = new System.Drawing.Point(20, 338);
            this.lstCompensation.Name = "lstCompensation";
            this.lstCompensation.Size = new System.Drawing.Size(596, 88);
            //
            // btnRetryNotification  (the recovery: the workflow finishes later)
            //
            this.btnRetryNotification.Location = new System.Drawing.Point(626, 338);
            this.btnRetryNotification.Name = "btnRetryNotification";
            this.btnRetryNotification.Size = new System.Drawing.Size(166, 36);
            this.btnRetryNotification.Text = "Retry notification";
            this.btnRetryNotification.ToolTipText = "EscalationWorkflow.RetryNotificationAsync(entry, session): the queued compensation is retried and resolved.";
            this.btnRetryNotification.Click += new System.EventHandler(this.btnRetryNotification_Click);
            //
            // btnResolveManually  (an audit gap cannot be retried — an operator closes it)
            //
            this.btnResolveManually.Location = new System.Drawing.Point(626, 382);
            this.btnResolveManually.Name = "btnResolveManually";
            this.btnResolveManually.Size = new System.Drawing.Size(166, 36);
            this.btnResolveManually.Text = "Resolve manually";
            this.btnResolveManually.ToolTipText = "Closes the selected compensation with a reason — the only way to clear an audit gap.";
            this.btnResolveManually.Click += new System.EventHandler(this.btnResolveManually_Click);
            //
            // lblStatusBar  (the last WorkflowResult, verbatim)
            //
            this.lblStatusBar.AutoSize = false;
            this.lblStatusBar.BackColor = System.Drawing.Color.FromArgb(15, 36, 64);
            this.lblStatusBar.Font = new System.Drawing.Font("monospace", 9F);
            this.lblStatusBar.ForeColor = System.Drawing.Color.FromArgb(159, 192, 232);
            this.lblStatusBar.Location = new System.Drawing.Point(20, 434);
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblStatusBar.Size = new System.Drawing.Size(772, 34);
            this.lblStatusBar.Text = "WorkflowResult — none yet";
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
            this.pnlTrace.Location = new System.Drawing.Point(852, 64);
            this.pnlTrace.Name = "pnlTrace";
            this.pnlTrace.Size = new System.Drawing.Size(472, 490);
            //
            // lblTraceTitle
            //
            this.lblTraceTitle.AutoSize = false;
            this.lblTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTraceTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTraceTitle.Name = "lblTraceTitle";
            this.lblTraceTitle.Size = new System.Drawing.Size(440, 28);
            this.lblTraceTitle.Text = "Server · live activity trace";
            //
            // lstTrace
            //
            this.lstTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lstTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.lstTrace.Location = new System.Drawing.Point(16, 46);
            this.lstTrace.Name = "lstTrace";
            this.lstTrace.Size = new System.Drawing.Size(440, 400);
            //
            // lblTraceFooter
            //
            this.lblTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.lblTraceFooter.AutoSize = false;
            this.lblTraceFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblTraceFooter.Location = new System.Drawing.Point(16, 452);
            this.lblTraceFooter.Name = "lblTraceFooter";
            this.lblTraceFooter.Size = new System.Drawing.Size(440, 28);
            this.lblTraceFooter.Text = "UI → · Service: · Security: · Data: · Integrations: · UI ←";
            //
            // pnlActions  (the failure paths, the headless run and the anti-pattern)
            //
            this.pnlActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.pnlActions.Controls.Add(this.btnFailNotify);
            this.pnlActions.Controls.Add(this.btnFailAudit);
            this.pnlActions.Controls.Add(this.btnFailDirectory);
            this.pnlActions.Controls.Add(this.btnRunHeadless);
            this.pnlActions.Controls.Add(this.btnAntiPattern);
            this.pnlActions.Controls.Add(this.btnClearTrace);
            this.pnlActions.Location = new System.Drawing.Point(24, 570);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(1300, 46);
            //
            // btnFailNotify  (failure path 1 — the lab's compensation case)
            //
            this.btnFailNotify.Location = new System.Drawing.Point(0, 4);
            this.btnFailNotify.Name = "btnFailNotify";
            this.btnFailNotify.Size = new System.Drawing.Size(214, 38);
            this.btnFailNotify.Text = "Fail: notification (SMTP)";
            this.btnFailNotify.ToolTipText = "Arms FakeNotificationGateway: the next send throws AFTER the escalation was persisted → CompensationAction.";
            this.btnFailNotify.Click += new System.EventHandler(this.btnFailNotify_Click);
            //
            // btnFailAudit  (failure path 2 — review question 3)
            //
            this.btnFailAudit.Location = new System.Drawing.Point(222, 4);
            this.btnFailAudit.Name = "btnFailAudit";
            this.btnFailAudit.Size = new System.Drawing.Size(214, 38);
            this.btnFailAudit.Text = "Fail: audit after notify";
            this.btnFailAudit.ToolTipText = "Arms AuditLog: the next write throws after the notification was sent — a sent notification cannot be unsent.";
            this.btnFailAudit.Click += new System.EventHandler(this.btnFailAudit_Click);
            //
            // btnFailDirectory  (failure path 3 — the matrix's timeout column)
            //
            this.btnFailDirectory.Location = new System.Drawing.Point(444, 4);
            this.btnFailDirectory.Name = "btnFailDirectory";
            this.btnFailDirectory.Size = new System.Drawing.Size(214, 38);
            this.btnFailDirectory.Text = "Fail: directory timeout";
            this.btnFailDirectory.ToolTipText = "Arms FakeApproverDirectory: the Approver step's lookup outlasts the wizard's 5 s timeout; the draft survives.";
            this.btnFailDirectory.Click += new System.EventHandler(this.btnFailDirectory_Click);
            //
            // btnRunHeadless  (review question 1: the workflow without any wizard)
            //
            this.btnRunHeadless.Location = new System.Drawing.Point(666, 4);
            this.btnRunHeadless.Name = "btnRunHeadless";
            this.btnRunHeadless.Size = new System.Drawing.Size(250, 38);
            this.btnRunHeadless.Text = "Run workflow without the wizard";
            this.btnRunHeadless.ToolTipText = "Builds an EscalationCommand in code and calls EscalateAsync — the same transition, no UI involved.";
            this.btnRunHeadless.Click += new System.EventHandler(this.btnRunHeadless_Click);
            //
            // btnAntiPattern  (the video's anti-pattern: the same flow written inside the page)
            //
            this.btnAntiPattern.Location = new System.Drawing.Point(924, 4);
            this.btnAntiPattern.Name = "btnAntiPattern";
            this.btnAntiPattern.Size = new System.Drawing.Size(254, 38);
            this.btnAntiPattern.Text = "Anti-pattern: logic in the page";
            this.btnAntiPattern.ToolTipText = "The same escalation inlined in this handler: it deletes the escalation when the notification fails — a false transaction.";
            this.btnAntiPattern.Click += new System.EventHandler(this.btnAntiPattern_Click);
            //
            // btnClearTrace
            //
            this.btnClearTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.btnClearTrace.Location = new System.Drawing.Point(1190, 4);
            this.btnClearTrace.Name = "btnClearTrace";
            this.btnClearTrace.Size = new System.Drawing.Size(110, 38);
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
            this.Text = "EnterpriseOps — Work queue";
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
        private Wisej.Web.Button btnEscalate;
        private Wisej.Web.Label lblStatus;
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
        private Wisej.Web.Button btnResolveManually;
        private Wisej.Web.Label lblStatusBar;
        private Wisej.Web.Panel pnlTrace;
        private Wisej.Web.Label lblTraceTitle;
        private Wisej.Web.ListBox lstTrace;
        private Wisej.Web.Label lblTraceFooter;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnFailNotify;
        private Wisej.Web.Button btnFailAudit;
        private Wisej.Web.Button btnFailDirectory;
        private Wisej.Web.Button btnRunHeadless;
        private Wisej.Web.Button btnAntiPattern;
        private Wisej.Web.Button btnClearTrace;
    }
}
