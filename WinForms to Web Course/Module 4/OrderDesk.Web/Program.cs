using System.Collections.Specialized;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Wisej.NET session entry point (Default.json "startup": "OrderDesk.Program.Main, OrderDesk").
    /// Runs once per browser SESSION — not once per process like the WinForms Program.Main — which is
    /// why Module 4 wires the session cleanup hooks here: every session gets its own subscription
    /// and the handler closes over its own page, never over a static.
    /// </summary>
    internal static class Program
    {
        static void Main(NameValueCollection args)
        {
            var page = new MainPage();
            Application.MainPage = page;

            // Session end: browser tab closed, session expired or Application.Exit() → release per-user state.
            Application.ApplicationExit += (s, e) => SessionLifecycle.OnApplicationExit(page);

            // Session about to time out: trace it; e.Handled stays false so the built-in prolong dialog appears.
            Application.SessionTimeout += (s, e) => SessionLifecycle.OnSessionTimeout(page);
        }
    }
}
