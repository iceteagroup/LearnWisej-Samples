namespace TicketOps.Views
{
    partial class TicketEditor
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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelCount = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnHours = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.labelTitleCaption = new Wisej.Web.Label();
            this.textTitle = new Wisej.Web.TextBox();
            this.labelPriorityCaption = new Wisej.Web.Label();
            this.comboPriority = new Wisej.Web.ComboBox();
            this.labelHoursCaption = new Wisej.Web.Label();
            this.numericHours = new Wisej.Web.NumericUpDown();
            this.buttonSave = new Wisej.Web.Button();
            this.buttonClose = new Wisej.Web.Button();
            this.buttonNew = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
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
            this.panelScreen.Controls.Add(this.gridTickets);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.labelTitleCaption);
            this.panelScreen.Controls.Add(this.textTitle);
            this.panelScreen.Controls.Add(this.labelPriorityCaption);
            this.panelScreen.Controls.Add(this.comboPriority);
            this.panelScreen.Controls.Add(this.labelHoursCaption);
            this.panelScreen.Controls.Add(this.numericHours);
            this.panelScreen.Controls.Add(this.buttonSave);
            this.panelScreen.Controls.Add(this.buttonClose);
            this.panelScreen.Controls.Add(this.buttonNew);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 492);
            //
            // labelScreenTitle
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(300, 30);
            this.labelScreenTitle.Text = "Open Tickets";
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
                this.columnHours});
            this.gridTickets.Location = new System.Drawing.Point(24, 92);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 226);
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            //
            // columns
            //
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 70;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 340;
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 90;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 110;
            this.columnHours.HeaderText = "Hours";
            this.columnHours.Name = "columnHours";
            this.columnHours.ReadOnly = true;
            this.columnHours.Width = 80;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 332);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(400, 22);
            this.labelSelected.Text = "New ticket";
            //
            // editor fields
            //
            this.labelTitleCaption.AutoSize = false;
            this.labelTitleCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTitleCaption.Location = new System.Drawing.Point(24, 362);
            this.labelTitleCaption.Name = "labelTitleCaption";
            this.labelTitleCaption.Size = new System.Drawing.Size(200, 18);
            this.labelTitleCaption.Text = "Title";
            this.textTitle.Location = new System.Drawing.Point(24, 382);
            this.textTitle.Name = "textTitle";
            this.textTitle.Size = new System.Drawing.Size(420, 30);
            this.textTitle.Watermark = "What is wrong?";
            this.labelPriorityCaption.AutoSize = false;
            this.labelPriorityCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelPriorityCaption.Location = new System.Drawing.Point(460, 362);
            this.labelPriorityCaption.Name = "labelPriorityCaption";
            this.labelPriorityCaption.Size = new System.Drawing.Size(130, 18);
            this.labelPriorityCaption.Text = "Priority";
            this.comboPriority.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboPriority.Location = new System.Drawing.Point(460, 382);
            this.comboPriority.Name = "comboPriority";
            this.comboPriority.Size = new System.Drawing.Size(130, 30);
            this.labelHoursCaption.AutoSize = false;
            this.labelHoursCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelHoursCaption.Location = new System.Drawing.Point(606, 362);
            this.labelHoursCaption.Name = "labelHoursCaption";
            this.labelHoursCaption.Size = new System.Drawing.Size(130, 18);
            this.labelHoursCaption.Text = "Hours logged";
            this.numericHours.DecimalPlaces = 2;
            this.numericHours.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            this.numericHours.Location = new System.Drawing.Point(606, 382);
            this.numericHours.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.numericHours.Name = "numericHours";
            this.numericHours.Size = new System.Drawing.Size(130, 30);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(24, 432);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(150, 36);
            this.buttonSave.Text = "Save ticket";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // buttonClose
            //
            this.buttonClose.Location = new System.Drawing.Point(184, 432);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(150, 36);
            this.buttonClose.Text = "Close ticket";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            //
            // buttonNew
            //
            this.buttonNew.Location = new System.Drawing.Point(344, 432);
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(120, 36);
            this.buttonNew.Text = "New";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(474, 432);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(120, 36);
            this.buttonRefresh.Text = "↻ Refresh";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // TicketEditor
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 552);
            this.Controls.Add(this.panelScreen);
            this.Name = "TicketEditor";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.TicketEditor_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelCount;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnHours;
        private Wisej.Web.Label labelSelected;
        private Wisej.Web.Label labelTitleCaption;
        private Wisej.Web.TextBox textTitle;
        private Wisej.Web.Label labelPriorityCaption;
        private Wisej.Web.ComboBox comboPriority;
        private Wisej.Web.Label labelHoursCaption;
        private Wisej.Web.NumericUpDown numericHours;
        private Wisej.Web.Button buttonSave;
        private Wisej.Web.Button buttonClose;
        private Wisej.Web.Button buttonNew;
        private Wisej.Web.Button buttonRefresh;
    }
}
