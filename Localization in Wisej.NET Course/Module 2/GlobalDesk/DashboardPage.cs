using System;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The GlobalDesk dashboard, now hosting the designer-localized customer editor.
    ///
    /// Module 1's split still holds - <see cref="ApplyTextResources"/> puts words on screen and
    /// <see cref="UpdateCulturePreview"/> puts values on screen - and Module 2 adds a third thing
    /// that behaves like neither: a control whose captions came from its own designer resource
    /// when it was constructed, and which therefore does not notice a culture change at all until
    /// it is rebuilt.
    /// </summary>
    public partial class DashboardPage : Page
    {
        private static readonly string[] Cultures = { "en-US", "de-DE" };

        private readonly DateTime dueDate = new DateTime(2026, 3, 15);
        private readonly int unitsOrdered = 12500;
        private readonly decimal orderTotal = 1850.75m;

        private CustomerEditor editor;

        public DashboardPage()
        {
            InitializeComponent();

            foreach (var culture in Cultures)
                this.cboCulture.Items.Add(culture);
            this.cboCulture.SelectedItem = Application.CurrentCulture.Name;
            if (this.cboCulture.SelectedIndex < 0)
                this.cboCulture.SelectedIndex = 0;

            CreateEditor();
            ApplyTextResources();
            UpdateCulturePreview();
        }

        private void ApplyTextResources()
        {
            this.Text = Texts.Get("App.Title");

            this.lblWelcome.Text = Texts.Get("Dashboard.Welcome");
            this.lblSubtitle.Text = Texts.Get("Dashboard.Subtitle");
            this.btnCustomers.Text = Texts.Get("Navigation.Customers");
            this.btnRecreate.Text = Texts.Get("Dashboard.RecreateEditor");

            this.lblPreviewHeading.Text = Texts.Get("Preview.Heading");
            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblQuantityCaption.Text = Texts.Get("Preview.Quantity");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
            this.lblCultureCaption.Text = Texts.Get("Preview.Culture");
        }

        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.dueDate.ToString("D", culture);
            this.lblQuantityValue.Text = this.unitsOrdered.ToString("N0", culture);
            this.lblAmountValue.Text = this.orderTotal.ToString("C", culture);
            this.lblCultureValue.Text = $"{culture.Name} - {culture.DisplayName}";
        }

        // ── the editor, and why it has to be rebuilt ────────────────────────────

        /// <summary>
        /// Disposes whatever editor is in the host and builds a new one. The new instance runs
        /// <c>InitializeComponent</c>, which runs <c>ApplyResources</c>, which reads the culture
        /// that is current <b>now</b> - so this is the only way a designer-localized control
        /// changes language.
        /// </summary>
        private void CreateEditor()
        {
            this.pnlEditorHost.Controls.Clear();
            this.editor?.Dispose();

            this.editor = new CustomerEditor { Location = new System.Drawing.Point(0, 0) };
            this.pnlEditorHost.Controls.Add(this.editor);
        }

        private void btnRecreate_Click(object sender, EventArgs e)
        {
            var before = this.editor.BuiltForCulture;

            CreateEditor();

            this.lblStatus.Text =
                $"Editor rebuilt. The old instance was constructed under {before}; " +
                $"the new one under {this.editor.BuiltForCulture}.";
        }

        // ── the crude culture switch, replaced properly in Module 4 ─────────────

        private void cboCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            var name = (string)this.cboCulture.SelectedItem;
            if (string.IsNullOrEmpty(name) || name == Application.CurrentCulture.Name)
                return;

            // Per session. Never a static field - see Module 4.
            Application.CurrentCulture = CultureInfo.GetCultureInfo(name);

            // The shared resources follow immediately, because Texts.Get is called again here.
            ApplyTextResources();
            UpdateCulturePreview();

            // Three different behaviours, and the status line names all three rather than hiding
            // the awkward one.
            this.lblStatus.Text =
                $"Culture is now {name}. The formatted values changed immediately. " +
                "The page's own captions did not, because there is no Strings.de.resx yet - " +
                "Module 3 adds it. And the editor did not either, for a different reason: it was " +
                $"constructed under {this.editor.BuiltForCulture}, and designer resources are " +
                "applied once, at construction. Press Recreate to see that one change.";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            this.lblStatus.Text = Texts.Get("Navigation.Customers") + ": " + Texts.Get("CustomerEditor.Title");
            this.editor?.Focus();
        }
    }
}
