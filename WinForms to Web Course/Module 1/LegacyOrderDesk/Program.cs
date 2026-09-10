using System;
using System.Windows.Forms;
using OrderDesk.Domain;

namespace LegacyOrderDesk
{
    /// <summary>
    /// The classic WinForms entry point. One process, one user: Application.Run owns the message
    /// loop until the last form closes. This is what the Wisej.NET startup (Program.Main +
    /// Default.json + Startup.cs) replaces in Module 2.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Desktop assumption #1: one static "current user" for the whole process.
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
                AppState.CurrentUser = login.User;
            }

            Application.Run(new OrdersForm());
        }
    }
}
