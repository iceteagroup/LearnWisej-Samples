using System;
using System.Drawing;
using Wisej.Web;

namespace OperationsConsole
{
    /// <summary>
    /// Template main page: replace with the module's Operations Console shell.
    /// Shows the active responsive profile and a Toast so the template can be smoke-tested.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Application.ResponsiveProfileChanged += Application_ResponsiveProfileChanged;
            UpdateProfileLabel();
        }

        private void Application_ResponsiveProfileChanged(object sender, ResponsiveProfileChangedEventArgs e)
        {
            UpdateProfileLabel();
            AddLog("profile changed → " + Application.ActiveProfile.Name);
        }

        private void UpdateProfileLabel()
        {
            lblProfile.Text = "Profile: " + Application.ActiveProfile.Name + "  ·  browser " + Application.Browser.Size.Width + "×" + Application.Browser.Size.Height;
        }

        private void btnToast_Click(object sender, EventArgs e)
        {
            new Toast("Template is running.", "icon-info") { AutoCloseDelay = 3000, Alignment = ContentAlignment.TopRight }.Show();
            AddLog("toast shown");
        }

        private void btnAlert_Click(object sender, EventArgs e)
        {
            AlertBox.Show("AlertBox from the template.", MessageBoxIcon.Information, alignment: ContentAlignment.TopRight, autoCloseDelay: 3000);
            AddLog("alert shown");
        }

        private void AddLog(string message)
        {
            lstEventLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + message);
            lstEventLog.SelectedIndex = lstEventLog.Items.Count - 1;
        }
    }
}
