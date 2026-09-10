namespace OrderDesk
{
    partial class MainPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Application.SessionTimeout is a static event: a page that forgets to unsubscribe is never collected.
                // (Application.ApplicationExit is subscribed in Program.Main, not by the page — see Program.cs.)
                DetachApplicationEvents();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelAudit = new Wisej.Web.Panel();
            this.labelAuditTitle = new Wisej.Web.Label();
            this.labelStatus = new Wisej.Web.Label();
            this.buttonAuditAll = new Wisej.Web.Button();
            this.buttonAuditKeep = new Wisej.Web.Button();
            this.buttonAuditSession = new Wisej.Web.Button();
            this.buttonAuditProfile = new Wisej.Web.Button();
            this.buttonAuditBrowser = new Wisej.Web.Button();
            this.labelAuditCount = new Wisej.Web.Label();
            this.gridAudit = new Wisej.Web.DataGridView();
            this.colMember = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colDeclaredIn = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colKind = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colVerdict = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelAuditDetail = new Wisej.Web.Label();
            this.panelIsolation = new Wisej.Web.Panel();
            this.labelIsolationTitle = new Wisej.Web.Label();
            this.radioLegacy = new Wisej.Web.RadioButton();
            this.radioContext = new Wisej.Web.RadioButton();
            this.labelOrdersCount = new Wisej.Web.Label();
            this.comboFilter = new Wisej.Web.ComboBox();
            this.comboCustomer = new Wisej.Web.ComboBox();
            this.buttonSignInKelly = new Wisej.Web.Button();
            this.buttonSignInSam = new Wisej.Web.Button();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colOrder = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.labelStores = new Wisej.Web.Label();
            this.buttonReread = new Wisej.Web.Button();
            this.buttonSecondSession = new Wisej.Web.Button();
            this.labelBanner = new Wisej.Web.Label();
            this.trace = new OrderDesk.Views.TracePanel();
            this.panelSettings = new Wisej.Web.Panel();
            this.labelSettingsTitle = new Wisej.Web.Label();
            this.labelSettingsValues = new Wisej.Web.Label();
            this.comboDensity = new Wisej.Web.ComboBox();
            this.buttonRegistry = new Wisej.Web.Button();
            this.buttonProfile = new Wisej.Web.Button();
            this.buttonBrowser = new Wisej.Web.Button();
            this.buttonSignOut = new Wisej.Web.Button();
            this.buttonTimeout = new Wisej.Web.Button();
            this.progressTimeout = new Wisej.Web.ProgressBar();
            this.buttonClear = new Wisej.Web.Button();
            this.timerTimeout = new Wisej.Web.Timer(this.components);
            this.panelAudit.SuspendLayout();
            this.panelIsolation.SuspendLayout();
            this.panelSettings.SuspendLayout();
            this.SuspendLayout();
            //
            // panelAudit  (card A: lab steps 1 + 2 — every static found, classified)
            //
            this.panelAudit.BackColor = System.Drawing.Color.White;
            this.panelAudit.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelAudit.Controls.Add(this.labelAuditTitle);
            this.panelAudit.Controls.Add(this.labelStatus);
            this.panelAudit.Controls.Add(this.buttonAuditAll);
            this.panelAudit.Controls.Add(this.buttonAuditKeep);
            this.panelAudit.Controls.Add(this.buttonAuditSession);
            this.panelAudit.Controls.Add(this.buttonAuditProfile);
            this.panelAudit.Controls.Add(this.buttonAuditBrowser);
            this.panelAudit.Controls.Add(this.labelAuditCount);
            this.panelAudit.Controls.Add(this.gridAudit);
            this.panelAudit.Controls.Add(this.labelAuditDetail);
            this.panelAudit.Location = new System.Drawing.Point(30, 30);
            this.panelAudit.Name = "panelAudit";
            this.panelAudit.Size = new System.Drawing.Size(640, 270);
            //
            // labelAuditTitle
            //
            this.labelAuditTitle.AutoSize = false;
            this.labelAuditTitle.Font = new System.Drawing.Font("default", 14F, System.Drawing.FontStyle.Bold);
            this.labelAuditTitle.Location = new System.Drawing.Point(20, 14);
            this.labelAuditTitle.Name = "labelAuditTitle";
            this.labelAuditTitle.Size = new System.Drawing.Size(340, 30);
            this.labelAuditTitle.Text = "Static-state audit · LegacyOrderDesk";
            //
            // labelStatus
            //
            this.labelStatus.AutoSize = false;
            this.labelStatus.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(31, 157, 87);
            this.labelStatus.Location = new System.Drawing.Point(360, 18);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(262, 24);
            this.labelStatus.Text = "● loading";
            this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // filter buttons: All + the four verdicts (Remove has no button)
            //
            this.buttonAuditAll.Location = new System.Drawing.Point(20, 50);
            this.buttonAuditAll.Name = "buttonAuditAll";
            this.buttonAuditAll.Size = new System.Drawing.Size(50, 28);
            this.buttonAuditAll.Text = "All";
            this.buttonAuditAll.Click += new System.EventHandler(this.buttonAuditAll_Click);
            this.buttonAuditKeep.Location = new System.Drawing.Point(76, 50);
            this.buttonAuditKeep.Name = "buttonAuditKeep";
            this.buttonAuditKeep.Size = new System.Drawing.Size(96, 28);
            this.buttonAuditKeep.Text = "Keep static";
            this.buttonAuditKeep.ToolTipText = "Immutable, thread-safe, user-independent: the value would NOT differ for two users.";
            this.buttonAuditKeep.Click += new System.EventHandler(this.buttonAuditKeep_Click);
            this.buttonAuditSession.Location = new System.Drawing.Point(178, 50);
            this.buttonAuditSession.Name = "buttonAuditSession";
            this.buttonAuditSession.Size = new System.Drawing.Size(80, 28);
            this.buttonAuditSession.Text = "Session";
            this.buttonAuditSession.ToolTipText = "Per-user workflow state → UserSessionContext in Application.Session.";
            this.buttonAuditSession.Click += new System.EventHandler(this.buttonAuditSession_Click);
            this.buttonAuditProfile.Location = new System.Drawing.Point(264, 50);
            this.buttonAuditProfile.Name = "buttonAuditProfile";
            this.buttonAuditProfile.Size = new System.Drawing.Size(100, 28);
            this.buttonAuditProfile.Text = "Profile store";
            this.buttonAuditProfile.ToolTipText = "Per-user settings the server owns: App_Data/profiles/<user>.json.";
            this.buttonAuditProfile.Click += new System.EventHandler(this.buttonAuditProfile_Click);
            this.buttonAuditBrowser.Location = new System.Drawing.Point(370, 50);
            this.buttonAuditBrowser.Name = "buttonAuditBrowser";
            this.buttonAuditBrowser.Size = new System.Drawing.Size(120, 28);
            this.buttonAuditBrowser.Text = "Browser storage";
            this.buttonAuditBrowser.ToolTipText = "Device-bound UI preferences in localStorage.";
            this.buttonAuditBrowser.Click += new System.EventHandler(this.buttonAuditBrowser_Click);
            //
            // labelAuditCount
            //
            this.labelAuditCount.AutoSize = false;
            this.labelAuditCount.Font = new System.Drawing.Font("default", 9F);
            this.labelAuditCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelAuditCount.Location = new System.Drawing.Point(496, 52);
            this.labelAuditCount.Name = "labelAuditCount";
            this.labelAuditCount.Size = new System.Drawing.Size(126, 24);
            this.labelAuditCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // gridAudit
            //
            this.gridAudit.AllowUserToAddRows = false;
            this.gridAudit.AllowUserToDeleteRows = false;
            this.gridAudit.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colMember, this.colDeclaredIn, this.colKind, this.colVerdict });
            this.gridAudit.Location = new System.Drawing.Point(20, 86);
            this.gridAudit.MultiSelect = false;
            this.gridAudit.Name = "gridAudit";
            this.gridAudit.ReadOnly = true;
            this.gridAudit.RowHeadersVisible = false;
            this.gridAudit.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridAudit.Size = new System.Drawing.Size(602, 120);
            this.gridAudit.SelectionChanged += new System.EventHandler(this.gridAudit_SelectionChanged);
            this.colMember.HeaderText = "Static member"; this.colMember.Name = "colMember"; this.colMember.Width = 210; this.colMember.ReadOnly = true;
            this.colDeclaredIn.HeaderText = "Declared in"; this.colDeclaredIn.Name = "colDeclaredIn"; this.colDeclaredIn.Width = 160; this.colDeclaredIn.ReadOnly = true;
            this.colKind.HeaderText = "Kind"; this.colKind.Name = "colKind"; this.colKind.Width = 104; this.colKind.ReadOnly = true;
            this.colVerdict.HeaderText = "Verdict"; this.colVerdict.Name = "colVerdict"; this.colVerdict.Width = 124; this.colVerdict.ReadOnly = true;
            //
            // labelAuditDetail  (why + replacement of the selected row)
            //
            this.labelAuditDetail.AutoSize = false;
            this.labelAuditDetail.Font = new System.Drawing.Font("monospace", 9F);
            this.labelAuditDetail.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelAuditDetail.Location = new System.Drawing.Point(20, 210);
            this.labelAuditDetail.Name = "labelAuditDetail";
            this.labelAuditDetail.Size = new System.Drawing.Size(602, 52);
            this.labelAuditDetail.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelIsolation  (card B: lab steps 3–5 — the Orders screen on two stores, tested with two sessions)
            //
            this.panelIsolation.BackColor = System.Drawing.Color.White;
            this.panelIsolation.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelIsolation.Controls.Add(this.labelIsolationTitle);
            this.panelIsolation.Controls.Add(this.radioLegacy);
            this.panelIsolation.Controls.Add(this.radioContext);
            this.panelIsolation.Controls.Add(this.labelOrdersCount);
            this.panelIsolation.Controls.Add(this.comboFilter);
            this.panelIsolation.Controls.Add(this.comboCustomer);
            this.panelIsolation.Controls.Add(this.buttonSignInKelly);
            this.panelIsolation.Controls.Add(this.buttonSignInSam);
            this.panelIsolation.Controls.Add(this.gridOrders);
            this.panelIsolation.Controls.Add(this.labelStores);
            this.panelIsolation.Controls.Add(this.buttonReread);
            this.panelIsolation.Controls.Add(this.buttonSecondSession);
            this.panelIsolation.Controls.Add(this.labelBanner);
            this.panelIsolation.Location = new System.Drawing.Point(30, 314);
            this.panelIsolation.Name = "panelIsolation";
            this.panelIsolation.Size = new System.Drawing.Size(640, 340);
            //
            // labelIsolationTitle
            //
            this.labelIsolationTitle.AutoSize = false;
            this.labelIsolationTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelIsolationTitle.Location = new System.Drawing.Point(20, 12);
            this.labelIsolationTitle.Name = "labelIsolationTitle";
            this.labelIsolationTitle.Size = new System.Drawing.Size(602, 28);
            this.labelIsolationTitle.Text = "Two-session isolation test · Orders (same screen, two stores)";
            //
            // mode toggle: which store the screen reads and writes
            //
            this.radioLegacy.Location = new System.Drawing.Point(20, 46);
            this.radioLegacy.Name = "radioLegacy";
            this.radioLegacy.Size = new System.Drawing.Size(150, 24);
            this.radioLegacy.Text = "Legacy statics";
            this.radioLegacy.ToolTipText = "✕ reads/writes Legacy.AppState.CurrentUser / CurrentCustomer / CurrentFilter — one slot for the whole server.";
            this.radioLegacy.CheckedChanged += new System.EventHandler(this.radioLegacy_CheckedChanged);
            this.radioContext.Location = new System.Drawing.Point(176, 46);
            this.radioContext.Name = "radioContext";
            this.radioContext.Size = new System.Drawing.Size(190, 24);
            this.radioContext.Text = "UserContext (session)";
            this.radioContext.ToolTipText = "✓ reads/writes SessionContext.Current — one typed context per browser session.";
            this.radioContext.CheckedChanged += new System.EventHandler(this.radioContext_CheckedChanged);
            //
            // labelOrdersCount
            //
            this.labelOrdersCount.AutoSize = false;
            this.labelOrdersCount.Font = new System.Drawing.Font("default", 9F);
            this.labelOrdersCount.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.labelOrdersCount.Location = new System.Drawing.Point(372, 46);
            this.labelOrdersCount.Name = "labelOrdersCount";
            this.labelOrdersCount.Size = new System.Drawing.Size(250, 24);
            this.labelOrdersCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // comboFilter / comboCustomer  (write to the active store, then re-filter)
            //
            this.comboFilter.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboFilter.Location = new System.Drawing.Point(20, 76);
            this.comboFilter.Name = "comboFilter";
            this.comboFilter.Size = new System.Drawing.Size(120, 28);
            this.comboFilter.ToolTipText = "Status filter — AppState.CurrentFilter or UserContext.CurrentFilter, depending on the mode.";
            this.comboFilter.SelectedIndexChanged += new System.EventHandler(this.comboFilter_SelectedIndexChanged);
            this.comboCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboCustomer.Location = new System.Drawing.Point(150, 76);
            this.comboCustomer.Name = "comboCustomer";
            this.comboCustomer.Size = new System.Drawing.Size(170, 28);
            this.comboCustomer.ToolTipText = "Current customer — AppState.CurrentCustomer or UserContext.CurrentCustomerId.";
            this.comboCustomer.SelectedIndexChanged += new System.EventHandler(this.comboCustomer_SelectedIndexChanged);
            //
            // sign-in buttons: kelly (Acme) and sam (Globex)
            //
            this.buttonSignInKelly.Location = new System.Drawing.Point(330, 74);
            this.buttonSignInKelly.Name = "buttonSignInKelly";
            this.buttonSignInKelly.Size = new System.Drawing.Size(110, 32);
            this.buttonSignInKelly.Text = "Sign in as kelly";
            this.buttonSignInKelly.ToolTipText = "kelly · Acme · Northwind Traders · filter Open";
            this.buttonSignInKelly.Click += new System.EventHandler(this.buttonSignInKelly_Click);
            this.buttonSignInSam.Location = new System.Drawing.Point(446, 74);
            this.buttonSignInSam.Name = "buttonSignInSam";
            this.buttonSignInSam.Size = new System.Drawing.Size(100, 32);
            this.buttonSignInSam.Text = "Sign in as sam";
            this.buttonSignInSam.ToolTipText = "sam · Globex · Fabrikam Inc · filter Invoiced";
            this.buttonSignInSam.Click += new System.EventHandler(this.buttonSignInSam_Click);
            //
            // gridOrders  (the five walkthrough orders, filtered by the active store)
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToDeleteRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] { this.colOrder, this.colCustomer, this.colTotal, this.colStatus });
            this.gridOrders.Location = new System.Drawing.Point(20, 112);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(290, 124);
            this.colOrder.HeaderText = "Order"; this.colOrder.Name = "colOrder"; this.colOrder.Width = 48; this.colOrder.ReadOnly = true;
            this.colCustomer.HeaderText = "Customer"; this.colCustomer.Name = "colCustomer"; this.colCustomer.Width = 116; this.colCustomer.ReadOnly = true;
            this.colTotal.HeaderText = "Total"; this.colTotal.Name = "colTotal"; this.colTotal.Width = 62; this.colTotal.ReadOnly = true;
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "N2";
            this.colStatus.HeaderText = "Status"; this.colStatus.Name = "colStatus"; this.colStatus.Width = 60; this.colStatus.ReadOnly = true;
            //
            // labelStores  (the static slot and this session's context, side by side)
            //
            this.labelStores.AutoSize = false;
            this.labelStores.Font = new System.Drawing.Font("monospace", 9F);
            this.labelStores.Location = new System.Drawing.Point(322, 112);
            this.labelStores.Name = "labelStores";
            this.labelStores.Size = new System.Drawing.Size(300, 124);
            this.labelStores.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // re-read + second session
            //
            this.buttonReread.Location = new System.Drawing.Point(20, 244);
            this.buttonReread.Name = "buttonReread";
            this.buttonReread.Size = new System.Drawing.Size(110, 32);
            this.buttonReread.Text = "Re-read state";
            this.buttonReread.ToolTipText = "Read the active store again and compare it with what this page last wrote.";
            this.buttonReread.Click += new System.EventHandler(this.buttonReread_Click);
            this.buttonSecondSession.Location = new System.Drawing.Point(136, 244);
            this.buttonSecondSession.Name = "buttonSecondSession";
            this.buttonSecondSession.Size = new System.Drawing.Size(150, 32);
            this.buttonSecondSession.Text = "Open second session ↗";
            this.buttonSecondSession.ToolTipText = "Application.Navigate(Application.Url, \"_blank\") — a second browser session in the same process.";
            this.buttonSecondSession.Click += new System.EventHandler(this.buttonSecondSession_Click);
            //
            // labelBanner
            //
            this.labelBanner.AutoSize = false;
            this.labelBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.labelBanner.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.labelBanner.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.labelBanner.Location = new System.Drawing.Point(20, 284);
            this.labelBanner.Name = "labelBanner";
            this.labelBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.labelBanner.Size = new System.Drawing.Size(602, 46);
            this.labelBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelBanner.Visible = false;
            //
            // trace  (the migration log)
            //
            this.trace.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.trace.Location = new System.Drawing.Point(690, 30);
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(628, 360);
            //
            // panelSettings  (card C: registry → profile store → browser storage, plus logout/timeout cleanup)
            //
            this.panelSettings.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelSettings.BackColor = System.Drawing.Color.White;
            this.panelSettings.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSettings.Controls.Add(this.labelSettingsTitle);
            this.panelSettings.Controls.Add(this.labelSettingsValues);
            this.panelSettings.Controls.Add(this.comboDensity);
            this.panelSettings.Controls.Add(this.buttonRegistry);
            this.panelSettings.Controls.Add(this.buttonProfile);
            this.panelSettings.Controls.Add(this.buttonBrowser);
            this.panelSettings.Controls.Add(this.buttonSignOut);
            this.panelSettings.Controls.Add(this.buttonTimeout);
            this.panelSettings.Controls.Add(this.progressTimeout);
            this.panelSettings.Controls.Add(this.buttonClear);
            this.panelSettings.Location = new System.Drawing.Point(690, 404);
            this.panelSettings.Name = "panelSettings";
            this.panelSettings.Size = new System.Drawing.Size(628, 250);
            //
            // labelSettingsTitle
            //
            this.labelSettingsTitle.AutoSize = false;
            this.labelSettingsTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelSettingsTitle.Location = new System.Drawing.Point(20, 12);
            this.labelSettingsTitle.Name = "labelSettingsTitle";
            this.labelSettingsTitle.Size = new System.Drawing.Size(588, 28);
            this.labelSettingsTitle.Text = "Settings storage · registry → server profile → browser  ·  session cleanup";
            //
            // labelSettingsValues
            //
            this.labelSettingsValues.AutoSize = false;
            this.labelSettingsValues.Font = new System.Drawing.Font("monospace", 9F);
            this.labelSettingsValues.Location = new System.Drawing.Point(20, 44);
            this.labelSettingsValues.Name = "labelSettingsValues";
            this.labelSettingsValues.Size = new System.Drawing.Size(588, 74);
            this.labelSettingsValues.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // comboDensity + the three destinations
            //
            this.comboDensity.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.comboDensity.Location = new System.Drawing.Point(20, 126);
            this.comboDensity.Name = "comboDensity";
            this.comboDensity.Size = new System.Drawing.Size(120, 28);
            this.comboDensity.ToolTipText = "GridDensity — the setting LegacyOrderDesk kept in HKCU.";
            this.buttonRegistry.Location = new System.Drawing.Point(150, 124);
            this.buttonRegistry.Name = "buttonRegistry";
            this.buttonRegistry.Size = new System.Drawing.Size(120, 32);
            this.buttonRegistry.Text = "Legacy registry";
            this.buttonRegistry.ToolTipText = "✕ RegistrySettings.Save → HKCU on THIS server, as the service account.";
            this.buttonRegistry.Click += new System.EventHandler(this.buttonRegistry_Click);
            this.buttonProfile.Location = new System.Drawing.Point(276, 124);
            this.buttonProfile.Name = "buttonProfile";
            this.buttonProfile.Size = new System.Drawing.Size(110, 32);
            this.buttonProfile.Text = "Profile store";
            this.buttonProfile.ToolTipText = "✓ UserProfileStore → App_Data/profiles/<user>.json (server-owned, per user).";
            this.buttonProfile.Click += new System.EventHandler(this.buttonProfile_Click);
            this.buttonBrowser.Location = new System.Drawing.Point(392, 124);
            this.buttonBrowser.Name = "buttonBrowser";
            this.buttonBrowser.Size = new System.Drawing.Size(120, 32);
            this.buttonBrowser.Text = "Browser storage";
            this.buttonBrowser.ToolTipText = "✓ localStorage via Application.Eval / EvalAsync (device-bound UI preference).";
            this.buttonBrowser.Click += new System.EventHandler(this.buttonBrowser_Click);
            //
            // logout / timeout cleanup
            //
            this.buttonSignOut.Location = new System.Drawing.Point(20, 166);
            this.buttonSignOut.Name = "buttonSignOut";
            this.buttonSignOut.Size = new System.Drawing.Size(90, 32);
            this.buttonSignOut.Text = "Sign out";
            this.buttonSignOut.ToolTipText = "SessionContext.Reset() + the cleanup routine (temp files, report jobs, locks).";
            this.buttonSignOut.Click += new System.EventHandler(this.buttonSignOut_Click);
            this.buttonTimeout.Location = new System.Drawing.Point(116, 166);
            this.buttonTimeout.Name = "buttonTimeout";
            this.buttonTimeout.Size = new System.Drawing.Size(130, 32);
            this.buttonTimeout.Text = "Simulate timeout";
            this.buttonTimeout.ToolTipText = "5 s countdown, then the cleanup routine Application.SessionTimeout → ApplicationExit would run — without ending the session.";
            this.buttonTimeout.Click += new System.EventHandler(this.buttonTimeout_Click);
            this.progressTimeout.Location = new System.Drawing.Point(252, 172);
            this.progressTimeout.Maximum = 100;
            this.progressTimeout.Minimum = 0;
            this.progressTimeout.Name = "progressTimeout";
            this.progressTimeout.Size = new System.Drawing.Size(200, 20);
            this.progressTimeout.Visible = false;
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(550, 166);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(58, 32);
            this.buttonClear.Text = "Clear";
            this.buttonClear.ToolTipText = "Clear the trace.";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // timerTimeout  (the countdown of the simulated timeout)
            //
            this.timerTimeout.Interval = 1000;
            this.timerTimeout.Tick += new System.EventHandler(this.timerTimeout_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelAudit);
            this.Controls.Add(this.panelIsolation);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelSettings);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1348, 680);
            this.Text = "OrderDesk — Sessions & Multi-User Safety";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelAudit.ResumeLayout(false);
            this.panelIsolation.ResumeLayout(false);
            this.panelSettings.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelAudit;
        private Wisej.Web.Label labelAuditTitle;
        private Wisej.Web.Label labelStatus;
        private Wisej.Web.Button buttonAuditAll;
        private Wisej.Web.Button buttonAuditKeep;
        private Wisej.Web.Button buttonAuditSession;
        private Wisej.Web.Button buttonAuditProfile;
        private Wisej.Web.Button buttonAuditBrowser;
        private Wisej.Web.Label labelAuditCount;
        private Wisej.Web.DataGridView gridAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colMember;
        private Wisej.Web.DataGridViewTextBoxColumn colDeclaredIn;
        private Wisej.Web.DataGridViewTextBoxColumn colKind;
        private Wisej.Web.DataGridViewTextBoxColumn colVerdict;
        private Wisej.Web.Label labelAuditDetail;
        private Wisej.Web.Panel panelIsolation;
        private Wisej.Web.Label labelIsolationTitle;
        private Wisej.Web.RadioButton radioLegacy;
        private Wisej.Web.RadioButton radioContext;
        private Wisej.Web.Label labelOrdersCount;
        private Wisej.Web.ComboBox comboFilter;
        private Wisej.Web.ComboBox comboCustomer;
        private Wisej.Web.Button buttonSignInKelly;
        private Wisej.Web.Button buttonSignInSam;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colOrder;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.Label labelStores;
        private Wisej.Web.Button buttonReread;
        private Wisej.Web.Button buttonSecondSession;
        private Wisej.Web.Label labelBanner;
        private OrderDesk.Views.TracePanel trace;
        private Wisej.Web.Panel panelSettings;
        private Wisej.Web.Label labelSettingsTitle;
        private Wisej.Web.Label labelSettingsValues;
        private Wisej.Web.ComboBox comboDensity;
        private Wisej.Web.Button buttonRegistry;
        private Wisej.Web.Button buttonProfile;
        private Wisej.Web.Button buttonBrowser;
        private Wisej.Web.Button buttonSignOut;
        private Wisej.Web.Button buttonTimeout;
        private Wisej.Web.ProgressBar progressTimeout;
        private Wisej.Web.Button buttonClear;
        private Wisej.Web.Timer timerTimeout;
    }
}
