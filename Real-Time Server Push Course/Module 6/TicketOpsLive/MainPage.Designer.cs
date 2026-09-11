namespace TicketOpsLive
{
    partial class MainPage
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
            this.ticketsBindingSource = new Wisej.Web.BindingSource();
            this.panelBoard = new Wisej.Web.Panel();
            this.labelBoardTitle = new Wisej.Web.Label();
            this.labelTenantCaption = new Wisej.Web.Label();
            this.tenantComboBox = new Wisej.Web.ComboBox();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTenant = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.publishButton = new Wisej.Web.Button();
            this.escalateButton = new Wisej.Web.Button();
            this.panelNotifications = new Wisej.Web.Panel();
            this.labelNotificationsTitle = new Wisej.Web.Label();
            this.notificationCountLabel = new Wisej.Web.Label();
            this.notificationsList = new Wisej.Web.ListBox();
            this.subscribeButton = new Wisej.Web.Button();
            this.unsubscribeButton = new Wisej.Web.Button();
            this.panelBoard.SuspendLayout();
            this.panelNotifications.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBoard
            //
            this.panelBoard.BackColor = System.Drawing.Color.White;
            this.panelBoard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBoard.Controls.Add(this.labelBoardTitle);
            this.panelBoard.Controls.Add(this.labelTenantCaption);
            this.panelBoard.Controls.Add(this.tenantComboBox);
            this.panelBoard.Controls.Add(this.ticketsGrid);
            this.panelBoard.Controls.Add(this.publishButton);
            this.panelBoard.Controls.Add(this.escalateButton);
            this.panelBoard.Location = new System.Drawing.Point(20, 18);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(640, 334);
            //
            // labelBoardTitle
            //
            this.labelBoardTitle.AutoSize = false;
            this.labelBoardTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelBoardTitle.Location = new System.Drawing.Point(24, 12);
            this.labelBoardTitle.Name = "labelBoardTitle";
            this.labelBoardTitle.Size = new System.Drawing.Size(300, 28);
            this.labelBoardTitle.Text = "Tickets";
            //
            // labelTenantCaption
            //
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTenantCaption.Location = new System.Drawing.Point(380, 12);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(70, 28);
            this.labelTenantCaption.Text = "Tenant";
            this.labelTenantCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // tenantComboBox
            //
            this.tenantComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.tenantComboBox.Items.Add("Contoso");
            this.tenantComboBox.Items.Add("Northwind");
            this.tenantComboBox.Location = new System.Drawing.Point(456, 12);
            this.tenantComboBox.Name = "tenantComboBox";
            this.tenantComboBox.SelectedIndex = 0;
            this.tenantComboBox.Size = new System.Drawing.Size(160, 28);
            this.tenantComboBox.SelectedIndexChanged += new System.EventHandler(this.tenantComboBox_SelectedIndexChanged);
            //
            // ticketsGrid
            //
            this.ticketsGrid.AllowUserToAddRows = false;
            this.ticketsGrid.AllowUserToDeleteRows = false;
            this.ticketsGrid.AutoGenerateColumns = false;
            this.ticketsGrid.Location = new System.Drawing.Point(24, 50);
            this.ticketsGrid.MultiSelect = false;
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.ReadOnly = true;
            this.ticketsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsGrid.Size = new System.Drawing.Size(592, 220);
            //
            // the columns
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.Width = 56;
            this.colTenant.DataPropertyName = "TenantId";
            this.colTenant.HeaderText = "Tenant";
            this.colTenant.Name = "colTenant";
            this.colTenant.Width = 86;
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 200;
            this.colOwner.DataPropertyName = "Owner";
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.Width = 100;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 80;
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss";
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.Width = 70;
            this.ticketsGrid.Columns.Add(this.colId);
            this.ticketsGrid.Columns.Add(this.colTenant);
            this.ticketsGrid.Columns.Add(this.colTitle);
            this.ticketsGrid.Columns.Add(this.colOwner);
            this.ticketsGrid.Columns.Add(this.colStatus);
            this.ticketsGrid.Columns.Add(this.colUpdatedAt);
            this.ticketsGrid.DataSource = this.ticketsBindingSource;
            //
            // publishButton
            //
            this.publishButton.Location = new System.Drawing.Point(24, 282);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(150, 36);
            this.publishButton.Text = "Publish Event";
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            //
            // escalateButton
            //
            this.escalateButton.Location = new System.Drawing.Point(182, 282);
            this.escalateButton.Name = "escalateButton";
            this.escalateButton.Size = new System.Drawing.Size(160, 36);
            this.escalateButton.Text = "Escalate selected";
            this.escalateButton.Click += new System.EventHandler(this.escalateButton_Click);
            //
            // panelNotifications
            //
            this.panelNotifications.BackColor = System.Drawing.Color.White;
            this.panelNotifications.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelNotifications.Controls.Add(this.labelNotificationsTitle);
            this.panelNotifications.Controls.Add(this.notificationCountLabel);
            this.panelNotifications.Controls.Add(this.notificationsList);
            this.panelNotifications.Controls.Add(this.subscribeButton);
            this.panelNotifications.Controls.Add(this.unsubscribeButton);
            this.panelNotifications.Location = new System.Drawing.Point(676, 18);
            this.panelNotifications.Name = "panelNotifications";
            this.panelNotifications.Size = new System.Drawing.Size(400, 334);
            //
            // labelNotificationsTitle
            //
            this.labelNotificationsTitle.AutoSize = false;
            this.labelNotificationsTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelNotificationsTitle.Location = new System.Drawing.Point(20, 12);
            this.labelNotificationsTitle.Name = "labelNotificationsTitle";
            this.labelNotificationsTitle.Size = new System.Drawing.Size(360, 28);
            this.labelNotificationsTitle.Text = "Live Notifications";
            //
            // notificationCountLabel
            //
            this.notificationCountLabel.AutoSize = false;
            this.notificationCountLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.notificationCountLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.notificationCountLabel.Location = new System.Drawing.Point(20, 42);
            this.notificationCountLabel.Name = "notificationCountLabel";
            this.notificationCountLabel.Size = new System.Drawing.Size(360, 20);
            this.notificationCountLabel.Text = "0 notifications in this session";
            //
            // notificationsList
            //
            this.notificationsList.Font = new System.Drawing.Font("monospace", 9F);
            this.notificationsList.Location = new System.Drawing.Point(20, 66);
            this.notificationsList.Name = "notificationsList";
            this.notificationsList.Size = new System.Drawing.Size(360, 204);
            //
            // subscribeButton
            //
            this.subscribeButton.Enabled = false;
            this.subscribeButton.Location = new System.Drawing.Point(20, 282);
            this.subscribeButton.Name = "subscribeButton";
            this.subscribeButton.Size = new System.Drawing.Size(120, 36);
            this.subscribeButton.Text = "Subscribe";
            this.subscribeButton.Click += new System.EventHandler(this.subscribeButton_Click);
            //
            // unsubscribeButton
            //
            this.unsubscribeButton.Location = new System.Drawing.Point(148, 282);
            this.unsubscribeButton.Name = "unsubscribeButton";
            this.unsubscribeButton.Size = new System.Drawing.Size(130, 36);
            this.unsubscribeButton.Text = "Unsubscribe";
            this.unsubscribeButton.Click += new System.EventHandler(this.unsubscribeButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBoard);
            this.Controls.Add(this.panelNotifications);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1096, 370);
            this.Text = "TicketOps Live — Notifications";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelBoard.ResumeLayout(false);
            this.panelNotifications.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Panel panelBoard;
        private Wisej.Web.Label labelBoardTitle;
        private Wisej.Web.Label labelTenantCaption;
        private Wisej.Web.ComboBox tenantComboBox;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTenant;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Button publishButton;
        private Wisej.Web.Button escalateButton;
        private Wisej.Web.Panel panelNotifications;
        private Wisej.Web.Label labelNotificationsTitle;
        private Wisej.Web.Label notificationCountLabel;
        private Wisej.Web.ListBox notificationsList;
        private Wisej.Web.Button subscribeButton;
        private Wisej.Web.Button unsubscribeButton;
    }
}
