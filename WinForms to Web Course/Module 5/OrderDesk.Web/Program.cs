using System.Collections.Specialized;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Wisej.NET session entry point (Default.json "startup": "OrderDesk.Program.Main, OrderDesk").
    /// This replaces the WinForms Program.Main with Application.EnableVisualStyles()/Application.Run(...):
    /// there is no process to run — the host (Startup.cs) owns the process, and this method
    /// runs once per browser session to show the first view.
    /// </summary>
    internal static class Program
    {
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
