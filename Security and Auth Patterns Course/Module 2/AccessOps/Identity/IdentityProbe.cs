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

        /// <summary>
        /// The masked Windows account Program.Main found on the page load ("CONTOSO\re•••1");
        /// the "masked account" the Module 2 lab records from this view.
        /// </summary>
        public static string MaskedAccount()
            => Try(() => (Application.MainPage as Pages.MainPage)?.SignedInUser?.MaskedAccount ?? "(none)");

        /// <summary>The AccessOps subject Program.Main mapped from the SID, redacted ("acct …7f3a · tenant A").</summary>
        public static string Subject()
            => Try(() =>
            {
                // before Program.Main created the page (its own trace line), the page-load principal is live
                var subject = (Application.MainPage as Pages.MainPage)?.SignedInUser?.Subject ?? Identity.Subject.From(Application.User);
                return subject.IsAnonymous
                    ? "none: no Windows account"
                    : $"{subject.Redacted} · tenant {subject.Tenant}";
            });

        /// <summary>
        /// Whether the <see cref="WindowsIdentity"/> that <c>Application.User</c> returns can
        /// still be used on this thread. Observed on Wisej-4 4.1.7 + Kestrel Negotiate: during a
        /// WebSocket event it is the identity captured on the page load and it is already
        /// disposed (Name and User throw ObjectDisposedException).
        /// </summary>
        public static string ApplicationUserState()
        {
            try
            {
                if (Application.User?.Identity is not WindowsIdentity windows)
                    return "not a Windows identity";
                _ = windows.User;   // throws when the token was disposed
                return "Windows identity, usable";
            }
            catch (ObjectDisposedException)
            {
                return "Windows identity, disposed (captured on the page load)";
            }
            catch (Exception ex)
            {
                return "unavailable (" + ex.GetType().Name + ")";
            }
        }

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
                    ? $"{identity.AuthenticationType} · {AccessOps.Security.WindowsName.Redact(identity.Name)}"
                    : "not authenticated (anonymous host)";
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
