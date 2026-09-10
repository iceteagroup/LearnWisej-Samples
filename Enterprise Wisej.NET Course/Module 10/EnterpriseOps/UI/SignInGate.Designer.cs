namespace EnterpriseOps.UI
{
    partial class SignInGate
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
            this.pnlGateHeader = new Wisej.Web.Panel();
            this.lblGateTitle = new Wisej.Web.Label();
            this.lblGateSimulated = new Wisej.Web.Label();
            this.lblGateHint = new Wisej.Web.Label();
            this.lblIdentitiesTitle = new Wisej.Web.Label();
            this.lstIdentities = new Wisej.Web.ListBox();
            this.lblAccountDescription = new Wisej.Web.Label();
            this.lblClaimsTitle = new Wisej.Web.Label();
            this.lstClaims = new Wisej.Web.ListBox();
            this.lblMappedTitle = new Wisej.Web.Label();
            this.lblMapped = new Wisej.Web.Label();
            this.lblGateError = new Wisej.Web.Label();
            this.lblGateFooter = new Wisej.Web.Label();
            this.btnSignIn = new Wisej.Web.Button();
            this.btnStaySignedOut = new Wisej.Web.Button();
            this.pnlGateHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlGateHeader
            //
            this.pnlGateHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlGateHeader.Controls.Add(this.lblGateTitle);
            this.pnlGateHeader.Controls.Add(this.lblGateSimulated);
            this.pnlGateHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlGateHeader.Name = "pnlGateHeader";
            this.pnlGateHeader.Size = new System.Drawing.Size(760, 56);
            //
            // lblGateTitle
            //
            this.lblGateTitle.AutoSize = false;
            this.lblGateTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblGateTitle.ForeColor = System.Drawing.Color.White;
            this.lblGateTitle.Location = new System.Drawing.Point(24, 0);
            this.lblGateTitle.Name = "lblGateTitle";
            this.lblGateTitle.Size = new System.Drawing.Size(460, 56);
            this.lblGateTitle.Text = "Sign in with EnterpriseOps SSO";
            this.lblGateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblGateSimulated
            //
            this.lblGateSimulated.AutoSize = false;
            this.lblGateSimulated.Font = new System.Drawing.Font("default", 9F, System.Drawing.FontStyle.Bold);
            this.lblGateSimulated.ForeColor = System.Drawing.Color.FromArgb(214, 228, 243);
            this.lblGateSimulated.Location = new System.Drawing.Point(484, 0);
            this.lblGateSimulated.Name = "lblGateSimulated";
            this.lblGateSimulated.Size = new System.Drawing.Size(252, 56);
            this.lblGateSimulated.Text = "SIMULATION · no password is collected";
            this.lblGateSimulated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lblGateHint
            //
            this.lblGateHint.AutoSize = false;
            this.lblGateHint.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblGateHint.Location = new System.Drawing.Point(24, 66);
            this.lblGateHint.Name = "lblGateHint";
            this.lblGateHint.Size = new System.Drawing.Size(712, 42);
            this.lblGateHint.Text = "Pick the identity the provider should assert. This stands in for an OIDC redirect: no identity provider is contacted"
                + " and no credential is ever entered. Everything the application sees is the claim list on the right.";
            //
            // lblIdentitiesTitle
            //
            this.lblIdentitiesTitle.AutoSize = false;
            this.lblIdentitiesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblIdentitiesTitle.Location = new System.Drawing.Point(24, 112);
            this.lblIdentitiesTitle.Name = "lblIdentitiesTitle";
            this.lblIdentitiesTitle.Size = new System.Drawing.Size(320, 22);
            this.lblIdentitiesTitle.Text = "Corporate directory";
            //
            // lstIdentities  (the accounts the simulated provider knows)
            //
            this.lstIdentities.Font = new System.Drawing.Font("monospace", 9F);
            this.lstIdentities.Location = new System.Drawing.Point(24, 136);
            this.lstIdentities.Name = "lstIdentities";
            this.lstIdentities.Size = new System.Drawing.Size(320, 200);
            this.lstIdentities.SelectedIndexChanged += new System.EventHandler(this.lstIdentities_SelectedIndexChanged);
            //
            // lblAccountDescription
            //
            this.lblAccountDescription.AutoSize = false;
            this.lblAccountDescription.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblAccountDescription.Location = new System.Drawing.Point(24, 344);
            this.lblAccountDescription.Name = "lblAccountDescription";
            this.lblAccountDescription.Size = new System.Drawing.Size(320, 56);
            this.lblAccountDescription.Text = "";
            //
            // lblClaimsTitle
            //
            this.lblClaimsTitle.AutoSize = false;
            this.lblClaimsTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblClaimsTitle.Location = new System.Drawing.Point(364, 112);
            this.lblClaimsTitle.Name = "lblClaimsTitle";
            this.lblClaimsTitle.Size = new System.Drawing.Size(372, 22);
            this.lblClaimsTitle.Text = "Claims the provider asserts";
            //
            // lstClaims  (what crosses the OIDC/SSO boundary — a list of type/value pairs, nothing else)
            //
            this.lstClaims.Font = new System.Drawing.Font("monospace", 9F);
            this.lstClaims.Location = new System.Drawing.Point(364, 136);
            this.lstClaims.Name = "lstClaims";
            this.lstClaims.Size = new System.Drawing.Size(372, 140);
            //
            // lblMappedTitle
            //
            this.lblMappedTitle.AutoSize = false;
            this.lblMappedTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblMappedTitle.Location = new System.Drawing.Point(364, 284);
            this.lblMappedTitle.Name = "lblMappedTitle";
            this.lblMappedTitle.Size = new System.Drawing.Size(372, 22);
            this.lblMappedTitle.Text = "After ClaimsMapper — the application's own words";
            //
            // lblMapped
            //
            this.lblMapped.AutoSize = false;
            this.lblMapped.BackColor = System.Drawing.Color.FromArgb(244, 247, 251);
            this.lblMapped.BorderStyle = Wisej.Web.BorderStyle.Solid;
            this.lblMapped.Font = new System.Drawing.Font("monospace", 9F);
            this.lblMapped.Location = new System.Drawing.Point(364, 308);
            this.lblMapped.Name = "lblMapped";
            this.lblMapped.Padding = new Wisej.Web.Padding(10, 8, 10, 8);
            this.lblMapped.Size = new System.Drawing.Size(372, 92);
            this.lblMapped.Text = "";
            //
            // lblGateError
            //
            this.lblGateError.AutoSize = false;
            this.lblGateError.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblGateError.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblGateError.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblGateError.Location = new System.Drawing.Point(24, 410);
            this.lblGateError.Name = "lblGateError";
            this.lblGateError.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblGateError.Size = new System.Drawing.Size(712, 38);
            this.lblGateError.Text = "";
            this.lblGateError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGateError.Visible = false;
            //
            // lblGateFooter
            //
            this.lblGateFooter.AutoSize = false;
            this.lblGateFooter.Font = new System.Drawing.Font("default", 8F);
            this.lblGateFooter.ForeColor = System.Drawing.Color.FromArgb(90, 107, 125);
            this.lblGateFooter.Location = new System.Drawing.Point(24, 458);
            this.lblGateFooter.Name = "lblGateFooter";
            this.lblGateFooter.Size = new System.Drawing.Size(400, 44);
            this.lblGateFooter.Text = "The gate is the only door: no screen and no service in this application can obtain a CommandContext until it closes.";
            //
            // btnStaySignedOut
            //
            this.btnStaySignedOut.Location = new System.Drawing.Point(444, 462);
            this.btnStaySignedOut.Name = "btnStaySignedOut";
            this.btnStaySignedOut.Size = new System.Drawing.Size(140, 38);
            this.btnStaySignedOut.Text = "Stay signed out";
            this.btnStaySignedOut.ToolTipText = "Closes the gate with DialogResult.Cancel: the screen stays locked and every service call is refused.";
            this.btnStaySignedOut.Click += new System.EventHandler(this.btnStaySignedOut_Click);
            //
            // btnSignIn
            //
            this.btnSignIn.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSignIn.Location = new System.Drawing.Point(596, 462);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(140, 38);
            this.btnSignIn.Text = "Sign in";
            this.btnSignIn.ToolTipText = "btnSignIn_Click → await _signIn.SignInAsync(subject) → provider → ClaimsMapper → SessionContext → role store.";
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            //
            // SignInGate
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(760, 522);
            this.Controls.Add(this.pnlGateHeader);
            this.Controls.Add(this.lblGateHint);
            this.Controls.Add(this.lblIdentitiesTitle);
            this.Controls.Add(this.lstIdentities);
            this.Controls.Add(this.lblAccountDescription);
            this.Controls.Add(this.lblClaimsTitle);
            this.Controls.Add(this.lstClaims);
            this.Controls.Add(this.lblMappedTitle);
            this.Controls.Add(this.lblMapped);
            this.Controls.Add(this.lblGateError);
            this.Controls.Add(this.lblGateFooter);
            this.Controls.Add(this.btnStaySignedOut);
            this.Controls.Add(this.btnSignIn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignInGate";
            this.ShowInTaskbar = false;
            this.StartPosition = Wisej.Web.FormStartPosition.CenterScreen;
            this.Text = "EnterpriseOps — corporate sign-in (simulated)";
            this.Load += new System.EventHandler(this.SignInGate_Load);
            this.pnlGateHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Panel pnlGateHeader;
        private Wisej.Web.Label lblGateTitle;
        private Wisej.Web.Label lblGateSimulated;
        private Wisej.Web.Label lblGateHint;
        private Wisej.Web.Label lblIdentitiesTitle;
        private Wisej.Web.ListBox lstIdentities;
        private Wisej.Web.Label lblAccountDescription;
        private Wisej.Web.Label lblClaimsTitle;
        private Wisej.Web.ListBox lstClaims;
        private Wisej.Web.Label lblMappedTitle;
        private Wisej.Web.Label lblMapped;
        private Wisej.Web.Label lblGateError;
        private Wisej.Web.Label lblGateFooter;
        private Wisej.Web.Button btnSignIn;
        private Wisej.Web.Button btnStaySignedOut;
    }
}
