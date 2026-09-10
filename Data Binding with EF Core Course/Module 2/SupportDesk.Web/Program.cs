using System.Collections.Specialized;
using Wisej.Web;

namespace SupportDesk.Web
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). Runs once per browser
        /// session; the Page it creates lives, on the server, for the whole session.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new TicketBrowserPage();
        }
    }
}
