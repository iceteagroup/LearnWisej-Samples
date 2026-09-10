using System.Collections.Specialized;
using Wisej.Web;

namespace OperationsConsole
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
