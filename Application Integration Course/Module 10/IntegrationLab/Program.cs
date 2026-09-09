using System.Collections.Specialized;
using Wisej.Web;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 10 uses a Page as the main surface: the Operations Dashboard fills the browser window.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new OperationsPage();
        }
    }
}
