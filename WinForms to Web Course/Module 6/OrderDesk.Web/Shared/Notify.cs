using System.Drawing;
using Wisej.Web;

namespace OrderDesk.Shared
{
    /// <summary>
    /// Notification policy for the migrated app (Module 3): informational messages are non-blocking
    /// (Toast / AlertBox); decisions and validation failures may still block (MessageBox).
    /// </summary>
    public static class Notify
    {
        /// <summary>"Saved." — no decision needed, so it must not block: a Toast.</summary>
        public static void Saved(string text)
        {
            var toast = new Toast(text, "icon-ok") { AutoCloseDelay = 3500, Alignment = ContentAlignment.BottomRight };
            toast.Show();
        }

        public static void Info(string text)
            => AlertBox.Show(text, MessageBoxIcon.Information, alignment: ContentAlignment.TopRight, autoCloseDelay: 4000);

        public static void Warning(string text)
            => AlertBox.Show(text, MessageBoxIcon.Warning, alignment: ContentAlignment.TopRight, autoCloseDelay: 5000);

        public static void Error(string text)
            => AlertBox.Show(text, MessageBoxIcon.Error, alignment: ContentAlignment.TopRight, autoCloseDelay: 6000);

        /// <summary>A blocking decision: keep it modal.</summary>
        public static DialogResult Confirm(string text, string caption = "Confirm")
            => MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    }
}
