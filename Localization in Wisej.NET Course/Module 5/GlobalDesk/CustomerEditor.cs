using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The customer screen: the code field, the contacts grid and the action buttons.
    ///
    /// Two kinds of text meet here, and Module 5 is where the difference becomes visible on
    /// screen. The code label and the two buttons are <b>designed</b> captions, applied by
    /// <c>ApplyResources</c> at construction. The grid's column headers and its city values are
    /// produced while the application runs, so they come from the shared resource through
    /// <see cref="Texts"/>.
    ///
    /// Counting the files is the check that catches a gap: one <c>.resx</c> per language per
    /// designer-localized control, plus one shared file per language. A language with fewer files
    /// than the others has a hole that falls back silently.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        /// <summary>
        /// The contact list. The company name is <b>data</b> - a registered legal name, the same
        /// characters in every language. The person's display name and the city go through
        /// resource keys, because both genuinely have a form per language: a city has exonyms
        /// (Milan / Mailand / ميلانو) and a person's name has a local-script form.
        ///
        /// What is stored is the key, never the rendered value. Store the rendering and the row
        /// belongs to whoever saved it.
        /// </summary>
        private static readonly string[][] Contacts =
        {
            new[] { "Contact.Dana",  "Northwind Traders", "City.Milan" },
            new[] { "Contact.Omar",  "Fabrikam Inc",      "City.Doha" },
            new[] { "Contact.Priya", "Contoso Ltd",       "City.Pune" },
        };

        public CustomerEditor()
        {
            InitializeComponent();

            // The container, not the children. Every control inside stays on RightToLeft.Inherit
            // and mirrors with it - except txtCustomerCode, which the designer file pins.
            this.RightToLeftLayout = true;

            // An identifier, not a caption: the same characters in every language.
            this.txtCustomerCode.Text = "GD-40117-AR";

            LoadContacts();
        }

        /// <summary>The culture this instance was constructed under - see Module 2.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        private void LoadContacts()
        {
            this.colContact.HeaderText = Texts.Get("Contacts.Contact");
            this.colCompany.HeaderText = Texts.Get("Contacts.Company");
            this.colCity.HeaderText = Texts.Get("Contacts.City");

            this.gridContacts.Rows.Clear();
            foreach (var contact in Contacts)
                this.gridContacts.Rows.Add(Texts.Get(contact[0]), contact[1], Texts.Get(contact[2]));

            if (this.gridContacts.Rows.Count > 0)
                this.gridContacts.Rows[0].Selected = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var result = CustomerSaveService.Save(this.txtCustomerCode.Text, null);
            MessageBox.Show(Texts.Get(CustomerSaveService.ResourceKeyFor(result)),
                Texts.Get("App.ProductName"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtCustomerCode.Text = "GD-40117-AR";
            LoadContacts();
        }
    }
}
