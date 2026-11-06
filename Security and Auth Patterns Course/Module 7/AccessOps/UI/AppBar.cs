using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The blue Wisej.NET application bar: "AccessOps · {area}" on the left, the three window
    /// glyphs on the right. Decoration only; it reports nothing and decides nothing.
    /// </summary>
    internal sealed class AppBar : Panel
    {
        private readonly Label _title;
        private readonly Label _subtitle;

        public AppBar()
        {
            this.Name = "pnlAppBar";
            this.Dock = DockStyle.Top;
            this.Height = 40;
            this.BackColor = Theme.Accent;
            this.Padding = new Padding(18, 0, 10, 0);

            _title = new Label
            {
                Name = "lblAppTitle",
                Text = "AccessOps",
                AutoSize = true,
                Dock = DockStyle.Left,
                ForeColor = Color.White,
                Font = Theme.Px(14.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            _subtitle = new Label
            {
                Name = "lblAppArea",
                AutoSize = true,
                Dock = DockStyle.Left,
                ForeColor = ColorTranslator.FromHtml("#dbe7fa"),
                Font = Theme.Px(13f),
                Padding = new Padding(9, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var glyphs = new FlowLayoutPanel
            {
                Name = "pnlWindowGlyphs",
                Dock = DockStyle.Right,
                Width = 84,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 11, 0, 0)
            };
            foreach (var g in new[] { "—", "□", "✕" })
            {
                glyphs.Controls.Add(new Label
                {
                    Text = g,
                    AutoSize = false,
                    Size = new Size(26, 18),
                    ForeColor = Color.White,
                    Font = Theme.Px(13f),
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            // Dock order is the reverse of Controls.Add: the subtitle docks after the title.
            this.Controls.Add(glyphs);
            this.Controls.Add(_subtitle);
            this.Controls.Add(_title);
        }

        public string Area
        {
            get => _subtitle.Text;
            set => _subtitle.Text = "· " + value;
        }
    }
}
