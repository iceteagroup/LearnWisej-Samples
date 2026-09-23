using System.Collections.Specialized;
using Wisej.Web;

namespace IconDesk
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// IconDesk is a Web Page Application: one Page fills the browser window.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new CommandPage();
        }
    }
}
