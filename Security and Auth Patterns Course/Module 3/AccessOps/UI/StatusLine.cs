using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The one-line status at the bottom of a screen, in one of the four mock tones. It reports
    /// what the server decided for the last action; it never decides anything itself.
    /// </summary>
    internal sealed class StatusLine : Label
    {
        public StatusLine()
        {
            this.Name = "lblStatus";
            this.Dock = DockStyle.Bottom;
            this.Height = 38;
            this.AutoSize = false;
            this.Padding = new Padding(16, 0, 16, 0);
            this.Font = Theme.Px(13f, FontStyle.Bold);
            this.TextAlign = ContentAlignment.MiddleLeft;
            this.CssStyle = "border-top:1px solid #e0e7ef;";
            Show("Ready", StatusTone.Idle);
        }

        /// <summary>The tone of the text on show.</summary>
        public StatusTone Tone { get; private set; }

        /// <summary>Shows one sentence as plain text (Label.AllowHtml stays false).</summary>
        public void Show(string text, StatusTone tone)
        {
            this.Text = text;
            this.Tone = tone;
            this.ForeColor = Theme.ToneInk(tone);
            this.BackColor = Theme.ToneWash(tone);
        }
    }
}
