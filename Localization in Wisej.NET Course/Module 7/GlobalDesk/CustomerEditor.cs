using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The customer record's two actions, as a designer-localized control.
    ///
    /// Its captions come from <c>CustomerEditor.resx</c> and its language companions, applied by
    /// <c>ApplyResources</c> when the control is constructed. Everything else on the capstone
    /// screen goes through <see cref="LocalizationService"/>, and the contrast between the two is
    /// one of the rows on the QA checklist.
    /// </summary>
    public partial class CustomerEditor : UserControl
    {
        public CustomerEditor()
        {
            InitializeComponent();
        }

        /// <summary>The culture this instance was constructed under - see Module 2.</summary>
        public string BuiltForCulture { get; } = Application.CurrentCulture.Name;

        private void btnSave_Click(object sender, EventArgs e)
        {
            // A Wisej.NET dialog: the title and the message are ours, the Yes and No buttons are
            // the framework's own text - see docs/TextKinds.md.
            var answer = MessageBox.Show(
                LocalizationService.Text("Dialog.UnsavedChanges"),
                LocalizationService.Text("Dialog.SaveChanges"),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            var result = CustomerSaveService.Save("Northwind Traders", null);
            MessageBox.Show(
                LocalizationService.Text(CustomerSaveService.ResourceKeyFor(result)),
                LocalizationService.Text("App.ProductName"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Nothing to discard on this screen; the capstone's editing lives in Module 6.
        }
    }
}
