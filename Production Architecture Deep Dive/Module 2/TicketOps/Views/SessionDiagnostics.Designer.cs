namespace TicketOps.Views
{
    partial class SessionDiagnostics
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
            this.panelApplication = new Wisej.Web.Panel();
            this.labelApplicationHeader = new Wisej.Web.Label();
            this.gridApplication = new Wisej.Web.DataGridView();
            this.columnAppKey = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAppValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.panelSession = new Wisej.Web.Panel();
            this.labelSessionHeader = new Wisej.Web.Label();
            this.gridSession = new Wisej.Web.DataGridView();
            this.columnSessionKey = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnSessionValue = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelUserCaption = new Wisej.Web.Label();
            this.comboUser = new Wisej.Web.ComboBox();
            this.labelTenantCaption = new Wisej.Web.Label();
            this.comboTenant = new Wisej.Web.ComboBox();
            this.labelThemeCaption = new Wisej.Web.Label();
            this.comboTheme = new Wisej.Web.ComboBox();
            this.buttonApply = new Wisej.Web.Button();
            this.buttonRefresh = new Wisej.Web.Button();
            this.labelTickets = new Wisej.Web.Label();
            this.gridTickets = new Wisej.Web.DataGridView();
            this.columnId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnTitle = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnPriority = new Wisej.Web.DataGridViewTextBoxColumn();
            this.columnAuthor = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelSelected = new Wisej.Web.Label();
            this.panelScreen.SuspendLayout();
            this.panelApplication.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.panelApplication);
            this.panelScreen.Controls.Add(this.panelSession);
            this.panelScreen.Controls.Add(this.labelUserCaption);
            this.panelScreen.Controls.Add(this.comboUser);
            this.panelScreen.Controls.Add(this.labelTenantCaption);
            this.panelScreen.Controls.Add(this.comboTenant);
            this.panelScreen.Controls.Add(this.labelThemeCaption);
            this.panelScreen.Controls.Add(this.comboTheme);
            this.panelScreen.Controls.Add(this.buttonApply);
            this.panelScreen.Controls.Add(this.buttonRefresh);
            this.panelScreen.Controls.Add(this.labelTickets);
            this.panelScreen.Controls.Add(this.gridTickets);
            this.panelScreen.Controls.Add(this.labelSelected);
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
            this.labelScreenTitle.Text = "Session diagnostics";
            //
            // statusBanner
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // panelApplication
            //
            this.panelApplication.BackColor = System.Drawing.Color.FromArgb(244, 247, 251);
            this.panelApplication.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelApplication.Controls.Add(this.labelApplicationHeader);
            this.panelApplication.Controls.Add(this.gridApplication);
            this.panelApplication.Location = new System.Drawing.Point(24, 80);
            this.panelApplication.Name = "panelApplication";
            this.panelApplication.Size = new System.Drawing.Size(350, 246);
            //
            // labelApplicationHeader
            //
            this.labelApplicationHeader.AutoSize = false;
            this.labelApplicationHeader.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.labelApplicationHeader.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelApplicationHeader.ForeColor = System.Drawing.Color.White;
            this.labelApplicationHeader.Location = new System.Drawing.Point(0, 0);
            this.labelApplicationHeader.Name = "labelApplicationHeader";
            this.labelApplicationHeader.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelApplicationHeader.Size = new System.Drawing.Size(348, 26);
            this.labelApplicationHeader.Text = "Application (all sessions)";
            this.labelApplicationHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridApplication
            //
            this.gridApplication.AllowUserToAddRows = false;
            this.gridApplication.AllowUserToDeleteRows = false;
            this.gridApplication.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnAppKey,
                this.columnAppValue});
            this.gridApplication.Location = new System.Drawing.Point(0, 26);
            this.gridApplication.MultiSelect = false;
            this.gridApplication.Name = "gridApplication";
            this.gridApplication.ReadOnly = true;
            this.gridApplication.RowHeadersVisible = false;
            this.gridApplication.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridApplication.Size = new System.Drawing.Size(348, 218);
            this.columnAppKey.HeaderText = "Setting";
            this.columnAppKey.Name = "columnAppKey";
            this.columnAppKey.ReadOnly = true;
            this.columnAppKey.Width = 150;
            this.columnAppValue.HeaderText = "Value";
            this.columnAppValue.Name = "columnAppValue";
            this.columnAppValue.ReadOnly = true;
            this.columnAppValue.Width = 180;
            //
            // panelSession
            //
            this.panelSession.BackColor = System.Drawing.Color.FromArgb(242, 250, 245);
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionHeader);
            this.panelSession.Controls.Add(this.gridSession);
            this.panelSession.Location = new System.Drawing.Point(386, 80);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(350, 246);
            //
            // labelSessionHeader
            //
            this.labelSessionHeader.AutoSize = false;
            this.labelSessionHeader.BackColor = System.Drawing.Color.FromArgb(31, 138, 76);
            this.labelSessionHeader.Font = new System.Drawing.Font("default", 9.5F, System.Drawing.FontStyle.Bold);
            this.labelSessionHeader.ForeColor = System.Drawing.Color.White;
            this.labelSessionHeader.Location = new System.Drawing.Point(0, 0);
            this.labelSessionHeader.Name = "labelSessionHeader";
            this.labelSessionHeader.Padding = new Wisej.Web.Padding(10, 0, 10, 0);
            this.labelSessionHeader.Size = new System.Drawing.Size(348, 26);
            this.labelSessionHeader.Text = "This session";
            this.labelSessionHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridSession
            //
            this.gridSession.AllowUserToAddRows = false;
            this.gridSession.AllowUserToDeleteRows = false;
            this.gridSession.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnSessionKey,
                this.columnSessionValue});
            this.gridSession.Location = new System.Drawing.Point(0, 26);
            this.gridSession.MultiSelect = false;
            this.gridSession.Name = "gridSession";
            this.gridSession.ReadOnly = true;
            this.gridSession.RowHeadersVisible = false;
            this.gridSession.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridSession.Size = new System.Drawing.Size(348, 218);
            this.columnSessionKey.HeaderText = "Setting";
            this.columnSessionKey.Name = "columnSessionKey";
            this.columnSessionKey.ReadOnly = true;
            this.columnSessionKey.Width = 150;
            this.columnSessionValue.HeaderText = "Value";
            this.columnSessionValue.Name = "columnSessionValue";
            this.columnSessionValue.ReadOnly = true;
            this.columnSessionValue.Width = 180;
            //
            // labelUserCaption
            //
            this.labelUserCaption.AutoSize = false;
            this.labelUserCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelUserCaption.Location = new System.Drawing.Point(24, 338);
            this.labelUserCaption.Name = "labelUserCaption";
            this.labelUserCaption.Size = new System.Drawing.Size(170, 18);
            this.labelUserCaption.Text = "Operator";
            //
            // comboUser
            //
            this.comboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboUser.Location = new System.Drawing.Point(24, 358);
            this.comboUser.Name = "comboUser";
            this.comboUser.Size = new System.Drawing.Size(170, 30);
            //
            // labelTenantCaption
            //
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTenantCaption.Location = new System.Drawing.Point(204, 338);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(140, 18);
            this.labelTenantCaption.Text = "Tenant";
            //
            // comboTenant
            //
            this.comboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTenant.Location = new System.Drawing.Point(204, 358);
            this.comboTenant.Name = "comboTenant";
            this.comboTenant.Size = new System.Drawing.Size(140, 30);
            //
            // labelThemeCaption
            //
            this.labelThemeCaption.AutoSize = false;
            this.labelThemeCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelThemeCaption.Location = new System.Drawing.Point(354, 338);
            this.labelThemeCaption.Name = "labelThemeCaption";
            this.labelThemeCaption.Size = new System.Drawing.Size(140, 18);
            this.labelThemeCaption.Text = "Theme";
            //
            // comboTheme
            //
            this.comboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTheme.Items.AddRange(new object[] { "Bootstrap-4", "Material-3", "FluentDark-5" });
            this.comboTheme.Location = new System.Drawing.Point(354, 358);
            this.comboTheme.Name = "comboTheme";
            this.comboTheme.Size = new System.Drawing.Size(140, 30);
            //
            // buttonApply
            //
            this.buttonApply.Location = new System.Drawing.Point(504, 355);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(150, 36);
            this.buttonApply.Text = "Apply to this session";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            //
            // buttonRefresh
            //
            this.buttonRefresh.Location = new System.Drawing.Point(660, 355);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(76, 36);
            this.buttonRefresh.Text = "↻";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // labelTickets
            //
            this.labelTickets.AutoSize = false;
            this.labelTickets.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTickets.Location = new System.Drawing.Point(24, 402);
            this.labelTickets.Name = "labelTickets";
            this.labelTickets.Size = new System.Drawing.Size(400, 22);
            this.labelTickets.Text = "Open tickets";
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
                this.columnAuthor});
            this.gridTickets.Location = new System.Drawing.Point(24, 426);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 100);
            this.gridTickets.SelectionChanged += new System.EventHandler(this.gridTickets_SelectionChanged);
            this.columnId.HeaderText = "Id";
            this.columnId.Name = "columnId";
            this.columnId.ReadOnly = true;
            this.columnId.Width = 70;
            this.columnTitle.HeaderText = "Title";
            this.columnTitle.Name = "columnTitle";
            this.columnTitle.ReadOnly = true;
            this.columnTitle.Width = 380;
            this.columnPriority.HeaderText = "Priority";
            this.columnPriority.Name = "columnPriority";
            this.columnPriority.ReadOnly = true;
            this.columnPriority.Width = 90;
            this.columnAuthor.HeaderText = "Author";
            this.columnAuthor.Name = "columnAuthor";
            this.columnAuthor.ReadOnly = true;
            this.columnAuthor.Width = 150;
            //
            // labelSelected
            //
            this.labelSelected.AutoSize = false;
            this.labelSelected.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelSelected.ForeColor = System.Drawing.Color.FromArgb(11, 106, 230);
            this.labelSelected.Location = new System.Drawing.Point(24, 532);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(500, 22);
            this.labelSelected.Text = "No ticket selected.";
            //
            // SessionDiagnostics
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 620);
            this.Controls.Add(this.panelScreen);
            this.Name = "SessionDiagnostics";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.SessionDiagnostics_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelApplication.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Panel panelApplication;
        private Wisej.Web.Label labelApplicationHeader;
        private Wisej.Web.DataGridView gridApplication;
        private Wisej.Web.DataGridViewTextBoxColumn columnAppKey;
        private Wisej.Web.DataGridViewTextBoxColumn columnAppValue;
        private Wisej.Web.Panel panelSession;
        private Wisej.Web.Label labelSessionHeader;
        private Wisej.Web.DataGridView gridSession;
        private Wisej.Web.DataGridViewTextBoxColumn columnSessionKey;
        private Wisej.Web.DataGridViewTextBoxColumn columnSessionValue;
        private Wisej.Web.Label labelUserCaption;
        private Wisej.Web.ComboBox comboUser;
        private Wisej.Web.Label labelTenantCaption;
        private Wisej.Web.ComboBox comboTenant;
        private Wisej.Web.Label labelThemeCaption;
        private Wisej.Web.ComboBox comboTheme;
        private Wisej.Web.Button buttonApply;
        private Wisej.Web.Button buttonRefresh;
        private Wisej.Web.Label labelTickets;
        private Wisej.Web.DataGridView gridTickets;
        private Wisej.Web.DataGridViewTextBoxColumn columnId;
        private Wisej.Web.DataGridViewTextBoxColumn columnTitle;
        private Wisej.Web.DataGridViewTextBoxColumn columnPriority;
        private Wisej.Web.DataGridViewTextBoxColumn columnAuthor;
        private Wisej.Web.Label labelSelected;
    }
}
