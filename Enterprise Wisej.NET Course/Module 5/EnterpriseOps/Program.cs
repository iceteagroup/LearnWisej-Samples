using System.Collections.Specialized;
using Wisej.Web;

namespace EnterpriseOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup"). Shows the Enterprise Work Queue as
        /// the main page; the per-session state it reads (grid state, saved views) lives in
        /// <see cref="Services.SessionContext"/>, not in the page.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new UI.WorkQueuePage();
        }
    }
}
