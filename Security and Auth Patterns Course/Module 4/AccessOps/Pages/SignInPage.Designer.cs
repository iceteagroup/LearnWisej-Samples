using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    partial class SignInPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.appBar = new AccessOps.UI.AppBar();
            this.pnlBody = new Wisej.Web.Panel();
            this.lblHeading = new Wisej.Web.Label();
            this.lblSubheading = new Wisej.Web.Label();
            this.btnSignIn = new Wisej.Web.Panel();
            this.lblProviderBadge = new Wisej.Web.Label();
            this.lblSignInText = new Wisej.Web.Label();
            this.lblSignInArrow = new Wisej.Web.Label();
            this.lblHint = new Wisej.Web.Label();
            this.lblDivider = new Wisej.Web.Label();
            this.tblService = new Wisej.Web.TableLayoutPanel();
            this.txtUserName = new Wisej.Web.TextBox();
            this.txtPassword = new Wisej.Web.TextBox();
            this.btnServiceSignIn = new Wisej.Web.Button();
            this.lblStatus = new AccessOps.UI.MessageStrip();
            this.statusStrip = new AccessOps.UI.StatusStrip("STATUS");
            this.SuspendLayout();

            this.appBar.Area = "Sign in";

            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Text = "Sign in to AccessOps";
            this.lblHeading.Dock = DockStyle.Top;
            this.lblHeading.AutoSize = false;
            this.lblHeading.Height = 28;
            this.lblHeading.Font = Theme.Px(20f, FontStyle.Bold);
            this.lblHeading.ForeColor = Theme.Ink;

            this.lblSubheading.Name = "lblSubheading";
            this.lblSubheading.Text = "Departments, employees, documents and approvals";
            this.lblSubheading.Dock = DockStyle.Top;
            this.lblSubheading.AutoSize = false;
            this.lblSubheading.Height = 26;
            this.lblSubheading.Font = Theme.Px(13f);
            this.lblSubheading.ForeColor = Theme.Muted;

            // "Sign in with your work account" - the external sign-in, first
            this.lblProviderBadge.Name = "lblProviderBadge";
            this.lblProviderBadge.Text = "C";
            this.lblProviderBadge.Dock = DockStyle.Left;
            this.lblProviderBadge.AutoSize = false;
            this.lblProviderBadge.Width = 20;
            this.lblProviderBadge.BackColor = Color.White;
            this.lblProviderBadge.ForeColor = Theme.Accent;
            this.lblProviderBadge.Font = Theme.Px(13f, FontStyle.Bold);
            this.lblProviderBadge.TextAlign = ContentAlignment.MiddleCenter;
            this.lblProviderBadge.CssStyle = "border-radius:5px; margin:2px 0;";
            this.lblSignInText.Name = "lblSignInText";
            this.lblSignInText.Text = "Sign in with your work account";
            this.lblSignInText.Dock = DockStyle.Fill;
            this.lblSignInText.AutoSize = false;
            this.lblSignInText.Padding = new Padding(12, 0, 0, 0);
            this.lblSignInText.ForeColor = Color.White;
            this.lblSignInText.Font = Theme.Px(15f, FontStyle.Bold);
            this.lblSignInText.TextAlign = ContentAlignment.MiddleLeft;
            this.lblSignInArrow.Name = "lblSignInArrow";
            this.lblSignInArrow.Text = "→";
            this.lblSignInArrow.Dock = DockStyle.Right;
            this.lblSignInArrow.AutoSize = false;
            this.lblSignInArrow.Width = 24;
            this.lblSignInArrow.ForeColor = Color.White;
            this.lblSignInArrow.Font = Theme.Px(17f);
            this.lblSignInArrow.TextAlign = ContentAlignment.MiddleCenter;
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Dock = DockStyle.Top;
            this.btnSignIn.Height = 48;
            this.btnSignIn.BackColor = Theme.Accent;
            this.btnSignIn.Padding = new Padding(18, 12, 18, 12);
            this.btnSignIn.Cursor = Cursors.Hand;
            this.btnSignIn.CssStyle = "border-radius:9px;";
            this.btnSignIn.Controls.Add(this.lblSignInText);
            this.btnSignIn.Controls.Add(this.lblSignInArrow);
            this.btnSignIn.Controls.Add(this.lblProviderBadge);
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            this.lblSignInText.Click += new System.EventHandler(this.btnSignIn_Click);
            this.lblProviderBadge.Click += new System.EventHandler(this.btnSignIn_Click);
            this.lblSignInArrow.Click += new System.EventHandler(this.btnSignIn_Click);

            this.lblHint.Name = "lblHint";
            this.lblHint.Text = "Redirects to your organisation. AccessOps never sees the password.";
            this.lblHint.Dock = DockStyle.Top;
            this.lblHint.AutoSize = false;
            this.lblHint.Height = 26;
            this.lblHint.Padding = new Padding(0, 8, 0, 0);
            this.lblHint.Font = Theme.Px(12f);
            this.lblHint.ForeColor = Theme.Faint;

            this.lblDivider.Name = "lblDivider";
            this.lblDivider.Text = "SERVICE ACCOUNTS ONLY";
            this.lblDivider.Dock = DockStyle.Top;
            this.lblDivider.AutoSize = false;
            this.lblDivider.Height = 44;
            this.lblDivider.Font = Theme.Px(11.5f, FontStyle.Bold);
            this.lblDivider.ForeColor = ColorTranslator.FromHtml("#9aa9b8");
            this.lblDivider.TextAlign = ContentAlignment.MiddleCenter;
            this.lblDivider.CssStyle = "background:linear-gradient(#e4eaf1,#e4eaf1) left 50%/38% 1px no-repeat, linear-gradient(#e4eaf1,#e4eaf1) right 50%/38% 1px no-repeat;";

            // the Module 2 password path, kept for named service accounts only
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Dock = DockStyle.Fill;
            this.txtUserName.Watermark = "account name";
            this.txtUserName.MaxLength = 64;
            this.txtUserName.Margin = new Padding(0, 0, 10, 0);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Dock = DockStyle.Fill;
            this.txtPassword.Watermark = "••••••••";
            this.txtPassword.InputType.Type = TextBoxType.Password;
            this.txtPassword.MaxLength = 128;
            this.txtPassword.Margin = new Padding(0, 0, 10, 0);
            this.btnServiceSignIn.Name = "btnServiceSignIn";
            this.btnServiceSignIn.Text = "Sign in";
            this.btnServiceSignIn.Dock = DockStyle.Fill;
            Theme.StyleButton(this.btnServiceSignIn, primary: false);
            this.btnServiceSignIn.Click += new System.EventHandler(this.btnServiceSignIn_Click);
            this.tblService.Name = "tblService";
            this.tblService.Dock = DockStyle.Top;
            this.tblService.Height = 38;
            this.tblService.ColumnCount = 3;
            this.tblService.RowCount = 1;
            this.tblService.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblService.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblService.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 96F));
            this.tblService.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tblService.Controls.Add(this.txtUserName, 0, 0);
            this.tblService.Controls.Add(this.txtPassword, 1, 0);
            this.tblService.Controls.Add(this.btnServiceSignIn, 2, 0);

            this.lblStatus.Name = "lblStatus";

            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = DockStyle.Top;
            this.pnlBody.Height = 360;
            this.pnlBody.Padding = new Padding(26, 22, 26, 22);
            this.pnlBody.Controls.Add(this.lblStatus);
            this.pnlBody.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16 });
            this.pnlBody.Controls.Add(this.tblService);
            this.pnlBody.Controls.Add(this.lblDivider);
            this.pnlBody.Controls.Add(this.lblHint);
            this.pnlBody.Controls.Add(this.btnSignIn);
            this.pnlBody.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 14 });
            this.pnlBody.Controls.Add(this.lblSubheading);
            this.pnlBody.Controls.Add(this.lblHeading);

            this.Name = "SignInPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.appBar);
            this.ResumeLayout(false);
        }

        #endregion

        private AccessOps.UI.AppBar appBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblHeading;
        private Wisej.Web.Label lblSubheading;
        private Wisej.Web.Panel btnSignIn;
        private Wisej.Web.Label lblProviderBadge;
        private Wisej.Web.Label lblSignInText;
        private Wisej.Web.Label lblSignInArrow;
        private Wisej.Web.Label lblHint;
        private Wisej.Web.Label lblDivider;
        private Wisej.Web.TableLayoutPanel tblService;
        private Wisej.Web.TextBox txtUserName;
        private Wisej.Web.TextBox txtPassword;
        private Wisej.Web.Button btnServiceSignIn;
        private AccessOps.UI.MessageStrip lblStatus;
        private AccessOps.UI.StatusStrip statusStrip;
    }
}
