using Wisej.Web;

namespace WisejTrainingApp.Views
{
    /// <summary>
    /// Settings page — the "UI settings" card shows the current theme (read from the shell, never from the
    /// service) and the "Shell" card lists what the visual layer owns. Same spacing rules as every other page.
    /// </summary>
    public partial class SettingsView : UserControl, IAppView
    {
        private readonly IAppShell shell;

        public SettingsView(IAppShell shell)
        {
            this.shell = shell;
            InitializeComponent();
        }

        public void RefreshView()
        {
            lblThemeValue.Text = "Current theme: " + shell.CurrentTheme;
        }
    }
}
