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
            this.lblIdentitiesTitle = new Wisej.Web.Label();
            this.lstIdentities = new Wisej.Web.ListBox();
            this.lblAccountDescription = new Wisej.Web.Label();
            this.lblClaimsTitle = new Wisej.Web.Label();
            this.lstClaims = new Wisej.Web.ListBox();
            this.lblGateError = new Wisej.Web.Label();
            this.btnSignIn = new Wisej.Web.Button();
            this.btnStaySignedOut = new Wisej.Web.Button();
            this.pnlGateHeader.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlGateHeader
            //
            this.pnlGateHeader.BackColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.pnlGateHeader.Controls.Add(this.lblGateTitle);
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
            this.lblGateTitle.Size = new System.Drawing.Size(712, 56);
            this.lblGateTitle.Text = "Sign in with EnterpriseOps SSO";
            this.lblGateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblIdentitiesTitle
            //
            this.lblIdentitiesTitle.AutoSize = false;
            this.lblIdentitiesTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblIdentitiesTitle.Location = new System.Drawing.Point(24, 72);
            this.lblIdentitiesTitle.Name = "lblIdentitiesTitle";
            this.lblIdentitiesTitle.Size = new System.Drawing.Size(320, 22);
            this.lblIdentitiesTitle.Text = "Corporate directory";
            //
            // lstIdentities
            //
            this.lstIdentities.Font = new System.Drawing.Font("monospace", 9F);
            this.lstIdentities.Location = new System.Drawing.Point(24, 96);
            this.lstIdentities.Name = "lstIdentities";
            this.lstIdentities.Size = new System.Drawing.Size(320, 200);
            this.lstIdentities.SelectedIndexChanged += new System.EventHandler(this.lstIdentities_SelectedIndexChanged);
            //
            // lblAccountDescription
            //
            this.lblAccountDescription.AutoSize = false;
            this.lblAccountDescription.ForeColor = System.Drawing.Color.FromArgb(70, 88, 106);
            this.lblAccountDescription.Location = new System.Drawing.Point(24, 304);
            this.lblAccountDescription.Name = "lblAccountDescription";
            this.lblAccountDescription.Size = new System.Drawing.Size(320, 44);
            this.lblAccountDescription.Text = "";
            //
            // lblClaimsTitle
            //
            this.lblClaimsTitle.AutoSize = false;
            this.lblClaimsTitle.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblClaimsTitle.Location = new System.Drawing.Point(364, 72);
            this.lblClaimsTitle.Name = "lblClaimsTitle";
            this.lblClaimsTitle.Size = new System.Drawing.Size(372, 22);
            this.lblClaimsTitle.Text = "Claims the provider asserts";
            //
            // lstClaims
            //
            this.lstClaims.Font = new System.Drawing.Font("monospace", 9F);
            this.lstClaims.Location = new System.Drawing.Point(364, 96);
            this.lstClaims.Name = "lstClaims";
            this.lstClaims.Size = new System.Drawing.Size(372, 252);
            //
            // lblGateError
            //
            this.lblGateError.AutoSize = false;
            this.lblGateError.BackColor = System.Drawing.Color.FromArgb(253, 236, 234);
            this.lblGateError.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.lblGateError.ForeColor = System.Drawing.Color.FromArgb(178, 59, 39);
            this.lblGateError.Location = new System.Drawing.Point(24, 358);
            this.lblGateError.Name = "lblGateError";
            this.lblGateError.Padding = new Wisej.Web.Padding(12, 0, 12, 0);
            this.lblGateError.Size = new System.Drawing.Size(712, 38);
            this.lblGateError.Text = "";
            this.lblGateError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblGateError.Visible = false;
            //
            // btnStaySignedOut
            //
            this.btnStaySignedOut.Location = new System.Drawing.Point(444, 406);
            this.btnStaySignedOut.Name = "btnStaySignedOut";
            this.btnStaySignedOut.Size = new System.Drawing.Size(140, 38);
            this.btnStaySignedOut.Text = "Stay signed out";
            this.btnStaySignedOut.Click += new System.EventHandler(this.btnStaySignedOut_Click);
            //
            // btnSignIn
            //
            this.btnSignIn.Font = new System.Drawing.Font("default", 10F, System.Drawing.FontStyle.Bold);
            this.btnSignIn.Location = new System.Drawing.Point(596, 406);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Size = new System.Drawing.Size(140, 38);
            this.btnSignIn.Text = "Sign in";
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            //
            // SignInGate
            //
            this.BackColor = System.Drawing.Color.FromArgb(238, 242, 247);
            this.ClientSize = new System.Drawing.Size(760, 464);
            this.Controls.Add(this.pnlGateHeader);
            this.Controls.Add(this.lblIdentitiesTitle);
            this.Controls.Add(this.lstIdentities);
            this.Controls.Add(this.lblAccountDescription);
            this.Controls.Add(this.lblClaimsTitle);
            this.Controls.Add(this.lstClaims);
            this.Controls.Add(this.lblGateError);
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
        private Wisej.Web.Label lblIdentitiesTitle;
        private Wisej.Web.ListBox lstIdentities;
        private Wisej.Web.Label lblAccountDescription;
        private Wisej.Web.Label lblClaimsTitle;
        private Wisej.Web.ListBox lstClaims;
        private Wisej.Web.Label lblGateError;
        private Wisej.Web.Button btnSignIn;
        private Wisej.Web.Button btnStaySignedOut;
    }
}
