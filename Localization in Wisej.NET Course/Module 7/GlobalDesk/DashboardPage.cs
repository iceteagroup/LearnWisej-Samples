using System;
using System.Collections.Generic;
using System.Globalization;
using Wisej.Web;

namespace GlobalDesk
{
    /// <summary>
    /// The dashboard with a language switch that finishes the job.
    ///
    /// The shape that matters is the indirection. The combo box handler does exactly one thing -
    /// it assigns <c>Application.CurrentCulture</c> - and then stops. Everything that has to
    /// happen afterwards hangs off <see cref="Application.CultureChanged"/>, so any other code
    /// that changes the culture (a URL parameter, a saved preference, a support tool) gets the
    /// same refresh for free. A handler that switched the culture and then also refreshed
    /// everything inline would work exactly once, from exactly one button.
    /// </summary>
    public partial class DashboardPage : Page
    {
        /// <summary>
        /// Five cultures: English, German, Austrian German (fallback), Arabic (right-to-left) and
        /// the pseudo-locale. qps-ploc is a test tool, not a language - every value in it is
        /// bracketed and lengthened, so a caption that is not bracketed is a string that never
        /// went through a resource.
        /// </summary>
        private static readonly string[] Cultures = { "en-US", "de-DE", "de-AT", "it-IT", "ar-EG", "qps-ploc" };

        private readonly DateTime dueDate = new DateTime(2026, 3, 15);
        private readonly int unitsOrdered = 12500;
        private readonly decimal orderTotal = 1850.75m;

        private CustomerEditor editor;
        private bool switchingCulture;

        public DashboardPage()
        {
            InitializeComponent();

            foreach (var culture in Cultures)
                this.cboCulture.Items.Add(culture);

            // One subscription, for the lifetime of the session. Everything that has to follow a
            // culture change is behind it.
            Application.CultureChanged += this.Application_CultureChanged;

            LoadContacts();
            LoadTickets();
            CreateEditor();
            SyncCultureCombo();
            ApplyTextResources();
            UpdateCulturePreview();
        }

        // ── the switch ──────────────────────────────────────────────────────────

        /// <summary>
        /// Assigns the culture and nothing else.
        ///
        /// <c>Application.CurrentCulture</c> is per session. It is a property of the user in front
        /// of this browser tab, not of the process - which is the whole reason it must never be
        /// copied into a <c>static</c> field. See docs/CultureSwitch.md.
        /// </summary>
        private void cboCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.switchingCulture)
                return;

            var name = (string)this.cboCulture.SelectedItem;
            if (string.IsNullOrEmpty(name) || name == Application.CurrentCulture.Name)
                return;

