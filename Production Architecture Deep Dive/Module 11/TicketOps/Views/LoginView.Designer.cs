namespace TicketOps.Views
{
    partial class LoginView
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
            this.labelQuestion = new Wisej.Web.Label();
            this.labelUserCaption = new Wisej.Web.Label();
            this.textUser = new Wisej.Web.TextBox();
            this.labelPasswordCaption = new Wisej.Web.Label();
            this.textPassword = new Wisej.Web.TextBox();
            this.buttonSignIn = new Wisej.Web.Button();
            this.labelDemoCaption = new Wisej.Web.Label();
            this.buttonUseTechnician = new Wisej.Web.Button();
            this.buttonUseSupervisor = new Wisej.Web.Button();
            this.buttonUseAdmin = new Wisej.Web.Button();
            this.labelDemoNote = new Wisej.Web.Label();
            this.labelIdentityCaption = new Wisej.Web.Label();
            this.labelIdentity = new Wisej.Web.Label();
            this.tracePanel = new TicketOps.Diagnostics.ActivityTracePanel();
            this.panelActions = new Wisej.Web.Panel();
            this.buttonWrongPassword = new Wisej.Web.Button();
            this.buttonUnknownUser = new Wisej.Web.Button();
            this.buttonStoreOutage = new Wisej.Web.Button();
            this.buttonClear = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
            this.panelActions.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (the login gate — display and input only)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelQuestion);
            this.panelScreen.Controls.Add(this.labelUserCaption);
            this.panelScreen.Controls.Add(this.textUser);
            this.panelScreen.Controls.Add(this.labelPasswordCaption);
            this.panelScreen.Controls.Add(this.textPassword);
            this.panelScreen.Controls.Add(this.buttonSignIn);
            this.panelScreen.Controls.Add(this.labelDemoCaption);
            this.panelScreen.Controls.Add(this.buttonUseTechnician);
            this.panelScreen.Controls.Add(this.buttonUseSupervisor);
            this.panelScreen.Controls.Add(this.buttonUseAdmin);
            this.panelScreen.Controls.Add(this.labelDemoNote);
            this.panelScreen.Controls.Add(this.labelIdentityCaption);
            this.panelScreen.Controls.Add(this.labelIdentity);
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
            this.labelScreenTitle.Text = "TicketOps — Sign in";
            //
            // statusBanner  (Controls/StatusBanner: "● state" + banner line)
            //
            this.statusBanner.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.statusBanner.Location = new System.Drawing.Point(24, 20);
            this.statusBanner.Name = "statusBanner";
            this.statusBanner.Size = new System.Drawing.Size(712, 58);
            //
            // labelQuestion
            //
            this.labelQuestion.AutoSize = false;
            this.labelQuestion.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.labelQuestion.Location = new System.Drawing.Point(24, 86);
            this.labelQuestion.Name = "labelQuestion";
            this.labelQuestion.Size = new System.Drawing.Size(340, 26);
            this.labelQuestion.Text = "Who is using this session?";
            //
            // labelUserCaption
            //
            this.labelUserCaption.AutoSize = false;
            this.labelUserCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelUserCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelUserCaption.Location = new System.Drawing.Point(24, 124);
            this.labelUserCaption.Name = "labelUserCaption";
            this.labelUserCaption.Size = new System.Drawing.Size(340, 18);
            this.labelUserCaption.Text = "USERNAME";
            //
            // textUser
            //
            this.textUser.Location = new System.Drawing.Point(24, 144);
            this.textUser.MaxLength = 64;
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(340, 36);
            this.textUser.TabIndex = 1;
            this.textUser.Watermark = "l.romero";
            //
            // labelPasswordCaption
            //
            this.labelPasswordCaption.AutoSize = false;
            this.labelPasswordCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelPasswordCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelPasswordCaption.Location = new System.Drawing.Point(24, 194);
            this.labelPasswordCaption.Name = "labelPasswordCaption";
            this.labelPasswordCaption.Size = new System.Drawing.Size(340, 18);
            this.labelPasswordCaption.Text = "PASSWORD";
            //
            // textPassword  (masked in the browser; compared on the server, never logged)
            //
            this.textPassword.Location = new System.Drawing.Point(24, 214);
            this.textPassword.MaxLength = 128;
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '●';
            this.textPassword.Size = new System.Drawing.Size(340, 36);
            this.textPassword.TabIndex = 2;
            this.textPassword.Watermark = "demo";
            //
            // buttonSignIn  (success path — also the form's AcceptButton, so Enter submits)
            //
            this.buttonSignIn.Location = new System.Drawing.Point(24, 268);
            this.buttonSignIn.Name = "buttonSignIn";
            this.buttonSignIn.Size = new System.Drawing.Size(160, 40);
            this.buttonSignIn.TabIndex = 3;
            this.buttonSignIn.Text = "Sign in";
            this.buttonSignIn.ToolTipText = "Success path: IAuthenticationService.SignInAsync verifies the credential on the server and binds the identity to the session";
            this.buttonSignIn.Click += new System.EventHandler(this.buttonSignIn_Click);
            //
            // labelDemoCaption
            //
            this.labelDemoCaption.AutoSize = false;
            this.labelDemoCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelDemoCaption.ForeColor = System.Drawing.Color.FromArgb(185, 119, 14);
            this.labelDemoCaption.Location = new System.Drawing.Point(400, 124);
            this.labelDemoCaption.Name = "labelDemoCaption";
            this.labelDemoCaption.Size = new System.Drawing.Size(336, 18);
            this.labelDemoCaption.Text = "DEMO ACCOUNTS · THIS LAB ONLY · PASSWORD \"demo\"";
            //
            // buttonUseTechnician
            //
            this.buttonUseTechnician.Location = new System.Drawing.Point(400, 144);
            this.buttonUseTechnician.Name = "buttonUseTechnician";
            this.buttonUseTechnician.Size = new System.Drawing.Size(336, 36);
            this.buttonUseTechnician.TabIndex = 10;
            this.buttonUseTechnician.Text = "Use l.romero · Technician (view, add notes)";
            this.buttonUseTechnician.ToolTipText = "Fills the form with the Technician demo account — the Delete button will be hidden for this role";
            this.buttonUseTechnician.Click += new System.EventHandler(this.buttonUseTechnician_Click);
            //
            // buttonUseSupervisor
            //
            this.buttonUseSupervisor.Location = new System.Drawing.Point(400, 188);
            this.buttonUseSupervisor.Name = "buttonUseSupervisor";
            this.buttonUseSupervisor.Size = new System.Drawing.Size(336, 36);
            this.buttonUseSupervisor.TabIndex = 11;
            this.buttonUseSupervisor.Text = "Use m.weber · Supervisor (+ close, delete)";
            this.buttonUseSupervisor.ToolTipText = "Fills the form with the Supervisor demo account — may close and delete tickets";
            this.buttonUseSupervisor.Click += new System.EventHandler(this.buttonUseSupervisor_Click);
            //
            // buttonUseAdmin
            //
            this.buttonUseAdmin.Location = new System.Drawing.Point(400, 232);
            this.buttonUseAdmin.Name = "buttonUseAdmin";
            this.buttonUseAdmin.Size = new System.Drawing.Size(336, 36);
            this.buttonUseAdmin.TabIndex = 12;
            this.buttonUseAdmin.Text = "Use s.okafor · Admin (everything)";
            this.buttonUseAdmin.ToolTipText = "Fills the form with the Admin demo account";
            this.buttonUseAdmin.Click += new System.EventHandler(this.buttonUseAdmin_Click);
            //
            // labelDemoNote
            //
            this.labelDemoNote.AutoSize = false;
            this.labelDemoNote.Font = new System.Drawing.Font("default", 8.5F);
            this.labelDemoNote.ForeColor = System.Drawing.Color.FromArgb(106, 118, 134);
            this.labelDemoNote.Location = new System.Drawing.Point(400, 274);
            this.labelDemoNote.Name = "labelDemoNote";
            this.labelDemoNote.Size = new System.Drawing.Size(336, 54);
            this.labelDemoNote.Text = "The passwords are demo constants in an in-memory store and are compared on the server, never in the browser. Production replaces AuthenticationService with a real identity provider (OpenID Connect / Windows auth).";
            //
            // labelIdentityCaption
            //
            this.labelIdentityCaption.AutoSize = false;
            this.labelIdentityCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelIdentityCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelIdentityCaption.Location = new System.Drawing.Point(24, 348);
            this.labelIdentityCaption.Name = "labelIdentityCaption";
            this.labelIdentityCaption.Size = new System.Drawing.Size(712, 18);
            this.labelIdentityCaption.Text = "SERVER-SIDE IDENTITY · what the session knows right now (the browser holds none of it)";
            //
            // labelIdentity  (AllowHtml stays false: this label shows server values as text)
            //
            this.labelIdentity.AutoSize = false;
            this.labelIdentity.BackColor = System.Drawing.Color.FromArgb(246, 249, 252);
            this.labelIdentity.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.labelIdentity.Font = new System.Drawing.Font("monospace", 9F);
            this.labelIdentity.Location = new System.Drawing.Point(24, 370);
            this.labelIdentity.Name = "labelIdentity";
            this.labelIdentity.Padding = new Wisej.Web.Padding(10, 8, 10, 8);
            this.labelIdentity.Size = new System.Drawing.Size(712, 166);
            this.labelIdentity.Text = "";
            //
            // tracePanel  (Diagnostics: the live activity trace)
            //
            this.tracePanel.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Right;
            this.tracePanel.Location = new System.Drawing.Point(810, 30);
            this.tracePanel.Name = "tracePanel";
            this.tracePanel.Size = new System.Drawing.Size(508, 560);
            this.tracePanel.Title = "Activity trace · UI → Service → Data · [SESSION] · [AUDIT]";
            //
            // panelActions  (bottom bar: failures / outage + recovery / clear)
            //
            this.panelActions.Anchor = Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelActions.Controls.Add(this.buttonWrongPassword);
            this.panelActions.Controls.Add(this.buttonUnknownUser);
            this.panelActions.Controls.Add(this.buttonStoreOutage);
            this.panelActions.Controls.Add(this.buttonClear);
            this.panelActions.Location = new System.Drawing.Point(30, 606);
            this.panelActions.Name = "panelActions";
            this.panelActions.Size = new System.Drawing.Size(1288, 44);
            //
            // buttonWrongPassword  (failure path 1)
            //
            this.buttonWrongPassword.Location = new System.Drawing.Point(0, 4);
            this.buttonWrongPassword.Name = "buttonWrongPassword";
            this.buttonWrongPassword.Size = new System.Drawing.Size(200, 36);
            this.buttonWrongPassword.Text = "Wrong password";
            this.buttonWrongPassword.ToolTipText = "Failure path: l.romero with a wrong password — the store finds the user, the service still refuses; no identity is bound";
            this.buttonWrongPassword.Click += new System.EventHandler(this.buttonWrongPassword_Click);
            //
            // buttonUnknownUser  (failure path 2)
            //
            this.buttonUnknownUser.Location = new System.Drawing.Point(210, 4);
            this.buttonUnknownUser.Name = "buttonUnknownUser";
            this.buttonUnknownUser.Size = new System.Drawing.Size(180, 36);
            this.buttonUnknownUser.Text = "Unknown user";
            this.buttonUnknownUser.ToolTipText = "Failure path: j.doe does not exist — the user sees the SAME neutral message as for a wrong password";
            this.buttonUnknownUser.Click += new System.EventHandler(this.buttonUnknownUser_Click);
            //
            // buttonStoreOutage  (error path + recovery)
            //
            this.buttonStoreOutage.Location = new System.Drawing.Point(400, 4);
            this.buttonStoreOutage.Name = "buttonStoreOutage";
            this.buttonStoreOutage.Size = new System.Drawing.Size(240, 36);
            this.buttonStoreOutage.Text = "Simulate user store outage";
            this.buttonStoreOutage.ToolTipText = "Error path: the directory is unreachable — ✖ in DATA with the LDAP host, a safe sentence in the UI; click again to recover and sign in";
            this.buttonStoreOutage.Click += new System.EventHandler(this.buttonStoreOutage_Click);
            //
            // buttonClear
            //
            this.buttonClear.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Right;
            this.buttonClear.Location = new System.Drawing.Point(1148, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(140, 36);
            this.buttonClear.Text = "Clear trace";
            this.buttonClear.ToolTipText = "Empties the activity trace (the audit trail is append-only and is not affected)";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // LoginView
            //
            this.AcceptButton = this.buttonSignIn;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(1348, 680);
            this.Controls.Add(this.panelScreen);
            this.Controls.Add(this.tracePanel);
            this.Controls.Add(this.panelActions);
            this.Name = "LoginView";
            this.Text = "TicketOps Console — Module 11 · Sign in";
            this.Load += new System.EventHandler(this.LoginView_Load);
            this.FormClosed += new Wisej.Web.FormClosedEventHandler(this.LoginView_FormClosed);
            this.panelScreen.ResumeLayout(false);
            this.panelActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelQuestion;
        private Wisej.Web.Label labelUserCaption;
        private Wisej.Web.TextBox textUser;
        private Wisej.Web.Label labelPasswordCaption;
        private Wisej.Web.TextBox textPassword;
        private Wisej.Web.Button buttonSignIn;
        private Wisej.Web.Label labelDemoCaption;
        private Wisej.Web.Button buttonUseTechnician;
        private Wisej.Web.Button buttonUseSupervisor;
        private Wisej.Web.Button buttonUseAdmin;
        private Wisej.Web.Label labelDemoNote;
        private Wisej.Web.Label labelIdentityCaption;
        private Wisej.Web.Label labelIdentity;
        private TicketOps.Diagnostics.ActivityTracePanel tracePanel;
        private Wisej.Web.Panel panelActions;
        private Wisej.Web.Button buttonWrongPassword;
        private Wisej.Web.Button buttonUnknownUser;
        private Wisej.Web.Button buttonStoreOutage;
        private Wisej.Web.Button buttonClear;
    }
}
