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
            // The real session-end cleanup lives HERE, not in the page: Wisej.NET may dispose the page
            // before ApplicationExit fires, and a handler the page detaches in Dispose would then never
            // run. Subscribed once per session in Main, the handler lives exactly as long as the session.
            // No UI work — there may be no page left to push to; the steps go to the host console.
            // It must never throw: an exception here would escape into the session teardown.
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