            Application.CurrentCulture = CultureInfo.GetCultureInfo(name);
        }

        /// <summary>
        /// Everything that has to happen when the session's culture changes, in one place, no
        /// matter who changed it.
        /// </summary>
        private void Application_CultureChanged(object sender, EventArgs e)
        {
            ApplyTextResources();      // shared resources: captions follow immediately
            UpdateCulturePreview();    // culture-sensitive values: reformatted
            CreateEditor();            // designer resources: only a new instance reads them
            LoadTickets();             // the grid holds formatted text, so it is rebuilt too
            SyncCultureCombo();        // the combo itself, if something else made the change

            var culture = Application.CurrentCulture;
            var textFrom = culture.Name == "de-AT" ? "Strings.de.resx (de-AT falls back to de)" : $"Strings.{culture.TwoLetterISOLanguageName}.resx";

            this.lblStatus.Text =
                $"CultureChanged: {culture.Name}. Text from {textFrom}; values formatted for " +
                $"{culture.Name}; the editor was rebuilt. RightToLeft: {Application.RightToLeft} " +
                "(set by rightToLeft: auto in Default.json, not by this code).";
        }

        /// <summary>
        /// Puts the combo in step with the session culture without re-entering the handler. The
        /// culture can be set by the <c>?lang=</c> URL parameter or by the browser's own
        /// Accept-Language, and then nobody touched this combo at all.
        /// </summary>
        private void SyncCultureCombo()
        {
            this.switchingCulture = true;
            try
            {
                var name = Application.CurrentCulture.Name;
                this.cboCulture.SelectedItem = name;

                if (this.cboCulture.SelectedIndex < 0)
                {
                    // The session arrived on a culture this application does not list - a French
                    // browser, say. That is not an error: the text falls back to the neutral file
                    // and the values are still formatted correctly.
                    this.cboCulture.Items.Insert(0, name);
                    this.cboCulture.SelectedIndex = 0;
                }
            }
            finally
            {
                this.switchingCulture = false;
            }
        }

        // ── words ───────────────────────────────────────────────────────────────

        private void ApplyTextResources()
        {
            this.Text = Texts.Get("App.Title");

            this.lblWelcome.Text = Texts.Get("Dashboard.Welcome");
            this.lblSubtitle.Text = Texts.Get("Dashboard.Subtitle");
            this.btnCustomers.Text = Texts.Get("Navigation.Customers");
            this.btnRecreate.Text = Texts.Get("Dashboard.RecreateEditor");
            this.btnSystemText.Text = Texts.Get("Dashboard.ShowSystemText");
            this.btnLastOrder.Text = Texts.Get("Dashboard.LastOrder");

            this.lblContactsHeader.Text = LocalizationService.Text("Dashboard.Contacts");
            this.lblTicketsHeader.Text = LocalizationService.Text("Dashboard.Tickets");
            this.btnQa.Text = LocalizationService.Text("Dashboard.RunQa");
            this.colContactName.HeaderText = Texts.Get("Contacts.Name");
            this.colContactRole.HeaderText = Texts.Get("Contacts.Role");
            this.colContactPhone.HeaderText = Texts.Get("Contacts.Phone");

            this.lblPreviewHeading.Text = Texts.Get("Preview.Heading");
            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblQuantityCaption.Text = Texts.Get("Preview.Quantity");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
            this.lblCultureCaption.Text = Texts.Get("Preview.Culture");
        }

        // ── values ──────────────────────────────────────────────────────────────

        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.dueDate.ToString("D", culture);
            this.lblQuantityValue.Text = this.unitsOrdered.ToString("N0", culture);
            this.lblAmountValue.Text = this.orderTotal.ToString("C", culture);

            // The parent culture is what decides which resource file answers. Showing it makes
            // de-AT's fallback visible instead of mysterious.
            var parent = culture.Parent;
            this.lblCultureValue.Text = parent != null && parent.Name.Length > 0
                ? $"{culture.Name} - {culture.DisplayName}  ({Texts.Get("Dashboard.Fallback")} {parent.Name})"
                : $"{culture.Name} - {culture.DisplayName}";
        }

        // ── the tickets grid: domain values, localized at the edge ──────────────

        private static readonly Ticket[] Tickets =
        {
            new Ticket("GD-1042", TicketStatus.New, new DateTime(2026, 2, 3), 480m),
            new Ticket("GD-1043", TicketStatus.InProgress, new DateTime(2026, 2, 17), 1275.50m),
            new Ticket("GD-1044", TicketStatus.Waiting, new DateTime(2026, 3, 1), 96m),
            new Ticket("GD-1045", TicketStatus.Closed, new DateTime(2026, 3, 9), 2310.25m),
        };

        /// <summary>
        /// The domain hands over a <see cref="TicketStatus"/>, a <see cref="DateTime"/> and a
        /// <see cref="decimal"/>. Every one of them becomes text here, at the edge, and nowhere
        /// else - which is why this method has to run again on a culture change.
        ///
        /// The reference is the exception: it is an identifier and is written straight through.
        /// </summary>
        private void LoadTickets()
        {
            // Both of these were hard-coded English until the pseudo-locale pass showed them
            // unbracketed beside everything else on the screen. That is what qps-ploc is for.
            this.colTicketRef.HeaderText = LocalizationService.Text("Ticket.Reference");
            this.colTicketStatus.HeaderText = LocalizationService.Text("Ticket.Status");
            this.colTicketRaised.HeaderText = LocalizationService.Text("Preview.Date");
            this.colTicketValue.HeaderText = LocalizationService.Text("Preview.Amount");

            this.gridTickets.Rows.Clear();

            foreach (var ticket in Tickets)
            {
                this.gridTickets.Rows.Add(
                    ticket.Reference,
                    LocalizationService.Text(Ticket.ResourceKeyFor(ticket.Status)),
                    LocalizationService.Date(ticket.Raised, "d"),
                    LocalizationService.Currency(ticket.Value));
            }
        }

        // ── the QA matrix ───────────────────────────────────────────────────────

        /// <summary>
        /// Runs the parts of the localization test matrix a program can answer, and says plainly
        /// which rows still need a person looking at the screen. The full matrix is in
        /// docs/LocalizationQA.md.
        /// </summary>
        private void btnQa_Click(object sender, EventArgs e)
        {
            var report = new System.Text.StringBuilder();
            var culture = Application.CurrentCulture;

            report.Append($"QA under {culture.Name}: ");

            // 1. fallback chain
            var parent = culture.Parent != null && culture.Parent.Name.Length > 0 ? culture.Parent.Name : "(neutral)";
            report.Append($"fallback {culture.Name} -> {parent} -> neutral. ");

            // 2. formatting
            var today = LocalizationService.Date(DateTime.Today, "d");
            var number = LocalizationService.Number(1234567);
            var amount = LocalizationService.Currency(1850.75m);

            report.Append($"date {today}, number {number}, currency {amount}. ");

            // 3. RTL
            report.Append($"RightToLeft {Application.RightToLeft}. ");

            // 4. a key that is deliberately absent from the pseudo-locale
            var probe = LocalizationService.Text("Ticket.Waiting");
            report.Append($"Ticket.Waiting -> \"{probe}\". ");

            // 5. culture folders in the deployed output
            report.Append(DescribeSatellites());

            // 6. what the missing-key policy has collected this session
            var missing = LocalizationService.MissingKeysThisSession;
            var missingList = string.Join(", ", missing);
            report.Append(missing.Count == 0
                ? "No missing keys this session."
                : $"Missing keys this session: {missingList}.");

            this.lblStatus.Text = report.ToString();
        }

        /// <summary>
        /// The check that catches a broken deployment: satellite assemblies live in per-culture
        /// folders beside the application, and a copy step that misses them leaves every session
        /// silently falling back to the neutral text.
        /// </summary>
        private static string DescribeSatellites()
        {
            try
            {
                var root = System.IO.Path.GetDirectoryName(typeof(DashboardPage).Assembly.Location);
                var found = new List<string>();

                foreach (var directory in System.IO.Directory.GetDirectories(root))
                {
                    var name = System.IO.Path.GetFileName(directory);
                    if (System.IO.File.Exists(System.IO.Path.Combine(directory, "GlobalDesk.resources.dll")))
                        found.Add(name);
                }

                found.Sort(StringComparer.Ordinal);
                return found.Count == 0
                    ? "No culture folders found beside the application - every session will fall back to neutral. "
                    : "Culture folders present: " + string.Join(", ", found) + ". ";
            }
            catch (Exception ex)
            {
                return $"Could not inspect the culture folders: {ex.Message}. ";
            }
        }

        // ── the contacts grid ───────────────────────────────────────────────────

        /// <summary>
        /// Three contacts, so the module has a grid to look at under an RTL culture. The phone
        /// numbers are deliberately in the set - they are the same kind of neutral-character
        /// identifier as the customer code, and what the grid does with them is the thing to
        /// record.
        /// </summary>
        private void LoadContacts()
        {
            this.gridContacts.DataSource = new[]
            {
                new Contact("Anna Vogel", "Purchasing", "+49 30 5550 118"),
                new Contact("Bilal Haddad", "Operations", "+20 2 2555 0447"),
                new Contact("Clara Moretti", "Finance", "+39 02 5550 903"),
            };
        }

        /// <summary>One row of the contacts grid.</summary>
        public class Contact
        {
            public Contact(string name, string role, string phone)
            {
                Name = name;
                Role = role;
                Phone = phone;
            }

            public string Name { get; }

            public string Role { get; }

            public string Phone { get; }
        }

        // ── the editor ──────────────────────────────────────────────────────────

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
                $"Editor rebuilt by hand. Old instance: {before}; new instance: {this.editor.BuiltForCulture}. " +
                "The CultureChanged handler already does this - the button is here so you can see it on its own.";
        }

        // ── the rest ────────────────────────────────────────────────────────────

        private void btnLastOrder_Click(object sender, EventArgs e)
        {
            this.editor.ShowLastOrder(this.dueDate, this.orderTotal);
            this.lblStatus.Text =
                "One resource string, two placeholders, string.Format with the session culture - " +
                "never three fragments concatenated, because word order is not universal.";
        }

        /// <summary>
        /// Wisej.NET's own text, beside ours, in one dialog. See docs/TextKinds.md in Module 3 for
        /// the override that does not take effect - the status line reports what is really there.
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
                $"see Module 3's docs/TextKinds.md. You answered: {answer}.";
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            this.lblStatus.Text = Texts.Get("Navigation.Customers") + ": " + Texts.Get("CustomerEditor.Title");
            this.editor?.Focus();
        }
    }
}
