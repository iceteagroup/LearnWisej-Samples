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
            this.errorProvider = new Wisej.Web.ErrorProvider(this.components);
            this.panelScreen.SuspendLayout();
            this.panelSummary.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
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
            // statusBanner
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
                this.columnHours});
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
            this.columnTitle.Width = 240;
            this.columnAssignee.HeaderText = "Assignee";
            this.columnAssignee.Name = "columnAssignee";
            this.columnAssignee.ReadOnly = true;
            this.columnAssignee.Width = 100;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 96;
            this.columnDue.HeaderText = "Due";
            this.columnDue.Name = "columnDue";
            this.columnDue.ReadOnly = true;
            this.columnDue.Width = 94;
            this.columnCost.HeaderText = "Cost";
            this.columnCost.Name = "columnCost";
            this.columnCost.ReadOnly = true;
            this.columnCost.Width = 78;
            this.columnHours.HeaderText = "Hours";
            this.columnHours.Name = "columnHours";
            this.columnHours.ReadOnly = true;
            this.columnHours.Width = 60;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 240);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(420, 22);
            this.labelSelected.Text = "New work order";
            //
            // labelRoleCaption
            //
            this.labelRoleCaption.AutoSize = false;
            this.labelRoleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelRoleCaption.Location = new System.Drawing.Point(506, 242);
            this.labelRoleCaption.Name = "labelRoleCaption";
            this.labelRoleCaption.Size = new System.Drawing.Size(90, 18);
            this.labelRoleCaption.Text = "Acting as";
            this.labelRoleCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // comboRole
            //
            this.comboRole.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboRole.Location = new System.Drawing.Point(604, 236);
            this.comboRole.Name = "comboRole";
            this.comboRole.Size = new System.Drawing.Size(132, 30);
            this.comboRole.SelectedIndexChanged += new System.EventHandler(this.comboRole_SelectedIndexChanged);
            //
            // labelTitleCaption
            //
            this.labelTitleCaption.AutoSize = false;
            this.labelTitleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTitleCaption.Location = new System.Drawing.Point(24, 272);
            this.labelTitleCaption.Name = "labelTitleCaption";
            this.labelTitleCaption.Size = new System.Drawing.Size(200, 18);
            this.labelTitleCaption.Text = "Title";
            //
            // textTitle
            //
            this.textTitle.Location = new System.Drawing.Point(24, 292);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(290, 30);
            this.textTitle.Watermark = "What needs doing?";
            //
            // labelAssigneeCaption
            //
            this.labelAssigneeCaption.AutoSize = false;
            this.labelAssigneeCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelAssigneeCaption.Location = new System.Drawing.Point(344, 272);
            this.labelAssigneeCaption.Name = "labelAssigneeCaption";
            this.labelAssigneeCaption.Size = new System.Drawing.Size(170, 18);
            this.labelAssigneeCaption.Text = "Assignee";
            //
            // textAssignee
            //
            this.textAssignee.Location = new System.Drawing.Point(344, 292);
            this.textAssignee.Name = "textAssignee";
            this.textAssignee.Size = new System.Drawing.Size(170, 30);
            this.textAssignee.Watermark = "Technician";
            //
            // labelStatusCaption
            //
            this.labelStatusCaption.AutoSize = false;
            this.labelStatusCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelStatusCaption.Location = new System.Drawing.Point(544, 272);
            this.labelStatusCaption.Name = "labelStatusCaption";
            this.labelStatusCaption.Size = new System.Drawing.Size(170, 18);
            this.labelStatusCaption.Text = "Status";
            //
            // comboStatus
            //
            this.comboStatus.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboStatus.Location = new System.Drawing.Point(544, 292);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(170, 30);
            //
            // labelDueCaption
            //
            this.labelDueCaption.AutoSize = false;
            this.labelDueCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelDueCaption.Location = new System.Drawing.Point(24, 332);
            this.labelDueCaption.Name = "labelDueCaption";
            this.labelDueCaption.Size = new System.Drawing.Size(140, 18);
            this.labelDueCaption.Text = "Due date";
            //
            // dateDue
            //
            this.dateDue.Format = Wisej.Web.DateTimePickerFormat.Short;
            this.dateDue.Location = new System.Drawing.Point(24, 352);
            this.dateDue.Name = "dateDue";
            this.dateDue.Size = new System.Drawing.Size(140, 30);
            //
            // labelCostCaption
            //
            this.labelCostCaption.AutoSize = false;
            this.labelCostCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelCostCaption.Location = new System.Drawing.Point(194, 332);
            this.labelCostCaption.Name = "labelCostCaption";
            this.labelCostCaption.Size = new System.Drawing.Size(120, 18);
            this.labelCostCaption.Text = "Estimated cost ($)";
            //
            // numericCost
            //
            this.numericCost.DecimalPlaces = 2;
            this.numericCost.Increment = new decimal(new int[] { 50, 0, 0, 0 });
            this.numericCost.Location = new System.Drawing.Point(194, 352);
            this.numericCost.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
            this.numericCost.Name = "numericCost";
            this.numericCost.Size = new System.Drawing.Size(120, 30);
            //
            // labelHoursCaption
            //
            this.labelHoursCaption.AutoSize = false;
            this.labelHoursCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelHoursCaption.Location = new System.Drawing.Point(344, 332);
            this.labelHoursCaption.Name = "labelHoursCaption";
            this.labelHoursCaption.Size = new System.Drawing.Size(110, 18);
            this.labelHoursCaption.Text = "Estimated hours";
            //
            // numericHours
            //
            this.numericHours.DecimalPlaces = 1;
            this.numericHours.Increment = new decimal(new int[] { 5, 0, 0, 65536 });
            this.numericHours.Location = new System.Drawing.Point(344, 352);
            this.numericHours.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numericHours.Name = "numericHours";
            this.numericHours.Size = new System.Drawing.Size(110, 30);
            //
            // labelPriorityCaption
            //
            this.labelPriorityCaption.AutoSize = false;
            this.labelPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelPriorityCaption.Location = new System.Drawing.Point(484, 332);
            this.labelPriorityCaption.Name = "labelPriorityCaption";
            this.labelPriorityCaption.Size = new System.Drawing.Size(110, 18);
            this.labelPriorityCaption.Text = "Priority";
            //
            // comboPriority
            //
            this.comboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboPriority.Location = new System.Drawing.Point(484, 352);
            this.comboPriority.Name = "comboPriority";
            this.comboPriority.Size = new System.Drawing.Size(110, 30);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(24, 398);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 36);
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // buttonNew
            //
            this.buttonNew.Location = new System.Drawing.Point(184, 398);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(100, 36);
            this.buttonNew.Text = "New";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(294, 398);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(100, 36);
            this.buttonRefresh.Text = "↻ Refresh";
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
            //
            // labelSummaryTitle
            //
            this.labelSummaryTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelSummaryTitle.AutoSize = false;
            this.labelSummaryTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSummaryTitle.ForeColor = System.Drawing.Color.FromArgb(156, 47, 47);
            this.labelSummaryTitle.Location = new System.Drawing.Point(12, 6);
            this.labelSummaryTitle.Name = "labelSummaryTitle";
            this.labelSummaryTitle.Size = new System.Drawing.Size(684, 22);
            this.labelSummaryTitle.Text = "! 3 problems need attention";
            //
            // labelSummaryList
            //
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
            // errorProvider  (field-level errors: glyph + tooltip beside the offending control)
            //
            this.errorProvider.BlinkStyle = Wisej.Web.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            //
            // WorkOrderEditor
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 620);
            this.Controls.Add(this.panelScreen);
            this.Name = "WorkOrderEditor";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.WorkOrderEditor_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelSummary.ResumeLayout(false);
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
        private Wisej.Web.ErrorProvider errorProvider;
    }
}
