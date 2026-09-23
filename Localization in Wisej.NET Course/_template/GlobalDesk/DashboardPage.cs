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
    /// - <see cref="UpdateCulturePreview"/> puts <b>values</b> on screen. A date, a count and an
    ///   amount are not words; they are formatted at the moment they are displayed, with the
    ///   session's culture as the format provider.
    ///
    /// Nothing stored here is ever a formatted string. The model holds a <see cref="DateTime"/>,
    /// an <see cref="int"/> and a <see cref="decimal"/>, which is what a database column and an
    /// API response should hold too.
    /// </summary>
    public partial class DashboardPage : Page
    {
        // The data. Real values, stored in real types - not "15/03/2026" and not "1.850,75 EUR".
        private readonly DateTime dueDate = new DateTime(2026, 3, 15);
        private readonly int unitsOrdered = 12500;
        private readonly decimal orderTotal = 1850.75m;

        public DashboardPage()
        {
            InitializeComponent();

            ApplyTextResources();
            UpdateCulturePreview();
        }

        /// <summary>
        /// Every user-visible word on this page, from a key. Written as its own method - rather
        /// than inline in the constructor - because from Module 4 on it is called again whenever
        /// the session's culture changes.
        /// </summary>
        private void ApplyTextResources()
        {
            this.Text = Texts.Get("App.Title");

            this.lblWelcome.Text = Texts.Get("Dashboard.Welcome");
            this.lblSubtitle.Text = Texts.Get("Dashboard.Subtitle");
            this.btnCustomers.Text = Texts.Get("Navigation.Customers");

            this.lblPreviewHeading.Text = Texts.Get("Preview.Heading");
            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblQuantityCaption.Text = Texts.Get("Preview.Quantity");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
            this.lblCultureCaption.Text = Texts.Get("Preview.Culture");
        }

        /// <summary>
        /// The other half of localization. Each of these three values is formatted against
        /// <c>Application.CurrentCulture</c>, which is the culture of <b>this session</b> - so two
        /// users in the same server process can read two different date patterns at the same time.
        ///
        /// Note what is not happening here: no string concatenation, no currency symbol typed by
        /// hand, no <c>"dd/MM/yyyy"</c>. The standard format specifiers - "D", "N0", "C" - let the
        /// culture decide the pattern, the separators and the symbol, which is the only way this
        /// works in a country nobody has thought about yet.
        /// </summary>
        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.dueDate.ToString("D", culture);
            this.lblQuantityValue.Text = this.unitsOrdered.ToString("N0", culture);
            this.lblAmountValue.Text = this.orderTotal.ToString("C", culture);

            this.lblCultureValue.Text = $"{culture.Name} - {culture.DisplayName}";

            this.lblStatus.Text =
                $"Stored: {this.dueDate:yyyy-MM-dd}, {this.unitsOrdered}, {this.orderTotal} - " +
                $"the same three values, before any culture touched them.";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            // The customer editor arrives in Module 2.
            this.lblStatus.Text = Texts.Get("Navigation.Customers") + ": " + Texts.Get("CustomerEditor.Title");
        }
    }
}
