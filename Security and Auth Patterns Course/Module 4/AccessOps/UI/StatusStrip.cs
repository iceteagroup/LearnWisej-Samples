using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The strip at the bottom of every AccessOps screen: a caption ("SERVER SAID"), what the
    /// server actually did for the last click, and a note on the right (row counts, what is on
    /// the wire). It reports results; it never decides anything.
    /// </summary>
    internal sealed class StatusStrip : Panel
    {
        private readonly Label _caption;
        private readonly Label _status;
        private readonly Label _note;

        public StatusStrip(string caption)
        {
            this.Name = "pnlStatus";
            this.Dock = DockStyle.Bottom;
            this.Height = 42;
            this.BackColor = Theme.Strip;
            this.Padding = new Padding(16, 0, 16, 0);
            this.CssStyle = "border-top:1px solid #e4eaf1;";

            _caption = new Label
            {
                Name = "lblStatusCaption",
                Text = caption,
                AutoSize = true,
                Dock = DockStyle.Left,
                Font = Theme.Px(11.5f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _status = new Label
            {
                Name = "lblServerSaid",
                AutoSize = false,
                Dock = DockStyle.Fill,
                Padding = new Padding(22, 0, 0, 0),
                Font = Theme.Px(13f, FontStyle.Bold),
                ForeColor = Theme.Muted,
                TextAlign = ContentAlignment.MiddleLeft
            };
            _note = new Label
            {
                Name = "lblStatusNote",
                AutoSize = true,
                Dock = DockStyle.Right,
                Font = Theme.Mono(11.5f),
                ForeColor = Theme.Faint,
                TextAlign = ContentAlignment.MiddleRight
            };

            this.Controls.Add(_status);
            this.Controls.Add(_note);
            this.Controls.Add(_caption);
        }

        public string Status => _status.Text;

        public void Report(string status, StatusTone tone, string note = null)
        {
            _status.Text = status;
            _status.ForeColor = Theme.Tone(tone);
            if (note != null)
                _note.Text = note;
        }

        public string Note
        {
            get => _note.Text;
            set => _note.Text = value;
        }
    }
}
