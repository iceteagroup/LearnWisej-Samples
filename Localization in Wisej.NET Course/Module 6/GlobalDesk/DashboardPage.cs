using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The dashboard, with the screen one translation round produced.
    ///
    /// Module 6 changed no behaviour. It added a language - Italian - the way a team adds one:
    /// every key and every language in one grid, the untranslated filter as the measure, comments
    /// on the keys whose meaning is not obvious out of context, invariant markers on the ones
    /// nobody may translate, and a review of the resource diff before the round was committed.
    ///
    /// The number in the rail is what makes that a process rather than a feeling.
    /// </summary>
    public partial class DashboardPage : Page
    {
        private readonly List<string> cultures = new List<string>
            { "en-US", "de-DE", "it-IT", "fr-CA", "ar-SA", "qps-ploc" };

        // Data. A date and two numbers, formatted at display time and never stored formatted.
        private readonly DateTime previewDate = new DateTime(2026, 9, 23);
        private readonly decimal previewCount = 1250m;
        private readonly decimal previewAmount = 1250m;

        private CustomerEditor editor;
        private bool switchingCulture;

        public DashboardPage()
        {
            InitializeComponent();

            this.RightToLeftLayout = true;

            FillLanguagePicker();
            Application.CultureChanged += this.Application_CultureChanged;

            CreateEditor();
            SyncLanguagePicker();
            ApplyTextResources();
            UpdateCulturePreview();
            ShowTranslationReport();
        }

        // ── the switch ──────────────────────────────────────────────────────────

        private void FillLanguagePicker()
        {
            this.switchingCulture = true;
            try
            {
                foreach (var culture in this.cultures)
                    this.cboLanguage.Items.Add(Texts.Get("Language." + culture));
            }
            finally
            {
                this.switchingCulture = false;
            }
        }

        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.switchingCulture)
                return;

            var index = this.cboLanguage.SelectedIndex;
            if (index < 0 || index >= this.cultures.Count)
                return;

            var name = this.cultures[index];
            if (string.Equals(name, Application.CurrentCulture.Name, StringComparison.OrdinalIgnoreCase))
                return;

            Application.CurrentCulture = CultureInfo.GetCultureInfo(name);
        }

        private void Application_CultureChanged(object sender, EventArgs e)
        {
            ApplyTextResources();
            UpdateCulturePreview();
            CreateEditor();
            SyncLanguagePicker();
            ShowTranslationReport();
        }

        /// <summary>
        /// Culture names are compared case-insensitively on purpose.
        /// <c>CultureInfo.GetCultureInfo("qps-ploc").Name</c> comes back as <c>qps-Ploc</c>:
        /// .NET normalises the case of the second part, and an ordinal comparison against the
        /// string this picker was built from then fails and reports the culture as unknown.
        /// </summary>
        private int IndexOfCulture(string name) =>
            this.cultures.FindIndex(c => string.Equals(c, name, StringComparison.OrdinalIgnoreCase));

        private void SyncLanguagePicker()
        {
            this.switchingCulture = true;
            try
            {
                var culture = Application.CurrentCulture;
                var index = IndexOfCulture(culture.Name);

                if (index < 0)
                {
                    this.cultures.Insert(0, culture.Name);
                    this.cboLanguage.Items.Insert(0, culture.NativeName);
                    index = 0;
                }

                this.cboLanguage.SelectedIndex = index;
            }
            finally
            {
                this.switchingCulture = false;
            }
        }

        // ── what a switch repaints ──────────────────────────────────────────────

        private void ApplyTextResources()
        {
            var culture = Application.CurrentCulture;

            this.lblAppTitle.Text = Texts.Get("App.Title");
            this.lblBrand.Text = Texts.Get("App.ProductName");
            this.lblRailBrand.Text = Texts.Get("App.ProductName");
            this.lblLanguage.Text = Texts.Get("Dashboard.Language");

            this.lblNavCustomers.Text = Texts.Get("Nav.Customers");
            this.lblNavTickets.Text = Texts.Get("Nav.Tickets");
            this.lblCultureHeading.Text = Texts.Get("Rail.Culture");

            this.lblTitle.Text = Texts.Get("Dashboard.Title");
            this.lblWelcome.Text = string.Format(culture,
                Texts.Get("Dashboard.WelcomeBack"), Texts.Get("User.DisplayName"));

            this.lblPreviewTitle.Text = Texts.Get("Preview.Title");
            this.lblChipOpen.Text = Texts.Get("Ticket.Open");
            this.lblChipClosed.Text = Texts.Get("Ticket.Closed");

            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblCountCaption.Text = Texts.Get("Preview.Count");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
        }

        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.previewDate.ToString("d", culture);
            this.lblCountValue.Text = this.previewCount.ToString("N2", culture);
            this.lblAmountValue.Text = this.previewAmount.ToString("C", culture);
        }

        private void CreateEditor()
        {
            this.pnlEditorHost.Controls.Clear();
            this.editor?.Dispose();

            this.editor = new CustomerEditor();
            this.editor.Dock = DockStyle.Fill;
            this.pnlEditorHost.Controls.Add(this.editor);
        }

        /// <summary>
        /// Which resource file this session is reading, and how many keys still answer with the
        /// neutral value.
        ///
        /// It is the untranslated filter from the grid, asked of the running application instead.
        /// Finishing the round in the grid is not the same as finishing it here: fallback would
        /// have shown English for a missing key and the screen would still have looked complete.
        /// </summary>
        private void ShowTranslationReport()
        {
            var culture = Application.CurrentCulture;
            var neutral = culture.TwoLetterISOLanguageName == "en";

            this.lblCultureLine.Text = culture.Name + " · " +
                (neutral ? Texts.Get("Culture.Neutral") : "Strings." + culture.TwoLetterISOLanguageName + ".resx");

            // The count is a heuristic, not a verdict: a value a translator legitimately left
            // identical - "Email" in Italian, "Ana" in any language - looks exactly like a key
            // nobody touched. It is amber rather than red for that reason, and the tooltip names
            // the keys so the reviewer can settle each one in a second.
            var same = Texts.UntranslatedKeys();
            this.lblUntranslated.Text = string.Format(culture, Texts.Get("Culture.Untranslated"), same.Count);
            this.lblUntranslated.ForeColor = same.Count == 0 ? Desk.GoodInk : Desk.WarnInk;
            this.lblUntranslated.ToolTipText = same.Count == 0 ? null : string.Join("\n", same);
        }
    }
}
