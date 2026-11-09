using System.Drawing;
using Wisej.Web;

namespace AccessOps.UI
{
    /// <summary>
    /// The AccessOps look taken from the walkthrough mocks (Modules 1-7): the blue app bar, the
    /// light area strip, the grid, the banner and the status tones. Colours and fonts only - no behaviour here.
    /// </summary>
    internal static class Theme
    {
        public static readonly Color Accent = ColorTranslator.FromHtml("#1565d8");
        public static readonly Color AccentWash = ColorTranslator.FromHtml("#eaf3ff");
        public static readonly Color Strip = ColorTranslator.FromHtml("#f4f7fa");
        public static readonly Color Line = ColorTranslator.FromHtml("#e4eaf1");
        public static readonly Color Ink = ColorTranslator.FromHtml("#0d1b2a");
        public static readonly Color Text = ColorTranslator.FromHtml("#1f2d3a");
        public static readonly Color Muted = ColorTranslator.FromHtml("#5a6b7d");
        public static readonly Color Body = ColorTranslator.FromHtml("#46586a");
        public static readonly Color Faint = ColorTranslator.FromHtml("#6a7b8d");
        public static readonly Color Deny = ColorTranslator.FromHtml("#b42318");
        public static readonly Color Ok = ColorTranslator.FromHtml("#1f9d6b");
        public static readonly Color Warn = ColorTranslator.FromHtml("#b26a00");

        /// <summary>Text colour of a status-line tone (the intranet route's status line).</summary>
        public static Color ToneInk(StatusTone tone) => tone switch
        {
            StatusTone.Ok => Ok,
            StatusTone.Bad => ColorTranslator.FromHtml("#c62828"),
            StatusTone.Warn => Warn,
            _ => Muted
        };

        /// <summary>Background of a status-line tone.</summary>
        public static Color ToneWash(StatusTone tone) => tone switch
        {
            StatusTone.Ok => ColorTranslator.FromHtml("#e8f6ef"),
            StatusTone.Bad => ColorTranslator.FromHtml("#fdecec"),
            StatusTone.Warn => ColorTranslator.FromHtml("#fff4e0"),
            _ => Strip
        };

        public static Font Px(float size, FontStyle style = FontStyle.Regular)
            => new Font("Segoe UI", size, style, GraphicsUnit.Pixel);

        public static Font Mono(float size, FontStyle style = FontStyle.Regular)
            => new Font("Consolas", size, style, GraphicsUnit.Pixel);

        /// <summary>Primary (blue) or secondary (outlined) button in the mock's style.</summary>
        public static void StyleButton(Button button, bool primary, float size = 14.5f)
        {
            button.Font = Px(size, FontStyle.Bold);
            button.BackColor = primary ? Accent : Color.White;
            button.ForeColor = primary ? Color.White : ColorTranslator.FromHtml("#3a4d63");
            button.CssStyle = primary
                ? "border-radius:7px; border:none;"
                : "border-radius:7px; border:1px solid #cdd9e6;";
        }

        /// <summary>The small bold field label ("User name", "Password").</summary>
        public static Label FieldLabel(string text) => new Label
        {
            Text = text,
            AutoSize = false,
            Height = 20,
            Font = Px(12.5f, FontStyle.Bold),
            ForeColor = Body,
            TextAlign = ContentAlignment.BottomLeft
        };

        /// <summary>The mock's 38 px boxed input.</summary>
        public static void StyleInput(TextBox box)
        {
            box.Height = 38;
            box.Font = Px(14.5f);
            box.ForeColor = Text;
            box.CssStyle = "border:1px solid #c7d3e0; border-radius:7px;";
        }

        /// <summary>
        /// Shows <paramref name="text"/> in a banner label in one of the mock's tones. The text is
        /// always plain text (<c>AllowHtml</c> stays false), so a value from the request cannot
        /// become markup.
        /// </summary>
        public static void ShowBanner(Label banner, string text, BannerTone tone)
        {
            if (string.IsNullOrEmpty(text))
            {
                banner.Visible = false;
                return;
            }

            var (ink, wash, border) = tone switch
            {
                BannerTone.Bad => ("#b42318", "#fdecea", "#f5c2bd"),
                BannerTone.Ok => ("#16723f", "#e7f6ee", "#b7e2c8"),
                _ => ("#1565d8", "#eaf3ff", "#c5ddff")
            };
            banner.AllowHtml = false;
            banner.Text = text;
            banner.ForeColor = ColorTranslator.FromHtml(ink);
            banner.BackColor = ColorTranslator.FromHtml(wash);
            banner.CssStyle = $"border:1px solid {border}; border-radius:7px;";
            banner.Visible = true;
        }
    }

    /// <summary>The banner tones of the Module 4 mock.</summary>
    internal enum BannerTone { Bad, Ok, Info }
}
