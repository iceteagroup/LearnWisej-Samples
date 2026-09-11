namespace TicketOps.Views
{
    partial class OperationsDashboard
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
            this.labelSummary = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelThemeCaption = new Wisej.Web.Label();
            this.comboTheme = new Wisej.Web.ComboBox();
            this.labelCultureCaption = new Wisej.Web.Label();
            this.comboCulture = new Wisej.Web.ComboBox();
            this.panelKpiOpen = new Wisej.Web.Panel();
            this.labelCountOpen = new Wisej.Web.Label();
            this.chipOpen = new TicketOps.Controls.StatusChip();
            this.panelKpiInProgress = new Wisej.Web.Panel();
            this.labelCountInProgress = new Wisej.Web.Label();
            this.chipInProgress = new TicketOps.Controls.StatusChip();
            this.panelKpiBlocked = new Wisej.Web.Panel();
            this.labelCountBlocked = new Wisej.Web.Label();
            this.chipBlocked = new TicketOps.Controls.StatusChip();
            this.panelKpiDone = new Wisej.Web.Panel();
            this.labelCountDone = new Wisej.Web.Label();
            this.chipDone = new TicketOps.Controls.StatusChip();
            this.gridWorkOrders = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnDue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnCost = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.chipSelected = new TicketOps.Controls.StatusChip();
            this.labelDueValue = new Wisej.Web.Label();
            this.labelCostValue = new Wisej.Web.Label();
            this.labelCreatedValue = new Wisej.Web.Label();
            this.labelHoursValue = new Wisej.Web.Label();
            this.buttonOpenDetail = new Wisej.Web.Button();
            this.buttonNextStatus = new Wisej.Web.Button();
            this.labelFooter = new Wisej.Web.Label();
            this.panelScreen.SuspendLayout();
            this.panelKpiOpen.SuspendLayout();
            this.panelKpiInProgress.SuspendLayout();
            this.panelKpiBlocked.SuspendLayout();
            this.panelKpiDone.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (NO BackColor: the theme paints the card)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.labelSummary);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelThemeCaption);
            this.panelScreen.Controls.Add(this.comboTheme);
            this.panelScreen.Controls.Add(this.labelCultureCaption);
            this.panelScreen.Controls.Add(this.comboCulture);
            this.panelScreen.Controls.Add(this.panelKpiOpen);
            this.panelScreen.Controls.Add(this.panelKpiInProgress);
            this.panelScreen.Controls.Add(this.panelKpiBlocked);
            this.panelScreen.Controls.Add(this.panelKpiDone);
            this.panelScreen.Controls.Add(this.gridWorkOrders);
            this.panelScreen.Controls.Add(this.labelSelected);
            this.panelScreen.Controls.Add(this.chipSelected);
            this.panelScreen.Controls.Add(this.labelDueValue);
            this.panelScreen.Controls.Add(this.labelCostValue);
            this.panelScreen.Controls.Add(this.labelCreatedValue);
            this.panelScreen.Controls.Add(this.labelHoursValue);
            this.panelScreen.Controls.Add(this.buttonOpenDetail);
            this.panelScreen.Controls.Add(this.buttonNextStatus);
            this.panelScreen.Controls.Add(this.labelFooter);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 560);
            //
            // labelScreenTitle  (text from Resources: Dashboard.Title)
            //
            this.labelScreenTitle.AutoSize = false;
            this.labelScreenTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelScreenTitle.Location = new System.Drawing.Point(24, 18);
            this.labelScreenTitle.Name = "labelScreenTitle";
            this.labelScreenTitle.Size = new System.Drawing.Size(330, 30);
            this.labelScreenTitle.Text = "Operations Dashboard";
            //
            // labelSummary
            //
            this.labelSummary.AutoSize = false;
            this.labelSummary.Font = new System.Drawing.Font("default", 9F);
            this.labelSummary.Location = new System.Drawing.Point(24, 48);
            this.labelSummary.Name = "labelSummary";
            this.labelSummary.Size = new System.Drawing.Size(330, 20);
            this.labelSummary.Text = "";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // header: theme picker
            //
            this.labelThemeCaption.AutoSize = false;
            this.labelThemeCaption.Location = new System.Drawing.Point(24, 92);
            this.labelThemeCaption.Name = "labelThemeCaption";
            this.labelThemeCaption.Size = new System.Drawing.Size(60, 22);
            this.labelThemeCaption.Text = "Theme";
            this.comboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTheme.Location = new System.Drawing.Point(86, 88);
            this.comboTheme.Name = "comboTheme";
            this.comboTheme.Size = new System.Drawing.Size(210, 28);
            this.comboTheme.SelectedIndexChanged += new System.EventHandler(this.comboTheme_SelectedIndexChanged);
            //
            // header: culture picker
            //
            this.labelCultureCaption.AutoSize = false;
            this.labelCultureCaption.Location = new System.Drawing.Point(320, 92);
            this.labelCultureCaption.Name = "labelCultureCaption";
            this.labelCultureCaption.Size = new System.Drawing.Size(64, 22);
            this.labelCultureCaption.Text = "Culture";
            this.comboCulture.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboCulture.Items.AddRange(new object[] {
                "English (United States)",
                "Deutsch (Deutschland)",
                "Italiano (Italia)"});
            this.comboCulture.Location = new System.Drawing.Point(386, 88);
            this.comboCulture.Name = "comboCulture";
            this.comboCulture.Size = new System.Drawing.Size(210, 28);
            this.comboCulture.SelectedIndexChanged += new System.EventHandler(this.comboCulture_SelectedIndexChanged);
            //
            // KPI cards: one StatusChip each (the same control, set — never styled — per status)
            //
            this.panelKpiOpen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiOpen.Controls.Add(this.labelCountOpen);
            this.panelKpiOpen.Controls.Add(this.chipOpen);
            this.panelKpiOpen.Location = new System.Drawing.Point(24, 126);
            this.panelKpiOpen.Name = "panelKpiOpen";
            this.panelKpiOpen.Size = new System.Drawing.Size(166, 90);
            this.labelCountOpen.AutoSize = false;
            this.labelCountOpen.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.labelCountOpen.Location = new System.Drawing.Point(12, 6);
            this.labelCountOpen.Name = "labelCountOpen";
            this.labelCountOpen.Size = new System.Drawing.Size(80, 40);
            this.labelCountOpen.Text = "–";
            this.chipOpen.Location = new System.Drawing.Point(12, 54);
            this.chipOpen.Name = "chipOpen";
            this.chipOpen.Size = new System.Drawing.Size(132, 26);
            this.chipOpen.Status = TicketOps.Domain.WorkOrderStatus.Open;
            this.chipOpen.Text = "Open";

            this.panelKpiInProgress.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiInProgress.Controls.Add(this.labelCountInProgress);
            this.panelKpiInProgress.Controls.Add(this.chipInProgress);
            this.panelKpiInProgress.Location = new System.Drawing.Point(206, 126);
            this.panelKpiInProgress.Name = "panelKpiInProgress";
            this.panelKpiInProgress.Size = new System.Drawing.Size(166, 90);
            this.labelCountInProgress.AutoSize = false;
            this.labelCountInProgress.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.labelCountInProgress.Location = new System.Drawing.Point(12, 6);
            this.labelCountInProgress.Name = "labelCountInProgress";
            this.labelCountInProgress.Size = new System.Drawing.Size(80, 40);
            this.labelCountInProgress.Text = "–";
            this.chipInProgress.Location = new System.Drawing.Point(12, 54);
            this.chipInProgress.Name = "chipInProgress";
            this.chipInProgress.Size = new System.Drawing.Size(132, 26);
            this.chipInProgress.Status = TicketOps.Domain.WorkOrderStatus.InProgress;
            this.chipInProgress.Text = "In progress";

            this.panelKpiBlocked.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiBlocked.Controls.Add(this.labelCountBlocked);
            this.panelKpiBlocked.Controls.Add(this.chipBlocked);
            this.panelKpiBlocked.Location = new System.Drawing.Point(388, 126);
            this.panelKpiBlocked.Name = "panelKpiBlocked";
            this.panelKpiBlocked.Size = new System.Drawing.Size(166, 90);
            this.labelCountBlocked.AutoSize = false;
            this.labelCountBlocked.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.labelCountBlocked.Location = new System.Drawing.Point(12, 6);
            this.labelCountBlocked.Name = "labelCountBlocked";
            this.labelCountBlocked.Size = new System.Drawing.Size(80, 40);
            this.labelCountBlocked.Text = "–";
            this.chipBlocked.Location = new System.Drawing.Point(12, 54);
            this.chipBlocked.Name = "chipBlocked";
            this.chipBlocked.Size = new System.Drawing.Size(132, 26);
            this.chipBlocked.Status = TicketOps.Domain.WorkOrderStatus.Blocked;
            this.chipBlocked.Text = "Blocked";

            this.panelKpiDone.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelKpiDone.Controls.Add(this.labelCountDone);
            this.panelKpiDone.Controls.Add(this.chipDone);
            this.panelKpiDone.Location = new System.Drawing.Point(570, 126);
            this.panelKpiDone.Name = "panelKpiDone";
            this.panelKpiDone.Size = new System.Drawing.Size(166, 90);
            this.labelCountDone.AutoSize = false;
            this.labelCountDone.Font = new System.Drawing.Font("default", 22F, System.Drawing.FontStyle.Bold);
            this.labelCountDone.Location = new System.Drawing.Point(12, 6);
            this.labelCountDone.Name = "labelCountDone";
            this.labelCountDone.Size = new System.Drawing.Size(80, 40);
            this.labelCountDone.Text = "–";
            this.chipDone.Location = new System.Drawing.Point(12, 54);
            this.chipDone.Name = "chipDone";
            this.chipDone.Size = new System.Drawing.Size(132, 26);
            this.chipDone.Status = TicketOps.Domain.WorkOrderStatus.Done;
            this.chipDone.Text = "Done";
            //
            // gridWorkOrders  (headers from Resources: Column.*; cells formatted by the session's culture)
            //
            this.gridWorkOrders.AllowUserToAddRows = false;
            this.gridWorkOrders.AllowUserToDeleteRows = false;
            this.gridWorkOrders.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridWorkOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnStatus,
                this.columnDue,
                this.columnCost});
            this.gridWorkOrders.Location = new System.Drawing.Point(24, 228);
            this.gridWorkOrders.MultiSelect = false;
            this.gridWorkOrders.Name = "gridWorkOrders";
            this.gridWorkOrders.ReadOnly = true;
            this.gridWorkOrders.RowHeadersVisible = false;
            this.gridWorkOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridWorkOrders.Size = new System.Drawing.Size(712, 176);
            this.gridWorkOrders.SelectionChanged += new System.EventHandler(this.gridWorkOrders_SelectionChanged);
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 70;
            this.columnTitle.HeaderText = "Work order";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 290;
            this.columnStatus.HeaderText = "Status";
            this.columnStatus.Name = "columnStatus";
            this.columnStatus.ReadOnly = true;
            this.columnStatus.Width = 120;
            this.columnDue.HeaderText = "Due";
            this.columnDue.Name = "columnDue";
            this.columnDue.ReadOnly = true;
            this.columnDue.Width = 100;
            this.columnCost.HeaderText = "Cost";
            this.columnCost.Name = "columnCost";
            this.columnCost.ReadOnly = true;
            this.columnCost.Width = 110;
            //
            // detail strip (the selected work order, formatted for the culture; the same StatusChip again)
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.labelSelected.Location = new System.Drawing.Point(24, 414);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(200, 26);
            this.labelSelected.Text = "";
            this.chipSelected.Location = new System.Drawing.Point(230, 414);
            this.chipSelected.Name = "chipSelected";
            this.chipSelected.Size = new System.Drawing.Size(132, 26);
            this.labelDueValue.AutoSize = false;
            this.labelDueValue.Location = new System.Drawing.Point(380, 416);
            this.labelDueValue.Name = "labelDueValue";
            this.labelDueValue.Size = new System.Drawing.Size(170, 22);
            this.labelDueValue.Text = "";
            this.labelCostValue.AutoSize = false;
            this.labelCostValue.Location = new System.Drawing.Point(556, 416);
            this.labelCostValue.Name = "labelCostValue";
            this.labelCostValue.Size = new System.Drawing.Size(180, 22);
            this.labelCostValue.Text = "";
            this.labelCreatedValue.AutoSize = false;
            this.labelCreatedValue.Location = new System.Drawing.Point(24, 444);
            this.labelCreatedValue.Name = "labelCreatedValue";
            this.labelCreatedValue.Size = new System.Drawing.Size(340, 22);
            this.labelCreatedValue.Text = "";
            this.labelHoursValue.AutoSize = false;
            this.labelHoursValue.Location = new System.Drawing.Point(380, 444);
            this.labelHoursValue.Name = "labelHoursValue";
            this.labelHoursValue.Size = new System.Drawing.Size(170, 22);
            this.labelHoursValue.Text = "";
            //
            // buttons
            //
            this.buttonOpenDetail.Location = new System.Drawing.Point(24, 476);
            this.buttonOpenDetail.Name = "buttonOpenDetail";
            this.buttonOpenDetail.Size = new System.Drawing.Size(170, 36);
            this.buttonOpenDetail.Text = "Open detail…";
            this.buttonOpenDetail.Click += new System.EventHandler(this.buttonOpenDetail_Click);
            this.buttonNextStatus.Location = new System.Drawing.Point(204, 476);
            this.buttonNextStatus.Name = "buttonNextStatus";
            this.buttonNextStatus.Size = new System.Drawing.Size(170, 36);
            this.buttonNextStatus.Text = "Next status";
            this.buttonNextStatus.Click += new System.EventHandler(this.buttonNextStatus_Click);
            //
            // labelFooter  (status line: "Theme: … — applied to every screen." / "Culture: … — labels, dates and currency follow the culture.")
            //
            this.labelFooter.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelFooter.AutoSize = false;
            this.labelFooter.Font = new System.Drawing.Font("default", 9F);
            this.labelFooter.Location = new System.Drawing.Point(24, 524);
            this.labelFooter.Name = "labelFooter";
            this.labelFooter.Size = new System.Drawing.Size(712, 22);
            this.labelFooter.Text = "";
            //
            // OperationsDashboard  (NO BackColor: the theme owns every surface)
            //
            this.ClientSize = new System.Drawing.Size(820, 620);
            this.Controls.Add(this.panelScreen);
            this.Name = "OperationsDashboard";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.OperationsDashboard_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelKpiOpen.ResumeLayout(false);
            this.panelKpiInProgress.ResumeLayout(false);
            this.panelKpiBlocked.ResumeLayout(false);
            this.panelKpiDone.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private Wisej.Web.Label labelSummary;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelThemeCaption;
        private Wisej.Web.ComboBox comboTheme;
        private Wisej.Web.Label labelCultureCaption;
        private Wisej.Web.ComboBox comboCulture;
        private Wisej.Web.Panel panelKpiOpen;
        private Wisej.Web.Label labelCountOpen;
        private TicketOps.Controls.StatusChip chipOpen;
        private Wisej.Web.Panel panelKpiInProgress;
        private Wisej.Web.Label labelCountInProgress;
        private TicketOps.Controls.StatusChip chipInProgress;
        private Wisej.Web.Panel panelKpiBlocked;
        private Wisej.Web.Label labelCountBlocked;
        private TicketOps.Controls.StatusChip chipBlocked;
        private Wisej.Web.Panel panelKpiDone;
        private Wisej.Web.Label labelCountDone;
        private TicketOps.Controls.StatusChip chipDone;
        private Wisej.Web.DataGridView gridWorkOrders;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnStatus;
        private Wisej.Web.DataGridViewTextBoxColumn columnDue;
        private Wisej.Web.DataGridViewTextBoxColumn columnCost;
        private Wisej.Web.Label labelSelected;
        private TicketOps.Controls.StatusChip chipSelected;
        private Wisej.Web.Label labelDueValue;
        private Wisej.Web.Label labelCostValue;
        private Wisej.Web.Label labelCreatedValue;
        private Wisej.Web.Label labelHoursValue;
        private Wisej.Web.Button buttonOpenDetail;
        private Wisej.Web.Button buttonNextStatus;
        private Wisej.Web.Label labelFooter;
    }
}
