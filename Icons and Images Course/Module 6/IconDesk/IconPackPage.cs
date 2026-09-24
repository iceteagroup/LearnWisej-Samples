using System;
using System.Collections.Generic;
using System.Drawing;
using Wisej.Web;
using Pack = IconDesk.Icons.AppIcons;

namespace IconDesk
{
    /// <summary>
    /// IconDesk running against its own icon pack.
    ///
    /// The gallery draws all twelve icons out of one referenced assembly: four assigned the way
    /// the designer writes them, eight from the <c>AppIcons</c> catalog. The recolour check below
    /// it puts the <c>?color=</c> suffix on the three status icons at four sizes and shows what
    /// happens when artwork fixes its own colours.
    /// </summary>
    public partial class IconPackPage : Page
    {
        // ── the two palettes the pack is checked under ──────────────────────────

        private sealed class PackTheme
        {
            public string Name;
            public Color Surface;
            public Color Card;
            public Color Line;
            public Color Ink;
            public Color ChipBack;
            public Color ChipInk;
        }

        private static readonly PackTheme Light = new PackTheme
        {
            Name = "Bootstrap-4",
            Surface = Color.FromArgb(0xF6, 0xF8, 0xFB),
            Card = Color.White,
            Line = Color.FromArgb(0xE4, 0xEA, 0xF1),
            Ink = Color.FromArgb(0x1F, 0x2D, 0x3A),
            ChipBack = Color.White,
            ChipInk = Color.FromArgb(0x3A, 0x4D, 0x63),
        };

        private static readonly PackTheme Dark = new PackTheme
        {
            Name = "BootstrapDark-4",
            Surface = Color.FromArgb(0x1A, 0x25, 0x32),
            Card = Color.FromArgb(0x22, 0x30, 0x3F),
            Line = Color.FromArgb(0x33, 0x45, 0x5A),
            Ink = Color.FromArgb(0xCF, 0xE0, 0xF5),
            ChipBack = Color.FromArgb(0x1F, 0x2D, 0x3A),
            ChipInk = Color.White,
        };

        /// <summary>
        /// The three colour checks: name, catalog source, suffix, and the colour it should land
        /// on. A theme colour <b>name</b> keeps making sense when the theme changes; the literal
        /// in the middle is here so the difference is on screen.
        /// </summary>
        private static readonly string[][] Checks =
        {
            new[] { "Success", Pack.Success, "?color=success", "#1f9d6b" },
            new[] { "Warning", Pack.Warning, "?color=#e8a13c", "#e8a13c" },
            new[] { "Error", Pack.Error, "?color=invalid", "#d93a2b" },
        };

        /// <summary>
        /// The Error icon as it arrived from the designer, embedded in the application rather than
        /// in the pack. Its shapes carry a literal stroke colour, so the suffix is a silent no-op.
        /// </summary>
        private const string ErrorDraft = "resource.wx/IconDesk/status-error-draft.svg";

        private readonly List<Panel> tiles = new List<Panel>();
        private readonly List<Label> tileNames = new List<Label>();
        private readonly List<Panel> checkCards = new List<Panel>();
        private readonly List<Label> checkNames = new List<Label>();
        private readonly List<Label> checkPills = new List<Label>();
        private readonly List<Label> checkFaults = new List<Label>();
        private readonly List<PictureBox[]> checkPictures = new List<PictureBox[]>();

        private PackTheme theme = Light;
        private bool cleaned;

        public IconPackPage()
        {
            InitializeComponent();

            this.lblPackVersion.Text = Pack.Assembly + " v" + Pack.Version;
            this.lblGalleryStatus.Text =
                "12 icons rendered from one referenced assembly — 4 picked in the designer, 8 from AppIcons";

            BuildGallery();
            BuildRecolourCheck();

            ApplyTheme();
            ReportRecolour();
        }

        // ── the gallery: twelve icons, one assembly ─────────────────────────────

