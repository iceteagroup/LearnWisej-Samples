using System.Collections.Specialized;
using AccessOps.Pages;
using Wisej.Web;

namespace AccessOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup"). Runs once per browser tab:
        /// the session ID lives in the tab's sessionStorage, so every tab is its own session.
        /// Nothing the user does is decided here yet; Module 1 is the shell and the threat model.
        /// </summary>
        static void Main(NameValueCollection args)
        {
            Application.MainPage = new LoginPage();
        }
    }
}
