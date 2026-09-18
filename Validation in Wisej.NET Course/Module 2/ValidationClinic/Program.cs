using System.Collections.Specialized;
using Wisej.Web;

namespace ValidationClinic
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// ValidationClinic is a Web Page Application: MainPage fills the browser window.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
