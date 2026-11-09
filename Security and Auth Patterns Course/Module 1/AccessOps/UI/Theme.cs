using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The AccessOps look taken from the Module 1 walkthrough mock: the blue app bar, the light
    /// area strip, the grid and the four status-line tones. Colours and fonts only - no
    /// behaviour lives here.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Accent = ColorTranslator.FromHtml("#1565d8");
        public static readonly Color AccentWash = ColorTranslator.FromHtml("#eaf3ff");
        public static readonly Color Strip = ColorTranslator.FromHtml("#f4f7fa");
        public static readonly Color Line = ColorTranslator.FromHtml("#e0e7ef");
        public static readonly Color Text = ColorTranslator.FromHtml("#1f2d3a");
        public static readonly Color Muted = ColorTranslator.FromHtml("#5a6b7d");
        public static readonly Color Body = ColorTranslator.FromHtml("#46586a");
        public static readonly Color Ok = ColorTranslator.FromHtml("#1f9d6b");
        public static readonly Color Deny = ColorTranslator.FromHtml("#c62828");
        public static readonly Color Warn = ColorTranslator.FromHtml("#b26a00");

        public static Font Px(float size, FontStyle style = FontStyle.Regular)
            => new Font("Segoe UI", size, style, GraphicsUnit.Pixel);

        public static Font Mono(float size, FontStyle style = FontStyle.Regular)
            => new Font("Consolas", size, style, GraphicsUnit.Pixel);

        /// <summary>Text colour of a status-line tone.</summary>
        public static Color ToneInk(StatusTone tone) => tone switch
        {
            StatusTone.Ok => Ok,
            StatusTone.Deny => Deny,
            StatusTone.Warn => Warn,
            _ => Muted
        };

        /// <summary>Background of a status-line tone.</summary>
        public static Color ToneWash(StatusTone tone) => tone switch
        {
            StatusTone.Ok => ColorTranslator.FromHtml("#e8f6ef"),
            StatusTone.Deny => ColorTranslator.FromHtml("#fdecec"),
            StatusTone.Warn => ColorTranslator.FromHtml("#fff4e0"),
            _ => Strip
        };

        /// <summary>Primary (blue) or secondary (outlined) button in the mock's style.</summary>
        public static void StyleButton(Button button, bool primary)
        {
            button.Font = Px(12.5f, FontStyle.Bold);
            button.BackColor = primary ? Accent : Color.White;
            button.ForeColor = primary ? Color.White : ColorTranslator.FromHtml("#3a4d63");
            button.Height = 30;
            button.CssStyle = primary
                ? "border-radius:7px; border:none;"
                : "border-radius:7px; border:1px solid #cdd9e6;";
        }

        /// <summary>The small bold field label ("Tenant", "Record").</summary>
        public static Label FieldLabel(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            Font = Px(12f, FontStyle.Bold),
            ForeColor = Muted,
            TextAlign = ContentAlignment.MiddleLeft
        };

        /// <summary>The mock's boxed monospace input.</summary>
        public static void StyleInput(TextBox box, int width)
        {
            box.Width = width;
            box.Height = 28;
            box.Font = Mono(13f);
            box.ForeColor = Text;
            box.CssStyle = "border:1px solid #b8c6d6; border-radius:5px;";
        }
    }

    /// <summary>The four tones of the status line: idle, ok, deny and warn.</summary>
    internal enum StatusTone { Idle, Ok, Deny, Warn }
}
