using System.Collections.Generic;
using System.Security.Claims;
using Wisej.Web;

namespace AccessOps.Identity
{
    /// <summary>
    /// Establishes the Wisej.NET application principal (the session override of
    /// <c>Application.User</c>). Adapted from the course pack's <c>common/ApplicationPrincipal.cs</c>,
    /// with one change: the pack assigns the user and then rotates the session ID; the lesson and
    /// this sample rotate first, then assign, on a page load.
    /// </summary>
    internal static class ApplicationPrincipal
    {
        /// <summary>The authentication type of every principal AccessOps establishes.</summary>
        public const string AuthenticationType = "Portal";

        /// <summary>
        /// Invoke only after credential verification, account policy and grant lookup, and only
        /// during a page load (inside <c>Application.ApplicationRefresh</c>): on Wisej-4 4.1.7
        /// <c>RefreshSessionId()</c> called from a WebSocket click switches the server to a new ID
        /// the browser never adopts, and the next request starts a fresh session.
        /// </summary>
        internal static void Establish(
            string accountId, string tenantId, string grantId, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim("tenant", tenantId),
                new Claim("grant", grantId ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            Application.RefreshSessionId();   // new session ID first, on the load
            Application.User = new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationType));
        }
    }
}
