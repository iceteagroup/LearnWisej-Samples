namespace TicketOps.Views
{
    partial class TicketWorkflow
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelSignedIn = new Wisej.Web.Label();
            this.comboOperator = new Wisej.Web.ComboBox();
            this.labelProfile = new Wisej.Web.Label();
            this.buttonSwitchProfile = new Wisej.Web.Button();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnHours = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAssignee = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.textReason = new Wisej.Web.TextBox();
            this.buttonClose = new Wisej.Web.Button();
            this.buttonAssign = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.labelServicesTitle = new Wisej.Web.Label();
            this.labelAudit = new Wisej.Web.Label();
            this.gridServices = new Wisej.Web.DataGridView();
            this.columnService = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnImplementation = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnLifetime = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnInstance = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnSecondResolve = new Wisej.Web.DataGridViewTextBoxColumn();
            this.progressTests = new Wisej.Web.ProgressBar();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonRunTests = new Wisej.Web.Button();
            this.buttonCloseAsViewer = new Wisej.Web.Button();
            this.buttonCloseNoHours = new Wisej.Web.Button();
            this.buttonMissingService = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerTests = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Ticket Workflow: operator, tickets, actions, injected services — display and input only)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.labelSignedIn);
            this.panelScreen.Controls.Add(this.comboOperator);
            this.panelScreen.Controls.Add(this.labelProfile);
            this.panelScreen.Controls.Add(this.buttonSwitchProfile);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.gridTickets);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.textReason);
            this.panelScreen.Controls.Add(this.buttonClose);
            this.panelScreen.Controls.Add(this.buttonAssign);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Controls.Add(this.labelServicesTitle);
            this.panelScreen.Controls.Add(this.labelAudit);
            this.panelScreen.Controls.Add(this.gridServices);
            this.panelScreen.Controls.Add(this.progressTests);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Ticket Workflow";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // signed-in operator (IUserService, Session) and the active registration profile
            //
            this.labelSignedIn.AutoSize = false;
            this.labelSignedIn.Font = new System.Drawing.Font("default", 9F);
            this.labelSignedIn.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelSignedIn.Location = new System.Drawing.Point(24, 60);
            this.labelSignedIn.Name = "labelSignedIn";
            this.labelSignedIn.Size = new System.Drawing.Size(84, 20);
            this.labelSignedIn.Text = "Signed in as";
            this.comboOperator.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboOperator.Location = new System.Drawing.Point(108, 54);
            this.comboOperator.Name = "comboOperator";
            this.comboOperator.Size = new System.Drawing.Size(232, 30);
            this.comboOperator.ToolTipText = "IUserService.SignInAs — per session: another browser tab keeps its own operator";
            this.comboOperator.SelectedIndexChanged += new System.EventHandler(this.comboOperator_SelectedIndexChanged);
            this.labelProfile.AutoSize = false;
            this.labelProfile.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelProfile.ForeColor = System.Drawing.Color.FromArgb(26, 134, 255);
            this.labelProfile.Location = new System.Drawing.Point(356, 60);
            this.labelProfile.Name = "labelProfile";
            this.labelProfile.Size = new System.Drawing.Size(236, 20);
            this.labelProfile.Text = "Profile: …";
            this.buttonSwitchProfile.Location = new System.Drawing.Point(600, 52);
            this.buttonSwitchProfile.Name = "buttonSwitchProfile";
            this.buttonSwitchProfile.Size = new System.Drawing.Size(136, 32);
            this.buttonSwitchProfile.Text = "Switch profile";
            this.buttonSwitchProfile.ToolTipText = "Infrastructure path: re-register the other profile with AddOrReplaceService, re-inject this Form, rebuild the presenter — the screen code does not change";
            this.buttonSwitchProfile.Click += new System.EventHandler(this.buttonSwitchProfile_Click);
            //
            // gridTickets
            //
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnPriority,
                this.columnStatus,
                this.columnHours,
                this.columnAssignee});
            this.gridTickets.Location = new System.Drawing.Point(24, 94);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 168);
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // ticket columns
            //
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 60;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 290;
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 80;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 100;
            this.columnHours.HeaderText = "Hours";
            this.columnHours.Name = "columnHours";
            this.columnHours.ReadOnly = true;
            this.columnHours.Width = 60;
            this.columnAssignee.HeaderText = "Assignee";
            this.columnAssignee.Name = "columnAssignee";
            this.columnAssignee.ReadOnly = true;
            this.columnAssignee.Width = 120;
            //
            // labelSelected + close reason + screen buttons (thin handlers → TicketWorkflowPresenter)
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 268);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(712, 20);
            this.labelSelected.Text = "Select a ticket";
            this.textReason.Location = new System.Drawing.Point(24, 292);
            this.textReason.Name = "textReason";
            this.textReason.Size = new System.Drawing.Size(300, 32);
            this.textReason.Watermark = "Close reason (required)";
            this.buttonClose.Location = new System.Drawing.Point(334, 292);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(130, 32);
            this.buttonClose.Text = "Close ticket";
            this.buttonClose.ToolTipText = "Success path: presenter.CloseAsync(id, reason) → permission → domain rule → ITicketService → IAuditLogService → INotificationService";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            this.buttonAssign.Location = new System.Drawing.Point(474, 292);
            this.buttonAssign.Name = "buttonAssign";
            this.buttonAssign.Size = new System.Drawing.Size(130, 32);
            this.buttonAssign.Text = "Assign to me";
            this.buttonAssign.ToolTipText = "Success path: presenter.AssignToMeAsync(id) — the current operator takes the selected ticket";
            this.buttonAssign.Click += new System.EventHandler(this.buttonAssign_Click);
            this.buttonRefresh.Location = new System.Drawing.Point(614, 292);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(122, 32);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // injected services grid (Diagnostics/ServiceProbe: contract → implementation → lifetime → instance → second resolve)
            //
            this.labelServicesTitle.AutoSize = false;
            this.labelServicesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelServicesTitle.Location = new System.Drawing.Point(24, 338);
            this.labelServicesTitle.Name = "labelServicesTitle";
            this.labelServicesTitle.Size = new System.Drawing.Size(330, 20);
            this.labelServicesTitle.Text = "Injected services · Application.Services";
            this.labelAudit.AutoSize = false;
            this.labelAudit.Font = new System.Drawing.Font("default", 9F);
            this.labelAudit.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelAudit.Location = new System.Drawing.Point(360, 338);
            this.labelAudit.Name = "labelAudit";
            this.labelAudit.Size = new System.Drawing.Size(376, 20);
            this.labelAudit.Text = "Audit trail: …";
            this.labelAudit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.gridServices.AllowUserToAddRows = false;
            this.gridServices.AllowUserToDeleteRows = false;
            this.gridServices.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridServices.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnService,
                this.columnImplementation,
                this.columnLifetime,
                this.columnInstance,
                this.columnSecondResolve});
            this.gridServices.Location = new System.Drawing.Point(24, 362);
            this.gridServices.MultiSelect = false;
            this.gridServices.Name = "gridServices";
            this.gridServices.ReadOnly = true;
            this.gridServices.RowHeadersVisible = false;
            this.gridServices.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridServices.Size = new System.Drawing.Size(712, 152);
            this.gridServices.ToolTipText = "Each contract resolved twice: Session and Shared services return the same instance, the Transient one a new instance";
            //
            // service columns
            //
            this.columnService.HeaderText = "Contract";
            this.columnService.Name = "columnService";
            this.columnService.ReadOnly = true;
            this.columnService.Width = 150;
            this.columnImplementation.HeaderText = "Implementation";
            this.columnImplementation.Name = "columnImplementation";
            this.columnImplementation.ReadOnly = true;
            this.columnImplementation.Width = 190;
            this.columnLifetime.HeaderText = "Lifetime";
            this.columnLifetime.Name = "columnLifetime";
            this.columnLifetime.ReadOnly = true;
            this.columnLifetime.Width = 80;
            this.columnInstance.HeaderText = "Instance";
            this.columnInstance.Name = "columnInstance";
            this.columnInstance.ReadOnly = true;
            this.columnInstance.Width = 80;
            this.columnSecondResolve.HeaderText = "Second resolve";
            this.columnSecondResolve.Name = "columnSecondResolve";
            this.columnSecondResolve.ReadOnly = true;
            this.columnSecondResolve.Width = 210;
            //
            // progressTests
            //
            this.progressTests.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressTests.Location = new System.Drawing.Point(24, 524);
            this.progressTests.Maximum = 8;
            this.progressTests.Name = "progressTests";
            this.progressTests.Size = new System.Drawing.Size(712, 18);
            this.progressTests.Visible = false;
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Presenter → Services → Data";
            //
            // panelActions  (bottom bar: progress / failures / DI failure / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonRunTests);
            this.panelActions.Controls.Add(this.buttonCloseAsViewer);
            this.panelActions.Controls.Add(this.buttonCloseNoHours);
            this.panelActions.Controls.Add(this.buttonMissingService);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonRunTests.Location = new System.Drawing.Point(0, 4);
            this.buttonRunTests.Name = "buttonRunTests";
            this.buttonRunTests.Size = new System.Drawing.Size(190, 36);
            this.buttonRunTests.Text = "▶ Run presenter tests";
            this.buttonRunTests.ToolTipText = "Progress path: a Timer runs one presenter test per tick, each against fresh fakes — no container, no browser in the decisions";
            this.buttonRunTests.Click += new System.EventHandler(this.buttonRunTests_Click);
            this.buttonCloseAsViewer.Location = new System.Drawing.Point(200, 4);
            this.buttonCloseAsViewer.Name = "buttonCloseAsViewer";
            this.buttonCloseAsViewer.Size = new System.Drawing.Size(170, 36);
            this.buttonCloseAsViewer.Text = "Close #1041 as Viewer";
            this.buttonCloseAsViewer.ToolTipText = "Failure path 1 (permission): IPermissionService.CanClose says no — nothing is written, nothing is audited";
            this.buttonCloseAsViewer.Click += new System.EventHandler(this.buttonCloseAsViewer_Click);
            this.buttonCloseNoHours.Location = new System.Drawing.Point(380, 4);
            this.buttonCloseNoHours.Name = "buttonCloseNoHours";
            this.buttonCloseNoHours.Size = new System.Drawing.Size(200, 36);
            this.buttonCloseNoHours.Text = "Close #1042 without hours";
            this.buttonCloseNoHours.ToolTipText = "Failure path 2 (domain rule): Ticket.CanClose says \"Log hours before closing.\" — the Form never knew the rule";
            this.buttonCloseNoHours.Click += new System.EventHandler(this.buttonCloseNoHours_Click);
            this.buttonMissingService.Location = new System.Drawing.Point(590, 4);
            this.buttonMissingService.Name = "buttonMissingService";
            this.buttonMissingService.Size = new System.Drawing.Size(200, 36);
            this.buttonMissingService.Text = "Resolve a missing service";
            this.buttonMissingService.ToolTipText = "Failure path 3 (DI): GetService<IExportService>() returns null because no profile registers it — the screen shows a clear message instead of crashing";
            this.buttonMissingService.Click += new System.EventHandler(this.buttonMissingService_Click);
            this.buttonOutage.Location = new System.Drawing.Point(800, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(200, 36);
            this.buttonOutage.Text = "Simulate data outage";
            this.buttonOutage.ToolTipText = "Error path: the ticket service throws; the log gets the details, the user gets a safe message. Click again to recover.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerTests
            //
            this.timerTests.Interval = 250;
            this.timerTests.Tick += new System.EventHandler(this.timerTests_Tick);
            //
            // TicketWorkflow
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "TicketWorkflow";
            this.Text = "TicketOps Console — Module 8 · Services, DI & testable UI";
            this.Load += new System.EventHandler(this.TicketWorkflow_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelSignedIn;
        private Wisej.Web.ComboBox comboOperator;
        private Wisej.Web.Label labelProfile;
        private Wisej.Web.Button buttonSwitchProfile;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnHours;
        private Wisej.Web.DataGridViewTextBoxColumn columnAssignee;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.TextBox textReason;
        private Wisej.Web.Button buttonClose;
        private Wisej.Web.Button buttonAssign;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Label labelServicesTitle;
        private Wisej.Web.Label labelAudit;
        private Wisej.Web.DataGridView gridServices;
        private Wisej.Web.DataGridViewTextBoxColumn columnService;
        private Wisej.Web.DataGridViewTextBoxColumn columnImplementation;
        private Wisej.Web.DataGridViewTextBoxColumn columnLifetime;
        private Wisej.Web.DataGridViewTextBoxColumn columnInstance;
        private Wisej.Web.DataGridViewTextBoxColumn columnSecondResolve;
        private Wisej.Web.ProgressBar progressTests;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonRunTests;
        private Wisej.Web.Button buttonCloseAsViewer;
        private Wisej.Web.Button buttonCloseNoHours;
        private Wisej.Web.Button buttonMissingService;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerTests;
    }
}
