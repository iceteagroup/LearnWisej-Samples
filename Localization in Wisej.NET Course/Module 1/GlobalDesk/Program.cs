using System.Collections.Specialized;
using Wisej.Web;

namespace GlobalDesk
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// One session, one culture: everything about language lives on this session and never in
        /// a static field.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new DashboardPage();
        }
    }
}