        /// <summary>
        /// The first four are assigned the way the Visual Studio image explorer writes them - a
        /// literal <c>resource.wx</c> string. The other eight come from the catalog, where a typo
        /// is a compiler error rather than a blank control.
        /// </summary>
        private void BuildGallery()
        {
            for (var i = 0; i < Pack.All.Count; i++)
            {
                var entry = Pack.All[i];
                var fromDesigner = i < 4;

                var picture = new PictureBox
                {
                    Location = new Point(0, 14),
                    Name = "pic" + entry.Key,
                    Size = new Size(28, 28),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ImageSource = entry.Value,
                };

                var name = new Label
                {
                    AutoSize = false,
                    Font = new Font("Consolas", 9.4F, FontStyle.Bold),
                    Location = new Point(4, 52),
                    Text = entry.Key,
                    TextAlign = ContentAlignment.TopCenter,
                };

                var fore = fromDesigner ? "#7d5ae0" : "#1565d8";
                var tag = new Label
                {
                    AutoSize = false,
                    BackColor = ColorTranslator.FromHtml(fromDesigner ? "#f2edfd" : "#eaf3ff"),
                    CssStyle = "border:1px solid " + fore + "55;border-radius:999px;",
                    Font = new Font("default", 7.9F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(fore),
                    Location = new Point(4, 76),
                    Text = fromDesigner ? "designer" : "AppIcons",
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                var tile = new Panel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, 12, 12),
                    Name = "tile" + entry.Key,
                };
                tile.Controls.Add(picture);
                tile.Controls.Add(name);
                tile.Controls.Add(tag);
                tile.Resize += (s, e) =>
                {
                    var width = tile.ClientSize.Width;
                    picture.Left = (width - picture.Width) / 2;
                    name.Size = new Size(width - 8, 22);
                    tag.Size = new Size(width - 8, 20);
                };

                this.layoutGrid.Controls.Add(tile, i % 4, i / 4);
                this.tiles.Add(tile);
                this.tileNames.Add(name);
            }
        }

        // ── the recolour check: three icons, four sizes each ────────────────────

