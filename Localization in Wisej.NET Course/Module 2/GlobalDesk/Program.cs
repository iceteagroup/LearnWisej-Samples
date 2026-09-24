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
        /// Module 2 is about the customer editor, so the session opens on the page that hosts it.
        /// Module 4 puts the editor back on the dashboard, beside the language picker.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new CustomerEditorPage();
        }
    }
}
