using System.Collections.Specialized;
using Wisej.Web;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 3 uses a Page (fills the browser) instead of a floating Form:
        /// the dashboard is the whole screen, exactly as the walkthrough shows it.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new DashboardPage();
        }
    }
}
