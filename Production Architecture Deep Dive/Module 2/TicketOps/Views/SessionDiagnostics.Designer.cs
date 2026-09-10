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
            this.components = new System.ComponentModel.Container();
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelSubtitle = new Wisej.Web.Label();
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
            this.progressSimulate = new Wisej.Web.ProgressBar();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonStamp = new Wisej.Web.Button();
            this.buttonSimulate = new Wisej.Web.Button();
            this.buttonForbiddenTenant = new Wisej.Web.Button();
            this.buttonLegacyStatic = new Wisej.Web.Button();
            this.buttonOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.timerSimulate = new Wisej.Web.Timer(this.components);
            this.panelScreen.SuspendLayout();
            this.panelApplication.SuspendLayout();
            this.panelSession.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (Session diagnostics: two panels + per-session selectors + the tenant's tickets)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelSubtitle);
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
            this.panelScreen.Controls.Add(this.progressSimulate);
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
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelSubtitle
            //
            this.labelSubtitle.AutoSize = false;
            this.labelSubtitle.Font = new System.Drawing.Font("default", 9F);
            this.labelSubtitle.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelSubtitle.Location = new System.Drawing.Point(24, 48);
            this.labelSubtitle.Name = "labelSubtitle";
            this.labelSubtitle.Size = new System.Drawing.Size(460, 20);
            this.labelSubtitle.Text = "Left: identical in every session · Right: this session only (injected SessionContext)";
            //
            // panelApplication  (global — the same values no matter who is logged in)
            //
            this.panelApplication.BackColor = System.Drawing.Color.FromArgb(244, 247, 251);
            this.panelApplication.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelApplication.Controls.Add(this.labelApplicationHeader);
            this.panelApplication.Controls.Add(this.gridApplication);
            this.panelApplication.Location = new System.Drawing.Point(24, 76);
            this.panelApplication.Name = "panelApplication";
            this.panelApplication.Size = new System.Drawing.Size(350, 250);
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
            this.labelApplicationHeader.Text = "Application · global (same for every session)";
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
            this.gridApplication.Size = new System.Drawing.Size(348, 222);
            this.columnAppKey.HeaderText = "Setting";
            this.columnAppKey.Name = "columnAppKey";
            this.columnAppKey.ReadOnly = true;
            this.columnAppKey.Width = 150;
            this.columnAppValue.HeaderText = "Value";
            this.columnAppValue.Name = "columnAppValue";
            this.columnAppValue.ReadOnly = true;
            this.columnAppValue.Width = 180;
            //
            // panelSession  (per session — from the injected SessionContext)
            //
            this.panelSession.BackColor = System.Drawing.Color.FromArgb(242, 250, 245);
            this.panelSession.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSession.Controls.Add(this.labelSessionHeader);
            this.panelSession.Controls.Add(this.gridSession);
            this.panelSession.Location = new System.Drawing.Point(386, 76);
            this.panelSession.Name = "panelSession";
            this.panelSession.Size = new System.Drawing.Size(350, 250);
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
            this.labelSessionHeader.Text = "This session · SessionContext (injected)";
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
            this.gridSession.Size = new System.Drawing.Size(348, 222);
            this.columnSessionKey.HeaderText = "Value of this session";
            this.columnSessionKey.Name = "columnSessionKey";
            this.columnSessionKey.ReadOnly = true;
            this.columnSessionKey.Width = 150;
            this.columnSessionValue.HeaderText = "Value";
            this.columnSessionValue.Name = "columnSessionValue";
            this.columnSessionValue.ReadOnly = true;
            this.columnSessionValue.Width = 180;
            //
            // per-session selectors: user / tenant / theme → "Apply to this session"
            //
            this.labelUserCaption.AutoSize = false;
            this.labelUserCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelUserCaption.Location = new System.Drawing.Point(24, 334);
            this.labelUserCaption.Name = "labelUserCaption";
            this.labelUserCaption.Size = new System.Drawing.Size(170, 18);
            this.labelUserCaption.Text = "Operator (this session)";
            this.comboUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboUser.Location = new System.Drawing.Point(24, 354);
            this.comboUser.Name = "comboUser";
            this.comboUser.Size = new System.Drawing.Size(170, 30);
            this.comboUser.ToolTipText = "Sign this session in as another operator — the other tab keeps its own";
            this.labelTenantCaption.AutoSize = false;
            this.labelTenantCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelTenantCaption.Location = new System.Drawing.Point(204, 334);
            this.labelTenantCaption.Name = "labelTenantCaption";
            this.labelTenantCaption.Size = new System.Drawing.Size(140, 18);
            this.labelTenantCaption.Text = "Tenant";
            this.comboTenant.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTenant.Location = new System.Drawing.Point(204, 354);
            this.comboTenant.Name = "comboTenant";
            this.comboTenant.Size = new System.Drawing.Size(140, 30);
            this.comboTenant.ToolTipText = "Switch this session's tenant (UserAccount.IsMemberOf decides)";
            this.labelThemeCaption.AutoSize = false;
            this.labelThemeCaption.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelThemeCaption.Location = new System.Drawing.Point(354, 334);
            this.labelThemeCaption.Name = "labelThemeCaption";
            this.labelThemeCaption.Size = new System.Drawing.Size(140, 18);
            this.labelThemeCaption.Text = "Theme";
            this.comboTheme.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboTheme.Items.AddRange(new object[] { "Bootstrap-4", "Material-3", "FluentDark-5" });
            this.comboTheme.Location = new System.Drawing.Point(354, 354);
            this.comboTheme.Name = "comboTheme";
            this.comboTheme.Size = new System.Drawing.Size(140, 30);
            this.comboTheme.ToolTipText = "Application.Theme is per session: this tab restyles, the other keeps its theme";
            this.buttonApply.Location = new System.Drawing.Point(504, 351);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(150, 36);
            this.buttonApply.Text = "Apply to this session";
            this.buttonApply.ToolTipText = "Success path: SignInAsync / SwitchTenantAsync / SelectTheme on ISessionService — the SessionContext of THIS session changes, nothing global does";
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            this.buttonRefresh.Location = new System.Drawing.Point(660, 351);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(76, 36);
            this.buttonRefresh.Text = "↻";
            this.buttonRefresh.ToolTipText = "Refresh both panels (IDiagnosticsService.GetSnapshot) — use it in the other tab to see what leaked and what did not";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            //
            // labelTickets + gridTickets  (this session's tenant only)
            //
            this.labelTickets.AutoSize = false;
            this.labelTickets.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.labelTickets.Location = new System.Drawing.Point(24, 396);
            this.labelTickets.Name = "labelTickets";
            this.labelTickets.Size = new System.Drawing.Size(400, 22);
            this.labelTickets.Text = "Open tickets";
            this.gridTickets.AllowUserToAddRows = false;
            this.gridTickets.AllowUserToDeleteRows = false;
            this.gridTickets.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.gridTickets.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.columnId,
                this.columnTitle,
                this.columnPriority,
                this.columnAuthor});
            this.gridTickets.Location = new System.Drawing.Point(24, 420);
            this.gridTickets.MultiSelect = false;
            this.gridTickets.Name = "gridTickets";
            this.gridTickets.ReadOnly = true;
            this.gridTickets.RowHeadersVisible = false;
            this.gridTickets.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridTickets.Size = new System.Drawing.Size(712, 104);
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
            this.labelSelected.Location = new System.Drawing.Point(24, 530);
            this.labelSelected.Name = "labelSelected";
            this.labelSelected.Size = new System.Drawing.Size(500, 22);
            this.labelSelected.Text = "Select a ticket — the selection is stored in this session only.";
            //
            // progressSimulate
            //
            this.progressSimulate.Location = new System.Drawing.Point(540, 532);
            this.progressSimulate.Maximum = 20;
            this.progressSimulate.Name = "progressSimulate";
            this.progressSimulate.Size = new System.Drawing.Size(196, 18);
            this.progressSimulate.Visible = false;
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Service → Session / Data";
            this.tracePanel.Footer = "UI collects input · SVC decides · SESSION = this session's SessionContext · DATA persists (shared store) · INFRA = once per process · ⚠ handled · ✖ failure (details stay here)";
            //
            // panelActions  (bottom bar: success / progress / failure / probe / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonStamp);
            this.panelActions.Controls.Add(this.buttonSimulate);
            this.panelActions.Controls.Add(this.buttonForbiddenTenant);
            this.panelActions.Controls.Add(this.buttonLegacyStatic);
            this.panelActions.Controls.Add(this.buttonOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // bottom bar buttons
            //
            this.buttonStamp.Location = new System.Drawing.Point(0, 4);
            this.buttonStamp.Name = "buttonStamp";
            this.buttonStamp.Size = new System.Drawing.Size(230, 36);
            this.buttonStamp.Text = "Stamp a ticket for this session";
            this.buttonStamp.ToolTipText = "Success path: ITicketService.CreateForCurrentUserAsync stamps Tenant + Author from SessionContext → shared store; the other tab sees the row after ↻ if it is in the same tenant";
            this.buttonStamp.Click += new System.EventHandler(this.buttonStamp_Click);
            this.buttonSimulate.Location = new System.Drawing.Point(240, 4);
            this.buttonSimulate.Name = "buttonSimulate";
            this.buttonSimulate.Size = new System.Drawing.Size(190, 36);
            this.buttonSimulate.Text = "▶ Simulate 20 sessions";
            this.buttonSimulate.ToolTipText = "Progress path: a Timer creates 20 SessionContexts (2 per tick) — the shared counter grows, this session's panel does not change";
            this.buttonSimulate.Click += new System.EventHandler(this.buttonSimulate_Click);
            this.buttonForbiddenTenant.Location = new System.Drawing.Point(440, 4);
            this.buttonForbiddenTenant.Name = "buttonForbiddenTenant";
            this.buttonForbiddenTenant.Size = new System.Drawing.Size(220, 36);
            this.buttonForbiddenTenant.Text = "Switch to a forbidden tenant";
            this.buttonForbiddenTenant.ToolTipText = "Failure path (rule): UserAccount.IsMemberOf says no — the SessionContext keeps its tenant, the user reads why";
            this.buttonForbiddenTenant.Click += new System.EventHandler(this.buttonForbiddenTenant_Click);
            this.buttonLegacyStatic.Location = new System.Drawing.Point(670, 4);
            this.buttonLegacyStatic.Name = "buttonLegacyStatic";
            this.buttonLegacyStatic.Size = new System.Drawing.Size(220, 36);
            this.buttonLegacyStatic.Text = "⚠ Write user to legacy static";
            this.buttonLegacyStatic.ToolTipText = "Audit probe (the bug): writes this session's user into a static field — refresh the OTHER tab and it shows this tab's user";
            this.buttonLegacyStatic.Click += new System.EventHandler(this.buttonLegacyStatic_Click);
            this.buttonOutage.Location = new System.Drawing.Point(900, 4);
            this.buttonOutage.Name = "buttonOutage";
            this.buttonOutage.Size = new System.Drawing.Size(210, 36);
            this.buttonOutage.Text = "Simulate directory outage";
            this.buttonOutage.ToolTipText = "Error path: the user directory throws; the log gets the LDAP details, the user gets a safe message, the SessionContext is untouched. Click again to recover.";
            this.buttonOutage.Click += new System.EventHandler(this.buttonOutage_Click);
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerSimulate
            //
            this.timerSimulate.Interval = 150;
            this.timerSimulate.Tick += new System.EventHandler(this.timerSimulate_Tick);
            //
            // SessionDiagnostics
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "SessionDiagnostics";
            this.Text = "TicketOps Console — Module 2 · Startup, configuration, session state & lifetime";
            this.Load += new System.EventHandler(this.SessionDiagnostics_Load);
            this.panelScreen.ResumeLayout(false);
            this.panelApplication.ResumeLayout(false);
            this.panelSession.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelSubtitle;
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
        private Wisej.Web.ProgressBar progressSimulate;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonStamp;
        private Wisej.Web.Button buttonSimulate;
        private Wisej.Web.Button buttonForbiddenTenant;
        private Wisej.Web.Button buttonLegacyStatic;
        private Wisej.Web.Button buttonOutage;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerSimulate;
    }
}
