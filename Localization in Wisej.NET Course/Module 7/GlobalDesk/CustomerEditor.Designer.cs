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
        // What is left of the designer-localized control on the capstone screen: the two actions
        // for the customer record. It is still Localizable, and it is still the thing that proves
        // the rule - its captions arrive when it is constructed, so the dashboard rebuilds it in
        // the CultureChanged handler rather than trying to refresh it.
        //
        // The QA checklist's "runtime switch with a designer-localized control on screen" row is
        // run against this control.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.pnlButtons = new Wisej.Web.FlowLayoutPanel();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.SuspendLayout();
            //
            // AutoSize with a MinimumSize in a FlowLayoutPanel that wraps: "Kundendatensatz
            // speichern" is twice the width of "Save customer record" and costs no override.
            //
            SetActionButton(this.btnSave, "btnSave", primary: true);
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Click += this.btnSave_Click;

            SetActionButton(this.btnCancel, "btnCancel", primary: false);
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Click += this.btnCancel_Click;

            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Dock = Wisej.Web.DockStyle.Fill;
            this.pnlButtons.FlowDirection = Wisej.Web.FlowDirection.LeftToRight;
            this.pnlButtons.WrapContents = true;
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            this.BackColor = Color.Transparent;
            this.Controls.Add(this.pnlButtons);
            this.ResumeLayout(false);
        }

        private static void SetActionButton(Wisej.Web.Button button, string name, bool primary)
        {
            button.Name = name;
            button.AutoSize = true;
            button.MinimumSize = new Size(88, 32);
            button.Margin = new Wisej.Web.Padding(0, 0, 10, 0);
            button.Padding = new Wisej.Web.Padding(15, 0, 15, 0);
            button.Font = Desk.Px(12.5F, FontStyle.Bold);
            button.BackColor = primary ? Desk.Accent : Color.White;
            button.ForeColor = primary ? Color.White : Desk.FieldInk;
            button.CssStyle = primary
                ? "border:1px solid #1565d8;border-radius:7px"
                : "border:1px solid #cdd9e6;border-radius:7px";
        }

        #endregion

        private Wisej.Web.FlowLayoutPanel pnlButtons;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
    }
}
