using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The AccessOps look: the blue Wisej.NET app bar, the light strips and the status tones the
    /// walkthrough mock uses. Colours and fonts only - no behaviour lives here.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Accent = ColorTranslator.FromHtml("#1565d8");
        public static readonly Color AccentWash = ColorTranslator.FromHtml("#eaf3ff");
        public static readonly Color Strip = ColorTranslator.FromHtml("#f8fafc");
        public static readonly Color HeaderBack = ColorTranslator.FromHtml("#f4f7fa");
        public static readonly Color Line = ColorTranslator.FromHtml("#e4eaf1");
        public static readonly Color Ink = ColorTranslator.FromHtml("#0d1b2a");
        public static readonly Color Text = ColorTranslator.FromHtml("#1f2d3a");
        public static readonly Color Muted = ColorTranslator.FromHtml("#5a6b7d");
        public static readonly Color Faint = ColorTranslator.FromHtml("#7d8ea0");
        public static readonly Color AltRow = ColorTranslator.FromHtml("#fbfcfe");
        public static readonly Color Ok = ColorTranslator.FromHtml("#1f9d6b");
        public static readonly Color Bad = ColorTranslator.FromHtml("#c0392b");
        public static readonly Color Warn = ColorTranslator.FromHtml("#b9770e");
        public static readonly Color BadWash = ColorTranslator.FromHtml("#fdecea");
        public static readonly Color BadInk = ColorTranslator.FromHtml("#7d2018");
        public static readonly Color OkWash = ColorTranslator.FromHtml("#eaf7f0");
        public static readonly Color OkInk = ColorTranslator.FromHtml("#155e40");
        public static readonly Color FieldBorder = ColorTranslator.FromHtml("#cdd9e6");
        public static readonly Color Disabled = ColorTranslator.FromHtml("#a8b4c0");

        public static Font Px(float size, FontStyle style = FontStyle.Regular)
            => new Font("Segoe UI", size, style, GraphicsUnit.Pixel);

        public static Font Mono(float size, FontStyle style = FontStyle.Regular)
            => new Font("Consolas", size, style, GraphicsUnit.Pixel);

        public static Color Tone(StatusTone tone) => tone switch
        {
            StatusTone.Ok => Ok,
            StatusTone.Bad => Bad,
            StatusTone.Warn => Warn,
            _ => Muted
        };

        /// <summary>Primary (blue) or secondary (outlined) button in the mock's style.</summary>
        public static void StyleButton(Button button, bool primary)
        {
            button.Font = Px(12.5f, FontStyle.Bold);
            button.BackColor = primary ? Accent : Color.White;
            button.ForeColor = primary ? Color.White : ColorTranslator.FromHtml("#3a4d63");
            button.CssStyle = primary
                ? "border-radius:7px; border:none;"
                : "border-radius:7px; border:1px solid #cdd9e6;";
        }

        /// <summary>The caption label used above input fields ("USER NAME").</summary>
        public static Label Caption(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 20,
                Font = Px(11.5f, FontStyle.Bold),
                ForeColor = Muted
            };
        }
    }

    internal enum StatusTone { Idle, Ok, Bad, Warn }
}
