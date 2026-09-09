using System.Collections.Specialized;
using Wisej.Web;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 8 uses a top-level Page: its [WebMethod] members are discovered
        /// automatically and exposed to the browser as App.MainPage.*.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
