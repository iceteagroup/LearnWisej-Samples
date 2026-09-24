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
        ///
        /// The session's culture is already decided by the time this runs - from the browser's
        /// Accept-Language, or from ?lang= in the URL - because Default.json says culture "auto".
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new DashboardPage();
        }
    }
}
