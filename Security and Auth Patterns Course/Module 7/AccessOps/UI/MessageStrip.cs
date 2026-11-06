using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The one-line notice under a form or above a grid: a coloured left border, a round badge
    /// and one sentence. Text is plain - AllowHtml stays off, so a user name typed into the
    /// login box is shown as characters, never as markup.
    /// </summary>
    internal sealed class MessageStrip : Panel
    {
        private readonly Label _badge;
        private readonly Label _text;

        public MessageStrip()
        {
            this.Name = "pnlMessage";
            this.Height = 46;
            this.Dock = DockStyle.Top;
            this.Padding = new Padding(14, 11, 14, 11);
            this.Visible = false;

            _badge = new Label
            {
                Name = "lblMessageBadge",
                AutoSize = false,
                Size = new Size(22, 22),
                Dock = DockStyle.Left,
                ForeColor = Color.White,
                Font = Theme.Px(13f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                CssStyle = "border-radius:999px;"
            };
            _text = new Label
            {
                Name = "lblMessage",
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = Theme.Px(13f),
                Padding = new Padding(12, 0, 0, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };

            this.Controls.Add(_text);
            this.Controls.Add(_badge);
        }

        public string Message => this.Visible ? _text.Text : null;

        public void Show(string text, StatusTone tone)
        {
            var border = tone == StatusTone.Ok ? Theme.Ok : tone == StatusTone.Warn ? Theme.Warn : Theme.Bad;
            this.BackColor = tone == StatusTone.Ok ? Theme.OkWash : tone == StatusTone.Warn ? ColorTranslator.FromHtml("#fdf4e6") : Theme.BadWash;
            this.CssStyle = $"border:1px solid {ColorTranslator.ToHtml(border)}55; border-left:4px solid {ColorTranslator.ToHtml(border)}; border-radius:9px;";
            _badge.BackColor = border;
            _badge.Text = tone == StatusTone.Ok ? "✓" : "!";
            _text.ForeColor = tone == StatusTone.Ok ? Theme.OkInk : tone == StatusTone.Warn ? ColorTranslator.FromHtml("#7a4d05") : Theme.BadInk;
            _text.Text = text;
            this.Visible = true;
        }

        public void Clear()
        {
            _text.Text = "";
            this.Visible = false;
        }
    }
}
