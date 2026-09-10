using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// The Command Center shell is the main page, which is why its [WebMethod]
        /// RunClientCommand is reachable from JavaScript as App.MainPage.RunClientCommand.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.CommandCenterShell();
        }
    }
}
