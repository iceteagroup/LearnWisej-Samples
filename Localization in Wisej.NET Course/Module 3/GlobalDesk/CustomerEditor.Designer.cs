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
        // Still Localizable, as Module 2 left it: every designed caption, size and position lives
        // in CustomerEditor.resx and its German companion, and ApplyResources puts them on the
        // controls when the control is constructed.
        //
        // What Module 3 adds is the other half of the screen - the last-order sentence, the save
        // confirmation and the validation message. Those are NOT designed properties of any
        // control: they are produced at run time, and they come from the shared Strings.resx
        // through Texts.Get. Their labels are created here with no text at all.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.lblName = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.lblEmail = new Wisej.Web.Label();
            this.txtEmail = new Wisej.Web.TextBox();
            this.lblNotes = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.lblValidation = new Wisej.Web.Label();
            this.lblLastOrder = new Wisej.Web.Label();
            this.lblMessage = new Wisej.Web.Label();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // the three rows - designed captions, from the designer resource
            //
            resources.ApplyResources(this.lblName, "lblName");
            SetFieldLabel(this.lblName, "lblName");
            resources.ApplyResources(this.txtName, "txtName");
            SetField(this.txtName, "txtName");

            resources.ApplyResources(this.lblEmail, "lblEmail");
            SetFieldLabel(this.lblEmail, "lblEmail");
            resources.ApplyResources(this.txtEmail, "txtEmail");
            SetField(this.txtEmail, "txtEmail");

            resources.ApplyResources(this.lblNotes, "lblNotes");
            SetFieldLabel(this.lblNotes, "lblNotes");
            resources.ApplyResources(this.txtNotes, "txtNotes");
            SetField(this.txtNotes, "txtNotes");
            //
            // lblValidation - a run-time message, so it has no designed text
            //
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.AutoSize = false;
            this.lblValidation.Location = new Point(442, 0);
            this.lblValidation.Size = new Size(400, 50);
            this.lblValidation.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.lblValidation.ForeColor = Desk.BadInk;
            this.lblValidation.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblLastOrder - one composed sentence, built at run time
            //
            this.lblLastOrder.Name = "lblLastOrder";
            this.lblLastOrder.AutoSize = false;
            this.lblLastOrder.Location = new Point(0, 160);
            this.lblLastOrder.Size = new Size(842, 24);
            this.lblLastOrder.Font = Desk.Px(13.5F, FontStyle.Italic);
            this.lblLastOrder.ForeColor = Desk.FieldInk;
            this.lblLastOrder.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblMessage - the save result, shown as a pill
            //
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.AutoSize = true;
            this.lblMessage.Visible = false;
            this.lblMessage.Location = new Point(0, 196);
            this.lblMessage.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.lblMessage.Padding = new Wisej.Web.Padding(14, 7, 14, 7);
            this.lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            //
            // btnSave
            //
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.BackColor = Desk.Accent;
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.btnSave.CssStyle = "border:1px solid #1565d8;border-radius:7px";
            this.btnSave.Click += this.btnSave_Click;
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.ForeColor = Desk.FieldInk;
            this.btnCancel.Font = Desk.Px(13.5F, FontStyle.Bold);
            this.btnCancel.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnCancel.Click += this.btnCancel_Click;
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.White;
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblValidation);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblLastOrder);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.ResumeLayout(false);
        }

        /// <summary>The caption column of one row. Its text, position and size come from the
        /// resource; its colour and font do not, because neither has a German variant.</summary>
        private static void SetFieldLabel(Wisej.Web.Label label, string name)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Font = Desk.Px(14.5F);
            label.ForeColor = Desk.FieldInk;
            label.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>The editor column of one row.</summary>
        private static void SetField(Wisej.Web.TextBox box, string name)
        {
            box.Name = name;
            box.Font = Desk.Mono(14.5F);
            box.ForeColor = Desk.Body;
            box.CssStyle = "border:1.5px solid #c9d4e0;border-radius:6px";
        }

        #endregion

        private Wisej.Web.Label lblName;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Label lblEmail;
        private Wisej.Web.TextBox txtEmail;
        private Wisej.Web.Label lblNotes;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Label lblValidation;
        private Wisej.Web.Label lblLastOrder;
        private Wisej.Web.Label lblMessage;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
