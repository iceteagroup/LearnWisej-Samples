using System.Drawing;
using Wisej.Web;

namespace TicketOps.Controls
{
    public enum StatusKind
    {
        Normal,
        Busy,
        Success,
        Warning,
        Error
    }

    /// <summary>
    /// Reusable status strip: a "● state" label on the right and a banner line that appears for
    /// warnings/errors and disappears on recovery. Display only — it never decides anything.
    /// </summary>
    public partial class StatusBanner : UserControl
    {
        public StatusBanner()
        {
            InitializeComponent();
        }

        public void SetStatus(string text, StatusKind kind)
        {
            this.labelStatus.Text = "● " + text;
            this.labelStatus.ForeColor = ColorFor(kind);
        }

        public void ShowBanner(string text, StatusKind kind)
        {
            this.labelBanner.Text = text;
            this.labelBanner.ForeColor = Color.White;
            this.labelBanner.BackColor = ColorFor(kind);
            this.labelBanner.Visible = true;
        }

        public void HideBanner()
        {
            this.labelBanner.Visible = false;
        }

        public static Color ColorFor(StatusKind kind) => kind switch
        {
            StatusKind.Busy => Color.FromArgb(26, 134, 255),
            StatusKind.Success => Color.FromArgb(31, 138, 76),
            StatusKind.Warning => Color.FromArgb(214, 122, 0),
            StatusKind.Error => Color.FromArgb(192, 57, 43),
            _ => Color.FromArgb(106, 118, 134)
        };
    }
}
