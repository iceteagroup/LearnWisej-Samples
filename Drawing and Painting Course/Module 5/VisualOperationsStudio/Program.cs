using System.Collections.Specialized;
using Wisej.Web;

namespace VisualOperationsStudio
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// VisualOperationsStudio is a Web Page Application: VisualOperationsPage fills the browser window.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new VisualOperationsPage();
        }
    }
}
