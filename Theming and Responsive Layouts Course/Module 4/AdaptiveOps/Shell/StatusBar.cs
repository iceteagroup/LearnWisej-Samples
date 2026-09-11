using Wisej.Web;

namespace AdaptiveOps.Shell
{
    /// <summary>
    /// Bottom status region of the console: a status label and the browser width, docked inside
    /// the control. The shell uses <see cref="ShowStatus"/> and <see cref="WidthText"/>.
    /// </summary>
    public partial class StatusBar : UserControl
    {
        public StatusBar()
        {
            InitializeComponent();
        }

        /// <summary>Shows the outcome of the last action ("Ready", "Saved T-1042", …).</summary>
        public void ShowStatus(string text)
        {
            this.lblStatus.Text = text;
        }

        /// <summary>The browser-width label ("Width: 1366 px").</summary>
        public string WidthText
        {
            get => this.widthLabel.Text;
            set => this.widthLabel.Text = value;
        }
    }
}
