using System;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The GlobalDesk dashboard, prepared for localization from the first line.
    ///
    /// Two jobs are kept apart on this page and the split is the whole point:
    ///
    /// - <see cref="ApplyTextResources"/> puts <b>words</b> on screen. Every one of them comes
    ///   from a resource key, so a translator can change all of them without touching code.
    /// - <see cref="UpdateCulturePreview"/> puts <b>values</b> on screen. A date, a quantity and
    ///   an amount are not words; they are formatted at the moment they are displayed, with the
    ///   session's culture as the format provider.
    ///
    /// Nothing stored here is ever a formatted string. The model holds a <see cref="DateTime"/>
    /// and two <see cref="decimal"/> values, which is what a database column and an API response
    /// should hold too.
    /// </summary>
    public partial class DashboardPage : Page
    {
        // The data. Real values, stored in real types - not "14/10/2025" and not "1.850,75 EUR".
        // These are the three the walkthrough uses, so the screen and the video read the same.
        private readonly DateTime previewDate = new DateTime(2025, 10, 14);
        private readonly decimal previewCount = 1234567.89m;
        private readonly decimal previewAmount = 1850.75m;

        public DashboardPage()
        {
            InitializeComponent();

            ApplyTextResources();
            UpdateCulturePreview();
            ReportStatus();
        }

        /// <summary>
        /// Every user-visible word on this page, from a key. Written as its own method - rather
        /// than inline in the constructor - because from Module 4 on it is called again whenever
        /// the session's culture changes.
        /// </summary>
        private void ApplyTextResources()
        {
            this.lblAppTitle.Text = Texts.Get("App.Title");
            this.lblWelcome.Text = Texts.Get("Dashboard.Welcome");
            this.btnCustomers.Text = Texts.Get("Navigation.Customers");

            this.lblPreviewTitle.Text = Texts.Get("Preview.Title");
            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblCountCaption.Text = Texts.Get("Preview.Count");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
        }

        /// <summary>
        /// The other half of localization. Each of these three values is formatted against
        /// <c>Application.CurrentCulture</c>, which is the culture of <b>this session</b> - so two
        /// users in the same server process can read two different date patterns at the same time.
        ///
        /// Note what is not happening here: no string concatenation, no currency symbol typed by
        /// hand, no <c>"dd/MM/yyyy"</c>. The standard format specifiers - "D", "N2", "C" - let the
        /// culture decide the pattern, the separators and the symbol, which is the only way this
        /// works in a country nobody has thought about yet.
        /// </summary>
        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.previewDate.ToString("D", culture);
            this.lblCountValue.Text = this.previewCount.ToString("N2", culture);
            this.lblAmountValue.Text = this.previewAmount.ToString("C", culture);

            // Not a caption and not a value: the name of the culture doing the formatting.
            // It is deliberately not translated - "de-DE" is the same string in every language.
            this.lblCulture.Text = "Application.CurrentCulture = " + culture.Name;
        }

        /// <summary>
        /// What the load actually did, counted rather than claimed: how many distinct resource
        /// keys came back with a value, and how many values the culture formatted.
        /// </summary>
        private void ReportStatus()
        {
            var culture = Application.CurrentCulture;
            var template = Texts.Get("Status.Ready");

            // string.Format gets the culture too. Without it, it would use whatever culture the
            // thread happens to be on, which in a server application is whatever it last did.
            this.lblStatus.Text = string.Format(culture, template, Texts.ResolvedCount, 3);
            this.lblStatus.BackColor = Desk.GoodBack;
            this.lblStatus.ForeColor = Desk.GoodInk;
        }
    }
}
