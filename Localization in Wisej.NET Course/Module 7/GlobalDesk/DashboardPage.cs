using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The capstone screen.
    ///
    /// Every user-visible string on it comes through <see cref="LocalizationService"/> - one door,
    /// one missing-key policy, one place that reads <c>Application.CurrentCulture</c>. The domain
    /// hands the UI values: a <see cref="TicketStatus"/>, a <see cref="DateTime"/> and a
    /// <see cref="decimal"/>, never a sentence and never a formatted string.
    ///
    /// The QA checklist in docs/LocalizationQA.md is run against this page.
    /// </summary>
    public partial class DashboardPage : Page
    {
        private readonly List<string> cultures = new List<string>
            { "en-US", "de-DE", "it-IT", "fr-CA", "ar-SA", "qps-ploc" };

        /// <summary>The week's tickets. Values in real types; nothing here is presentation.</summary>
        private static readonly Ticket[] Tickets =
        {
            new Ticket("GD-1042", "Northwind Traders", TicketStatus.Open,    new DateTime(2026, 9, 23), 1290.50m),
            new Ticket("GD-1041", "Contoso Ltd",       TicketStatus.Waiting, new DateTime(2026, 9, 24),  760.00m),
            new Ticket("GD-1040", "Fabrikam Inc",      TicketStatus.Closed,  new DateTime(2026, 9, 25), 2769.50m),
        };

        private readonly DateTime dueDate = new DateTime(2026, 9, 23);
        private readonly decimal openAmount = 4820.00m;
        private readonly decimal resolvedShare = 0.875m;

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
            LoadTickets();
        }

        // ── the switch ──────────────────────────────────────────────────────────

        private void FillLanguagePicker()
        {
            this.switchingCulture = true;
            try
            {
                foreach (var culture in this.cultures)
                    this.cboLanguage.Items.Add(LocalizationService.Text("Language." + culture));
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

        /// <summary>
        /// Everything a culture change has to repaint, in one place: the captions, the values,
        /// the grid (whose cells hold formatted text), and the designer-localized control, which
        /// is rebuilt rather than refreshed. The screen is never half translated.
        /// </summary>
        private void Application_CultureChanged(object sender, EventArgs e)
        {
            ApplyTextResources();
            LoadTickets();
            CreateEditor();
            SyncLanguagePicker();
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
            this.lblAppTitle.Text = LocalizationService.Text("App.Title");

            this.lblNavDashboard.Text = LocalizationService.Text("Nav.Dashboard");
            this.lblNavCustomers.Text = LocalizationService.Text("Nav.Customers");
            this.lblNavTickets.Text = LocalizationService.Text("Nav.Tickets");
            this.lblNavSettings.Text = LocalizationService.Text("Nav.Settings");

            this.lblHeading.Text = LocalizationService.Text("Dashboard.OpenTickets");

            this.lblDueCaption.Text = LocalizationService.Text("Kpi.DueDate");
            this.lblAmountCaption.Text = LocalizationService.Text("Kpi.OpenAmount");
            this.lblResolvedCaption.Text = LocalizationService.Text("Kpi.Resolved");

            // Values, through the same service. Nothing here writes a separator or a symbol.
            this.lblDueValue.Text = LocalizationService.Date(this.dueDate);
            this.lblAmountValue.Text = LocalizationService.Currency(this.openAmount);
            this.lblResolvedValue.Text = LocalizationService.Number(this.resolvedShare, "P1");
        }

        /// <summary>
        /// The grid holds formatted text, so it is rebuilt on every culture change rather than
        /// reformatted in place.
        ///
        /// The status column is the point of the module: the model returns a
        /// <see cref="TicketStatus"/>, the UI maps it to a resource key, and the mapping lives
        /// beside the enum so renaming a member is a compile error rather than a missing key.
        /// </summary>
        private void LoadTickets()
        {
            this.colReference.HeaderText = LocalizationService.Text("Ticket.Reference");
            this.colCustomer.HeaderText = LocalizationService.Text("Ticket.Customer");
            this.colDue.HeaderText = LocalizationService.Text("Ticket.Due");
            this.colAmount.HeaderText = LocalizationService.Text("Ticket.Amount");
            this.colStatus.HeaderText = LocalizationService.Text("Ticket.Status");

            this.gridTickets.Rows.Clear();
            foreach (var ticket in Tickets)
            {
                this.gridTickets.Rows.Add(
                    ticket.Reference,
                    ticket.Customer,
                    LocalizationService.Date(ticket.Due),
                    LocalizationService.Currency(ticket.Amount),
                    LocalizationService.Text(Ticket.ResourceKeyFor(ticket.Status)));
            }
        }

        private void CreateEditor()
        {
            this.pnlEditorHost.Controls.Clear();
            this.editor?.Dispose();

            this.editor = new CustomerEditor();
            this.editor.Dock = DockStyle.Fill;
            this.pnlEditorHost.Controls.Add(this.editor);
        }
    }
}
