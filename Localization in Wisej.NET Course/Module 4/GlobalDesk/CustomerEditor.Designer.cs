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
        // Still Localizable, as Module 2 made it: every designed caption, size and position lives
        // in CustomerEditor.resx and its German companion, and ApplyResources puts them on the
        // controls when the control is CONSTRUCTED. The dashboard's CultureChanged handler is the
        // other half of that sentence - it builds a new one.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.pnlHeader = new Wisej.Web.Panel();
            this.lblPanelTitle = new Wisej.Web.Label();
            this.lblBadge = new Wisej.Web.Label();
            this.lblCompany = new Wisej.Web.Label();
            this.txtCompany = new Wisej.Web.TextBox();
            this.lblCode = new Wisej.Web.Label();
            this.txtCode = new Wisej.Web.TextBox();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // the panel header
            //
            resources.ApplyResources(this.lblPanelTitle, "lblPanelTitle");
            this.lblPanelTitle.Name = "lblPanelTitle";
            this.lblPanelTitle.AutoSize = false;
            this.lblPanelTitle.Dock = Wisej.Web.DockStyle.Fill;
            this.lblPanelTitle.Font = Desk.Px(12, FontStyle.Bold);
            this.lblPanelTitle.ForeColor = Desk.Muted;
            this.lblPanelTitle.TextAlign = ContentAlignment.MiddleLeft;

            // Set by the page after a rebuild: it reports which culture this instance was built
            // for, which is a fact about the instance rather than a designed caption.
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.AutoSize = true;
            this.lblBadge.Visible = false;
            this.lblBadge.Dock = Wisej.Web.DockStyle.Right;
            this.lblBadge.Font = Desk.Px(11, FontStyle.Bold);
            this.lblBadge.ForeColor = Color.White;
            this.lblBadge.Padding = new Wisej.Web.Padding(9, 2, 9, 2);
            this.lblBadge.TextAlign = ContentAlignment.MiddleCenter;

            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Dock = Wisej.Web.DockStyle.Top;
            this.pnlHeader.Size = new Size(472, 32);
            this.pnlHeader.BackColor = Color.White;
            this.pnlHeader.Padding = new Wisej.Web.Padding(16, 3, 10, 3);
            this.pnlHeader.CssStyle = "border-bottom:1px solid #e6ecf3";
            this.pnlHeader.Controls.Add(this.lblPanelTitle);
            this.pnlHeader.Controls.Add(this.lblBadge);
            //
            // the two fields
            //
            resources.ApplyResources(this.lblCompany, "lblCompany");
            SetFieldLabel(this.lblCompany, "lblCompany");
            resources.ApplyResources(this.txtCompany, "txtCompany");
            SetField(this.txtCompany, "txtCompany", mono: false);

            resources.ApplyResources(this.lblCode, "lblCode");
            SetFieldLabel(this.lblCode, "lblCode");
            resources.ApplyResources(this.txtCode, "txtCode");
            SetField(this.txtCode, "txtCode", mono: true);
            //
            // btnSave
            //
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.BackColor = Desk.Accent;
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.btnSave.CssStyle = "border:1px solid #1565d8;border-radius:6px";
            this.btnSave.Click += this.btnSave_Click;
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.ForeColor = Desk.FieldInk;
            this.btnCancel.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.btnCancel.CssStyle = "border:1px solid #c9d5e2;border-radius:6px";
            this.btnCancel.Click += this.btnCancel_Click;
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.White;
            this.CssStyle = "border:1px solid #dbe3ec;border-radius:8px";
            this.Controls.Add(this.lblCompany);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.pnlHeader);
            this.ResumeLayout(false);
        }

        private static void SetFieldLabel(Wisej.Web.Label label, string name)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Font = Desk.Px(12.5F, FontStyle.Bold);
            label.ForeColor = Desk.Muted;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        private static void SetField(Wisej.Web.TextBox box, string name, bool mono)
        {
            box.Name = name;
            box.Font = mono ? Desk.Mono(13.5F) : Desk.Px(13.5F);
            box.ForeColor = Desk.Body;
            box.CssStyle = "border:1px solid #c9d5e2;border-radius:5px";
        }

        #endregion

        private Wisej.Web.Panel pnlHeader;
        private Wisej.Web.Label lblPanelTitle;
        private Wisej.Web.Label lblBadge;
        private Wisej.Web.Label lblCompany;
        private Wisej.Web.TextBox txtCompany;
        private Wisej.Web.Label lblCode;
        private Wisej.Web.TextBox txtCode;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
