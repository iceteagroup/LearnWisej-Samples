using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The Module 6 banner: a round ✓ or ! badge and one sentence, green or red, with a left accent
    /// bar. The sentence is always plain text (<c>AllowHtml</c> false) - it can contain a passkey
    /// label the user typed.
    /// </summary>
    internal sealed class Banner : Panel
    {
        private readonly Label _badge = new Label();
        private readonly Label _text = new Label();

        public Banner()
        {
            this.Name = "pnlBanner";
            this.Height = 46;
            this.Padding = new Padding(14, 0, 14, 0);
            this.Visible = false;

            _badge.Name = "lblBannerBadge";
            _badge.Dock = DockStyle.Left;
            _badge.Width = 34;
            _badge.Font = Theme.Px(13f, FontStyle.Bold);
            _badge.TextAlign = ContentAlignment.MiddleLeft;
            _badge.AllowHtml = true;   // fixed markup only (the round badge), never input

            _text.Name = "lblBannerText";
            _text.Dock = DockStyle.Fill;
            _text.Font = Theme.Px(13.5f);
            _text.TextAlign = ContentAlignment.MiddleLeft;
            _text.AllowHtml = false;

            this.Controls.Add(_text);
            this.Controls.Add(_badge);
        }

        /// <summary>The sentence on show.</summary>
        public string Message => this.Visible ? _text.Text : "";

        /// <summary>Shows <paramref name="text"/> (plain text) as a success or a refusal.</summary>
        public void Show(string text, bool ok)
        {
            var (wash, border, accent, ink) = ok
                ? ("#e9f7f0", "#a7ddc6", "#1f9d6b", "#14603f")
                : ("#fdecea", "#f0b4ad", "#c0392b", "#7d2018");
            this.BackColor = ColorTranslator.FromHtml(wash);
            this.CssStyle = $"border:1px solid {border}; border-left:4px solid {accent}; border-radius:9px;";
            _badge.Text = $"<span style=\"display:inline-flex;width:22px;height:22px;border-radius:999px;background:{accent};color:#fff;align-items:center;justify-content:center;font-size:13px;font-weight:800\">{(ok ? "✓" : "!")}</span>";
            _text.Text = text ?? "";
            _text.ForeColor = ColorTranslator.FromHtml(ink);
            this.Visible = true;
        }

        /// <summary>Hides the banner.</summary>
        public void Clear() => this.Visible = false;
    }
}
