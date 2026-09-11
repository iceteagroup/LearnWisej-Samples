using System;
using OrderDesk.Shell;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// The browser page: hosts the ported application shell (MenuBar, ToolBar, screen host,
    /// StatusBar) and opens it on the Orders screen.
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void MainPage_Load(object sender, EventArgs e)
        {
            shell.NavigateTo(AppShell.OrdersScreenName);
        }
    }
}
