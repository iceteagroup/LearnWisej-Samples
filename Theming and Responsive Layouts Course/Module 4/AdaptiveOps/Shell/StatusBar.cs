using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>Kind of message shown in the status region: decides the colour of the ● indicator.</summary>
    public enum StatusKind { Normal, Warn, Error }

    /// <summary>
    /// Bottom status region of the console (Dock = Bottom in MainPage). Its local layout is three
    /// docked labels declared in StatusBar.Designer.cs; the shell only sees three members:
    /// <see cref="ShowStatus"/>, <see cref="BrowserText"/> and <see cref="ThemeText"/>.
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
        }

        /// <summary>Writes "● text" with the colour of the kind.</summary>
        public void ShowStatus(string text, StatusKind kind)
        {
            this.lblStatus.Text = "● " + text;

            // The three colours are still control properties here; Modules 2 and 3 move them into
            // theme colour tokens (success / warning / danger). Module 4 is about layout, not colour.
            this.lblStatus.ForeColor = kind switch
            {
                StatusKind.Error => System.Drawing.Color.FromArgb(180, 35, 24),
                StatusKind.Warn => System.Drawing.Color.FromArgb(181, 71, 8),
                _ => System.Drawing.Color.FromArgb(2, 122, 72),
            };
        }

        /// <summary>The browser-size label in the middle ("Browser 1348 × 680 px · Desktop").</summary>
        public string BrowserText
        {
            get => this.lblBrowser.Text;
            set => this.lblBrowser.Text = value;
        }

        /// <summary>The right-hand label ("theme: Bootstrap-4").</summary>
        public string ThemeText
        {
            get => this.lblTheme.Text;
            set => this.lblTheme.Text = value;
        }
    }
}
