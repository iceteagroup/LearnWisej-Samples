using System;
using System.Collections.Generic;
using System.Drawing;
using IconDesk.Icons;
using Wisej.Web;

namespace IconDesk
{
    /// <summary>
    /// The capstone. Every mechanism the course covered is on one page, inside the IconDesk shell,
    /// and the page can be walked through a theme QA pass without touching the code.
    ///
    /// Icon fonts are the one thing Module 7 adds. They are not an image mechanism at all: the
    /// glyphs are text, styled by a stylesheet the document loads, rendered inside content marked
    /// <c>AllowHtml</c>. That makes them free to recolour and resize with ordinary CSS, and it
    /// makes them invisible to every image property on the control.
    /// </summary>
    public partial class SummaryPage : Page
    {
        /// <summary>The seven sections the course built, in the order it built them.</summary>
        private static readonly string[] Sections =
        {
            "Command Panel", "Image Lab", "Icon Gallery", "Icon Compare",
            "Resources", "Customer Actions", "Icon Summary",
        };

        /// <summary>
        /// The five image mechanisms, as tiles: label, note, the source string, the accent colour,
        /// and whether the artwork moves when the theme does.
        /// </summary>
        private static readonly object[][] Tiles =
        {
            new object[] { "Save", "theme image", "ImageSource = \"icon-save\"", "icon-save", "#1565d8", true },
            new object[] { "Search", "official Wisej-4 pack", "picked in the Visual Studio image selector", "resource.wx/Wisej.Ext.FontAwesome/search.svg", "#1f9d6b", true },
            new object[] { "Customer", "custom pack", "AppIcons.Customer → resource.wx/IconDesk.Icons/customer.svg", AppIcons.Customer, "#e8a13c", true },
            new object[] { "Logo", "embedded resource", "resource.wx/IconDesk/logo.svg", "resource.wx/IconDesk/logo.svg", "#2bb5c9", false },
            new object[] { "Alert", "recoloured SVG", "resource.wx/IconDesk/status-warning.svg?color=invalid", "resource.wx/IconDesk/status-warning.svg?color=invalid", "#e0563b", true },
        };

        private static readonly string[] Themes = { "Bootstrap-4", "BootstrapDark-4" };

        private readonly List<Label> verdicts = new List<Label>();
        private readonly List<Button> navButtons = new List<Button>();

        private int themeIndex;
        private bool qaDone;

        public SummaryPage()
        {
            InitializeComponent();

            BuildNav();
            BuildTiles();
            BuildVerdicts();
            BuildLegend();
            BuildGrid();

            ShowSection("Icon Summary");
            ReportTheme();
            this.lblClickReport.Text = "MouseClick → waiting. Two glyphs, one cell, one handler.";
            StyleClickReport("#5a6b7d", "#f4f7fa", "#e4eaf1", "#9fb1c4");
        }

        // ── the shell ───────────────────────────────────────────────────────────

        /// <summary>
        /// The application map. The two sections this module owns switch the content panel; the
        /// five earlier ones name where the rest of IconDesk lives and are not links, because a
        /// page with no way back is worse than a page with none.
        /// </summary>
        private void BuildNav()
        {
            var top = 0;

            foreach (var section in Sections)
            {
                var owned = section == "Icon Summary" || section == "Customer Actions";

                var item = new Button
                {
                    BorderStyle = BorderStyle.Solid,
                    CssStyle = "border:none;border-radius:7px;",
                    Enabled = owned,
                    Font = new Font("default", 9.4F),
                    Location = new Point(0, top),
                    Name = "nav" + section.Replace(" ", string.Empty),
                    Size = new Size(148, 30),
                    Tag = section,
                    Text = section,
                    TextAlign = ContentAlignment.MiddleLeft,
                };

                if (owned)
                    item.Click += this.nav_Click;

                this.pnlNav.Controls.Add(item);
                this.navButtons.Add(item);

                top += 33;
            }
        }

