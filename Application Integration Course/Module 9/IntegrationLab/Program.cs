using System.Collections.Specialized;
using Wisej.Web;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 9 uses a Page (a top-level container) so the pivot's DevExtreme-style
        /// CustomStore can call a [WebMethod] on it as App.MainPage.LoadPivotAsync(...).
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
