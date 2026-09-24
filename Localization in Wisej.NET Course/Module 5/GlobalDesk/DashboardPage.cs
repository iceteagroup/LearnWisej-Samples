using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The dashboard, now able to survive a right-to-left culture and a translation half as long
    /// again as the English.
    ///
    /// Two settings do the mirroring: <c>"rightToLeft": "auto"</c> in <c>Default.json</c> and
    /// <see cref="RightToLeftLayout"/> on this page and on the customer screen. Every child
    /// control is left on <c>RightToLeft.Inherit</c>, which is the default - <b>set the container,
    /// leave the children alone</b>. Nothing in this file tests for Arabic.
    /// </summary>
    public partial class DashboardPage : Page
    {
        /// <summary>
        /// Five cultures: the neutral language, German, a language-region culture with no resource
        /// file of its own, a right-to-left culture, and the pseudo-locale. <c>qps-ploc</c> is a
        /// test tool, not a language - every value in it is bracketed and lengthened, so a caption
        /// that is not bracketed is a string that never went through a resource.
        /// </summary>
        private readonly List<string> cultures = new List<string> { "en-US", "de-DE", "fr-CA", "ar-SA", "qps-ploc" };

        private CustomerEditor editor;
        private bool switchingCulture;

        public DashboardPage()
        {
            InitializeComponent();

            // The whole mirroring technique, in one property. The children inherit.
            this.RightToLeftLayout = true;

            FillLanguagePicker();
            Application.CultureChanged += this.Application_CultureChanged;

            CreateEditor();
            SyncLanguagePicker();
            ApplyTextResources();
            ShowStatus();
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
            CreateEditor();          // designer resources are read at construction
            SyncLanguagePicker();
            ShowStatus();
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
            this.lblWelcome.Text = string.Format(culture,
                Texts.Get("Dashboard.WelcomeBack"), Texts.Get("User.DisplayName"));

            this.lblNavDashboard.Text = Texts.Get("Nav.Dashboard");
            this.lblNavCustomers.Text = Texts.Get("Nav.Customers");
            this.lblNavContacts.Text = Texts.Get("Nav.Contacts");
            this.lblNavSettings.Text = Texts.Get("Nav.Settings");
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
        /// The culture and the reading direction, reported rather than assumed.
        ///
        /// <c>Application.RightToLeft</c> is derived from the session's culture by the
        /// <c>rightToLeft: "auto"</c> setting. No code here keeps a list of right-to-left
        /// languages, which is the only version of this that is still correct next year.
        /// </summary>
        private void ShowStatus()
        {
            var culture = Application.CurrentCulture;
            // Wisej exposes this as a bool on Application: it is derived from the session culture
            // by the rightToLeft: "auto" setting, not decided by this code.
            var rtl = Application.RightToLeft;

            this.lblStatus.Text = string.Format(culture,
                Texts.Get("Status.Direction"),
                culture.Name,
                Texts.Get(rtl ? "Direction.Rtl" : "Direction.Ltr"));
        }
    }
}