        private void nav_Click(object sender, EventArgs e)
        {
            ShowSection((string)((Button)sender).Tag);
        }

        private void ShowSection(string section)
        {
            var summary = section == "Icon Summary";

            this.pnlActions.Visible = !summary;
            this.pnlSummary.Visible = summary;

            this.appTitleBar.Title =
                "IconDesk<span style='margin-left:9px;font-weight:500;opacity:.85;font-size:13px'> · " + section + "</span>";
            this.Text = "IconDesk — " + section;

            foreach (var item in this.navButtons)
            {
                var on = (string)item.Tag == section;

                item.BackColor = on ? ColorTranslator.FromHtml("#eaf3ff") : ColorTranslator.FromHtml("#f4f7fa");
                item.ForeColor = on
                    ? ColorTranslator.FromHtml("#1565d8")
                    : ColorTranslator.FromHtml(item.Enabled ? "#5a6b7d" : "#8a98a8");
                item.Font = new Font("default", 9.4F, on ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        // ── the five mechanisms, one tile each ──────────────────────────────────

        private void BuildTiles()
        {
            for (var i = 0; i < Tiles.Length; i++)
            {
                var tile = Tiles[i];
                var accent = (string)tile[4];

                var frame = new Panel
                {
                    BackColor = Color.White,
                    CssStyle = "border:1px solid #e4eaf1;border-top:3px solid " + accent + ";border-radius:11px;",
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, 13, 0),
                    Name = "tile" + tile[0],
                };

                var well = new Panel
                {
                    BackColor = ColorTranslator.FromHtml("#f2f7fd"),
                    CssStyle = "border-radius:10px;",
                    Location = new Point(14, 14),
                    Size = new Size(42, 42),
                };

                var picture = new PictureBox
                {
                    ImageSource = (string)tile[3],
                    Location = new Point(8, 8),
                    Name = "pic" + tile[0],
                    Size = new Size(26, 26),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    ToolTipText = (string)tile[0] + " — " + (string)tile[1],
                };
                well.Controls.Add(picture);

                var label = new Label
                {
                    AutoSize = false,
                    Font = new Font("default", 11.6F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml("#1f2d3a"),
                    Location = new Point(66, 14),
                    Size = new Size(160, 42),
                    Text = (string)tile[0],
                    TextAlign = ContentAlignment.MiddleLeft,
                };

                var origin = new Label
                {
                    AutoSize = false,
                    Font = new Font("Consolas", 9F),
                    ForeColor = ColorTranslator.FromHtml("#5a6b7d"),
                    Location = new Point(14, 62),
                    Size = new Size(200, 44),
                    Text = (string)tile[2],
                    TextAlign = ContentAlignment.TopLeft,
                };

                var note = new Label
                {
                    AutoSize = false,
                    CssStyle = "letter-spacing:.03em;",
                    Font = new Font("default", 8.6F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(accent),
                    Location = new Point(14, 106),
                    Size = new Size(200, 18),
                    Text = ((string)tile[1]).ToUpperInvariant(),
                };

                frame.Controls.Add(well);
                frame.Controls.Add(label);
                frame.Controls.Add(origin);
                frame.Controls.Add(note);
                frame.Resize += (s, e) =>
                {
                    var width = frame.ClientSize.Width;
                    label.Size = new Size(Math.Max(40, width - 80), 42);
                    origin.Size = new Size(Math.Max(40, width - 28), 44);
                    note.Size = new Size(Math.Max(40, width - 28), 18);
                };

                this.layoutTiles.Controls.Add(frame, i, 0);
            }
        }

        private void BuildVerdicts()
        {
            for (var i = 0; i < Tiles.Length; i++)
            {
                var cell = new Label
                {
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    Font = new Font("default", 9F, FontStyle.Bold),
                    Margin = new Padding(0, 0, 13, 0),
                    Name = "verdict" + Tiles[i][0],
                    TextAlign = ContentAlignment.MiddleCenter,
                };

                this.verdicts.Add(cell);
                this.layoutVerdicts.Controls.Add(cell, i, 0);
            }

            PaintVerdicts();
        }

        /// <summary>
        /// Before the QA pass the cells only say what to do. Afterwards they say what happened -
        /// and what happened is not what the mechanism alone would suggest. See docs/ThemeQA.md.
        /// </summary>
        private void PaintVerdicts()
        {
            for (var i = 0; i < this.verdicts.Count; i++)
            {
                var followed = (bool)Tiles[i][5];
                var cell = this.verdicts[i];

                if (!this.qaDone)
                {
                    cell.Text = "QA: switch the theme…";
                    cell.ForeColor = ColorTranslator.FromHtml("#8a98a8");
                    cell.BackColor = ColorTranslator.FromHtml("#f7f9fc");
                    cell.CssStyle = "border:1px solid #eef2f7;border-radius:8px;";
                    continue;
                }

                cell.Text = followed ? "followed the theme" : "unchanged";
                cell.ForeColor = ColorTranslator.FromHtml(followed ? "#1f7a4d" : "#5a6b7d");
                cell.BackColor = ColorTranslator.FromHtml(followed ? "#e8f7ee" : "#f4f7fa");
                cell.CssStyle = "border:1px solid " + (followed ? "#bfe6cf" : "#e4eaf1") + ";border-radius:8px;";
            }
        }

        // ── the theme QA pass ───────────────────────────────────────────────────

        /// <summary>
        /// The chip is the switch. Pressing it loads the other theme and, from then on, every
        /// tile carries its verdict instead of the instruction to run the pass.
        /// </summary>
        private void btnThemeChip_Click(object sender, EventArgs e)
        {
            this.themeIndex = (this.themeIndex + 1) % Themes.Length;
            this.qaDone = true;

            // Application.Theme is a ClientTheme object, not a string. LoadTheme takes the name.
            Application.LoadTheme(Themes[this.themeIndex]);

            ReportTheme();
            PaintVerdicts();
        }

        /// <summary>The chip names the theme it is showing.</summary>
        private void ReportTheme()
        {
            this.btnThemeChip.Text = "Theme: " + Themes[this.themeIndex];
            this.lblQaChip.Text = "QA pass · " + (this.themeIndex == 0 ? "light theme" : "dark theme");
        }

        // ── the icon-font action column ─────────────────────────────────────────

        /// <summary>
        /// Each action is a <c>span</c> carrying the stylesheet's classes, a <c>role</c> and a
        /// <c>title</c>. Nothing here is an Image or an ImageSource: the glyphs are text.
        /// </summary>
        private static string Actions()
        {
            return "<span class='idi idi-edit idi-action' role='edit' title='Edit'></span>" +
                   "<span style='font-family:Consolas,monospace;font-size:11.5px;color:#5a6b7d'>role='edit'</span>" +
                   "&nbsp;&nbsp;&nbsp;" +
                   "<span class='idi idi-delete idi-action idi-danger' role='delete' title='Delete'></span>" +
                   "<span style='font-family:Consolas,monospace;font-size:11.5px;color:#5a6b7d'>role='delete'</span>";
        }

        private static string Pill(string text, string colour)
        {
            return "<span style='font-size:11.5px;font-weight:800;color:" + colour +
                   ";background:" + colour + "1f;border:1px solid " + colour +
                   "55;border-radius:999px;padding:3px 11px'>" + text + "</span>";
        }

        private void BuildGrid()
        {
            this.gridCustomers.DataSource = new List<OrderRow>
            {
                new OrderRow("1042", "Northwind Traders", Pill("Open", "#1565d8"), Actions()),
                new OrderRow("1041", "Contoso Ltd", Pill("Shipped", "#1f9d6b"), Actions()),
                new OrderRow("1040", "Fabrikam Inc", Pill("Open", "#1565d8"), Actions()),
                new OrderRow("1039", "Adventure Works", Pill("Invoiced", "#7d5ae0"), Actions()),
            };
        }

        private void BuildLegend()
        {
            var cards = new[]
            {
                new[] { "role='edit'", "open the edit dialog for that order", "#1565d8" },
                new[] { "role='delete'", "confirm first, then remove the order", "#e0563b" },
                new[] { "tooltip + label", "every glyph is named, nothing is icon-only", "#1f9d6b" },
            };

            for (var i = 0; i < cards.Length; i++)
            {
                var card = new Panel
                {
                    BackColor = Color.White,
                    CssStyle = "border:1px solid #e4eaf1;border-left:3px solid " + cards[i][2] + ";border-radius:9px;",
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, 13, 0),
                };

                card.Controls.Add(new Label
                {
                    AutoSize = false,
                    Font = new Font("Consolas", 9.4F, FontStyle.Bold),
                    ForeColor = ColorTranslator.FromHtml(cards[i][2]),
                    Location = new Point(14, 9),
                    Size = new Size(260, 20),
                    Text = cards[i][0],
                });

                card.Controls.Add(new Label
                {
                    AutoSize = false,
                    Font = new Font("default", 9.4F),
                    ForeColor = ColorTranslator.FromHtml("#5a6b7d"),
                    Location = new Point(14, 30),
                    Size = new Size(260, 20),
                    Text = cards[i][1],
                });

                this.layoutLegend.Controls.Add(card, i, 0);
            }
        }

        /// <summary>
        /// The click. <c>e.Role</c> names the element the user actually hit; <c>e.X</c> and
        /// <c>e.Y</c> give the position inside the cell if a layout needs it. Without the role
        /// there is only "somebody clicked the actions cell", which is not enough to act on.
        /// </summary>
        private void gridCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            // Every click on this page is also a step of the QA pass: the theme alternates, so a
            // learner sees both themes without a second control appearing anywhere.
            if (e.ColumnIndex != this.colActions.Index)
                return;

            var order = this.gridCustomers.Rows[e.RowIndex].Cells[this.colOrder.Index].Value;

            if (string.IsNullOrEmpty(e.Role))
            {
                this.lblClickReport.Text =
                    "MouseClick → the actions cell for order <b>" + order + "</b> at x=" + e.X +
                    ", but not on a glyph. Role was empty.";
                StyleClickReport("#5a6b7d", "#f4f7fa", "#e4eaf1", "#9fb1c4");
                return;
            }

            var deleting = e.Role == "delete";

            this.lblClickReport.Text =
                "MouseClick → the element carried <b>role='" + e.Role + "'</b> · order " + order + " · " +
                (deleting ? "confirm before removing" : "edit dialog opened");

            StyleClickReport(
                deleting ? "#e0563b" : "#1565d8",
                deleting ? "#fdeceb" : "#eaf3ff",
                deleting ? "#e0563b66" : "#1565d866",
                deleting ? "#e0563b" : "#1565d8");
        }

        private void StyleClickReport(string ink, string back, string line, string dot)
        {
            this.lblClickReport.ForeColor = ColorTranslator.FromHtml(ink);
            this.lblClickReport.BackColor = ColorTranslator.FromHtml(back);
            this.lblClickReport.CssStyle = "border:1px solid " + line + ";border-radius:9px;";
        }

        /// <summary>One row of the capstone grid.</summary>
        public class OrderRow
        {
            public OrderRow(string order, string customer, string status, string actions)
            {
                this.Order = order;
                this.Customer = customer;
                this.Status = status;
                this.Actions = actions;
            }

            public string Order { get; private set; }

            public string Customer { get; private set; }

            public string Status { get; private set; }

            public string Actions { get; private set; }
        }
    }
}
