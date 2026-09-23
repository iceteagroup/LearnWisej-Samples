using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// A designer-localized user control.
    ///
    /// Nothing in this file or in <c>CustomerEditor.Designer.cs</c> contains a caption. Every
    /// localizable property lives in <c>CustomerEditor.resx</c>, with the German overrides in
    /// <c>CustomerEditor.de.resx</c>, and <c>ApplyResources</c> in <c>InitializeComponent</c>
    /// puts them on the controls when the control is constructed.
    ///
    /// The word "constructed" is the whole lesson of Module 2. Designer resources are applied
    /// once, at construction. Change <c>Application.CurrentCulture</c> afterwards and this control
    /// keeps every caption it was born with - which is why the dashboard has a button that throws
    /// it away and builds a new one.
    ///
    /// Messages the control produces at run time are a different matter: they come from
    /// <see cref="Texts"/>, the shared resource, because they are not designed properties of any
    /// control.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        public CustomerEditor()
        {
            InitializeComponent();

            // Country names are data with a display form, not designed captions, so they are
            // filled in code rather than stored in the designer resource.
            this.cboCountry.Items.AddRange(new object[] { "Germany", "France", "Italy", "United Kingdom" });
            this.cboCountry.SelectedIndex = 0;
        }

        /// <summary>The culture this instance was constructed under - the point of the Module 2 demo.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validation messages are run-time text, so they come from the shared resource rather
            // than from this control's designer resource.
            if (string.IsNullOrWhiteSpace(this.txtName.Text))
            {
                this.lblMessage.Text = Texts.Get("Validation.Required");
                return;
            }

            this.lblMessage.Text = Texts.Get("CustomerEditor.Saved");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtName.Text = string.Empty;
            this.txtCode.Text = string.Empty;
            this.lblMessage.Text = string.Empty;
        }
    }
}
