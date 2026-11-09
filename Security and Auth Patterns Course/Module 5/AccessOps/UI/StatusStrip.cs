using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>The tones of the Module 5 status strip.</summary>
    internal enum StatusTone { Idle, Ok, Bad }

    /// <summary>
    /// The bottom strip of the Module 5 AccessOps window: "STATUS", the server's outcome in one
    /// sentence and a monospace note on the right ("partner route · https", "grant G-xxxx…").
    /// Plain text only (<c>AllowHtml</c> stays false).
    /// </summary>
    internal sealed class StatusStrip : Panel
    {
        private readonly Label _status = new Label();
        private readonly Label _note = new Label();

        public StatusStrip()
        {
            this.Name = "pnlStatus";
            this.Dock = DockStyle.Bottom;
            this.Height = 40;
            this.Padding = new Padding(16, 0, 16, 0);
            this.BackColor = ColorTranslator.FromHtml("#f8fafc");
            this.CssStyle = "border-top:1px solid #e4eaf1;";

            var tag = new Label
            {
                Name = "lblStatusTag",
                Text = "STATUS",
                Dock = DockStyle.Left,
                Width = 66,
                Font = Theme.Px(11.5f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleLeft,
                CssStyle = "letter-spacing:.04em;"
            };
            _status.Name = "lblStatus";
            _status.Dock = DockStyle.Fill;
            _status.Font = Theme.Px(13f, FontStyle.Bold);
            _status.TextAlign = ContentAlignment.MiddleLeft;
            _note.Name = "lblStatusNote";
            _note.Dock = DockStyle.Right;
            _note.Width = 420;
            _note.Font = Theme.Mono(11.5f);
            _note.ForeColor = ColorTranslator.FromHtml("#7d8ea0");
            _note.TextAlign = ContentAlignment.MiddleRight;

            this.Controls.Add(_status);
            this.Controls.Add(_note);
            this.Controls.Add(tag);
            Show("Ready", StatusTone.Idle, "");
        }

        /// <summary>The status text on show.</summary>
        public string StatusText => _status.Text;

        /// <summary>Shows one status sentence and the note.</summary>
        public void Show(string status, StatusTone tone, string note)
        {
            _status.Text = status ?? "";
            _status.ForeColor = tone switch
            {
                StatusTone.Ok => ColorTranslator.FromHtml("#1f9d6b"),
                StatusTone.Bad => ColorTranslator.FromHtml("#c0392b"),
                _ => Theme.Muted
            };
            _note.Text = note ?? "";
        }
    }
}
