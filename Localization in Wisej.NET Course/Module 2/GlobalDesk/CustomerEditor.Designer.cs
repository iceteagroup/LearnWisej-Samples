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
        // This is what the designer writes once Localizable is set to true.
        //
        // Every localizable property - Text, Size, Location and the rest - moves out of this file
        // and into CustomerEditor.resx, and ApplyResources puts it back at run time from whichever
        // .resx matches the current culture. Nothing below says "Save changes" or "128, 38"; it
        // says "look up btnSave".
        //
        // The consequence that catches everyone: ApplyResources runs here, in InitializeComponent,
        // which means it runs when the control is CONSTRUCTED. A culture change afterwards does
        // not re-run it. That is why the host page has a Recreate editor button.
        //
        // What is still set in code below is what is NOT language-specific: colours, fonts and
        // border styling. A colour does not have a German variant, and leaving it in the resource
        // file would mean maintaining it once per language.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.pnlDetails = new Wisej.Web.Panel();
            this.lblDetails = new Wisej.Web.Label();
            this.lblCode = new Wisej.Web.Label();
            this.txtCode = new Wisej.Web.TextBox();
            this.lblName = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.lblCity = new Wisej.Web.Label();
            this.txtCity = new Wisej.Web.TextBox();
            this.lblNotes = new Wisej.Web.Label();
            this.txtNotes = new Wisej.Web.TextBox();
            this.btnCancel = new Wisej.Web.Button();
            this.btnSave = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // lblDetails - the group header
            //
            resources.ApplyResources(this.lblDetails, "lblDetails");
            this.lblDetails.Name = "lblDetails";
            this.lblDetails.AutoSize = false;
            this.lblDetails.Font = Desk.Px(14, FontStyle.Bold);
            this.lblDetails.ForeColor = Desk.GroupInk;
            this.lblDetails.TextAlign = ContentAlignment.MiddleLeft;
            //
            // the three detail rows
            //
            resources.ApplyResources(this.lblCode, "lblCode");
            SetFieldLabel(this.lblCode, "lblCode");
            resources.ApplyResources(this.txtCode, "txtCode");
            SetField(this.txtCode, "txtCode");

            resources.ApplyResources(this.lblName, "lblName");
            SetFieldLabel(this.lblName, "lblName");
            resources.ApplyResources(this.txtName, "txtName");
            SetField(this.txtName, "txtName");

            resources.ApplyResources(this.lblCity, "lblCity");
            SetFieldLabel(this.lblCity, "lblCity");
            resources.ApplyResources(this.txtCity, "txtCity");
            SetField(this.txtCity, "txtCity");
            //
            // pnlDetails
            //
            resources.ApplyResources(this.pnlDetails, "pnlDetails");
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.BackColor = Color.White;
            this.pnlDetails.CssStyle = "border:1px solid #dbe3ec;border-radius:8px";
            this.pnlDetails.Controls.Add(this.lblDetails);
            this.pnlDetails.Controls.Add(this.lblCode);
            this.pnlDetails.Controls.Add(this.txtCode);
            this.pnlDetails.Controls.Add(this.lblName);
            this.pnlDetails.Controls.Add(this.txtName);
            this.pnlDetails.Controls.Add(this.lblCity);
            this.pnlDetails.Controls.Add(this.txtCity);
            //
            // the notes row
            //
            resources.ApplyResources(this.lblNotes, "lblNotes");
            SetFieldLabel(this.lblNotes, "lblNotes");
            this.lblNotes.TextAlign = ContentAlignment.TopLeft;

            resources.ApplyResources(this.txtNotes, "txtNotes");
            SetField(this.txtNotes, "txtNotes");
            this.txtNotes.Multiline = true;
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.ForeColor = Desk.FieldInk;
            this.btnCancel.Font = Desk.Px(14, FontStyle.Bold);
            this.btnCancel.CssStyle = "border:1px solid #cdd9e6;border-radius:7px";
            this.btnCancel.Click += this.btnCancel_Click;
            //
            // btnSave
            //
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.BackColor = Desk.Accent;
            this.btnSave.ForeColor = Color.White;
            this.btnSave.Font = Desk.Px(14, FontStyle.Bold);
            this.btnSave.CssStyle = "border:1px solid #1565d8;border-radius:7px";
            this.btnSave.Click += this.btnSave_Click;
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.White;
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.ResumeLayout(false);
        }

        /// <summary>The caption column of one row. Its text, position and size come from the
        /// resource; its colour and font do not, because neither has a German variant.</summary>
        private static void SetFieldLabel(Wisej.Web.Label label, string name)
        {
            label.Name = name;
            label.AutoSize = false;
            label.Font = Desk.Px(15);
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

        private Wisej.Web.Panel pnlDetails;
        private Wisej.Web.Label lblDetails;
        private Wisej.Web.Label lblCode;
        private Wisej.Web.TextBox txtCode;
        private Wisej.Web.Label lblName;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Label lblCity;
        private Wisej.Web.TextBox txtCity;
        private Wisej.Web.Label lblNotes;
        private Wisej.Web.TextBox txtNotes;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Button btnSave;
    }
}
