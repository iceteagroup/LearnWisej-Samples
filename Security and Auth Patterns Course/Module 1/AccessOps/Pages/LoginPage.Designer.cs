using System.Drawing;
using AccessOps.UI;
using Wisej.Web;

namespace AccessOps.Pages
{
    partial class LoginPage
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
            this.tblFields = new Wisej.Web.TableLayoutPanel();
            this.pnlUser = new Wisej.Web.Panel();
            this.txtUserName = new Wisej.Web.TextBox();
            this.pnlPassword = new Wisej.Web.Panel();
            this.txtPassword = new Wisej.Web.TextBox();
            this.pnlActions = new Wisej.Web.Panel();
            this.btnSignIn = new Wisej.Web.Button();
            this.lblStatus = new AccessOps.UI.MessageStrip();
            this.pnlFooter = new Wisej.Web.Panel();
            this.lblFooterCaption = new Wisej.Web.Label();
            this.lblFooterMeta = new Wisej.Web.Label();
            this.SuspendLayout();

            // app bar
            this.appBar.Area = "Sign in";

            // heading
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Text = "Sign in to AccessOps";
            this.lblHeading.Dock = DockStyle.Top;
            this.lblHeading.AutoSize = false;
            this.lblHeading.Height = 28;
            this.lblHeading.Font = Theme.Px(20f, FontStyle.Bold);
            this.lblHeading.ForeColor = Theme.Ink;

            this.lblSubheading.Name = "lblSubheading";
            this.lblSubheading.Text = "Departments · employees · confidential documents · approvals";
            this.lblSubheading.Dock = DockStyle.Top;
            this.lblSubheading.AutoSize = false;
            this.lblSubheading.Height = 22;
            this.lblSubheading.Font = Theme.Px(13f);
            this.lblSubheading.ForeColor = Theme.Muted;

            // the two credential fields, side by side
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Dock = DockStyle.Top;
            this.txtUserName.Height = 36;
            this.txtUserName.Font = Theme.Mono(13.5f);
            this.txtUserName.MaxLength = 64;
            this.pnlUser.Name = "pnlUser";
            this.pnlUser.Dock = DockStyle.Fill;
            this.pnlUser.Padding = new Padding(0, 0, 9, 0);
            this.pnlUser.Controls.Add(this.txtUserName);
            this.pnlUser.Controls.Add(Theme.Caption("USER NAME"));

            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Dock = DockStyle.Top;
            this.txtPassword.Height = 36;
            this.txtPassword.InputType.Type = TextBoxType.Password;
            this.txtPassword.MaxLength = 128;
            this.pnlPassword.Name = "pnlPassword";
            this.pnlPassword.Dock = DockStyle.Fill;
            this.pnlPassword.Padding = new Padding(9, 0, 0, 0);
            this.pnlPassword.Controls.Add(this.txtPassword);
            this.pnlPassword.Controls.Add(Theme.Caption("PASSWORD"));

            this.tblFields.Name = "tblFields";
            this.tblFields.Dock = DockStyle.Top;
            this.tblFields.Height = 76;
            this.tblFields.Margin = new Padding(0);
            this.tblFields.Padding = new Padding(0, 20, 0, 0);
            this.tblFields.ColumnCount = 2;
            this.tblFields.RowCount = 1;
            this.tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.tblFields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.tblFields.Controls.Add(this.pnlUser, 0, 0);
            this.tblFields.Controls.Add(this.pnlPassword, 1, 0);

            // Sign in
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.Text = "Sign in";
            this.btnSignIn.Dock = DockStyle.Left;
            this.btnSignIn.Width = 104;
            Theme.StyleButton(this.btnSignIn, primary: true);
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = DockStyle.Top;
            this.pnlActions.Height = 56;
            this.pnlActions.Padding = new Padding(0, 18, 0, 0);
            this.pnlActions.Controls.Add(this.btnSignIn);

            // message strip (lblStatus): one sentence, set by the handler
            this.lblStatus.Name = "lblStatus";

            var spacer = new Panel { Dock = DockStyle.Top, Height = 18 };

            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Dock = DockStyle.Top;
            this.pnlBody.Height = 300;
            this.pnlBody.BackColor = Theme.Strip;
            this.pnlBody.Padding = new Padding(30, 26, 30, 22);
            this.pnlBody.Controls.Add(this.lblStatus);
            this.pnlBody.Controls.Add(spacer);
            this.pnlBody.Controls.Add(this.pnlActions);
            this.pnlBody.Controls.Add(this.tblFields);
            this.pnlBody.Controls.Add(this.lblSubheading);
            this.pnlBody.Controls.Add(this.lblHeading);

            // footer
            this.lblFooterCaption.Name = "lblFooterCaption";
            this.lblFooterCaption.Text = "SERVER";
            this.lblFooterCaption.Dock = DockStyle.Left;
            this.lblFooterCaption.AutoSize = true;
            this.lblFooterCaption.Font = Theme.Px(11.5f, FontStyle.Bold);
            this.lblFooterCaption.ForeColor = Theme.Muted;
            this.lblFooterCaption.TextAlign = ContentAlignment.MiddleLeft;
            this.lblFooterMeta.Name = "lblFooterMeta";
            this.lblFooterMeta.Dock = DockStyle.Right;
            this.lblFooterMeta.AutoSize = true;
            this.lblFooterMeta.Font = Theme.Mono(11.5f);
            this.lblFooterMeta.ForeColor = Theme.Faint;
            this.lblFooterMeta.TextAlign = ContentAlignment.MiddleRight;
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Dock = DockStyle.Top;
            this.pnlFooter.Height = 40;
            this.pnlFooter.BackColor = Color.White;
            this.pnlFooter.Padding = new Padding(18, 0, 18, 0);
            this.pnlFooter.CssStyle = "border-top:1px solid #e4eaf1;";
            this.pnlFooter.Controls.Add(this.lblFooterMeta);
            this.pnlFooter.Controls.Add(this.lblFooterCaption);

            // LoginPage
            this.Name = "LoginPage";
            this.Text = "AccessOps";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.appBar);
            this.ResumeLayout(false);
        }

        #endregion

        private AccessOps.UI.AppBar appBar;
        private Wisej.Web.Panel pnlBody;
        private Wisej.Web.Label lblHeading;
        private Wisej.Web.Label lblSubheading;
        private Wisej.Web.TableLayoutPanel tblFields;
        private Wisej.Web.Panel pnlUser;
        private Wisej.Web.TextBox txtUserName;
        private Wisej.Web.Panel pnlPassword;
        private Wisej.Web.TextBox txtPassword;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Button btnSignIn;
        private AccessOps.UI.MessageStrip lblStatus;
        private Wisej.Web.Panel pnlFooter;
        private Wisej.Web.Label lblFooterCaption;
        private Wisej.Web.Label lblFooterMeta;
    }
}
