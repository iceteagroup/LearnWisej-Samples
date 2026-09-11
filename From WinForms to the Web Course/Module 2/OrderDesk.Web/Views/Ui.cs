using System.Drawing;
using Wisej.Web;

namespace OrderDesk.Views
{
    /// <summary>Small UI helpers shared by the screens.</summary>
    public static class Ui
    {
        /// <summary>A non-blocking notification (AlertBox) instead of an informational MessageBox.</summary>
        public static void Toast(string text, MessageBoxIcon icon = MessageBoxIcon.Information, int delay = 4000)
        {
            AlertBox.Show(text, icon, alignment: ContentAlignment.TopRight, autoCloseDelay: delay);
        }
    }
}
