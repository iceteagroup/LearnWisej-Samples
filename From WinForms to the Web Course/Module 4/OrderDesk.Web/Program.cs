using System;
using System.Collections.Specialized;
using OrderDesk.Services;
using Wisej.Web;

namespace OrderDesk
{
    /// <summary>
    /// Wisej.NET session entry point (Default.json → "startup"). Runs once per browser session.
    /// This replaces WinForms' Application.EnableVisualStyles() + Application.Run(new OrdersForm()).
    /// </summary>
    internal static class Program
    {
        static void Main(NameValueCollection args)
        {
            // Session-end cleanup (timeout expired, Application.Exit, browser gone). Subscribed here, once
            // per session, so it runs even after the page is disposed. No UI work: there may be no page left
            // to push to, so the steps go to the host console. It must never throw.
            Application.ApplicationExit += (s, e) =>
            {
                try
                {
                    foreach (var step in SessionCleanup.Run("ApplicationExit", Application.SessionId))
                        Console.WriteLine($"[OrderDesk] session {SessionCleanup.ShortId(Application.SessionId)} · {step}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[OrderDesk] session {SessionCleanup.ShortId(Application.SessionId)} · cleanup failed: {ex.GetType().Name}: {ex.Message}");
                }
            };

            Application.MainPage = new MainPage();
        }
    }
}