        private void BuildRecolourCheck()
        {
            var sizes = new[] { 16, 24, 32, 48 };

            for (var i = 0; i < Checks.Length; i++)
            {
                var check = Checks[i];
                var pictures = new PictureBox[sizes.Length];
                var card = new Panel { Name = "card" + check[0] };

                for (var s = 0; s < sizes.Length; s++)
                {
                    pictures[s] = new PictureBox
                    {
                        Location = new Point(16, 18 + (48 - sizes[s])),
                        Name = "pic" + check[0] + sizes[s],
                        Size = new Size(sizes[s], sizes[s]),
                        SizeMode = PictureBoxSizeMode.Zoom,
                    };
                    card.Controls.Add(pictures[s]);
                }

                var name = new Label
                {
                    AutoSize = false,
                    Font = new Font("Consolas", 10.1F),
                    Location = new Point(0, 84),
                    TextAlign = ContentAlignment.TopCenter,
                };

                var pill = new Label
                {
                    AutoSize = false,
                    Font = new Font("Consolas", 9F, FontStyle.Bold),
                    Location = new Point(0, 110),
                    Text = check[2],
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                var fault = new Label
                {
                    AutoSize = false,
                    Font = new Font("default", 9.4F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#b3261e"),
                    Location = new Point(0, 138),
                    TextAlign = ContentAlignment.TopCenter,
                    Visible = false,
                };

                card.Controls.Add(name);
                card.Controls.Add(pill);
                card.Controls.Add(fault);
                card.Resize += (s, e) =>
                {
                    var width = card.ClientSize.Width;
                    var glyphWidth = 16 + 24 + 32 + 48 + 3 * 14;
                    var x = (width - glyphWidth) / 2;

                    foreach (var picture in pictures)
                    {
                        picture.Left = x;
                        x += picture.Width + 14;
                    }

                    name.Size = new Size(width, 22);
                    pill.Size = new Size(width - 60, 24);
                    pill.Left = 30;
                    fault.Size = new Size(width, 20);
                };

                this.pnlRecolour.Controls.Add(card);
                this.checkCards.Add(card);
                this.checkNames.Add(name);
                this.checkPills.Add(pill);
                this.checkFaults.Add(fault);
                this.checkPictures.Add(pictures);
            }

            // The cleanup is a button on the card that failed, so the QA pass can be walked
            // through without a second control appearing anywhere else on the page.
            this.btnCleanUp = new Button
            {
                BackColor = ColorTranslator.FromHtml("#fff3f1"),
                CssStyle = "border:1px solid #f2b8b0;border-radius:7px;",
                Font = new Font("default", 9.4F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#b3261e"),
                Location = new Point(30, 164),
                Name = "btnCleanUp",
                Size = new Size(200, 28),
                Text = "Remove the hard-coded fill",
            };
            this.btnCleanUp.Click += this.btnCleanUp_Click;
            this.checkCards[2].Controls.Add(this.btnCleanUp);
            this.checkCards[2].Resize += (s, e) =>
            {
                this.btnCleanUp.Left = (this.checkCards[2].ClientSize.Width - this.btnCleanUp.Width) / 2;
            };

            this.pnlRecolour.Resize += (s, e) => LayOutCheckCards();

            LayOutCheckCards();
            ApplyCheckSources();
        }

        private Button btnCleanUp;

        private void LayOutCheckCards()
        {
            var inner = this.pnlRecolour.ClientSize.Width - 40;
            var cardWidth = (inner - 2 * 16) / 3;

            for (var i = 0; i < this.checkCards.Count; i++)
            {
                this.checkCards[i].Location = new Point(20 + i * (cardWidth + 16), 22);
                this.checkCards[i].Size = new Size(cardWidth, 170);
            }
        }

        /// <summary>
        /// Points the three cards at their sources. The Error card starts on the draft artwork,
        /// which fixes its own stroke colours and therefore ignores the suffix.
        /// </summary>
        private void ApplyCheckSources()
        {
            for (var i = 0; i < Checks.Length; i++)
            {
                var suffix = Checks[i][2].Substring("?color=".Length);
                var icon = i == 2 && !this.cleaned ? ErrorDraft : Checks[i][1];

                foreach (var picture in this.checkPictures[i])
                    picture.ImageSource = icon + "?color=" + suffix;

                this.checkNames[i].Text = i == 2 && !this.cleaned
                    ? "IconDesk/status-error-draft.svg"
                    : "AppIcons." + Checks[i][0];
            }
        }

        private void btnCleanUp_Click(object sender, EventArgs e)
        {
            this.cleaned = true;
            this.btnCleanUp.Visible = false;

            ApplyCheckSources();
            ApplyTheme();
            ReportRecolour();
        }

        private void ReportRecolour()
        {
            if (!this.cleaned)
            {
                SetStatus("bad", "status-error-draft.svg ignored the colour suffix — the SVG carries a hard-coded stroke colour");
                return;
            }

            SetStatus("good", this.theme == Dark
                ? "Three icons verified: theme colour, hex colour, light and dark theme"
                : "Hard-coded colours removed — AppIcons.Error now follows the requested colour");
        }

        // ── theme ───────────────────────────────────────────────────────────────

        private void btnTheme_Click(object sender, EventArgs e)
        {
            this.theme = this.theme == Light ? Dark : Light;
            Application.LoadTheme(this.theme.Name);

            ApplyTheme();
            ReportRecolour();
        }

        private void ApplyTheme()
        {
            var t = this.theme;

            this.layoutGrid.BackColor = t.Surface;
            this.pnlRecolour.BackColor = t.Surface;

            foreach (var tile in this.tiles)
            {
                tile.BackColor = t.Card;
                tile.CssStyle = "border:1px solid " + Css(t.Line) + ";border-radius:10px;";
            }

            foreach (var name in this.tileNames)
                name.ForeColor = t.Ink;

            this.btnTheme.BackColor = t.ChipBack;
            this.btnTheme.ForeColor = t.ChipInk;
            this.btnTheme.Text = t == Dark ? "Dark theme" : "Light theme";

            for (var i = 0; i < this.checkCards.Count; i++)
            {
                var bad = i == 2 && !this.cleaned;
                var colour = Checks[i][3];

                this.checkCards[i].BackColor = t.Card;
                this.checkCards[i].CssStyle = "border:1px solid " + (bad ? "#f2b8b0" : Css(t.Line)) + ";border-radius:12px;";
                this.checkNames[i].ForeColor = t.Ink;

                this.checkPills[i].ForeColor = ColorTranslator.FromHtml(bad ? "#b3261e" : colour);
                this.checkPills[i].BackColor = bad ? ColorTranslator.FromHtml("#fff3f1") : t.Card;
                this.checkPills[i].CssStyle = "border:1px solid " + (bad ? "#f2b8b0" : colour + "66") + ";border-radius:999px;";

                this.checkFaults[i].Visible = bad;
                this.checkFaults[i].Text = bad ? "stroke=\"#8a8f98\" in the artwork" : string.Empty;
            }
        }

        private static string Css(Color color)
        {
            return "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }

        // ── status ──────────────────────────────────────────────────────────────

        private void SetStatus(string kind, string text)
        {
            string back, line, ink, glyph;

            switch (kind)
            {
                case "bad":
                    back = "#fff3f1"; line = "#f2b8b0"; ink = "#b3261e"; glyph = "!";
                    break;
                case "good":
                    back = "#ecf8f1"; line = "#a9e0c4"; ink = "#16774d"; glyph = "✓";
                    break;
                default:
                    back = "#f6f8fb"; line = "#e3e9f0"; ink = "#5a6b7d"; glyph = "·";
                    break;
            }

            this.pnlStatus.BackColor = ColorTranslator.FromHtml(back);
            this.pnlStatus.CssStyle = "border-top:1px solid " + line + ";";
            this.lblStatusGlyph.ForeColor = ColorTranslator.FromHtml(ink);
            this.lblStatusGlyph.Text = glyph;
            this.lblPackStatus.ForeColor = ColorTranslator.FromHtml(ink);
            this.lblPackStatus.Text = text;
        }
    }
}
