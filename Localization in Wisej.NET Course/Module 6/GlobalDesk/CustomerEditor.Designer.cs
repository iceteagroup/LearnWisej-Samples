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
        // Every localizable property - Text, Size, Location, RightToLeft and the rest - moves out
        // of this file and into CustomerEditor.resx, and ApplyResources puts it back at run time
        // from whichever .resx matches the current culture. Nothing below says "Save" or "150, 38";
        // it says "look up btnSave".
        //
        // The consequence that catches everyone: ApplyResources runs here, in InitializeComponent,
        // which means it runs when the control is CONSTRUCTED. A culture change afterwards does
        // not re-run it. That is why the page has a Recreate button.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(CustomerEditor));

            this.lblTitle = new Wisej.Web.Label();
            this.lblName = new Wisej.Web.Label();
            this.txtName = new Wisej.Web.TextBox();
            this.lblCode = new Wisej.Web.Label();
            this.txtCode = new Wisej.Web.TextBox();
            this.lblCountry = new Wisej.Web.Label();
            this.cboCountry = new Wisej.Web.ComboBox();
            this.btnSave = new Wisej.Web.Button();
            this.btnCancel = new Wisej.Web.Button();
            this.lblMessage = new Wisej.Web.Label();
            this.SuspendLayout();
            //
            // lblTitle
            //
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Font = new System.Drawing.Font("default", 12F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Name = "lblTitle";
            //
            // lblName
            //
            resources.ApplyResources(this.lblName, "lblName");
            this.lblName.Name = "lblName";
            //
            // txtName
            //
            resources.ApplyResources(this.txtName, "txtName");
            this.txtName.Name = "txtName";
            //
            // lblCode
            //
            resources.ApplyResources(this.lblCode, "lblCode");
            this.lblCode.Name = "lblCode";
            //
            // txtCode
            //
            resources.ApplyResources(this.txtCode, "txtCode");
            this.txtCode.Name = "txtCode";
            // THE ONE EXCEPTION ON THIS SCREEN.
            //
            // A customer code is an identifier, not prose: AT0417 is typed, read aloud, quoted in
            // an email and compared character by character. Under an RTL layout the browser would
            // reorder it on screen - the letters and the digits are neutral characters and take
            // their direction from the paragraph - so a user reading it back would say the wrong
            // thing. Forcing LTR here keeps the identifier identical in every language.
            //
            // The same applies to IBANs, part numbers, licence keys and anything a user has to
            // transcribe. It does NOT apply to names or addresses, which are prose and must
            // mirror.
            this.txtCode.RightToLeft = Wisej.Web.RightToLeft.No;
            //
            // lblCountry
            //
            resources.ApplyResources(this.lblCountry, "lblCountry");
            this.lblCountry.Name = "lblCountry";
            //
            // cboCountry
            //
            resources.ApplyResources(this.cboCountry, "cboCountry");
            this.cboCountry.DropDownStyle = Wisej.Web.ComboBoxStyle.DropDownList;
            this.cboCountry.Name = "cboCountry";
            //
            // btnSave
            //
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += this.btnSave_Click;
            //
            // btnCancel
            //
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Click += this.btnCancel_Click;
            //
            // lblMessage
            //
            resources.ApplyResources(this.lblMessage, "lblMessage");
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(21, 101, 216);
            this.lblMessage.Name = "lblMessage";
            //
            // CustomerEditor
            //
            resources.ApplyResources(this, "$this");
            this.Name = "CustomerEditor";
            // Module 5: the editor mirrors with the page. Its children stay on
            // RightToLeft.Inherit - except txtCode, below.
            this.RightToLeftLayout = true;
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cboCountry);
            this.Controls.Add(this.lblCountry);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblTitle);
            this.ResumeLayout(false);
        }

        #endregion

        private Wisej.Web.Label lblTitle;
        private Wisej.Web.Label lblName;
        private Wisej.Web.TextBox txtName;
        private Wisej.Web.Label lblCode;
        private Wisej.Web.TextBox txtCode;
        private Wisej.Web.Label lblCountry;
        private Wisej.Web.ComboBox cboCountry;
        private Wisej.Web.Button btnSave;
        private Wisej.Web.Button btnCancel;
        private Wisej.Web.Label lblMessage;
    }
}
