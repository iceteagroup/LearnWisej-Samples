using System;
using System.Windows.Forms;

namespace LegacyOrderDesk
{
    /// <summary>
    /// The classic WinForms entry point: one process, one user, one main form.
    /// Everything in here is replaced by the Wisej.NET startup files in Module 2.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new OrdersForm());
        }
    }
}
