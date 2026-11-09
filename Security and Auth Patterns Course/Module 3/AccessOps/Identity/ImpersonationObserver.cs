#if DEBUG
using System;
using System.ComponentModel;
using System.Security.Principal;
using System.Threading;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// DEBUG ONLY. The lab's logging handlers for <c>Application.ImpersonationBegin</c> /
    /// <c>ImpersonationEnd</c> (static, process-wide events: attached once at start-up, never per
    /// session) plus <c>ThreadBegin</c> / <c>ThreadEnd</c> to see the order. They only observe:
    /// <c>Handled</c> stays false. Wisej raises the impersonation events only when Default.json has
    /// <c>"impersonate": true</c>; on .NET 10 Wisej does not impersonate by default, so the effective
    /// identity these lines log stays the process identity (docs/impersonate-setting-and-starttask.md).
    /// </summary>
    internal static class ImpersonationObserver
    {
        private static int _attached;

        /// <summary>Attaches the observers once per process.</summary>
        public static void AttachOnce(ILoggerFactory loggers)
        {
            if (Interlocked.Exchange(ref _attached, 1) == 1)
                return;

            var logger = loggers.CreateLogger("AccessOps.ImpersonationObserver");
            Application.ImpersonationBegin += (s, e) => Log(logger, "ImpersonationBegin", e);
            Application.ImpersonationEnd += (s, e) => Log(logger, "ImpersonationEnd", e);
            try
            {
                Application.ThreadBegin += (s, e) => { if (Impersonating()) Log(logger, "ThreadBegin", null); };
                Application.ThreadEnd += (s, e) => { if (Impersonating()) Log(logger, "ThreadEnd", null); };
            }
            catch (Exception ex)
            {
                logger.LogWarning("ThreadBegin/ThreadEnd could not be attached at start-up: {Error}", ex.GetType().Name);
            }
        }

        private static bool Impersonating()
        {
            try { return Application.Configuration?.Impersonate == true; }
            catch { return false; }
        }

        private static void Log(ILogger logger, string point, HandledEventArgs e)
        {
            string effective;
            try
            {
                using var current = WindowsIdentity.GetCurrent();
                effective = $"{current.Name} level={current.ImpersonationLevel}";
            }
            catch (Exception ex)
            {
                effective = "unavailable (" + ex.GetType().Name + ")";
            }

            string transport;
            try { transport = Application.IsWebSocket ? "WebSocket" : "HTTP"; }
            catch { transport = "no session"; }

            logger.LogInformation("{Point} thread={Thread} transport={Transport} handled={Handled} effective={Effective}",
                point, Environment.CurrentManagedThreadId, transport, e?.Handled.ToString() ?? "-", effective);
        }
    }
}
#endif
