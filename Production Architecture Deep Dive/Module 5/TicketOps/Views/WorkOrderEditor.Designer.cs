namespace TicketOps.Views
{
    partial class WorkOrderEditor
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
            this.labelCount = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAssignee = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnCost = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnHours = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnVersion = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.labelRoleCaption = new Wisej.Web.Label();
            this.comboRole = new Wisej.Web.ComboBox();
            this.labelTitleCaption = new Wisej.Web.Label();
            this.textTitle = new Wisej.Web.TextBox();
            this.labelAssigneeCaption = new Wisej.Web.Label();
            this.textAssignee = new Wisej.Web.TextBox();
            this.labelStatusCaption = new Wisej.Web.Label();
            this.comboStatus = new Wisej.Web.ComboBox();
            this.labelDueCaption = new Wisej.Web.Label();
            this.dateDue = new Wisej.Web.DateTimePicker();
            this.labelCostCaption = new Wisej.Web.Label();
            this.numericCost = new Wisej.Web.NumericUpDown();
            this.labelHoursCaption = new Wisej.Web.Label();
            this.numericHours = new Wisej.Web.NumericUpDown();
            this.labelPriorityCaption = new Wisej.Web.Label();
            this.comboPriority = new Wisej.Web.ComboBox();
            this.buttonSave = new Wisej.Web.Button();
            this.buttonNew = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.panelSummary = new Wisej.Web.Panel();
            this.labelSummaryTitle = new Wisej.Web.Label();
            this.labelSummaryList = new Wisej.Web.Label();
            this.progressTests = new Wisej.Web.ProgressBar();
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonRunTests = new Wisej.Web.Button();
            this.buttonEmptyTitle = new Wisej.Web.Button();
            this.buttonHoursRange = new Wisej.Web.Button();
            this.buttonEditClosed = new Wisej.Web.Button();
            this.buttonBypass = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerTests = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelSummary.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Work Orders: grid + editor + summary panel — display and input only)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelCount);
            this.panelScreen.Controls.Add(this.gridOrders);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.labelRoleCaption);
            this.panelScreen.Controls.Add(this.comboRole);
            this.panelScreen.Controls.Add(this.labelTitleCaption);
            this.panelScreen.Controls.Add(this.textTitle);
            this.panelScreen.Controls.Add(this.labelAssigneeCaption);
            this.panelScreen.Controls.Add(this.textAssignee);
            this.panelScreen.Controls.Add(this.labelStatusCaption);
            this.panelScreen.Controls.Add(this.comboStatus);
            this.panelScreen.Controls.Add(this.labelDueCaption);
            this.panelScreen.Controls.Add(this.dateDue);
            this.panelScreen.Controls.Add(this.labelCostCaption);
            this.panelScreen.Controls.Add(this.numericCost);
            this.panelScreen.Controls.Add(this.labelHoursCaption);
            this.panelScreen.Controls.Add(this.numericHours);
            this.panelScreen.Controls.Add(this.labelPriorityCaption);
            this.panelScreen.Controls.Add(this.comboPriority);
            this.panelScreen.Controls.Add(this.buttonSave);
            this.panelScreen.Controls.Add(this.buttonNew);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Controls.Add(this.panelSummary);
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
            this.labelScreenTitle.Text = "Work Orders";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelCount
            //
            this.labelCount.AutoSize = false;
            this.labelCount.Font = new System.Drawing.Font("default", 9F);
            this.labelCount.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCount.Location = new System.Drawing.Point(24, 48);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(300, 20);
            this.labelCount.Text = "loading…";
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnAssignee,
                this.columnStatus,
                this.columnDue,
                this.columnCost,
                this.columnHours,
                this.columnVersion});
            this.gridOrders.Location = new System.Drawing.Point(24, 78);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(712, 152);
            this.gridOrders.SelectionChanged += new System.EventHandler(this.gridOrders_SelectionChanged);
            //
            // columns
            //
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 60;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 220;
            this.columnAssignee.HeaderText = "Assignee";
            this.columnAssignee.Name = "columnAssignee";
            this.columnAssignee.ReadOnly = true;
            this.columnAssignee.Width = 90;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 92;
            this.columnDue.HeaderText = "Due";
            this.columnDue.Name = "columnDue";
            this.columnDue.ReadOnly = true;
            this.columnDue.Width = 94;
            this.columnCost.HeaderText = "Cost";
            this.columnCost.Name = "columnCost";
            this.columnCost.ReadOnly = true;
            this.columnCost.Width = 76;
            this.columnHours.HeaderText = "Hours";
            this.columnHours.Name = "columnHours";
            this.columnHours.ReadOnly = true;
            this.columnHours.Width = 54;
            this.columnVersion.HeaderText = "Ver";
            this.columnVersion.Name = "columnVersion";
            this.columnVersion.ReadOnly = true;
            this.columnVersion.Width = 44;
            //
            // labelSelected + "Acting as" role (the session's role — server-side, never part of the command)
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 240);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(420, 22);
            this.labelSelected.Text = "New work order";
            this.labelRoleCaption.AutoSize = false;
            this.labelRoleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelRoleCaption.Location = new System.Drawing.Point(506, 242);
            this.labelRoleCaption.Name = "labelRoleCaption";
            this.labelRoleCaption.Size = new System.Drawing.Size(90, 18);
            this.labelRoleCaption.Text = "Acting as";
            this.labelRoleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.comboRole.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboRole.Location = new System.Drawing.Point(604, 236);
            this.comboRole.Name = "comboRole";
            this.comboRole.Size = new System.Drawing.Size(132, 30);
            this.comboRole.ToolTipText = "Role-based restriction: the server reads the role from SessionContext. Technician cannot set cost above $2,500 or touch a closed order.";
            this.comboRole.SelectedIndexChanged += new System.EventHandler(this.comboRole_SelectedIndexChanged);
            //
            // editor row 1: Title · Assignee · Status   (30 px gap after each field for the ErrorProvider glyph)
            //
            this.labelTitleCaption.AutoSize = false;
            this.labelTitleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTitleCaption.Location = new System.Drawing.Point(24, 272);
            this.labelTitleCaption.Name = "labelTitleCaption";
            this.labelTitleCaption.Size = new System.Drawing.Size(200, 18);
            this.labelTitleCaption.Text = "Title";
            this.textTitle.Location = new System.Drawing.Point(24, 292);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(290, 30);
            this.textTitle.Watermark = "What needs doing?";
            this.labelAssigneeCaption.AutoSize = false;
            this.labelAssigneeCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelAssigneeCaption.Location = new System.Drawing.Point(344, 272);
            this.labelAssigneeCaption.Name = "labelAssigneeCaption";
            this.labelAssigneeCaption.Size = new System.Drawing.Size(170, 18);
            this.labelAssigneeCaption.Text = "Assignee";
            this.textAssignee.Location = new System.Drawing.Point(344, 292);
            this.textAssignee.Name = "textAssignee";
            this.textAssignee.Size = new System.Drawing.Size(170, 30);
            this.textAssignee.Watermark = "Technician";
            this.labelStatusCaption.AutoSize = false;
            this.labelStatusCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelStatusCaption.Location = new System.Drawing.Point(544, 272);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(170, 18);
            this.labelStatusCaption.Text = "Status";
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Location = new System.Drawing.Point(544, 292);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(170, 30);
            this.comboStatus.ToolTipText = "Legal moves live in Domain/WorkOrderTransitions; an illegal move is a summary error, not a field error.";
            //
            // editor row 2: Due date · Cost · Hours · Priority
            //
            this.labelDueCaption.AutoSize = false;
            this.labelDueCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelDueCaption.Location = new System.Drawing.Point(24, 332);
            this.labelDueCaption.Name = "labelDueCaption";
            this.labelDueCaption.Size = new System.Drawing.Size(140, 18);
            this.labelDueCaption.Text = "Due date";
            this.dateDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dateDue.Location = new System.Drawing.Point(24, 352);
            this.dateDue.Name = "dateDue";
            this.dateDue.Size = new System.Drawing.Size(140, 30);
            this.labelCostCaption.AutoSize = false;
            this.labelCostCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCostCaption.Location = new System.Drawing.Point(194, 332);
            this.labelCostCaption.Name = "labelCostCaption";
            this.labelCostCaption.Size = new System.Drawing.Size(120, 18);
            this.labelCostCaption.Text = "Estimated cost ($)";
            this.numericCost.DecimalPlaces = 2;
            this.numericCost.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericCost.Location = new System.Drawing.Point(194, 352);
            this.numericCost.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numericCost.Name = "numericCost";
            this.numericCost.Size = new System.Drawing.Size(120, 30);
            this.numericCost.ToolTipText = "The spin box allows up to 1,000,000 on purpose: the range rule ($0–$10,000) is enforced by the validator, not by the control.";
            this.labelHoursCaption.AutoSize = false;
            this.labelHoursCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelHoursCaption.Location = new System.Drawing.Point(344, 332);
            this.labelHoursCaption.Name = "labelHoursCaption";
            this.labelHoursCaption.Size = new System.Drawing.Size(110, 18);
            this.labelHoursCaption.Text = "Estimated hours";
            this.numericHours.DecimalPlaces = 1;
            this.numericHours.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numericHours.Location = new System.Drawing.Point(344, 352);
            this.numericHours.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numericHours.Name = "numericHours";
            this.numericHours.Size = new System.Drawing.Size(110, 30);
            this.labelPriorityCaption.AutoSize = false;
            this.labelPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelPriorityCaption.Location = new System.Drawing.Point(484, 332);
            this.labelPriorityCaption.Name = "labelPriorityCaption";
            this.labelPriorityCaption.Size = new System.Drawing.Size(110, 18);
            this.labelPriorityCaption.Text = "Priority";
            this.comboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboPriority.Location = new System.Drawing.Point(484, 352);
            this.comboPriority.Name = "comboPriority";
            this.comboPriority.Size = new System.Drawing.Size(110, 30);
            //
            // screen buttons (thin handlers → IWorkOrderService)
            //
            this.buttonSave.Location = new System.Drawing.Point(24, 398);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 36);
            this.buttonSave.Text = "Save";
            this.buttonSave.ToolTipText = "Success path: BuildSaveCommandFromEditor() → _validator.Validate (UX pre-check) → await _workOrders.SaveAsync(command, _session) → ShowSaveResult";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            this.buttonNew.Location = new System.Drawing.Point(184, 398);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(100, 36);
            this.buttonNew.Text = "New";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            this.buttonRefresh.Location = new System.Drawing.Point(294, 398);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(100, 36);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.ToolTipText = "Reload the grid from the store and discard the editor's unsaved values";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // panelSummary  (the error summary panel: every problem, field or workflow, in one place)
            //
            this.panelSummary.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelSummary.BackColor = System.Drawing.Color.FromArgb(253, 243, 243);
            this.panelSummary.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSummary.Controls.Add(this.labelSummaryTitle);
            this.panelSummary.Controls.Add(this.labelSummaryList);
            this.panelSummary.Location = new System.Drawing.Point(24, 444);
            this.panelSummary.Name = "panelSummary";
            this.panelSummary.Size = new System.Drawing.Size(712, 96);
            this.panelSummary.Visible = false;
            this.labelSummaryTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelSummaryTitle.AutoSize = false;
            this.labelSummaryTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryTitle.ForeColor = System.Drawing.Color.FromArgb(156, 47, 47);
            this.labelSummaryTitle.Location = new System.Drawing.Point(12, 6);
            this.labelSummaryTitle.Name = "labelSummaryTitle";
            this.labelSummaryTitle.Size = new System.Drawing.Size(684, 22);
            this.labelSummaryTitle.Text = "! 3 problems need attention";
            this.labelSummaryList.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelSummaryList.AutoSize = false;
            this.labelSummaryList.Font = new System.Drawing.Font("default", 9F);
            this.labelSummaryList.ForeColor = System.Drawing.Color.FromArgb(124, 42, 42);
            this.labelSummaryList.Location = new System.Drawing.Point(12, 30);
            this.labelSummaryList.Name = "labelSummaryList";
            this.labelSummaryList.Size = new System.Drawing.Size(684, 60);
            this.labelSummaryList.Text = "";
            this.labelSummaryList.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // progressTests  (progress path: one test case per Timer tick)
            //
            this.progressTests.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.progressTests.Location = new System.Drawing.Point(24, 546);
            this.progressTests.Maximum = 15;
            this.progressTests.Name = "progressTests";
            this.progressTests.Size = new System.Drawing.Size(712, 10);
            this.progressTests.Visible = false;
            //
            // errorProvider  (field-level errors: glyph + tooltip beside the offending control)
            //
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Service → Domain → Data";
            //
            // panelActions  (bottom bar: progress / failures / bypass / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonRunTests);
            this.panelActions.Controls.Add(this.buttonEmptyTitle);
            this.panelActions.Controls.Add(this.buttonHoursRange);
            this.panelActions.Controls.Add(this.buttonEditClosed);
            this.panelActions.Controls.Add(this.buttonBypass);
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
            this.buttonRunTests.Text = "▶ Run 15 test cases";
            this.buttonRunTests.ToolTipText = "Progress path: a Timer runs the documented cases (docs/TestCases.md) through WorkOrderValidator and WorkOrderRules — PASS/FAIL per case in the trace";
            this.buttonRunTests.Click += new System.EventHandler(this.buttonRunTests_Click);
            this.buttonEmptyTitle.Location = new System.Drawing.Point(200, 4);
            this.buttonEmptyTitle.Name = "buttonEmptyTitle";
            this.buttonEmptyTitle.Size = new System.Drawing.Size(170, 36);
            this.buttonEmptyTitle.Text = "Save with empty title";
            this.buttonEmptyTitle.ToolTipText = "Failure path 1 (validation): the command skips the pre-check and the server's validator rejects it — glyph on Title, summary panel, nothing persisted";
            this.buttonEmptyTitle.Click += new System.EventHandler(this.buttonEmptyTitle_Click);
            this.buttonHoursRange.Location = new System.Drawing.Point(380, 4);
            this.buttonHoursRange.Name = "buttonHoursRange";
            this.buttonHoursRange.Size = new System.Drawing.Size(170, 36);
            this.buttonHoursRange.Text = "Save 1,200 hours";
            this.buttonHoursRange.ToolTipText = "Failure path 2 (range): hours must be 0–999; the spin box accepted it, the rule did not";
            this.buttonHoursRange.Click += new System.EventHandler(this.buttonHoursRange_Click);
            this.buttonEditClosed.Location = new System.Drawing.Point(560, 4);
            this.buttonEditClosed.Name = "buttonEditClosed";
            this.buttonEditClosed.Size = new System.Drawing.Size(170, 36);
            this.buttonEditClosed.Text = "Edit closed #2006";
            this.buttonEditClosed.ToolTipText = "Failure path 3 (business rule): every field is valid; WorkOrderRules rejects editing a Closed order — a summary error, no field glyph";
            this.buttonEditClosed.Click += new System.EventHandler(this.buttonEditClosed_Click);
            this.buttonBypass.Location = new System.Drawing.Point(740, 4);
            this.buttonBypass.Name = "buttonBypass";
            this.buttonBypass.Size = new System.Drawing.Size(200, 36);
            this.buttonBypass.Text = "Bypass: crafted command";
            this.buttonBypass.ToolTipText = "UI bypass (role rule): a command built without the editor sets cost 9,500 on #2002. Validator passes; the server's role check rejects a Technician. Switch Acting as → Supervisor and it saves.";
            this.buttonBypass.Click += new System.EventHandler(this.buttonBypass_Click);
            this.buttonOutage.Location = new System.Drawing.Point(950, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(190, 36);
            this.buttonOutage.Text = "Simulate write outage";
            this.buttonOutage.ToolTipText = "Error path: the COMMIT fails after validation passed; the transaction rolls back, the log gets the details, the user gets a safe message and keeps the edits. Click again to recover and retry.";
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
            this.timerTests.Interval = 220;
            this.timerTests.Tick += new System.EventHandler(this.timerTests_Tick);
            //
            // WorkOrderEditor
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "WorkOrderEditor";
            this.Text = "TicketOps Console — Module 5 · Validation, error UX & safe save pipelines";
            this.Load += new System.EventHandler(this.WorkOrderEditor_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelSummary.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelCount;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnAssignee;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnDue;
        private Wisej.Web.DataGridViewTextBoxColumn columnCost;
        private Wisej.Web.DataGridViewTextBoxColumn columnHours;
        private Wisej.Web.DataGridViewTextBoxColumn columnVersion;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.Label labelRoleCaption;
        private Wisej.Web.ComboBox comboRole;
        private Wisej.Web.Label labelTitleCaption;
        private Wisej.Web.TextBox textTitle;
        private Wisej.Web.Label labelAssigneeCaption;
        private Wisej.Web.TextBox textAssignee;
        private Wisej.Web.Label labelStatusCaption;
        private Wisej.Web.ComboBox comboStatus;
        private Wisej.Web.Label labelDueCaption;
        private Wisej.Web.DateTimePicker dateDue;
        private Wisej.Web.Label labelCostCaption;
        private Wisej.Web.NumericUpDown numericCost;
        private Wisej.Web.Label labelHoursCaption;
        private Wisej.Web.NumericUpDown numericHours;
        private Wisej.Web.Label labelPriorityCaption;
        private Wisej.Web.ComboBox comboPriority;
        private Wisej.Web.Button buttonSave;
        private Wisej.Web.Button buttonNew;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Panel panelSummary;
        private Wisej.Web.Label labelSummaryTitle;
        private Wisej.Web.Label labelSummaryList;
        private Wisej.Web.ProgressBar progressTests;
        private Wisej.Web.ErrorProvider errorProvider;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonRunTests;
        private Wisej.Web.Button buttonEmptyTitle;
        private Wisej.Web.Button buttonHoursRange;
        private Wisej.Web.Button buttonEditClosed;
        private Wisej.Web.Button buttonBypass;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerTests;
    }
}
