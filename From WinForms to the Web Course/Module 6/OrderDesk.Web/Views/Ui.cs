using System.Drawing;
using Wisej.Web;

namespace OrderDesk.Views
{
    /// <summary>Colours and small helpers shared by the screens.</summary>
    public static class Ui
    {
        public static readonly Color Muted = Color.FromArgb(90, 107, 125);
        public static readonly Color Ok = Color.FromArgb(31, 157, 87);
        public static readonly Color Warn = Color.FromArgb(232, 161, 60);
        public static readonly Color Error = Color.FromArgb(224, 86, 59);
        public static readonly Color Accent = Color.FromArgb(26, 134, 255);

        /// <summary>A non-blocking notification (AlertBox) instead of an informational MessageBox.</summary>
        public static void Toast(string text, MessageBoxIcon icon = MessageBoxIcon.Information, int delay = 4000)
        {
            AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: delay);
        }
    }
}
