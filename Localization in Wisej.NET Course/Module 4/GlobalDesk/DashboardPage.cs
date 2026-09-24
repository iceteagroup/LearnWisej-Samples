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
    /// that changes the culture (the <c>?lang=</c> parameter, a saved preference, a support tool)
    /// gets the same refresh for free. A handler that switched the culture and then also refreshed
    /// everything inline would work exactly once, from exactly one control.
    /// </summary>
    public partial class DashboardPage : Page
    {
        /// <summary>
        /// Three cultures: the neutral language, German, and a language-region culture with no
        /// resource file of its own. <c>fr-CA</c> makes fallback visible - English text, because
        /// there is no French resource, with Canadian French formatting, because formatting does
        /// not come from a resource file at all.
        /// </summary>
        private readonly List<string> cultures = new List<string> { "en-US", "de-DE", "fr-CA" };

        // The data. Real values in real types - the three the walkthrough shows.
        private readonly DateTime previewDate = new DateTime(2026, 9, 23);
        private readonly decimal previewCount = 1234567.89m;
        private readonly decimal previewAmount = 1850.75m;

        private CustomerEditor editor;
        private bool switchingCulture;

        public DashboardPage()
        {
            InitializeComponent();

            FillLanguagePicker();

            // One subscription, for the lifetime of the page. Everything that has to follow a
            // culture change is behind it. Removed again in Dispose - the event outlives us.
            Application.CultureChanged += this.Application_CultureChanged;

            CreateEditor(rebuilt: false);
            SyncLanguagePicker();
            ApplyTextResources();
            UpdateCulturePreview();
            ShowSessionCulture(null);
        }

        // ── the switch ──────────────────────────────────────────────────────────

        /// <summary>
        /// The captions in the list are resource values, not hard-coded words - and each one is
        /// the language's own name, which is what a language list shows in every product worth
        /// copying. A German speaker looking for German finds "Deutsch", not "German".
        /// </summary>
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

        /// <summary>
        /// Assigns the culture and nothing else.
        ///
        /// <c>Application.CurrentCulture</c> is per session. It is a property of the user in front
        /// of this browser tab, not of the process - which is the whole reason it must never be
        /// copied into a <c>static</c> field. See docs/CultureSwitch.md.
        /// </summary>
        private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.switchingCulture)
                return;

            var index = this.cboLanguage.SelectedIndex;
            if (index < 0 || index >= this.cultures.Count)
                return;

            var name = this.cultures[index];
            if (name == Application.CurrentCulture.Name)
                return;

            Application.CurrentCulture = CultureInfo.GetCultureInfo(name);
        }

        /// <summary>
        /// Everything that has to happen when the session's culture changes, in one place, no
        /// matter who changed it.
        ///
        /// The order is deliberate: the editor is rebuilt in the same handler as the captions and
        /// the values, so the window is never half translated. The walkthrough shows six seconds
        /// of that state on purpose, and then tells you never to ship it.
        /// </summary>
        private void Application_CultureChanged(object sender, EventArgs e)
        {
            ApplyTextResources();       // shared resources: captions follow immediately
            UpdateCulturePreview();     // culture-sensitive values: reformatted
            CreateEditor(rebuilt: true);// designer resources: only a new instance reads them
            SyncLanguagePicker();       // the picker itself, if something else made the change

            ShowSessionCulture(Texts.Get("Status.EditorRecreated"));
        }

        /// <summary>
        /// Puts the picker in step with the session culture without re-entering the handler. The
        /// culture can be set by <c>?lang=</c> or by the browser's own Accept-Language, and then
        /// nobody touched this combo at all.
        /// </summary>
        private void SyncLanguagePicker()
        {
            this.switchingCulture = true;
            try
            {
                var culture = Application.CurrentCulture;
                var index = this.cultures.IndexOf(culture.Name);

                if (index < 0)
                {
                    // The session arrived on a culture this application does not list - a Spanish
                    // browser, say, or a ?lang= somebody typed. That is not an error: the text
                    // falls back to the neutral file and the values are still formatted correctly.
                    // Showing it keeps the picker honest about where the session actually is. Its
                    // caption is the culture's own native name, because no resource key exists.
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

        // ── the three things a switch has to repaint ────────────────────────────

        /// <summary>Every user-visible word that comes from the shared resource.</summary>
        private void ApplyTextResources()
        {
            this.lblAppTitle.Text = Texts.Get("App.Title");
            this.lblWelcome.Text = Texts.Get("Dashboard.Welcome");
            this.btnCustomers.Text = Texts.Get("Navigation.Customers");
            this.lblLanguage.Text = Texts.Get("Dashboard.Language");
            this.lblPreviewTitle.Text = Texts.Get("Preview.Title");
            this.lblDateCaption.Text = Texts.Get("Preview.Date");
            this.lblCountCaption.Text = Texts.Get("Preview.Count");
            this.lblAmountCaption.Text = Texts.Get("Preview.Amount");
        }

        /// <summary>
        /// The values. Formatted at display time against the session's culture, never stored
        /// formatted, never assembled by hand.
        /// </summary>
        private void UpdateCulturePreview()
        {
            var culture = Application.CurrentCulture;

            this.lblDateValue.Text = this.previewDate.ToString("D", culture);
            this.lblCountValue.Text = this.previewCount.ToString("N2", culture);
            this.lblAmountValue.Text = this.previewAmount.ToString("C", culture);
        }

        /// <summary>
        /// Remove, dispose, construct, add back. The new instance reads the resources of the
        /// culture that is current now, which is the only way a designer-localized control
        /// changes language.
        /// </summary>
        private void CreateEditor(bool rebuilt)
        {
            this.pnlEditorHost.Controls.Clear();

            if (this.editor != null)
            {
                this.editor.Message -= this.Editor_Message;
                this.editor.Dispose();
            }

            this.editor = new CustomerEditor();
            this.editor.Location = new System.Drawing.Point(0, 0);
            this.editor.Message += this.Editor_Message;
            this.pnlEditorHost.Controls.Add(this.editor);

            // The badge states a fact the page can check rather than one it hopes for: which
            // culture this instance was built for, against the one the session is on now.
            if (rebuilt)
            {
                var matches = this.editor.BuiltForCulture == Application.CurrentCulture.Name;
                this.editor.ShowBadge(
                    matches ? Texts.Get("Editor.Recreated")
                            : string.Format(Application.CurrentCulture, Texts.Get("Editor.Stale"), this.editor.BuiltForCulture),
                    matches);
            }
        }

        private void Editor_Message(string message) => ShowSessionCulture(message);

        /// <summary>The status strip: which culture this session is on, and what the last thing
        /// that happened was.</summary>
        private void ShowSessionCulture(string note)
        {
            var culture = Application.CurrentCulture;

            this.lblStatus.Text = string.Format(culture, Texts.Get("Status.SessionCulture"), culture.Name);
            this.lblStatusNote.Visible = note != null;
            this.lblStatusNote.Text = note == null ? string.Empty : "✓ " + note;
        }
    }
}
