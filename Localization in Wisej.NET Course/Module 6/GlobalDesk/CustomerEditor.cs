using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The customer card: the two fields, the last-order sentence and the two actions.
    ///
    /// Module 6 did not change what this control does. It changed how its text got there: the
    /// Italian column was filled in a grid of keys against languages, reviewed, and committed as
    /// one change. The file it produced, <c>CustomerEditor.it.resx</c>, is beside the others.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        // Data. An order number is an identifier and a date is a date; neither is words.
        private const int LastOrderNumber = 4187;
        private static readonly DateTime LastOrderDate = new DateTime(2026, 9, 23);

        public CustomerEditor()
        {
            InitializeComponent();

            this.txtName.Text = "Ana Silva";
            this.txtEmail.Text = "ana.silva@example.com";

            ShowLastOrder();
        }

        /// <summary>The culture this instance was constructed under - see Module 2.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        /// <summary>
        /// One resource string, two placeholders. The Italian review caught a machine draft that
        /// had returned them swapped - <c>{1} il {0}</c> - which compiles, ships, and then prints
        /// the date where the order number belongs.
        /// </summary>
        private void ShowLastOrder()
        {
            var culture = Application.CurrentCulture;

            this.lblLastOrder.Text = string.Format(culture,
                Texts.Get("Customer.LastOrder"),
                LastOrderNumber,
                LastOrderDate.ToString("d", culture));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var result = CustomerSaveService.Save(this.txtName.Text, this.txtEmail.Text);
            MessageBox.Show(Texts.Get(CustomerSaveService.ResourceKeyFor(result)),
                Texts.Get("App.ProductName"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.txtName.Text = "Ana Silva";
            this.txtEmail.Text = "ana.silva@example.com";
        }
    }
}
