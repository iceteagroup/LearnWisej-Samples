using System.Drawing;
using Wisej.Web;

namespace OrderDesk.Views
{
    /// <summary>Colours, fonts and small helpers shared by every screen in the course samples.</summary>
    public static class Ui
    {
        public static readonly Color PageBack = Color.FromArgb(238, 242, 247);
        public static readonly Color CardBack = Color.White;
        public static readonly Color Muted = Color.FromArgb(90, 107, 125);
        public static readonly Color Ok = Color.FromArgb(31, 157, 87);
        public static readonly Color Warn = Color.FromArgb(232, 161, 60);
        public static readonly Color Error = Color.FromArgb(224, 86, 59);
        public static readonly Color Accent = Color.FromArgb(26, 134, 255);
        public static readonly Color Purple = Color.FromArgb(125, 90, 224);
        public static readonly Color BannerErrorBack = Color.FromArgb(253, 236, 234);
        public static readonly Color BannerErrorFore = Color.FromArgb(178, 59, 39);
        public static readonly Color BannerWarnBack = Color.FromArgb(255, 244, 229);
        public static readonly Color BannerWarnFore = Color.FromArgb(146, 64, 14);
        public static readonly Color BannerOkBack = Color.FromArgb(230, 247, 237);
        public static readonly Color BannerOkFore = Color.FromArgb(22, 101, 52);

        public static Font Title => new Font("default", 14F, FontStyle.Bold);
        public static Font CardTitle => new Font("default", 12F, FontStyle.Bold);
        public static Font Bold => new Font("default", 10F, FontStyle.Bold);
        public static Font Small => new Font("default", 9F);
        public static Font SmallBold => new Font("default", 9F, FontStyle.Bold);
        public static Font Mono => new Font("monospace", 9F);
        public static Font MonoBold => new Font("monospace", 9F, FontStyle.Bold);

        /// <summary>A non-blocking confirmation — the Module 3 replacement for an informational MessageBox.</summary>
        public static void Toast(string text, MessageBoxIcon icon = MessageBoxIcon.Information, int delay = 4000)
        {
            AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: delay);
        }

        public static void SetStatus(Label label, string text, Color color)
        {
            label.Text = "● " + text;
            label.ForeColor = color;
        }

        public enum BannerKind { Ok, Warn, Error }

        public static void ShowBanner(Label banner, string text, BannerKind kind)
        {
            banner.Text = text;
            switch (kind)
            {
                case BannerKind.Ok: banner.BackColor = BannerOkBack; banner.ForeColor = BannerOkFore; break;
                case BannerKind.Warn: banner.BackColor = BannerWarnBack; banner.ForeColor = BannerWarnFore; break;
                default: banner.BackColor = BannerErrorBack; banner.ForeColor = BannerErrorFore; break;
            }
            banner.Visible = true;
        }

        public static void HideBanner(Label banner) => banner.Visible = false;
    }
}
