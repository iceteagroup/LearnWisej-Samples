#if DEBUG
using System;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// DEBUG ONLY. Reads the three identities of the current request without ever exposing a raw
    /// session ID, cookie or token: the session ID and account ID are cut to their last four
    /// characters. Used by <c>IdentityDiagnosticsView</c> and by <see cref="IdentityTrace"/>.
    /// Every read is guarded, because on a raw <c>Task.Run</c> there is no session to ask.
    /// </summary>
    internal static class IdentityProbe
    {
        /// <summary><c>Application.User.Identity.AuthenticationType</c>, or "(none)".</summary>
        public static string AuthenticationType()
            => Try(() => Application.User?.Identity?.AuthenticationType ?? "(none)");

        /// <summary><c>Application.IsAuthenticated</c>.</summary>
        public static string IsAuthenticated()
            => Try(() => Application.IsAuthenticated ? "True" : "False");

        /// <summary>The AccessOps subject, redacted ("acct …7f3a · tenant A").</summary>
        public static string Subject()
            => Try(() =>
            {
                var subject = Identity.Subject.From(Application.User);
                return subject.IsAnonymous
                    ? "none: host principal returned"
                    : $"{subject.Redacted} · tenant {subject.Tenant}";
            });

        /// <summary>
        /// The host request principal (ASP.NET Core <c>HttpContext.User</c>) as far as the
        /// current thread can see it through <see cref="IHttpContextAccessor"/>.
        /// </summary>
        public static string HostPrincipal()
            => Try(() =>
            {
                var context = AppHost.Get<IHttpContextAccessor>().HttpContext;
                if (context == null)
                    return "unavailable on this thread";
                var identity = context.User?.Identity;
                return identity?.IsAuthenticated == true
                    ? $"{identity.AuthenticationType} · …{Redact.Tail(identity.Name, 4)}"
                    : "not authenticated (Kestrel)";
            });

        /// <summary>The Windows identity of the process token, i.e. what the file system checks.</summary>
        public static string EffectiveOsIdentity()
            => Try(() => OperatingSystem.IsWindows()
                ? WindowsIdentity.GetCurrent().Name + " (dotnet process)"
                : Environment.UserName + " (dotnet process)");

        /// <summary>The session ID cut to its last four characters, or "no session".</summary>
        public static string SessionTail()
            => Try(() => string.IsNullOrEmpty(Application.SessionId) ? "no session" : "…" + Redact.Tail(Application.SessionId, 4));

        /// <summary>"HTTP load" or "WebSocket", from <c>Application.IsWebSocket</c>.</summary>
        public static string Transport()
            => Try(() => Application.IsWebSocket ? "WebSocket" : "HTTP load");

        private static string Try(Func<string> read)
        {
            try
            {
                return read();
            }
            catch (Exception ex)
            {
                return "unavailable (" + ex.GetType().Name + ")";
            }
        }
    }

    /// <summary>
    /// DEBUG ONLY. Writes one log line per lifecycle point (startup, refresh, click, background
    /// job) with the fields of the lab's identity matrix. Never a raw session ID or token.
    /// </summary>
    internal static class IdentityTrace
    {
        /// <summary>Logs the identities visible at <paramref name="point"/>.</summary>
        public static void Write(string point, Subject explicitSubject = null)
        {
            var logger = AppHost.Get<ILoggerFactory>().CreateLogger("AccessOps.IdentityTrace");
            logger.LogInformation(
                "trace point={Point} transport={Transport} session={Session} authType={AuthType} isAuthenticated={IsAuthenticated} appSubject={Subject} explicitSubject={Explicit} host={Host} os={Os}",
                point,
                IdentityProbe.Transport(),
                IdentityProbe.SessionTail(),
                IdentityProbe.AuthenticationType(),
                IdentityProbe.IsAuthenticated(),
                IdentityProbe.Subject(),
                explicitSubject == null ? "-" : $"{explicitSubject.Redacted} · tenant {explicitSubject.Tenant}",
                IdentityProbe.HostPrincipal(),
                IdentityProbe.EffectiveOsIdentity());
        }
    }
}
#endif
