using System.Drawing;
using OrderDesk.Shared;
using Wisej.Web;

namespace OrderDesk.Pages
{
    /// <summary>
    /// Settings = the three capstone review cards: Theme &amp; client profiles, Security review (AllowHtml), and
    /// Deployment (readiness checklist, /health, configuration). The buttons inside the cards are the
    /// "tool buttons" of the modernization toolkit: small actions live next to what they act on; MainPage
    /// wires them (MainPage.Designer.cs) and owns the behaviour.
    /// </summary>
    public sealed class SettingsView : ViewBase
    {
        public static readonly string[] ChecklistItems =
        {
            "Starts through Wisej.NET startup files → main view",
            "No per-user state in unsafe static fields",
            "Registry / local file / Office / process assumptions reviewed",
            "Large grids tested with production-like row counts",
            "Transient dialogs disposed",
            "AllowHtml only with trusted or sanitized content",
            "Responsive tested at desktop, tablet and phone sizes",
            "Config, secrets, logs, temp paths, /health documented",
        };

        // Theme & profiles
        private readonly Panel _themeCard = Card();
        private readonly Label _themeTitle = CardTitle("Theme & client profiles");
        private readonly Label _themeLine = Lbl("", F(9F), Palette.Ink);
        private readonly Label _profileLine = Lbl("", F(9F), Palette.Ink);
        private readonly Label _profilesLine = Lbl("ClientProfiles.json: Phone ≤ 600 · Tablet 601–1024 · Desktop ≥ 1025 (merged with the built-in Phone (Landscape), Tablet (Landscape), Small Desktop)", F(8F), Palette.MutedText);
        public Button ButtonPreviewPhone { get; } = Btn("Preview phone", 110);
        public Button ButtonPreviewTablet { get; } = Btn("Preview tablet", 110);
        public Button ButtonPreviewDesktop { get; } = Btn("Preview desktop", 118);
        public Button ButtonAuto { get; } = Btn("Auto (follow profile)", 140);

        // Security
        private readonly Panel _securityCard = Card();
        private readonly Label _securityTitle = CardTitle("Security review · AllowHtml on user data");
        private readonly Label _rawCaption = Lbl("Order 1038 · Notes as stored (user-entered):", F(9F, true), Palette.MutedText);
        public TextBox RawNotes { get; } = new TextBox { ReadOnly = true, Multiline = true, Font = Mono(9F) };
        private readonly Label _renderedCaption = Lbl("Rendered in a Label:", F(9F, true), Palette.MutedText);
        public Label Rendered { get; } = new Label { AutoSize = false, Font = F(10F), ForeColor = Palette.Ink, BorderStyle = BorderStyle.Solid, BackColor = Palette.PanelBackground, Padding = new Padding(8, 0, 8, 0), TextAlign = ContentAlignment.MiddleLeft, Text = "—" };
        public Label SecurityResult { get; } = new Label { AutoSize = false, Font = F(9F), ForeColor = Palette.MutedText, TextAlign = ContentAlignment.TopLeft, Text = "Choose a render mode." };
        public Button ButtonRenderEncoded { get; } = Btn("Render encoded ✓", 138);
        public Button ButtonRenderAllowHtml { get; } = Btn("Render AllowHtml ✕", 148);
        public Button ButtonRenderSanitized { get; } = Btn("Render sanitized ✓", 142);
        public Label SessionLine { get; } = new Label { AutoSize = false, Font = F(8F), ForeColor = Palette.MutedText, TextAlign = ContentAlignment.MiddleLeft };

        // Deployment
        private readonly Panel _deployCard = Card();
        private readonly Label _deployTitle = CardTitle("Deployment · final readiness checklist");
        private readonly Label[] _checks = new Label[8];
        private readonly bool[] _checkState = new bool[8];
        public Label HealthLine { get; } = new Label { AutoSize = false, Font = Mono(8F), ForeColor = Palette.Ink, TextAlign = ContentAlignment.TopLeft, Text = "GET /health → (run Health check)" };
        public Label ConfigLine { get; } = new Label { AutoSize = false, Font = F(8F), ForeColor = Palette.Ink, TextAlign = ContentAlignment.TopLeft, Text = "Web.config OrderDesk.StorageRoot → (run Show config)" };
        public Label AffinityLine { get; } = new Label { AutoSize = false, Font = F(8F), ForeColor = Palette.MutedText, TextAlign = ContentAlignment.TopLeft, Text = "Session affinity: Wisej.NET keeps the session (controls, UserContext) in the server's memory → behind a load balancer use sticky sessions (cookie affinity) or one node per user group; the /health probe lets the balancer drop a dead node." };
        public Button ButtonShowConfig { get; } = Btn("Show config", 100);
        public LinkLabel HealthLink { get; } = new LinkLabel { Text = "Open /health ↗", AutoSize = false, Font = F(9F), TextAlign = ContentAlignment.MiddleLeft };

