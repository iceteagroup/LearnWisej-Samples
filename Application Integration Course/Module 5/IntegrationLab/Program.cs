using System.Collections.Specialized;
using Wisej.Web;

namespace IntegrationLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 5 uses a Page instead of a Form: the Operations Dashboard fills the browser.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new EnterprisePage();
        }
    }
}
