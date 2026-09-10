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
            this.labelStatus = new Wisej.Web.Label();
            this.labelTenantCaption = new Wisej.Web.Label();
            this.tenantComboBox = new Wisej.Web.ComboBox();
            this.labelTenantHint = new Wisej.Web.Label();
            this.ticketsGrid = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTenant = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colOwner = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colUpdatedAt = new Wisej.Web.DataGridViewTextBoxColumn();
            this.selectedLabel = new Wisej.Web.Label();
            this.labelBanner = new Wisej.Web.Label();
            this.panelNotifications = new Wisej.Web.Panel();
            this.labelNotificationsTitle = new Wisej.Web.Label();
            this.notificationCountLabel = new Wisej.Web.Label();
            this.notificationsList = new Wisej.Web.ListBox();
            this.subscribersLabel = new Wisej.Web.Label();
            this.filteredOutLabel = new Wisej.Web.Label();
            this.hubStatsLabel = new Wisej.Web.Label();
            this.labelState = new Wisej.Web.Label();
            this.panelTrace = new Wisej.Web.Panel();
            this.labelTraceTitle = new Wisej.Web.Label();
            this.listTrace = new Wisej.Web.ListBox();
            this.labelTraceFooter = new Wisej.Web.Label();
            this.panelActions = new Wisej.Web.Panel();
            this.publishButton = new Wisej.Web.Button();
            this.publishOtherButton = new Wisej.Web.Button();
            this.escalateButton = new Wisej.Web.Button();
            this.subscribeButton = new Wisej.Web.Button();
            this.unsubscribeButton = new Wisej.Web.Button();
            this.invalidButton = new Wisej.Web.Button();
            this.burstButton = new Wisej.Web.Button();
            this.openSessionButton = new Wisej.Web.Button();
            this.clearButton = new Wisej.Web.Button();
            this.panelBoard.SuspendLayout();
            this.panelNotifications.SuspendLayout();
            this.panelTrace.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelBoard  (the ticket board of THIS session — its own filter, its own bound list)
            //
            this.panelBoard.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left;
            this.panelBoard.BackColor = System.Drawing.Color.White;
            this.panelBoard.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelBoard.Controls.Add(this.labelBoardTitle);
            this.panelBoard.Controls.Add(this.labelStatus);
            this.panelBoard.Controls.Add(this.labelTenantCaption);
            this.panelBoard.Controls.Add(this.tenantComboBox);
            this.panelBoard.Controls.Add(this.labelTenantHint);
            this.panelBoard.Controls.Add(this.ticketsGrid);
            this.panelBoard.Controls.Add(this.selectedLabel);
            this.panelBoard.Controls.Add(this.labelBanner);
            this.panelBoard.Location = new System.Drawing.Point(30, 18);
            this.panelBoard.Name = "panelBoard";
            this.panelBoard.Size = new System.Drawing.Size(700, 330);
            //
            // labelBoardTitle
            //
            this.labelBoardTitle.AutoSize = false;
            this.labelBoardTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelBoardTitle.Location = new System.Drawing.Point(24, 12);
            this.labelBoardTitle.Name = "labelBoardTitle";
            this.labelBoardTitle.Size = new System.Drawing.Size(370, 28);
            this.labelBoardTitle.Text = "Ticket board (this session)";
            //
            // labelStatus
            //
            this.labelStatus.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(400, 14);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(276, 26);
            this.labelStatus.Text = "● idle";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelTenantCaption / tenantComboBox  (the metadata filter — per session)
            //
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTenantCaption.Location = new System.Drawing.Point(24, 48);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(110, 24);
            this.labelTenantCaption.Text = "TENANT";
            this.labelTenantCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tenantComboBox.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.tenantComboBox.Items.Add("Contoso");
            this.tenantComboBox.Items.Add("Northwind");
            this.tenantComboBox.Location = new System.Drawing.Point(136, 46);
            this.tenantComboBox.Name = "tenantComboBox";
            this.tenantComboBox.SelectedIndex = 0;
            this.tenantComboBox.Size = new System.Drawing.Size(150, 28);
            this.tenantComboBox.ToolTipText = "Which tenant this session works for. The hub sends every event to every subscriber; this session drops the ones that are not its tenant.";
            this.tenantComboBox.SelectedIndexChanged += new System.EventHandler(this.tenantComboBox_SelectedIndexChanged);
            //
            // labelTenantHint
            //
            this.labelTenantHint.AutoSize = false;
            this.labelTenantHint.Font = new System.Drawing.Font("monospace", 9F);
            this.labelTenantHint.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTenantHint.Location = new System.Drawing.Point(300, 50);
            this.labelTenantHint.Name = "labelTenantHint";
            this.labelTenantHint.Size = new System.Drawing.Size(376, 22);
            this.labelTenantHint.Text = "the hub carries the metadata · the session decides";
            this.labelTenantHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // ticketsGrid  (bound to this session's own BindingList through ticketsBindingSource)
            //
            this.ticketsGrid.AllowUserToAddRows = false;
            this.ticketsGrid.AllowUserToDeleteRows = false;
            this.ticketsGrid.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.ticketsGrid.AutoGenerateColumns = false;
            this.ticketsGrid.Location = new System.Drawing.Point(24, 80);
            this.ticketsGrid.MultiSelect = false;
            this.ticketsGrid.Name = "ticketsGrid";
            this.ticketsGrid.ReadOnly = true;
            this.ticketsGrid.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.ticketsGrid.Size = new System.Drawing.Size(652, 164);
            //
            // the columns (declared by hand — AutoGenerateColumns is false)
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Id";
            this.colId.Name = "colId";
            this.colId.Width = 60;
            this.colTenant.DataPropertyName = "TenantId";
            this.colTenant.HeaderText = "Tenant";
            this.colTenant.Name = "colTenant";
            this.colTenant.Width = 90;
            this.colTitle.DataPropertyName = "Title";
            this.colTitle.HeaderText = "Title";
            this.colTitle.Name = "colTitle";
            this.colTitle.Width = 236;
            this.colOwner.DataPropertyName = "Owner";
            this.colOwner.HeaderText = "Owner";
            this.colOwner.Name = "colOwner";
            this.colOwner.Width = 110;
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 90;
            this.colUpdatedAt.DataPropertyName = "UpdatedAt";
            this.colUpdatedAt.DefaultCellStyle.Format = "HH:mm:ss";
            this.colUpdatedAt.HeaderText = "Updated";
            this.colUpdatedAt.Name = "colUpdatedAt";
            this.colUpdatedAt.Width = 90;
            this.ticketsGrid.Columns.Add(this.colId);
            this.ticketsGrid.Columns.Add(this.colTenant);
            this.ticketsGrid.Columns.Add(this.colTitle);
            this.ticketsGrid.Columns.Add(this.colOwner);
            this.ticketsGrid.Columns.Add(this.colStatus);
            this.ticketsGrid.Columns.Add(this.colUpdatedAt);
            this.ticketsGrid.DataSource = this.ticketsBindingSource;
            this.ticketsGrid.SelectionChanged += new System.EventHandler(this.ticketsGrid_SelectionChanged);
            //
            // selectedLabel
            //
            this.selectedLabel.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.selectedLabel.AutoSize = false;
            this.selectedLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.selectedLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.selectedLabel.Location = new System.Drawing.Point(24, 250);
            this.selectedLabel.Name = "selectedLabel";
            this.selectedLabel.Size = new System.Drawing.Size(652, 22);
            this.selectedLabel.Text = "no row selected — Publish creates a new ticket; select a row to update or escalate it";
            //
            // labelBanner
            //
            this.labelBanner.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(24, 276);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(652, 44);
            this.labelBanner.Text = "";
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // panelNotifications  (per-session notifications + hub counters + SERVER STATE)
            //
            this.panelNotifications.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left;
            this.panelNotifications.BackColor = System.Drawing.Color.White;
            this.panelNotifications.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelNotifications.Controls.Add(this.labelNotificationsTitle);
            this.panelNotifications.Controls.Add(this.notificationCountLabel);
            this.panelNotifications.Controls.Add(this.notificationsList);
            this.panelNotifications.Controls.Add(this.subscribersLabel);
            this.panelNotifications.Controls.Add(this.filteredOutLabel);
            this.panelNotifications.Controls.Add(this.hubStatsLabel);
            this.panelNotifications.Controls.Add(this.labelState);
            this.panelNotifications.Location = new System.Drawing.Point(30, 356);
            this.panelNotifications.Name = "panelNotifications";
            this.panelNotifications.Size = new System.Drawing.Size(700, 242);
            //
            // labelNotificationsTitle
            //
            this.labelNotificationsTitle.AutoSize = false;
            this.labelNotificationsTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelNotificationsTitle.Location = new System.Drawing.Point(24, 10);
            this.labelNotificationsTitle.Name = "labelNotificationsTitle";
            this.labelNotificationsTitle.Size = new System.Drawing.Size(360, 28);
            this.labelNotificationsTitle.Text = "Notifications";
            //
            // notificationCountLabel  (the extension challenge: per-session count)
            //
            this.notificationCountLabel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.notificationCountLabel.AutoSize = false;
            this.notificationCountLabel.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.notificationCountLabel.ForeColor = System.Drawing.Color.FromArgb(21, 79, 143);
            this.notificationCountLabel.Location = new System.Drawing.Point(390, 12);
            this.notificationCountLabel.Name = "notificationCountLabel";
            this.notificationCountLabel.Size = new System.Drawing.Size(286, 24);
            this.notificationCountLabel.Text = "0 notifications in this session";
            this.notificationCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // notificationsList
            //
            this.notificationsList.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.notificationsList.Font = new System.Drawing.Font("monospace", 9F);
            this.notificationsList.Location = new System.Drawing.Point(24, 40);
            this.notificationsList.Name = "notificationsList";
            this.notificationsList.Size = new System.Drawing.Size(652, 72);
            //
            // subscribersLabel / filteredOutLabel
            //
            this.subscribersLabel.AutoSize = false;
            this.subscribersLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.subscribersLabel.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.subscribersLabel.Location = new System.Drawing.Point(24, 118);
            this.subscribersLabel.Name = "subscribersLabel";
            this.subscribersLabel.Size = new System.Drawing.Size(330, 20);
            this.subscribersLabel.Text = "hub subscribers: 0";
            this.filteredOutLabel.AutoSize = false;
            this.filteredOutLabel.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.filteredOutLabel.ForeColor = System.Drawing.Color.FromArgb(146, 64, 14);
            this.filteredOutLabel.Location = new System.Drawing.Point(358, 118);
            this.filteredOutLabel.Name = "filteredOutLabel";
            this.filteredOutLabel.Size = new System.Drawing.Size(318, 20);
            this.filteredOutLabel.Text = "filtered out (wrong tenant): 0";
            this.filteredOutLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // hubStatsLabel
            //
            this.hubStatsLabel.AutoSize = false;
            this.hubStatsLabel.Font = new System.Drawing.Font("monospace", 9F);
            this.hubStatsLabel.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.hubStatsLabel.Location = new System.Drawing.Point(24, 140);
            this.hubStatsLabel.Name = "hubStatsLabel";
            this.hubStatsLabel.Size = new System.Drawing.Size(652, 20);
            this.hubStatsLabel.Text = "hub: 0 tickets · 0 events published (global — the same numbers in every session)";
            //
            // labelState  (what the server owns right now)
            //
            this.labelState.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelState.AutoSize = false;
            this.labelState.Font = new System.Drawing.Font("monospace", 9F);
            this.labelState.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelState.Location = new System.Drawing.Point(24, 162);
            this.labelState.Name = "labelState";
            this.labelState.Size = new System.Drawing.Size(652, 70);
            this.labelState.Text = "";
            this.labelState.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelTrace  (Server → Browser live push trace)
            //
            this.panelTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelTrace.BackColor = System.Drawing.Color.White;
            this.panelTrace.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelTrace.Controls.Add(this.labelTraceTitle);
            this.panelTrace.Controls.Add(this.listTrace);
            this.panelTrace.Controls.Add(this.labelTraceFooter);
            this.panelTrace.Location = new System.Drawing.Point(748, 18);
            this.panelTrace.Name = "panelTrace";
            this.panelTrace.Size = new System.Drawing.Size(570, 580);
            //
            // labelTraceTitle
            //
            this.labelTraceTitle.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceTitle.AutoSize = false;
            this.labelTraceTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelTraceTitle.Location = new System.Drawing.Point(20, 14);
            this.labelTraceTitle.Name = "labelTraceTitle";
            this.labelTraceTitle.Size = new System.Drawing.Size(530, 30);
            this.labelTraceTitle.Text = "Server → Browser  ·  live push trace";
            //
            // listTrace
            //
            this.listTrace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.listTrace.Font = new System.Drawing.Font("monospace", 9F);
            this.listTrace.Location = new System.Drawing.Point(20, 52);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(530, 478);
            //
            // labelTraceFooter
            //
            this.labelTraceFooter.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.labelTraceFooter.AutoSize = false;
            this.labelTraceFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelTraceFooter.Location = new System.Drawing.Point(20, 540);
            this.labelTraceFooter.Name = "labelTraceFooter";
            this.labelTraceFooter.Size = new System.Drawing.Size(530, 26);
            this.labelTraceFooter.Text = "→ push = Application.Update(_context)   ·   ← request = the browser asked   ·   • server = hub decision";
            //
            // panelActions
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.publishButton);
            this.panelActions.Controls.Add(this.publishOtherButton);
            this.panelActions.Controls.Add(this.escalateButton);
            this.panelActions.Controls.Add(this.subscribeButton);
            this.panelActions.Controls.Add(this.unsubscribeButton);
            this.panelActions.Controls.Add(this.invalidButton);
            this.panelActions.Controls.Add(this.burstButton);
            this.panelActions.Controls.Add(this.openSessionButton);
            this.panelActions.Controls.Add(this.clearButton);
            this.panelActions.Location = new System.Drawing.Point(30, 614);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // publishButton  (success path — fans out to every subscribed session of this tenant)
            //
            this.publishButton.Location = new System.Drawing.Point(0, 4);
            this.publishButton.Name = "publishButton";
            this.publishButton.Size = new System.Drawing.Size(180, 36);
            this.publishButton.Text = "Publish ticket (my tenant)";
            this.publishButton.ToolTipText = "TicketHub.AddOrUpdate: updates the selected ticket, or opens a new one when nothing is selected. Every subscribed session of this tenant receives it.";
            this.publishButton.Click += new System.EventHandler(this.publishButton_Click);
            //
            // publishOtherButton  (targeted update — arrives only in the OTHER tenant's sessions)
            //
            this.publishOtherButton.Location = new System.Drawing.Point(184, 4);
            this.publishOtherButton.Name = "publishOtherButton";
            this.publishOtherButton.Size = new System.Drawing.Size(185, 36);
            this.publishOtherButton.Text = "Publish for the other tenant";
            this.publishOtherButton.ToolTipText = "Opens a ticket for the tenant this session is NOT watching: here it is counted as filtered out, in the other tenant's sessions it appears.";
            this.publishOtherButton.Click += new System.EventHandler(this.publishOtherButton_Click);
            //
            // escalateButton  (the one event TYPE that is filtered — only Escalated pops a toast)
            //
            this.escalateButton.Location = new System.Drawing.Point(373, 4);
            this.escalateButton.Name = "escalateButton";
            this.escalateButton.Size = new System.Drawing.Size(130, 36);
            this.escalateButton.Text = "Escalate selected";
            this.escalateButton.ToolTipText = "Publishes ChangeType = \"Escalated\". Matching sessions also show an AlertBox toast; every other change type does not.";
            this.escalateButton.Click += new System.EventHandler(this.escalateButton_Click);
            //
            // subscribeButton / unsubscribeButton  (the subscription is visible and reversible)
            //
            this.subscribeButton.Enabled = false;
            this.subscribeButton.Location = new System.Drawing.Point(507, 4);
            this.subscribeButton.Name = "subscribeButton";
            this.subscribeButton.Size = new System.Drawing.Size(90, 36);
            this.subscribeButton.Text = "Subscribe";
            this.subscribeButton.ToolTipText = "hub.TicketChanged += Hub_TicketChanged — watch the subscriber count.";
            this.subscribeButton.Click += new System.EventHandler(this.subscribeButton_Click);
            this.unsubscribeButton.Location = new System.Drawing.Point(601, 4);
            this.unsubscribeButton.Name = "unsubscribeButton";
            this.unsubscribeButton.Size = new System.Drawing.Size(100, 36);
            this.unsubscribeButton.Text = "Unsubscribe";
            this.unsubscribeButton.ToolTipText = "hub.TicketChanged -= Hub_TicketChanged — this session stops receiving events; the others keep working.";
            this.unsubscribeButton.Click += new System.EventHandler(this.unsubscribeButton_Click);
            //
            // invalidButton  (failure path — the hub validates and throws; global state is unchanged)
            //
            this.invalidButton.Location = new System.Drawing.Point(705, 4);
            this.invalidButton.Name = "invalidButton";
            this.invalidButton.Size = new System.Drawing.Size(155, 36);
            this.invalidButton.Text = "Publish invalid ticket";
            this.invalidButton.ToolTipText = "A ticket with no title and no tenant: the hub throws ArgumentException before touching its list — nothing is published.";
            this.invalidButton.Click += new System.EventHandler(this.invalidButton_Click);
            //
            // burstButton  (progress path — cadence: 20 events, 150 ms apart, from a task)
            //
            this.burstButton.Location = new System.Drawing.Point(864, 4);
            this.burstButton.Name = "burstButton";
            this.burstButton.Size = new System.Drawing.Size(125, 36);
            this.burstButton.Text = "Burst 20 events";
            this.burstButton.ToolTipText = "Application.StartTask publishes 20 updates 150 ms apart. Every subscribed session of this tenant renders them.";
            this.burstButton.Click += new System.EventHandler(this.burstButton_Click);
            //
            // openSessionButton
            //
            this.openSessionButton.Location = new System.Drawing.Point(993, 4);
            this.openSessionButton.Name = "openSessionButton";
            this.openSessionButton.Size = new System.Drawing.Size(175, 36);
            this.openSessionButton.Text = "Open another session ↗";
            this.openSessionButton.ToolTipText = "Application.Navigate(\"/\", \"_blank\") — a second browser tab is a second session with its own MainPage.";
            this.openSessionButton.Click += new System.EventHandler(this.openSessionButton_Click);
            //
            // clearButton
            //
            this.clearButton.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.clearButton.Location = new System.Drawing.Point(1178, 4);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 36);
            this.clearButton.Text = "Clear trace";
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelBoard);
            this.Controls.Add(this.panelNotifications);
            this.Controls.Add(this.panelTrace);
            this.Controls.Add(this.panelActions);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 700);
            this.Text = "TicketOps Live — From One Session to Many: TicketHub Events";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelBoard.ResumeLayout(false);
            this.panelNotifications.ResumeLayout(false);
            this.panelTrace.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.BindingSource ticketsBindingSource;
        private Wisej.Web.Panel panelBoard;
        private Wisej.Web.Label labelBoardTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Label labelTenantCaption;
        private Wisej.Web.ComboBox tenantComboBox;
        private Wisej.Web.Label labelTenantHint;
        private Wisej.Web.DataGridView ticketsGrid;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colTenant;
        private Wisej.Web.DataGridViewTextBoxColumn colTitle;
        private Wisej.Web.DataGridViewTextBoxColumn colOwner;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colUpdatedAt;
        private Wisej.Web.Label selectedLabel;
        private Wisej.Web.Label labelBanner;
        private Wisej.Web.Panel panelNotifications;
        private Wisej.Web.Label labelNotificationsTitle;
        private Wisej.Web.Label notificationCountLabel;
        private Wisej.Web.ListBox notificationsList;
        private Wisej.Web.Label subscribersLabel;
        private Wisej.Web.Label filteredOutLabel;
        private Wisej.Web.Label hubStatsLabel;
        private Wisej.Web.Label labelState;
        private Wisej.Web.Panel panelTrace;
        private Wisej.Web.Label labelTraceTitle;
        private Wisej.Web.ListBox listTrace;
        private Wisej.Web.Label labelTraceFooter;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button publishButton;
        private Wisej.Web.Button publishOtherButton;
        private Wisej.Web.Button escalateButton;
        private Wisej.Web.Button subscribeButton;
        private Wisej.Web.Button unsubscribeButton;
        private Wisej.Web.Button invalidButton;
        private Wisej.Web.Button burstButton;
        private Wisej.Web.Button openSessionButton;
        private Wisej.Web.Button clearButton;
    }
}
