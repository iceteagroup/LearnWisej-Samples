using System.Collections.Specialized;
using Wisej.Web;

namespace WisejTrainingApp
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (configured in Default.json "startup").
        /// Module 3 builds the shell as a Page, so it fills the browser like a normal web app
        /// (no window chrome, no minimize button to lose the app behind).
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new MainPage();
        }
    }
}
