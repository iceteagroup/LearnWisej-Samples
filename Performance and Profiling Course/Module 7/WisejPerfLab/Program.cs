using System.Collections.Specialized;
using Wisej.Web;

namespace WisejPerfLab
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). It runs once per browser
        /// session, and the page it creates lives on the server for as long as that session does — which
        /// is why a session, not a request, is the unit of capacity in this course.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
