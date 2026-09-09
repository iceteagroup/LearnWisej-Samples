using System.Drawing;
using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Base class for every screen the shell swaps into pnlContent. It gives each view the shell handle, one
    /// hook the shell calls when the screen opens (ActivateScreen), and the shared status-label colours, so the
    /// eight views look and behave like one app.
    /// </summary>
    public class HelpdeskView : UserControl
    {
        public static readonly Color CardBackground = Color.White;
        public static readonly Color PageBackground = Color.FromArgb(238, 242, 247);
        public static readonly Color MutedText = Color.FromArgb(90, 107, 125);
        public static readonly Color OkColor = Color.FromArgb(31, 157, 87);
        public static readonly Color WarnColor = Color.FromArgb(232, 161, 60);
        public static readonly Color ErrorColor = Color.FromArgb(224, 86, 59);

        /// <summary>The shell this view lives in (null only while the Designer instantiates the view).</summary>
        protected IHelpdeskShell Shell { get; }

        /// <summary>Parameterless constructor for the Designer only.</summary>
        public HelpdeskView()
        {
        }

        protected HelpdeskView(IHelpdeskShell shell)
        {
            Shell = shell;
        }

        /// <summary>Called by Window1.NavigateTo every time this screen becomes visible. Refresh what may have changed.</summary>
        public virtual void ActivateScreen()
        {
        }

        /// <summary>The one way a status label gets its text and colour — same look on every screen.</summary>
        protected static void ShowStatus(Label label, string text, StatusKind kind)
        {
            label.Text = "● " + text;
            label.ForeColor = kind switch
            {
                StatusKind.Error => ErrorColor,
                StatusKind.Warn => WarnColor,
                _ => OkColor,
            };
        }
    }
}