        public SettingsView()
        {
            this.AutoScroll = true;

            _themeCard.Controls.Add(_themeTitle);
            _themeCard.Controls.Add(_themeLine);
            _themeCard.Controls.Add(_profileLine);
            _themeCard.Controls.Add(_profilesLine);
            _themeCard.Controls.Add(ButtonPreviewPhone);
            _themeCard.Controls.Add(ButtonPreviewTablet);
            _themeCard.Controls.Add(ButtonPreviewDesktop);
            _themeCard.Controls.Add(ButtonAuto);
            this.Controls.Add(_themeCard);

            _securityCard.Controls.Add(_securityTitle);
            _securityCard.Controls.Add(_rawCaption);
            _securityCard.Controls.Add(RawNotes);
            _securityCard.Controls.Add(_renderedCaption);
            _securityCard.Controls.Add(Rendered);
            _securityCard.Controls.Add(SecurityResult);
            _securityCard.Controls.Add(ButtonRenderEncoded);
            _securityCard.Controls.Add(ButtonRenderAllowHtml);
            _securityCard.Controls.Add(ButtonRenderSanitized);
            _securityCard.Controls.Add(SessionLine);
            this.Controls.Add(_securityCard);

            _deployCard.Controls.Add(_deployTitle);
            for (int i = 0; i < 8; i++)
            {
                _checks[i] = Lbl("☐ " + ChecklistItems[i], F(8F), Palette.MutedText);
                _deployCard.Controls.Add(_checks[i]);
            }
            _deployCard.Controls.Add(HealthLine);
            _deployCard.Controls.Add(ConfigLine);
            _deployCard.Controls.Add(AffinityLine);
            _deployCard.Controls.Add(ButtonShowConfig);
            _deployCard.Controls.Add(HealthLink);
            this.Controls.Add(_deployCard);
        }

        public void ShowTheme(string text) => _themeLine.Text = text;
        public void ShowProfile(string text) => _profileLine.Text = text;

        /// <summary>Turns one checklist item green (☑) or back to pending (☐) with a short note.</summary>
        public void SetCheck(int index, bool ok, string note = null)
        {
            _checkState[index] = ok;
            var label = _checks[index];
            label.Text = (ok ? "☑ " : "☐ ") + ChecklistItems[index] + (string.IsNullOrEmpty(note) ? "" : " — " + note);
            label.ForeColor = ok ? Palette.Good : Palette.MutedText;
            label.ToolTipText = note ?? "";
        }

        public int CheckedCount
        {
            get { int n = 0; foreach (var b in _checkState) if (b) n++; return n; }
        }

        protected override void Relayout()
        {
            int w = this.Width - 2 * Pad;
            if (w < 200) return;
            int y = Pad;
            bool narrow = w < 560;

            // Theme & profiles
            int themeH = narrow ? 138 : 104;
            _themeCard.SetBounds(Pad, y, w, themeH);
            _themeTitle.SetBounds(12, 6, w - 24, 22);
            _themeLine.SetBounds(12, 28, w - 24, 18);
            _profileLine.SetBounds(12, 46, w - 24, 18);
            _profilesLine.SetBounds(12, 64, narrow ? w - 24 : w - 24, 16);
            int bx = 12, by = narrow ? 84 : 80;
            if (!narrow) { _profilesLine.SetBounds(12, 62, w - 24, 16); }
            foreach (var b in new[] { ButtonPreviewPhone, ButtonPreviewTablet, ButtonPreviewDesktop, ButtonAuto })
            {
                if (bx + b.Width > w - 12) { bx = 12; by += 34; }
                b.Location = new Point(bx, by);
                b.Height = 24;
                bx += b.Width + 6;
            }
            y += themeH + 8;

            // Security
            int secH = 236;
            _securityCard.SetBounds(Pad, y, w, secH);
            _securityTitle.SetBounds(12, 6, w - 24, 22);
            _rawCaption.SetBounds(12, 30, w - 24, 16);
            RawNotes.SetBounds(12, 48, w - 24, 44);
            _renderedCaption.SetBounds(12, 96, w - 24, 16);
            Rendered.SetBounds(12, 114, w - 24, 34);
            SecurityResult.SetBounds(12, 152, w - 24, 34);
            bx = 12; by = 186;
            foreach (var b in new[] { ButtonRenderEncoded, ButtonRenderAllowHtml, ButtonRenderSanitized })
            {
                if (bx + b.Width > w - 12) { bx = 12; by += 30; }
                b.Location = new Point(bx, by);
                b.Height = 26;
                bx += b.Width + 6;
            }
            SessionLine.SetBounds(12, secH - 20, w - 24, 16);
            y += secH + 8;

            // Deployment
            int cols = narrow ? 1 : 2;
            int rows = 8 / cols;
            int colW = (w - 24) / cols;
            int deployH = 30 + rows * 20 + 6 + 46 + 4 + 34 + 4 + 44 + 34;
            _deployCard.SetBounds(Pad, y, w, deployH);
            _deployTitle.SetBounds(12, 6, w - 24, 22);
            for (int i = 0; i < 8; i++)
            {
                int col = i / rows, row = i % rows;
                _checks[i].SetBounds(12 + col * colW, 30 + row * 20, colW - 6, 20);
            }
            int cy = 30 + rows * 20 + 6;
            HealthLine.SetBounds(12, cy, w - 24, 46); cy += 46 + 4;
            ConfigLine.SetBounds(12, cy, w - 24, 34); cy += 34 + 4;
            AffinityLine.SetBounds(12, cy, w - 24, 44); cy += 44 + 4;
            ButtonShowConfig.Location = new Point(12, cy); ButtonShowConfig.Height = 26;
            HealthLink.SetBounds(12 + ButtonShowConfig.Width + 10, cy, 160, 26);
            y += deployH + Pad;

            // Tell the scroll container how tall the content is.
            this.AutoScrollMinSize = new Size(0, y);
        }
    }
}
