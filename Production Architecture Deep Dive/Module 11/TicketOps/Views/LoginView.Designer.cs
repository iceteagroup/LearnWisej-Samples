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
            this.panelScreen = new Wisej.Web.Panel();
            this.labelScreenTitle = new Wisej.Web.Label();
            this.statusBanner = new TicketOps.Controls.StatusBanner();
            this.labelUserCaption = new Wisej.Web.Label();
            this.textUser = new Wisej.Web.TextBox();
            this.labelPasswordCaption = new Wisej.Web.Label();
            this.textPassword = new Wisej.Web.TextBox();
            this.buttonSignIn = new Wisej.Web.Button();
            this.panelScreen.SuspendLayout();
            this.SuspendLayout();
            //
            // panelScreen  (the login gate)
            //
            this.panelScreen.Anchor = Wisej.Web.AnchorStyles.Top | Wisej.Web.AnchorStyles.Bottom | Wisej.Web.AnchorStyles.Left | Wisej.Web.AnchorStyles.Right;
            this.panelScreen.BackColor = System.Drawing.Color.White;
            this.panelScreen.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.panelScreen.Controls.Add(this.labelScreenTitle);
            this.panelScreen.Controls.Add(this.statusBanner);
            this.panelScreen.Controls.Add(this.labelUserCaption);
            this.panelScreen.Controls.Add(this.textUser);
            this.panelScreen.Controls.Add(this.labelPasswordCaption);
            this.panelScreen.Controls.Add(this.textPassword);
            this.panelScreen.Controls.Add(this.buttonSignIn);
            this.panelScreen.Location = new System.Drawing.Point(30, 30);
            this.panelScreen.Name = "panelScreen";
            this.panelScreen.Size = new System.Drawing.Size(760, 300);
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
            // labelUserCaption
            //
            this.labelUserCaption.AutoSize = false;
            this.labelUserCaption.Font = new System.Drawing.Font("default", 8.5F, System.Drawing.FontStyle.Bold);
            this.labelUserCaption.ForeColor = System.Drawing.Color.FromArgb(74, 90, 106);
            this.labelUserCaption.Location = new System.Drawing.Point(24, 94);
            this.labelUserCaption.Name = "labelUserCaption";
            this.labelUserCaption.Size = new System.Drawing.Size(340, 18);
            this.labelUserCaption.Text = "USERNAME";
            //
            // textUser
            //
            this.textUser.Location = new System.Drawing.Point(24, 114);
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
            this.labelPasswordCaption.Location = new System.Drawing.Point(24, 162);
            this.labelPasswordCaption.Name = "labelPasswordCaption";
            this.labelPasswordCaption.Size = new System.Drawing.Size(340, 18);
            this.labelPasswordCaption.Text = "PASSWORD";
            //
            // textPassword  (masked in the browser; compared on the server, never logged)
            //
            this.textPassword.Location = new System.Drawing.Point(24, 182);
            this.textPassword.MaxLength = 128;
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '●';
            this.textPassword.Size = new System.Drawing.Size(340, 36);
            this.textPassword.TabIndex = 2;
            //
            // buttonSignIn  (also the form's AcceptButton, so Enter submits)
            //
            this.buttonSignIn.Location = new System.Drawing.Point(24, 236);
            this.buttonSignIn.Name = "buttonSignIn";
            this.buttonSignIn.Size = new System.Drawing.Size(160, 40);
            this.buttonSignIn.TabIndex = 3;
            this.buttonSignIn.Text = "Sign in";
            this.buttonSignIn.Click += new System.EventHandler(this.buttonSignIn_Click);
            //
            // LoginView
            //
            this.AcceptButton = this.buttonSignIn;
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(820, 360);
            this.Controls.Add(this.panelScreen);
            this.Name = "LoginView";
            this.Text = "TicketOps Console";
            this.Load += new System.EventHandler(this.LoginView_Load);
            this.panelScreen.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel panelScreen;
        private Wisej.Web.Label labelScreenTitle;
        private TicketOps.Controls.StatusBanner statusBanner;
        private Wisej.Web.Label labelUserCaption;
        private Wisej.Web.TextBox textUser;
        private Wisej.Web.Label labelPasswordCaption;
        private Wisej.Web.TextBox textPassword;
        private Wisej.Web.Button buttonSignIn;
    }
}
