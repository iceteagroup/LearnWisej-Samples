using System.Collections.Specialized;
using System.Security.Principal;
using AccessOps.Identity;
using AccessOps.Pages;
using AccessOps.Security;
using Wisej.Web;

namespace AccessOps
{
    internal static class Program
    {
        /// <summary>
        /// Wisej.NET session entry point (Default.json "startup") of the intranet route. By the time
        /// it runs, the host has already answered the Windows challenge: a 401 never reaches this
        /// method. It reads the host principal through <c>Application.IsAuthenticated</c> and
        /// <c>Application.User</c>, requires a <see cref="WindowsIdentity"/>, and looks the
        /// AccessOps grant up from the account SID. The Windows account is evidence; the grant is
        /// the permission, and <c>DocumentService</c> denies a null grant.
        /// </summary>
        static void Main(NameValueCollection args)
        {
#if DEBUG
            LifecycleTrace.Write("Startup · Program.Main");
#endif
            // IIS (or Kestrel Negotiate) answered the challenge; Wisej captured HttpContext.User.
            if (!Application.IsAuthenticated ||
                Application.User?.Identity is not WindowsIdentity windows)
            {
                new HostFaultPage().Show();   // host layer, not an app deny
                return;
            }

            // The Windows account is evidence; the grant is the permission.
            var sid = windows.User?.Value;
            var grant = AccessGrants.FindForAccount(sid);
            var user = new IntranetUser(
                WindowsName.Redact(windows.Name),
                AccessGrants.SubjectForAccount(sid),
                grant);

            new MainPage(user).Show();   // DocumentService denies a null grant
        }
    }
}
