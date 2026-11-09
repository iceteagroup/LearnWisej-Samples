using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The blue Wisej.NET application bar: "AccessOps · {screen}" on the left, the three window
    /// glyphs on the right and nothing else (no identity, session or tenant chips).
    /// </summary>
    internal sealed class AppBar : Panel
    {
        private readonly Label _title;

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
                AutoSize = false,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = Theme.Px(15f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var glyphs = new FlowLayoutPanel
            {
                Name = "pnlWindowGlyphs",
                Dock = DockStyle.Right,
                Width = 96,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 11, 0, 0)
            };
            foreach (var g in new[] { "—", "☐", "✕" })
            {
                glyphs.Controls.Add(new Label
                {
                    Text = g,
                    AutoSize = false,
                    Size = new Size(28, 18),
                    Margin = new Padding(0),
                    ForeColor = Color.White,
                    Font = Theme.Px(13f),
                    TextAlign = ContentAlignment.MiddleCenter
                });
            }

            // Dock order is the reverse of Controls.Add: the glyphs dock first (right), the title fills.
            this.Controls.Add(_title);
            this.Controls.Add(glyphs);
        }

        /// <summary>The screen name after "AccessOps · ".</summary>
        public string Screen
        {
            get => _title.Text;
            set => _title.Text = "AccessOps · " + value;
        }
    }
}
