namespace OrderDesk
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
            this.panelAppBar = new Wisej.Web.Panel();
            this.lblAppTitle = new Wisej.Web.Label();
            this.lblStatus = new Wisej.Web.Label();
            this.trace = new OrderDesk.Shared.TracePanel();
            this.panelSignIn = new Wisej.Web.Panel();
            this.lblSignInTitle = new Wisej.Web.Label();
            this.lblUser = new Wisej.Web.Label();
            this.cmbUser = new Wisej.Web.ComboBox();
            this.btnSignIn = new Wisej.Web.Button();
            this.lblSignedIn = new Wisej.Web.Label();
            this.lblSession = new Wisej.Web.Label();
            this.panelState = new Wisej.Web.Panel();
            this.lblStateTitle = new Wisej.Web.Label();
            this.rdoStatic = new Wisej.Web.RadioButton();
            this.rdoSession = new Wisej.Web.RadioButton();
            this.lblCustomer = new Wisej.Web.Label();
            this.cmbCustomer = new Wisej.Web.ComboBox();
            this.lblFilter = new Wisej.Web.Label();
            this.cmbFilter = new Wisej.Web.ComboBox();
            this.panelReadback = new Wisej.Web.Panel();
            this.lblKeyUser = new Wisej.Web.Label();
            this.lblValUser = new Wisej.Web.Label();
            this.lblKeyCompany = new Wisej.Web.Label();
            this.lblValCompany = new Wisej.Web.Label();
            this.lblKeyCustomer = new Wisej.Web.Label();
            this.lblValCustomer = new Wisej.Web.Label();
            this.lblKeyFilter = new Wisej.Web.Label();
            this.lblValFilter = new Wisej.Web.Label();
            this.lblKeyStore = new Wisej.Web.Label();
            this.lblValStore = new Wisej.Web.Label();
            this.lblStoreLocation = new Wisej.Web.Label();
            this.panelOrders = new Wisej.Web.Panel();
            this.lblOrdersTitle = new Wisej.Web.Label();
            this.gridOrders = new Wisej.Web.DataGridView();
            this.colId = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colCustomer = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colTotal = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colStatus = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colMatch = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblAuditTitle = new Wisej.Web.Label();
            this.gridAudit = new Wisej.Web.DataGridView();
            this.colAuditStatic = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditClass = new Wisej.Web.DataGridViewTextBoxColumn();
            this.colAuditDecision = new Wisej.Web.DataGridViewTextBoxColumn();
            this.lblBanner = new Wisej.Web.Label();
            this.btnReadBack = new Wisej.Web.Button();
            this.btnCorrupt = new Wisej.Web.Button();
            this.btnUseContext = new Wisej.Web.Button();
            this.btnReplay = new Wisej.Web.Button();
            this.btnRegistry = new Wisej.Web.Button();
            this.btnServerProfile = new Wisej.Web.Button();
            this.btnSignOut = new Wisej.Web.Button();
            this.timerReplay = new Wisej.Web.Timer(this.components);
            this.panelAppBar.SuspendLayout();
            this.panelSignIn.SuspendLayout();
            this.panelState.SuspendLayout();
            this.panelReadback.SuspendLayout();
            this.panelOrders.SuspendLayout();
            this.SuspendLayout();
            //
            // panelAppBar
            //
            this.panelAppBar.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.panelAppBar.Controls.Add(this.lblAppTitle);
            this.panelAppBar.Controls.Add(this.lblStatus);
            this.panelAppBar.Dock = Wisej.Web.DockStyle.Top;
            this.panelAppBar.Name = "panelAppBar";
            this.panelAppBar.Size = new System.Drawing.Size(1400, 44);
            //
            // lblAppTitle
            //
            this.lblAppTitle.AutoSize = false;
            this.lblAppTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.Location = new System.Drawing.Point(20, 0);
            this.lblAppTitle.Name = "lblAppTitle";
            this.lblAppTitle.Size = new System.Drawing.Size(560, 44);
            this.lblAppTitle.Text = "OrderDesk — Sessions, Statics & Multi-User Safety (Module 4)";
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblStatus.ForeColor = System.Drawing.Color.White;
            this.lblStatus.Location = new System.Drawing.Point(600, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 44);
            this.lblStatus.Text = "● idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // trace
            //
            this.trace.Dock = Wisej.Web.DockStyle.Right;
            this.trace.Name = "trace";
            this.trace.Size = new System.Drawing.Size(560, 716);
            //
            // panelSignIn  (the LoginForm, ported: a card instead of a modal dialog)
            //
            this.panelSignIn.BackColor = System.Drawing.Color.White;
            this.panelSignIn.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelSignIn.Controls.Add(this.lblSignInTitle);
            this.panelSignIn.Controls.Add(this.lblUser);
            this.panelSignIn.Controls.Add(this.cmbUser);
            this.panelSignIn.Controls.Add(this.btnSignIn);
            this.panelSignIn.Controls.Add(this.lblSignedIn);
            this.panelSignIn.Controls.Add(this.lblSession);
            this.panelSignIn.Location = new System.Drawing.Point(20, 54);
            this.panelSignIn.Name = "panelSignIn";
            this.panelSignIn.Size = new System.Drawing.Size(780, 72);
            //
            // lblSignInTitle
            //
            this.lblSignInTitle.AutoSize = false;
            this.lblSignInTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblSignInTitle.Location = new System.Drawing.Point(14, 6);
            this.lblSignInTitle.Name = "lblSignInTitle";
            this.lblSignInTitle.Size = new System.Drawing.Size(300, 24);
            this.lblSignInTitle.Text = "Sign in";
            this.lblSignInTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblUser
            //
            this.lblUser.AutoSize = false;
            this.lblUser.Location = new System.Drawing.Point(14, 36);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(40, 26);
            this.lblUser.Text = "User";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbUser
            //
            this.cmbUser.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbUser.Items.AddRange(new object[] { "kelly", "sam", "dana", "priya" });
            this.cmbUser.Location = new System.Drawing.Point(58, 36);
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(150, 26);
            //
            // btnSignIn
            //
            this.btnSignIn.Location = new System.Drawing.Point(220, 35);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(100, 28);
            this.btnSignIn.Text = "Sign in";
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            //
            // lblSignedIn
            //
            this.lblSignedIn.AutoSize = false;
            this.lblSignedIn.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblSignedIn.Location = new System.Drawing.Point(340, 6);
            this.lblSignedIn.Name = "lblSignedIn";
            this.lblSignedIn.Size = new System.Drawing.Size(426, 24);
            this.lblSignedIn.Text = "Not signed in — pick a user and click Sign in.";
            this.lblSignedIn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSession
            //
            this.lblSession.AutoSize = false;
            this.lblSession.Font = new System.Drawing.Font("monospace", 9F);
            this.lblSession.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblSession.Location = new System.Drawing.Point(340, 34);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(426, 26);
            this.lblSession.Text = "Application.SessionId · SessionCount";
            this.lblSession.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelState  (the State store card: legacy static vs session context)
            //
            this.panelState.BackColor = System.Drawing.Color.White;
            this.panelState.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelState.Controls.Add(this.lblStateTitle);
            this.panelState.Controls.Add(this.rdoStatic);
            this.panelState.Controls.Add(this.rdoSession);
            this.panelState.Controls.Add(this.lblCustomer);
            this.panelState.Controls.Add(this.cmbCustomer);
            this.panelState.Controls.Add(this.lblFilter);
            this.panelState.Controls.Add(this.cmbFilter);
            this.panelState.Controls.Add(this.panelReadback);
            this.panelState.Controls.Add(this.lblStoreLocation);
            this.panelState.Location = new System.Drawing.Point(20, 136);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(370, 420);
            //
            // lblStateTitle
            //
            this.lblStateTitle.AutoSize = false;
            this.lblStateTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblStateTitle.Location = new System.Drawing.Point(14, 6);
            this.lblStateTitle.Name = "lblStateTitle";
            this.lblStateTitle.Size = new System.Drawing.Size(340, 24);
            this.lblStateTitle.Text = "State store — where \"current user / customer / filter\" live";
            this.lblStateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // rdoStatic
            //
            this.rdoStatic.Checked = true;
            this.rdoStatic.Location = new System.Drawing.Point(14, 36);
            this.rdoStatic.Name = "rdoStatic";
            this.rdoStatic.Size = new System.Drawing.Size(340, 24);
            this.rdoStatic.Text = "Legacy static (AppState) ✕ — one slot for the whole server";
            this.rdoStatic.CheckedChanged += new System.EventHandler(this.rdoStore_CheckedChanged);
            //
            // rdoSession
            //
            this.rdoSession.Location = new System.Drawing.Point(14, 60);
            this.rdoSession.Name = "rdoSession";
            this.rdoSession.Size = new System.Drawing.Size(340, 24);
            this.rdoSession.Text = "Session context (UserContext) ✓ — one per session";
            this.rdoSession.CheckedChanged += new System.EventHandler(this.rdoStore_CheckedChanged);
            //
            // lblCustomer
            //
            this.lblCustomer.AutoSize = false;
            this.lblCustomer.Location = new System.Drawing.Point(14, 96);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(90, 26);
            this.lblCustomer.Text = "Customer";
            this.lblCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbCustomer
            //
            this.cmbCustomer.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbCustomer.Location = new System.Drawing.Point(110, 96);
            this.cmbCustomer.Name = "cmbCustomer";
            this.cmbCustomer.Size = new System.Drawing.Size(244, 26);
            this.cmbCustomer.SelectedIndexChanged += new System.EventHandler(this.cmbCustomer_SelectedIndexChanged);
            //
            // lblFilter
            //
            this.lblFilter.AutoSize = false;
            this.lblFilter.Location = new System.Drawing.Point(14, 130);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(90, 26);
            this.lblFilter.Text = "Filter";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // cmbFilter
            //
            this.cmbFilter.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cmbFilter.Items.AddRange(new object[] { "Open", "InProgress", "Shipped", "Invoiced", "Hold" });
            this.cmbFilter.Location = new System.Drawing.Point(110, 130);
            this.cmbFilter.Name = "cmbFilter";
            this.cmbFilter.Size = new System.Drawing.Size(244, 26);
            this.cmbFilter.SelectedIndexChanged += new System.EventHandler(this.cmbFilter_SelectedIndexChanged);
            //
            // panelReadback  (what the ACTIVE store returns right now)
            //
            this.panelReadback.BackColor = System.Drawing.Color.FromArgb(244, 246, 249);
            this.panelReadback.Controls.Add(this.lblKeyUser);
            this.panelReadback.Controls.Add(this.lblValUser);
            this.panelReadback.Controls.Add(this.lblKeyCompany);
            this.panelReadback.Controls.Add(this.lblValCompany);
            this.panelReadback.Controls.Add(this.lblKeyCustomer);
            this.panelReadback.Controls.Add(this.lblValCustomer);
            this.panelReadback.Controls.Add(this.lblKeyFilter);
            this.panelReadback.Controls.Add(this.lblValFilter);
            this.panelReadback.Controls.Add(this.lblKeyStore);
            this.panelReadback.Controls.Add(this.lblValStore);
            this.panelReadback.Location = new System.Drawing.Point(14, 170);
            this.panelReadback.Name = "panelReadback";
            this.panelReadback.Size = new System.Drawing.Size(340, 190);
            //
            // readback rows (key · value)
            //
            this.lblKeyUser.AutoSize = false;
            this.lblKeyUser.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblKeyUser.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblKeyUser.Location = new System.Drawing.Point(12, 10);
            this.lblKeyUser.Name = "lblKeyUser";
            this.lblKeyUser.Size = new System.Drawing.Size(130, 26);
            this.lblKeyUser.Text = "CURRENT USER";
            this.lblKeyUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblValUser.AutoSize = false;
            this.lblValUser.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.lblValUser.Location = new System.Drawing.Point(150, 10);
            this.lblValUser.Name = "lblValUser";
            this.lblValUser.Size = new System.Drawing.Size(180, 26);
            this.lblValUser.Text = "—";
            this.lblValUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKeyCompany.AutoSize = false;
            this.lblKeyCompany.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblKeyCompany.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblKeyCompany.Location = new System.Drawing.Point(12, 44);
            this.lblKeyCompany.Name = "lblKeyCompany";
            this.lblKeyCompany.Size = new System.Drawing.Size(130, 26);
            this.lblKeyCompany.Text = "COMPANY";
            this.lblKeyCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblValCompany.AutoSize = false;
            this.lblValCompany.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.lblValCompany.Location = new System.Drawing.Point(150, 44);
            this.lblValCompany.Name = "lblValCompany";
            this.lblValCompany.Size = new System.Drawing.Size(180, 26);
            this.lblValCompany.Text = "—";
            this.lblValCompany.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKeyCustomer.AutoSize = false;
            this.lblKeyCustomer.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblKeyCustomer.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblKeyCustomer.Location = new System.Drawing.Point(12, 78);
            this.lblKeyCustomer.Name = "lblKeyCustomer";
            this.lblKeyCustomer.Size = new System.Drawing.Size(130, 26);
            this.lblKeyCustomer.Text = "CURRENT CUSTOMER";
            this.lblKeyCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblValCustomer.AutoSize = false;
            this.lblValCustomer.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.lblValCustomer.Location = new System.Drawing.Point(150, 78);
            this.lblValCustomer.Name = "lblValCustomer";
            this.lblValCustomer.Size = new System.Drawing.Size(180, 26);
            this.lblValCustomer.Text = "—";
            this.lblValCustomer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKeyFilter.AutoSize = false;
            this.lblKeyFilter.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblKeyFilter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblKeyFilter.Location = new System.Drawing.Point(12, 112);
            this.lblKeyFilter.Name = "lblKeyFilter";
            this.lblKeyFilter.Size = new System.Drawing.Size(130, 26);
            this.lblKeyFilter.Text = "ACTIVE FILTER";
            this.lblKeyFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblValFilter.AutoSize = false;
            this.lblValFilter.Font = new System.Drawing.Font("monospace", 10F, System.Drawing.FontStyle.Bold);
            this.lblValFilter.Location = new System.Drawing.Point(150, 112);
            this.lblValFilter.Name = "lblValFilter";
            this.lblValFilter.Size = new System.Drawing.Size(180, 26);
            this.lblValFilter.Text = "—";
            this.lblValFilter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblKeyStore.AutoSize = false;
            this.lblKeyStore.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblKeyStore.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblKeyStore.Location = new System.Drawing.Point(12, 150);
            this.lblKeyStore.Name = "lblKeyStore";
            this.lblKeyStore.Size = new System.Drawing.Size(130, 26);
            this.lblKeyStore.Text = "READ FROM";
            this.lblKeyStore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblValStore.AutoSize = false;
            this.lblValStore.Font = new System.Drawing.Font("monospace", 9F, System.Drawing.FontStyle.Bold);
            this.lblValStore.Location = new System.Drawing.Point(150, 150);
            this.lblValStore.Name = "lblValStore";
            this.lblValStore.Size = new System.Drawing.Size(180, 26);
            this.lblValStore.Text = "AppState (static)";
            this.lblValStore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblStoreLocation
            //
            this.lblStoreLocation.AutoSize = false;
            this.lblStoreLocation.Font = new System.Drawing.Font("default", 9F);
            this.lblStoreLocation.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblStoreLocation.Location = new System.Drawing.Point(14, 366);
            this.lblStoreLocation.Name = "lblStoreLocation";
            this.lblStoreLocation.Size = new System.Drawing.Size(340, 44);
            this.lblStoreLocation.Text = "static fields · one slot per PROCESS · shared by all sessions";
            this.lblStoreLocation.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            //
            // panelOrders  (the Orders screen for the signed-in user + the static-state audit)
            //
            this.panelOrders.BackColor = System.Drawing.Color.White;
            this.panelOrders.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelOrders.Controls.Add(this.lblOrdersTitle);
            this.panelOrders.Controls.Add(this.gridOrders);
            this.panelOrders.Controls.Add(this.lblAuditTitle);
            this.panelOrders.Controls.Add(this.gridAudit);
            this.panelOrders.Location = new System.Drawing.Point(400, 136);
            this.panelOrders.Name = "panelOrders";
            this.panelOrders.Size = new System.Drawing.Size(400, 420);
            //
            // lblOrdersTitle
            //
            this.lblOrdersTitle.AutoSize = false;
            this.lblOrdersTitle.Font = new System.Drawing.Font("default", 11F, System.Drawing.FontStyle.Bold);
            this.lblOrdersTitle.Location = new System.Drawing.Point(14, 6);
            this.lblOrdersTitle.Name = "lblOrdersTitle";
            this.lblOrdersTitle.Size = new System.Drawing.Size(372, 24);
            this.lblOrdersTitle.Text = "Orders — sign in to load";
            this.lblOrdersTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridOrders
            //
            this.gridOrders.AllowUserToAddRows = false;
            this.gridOrders.AllowUserToResizeRows = false;
            this.gridOrders.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colId,
                this.colCustomer,
                this.colTotal,
                this.colStatus,
                this.colMatch});
            this.gridOrders.Location = new System.Drawing.Point(14, 36);
            this.gridOrders.MultiSelect = false;
            this.gridOrders.Name = "gridOrders";
            this.gridOrders.ReadOnly = true;
            this.gridOrders.RowHeadersVisible = false;
            this.gridOrders.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridOrders.Size = new System.Drawing.Size(372, 176);
            //
            // colId
            //
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "Order";
            this.colId.Name = "colId";
            this.colId.Width = 60;
            //
            // colCustomer
            //
            this.colCustomer.DataPropertyName = "Customer";
            this.colCustomer.HeaderText = "Customer";
            this.colCustomer.Name = "colCustomer";
            this.colCustomer.Width = 130;
            //
            // colTotal
            //
            this.colTotal.DataPropertyName = "Total";
            this.colTotal.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleRight;
            this.colTotal.DefaultCellStyle.Format = "C2";
            this.colTotal.HeaderText = "Total";
            this.colTotal.Name = "colTotal";
            this.colTotal.Width = 84;
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.Width = 66;
            //
            // colMatch
            //
            this.colMatch.DataPropertyName = "Match";
            this.colMatch.DefaultCellStyle.Alignment = Wisej.Web.DataGridViewContentAlignment.MiddleCenter;
            this.colMatch.HeaderText = "Filter";
            this.colMatch.Name = "colMatch";
            this.colMatch.ToolTipText = "✓ = the row matches the active filter read from the active store";
            this.colMatch.Width = 30;
            //
            // lblAuditTitle
            //
            this.lblAuditTitle.AutoSize = false;
            this.lblAuditTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblAuditTitle.Location = new System.Drawing.Point(14, 218);
            this.lblAuditTitle.Name = "lblAuditTitle";
            this.lblAuditTitle.Size = new System.Drawing.Size(372, 22);
            this.lblAuditTitle.Text = "Static-state audit (lab steps 1–2)";
            this.lblAuditTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // gridAudit
            //
            this.gridAudit.AllowUserToAddRows = false;
            this.gridAudit.AllowUserToResizeRows = false;
            this.gridAudit.Columns.AddRange(new Wisej.Web.DataGridViewColumn[] {
                this.colAuditStatic,
                this.colAuditClass,
                this.colAuditDecision});
            this.gridAudit.Location = new System.Drawing.Point(14, 242);
            this.gridAudit.MultiSelect = false;
            this.gridAudit.Name = "gridAudit";
            this.gridAudit.ReadOnly = true;
            this.gridAudit.RowHeadersVisible = false;
            this.gridAudit.SelectionMode = Wisej.Web.DataGridViewSelectionMode.FullRowSelect;
            this.gridAudit.Size = new System.Drawing.Size(372, 170);
            //
            // colAuditStatic
            //
            this.colAuditStatic.DataPropertyName = "Static";
            this.colAuditStatic.HeaderText = "Static";
            this.colAuditStatic.Name = "colAuditStatic";
            this.colAuditStatic.Width = 150;
            //
            // colAuditClass
            //
            this.colAuditClass.DataPropertyName = "Class";
            this.colAuditClass.HeaderText = "Class";
            this.colAuditClass.Name = "colAuditClass";
            this.colAuditClass.Width = 82;
            //
            // colAuditDecision
            //
            this.colAuditDecision.DataPropertyName = "Decision";
            this.colAuditDecision.HeaderText = "Decision";
            this.colAuditDecision.Name = "colAuditDecision";
            this.colAuditDecision.Width = 138;
            //
            // lblBanner  (the finding / alarm banner — hidden until a path explains itself)
            //
            this.lblBanner.AutoSize = false;
            this.lblBanner.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblBanner.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblBanner.ForeColor = System.Drawing.Color.FromArgb(224, 86, 59);
            this.lblBanner.Location = new System.Drawing.Point(20, 566);
            this.lblBanner.Name = "lblBanner";
            this.lblBanner.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblBanner.Size = new System.Drawing.Size(780, 34);
            this.lblBanner.Text = "";
            this.lblBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBanner.Visible = false;
            //
            // button bar — row 1: the state paths
            //
            this.btnReadBack.Location = new System.Drawing.Point(20, 612);
            this.btnReadBack.Name = "btnReadBack";
            this.btnReadBack.Size = new System.Drawing.Size(110, 32);
            this.btnReadBack.Text = "Read back";
            this.btnReadBack.ToolTipText = "Log what the active store returns right now";
            this.btnReadBack.Click += new System.EventHandler(this.btnReadBack_Click);
            this.btnCorrupt.Location = new System.Drawing.Point(138, 612);
            this.btnCorrupt.Name = "btnCorrupt";
            this.btnCorrupt.Size = new System.Drawing.Size(170, 32);
            this.btnCorrupt.Text = "Corrupt from here ✕";
            this.btnCorrupt.ToolTipText = "A simulated second session (sam) writes the shared AppState static; kelly reads it back";
            this.btnCorrupt.Click += new System.EventHandler(this.btnCorrupt_Click);
            this.btnUseContext.Location = new System.Drawing.Point(316, 612);
            this.btnUseContext.Name = "btnUseContext";
            this.btnUseContext.Size = new System.Drawing.Size(170, 32);
            this.btnUseContext.Text = "Use UserContext ✓";
            this.btnUseContext.ToolTipText = "The same sequence through Application.Session — kelly keeps Northwind";
            this.btnUseContext.Click += new System.EventHandler(this.btnUseContext_Click);
            this.btnReplay.Location = new System.Drawing.Point(494, 612);
            this.btnReplay.Name = "btnReplay";
            this.btnReplay.Size = new System.Drawing.Size(180, 32);
            this.btnReplay.Text = "Two-session replay ▶";
            this.btnReplay.ToolTipText = "Timer: replays the video's two-session test step by step";
            this.btnReplay.Click += new System.EventHandler(this.btnReplay_Click);
            //
            // button bar — row 2: the preference paths + sign out
            //
            this.btnRegistry.Location = new System.Drawing.Point(20, 652);
            this.btnRegistry.Name = "btnRegistry";
            this.btnRegistry.Size = new System.Drawing.Size(180, 32);
            this.btnRegistry.Text = "Registry preference ✕";
            this.btnRegistry.ToolTipText = "Legacy UserPreferences.Get(\"LastUser\") — HKCU on the SERVER";
            this.btnRegistry.Click += new System.EventHandler(this.btnRegistry_Click);
            this.btnServerProfile.Location = new System.Drawing.Point(208, 652);
            this.btnServerProfile.Name = "btnServerProfile";
            this.btnServerProfile.Size = new System.Drawing.Size(160, 32);
            this.btnServerProfile.Text = "Server profile ✓";
            this.btnServerProfile.ToolTipText = "Per-user JSON under App_Data/users/<name>.json";
            this.btnServerProfile.Click += new System.EventHandler(this.btnServerProfile_Click);
            this.btnSignOut.Location = new System.Drawing.Point(376, 652);
            this.btnSignOut.Name = "btnSignOut";
            this.btnSignOut.Size = new System.Drawing.Size(110, 32);
            this.btnSignOut.Text = "Sign out";
            this.btnSignOut.ToolTipText = "UserContext.Clear() — this session only";
            this.btnSignOut.Click += new System.EventHandler(this.btnSignOut_Click);
            //
            // timerReplay
            //
            this.timerReplay.Interval = 700;
            this.timerReplay.Tick += new System.EventHandler(this.timerReplay_Tick);
            //
            // MainPage
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.Controls.Add(this.panelSignIn);
            this.Controls.Add(this.panelState);
            this.Controls.Add(this.panelOrders);
            this.Controls.Add(this.lblBanner);
            this.Controls.Add(this.btnReadBack);
            this.Controls.Add(this.btnCorrupt);
            this.Controls.Add(this.btnUseContext);
            this.Controls.Add(this.btnReplay);
            this.Controls.Add(this.btnRegistry);
            this.Controls.Add(this.btnServerProfile);
            this.Controls.Add(this.btnSignOut);
            this.Controls.Add(this.trace);
            this.Controls.Add(this.panelAppBar);
            this.Name = "MainPage";
            this.Size = new System.Drawing.Size(1400, 760);
            this.Text = "OrderDesk — Module 4";
            this.Load += new System.EventHandler(this.MainPage_Load);
            this.panelAppBar.ResumeLayout(false);
            this.panelSignIn.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            this.panelReadback.ResumeLayout(false);
            this.panelOrders.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelAppBar;
        private Wisej.Web.Label lblAppTitle;
        private Wisej.Web.Label lblStatus;
        private OrderDesk.Shared.TracePanel trace;
        private Wisej.Web.Panel panelSignIn;
        private Wisej.Web.Label lblSignInTitle;
        private Wisej.Web.Label lblUser;
        private Wisej.Web.ComboBox cmbUser;
        private Wisej.Web.Button btnSignIn;
        private Wisej.Web.Label lblSignedIn;
        private Wisej.Web.Label lblSession;
        private Wisej.Web.Panel panelState;
        private Wisej.Web.Label lblStateTitle;
        private Wisej.Web.RadioButton rdoStatic;
        private Wisej.Web.RadioButton rdoSession;
        private Wisej.Web.Label lblCustomer;
        private Wisej.Web.ComboBox cmbCustomer;
        private Wisej.Web.Label lblFilter;
        private Wisej.Web.ComboBox cmbFilter;
        private Wisej.Web.Panel panelReadback;
        private Wisej.Web.Label lblKeyUser;
        private Wisej.Web.Label lblValUser;
        private Wisej.Web.Label lblKeyCompany;
        private Wisej.Web.Label lblValCompany;
        private Wisej.Web.Label lblKeyCustomer;
        private Wisej.Web.Label lblValCustomer;
        private Wisej.Web.Label lblKeyFilter;
        private Wisej.Web.Label lblValFilter;
        private Wisej.Web.Label lblKeyStore;
        private Wisej.Web.Label lblValStore;
        private Wisej.Web.Label lblStoreLocation;
        private Wisej.Web.Panel panelOrders;
        private Wisej.Web.Label lblOrdersTitle;
        private Wisej.Web.DataGridView gridOrders;
        private Wisej.Web.DataGridViewTextBoxColumn colId;
        private Wisej.Web.DataGridViewTextBoxColumn colCustomer;
        private Wisej.Web.DataGridViewTextBoxColumn colTotal;
        private Wisej.Web.DataGridViewTextBoxColumn colStatus;
        private Wisej.Web.DataGridViewTextBoxColumn colMatch;
        private Wisej.Web.Label lblAuditTitle;
        private Wisej.Web.DataGridView gridAudit;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditStatic;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditClass;
        private Wisej.Web.DataGridViewTextBoxColumn colAuditDecision;
        private Wisej.Web.Label lblBanner;
        private Wisej.Web.Button btnReadBack;
        private Wisej.Web.Button btnCorrupt;
        private Wisej.Web.Button btnUseContext;
        private Wisej.Web.Button btnReplay;
        private Wisej.Web.Button btnRegistry;
        private Wisej.Web.Button btnServerProfile;
        private Wisej.Web.Button btnSignOut;
        private Wisej.Web.Timer timerReplay;
    }
}
