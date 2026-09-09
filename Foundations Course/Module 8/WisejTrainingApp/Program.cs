using System.Collections.Specialized;
using Wisej.Web;

namespace WisejTrainingApp
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 8 uses a Page (fills the browser window) instead of a floating Form.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new StatusPage();
        }
    }
}
