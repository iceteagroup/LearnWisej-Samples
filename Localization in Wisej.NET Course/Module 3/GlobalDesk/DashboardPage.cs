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
            this.btnSystemText.Text = Texts.Get("Dashboard.ShowSystemText");
            this.btnLastOrder.Text = Texts.Get("Dashboard.LastOrder");

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

            // Two behaviours now, not three: Strings.de.resx exists, so the page's own captions
            // follow the switch as soon as ApplyTextResources runs again. The editor still does
            // not, and that difference is the whole reason it gets its own button.
            this.lblStatus.Text =
                $"Culture is now {name}. The formatted values and every caption on this page " +
                "changed, because Strings.de.resx exists and ApplyTextResources just ran again. " +
                $"The editor did not: it was constructed under {this.editor.BuiltForCulture} and " +
                "designer resources are applied once, at construction. Press Recreate.";
        }

        /// <summary>
        /// A composed sentence, built from one resource string with two placeholders. The date and
        /// the amount are formatted first, for this culture, and then dropped into the sentence -
        /// which is also given the culture, so that string.Format itself does not fall back to the
        /// thread's.
        /// </summary>
        private void btnLastOrder_Click(object sender, EventArgs e)
        {
            this.editor.ShowLastOrder(this.dueDate, this.orderTotal);
            this.lblStatus.Text =
                "One resource string, two placeholders, string.Format with the session culture - " +
                "never three fragments concatenated, because word order is not universal.";
        }

        /// <summary>
        /// Wisej.NET's own text, beside ours, in one dialog.
        ///
        /// The title and the message come from our <c>Strings</c> resource and follow the session
        /// culture. The <b>buttons</b> do not: they are the framework's strings - keys <c>yes</c>
        /// and <c>no</c> in <c>Wisej.Resources</c> - and Wisej.NET already ships German for them,
        /// which is why they read "Ja" and "Nein" without this project doing anything.
        ///
        /// This project also contains <c>Resources.de.resx</c> attempting to override those two
        /// captions, and it does <b>not</b> take effect. The status line reports what is actually
        /// on the buttons rather than what was intended; docs/TextKinds.md records both namings
        /// that were tried and how to check.
        /// </summary>
        private void btnSystemText_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show(
                Texts.Get("CustomerEditor.Saved"),
                Texts.Get("App.Title"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            var language = Application.CurrentCulture.TwoLetterISOLanguageName;

            this.lblStatus.Text =
                $"Title and message: ours, from Strings.{language}.resx. " +
                "Buttons: Wisej.NET's own, from its Wisej.Resources - it ships German for them " +
                "already. The Resources.de.resx override in this project did not replace them; " +
                $"see docs/TextKinds.md. You answered: {answer}.";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            this.lblStatus.Text = Texts.Get("Navigation.Customers") + ": " + Texts.Get("CustomerEditor.Title");
            this.editor?.Focus();
        }
    }
}
