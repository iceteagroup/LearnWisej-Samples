using System.Drawing;
using Wisej.Web;

namespace GlobalDesk
{
    partial class CustomerEditor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Wisej.NET Designer generated code

        //
        // The customer card: two fields, the last-order sentence and the two actions.
        //
        // Still Localizable, and now with five language files beside the neutral one. The Italian
        // file is the one Module 6 produced, and it was produced the same way as the others - as
        // a column in a grid, with comments and invariant markers, not by editing XML.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.pnlFields = new Wisej.Web.Panel();
            this.pnlName = new Wisej.Web.Panel();
            this.lblName = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.pnlEmail = new Wisej.Web.Panel();
            this.lblEmail = new Wisej.Web.Label();
            this.txtEmail = new Wisej.Web.TextBox();
            this.pnlActions = new Wisej.Web.Panel();
            this.lblLastOrder = new Wisej.Web.Label();
            this.pnlButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // the two field columns
            //
            resources.ApplyResources(this.lblName, "lblName");
            SetFieldLabel(this.lblName, "lblName");
            SetField(this.txtName, "txtName");
            this.pnlName.Name = "pnlName";
            this.pnlName.Dock = Wisej.Web.DockStyle.Left;
            this.pnlName.Size = new Size(430, 62);
            this.pnlName.BackColor = Color.White;
            this.pnlName.Padding = new Wisej.Web.Padding(0, 0, 14, 0);
            this.pnlName.Controls.Add(this.txtName);
            this.pnlName.Controls.Add(this.lblName);

            resources.ApplyResources(this.lblEmail, "lblEmail");
            SetFieldLabel(this.lblEmail, "lblEmail");
            SetField(this.txtEmail, "txtEmail");
            this.pnlEmail.Name = "pnlEmail";
            this.pnlEmail.Dock = Wisej.Web.DockStyle.Left;
            this.pnlEmail.Size = new Size(430, 62);
            this.pnlEmail.BackColor = Color.White;
            this.pnlEmail.Controls.Add(this.txtEmail);
            this.pnlEmail.Controls.Add(this.lblEmail);

            this.pnlFields.Name = "pnlFields";
            this.pnlFields.Dock = Wisej.Web.DockStyle.Top;
            this.pnlFields.Size = new Size(1100, 62);
            this.pnlFields.BackColor = Color.White;
            this.pnlFields.Controls.Add(this.pnlEmail);
            this.pnlFields.Controls.Add(this.pnlName);
            //
            // the last-order sentence and the two actions
            //
            // lblLastOrder carries no designed text: it is composed at run time from one resource
            // string with two placeholders, so it lives in Strings.resx, not here.
            this.lblLastOrder.Name = "lblLastOrder";
            this.lblLastOrder.AutoSize = false;
            this.lblLastOrder.Dock = Wisej.Web.DockStyle.Fill;
            this.lblLastOrder.Font = Desk.Px(13);
            this.lblLastOrder.ForeColor = Desk.Muted;
            this.lblLastOrder.TextAlign = ContentAlignment.MiddleLeft;

            SetActionButton(this.btnSave, "btnSave", primary: true);
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Click += this.btnSave_Click;

            SetActionButton(this.btnCancel, "btnCancel", primary: false);
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Click += this.btnCancel_Click;

            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Dock = Wisej.Web.DockStyle.Right;
            this.pnlButtons.Size = new Size(320, 34);
            this.pnlButtons.FlowDirection = Wisej.Web.FlowDirection.RightToLeft;
            this.pnlButtons.WrapContents = false;
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnSave);

            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Dock = Wisej.Web.DockStyle.Top;
            this.pnlActions.Size = new Size(1100, 34);
            this.pnlActions.BackColor = Color.White;
            this.pnlActions.Margin = new Wisej.Web.Padding(0, 14, 0, 0);
            this.pnlActions.Controls.Add(this.lblLastOrder);
            this.pnlActions.Controls.Add(this.pnlButtons);
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.White;
            this.Padding = new Wisej.Web.Padding(17, 15, 17, 15);
            this.CssStyle = "border:1px solid #e1e9f2;border-radius:10px";
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlFields);
            this.ResumeLayout(false);
        }

        private static void SetFieldLabel(Wisej.Web.Label label, string name)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Dock = Wisej.Web.DockStyle.Top;
            label.Size = new Size(416, 20);
            label.Font = Desk.Px(12.5F, FontStyle.Bold);
            label.ForeColor = Desk.Muted;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void SetField(Wisej.Web.TextBox box, string name)
        {
            box.Name = name;
            box.Dock = Wisej.Web.DockStyle.Top;
            box.Size = new Size(416, 34);
            box.Font = Desk.Px(14);
            box.ForeColor = Desk.Body;
            box.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
        }

        private static void SetActionButton(Wisej.Web.Button button, string name, bool primary)
        {
            button.Name = name;
            button.AutoSize = true;
            button.MinimumSize = new Size(88, 32);
            button.Margin = new Wisej.Web.Padding(10, 0, 0, 0);
            button.Padding = new Wisej.Web.Padding(18, 0, 18, 0);
            button.Font = Desk.Px(13, FontStyle.Bold);
            button.BackColor = primary ? Desk.Accent : Color.White;
            button.ForeColor = primary ? Color.White : Color.FromArgb(0x41, 0x56, 0x6C);
            button.CssStyle = primary
                ? "border:1px solid #1565d8;border-radius:7px"
                : "border:1px solid #cdd9e6;border-radius:7px";
        }

        #endregion

        private Wisej.Web.Panel pnlFields;
        private Wisej.Web.Panel pnlName;
        private Wisej.Web.Label lblName;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Panel pnlEmail;
        private Wisej.Web.Label lblEmail;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Panel pnlActions;
        private Wisej.Web.Label lblLastOrder;
        private Wisej.Web.FlowLayoutPanel pnlButtons;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
